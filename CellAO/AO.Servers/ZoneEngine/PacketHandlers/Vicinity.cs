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
using AO.Core;
using ZoneEngine.Misc;
#endregion

namespace ZoneEngine.PacketHandlers
{
    public static class Vicinity
    {
        public static void Read(ref byte[] packet, Client cli)
        {
            PacketReader reader = new PacketReader(ref packet);
            reader.PopShort(); //magic 0xDFDF
            short pktType = reader.PopShort();
            if (pktType != 0x0005)
            {
                //TextMessage type
                throw new Exception("Wrong packet type given to VicinityHandler.");
            }
            reader.PopShort(); //unknown
            short packetSize = reader.PopShort();
            UInt32 senderId = reader.PopUInt();
            reader.PopInt(); // Receiver
            int pktId = reader.PopInt(); // Packet ID


            // 3 unknown values
            reader.PopInt();
            reader.PopInt();
            reader.PopInt();

            short msgLen = reader.PopShort();
            string msg = reader.PopString(msgLen);
            byte msgType = reader.PopByte();

            Console.WriteLine("Vicinity: " + msg);

            float range = 0f;
            switch (msgType)
            {
                case 0:
                    // Say
                    range = 10.0f;
                    break;
                case 1:
                    // Whisper
                    range = 1.5f;
                    break;
                case 2:
                    // Shout
                    range = 60.0f;
                    break;
                default:
                    break;
            }

            List<Client> clients = FindClient.GetClientsInRadius(cli, range);
            UInt32[] recvers = new UInt32[clients.Count];
            int index = 0;

            foreach (Client child in clients)
            {
                recvers[index] = (UInt32) child.Character.ID;
                index++;
            }

            ChatCom.SendVicinity(senderId, msgType, recvers, msg);
        }
    }
}