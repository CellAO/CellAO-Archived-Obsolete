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

#region Usings...

#endregion

namespace ZoneEngine
{
    using System;
    using System.IO;
    using System.Threading;

    public static class ChatCom
    {
        /* Msg types
         * 0: Say
         * 1: Whisper
         * 2: Shout
         */

        public static void SendVicinity(UInt32 sender, byte msgType, UInt32[] receivers, string msg)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(sender);
            writer.Write(msgType);
            writer.Write((short)(receivers.Length));

            for (int i = 0; i < receivers.Length; i++)
            {
                writer.Write(receivers[i]);
            }
            writer.Write(msg);
            byte[] toSend = stream.GetBuffer();
            writer.Close();
            stream.Dispose();

            Server.SendMessage(0x0F, toSend);
        }

        public static void StartLink(int Port)
        {
            port = Port;
            Thread t = new Thread(LinkThread);
            t.Start();
        }

        public static void LinkThread()
        {
            Server = new ISCServer(port);
            // Prevent the thread from running out
            while (true)
            {
                Thread.Sleep(10000);
            }
        }

        private static int port;

        public static ISCServer Server;
    }
}