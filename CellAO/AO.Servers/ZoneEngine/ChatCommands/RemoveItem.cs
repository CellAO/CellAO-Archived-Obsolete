#region License
/*
Copyright (c) 2005-2012, CellAO Team

All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion

#region Usings...
using System;
using System.Collections.Generic;
using AO.Core;
using ZoneEngine.Misc;
using ZoneEngine.Packets;
using ZoneEngine.Script;
#endregion

namespace ZoneEngine.ChatCommands
{
    public class ChatCommandRemoveItem : AOChatCommand
    {
        public override void ExecuteCommand(Client client, Identity target, string[] args)
        {
            Client mClient;
            if (FindClient.FindClientByName(args[1], out mClient))
            {
                bool itemExists = (ItemExists(Convert.ToInt32(args[2]), mClient));
                if (itemExists)
                {
                    DeleteItem.Send(mClient.Character, 104, Convert.ToInt32(args[2]));
                    mClient.Character.Inventory.Remove(mClient.Character.getInventoryAt(Convert.ToInt32(args[2])));
                    return;
                }
                mClient.SendChatText("There exists no item in the slot you choose");
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
            List<Type> check = new List<Type>();
            check.Add(typeof (string));
            check.Add(typeof (int));

            return CheckArgumentHelper(check, args);
        }

        public override int GMLevelNeeded()
        {
            return 1;
        }

        public override List<string> GetCommands()
        {
            List<string> temp = new List<string>();
            temp.Add("remitem");
            temp.Add("delitem");
            return temp;
        }

        public bool ItemExists(int Placement, Client m_client)
        {
            return (m_client.Character.getInventoryAt(Placement) != null);
        }
    }
}