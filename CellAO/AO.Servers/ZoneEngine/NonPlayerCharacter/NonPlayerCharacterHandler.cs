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

namespace ZoneEngine.NonPlayerCharacter
{
    using System;
    using System.Data;

    using AO.Core;

    public static class NonPlayerCharacterHandler
    {
        /// <summary>
        /// Spawns new NPC to world.
        /// </summary>
        /// <param name="cli">Client that spawns NPC</param>
        /// <param name="hash">Hash of NPC to spawn</param>
        /// <param name="level">Level of NPC to spawn</param>
        /// <returns></returns>
        public static void SpawnMonster(Client cli, string hash, uint level)
        {
            NonPlayerCharacterClass mMonster = new NonPlayerCharacterClass(0, 0); //(cli, hash, level);
            mMonster.LoadTemplate(hash, level);
            mMonster.PurgeTimer(-1);
            mMonster.PurgeTimer(0);
            mMonster.PurgeTimer(1);
            mMonster.ID = FindNextFreeId();
            mMonster.rawCoord = cli.Character.rawCoord;
            mMonster.rawHeading = cli.Character.rawHeading;
            mMonster.PlayField = cli.Character.PlayField;
            mMonster.AddHpnpTick();
            if (String.IsNullOrEmpty(mMonster.Name))
            {
                return;
            }
            mMonster.CalculateSkills();
            mMonster.AddToCache();
            mMonster.SpawnToPlayfield(mMonster.PlayField);
        }

        /// <summary>
        /// Spawns all NPCs in playfield to given client.
        /// </summary>
        /// <param name="cli">Client to spawn NPCs to</param>
        /// <param name="playfield">Playfield</param>
        /// <returns></returns>
        public static void GetMonstersInPF(Client cli, int playfield)
        {
            foreach (NonPlayerCharacterClass mMonster in Program.zoneServer.Monsters)
            {
                if (mMonster.PlayField != playfield)
                {
                    continue;
                }
                mMonster.SpawnToClient(cli);
            }
        }

        public static int FindNextFreeId()
        {
            int freeID = 100000; // minimum ID for mobs
            foreach (NonPlayerCharacterClass mob in Program.zoneServer.Monsters)
            {
                freeID = Math.Max(freeID, mob.ID);
            }
            freeID++;
            return freeID;
        }

        /// <summary>
        /// Removes NPC from world.
        /// </summary>
        /// <param name="cli">Client that despawned NPC</param>
        /// <param name="monster">Monster to remove</param>
        /// <returns></returns>
        public static void DespawnMonster(Identity monster)
        {
            if (monster.Type != 50000)
            {
                return;
            }
            foreach (NonPlayerCharacterClass mMonster in Program.zoneServer.Monsters)
            {
                if (mMonster.ID != monster.Instance)
                {
                    continue;
                }
                mMonster.Despawn();
                mMonster.RemoveFromCache();
                break;
            }
        }

