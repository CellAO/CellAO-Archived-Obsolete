// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="">
//   
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region License

// Copyright (c) 2005-2012, CellAO Team
// All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//     * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace RevisionGetter
{
    #region Usings ...

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    #endregion

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        #region Public Methods and Operators

        /// <summary>
        /// The find git.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string FindGit()
        {
            if (Environment.GetEnvironmentVariable("ProgramFiles") != null)
            {
                // We're on windows
                string testPath =
                    Path.Combine(
                        Path.Combine(Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles"), "git"), "bin"),
                        "git.exe");
                if (File.Exists(testPath))
                {
                    return testPath;
                }
            }

            if (Environment.GetEnvironmentVariable("ProgramFiles(x86)") != null)
            {
                // We're on windows
                string testPath =
                    Path.Combine(
                        Path.Combine(
                            Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "git"), "bin"),
                        "git.exe");
                if (File.Exists(testPath))
                {
                    return testPath;
                }
            }

            if (Environment.GetEnvironmentVariable("ProgramFilesW6432") != null)
            {
                // We're on windows
                string testPath =
                    Path.Combine(
                        Path.Combine(
                            Path.Combine(Environment.GetEnvironmentVariable("ProgramFilesW6432"), "git"), "bin"),
                        "git.exe");
                if (File.Exists(testPath))
                {
                    return testPath;
                }
            }

            string[] EnvironmentPaths = Environment.GetEnvironmentVariable("PATH").Split(';');

            foreach (string environmentPath in EnvironmentPaths)
            {
                if (File.Exists(Path.Combine(environmentPath, "git.exe")))
                {
                    return Path.Combine(environmentPath, "git.exe");
                }
            }

            // unix/mono part coming soon
            return string.Empty;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public static string Findsvn()
        {
            if (Environment.GetEnvironmentVariable("ProgramFiles") != null)
            {
                // We're on windows
                string testPath =
                    Path.Combine(
                        Path.Combine(
                            Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles"), "tortoisesvn"), "bin"),
                        "svn.exe");
                if (File.Exists(testPath))
                {
                    return testPath;
                }
            }

            if (Environment.GetEnvironmentVariable("ProgramFiles(x86)") != null)
            {
                // We're on windows
                string testPath =
                    Path.Combine(
                        Path.Combine(
                            Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "tortoisesvn"), "bin"),
                        "svn.exe");
                if (File.Exists(testPath))
                {
                    return testPath;
                }
            }

            if (Environment.GetEnvironmentVariable("ProgramFilesW6432") != null)
            {
                // We're on windows
                string testPath =
                    Path.Combine(
                        Path.Combine(
                            Path.Combine(Environment.GetEnvironmentVariable("ProgramFilesW6432"), "tortoisesvn"), "bin"),
                        "svn.exe");
                if (File.Exists(testPath))
                {
                    return testPath;
                }
            }

            string[] EnvironmentPaths = Environment.GetEnvironmentVariable("PATH").Split(';');

            foreach (string environmentPath in EnvironmentPaths)
            {
                if (File.Exists(Path.Combine(environmentPath, "svn.exe")))
                {
                    return Path.Combine(environmentPath, "svn.exe");
                }
            }

            // unix/mono part coming soon
            return string.Empty;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Apply Revision number to AssemblyInfo.cs (GIT version)
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <param name="filename">
        /// The filename.
        /// </param>
        private static void ApplyRevisionToAssemblyInfos(string tag, string filename)
        {
            string newTag = tag.Split('-')[0] + "." + tag.Split('-')[1];

            TextReader tr = new StreamReader(filename + ".template");
            List<string> content = new List<string>();
            string line;
            while ((line = tr.ReadLine()) != null)
            {
                content.Add(line);
            }

            tr.Close();

            TextWriter tw = new StreamWriter(filename, false);

            foreach (string sline in content)
            {
                string wline = sline;
                if (sline == "[assembly: AssemblyTrademark(\"0;0\")]")
                {
                    wline = "[assembly: AssemblyTrademark(\"" + tag + "-GIT\")]";
                }

                if (sline.IndexOf("\"1.4.1.0\"") > -1)
                {
                    wline = sline.Replace("\"1.4.1.0\"", "\"" + newTag + "\"");
                }

                tw.WriteLine(wline);
            }

            tw.Close();
        }

        /// <summary>
        /// Apply Revision number to AssemblyInfo.cs (SVN version)
        /// </summary>
        /// <param name="tag">
        /// </param>
        /// <param name="filename">
        /// </param>
        private static void ApplyRevisionToAssemblyInfosSVN(string tag, string filename)
        {
            TextReader tr = new StreamReader(filename + ".template");
            List<string> content = new List<string>();
            string line;
            while ((line = tr.ReadLine()) != null)
            {
                content.Add(line);
            }

            tr.Close();

            TextWriter tw = new StreamWriter(filename, false);

            foreach (string sline in content)
            {
                string wline = sline;
                if (sline == "[assembly: AssemblyTrademark(\"0;0\")]")
                {
                    // TODO: remove the 1.4.1. and get that 
                    wline = "[assembly: AssemblyTrademark(\"1.4.1." + tag + "-SVN\")]";
                }

                if (sline.IndexOf("\"1.4.1.0\"") > -1)
                {
                    wline = sline.Replace("\"1.4.1.0\"", "\"1.4.1." + tag + "\"");
                }

                tw.WriteLine(wline);
            }

            tw.Close();
        }

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// </returns>
        private static int Main(string[] args)
        {
            try
            {
                Console.WriteLine("Changing File: " + args[0]);

                string PathToGit = FindGit();
                if ((PathToGit != string.Empty) && (Directory.Exists("..\\..\\..\\.git")))
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(PathToGit);
                    startInfo.UseShellExecute = false;
                    startInfo.WorkingDirectory = ".";
                    startInfo.Arguments = "describe --tags --long --dirty";
                    startInfo.RedirectStandardOutput = true;
                    Process process = new Process();
                    process.StartInfo = startInfo;
                    process.Start();

                    List<string> output = new List<string>();
                    string lineVal = process.StandardOutput.ReadLine();
                    string tag = lineVal;
                    while (lineVal != null)
                    {
                        output.Add(lineVal);
                        lineVal = process.StandardOutput.ReadLine();
                    }

                    ApplyRevisionToAssemblyInfos(tag, args[0]);
                    process.Close();
                    process.Dispose();
                    return 0;
                }

                string PathToSvn = Findsvn();
                if ((PathToSvn != string.Empty) && (Directory.Exists("..\\..\\..\\.svn")))
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(PathToSvn);
                    startInfo.UseShellExecute = false;
                    startInfo.WorkingDirectory = "..\\..\\..";
                    startInfo.Arguments = "info";
                    startInfo.RedirectStandardOutput = true;
                    startInfo.RedirectStandardError = true;
                    Process process = new Process();
                    process.StartInfo = startInfo;
                    process.Start();

                    List<string> output = new List<string>();
                    string lineVal = string.Empty;
                    while ((lineVal = process.StandardOutput.ReadLine()) != null)
                    {
                        output.Add(lineVal);
                    }
                    while ((lineVal = process.StandardError.ReadLine()) != null)
                    {
                        output.Add(lineVal);
                    }

                    string tag = string.Empty;
                    foreach (string line in output)
                    {
                        if (line.Length >= 9)
                        {
                            if (line.Substring(0, 9) == "Revision:")
                            {
                                tag = line.Substring(9).Trim();
                            }
                        }
                    }

                    ApplyRevisionToAssemblyInfosSVN(tag, args[0]);
                    process.Close();
                    process.Dispose();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("An Error occurred: " + ex.Message);
                return 1;
            }

            Console.Error.WriteLine(
                "No git or svn (svn) found. Please install either svn or git and download the last revision again");
            Console.Error.WriteLine("Please make sure, your git or svn is in the PATH environment variable.");
            return 1;
        }

        #endregion
    }
}