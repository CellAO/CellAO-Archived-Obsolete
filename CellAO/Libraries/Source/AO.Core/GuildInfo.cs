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
using System.Data;
#endregion

namespace AO.Core
{
    /// <summary>
    /// 
    /// </summary>
    public static class GuildInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="guildID"></param>
        /// <returns></returns>
        public static List<GuildEntry> GetGuildInfo(int guildID)
        {
            List<GuildEntry> Guild = new List<GuildEntry>();
            SqlWrapper ms = new SqlWrapper();
            string SqlQuery = "SELECT * FROM `organizations` WHERE ID=" + "'" + guildID + "'";
            DataTable dt = ms.ReadDatatable(SqlQuery);
            foreach (DataRow row in dt.Rows)
            {
                GuildEntry guildEntry = new GuildEntry();
                guildEntry.guildID = (UInt32) row["ID"];
                guildEntry.creation = (DateTime) row["creation"];
                guildEntry.Name = (string) row["Name"];
                guildEntry.LeaderID = (Int32) row["LeaderID"];
                guildEntry.GovernmentForm = (Int32) row["GovernmentForm"];
                guildEntry.Description = (string) row["Description"];
                guildEntry.Objective = (string) row["Objective"];
                guildEntry.History = (string) row["History"];
                guildEntry.Tax = (Int32) row["Tax"];
                guildEntry.Bank = (UInt64) row["Bank"];
                guildEntry.Comission = (Int32) row["Comission"];
                guildEntry.ContractsID = (Int32) row["ContractsID"];
                guildEntry.TowerFieldID = (Int32) row["TowerfieldID"];
                Guild.Add(guildEntry);
            }
            return Guild;
        }
    }
}