        // Call this _only_ on server startup!
        /// <summary>
        /// Reads all NPCs from database and adds them to servers list.
        /// </summary>
        /// <returns>Number of NPCs loaded</returns>
        public static int CacheAllFromDB()
        {
            int npcCount = 0;
            long counter;
            int npcall;
            NonPlayerCharacterClass mMonster;
            SqlWrapper msql = new SqlWrapper();
            AOCoord coord;
            byte[] btd = new byte[4];
            AOWeaponpairs tempwp;
            AONano temprn;
            AOMeshs tempm;
            AOAddMeshs tempa;

            // TODO:COUNT
            string Sql = "SELECT count(*) FROM `mobspawns`";
            npcall = msql.SqlCount(Sql);

            Console.Write("Reading spawns: 0/" + npcall.ToString());
            Sql = "SELECT * FROM `mobspawns`";
            DataTable dt = msql.ReadDatatable(Sql);
            msql = new SqlWrapper();
            DataTable dtstats = msql.ReadDatatable("SELECT * from mobspawns_stats ORDER BY id, stat ASC");
            msql = new SqlWrapper();
            DataTable dtinventory = msql.ReadDatatable("SELECT * from mobspawnsinventory order by id, placement ASC");
            int statcount = 0;
            int invcount = 0;
            foreach (DataRow row in dt.Rows)
            {
                mMonster = new NonPlayerCharacterClass(0, 0);
                mMonster.Starting = true;
                mMonster.ID = (Int32)row["ID"];

                mMonster.PlayField = (Int32)row["Playfield"];
                mMonster.Name = (string)row["Name"]
#if DEBUG
                                + " " + mMonster.ID.ToString() // ID is for debug purpose only
#endif
                    ;
                mMonster.readcoordsheadingfast(row);
                statcount = mMonster.ReadStatsfast(dtstats, statcount);
                invcount = mMonster.readInventoryfromSqlfast(dtinventory, invcount);
                //                mMonster.readMeshsfromSql();
                //                mMonster.readNanosfromSql();
                //                mMonster.readTimersfromSql();
                //                mMonster.readWaypointsfromSql();
                //                mMonster.readWeaponpairsfromSql();

                mMonster.readTexturesfromSqlfast(row);
                byte[] tempb;
                if (!(row[15] is DBNull))
                {
                    tempb = (byte[])row[15]; // Waypoints
                    counter = 0;
                    while (counter < tempb.Length)
                    {
                        coord = new AOCoord();
                        coord.x = BitConverter.ToSingle(tempb, (int)counter);
                        counter += 4;
                        coord.y = BitConverter.ToSingle(tempb, (int)counter);
                        counter += 4;
                        coord.z = BitConverter.ToSingle(tempb, (int)counter);
                        counter += 4;
                        mMonster.Waypoints.Add(coord);
                    }
                }

                if (!(row[16] is DBNull))
                {
                    tempb = (byte[])row[16]; // Weaponpairs
                    counter = 0;
                    while (counter < tempb.Length)
                    {
                        tempwp = new AOWeaponpairs();
                        tempwp.value1 = BitConverter.ToInt32(tempb, (int)counter);
                        counter += 4;
                        tempwp.value2 = BitConverter.ToInt32(tempb, (int)counter);
                        counter += 4;
                        tempwp.value3 = BitConverter.ToInt32(tempb, (int)counter);
                        counter += 4;
                        tempwp.value4 = BitConverter.ToInt32(tempb, (int)counter);
                        counter += 4;
                        mMonster.Weaponpairs.Add(tempwp);
                    }
                }

                if (!(row[17] is DBNull))
                {
                    tempb = (byte[])row[17]; // Running Nanos
                    counter = 0;
                    while (counter < tempb.Length)
                    {
                        temprn = new AONano();

                        temprn.Nanotype = BitConverter.ToInt32(tempb, (int)counter);
                        counter += 4;
                        temprn.Instance = BitConverter.ToInt32(tempb, (int)counter);
                        counter += 4;
                        temprn.Value3 = BitConverter.ToInt32(tempb, (int)counter);
                        counter += 4;
                        temprn.Time1 = BitConverter.ToInt32(tempb, (int)counter);
                        counter += 4;
                        temprn.Time2 = BitConverter.ToInt32(tempb, (int)counter);
                        counter += 4;
                        mMonster.ActiveNanos.Add(temprn);
                    }
                }

                if (!(row[18] is DBNull))
                {
                    counter = 0;
                    tempb = (byte[])row[18]; // Meshs
                    while (counter < tempb.Length)
                    {
                        tempm = new AOMeshs();
                        tempm.Position = BitConverter.ToInt32(tempb, (int)counter);
                        counter += 4;
                        tempm.Mesh = BitConverter.ToInt32(tempb, (int)counter);
                        counter += 4;
                        tempm.OverrideTexture = BitConverter.ToInt32(tempb, (int)counter);
                        counter += 4;

                        mMonster.Meshs.Add(tempm);
                        tempm = null;
                    }
                }

                if (!(row[19] is DBNull))
                {
                    counter = 0;
                    tempb = (byte[])row[19]; // Additional Meshs
                    while (counter < tempb.Length)
                    {
                        tempa = new AOAddMeshs();
                        tempa.position = tempb[counter++];
                        tempa.meshvalue1 = BitConverter.ToInt32(tempb, (int)counter);
                        counter += 4;
                        tempa.meshvalue2 = BitConverter.ToInt32(tempb, (int)counter);
                        counter += 4;
                        tempa.priority = tempb[counter++];

                        mMonster.AdditionalMeshs.Add(tempa);
                        tempa = null;
                    }
                }
                mMonster.Starting = false;

                Program.zoneServer.Monsters.Add(mMonster);
                npcCount += 1;
                if ((npcCount % 100) == 0)
                {
                    Console.Write("\rReading spawns: " + npcCount.ToString() + "/" + npcall.ToString());
                }
            }

            Console.Write("\r                                                    \r");
            dt = null;
            dtstats = null;
            dtinventory = null;
            return npcCount;
        }
    }
}