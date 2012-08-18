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

    public static class ShopInventory
    {
        public static void Send(Client client, VendingMachine vendingMachine)
        {
            PacketWriter packetWriter = new PacketWriter();

            packetWriter.PushByte(0xdf);
            packetWriter.PushByte(0xdf);
            packetWriter.PushShort(0xa);
            packetWriter.PushShort(1);
            packetWriter.PushShort(0);
            packetWriter.PushInt(3086);
            packetWriter.PushInt(client.Character.ID);
            packetWriter.PushInt(0x58362220);
            packetWriter.PushIdentity(vendingMachine.Type, vendingMachine.ID);
            packetWriter.PushByte(1);
            packetWriter.Push3F1Count(vendingMachine.Inventory.Count);
            foreach (InventoryEntries ie in vendingMachine.Inventory)
            {
                packetWriter.PushInt(ie.Item.LowID);
                packetWriter.PushInt(ie.Item.HighID);
                packetWriter.PushInt(ie.Item.Quality);
            }
            byte[] packet = packetWriter.Finish();
            client.SendCompressed(packet);
        }
    }
}