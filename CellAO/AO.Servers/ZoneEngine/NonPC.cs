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

//region Bandit strikes again!!!!

#region Usings...
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using AO.Core;
using ZoneEngine.Misc;
using ZoneEngine.Packets;
#endregion

namespace ZoneEngine
{
    public class NonPC : Character
    {
        /// <summary>
        /// List of waypoints to walk
        /// </summary>
        public List<AOCoord> Waypoints = new List<AOCoord>();

        /// <summary>
        /// Mesh list
        /// </summary>
        public List<AOMeshs> Meshs;

        /// <summary>
        /// Additional Mesh list
        /// </summary>
        public List<AOAddMeshs> AdditionalMeshs;

        /// <summary>
        /// Weaponpairs
        /// </summary>
        public List<AOWeaponpairs> Weaponpairs;

        public KnuBotClass KnuBot;

        public string Hash;

        /// <summary>
        /// Create a new NonPC
        /// </summary>
        /// <param name="_id">Unique ID</param>
        /// <param name="_playfield">Plafield number</param>
        public NonPC(int _id, int _playfield)
            : base(_id, _playfield)
        {
            ID = _id;
            PlayField = _playfield;
            Type = 50000;
            ourType = 1;
            Meshs = new List<AOMeshs>();
            AdditionalMeshs = new List<AOAddMeshs>();
            Weaponpairs = new List<AOWeaponpairs>();
            Stats = new Character_Stats(this);
            startup = false;
            addHPNPtick();
            dontdotimers = false;
        }

        public NonPC()
        {
        }

        #region Waypoints (read/write)
        /// <summary>
        /// Read waypoints from database
        /// TODO: catch exceptions
        /// </summary>
        public void readWaypointsfromSQL()
        {
            SqlWrapper ms = new SqlWrapper();
            AOCoord m_wp;

            DataTable dt =
                ms.ReadDT("SELECT * FROM " + getSQLTablefromDynelType() + "waypoints WHERE ID=" + ID.ToString());

            foreach (DataRow row in dt.Rows)
            {
                m_wp = new AOCoord((Single) row["X"], (Single) row["Y"], (Single) row["Z"]);
                Waypoints.Add(m_wp);
            }
        }

        /// <summary>
        /// Write waypoints to database
        /// TODO: catch exceptions, replace the string formats with something more convenient
        /// </summary>
        public void writeWaypointstoSQL()
        {
            SqlWrapper ms = new SqlWrapper();
            int count;

            ms.SqlDelete("DELETE FROM " + getSQLTablefromDynelType() + "waypoints WHERE ID=" + ID.ToString());

            for (count = 0; count < Waypoints.Count; count++)
            {
                ms.SqlInsert("INSERT INTO " + getSQLTablefromDynelType() + "waypoints VALUES (" + ID.ToString() + "," +
                             PlayField.ToString() + ","
                             + String.Format(CultureInfo.InvariantCulture, "'{0}'", Waypoints[count].x) + ","
                             + String.Format(CultureInfo.InvariantCulture, "'{0}'", Waypoints[count].y) + ","
                             + String.Format(CultureInfo.InvariantCulture, "'{0}'", Waypoints[count].z) + ")");
            }
            if (Waypoints.Count > 0)
            {
            }
        }
        #endregion

        #region Weaponpairs (read/write)
        /// <summary>
        /// Write weaponpairs to database
        /// TODO: catch exceptions
        /// </summary>
        public void writeWeaponpairstoSQL()
        {
            SqlWrapper Sql = new SqlWrapper();
            Sql.SqlDelete("DELETE FROM " + getSQLTablefromDynelType() + "weaponpairs WHERE ID=" + ID.ToString() +
                          " AND playfield=" + PlayField.ToString() + ";");
            int c;
            for (c = 0; c < Weaponpairs.Count; c++)
            {
                Sql.SqlInsert("INSERT INTO " + getSQLTablefromDynelType() + "weaponpairs VALUES (" + ID.ToString() + "," +
                              PlayField.ToString() + "," + Weaponpairs[c].value1.ToString() + "," +
                              Weaponpairs[c].value2.ToString() + "," + Weaponpairs[c].value3.ToString() + "," +
                              Weaponpairs[c].value4.ToString() + ");");
            }
        }

