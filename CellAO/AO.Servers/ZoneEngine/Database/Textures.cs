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
using System.Data;
using AO.Core;
#endregion

namespace ZoneEngine.Database
{
    /// <summary>
    /// 
    /// </summary>
    public class Textures
    {
        /// <summary>
        /// 
        /// </summary>
        public int Textures0;

        /// <summary>
        /// 
        /// </summary>
        public int Textures1;

        /// <summary>
        /// 
        /// </summary>
        public int Textures2;

        /// <summary>
        /// 
        /// </summary>
        public int Textures3;

        /// <summary>
        /// 
        /// </summary>
        public int Textures4;

        private readonly SqlWrapper ms = new SqlWrapper();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="charID"></param>
        public void GetTextures(int charID)
        {
            string SqlQuery =
                "SELECT `Textures0`, `Textures1`, `Textures2`, `Textures3`, `Textures4` FROM `characters` WHERE ID = " +
                "'" + charID + "'";
            DataTable dt = ms.ReadDT(SqlQuery);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Textures0 = (Int32) row[0];
                    Textures1 = (Int32) row[1];
                    Textures2 = (Int32) row[2];
                    Textures3 = (Int32) row[3];
                    Textures4 = (Int32) row[4];
                }
            }
        }
    }
}