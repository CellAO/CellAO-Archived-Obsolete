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

namespace ChatEngine
{
    using System;
    using System.IO;

    using ChatEngine.Packets;

    using ISComm;
    using ISComm.EventArgs;

    /// <summary>
    /// The isc client.
    /// </summary>
    public class IscClient : ClientBaseClass
    {
        /// <summary>
        /// Handler
        /// </summary>
        /// <param name="s">
        /// </param>
        /// <param name="a">
        /// </param>
        private void ISCClient_OnMessage(object s, OnMessageArgs a)
        {
            Console.WriteLine("[ISComm] Packet '" + a.ID + "' catched.");
            switch (a.ID)
            {
                case 0x0F:
                    {
                        MemoryStream stream = new MemoryStream(a.Data);
                        BinaryReader reader = new BinaryReader(stream);

                        uint senderId = reader.ReadUInt32();
                        byte msgType = reader.ReadByte();
                        short receiversAmount = reader.ReadInt16();
                        uint[] receivers = new uint[receiversAmount];

                        for (int i = 0; i < receiversAmount; i++)
                        {
                            receivers[i] = reader.ReadUInt32();
                        }

                        string msg = reader.ReadString();
                        stream.Close();
                        reader.Close();

                        string lookup = string.Empty;
                        foreach (Client cli in this.server.Clients)
                        {
                            if (cli.Character.characterId == senderId)
                            {
                                lookup = cli.Character.characterName;
                            }
                        }

                        byte[] namelookup = new NameLookupResult().Create(senderId, lookup);

                        Console.WriteLine("Got chat from ZoneEngine: " + msg);
                        MsgVicinity pkt = new MsgVicinity();
                        byte[] packet = pkt.Create(senderId, msg, msgType);

                        foreach (uint sendto in receivers)
                        {
                            foreach (Client cli in this.server.Clients)
                            {
                                if (cli.Character.characterId == sendto)
                                {
                                    if (!cli.KnownClients.Contains(senderId))
                                    {
                                        cli.Send(ref namelookup); // sending a namelookup ahead of the message
                                        cli.KnownClients.Add(senderId);
                                    }

                                    cli.Send(ref packet);
                                }
                            }
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IscClient"/> class.
        /// </summary>
        /// <param name="ip">
        /// The ip.
        /// </param>
        /// <param name="port">
        /// The port.
        /// </param>
        /// <param name="srv">
        /// The srv.
        /// </param>
        public IscClient(string ip, int port, Server srv)
            : base(ip, port)
        {
            this.OnMessage += this.ISCClient_OnMessage;

            this.server = srv;
        }

        /// <summary>
        /// The server.
        /// </summary>
        internal Server server;
    }
}