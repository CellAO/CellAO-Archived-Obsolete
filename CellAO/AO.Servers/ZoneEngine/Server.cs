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
#endregion

namespace ZoneEngine
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Threading;
    using System.Timers;

    using Cell.Core;

    using ZoneEngine.Collision;
    using ZoneEngine.Misc;
    using ZoneEngine.PacketHandlers;

    using Timer = System.Timers.Timer;

    /// <summary>
    /// 
    /// </summary>
    public class Server : ServerBase
    {
        #region Data Members
        public SystemMessage SystemMessageHandler;

        public TextMessage TextMessageHandler;

        // TODO: Add Ping Message Handler
        // TODO: Add Operator Message Handler

        public WallCollision ZoneBorderHandler;
        #endregion

        #region Needed Overrides
        /// <summary>
        /// 
        /// </summary>
        public Server()
        {
            this.SystemMessageHandler = new SystemMessage();
            this.TextMessageHandler = new TextMessage();

            // TODO: Add Ping Message Handler Construction
            // TODO: Add Operator Message Handler Construction

            this.ZoneBorderHandler = new WallCollision();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="num_bytes"></param>
        /// <param name="buf"></param>
        /// <param name="ip"></param>
        protected override void OnReceiveUDP(int num_bytes, byte[] buf, IPEndPoint ip)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientIP"></param>
        /// <param name="num_bytes"></param>
        protected override void OnSendTo(IPEndPoint clientIP, int num_bytes)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override ClientBase CreateClient()
        {
            return new Client(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        public override void Warning(ClientBase client, Exception e)
        {
            base.Warning(client, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="msg"></param>
        /// <param name="parms"></param>
        public override void Warning(ClientBase client, string msg, params object[] parms)
        {
            base.Warning(client, msg, parms);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="msg"></param>
        /// <param name="parms"></param>
        public override void Info(ClientBase client, string msg, params object[] parms)
        {
            base.Info(client, msg, parms);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="msg"></param>
        /// <param name="parms"></param>
        public override void Debug(ClientBase client, string msg, params object[] parms)
        {
            base.Debug(client, msg, parms);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        protected override bool OnClientConnected(ClientBase client)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="forced"></param>
        protected override void OnClientDisconnected(ClientBase clientBase, bool forced)
        {
            Client client = (Client)clientBase;

            //client.Server.ConnectedClients.Remove(client.CharacterID);
            byte[] m_ID = BitConverter.GetBytes(client.Character.Id);
            Array.Reverse(m_ID);
            Byte[] logoff = new byte[]
                {
                    0xdf, 0xdf, 0x00, 0x0a, 0x00, 0x01, 0x00, 0x1d, 0x00, 0x00, 0x0c, 0x0e, 0x00, 0x00, 0x00, 0x00, 0x36,
                    0x51, 0x00, 0x78, 0x00, 0x00, 0xc3, 0x50, m_ID[0], m_ID[1], m_ID[2], m_ID[3], 0x01
                };
            Announce.PlayfieldOthers(client, logoff);

            client.Character.Purge();

            base.OnClientDisconnected(clientBase, forced);

            //Lua:
            /* Removed Lua Hook
            Program.Script.CallHook("OnClientDisconnect", client);
             */
        }
        #endregion

        #region Misc overrides
        /// <summary>
        /// Walker is the main handler of client based timers
        /// </summary>
        public void Walker_Elapsed(object obj, ElapsedEventArgs e)
        {
            DateTime _now = DateTime.Now;
            this.Walker.Enabled = false;
            try
            {
                foreach (Client cl in this.Clients)
                {
                    cl.Character.ProcessTimers(_now);
                }
            }
            catch
            {
                // do nothing, sometimes Clients is empty for last Walker_Elapsed run
            }

            foreach (Character ch in this.Monsters)
            {
                ch.ProcessTimers(_now);
            }
            this.Walker.Enabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Start()
        {
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            // If we just started, how can anyone be currently online?
            //CharStatus.SetAllOffline();
            CharStatus charS = new CharStatus();
            charS.SetAllOffline();
            this.Walker.AutoReset = false;
            this.Walker.Elapsed += this.Walker_Elapsed;
            this.Walker.Start();
            base.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Stop()
        {
            base.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        public override int MaximumPendingConnections
        {
            get
            {
                return base.MaximumPendingConnections;
            }
            set
            {
                base.MaximumPendingConnections = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override IPAddress TcpIP
        {
            get
            {
                return base.TcpIP;
            }
            set
            {
                base.TcpIP = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override int TcpPort
        {
            get
            {
                return base.TcpPort;
            }
            set
            {
                base.TcpPort = value;
            }
        }
        #endregion

        #region Our Own Stuff
        public List<NonPlayerCharacterClass> Monsters;

        public List<VendingMachine> Vendors;

        public List<Doors> Doors;

        public Timer Walker = new Timer(100);
        #endregion
    }
}