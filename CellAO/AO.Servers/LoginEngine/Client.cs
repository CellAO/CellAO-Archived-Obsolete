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
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    using AO.Core;
    using AO.Core.Components;
    using AO.Core.Config;

    using Cell.Core;

    using LoginEngine.Packets;
    using LoginEngine.QueryBase;

    using SmokeLounge.AOtomation.Messaging.GameData;
    using SmokeLounge.AOtomation.Messaging.Messages;
    using SmokeLounge.AOtomation.Messaging.Messages.SystemMessages;

    using Header = SmokeLounge.AOtomation.Messaging.Messages.Header;
    using Identity = SmokeLounge.AOtomation.Messaging.GameData.Identity;

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

            var selectCharacterMessage = message.Body as SelectCharacterMessage;
            if (selectCharacterMessage != null)
            {
                this.OnSelectCharacterMessage(selectCharacterMessage);
                return;
            }

            var randomNameRequestMessage = message.Body as RandomNameRequestMessage;
            if (randomNameRequestMessage != null)
            {
                this.OnRandomNameRequestMessage(randomNameRequestMessage);
                return;
            }

            var deleteCharacterMessage = message.Body as DeleteCharacterMessage;
            if (deleteCharacterMessage != null)
            {
                this.OnDeleteCharacterMessage(deleteCharacterMessage);
                return;
            }

            var createCharacterMessage = message.Body as CreateCharacterMessage;
            if (createCharacterMessage != null)
            {
                this.OnCreateCharacterMessage(createCharacterMessage);
                return;
            }

            var messageNumber = this.GetMessageNumber(packet);
            this.Server.Warning(this, "Client sent unknown message {0}", messageNumber.ToString(CultureInfo.InvariantCulture));
        }

        private void OnCreateCharacterMessage(CreateCharacterMessage createCharacterMessage)
        {
            var characterName = new CharacterName
                                    {
                                        AccountName = this.accountName,
                                        Name = createCharacterMessage.Name,
                                        Breed = (int)createCharacterMessage.Breed,
                                        Gender = (int)createCharacterMessage.Gender,
                                        Profession = (int)createCharacterMessage.Profession,
                                        Level = createCharacterMessage.Level,
                                        HeadMesh = createCharacterMessage.HeadMesh,
                                        MonsterScale = createCharacterMessage.MonsterScale,
                                        Fatness = createCharacterMessage.Fatness
                                    };
            var characterId = characterName.CheckAgainstDatabase();

            if (characterId < 1)
            {
                this.Send(0x0000FFFF, new NameInUseMessage());
                return;
            }
            
            characterName.SendNameToStartPlayfield(createCharacterMessage.StarterArea == StarterArea.Shadowlands, characterId);
            this.Send(0x0000FFFF, new CharacterCreatedMessage { CharacterId = characterId });
        }

        private void OnDeleteCharacterMessage(DeleteCharacterMessage deleteCharacterMessage)
        {
            var characterName = new CharacterName();
            characterName.DeleteChar(deleteCharacterMessage.CharacterId);
            var characterDeletedMessage = new CharacterDeletedMessage { CharacterId = deleteCharacterMessage.CharacterId };
            this.Send(0x0000FFFF, characterDeletedMessage);
        }

        private void OnRandomNameRequestMessage(RandomNameRequestMessage randomNameRequestMessage)
        {
            var characterName = new CharacterName();
            var suggestNameMessage = new SuggestNameMessage
                                         {
                                             Name =
                                                 characterName.GetRandomName(
                                                     randomNameRequestMessage.Profession)
                                         };
            this.Send(0x0000FFFF, suggestNameMessage);
        }

        private void OnSelectCharacterMessage(SelectCharacterMessage selectCharacterMessage)
        {
            var checkLogin = new CheckLogin();
            if (checkLogin.IsCharacterOnAccount(this, selectCharacterMessage.CharacterId) == false)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(
                    "Client '" + this.AccountName + "' tried to log in as CharID " + selectCharacterMessage.CharacterId
                    + " but it is not on their account!");
                Console.ResetColor();

                // NV: Is this really what we want to send? Should find out sometime...
                this.Send(0x00001F83, new LoginErrorMessage { Error = LoginError.InvalidUserNamePassword });
                this.Server.DisconnectClient(this);
                return;
            }

            if (OnlineChars.IsOnline(selectCharacterMessage.CharacterId))
            {
                Console.WriteLine(
                    "Client '" + this.AccountName
                    + "' is trying to login, but the requested character is already logged in.");
                this.Send(0x00001F83, new LoginErrorMessage { Error = LoginError.AlreadyLoggedIn });
                this.Server.DisconnectClient(this);
                return;
            }

            OnlineChars.SetOnline(selectCharacterMessage.CharacterId);

            IPAddress zoneIpAdress;
            if (IPAddress.TryParse(ConfigReadWrite.Instance.CurrentConfig.ZoneIP, out zoneIpAdress) == false)
            {
                var zoneHost = Dns.GetHostEntry(ConfigReadWrite.Instance.CurrentConfig.ZoneIP);
                zoneIpAdress = zoneHost.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            }

            var zoneRedirectionMessage = new ZoneRedirectionMessage
                                             {
                                                 CharacterId = selectCharacterMessage.CharacterId,
                                                 ServerIpAddress = zoneIpAdress,
                                                 ServerPort =
                                                     (ushort)
                                                     ConfigReadWrite.Instance.CurrentConfig
                                                                    .ZonePort
                                             };
            this.Send(0x0000615B, zoneRedirectionMessage);
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

                this.Send(0x00001F83, new LoginErrorMessage { Error = LoginError.InvalidUserNamePassword });
                this.Server.DisconnectClient(this);
                return;
            }

            if (checkLogin.IsLoginCorrect(this, userCredentialsMessage.Credentials) == false)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Client '" + this.AccountName + "' failed Authentication.");
                Console.ResetColor();

                this.Send(0x00001F83, new LoginErrorMessage { Error = LoginError.InvalidUserNamePassword });
                this.Server.DisconnectClient(this);
                return;
            }

            var expansions = 0;
            var allowedCharacters = 0;

            /* This checks your expansions and
               number of characters allowed (num. of chars doesn't work)*/
            string sqlQuery = "SELECT `Expansions`,`Allowed_Characters` FROM `login` WHERE Username = '" + accountName
                              + "'";
            var ms = new SqlWrapper();
            var dt = ms.ReadDatatable(sqlQuery);
            if (dt.Rows.Count > 0)
            {
                expansions = int.Parse((string)dt.Rows[0][0]);
                allowedCharacters = (int)dt.Rows[0][1];
            }

            var characters = from c in CharacterList.LoadCharacters(this.accountName) 
                             select new LoginCharacterInfo
                                        {
                                            Unknown1 = 4, 
                                            Id = c.Id,
                                            PlayfieldProxyVersion = 0x61,
                                            PlayfieldId = new Identity { IdentityType = IdentityType.Playfield, Instance = c.Playfield },
                                            PlayfieldAttribute = 1,
                                            ExitDoor = 0,
                                            ExitDoorId = Identity.None,
                                            Unknown2 = 1,
                                            CharacterInfoVersion = 5,
                                            CharacterId = c.Id,
                                            Name = c.Name,
                                            Breed = (Breed)c.Breed,
                                            Gender = (Gender)c.Gender,
                                            Profession = (Profession)c.Profession,
                                            Level = c.Level,
                                            AreaName = "area unknown",
                                            Status = CharacterStatus.Active
                                        };
            var characterListMessage = new CharacterListMessage
                                           {
                                               Characters = characters.ToArray(),
                                               AllowedCharacters = allowedCharacters,
                                               Expansions = expansions
                                           };
            this.Send(0x0000615B, characterListMessage);
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
            this.Send(0x00002B3F, serverSaltMessage);
        }
        
        // TODO: Investigate if reciever is a timestamp
        private void Send(int receiver, MessageBody messageBody)
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
                                              Receiver = receiver
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