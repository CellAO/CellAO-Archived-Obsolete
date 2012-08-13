#region License
// Copyright (c) 2005-2012, CellAO Team
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//     * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 
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

#region Usings...
using Config = AO.Core.Config.ConfigReadWrite;

#endregion

namespace LoginEngine
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Threading.Tasks;

    using AO.Core;

    using Cell.Core;

    using MySql.Data.MySqlClient;

    using NBug;

    using NLog;
    using NLog.Config;
    using NLog.Targets;

    public class Program
    {
        public static Server LoginServer;

        public static bool ismodified()
        {
            string[] info = AssemblyInfoclass.Trademark.Split(';');
            return (info[1] == "1");
        }

        public static bool ismixed()
        {
            string[] info = AssemblyInfoclass.Trademark.Split(';');
            return (info[0] == "1");
        }

        private static void Main(string[] args)
        {
            #region Console Texts...
            Console.Title = "CellAO " + AssemblyInfoclass.Title + " Console. Version: " + AssemblyInfoclass.Description
                            + " " + AssemblyInfoclass.AssemblyVersion;
            ConsoleText ct = new ConsoleText();
            ct.TextRead("main.txt");
            Console.WriteLine("Loading " + AssemblyInfoclass.Title + "...");
            if (ismodified())
            {
                Console.WriteLine("Your " + AssemblyInfoclass.Title + " was compiled from modified source code.");
            }
            else if (ismixed())
            {
                Console.WriteLine("Your " + AssemblyInfoclass.Title + " uses mixed SVN revisions.");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[OK]");
            Console.ResetColor();
            #endregion

            //Sying helped figure all this code out, about 5 yearts ago! :P
            bool processedargs = false;
            LoginServer = new Server();
            LoginServer.EnableTCP = true;
            LoginServer.EnableUDP = false;
            try
            {
                LoginServer.TcpIP = IPAddress.Parse(Config.Instance.CurrentConfig.ListenIP);
            }
            catch
            {
                ct.TextRead("ip_config_parse_error.txt");
                Console.ReadKey();
                return;
            }
            LoginServer.TcpPort = Convert.ToInt32(Config.Instance.CurrentConfig.LoginPort);

            #region NLog
            LoggingConfiguration config = new LoggingConfiguration();
            ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget();
            consoleTarget.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}";
            FileTarget fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);
            fileTarget.FileName = "${basedir}/LoginEngineLog.txt";
            fileTarget.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}";
            LoggingRule rule1 = new LoggingRule("*", LogLevel.Trace, consoleTarget);
            config.LoggingRules.Add(rule1);
            LoggingRule rule2 = new LoggingRule("*", LogLevel.Trace, fileTarget);
            config.LoggingRules.Add(rule2);
            LogManager.Configuration = config;
            #endregion

            #region NBug
            AppDomain.CurrentDomain.UnhandledException += Handler.UnhandledException;
            TaskScheduler.UnobservedTaskException += Handler.UnobservedTaskException;
            //TODO: ADD More Handlers.
            #endregion

            LoginServer.MaximumPendingConnections = 100;

            #region Console Commands
            //Andyzweb: Added checks for start and stop
            //also added a running command to return status of the server
            //and added Console.Write("\nServer Command >>"); to login server
            string consoleCommand;
            ct.TextRead("login_consolecommands.txt");
            while (true)
            {
                if (!processedargs)
                {
                    if (args.Length == 1)
                    {
                        if (args[0].ToLower() == "/autostart")
                        {
                            ct.TextRead("autostart.txt");
                            ThreadMgr.Start();
                            LoginServer.Start();
                        }
                    }
                    processedargs = true;
                }
                Console.Write("\nServer Command >>");

                consoleCommand = Console.ReadLine();
                string temp = "";
                while (temp != consoleCommand)
                {
                    temp = consoleCommand;
                    consoleCommand = consoleCommand.Replace("  ", " ");
                }
                consoleCommand = consoleCommand.Trim();
                switch (consoleCommand.ToLower())
                {
                    case "start":
                        if (LoginServer.Running)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            ct.TextRead("loginisrunning.txt");
                            Console.ResetColor();
                            break;
                        }
                        ThreadMgr.Start();
                        LoginServer.Start();
                        break;
                    case "stop":
                        if (!LoginServer.Running)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            ct.TextRead("loginisnotrunning.txt");
                            Console.ResetColor();
                            break;
                        }
                        ThreadMgr.Stop();
                        LoginServer.Stop();
                        break;
                    case "exit":
                        Process.GetCurrentProcess().Kill();
                        break;
                    case "running":
                        if (LoginServer.Running)
                        {
                            //Console.WriteLine("Login Server is running");
                            ct.TextRead("loginisrunning.txt");
                            break;
                        }
                        //Console.WriteLine("Login Server not running");
                        ct.TextRead("loginisnotrunning.txt");
                        break;

                        #region Help Commands....
                    case "help":
                        ct.TextRead("logincmdhelp.txt");
                        break;
                    case "help start":
                        ct.TextRead("helpstart.txt");
                        break;
                    case "help exit":
                        ct.TextRead("helpstop.txt");
                        break;
                    case "help running":
                        ct.TextRead("loginhelpcmdrunning.txt");
                        break;
                    case "help Adduser":
                        ct.TextRead("logincmdadduserhelp.txt");
                        break;
                    case "help setpass":
                        ct.TextRead("logincmdhelpsetpass.txt");
                        break;
                        #endregion

                    default:

                        #region Adduser
                        //This section handles the command for adding a user to the database
                        if (consoleCommand.ToLower().StartsWith("adduser"))
                        {
                            string[] parts = consoleCommand.Split(' ');
                            if (parts.Length < 9)
                            {
                                Console.WriteLine(
                                    "Invalid command syntax.\nPlease use:\nAdduser <username> <password> <number of characters> <expansion> <gm level> <email> <FirstName> <LastName>");
                                break;
                            }
                            string username = parts[1];
                            string password = parts[2];
                            int numChars = 0;
                            try
                            {
                                numChars = int.Parse(parts[3]);
                            }
                            catch
                            {
                                Console.WriteLine("Error: <number of characters> must be a number (duh!)");
                                break;
                            }
                            int expansions = 0;
                            try
                            {
                                expansions = int.Parse(parts[4]);
                                if (expansions < 0 || expansions > 2047)
                                {
                                    throw new Exception();
                                }
                            }
                            catch
                            {
                                Console.WriteLine("Error: <expansions> must be a number between 0 and 2047!");
                                break;
                            }
                            int gm = 0;
                            try
                            {
                                gm = int.Parse(parts[5]);
                            }
                            catch
                            {
                                Console.WriteLine("Error: <GM Level> must be number (duh!)");
                                break;
                            }

                            string email = parts[6];
                            try
                            {
                                if (email == null)
                                {
                                    throw new Exception();
                                }
                            }
                            catch
                            {
                                Console.WriteLine("Error: <Email> You must supply an email address for this account");
                                break;
                            }
                            string firstname = parts[7];
                            try
                            {
                                if (firstname == null)
                                {
                                    throw new Exception();
                                }
                            }
                            catch
                            {
                                Console.WriteLine("Error: <FirstName> You must supply a first name for this accout");
                                break;
                            }
                            string lastname = parts[8];
                            try
                            {
                                if (lastname == null)
                                {
                                    throw new Exception();
                                }
                            }
                            catch
                            {
                                Console.WriteLine("Error: <LastName> You must supply a last name for this account");
                                break;
                            }

                            string formatString;
                            formatString =
                                "INSERT INTO `login` (`CreationDate`, `Flags`,`AccountFlags`,`Username`,`Password`,`Allowed_Characters`,`Expansions`, `GM`, `Email`, `FirstName`, `LastName`) VALUES "
                                + "(NOW(), '0', '0', '{0}', '{1}', {2}, {3}, {4}, '{5}', '{6}', '{7}');";

                            LoginEncryption le = new LoginEncryption();

                            string hashedPassword = le.GeneratePasswordHash(password);

                            string sql = String.Format(
                                formatString,
                                username,
                                hashedPassword,
                                numChars,
                                expansions,
                                gm,
                                email,
                                firstname,
                                lastname);
                            SqlWrapper wrp = new SqlWrapper();
                            try
                            {
                                wrp.SqlInsert(sql);
                            }
                            catch (MySqlException ex)
                            {
                                switch (ex.Number)
                                {
                                    case 1062: //duplicate entry for key
                                        Console.WriteLine("A user account with this username already exists.");
                                        break;
                                    default:
                                        Console.WriteLine(
                                            "An error occured while trying to add a new user account:\n{0}", ex.Message);
                                        break;
                                }
                                break;
                            }
                            Console.WriteLine("User added successfully.");
                            break;
                        }
                        #endregion

                        #region Hashpass
                        //This function just hashes the string you enter using the loginencryption method
                        if (consoleCommand.ToLower().StartsWith("hash"))
                        {
                            string Syntax =
                                "The Syntax for this command is \"hash <String to hash>\" alphanumeric no spaces";
                            string[] parts = consoleCommand.Split(' ');
                            if (parts.Length != 2)
                            {
                                Console.WriteLine(Syntax);
                                break;
                            }
                            string pass = parts[1];
                            LoginEncryption le = new LoginEncryption();
                            string hashed = le.GeneratePasswordHash(pass);
                            Console.WriteLine(hashed);
                            break;
                        }
                        #endregion

                        #region setpass
                        //sets the password for the given username
                        //Added by Andyzweb
                        //Still TODO add exception and error handling
                        if (consoleCommand.ToLower().StartsWith("setpass"))
                        {
                            string Syntax =
                                "The syntax for this command is \"setpass <account username> <newpass>\" where newpass is alpha numeric no spaces";
                            string[] parts = consoleCommand.Split(' ');
                            if (parts.Length != 3)
                            {
                                Console.WriteLine(Syntax);
                                break;
                            }
                            string username = parts[1];
                            string newpass = parts[2];
                            LoginEncryption le = new LoginEncryption();
                            string hashed = le.GeneratePasswordHash(newpass);
                            string formatString;
                            formatString = "UPDATE `login` SET Password = '{0}' WHERE login.Username = '{1}'";

                            string sql = String.Format(formatString, hashed, username);

                            SqlWrapper updt = new SqlWrapper();
                            try
                            {
                                updt.SqlUpdate(sql);
                            }
                                //yeah this part here, some kind of exception handling for mysql errors
                            catch
                            {
                            }
                        }
                        #endregion

                        ct.TextRead("login_consolecmdsdefault.txt");
                        break;
                }
            }
            #endregion
        }
    }
}