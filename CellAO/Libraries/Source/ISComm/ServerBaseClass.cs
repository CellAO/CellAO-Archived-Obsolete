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
using System.Net.Sockets;
using System.Net;
using System.Threading;
#endregion

namespace ISComm
{
    public class ServerBaseClass
    {
        public ServerBaseClass(int Port)
        {
            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            endPoint = new IPEndPoint(IPAddress.Any, Port);
            Sock.Bind(endPoint);

            //lol @ shitty impl.
            /*Thread linkWaiter = new Thread(WaitForLink);
            linkWaiter.Start();*/

            WaitForLink();
        }

        public void WaitForLink()
        {
            Sock.Listen(1);
            Client = Sock.Accept();
            Connection = true;
            /*System.EventArgs lol = new System.EventArgs();
            OnConnect(this, lol);*/

            Thread t = new Thread(Loop);
            t.Start();

            Handshake();
            return;
        }

        private void Loop()
        {
            while (true)
            {
                try
                {
                    byte[] DataLength = new byte[2];
                    int bytes = Client.Receive(DataLength);
                    short Length = System.BitConverter.ToInt16(DataLength, 0);

                    byte[] PacketIdentifier = new byte[1];
                    Client.Receive(PacketIdentifier);

                    byte[] Packet = new byte[Length];
                    Client.Receive(Packet);

                    if (PacketIdentifier[0] == 0xFF)
                    {
                        //get some shit
                        switch (Packet[0])
                        {
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
                    //socket has closed, call OnDisconnect function and return.
                    OnDisconnect(this, System.EventArgs.Empty);
                    Connection = false;
                    return;
                }
                catch (SocketException)
                {
                    OnDisconnect(this, System.EventArgs.Empty);
                    Connection = false;
                    return;
                }
            }
        }

        /// <summary>
        /// Sends a packet to the other end.
        /// </summary>
        /// <param name="ID">Packet identifier</param>
        /// <param name="Data">Byte array with payload.</param>
        public void SendMessage(byte ID, byte[] Data)
        {
            byte[] Packet = new byte[3 + Data.Length];
            byte[] PacketLength = new byte[2];
            PacketLength = System.BitConverter.GetBytes(Data.Length);
            Packet[0] = PacketLength[0];
            Packet[1] = PacketLength[1];
            Packet[2] = ID;
            Data.CopyTo(Packet, 3);

            Client.Send(Packet);
        
        }

        public void Handshake()
        {
            //packet id FF (protocol opers)
            byte[] data = new byte[2];
            data[0] = 0x01; //operation handshake
            data[1] = 0x00;

            SendMessage(0xFF, data);
        }

        internal Socket Client;
        internal Socket Sock;
        internal IPEndPoint endPoint;

        public bool Connection;

        public delegate void OnMessageHandler(object s, EventArgs.OnMessageArgs a);
        public event OnMessageHandler OnMessage;
        public delegate void OnDisconnectHandler(object s, System.EventArgs a);
        public event OnDisconnectHandler OnDisconnect;
        public delegate void OnConnectHandler(object s, System.EventArgs a);
        public event OnConnectHandler OnConnect;
    }
}
