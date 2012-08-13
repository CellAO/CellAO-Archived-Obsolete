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

namespace ZoneEngine.Packets
{
    using AO.Core;

    internal class Unequip
    {
        public static void Send(Client cli, AOItem it, int page, int placement, bool fromsocial)
        {
            // tell the client to remove (07) the item modifiers (AC, skills and so on)
            PacketWriter unequippacket = new PacketWriter();
            PacketWriter action97 = new PacketWriter();

            //if (placement == 6)
            switch (placement)
            {
                case 6: // Right Hand
                    // Action 97
                    action97.PushByte(0xdf);
                    action97.PushByte(0xdf);
                    action97.PushShort(0xa);
                    action97.PushShort(0x1);
                    action97.PushShort(0);
                    action97.PushInt(3086);
                    action97.PushInt(cli.Character.ID);
                    action97.PushInt(0x5E477770);
                    action97.PushInt(50000);
                    action97.PushInt(cli.Character.ID);
                    action97.PushByte(0);
                    action97.PushInt(0x61);
                    action97.PushInt(0);
                    action97.PushInt(0);
                    action97.PushInt(0);
                    action97.PushInt(0);
                    action97.PushInt(0x6);
                    action97.PushShort(0);

                    byte[] ActionReply = action97.Finish();
                    cli.SendCompressed(ActionReply);

                    break;

                default:
                    unequippacket.PushByte(0xdf);
                    unequippacket.PushByte(0xdf);
                    unequippacket.PushShort(0xa);
                    unequippacket.PushShort(0x1);
                    unequippacket.PushShort(0);
                    unequippacket.PushInt(3086);
                    unequippacket.PushInt(cli.Character.ID);
                    unequippacket.PushInt(0x35505644);
                    unequippacket.PushInt(50000);
                    unequippacket.PushInt(cli.Character.ID);
                    unequippacket.PushByte(0);
                    unequippacket.PushInt(it.lowID);
                    unequippacket.PushInt(it.highID);
                    unequippacket.PushInt(it.Quality);
                    unequippacket.PushInt(1);
                    unequippacket.PushInt(7);
                    unequippacket.PushInt(page);
                    unequippacket.PushInt(placement);
                    unequippacket.PushInt(0);
                    unequippacket.PushInt(0);

                    byte[] reply = unequippacket.Finish();
                    cli.SendCompressed(reply);
                    break;
            }
        }
    }
}