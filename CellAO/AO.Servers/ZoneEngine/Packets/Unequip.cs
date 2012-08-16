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

namespace ZoneEngine.Packets
{
    using AO.Core;

    internal class Unequip
    {
        public static void Send(Client client, AOItem item, int page, int placement, bool fromSocial)
        {
            // tell the client to remove (07) the item modifiers (AC, skills and so on)
            PacketWriter unequipPacketWriter = new PacketWriter();
            PacketWriter action97PacketWriter = new PacketWriter();

            //if (placement == 6)
            switch (placement)
            {
                case 6: // Right Hand
                    // Action 97
                    action97PacketWriter.PushByte(0xdf);
                    action97PacketWriter.PushByte(0xdf);
                    action97PacketWriter.PushShort(0xa);
                    action97PacketWriter.PushShort(0x1);
                    action97PacketWriter.PushShort(0);
                    action97PacketWriter.PushInt(3086);
                    action97PacketWriter.PushInt(client.Character.ID);
                    action97PacketWriter.PushInt(0x5E477770);
                    action97PacketWriter.PushInt(50000);
                    action97PacketWriter.PushInt(client.Character.ID);
                    action97PacketWriter.PushByte(0);
                    action97PacketWriter.PushInt(0x61);
                    action97PacketWriter.PushInt(0);
                    action97PacketWriter.PushInt(0);
                    action97PacketWriter.PushInt(0);
                    action97PacketWriter.PushInt(0);
                    action97PacketWriter.PushInt(0x6);
                    action97PacketWriter.PushShort(0);

                    byte[] action97Reply = action97PacketWriter.Finish();
                    client.SendCompressed(action97Reply);

                    break;

                default:
                    unequipPacketWriter.PushByte(0xdf);
                    unequipPacketWriter.PushByte(0xdf);
                    unequipPacketWriter.PushShort(0xa);
                    unequipPacketWriter.PushShort(0x1);
                    unequipPacketWriter.PushShort(0);
                    unequipPacketWriter.PushInt(3086);
                    unequipPacketWriter.PushInt(client.Character.ID);
                    unequipPacketWriter.PushInt(0x35505644);
                    unequipPacketWriter.PushInt(50000);
                    unequipPacketWriter.PushInt(client.Character.ID);
                    unequipPacketWriter.PushByte(0);
                    unequipPacketWriter.PushInt(item.LowID);
                    unequipPacketWriter.PushInt(item.HighID);
                    unequipPacketWriter.PushInt(item.Quality);
                    unequipPacketWriter.PushInt(1);
                    unequipPacketWriter.PushInt(7);
                    unequipPacketWriter.PushInt(page);
                    unequipPacketWriter.PushInt(placement);
                    unequipPacketWriter.PushInt(0);
                    unequipPacketWriter.PushInt(0);

                    byte[] unequipReply = unequipPacketWriter.Finish();
                    client.SendCompressed(unequipReply);
                    break;
            }
        }
    }
}