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

namespace LoginEngine.Packets
{
    using System;
    using System.Data;
    using System.IO;
    using System.Net;
    using System.Text;

    using AO.Core;

    using SmokeLounge.AOtomation.Messaging.GameData;

    /// <summary>
    /// 
    /// </summary>
    public class CharacterName
    {
        #region Int / String / Ect / Setup...
        /* strings to select from */

        /// <summary>
        /// 
        /// </summary>
        private static string optionalOrdCon = "vybcfghjqktdnpmrlws"; /* 19 chars */

        /// <summary>
        /// 
        /// </summary>
        private static string mandatoryVowel = "aiueo"; /* 5 chars */

        /// <summary>
        /// 
        /// </summary>
        private static string optionalOrdEnd = "nmrlstyzx"; /* 9 chars */
        #endregion

        #region character stats
        /// <summary>
        /// 
        /// </summary>
        private int[] Abis { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Breed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Gender { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Profession { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Level { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Int32 HeadMesh { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Int32 MonsterScale { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Fatness { get; set; }
        #endregion

        #region send random name
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="profession"></param>
        [Obsolete]
        public void GetRandomName(Client client, Int32 profession)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            Random rand1 = new Random();
            byte randomLength, randomNameLength = 0;

            /* message header */
            writer.Write(
                new byte[]
                    {
                        0xDF, 0xDF, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, /* msg size - 2 byte */
                        0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0xff, 0xff, /* possible timecode */
                        0x00, 0x00, 0x00, 0x56, /* answer 0x56, suggest name */
                        0x00, 0x00, 0x00, 0x00
                    });
            writer.Write(IPAddress.HostToNetworkOrder(profession));
            writer.Write(IPAddress.HostToNetworkOrder(profession));
            writer.Seek(-10, SeekOrigin.Current);
            /* get random name, size from 4 to 8 */
            // 
            randomLength = (byte)rand1.Next(3, 8);
            while (randomNameLength <= randomLength)
            {
                if (rand1.Next(14) > 4)
                {
                    writer.Write(Encoding.ASCII.GetBytes(optionalOrdCon.Substring(rand1.Next(0, 18), 1)));
                    randomNameLength++;
                }

                writer.Write(Encoding.ASCII.GetBytes(mandatoryVowel.Substring(rand1.Next(0, 4), 1)));
                randomNameLength++;

                if (rand1.Next(14) > 4)
                {
                    writer.Write(Encoding.ASCII.GetBytes(optionalOrdEnd.Substring(rand1.Next(0, 8), 1)));
                    randomNameLength++;
                }
            }

            writer.Flush();
            stream.Capacity = (byte)stream.Length;
            byte[] reply = stream.GetBuffer();
            writer.Close();
            stream.Dispose();

            /* insert size */
            byte[] packetlength = BitConverter.GetBytes(reply.Length);
            /* insert name length */
            reply[7] = packetlength[0];
            reply[21] = randomNameLength;
            /* first char uppercase */
            reply[22] ^= 32;
            /* send response */
            client.Send(reply);
        }
        #endregion

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

        #region check if name available;call create new char
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Int32 CheckAgainstDatabase()
        {
            SqlWrapper ms = new SqlWrapper();
            Int32 charCount = 0;
            // TODO:COUNT
            string sqlQuery = "SELECT count(`ID`) FROM `characters` WHERE Name = " + "'" + this.Name + "'";
            charCount = ms.SqlCount(sqlQuery);

            /* name in use */
            if (charCount > 0)
            {
                return 0;
            }
            return this.CreateNewChar();
        }
        #endregion

        #region create new char
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Int32 CreateNewChar()
        {
            SqlWrapper ms = new SqlWrapper();
            Int32 charID = 0;
            switch (this.Breed)
            {
                case 0x1: /* solitus */
                    this.Abis = new int[6] { 6, 6, 6, 6, 6, 6 };
                    break;
                case 0x2: /* opifex */
                    this.Abis = new int[6] { 3, 3, 10, 6, 6, 15 };
                    break;
                case 0x3: /* nanomage */
                    this.Abis = new int[6] { 3, 10, 6, 15, 3, 3 };
                    break;
                case 0x4: /* atrox */
                    this.Abis = new int[6] { 15, 3, 3, 3, 10, 6 };
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
            String sqlInsert = "INSERT INTO `characters` (`Username`,`Name`,`FirstName`,`LastName`,";
            String sqlValues = "VALUES('" + this.AccountName + "','" + this.Name + "','','',";
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
                string sqlQuery = "SELECT `ID` FROM `characters` WHERE Name = " + "'" + this.Name + "'";
                DataTable dt = ms.ReadDatatable(sqlQuery);

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

        #region delete char
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="charid"></param>
        [Obsolete]
        public void DeleteChar(Client client, Int32 charid)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            SqlWrapper ms = new SqlWrapper();

            try
            {
                /* delete char */
                /* i assume there should be somewhere a flag, caus FC can reenable a deleted char.. */
                string sqlQuery = "DELETE FROM `characters` WHERE ID = " + charid;
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
            /* Send Cleint delete message */
            writer.Write(
                new byte[]
                    {
                        0xDF, 0xDF, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, /* msg size - 2 byte */
                        0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0xff, 0xff, 0x00, 0x00, 0x00, 0x15 /* answer 0x15, char_id */
                    });
            writer.Write(IPAddress.HostToNetworkOrder(charid));
            writer.Flush();
            stream.Capacity = (int)stream.Length;
            byte[] reply = stream.GetBuffer();
            writer.Close();
            stream.Dispose();
            /* insert size */
            byte[] packetlength = BitConverter.GetBytes(reply.Length);
            reply[7] = packetlength[0];
            /* send response */
            client.Send(reply);
        }

        public void DeleteChar(int charid)
        {
            var ms = new SqlWrapper();

            try
            {
                /* delete char */
                /* i assume there should be somewhere a flag, caus FC can reenable a deleted char.. */
                string sqlQuery = "DELETE FROM `characters` WHERE ID = " + charid;
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
        #endregion

        #region send char to pf
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="startInSL"></param>
        /// <param name="charid"></param>
        [Obsolete]
        public void SendNameToStartPlayfield(Client client, bool startInSL, Int32 charid)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            SqlWrapper ms = new SqlWrapper();

            /* set startplayfield */
            string sqlUpdate = "UPDATE `characters` set ";

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

            writer.Write(
                new byte[]
                    {
                        0xDF, 0xDF, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, /* msg size - 2 byte */
                        0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0xff, 0xff, /* possible timecode */
                        0x00, 0x00, 0x00, 0x11 /* answer 0x11, char_id */
                    });
            writer.Write(IPAddress.HostToNetworkOrder(charid));
            writer.Write(new byte[] { 0xb0, 0xd2, 0xff, 0xff }); /* unknown */
            writer.Flush();
            stream.Capacity = (int)stream.Length;
            byte[] reply = stream.GetBuffer();
            writer.Close();
            stream.Dispose();
            /* insert size */
            byte[] packetlength = BitConverter.GetBytes(reply.Length);
            reply[7] = packetlength[0];

            /* send response */
            client.Send(reply);
        }

        public void SendNameToStartPlayfield(bool startInSL, int charid)
        {
            SqlWrapper ms = new SqlWrapper();

            /* set startplayfield */
            string sqlUpdate = "UPDATE `characters` set ";

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

        #region send name in use
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        [Obsolete]
        public void SendNameInUse(Client client)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            /* Name in use sequenze */
            writer.Write(
                new byte[]
                    {
                        0xDF, 0xDF, 0x00, 0x01, 0x00, 0x01, 0x00, 0x18, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0xff, 0xff,
                        /* possible time */
                        0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x1e
                    });
            writer.Flush();
            stream.Capacity = (byte)stream.Length;
            byte[] reply = stream.GetBuffer();
            writer.Close();
            stream.Dispose();

            /* send response */
            client.Send(reply);
        }
        #endregion
    }
}