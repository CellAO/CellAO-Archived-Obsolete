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

namespace ZoneEngine
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Text.RegularExpressions;

    using AO.Core;

    using SmokeLounge.AOtomation.Messaging.GameData;

    using ZoneEngine.Misc;

    using Quaternion = AO.Core.Quaternion;

    /// <summary>
    /// Main Dynamic Element Class
    /// </summary>
    public class Dynel
    {
        /// <summary>
        /// Unique Dynel ID
        /// </summary>
        public Identity Id { get; set; }

        /// <summary>
        /// Playfield 
        /// </summary>
        public int PlayField { get; set; }

        /// <summary>
        /// Resource... Check needed what it should do
        /// </summary>
        public int Resource { get; set; }

        /// <summary>
        /// Our Type number
        /// </summary>
        public int OurType { get; set; }

        /// <summary>
        /// Dynel Name (Player Name, Item name, Mob name etc)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Icon Number
        /// </summary>
        public int Icon { get; set; }

        /// <summary>
        /// Mesh Number
        /// </summary>
        public int Mesh { get; set; }

        /// <summary>
        /// Flags
        /// </summary>
        public int Flags { get; set; }

        /// <summary>
        /// Coordinates
        /// </summary>
        public AOCoord RawCoord { get; set; }

        /// <summary>
        /// Heading
        /// </summary>
        public Quaternion RawHeading { get; set; }

        /// <summary>
        /// Dynel Events
        /// </summary>
        private readonly List<AOEvents> events;

        /// <summary>
        /// Dynel Actions
        /// </summary>
        private readonly List<AOActions> actions;

        /// <summary>
        /// Textures
        /// </summary>
        public List<AOTextures> Textures;

        /// <summary>
        /// Social Tab structure. Mostly a 'cache', so we don't have to recalc the stuff all over again
        /// </summary>
        public Dictionary<int, int> SocialTab = new Dictionary<int, int>();

        /// <summary>
        /// Startup flag, true while Dynel is in 'creation' or 'loading' mode
        /// </summary>
        public bool Starting = true;

        //        public AOKnuBot KnuBot;     // TODO: Create a proper class for KnuBot contents

        /// <summary>
        /// Create a Dynel
        /// </summary>
        /// <param name="id">Unique ID</param>
        /// <param name="playfield">on Playfield</param>
        public Dynel(Identity id, int playfield)
        {
            lock (this)
            {
                this.Id = id;
                this.OurType = 0; // our type to identify the subclasses
                this.PlayField = playfield;
                this.RawCoord = new AOCoord();
                this.RawHeading = new Quaternion(0, 0, 0, 0);
                this.events = new List<AOEvents>();
                this.actions = new List<AOActions>();
                this.Textures = new List<AOTextures>();
                this.SocialTab = new Dictionary<int, int>();
            }
        }

        public Dynel()
        {
            // Do nothing at all
        }

        /// <summary>
        /// Get Sql Table prefix
        /// </summary>
        /// <returns>Prefix of the table we want to read or write</returns>
        public string GetSqlTablefromDynelType()
        {
            // maybe we should do this as a List, then new classes could self-register their tables
            switch (this.OurType)
            {
                case 0:
                    return "characters";
                case 1:
                    return "mobspawns";
                case 2:
                    return "pets";
                case 3:
                    return "vendors";
                default:
                    return "THIS SHOULD NOT BE REACHED AT ANY TIME!!!";
            }
        }

        #region AddSlashes helper
        /// <summary>
        /// Returns a string with backslashes before characters that need to be quoted
        /// </summary>
        /// <param name="inputTxt">Text string need to be escape with slashes</param>
        public static string AddSlashes(string inputTxt)
        {
            // List of characters handled:
            // \000 null
            // \010 backspace
            // \011 horizontal tab
            // \012 new line
            // \015 carriage return
            // \032 substitute
            // \042 double quote
            // \047 single quote
            // \134 backslash
            // \140 grave accent

            string result = inputTxt;

            try
            {
                result = Regex.Replace(inputTxt, @"[\000\010\011\012\015\032\042\047\134\140]", "\\$0");
            }
            catch (Exception Ex)
            {
                // handle any exception here
                Console.WriteLine(Ex.Message);
            }

            return result;
        }
        #endregion

        #region Heading and Coordinates are raw for main dynels
        /// <summary>
        /// Heading of the Dynel
        /// </summary>
        public Quaternion Heading
        {
            get
            {
                return this.RawHeading;
            }
        }

        /// <summary>
        /// Coordinates of the Dynel
        /// </summary>
        public AOCoord Coordinates
        {
            get
            {
                return this.RawCoord;
            }
        }

        /// <summary>
        /// Dynel Events
        /// </summary>
        public List<AOEvents> Events
        {
            get
            {
                return this.events;
            }
        }

        /// <summary>
        /// Dynel Actions
        /// </summary>
        public List<AOActions> Actions
        {
            get
            {
                return this.actions;
            }
        }
        #endregion

        #region Coordinates (read/write)

        /// <summary>
        /// Read Coordinates from Sql Table
        /// </summary>
        public void ReadCoordsFromSql()
        {
            SqlWrapper ms = new SqlWrapper();

            if (this.Id.Type == IdentityType.None)
            {
                return;
            }
            string SqlTable = this.GetSqlTablefromDynelType();
            DataTable dt =
                ms.ReadDatatable("SELECT Playfield, X,Y,Z from " + SqlTable + " WHERE ID=" + this.Id.Instance.ToString() + ";");

            if (dt.Rows.Count > 0)
            {
                this.PlayField = (Int32)dt.Rows[0][0];
                this.Coordinates.x = (Single)dt.Rows[0][1];
                this.Coordinates.y = (Single)dt.Rows[0][2];
                this.Coordinates.z = (Single)dt.Rows[0][3];
            }
        }

        /// <summary>
        /// Write Coordinates to Sql Table
        /// </summary>
        public virtual void WriteCoordinatesToSql()
        {
            SqlWrapper sqlWrapper = new SqlWrapper();

            sqlWrapper.SqlUpdate(
                "UPDATE " + this.GetSqlTablefromDynelType() + " SET playfield=" + this.PlayField.ToString() + ", X="
                + String.Format(CultureInfo.InvariantCulture, "'{0}'", this.Coordinates.x) + ", Y="
                + String.Format(CultureInfo.InvariantCulture, "'{0}'", this.Coordinates.y) + ", Z="
                + String.Format(CultureInfo.InvariantCulture, "'{0}'", this.Coordinates.z) + " WHERE ID="
                + this.Id.Instance.ToString() + ";");
        }
        #endregion

        #region Heading (read/write)

        /// <summary>
        /// Read Heading from Sql Table
        /// </summary>
        public void ReadHeadingFromSql()
        {
            SqlWrapper sqlWrapper = new SqlWrapper();

            if (this.Id.Type == IdentityType.None)
            {
                return;
            }
            string sqlTable = this.GetSqlTablefromDynelType();
            DataTable dataTable =
                sqlWrapper.ReadDatatable(
                    "SELECT HeadingX,HeadingY,HeadingZ,HeadingW from " + sqlTable + " WHERE ID=" + this.Id.Instance.ToString()
                    + ";");

            if (dataTable.Rows.Count > 0)
            {
                this.Heading.x = (Single)dataTable.Rows[0][0];
                this.Heading.y = (Single)dataTable.Rows[0][1];
                this.Heading.z = (Single)dataTable.Rows[0][2];
                this.Heading.w = (Single)dataTable.Rows[0][3];
            }
        }

        /// <summary>
        /// Write Heading to Sql Table
        /// </summary>
        public void WriteHeadingToSql()
        {
            SqlWrapper sqlWrapper = new SqlWrapper();
            sqlWrapper.SqlUpdate(
                "UPDATE " + this.GetSqlTablefromDynelType() + " SET HeadingX="
                + String.Format(CultureInfo.InvariantCulture, "'{0}'", this.Heading.x) + ", HeadingY="
                + String.Format(CultureInfo.InvariantCulture, "'{0}'", this.Heading.y) + ", HeadingZ="
                + String.Format(CultureInfo.InvariantCulture, "'{0}'", this.Heading.z) + ", HeadingW="
                + String.Format(CultureInfo.InvariantCulture, "'{0}'", this.Heading.w) + " WHERE ID="
                + this.Id.Instance.ToString() + ";");
        }
        #endregion

        #region Textures (read/write)

        /// <summary>
        /// Read Textures from Sql Table
        /// </summary>
        public void ReadTexturesFromSql()
        {
            SqlWrapper ms = new SqlWrapper();
            this.Textures.Clear();

            DataTable dt =
                ms.ReadDatatable(
                    "SELECT textures0, textures1, textures2, textures3, textures4 from "
                    + this.GetSqlTablefromDynelType() + " WHERE ID=" + this.Id.Instance.ToString() + ";");
            if (dt.Rows.Count > 0)
            {
                AOTextures textures = new AOTextures(0, (Int32)dt.Rows[0][0]);
                this.Textures.Add(textures);

                textures = new AOTextures(1, (Int32)dt.Rows[0][1]);
                this.Textures.Add(textures);

                textures = new AOTextures(2, (Int32)dt.Rows[0][2]);
                this.Textures.Add(textures);

                textures = new AOTextures(3, (Int32)dt.Rows[0][3]);
                this.Textures.Add(textures);

                textures = new AOTextures(4, (Int32)dt.Rows[0][4]);
                this.Textures.Add(textures);
            }
        }

        /// <summary>
        /// Write Textures to Sql Table
        /// </summary>
        public void WriteTexturesToSql()
        {
            SqlWrapper sqlWrapper = new SqlWrapper();
            int count;
            string upd = "";
            for (count = 0; count < this.Textures.Count; count++)
            {
                upd += "textures" + this.Textures[count].place.ToString() + "="
                       + this.Textures[count].Texture.ToString();
                if (count < this.Textures.Count - 1)
                {
                    upd += ", ";
                }
            }
            sqlWrapper.SqlUpdate(
                "UPDATE " + this.GetSqlTablefromDynelType() + " SET " + upd + " WHERE ID=" + this.Id.Instance.ToString() + ";");
        }
        #endregion

        #region Send text to vicinity LUA hook
        /// <summary>
        /// Dynel send to Vicinity
        /// </summary>
        /// <param name="message">Message string</param>
        public void SendVicinityChat(string message)
        {
            // Default vicinity radius is 10 -- Midian
            List<Dynel> clients = FindClient.GetClientsInRadius(this, 10.0f);
            UInt32[] recvers = new UInt32[clients.Count];
            int index = 0;

            foreach (Character child in clients)
            {
                recvers[index] = (UInt32)child.Id.Instance;
                index++;
            }

            ChatCom.SendVicinity((UInt32)this.Id.Instance, 0, recvers, message);
        }
        #endregion
    }
}