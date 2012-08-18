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
            mMonster.Id = FindNextFreeId();
            mMonster.RawCoord = cli.Character.RawCoord;
            mMonster.RawHeading = cli.Character.RawHeading;
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
        /// <param name="client">Client to spawn NPCs to</param>
        /// <param name="playfield">Playfield</param>
        /// <returns></returns>
        public static void SpawnMonstersInPlayfieldToClient(Client client, int playfield)
        {
            foreach (NonPlayerCharacterClass mMonster in Program.zoneServer.Monsters)
            {
                if (mMonster.PlayField != playfield)
                {
                    continue;
                }
                mMonster.SpawnToClient(client);
            }
        }

        public static int FindNextFreeId()
        {
            int freeID = 100000; // minimum ID for mobs
            foreach (NonPlayerCharacterClass mob in Program.zoneServer.Monsters)
            {
                freeID = Math.Max(freeID, mob.Id);
            }
            freeID++;
            return freeID;
        }

        /// <summary>
        /// Removes NPC from world.
        /// </summary>
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
                if (mMonster.Id != monster.Instance)
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
            SqlWrapper sqlWrapper = new SqlWrapper();

            // TODO:COUNT
            string sql = "SELECT count(*) FROM `mobspawns`";
            int numberOfNpc = sqlWrapper.SqlCount(sql);

            Console.Write("Reading spawns: 0/" + numberOfNpc.ToString());
            sql = "SELECT * FROM `mobspawns`";
            DataTable dt = sqlWrapper.ReadDatatable(sql);
            sqlWrapper = new SqlWrapper();
            DataTable dtstats = sqlWrapper.ReadDatatable("SELECT * from mobspawns_stats ORDER BY id, stat ASC");
            sqlWrapper = new SqlWrapper();
            DataTable dtinventory = sqlWrapper.ReadDatatable("SELECT * from mobspawnsinventory order by id, placement ASC");
            int statcount = 0;
            int invcount = 0;
            foreach (DataRow row in dt.Rows)
            {
                NonPlayerCharacterClass monster = new NonPlayerCharacterClass(0, 0)
                    { Starting = true, Id = (Int32)row["ID"], PlayField = (Int32)row["Playfield"] };

                monster.Name = (string)row["Name"]
#if DEBUG
                                + " " + monster.Id.ToString() // ID is for debug purpose only
#endif
                    ;
                monster.readcoordsheadingfast(row);
                statcount = monster.ReadStatsfast(dtstats, statcount);
                invcount = monster.readInventoryfromSqlfast(dtinventory, invcount);
                //                mMonster.readMeshsfromSql();
                //                mMonster.readNanosfromSql();
                //                mMonster.readTimersfromSql();
                //                mMonster.readWaypointsfromSql();
                //                mMonster.readWeaponpairsfromSql();

                monster.readTexturesfromSqlfast(row);
                byte[] bytes;
                long counter;
                if (!(row[15] is DBNull))
                {
                    bytes = (byte[])row[15]; // Waypoints
                    counter = 0;
                    while (counter < bytes.Length)
                    {
                        AOCoord aoCoord = new AOCoord();
                        aoCoord.x = BitConverter.ToSingle(bytes, (int)counter);
                        counter += 4;
                        aoCoord.y = BitConverter.ToSingle(bytes, (int)counter);
                        counter += 4;
                        aoCoord.z = BitConverter.ToSingle(bytes, (int)counter);
                        counter += 4;
                        monster.Waypoints.Add(aoCoord);
                    }
                }

                if (!(row[16] is DBNull))
                {
                    bytes = (byte[])row[16]; // Weaponpairs
                    counter = 0;
                    while (counter < bytes.Length)
                    {
                        AOWeaponpairs tempWeaponpairs = new AOWeaponpairs();
                        tempWeaponpairs.value1 = BitConverter.ToInt32(bytes, (int)counter);
                        counter += 4;
                        tempWeaponpairs.value2 = BitConverter.ToInt32(bytes, (int)counter);
                        counter += 4;
                        tempWeaponpairs.value3 = BitConverter.ToInt32(bytes, (int)counter);
                        counter += 4;
                        tempWeaponpairs.value4 = BitConverter.ToInt32(bytes, (int)counter);
                        counter += 4;
                        monster.Weaponpairs.Add(tempWeaponpairs);
                    }
                }

                if (!(row[17] is DBNull))
                {
                    bytes = (byte[])row[17]; // Running Nanos
                    counter = 0;
                    while (counter < bytes.Length)
                    {
                        AONano tempNano = new AONano();

                        tempNano.Nanotype = BitConverter.ToInt32(bytes, (int)counter);
                        counter += 4;
                        tempNano.Instance = BitConverter.ToInt32(bytes, (int)counter);
                        counter += 4;
                        tempNano.Value3 = BitConverter.ToInt32(bytes, (int)counter);
                        counter += 4;
                        tempNano.Time1 = BitConverter.ToInt32(bytes, (int)counter);
                        counter += 4;
                        tempNano.Time2 = BitConverter.ToInt32(bytes, (int)counter);
                        counter += 4;
                        monster.ActiveNanos.Add(tempNano);
                    }
                }

                if (!(row[18] is DBNull))
                {
                    counter = 0;
                    bytes = (byte[])row[18]; // Meshs
                    while (counter < bytes.Length)
                    {
                        AOMeshs tempMeshs = new AOMeshs();
                        tempMeshs.Position = BitConverter.ToInt32(bytes, (int)counter);
                        counter += 4;
                        tempMeshs.Mesh = BitConverter.ToInt32(bytes, (int)counter);
                        counter += 4;
                        tempMeshs.OverrideTexture = BitConverter.ToInt32(bytes, (int)counter);
                        counter += 4;

                        monster.Meshs.Add(tempMeshs);
                    }
                }

                if (!(row[19] is DBNull))
                {
                    counter = 0;
                    bytes = (byte[])row[19]; // Additional Meshs
                    while (counter < bytes.Length)
                    {
                        AOAddMeshs tempAdditionalMeshs = new AOAddMeshs();
                        tempAdditionalMeshs.position = bytes[counter++];
                        tempAdditionalMeshs.meshvalue1 = BitConverter.ToInt32(bytes, (int)counter);
                        counter += 4;
                        tempAdditionalMeshs.meshvalue2 = BitConverter.ToInt32(bytes, (int)counter);
                        counter += 4;
                        tempAdditionalMeshs.priority = bytes[counter++];

                        monster.AdditionalMeshs.Add(tempAdditionalMeshs);
                    }
                }
                monster.Starting = false;

                Program.zoneServer.Monsters.Add(monster);
                npcCount += 1;
                if ((npcCount % 100) == 0)
                {
                    Console.Write("\rReading spawns: " + npcCount.ToString() + "/" + numberOfNpc.ToString());
                }
            }

            Console.Write("\r                                                    \r");
            return npcCount;
        }
    }
}