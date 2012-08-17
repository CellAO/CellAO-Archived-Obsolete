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

#region Usings

#endregion

namespace ZoneEngine
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using AO.Core;

    using ZoneEngine.Misc;
    using ZoneEngine.Packets;

    public class VendingMachine : NonPlayerCharacterClass
    {
        public int TemplateID;

        public string HASH;

        public VendingMachine(int _id, int _playfield, string hash)
        {
            this.ID = _id;
            this.PlayField = _playfield;
            // Vending machines = type 51035
            this.Type = 51035;
            this.ourType = 3;
            this.rawCoord = new AOCoord();
            this.rawHeading = new Quaternion(0, 0, 0, 0);
            this.HASH = hash;
            this.dontdotimers = true;
            this.Stats = new CharacterStats(this);
            if (this.ID != 0)
            {
                LoadTemplate(hash); // All shops will have level 1
            }
            this.dontdotimers = false;
        }

        public VendingMachine(int _id, int _playfield, int template)
        {
            this.ID = _id;
            this.PlayField = _playfield;
            // Vending machines = type 51035
            this.Type = 51035;
            this.ourType = 3;
            this.rawCoord = new AOCoord();
            this.rawHeading = new Quaternion(0, 0, 0, 0);
            this.TemplateID = template;
            this.dontdotimers = true;
            this.Stats = new CharacterStats(this);
            if (this.ID != 0)
            {
                this.LoadTemplate(this.TemplateID); // All shops will have level 1
            }
            this.dontdotimers = false;
        }

        #region fillInventory
        private class ShopInv
        {
            public string HASH;

            public int minQL;

            public int maxQL;
        }

        public void fillInventory()
        {
            InventoryEntries ie;
            List<ShopInv> shopinvs = new List<ShopInv>();
            ShopInv temp;
            int place = 0;
            int iminql = 0;
            int imaxql = 0;
            Random r = new Random();
            string like = "";
            SqlWrapper Sql = new SqlWrapper();
            DataTable dt = Sql.ReadDatatable("SELECT * from vendortemplate where HASH='" + this.HASH + "'");
            foreach (DataRow row in dt.Rows)
            {
                temp = new ShopInv();
                temp.HASH = (string)row["ShopInvHash"];
                temp.minQL = (Int32)row["minQL"];
                temp.maxQL = (Int32)row["maxQL"];
                shopinvs.Add(temp);
                if (like != "")
                {
                    like += "OR ";
                }
                like += "HASH LIKE '%" + temp.HASH + "%' ";
            }
            if (like != "")
            {
                this.Inventory.Clear();
                dt = Sql.ReadDatatable("SELECT * from shopinventorytemplates where " + like + "and active = 1");
                string thishash;
                foreach (DataRow row in dt.Rows)
                {
                    thishash = (string)row["Hash"];
                    foreach (ShopInv si in shopinvs)
                    {
                        if (si.HASH == thishash)
                        {
                            iminql = (Int32)row["minql"];
                            imaxql = (Int32)row["maxql"];
                            // Dont add Items that are not between si.minQL and si.maxQL
                            if ((iminql <= si.maxQL) && (imaxql >= si.minQL))
                            {
                                ie = new InventoryEntries();
                                ie.Container = 104;
                                ie.Placement = place++;
                                ie.Item.LowID = (Int32)row["lowid"];
                                ie.Item.HighID = (Int32)row["highid"];
                                ie.Item.MultipleCount = (Int32)row["multiplecount"];
                                ie.Item.Nothing = 0;
                                ie.Item.Quality = Math.Min(
                                    Math.Max(Convert.ToInt32(r.Next(si.minQL, si.maxQL)), iminql), imaxql);
                                this.Inventory.Add(ie);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Create
        public void Create(int _templateid)
        {
            AOItem i = ItemHandler.GetItemTemplate(_templateid);
            int c;
            for (c = 0; c < i.Stats.Count; c++)
            {
                this.Stats.Set(i.Stats[c].Stat, (uint)i.Stats[c].Value);
            }
            // TODO i.applyon(this, ItemHandler.eventtype_ontrade, false, false, 0);
        }
        #endregion

        #region getfreshID
        public static int GetFreshID()
        {
            int freeID = 100000; // minimum ID for mobs
            foreach (VendingMachine vm in Program.zoneServer.Vendors)
            {
                freeID = Math.Max(freeID, vm.ID);
            }
            freeID++;
            return freeID;
        }
        #endregion

        #region SpawntoPlayfield
        /// <summary>
        /// Spawns this Vendor to every client in given playfield.
        /// </summary>
        /// <param name="playfield">Playfield to spawn to</param>
        public new void SpawnToPlayfield(int playfield)
        {
            foreach (Client cli in Program.zoneServer.Clients)
            {
                if (cli.Character.PlayField == playfield)
                {
                    VendingMachineFullUpdate.Send(cli, this);
                    ShopInventory.Send(cli, this);
                }
            }
        }
        #endregion

        #region LoadTemplate
        public bool LoadTemplate(string hash)
        {
            SqlWrapper Sql = new SqlWrapper();
            DataTable dt = Sql.ReadDatatable("SELECT * from vendortemplate WHERE HASH='" + hash + "'");
            if (dt.Rows.Count > 0)
            {
                this.TemplateID = (Int32)dt.Rows[0]["itemtemplate"];
                this.Name = (string)dt.Rows[0]["Name"];
                AOItem it = ItemHandler.GetItemTemplate(this.TemplateID);
                foreach (AOItemAttribute ia in it.Stats)
                {
                    this.Stats.Set(ia.Stat, (uint)ia.Value);
                }
                Sql.sqlclose();
                this.fillInventory();
                return true;
            }
            return false;
        }

        public bool LoadTemplate(int id)
        {
            AOItem it = ItemHandler.GetItemTemplate(id);
            foreach (AOItemAttribute ia in it.Stats)
            {
                this.Stats.Set(ia.Stat, (uint)ia.Value);
            }
            this.fillInventory();
            return true;
        }
        #endregion

        #region AddtoCache
        public new void AddToCache()
        {
            Program.zoneServer.Vendors.Add(this);
        }
        #endregion

        #region Add NPC to db
        public new void AddToDB()
        {
            SqlWrapper Sql = new SqlWrapper();
            // Fix from Moin, thx
            Sql.SqlInsert(
                "INSERT INTO " + this.getSQLTablefromDynelType() + " (ID, Playfield, TemplateID, Hash) VALUES ("
                + this.ID.ToString() + "," + this.PlayField.ToString() + "," + this.TemplateID.ToString() + ",'"
                + this.HASH + "')");
            this.writeCoordinatestoSQL();
            this.WriteHeadingToSQL();
        }
        #endregion

        #region Purge
        /// <summary>
        /// 
        /// </summary>
        public new void Purge()
        {
            if ((this.ID != 0) && (this.needpurge))
            {
                this.needpurge = false;

                this.writeCoordinatestoSQL();
                this.WriteHeadingToSQL();
            }
        }
        #endregion
    }
}