// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="">
//   
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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

            // unix/mono part coming soon
            return "";
        }

        #endregion

        #region Methods

        /// <summary>
        /// The apply revision to assembly infos.
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <param name="filename">
        /// The filename.
        /// </param>
        private static void ApplyRevisionToAssemblyInfos(string tag, string filename)
        {
            string newTag = tag.Split('-')[0]+"."+tag.Split('-')[1];

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
                    wline = "[assembly: AssemblyTrademark(\"" + tag + "\")]";
                }

                if (sline.IndexOf("\"1.4.1.0\"") > -1)
                {
                    wline = sline.Replace("\"1.4.1.0\"","\""+newTag+"\"");
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
        private static int Main(string[] args)
        {
            try
            {
                Console.WriteLine("Changing File: " + args[0]);

                string PathToGit = FindGit();
                if (PathToGit != string.Empty)
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
                }
                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        #endregion
    }
}