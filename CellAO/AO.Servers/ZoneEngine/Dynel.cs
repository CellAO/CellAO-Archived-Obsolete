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
using System.Globalization;
using System.Text.RegularExpressions;
using AO.Core;
using ZoneEngine.Misc;
#endregion

namespace ZoneEngine
{
    /// <summary>
    /// Main Dynamic Element Class
    /// </summary>
    public class Dynel
    {
        /// <summary>
        /// Unique Dynel ID
        /// </summary>
        public int ID;

        /// <summary>
        /// Dynel Type (AO Type)
        /// </summary>
        public int Type;

        /// <summary>
        /// Playfield 
        /// </summary>
        public int PlayField;

        /// <summary>
        /// Resource... Check needed what it should do
        /// </summary>
        public int resource;

        /// <summary>
        /// Our Type number
        /// </summary>
        public int ourType;

        /// <summary>
        /// Dynel Name (Player Name, Item name, Mob name etc)
        /// </summary>
        public string Name;

        /// <summary>
        /// Icon Number
        /// </summary>
        public int Icon;

        /// <summary>
        /// Mesh Number
        /// </summary>
        public int Mesh;

        /// <summary>
        /// Flags
        /// </summary>
        public int Flags;

        /// <summary>
        /// Coordinates
        /// </summary>
        public AOCoord rawCoord;

        /// <summary>
        /// Heading
        /// </summary>
        public Quaternion rawHeading;

        /// <summary>
        /// Dynel Events
        /// </summary>
        public List<AOEvents> Events;

        /// <summary>
        /// Dynel Actions
        /// </summary>
        public List<AOActions> Actions;

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
        public bool startup = true;

//        public AOKnuBot KnuBot;     // TODO: Create a proper class for KnuBot contents


        /// <summary>
        /// Create a Dynel
        /// </summary>
        /// <param name="_id">Unique ID</param>
        /// <param name="_playfield">on Playfield</param>
        public Dynel(int _id, int _playfield)
        {
            lock (this)
            {
                ID = _id;
                Type = 0; // empty Dynel has no type, subclasses are setting it
                ourType = 0; // our type to identify the subclasses
                PlayField = _playfield;
                rawCoord = new AOCoord();
                rawHeading = new Quaternion(0, 0, 0, 0);
                Events = new List<AOEvents>();
                Actions = new List<AOActions>();
                Textures = new List<AOTextures>();
                SocialTab = new Dictionary<int, int>();
            }
        }

        public Dynel()
        {
            // Do nothing at all
        }

        /// <summary>
        /// Get SQL Table prefix
        /// </summary>
        /// <returns>Prefix of the table we want to read or write</returns>
        public string getSQLTablefromDynelType()
        {
            /// maybe we should do this as a List, then new classes could self-register their tables
            switch (ourType)
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
        /// <param name="InputTxt">Text string need to be escape with slashes</param>
        public static string AddSlashes(string InputTxt)
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

            string Result = InputTxt;

            try
            {
                Result = Regex.Replace(InputTxt, @"[\000\010\011\012\015\032\042\047\134\140]", "\\$0");
            }
            catch (Exception Ex)
            {
                // handle any exception here
                Console.WriteLine(Ex.Message);
            }

            return Result;
        }
        #endregion

        #region Heading and Coordinates are raw for main dynels
        /// <summary>
        /// Heading of the Dynel
        /// </summary>
        public Quaternion Heading
        {
            get { return rawHeading; }
        }

        /// <summary>
        /// Coordinates of the Dynel
        /// </summary>
        public AOCoord Coordinates
        {
            get { return rawCoord; }
        }
        #endregion

        #region Coordinates (read/write)
        /// <summary>
        /// Read Coordinates from a packet reader
        /// </summary>
        /// <param name="packet">Packetreader ref</param>
        public void readCoordsfromPacket(ref PacketReader packet)
        {
            rawCoord = packet.PopCoord();
        }

        /// <summary>
        /// Read Coordinates from SQL Table
        /// </summary>
        public void readCoordsfromSQL()
        {
            SqlWrapper ms = new SqlWrapper();

            if (Type == 0)
            {
                return;
            }
            string SQLTable = getSQLTablefromDynelType();
            DataTable dt = ms.ReadDT("SELECT Playfield, X,Y,Z from " + SQLTable + " WHERE ID=" + ID.ToString() + ";");

            if (dt.Rows.Count > 0)
            {
                PlayField = (Int32) dt.Rows[0][0];
                Coordinates.x = (Single) dt.Rows[0][1];
                Coordinates.y = (Single) dt.Rows[0][2];
                Coordinates.z = (Single) dt.Rows[0][3];
            }
        }

        /// <summary>
        /// Write Coordinates to packet
        /// </summary>
        /// <param name="packet">Packet ref to write to</param>
        public void writeCoordinatestoPacket(ref PacketWriter packet)
        {
            packet.PushCoord(Coordinates);
        }

        /// <summary>
        /// Write Coordinates to SQL Table
        /// </summary>
        public void writeCoordinatestoSQL()
        {
            SqlWrapper ms = new SqlWrapper();

            ms.SqlUpdate("UPDATE " + getSQLTablefromDynelType() + " SET playfield=" + PlayField.ToString() + ", X=" +
                         String.Format(CultureInfo.InvariantCulture, "'{0}'", Coordinates.x) + ", Y=" +
                         String.Format(CultureInfo.InvariantCulture, "'{0}'", Coordinates.y) + ", Z=" +
                         String.Format(CultureInfo.InvariantCulture, "'{0}'", Coordinates.z) + " WHERE ID=" +
                         ID.ToString() + ";");
        }
        #endregion

