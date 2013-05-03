// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericCmd.cs" company="CellAO Team">
//   Copyright © 2005-2013 CellAO Team.
//   
//   All rights reserved.
//   
//   Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//   
//       * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//       * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//       * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//   
//   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//   A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//   CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//   EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//   PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//   LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//   NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//   SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// </copyright>
// <summary>
//   Defines the GenericCmd type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZoneEngine.PacketHandlers
{
    using System;

    using AO.Core;

    using SmokeLounge.AOtomation.Messaging.GameData;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    using ZoneEngine.NonPlayerCharacter;
    using ZoneEngine.Packets;

    public static class GenericCmd
    {
        #region Public Methods and Operators

        public static void Read(GenericCmdMessage message, Client client)
        {
            var sender = client;
            var feedback = true;
            switch (message.Action)
            {
                case GenericCmdAction.Get:

                    // Get
                    break;
                case GenericCmdAction.Drop:

                    // Drop
                    break;
                case GenericCmdAction.Use:

                    // Use
                    var newcoord = client.Character.Coordinates;
                    feedback = false;

                    if (Statels.StatelppfonUse.ContainsKey(client.Character.PlayField))
                    {
                        foreach (var s in Statels.StatelppfonUse[client.Character.PlayField])
                        {
                            if (s.onUse(client, message.Target))
                            {
                                return;
                            }
                        }
                    }

                    var teleport = false;
                    var playfield = 152;
                    switch (message.Target.Instance)
                    {
                            // Need to add feedback to the character 
                            // Are the Newer Grid points in this list???
                            // No newer Grid points in list, will be replaced by a check against a list of statels read from rdb anyway
                            // - Algorithman
                        case -1073605919: // Teleport Tower(noobisland)(right)
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

                        case -1073736991: // Teleport Tower(noobisland)(left)
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

                        case -1073671455: // Teleport Tower(noobisland)(middle)
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

                        case -1073741189: // 2ho -> Stret west
                            if (client.Character.Stats.Cash.Value < 50)
                            {
                                // check if you got enough credits to use the ferry
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

                        case -1073478890: // Stret West -> 2ho
                            if (client.Character.Stats.Cash.Value < 50)
                            {
                                // check if you got enough credits to use the ferry
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

                        case -1073216841: // Harry's -> Plesant Meadows
                            if (client.Character.Stats.Cash.Value < 50)
                            {
                                // check if you got enough credits to use the ferry
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
                            if (client.Character.Stats.Cash.Value < 50)
                            {
                                // check if you got enough credits to use the ferry
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

                        case -1073282442: // Pleasant Meadows -> Omni-Tek outpost in Lush Fields
                            if (client.Character.Stats.Cash.Value < 50)
                            {
                                // check if you got enough credits to use the ferry
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

                        case -1073413449: // Omni-Tek outpost in Lush Fields -> Pleasant Meadows
                            if (client.Character.Stats.Cash.Value < 50)
                            {
                                // check if you got enough credits to use the ferry
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

                        case -1073347913: // Harry's trading outpost -> Omni-1 Trade (free)
                            newcoord.x = 3569;
                            newcoord.z = 912;
                            newcoord.y = 9;
                            playfield = 695;
                            break;

                        case -1073282377: // Omni-1 Trade -> Harry's trading outpost (free)
                            newcoord.x = 3290;
                            newcoord.z = 2922;
                            newcoord.y = 7;
                            playfield = 695;
                            break;

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
                    if (message.Target.Type == IdentityType.Inventory)
                    {
                        var ie = client.Character.GetInventoryAt(message.Target.Instance);
                        var mi = ItemHandler.GetItemTemplate(ie.Item.LowID);

                        // TODO mi.applyon(client.Character, ItemHandler.eventtype_onuse, true, false, ie.Placement);
                        TemplateAction.Send(client.Character, ie);
                        if (mi.isConsumable())
                        {
                            ie.Item.MultipleCount--;
                            if (ie.Item.MultipleCount <= 0)
                            {
                                client.Character.Inventory.Remove(ie);
                                DeleteItem.Send(client.Character, ie.Container, ie.Placement);

                                // Packets.Stat.Set(client, 0, client.Character.Stats.GetStat(0),false);
                            }
                        }

                        foreach (var aoe in mi.Events)
                        {
                            if (aoe.EventType == Constants.EventtypeOnUse)
                            {
                                sender.Character.ExecuteEvent(
                                    sender.Character, sender.Character, aoe, true, false, 0, CheckReqs.doCheckReqs);
                                SkillUpdate.SendStat(client, 0x209, client.Character.Stats.SocialStatus.Value, false);

                                // Social Status
                                return;
                            }
                        }

                        var useReply = new GenericCmdMessage
                                           {
                                               Identity = message.Identity, 
                                               Unknown = 0x00, 
                                               Action = message.Action, 
                                               Count = 3, 
                                               Target = message.Target, 
                                               Temp1 = 1, 
                                               Temp4 = message.Temp4, 
                                               User = message.User
                                           };

                        client.SendCompressed(useReply);
                        SkillUpdate.SendStat(client, 0x209, client.Character.Stats.SocialStatus.Value, false);

                        // Social Status
                        return;
                    }
                    else if (message.Target.Type == IdentityType.VendingMachine)
                    {
                        // Shops
                        var vm = VendorHandler.GetVendorById(message.Target.Instance);
                        ShopInventory.Send(client, vm);
                        Trade.Send(client, client.Character, vm);
                        Trade.Send(client, vm, client.Character);
                        Trade.Send(client, vm, client.Character);

                        var shopReply = new GenericCmdMessage
                                            {
                                                Identity = message.Identity, 
                                                Unknown = 0x00, 
                                                Action = message.Action, 
                                                Count = message.Count, 
                                                Target = message.Target, 
                                                Temp1 = 1, 
                                                Temp4 = message.Temp4, 
                                                User = message.User
                                            };

                        client.Character.LastTrade = message.Target;
                        client.SendCompressed(shopReply);
                    }
                    else if (message.Target.Type == IdentityType.Corpse)
                    {
                        // Open corpse
                    }

                    break;
                case GenericCmdAction.Repair:

                    // Repair
                    break;
                case GenericCmdAction.UseItemOnItem:

                    // UseItemOnItem
#if DEBUG
                    Console.WriteLine("Use Item on Item not defined yet");
                    Console.WriteLine("Packet data:");
                    Console.WriteLine(
                        "Action: {0} Count: {1} Target: ({2}, {3}), User: ({4}, {5}), Temp1: {6}, Temp4: {7}", 
                        message.Action, 
                        message.Count,
                        message.Target.Type, 
                        message.Target.Instance,
                        message.User.Type, 
                        message.User.Instance, 
                        message.Temp1, 
                        message.Temp4);
#endif
                    break;
                default:
                    break;
            }

            if (feedback)
            {
#if DEBUG
                var Feedback1 = string.Format(
                    "T1 {0}, Count {1}, Action {2}, T4 {3}", message.Temp1, message.Count, message.Action, message.Temp4);
                var Feedback2 = string.Format(
                    "User {0}:{1}, Target {2}:{3} ({4}:{5})",
                    message.User.Type, 
                    message.User.Instance,
                    message.Target.Type,
                    (uint)message.Target.Instance,
                    ((int)message.Target.Type).ToString("X4"),
                    ((uint)message.Target.Instance).ToString("X8"));
                Statels.Statel b = null;
                if (Statels.Statelppf.ContainsKey(client.Character.PlayField))
                {
                    foreach (var z in Statels.Statelppf[client.Character.PlayField])
                    {
                        if ((z.Type == (int)message.Target.Type) && ((Int32)z.Instance == message.Target.Instance))
                        {
                            b = z;
                            break;
                        }
                    }
                }

                if (b != null)
                {
                    foreach (var e in b.Events)
                    {
                        Console.WriteLine("DebugOutput: \r\n" + e);
                    }

                    Console.WriteLine(b.Coordinates.ToString());
                }
                else
                {
                    Console.WriteLine(
                        "No Statel defined in database for #" + message.Target.Type + ":" + (UInt32)message.Target.Instance + " ("
                        + ((int)message.Target.Type).ToString("X4") + ":" + message.Target.Instance.ToString("X8") + ")");
                }

                client.SendChatText(Feedback1);
                client.SendChatText(Feedback2);
#endif
            }
        }

        #endregion
    }
}