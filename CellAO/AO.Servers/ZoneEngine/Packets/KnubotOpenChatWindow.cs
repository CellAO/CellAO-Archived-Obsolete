﻿#region License
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

    using ZoneEngine.Misc;

    public static class KnuBotOpenChatWindow
    {
        public static void Send(Client client, NonPlayerCharacterClass knubotTarget)
        {
            PacketWriter packetWriter = new PacketWriter();

            packetWriter.PushByte(0xdf);
            packetWriter.PushByte(0xdf);
            packetWriter.PushShort(0xa);
            packetWriter.PushShort(1);
            packetWriter.PushShort(0);
            packetWriter.PushInt(3086);
            packetWriter.PushInt(client.Character.ID);
            packetWriter.PushInt(0x3b132d64);
            packetWriter.PushIdentity(client.Character.Type, client.Character.ID);
            packetWriter.PushByte(0);
            packetWriter.PushShort(2);
            packetWriter.PushIdentity(knubotTarget.Type, knubotTarget.ID);
            packetWriter.PushInt(1);
            packetWriter.PushInt(0);

            byte[] packet = packetWriter.Finish();

            client.SendCompressed(packet);
        }

        public static void Read(byte[] packet)
        {
            PacketReader packetReader = new PacketReader(packet);

            Header header = packetReader.PopHeader();
            packetReader.PopByte();
            packetReader.PopShort();
            int type = packetReader.PopInt();
            int instance = packetReader.PopInt();
            NonPlayerCharacterClass npc = (NonPlayerCharacterClass)FindDynel.FindDynelById(type, instance);
            Character character = FindClient.FindClientById(header.Sender).Character;
            character.KnuBotTarget = npc;
        }
    }
}