        /// <summary>
        /// Read weaponpairs from database
        /// TODO: catch exceptions
        /// </summary>
        public void readWeaponpairsfromSQL()
        {
            AOWeaponpairs m_wp;
            SqlWrapper Sql = new SqlWrapper();
            Weaponpairs.Clear();
            DataTable dt =
                Sql.ReadDT("SELECT * FROM " + getSQLTablefromDynelType() + "weaponpairs WHERE ID=" + ID.ToString() +
                           " AND playfield=" + PlayField.ToString());

            foreach (DataRow row in dt.Rows)
            {
                m_wp = new AOWeaponpairs();
                m_wp.value1 = (Int32) row["value1"];
                m_wp.value2 = (Int32) row["value2"];
                m_wp.value3 = (Int32) row["value3"];
                m_wp.value4 = (Int32) row["value4"];
                Weaponpairs.Add(m_wp);
            }
        }
        #endregion

        #region Meshs (read/write)
        /// <summary>
        /// Write meshs to database
        /// TODO: catch exceptions
        /// </summary>
        public void writeMeshstoSQL()
        {
            SqlWrapper Sql = new SqlWrapper();
            Sql.SqlDelete("DELETE FROM " + getSQLTablefromDynelType() + "meshs WHERE ID=" + ID.ToString() +
                          " AND playfield=" + PlayField.ToString());
            int c;
            for (c = 0; c < Meshs.Count; c++)
            {
                Sql.SqlInsert("INSERT INTO " + getSQLTablefromDynelType() + "meshs VALUES (" + ID.ToString() + "," +
                              PlayField.ToString() + "," + Meshs[c].Position.ToString() + "," + Meshs[c].Mesh.ToString() +
                              "," + Meshs[c].OverrideTexture.ToString() + ")");
            }
        }

        /// <summary>
        /// Read meshs from database
        /// TODO: catch exceptions
        /// </summary>
        public void readMeshsfromSQL()
        {
            SqlWrapper Sql = new SqlWrapper();
            Meshs.Clear();
            AOMeshs m_m;
            DataTable dt =
                Sql.ReadDT("SELECT * from " + getSQLTablefromDynelType() + "meshs WHERE ID=" + ID.ToString() +
                           " AND playfield=" + PlayField.ToString());

            foreach (DataRow row in dt.Rows)
            {
                m_m = new AOMeshs();
                m_m.Position = (Int32) row["meshvalue1"];
                m_m.Mesh = (Int32) row["meshvalue2"];
                m_m.OverrideTexture = (Int32) row["meshvalue3"];
            }
        }
        #endregion

        #region Write Main Stats to SQL
        /// <summary>
        /// Write main stats to database
        /// TODO: catch exceptions, maybe change this thing to REPLACE INTO
        /// </summary>
        public void writeMainStatstoSQL()
        {
            string sqlquery = "UPDATE " + getSQLTablefromDynelType() + " SET Name='" + AddSlashes(Name) + "'";
            foreach (AOTextures at in Textures)
            {
                sqlquery += ",Textures" + at.place.ToString() + "=" + at.Texture.ToString();
            }
            sqlquery += " WHERE ID=" + ID.ToString();

            SqlWrapper Sql = new SqlWrapper();
            Sql.SqlUpdate(sqlquery);
        }
        #endregion

        #region Add NPC to db
        /// <summary>
        /// Write newly created NPC to database
        /// </summary>
        public void AddToDB()
        {
            SqlWrapper Sql = new SqlWrapper();
            Sql.SqlInsert("INSERT INTO " + getSQLTablefromDynelType() + " (ID, Playfield) VALUES (" + ID.ToString() +
                          "," + PlayField.ToString() + ")");
            writeCoordinatestoSQL();
            writeHeadingtoSQL();
            writeMainStatstoSQL();

            WriteStats();
            writeInventorytoSQL();
            writeNanostoSQL();
            writeWaypointstoSQL();
            writeWeaponpairstoSQL();
            writeMeshstoSQL();
        }
        #endregion

