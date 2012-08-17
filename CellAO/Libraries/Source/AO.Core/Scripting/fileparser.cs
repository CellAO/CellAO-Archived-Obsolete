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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
#endregion

namespace AO.Core.Scripting
{
    /// <summary>
    /// ParsingParams is an class that holds parsing parameters. 
    /// At this moment they are namespace renaming rules only.
    /// </summary>
    internal class ParsingParams
    {
        #region Public interface...
        public ParsingParams()
        {
            renameNamespaceMap = new ArrayList();
        }

        public string[][] RenameNamespaceMap
        {
            get { return (string[][]) renameNamespaceMap.ToArray(typeof (string[])); }
        }

        public void AddRenameNamespaceMap(string[] names)
        {
            renameNamespaceMap.Add(names);
        }

        /// <summary>
        /// Compare() is to be used to help with implementation of IComparer for sorting operations.
        /// </summary>
        public static int Compare(ParsingParams xPrams, ParsingParams yPrams)
        {
            if (xPrams == null && yPrams == null)
                return 0;

            int retval = xPrams == null ? -1 : (yPrams == null ? 1 : 0);

            if (retval == 0)
            {
                string[][] xNames = xPrams.RenameNamespaceMap;
                string[][] yNames = yPrams.RenameNamespaceMap;
                retval = Comparer.Default.Compare(xNames.Length, yNames.Length);
                if (retval == 0)
                {
                    for (int i = 0; i < xNames.Length && retval == 0; i++)
                    {
                        retval = Comparer.Default.Compare(xNames[i].Length, yNames[i].Length);
                        if (retval == 0)
                        {
                            for (int j = 0; j < xNames[i].Length; j++)
                            {
                                retval = Comparer.Default.Compare(xNames[i][j], yNames[i][j]);
                            }
                        }
                    }
                }
            }
            return retval;
        }
        #endregion

        private readonly ArrayList renameNamespaceMap;
    }

    /// <summary>
    /// ParserBase is a base class that implements some ReularExtressions functionality.
    /// </summary>
    internal class ParserBase
    {
        public delegate object ParseStatement(string statement);

        private const RegexOptions regexOptions = RegexOptions.Multiline |
                                                  RegexOptions.IgnorePatternWhitespace |
                                                  RegexOptions.Compiled;

        /// <summary>
        /// Returns array of matches; Optionally every match can be processed with delegate ParseStatement.
        /// </summary>
        public static ArrayList RegexGetMatches(string text, string patern, ParseStatement parser)
        {
            ArrayList retval = new ArrayList();
            Regex regex = new Regex(patern, regexOptions);
            Match m = regex.Match(text);
            string retvalStr = "";
            while (m.Success)
            {
                retvalStr = m.ToString();
                if (parser != null)
                    retval.Add(parser(retvalStr));
                else
                    retval.Add(retvalStr);
                m = m.NextMatch();
            }
            return retval;
        }

        /// <summary> 
        /// Returns array of matches for a statement (word(s) ended with ';'); Optionally every match can be processed with delegate ParseStatement. 
        /// </summary> 
        public static ArrayList GetStatements(string text, string patern, ParseStatement parser)
        {
            ArrayList retval = new ArrayList();
            Regex regex = new Regex(patern, regexOptions);
            Match m = regex.Match(text);

            string retvalStr = "";

            while (m.Success)
            {
                int startPos = m.Index;
                int endPos = text.IndexOf(";", m.Index);

                if (endPos != -1)
                {
                    retvalStr = text.Substring(startPos, endPos - startPos);
                    if (parser != null)
                        retval.Add(parser(retvalStr));
                    else
                        retval.Add(retvalStr);
                }
                m = m.NextMatch();
            }
            return retval;
        }

        /// <summary>
        /// Replaces all search matches; Optionally replacing can be done repeatedly until no replacement can be done.
        /// </summary>
        public static string RegexReplace(string text, string patern, string replacement, bool recurcive)
        {
            string retval = text;
            Regex regex = new Regex(patern, regexOptions);
            int oldLength = 0;
            do
            {
                oldLength = retval.Length;
                retval = regex.Replace(retval, replacement);
            } while (recurcive && oldLength != retval.Length);
            return retval;
        }

        /// <summary>
        /// Retruns position of the first match.
        /// </summary>
        public static int RegexFind(string text, string patern)
        {
            Regex regex = new Regex(patern, regexOptions);
            Match m = regex.Match(text);
            if (m.Success)
                return m.Index;
            else
                return -1;
        }

