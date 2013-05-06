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

namespace ZoneEngine
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using AO.Core;

    using SmokeLounge.AOtomation.Messaging.GameData;

    using ZoneEngine.Misc;
    using ZoneEngine.Packets;

    using Quaternion = AO.Core.Quaternion;

    public class VendingMachine : NonPlayerCharacterClass
    {
        public int TemplateId { get; set; }

        public VendingMachine(Identity id, int playfield, string hash)
        {
            this.Id = id;
            this.PlayField = playfield;
            this.OurType = 3;
            this.RawCoord = new AOCoord();
            this.RawHeading = new Quaternion(0, 0, 0, 0);
            this.Hash = hash;
            this.DoNotDoTimers = true;
            this.Stats = new CharacterStats(this);
            if (this.Id.Instance != 0)
            {
                LoadTemplate(hash); // All shops will have level 1
            }
            this.DoNotDoTimers = false;
        }

        public VendingMachine(Identity id, int playfield, int templateId)
        {
            this.Id = id;
            this.PlayField = playfield;
            this.OurType = 3;
            this.RawCoord = new AOCoord();
            this.RawHeading = new Quaternion(0, 0, 0, 0);
            this.TemplateId = templateId;
            this.DoNotDoTimers = true;
            this.Stats = new CharacterStats(this);
            if (this.Id.Instance != 0)
            {
                this.LoadTemplate(this.TemplateId); // All shops will have level 1
            }
            this.DoNotDoTimers = false;
        }

        #region fillInventory
        private class ShopInv
        {
            public string Hash;

            public int MinQl;

            public int MaxQl;
        }

        public void FillInventory()
        {
            List<ShopInv> shopinvs = new List<ShopInv>();
            int place = 0;
            Random r = new Random();
            string like = "";
            SqlWrapper sqlWrapper = new SqlWrapper();
            DataTable dt = sqlWrapper.ReadDatatable("SELECT * from vendortemplate where HASH='" + this.Hash + "'");
            foreach (DataRow row in dt.Rows)
            {
                ShopInv shopInventory = new ShopInv
                    { Hash = (string)row["ShopInvHash"], MinQl = (Int32)row["minQL"], MaxQl = (Int32)row["maxQL"] };
                shopinvs.Add(shopInventory);
                if (like != "")
                {
                    like += "OR ";
                }
                like += "HASH LIKE '%" + shopInventory.Hash + "%' ";
            }
            if (like != "")
            {
                this.Inventory.Clear();
                dt = sqlWrapper.ReadDatatable("SELECT * from shopinventorytemplates where " + like + "and active = 1");
                foreach (DataRow row in dt.Rows)
                {
                    string thisHash = (string)row["Hash"];
                    foreach (ShopInv si in shopinvs)
                    {
                        if (si.Hash == thisHash)
                        {
                            int minQl = (Int32)row["minql"];
                            int maxQl = (Int32)row["maxql"];
                            // Dont add Items that are not between si.minQL and si.maxQL
                            if ((minQl <= si.MaxQl) && (maxQl >= si.MinQl))
                            {
                                InventoryEntries inventoryEntry = new InventoryEntries
                                    {
                                        Container = 104,
                                        Placement = place++,
                                        Item =
                                            {
                                                LowID = (Int32)row["lowid"],
                                                HighID = (Int32)row["highid"],
                                                MultipleCount = (Int32)row["multiplecount"],
                                                Nothing = 0,
                                                Quality =
                                                    Math.Min(
                                                        Math.Max(Convert.ToInt32(r.Next(si.MinQl, si.MaxQl)), minQl),
                                                        maxQl)
                                            }
                                    };
                                this.Inventory.Add(inventoryEntry);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Create
        public void Create(int templateId)
        {
            AOItem i = ItemHandler.GetItemTemplate(templateId);
            int c;
            for (c = 0; c < i.Stats.Count; c++)
            {
                this.Stats.SetStatValueByName(i.Stats[c].Stat, (uint)i.Stats[c].Value);
            }
            // TODO i.applyon(this, ItemHandler.eventtype_ontrade, false, false, 0);
            // eventtype_ontrade? Why?
        }
        #endregion

        #region getfreshID
        public static Identity NextFreeId()
        {
            int freeID = 100000; // minimum ID for mobs
            foreach (VendingMachine vm in Program.zoneServer.Vendors)
            {
                freeID = Math.Max(freeID, vm.Id.Instance);
            }
            freeID++;
            return new Identity { Type = IdentityType.VendingMachine, Instance = freeID };
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
            SqlWrapper sqlWrapper = new SqlWrapper();
            DataTable dataTable = sqlWrapper.ReadDatatable("SELECT * from vendortemplate WHERE HASH='" + hash + "'");
            if (dataTable.Rows.Count > 0)
            {
                this.TemplateId = (Int32)dataTable.Rows[0]["itemtemplate"];
                this.Name = (string)dataTable.Rows[0]["Name"];
                AOItem item = ItemHandler.GetItemTemplate(this.TemplateId);
                foreach (AOItemAttribute ia in item.Stats)
                {
                    this.Stats.SetStatValueByName(ia.Stat, (uint)ia.Value);
                }
                sqlWrapper.sqlclose();
                this.FillInventory();
                return true;
            }
            return false;
        }

        public bool LoadTemplate(int id)
        {
            AOItem it = ItemHandler.GetItemTemplate(id);
            foreach (AOItemAttribute ia in it.Stats)
            {
                this.Stats.SetStatValueByName(ia.Stat, (uint)ia.Value);
            }
            this.FillInventory();
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
            SqlWrapper sqlWrapper = new SqlWrapper();
            // Fix from Moin, thx
            sqlWrapper.SqlInsert(
                "INSERT INTO " + this.GetSqlTablefromDynelType() + " (ID, Playfield, TemplateID, Hash) VALUES ("
                + this.Id.Instance.ToString() + "," + this.PlayField.ToString() + "," + this.TemplateId.ToString() + ",'"
                + this.Hash + "')");
            this.WriteCoordinatesToSql();
            this.WriteHeadingToSql();
        }
        #endregion

        #region Purge
        /// <summary>
        /// 
        /// </summary>
        public new void Purge()
        {
            if ((this.Id.Instance != 0) && (this.NeedPurge))
            {
                this.NeedPurge = false;

                this.WriteCoordinatesToSql();
                this.WriteHeadingToSql();
            }
        }
        #endregion
    }
}