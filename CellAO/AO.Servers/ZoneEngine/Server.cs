// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Server.cs" company="CellAO Team">
//   Copyright © 2005-2013 CellAO Team.
//   
//   All rights reserved.
//   
//   Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//   
//       * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//       * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//       * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//   
//   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//   A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//   CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//   EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//   PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//   LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//   NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//   SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// </copyright>
// <summary>
//   Defines the Server type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZoneEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Globalization;
    using System.Net;
    using System.Threading;
    using System.Timers;

    using Cell.Core;

    using ZoneEngine.Collision;
    using ZoneEngine.Component;
    using ZoneEngine.Misc;
    using ZoneEngine.PacketHandlers;

    using Timer = System.Timers.Timer;

    [Export]
    public class Server : ServerBase
    {
        private readonly ClientFactory clientFactory;

        #region Fields

        public List<Doors> Doors;

        public List<NonPlayerCharacterClass> Monsters;

        public SystemMessage SystemMessageHandler;

        public TextMessage TextMessageHandler;

        public List<VendingMachine> Vendors;

        public Timer Walker = new Timer(100);

        // TODO: Add Ping Message Handler
        // TODO: Add Operator Message Handler
        public WallCollision ZoneBorderHandler;

        #endregion

        #region Constructors and Destructors

        [ImportingConstructor]
        public Server(ClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
            this.TextMessageHandler = new TextMessage();

            // TODO: Add Ping Message Handler Construction
            // TODO: Add Operator Message Handler Construction
            this.ZoneBorderHandler = new WallCollision();
        }

        #endregion

        #region Public Methods and Operators

        public override void Start()
        {
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            // If we just started, how can anyone be currently online?
            // CharStatus.SetAllOffline();
            var charS = new CharStatus();
            charS.SetAllOffline();
            this.Walker.AutoReset = false;
            this.Walker.Elapsed += this.Walker_Elapsed;
            this.Walker.Start();
            base.Start();
        }

        /// <summary>
        /// Walker is the main handler of client based timers
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void Walker_Elapsed(object obj, ElapsedEventArgs e)
        {
            var _now = DateTime.Now;
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

        #endregion

        #region Methods

        protected override ClientBase CreateClient()
        {
            return this.clientFactory.Create(this);
        }

        protected override bool OnClientConnected(ClientBase client)
        {
            return true;
        }

        protected override void OnClientDisconnected(ClientBase clientBase, bool forced)
        {
            var client = (Client)clientBase;

            // client.Server.ConnectedClients.Remove(client.CharacterID);
            var m_ID = BitConverter.GetBytes(client.Character.Id);
            Array.Reverse(m_ID);
            var logoff = new byte[]
                             {
                                 0xdf, 0xdf, 0x00, 0x0a, 0x00, 0x01, 0x00, 0x1d, 0x00, 0x00, 0x0c, 0x0e, 0x00, 0x00, 0x00, 
                                 0x00, 0x36, 0x51, 0x00, 0x78, 0x00, 0x00, 0xc3, 0x50, m_ID[0], m_ID[1], m_ID[2], m_ID[3], 
                                 0x01
                             };
            Announce.PlayfieldOthers(client, logoff);

            client.Character.Purge();

            base.OnClientDisconnected(clientBase, forced);

            // Lua:
            /* Removed Lua Hook
            Program.Script.CallHook("OnClientDisconnect", client);
             */
        }

        protected override void OnReceiveUDP(int num_bytes, byte[] buf, IPEndPoint ip)
        {
        }

        protected override void OnSendTo(IPEndPoint clientIP, int num_bytes)
        {
        }

        #endregion
    }
}