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
        public static void Read(ref byte[] packet, Client client, Dynel dyn)
        {
            PacketWriter _writer = new PacketWriter();
            PacketReader _reader = new PacketReader(ref packet);

            Header header = _reader.PopHeader();
            byte unknown1 = _reader.PopByte();
            byte unknown2 = _reader.PopByte();
            Identity tofollow = _reader.PopIdentity();
            int unknown3 = _reader.PopInt();
            int unknown4 = _reader.PopInt();
            int unknown5 = _reader.PopInt();
            int unknown6 = _reader.PopInt();
            byte unknown7 = _reader.PopByte();
            _reader.Finish();

            /* start of packet */
            _writer.PushByte(0xDF);
            _writer.PushByte(0xDF);
            /* packet type */
            _writer.PushShort(10);
            /* unknown */
            _writer.PushShort(1);
            /* packet length (writer takes care of this) */
            _writer.PushShort(0);
            /* server ID */
            _writer.PushInt(3086);
            // Announcer takes care of ID's
            _writer.PushInt(0);
            /* packet ID */
            _writer.PushInt(0x260f3671);
            /* affected dynel identity */
            _writer.PushIdentity(50000, client.Character.ID);
            /* ? */
            _writer.PushByte(0);
            /* movement type */
            _writer.PushByte(unknown2);
            // Target's Identity
            _writer.PushIdentity(tofollow);
            _writer.PushInt(unknown3);
            _writer.PushInt(unknown4);
            _writer.PushInt(unknown5);
            _writer.PushInt(unknown6);
            _writer.PushByte(0);
            _writer.PushByte(0);

            byte[] reply = _writer.Finish();
            Announce.Playfield(client.Character.PlayField, ref reply);
        }
    }
}