        /// <summary>
        /// Retruns position of the first match and populates out paramater with the matchins result. 
        /// </summary>
        public static int RegexFind(string text, string patern,ref string match)
        {
            Regex regex = new Regex(patern, regexOptions);
            Match m = regex.Match(text);
            if (m.Success)
            {
                match = m.ToString();
                return m.Index;
            }
            else
                return -1;
        }

        public static string headerTemplate =
            @"/*" + Environment.NewLine +
            @" Created by {0}" +
            @" Original location: {1}" + Environment.NewLine +
            @" C# source equivalent of {2}" + Environment.NewLine +
            @" compiler-generated file created {3} - DO NOT EDIT!" + Environment.NewLine +
            @"*/" + Environment.NewLine;

        public string ComposeHeader(string path)
        {
            return string.Format(headerTemplate, AppInfo.appLogoShort, path, fileName, DateTime.Now);
        }

        public string fileName = "";
        private int headerLinesTotal;

        public int HeaderLinesTotal
        {
            get
            {
                if (headerLinesTotal == 0)
                {
                    StringReader strReader = new StringReader(headerTemplate);
                    while (strReader.ReadLine() != null)
                    {
                        headerLinesTotal++;
                    }
                }
                return headerLinesTotal;
            }
        }
    }

    /// <summary>
    /// Class which is a placeholder for general infoirmation of the script file
    /// </summary>
    internal class ScriptInfo : ParserBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="statement">'Import' statement from the script file to be parsed</param>
        public ScriptInfo(string statement)
        {
            RegexFind(statement, @"\w*,?", ref fileName);
            fileName = fileName.Trim(" ,".ToCharArray());

            string rename = "";
            foreach (
                string match in RegexGetMatches(statement, @"rename_namespace\s*\(\s* \w+ \s* , \s* \w+ \s*\)", null))
            {
                if (match.Length > 0)
                {
                    rename = match.Replace("rename_namespace", "").Trim("()".ToCharArray());
                    if (parseParams == null)
                        parseParams = new ParsingParams();

                    parseParams.AddRenameNamespaceMap(rename.Split(",".ToCharArray()));
                }
            }
        }

