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

namespace ZoneEngine.PacketHandlers
{
    using System.Threading;

    using AO.Core;

    using ZoneEngine.Packets;

    public static class ContainerAddItem
    {
        public static int InventoryPage(int placement)
        {
            if (placement < 16)
            {
                return 0x65;
            }
            if (placement < 32)
            {
                return 0x66;
            }
            if (placement < 48)
            {
                return 0x67;
            }
            if (placement < 64)
            {
                return 0x73;
            }
            return -1;
        }

        public static void AddItemToContainer(byte[] packet, Client cli)
        {
            PacketReader packetReader = new PacketReader(packet);
            bool noAppearanceUpdate = false;
            /* Container ID's:
             * 0065 Weaponpage
             * 0066 Armorpage
             *  0067 Implantpage
             *  0068 Inventory (places 64-93)
             *  0069 Bank
             *  006B Backpack
             *  006C KnuBot Trade Window
             *  006E Overflow window
             *  006F Trade Window
             *  0073 Socialpage
             *  0767 Shop Inventory
             *  0790 Playershop Inventory
             *  DEAD Trade Window (incoming)
             */

            packetReader.PopInt();
            packetReader.PopInt();
            int sender = packetReader.PopInt();
            packetReader.PopInt();
            packetReader.PopInt();
            Identity fromIdentity = packetReader.PopIdentity();
            byte flag = packetReader.PopByte();
            int c350 = fromIdentity.Type;
            int fromId = fromIdentity.Instance;

            int fromContainerID = packetReader.PopInt();
            int fromPlacement = packetReader.PopInt();
            Identity toIdentity = packetReader.PopIdentity();
            int toid = toIdentity.Instance;
            c350 = toIdentity.Type;
            int toPlacement = packetReader.PopInt();

            int counterFrom = 0;
            if ((fromContainerID <= 0x68) || (fromContainerID == 0x73)) // Inventory or Equipmentpages?
            {
                for (counterFrom = 0;
                     (counterFrom < cli.Character.Inventory.Count)
                     && (fromPlacement != cli.Character.Inventory[counterFrom].Placement);
                     counterFrom++)
                {
                    ;
                }
            }
            else
            {
                if (fromContainerID == 0x69)
                {
                    for (counterFrom = 0;
                         (counterFrom < cli.Character.Bank.Count)
                         && (fromPlacement != cli.Character.Bank[counterFrom].Flags);
                         counterFrom++)
                    {
                        ;
                    }
                }
                else
                {
                    counterFrom = -1;
                }
            }

            // TODO: Add check for instanced items (fromcontainerid:fromplacement)
            if (counterFrom == -1)
            {
                return;
            }

            int counterTo;
            if (toIdentity.Type == 0xdead) // Transferring to a trade window??? (only bank trade window yet)
            {
                counterTo = cli.Character.Bank.Count;
            }
            else
            {
                for (counterTo = 0;
                     (counterTo < cli.Character.Inventory.Count)
                     && (toPlacement != cli.Character.Inventory[counterTo].Placement);
                     counterTo++)
                {
                    ;
                }
            }

            AOItem itemFrom = null;
            if (counterFrom < cli.Character.Inventory.Count)
            {
                itemFrom = ItemHandler.interpolate(
                    cli.Character.Inventory[counterFrom].Item.LowID,
                    cli.Character.Inventory[counterFrom].Item.HighID,
                    cli.Character.Inventory[counterFrom].Item.Quality);
            }

            AOItem itemTo = null;
            if (counterTo < cli.Character.Inventory.Count)
            {
                itemTo = ItemHandler.interpolate(
                    cli.Character.Inventory[counterTo].Item.LowID,
                    cli.Character.Inventory[counterTo].Item.HighID,
                    cli.Character.Inventory[counterTo].Item.Quality);
            }

            // Calculating delay for equip/unequip/switch gear
            int delay = 0;

            if (itemFrom != null)
            {
                delay += itemFrom.getItemAttribute(211);
            }
            if (itemTo != null)
            {
                delay += itemTo.getItemAttribute(211);
            }
            if (delay == 0)
            {
                delay = 200;
            }
            int counter;
            if (toPlacement == 0x6f) // 0x6f = next free inventory place, we need to send back the actual spot where the item goes
                // something has to be free, client checks for full inventory
            {
                counter = 0;
                int counter2 = 0;
                int counter3 = 0;
                if (toIdentity.Type == 0xdead)
                {
                    while (counter3 == 0)
                    {
                        counter3 = 1;
                        for (counter2 = 0; counter2 < cli.Character.Bank.Count; counter2++)
                        {
                            if (cli.Character.Bank[counter2].Flags == counter) // using flags for placement
                            {
                                counter++;
                                counter3 = 0;
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    counter = 64;
                    while (counter3 == 0)
                    {
                        counter3 = 1;
                        for (counter2 = 0; counter2 < cli.Character.Inventory.Count; counter2++)
                        {
                            if (cli.Character.Inventory[counter2].Placement == counter)
                            {
                                counter++;
                                counter3 = 0;
                                continue;
                            }
                        }
                    }
                }
                toPlacement = counter;
            }

            cli.Character.DoNotDoTimers = true;
            if (toIdentity.Type == 0xdead) // 0xdead only stays for bank at the moment
            {
                cli.Character.TransferItemtoBank(fromPlacement, toPlacement);
                noAppearanceUpdate = true;
            }
            else
            {
                switch (fromContainerID)
                {
                    case 0x68:
                        // from Inventory
                        if (toPlacement <= 30)
                        {
                            // to Weaponspage or Armorpage
                            // TODO: Send some animation
                            if (itemTo != null)
                            {
                                cli.Character.UnequipItem(itemTo, cli.Character, false, fromPlacement);
                                // send interpolated item
                                Unequip.Send(cli, itemTo, InventoryPage(toPlacement), toPlacement);
                                // client takes care of hotswap

                                cli.Character.EquipItem(itemFrom, cli.Character, false, toPlacement);
                                Equip.Send(cli, itemFrom, InventoryPage(toPlacement), toPlacement);
                            }
                            else
                            {
                                cli.Character.EquipItem(itemFrom, cli.Character, false, toPlacement);
                                Equip.Send(cli, itemFrom, InventoryPage(toPlacement), toPlacement);
                            }
                        }
                        else
                        {
                            if (toPlacement < 46)
                            {
                                if (itemTo == null)
                                {
                                    cli.Character.EquipItem(itemFrom, cli.Character, false, toPlacement);
                                    Equip.Send(cli, itemFrom, InventoryPage(toPlacement), toPlacement);
                                }
                            }
                            // Equiping to social page
                            if ((toPlacement >= 49) && (toPlacement <= 63))
                            {
                                if (itemTo != null)
                                {
                                    cli.Character.UnequipItem(itemTo, cli.Character, true, fromPlacement);
                                    // send interpolated item
                                    cli.Character.EquipItem(itemFrom, cli.Character, true, toPlacement);
                                }
                                else
                                {
                                    cli.Character.EquipItem(itemFrom, cli.Character, true, toPlacement);
                                }

                                //cli.Character.switchItems(cli, fromplacement, toplacement);
                            }
                        }
                        cli.Character.SwitchItems(fromPlacement, toPlacement);
                        cli.Character.CalculateSkills();
                        noAppearanceUpdate = false;
                        break;
                    case 0x66:
                        // from Armorpage
                        cli.Character.UnequipItem(itemFrom, cli.Character, false, fromPlacement);
                        // send interpolated item
                        Unequip.Send(cli, itemFrom, InventoryPage(fromPlacement), fromPlacement);
                        cli.Character.SwitchItems(fromPlacement, toPlacement);
                        cli.Character.CalculateSkills();
                        noAppearanceUpdate = false;
                        break;
                    case 0x65:
                        // from Weaponspage
                        cli.Character.UnequipItem(itemFrom, cli.Character, false, fromPlacement);
                        // send interpolated item
                        Unequip.Send(cli, itemFrom, InventoryPage(fromPlacement), fromPlacement);
                        cli.Character.SwitchItems(fromPlacement, toPlacement);
                        cli.Character.CalculateSkills();
                        noAppearanceUpdate = false;
                        break;
                    case 0x67:
                        // from Implantpage
                        cli.Character.UnequipItem(itemFrom, cli.Character, false, fromPlacement);
                        // send interpolated item
                        Unequip.Send(cli, itemFrom, InventoryPage(fromPlacement), fromPlacement);
                        cli.Character.SwitchItems(fromPlacement, toPlacement);
                        cli.Character.CalculateSkills();
                        noAppearanceUpdate = true;
                        break;
                    case 0x73:
                        cli.Character.UnequipItem(itemFrom, cli.Character, true, fromPlacement);

                        cli.Character.SwitchItems(fromPlacement, toPlacement);
                        cli.Character.CalculateSkills();
                        break;
                    case 0x69:
                        cli.Character.TransferItemfromBank(fromPlacement, toPlacement);
                        toPlacement = 0x6f; // setting back to 0x6f for packet reply
                        noAppearanceUpdate = true;
                        break;
                    case 0x6c:
                        // KnuBot Trade Window
                        cli.Character.TransferItemfromKnuBotTrade(fromPlacement, toPlacement);
                        break;
                    default:
                        break;
                }
            }
            cli.Character.DoNotDoTimers = false;
            if ((fromPlacement < 0x30) || (toPlacement < 0x30)) // Equipmentpages need delays
            {
                // Delay when equipping/unequipping
                // has to be redone, jumping breaks the equiping/unequiping 
                // and other messages have to be done too
                // like heartbeat timer, damage from environment and such
                Thread.Sleep(delay);
            }
            else
            {
                Thread.Sleep(200); //social has to wait for 0.2 secs too (for helmet update)
            }
            SwitchItem.Send(cli, fromContainerID, fromPlacement, toIdentity, toPlacement);
            cli.Character.Stats.ClearChangedFlags();
            if (!noAppearanceUpdate)
            {
                cli.Character.AppearanceUpdate();
            }
            itemFrom = null;
            itemTo = null;
        }
    }
}