        #region Heading (read/write)
        /// <summary>
        /// Read Heading from packet
        /// </summary>
        /// <param name="packet">Packetreader ref</param>
        public void readHeadingfromPacket(ref PacketReader packet)
        {
            Heading.x = packet.PopFloat();
            Heading.y = packet.PopFloat();
            Heading.z = packet.PopFloat();
            Heading.w = packet.PopFloat();
        }

        /// <summary>
        /// Read Heading from SQL Table
        /// </summary>
        public void readHeadingfromSQL()
        {
            SqlWrapper ms = new SqlWrapper();

            if (Type == 0)
            {
                return;
            }
            string SQLTable = getSQLTablefromDynelType();
            DataTable dt =
                ms.ReadDT("SELECT HeadingX,HeadingY,HeadingZ,HeadingW from " + SQLTable + " WHERE ID=" + ID.ToString() +
                          ";");

            if (dt.Rows.Count > 0)
            {
                Heading.x = (Single) dt.Rows[0][0];
                Heading.y = (Single) dt.Rows[0][1];
                Heading.z = (Single) dt.Rows[0][2];
                Heading.w = (Single) dt.Rows[0][3];
            }
        }

        /// <summary>
        /// Write Heading to packetwriter
        /// </summary>
        /// <param name="packet">Packetwriter ref</param>
        public void writeHeadingtoPacket(ref PacketWriter packet)
        {
            packet.PushFloat((float) Heading.x);
            packet.PushFloat((float) Heading.y);
            packet.PushFloat((float) Heading.z);
            packet.PushFloat((float) Heading.w);
        }

        /// <summary>
        /// Write Heading to SQL Table
        /// </summary>
        public void writeHeadingtoSQL()
        {
            SqlWrapper ms = new SqlWrapper();
            ms.SqlUpdate("UPDATE " + getSQLTablefromDynelType() + " SET HeadingX=" +
                         String.Format(CultureInfo.InvariantCulture, "'{0}'", Heading.x) + ", HeadingY=" +
                         String.Format(CultureInfo.InvariantCulture, "'{0}'", Heading.y) + ", HeadingZ=" +
                         String.Format(CultureInfo.InvariantCulture, "'{0}'", Heading.z) + ", HeadingW=" +
                         String.Format(CultureInfo.InvariantCulture, "'{0}'", Heading.w) + " WHERE ID=" + ID.ToString() +
                         ";");
        }
        #endregion

        #region Textures (read/write)
        /// <summary>
        /// Read Textures from Packetreader
        /// </summary>
        /// <param name="packet">Packetreader ref</param>
        public void readTexturesfromPacket(ref PacketReader packet)
        {
            int count = packet.Pop3F1Count();
            AOTextures m_tex;
            while (count > 0)
            {
                m_tex = new AOTextures(packet.PopInt(), packet.PopInt());
                count--;
            }
        }

        /// <summary>
        /// Read Textures from SQL Table
        /// </summary>
        public void readTexturesfromSQL()
        {
            SqlWrapper ms = new SqlWrapper();
            AOTextures m_tex;
            Textures.Clear();

            DataTable dt =
                ms.ReadDT("SELECT textures0, textures1, textures2, textures3, textures4 from " +
                          getSQLTablefromDynelType() + " WHERE ID=" + ID.ToString() + ";");
            if (dt.Rows.Count > 0)
            {
                m_tex = new AOTextures(0, (Int32) dt.Rows[0][0]);
                Textures.Add(m_tex);

                m_tex = new AOTextures(1, (Int32) dt.Rows[0][1]);
                Textures.Add(m_tex);

                m_tex = new AOTextures(2, (Int32) dt.Rows[0][2]);
                Textures.Add(m_tex);

                m_tex = new AOTextures(3, (Int32) dt.Rows[0][3]);
                Textures.Add(m_tex);

                m_tex = new AOTextures(4, (Int32) dt.Rows[0][4]);
                Textures.Add(m_tex);
            }
        }

        /// <summary>
        /// Write Textures to PacketWriter
        /// </summary>
        /// <param name="packet">PacketWriter ref</param>
        public void writeTexturestoPacket(ref PacketWriter packet)
        {
            packet.Push3F1Count(Textures.Count);
            int count;
            for (count = 0; count < Textures.Count; count++)
            {
                packet.PushInt(Textures[count].place);
                packet.PushInt(Textures[count].Texture);
                packet.PushInt(0);
            }
        }

        /// <summary>
        /// Write Textures to SQL Table
        /// </summary>
        public void writeTexturestoSQL()
        {
            SqlWrapper ms = new SqlWrapper();
            int count;
            string upd = "";
            for (count = 0; count < Textures.Count; count++)
            {
                upd += "textures" + Textures[count].place.ToString() + "=" + Textures[count].Texture.ToString();
                if (count < Textures.Count - 1)
                {
                    upd += ", ";
                }
            }
            ms.SqlUpdate("UPDATE " + getSQLTablefromDynelType() + " SET " + upd + " WHERE ID=" + ID.ToString() + ";");
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
            List<Dynel> Clients = FindClient.GetClientsInRadius(this, 10.0f);
            UInt32[] recvers = new UInt32[Clients.Count];
            int index = 0;

            foreach (Character child in Clients)
            {
                recvers[index] = (UInt32) child.ID;
                index++;
            }

            ChatCom.SendVicinity((UInt32) ID, 0, recvers, message);
        }
        #endregion
    }
}