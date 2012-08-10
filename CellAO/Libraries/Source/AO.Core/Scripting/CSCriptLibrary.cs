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
using System.Diagnostics;
using System.Reflection;
#endregion

namespace AO.Core.Scripting
{
    //public delegate void PrintDelegate(string msg);
    /// <summary>
    /// Class CSScript which is implements class library for CSExecutor.
    /// </summary>
    public class CSScript
    {
        /// <summary>
        /// Main creator
        /// </summary>
        public CSScript()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            AppInfo.appName = fvi.FileName;
            Rethrow = false;
        }

        /// <summary>
        /// Force caught exceptions to be rethrown.
        /// </summary>
        public static bool Rethrow { get; set; }

        /// <summary>
        /// Invokes CSExecutor (C# script engine)
        /// </summary>
        public static void Execute(PrintDelegate print, string[] args)
        {
            // AppInfo.appName = exename;
            CSExecutor exec = new CSExecutor();

            exec.Rethrow = Rethrow;
            exec.Execute(args, print != null ? print : DefaultPrint);
        }

        /// <summary>
        /// Invokes CSExecutor (C# script engine)
        /// </summary>
        public void Execute(PrintDelegate print, string[] args, bool rethrow)
        {
            // AppInfo.appName = new FileInfo(Application.ExecutablePath).Name;
            CSExecutor exec = new CSExecutor();
            exec.Rethrow = rethrow;
            exec.Execute(args, print != null ? print : DefaultPrint);
        }

        /// <summary>
        /// Compiles script into assembly with CSExecutor
        /// </summary>
        /// <param name="scriptFile">The name of script file to be compiled.</param>
        /// <param name="assemblyFile">The name of compiled assembly. If set to null a temnporary file name will be used.</param>
        /// <param name="debugBuild">true if debug information should be included in assembly; otherwise, false.</param>
        /// <returns></returns>
        public static string Compile(string scriptFile, string assemblyFile, bool debugBuild)
        {
            CSExecutor exec = new CSExecutor();
            exec.Rethrow = true;
            return exec.Compile(scriptFile, assemblyFile, debugBuild);
        }

        /// <summary>
        /// Compiles script into assembly with CSExecutor and loads it in current AppDomain
        /// </summary>
        /// <param name="scriptFile">The name of script file to be compiled.</param>
        /// <param name="assemblyFile">The name of compiled assembly. If set to null a temnporary file name will be used.</param>
        /// <param name="debugBuild">true if debug information should be included in assembly; otherwise, false.</param>
        /// <returns></returns>
        public static Assembly Load(string scriptFile, string assemblyFile, bool debugBuild)
        {
            CSExecutor exec = new CSExecutor();
            exec.Rethrow = true;
            string outputFile = exec.Compile(scriptFile, assemblyFile, debugBuild);

            AssemblyName asmName = AssemblyName.GetAssemblyName(outputFile);
            return AppDomain.CurrentDomain.Load(asmName);
        }

        /// <summary>
        /// Default implementation of displaying application messages.
        /// </summary>
        private static void DefaultPrint(string msg)
        {
            //do nothing
        }
    }
}

namespace csscript
{
    internal delegate void PrintDelegate(string msg);

    /// <summary>
    /// Repository for application specific data
    /// </summary>
    internal class AppInfo
    {
        public static string appName = "CSScriptLibrary";
        public static bool appConsole;

        public static string appLogo
        {
            get
            {
                return "C# Script execution engine. Version " + Assembly.GetExecutingAssembly().GetName().Version +
                       ".\nCopyright (C) 2004 Oleg Shilo.\n";
            }
        }

        public static string appLogoShort
        {
            get { return "C# Script execution engine. Version " + Assembly.GetExecutingAssembly().GetName().Version + ".\n"; }
        }

        public static string appParams = "[/nl]:";
        public static string appParamsHelp = "nl	-	No logo mode: No banner will be shown at execution time.\n";
    }
}