        #region AddtoCache
        /// <summary>
        /// Add NPC to Mob-Cache
        /// </summary>
        public void AddToCache()
        {
            Program.zoneServer.Monsters.Add(this);
        }
        #endregion

        #region Remove NPC from db
        /// <summary>
        /// Purge NPC from database
        /// TODO: catch exceptions
        /// </summary>
        public void RemoveFromDB()
        {
            SqlWrapper Sql = new SqlWrapper();
            Sql.SqlDelete("DELETE FROM " + getSQLTablefromDynelType() + " WHERE ID=" + ID.ToString() + " AND playfield=" +
                          PlayField.ToString());
            Sql.SqlDelete("DELETE FROM " + getSQLTablefromDynelType() + "inventory WHERE ID=" + ID.ToString() +
                          " AND playfield=" + PlayField.ToString());
            Sql.SqlDelete("DELETE FROM " + getSQLTablefromDynelType() + "activenanos WHERE ID=" + ID.ToString() +
                          " AND playfield=" + PlayField.ToString());
            Sql.SqlDelete("DELETE FROM " + getSQLTablefromDynelType() + "meshs WHERE ID=" + ID.ToString() +
                          " AND playfield=" + PlayField.ToString());
            Sql.SqlDelete("DELETE FROM " + getSQLTablefromDynelType() + "timers WHERE ID=" + ID.ToString() +
                          " AND playfield=" + PlayField.ToString());
            Sql.SqlDelete("DELETE FROM " + getSQLTablefromDynelType() + "waypoints WHERE ID=" + ID.ToString() +
                          " AND playfield=" + PlayField.ToString());
            Sql.SqlDelete("DELETE FROM " + getSQLTablefromDynelType() + "weaponpairs WHERE ID=" + ID.ToString() +
                          " AND playfield=" + PlayField.ToString());
            Sql.SqlDelete("DELETE FROM " + getSQLTablefromDynelType() + "_stats WHERE ID=" + ID.ToString() +
                          " AND playfield=" + PlayField.ToString());
        }
        #endregion

        #region Despawn
        /// <summary>
        /// Despawn a NPC, no further action taken
        /// </summary>
        public void Despawn()
        {
            Packets.Despawn.DespawnPacket(ID);
        }
        #endregion

        #region RemoveFromCache
        /// <summary>
        /// Remove NPC from Cache
        /// </summary>
        public void RemoveFromCache()
        {
            Program.zoneServer.Monsters.Remove(this);
        }
        #endregion

        #region SpawntoPlayfield
        /// <summary>
        /// Spawns this NPC to every client in given playfield.
        /// </summary>
        /// <param name="playfield">Playfield to spawn to</param>
        public void SpawnToPlayfield(int playfield)
        {
            NPCSpawn.NPCSpawntoClient(this, null, true);
        }
        #endregion

        #region SpawnToClient
        /// <summary>
        /// Spawn NPC to specified client
        /// </summary>
        /// <param name="cli"></param>
        public void SpawnToClient(Client cli)
        {
            NPCSpawn.NPCSpawntoClient(this, cli, false);
        }
        #endregion

        #region WriteCoordinatestoSQL
        /// <summary>
        /// Write NPC coordinates to database
        /// TODO: catch exceptions, replace string.formats
        /// </summary>
        public new void writeCoordinatestoSQL()
        {
            SqlWrapper Sql = new SqlWrapper();
            Sql.SqlUpdate("UPDATE " + getSQLTablefromDynelType() + " SET playfield=" + PlayField.ToString() + ", X=" +
                          String.Format(CultureInfo.InvariantCulture, "'{0}'", Coordinates.x) + ", Y=" +
                          String.Format(CultureInfo.InvariantCulture, "'{0}'", Coordinates.y) + ", Z=" +
                          String.Format(CultureInfo.InvariantCulture, "'{0}'", Coordinates.z) + " WHERE ID=" +
                          ID.ToString() + ";");
        }
        #endregion

