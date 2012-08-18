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

namespace ZoneEngine.ChatCommands
{
    using System;
    using System.Collections.Generic;

    using AO.Core;

    using ZoneEngine.Misc;
    using ZoneEngine.Packets;
    using ZoneEngine.Script;

    public class ChatCommandRemoveItem : AOChatCommand
    {
        public override void ExecuteCommand(Client client, Identity target, string[] args)
        {
            Client targetClient;
            if ((targetClient = FindClient.FindClientByName(args[1])) != null)
            {
                bool itemExists = (this.ItemExists(Convert.ToInt32(args[2]), targetClient));
                if (itemExists)
                {
                    DeleteItem.Send(targetClient.Character, 104, Convert.ToInt32(args[2]));
                    targetClient.Character.Inventory.Remove(
                        targetClient.Character.GetInventoryAt(Convert.ToInt32(args[2])));
                    return;
                }
                targetClient.SendChatText("There exists no item in the slot you choose");
            }
        }

        public override void CommandHelp(Client client)
        {
            client.SendChatText("Usage: /command remitem charname ItemLocation");
            client.SendChatText("Usage: /command delitem charname ItemLocation");
            return;
        }

        public override bool CheckCommandArguments(string[] args)
        {
            List<Type> check = new List<Type> { typeof(string), typeof(int) };
            return CheckArgumentHelper(check, args);
        }

        public override int GMLevelNeeded()
        {
            return 1;
        }

        public override List<string> ListCommands()
        {
            return new List<string> { "remitem", "delitem" };
        }

        public bool ItemExists(int placement, Client client)
        {
            return (client.Character.GetInventoryAt(placement) != null);
        }
    }
}