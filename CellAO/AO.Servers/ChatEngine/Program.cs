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

namespace ChatEngine
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Threading.Tasks;

    using AO.Core;

    using Cell.Core;

    using NBug;

    using NLog;
    using NLog.Config;
    using NLog.Targets;

    /// <summary>
    /// The program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The is modified.
        /// </summary>
        /// <returns>
        /// Returns true if code is modified against the SVN revision
        /// </returns>
        public static bool IsModified()
        {
            string[] info = AssemblyInfoclass.Trademark.Split(';');
            return info[1] == "1";
        }

        /// <summary>
        /// The is mixed.
        /// </summary>
        /// <returns>
        /// Returns true if code is mixed against the SVN revision
        /// </returns>
        public static bool IsMixed()
        {
            string[] info = AssemblyInfoclass.Trademark.Split(';');
            return info[0] == "1";
        }

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// Program arguments
        /// </param>
        private static void Main(string[] args)
        {
            Console.Title = "CellAO " + AssemblyInfoclass.Title + " Console. Version: " + AssemblyInfoclass.Description
                            + " " + AssemblyInfoclass.AssemblyVersion;
            ConsoleText ct = new ConsoleText();
            ct.TextRead("main.txt");
            Console.WriteLine("Loading " + AssemblyInfoclass.Title + "...");
            if (IsModified())
            {
                Console.WriteLine("Your " + AssemblyInfoclass.Title + " was compiled from modified source code.");
            }
            else if (IsMixed())
            {
                Console.WriteLine("Your " + AssemblyInfoclass.Title + " uses mixed SVN revisions.");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Using ISComm v1.0");
            Console.WriteLine("[OK]");
            Console.ResetColor();

            bool processedArgs = false;
            ChatServer chatServer = new ChatServer();

            Console.WriteLine("[ISComm] Waiting for link...");

            LoggingConfiguration config = new LoggingConfiguration();
            ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget();
            consoleTarget.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}";
            FileTarget fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);
            fileTarget.FileName = "${basedir}/ChatEngineLog.txt";
            fileTarget.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}";
            LoggingRule rule1 = new LoggingRule("*", LogLevel.Trace, consoleTarget);
            config.LoggingRules.Add(rule1);
            LoggingRule rule2 = new LoggingRule("*", LogLevel.Trace, fileTarget);
            config.LoggingRules.Add(rule2);
            LogManager.Configuration = config;

            IPAddress localISComm;

            try
            {
                // Local ISComm IP valid?
                localISComm = IPAddress.Parse(Config.Instance.CurrentConfig.ISCommLocalIP);
            }
            catch
            {
                // Fallback to ZoneIP
                localISComm = IPAddress.Parse(Config.Instance.CurrentConfig.ZoneIP);
            }

            if (!ZoneCom.Link(localISComm.ToString(), Config.Instance.CurrentConfig.CommPort, chatServer))
            {
                Console.WriteLine("[ISComm] Unable to link to ZoneEngine. :(");
                return;
            }

            Console.WriteLine("[ISComm] Linked with ZoneEngine! :D");

            chatServer.EnableTCP = true;
            chatServer.EnableUDP = false;

            try
            {
                chatServer.TcpIP = IPAddress.Parse(Config.Instance.CurrentConfig.ListenIP);
            }
            catch
            {
                ct.TextRead("ip_config_parse_error.txt");
                Console.ReadKey();
                return;
            }

            try
            {
                chatServer.UdpIP = IPAddress.Parse(Config.Instance.CurrentConfig.ListenIP);
            }
            catch
            {
                ct.TextRead("ip_config_parse_error.txt");
                Console.ReadKey();
                return;
            }

            chatServer.TcpPort = Convert.ToInt32(Config.Instance.CurrentConfig.ChatPort);
            chatServer.MaximumPendingConnections = 100;

            #region NBug
            AppDomain.CurrentDomain.UnhandledException += Handler.UnhandledException;
            TaskScheduler.UnobservedTaskException += Handler.UnobservedTaskException;

            // TODO: ADD More Handlers.
            #endregion

            #region Console Commands
            // Andyzweb: I added checks for if the server is running or not
            // also a command called running that returns the status of the server
            // and added the Console.Write("\nServer Command >>"); to chatserver
            string consoleCommand;
            ct.TextRead("chat_consolecommands.txt");
            while (true)
            {
                if (!processedArgs)
                {
                    if (args.Length == 1)
                    {
                        if (args[0].ToLower() == "/autostart")
                        {
                            ct.TextRead("autostart.txt");
                            ThreadMgr.Start();
                            chatServer.Start();
                        }
                    }

                    processedArgs = true;
                }

                Console.Write("\nServer Command >>");
                consoleCommand = Console.ReadLine();
                switch (consoleCommand.ToLower())
                {
                    case "start":
                        if (chatServer.Running)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Chat Server is already running");
                            Console.ResetColor();
                            break;
                        }

                        ThreadMgr.Start();
                        chatServer.Start();
                        break;
                    case "stop":
                        if (!chatServer.Running)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Chat Server is not running");
                            Console.ResetColor();
                            break;
                        }

                        ThreadMgr.Stop();
                        chatServer.Stop();
                        break;
                    case "exit":
                        Process.GetCurrentProcess().Kill();
                        break;
                    case "running":
                        if (chatServer.Running)
                        {
                            Console.WriteLine("Chat Server is Running");
                            break;
                        }

                        Console.WriteLine("Chat Server not Running");
                        break;
                    default:
                        ct.TextRead("chat_consolecmdsdefault.txt");
                        break;
                }
            }
            #endregion
        }
    }
}