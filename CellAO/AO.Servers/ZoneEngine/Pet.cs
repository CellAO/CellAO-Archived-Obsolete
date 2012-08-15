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

namespace ZoneEngine
{
    using System.Data;

    using AO.Core;

    internal class Pet : NonPlayerCharacterClass
    {
        public Pet(int _id, int _playfield)
            : base(_id, _playfield)
        {
            this.Type = 50000;
        }

        public Character Owner;

        #region Owner (read/write), only needed if we decide to make persistant pets
        public void readOwnerfromSQL()
        {
            SqlWrapper ms = new SqlWrapper();

            DataTable dt =
                ms.ReadDatatable("SELECT * FROM " + this.getSQLTablefromDynelType() + "owner WHERE ID=" + this.ID.ToString());
            if (dt.Rows.Count > 0)
            {
                //TODO: Add Pet code here
                // Owner = FindCharacterByID(ms.myreader.GetInt32("owner");
            }
        }

        public void writeOwnertoSQL()
        {
            SqlWrapper ms = new SqlWrapper();

            ms.SqlInsert(
                "INSERT INTO " + this.getSQLTablefromDynelType() + "owner VALUES (" + this.ID.ToString() + ","
                + this.Owner.ID.ToString() + ")");
        }
        #endregion
    }
}