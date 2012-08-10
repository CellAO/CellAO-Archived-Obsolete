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

#region Usings
using System;
using System.Data;
using System.Threading;
using AO.Core;
using ZoneEngine.Packets;
#endregion

namespace ZoneEngine.NPC
{
    public class VendorHandler
    {
        #region get next free vendor ID
        public static int GetFreshID(int playfield)
        {
            int freeID = 0x0; // minimum ID for mobs
            int tempid = 0;
            foreach (VendingMachine vm in Program.zoneServer.Vendors)
            {
                if (vm.PlayField == playfield)
                {
                    tempid = vm.ID & 0xffff;
                    freeID = Math.Max(freeID, tempid);
                }
            }
            freeID++;
            return (freeID + (playfield << 16));
        }
        #endregion

        #region CacheAllfromDB
        // Call this _only_ on server startup!
        /// <summary>
        /// Reads all NPCs from database and adds them to servers list.
        /// </summary>
        /// <returns>Number of NPCs loaded</returns>
        public static int CacheAllFromDB()
        {
            int npcCount = 0;
            int npcall;
            VendingMachine mVendor;
            SqlWrapper msql = new SqlWrapper();
            // TODO:COUNT
            npcall = msql.SqlCount("SELECT count(*) FROM `vendors`");
            Console.Write("Reading vendors: 0/" + npcall.ToString());
            DataTable dt = msql.ReadDT("SELECT * FROM `vendors`");

            foreach (DataRow row in dt.Rows)
            {
                mVendor = new VendingMachine((Int32) row["ID"], (Int32) row["Playfield"], (Int32) row["TemplateID"]);
                mVendor.Coordinates.x = (Single) row["X"];
                mVendor.Coordinates.y = (Single) row["Y"];
                mVendor.Coordinates.z = (Single) row["Z"];
                mVendor.Heading.x = (Single) row["HeadingX"];
                mVendor.Heading.y = (Single) row["HeadingY"];
                mVendor.Heading.z = (Single) row["HeadingZ"];
                mVendor.Heading.w = (Single) row["HeadingW"];
                mVendor.HASH = (string) row["Hash"];
                mVendor.Name = (string) row["Name"]
#if DEBUG
 + " " + mVendor.ID.ToString(); // ID is for debug purpose only
#endif
                    ;
                mVendor.fillInventory();

                Program.zoneServer.Vendors.Add(mVendor);
                npcCount++;
                if ((npcCount%100) == 0)
                {
                    Console.Write("\rReading vendors: " + npcCount.ToString() + "/" + npcall.ToString());
                }
            }
            Console.Write("\r                                                   \r");
            return npcCount;
        }
        #endregion

        #region SpawnVendor
        public static void SpawnVendor(Client cli, string hash)
        {
            VendingMachine vm = new VendingMachine(GetFreshID(cli.Character.PlayField), cli.Character.PlayField, hash);
            //(cli, hash, level);
            vm.rawCoord = cli.Character.rawCoord;
            vm.rawHeading = cli.Character.rawHeading;
            vm.PlayField = cli.Character.PlayField;
            if (String.IsNullOrEmpty(vm.Name)) return;
            vm.AddToCache();
            vm.SpawnToPlayfield(vm.PlayField);
        }
        #endregion

        #region GetVendorsInPF
        public static void GetVendorsInPF(Client cli)
        {
            int pf = cli.Character.PlayField;
            foreach (VendingMachine vm in Program.zoneServer.Vendors)
            {
                if (vm.PlayField == pf)
                {
                    VendingMachineFullUpdate.Send(cli, vm);
                    Thread.Sleep(50);
                }
            }
        }
        #endregion

        #region getfirstmachine
        public static int getFirstVendor(int playfield)
        {
            int retid = 0;
            foreach (VendingMachine vm in Program.zoneServer.Vendors)
            {
                if (vm.PlayField == playfield)
                {
                    if (retid == 0)
                    {
                        retid = vm.ID;
                    }
                    else
                    {
                        retid = Math.Min(retid, vm.ID);
                    }
                }
            }
            return retid;
        }
        #endregion

        #region getNumberofVendorsinPlayfield
        public static int getNumberofVendorsinPlayfield(int playfield)
        {
            int count = 0;
            foreach (VendingMachine vm in Program.zoneServer.Vendors)
            {
                if (vm.PlayField == playfield)
                {
                    count++;
                }
            }
            return count;
        }
        #endregion

        #region GetVendorByID
        public static VendingMachine GetVendorByID(int ID)
        {
            foreach (VendingMachine vm in Program.zoneServer.Vendors)
            {
                if (vm.ID == ID)
                {
                    return vm;
                }
            }
            return null;
        }
        #endregion
    }
}