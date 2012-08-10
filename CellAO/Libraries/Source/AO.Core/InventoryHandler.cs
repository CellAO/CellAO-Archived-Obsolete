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
using System.Linq;
using System.Text;
using System.Data;
using AO.Core;

#endregion
// New Inventory system Development

// :::::::DYNEL Class to be removed when ZoneEngine Dynel is Moved :::::::
namespace AO.Core
{
    public class ItemContainer
    {

        public ItemContainer(int _type, uint _instance, Dynel _parent)
        {
            Type = _type;
            Instance = _instance;
            LoadFromSQL(_parent.getSQLTablefromDynelType() + "inventory");
            switch (_type)
            {
                // only Characters and NonPC's for now
                case 50000:
                    NumberOfSlots = 30;
                    break;
                default:
                    throw new NotImplementedException("Can't create untyped container");
            }
            Items = new Object[NumberOfSlots];
        }


        public int Type = 0;
        public uint Instance = 0;

        int NumberOfSlots = 0;
        Dynel Parent = null;

        Object[] Items;


        // Load All Container Arrays from  SQL
        void LoadFromSQL(string tablename)
        {
            // Empty the Array first
            for (int i = 0; i < Items.Length; i++)
                Items[i] = null;
            SqlWrapper sql = new SqlWrapper();
            DataTable dt = sql.ReadDT("SELECT * FROM " + tablename + " WHERE container=" + Type + " AND ID=" + Instance);
            foreach (DataRow row in dt.Rows)
            {
                int place = (Int32)row["placement"];
                if (place < NumberOfSlots)
                {
                    if (((Int32)row["type"] != 0) && ((Int32)row["instance"] != 0))
                    {
                        // Do stuff with instanced items
                        // Create item from lowid/highid interpolated by QL and read stats from sql
                    }
                    else
                    {
                        ContainerEntry ce = new ContainerEntry();
                        ce.LowID = (Int32)row["lowid"];
                        ce.HighID = (Int32)row["highid"];
                        ce.QL = (Int32)row["quality"];
                        ce.Amount = (Int32)row["multiplecount"];
                        ce.Flags = (uint)row["flags"];
                        ce.InstanceID = 0;
                        ce.Type = 0;
                        Items[place] = ce;
                    }
                }
            }
        }

        // Save All Container Arrays To SQL
        void SaveToSQL(string tablename)
        {
            SqlWrapper sql = new SqlWrapper();
            sql.SqlDelete("DELETE FROM " + tablename + " WHERE container=" + Type + " AND ID=" + Instance);

            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i] is ContainerEntry)
                {
                    // Not instanced items first
                    ContainerEntry ce = (ContainerEntry)Items[i];
                    sql.SqlInsert("INSERT INTO " + tablename + " (ID,Placement, flags, multiplecount, type,instance, lowid,highid, quality,nothing, container) VALUES (" + Instance + "," + i + "," + ce.Flags + "," + ce.Amount + ",0,0," + ce.LowID + "," + ce.HighID + "," + ce.QL + ",0," + Type + ")");
                }
                else
                {
                    // Do instanced items stuff here
                    // insert into inventory table AND store item's stats
                }
            }
        }

        // Add Item To Container
        bool AddItem(int place, int lowID, int HighID, int Type, uint InstanceItemID, int Amount, int QL, uint Flags)
        {
            if (place == 0x6f)
            {
                // find next free
                int i = 0;
                while (i < NumberOfSlots)
                {
                    if (Items[i] == null)
                        break;
                    i++;
                }

                // No free slot found?
                if (i == NumberOfSlots)
                    return false;

                place = i;
            }

            if ((Type == 0) && (InstanceItemID == 0))
            {
                ContainerEntry ce = new ContainerEntry();
                ce.Type = 0;
                ce.InstanceID = 0;

                ce.LowID = lowID;
                ce.HighID = HighID;
                ce.QL = QL;
                ce.Flags = Flags;

                Items[place] = ce;
            }
            else
            {
                //Instanced items stuff here
            }
            return true;
        }

        // Remove Item From Container (Instanced)
        void RemoveItem(uint ContainerID, uint InstanceItemID, int Amount)
        {

        }

        void RemoveItem(int Place, int Amount)
        {
            ContainerEntry ce = (ContainerEntry)Items[Place];
            if (Items[Place] == null)
                throw new NoNullAllowedException("Item at place " + Place + " of Container " + Type + ":" + Instance + " is NULL");

            ce.Amount -= Amount;
            if (ce.Amount <= 0)
            {
                Items[Place] = null;
            }
        }

        // Equip Item
        void EquipItem(int SourceSlot, int TargetSlot, uint InstanceItemID)
        {


        }

        // Unequip Item
        void UequipItem(int SourceSlot, int TargetSlot, uint InstanceItemID)
        {

        }

        // Slot Contents Information
        class ContainerEntry
        {
            public int LowID;
            public int HighID;
            public int Type;
            public uint InstanceID;
            public int Amount;
            public uint Flags;
            public int QL;
        }
    }
}