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
        public static void read(ref byte[] packet, Client client, Dynel dyn)
        {
            PacketReader reader = new PacketReader(ref packet);
            PacketWriter pw = new PacketWriter();
            Header _header = reader.PopHeader();
            reader.PopByte();
            reader.PopInt(); // unknown
            byte action = reader.PopByte(); // unknown
            Identity ident = reader.PopIdentity();
            int _container = reader.PopInt();
            int _place = reader.PopInt();

            Character ch = (Character)FindDynel.FindDynelByID(ident.Type, ident.Instance);
            Character chaffected =
                (Character)FindDynel.FindDynelByID(_header.AffectedId.Type, _header.AffectedId.Instance);

            // If target is a NPC, call its Action 0
            if ((ch is NonPlayerCharacterClass) && (action == 0))
            {
                if (((NonPlayerCharacterClass)ch).KnuBot != null)
                {
                    ch.KnuBotTarget = ch;
                    ((NonPlayerCharacterClass)ch).KnuBot.TalkingTo = chaffected;
                    ((NonPlayerCharacterClass)ch).KnuBot.Action(0);
                }
                return;
            }

            int nextfree;
            int cashdeduct = 0;
            AOItem it;
            int c;
            int c2;
            InventoryEntries ie;

            switch (action)
            {
                case 1: // end trade
                    c = client.Character.Inventory.Count - 1;
                    while (c >= 0)
                    {
                        ie = client.Character.Inventory[c];
                        if (ie.Container == -1)
                        {
                            nextfree = client.Character.GetNextFreeInventory(104); // next free spot on main inventory
                            it = ItemHandler.GetItemTemplate(ie.Item.lowID);
                            int Price = it.getItemAttribute(74);
                            int mult = it.getItemAttribute(212); // original multiplecount
                            if (mult == 0)
                            {
                                mult = 1;
                                ie.Item.multiplecount = 1;
                            }
                            // Deduct Cash (ie.item.multiplecount) div mult * price
                            cashdeduct +=
                                Convert.ToInt32(
                                    mult * Price
                                    *
                                    (100
                                     - Math.Floor(Math.Min(1500, client.Character.Stats.ComputerLiteracy.Value) / 40.0))
                                    / 2500);
                            // Add the Shop modificator and exchange the CompLit for skill form vendortemplate table
                            ie.Placement = nextfree;
                            ie.Container = 104;
                            if (!it.isStackable())
                            {
                                c2 = ie.Item.multiplecount;
                                ie.Item.multiplecount = 0;
                                while (c2 > 0)
                                {
                                    AddTemplate.Send(client, ie);
                                    c2--;
                                }
                            }
                            else
                            {
                                AddTemplate.Send(client, ie);
                            }
                        }
                        if (ie.Container == -2)
                        {
                            it = ItemHandler.interpolate(ie.Item.lowID, ie.Item.highID, ie.Item.Quality);
                            double mult = it.getItemAttribute(212); // original multiplecount
                            int Price = it.getItemAttribute(74);
                            if (mult == 0)
                            {
                                mult = 1.0;
                            }
                            else
                            {
                                mult = ie.Item.multiplecount / mult;
                            }
                            cashdeduct -=
                                Convert.ToInt32(
                                    mult * Price
                                    *
                                    (100
                                     + Math.Floor(Math.Min(1500, client.Character.Stats.ComputerLiteracy.Value) / 40.0))
                                    / 2500);
                            // Add the Shop modificator and exchange the CompLit for skill form vendortemplate table
                            client.Character.Inventory.Remove(ie);
                        }
                        c--;
                    }

                    client.Character.Stats.Cash.Set((uint)(client.Character.Stats.Cash.Value - cashdeduct));
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

                    pw.PushBytes(reply0);
                    pw.PushByte(1);
                    pw.PushByte(4);
                    pw.PushIdentity(client.Character.LastTrade);
                    pw.PushIdentity(client.Character.LastTrade);
                    client.Character.LastTrade.Type = 0;
                    client.Character.LastTrade.Instance = 0;
                    byte[] rep2 = pw.Finish();
                    client.SendCompressed(rep2);
                    break;
                case 2:
                    // Decline trade
                    c = client.Character.Inventory.Count - 1;
                    while (c >= 0)
                    {
                        ie = client.Character.Inventory[c];
                        if (ie.Container == -1)
                        {
                            client.Character.Inventory.Remove(ie);
                        }
                        else
                        {
                            if (ie.Container == -2)
                            {
                                ie.Placement = client.Character.GetNextFreeInventory(104);
                                ie.Container = 104;
                            }
                        }
                        c--;
                    }

                    byte[] reply1 = new byte[50];
                    Array.Copy(packet, reply1, 50);

                    // pushing in server ID
                    reply1[8] = 0;
                    reply1[9] = 0;
                    reply1[10] = 12;
                    reply1[11] = 14;

                    // pushing in Client ID
                    reply1[12] = (byte)(client.Character.ID >> 24);
                    reply1[13] = (byte)(client.Character.ID >> 16);
                    reply1[14] = (byte)(client.Character.ID >> 8);
                    reply1[15] = (byte)(client.Character.ID);

                    pw.PushBytes(reply1);
                    byte[] rep1 = pw.Finish();

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
                    if (ch.Inventory.Count == 0)
                    {
                        ((VendingMachine)ch).LoadTemplate(((VendingMachine)ch).TemplateID);
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
                    pw.PushBytes(reply);
                    byte[] rep3 = pw.Finish();
                    client.SendCompressed(rep3);

                    if (client.Character == ch)
                    {
                        if (action == 5)
                        {
                            ie = ch.getInventoryAt(_place);
                            ie.Placement = ch.GetNextFreeInventory(-2);
                            ie.Container = -2;
                        }
                        if (action == 6)
                        {
                            ie = ch.getInventoryAt(_place, -2);
                            ie.Placement = ch.GetNextFreeInventory(104);
                            ie.Container = 104;
                        }
                    }
                    else
                    {
                        InventoryEntries inew = new InventoryEntries();
                        inew.Container = -1;
                        inew.Placement = ch.GetNextFreeInventory(-1);
                        int oldplace = ((packet[46] >> 24) + (packet[47] >> 16) + (packet[48] >> 8) + packet[49]);
                        InventoryEntries totrade = ch.getInventoryAt(oldplace);
                        inew.Item.lowID = totrade.Item.lowID;
                        inew.Item.highID = totrade.Item.highID;
                        inew.Item.multiplecount = totrade.Item.multiplecount;
                        if (action == 6) // Remove item from trade window
                        {
                            inew.Item.multiplecount = -inew.Item.multiplecount;
                        }
                        inew.Item.Quality = totrade.Item.Quality;
                        chaffected.InventoryReplaceAdd(inew);
                    }
                    break;
            }
        }
    }
}