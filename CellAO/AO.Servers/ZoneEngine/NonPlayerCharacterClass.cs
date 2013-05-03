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

//region Bandit strikes again!!!!

#region Usings...
#endregion

namespace ZoneEngine
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;

    using AO.Core;

    using SmokeLounge.AOtomation.Messaging.GameData;

    using ZoneEngine.Misc;
    using ZoneEngine.Packets;

    public class NonPlayerCharacterClass : Character
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
        public NonPlayerCharacterClass(Identity _id, int _playfield)
            : base(_id, _playfield)
        {
            this.Id = _id;
            this.PlayField = _playfield;
            this.OurType = 1;
            this.Meshs = new List<AOMeshs>();
            this.AdditionalMeshs = new List<AOAddMeshs>();
            this.Weaponpairs = new List<AOWeaponpairs>();
            this.Stats = new CharacterStats(this);
            this.Starting = false;
            this.AddHpnpTick();
            this.DoNotDoTimers = false;
        }

        public NonPlayerCharacterClass()
        {
        }

        #region Waypoints (read/write)
        /// <summary>
        /// Read waypoints from database
        /// TODO: catch exceptions
        /// </summary>
        public void readWaypointsfromSql()
        {
            SqlWrapper ms = new SqlWrapper();
            AOCoord m_wp;

            DataTable dt =
                ms.ReadDatatable(
                    "SELECT * FROM " + this.GetSqlTablefromDynelType() + "waypoints WHERE ID=" + this.Id.ToString());

            foreach (DataRow row in dt.Rows)
            {
                m_wp = new AOCoord((Single)row["X"], (Single)row["Y"], (Single)row["Z"]);
                this.Waypoints.Add(m_wp);
            }
        }

        /// <summary>
        /// Write waypoints to database
        /// TODO: catch exceptions, replace the string formats with something more convenient
        /// </summary>
        public void writeWaypointstoSql()
        {
            SqlWrapper ms = new SqlWrapper();
            int count;

            ms.SqlDelete("DELETE FROM " + this.GetSqlTablefromDynelType() + "waypoints WHERE ID=" + this.Id.ToString());

            for (count = 0; count < this.Waypoints.Count; count++)
            {
                ms.SqlInsert(
                    "INSERT INTO " + this.GetSqlTablefromDynelType() + "waypoints VALUES (" + this.Id.ToString() + ","
                    + this.PlayField.ToString() + ","
                    + String.Format(CultureInfo.InvariantCulture, "'{0}'", this.Waypoints[count].x) + ","
                    + String.Format(CultureInfo.InvariantCulture, "'{0}'", this.Waypoints[count].y) + ","
                    + String.Format(CultureInfo.InvariantCulture, "'{0}'", this.Waypoints[count].z) + ")");
            }
            if (this.Waypoints.Count > 0)
            {
            }
        }
        #endregion

        #region Weaponpairs (read/write)
        /// <summary>
        /// Write weaponpairs to database
        /// TODO: catch exceptions
        /// </summary>
        public void WriteWeaponpairstoSql()
        {
            SqlWrapper Sql = new SqlWrapper();
            Sql.SqlDelete(
                "DELETE FROM " + this.GetSqlTablefromDynelType() + "weaponpairs WHERE ID=" + this.Id.ToString()
                + " AND playfield=" + this.PlayField.ToString() + ";");
            int c;
            for (c = 0; c < this.Weaponpairs.Count; c++)
            {
                Sql.SqlInsert(
                    "INSERT INTO " + this.GetSqlTablefromDynelType() + "weaponpairs VALUES (" + this.Id.ToString() + ","
                    + this.PlayField.ToString() + "," + this.Weaponpairs[c].value1.ToString() + ","
                    + this.Weaponpairs[c].value2.ToString() + "," + this.Weaponpairs[c].value3.ToString() + ","
                    + this.Weaponpairs[c].value4.ToString() + ");");
            }
        }

        /// <summary>
        /// Read weaponpairs from database
        /// TODO: catch exceptions
        /// </summary>
        public void readWeaponpairsfromSql()
        {
            AOWeaponpairs m_wp;
            SqlWrapper Sql = new SqlWrapper();
            this.Weaponpairs.Clear();
            DataTable dt =
                Sql.ReadDatatable(
                    "SELECT * FROM " + this.GetSqlTablefromDynelType() + "weaponpairs WHERE ID=" + this.Id.ToString()
                    + " AND playfield=" + this.PlayField.ToString());

            foreach (DataRow row in dt.Rows)
            {
                m_wp = new AOWeaponpairs();
                m_wp.value1 = (Int32)row["value1"];
                m_wp.value2 = (Int32)row["value2"];
                m_wp.value3 = (Int32)row["value3"];
                m_wp.value4 = (Int32)row["value4"];
                this.Weaponpairs.Add(m_wp);
            }
        }
        #endregion

        #region Meshs (read/write)
        /// <summary>
        /// Write meshs to database
        /// TODO: catch exceptions
        /// </summary>
        public void writeMeshstoSql()
        {
            SqlWrapper Sql = new SqlWrapper();
            Sql.SqlDelete(
                "DELETE FROM " + this.GetSqlTablefromDynelType() + "meshs WHERE ID=" + this.Id.ToString()
                + " AND playfield=" + this.PlayField.ToString());
            int c;
            for (c = 0; c < this.Meshs.Count; c++)
            {
                Sql.SqlInsert(
                    "INSERT INTO " + this.GetSqlTablefromDynelType() + "meshs VALUES (" + this.Id.ToString() + ","
                    + this.PlayField.ToString() + "," + this.Meshs[c].Position.ToString() + ","
                    + this.Meshs[c].Mesh.ToString() + "," + this.Meshs[c].OverrideTexture.ToString() + ")");
            }
        }

        /// <summary>
        /// Read meshs from database
        /// TODO: catch exceptions
        /// </summary>
        public void readMeshsfromSql()
        {
            SqlWrapper Sql = new SqlWrapper();
            this.Meshs.Clear();
            AOMeshs m_m;
            DataTable dt =
                Sql.ReadDatatable(
                    "SELECT * from " + this.GetSqlTablefromDynelType() + "meshs WHERE ID=" + this.Id.ToString()
                    + " AND playfield=" + this.PlayField.ToString());

            foreach (DataRow row in dt.Rows)
            {
                m_m = new AOMeshs();
                m_m.Position = (Int32)row["meshvalue1"];
                m_m.Mesh = (Int32)row["meshvalue2"];
                m_m.OverrideTexture = (Int32)row["meshvalue3"];
            }
        }
        #endregion

        #region Write Main Stats to Sql
        /// <summary>
        /// Write main stats to database
        /// TODO: catch exceptions, maybe change this thing to REPLACE INTO
        /// </summary>
        public void writeMainStatstoSql()
        {
            string sqlquery = "UPDATE " + this.GetSqlTablefromDynelType() + " SET Name='" + AddSlashes(this.Name) + "'";
            foreach (AOTextures at in this.Textures)
            {
                sqlquery += ",Textures" + at.place.ToString() + "=" + at.Texture.ToString();
            }
            sqlquery += " WHERE ID=" + this.Id.ToString();

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
            Sql.SqlInsert(
                "INSERT INTO " + this.GetSqlTablefromDynelType() + " (ID, Playfield) VALUES (" + this.Id.ToString()
                + "," + this.PlayField.ToString() + ")");
            this.WriteCoordinatesToSql();
            this.WriteHeadingToSql();
            this.writeMainStatstoSql();

            this.WriteStats();
            this.WriteInventoryToSql();
            this.WriteNanosToSql();
            this.writeWaypointstoSql();
            this.WriteWeaponpairstoSql();
            this.writeMeshstoSql();
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
            Sql.SqlDelete(
                "DELETE FROM " + this.GetSqlTablefromDynelType() + " WHERE ID=" + this.Id.ToString() + " AND playfield="
                + this.PlayField.ToString());
            Sql.SqlDelete(
                "DELETE FROM " + this.GetSqlTablefromDynelType() + "inventory WHERE ID=" + this.Id.ToString()
                + " AND playfield=" + this.PlayField.ToString());
            Sql.SqlDelete(
                "DELETE FROM " + this.GetSqlTablefromDynelType() + "activenanos WHERE ID=" + this.Id.ToString()
                + " AND playfield=" + this.PlayField.ToString());
            Sql.SqlDelete(
                "DELETE FROM " + this.GetSqlTablefromDynelType() + "meshs WHERE ID=" + this.Id.ToString()
                + " AND playfield=" + this.PlayField.ToString());
            Sql.SqlDelete(
                "DELETE FROM " + this.GetSqlTablefromDynelType() + "timers WHERE ID=" + this.Id.ToString()
                + " AND playfield=" + this.PlayField.ToString());
            Sql.SqlDelete(
                "DELETE FROM " + this.GetSqlTablefromDynelType() + "waypoints WHERE ID=" + this.Id.ToString()
                + " AND playfield=" + this.PlayField.ToString());
            Sql.SqlDelete(
                "DELETE FROM " + this.GetSqlTablefromDynelType() + "weaponpairs WHERE ID=" + this.Id.ToString()
                + " AND playfield=" + this.PlayField.ToString());
            Sql.SqlDelete(
                "DELETE FROM " + this.GetSqlTablefromDynelType() + "_stats WHERE ID=" + this.Id.ToString()
                + " AND playfield=" + this.PlayField.ToString());
        }
        #endregion

        #region Despawn
        /// <summary>
        /// Despawn a NPC, no further action taken
        /// </summary>
        public void Despawn()
        {
            Packets.Despawn.DespawnPacket(this.Id);
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
            NonPlayerCharacterSpawn.SpawnNpcToClient(this, null, true);
        }
        #endregion

        #region SpawnToClient
        /// <summary>
        /// Spawn NPC to specified client
        /// </summary>
        /// <param name="cli"></param>
        public void SpawnToClient(Client cli)
        {
            NonPlayerCharacterSpawn.SpawnNpcToClient(this, cli, false);
        }
        #endregion

        #region readTexturesfromSql
        // New one with id AND playfield as filter
        /// <summary>
        /// Read NPC textures from database
        /// </summary>
        public void ReadTexturesfromSql()
        {
            SqlWrapper ms = new SqlWrapper();
            AOTextures m_tex;
            this.Textures.Clear();

            DataTable dt =
                ms.ReadDatatable(
                    "SELECT textures0, textures1, textures2, textures3, textures4 from "
                    + this.GetSqlTablefromDynelType() + " WHERE ID=" + this.Id.ToString() + " AND playfield="
                    + this.PlayField.ToString());
            if (dt.Rows.Count > 0)
            {
                m_tex = new AOTextures(0, (Int32)dt.Rows[0]["textures0"]);
                this.Textures.Add(m_tex);

                m_tex = new AOTextures(1, (Int32)dt.Rows[0]["textures1"]);
                this.Textures.Add(m_tex);

                m_tex = new AOTextures(2, (Int32)dt.Rows[0]["textures2"]);
                this.Textures.Add(m_tex);

                m_tex = new AOTextures(3, (Int32)dt.Rows[0]["textures3"]);
                this.Textures.Add(m_tex);

                m_tex = new AOTextures(4, (Int32)dt.Rows[0]["textures4"]);
                this.Textures.Add(m_tex);
            }
        }
        #endregion

        #region readTexturesfromSqlfast
        // New one with id AND playfield as filter
        /// <summary>
        /// Read textures from database row
        /// </summary>
        /// <param name="row">database row</param>
        public void readTexturesfromSqlfast(DataRow row)
        {
            this.Textures.Clear();
            AOTextures m_tex;
            m_tex = new AOTextures(0, (Int32)row["textures0"]);
            this.Textures.Add(m_tex);

            m_tex = new AOTextures(1, (Int32)row["textures1"]);
            this.Textures.Add(m_tex);

            m_tex = new AOTextures(2, (Int32)row["textures2"]);
            this.Textures.Add(m_tex);

            m_tex = new AOTextures(3, (Int32)row["textures3"]);
            this.Textures.Add(m_tex);

            m_tex = new AOTextures(4, (Int32)row["textures4"]);
            this.Textures.Add(m_tex);
        }
        #endregion

        #region readcoordsheadingfast
        /// <summary>
        /// Read coordinates from database row
        /// </summary>
        /// <param name="row">database row</param>
        public void readcoordsheadingfast(DataRow row)
        {
            this.Coordinates.x = (Single)row["X"];
            this.Coordinates.y = (Single)row["Y"];
            this.Coordinates.z = (Single)row["Z"];
            this.PlayField = (Int32)row["Playfield"];
            this.Heading.x = (Single)row["HeadingX"];
            this.Heading.y = (Single)row["HeadingY"];
            this.Heading.z = (Single)row["HeadingZ"];
            this.Heading.w = (Single)row["HeadingW"];
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
            DataTable dt = ms.ReadDatatable("SELECT * FROM mobtemplate where hash='" + hash + "'");
            if (dt.Rows.Count > 0)
            {
                this.Starting = true;
                level = (UInt32)Math.Min(Math.Max(level, (Int32)dt.Rows[0]["MinLvl"]), (Int32)dt.Rows[0]["MaxLvl"]);
                this.Stats.Level.Set(level);
                this.Stats.Side.Set((Int32)dt.Rows[0]["Side"]);
                this.Stats.Breed.Set((Int32)dt.Rows[0]["Breed"]);
                this.Stats.Fatness.Set((Int32)dt.Rows[0]["Fatness"]);
                this.Stats.Sex.Set((Int32)dt.Rows[0]["Sex"]);
                this.Stats.Race.Set((Int32)dt.Rows[0]["Race"]);
                this.Name = (string)dt.Rows[0]["Name"];
                this.Stats.Flags.Set((Int32)dt.Rows[0]["Flags"]);
                this.Stats.NpcFamily.Set((Int32)dt.Rows[0]["NPCFamily"]);
                this.Stats.Health.Set((Int32)dt.Rows[0]["Health"]);
                this.Stats.Life.Set((Int32)dt.Rows[0]["Health"]);
                this.Stats.MonsterData.Set((Int32)dt.Rows[0]["Monsterdata"]);
                this.Stats.MonsterScale.Set((Int32)dt.Rows[0]["MonsterScale"]);
                this.Hash = hash;

                AOTextures m_t = new AOTextures(0, (Int32)dt.Rows[0]["TextureHands"]);
                this.Textures.Add(m_t);

                m_t = new AOTextures(1, (Int32)dt.Rows[0]["TextureBody"]);
                this.Textures.Add(m_t);

                m_t = new AOTextures(2, (Int32)dt.Rows[0]["TextureFeet"]);
                this.Textures.Add(m_t);

                m_t = new AOTextures(3, (Int32)dt.Rows[0]["TextureArms"]);
                this.Textures.Add(m_t);

                m_t = new AOTextures(4, (Int32)dt.Rows[0]["TextureLegs"]);
                this.Textures.Add(m_t);
                this.Starting = false;
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
            this.Waypoints.Add(coord);
            this.writeWaypointstoSql();
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
                recvers[index] = (UInt32)child.Id.Instance;
                index++;
            }

            ChatCom.SendVicinity((UInt32)this.Id.Instance, 0, recvers, message);
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
            while ((count < dt.Rows.Count) && ((Int32)dt.Rows[count][0] == this.Id.Instance))
            {
                this.Stats.SetStatValueByName((Int32)dt.Rows[count][2], (UInt32)(Int32)dt.Rows[count][3]);
                count++;
            }
            return count;
        }
        #endregion

        #region readInventoryfromSqlfast
        /// <summary>
        /// Read NPC's inventory from datable object
        /// TODO: actually READ ;)
        /// </summary>
        /// <param name="dt">Datatable object</param>
        /// <param name="startcount">start at row</param>
        /// <returns></returns>
        public int readInventoryfromSqlfast(DataTable dt, int startcount)
        {
            int count = startcount;
            while ((count < dt.Rows.Count) && ((Int32)dt.Rows[count][0] == this.Id.Instance))
            {
                count++;
            }
            return count;
        }
        #endregion

        #region KnuBot Stuff
        public void KnuBotAnswer(Character ch, Int32 answer)
        {
            if (this.KnuBot != null)
            {
                this.KnuBot.KnuBotAnswer(ch, answer);
            }
        }

        public void KnuBotOpenChatWindow(Character character)
        {
            if (this.KnuBot != null)
            {
                this.KnuBot.KnuBotOpenChatWindow(character);
            }
        }

        public void KnuBotCloseChatWindow(Character character)
        {
            if (this.KnuBot != null)
            {
                this.KnuBot.KnuBotCloseChatWindow(character);
            }
        }

        public void KnuBotDeclineTrade(Character character)
        {
            if (this.KnuBot != null)
            {
                this.KnuBot.KnuBotDeclineTrade(character);
            }
        }

        public void KnuBotAcceptTrade(Character character)
        {
            if (this.KnuBot != null)
            {
                this.KnuBot.KnuBotAcceptTrade(character);
            }
        }

        public void KnuBotFinishTrade(Character character, int decline)
        {
            if (decline == 1)
            {
                this.KnuBotDeclineTrade(character);
            }
            else
            {
                this.KnuBotAcceptTrade(character);
            }
        }

        public void KnuBotStartTrade(Character character)
        {
            if (this.KnuBot != null)
            {
                this.KnuBot.KnuBotStartTrade(character);
            }
        }

        public void KnuBotTrade(Character character, AOItem item)
        {
            if (this.KnuBot != null)
            {
                this.KnuBot.KnuBotTrade(character, item);
            }
        }
        #endregion
    }
}