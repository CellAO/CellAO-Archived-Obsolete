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
    using System.Data;

    using AO.Core;

    using ZoneEngine.NonPlayerCharacter;
    using ZoneEngine.Script;

    public class ChatCommandShopSpawn : AOChatCommand
    {
        public override void ExecuteCommand(Client client, Identity target, string[] args)
        {
            if ((args.Length == 2) && (args[1].ToLower() != "list"))
            {
                VendorHandler.SpawnVendor(client, args[1]);
            }
            else
            {
                if (args.Length >= 2)
                {
                    string filter = "";
                    if (args.Length > 2)
                    {
                        for (int i = 2; i < args.Length; i++)
                        {
                            if (filter.Length > 0)
                            {
                                filter = filter + " AND ";
                            }
                            if (filter.Length == 0)
                            {
                                filter = "WHERE ";
                            }
                            filter = filter + "name like '%" + args[i] + "%' ";
                        }
                    }
                    SqlWrapper sql = new SqlWrapper();
                    DataTable dt = sql.ReadDatatable("SELECT Hash, Name FROM vendortemplate " + filter + " order by Name ASC");
                    client.SendChatText("List of vendortemplates: ");
                    foreach (DataRow row in dt.Rows)
                    {
                        client.SendChatText(row[0] + " " + row[1]);
                    }
                }
            }
        }

        public override void CommandHelp(Client client)
        {
            client.SendChatText("Usage: /command shopspawn hash");
            client.SendChatText("For a list of available templates: /command shopspawn list [filter1,filter2...]");
            client.SendChatText("Filter will be applied to vendor name");
            return;
        }

        public override bool CheckCommandArguments(string[] args)
        {
            // Only strings
            return true;
        }

        public override int GMLevelNeeded()
        {
            return 1;
        }

        public override List<string> GetCommands()
        {
            List<string> temp = new List<string>();
            temp.Add("shopspawn");
            return temp;
        }
    }
}