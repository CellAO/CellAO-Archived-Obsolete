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

    using SmokeLounge.AOtomation.Messaging.GameData;

    using ZoneEngine.Misc;
    using ZoneEngine.Script;

    public class ChatCommandSetName : AOChatCommand
    {
        public override void ExecuteCommand(Client client, Identity target, string[] args)
        {
            Client mClient = null;

            if (target.Type != IdentityType.CanbeAffected)
            {
                client.SendChatText("Target must be player");
                return;
            }
            if (args.Length < 3)
            {
                this.CommandHelp(client);
                return;
            }

            if (target.Type != IdentityType.CanbeAffected)
            {
                client.SendChatText("Target must be player");
                return;
            }
            string fullArgs = "";
            int c = 0;
            while (c < args.Length)
            {
                fullArgs = fullArgs + " " + args[c++];
            }
            fullArgs = fullArgs.Trim();

            string newName = fullArgs.Substring(args[0].Length + 1 + args[1].Length + 1);

            if ((mClient = FindClient.FindClientById(target.Instance)) != null)
            {
                switch (args[1].ToLower())
                {
                    case "first":
                    case "firstname":
                        if (newName == "\"\"")
                        {
                            newName = "";
                        }
                        client.SendChatText(mClient.Character.Name + "'s first name has been set to " + newName);
                        mClient.Character.FirstName = newName;
                        mClient.Character.WriteNames();
                        break;
                    case "last":
                    case "lastname":
                        if (newName == "\"\"")
                        {
                            newName = "";
                        }
                        client.SendChatText(mClient.Character.Name + "'s last name has been set to " + newName);
                        mClient.Character.LastName = newName;
                        mClient.Character.WriteNames();
                        break;
                    case "nick":
                    case "nickname":
                        if (args.Length != 3)
                        {
                            client.SendChatText("Usage: /command setname &lt;first/last/nick&gt; &lt;newname&gt;");
                            break;
                        }
                        client.SendChatText(mClient.Character.Name + "'s nick name has been set to " + newName);
                        mClient.Character.Name = newName;
                        mClient.Character.WriteNames();
                        break;
                    default:
                        client.SendChatText("Usage: /command setname &lt;first/last/nick&gt; &lt;newname&gt;");
                        break;
                }
                return;
            }
            client.SendChatText("Unable to find target.");
        }

        public override void CommandHelp(Client client)
        {
            client.SendChatText("Usage: /command setname [first/last/nick] [name]");
            return;
        }

        public override bool CheckCommandArguments(string[] args)
        {
            // Always true, only string arguments
            return true;
        }

        public override int GMLevelNeeded()
        {
            return 1;
        }

        public override List<string> ListCommands()
        {
            List<string> temp = new List<string>();
            temp.Add("name");
            temp.Add("setname");
            return temp;
        }
    }
}