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
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.CSharp;
#endregion

namespace AO.Core.Scripting
{
    /// <summary>
    /// CSExecutor is an class that implements execution of *.cs files. TODO: Rewrite this for .Net 4.0
    /// </summary>
    internal class CSExecutor
    {
        #region Public interface...
        /// <summary>
        /// Force caught exceptions to be rethrown.
        /// </summary>
        public bool Rethrow
        {
            get { return rethrow; }
            set { rethrow = value; }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public void Execute(string[] args, PrintDelegate printDelg)
        {
            print = printDelg;
            if (args.Length > 0)
            {
                try
                {
                    #region Parse command-line arguments...
                    //here we need to separeate application arguments from script ones
                    //script engine arguments are followed by script arguments
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (args[i].StartsWith("/"))
                        {
                            if (args[i] == "/nl")
                            {
                                options["noLogo"] = true;
                            }
                            else if (args[i] == "/c")
                            {
                                options["useCompiled"] = true;
                            }
                            else if (args[i].StartsWith("/ca"))
                            {
                                options["useCompiled"] = true;
                                options["supressExecution"] = true;
                            }
                            else if (args[i].StartsWith("/cd"))
                            {
                                options["useCompiled"] = true;
                                options["supressExecution"] = true;
                                options["DLLExtension"] = true;
                            }
                            else if (args[i].StartsWith("/dbg"))
                            {
                                options["DBG"] = true;
                            }
                            else if (args[i].StartsWith("/r:"))
                            {
                                string[] assemblies = args[i].Remove(0, 3).Split(":".ToCharArray());
                                options["refAssemblies"] = assemblies;
                            }
                            else if (args[i].StartsWith("/e"))
                            {
                                options["buildExecutable"] = true;
                                options["supressExecution"] = true;
                                options["buildWinExecutable"] = args[i].StartsWith("/ew");
                            }
                            else if (args[0] == "/?" || args[0] == "-?")
                            {
                                ShowHelp();
                                options["processFile"] = false;
                                break;
                            }
                            else if (args[0] == "/s")
                            {
                                ShowSample();
                                options["processFile"] = false;
                                break;
                            }
                        }
                        else
                        {
                            //this is the end of application arguments
                            options["scriptFileName"] = args[i];

                            //prepare script arguments array
                            scriptArgs = new string[args.Length - (1 + i)];
                            Array.Copy(args, (1 + i), scriptArgs, 0, args.Length - (1 + i));
                            break;
                        }
                    }
                    #endregion

                    if (options.GetBool("processFile"))
                    {
                        options["scriptFileName"] = FileParser.ResolveFile((string) options["scriptFileName"], null);

                        if (!options.GetBool("noLogo"))
                        {
                            Console.WriteLine(AppInfo.appLogo);
                        }

                        //compile
                        string assemblyFileName = GetAvailableAssembly((string) options["scriptFileName"]);
                        if (!options.GetBool("buildExecutable") || !options.GetBool("useCompiled") ||
                            (options.GetBool("useCompiled") && assemblyFileName == null))
                        {
                            try
                            {
                                assemblyFileName = Compile((string) options["scriptFileName"]);
                            }
                            catch (Exception e)
                            {
                                print("Error: Specified file could not be compiled.\n");
                                throw e;
                            }
                        }

                        //execute
                        if (!options.GetBool("supressExecution"))
                        {
                            try
                            {
                                ExecuteAssembly(assemblyFileName);
                            }
                            catch (Exception e)
                            {
                                print("Error: Specified file could not be executed.\n");
                                throw e;
                            }

                            //cleanup
                            if (File.Exists(assemblyFileName) && !options.GetBool("useCompiled"))
                            {
                                try
                                {
                                    File.Delete(assemblyFileName);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    if (rethrow)
                    {
                        throw e;
                    }
                    else
                    {
                        print("Exception: " + e);
                    }
                }
            }
            else
            {
                ShowHelp();
            }
        }


        /// <summary>
        /// Compiles C# script file into assembly.
        /// </summary>
        public string Compile(string scriptFile, string assemblyFile, bool debugBuild)
        {
            if (assemblyFile != null)
                options["forceOutputAssembly"] = assemblyFile;
            else
                options["forceOutputAssembly"] = Path.GetTempFileName();
            if (debugBuild)
                options["DBG"] = true;
            return Compile(scriptFile);
        }
        #endregion

        #region Class nested declarations...
        /// <summary>
        /// Application specific version of Hashtable
        /// </summary>
        private class ExecuteOptions : Hashtable
        {
            public ExecuteOptions()
            {
                this["processFile"] = true;
                this["scriptFileName"] = "";
            }

            public bool IsSet(string name)
            {
                return this[name] != null;
            }

            public bool GetBool(string name)
            {
                return this[name] != null ? (bool) this[name] : false; //default is false
            }
        }
        #endregion

        #region Class data...
        /// <summary>
        /// C# Script arguments array (sub array of application arguments array).
        /// </summary>
        private string[] scriptArgs;

        /// <summary>
        /// Callback to print application messages to appropriate output.
        /// </summary>
        private static PrintDelegate print;

        /// <summary>
        /// Container for paresed command line parguments
        /// </summary>
        private readonly ExecuteOptions options;

        /// <summary>
        /// Flag to force to rethrow critical exceptions
        /// </summary>
        private bool rethrow;
        #endregion

        #region Class methods...
        /// <summary>
        /// Constructor
        /// </summary>
        public CSExecutor()
        {
            rethrow = false;
            options = new ExecuteOptions();
        }

        /// <summary>
        /// Checks/returns if compiled C# script file (ScriptName + "c") available and valid.
        /// </summary>
        private string GetAvailableAssembly(string scripFileName)
        {
            string retval = null;
            string asmFileName = scripFileName + "c";
            if (File.Exists(asmFileName) && File.Exists(scripFileName))
            {
                FileInfo scriptFile = new FileInfo(scripFileName);
                FileInfo asmFile = new FileInfo(asmFileName);
                if (asmFile.LastWriteTime == scriptFile.LastWriteTime &&
                    asmFile.LastWriteTimeUtc == scriptFile.LastWriteTimeUtc)
                {
                    retval = asmFileName;
                }
            }
            return retval;
        }

        /// <summary>
        /// Compiles C# script file.
        /// </summary>
        private string Compile(string scriptFileName)
        {
            bool generateExe = options.GetBool("buildExecutable");
            bool localAssembliesUsed = false;
            string scriptDir = Path.GetDirectoryName(scriptFileName);
            string assemblyFileName = "";

            //parse source file in order to find all referenced assemblies
            //ASSUMPTION: assembly name is the same as namespace + ".dll"
            //if script doesn't follow this assumption user will need to 
            //specify assemblies explicitly  
            ScriptParser parser = new ScriptParser(scriptFileName);

            if (parser.apartmentState != ApartmentState.Unknown)
                Thread.CurrentThread.SetApartmentState(parser.apartmentState);

            //ICodeCompiler compiler = (new CSharpCodeProvider()).CreateCompiler();
            CodeDomProvider c = new CSharpCodeProvider();

            CompilerParameters compileParams = new CompilerParameters();


            compileParams.IncludeDebugInformation = options.GetBool("DBG");
            compileParams.GenerateExecutable = generateExe;
            compileParams.GenerateInMemory = !generateExe;

            //some assemblies were referenced from command line
            if (options.IsSet("refAssemblies"))
            {
                foreach (string asmName in (string[]) options["refAssemblies"])
                {
                    string asmLocation = Path.Combine(scriptDir, asmName);
                    if (File.Exists(asmLocation))
                    {
                        compileParams.ReferencedAssemblies.Add(asmLocation);
                        localAssembliesUsed = true;
                    }
                }
            }


            AssemblyResolver.ignoreFileName = Path.GetFileNameWithoutExtension(scriptFileName) + ".dll";

            foreach (string nmSpace in parser.ReferencedNamespaces)
            {
                //find local and global assemblies assuming assembly name is the same as a namespace
                foreach (string asmLocation in AssemblyResolver.FindAssembly(nmSpace, scriptDir))
                {
                    compileParams.ReferencedAssemblies.Add(asmLocation);

                    if (!localAssembliesUsed)
                        localAssembliesUsed = (Path.GetDirectoryName(asmLocation) == scriptDir);
                }
            }

            foreach (string asmName in parser.ReferencedAssemblies) //some assemblies were referenced from code
            {
                string asmLocation = Path.Combine(scriptDir, asmName);
                if (File.Exists(asmLocation))
                {
                    compileParams.ReferencedAssemblies.Add(asmLocation);
                    localAssembliesUsed = true;
                }
            }

            if (options.IsSet("forceOutputAssembly"))
            {
                assemblyFileName = (string) options["forceOutputAssembly"];
            }
            else
            {
                if (generateExe)
                    assemblyFileName = Path.Combine(scriptDir, Path.GetFileNameWithoutExtension(scriptFileName) + ".exe");
                else if (options.GetBool("useCompiled") || localAssembliesUsed)
                    if (options.GetBool("DLLExtension"))
                        assemblyFileName = Path.Combine(scriptDir,
                                                        Path.GetFileNameWithoutExtension(scriptFileName) + ".dll");
                    else
                        assemblyFileName = Path.Combine(scriptDir, scriptFileName + "c");
                else
                    assemblyFileName = Path.GetTempFileName();
            }

            compileParams.OutputAssembly = assemblyFileName;

            if (generateExe && options.GetBool("buildWinExecutable"))
                compileParams.CompilerOptions = "/target:winexe";

            if (File.Exists(assemblyFileName))
                File.Delete(assemblyFileName);

            CompilerResults results = c.CompileAssemblyFromFile(compileParams, parser.FilesToCompile);
            if (results.Errors.Count != 0)
            {
                StringBuilder compileErr = new StringBuilder();
                foreach (CompilerError err in results.Errors)
                {
                    compileErr.Append(err.ToString());
                }
                throw new Exception(compileErr.ToString());
            }
            else
            {
                parser.DeleteImportedFiles();

                if (!options.GetBool("DBG")) //pdb file might be needed for a debugger
                {
                    string pdbFile = Path.Combine(Path.GetDirectoryName(assemblyFileName),
                                                  Path.GetFileNameWithoutExtension(assemblyFileName) + ".pdb");
                    if (File.Exists(pdbFile))
                        File.Delete(pdbFile);
                }

                FileInfo scriptFile = new FileInfo(scriptFileName);
                FileInfo asmFile = new FileInfo(assemblyFileName);
                if (scriptFile != null && asmFile != null)
                {
                    asmFile.LastWriteTime = scriptFile.LastWriteTime;
                    asmFile.LastWriteTimeUtc = scriptFile.LastWriteTimeUtc;
                }
            }
            return assemblyFileName;
        }

        /// <summary>
        /// Executes compiled C# script file.
        /// Invokes static method 'Main' from the assembly.
        /// </summary>
        private void ExecuteAssembly(string assemblyFile)
        {
            //execute assembly in a different domain to make it possible to unload assembly before clean up
            AssemblyExecutor executor = new AssemblyExecutor(assemblyFile, "AsmExecution");
            executor.Execute(scriptArgs);
        }

        /// <summary>
        /// Prints Help info.
        /// </summary>
        private void ShowHelp()
        {
            StringBuilder msgBuilder = new StringBuilder();
            msgBuilder.Append(AppInfo.appLogo);
            msgBuilder.Append("\nUsage: " + AppInfo.appName + " <switch 1> <switch 2> <file> [params]\n");
            msgBuilder.Append("\n");
            msgBuilder.Append("<switch 1>\n");
            if (AppInfo.appParamsHelp != "")
                msgBuilder.Append(" /" + AppInfo.appParamsHelp); //application specific usage info
            msgBuilder.Append(" /?	-	Display help info.\n");
            msgBuilder.Append(" /e	-	Compiles script into console application executable.\n");
            msgBuilder.Append(" /ew	-	Compiles script into Windows application executable.\n");
            msgBuilder.Append(" /c	-	Use compiled file (.csc) if found (to improve performance).\n");
            msgBuilder.Append(" /ca	-	Compiles script file into assembly (.csc).\n");
            msgBuilder.Append(" /cd	-	Compiles script file into assembly (.dll).\n");
            msgBuilder.Append(" /dbg	-	Forces compiler to include debug information.\n");
            msgBuilder.Append(" /s	-	Prints content of sample script file.\n");
            msgBuilder.Append("			(Example: " + AppInfo.appName + " /s > sample.cs)\n");
            msgBuilder.Append("\n");
            msgBuilder.Append("<switch 2>\n");
            msgBuilder.Append(" /r:<assembly 1>:<assembly N>\n");
            msgBuilder.Append("	-	Use explicitly referenced assembly. It is required only for\n");
            msgBuilder.Append("		rare cases when namespace cannot be resolved into assembly.\n");
            msgBuilder.Append("			(Example: " + AppInfo.appName + " /r:myLib.dll myScript.cs).\n");
            msgBuilder.Append("\n");
            msgBuilder.Append("file	-	Specifies name of a script file to be run.\n");
            msgBuilder.Append("\n");
            msgBuilder.Append("params	-	Specifies optional parameters for a script file to be run.\n");
            msgBuilder.Append("\n");
            //if (AppInfo.appConsole) // a temporary hack to prevent showing a huge message box when not in console mode
            if (AppInfo.appConsole)
            {
                msgBuilder.Append("\n");
                msgBuilder.Append("**************************************\n");
                msgBuilder.Append("Script specific syntax\n");
                msgBuilder.Append("**************************************\n");
                msgBuilder.Append("\n");
                msgBuilder.Append("Engine directives:\n");
                msgBuilder.Append("\n");
                msgBuilder.Append("   //css_import <file>[, rename_namespace(<oldName>, <newName>)];\n");
                msgBuilder.Append("\n");
                msgBuilder.Append("file	- name of a script file to be imported at compile-time.\n");
                msgBuilder.Append("oldName - name of a namespace to be renamed during importing\n");
                msgBuilder.Append("newName - new name of a namespace to be renamed during importing\n");
                msgBuilder.Append("\n");
                msgBuilder.Append("   //css_reference <file>;\n");
                msgBuilder.Append("\n");
                msgBuilder.Append(
                    "file	- name of an assembly file to be loaded at run-time. The assembly must be in the same folder as script file.\n");
                msgBuilder.Append("------------------------------------\n");
                msgBuilder.Append(
                    "css_import directive is used to inject one script into another at compile time. Thus code from one script can be exercised in another one.\n");
                msgBuilder.Append("\n");
                msgBuilder.Append(
                    "css_reference directive is used to reference a local assembliy required at run time.\n");
                msgBuilder.Append("\n");
                msgBuilder.Append(
                    "Any directive has to be written as a single line in order to have no impact when the script ");
                msgBuilder.Append("is compiled by CLI complient compiler.\n");
                msgBuilder.Append("\n");
                msgBuilder.Append("It also must not be placed after any namespace or class declaration.\n");
                msgBuilder.Append("\n");
                msgBuilder.Append("'Rename' clause can appear in the directive multiple tims.\n");
                msgBuilder.Append("------------------------------------\n");
                msgBuilder.Append("Example:\n");
                msgBuilder.Append("\n");
                msgBuilder.Append(" using System;\n");
                msgBuilder.Append(" //css_import tick, rename_namespace(CSScript, TickScript);\n");
                msgBuilder.Append(" //css_reference teechart.lite.dll;\n");
                msgBuilder.Append(" \n");
                msgBuilder.Append(" namespace CSScript\n");
                msgBuilder.Append(" {\n");
                msgBuilder.Append("   class TickImporter\n");
                msgBuilder.Append("   {\n");
                msgBuilder.Append("     static public void Main(string[] args)\n");
                msgBuilder.Append("     {\n");
                msgBuilder.Append("       TickScript.Ticker.i_Main(args);\n");
                msgBuilder.Append("     }\n");
                msgBuilder.Append("   }\n");
                msgBuilder.Append(" }\n");
                msgBuilder.Append("\n");
            }

            print(msgBuilder.ToString());
        }

        /// <summary>
        /// Show sample C# scrip file.
        /// </summary>
        private void ShowSample()
        {
            StringBuilder msgBuilder = new StringBuilder();
            msgBuilder.Append("using System;\r\n");
            msgBuilder.Append("using System.Windows.Forms;\r\n");
            msgBuilder.Append("\r\n");
            msgBuilder.Append("class Script\r\n");
            msgBuilder.Append("{\r\n");
            msgBuilder.Append("	static public void Main(string[] args)\r\n");
            msgBuilder.Append("	{\r\n");
            msgBuilder.Append("		MessageBox.Show(\"Just a test!\");\r\n");
            msgBuilder.Append("\r\n");
            msgBuilder.Append("		for (int i = 0; i < args.Length; i++)\r\n");
            msgBuilder.Append("		{\r\n");
            msgBuilder.Append("			Console.WriteLine(args[i]);\r\n");
            msgBuilder.Append("		}\r\n");
            msgBuilder.Append("	}\r\n");
            msgBuilder.Append("}\r\n");
            print(msgBuilder.ToString());
        }
        #endregion
    }
}