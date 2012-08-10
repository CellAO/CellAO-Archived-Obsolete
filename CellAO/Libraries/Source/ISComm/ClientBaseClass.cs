#region License
/*
Copyright (c) 2005-2012, CellAO Team

All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion

#region Usings...
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
#endregion

namespace ISComm
{
    public class ClientBaseClass
    {
        /// <summary>
        /// Send a packet to the other end
        /// </summary>
        /// <param name="ID">Packet identifier</param>
        /// <param name="Data">Byte array with Payload.</param>
        public void SendMessage(byte ID, byte[] Data)
        {
            byte[] Packet = new byte[3 + Data.Length];
            byte[] PacketLength = new byte[2];
            PacketLength = System.BitConverter.GetBytes(Data.Length);
            Packet[0] = PacketLength[0];
            Packet[1] = PacketLength[1];
            Packet[2] = ID;
            Data.CopyTo(Packet, 3);

            Sock.Send(Packet);
        }


        private void Loop()
        {
            while (true)
            {
                try
                {
                    byte[] DataLength = new byte[2];
                    Sock.Receive(DataLength);
                    short Length = System.BitConverter.ToInt16(DataLength, 0);

                    byte[] PacketIdentifier = new byte[1];
                    Sock.Receive(PacketIdentifier);

                    byte[] Packet = new byte[Length];
                    Sock.Receive(Packet);

                    if (PacketIdentifier[0] == 0xFF)
                    {
                        //get some shit
                        switch (Packet[0])
                        {
                            case 1:
                                //handshake
                                Handshake();
                                break;
                            default: throw new Exception("Got protocol ID, but is malformed.");
                        }
                    }
                    else
                    {

                        ISComm.EventArgs.OnMessageArgs args = new ISComm.EventArgs.OnMessageArgs();
                        args.ID = PacketIdentifier[0];
                        if (args.ID == 0xFF)
                        {
                            args.IsProtocolPacket = true;
                        }
                        else
                        {
                            args.IsProtocolPacket = false;
                        }
                        args.Data = Packet;
                        args.Length = (short)(3 + Packet.Length);

                        //now call event handler
                        OnMessage(this, args);
                    }
                }
                catch (ObjectDisposedException)
                {
                    return;
                }
                catch (SocketException)
                {
                    return;
                }
            }
        }

        public ClientBaseClass(string Host, int Port)
        {
            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            Sock.Connect(Host, Port);

            Thread t = new Thread(Loop);
            t.Start();
        }

        public void Handshake()
        {
            //nothing here yet.
        }

        internal Socket Sock;
        public delegate void OnMessageHandler(object s, EventArgs.OnMessageArgs a);
        public event OnMessageHandler OnMessage;        
    }
}
