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
using System.Collections;
using System.IO;
using System.Reflection;
#endregion

namespace AO.Core.Scripting
{
    /// <summary>
    /// Class for resolving assembly name to assembly file
    /// </summary>
    public class AssemblyResolver
    {
        #region Class public data...
        /// <summary>
        /// File to be excluded from assembly search
        /// </summary>
        public static string ignoreFileName = "";
        #endregion

        #region Class public methods...
        /// <summary>
        /// Resolves assembly name to assembly file
        /// </summary>
        /// <param name="assemblyName">The name of assembly</param>
        /// <param name="workingDir">The name of directory where local assemblies are expected to be</param>
        /// <returns></returns>
        public static Assembly ResolveAssembly(string assemblyName, string workingDir)
        {
            //try file with name AssemblyDisplayName + .dll 
            string[] asmFileNameTokens = assemblyName.Split(", ".ToCharArray(), 5);

            string asmFileName = Path.Combine(workingDir, asmFileNameTokens[0]) + ".dll";
            if (ignoreFileName != Path.GetFileName(asmFileName) && File.Exists(asmFileName))
            {
                try
                {
                    AssemblyName asmName = AssemblyName.GetAssemblyName(asmFileName);
                    if (asmName != null && asmName.FullName == assemblyName)
                    {
                        return Assembly.LoadFrom(asmFileName);
                    }
                }
                catch
                {
                }
            }

            //try all dll files (in script folder) which contain namespace as a part of file name
            ArrayList asm_ALIKE_Files =
                new ArrayList(Directory.GetFileSystemEntries(workingDir,
                                                             string.Format("*{0}*.dll", asmFileNameTokens[0])));
            foreach (string asmFile in asm_ALIKE_Files)
            {
                try
                {
                    if (ignoreFileName != Path.GetFileName(asmFile))
                    {
                        AssemblyName asmName = AssemblyName.GetAssemblyName(asmFile);
                        if (asmName != null && asmName.FullName == assemblyName)
                        {
                            return Assembly.LoadFrom(asmFile);
                        }
                    }
                }
                catch
                {
                }
            }

            //try all the rest of dll files in script folder
            string[] asmFiles = Directory.GetFileSystemEntries(workingDir, "*.dll");
            foreach (string asmFile in asmFiles)
            {
                if (asm_ALIKE_Files.Contains(asmFile))
                    continue;
                try
                {
                    if (ignoreFileName != Path.GetFileName(asmFile))
                    {
                        AssemblyName asmName = AssemblyName.GetAssemblyName(asmFile);
                        if (asmName != null && asmName.FullName == assemblyName)
                        {
                            return Assembly.LoadFrom(asmFile);
                        }
                    }
                }
                catch
                {
                }
            }
            return null;
        }

        /// <summary>
        /// Resolves namespace into array of assembly locations (local and GAC ones).
        /// </summary>
        public static string[] FindAssembly(string nmSpace, string workingDir)
        {
            ArrayList retval = new ArrayList();
            string[] asmLocations = FindLocalAssembly(nmSpace, workingDir);

            if (asmLocations.Length != 0)
            {
                foreach (string asmLocation in asmLocations) //local assemblies
                {
                    retval.Add(asmLocation);
                }
            }
            else
            {
                string[] asmGACLocations = FindGlobalAssembly(nmSpace); //global assemblies
                foreach (string asmGACLocation in asmGACLocations)
                {
                    retval.Add(asmGACLocation);
                }
            }
            return (string[]) retval.ToArray(typeof (string));
        }

        /// <summary>
        /// Resolves namespace into array of local assembly locations.
        /// (Currently it returns only one assembly location but in future 
        /// it can be extended to collect all assemblies with the same namespace)
        /// </summary>
        public static string[] FindLocalAssembly(string refNamespace, string workingDir)
        {
            ArrayList retval = new ArrayList();

            //try to predict assembly file name on the base of namespace
            string asesemblyLocation = String.Format(Path.Combine(workingDir, refNamespace + ".dll"));

            if (ignoreFileName != Path.GetFileName(asesemblyLocation) && File.Exists(asesemblyLocation))
            {
                retval.Add(asesemblyLocation);
                return (string[]) retval.ToArray(typeof (string));
            }

            //try all dll files (in script folder) which contain namespace as a part of file name
            string tmp = string.Format("*{0}*.dll", refNamespace);
            ArrayList asm_ALIKE_Files =
                new ArrayList(Directory.GetFileSystemEntries(workingDir, string.Format("*{0}*.dll", refNamespace)));
            foreach (string asmFile in asm_ALIKE_Files)
            {
                if (ignoreFileName != Path.GetFileName(asmFile) && IsNamespaceDefinedInAssembly(asmFile, refNamespace))
                {
                    retval.Add(asmFile);
                    return (string[]) retval.ToArray(typeof (string));
                }
            }

            //try all the rest of dll files in script folder
            string[] asmFiles = Directory.GetFileSystemEntries(workingDir, "*.dll");
            foreach (string asmFile in asmFiles)
            {
                if (asm_ALIKE_Files.Contains(asmFile))
                    continue;

                if (ignoreFileName != Path.GetFileName(asmFile) && IsNamespaceDefinedInAssembly(asmFile, refNamespace))
                {
                    retval.Add(asmFile);
                    return (string[]) retval.ToArray(typeof (string));
                }
            }
            return (string[]) retval.ToArray(typeof (string));
        }

        /// <summary>
        /// Resolves namespace into array of global assembly (GAC) locations.
        /// </summary>
        public static string[] FindGlobalAssembly(String namespaceStr)
        {
            ArrayList retval = new ArrayList();
            AssemblyEnum asmEnum = new AssemblyEnum(namespaceStr);

            String asmName;
            while ((asmName = asmEnum.GetNextAssembly()) != null)
            {
                string asmLocation = AssemblyCache.QueryAssemblyInfo(asmName);
                retval.Add(asmLocation);
            }
            return (string[]) retval.ToArray(typeof (string));
        }
        #endregion

        /// <summary>
        /// Search for namespace into local assembly file.
        /// </summary>
        private static bool IsNamespaceDefinedInAssembly(string asmFileName, string namespaceStr)
        {
            if (File.Exists(asmFileName))
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(asmFileName);
                    if (assembly != null)
                    {
                        foreach (Module m in assembly.GetModules())
                        {
                            foreach (Type t in m.GetTypes())
                            {
                                if (namespaceStr == t.Namespace)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            return false;
        }
    }
}