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

namespace ZoneEngine.PacketHandlers
{
    using AO.Core;

    using ZoneEngine.Misc;

    public static class FollowTarget
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="client"></param>
        public static void Read(byte[] packet, Client client, Dynel dynel)
        {
            PacketWriter packetWriter = new PacketWriter();
            PacketReader packetReader = new PacketReader(packet);

            Header header = packetReader.PopHeader();
            byte unknown1 = packetReader.PopByte();
            byte unknown2 = packetReader.PopByte();
            Identity tofollow = packetReader.PopIdentity();
            int unknown3 = packetReader.PopInt();
            int unknown4 = packetReader.PopInt();
            int unknown5 = packetReader.PopInt();
            int unknown6 = packetReader.PopInt();
            byte unknown7 = packetReader.PopByte();
            packetReader.Finish();

            /* start of packet */
            packetWriter.PushByte(0xDF);
            packetWriter.PushByte(0xDF);
            /* packet type */
            packetWriter.PushShort(10);
            /* unknown */
            packetWriter.PushShort(1);
            /* packet length (writer takes care of this) */
            packetWriter.PushShort(0);
            /* server ID */
            packetWriter.PushInt(3086);
            // Announcer takes care of ID's
            packetWriter.PushInt(0);
            /* packet ID */
            packetWriter.PushInt(0x260f3671);
            /* affected dynel identity */
            packetWriter.PushIdentity(50000, client.Character.Id);
            /* ? */
            packetWriter.PushByte(0);
            /* movement type */
            packetWriter.PushByte(unknown2);
            // Target's Identity
            packetWriter.PushIdentity(tofollow);
            packetWriter.PushInt(unknown3);
            packetWriter.PushInt(unknown4);
            packetWriter.PushInt(unknown5);
            packetWriter.PushInt(unknown6);
            packetWriter.PushByte(0);
            packetWriter.PushByte(0);

            byte[] reply = packetWriter.Finish();
            Announce.Playfield(client.Character.PlayField, reply);
        }
    }
}