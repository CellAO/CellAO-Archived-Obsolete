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

namespace ZoneEngine
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Timers;

    using AO.Core;
    using AO.Core.Components;
    using AO.Core.Events;

    using Cell.Core;

    using ComponentAce.Compression.Libs.zlib;

    using SmokeLounge.AOtomation.Messaging.GameData;
    using SmokeLounge.AOtomation.Messaging.Messages;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
    using SmokeLounge.AOtomation.Messaging.Messages.SystemMessages;

    using ZoneEngine.Misc;
    using ZoneEngine.Packets;

    using Config = AO.Core.Config.ConfigReadWrite;
    using Header = SmokeLounge.AOtomation.Messaging.Messages.Header;
    using Identity = AO.Core.Identity;
    using Quaternion = AO.Core.Quaternion;
    using Timer = System.Timers.Timer;
    using Vector3 = SmokeLounge.AOtomation.Messaging.GameData.Vector3;

    public class Client : ClientBase
    {
        #region Static Fields

        private static Timer LogoutTimer = new Timer();

        #endregion

        #region Fields

        public Character Character = new Character(0, 0);

        public List<AOTimers> CoreTimers = new List<AOTimers>();

        public ushort packetNumber = 1;

        private readonly IBus bus;

        private readonly IMessageSerializer messageSerializer;

        private readonly int serverId;

        private bool SkipCoreTimers = true;

        private NetworkStream netStream;

        private ZOutputStream zStream;

        private bool zStreamSetup;

        #endregion

        #region Constructors and Destructors

        public Client(Server srvr, IMessageSerializer messageSerializer, IBus bus)
            : base(srvr)
        {
            this.messageSerializer = messageSerializer;
            this.bus = bus;
            this.serverId = srvr.Id;
        }

        #endregion

        // Core Timers Enable Variable
        #region Public Methods and Operators

        public void AddCoreTimer(int strain, DateTime time, AOFunctions aof)
        {
            var newCoretimer = new AOTimers();
            newCoretimer.Function = aof;
            newCoretimer.Timestamp = time;
            newCoretimer.Strain = strain;
            this.CoreTimers.Add(newCoretimer);
        }

        public void CancelLogOut()
        {
            LogoutTimer.Enabled = false;
            var message = new CharacterActionMessage
                              {
                                  Identity =
                                      new SmokeLounge.AOtomation.Messaging.GameData.Identity
                                          {
                                              Type
                                                  =
                                                  IdentityType
                                                  .CanbeAffected, 
                                              Instance
                                                  =
                                                  this
                                                  .Character
                                                  .Id
                                          }, 
                                  Unknown = 0x00, 
                                  Action = CharacterActionType.StopLogout, 
                                  Target =
                                      SmokeLounge.AOtomation.Messaging.GameData.Identity.None
                              };
            this.SendCompressed(message);
        }

        public override void Cleanup()
        {
            base.Cleanup();

            // AH FINALLY, Man, get some NORMAL names (OnDisconnect maybe?).
            var foundnextclient = false;
            foreach (Client c in this.Server.Clients)
            {
                if (this == c)
                {
                    continue;
                }

                if (c.Character != null)
                {
                    if (c.Character.Id == this.Character.Id)
                    {
                        foundnextclient = true;
                        break;
                    }
                }
            }

            if (!foundnextclient)
            {
                var charS = new CharStatus();
                charS.SetOffline(this.Character.Id);
            }
        }

        public void PurgeCoreTimer(int strain)
        {
            var c = this.CoreTimers.Count() - 1;
            while (c >= 0)
            {
                if (this.CoreTimers[c].Strain == strain)
                {
                    this.CoreTimers.RemoveAt(c);
                }

                c--;
            }
        }

        public void Send(MessageBody messageBody)
        {
            var message = new Message
                              {
                                  Body = messageBody, 
                                  Header =
                                      new Header
                                          {
                                              MessageId = BitConverter.ToInt16(new byte[] { 0xDF, 0xDF }, 0), 
                                              PacketType = messageBody.PacketType, 
                                              Unknown = 0x0001, 
                                              Sender = 0x03000000, 
                                              Receiver = 0x00000000
                                          }
                              };

            var buffer = this.messageSerializer.Serialize(message);
            this.Send(buffer);
        }

        public override void Send(byte[] packet)
        {
            // 18.1 Fix
            var pn = BitConverter.GetBytes(this.packetNumber++);
            packet[0] = pn[1];
            packet[1] = pn[0];

            this.Send(packet);
        }

        public bool SendChatText(string text)
        {
            var message = new ChatTextMessage
                              {
                                  Identity =
                                      new SmokeLounge.AOtomation.Messaging.GameData.Identity
                                          {
                                              Type =
                                                  IdentityType
                                                  .CanbeAffected, 
                                              Instance
                                                  =
                                                  this
                                                  .Character
                                                  .Id
                                          }, 
                                  Unknown = 0x00, 
                                  Text = text, 
                                  Unknown1 = 0x1000, 
                                  Unknown2 = 0x00000000
                              };
            this.SendCompressed(message);
            return true;
        }

        public void SendCompressed(MessageBody messageBody)
        {
            var message = new Message
                              {
                                  Body = messageBody, 
                                  Header =
                                      new Header
                                          {
                                              MessageId = BitConverter.ToInt16(new byte[] { 0xDF, 0xDF }, 0), 
                                              PacketType = messageBody.PacketType, 
                                              Unknown = 0x0001, 
                                              Sender = this.serverId, 
                                              Receiver = this.Character.Id
                                          }
                              };

            var buffer = this.messageSerializer.Serialize(message);
            this.SendCompressed(buffer);
        }

        public void SendCompressed(byte[] packet)
        {
            var tries = 0;
            var done = false;

            // 18.1 Fix
            var pn = BitConverter.GetBytes(this.packetNumber++);
            packet[0] = pn[1];
            packet[1] = pn[0];

            while ((!done) && (tries < 3))
            {
                try
                {
                    done = true;
                    if (!this.zStreamSetup)
                    {
                        // Create the zStream
                        this.netStream = new NetworkStream(this.TcpSocket);
                        this.zStream = new ZOutputStream(this.netStream, zlibConst.Z_BEST_COMPRESSION);
                        this.zStream.FlushMode = zlibConst.Z_SYNC_FLUSH;
                        this.zStreamSetup = true;
                    }

                    this.zStream.Write(packet, 0, packet.Length);
                    this.zStream.Flush();
                }
                catch (Exception)
                {
                    tries++;
                    done = false;
                    this.Server.DisconnectClient(this);
                    return;
                }
            }

            if (!done)
            {
                // Old Code, probably not needed anymore
                /*
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception caught: "+Program.zoneServer.Clients.Count+" Clients active, Startup: "+Character.startup.ToString()+" DDT: "+Character.dontdotimers.ToString()+" Port: "+TcpPort);
                Console.WriteLine(Debughelpers.PacketToHex(packet));
                Console.WriteLine();
                Console.ResetColor();
                FileInfo t = new FileInfo("exception.log");
                if (t.Exists == true)
                {
                    TextWriter tex = new StreamWriter(t.OpenWrite());
                    tex.WriteLine("Date/Time: " + DateTime.Now.ToString());
                    tex.WriteLine(" ");
                    tex.WriteLine("Dump:");
                    tex.WriteLine(Debughelpers.PacketToHex(packet));
                    
                    tex.Flush();
                    tex.Close();
                    tex = null;
                    t = null;
                }
                else
                {
                    StreamWriter sw = t.CreateText();
                    sw.WriteLine("Date/Time: " + DateTime.Now.ToString());
                    sw.WriteLine(" ");
                    sw.WriteLine("Data: " + BitConverter.ToString(packet).Replace('-', ' '));
                    sw.WriteLine(" ");
                    sw.Write(sw.NewLine);
                    sw.Flush();
                    sw.Close();
                    sw = null;
                    t = null;
                }
                 */
            }
        }

        public bool SendFeedback(int MsgCategory, int MsgNum)
        {
            var message = new FeedbackMessage
                              {
                                  Identity =
                                      new SmokeLounge.AOtomation.Messaging.GameData.Identity
                                          {
                                              Type =
                                                  IdentityType
                                                  .CanbeAffected, 
                                              Instance
                                                  =
                                                  this
                                                  .Character
                                                  .Id
                                          }, 
                                  Unknown = 0x01, 
                                  Unknown1 = 0x00000000, 
                                  CategoryId = MsgCategory, 
                                  MessageId = MsgNum
                              };
            this.SendCompressed(message);
            return true;
        }

        public void StandCancelLogout()
        {
            var standUp = new PacketWriter();

            // start packet header
            standUp.PushByte(0xDF);
            standUp.PushByte(0xDF);
            standUp.PushShort(10);
            standUp.PushShort(1);
            standUp.PushShort(0);
            standUp.PushInt(3086); // Sender (server ID)
            standUp.PushInt(this.Character.Id); // Receiver
            standUp.PushInt(0x5E477770); // CharacterAction packet ID
            standUp.PushIdentity(50000, this.Character.Id); // affected identity
            standUp.PushByte(0);

            // end packet header
            standUp.PushByte(0);
            standUp.PushShort(0);
            standUp.PushByte(0x57); // stand packet flag
            standUp.PushInt(0);
            standUp.PushInt(0);
            standUp.PushInt(0);
            standUp.PushInt(0);
            standUp.PushInt(0);
            standUp.PushShort(0);
            var standUpPacket = standUp.Finish();
            Announce.Playfield(this.Character.PlayField, standUpPacket);

            // SendCompressed(standUpPacket);
            if (LogoutTimer.Enabled)
            {
                this.CancelLogOut();
            }

            // If logout timer is running, CancelLogOut method stops it.
        }

        public bool Teleport(AOCoord destination, Quaternion heading, int playfield)
        {
            var message = new N3TeleportMessage
                              {
                                  Identity =
                                      new SmokeLounge.AOtomation.Messaging.GameData.Identity
                                          {
                                              Type
                                                  =
                                                  IdentityType
                                                  .CanbeAffected, 
                                              Instance
                                                  =
                                                  this
                                                  .Character
                                                  .Id
                                          }, 
                                  Unknown = 0x00, 
                                  Destination =
                                      new Vector3
                                          {
                                              X = destination.x, 
                                              Y = destination.y, 
                                              Z = destination.z
                                          }, 
                                  Heading =
                                      new SmokeLounge.AOtomation.Messaging.GameData.Quaternion
                                          {
                                              X =
                                                  heading
                                                  .xf, 
                                              Y =
                                                  heading
                                                  .yf, 
                                              Z =
                                                  heading
                                                  .zf, 
                                              W =
                                                  heading
                                                  .wf
                                          }, 
                                  Unknown1 = 0x61, 
                                  Playfield =
                                      new SmokeLounge.AOtomation.Messaging.GameData.Identity
                                          {
                                              Type
                                                  =
                                                  IdentityType
                                                  .Playfield1, 
                                              Instance
                                                  =
                                                  playfield
                                          }, 
                                  ChangePlayfield =
                                      playfield != this.Character.PlayField
                                          ? new SmokeLounge.AOtomation.Messaging.GameData.Identity
                                                {
                                                    Type
                                                        =
                                                        IdentityType
                                                        .Playfield2, 
                                                    Instance
                                                        =
                                                        playfield
                                                }
                                          : SmokeLounge.AOtomation.Messaging.GameData.Identity.None, 
                                  Playfield2 =
                                      new SmokeLounge.AOtomation.Messaging.GameData.Identity
                                          {
                                              Type
                                                  =
                                                  IdentityType
                                                  .Playfield3, 
                                              Instance
                                                  =
                                                  playfield
                                          }, 
                              };

            Despawn.DespawnPacket(this.Character.Id);
            this.SendCompressed(message);
            this.Character.DoNotDoTimers = true;
            this.Character.Stats.ExtenalDoorInstance.Value = 0;
            this.Character.Stats.ExtenalPlayfieldInstance.Value = 0;
            this.Character.Stats.LastConcretePlayfieldInstance.Value = 0;

            this.Character.StopMovement();
            this.Character.RawCoord = destination;
            this.Character.RawHeading = heading;
            if (playfield == this.Character.PlayField)
            {
                return true;
            }

            this.Character.PlayField = playfield;
            this.Character.Purge(); // Purge character information to DB before client reconnect

            IPAddress tempIp;
            if (IPAddress.TryParse(Config.Instance.CurrentConfig.ZoneIP, out tempIp) == false)
            {
                var zoneHost = Dns.GetHostEntry(Config.Instance.CurrentConfig.ZoneIP);
                foreach (var ip in zoneHost.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        tempIp = ip;
                        break;
                    }
                }
            }

            var zonePort = Convert.ToInt16(Config.Instance.CurrentConfig.ZonePort);

            Thread.Sleep(1000);

            var redirect = new ZoneRedirectionMessage { ServerIpAddress = tempIp, ServerPort = (ushort)zonePort };
            this.SendCompressed(redirect);
            return true;
        }

        public bool TeleportProxy(
            AOCoord destination, 
            Quaternion heading, 
            int playfield, 
            Identity pfinstance, 
            int GS, 
            int SG, 
            Identity R, 
            Identity dest)
        {
            var message = new N3TeleportMessage
                              {
                                  Identity =
                                      new SmokeLounge.AOtomation.Messaging.GameData.Identity
                                          {
                                              Type
                                                  =
                                                  IdentityType
                                                  .CanbeAffected, 
                                              Instance
                                                  =
                                                  this
                                                  .Character
                                                  .Id
                                          }, 
                                  Unknown = 0x00, 
                                  Destination =
                                      new Vector3
                                          {
                                              X = this.Character.RawCoord.x, 
                                              Y = this.Character.RawCoord.y, 
                                              Z = this.Character.RawCoord.z
                                          }, 
                                  Heading =
                                      new SmokeLounge.AOtomation.Messaging.GameData.Quaternion
                                          {
                                              X =
                                                  this
                                                  .Character
                                                  .RawHeading
                                                  .xf, 
                                              Y =
                                                  this
                                                  .Character
                                                  .RawHeading
                                                  .yf, 
                                              Z =
                                                  this
                                                  .Character
                                                  .RawHeading
                                                  .zf, 
                                              W =
                                                  this
                                                  .Character
                                                  .RawHeading
                                                  .wf
                                          }, 
                                  Unknown1 = 0x61, 
                                  Playfield =
                                      new SmokeLounge.AOtomation.Messaging.GameData.Identity
                                          {
                                              Type
                                                  =
                                                  (
                                                  IdentityType
                                                  )
                                                  pfinstance
                                                      .Type, 
                                              Instance
                                                  =
                                                  pfinstance
                                                  .Instance
                                          }, 
                                  GameServerId = GS, 
                                  SgId = SG, 
                                  ChangePlayfield =
                                      new SmokeLounge.AOtomation.Messaging.GameData.Identity
                                          {
                                              Type
                                                  =
                                                  IdentityType
                                                  .Playfield2, 
                                              Instance
                                                  =
                                                  playfield
                                          }, 
                                  Playfield2 =
                                      new SmokeLounge.AOtomation.Messaging.GameData.Identity
                                          {
                                              Type
                                                  =
                                                  (
                                                  IdentityType
                                                  )
                                                  dest
                                                      .Type, 
                                              Instance
                                                  =
                                                  dest
                                                  .Instance
                                          }, 
                              };

            this.Character.DoNotDoTimers = true;
            Despawn.DespawnPacket(this.Character.Id);
            this.SendCompressed(message);
            this.Character.DoNotDoTimers = true;
            this.Character.Stats.LastConcretePlayfieldInstance.Value = this.Character.PlayField;
            this.Character.Stats.ExtenalDoorInstance.Value = SG;

            this.Character.StopMovement();
            this.Character.RawCoord = destination;
            this.Character.RawHeading = heading;
            this.Character.PlayField = playfield;
            this.Character.Resource = 0x3c000;
            this.Character.Purge(); // Purge character information to DB before client reconnect

            IPAddress tempIp;
            if (IPAddress.TryParse(Config.Instance.CurrentConfig.ZoneIP, out tempIp) == false)
            {
                var zoneHost = Dns.GetHostEntry(Config.Instance.CurrentConfig.ZoneIP);
                foreach (var ip in zoneHost.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        tempIp = ip;
                        break;
                    }
                }
            }

            var zonePort = Convert.ToInt16(Config.Instance.CurrentConfig.ZonePort);

            Thread.Sleep(1000);
            var redirect = new ZoneRedirectionMessage { ServerIpAddress = tempIp, ServerPort = (ushort)zonePort };
            this.SendCompressed(redirect);
            return true;
        }

        public void processCoreTimers(DateTime _now)
        {
            // Current Timer
            int c;

            // Current Strain
            int strain;

            // if Charachter is skipping timers Leave Function
            if (this.SkipCoreTimers)
            {
                return;
            }

            // Create Comparison Value
            int DTCompResult;

            // Backwards, easier to maintain integrity when removing something from the list
            for (c = this.CoreTimers.Count - 1; c >= 0; c--)
            {
                // Compare the Timer Value to the Current Time
                DTCompResult = _now.CompareTo(this.CoreTimers[c].Timestamp);
                if (DTCompResult >= 0)
                {
                    strain = this.CoreTimers[c].Strain;
                    switch (strain)
                    {
                        case 0:

                            // Add new Timer 
                            if (this != null)
                            {
                            }

                            break;

                        case 1:
                            if (this != null)
                            {
                            }

                            break;
                        default:
                            this.CoreTimers[c].Function.Apply(true);
                            break;
                    }

                    if (this.CoreTimers[c].Function.TickCount >= 0)
                    {
                        this.CoreTimers[c].Function.TickCount--;
                    }

                    if (this.CoreTimers[c].Function.TickCount == 0)
                    {
                        // Remove Timer if Ticks ran out
                        this.CoreTimers.RemoveAt(c);
                    }
                    else
                    {
                        // Reinvoke the timer after the TickInterval
                        this.CoreTimers[c].Timestamp = _now
                                                       + TimeSpan.FromMilliseconds(
                                                           this.CoreTimers[c].Function.TickInterval);
                    }
                }
            }
        }

        // Starts a 30 second timer
        public void startLogoutTimer()
        {
            LogoutTimer = new Timer(30000);
            LogoutTimer.Elapsed += this.LogOut;
            LogoutTimer.Enabled = true;
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

        // Called after 30 second timer elapses
        private void LogOut(object sender, ElapsedEventArgs e)
        {
            this.Server.DisconnectClient(this);
        }

        #endregion

        /* Called from CharacterAction class in case of 'stop logout packet'
         * Stops the 30 second timer and sends 'stop logout packet' back to client
        */
    }
}