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

#region Using
using System;
using System.Collections.Generic;
using System.Data;
#endregion

namespace AO.Core
{
    /// <summary>
    /// Base Character Class
    /// </summary>
    public class CharacterBase
    {
        #region Sql
        /// <summary>
        /// MySQL Wrapper
        /// </summary>
        public SqlWrapper mySql = new SqlWrapper();
        #endregion

        #region Constructor / Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="characterId">Character ID</param>
        public CharacterBase(UInt32 characterId)
        {
            this.characterId = characterId;

            if (characterId != 0)
            {
                ReadNames();
            }
        }
        #endregion

        #region Character/Org Name(s) and ID(s)
        /// <summary>
        /// Read org and character names from DB
        /// </summary>
        public bool ReadNames()
        {
            DataTable dt =
                mySql.ReadDT("SELECT `Name`, `FirstName`, `LastName` FROM `characters` WHERE ID = '" + characterId +
                             "' LIMIT 1");
            if (dt.Rows.Count > 0)
            {
                characterName = (string) dt.Rows[0][0];
                characterFirstName = (string) dt.Rows[0][1];
                characterLastName = (string) dt.Rows[0][2];
            }
            else
            {
                return false;
            }

            // Read stat# 5 (Clan) - OrgID from character stats table
            dt =
                mySql.ReadDT("SELECT `Value` FROM `characters_stats` WHERE ID = " + characterId +
                             " AND Stat = 5 LIMIT 1");

            if (dt.Rows.Count > 0)
            {
                _orgId = (Int32) dt.Rows[0][0];
            }
            if (_orgId == 0)
            {
                orgName = string.Empty;
            }
            else
            {
                List<GuildEntry> m_Guild = GuildInfo.GetGuildInfo(_orgId);

                foreach (GuildEntry ge in m_Guild)
                {
                    orgName = ge.Name;
                }
            }
            return true;
        }

        /// <summary>
        /// Writes names to DB after a name change
        /// </summary>
        public bool WriteNames()
        {
            mySql.SqlUpdate("UPDATE `characters` SET `Name` = '" + characterName + "', `FirstName` = '" +
                            characterFirstName + "', `LastName` = '" + characterLastName + "' WHERE `ID` = " + "'" +
                            characterId + "'");
            return true;
        }

        /// <summary>
        /// Character ID
        /// </summary>
        public UInt32 characterId;

        /// <summary>
        /// Character Name
        /// </summary>
        public string characterName;

        /// <summary>
        /// Character First Name
        /// </summary>
        public string characterFirstName = string.Empty;

        /// <summary>
        /// Character Last Name
        /// </summary>
        public string characterLastName = string.Empty;

        private int _orgId;

        /// <summary>
        /// Organisation ID
        /// </summary>
        public virtual int orgId
        {
            get { return _orgId; }
            set { throw new Exception("orgId should not be set in the ChatEngine!"); }
        }

        /// <summary>
        /// Organisation Name
        /// </summary>
        public string orgName;
        #endregion
    }
}