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

namespace ChatEngine.Lists
{
    using System.Collections.ObjectModel;
    using System.Data;

    using AO.Core;

    /// <summary>
    /// Buddy List
    /// </summary>
    public static class BuddyList
    {
        /// <summary>
        /// The load buddy list.
        /// </summary>
        /// <param name="charId">
        /// The char Id.
        /// </param>
        /// <returns>
        /// Buddy list
        /// </returns>
        public static Collection<BuddyListEntry> LoadBuddyList(int charId)
        {
            Collection<BuddyListEntry> buddyList = new Collection<BuddyListEntry>();
            SqlWrapper ms = new SqlWrapper();
            string sqlQuery = "SELECT `BuddyID` FROM `buddylist` WHERE PlayerID = " + "'" + charId + "'";
            DataTable dt = ms.ReadDT(sqlQuery);

            foreach (DataRow buddyRow in dt.Rows)
            {
                BuddyListEntry buddylistentry = new BuddyListEntry();
                buddylistentry.BuddyId = uint.Parse(buddyRow["BuddyID"].ToString());
                buddyList.Add(buddylistentry);
            }

            return buddyList;
        }

        // LoadRecentMsgsList unused?

        /// <summary>
        /// The load recent msgs list.
        /// </summary>
        /// <param name="charId">
        /// The char Id.
        /// </param>
        /// <returns>
        /// List of received messages
        /// </returns>
        public static Collection<RecentMsgsEntry> LoadRecentMsgsList(uint charId)
        {
            Collection<RecentMsgsEntry> reciviedMsgsList = new Collection<RecentMsgsEntry>();
            SqlWrapper ms = new SqlWrapper();
            string sqlQuery = "SELECT `ReceivedID` FROM `receivedmsgs` WHERE PlayerID =" + "'" + charId + "'";
            DataTable dt = ms.ReadDT(sqlQuery);
            foreach (DataRow msgsRow in dt.Rows)
            {
                RecentMsgsEntry rme = new RecentMsgsEntry { ReceivedId = uint.Parse(msgsRow["ReceivedID"].ToString()) };
                reciviedMsgsList.Add(rme);
            }

            return reciviedMsgsList;
        }
    }
}