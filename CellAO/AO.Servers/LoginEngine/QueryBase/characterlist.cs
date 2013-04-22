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

namespace LoginEngine.QueryBase
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using AO.Core;

    using LoginEngine.Packets;

    /// <summary>
    /// 
    /// </summary>
    public static class CharacterList
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        public static List<CharacterEntry> LoadCharacters(string accountName)
        {
            List<CharacterEntry> characters = new List<CharacterEntry>();
            SqlWrapper ms = new SqlWrapper();

            string SqlQuery =
                "SELECT `characters`.`ID`, `characters`.`Name`, `characters`.`playfield`, (SELECT `Value` FROM `characters_stats` WHERE `characters`.`ID` = `characters_stats`.`ID` AND `Stat` = 54) as level, (SELECT `Value` FROM `characters_stats` WHERE `characters`.`ID` = `characters_stats`.`ID` AND `Stat` = 4) as breed, (SELECT `Value` FROM `characters_stats` WHERE `characters`.`ID` = `characters_stats`.`ID` AND `Stat` = 59) as gender, (SELECT `Value` FROM `characters_stats` WHERE `characters`.`ID` = `characters_stats`.`ID` AND `Stat` = 60) as profession FROM `characters` WHERE `characters`.Username = '"
                + accountName + "'";
            DataTable dt = ms.ReadDatatable(SqlQuery);

            foreach (DataRow row in dt.Rows)
            {
                CharacterEntry charentry = new CharacterEntry();
                charentry.Id = (Int32)row["ID"];
                charentry.Name = ((string)row["Name"]);
                charentry.Playfield = (Int32)row["playfield"];
                charentry.Level = (Int32)row["level"];
                charentry.Breed = (Int32)row["breed"];
                charentry.Gender = (Int32)row["gender"];
                charentry.Profession = (Int32)row["profession"];
                characters.Add(charentry);
            }
            return characters;
        }
    }
}