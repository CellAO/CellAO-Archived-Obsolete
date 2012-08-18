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

namespace LoginEngine.QueryBase
{
    using System;
    using System.Data;
    using System.Text;

    using AO.Core;

    /// <summary>
    /// 
    /// </summary>
    public class LoginPlayer
    {
        private int cbreedint, cprofint, playfield;

        private byte[] name, breed, prof, zone;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recvLogin"></param>
        public void GetCharacterName(string recvLogin)
        {
            string sqlQuery = "SELECT `Name`, `Breed`, `Profession` FROM `characters` WHERE Username = " + "'"
                              + recvLogin + "'";
            SqlWrapper ms = new SqlWrapper();
            DataTable dt = ms.ReadDatatable(sqlQuery);

            foreach (DataRow datarow1 in dt.Rows)
            {
                this.name = Encoding.ASCII.GetBytes(datarow1["Name"].ToString().PadRight(11, '\u0000'));
                this.cbreedint = int.Parse(datarow1["Breed"].ToString());
                this.breed = BitConverter.GetBytes(this.cbreedint);
                this.cprofint = int.Parse(datarow1["Profession"].ToString());
                this.prof = BitConverter.GetBytes(this.cprofint);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recvLogin"></param>
        public void GetCharacterZone(string recvLogin)
        {
            string sqlQuery = "SELECT `playfield` FROM `characters` WHERE Username = " + "'" + recvLogin + "'";
            SqlWrapper ms = new SqlWrapper();
            DataTable dt = ms.ReadDatatable(sqlQuery);

            foreach (DataRow datarow2 in dt.Rows)
            {
                this.playfield = (Int32)datarow2["playfield"];
                this.zone = BitConverter.GetBytes(this.playfield);
            }
        }
    }
}