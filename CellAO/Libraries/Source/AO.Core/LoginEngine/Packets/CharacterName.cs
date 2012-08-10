#region License
/*
Copyright (c) 2005-2011, CellAO Team

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
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using AO.Core;
using AO.Core.Config;
using Cell.Core;
#endregion

namespace LoginEngine.Packets
{
    /// <summary>
    /// 
    /// </summary>
    public class CharacterName
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public CharacterName()
            : base()
        {
        }

        #endregion

        #region Int / String / Ect / Setup...

        /* strings to select from */
        /// <summary>
        /// 
        /// </summary>
        private static string optional_ord_con = "vybcfghjqktdnpmrlws"; /* 19 chars */
        /// <summary>
        /// 
        /// </summary>
        private static string mandatory_vowl = "aiueo"; /* 5 chars */
        /// <summary>
        /// 
        /// </summary>
        private static string optional_ord_end = "nmrlstyzx"; /* 9 chars */

        #endregion

        #region character stats

        /// <summary>
        /// 
        /// </summary>
        private int[] Abis
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string m_accountName
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string m_name
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Int32 m_breed
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Int32 m_gender
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Int32 m_profession
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Int32 m_level
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Int32 m_headmesh
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Int32 m_monsterscale
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Int32 m_fatness
        {
            get;
            set;
        }

        #endregion

        #region send random name

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="profession"></param>
        public void GetRandomName(Client client, Int32 profession)
        {
            MemoryStream m_stream = new MemoryStream();
            BinaryWriter m_writer = new BinaryWriter(m_stream);
            Random rand1 = new Random();
            byte rnd_len, rnd_name_len = 0;

            /* message header */
            m_writer.Write(new byte[] 
            {
                0xDF, 0xDF, 0x00, 0x01, 0x00, 0x01, 0x00,
                0x00,                               /* msg size - 2 byte */
                0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 
                0xff, 0xff,                         /* possible timecode */
                0x00, 0x00, 0x00, 
                0x56,                               /* answer 0x56, suggest name */
                0x00, 0x00, 0x00, 0x00
            });
            m_writer.Write(IPAddress.HostToNetworkOrder(profession));
            m_writer.Write(IPAddress.HostToNetworkOrder(profession));
            m_writer.Seek(-10, SeekOrigin.Current);
            /* get random name, size from 4 to 8 */
            // 
            rnd_len = (byte)rand1.Next(3, 8);
            while (rnd_name_len <= rnd_len)
            {
                if (rand1.Next(14) > 4)
                {
                    m_writer.Write(Encoding.ASCII.GetBytes(optional_ord_con.Substring(rand1.Next(0, 18), 1)));
                    rnd_name_len++;
                }

                m_writer.Write(Encoding.ASCII.GetBytes(mandatory_vowl.Substring(rand1.Next(0, 4), 1)));
                rnd_name_len++;

                if (rand1.Next(14) > 4)
                {
                    m_writer.Write(Encoding.ASCII.GetBytes(optional_ord_end.Substring(rand1.Next(0, 8), 1)));
                    rnd_name_len++;
                }
            }

            m_writer.Flush();
            m_stream.Capacity = (byte)m_stream.Length;
            byte[] reply = m_stream.GetBuffer();
            m_writer.Close();
            m_stream.Dispose();

            /* insert size */
            byte[] packetlength = BitConverter.GetBytes(reply.Length);
            /* insert name length */
            reply[7] = packetlength[0];
            reply[21] = (byte)rnd_name_len;
            /* first char uppercase */
            reply[22] ^= 32;
            /* send response */
            client.Send(ref reply);
        }

        #endregion

        #region check if name available;call create new char

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Int32 CheckAgainstDB()
        {
            SqlWrapper ms = new SqlWrapper();
            Int32 char_count = 0;
            // TODO:COUNT
            string SqlQuery = "SELECT count(`ID`) FROM `characters` WHERE Name = " + "'" + this.m_name + "'";
            char_count = ms.SqlCount(SqlQuery);

            /* name in use */
            if (char_count > 0) { return 0; }
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
            Int32 char_id = 0;
            switch (this.m_breed)
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
                    Console.WriteLine("unknown breed: ", this.m_breed);
                    break;

            }

            /*
             * Note, all default values are not specified here as defaults are handled
             * in the CharacterStats Class for us automatically. Also minimises SQL
             * usage for default stats that are never changed from their default value
             *           ~NV
             */
            Console.WriteLine("Problem");

            ms.SqlDelete("DELETE FROM `characters_stats` WHERE ID=" + char_id);
            String SqlInsert = "INSERT INTO `characters` (`Username`,`Name`,`FirstName`,`LastName`,";
            String SqlValues = "VALUES('" + this.m_accountName + "','" + this.m_name + "','','',";
            SqlInsert += "`playfield`,`X`,`Y`,`Z`,`HeadingX`,`HeadingY`,`HeadingZ`,`HeadingW`)";
            SqlValues += "0,0,0,0,0,0,0,0)";
            SqlInsert += SqlValues;

            try
            {
                ms.SqlInsert(SqlInsert);
            }
            catch (Exception e)
            {
                Console.WriteLine(SqlInsert + e.Message);
                return 0;
            }

            try
            {   /* select new char id */
                string SqlQuery = "SELECT `ID` FROM `characters` WHERE Name = " + "'" + this.m_name + "'";
                DataTable dt = ms.ReadDT(SqlQuery);

                foreach (DataRow row in dt.Rows)
                {
                    char_id = (Int32)row[0];
                }
            }
            catch (Exception e)
            {
                ms.sqlclose();
                Console.WriteLine(this.m_name + e.Message);
                return 0;
            }

            ms.SqlDelete("DELETE FROM `characters_stats` WHERE ID=" + char_id);
            SqlInsert = "INSERT INTO `characters_stats` (`ID`, `Stat`, `Value`) VALUES ";
   
            // Flags / 0 (Player) 
            SqlInsert += "(" + char_id + ", 0, " + 20 + "),";
            // Level / 54
            SqlInsert += "(" + char_id + ", 54, " + 1 + "),";
            // HeadMesh / 64
            SqlInsert += "(" + char_id + ", 64, " + this.m_headmesh + "),";
            // MonsterScale / 360
            SqlInsert += "(" + char_id + ", 360, " + this.m_monsterscale + "),";
            // Sex / 59
            SqlInsert += "(" + char_id + ", 59, " + this.m_gender + "),";
            // VisualSex / 369
            SqlInsert += "(" + char_id + ", 369, " + this.m_gender + "),";
            // Breed / 4
            SqlInsert += "(" + char_id + ", 4, " + this.m_breed + "),";
            // VisualBreed / 367
            SqlInsert += "(" + char_id + ", 367, " + this.m_breed + "),";
            // Profession / 60
            SqlInsert += "(" + char_id + ", 60, " + this.m_profession + "),";
            // VisualProfession / 368
            SqlInsert += "(" + char_id + ", 368, " + this.m_profession + "),";
            // Fatness / 47
            SqlInsert += "(" + char_id + ", 47, " + this.m_fatness + "),";

            // Strength / 16
            SqlInsert += "(" + char_id + ", 16, " + this.Abis[0] + "),";
            // Psychic / 21
            SqlInsert += "(" + char_id + ", 21, " + this.Abis[1] + "),";
            // Sense / 20
            SqlInsert += "(" + char_id + ", 20, " + this.Abis[2] + "),";
            // Intelligence / 19
            SqlInsert += "(" + char_id + ", 19, " + this.Abis[3] + "),";
            // Stamina / 18
            SqlInsert += "(" + char_id + ", 18, " + this.Abis[4] + "),";
            // Agility / 17
            SqlInsert += "(" + char_id + ", 17, " + this.Abis[5] + ");";
            ms.SqlInsert(SqlInsert);
            return char_id;
        }

        #endregion

        #region delete char

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="charid"></param>
        public void DeleteChar(Client client, Int32 charid)
        {
            MemoryStream m_stream = new MemoryStream();
            BinaryWriter m_writer = new BinaryWriter(m_stream);
            SqlWrapper ms = new SqlWrapper();

            try
            {   /* delete char */
                /* i assume there should be somewhere a flag, caus FC can reenable a deleted char.. */
                string SqlQuery = "DELETE FROM `characters` WHERE ID = " + charid;
                ms.SqlDelete(SqlQuery);
                SqlQuery = "DELETE FROM `characters_stats` WHERE ID = " + charid;
                ms.SqlDelete(SqlQuery);
                SqlQuery = "DELETE FROM `organizations` WHERE ID = " + charid;
                ms.SqlDelete(SqlQuery);
                SqlQuery = "DELETE FROM `inventory` WHERE ID = " + charid;
                ms.SqlDelete(SqlQuery);
            }
            catch (Exception e)
            {
                Console.WriteLine(this.m_name + e.Message);
            }
            /* Send Cleint delete message */
            m_writer.Write(new byte[] 
            {
                0xDF, 0xDF, 0x00, 0x01, 0x00, 0x01, 0x00,
                0x00,                               /* msg size - 2 byte */
                0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 
                0xff, 0xff,                        
                0x00, 0x00, 0x00, 
                0x15                               /* answer 0x15, char_id */
            });
            m_writer.Write(IPAddress.HostToNetworkOrder(charid));
            m_writer.Flush();
            m_stream.Capacity = (int)m_stream.Length;
            byte[] reply = m_stream.GetBuffer();
            m_writer.Close();
            m_stream.Dispose();
            /* insert size */
            byte[] packetlength = BitConverter.GetBytes(reply.Length);
            reply[7] = packetlength[0];
            /* send response */
            client.Send(ref reply);
        }


        #endregion

        #region send char to pf

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="start_in_sl"></param>
        /// <param name="charid"></param>
        public void SendNameToStartPF(Client client, bool start_in_sl, Int32 charid)
        {
            MemoryStream m_stream = new MemoryStream();
            BinaryWriter m_writer = new BinaryWriter(m_stream);
            SqlWrapper ms = new SqlWrapper();

            /* set startplayfield */
            string SqlUpdate = "UPDATE `characters` set ";

                if (start_in_sl)
                {
                    SqlUpdate += "`playfield`=4001,`X`=850,`Y`=43,`Z`=565 ";
                }
                else
                {
                    SqlUpdate += "`playfield`=4582,`X`=939,`Y`=20,`Z`=732 ";
                }
                SqlUpdate += " where `ID` = " + charid;

                ms.SqlUpdate(SqlUpdate);

            m_writer.Write(new byte[] 
            {
                0xDF, 0xDF, 0x00, 0x01, 0x00, 0x01, 0x00,
                0x00,                               /* msg size - 2 byte */
                0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 
                0xff, 0xff,                         /* possible timecode */
                0x00, 0x00, 0x00, 
                0x11                               /* answer 0x11, char_id */
            });
            m_writer.Write(IPAddress.HostToNetworkOrder(charid));
            m_writer.Write(new byte[] { 0xb0, 0xd2, 0xff, 0xff }); /* unknown */
            m_writer.Flush();
            m_stream.Capacity = (int)m_stream.Length;
            byte[] reply = m_stream.GetBuffer();
            m_writer.Close();
            m_stream.Dispose();
            /* insert size */
            byte[] packetlength = BitConverter.GetBytes(reply.Length);
            reply[7] = packetlength[0];

            /* send response */
            client.Send(ref reply);

        }

        #endregion

        #region send name in use

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        public void SendNameInUse(Client client)
        {
            MemoryStream m_stream = new MemoryStream();
            BinaryWriter m_writer = new BinaryWriter(m_stream);
            /* Name in use sequenze */
            m_writer.Write(new byte[] 
            {
                0xDF, 0xDF, 0x00, 0x01, 0x00, 0x01, 0x00,
                0x18,                               
                0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 
                0xff, 0xff, /* possible time */
                0x00, 0x00, 0x00, 
                0x10,                               
                0x00, 0x00, 0x00, 0x1e
            });
            m_writer.Flush();
            m_stream.Capacity = (byte)m_stream.Length;
            byte[] reply = m_stream.GetBuffer();
            m_writer.Close();
            m_stream.Dispose();

            /* send response */
            client.Send(ref reply);
        }

        #endregion
    }

}


