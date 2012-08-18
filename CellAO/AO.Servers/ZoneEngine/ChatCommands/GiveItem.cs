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

namespace ZoneEngine.ChatCommands
{
    using System;
    using System.Collections.Generic;

    using AO.Core;

    using ZoneEngine.Misc;
    using ZoneEngine.Packets;
    using ZoneEngine.Script;

    public class ChatCommandGiveItem : AOChatCommand
    {
        public override void ExecuteCommand(Client client, Identity target, string[] args)
        {
            Client targetClient = null;
            if ((targetClient = FindClient.FindClientByName(args[1])) != null)
            {
                int firstfree = 64;
                firstfree = targetClient.Character.GetNextFreeInventory(104);
                if (firstfree <= 93)
                {
                    InventoryEntries mi = new InventoryEntries();
                    AOItem it = ItemHandler.GetItemTemplate(Convert.ToInt32(args[2]));
                    mi.Placement = firstfree;
                    mi.Container = 104;
                    mi.Item.LowID = Convert.ToInt32(args[2]);
                    mi.Item.HighID = Convert.ToInt32(args[3]);
                    mi.Item.Quality = Convert.ToInt32(args[4]);
                    if (it.ItemType != 1)
                    {
                        mi.Item.MultipleCount = Math.Max(1, it.getItemAttribute(212));
                    }
                    else
                    {
                        bool found = false;
                        foreach (AOItemAttribute a in mi.Item.Stats)
                        {
                            if (a.Stat != 212)
                            {
                                continue;
                            }
                            found = true;
                            a.Value = Math.Max(1, it.getItemAttribute(212));
                            break;
                        }
                        if (!found)
                        {
                            AOItemAttribute aoi = new AOItemAttribute();
                            aoi.Stat = 212;
                            aoi.Value = Math.Max(1, it.getItemAttribute(212));
                            mi.Item.Stats.Add(aoi);
                        }
                    }
                    targetClient.Character.Inventory.Add(mi);
                    AddTemplate.Send(targetClient, mi);
                }
                else
                {
                    client.SendChatText("Your Inventory is full");
                }
            }
        }

        public override void CommandHelp(Client client)
        {
            client.SendChatText("Usage: /command giveitem charactername lowid highid ql");
            return;
        }

        public override bool CheckCommandArguments(string[] args)
        {
            List<Type> check = new List<Type>();
            check.Add(typeof(string));
            check.Add(typeof(int));
            check.Add(typeof(int));
            check.Add(typeof(int));

            return CheckArgumentHelper(check, args);
        }

        public override int GMLevelNeeded()
        {
            return 1;
        }

        public override List<string> ListCommands()
        {
            List<string> temp = new List<string>();
            temp.Add("giveitem");
            return temp;
        }
    }
}