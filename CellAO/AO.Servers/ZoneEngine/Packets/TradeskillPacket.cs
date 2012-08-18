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

    public static class TradeskillPacket
    {
        public static void SendResult(Character character, int min, int max, int low, int high)
        {
            PacketWriter packetWriter = new PacketWriter();

            packetWriter.PushByte(0xdf);
            packetWriter.PushByte(0xdf);
            packetWriter.PushShort(0xa);
            packetWriter.PushShort(1);
            packetWriter.PushShort(0);
            packetWriter.PushInt(3086);
            packetWriter.PushInt(character.Id);
            packetWriter.PushInt(0x5E477770);
            packetWriter.PushIdentity(50000, character.Id);
            packetWriter.PushByte(0);
            packetWriter.PushInt(0xE4);
            packetWriter.PushInt(0);
            packetWriter.PushInt(max);
            packetWriter.PushInt(high);
            packetWriter.PushInt(min);
            packetWriter.PushInt(low);
            packetWriter.PushByte(0);
            packetWriter.PushByte(0);
            byte[] pack = packetWriter.Finish();
            character.Client.SendCompressed(pack);
        }

        public static void SendRequirement(Character character, TradeSkillSkillInfo tradeSkillSkillInfo)
        {
            PacketWriter packetWriter = new PacketWriter();

            packetWriter.PushByte(0xdf);
            packetWriter.PushByte(0xdf);
            packetWriter.PushShort(0xa);
            packetWriter.PushShort(1);
            packetWriter.PushShort(0);
            packetWriter.PushInt(3086);
            packetWriter.PushInt(character.Id);
            packetWriter.PushInt(0x5E477770);
            packetWriter.PushIdentity(50000, character.Id);
            packetWriter.PushByte(0);
            packetWriter.PushInt(0xE3);
            packetWriter.PushInt(0);
            packetWriter.PushInt(0);
            packetWriter.PushInt(0);
            packetWriter.PushInt(tradeSkillSkillInfo.Skill);
            packetWriter.PushInt(tradeSkillSkillInfo.Requirement);
            packetWriter.PushByte(0);
            packetWriter.PushByte(0);
            byte[] packet = packetWriter.Finish();
            character.Client.SendCompressed(packet);
        }

        public static void SendSource(Character character, int count)
        {
            PacketWriter packetWriter = new PacketWriter();

            packetWriter.PushByte(0xdf);
            packetWriter.PushByte(0xdf);
            packetWriter.PushShort(0xa);
            packetWriter.PushShort(1);
            packetWriter.PushShort(0);
            packetWriter.PushInt(3086);
            packetWriter.PushInt(character.Id);
            packetWriter.PushInt(0x5E477770);
            packetWriter.PushIdentity(50000, character.Id);
            packetWriter.PushByte(0);
            packetWriter.PushInt(0xdf);
            packetWriter.PushInt(0);
            packetWriter.PushInt(0);
            packetWriter.PushInt(0);
            packetWriter.PushInt(0);
            packetWriter.PushInt(count);
            packetWriter.PushByte(0);
            packetWriter.PushByte(0);
            byte[] packet = packetWriter.Finish();
            character.Client.SendCompressed(packet);
        }

        public static void SendTarget(Character character, int count)
        {
            PacketWriter packetWriter = new PacketWriter();

            packetWriter.PushByte(0xdf);
            packetWriter.PushByte(0xdf);
            packetWriter.PushShort(0xa);
            packetWriter.PushShort(1);
            packetWriter.PushShort(0);
            packetWriter.PushInt(3086);
            packetWriter.PushInt(character.Id);
            packetWriter.PushInt(0x5E477770);
            packetWriter.PushIdentity(50000, character.Id);
            packetWriter.PushByte(0);
            packetWriter.PushInt(0xE0);
            packetWriter.PushInt(0);
            packetWriter.PushInt(0);
            packetWriter.PushInt(0);
            packetWriter.PushInt(0);
            packetWriter.PushInt(count);
            packetWriter.PushByte(0);
            packetWriter.PushByte(0);
            byte[] packet = packetWriter.Finish();
            character.Client.SendCompressed(packet);
        }

        public static void SendOutOfRange(Character character, int min)
        {
            PacketWriter packetWriter = new PacketWriter();

            packetWriter.PushByte(0xdf);
            packetWriter.PushByte(0xdf);
            packetWriter.PushShort(0xa);
            packetWriter.PushShort(1);
            packetWriter.PushShort(0);
            packetWriter.PushInt(3086);
            packetWriter.PushInt(character.Id);
            packetWriter.PushInt(0x5E477770);
            packetWriter.PushIdentity(50000, character.Id);
            packetWriter.PushByte(0);
            packetWriter.PushInt(0xE2);
            packetWriter.PushInt(0);
            packetWriter.PushInt(0);
            packetWriter.PushInt(0);
            packetWriter.PushInt(0);
            packetWriter.PushInt(min);
            packetWriter.PushByte(0);
            packetWriter.PushByte(0);
            byte[] packet = packetWriter.Finish();
            character.Client.SendCompressed(packet);
        }

        public static void SendNotTradeskill(Character character)
        {
            PacketWriter packetWriter = new PacketWriter();

            packetWriter.PushByte(0xdf);
            packetWriter.PushByte(0xdf);
            packetWriter.PushShort(0xa);
            packetWriter.PushShort(1);
            packetWriter.PushShort(0);
            packetWriter.PushInt(3086);
            packetWriter.PushInt(character.Id);
            packetWriter.PushInt(0x5E477770);
            packetWriter.PushIdentity(50000, character.Id);
            packetWriter.PushByte(0);
            packetWriter.PushInt(0xE1);
            packetWriter.PushInt(0);
            packetWriter.PushInt(0);
            packetWriter.PushInt(0);
            packetWriter.PushInt(0);
            packetWriter.PushInt(0);
            packetWriter.PushByte(0);
            packetWriter.PushByte(0);
            byte[] packet = packetWriter.Finish();
            character.Client.SendCompressed(packet);
        }
    }
}