        #region readTexturesfromSQL
        // New one with id AND playfield as filter
        /// <summary>
        /// Read NPC textures from database
        /// </summary>
        public new void readTexturesfromSQL()
        {
            SqlWrapper ms = new SqlWrapper();
            AOTextures m_tex;
            Textures.Clear();

            DataTable dt =
                ms.ReadDT("SELECT textures0, textures1, textures2, textures3, textures4 from " +
                          getSQLTablefromDynelType() + " WHERE ID=" + ID.ToString() + " AND playfield=" +
                          PlayField.ToString());
            if (dt.Rows.Count > 0)
            {
                m_tex = new AOTextures(0, (Int32) dt.Rows[0]["textures0"]);
                Textures.Add(m_tex);

                m_tex = new AOTextures(1, (Int32) dt.Rows[0]["textures1"]);
                Textures.Add(m_tex);

                m_tex = new AOTextures(2, (Int32) dt.Rows[0]["textures2"]);
                Textures.Add(m_tex);

                m_tex = new AOTextures(3, (Int32) dt.Rows[0]["textures3"]);
                Textures.Add(m_tex);

                m_tex = new AOTextures(4, (Int32) dt.Rows[0]["textures4"]);
                Textures.Add(m_tex);
            }
        }
        #endregion

        #region readTexturesfromSQLfast
        // New one with id AND playfield as filter
        /// <summary>
        /// Read textures from database row
        /// </summary>
        /// <param name="row">database row</param>
        public void readTexturesfromSQLfast(DataRow row)
        {
            Textures.Clear();
            AOTextures m_tex;
            m_tex = new AOTextures(0, (Int32) row["textures0"]);
            Textures.Add(m_tex);

            m_tex = new AOTextures(1, (Int32) row["textures1"]);
            Textures.Add(m_tex);

            m_tex = new AOTextures(2, (Int32) row["textures2"]);
            Textures.Add(m_tex);

            m_tex = new AOTextures(3, (Int32) row["textures3"]);
            Textures.Add(m_tex);

            m_tex = new AOTextures(4, (Int32) row["textures4"]);
            Textures.Add(m_tex);
        }
        #endregion

        #region readcoordsheadingfast
        /// <summary>
        /// Read coordinates from database row
        /// </summary>
        /// <param name="row">database row</param>
        public void readcoordsheadingfast(DataRow row)
        {
            Coordinates.x = (Single) row["X"];
            Coordinates.y = (Single) row["Y"];
            Coordinates.z = (Single) row["Z"];
            PlayField = (Int32) row["Playfield"];
            Heading.x = (Single) row["HeadingX"];
            Heading.y = (Single) row["HeadingY"];
            Heading.z = (Single) row["HeadingZ"];
            Heading.w = (Single) row["HeadingW"];
        }
        #endregion

        #region LoadTemplate
        /// <summary>
        /// Read NPC template
        /// TODO: catch exceptions
        /// </summary>
        /// <param name="hash">Hash string</param>
        /// <param name="level">NPC level</param>
        public void LoadTemplate(string hash, uint level)
        {
            SqlWrapper ms = new SqlWrapper();
            DataTable dt = ms.ReadDT("SELECT * FROM mobtemplate where hash='" + hash + "'");
            if (dt.Rows.Count > 0)
            {
                startup = true;
                level = (UInt32) Math.Min(Math.Max(level, (Int32) dt.Rows[0]["MinLvl"]), (Int32) dt.Rows[0]["MaxLvl"]);
                Stats.Level.Set(level);
                Stats.Side.Set((Int32) dt.Rows[0]["Side"]);
                Stats.Breed.Set((Int32) dt.Rows[0]["Breed"]);
                Stats.Fatness.Set((Int32) dt.Rows[0]["Fatness"]);
                Stats.Sex.Set((Int32) dt.Rows[0]["Sex"]);
                Stats.Race.Set((Int32) dt.Rows[0]["Race"]);
                Name = (string) dt.Rows[0]["Name"];
                Stats.Flags.Set((Int32) dt.Rows[0]["Flags"]);
                Stats.NPCFamily.Set((Int32) dt.Rows[0]["NPCFamily"]);
                Stats.Health.Set((Int32) dt.Rows[0]["Health"]);
                Stats.Life.Set((Int32) dt.Rows[0]["Health"]);
                Stats.MonsterData.Set((Int32) dt.Rows[0]["Monsterdata"]);
                Stats.MonsterScale.Set((Int32) dt.Rows[0]["MonsterScale"]);
                Hash = hash;

                AOTextures m_t = new AOTextures(0, (Int32) dt.Rows[0]["TextureHands"]);
                Textures.Add(m_t);

                m_t = new AOTextures(1, (Int32) dt.Rows[0]["TextureBody"]);
                Textures.Add(m_t);

                m_t = new AOTextures(2, (Int32) dt.Rows[0]["TextureFeet"]);
                Textures.Add(m_t);

                m_t = new AOTextures(3, (Int32) dt.Rows[0]["TextureArms"]);
                Textures.Add(m_t);

                m_t = new AOTextures(4, (Int32) dt.Rows[0]["TextureLegs"]);
                Textures.Add(m_t);
                startup = false;
            }
        }
        #endregion

