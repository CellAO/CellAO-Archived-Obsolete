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

    public static class SimpleItemFullUpdate
    {
        public static void UpdateItemsCache(Character character)
        {
            int currentSlot = 64;

            while (currentSlot < 109)
            {
                currentSlot++;
            }
        }

        public static void SendPlayField(Client client, VendingMachine vendingMachine, int itemNumber)
        {
            PacketWriter packetWriter = new PacketWriter();

            packetWriter.PushByte(0xdf);
            packetWriter.PushByte(0xdf);
            packetWriter.PushShort(0xa);
            packetWriter.PushShort(1);
            packetWriter.PushShort(0);
            packetWriter.PushInt(3086);
            packetWriter.PushInt(client.Character.ID);
            packetWriter.PushInt(0x3b11256f);
            packetWriter.PushIdentity(0xc76e, 0x021fa86f); // whats this one???
            packetWriter.PushByte(0);
            packetWriter.PushInt(11);
            packetWriter.PushIdentity(client.Character.Type, client.Character.ID);
            packetWriter.PushInt(client.Character.PlayField);
            packetWriter.PushInt(0x0f424f);
            packetWriter.PushInt(0);
            packetWriter.PushShort(0x656f); // ??????
            packetWriter.Push3F1Count(6);
            packetWriter.PushInt(0);
            packetWriter.PushByte(0x80);
            packetWriter.PushByte(0);
            packetWriter.PushShort(0x0203);
            packetWriter.PushInt(0x17);
            packetWriter.PushInt(vendingMachine.Inventory[itemNumber].Item.LowID); // TODO: 3 times lowID and no highID?
            packetWriter.PushInt(0x2bd);
            packetWriter.PushInt(1);
            packetWriter.PushInt(0x2be);
            packetWriter.PushInt(vendingMachine.Inventory[itemNumber].Item.LowID);
            packetWriter.PushInt(0x2bf);
            packetWriter.PushInt(vendingMachine.Inventory[itemNumber].Item.LowID);
            packetWriter.PushInt(0x19c);
            packetWriter.PushInt(1);
            packetWriter.PushInt(0);

            // TODO: Actually send the data, probably research needed
        }

        public static void SendOwner(Client client, VendingMachine vendingMachine, int itemNumber)
        {
            PacketWriter packetWriter = new PacketWriter();

            packetWriter.PushByte(0xdf);
            packetWriter.PushByte(0xdf);
            packetWriter.PushShort(0xa);
            packetWriter.PushShort(1);
            packetWriter.PushShort(0);
            packetWriter.PushInt(3086);
            packetWriter.PushInt(client.Character.ID);
            packetWriter.PushInt(0x3b11256f);
            packetWriter.PushIdentity(0xc76e, 0x021fa86f); // whats this one???
            packetWriter.PushByte(0);
            packetWriter.PushInt(11);
            packetWriter.PushIdentity(client.Character.Type, client.Character.ID);
            packetWriter.PushInt(client.Character.PlayField);
            packetWriter.PushInt(0x0f424f);
            packetWriter.PushInt(0);
            packetWriter.PushShort(0x656f); // ??????
            packetWriter.Push3F1Count(6);
            packetWriter.PushInt(0);
            packetWriter.PushByte(0x80);
            packetWriter.PushByte(0);
            packetWriter.PushShort(0x0203);
            packetWriter.PushInt(0x17);
            packetWriter.PushInt(vendingMachine.Inventory[itemNumber].Item.LowID);
            // TODO: Three times low id and no high id?
            packetWriter.PushInt(0x2bd);
            packetWriter.PushInt(1);
            packetWriter.PushInt(0x2be);
            packetWriter.PushInt(vendingMachine.Inventory[itemNumber].Item.LowID);
            packetWriter.PushInt(0x2bf);
            packetWriter.PushInt(vendingMachine.Inventory[itemNumber].Item.LowID);
            packetWriter.PushInt(0x19c);
            packetWriter.PushInt(1);
            packetWriter.PushInt(0);
            // TODO: Actually send the data, probably research needed
        }
    }
}