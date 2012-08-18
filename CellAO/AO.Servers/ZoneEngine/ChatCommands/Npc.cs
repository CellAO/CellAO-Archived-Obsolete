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
    using System.Collections.Generic;

    using AO.Core;

    using ZoneEngine.Script;

    public class ChatCommandNonPlayerCharacter : AOChatCommand
    {
        public override void ExecuteCommand(Client client, Identity target, string[] args)
        {
            if ((target.Type != 50000) && (target.Type != 51035))
            {
                this.CommandHelp(client);
                return;
            }
            NonPlayerCharacterClass targetMonster = null;
            foreach (NonPlayerCharacterClass mMonster in Program.zoneServer.Monsters)
            {
                if (mMonster.Id != target.Instance)
                {
                    continue;
                }
                targetMonster = mMonster;
                break;
            }
            if (targetMonster == null) // Perhaps a vendor?
            {
                foreach (VendingMachine vm in Program.zoneServer.Vendors)
                {
                    if (vm.Id != target.Instance)
                    {
                        continue;
                    }
                    targetMonster = vm;
                    break;
                }
            }
            if (targetMonster == null)
            {
                return;
            }
            switch (args[1].ToLower())
            {
                case "save":
                    {
                        if (targetMonster is VendingMachine)
                        {
                            ((VendingMachine)targetMonster).AddToDB();
                        }
                        else
                        {
                            targetMonster.AddToDB();
                        }
                    }
                    break;
                case "delete":
                    {
                        targetMonster.RemoveFromDB();
                    }
                    break;
                case "remove":
                    {
                        targetMonster.RemoveFromDB();
                    }
                    break;
                case "despawn":
                    {
                        targetMonster.Despawn();
                    }
                    break;
                default:
                    this.CommandHelp(client);
                    break;
            }
        }

        public override void CommandHelp(Client client)
        {
            client.SendChatText("/npc command help");
            client.SendChatText("Target a NPC or a Vendingmachine.");
            client.SendChatText("Available subcommands: save, delete, remove, despawn");
            client.SendChatText("save: saves a existing NPC/vendingmachine to database");
            client.SendChatText("delete, remove: removes a NPC/vendingmachine from database");
            client.SendChatText("despawn: despawns the NPC/vendingmachine without removing it from database");
            return;
        }

        public override bool CheckCommandArguments(string[] args)
        {
            // Always true, only strings to check
            return true;
        }

        public override int GMLevelNeeded()
        {
            // Be at least some GM, has to be adjusted when i find the gm level descriptions again
            return 1;
        }

        public override List<string> ListCommands()
        {
            List<string> temp = new List<string> { "npc" };
            return temp;
        }
    }
}