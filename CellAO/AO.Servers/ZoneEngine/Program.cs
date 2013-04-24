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

namespace ZoneEngine
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    using AO.Core;
    using AO.Core.Components;

    using Cell.Core;

    using MySql.Data.MySqlClient;

    using NBug;
    using NBug.Properties;

    using NLog;
    using NLog.Config;
    using NLog.Targets;

    using ZoneEngine.Functions;
    using ZoneEngine.Misc;
    using ZoneEngine.NonPlayerCharacter;
    using ZoneEngine.Script;

    public class Program
    {
        public static readonly IContainer Container = new MefContainer();

        public static Server zoneServer;

        public static ScriptCompiler csc;

        public static FunctionCollection FunctionC = new FunctionCollection();

        private static void Main(string[] args)
        {
            bool processedargs = false;

            // Please dont kill the commented out lines below for the moment -NV
            //Misc.Playfields.Instance.playfields[0].districts.Add(new ZoneEngine.Misc.DistrictInfo());
            //Misc.Playfields.Instance.playfields[0].districts[0].districtName = "some district";
            //Misc.Playfields.Instance.playfields[0].districts.Add(new ZoneEngine.Misc.DistrictInfo());
            //Misc.Playfields.Instance.playfields[0].districts[1].districtName = "some other district";
            //Misc.DistrictInfo.DumpXML(@"C:\list.xml", Misc.Playfields.Instance.playfields[0]);

            #region Console Text...
            Console.Title = "CellAO " + AssemblyInfoclass.Title + " Console. Version: " + AssemblyInfoclass.Description
                + " " + AssemblyInfoclass.AssemblyVersion + " " + AssemblyInfoclass.Trademark;

            ConsoleText ct = new ConsoleText();
            ct.TextRead("main.txt");
            Console.WriteLine("Loading " + AssemblyInfoclass.Title + "...");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Using ISComm v1.0");
            Console.WriteLine("[OK]");
            Console.ResetColor();
            #endregion

            #region Delete old SqlError.log, so it doesnt get too big
            if (File.Exists("sqlerror.log"))
            {
                File.Delete("sqlerror.log");
            }
            #endregion

            #region ISComm Code Area...
            Console.WriteLine("[ISComm] Waiting for Link...");
            ChatCom.StartLink(Config.Instance.CurrentConfig.CommPort);
            //System.Console.WriteLine("[ISComm] Linked Successfully! :D");
            #endregion

            zoneServer = Container.GetInstance<Server>();
            zoneServer.EnableTCP = true;
            zoneServer.EnableUDP = false;

            #region Script Loading Code Area..
            csc = new ScriptCompiler();
            #endregion

            try
            {
                zoneServer.TcpIP = IPAddress.Parse(Config.Instance.CurrentConfig.ListenIP);
            }
            catch
            {
                ct.TextRead("ip_config_parse_error.txt");
                Console.ReadKey();
                return;
            }
            try
            {
                zoneServer.UdpIP = IPAddress.Parse(Config.Instance.CurrentConfig.ListenIP);
            }
            catch
            {
                ct.TextRead("ip_config_parse_error.txt");
                Console.ReadKey();
                return;
            }

            zoneServer.TcpPort = Convert.ToInt32(Config.Instance.CurrentConfig.ZonePort);

            #region NLog
            LoggingConfiguration config = new LoggingConfiguration();
            ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget();
            consoleTarget.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}";
            FileTarget fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);
            fileTarget.FileName = "${basedir}/ZoneEngineLog.txt";
            fileTarget.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}";
            LoggingRule rule1 = new LoggingRule("*", LogLevel.Trace, consoleTarget);
            config.LoggingRules.Add(rule1);
            LoggingRule rule2 = new LoggingRule("*", LogLevel.Trace, fileTarget);
            config.LoggingRules.Add(rule2);
            LogManager.Configuration = config;
            #endregion

            #region NBug
            SettingsOverride.LoadCustomSettings("NBug.Config");
            NBug.Settings.WriteLogToDisk = true;
            AppDomain.CurrentDomain.UnhandledException += Handler.UnhandledException;
            TaskScheduler.UnobservedTaskException += Handler.UnobservedTaskException;
            //TODO: ADD More Handlers.
            #endregion

            FunctionC.ReadFunctions();
            Console.WriteLine("Registered " + FunctionC.NumberofRegisteredFunctions().ToString() + " Functions");

            #region Console Commands...
            string consoleCommand;
            ct.TextRead("zone_consolecommands.txt");
            // removed CheckDBs here, added commands check and updatedb (updatedb will change to a versioning 

            while (true)
            {
                if (!processedargs)
                {
                    if (args.Length == 1)
                    {
                        if (args[0].ToLower() == "/autostart")
                        {
                            ct.TextRead("autostart.txt");
                            csc.Compile(false);
                            StartTheServer();
                        }
                    }
                    processedargs = true;
                }
                Console.Write("\nServer Command >>");
                consoleCommand = Console.ReadLine();
                switch (consoleCommand.ToLower())
                {
                    case "start":
                        if (zoneServer.Running)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Zone Server is already running");
                            Console.ResetColor();
                            break;
                        }

                        //TODO: Add Sql Check.
                        csc.Compile(false);
                        StartTheServer();
                        break;
                    case "startm": // Multiple dll compile
                        if (zoneServer.Running)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Zone Server is already running");
                            Console.ResetColor();
                            break;
                        }

                        //TODO: Add Sql Check.
                        csc.Compile(true);
                        StartTheServer();
                        break;
                    case "stop":
                        if (!zoneServer.Running)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Zone Server is not running");
                            Console.ResetColor();
                            break;
                        }
                        zoneServer.Stop();
                        ThreadMgr.Stop();
                        break;
                    case "check":
                    case "updatedb":
                        using (SqlWrapper sqltester = new SqlWrapper())
                        {
                            sqltester.CheckDBs();
                            Console.ResetColor();
                        }
                        break;
                    case "exit":
                    case "quit":
                        if (zoneServer.Running)
                        {
                            zoneServer.Stop();
                            ThreadMgr.Stop();
                        }
                        Process.GetCurrentProcess().Kill();
                        break;
                    case "ls": //list all available scripts, dont remove it since it does what it should
                        Console.WriteLine("Available scripts");
                        /* Old Lua way
                        string[] files = Directory.GetFiles("Scripts");*/
                        string[] files = Directory.GetFiles("Scripts\\", "*.cs", SearchOption.AllDirectories);
                        if (files.Length == 0)
                        {
                            Console.WriteLine("No scripts were found.");
                            break;
                        }
                        Console.ForegroundColor = ConsoleColor.Green;
                        foreach (string s in files)
                        {
                            Console.WriteLine(s);
                            /* Old Lua way
                            if (s.EndsWith(".lua"))
                            {
                                Console.WriteLine(s.Split('\\')[1].Split('.')[0]);
                            }*/
                        }
                        Console.ResetColor();
                        break;
                    case "ping":
                        // ChatCom.Server.Ping();
                        Console.WriteLine("Ping is disabled till we can fix it");
                        break;
                    case "running":
                        if (zoneServer.Running)
                        {
                            Console.WriteLine("Zone Server is Running");
                            break;
                        }
                        Console.WriteLine("Zone Server not Running");
                        break;
                    case "online":
                        if (zoneServer.Running)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            lock (zoneServer.Clients)
                            {
                                foreach (Client c in zoneServer.Clients)
                                {
                                    Console.WriteLine("Character " + c.Character.Name + " online");
                                }
                            }
                            Console.ResetColor();
                        }
                        break;
                    default:
                        ct.TextRead("zone_consolecmdsdefault.txt");
                        break;
                }
            }
        }
        #endregion

        public class lintel
        {
            public int ID;

            public Single X;

            public Single Y;

            public Single Z;

            public Single HZ;

            public int frompf;

            public int topf;

            public int toid;

            public int proxy;
        }

        public static void lintels()
        {
            SqlWrapper ms = new SqlWrapper();
            List<lintel> doors = new List<lintel>();
            lintel ll;
            ms.SqlRead("SELECT * FROM doors");

            #region MySql
            if (ms.ismysql)
            {
                while (ms.myreader.Read())
                {
                    ll = new lintel();
                    ll.ID = ms.myreader.GetInt32("ID");
                    ll.X = ms.myreader.GetFloat("X");
                    ll.Y = ms.myreader.GetFloat("Y");
                    ll.Z = ms.myreader.GetFloat("Z");
                    ll.HZ = ms.myreader.GetFloat("HZ");
                    ll.frompf = ms.myreader.GetInt32("playfield");
                    ll.topf = ms.myreader.GetInt32("toplayfield");
                    ll.toid = ms.myreader.GetInt32("toid");
                    ll.proxy = ms.myreader.GetInt32("proxy");
                    doors.Add(ll);
                }
                ms.myreader.Close();
                ms.mcc.Close();
                ms.mcc.Dispose();
            }
            #endregion

            #region MsSql
            if (ms.ismssql)
            {
                while (ms.sqlreader.Read())
                {
                    ll = new lintel();
                    ll.ID = ms.sqlreader.GetInt32(0);
                    ll.X = ms.sqlreader.GetFloat(1);
                    ll.Y = ms.sqlreader.GetFloat(2);
                    ll.Z = ms.sqlreader.GetFloat(3);
                    ll.HZ = ms.sqlreader.GetFloat(4);
                    ll.frompf = ms.sqlreader.GetInt32(5);
                    ll.topf = ms.sqlreader.GetInt32(6);
                    ll.toid = ms.sqlreader.GetInt32(7);
                    ll.proxy = ms.sqlreader.GetInt32(8);
                    doors.Add(ll);
                }
                ms.sqlreader.Close();
                ms.sqlcc.Close();
                ms.sqlcc.Dispose();
            }
            #endregion

            #region PostgreSql
            if (ms.isnpgsql)
            {
                while (ms.npgreader.Read())
                {
                    ll = new lintel();
                    ll.ID = ms.npgreader.GetInt32(0);
                    ll.X = ms.npgreader.GetFloat(1);
                    ll.Y = ms.npgreader.GetFloat(2);
                    ll.Z = ms.npgreader.GetFloat(3);
                    ll.HZ = ms.npgreader.GetFloat(4);
                    ll.frompf = ms.npgreader.GetInt32(5);
                    ll.topf = ms.npgreader.GetInt32(6);
                    ll.toid = ms.npgreader.GetInt32(7);
                    ll.proxy = ms.npgreader.GetInt32(8);
                    doors.Add(ll);
                }
                ms.npgreader.Close();
                ms.npgcc.Close();
                ms.npgcc.Dispose();
            }
            #endregion

            bool found;
            foreach (lintel l1 in doors)
            {
                found = false;
                foreach (lintel l2 in doors)
                {
                    if (l1.ID != l2.ID)
                    {
                        if (l1.topf != 0)
                        {
                            if ((l1.frompf == l2.topf) && (l1.topf == l2.frompf))
                            {
                                found = true;
                                l1.toid = l2.ID;
                                ms.SqlUpdate(
                                    "UPDATE doors set toid=" + l2.ID.ToString() + " where id=" + l1.ID.ToString());
                                Console.WriteLine(l1.ID.ToString());
                            }
                            else if ((l1.topf == l2.frompf) && (l2.topf == 0))
                            {
                                l1.toid = l2.ID;
                                ms.SqlUpdate(
                                    "UPDATE doors set toid=" + l2.ID.ToString() + " where id=" + l1.ID.ToString());
                                Console.WriteLine(l1.ID.ToString());
                            }
                        }
                    }
                }
                if (!found)
                {
                    ms.SqlUpdate("UPDATE doors SET proxy=1 where ID=" + l1.ID.ToString());
                }
            }
        }

        public static void StartTheServer()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Green;
                zoneServer.Monsters = new List<NonPlayerCharacterClass>();
                zoneServer.Vendors = new List<VendingMachine>();
                zoneServer.Doors = new List<Doors>();

                using (SqlWrapper sqltester = new SqlWrapper())
                {
                    if (sqltester.SQLCheck() != SqlWrapper.DBCheckCodes.DBC_ok)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Database setup not correct");
                        Console.WriteLine("Error: #" + sqltester.lasterrorcode + " - " + sqltester.lasterrormessage);
                        Console.WriteLine("Please press Enter to exit.");
                        Console.ReadLine();
                        Process.GetCurrentProcess().Kill();
                    }
                    sqltester.CheckDBs();
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Loaded {0} items", ItemHandler.CacheAllItems());
                Console.WriteLine("Loaded {0} nanos", NanoHandler.CacheAllNanos());
                Console.WriteLine("Loaded {0} spawns", NonPlayerCharacterHandler.CacheAllFromDB());
                Console.WriteLine("Loaded {0} vendors", VendorHandler.CacheAllFromDB());
                Console.WriteLine("Loaded {0} teleports", DoorHandler.CacheAllFromDB());
                Console.WriteLine("Loaded {0} statels", Statels.CacheAllStatels());

                LootHandler.CacheAllFromDB();
                Tradeskill.CacheItemNames();

                csc.AddScriptMembers();
                csc.CallMethod("Init", null);

                ThreadMgr.Start();
                zoneServer.Start();
                Console.ResetColor();
            }
            catch (MySqlException e)
            {
                Console.WriteLine("MySql Error. Server Cannot Start");
                Console.WriteLine("Exception: " + e.Message);
                string current = DateTime.Now.ToString("HH:mm:ss");
                StreamWriter logfile = File.AppendText("ZoneEngineLog.txt");
                logfile.WriteLine(current + " " + e.Source + " MySql Error. Server Cannot Start");
                logfile.WriteLine(current + " " + e.Source + " Exception: " + e.Message);
                logfile.Close();
                zoneServer.Stop();
                ThreadMgr.Stop();
                Process.GetCurrentProcess().Kill();
            }
        }

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
    }
}