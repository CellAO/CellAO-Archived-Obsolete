// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Client.cs" company="CellAO Team">
//   Copyright � 2005-2013 CellAO Team.
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
    using System.Text;
    using System.Threading;
    using System.Timers;

    using AO.Core;
    using AO.Core.Components;
    using AO.Core.Events;

    using Cell.Core;

    using ComponentAce.Compression.Libs.zlib;

    using SmokeLounge.AOtomation.Messaging.Messages;

    using ZoneEngine.Misc;
    using ZoneEngine.Packets;

    using Config = AO.Core.Config.ConfigReadWrite;
    using Header = SmokeLounge.AOtomation.Messaging.Messages.Header;
    using N3Message = ZoneEngine.PacketHandlers.N3Message;
    using Timer = System.Timers.Timer;

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
            var stopLogout = new PacketWriter();

            // start packet header
            stopLogout.PushByte(0xDF);
            stopLogout.PushByte(0xDF);
            stopLogout.PushShort(10);
            stopLogout.PushShort(1);
            stopLogout.PushShort(0);
            stopLogout.PushInt(3086); // Sender (server ID)
            stopLogout.PushInt(this.Character.Id); // Receiver
            stopLogout.PushInt(0x5E477770); // CharacterAction packet ID
            stopLogout.PushIdentity(50000, this.Character.Id); // affected identity
            stopLogout.PushByte(0);

            // end packet header
            stopLogout.PushByte(0);
            stopLogout.PushShort(0);
            stopLogout.PushByte(0x7A); // stop logout flag?
            stopLogout.PushInt(0);
            stopLogout.PushInt(0);
            stopLogout.PushInt(0);
            stopLogout.PushInt(0);
            stopLogout.PushInt(0);
            stopLogout.PushShort(0);
            var stoplogOutPacket = stopLogout.Finish();
            this.SendCompressed(stoplogOutPacket);
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

        public void Send(int sender, int receiver, MessageBody messageBody)
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
                                              Sender = sender, 
                                              Receiver = receiver
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

        public bool SendChatText(string Text)
        {
            var _writer = new PacketWriter();

            _writer.PushByte(0xDF);
            _writer.PushByte(0xDF);
            _writer.PushShort(10);
            _writer.PushShort(1);
            _writer.PushShort(0);
            _writer.PushInt(3086);
            _writer.PushInt(this.Character.Id);
            _writer.PushInt(0x5F4B442A);
            _writer.PushIdentity(50000, this.Character.Id);
            _writer.PushByte(0);
            _writer.PushShort((short)Text.Length);
            _writer.PushBytes(Encoding.ASCII.GetBytes(Text));
            _writer.PushShort(0x1000);
            _writer.PushInt(0);
            var reply = _writer.Finish();
            this.SendCompressed(reply);
            return true;
        }

        public void SendCompressed(int sender, int receiver, MessageBody messageBody)
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
                                              Sender = sender, 
                                              Receiver = receiver
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
            var _writer = new PacketWriter();
            _writer.PushByte(0xDF);
            _writer.PushByte(0xDF);
            _writer.PushShort(10); /* Packet type */
            _writer.PushShort(1); /* ? */
            _writer.PushShort(0); /* Packet size (0 for now, PacketWriter takes care of it)*/
            _writer.PushInt(3086); /* Sender (our server ID)*/
            _writer.PushInt(this.Character.Id); /* Receiver */
            _writer.PushInt(0x50544d19); /* Packet ID */
            _writer.PushIdentity(50000, this.Character.Id); /* Affected identity */
            _writer.PushByte(1); /* ? */
            _writer.PushInt(0); /* ? */
            _writer.PushInt(MsgCategory); /* Message category ID */
            _writer.PushInt(MsgNum); /* message ID */
            var reply = _writer.Finish();
            this.SendCompressed(reply);
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
            var writer = new PacketWriter();

            // header starts
            writer.PushByte(0xDF);
            writer.PushByte(0xDF);
            writer.PushShort(10);
            writer.PushShort(1);
            writer.PushShort(0);
            writer.PushInt(3086);
            writer.PushInt(this.Character.Id);
            writer.PushInt(0x43197D22);
            writer.PushIdentity(50000, this.Character.Id);
            writer.PushByte(0);

            // Header ends
            writer.PushCoord(destination);
            writer.PushQuat(heading);
            writer.PushByte(97);
            writer.PushIdentity(51100, playfield);
            writer.PushInt(0);
            writer.PushInt(0);
            if (playfield != this.Character.PlayField)
            {
            writer.PushIdentity(40016, playfield);
            }
            else
            {
                writer.PushIdentity(0, 0);
            }
            writer.PushInt(0);
            writer.PushInt(0);
            writer.PushIdentity(100001, playfield);
            writer.PushInt(0);
            writer.PushInt(0);
            writer.PushInt(0);
            var tpreply = writer.Finish();
            Despawn.DespawnPacket(this.Character.Id);
            this.SendCompressed(tpreply);
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

            IPAddress tempIP;
            if (IPAddress.TryParse(Config.Instance.CurrentConfig.ZoneIP, out tempIP) == false)
            {
                var zoneHost = Dns.GetHostEntry(Config.Instance.CurrentConfig.ZoneIP);
                foreach (var ip in zoneHost.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        tempIP = ip;
                        break;
                    }
                }
            }

            var zoneIP = IPAddress.HostToNetworkOrder(BitConverter.ToInt32(tempIP.GetAddressBytes(), 0));
            var zonePort = Convert.ToInt16(Config.Instance.CurrentConfig.ZonePort);

            Thread.Sleep(1000);
            var writer2 = new PacketWriter();
            writer2.PushByte(0xDF);
            writer2.PushByte(0xDF);
            writer2.PushShort(1);
            writer2.PushShort(1);
            writer2.PushShort(0);
            writer2.PushInt(3086);
            writer2.PushInt(this.Character.Id);
            writer2.PushInt(60);
            writer2.PushInt(zoneIP);
            writer2.PushShort(zonePort);
            var connect = writer2.Finish();
            this.SendCompressed(connect);
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
            var writer = new PacketWriter();

            // header starts
            writer.PushByte(0xDF);
            writer.PushByte(0xDF);
            writer.PushShort(10);
            writer.PushShort(1);
            writer.PushShort(0);
            writer.PushInt(3086);
            writer.PushInt(this.Character.Id);
            writer.PushInt(0x43197D22);
            writer.PushIdentity(50000, this.Character.Id);
            writer.PushByte(0);

            // Header ends
            writer.PushCoord(this.Character.RawCoord);
            writer.PushQuat(this.Character.RawHeading);
            writer.PushByte(97);
            writer.PushIdentity(pfinstance.Type, pfinstance.Instance);
            writer.PushInt(GS);
            writer.PushInt(SG);
            writer.PushIdentity(40016, playfield);

            // Dont know for sure if its correct to only transfer the playfield here
            writer.PushInt(0);
            writer.PushInt(0);
            writer.PushIdentity(dest.Type, dest.Instance);
            writer.PushInt(0);
            var tpreply = writer.Finish();
            this.Character.DoNotDoTimers = true;
            Despawn.DespawnPacket(this.Character.Id);
            this.SendCompressed(tpreply);
            this.Character.DoNotDoTimers = true;
            this.Character.Stats.LastConcretePlayfieldInstance.Value = this.Character.PlayField;
            this.Character.Stats.ExtenalDoorInstance.Value = SG;

            this.Character.StopMovement();
            this.Character.RawCoord = destination;
            this.Character.RawHeading = heading;
            this.Character.PlayField = playfield;
            this.Character.Resource = 0x3c000;
            this.Character.Purge(); // Purge character information to DB before client reconnect

            IPAddress tempIP;
            if (IPAddress.TryParse(Config.Instance.CurrentConfig.ZoneIP, out tempIP) == false)
            {
                var zoneHost = Dns.GetHostEntry(Config.Instance.CurrentConfig.ZoneIP);
                foreach (var ip in zoneHost.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        tempIP = ip;
                        break;
                    }
                }
            }

            var zoneIP = IPAddress.HostToNetworkOrder(BitConverter.ToInt32(tempIP.GetAddressBytes(), 0));
            var zonePort = Convert.ToInt16(Config.Instance.CurrentConfig.ZonePort);

            Thread.Sleep(1000);

            var writer2 = new PacketWriter();
            writer2.PushByte(0xDF);
            writer2.PushByte(0xDF);
            writer2.PushShort(1);
            writer2.PushShort(1);
            writer2.PushShort(0);
            writer2.PushInt(3086);
            writer2.PushInt(this.Character.Id);
            writer2.PushInt(60);
            writer2.PushInt(zoneIP);
            writer2.PushShort(zonePort);
            var connect = writer2.Finish();
            this.SendCompressed(connect);
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

            if (message != null)
            {
                this.bus.Publish(new MessageReceivedEvent(this, message));
                return;
            }

            var reader = new PacketReader(packet);

            // TODO: make check here to see if packet starts with 0xDFDF
            reader.ReadBytes(2);

            // Packet type
            var type = reader.PopShort();

            // Unknown?
            reader.PopShort();

            // Packet Length
            reader.PopShort();

            // Sender
            reader.PopInt();

            // Receiver
            reader.PopInt();

            // PacketID
            var id = reader.PopInt();

            switch (type)
            {
                case 0x0A:
                    {
                        // N3Message
                        N3Message.Parse(this, packet, id);
                        break;
        }

                case 0x0B:
                    {
                        // PingMessage
                        break;
                    }

                case 0x0E:
        {
                        // OperatorMessage
                        break;
                    }

                default:
                    {
                        var messageNumber = this.GetMessageNumber(packet);
                        this.Server.Warning(
                            this, 
                            "Client sent unknown message {0}", 
                            messageNumber.ToString(CultureInfo.InvariantCulture));
                        break;
                    }
            }

            reader.Finish();
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