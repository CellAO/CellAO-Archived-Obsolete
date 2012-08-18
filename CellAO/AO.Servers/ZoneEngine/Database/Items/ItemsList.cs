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

namespace ZoneEngine.Database.Items
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using AO.Core;

    /// <summary>
    /// 
    /// </summary>
    public static class ItemsList
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="charId"></param>
        /// <returns></returns>
        public static List<ItemsEntry> LoadItems(int charId)
        {
            List<ItemsEntry> items = new List<ItemsEntry>();
            SqlWrapper sqlWrapper = new SqlWrapper();
            try
            {
                string sqlQuery =
                    "SELECT `Placement`, `Flags`, `MultipleCount`, `Type`, `Instance`, `LowID`, `HighID`, `Quality`, `Nothing` FROM `inventory` WHERE ID = "
                    + "'" + charId + "' ORDER BY Placement ASC";
                DataTable dataTable = sqlWrapper.ReadDatatable(sqlQuery);

                foreach (DataRow itemRow in dataTable.Rows)
                {
                    ItemsEntry itemEntry = new ItemsEntry();
                    itemEntry.Placement = (Int32)itemRow["Placement"];
                    itemEntry.Flags = (Int16)itemRow["Flags"];
                    itemEntry.MultipleCount = (Int16)itemRow["MultipleCount"];
                    itemEntry.ItemType = (Int32)itemRow["Type"];
                    itemEntry.Instance = (Int32)itemRow["Instance"];
                    itemEntry.LowId = (Int32)itemRow["LowID"];
                    itemEntry.HighId = (Int32)itemRow["HighID"];
                    itemEntry.Quality = (Int32)itemRow["Quality"];
                    itemEntry.Nothing = (Int32)itemRow["Nothing"];

                    items.Add(itemEntry);
                }
            }
            catch (Exception e)
            {
                sqlWrapper.sqlclose();
                Console.WriteLine("Error: CharacterID: " + charId + "Message: " + e.Message);
            }
            return items;
        }
    }
}