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
using System.Reflection;
#endregion

namespace AO.Core.Scripting
{
    /// <summary>
    /// PrintDelegate
    /// </summary>
    /// <param name="msg">String to print</param>
    public delegate void PrintDelegate(string msg);

    /// <summary>
    /// Wrapper class that runs CSExecutor within console application context.
    /// </summary>
    public class AOScriptExecuter
    {
        //Keep For Later Investigations! - Ashly

        //private Dictionary<string, string> ScriptList;
        ///// <summary>
        ///// This will get the Script Listing and Convert it to a Dictonary for later use.
        ///// </summary>
        //public void GetScriptList()
        //{
        //    DirectoryInfo di = new DirectoryInfo("Scripts/");
        //    FileInfo[] aoscripts = di.GetFiles("*.cs");

        //    foreach (FileInfo fi in aoscripts)
        //    {
        //        ScriptList.Add("ScriptName", fi.Name);
        //    }
        //}
        //public void InitalizeScripts()
        //{

        //}

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] args)
        {
            // AppInfo.appName = new FileInfo(Application.ExecutablePath).Name;

            CSExecutor exec = new CSExecutor();
            exec.Execute(args, Print);
        }

        /// <summary>
        /// Implementation of displaying application messages.
        /// </summary>
        private static void Print(string msg)
        {
            Console.WriteLine(msg);
        }
    }

    /// <summary>
    /// Repository for application specific data
    /// </summary>
    internal class AppInfo
    {
        public static string appName = "cscscript";
        public static bool appConsole = true;

        public static string appLogo
        {
            get
            {
                return "C# Script execution engine. Version " + Assembly.GetExecutingAssembly().GetName().Version +
                       ".\nCopyright (C) 2004-2005 Oleg Shilo.\n";
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