        public ParsingParams parseParams;
    }

    /// <summary>
    /// Class that implements parsing the single C# Script file
    /// </summary>
    internal class FileParser : ParserBase
    {
        public ApartmentState apartmentState = ApartmentState.Unknown;

        public FileParser()
        {
        }

        public FileParser(string fileName, ParsingParams prams, bool process, bool imported, string[] searchDirs)
        {
            this.imported = imported;
            this.prams = prams;
            this.searchDirs = searchDirs;
            this.fileName = ResolveFile(fileName, searchDirs);
            if (process)
                ProcessFile();
        }


        public string fileNameImported = "";
        public ParsingParams prams;

        public string FileToCompile
        {
            get { return imported ? fileNameImported : fileName; }
        }

        public string[] SearchDirs
        {
            get { return searchDirs; }
        }

        public bool Imported
        {
            get { return imported; }
        }

        public string[] ReferencedNamespaces
        {
            get { return (string[]) referencedNamespaces.ToArray(typeof (string)); }
        }

        public string[] ReferencedAssemblies
        {
            get { return (string[]) referencedAssemblies.ToArray(typeof (string)); }
        }

        public ScriptInfo[] ReferencedScripts
        {
            get { return (ScriptInfo[]) referencedScripts.ToArray(typeof (ScriptInfo)); }
        }


        public void ProcessFile()
        {
            string codeStr = "";
            using (StreamReader sr = new StreamReader(fileName, Encoding.GetEncoding(0)))
            {
                codeStr = sr.ReadToEnd();
            }

            Parse(codeStr);

            if (imported)
            {
                fileNameImported = Path.Combine(Path.GetTempPath(),
                                                string.Format("i_{0}_{1}{2}", Path.GetFileNameWithoutExtension(fileName),
                                                              Path.GetDirectoryName(fileName).GetHashCode(),
                                                              Path.GetExtension(fileName)));
                if (File.Exists(fileNameImported))
                {
                    File.SetAttributes(fileNameImported, FileAttributes.Normal);
                    File.Delete(fileNameImported);
                }

                using (StreamWriter scriptWriter = new StreamWriter(fileNameImported, false, Encoding.GetEncoding(0)))
                {
                    //scriptWriter.Write(ComposeHeader(fileNameImported)); //using a big header at start is overkill but maight be required in future
                    scriptWriter.WriteLine(Import(codeStr));
                    scriptWriter.WriteLine("///////////////////////////////////////////");
                    scriptWriter.WriteLine("// Compiler-generated file - DO NOT EDIT!");
                    scriptWriter.WriteLine("///////////////////////////////////////////");
                }
                File.SetAttributes(fileNameImported, FileAttributes.ReadOnly);
            }
        }

        private void Parse(string codeStr)
        {
            referencedScripts.Clear();
            referencedNamespaces.Clear();

            string clearedCode = codeStr;
            int codeStartPos = 0;

            //remove comments of '/**/' style
            clearedCode = RegexReplace(clearedCode, @"/\* ([^/] | /[^\*])*?  \*/", "", true);

            codeStartPos = RegexFind(clearedCode, @"(namespace|class)\s");
            string declarationRegion = codeStartPos != -1 ? clearedCode.Substring(0, codeStartPos - 1) : clearedCode;

            //extract all referenced scripts, assemblies and namespaces
            referencedScripts.AddRange(GetStatements(declarationRegion,
                                                     @"(/[^/]|[^/]) //css_import\s*",
                                                     ParseImport));

            referencedAssemblies.AddRange(GetStatements(declarationRegion,
                                                        @"(/[^/]|[^/]) //css_reference\s*",
                                                        ParseAsmRefference));

            referencedNamespaces.AddRange(GetStatements(declarationRegion,
                                                        @"using\s*",
                                                        ParseUsing));

            string matchText = "";
            if (
                RegexFind(clearedCode.Substring(declarationRegion.Length - 1),
                          @"(\s+|\n)\[(\s*\w*\s*)\]\s*(\w*)\s*(static|static\s*static)(\s*\w*\s*)\s+ void\s*Main\s*\(",
                          ref matchText) != -1)
            {
                if (matchText.IndexOf("STAThread") != -1)
                    apartmentState = ApartmentState.STA;
                else if (matchText.IndexOf("MTAThread") != -1)
                    apartmentState = ApartmentState.MTA;
            }
        }

        //works nice but not with .NET v2.0
        private void ParseAgreciveRE(string codeStr)
        {
            referencedScripts.Clear();
            referencedNamespaces.Clear();

            string clearedCode = codeStr;
            int codeStartPos = 0;

            //remove comments of '/**/' style
            clearedCode = RegexReplace(clearedCode, @"/\* ([^/] | /[^\*])*?  \*/", "", true);

            codeStartPos = RegexFind(clearedCode, @"(namespace|class)\s");
            string declarationRegion = codeStartPos != -1 ? clearedCode.Substring(0, codeStartPos - 1) : clearedCode;

            //extract all referenced scripts, assemblies and namespaces
            referencedScripts.AddRange(RegexGetMatches(declarationRegion,
                                                       @"(/[^/]|[^/]) //css_import \s* (\w|\W)*? ;",
                                                       ParseImport));

            referencedAssemblies.AddRange(RegexGetMatches(declarationRegion,
                                                          @"(/[^/]|[^/]) //css_reference \s* (\w|\W)*? ;",
                                                          ParseAsmRefference));

            referencedNamespaces.AddRange(RegexGetMatches(declarationRegion,
                                                          @"using\s*(([^(]|[^\n])*)\w+;",
                                                          ParseUsing));
        }

        private string Import(string codeStr)
        {
            string importedCode = codeStr;
            if (imported)
            {
                //replace 'static void Main(...)' with 'void i_Main(...)'
                //importedCode = RegexReplace(importedCode, @"(static|static\s*public|public\s*|private|private\s*static)\s*void\s*Main\s*\(", string.Format("static public void {0} (", "i_Main"), false);
                importedCode = RegexReplace(importedCode, @"(static|static\s*static)(\s*\w*\s*)\s+void\s*Main\s*\(",
                                            string.Format("static public void {0} (", "i_Main"), false);
                importedCode = importedCode.TrimEnd("\n\r".ToCharArray());

                if (prams != null)
                {
                    foreach (string[] names in prams.RenameNamespaceMap)
                    {
                        importedCode = RegexReplace(importedCode, string.Format("namespace\\s*{0}\\s*", names[0]),
                                                    string.Format("namespace {0}{1}", names[1], Environment.NewLine),
                                                    false);
                    }
                }
            }
            return importedCode;
        }


        private readonly ArrayList referencedScripts = new ArrayList();
        private readonly ArrayList referencedNamespaces = new ArrayList();
        private readonly ArrayList referencedAssemblies = new ArrayList();

        private readonly string[] searchDirs;
        private readonly bool imported;

        /// <summary>
        /// Searches for script file by ginven script name. Search order:
        /// 1. Current directory
        /// 2. extraDirs (some arbitrary directories usually location of the imported scripts)
        /// 3. CSSCRIPT_DIR + \Lib
        /// 4. PATH
        /// Also fixes file name if user did not provide extension for script file (assuming .cs extension)
        /// </summary>
        public static string ResolveFile(string fileName, string[] extraDirs)
        {
            //current directory
            if (File.Exists(fileName))
            {
                return (new FileInfo(fileName).FullName);
            }
            else if (File.Exists(fileName + ".cs"))
            {
                return (new FileInfo(fileName + ".cs").FullName);
            }

            //arbitrary directories
            if (extraDirs != null)
            {
                foreach (string extraDir in extraDirs)
                {
                    string dir = extraDir;
                    if (File.Exists(fileName))
                    {
                        return (new FileInfo(Path.Combine(dir, fileName)).FullName);
                    }
                    else if (File.Exists(Path.Combine(dir, fileName) + ".cs"))
                    {
                        return (new FileInfo(Path.Combine(dir, fileName) + ".cs").FullName);
                    }
                }
            }

            //CSSCRIPT_DIR + \Lib
            string libDir = Environment.GetEnvironmentVariable("CSSCRIPT_DIR");
            if (libDir != null)
            {
                libDir = Path.Combine(libDir, "Lib");
                if (File.Exists(fileName))
                {
                    return (new FileInfo(Path.Combine(libDir, fileName)).FullName);
                }
                else if (File.Exists(Path.Combine(libDir, fileName) + ".cs"))
                {
                    return (new FileInfo(Path.Combine(libDir, fileName) + ".cs").FullName);
                }
            }

            //PATH
            string[] pathDirs = Environment.GetEnvironmentVariable("PATH").Split(';');
            foreach (string pathDir in pathDirs)
            {
                string dir = pathDir;
                if (File.Exists(fileName))
                {
                    return (new FileInfo(Path.Combine(dir, fileName)).FullName);
                }
                else if (File.Exists(Path.Combine(dir, fileName) + ".cs"))
                {
                    return (new FileInfo(Path.Combine(dir, fileName) + ".cs").FullName);
                }
            }

            throw new FileNotFoundException(string.Format("Could not find file \"{0}\"", fileName));
        }

        private object ParseUsing(string statement)
        {
            string namespaseStatement =
                statement.Replace("//", "").Replace(";", "").Replace("\n", "").Replace("\r", "").Replace("using", "").
                    Trim();
            string[] parts = namespaseStatement.Split("=".ToCharArray());
            return parts[parts.Length - 1].Trim();
        }

        private object ParseImport(string statement)
        {
            return
                new ScriptInfo(
                    statement.Replace("//css_import", "").Replace(";", "").Replace("\n", "").Replace("\r", "").Trim());
        }

        private object ParseAsmRefference(string statement)
        {
            return statement.Replace("//css_reference", "").Replace(";", "").Replace("\n", "").Replace("\r", "").Trim();
        }
    }

    /// <summary>
    /// Class that implements parsing the single C# Script file
    /// </summary>
    /// <summary>
    /// Implementation of the IComparer for sorting operations of collections of FileParser instances
    /// </summary>
    internal class FileParserComparer : IComparer
    {
        int IComparer.Compare(object x, object y)
        {
            if (x == null && y == null)
                return 0;

            int retval = x == null ? -1 : (y == null ? 1 : 0);

            if (retval == 0)
            {
                FileParser xParser = (FileParser) x;
                FileParser yParser = (FileParser) y;
                retval = string.Compare(xParser.fileName, yParser.fileName, true);
                if (retval == 0)
                {
                    retval = ParsingParams.Compare(xParser.prams, yParser.prams);
                }
            }

            return retval;
        }
    }

    /// <summary>
    /// Class that manages parsing the main and all imported (if any) C# Script files
    /// </summary>
    public class ScriptParser
    {
        /// <summary>
        /// 
        /// </summary>
        public ApartmentState apartmentState = ApartmentState.Unknown;

        /// <summary>
        /// 
        /// </summary>
        public string[] FilesToCompile
        {
            get
            {
                ArrayList retval = new ArrayList();
                foreach (FileParser file in fileParsers)
                    retval.Add(file.FileToCompile);
                return (string[]) retval.ToArray(typeof (string));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string[] ReferencedNamespaces
        {
            get { return (string[]) referencedNamespaces.ToArray(typeof (string)); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string[] ReferencedAssemblies
        {
            get { return (string[]) referencedAssemblies.ToArray(typeof (string)); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public ScriptParser(string fileName)
        {
            referencedNamespaces = new ArrayList();
            referencedAssemblies = new ArrayList();

            //process main file
            FileParser mainFile = new FileParser(fileName, null, true, false, null);
            apartmentState = mainFile.apartmentState;

            foreach (string namespaceName in mainFile.ReferencedNamespaces)
                PushNamespace(namespaceName);

            foreach (string asmName in mainFile.ReferencedAssemblies)
                PushAssembly(asmName);

            searchDirs = new[] {Path.GetDirectoryName(mainFile.fileName)};
            //note: mainFile.fileName is warrantied to be a full name but fileName is not

            //process impported files if any
            foreach (ScriptInfo fileInfo in mainFile.ReferencedScripts)
                ProcessFile(fileInfo);

            //Main script file shall always be the first. Add it now as previously array was sorted a few times
            fileParsers.Insert(0, mainFile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileInfo"></param>
        private void ProcessFile(ScriptInfo fileInfo)
        {
            FileParserComparer fileComparer = new FileParserComparer();

            FileParser importedFile = new FileParser(fileInfo.fileName, fileInfo.parseParams, false, true, searchDirs);
            //do not parse it yet (the third param is false)
            if (fileParsers.BinarySearch(importedFile, fileComparer) < 0)
            {
                importedFile.ProcessFile();
                //parse now namespaces, ref. assemblies and scripts; also it will do namespace renaming

                fileParsers.Add(importedFile);
                fileParsers.Sort(fileComparer);

                foreach (string namespaceName in importedFile.ReferencedNamespaces)
                    PushNamespace(namespaceName);

                foreach (string asmName in importedFile.ReferencedAssemblies)
                    PushAssembly(asmName);

                foreach (ScriptInfo scriptFile in importedFile.ReferencedScripts)
                    ProcessFile(scriptFile);
            }
        }

        private readonly ArrayList fileParsers = new ArrayList();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string[] SaveImportedScripts()
        {
            string workingDir = Path.GetDirectoryName(((FileParser) fileParsers[0]).fileName);
            ArrayList retval = new ArrayList();

            for (int i = 1; i < FilesToCompile.Length; i++) //imported script file only
            {
                string scriptFile = FilesToCompile[i];
                string newFileName = Path.Combine(workingDir, Path.GetFileName(scriptFile));
                if (File.Exists(newFileName))
                    File.SetAttributes(newFileName, FileAttributes.Normal);
                File.Copy(scriptFile, newFileName, true);
                retval.Add(newFileName);
            }
            return (string[]) retval.ToArray(typeof (string));
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeleteImportedFiles()
        {
            for (int i = 1; i < FilesToCompile.Length; i++) //do not delete main script file (index == 0)
            {
                if (i != 0)
                {
                    try
                    {
                        File.SetAttributes(FilesToCompile[i], FileAttributes.Normal);
                        File.Delete(FilesToCompile[i]);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private readonly ArrayList referencedNamespaces;
        private readonly ArrayList referencedAssemblies;
        private readonly string[] searchDirs;

        private void PushNamespace(string nameSpace)
        {
            if (referencedNamespaces.Count > 1)
                referencedNamespaces.Sort();

            if (referencedNamespaces.BinarySearch(nameSpace) < 0)
                referencedNamespaces.Add(nameSpace);
        }

        private void PushAssembly(string asmName)
        {
            string entrtyName = asmName.ToLower();
            if (referencedAssemblies.Count > 1)
                referencedAssemblies.Sort();

            if (referencedAssemblies.BinarySearch(entrtyName) < 0)
                referencedAssemblies.Add(entrtyName);
        }
    }
}