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

namespace LoginEngine
{
    using System;
    using System.Globalization;
    using System.Net.Sockets;
    using System.Text;

    using AO.Core.Components;

    using Cell.Core;

    using LoginEngine.Packets;

    using SmokeLounge.AOtomation.Messaging.Messages;
    using SmokeLounge.AOtomation.Messaging.Messages.SystemMessages;

    /// <summary>
    /// The client.
    /// </summary>
    public class Client : ClientBase
    {
        private readonly IMessageSerializer messageSerializer;

        /// <summary>
        /// The packet number.
        /// </summary>
        private ushort packetNumber = 1;

        /// <summary>
        /// The account name.
        /// </summary>
        private string accountName = string.Empty;

        /// <summary>
        /// The client version.
        /// </summary>
        private string clientVersion = string.Empty;

        /// <summary>
        /// The server salt.
        /// </summary>
        private string serverSalt = string.Empty;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class. 
        /// The client.
        /// </summary>
        /// <param name="srvr">
        ///     Server object
        /// </param>
        /// <param name="messageSerializer"></param>
        public Client(LoginServer srvr, IMessageSerializer messageSerializer)
            : base(srvr)
        {
            this.messageSerializer = messageSerializer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class. 
        /// The client.
        /// </summary>
        public Client()
            : base(null)
        {
        }

        /// <summary>
        /// The account name.
        /// </summary>
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

        /// <summary>
        /// The client version.
        /// </summary>
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

        /// <summary>
        /// The server salt.
        /// </summary>
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

        #region Misc overrides
        /// <summary>
        /// Send packet data
        /// </summary>
        /// <param name="packet">
        /// The packet data
        /// </param>
        public override void Send(byte[] packet)
        {
            // 18.1 Fix - Dont ask why its not in network byte order like ZoneEngine packets, its too early in the morning
            byte[] pn = BitConverter.GetBytes(this.packetNumber++);
            packet[0] = pn[0];
            packet[1] = pn[1];

            base.Send(packet);
        }

        /// <summary>
        /// Send packet data direct
        /// </summary>
        /// <param name="packet">
        /// The packet data
        /// </param>
        public void Senddirect(byte[] packet)
        {
            if (this.m_tcpSock.Connected)
            {
                using (SocketAsyncEventArgs args = new SocketAsyncEventArgs())
                {
                    args.Completed += SendAsyncComplete2;
                    args.SetBuffer(packet, 0, packet.Length);
                    args.UserToken = this;
                    this.m_tcpSock.SendAsync(args);
                }
            }
        }
        #endregion

        #region Needed overrides
        /// <summary>
        /// The on receive.
        /// </summary>
        /// <param name="numBytes">
        /// Number of bytes
        /// </param>
        protected override void OnReceive(int numBytes)
        {
            byte[] packet = new byte[numBytes];
            Array.Copy(this.m_readBuffer.Array, this.m_readBuffer.Offset, packet, 0, numBytes);

            var message = this.messageSerializer.Deserialize(packet);
            var userLoginMessage = message.Body as UserLoginMessage;
            if (userLoginMessage != null)
            {
                this.OnUserLoginMessage(userLoginMessage);
                return;
            }

            var userCredentialsMessage = message.Body as UserCredentialsMessage;
            if (userCredentialsMessage != null)
            {
                this.OnUserCredentialsMessage(userCredentialsMessage);
                return;
            }

            uint messageNumber = this.GetMessageNumber(packet);
            Parser myParser = new Parser();
            myParser.Parse(this, packet, messageNumber);
        }

        private void OnUserCredentialsMessage(UserCredentialsMessage userCredentialsMessage)
        {
            var checkLogin = new CheckLogin();
            if (checkLogin.IsLoginAllowed(this, userCredentialsMessage.UserName) == false)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(
                    "Client '" + this.AccountName
                    + "' banned, not a valid username, or sent a malformed Authentication Packet");
                Console.ResetColor();

                this.Send(LoginWrong.GetPacket());
                this.Server.DisconnectClient(this);
                return;
            }

            if (checkLogin.IsLoginCorrect(this, userCredentialsMessage.Credentials) == false)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Client '" + this.AccountName + "' failed Authentication.");
                Console.ResetColor();

                this.Send(LoginWrong.GetPacket());
                this.Server.DisconnectClient(this);
                return;
            }
            
            // All's well, send CharacterList Packet
            var charlist = new CharacterListPacket();
            charlist.SendPacket(this, this.AccountName);
        }

        private void OnUserLoginMessage(UserLoginMessage userLoginMessage)
        {
            this.accountName = userLoginMessage.UserName;
            this.clientVersion = userLoginMessage.ClientVersion;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(
                "Client '" + this.accountName + "' connected using version '" + this.clientVersion + "'");
            Console.ResetColor();

            var salt = new byte[0x20];
            var rand = new Random();

            rand.NextBytes(salt);

            var sb = new StringBuilder();
            for (int i = 0; i < 32; i++)
            {
                // 0x00 Breaks Things
                if (salt[i] == 0)
                {
                    salt[i] = 42; // So we change it to something nicer
                }

                sb.Append(salt[i].ToString("x2", CultureInfo.InvariantCulture));
            }

            this.serverSalt = sb.ToString();
            var serverSaltMessage = new ServerSaltMessage { ServerSalt = salt };
            this.Send(serverSaltMessage);
        }

        private void Send(MessageBody messageBody)
        {
            var message = new Message
                              {
                                  Body = messageBody,
                                  Header =
                                      new Header
                                          {
                                              MessageId = BitConverter.ToInt16(new byte[] { 0xDF, 0xDF }, 0),
                                              PacketType = (PacketType)0x0001,
                                              Unknown = 0x0001,
                                              Sender = 0x00000001,
                                              Receiver = 0x00002B3F
                                          }
                              };
            var buffer = this.messageSerializer.Serialize(message);
            this.Send(buffer);
        }

        #endregion

        #region Our own stuff
        /// <summary>
        /// Gets the message number.
        /// </summary>
        /// <param name="packet">
        /// The packet data
        /// </param>
        /// <returns>
        /// The get message number.
        /// </returns>
        protected uint GetMessageNumber(byte[] packet)
        {
            byte[] messageNumberArray = new byte[4];
            messageNumberArray[3] = packet[16];
            messageNumberArray[2] = packet[17];
            messageNumberArray[1] = packet[18];
            messageNumberArray[0] = packet[19];
            uint reply = BitConverter.ToUInt32(messageNumberArray, 0);
            return reply;
        }
        #endregion

        /// <summary>
        /// The send async complete 2.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void SendAsyncComplete2(object sender, SocketAsyncEventArgs args)
        {
        }
    }
}