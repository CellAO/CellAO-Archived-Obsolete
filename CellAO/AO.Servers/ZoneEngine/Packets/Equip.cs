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

    internal class Equip
    {
        public static void Send(Client cli, AOItem it, int page, int placement)
        {
            PacketWriter equippacket = new PacketWriter();
            PacketWriter action167packet = new PacketWriter();
            PacketWriter action131packet = new PacketWriter();

            //if (placement == 6)
            switch (placement)
            {
                case 6: // Right Hand
                    /*
                    // ContainerAddItem Reply
                    equippacket.PushByte(0xdf);
                    equippacket.PushByte(0xdf);
                    equippacket.PushShort(0xa);
                    equippacket.PushShort(0x1);
                    equippacket.PushShort(0);
                    equippacket.PushInt(3086);
                    equippacket.PushInt(cli.Character.ID);
                    equippacket.PushInt(0x47537A24);
                    equippacket.PushInt(50000);
                    equippacket.PushInt(cli.Character.ID);
                    equippacket.PushByte(0);
                    equippacket.PushInt(0x60);
                    equippacket.PushInt(0x40);
                    equippacket.PushInt(50000);
                    equippacket.PushInt(cli.Character.ID);
                    equippacket.PushInt(0x6);
                    
                    byte[] reply = equippacket.Finish();
                    cli.SendCompressed(reply);
                    */
                    // Action 167 Reply
                    action167packet.PushByte(0xDF);
                    action167packet.PushByte(0xDF);
                    action167packet.PushShort(0xA);
                    action167packet.PushShort(0x1);
                    action167packet.PushShort(0);
                    action167packet.PushInt(3086);
                    action167packet.PushInt(cli.Character.ID);
                    action167packet.PushInt(0x5E477770);
                    action167packet.PushInt(50000);
                    action167packet.PushInt(cli.Character.ID);
                    action167packet.PushByte(0);
                    action167packet.PushInt(0xA7);
                    action167packet.PushInt(0);
                    action167packet.PushInt(0);
                    action167packet.PushInt(0);
                    action167packet.PushInt(0);
                    action167packet.PushInt(0);
                    action167packet.PushShort(0);

                    byte[] replyAction167 = action167packet.Finish();
                    cli.SendCompressed(replyAction167);

                    // Action 131 Reply
                    action131packet.PushByte(0xDF);
                    action131packet.PushByte(0xDF);
                    action131packet.PushShort(0xA);
                    action131packet.PushShort(0x1);
                    action131packet.PushShort(0);
                    action131packet.PushInt(3086);
                    action131packet.PushInt(cli.Character.ID);
                    action131packet.PushInt(0x5E477770);
                    action131packet.PushInt(50000);
                    action131packet.PushInt(cli.Character.ID);
                    action131packet.PushByte(0);
                    action131packet.PushInt(0x83);
                    action131packet.PushInt(0);
                    action131packet.PushInt(0xC74A);
                    action131packet.PushInt(0x4598815B);
                    action131packet.PushInt(0);
                    action131packet.PushInt(0x06);
                    action131packet.PushShort(0);

                    byte[] replyAction131 = action131packet.Finish();
                    cli.SendCompressed(replyAction131);

                    break;

                default:
                    // Default Template Action
                    equippacket.PushByte(0xdf);
                    equippacket.PushByte(0xdf);
                    equippacket.PushShort(0xa);
                    equippacket.PushShort(0x1);
                    equippacket.PushShort(0);
                    equippacket.PushInt(3086);
                    equippacket.PushInt(cli.Character.ID);
                    equippacket.PushInt(0x35505644);
                    equippacket.PushInt(50000);
                    equippacket.PushInt(cli.Character.ID);
                    equippacket.PushByte(0);
                    equippacket.PushInt(it.lowID);
                    equippacket.PushInt(it.highID);
                    equippacket.PushInt(it.Quality);
                    equippacket.PushInt(1);
                    if ((placement >= 49) && (placement <= 63))
                    {
                        equippacket.PushInt(7);
                    }
                    else
                    {
                        equippacket.PushInt(6);
                    }
                    equippacket.PushInt(page);
                    equippacket.PushInt(placement);
                    equippacket.PushInt(0);
                    equippacket.PushInt(0);

                    byte[] defaultReply = equippacket.Finish();
                    if (!((placement >= 49) && (placement <= 63)))
                    {
                        cli.SendCompressed(defaultReply);
                    }
                    break;
            }
        }
    }
}