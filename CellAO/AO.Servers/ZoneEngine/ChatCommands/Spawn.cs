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
    using System.Data;

    using AO.Core;

    using ZoneEngine.NPC;
    using ZoneEngine.Script;

    /// <summary>
    /// The chat command spawn.
    /// </summary>
    public class ChatCommandSpawn : AOChatCommand
    {
        /// <summary>
        /// The execute command.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public override void ExecuteCommand(Client client, Identity target, string[] args)
        {
            if (args.Length >= 2)
            {
                if (args[1].ToLower() == "list")
                {
                    string filter = string.Empty;
                    if (args.Length > 2)
                    {
                        for (int i = 2; i < args.Length; i++)
                        {
                            if (filter != string.Empty)
                            {
                                filter = filter + " AND ";
                            }

                            if (filter == string.Empty)
                            {
                                filter = "WHERE ";
                            }

                            filter = filter + "name like '%" + args[i] + "%' ";
                        }
                    }

                    SqlWrapper sql = new SqlWrapper();
                    DataTable dt = sql.ReadDT("SELECT Hash, Name FROM mobtemplate " + filter + " order by Name ASC");
                    client.SendChatText("List of mobtemplates: ");
                    foreach (DataRow row in dt.Rows)
                    {
                        client.SendChatText(row[0] + " " + row[1]);
                    }
                }

                return;
            }

            if (args.Length == 3)
            {
                NPCHandler.SpawnMonster(client, args[1], uint.Parse(args[2]));
            }
        }

        /// <summary>
        /// The command help.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        public override void CommandHelp(Client client)
        {
            client.SendChatText("Usage: /command Spawn hash level");
            client.SendChatText("For a list of available templates: /command spawn list [filter1,filter2...]");
            client.SendChatText("Filter will be applied to mob name");
            return;
        }

        /// <summary>
        /// The check command arguments.
        /// </summary>
        /// <param name="args">
        /// The arguments to check
        /// </param>
        /// <returns>
        /// True if check succeeded
        /// </returns>
        public override bool CheckCommandArguments(string[] args)
        {
            List<Type> check = new List<Type>();
            check.Add(typeof(string));
            check.Add(typeof(uint));
            bool check1 = this.CheckArgumentHelper(check, args);
            if (check1)
            {
                return true;
            }

            if (args[1].ToLower() != "list")
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// The gm level needed.
        /// </summary>
        /// <returns>
        /// Return the GM level needed to use this command
        /// </returns>
        public override int GMLevelNeeded()
        {
            return 1;
        }

        /// <summary>
        /// Get List of commands
        /// </summary>
        /// <returns>
        /// Returns the command list
        /// </returns>
        public override List<string> GetCommands()
        {
            List<string> temp = new List<string> { "spawn" };
            return temp;
        }
    }
}