        #region Add and save Waypoints
        /// <summary>
        /// Add a waypoint and save all waypoints to database
        /// </summary>
        /// <param name="coord"></param>
        public void AddWaypoint(AOCoord coord)
        {
            Waypoints.Add(coord);
            writeWaypointstoSQL();
        }
        #endregion

        #region Send text to vicinity
        /// <summary>
        /// NPC talks to vicinity (could be enhanced with a second parameter radius)
        /// </summary>
        /// <param name="message">Message to send</param>
        public new void SendVicinityChat(string message)
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

        #region ReadStatsfast
        /// <summary>
        /// Read stats from datatable
        /// </summary>
        /// <param name="dt">Datatable object</param>
        /// <param name="startcount">start at row</param>
        /// <returns></returns>
        public int ReadStatsfast(DataTable dt, int startcount)
        {
            int count = startcount;
            while ((count < dt.Rows.Count) && ((Int32) dt.Rows[count][0] == ID))
            {
                Stats.Set((Int32) dt.Rows[count][2], (UInt32) (Int32) dt.Rows[count][3]);
                count++;
            }
            return count;
        }
        #endregion

        #region readInventoryfromSQLfast
        /// <summary>
        /// Read NPC's inventory from datable object
        /// TODO: actually READ ;)
        /// </summary>
        /// <param name="dt">Datatable object</param>
        /// <param name="startcount">start at row</param>
        /// <returns></returns>
        public int readInventoryfromSQLfast(DataTable dt, int startcount)
        {
            int count = startcount;
            while ((count < dt.Rows.Count) && ((Int32) dt.Rows[count][0] == ID))
            {
                count++;
            }
            return count;
        }
        #endregion

        #region KnuBot Stuff
        public void KnuBotAnswer(Character ch, Int32 answer)
        {
            if (KnuBot != null)
            {
                KnuBot.KnuBotAnswer(ch, answer);
            }
        }

        public void KnuBotOpenChatWindow(Character character)
        {
            if (KnuBot != null)
            {
                KnuBot.KnuBotOpenChatWindow(character);
            }
        }

        public void KnuBotCloseChatWindow(Character character)
        {
            if (KnuBot != null)
            {
                KnuBot.KnuBotCloseChatWindow(character);
            }
        }

        public void KnuBotDeclineTrade(Character character)
        {
            if (KnuBot != null)
            {
                KnuBot.KnuBotDeclineTrade(character);
            }
        }

        public void KnuBotAcceptTrade(Character character)
        {
            if (KnuBot != null)
            {
                KnuBot.KnuBotAcceptTrade(character);
            }
        }

        public void KnuBotFinishTrade(Character character, int decline)
        {
            if (decline == 1)
            {
                KnuBotDeclineTrade(character);
            }
            else
            {
                KnuBotAcceptTrade(character);
            }
        }

        public void KnuBotStartTrade(Character character)
        {
            if (KnuBot != null)
            {
                KnuBot.KnuBotStartTrade(character);
            }
        }

        public void KnuBotTrade(Character character, AOItem item)
        {
            if (KnuBot != null)
            {
                KnuBot.KnuBotTrade(character, item);
            }
        }
        #endregion
    }
}