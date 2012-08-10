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
using ZoneEngine.Script;
#endregion

namespace ZoneEngine.ChatCommands
{
    public class ChatCommandDebugItem : AOChatCommand
    {
        public override void ExecuteCommand(Client client, Identity target, string[] args)
        {
            int itemid = Int32.Parse(args[1]);
            AOItem it = ItemHandler.GetItemTemplate(itemid);
            if (it == null)
            {
                client.SendChatText("No Item with id " + itemid + " found.");
                return;
            }
            client.SendChatText("Item Debug Info for Item " + itemid);
            client.SendChatText("Attack values:");
            foreach (AOItemAttribute at in it.Attack)
            {
                client.SendChatText("Type: " + at.Stat + " Value: " + at.Value);
            }
            client.SendChatText("Defense values:");
            foreach (AOItemAttribute at in it.Defend)
            {
                client.SendChatText("Type: " + at.Stat + " Value: " + at.Value);
            }
            client.SendChatText("Item Attributes:");
            foreach (AOItemAttribute at in it.Stats)
            {
                client.SendChatText("Type: " + at.Stat + " Value: " + at.Value);
            }

            client.SendChatText("Events/Functions:");
            foreach (AOEvents ev in it.Events)
            {
                client.SendChatText("Eventtype: " + ev.EventType);
                foreach (AOFunctions fu in ev.Functions)
                {
                    client.SendChatText("  Functionnumber: " + fu.FunctionType);
                    foreach (object arg in fu.Arguments)
                    {
                        client.SendChatText("    Argument: " + arg);
                    }
                    foreach (AORequirements aor in fu.Requirements)
                    {
                        client.SendChatText("    Reqs: " + aor.Statnumber + " " + aor.Operator + " " + aor.Value + " " +
                                            aor.ChildOperator);
                    }
                }
            }
        }

        public override void CommandHelp(Client client)
        {
            client.SendChatText("Usage: /command debugitem ItemID");
            return;
        }

        public override bool CheckCommandArguments(string[] args)
        {
            List<Type> check = new List<Type>();
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
            temp.Add("debugitem");
            return temp;
        }
    }
}