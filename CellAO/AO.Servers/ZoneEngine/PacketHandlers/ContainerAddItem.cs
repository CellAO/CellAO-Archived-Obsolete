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

    internal class ContainerAddItem
    {
        public static int getpage(int placement)
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

        public static void Do(ref byte[] packet, Client cli)
        {
            PacketReader m_reader = new PacketReader(ref packet);
            bool noAppearanceUpdate = false;
            /// Container ID's:
            /// 0065 Weaponpage
            /// 0066 Armorpage
            /// 0067 Implantpage
            /// 0068 Inventory (places 64-93)
            /// 0069 Bank
            /// 006B Backpack
            /// 006C KnuBot Trade Window
            /// 006E Overflow window
            /// 006F Trade Window
            /// 0073 Socialpage
            /// 0767 Shop Inventory
            /// 0790 Playershop Inventory
            /// DEAD Trade Window (incoming)

            m_reader.PopInt();
            m_reader.PopInt();
            int sender = m_reader.PopInt();
            m_reader.PopInt();
            m_reader.PopInt();
            Identity from_identity = m_reader.PopIdentity();
            byte flag = m_reader.PopByte();
            int c350 = from_identity.Type;
            int fromid = from_identity.Instance;

            int fromcontainerid = m_reader.PopInt();
            int fromplacement = m_reader.PopInt();
            Identity to_identity = m_reader.PopIdentity();
            int toid = to_identity.Instance;
            c350 = to_identity.Type;
            int toplacement = m_reader.PopInt();

            int c_from = 0;
            if ((fromcontainerid <= 0x68) || (fromcontainerid == 0x73)) // Inventory or Equipmentpages?
            {
                for (c_from = 0;
                     (c_from < cli.Character.Inventory.Count)
                     && (fromplacement != cli.Character.Inventory[c_from].Placement);
                     c_from++)
                {
                    ;
                }
            }
            else
            {
                if (fromcontainerid == 0x69)
                {
                    for (c_from = 0;
                         (c_from < cli.Character.Bank.Count) && (fromplacement != cli.Character.Bank[c_from].Flags);
                         c_from++)
                    {
                        ;
                    }
                }
                else
                {
                    c_from = -1;
                }
            }

            // TODO: Add check for instanced items (fromcontainerid:fromplacement)
            if (c_from == -1)
            {
                return;
            }

            int c_to;
            if (to_identity.Type == 0xdead) // Transferring to a trade window??? (only bank trade window yet)
            {
                c_to = cli.Character.Bank.Count;
            }
            else
            {
                for (c_to = 0;
                     (c_to < cli.Character.Inventory.Count) && (toplacement != cli.Character.Inventory[c_to].Placement);
                     c_to++)
                {
                    ;
                }
            }

            AOItem m_from = null;
            if (c_from < cli.Character.Inventory.Count)
            {
                m_from = ItemHandler.interpolate(
                    cli.Character.Inventory[c_from].Item.LowID,
                    cli.Character.Inventory[c_from].Item.HighID,
                    cli.Character.Inventory[c_from].Item.Quality);
            }

            AOItem m_to = null;
            if (c_to < cli.Character.Inventory.Count)
            {
                m_to = ItemHandler.interpolate(
                    cli.Character.Inventory[c_to].Item.LowID,
                    cli.Character.Inventory[c_to].Item.HighID,
                    cli.Character.Inventory[c_to].Item.Quality);
            }

            // Calculating delay for equip/unequip/switch gear
            int delay = 0;

            if (m_from != null)
            {
                delay += m_from.getItemAttribute(211);
            }
            if (m_to != null)
            {
                delay += m_to.getItemAttribute(211);
            }
            if (delay == 0)
            {
                delay = 200;
            }
            int c;
            if (toplacement == 0x6f) // 0x6f = next free inventory place, we need to send back the actual spot where the item goes
                // something has to be free, client checks for full inventory
            {
                c = 0;
                int c2 = 0;
                int c3 = 0;
                if (to_identity.Type == 0xdead)
                {
                    while (c3 == 0)
                    {
                        c3 = 1;
                        for (c2 = 0; c2 < cli.Character.Bank.Count; c2++)
                        {
                            if (cli.Character.Bank[c2].Flags == c) // using flags for placement
                            {
                                c++;
                                c3 = 0;
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    c = 64;
                    while (c3 == 0)
                    {
                        c3 = 1;
                        for (c2 = 0; c2 < cli.Character.Inventory.Count; c2++)
                        {
                            if (cli.Character.Inventory[c2].Placement == c)
                            {
                                c++;
                                c3 = 0;
                                continue;
                            }
                        }
                    }
                }
                toplacement = c;
            }

            cli.Character.dontdotimers = true;
            if (to_identity.Type == 0xdead) // 0xdead only stays for bank at the moment
            {
                cli.Character.TransferItemtoBank(fromplacement, toplacement);
                noAppearanceUpdate = true;
            }
            else
            {
                switch (fromcontainerid)
                {
                    case 0x68:
                        // from Inventory
                        if (toplacement <= 30)
                        {
                            // to Weaponspage or Armorpage
                            // TODO: Send some animation
                            if (m_to != null)
                            {
                                cli.Character.UnequipItem(m_to, cli.Character, false, fromplacement);
                                // send interpolated item
                                Unequip.Send(cli, m_to, getpage(toplacement), toplacement, false);
                                // client takes care of hotswap

                                cli.Character.EquipItem(m_from, cli.Character, false, toplacement);
                                Equip.Send(cli, m_from, getpage(toplacement), toplacement);
                            }
                            else
                            {
                                cli.Character.EquipItem(m_from, cli.Character, false, toplacement);
                                Equip.Send(cli, m_from, getpage(toplacement), toplacement);
                            }
                        }
                        else
                        {
                            if (toplacement < 46)
                            {
                                if (m_to == null)
                                {
                                    cli.Character.EquipItem(m_from, cli.Character, false, toplacement);
                                    Equip.Send(cli, m_from, getpage(toplacement), toplacement);
                                }
                            }
                            // Equiping to social page
                            if ((toplacement >= 49) && (toplacement <= 63))
                            {
                                if (m_to != null)
                                {
                                    cli.Character.UnequipItem(m_to, cli.Character, true, fromplacement);
                                    // send interpolated item
                                    cli.Character.EquipItem(m_from, cli.Character, true, toplacement);
                                }
                                else
                                {
                                    cli.Character.EquipItem(m_from, cli.Character, true, toplacement);
                                }

                                //cli.Character.switchItems(cli, fromplacement, toplacement);
                            }
                        }
                        cli.Character.switchItems(cli, fromplacement, toplacement);
                        cli.Character.CalculateSkills();
                        noAppearanceUpdate = false;
                        break;
                    case 0x66:
                        // from Armorpage
                        cli.Character.UnequipItem(m_from, cli.Character, false, fromplacement);
                        // send interpolated item
                        Unequip.Send(cli, m_from, getpage(fromplacement), fromplacement, false);
                        cli.Character.switchItems(cli, fromplacement, toplacement);
                        cli.Character.CalculateSkills();
                        noAppearanceUpdate = false;
                        break;
                    case 0x65:
                        // from Weaponspage
                        cli.Character.UnequipItem(m_from, cli.Character, false, fromplacement);
                        // send interpolated item
                        Unequip.Send(cli, m_from, getpage(fromplacement), fromplacement, false);
                        cli.Character.switchItems(cli, fromplacement, toplacement);
                        cli.Character.CalculateSkills();
                        noAppearanceUpdate = false;
                        break;
                    case 0x67:
                        // from Implantpage
                        cli.Character.UnequipItem(m_from, cli.Character, false, fromplacement);
                        // send interpolated item
                        Unequip.Send(cli, m_from, getpage(fromplacement), fromplacement, false);
                        cli.Character.switchItems(cli, fromplacement, toplacement);
                        cli.Character.CalculateSkills();
                        noAppearanceUpdate = true;
                        break;
                    case 0x73:
                        cli.Character.UnequipItem(m_from, cli.Character, true, fromplacement);

                        cli.Character.switchItems(cli, fromplacement, toplacement);
                        cli.Character.CalculateSkills();
                        break;
                    case 0x69:
                        cli.Character.TransferItemfromBank(fromplacement, toplacement);
                        toplacement = 0x6f; // setting back to 0x6f for packet reply
                        noAppearanceUpdate = true;
                        break;
                    case 0x6c:
                        // KnuBot Trade Window
                        cli.Character.TransferItemfromKnuBotTrade(fromplacement, toplacement);
                        break;
                    default:
                        break;
                }
            }
            cli.Character.dontdotimers = false;
            if ((fromplacement < 0x30) || (toplacement < 0x30)) // Equipmentpages need delays
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
            SwitchItem.Send(cli, fromcontainerid, fromplacement, to_identity, toplacement);
            cli.Character.Stats.ClearChangedFlags();
            if (!noAppearanceUpdate)
            {
                cli.Character.AppearanceUpdate();
            }
            m_from = null;
            m_to = null;
        }
    }
}