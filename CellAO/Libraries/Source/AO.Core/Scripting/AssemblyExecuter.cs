#region License
/*
Copyright (c) 2005-2012, CellAO Team

All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion

#region Usings...
using System;
using System.IO;
using System.Reflection;
#endregion

namespace AO.Core.Scripting
{
    /// <summary>
    /// Executes "public static void Main(..)" of assembly in a separate domain.
    /// </summary>
    internal class AssemblyExecutor
    {
        private AppDomain appDomain;
        private readonly RemoteExecutor remoteExecutor;
        private readonly string assemblyFileName;

        public AssemblyExecutor(string fileNname, string domainName)
        {
            assemblyFileName = fileNname;
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = Path.GetDirectoryName(assemblyFileName);
            setup.PrivateBinPath = AppDomain.CurrentDomain.BaseDirectory;
            setup.ApplicationName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
            setup.ShadowCopyFiles = "true";
            setup.ShadowCopyDirectories = Path.GetDirectoryName(assemblyFileName);
            appDomain = AppDomain.CreateDomain(domainName, null, setup);

            remoteExecutor =
                (RemoteExecutor)
                appDomain.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().Location,
                                                      typeof (RemoteExecutor).ToString());
        }

        public void Execute(string[] args)
        {
            remoteExecutor.ExecuteAssembly(assemblyFileName, args);
        }

        public void Unload()
        {
            AppDomain.Unload(appDomain);
            appDomain = null;
        }
    }

    /// <summary>
    /// Invokes static method 'Main' from the assembly.
    /// </summary>
    internal class RemoteExecutor : MarshalByRefObject
    {
        private string workingDir;

        /// <summary>
        /// AppDomain evant handler. This handler will be called if CLR cannot resolve 
        /// referenced local assemblies 
        /// </summary>
        public Assembly ResolveEventHandler(object sender, ResolveEventArgs args)
        {
            return AssemblyResolver.ResolveAssembly(args.Name, workingDir);
        }

        public void ExecuteAssembly(string filename, string[] args)
        {
            workingDir = Path.GetDirectoryName(filename);
            AppDomain.CurrentDomain.AssemblyResolve += ResolveEventHandler;
            Assembly assembly = Assembly.LoadFrom(filename);
            InvokeStaticMain(assembly, args);
        }

        private void InvokeStaticMain(Assembly compiledAssembly, string[] scriptArgs)
        {
            MethodInfo method = null;
            foreach (Module m in compiledAssembly.GetModules())
            {
                foreach (Type t in m.GetTypes())
                {
                    BindingFlags bf = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod |
                                      BindingFlags.Static;
                    foreach (MemberInfo mi in t.GetMembers(bf))
                    {
                        if (mi.Name == "Main")
                        {
                            method = t.GetMethod(mi.Name, bf);
                        }
                        if (method != null)
                            break;
                    }
                    if (method != null)
                        break;
                }
                if (method != null)
                    break;
            }
            if (method != null)
            {
                if (method.GetParameters().Length != 0)
                {
                    method.Invoke(new object(), new[] {(Object) scriptArgs});
                }
                else
                {
                    method.Invoke(new object(), null);
                }
            }
            else
            {
                throw new Exception(
                    "Cannot find entry point. Make sure script file contains methos: 'public static Main(...)'");
            }
        }
    }
}