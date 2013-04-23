// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CharacterName.cs" company="CellAO Team">
//   Copyright © 2005-2013 CellAO Team.
//   
//   All rights reserved.
//   
//   Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//   
//       * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//       * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//       * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//   
//   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//   A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//   CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//   EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//   PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//   LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//   NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//   SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// </copyright>
// <summary>
//   Defines the CharacterName type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoginEngine.Packets
{
    using System;
    using System.Data;
    using System.Text;

    using AO.Core;

    using SmokeLounge.AOtomation.Messaging.GameData;

    public class CharacterName
    {
        #region Static Fields

        private static string mandatoryVowel = "aiueo"; /* 5 chars */

        private static string optionalOrdCon = "vybcfghjqktdnpmrlws"; /* 19 chars */

        private static string optionalOrdEnd = "nmrlstyzx"; /* 9 chars */

        #endregion

        #region Public Properties

        public string AccountName { get; set; }

        public int Breed { get; set; }

        public int Fatness { get; set; }

        public int Gender { get; set; }

        public int HeadMesh { get; set; }

        public int Level { get; set; }

        public int MonsterScale { get; set; }

        public string Name { get; set; }

        public int Profession { get; set; }

        #endregion

        #region Properties

        private int[] Abis { get; set; }

        #endregion

        #region Public Methods and Operators

        public int CheckAgainstDatabase()
        {
            var ms = new SqlWrapper();
            var charCount = 0;

            // TODO:COUNT
            var sqlQuery = "SELECT count(`ID`) FROM `characters` WHERE Name = " + "'" + this.Name + "'";
            charCount = ms.SqlCount(sqlQuery);

            /* name in use */
            if (charCount > 0)
            {
                return 0;
            }

            return this.CreateNewChar();
        }

        public void DeleteChar(int charid)
        {
            var ms = new SqlWrapper();

            try
            {
                /* delete char */
                /* i assume there should be somewhere a flag, caus FC can reenable a deleted char.. */
                var sqlQuery = "DELETE FROM `characters` WHERE ID = " + charid;
                ms.SqlDelete(sqlQuery);
                sqlQuery = "DELETE FROM `characters_stats` WHERE ID = " + charid;
                ms.SqlDelete(sqlQuery);
                sqlQuery = "DELETE FROM `organizations` WHERE ID = " + charid;
                ms.SqlDelete(sqlQuery);
                sqlQuery = "DELETE FROM `inventory` WHERE ID = " + charid;
                ms.SqlDelete(sqlQuery);
            }
            catch (Exception e)
            {
                Console.WriteLine(this.Name + e.Message);
            }
        }

        public string GetRandomName(Profession profession)
        {
            var random = new Random();
            byte randomNameLength = 0;
            var randomLength = (byte)random.Next(3, 8);
            var sb = new StringBuilder();
            while (randomNameLength <= randomLength)
            {
                if (random.Next(14) > 4)
                {
                    sb.Append(optionalOrdCon.Substring(random.Next(0, 18), 1));
                    randomNameLength++;
                }

                sb.Append(mandatoryVowel.Substring(random.Next(0, 4), 1));
                randomNameLength++;

                if (random.Next(14) <= 4)
                {
                    continue;
                }

                sb.Append(optionalOrdEnd.Substring(random.Next(0, 8), 1));
                randomNameLength++;
            }

            var name = sb.ToString();
            name = char.ToUpper(name[0]) + name.Substring(1);
            return name;
        }

        public void SendNameToStartPlayfield(bool startInSL, int charid)
        {
            var ms = new SqlWrapper();

            /* set startplayfield */
            var sqlUpdate = "UPDATE `characters` set ";

            if (startInSL)
            {
                sqlUpdate += "`playfield`=4001,`X`=850,`Y`=43,`Z`=565 ";
            }
            else
            {
                sqlUpdate += "`playfield`=4582,`X`=939,`Y`=20,`Z`=732 ";
            }

            sqlUpdate += " where `ID` = " + charid;

            ms.SqlUpdate(sqlUpdate);
        }

        #endregion

        #region Methods

        private int CreateNewChar()
        {
            var ms = new SqlWrapper();
            var charID = 0;
            switch (this.Breed)
            {
                case 0x1: /* solitus */
                    this.Abis = new[] { 6, 6, 6, 6, 6, 6 };
                    break;
                case 0x2: /* opifex */
                    this.Abis = new[] { 3, 3, 10, 6, 6, 15 };
                    break;
                case 0x3: /* nanomage */
                    this.Abis = new[] { 3, 10, 6, 15, 3, 3 };
                    break;
                case 0x4: /* atrox */
                    this.Abis = new[] { 15, 3, 3, 3, 10, 6 };
                    break;
                default:
                    Console.WriteLine("unknown breed: ", this.Breed);
                    break;
            }

            /*
             * Note, all default values are not specified here as defaults are handled
             * in the CharacterStats Class for us automatically. Also minimises SQL
             * usage for default stats that are never changed from their default value
             *           ~NV
             */
            ms.SqlDelete("DELETE FROM `characters_stats` WHERE ID=" + charID);
            var sqlInsert = "INSERT INTO `characters` (`Username`,`Name`,`FirstName`,`LastName`,";
            var sqlValues = "VALUES('" + this.AccountName + "','" + this.Name + "','','',";
            sqlInsert += "`playfield`,`X`,`Y`,`Z`,`HeadingX`,`HeadingY`,`HeadingZ`,`HeadingW`)";
            sqlValues += "0,0,0,0,0,0,0,0)";
            sqlInsert += sqlValues;

            try
            {
                ms.SqlInsert(sqlInsert);
            }
            catch (Exception e)
            {
                Console.WriteLine(sqlInsert + e.Message);
                return 0;
            }

            try
            {
                /* select new char id */
                var sqlQuery = "SELECT `ID` FROM `characters` WHERE Name = " + "'" + this.Name + "'";
                var dt = ms.ReadDatatable(sqlQuery);

                foreach (DataRow row in dt.Rows)
                {
                    charID = (Int32)row[0];
                }
            }
            catch (Exception e)
            {
                ms.sqlclose();
                Console.WriteLine(this.Name + e.Message);
                return 0;
            }

            ms.SqlDelete("DELETE FROM `characters_stats` WHERE ID=" + charID);
            sqlInsert = "INSERT INTO `characters_stats` (`ID`, `Stat`, `Value`) VALUES ";

            // Flags / 0 (Player) 
            sqlInsert += "(" + charID + ", 0, " + 20 + "),";

            // Level / 54
            sqlInsert += "(" + charID + ", 54, " + 1 + "),";

            // HeadMesh / 64
            sqlInsert += "(" + charID + ", 64, " + this.HeadMesh + "),";

            // MonsterScale / 360
            sqlInsert += "(" + charID + ", 360, " + this.MonsterScale + "),";

            // Sex / 59
            sqlInsert += "(" + charID + ", 59, " + this.Gender + "),";

            // VisualSex / 369
            sqlInsert += "(" + charID + ", 369, " + this.Gender + "),";

            // Breed / 4
            sqlInsert += "(" + charID + ", 4, " + this.Breed + "),";

            // VisualBreed / 367
            sqlInsert += "(" + charID + ", 367, " + this.Breed + "),";

            // Profession / 60
            sqlInsert += "(" + charID + ", 60, " + this.Profession + "),";

            // VisualProfession / 368
            sqlInsert += "(" + charID + ", 368, " + this.Profession + "),";

            // Fatness / 47
            sqlInsert += "(" + charID + ", 47, " + this.Fatness + "),";

            // Strength / 16
            sqlInsert += "(" + charID + ", 16, " + this.Abis[0] + "),";

            // Psychic / 21
            sqlInsert += "(" + charID + ", 21, " + this.Abis[1] + "),";

            // Sense / 20
            sqlInsert += "(" + charID + ", 20, " + this.Abis[2] + "),";

            // Intelligence / 19
            sqlInsert += "(" + charID + ", 19, " + this.Abis[3] + "),";

            // Stamina / 18
            sqlInsert += "(" + charID + ", 18, " + this.Abis[4] + "),";

            // Agility / 17
            sqlInsert += "(" + charID + ", 17, " + this.Abis[5] + "),";

            // Set HP and NP auf 1
            sqlInsert += "(" + charID + ",1,1),";
            sqlInsert += "(" + charID + ",214,1);";
            ms.SqlInsert(sqlInsert);
            return charID;
        }

        #endregion
    }
}