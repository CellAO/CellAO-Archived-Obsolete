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
    using System;

    using AO.Core;

    using ZoneEngine.NonPlayerCharacter;
    using ZoneEngine.Packets;

    public class GenericCmd
    {
        private static int temp1, count, action, temp4;

        private static Identity user, target;

        private static Client sender;

        public static void Read(byte[] packet, Client client, Dynel dynel)
        {
            sender = client;
            PacketReader packetReader = new PacketReader(packet);
            packetReader.PopHeader();
            packetReader.PopByte();
            temp1 = packetReader.PopInt();
            count = packetReader.PopInt(); // Count of commands sent
            action = packetReader.PopInt();
            temp4 = packetReader.PopInt();
            user = packetReader.PopIdentity();
            target = packetReader.PopIdentity();
            packetReader.Finish();
            bool feedback = true;
            switch (action)
            {
                case 1:
                    // Get
                    break;
                case 2:
                    // Drop
                    break;
                case 3:
                    // Use
                    OnUse();
                    var newcoord = client.Character.Coordinates;
                    feedback = false;

                    if (Statels.StatelppfonUse.ContainsKey(client.Character.PlayField))
                    {
                        foreach (Statels.Statel s in Statels.StatelppfonUse[client.Character.PlayField])
                        {
                            if (s.onUse(client, target))
                            {
                                return;
                            }
                        }
                    }
                    bool teleport = false;
                    int playfield = 152;
                    switch (target.Instance)
                    {
                            // Need to add feedback to the character 
                            // Are the Newer Grid points in this list???
                            //
                            // No newer Grid points in list, will be replaced by a check against a list of statels read from rdb anyway
                            // - Algorithman

                            #region teleporter tower (Noobisland)
                        case -1073605919: //Teleport Tower(noobisland)(right)
                            if (client.Character.Stats.Side.Value != 2)
                            {
                                client.SendChatText("You need to be omni to use this teleporter!");
                                teleport = false;
                            }
                            else
                            {
                                newcoord.x = 202;
                                newcoord.z = 878;
                                newcoord.y = 16;
                                playfield = 687;
                            }
                            break;

                        case -1073736991: //Teleport Tower(noobisland)(left)
                            if (client.Character.Stats.Side.Value != 1)
                            {
                                client.SendChatText("You need to be clan to use this teleporter!");
                                teleport = false;
                            }
                            else
                            {
                                newcoord.x = 390;
                                newcoord.z = 340;
                                newcoord.y = 0;
                                playfield = 545;
                            }
                            break;

                        case -1073671455: //Teleport Tower(noobisland)(middle)
                            if (client.Character.Stats.Side.Value != 0)
                            {
                                client.SendChatText("You need to be neutral to use this teleporter!");
                                teleport = false;
                            }
                            else
                            {
                                newcoord.x = 685;
                                newcoord.z = 480;
                                newcoord.y = 73;
                                playfield = 800;
                            }
                            break;
                            #endregion

                            #region Ferrys
                        case -1073741189: //2ho -> Stret west
                            if (client.Character.Stats.Cash.Value < 50) //check if you got enough credits to use the ferry
                            {
                                client.SendChatText("You need atleast 50 credits to board this ferry!");
                                teleport = false;
                            }
                            else
                            {
                                client.Character.Stats.Cash.Set(client.Character.Stats.Cash.Value - 50);
                                newcoord.x = 1143;
                                newcoord.z = 541;
                                newcoord.y = 8;
                                playfield = 790;
                            }
                            break;

                        case -1073478890: //Stret West -> 2ho
                            if (client.Character.Stats.Cash.Value < 50) //check if you got enough credits to use the ferry
                            {
                                client.SendChatText("You need atleast 50 credits to board this ferry!");
                                teleport = false;
                            }
                            else
                            {
                                client.Character.Stats.Cash.Set(client.Character.Stats.Cash.Value - 50);
                                newcoord.x = 760;
                                newcoord.z = 1982;
                                newcoord.y = 7;
                                playfield = 635;
                            }
                            break;

                        case -1073216841: //Harry's -> Plesant Meadows
                            if (client.Character.Stats.Cash.Value < 50) //check if you got enough credits to use the ferry
                            {
                                client.SendChatText("You need atleast 50 credits to board this ferry!");
                                teleport = false;
                            }
                            else
                            {
                                client.Character.Stats.Cash.Set(client.Character.Stats.Cash.Value - 50);
                                newcoord.x = 370;
                                newcoord.z = 1564;
                                newcoord.y = 7;
                                playfield = 630;
                            }
                            break;

                        case -1073216906: // Plesant Meadows -> Harry's
                            if (client.Character.Stats.Cash.Value < 50) //check if you got enough credits to use the ferry
                            {
                                client.SendChatText("You need atleast 50 credits to board this ferry!");
                                teleport = false;
                            }
                            else
                            {
                                client.Character.Stats.Cash.Set(client.Character.Stats.Cash.Value - 50);
                                newcoord.x = 3196;
                                newcoord.z = 3172;
                                newcoord.y = 7;
                                playfield = 695;
                            }
                            break;

                        case -1073282442: //Pleasant Meadows -> Omni-Tek outpost in Lush Fields
                            if (client.Character.Stats.Cash.Value < 50) //check if you got enough credits to use the ferry
                            {
                                client.SendChatText("You need atleast 50 credits to board this ferry!");
                                teleport = false;
                            }
                            else
                            {
                                client.Character.Stats.Cash.Set(client.Character.Stats.Cash.Value - 50);
                                newcoord.x = 3389;
                                newcoord.z = 800;
                                newcoord.y = 8;
                                playfield = 695;
                            }
                            break;

                        case -1073413449: //Omni-Tek outpost in Lush Fields -> Pleasant Meadows
                            if (client.Character.Stats.Cash.Value < 50) //check if you got enough credits to use the ferry
                            {
                                client.SendChatText("You need atleast 50 credits to board this ferry!");
                                teleport = false;
                            }
                            else
                            {
                                client.Character.Stats.Cash.Set(client.Character.Stats.Cash.Value - 50);
                                newcoord.x = 370;
                                newcoord.z = 1562;
                                newcoord.y = 7;
                                playfield = 630;
                            }
                            break;

                        case -1073347913: //Harry's trading outpost -> Omni-1 Trade (free)
                            newcoord.x = 3569;
                            newcoord.z = 912;
                            newcoord.y = 9;
                            playfield = 695;
                            break;

                        case -1073282377: //Omni-1 Trade -> Harry's trading outpost (free)
                            newcoord.x = 3290;
                            newcoord.z = 2922;
                            newcoord.y = 7;
                            playfield = 695;
                            break;
                            #endregion

                        default:
                            feedback = true;
                            teleport = false;
                            break;
                    }

                    if (teleport)
                    {
                        client.Teleport(newcoord, client.Character.Heading, playfield);
                    }

                    // Use item in inventory
                    if (target.Type == 104)
                    {
                        InventoryEntries ie = client.Character.GetInventoryAt(target.Instance);
                        AOItem mi = ItemHandler.GetItemTemplate(ie.Item.LowID);
                        // TODO mi.applyon(client.Character, ItemHandler.eventtype_onuse, true, false, ie.Placement);
                        TemplateAction.Send(client.Character, ie);
                        if (mi.isConsumable())
                        {
                            ie.Item.MultipleCount--;
                            if (ie.Item.MultipleCount <= 0)
                            {
                                client.Character.Inventory.Remove(ie);
                                DeleteItem.Send(client.Character, ie.Container, ie.Placement);
                                //                                Packets.Stat.Set(client, 0, client.Character.Stats.GetStat(0),false);
                            }
                        }
                        foreach (AOEvents aoe in mi.Events)
                        {
                            if (aoe.EventType == Constants.eventtype_onuse)
                            {
                                sender.Character.ExecuteEvent(
                                    sender.Character, sender.Character, aoe, true, false, 0, CheckReqs.doCheckReqs);
                                SkillUpdate.SendStat(client, 0x209, client.Character.Stats.SocialStatus.Value, false);
                                // Social Status
                                return;
                            }
                        }
                        int le = packet[7] + packet[6] * 256;
                        byte[] reply = new byte[le];
                        Array.Copy(packet, reply, le);
                        reply[0] = 0xdf;
                        reply[1] = 0xdf;
                        reply[8] = 0x00;
                        reply[9] = 0x00;
                        reply[10] = 0x0C;
                        reply[11] = 0x0E;
                        reply[12] = (byte)(client.Character.ID >> 24);
                        reply[13] = (byte)(client.Character.ID >> 16);
                        reply[14] = (byte)(client.Character.ID >> 8);
                        reply[15] = (byte)(client.Character.ID);
                        reply[0x1c] = 0;
                        reply[32] = 1;
                        reply[36] = 3;

                        PacketWriter pw = new PacketWriter();
                        pw.PushBytes(reply);
                        byte[] rep = pw.Finish();
                        client.SendCompressed(rep);
                        SkillUpdate.SendStat(client, 0x209, client.Character.Stats.SocialStatus.Value, false);
                        // Social Status
                        return;
                    }
                    else if (target.Type == 51035) // Shops
                    {
                        VendingMachine vm = VendorHandler.GetVendorById(target.Instance);
                        ShopInventory.Send(client, vm);
                        Trade.Send(client, client.Character, vm);
                        Trade.Send(client, vm, client.Character);
                        Trade.Send(client, vm, client.Character);
                        int le = packet[7] + packet[6] * 256;
                        byte[] reply = new byte[le];
                        Array.Copy(packet, reply, le);
                        reply[0] = 0xdf;
                        reply[1] = 0xdf;
                        reply[8] = 0x00;
                        reply[9] = 0x00;
                        reply[10] = 0x0C;
                        reply[11] = 0x0E;
                        reply[12] = (byte)(client.Character.ID >> 24);
                        reply[13] = (byte)(client.Character.ID >> 16);
                        reply[14] = (byte)(client.Character.ID >> 8);
                        reply[15] = (byte)(client.Character.ID);
                        reply[0x1c] = 0;
                        reply[0x20] = 1;

                        client.Character.LastTrade = target;
                        
                        PacketWriter pw = new PacketWriter();
                        pw.PushBytes(reply);
                        byte[] rep = pw.Finish();
                        client.SendCompressed(rep);
                    }
                    else if (target.Type == 51050) // Open corpse
                    {
                    }
                    break;
                case 4:
                    // Repair
                    break;
                case 5:
                    // UseItemOnItem
                    break;
                default:
                    break;
            }
            if (feedback)
            {
#if DEBUG
                string Feedback1 = string.Format(
                    "T1 {0}, Count {1}, Action {2}, T4 {3}", temp1, count, action, temp4);
                string Feedback2 = string.Format(
                    "User {0}:{1}, Target {2}:{3} ({4}:{5})",
                    user.Type,
                    user.Instance,
                    target.Type,
                    (uint)target.Instance,
                    target.Type.ToString("X4"),
                    ((uint)target.Instance).ToString("X8"));
                Statels.Statel b = null;
                if (Statels.Statelppf.ContainsKey(client.Character.PlayField))
                {
                    foreach (Statels.Statel z in Statels.Statelppf[client.Character.PlayField])
                    {
                        if ((z.Type == target.Type) && ((Int32)z.Instance == target.Instance))
                        {
                            b = z;
                            break;
                        }
                    }
                }
                if (b != null)
                {
                    foreach (Statels.Statel_Event e in b.Events)
                    {
                        Console.WriteLine("DebugOutput: \r\n" + e);
                    }
                    Console.WriteLine(b.Coordinates.ToString());
                }
                else
                {
                    Console.WriteLine(
                        "No Statel defined in database for #" + target.Type + ":" + (UInt32)target.Instance + " ("
                        + target.Type.ToString("X4") + ":" + target.Instance.ToString("X8") + ")");
                }
                client.SendChatText(Feedback1);
                client.SendChatText(Feedback2);
#endif
            }
        }

        private static void OnUse()
        {
            uint _t_instance = BitConverter.ToUInt32(BitConverter.GetBytes(target.Instance), 0);

            //LuaInterface.LuaTable tbl = Program.Script.GetLua().GetTable("PlayfieldSettings");
            /* Removed Lua Hook, Just reply
            bool result = Program.Script.CallHook("OnUse", _sender.Character.PlayField, _sender, _target, _t_instance);
            if (result == true)
            {*/
            Reply();
            //}

            #region Not sure whats going to come of this code.. but wont delete it.
            /*switch (_sender.Character.pf)
            {
                case 500:
                    // Soldier
                    if ((_target.Type == 51005) && (_t_instance == 3222340084))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(60, 1);
                            _sender.Character.Stats.SetStat(368, 1);
                            Reply();
                        }
                    }
                    // Martial Artist
                    if ((_target.Type == 51005) && (_t_instance == 3222405620))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(60, 2);
                            _sender.Character.Stats.SetStat(368, 2);
                            Reply();
                        }
                    }
                    // Engineer
                    if ((_target.Type == 51005) && (_t_instance == 3222471156))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(60, 3);
                            _sender.Character.Stats.SetStat(368, 3);
                            Reply();
                        }
                    }
                    // Fixer 
                    if ((_target.Type == 51005) && (_t_instance == 3222536692))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(60, 4);
                            _sender.Character.Stats.SetStat(368, 4);
                            Reply();
                        }
                    }
                    // Agent
                    if ((_target.Type == 51005) && (_t_instance == 3222602228))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(60, 5);
                            _sender.Character.Stats.SetStat(368, 5);
                            Reply();
                        }
                    }
                    // Adventurer
                    if ((_target.Type == 51005) && (_t_instance == 3222667764))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(60, 6);
                            _sender.Character.Stats.SetStat(368, 6);
                            Reply();
                        }
                    }
                    // Trader
                    if ((_target.Type == 51005) && (_t_instance == 3222733300))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(60, 7);
                            _sender.Character.Stats.SetStat(368, 7);
                            Reply();
                        }
                    }
                    // Bureaucrat
                    if ((_target.Type == 51005) && (_t_instance == 3222798836))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(60, 8);
                            _sender.Character.Stats.SetStat(368, 8);
                            Reply();
                        }
                    }
                    // Enforcer
                    if ((_target.Type == 51005) && (_t_instance == 3222864372))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(60, 9);
                            _sender.Character.Stats.SetStat(368, 9);
                            Reply();
                        }
                    }
                    // Doctor
                    if ((_target.Type == 51005) && (_t_instance == 3222929908))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(60, 10);
                            _sender.Character.Stats.SetStat(368, 10);
                            Reply();
                        }
                    }
                    // Nano Techician
                    if ((_target.Type == 51005) && (_t_instance == 3222995444))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(60, 11);
                            _sender.Character.Stats.SetStat(368, 11);
                            Reply();
                        }
                    }
                    // Meta Phycisist
                    if ((_target.Type == 51005) && (_t_instance == 3223060980))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(60, 12);
                            _sender.Character.Stats.SetStat(368, 12);
                            Reply();
                        }
                    }
                    // Shade
                    if ((_target.Type == 51005) && (_t_instance == 3223323124))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(60, 15);
                            _sender.Character.Stats.SetStat(368, 15);
                            Reply();
                        }
                    }
                    // Keeper
                    if ((_target.Type == 51005) && (_t_instance == 3223388660))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(60, 14);
                            _sender.Character.Stats.SetStat(368, 14);
                            Reply();
                        }
                    }
                    // Neutral
                    if ((_target.Type == 51005) && (_t_instance == 3223126516))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(33, 0);
                            Reply();
                        }
                    }
                    // Clan
                    if ((_target.Type == 51005) && (_t_instance == 3223192052))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(33, 1);
                            Reply();
                        }
                    }
                    // Omni-Tek
                    if ((_target.Type == 51005) && (_t_instance == 3223257588))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(33, 2);
                            Reply();
                        }
                    }

                    // Nano Male
                    if ((_target.Type == 51005) && (_t_instance == 3221553652))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(367, 3);
                            _sender.Character.Stats.SetStat(369, 2);
                            _sender.Character.Stats.SetStat(59, 2);
                            _sender.Character.Stats.SetStat(4, 3);
                            Reply();
                        }
                    }
                    // Nano Female
                    if ((_target.Type == 51005) && (_t_instance == 3221619188))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(367, 3);
                            _sender.Character.Stats.SetStat(369, 3);
                            _sender.Character.Stats.SetStat(59, 3);
                            _sender.Character.Stats.SetStat(4, 3);
                            Reply();
                        }
                    }
                    // Atrox
                    if ((_target.Type == 51005) && (_t_instance == 3221684724))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(367, 4);
                            _sender.Character.Stats.SetStat(369, 1);
                            _sender.Character.Stats.SetStat(59, 1);
                            _sender.Character.Stats.SetStat(4, 4);
                            Reply();
                        }
                    }
                    // Opifex Female
                    if ((_target.Type == 51005) && (_t_instance == 3221488116))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(367, 2);
                            _sender.Character.Stats.SetStat(369, 3);
                            _sender.Character.Stats.SetStat(59, 3);
                            _sender.Character.Stats.SetStat(4, 2);
                            Reply();
                        }
                    }
                    // Opifex Male
                    if ((_target.Type == 51005) && (_t_instance == 3221422580))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(367, 2);
                            _sender.Character.Stats.SetStat(369, 2);
                            _sender.Character.Stats.SetStat(59, 2);
                            _sender.Character.Stats.SetStat(4, 2);
                            Reply();
                        }
                    }
                    // Solitus Female
                    if ((_target.Type == 51005) && (_t_instance == 3221357044))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(367, 1);
                            _sender.Character.Stats.SetStat(369, 3);
                            _sender.Character.Stats.SetStat(59, 3);
                            _sender.Character.Stats.SetStat(4, 1);
                            Reply();
                        }
                    }
                    // Solitus Male
                    if ((_target.Type == 51005) && (_t_instance == 3221291508))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(367, 1);
                            _sender.Character.Stats.SetStat(369, 2);
                            _sender.Character.Stats.SetStat(59, 2);
                            _sender.Character.Stats.SetStat(4, 1);
                            _sender.Character.Stats.SetStat(58, 99);
                            Reply();
                        }
                    }
                    // Ectomorph
                    if ((_target.Type == 51005) && (_t_instance == 3222077940))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(47, 0);
                            Reply();
                        }
                    }
                    // Mesomorph
                    if ((_target.Type == 51005) && (_t_instance == 3221946868))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(47, 1);
                            Reply();
                        }
                    }
                    // Endomorph
                    if ((_target.Type == 51005) && (_t_instance == 3222012404))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(47, 2);
                            Reply();
                        }
                    }
                    // Tall
                    if ((_target.Type == 51005) && (_t_instance == 3222209012))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(360, 110);
                            Reply();
                        }
                    }
                    // Normalize
                    if ((_target.Type == 51005) && (_t_instance == 3222143476))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(360, 100);
                            Reply();
                        }
                    }
                    // Short
                    if ((_target.Type == 51005) && (_t_instance == 3222274548))
                    {
                        if (_sender.Character.Stats.GetStat("GmLevel") > 0)
                        {
                            _sender.Character.Stats.SetStat(360, 90);
                            Reply();
                        }
                    }
                    break;
                default:
                    break;
            }*/
            #endregion
        }

        internal static void Reply()
        {
            PacketWriter writer = new PacketWriter();
            writer.PushBytes(new byte[] { 0xDF, 0xDF });
            writer.PushShort(10);
            writer.PushShort(1);
            writer.PushShort(0);
            writer.PushInt(3086);
            writer.PushInt(sender.Character.ID);
            writer.PushInt(0x52526858);
            writer.PushIdentity(user);
            writer.PushByte(0);
            writer.PushInt(1);
            writer.PushInt(count);
            writer.PushInt(action);
            writer.PushInt(temp4);
            writer.PushIdentity(user);
            writer.PushIdentity(target);
            byte[] reply = writer.Finish();
            sender.SendCompressed(reply);
        }
    }
}