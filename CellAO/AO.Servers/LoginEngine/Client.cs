// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Client.cs" company="CellAO Team">
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
//   Defines the Client type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoginEngine
{
    using System;
    using System.Globalization;
    using System.Net.Sockets;

    using AO.Core.Components;
    using AO.Core.Events;

    using Cell.Core;

    using SmokeLounge.AOtomation.Messaging.Messages;

    public class Client : ClientBase
    {
        #region Fields

        private readonly IBus bus;

        private readonly IMessageSerializer messageSerializer;

        private string accountName = string.Empty;

        private string clientVersion = string.Empty;

        private ushort packetNumber = 1;

        private string serverSalt = string.Empty;

        #endregion

        #region Constructors and Destructors

        public Client(LoginServer srvr, IMessageSerializer messageSerializer, IBus bus)
            : base(srvr)
        {
            this.messageSerializer = messageSerializer;
            this.bus = bus;
        }

        #endregion

        #region Public Properties

        public string AccountName
        {
            get
            {
                return this.accountName;
            }

            set
            {
                this.accountName = value;
            }
        }

        public string ClientVersion
        {
            get
            {
                return this.clientVersion;
            }

            set
            {
                this.clientVersion = value;
            }
        }

        public string ServerSalt
        {
            get
            {
                return this.serverSalt;
            }

            set
            {
                this.serverSalt = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        public override void Send(byte[] packet)
        {
            // 18.1 Fix - Dont ask why its not in network byte order like ZoneEngine packets, its too early in the morning
            var pn = BitConverter.GetBytes(this.packetNumber++);
            packet[0] = pn[0];
            packet[1] = pn[1];
            if (packet.Length % 4 > 0)
            {
                Array.Resize(ref packet, packet.Length + (4 - (packet.Length % 4)));
            }

            base.Send(packet);
        }

        public void Send(int receiver, MessageBody messageBody)
        {
            // TODO: Investigate if reciever is a timestamp
            var message = new Message
                              {
                                  Body = messageBody, 
                                  Header =
                                      new Header
                                          {
                                              MessageId = BitConverter.ToInt16(new byte[] { 0xDF, 0xDF }, 0), 
                                              PacketType = messageBody.PacketType, 
                                              Unknown = 0x0001, 
                                              Sender = 0x00000001, 
                                              Receiver = receiver
                                          }
                              };
            var buffer = this.messageSerializer.Serialize(message);
            this.Send(buffer);
        }

        public void Senddirect(byte[] packet)
        {
            if (this.m_tcpSock.Connected)
            {
                using (var args = new SocketAsyncEventArgs())
                {
                    args.Completed += SendAsyncComplete2;
                    args.SetBuffer(packet, 0, packet.Length);
                    args.UserToken = this;
                    this.m_tcpSock.SendAsync(args);
                }
            }
        }

        #endregion

        #region Methods

        protected uint GetMessageNumber(byte[] packet)
        {
            var messageNumberArray = new byte[4];
            messageNumberArray[3] = packet[16];
            messageNumberArray[2] = packet[17];
            messageNumberArray[1] = packet[18];
            messageNumberArray[0] = packet[19];
            var reply = BitConverter.ToUInt32(messageNumberArray, 0);
            return reply;
        }

        protected override void OnReceive(int numBytes)
        {
            var packet = new byte[numBytes];
            Array.Copy(this.m_readBuffer.Array, this.m_readBuffer.Offset, packet, 0, numBytes);

            Message message = null;
            try
            {
                message = this.messageSerializer.Deserialize(packet);
            }
            catch (Exception)
            {
                var messageNumber = this.GetMessageNumber(packet);
                this.Server.Warning(
                    this, "Client sent malformed message {0}", messageNumber.ToString(CultureInfo.InvariantCulture));
            }

            if (message == null)
            {
                var messageNumber = this.GetMessageNumber(packet);
                this.Server.Warning(
                    this, "Client sent unknown message {0}", messageNumber.ToString(CultureInfo.InvariantCulture));
                return;
            }

            this.bus.Publish(new MessageReceivedEvent(this, message));
        }

        private static void SendAsyncComplete2(object sender, SocketAsyncEventArgs args)
        {
        }

        #endregion
    }
}