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

#region Usings

#endregion

namespace ZoneEngine.PacketHandlers
{
    using System;

    using AO.Core;

    using ZoneEngine.Misc;
    using ZoneEngine.Packets;

    public static class OnTrade
    {
        public static void Read(byte[] packet, Client client, Dynel dynel)
        {
            PacketReader reader = new PacketReader(packet);
            PacketWriter packetWriter = new PacketWriter();
            Header header = reader.PopHeader();
            reader.PopByte();
            reader.PopInt(); // unknown
            byte action = reader.PopByte(); // unknown
            Identity ident = reader.PopIdentity();
            int container = reader.PopInt();
            int place = reader.PopInt();

            Character character = (Character)FindDynel.FindDynelByID(ident.Type, ident.Instance);
            Character chaffected =
                (Character)FindDynel.FindDynelByID(header.AffectedId.Type, header.AffectedId.Instance);

            // If target is a NPC, call its Action 0
            if ((character is NonPlayerCharacterClass) && (action == 0))
            {
                if (((NonPlayerCharacterClass)character).KnuBot != null)
                {
                    character.KnuBotTarget = character;
                    ((NonPlayerCharacterClass)character).KnuBot.TalkingTo = chaffected;
                    ((NonPlayerCharacterClass)character).KnuBot.Action(0);
                }
                return;
            }

            int cashDeduct = 0;
            int inventoryCounter;
            InventoryEntries inventoryEntry;

            switch (action)
            {
                case 1: // end trade
                    inventoryCounter = client.Character.Inventory.Count - 1;
                    while (inventoryCounter >= 0)
                    {
                        inventoryEntry = client.Character.Inventory[inventoryCounter];
                        AOItem aoItem;
                        if (inventoryEntry.Container == -1)
                        {
                            int nextFree = client.Character.GetNextFreeInventory(104);
                            aoItem = ItemHandler.GetItemTemplate(inventoryEntry.Item.LowID);
                            int price = aoItem.getItemAttribute(74);
                            int mult = aoItem.getItemAttribute(212); // original multiplecount
                            if (mult == 0)
                            {
                                mult = 1;
                                inventoryEntry.Item.MultipleCount = 1;
                            }
                            // Deduct Cash (ie.item.multiplecount) div mult * price
                            cashDeduct +=
                                Convert.ToInt32(
                                    mult * price
                                    *
                                    (100
                                     - Math.Floor(Math.Min(1500, client.Character.Stats.ComputerLiteracy.Value) / 40.0))
                                    / 2500);
                            // Add the Shop modificator and exchange the CompLit for skill form vendortemplate table
                            inventoryEntry.Placement = nextFree;
                            inventoryEntry.Container = 104;
                            if (!aoItem.isStackable())
                            {
                                int multiplicator = inventoryEntry.Item.MultipleCount;
                                inventoryEntry.Item.MultipleCount = 0;
                                while (multiplicator > 0)
                                {
                                    AddTemplate.Send(client, inventoryEntry);
                                    multiplicator--;
                                }
                            }
                            else
                            {
                                AddTemplate.Send(client, inventoryEntry);
                            }
                        }
                        if (inventoryEntry.Container == -2)
                        {
                            aoItem = ItemHandler.interpolate(inventoryEntry.Item.LowID, inventoryEntry.Item.HighID, inventoryEntry.Item.Quality);
                            double multipleCount = aoItem.getItemAttribute(212); // original multiplecount
                            int price = aoItem.getItemAttribute(74);
                            if (multipleCount == 0.0)
                            {
                                multipleCount = 1.0;
                            }
                            else
                            {
                                multipleCount = inventoryEntry.Item.MultipleCount / multipleCount;
                            }
                            cashDeduct -=
                                Convert.ToInt32(
                                    multipleCount * price
                                    *
                                    (100
                                     + Math.Floor(Math.Min(1500, client.Character.Stats.ComputerLiteracy.Value) / 40.0))
                                    / 2500);
                            // Add the Shop modificator and exchange the CompLit for skill form vendortemplate table
                            client.Character.Inventory.Remove(inventoryEntry);
                        }
                        inventoryCounter--;
                    }

                    client.Character.Stats.Cash.Set((uint)(client.Character.Stats.Cash.Value - cashDeduct));
                    //                    Packets.Stat.Set(client, 61, client.Character.Stats.Cash.StatValue - cashdeduct, false);
                    byte[] reply0 = new byte[32];
                    Array.Copy(packet, reply0, 32);

                    // pushing in server ID
                    reply0[8] = 0;
                    reply0[9] = 0;
                    reply0[10] = 12;
                    reply0[11] = 14;

                    // pushing in Client ID
                    reply0[12] = (byte)(client.Character.ID >> 24);
                    reply0[13] = (byte)(client.Character.ID >> 16);
                    reply0[14] = (byte)(client.Character.ID >> 8);
                    reply0[15] = (byte)(client.Character.ID);

                    packetWriter.PushBytes(reply0);
                    packetWriter.PushByte(1);
                    packetWriter.PushByte(4);
                    packetWriter.PushIdentity(client.Character.LastTrade);
                    packetWriter.PushIdentity(client.Character.LastTrade);
                    client.Character.LastTrade.Type = 0;
                    client.Character.LastTrade.Instance = 0;
                    byte[] reply2 = packetWriter.Finish();
                    client.SendCompressed(reply2);
                    break;
                case 2:
                    // Decline trade
                    inventoryCounter = client.Character.Inventory.Count - 1;
                    while (inventoryCounter >= 0)
                    {
                        inventoryEntry = client.Character.Inventory[inventoryCounter];
                        if (inventoryEntry.Container == -1)
                        {
                            client.Character.Inventory.Remove(inventoryEntry);
                        }
                        else
                        {
                            if (inventoryEntry.Container == -2)
                            {
                                inventoryEntry.Placement = client.Character.GetNextFreeInventory(104);
                                inventoryEntry.Container = 104;
                            }
                        }
                        inventoryCounter--;
                    }

                    byte[] replyCopy = new byte[50];
                    Array.Copy(packet, replyCopy, 50);

                    // pushing in server ID
                    replyCopy[8] = 0;
                    replyCopy[9] = 0;
                    replyCopy[10] = 12;
                    replyCopy[11] = 14;

                    // pushing in Client ID
                    replyCopy[12] = (byte)(client.Character.ID >> 24);
                    replyCopy[13] = (byte)(client.Character.ID >> 16);
                    replyCopy[14] = (byte)(client.Character.ID >> 8);
                    replyCopy[15] = (byte)(client.Character.ID);

                    packetWriter.PushBytes(replyCopy);
                    byte[] rep1 = packetWriter.Finish();

                    client.SendCompressed(rep1);
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5: // add item to trade window
                case 6: // remove item from trade window
                    byte[] reply = new byte[50];
                    Array.Copy(packet, reply, 50);
                    if (character.Inventory.Count == 0)
                    {
                        ((VendingMachine)character).LoadTemplate(((VendingMachine)character).TemplateID);
                    }

                    // pushing in server ID
                    reply[8] = 0;
                    reply[9] = 0;
                    reply[10] = 12;
                    reply[11] = 14;

                    // pushing in Client ID
                    reply[12] = (byte)(client.Character.ID >> 24);
                    reply[13] = (byte)(client.Character.ID >> 16);
                    reply[14] = (byte)(client.Character.ID >> 8);
                    reply[15] = (byte)(client.Character.ID);

                    //PacketWriter pw = new PacketWriter();
                    packetWriter.PushBytes(reply);
                    byte[] replyRemoveItemFromTradeWindow = packetWriter.Finish();
                    client.SendCompressed(replyRemoveItemFromTradeWindow);

                    if (client.Character == character)
                    {
                        if (action == 5)
                        {
                            inventoryEntry = character.getInventoryAt(place);
                            inventoryEntry.Placement = character.GetNextFreeInventory(-2);
                            inventoryEntry.Container = -2;
                        }
                        if (action == 6)
                        {
                            inventoryEntry = character.getInventoryAt(place, -2);
                            inventoryEntry.Placement = character.GetNextFreeInventory(104);
                            inventoryEntry.Container = 104;
                        }
                    }
                    else
                    {
                        InventoryEntries inew = new InventoryEntries
                            { Container = -1, Placement = character.GetNextFreeInventory(-1) };
                        int oldPlacement = ((packet[46] >> 24) + (packet[47] >> 16) + (packet[48] >> 8) + packet[49]);
                        InventoryEntries totrade = character.getInventoryAt(oldPlacement);
                        inew.Item.LowID = totrade.Item.LowID;
                        inew.Item.HighID = totrade.Item.HighID;
                        inew.Item.MultipleCount = totrade.Item.MultipleCount;
                        if (action == 6) // Remove item from trade window
                        {
                            inew.Item.MultipleCount = -inew.Item.MultipleCount;
                        }
                        inew.Item.Quality = totrade.Item.Quality;
                        chaffected.InventoryReplaceAdd(inew);
                    }
                    break;
            }
        }
    }
}