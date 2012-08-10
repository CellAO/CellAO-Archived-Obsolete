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
using AO.Core;
#endregion

namespace ZoneEngine.Packets
{
    public class GenericCmd
    {
        public static void Send(Character ch, InventoryEntries ie)
        {
            PacketWriter packet = new PacketWriter();

            packet.PushByte(0xdf);
            packet.PushByte(0xdf);
            packet.PushShort(0xa);
            packet.PushShort(1);
            packet.PushShort(0);
            packet.PushInt(3086);
            packet.PushInt(ch.ID);
            packet.PushInt(0x52526858);
            packet.PushInt(50000);
            packet.PushInt(ch.ID);
            packet.PushByte(0);
            packet.PushInt(1);
            packet.PushInt(3);
            packet.PushInt(3);
            packet.PushInt(0);
            packet.PushInt(50000);
            packet.PushInt(ch.ID);
            packet.PushInt(ie.Container);
            packet.PushInt(ie.Placement);
            byte[] pack = packet.Finish();
            ch.client.SendCompressed(pack);
        }
    }
}