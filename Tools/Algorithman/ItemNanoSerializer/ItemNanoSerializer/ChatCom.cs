#region License
/*
Copyright (c) 2005-2011, CellAO Team

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
using AO.Core;
using Cell.Core;
using System.IO;
#endregion

namespace ZoneEngine
{
    public static class ChatCom
    {
        public static void SendVicinity(int[] Receivers, string msg)
        {
            //make changes to protocol later (TODO!)
           /* PacketWriter _writer = new PacketWriter();
            _writer.PushInt(charID);
            _writer.PushShort((short)msg.Length);
            _writer.PushBytes(Encoding.ASCII.GetBytes(msg));
            byte[] Tosend = _writer.Finish();*/

            /*byte[] PayLoad = new byte[(sizeof(int) * Receivers.Length) + msg.Length + sizeof(short)];
            //lo-hi
            PayLoad[0] = (byte)((Receivers.Length) & 0xFF);
            PayLoad[1] = (byte)((Receivers.Length) << 8);
            int idx = 2;
            foreach (int r in Receivers)
            {

            }*/

            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write((short)(Receivers.Length));
            int c;
            for (c=0;c<Receivers.Length;c++)
            {
                writer.Write(Receivers[c]);
            }
            //writer.Write((short)msg.Length);
            writer.Write(msg);

            byte[] ToSend = stream.GetBuffer();
            writer.Close();
            stream.Dispose();

            Server.SendMessage(0x0F, ToSend);
        }

        public static void StartLink(int Port)
        {
            port = Port;
            System.Threading.Thread t = new System.Threading.Thread(LinkThread);
            t.Start();
        }

        public static void LinkThread()
        {
            Server = new ISCServer(port);
            // Prevent the thread from running out
            while (true)
            {
                System.Threading.Thread.Sleep(10000);
            }
        }

        private static int port;
        public static ISCServer Server;
    }
}
