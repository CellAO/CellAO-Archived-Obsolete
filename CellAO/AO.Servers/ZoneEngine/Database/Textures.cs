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

namespace ZoneEngine.Database
{
    using System;
    using System.Data;

    using AO.Core;

    /// <summary>
    /// 
    /// </summary>
    public class Textures
    {
        /// <summary>
        /// Texture number 0
        /// </summary>
        private int textures0;

        /// <summary>
        /// Texture number 1
        /// </summary>
        private int textures1;

        /// <summary>
        /// Texture number 2
        /// </summary>
        private int textures2;

        /// <summary>
        /// Texture number 3
        /// </summary>
        private int textures3;

        /// <summary>
        /// Texture number 4
        /// </summary>
        private int textures4;

        /// <summary>
        /// Getter/Setter Texture 0
        /// </summary>
        public int Textures0
        {
            get
            {
                return textures0;
            }
            set
            {
                textures0 = value;
            }
        }

        /// <summary>
        /// Getter/Setter Texture 1
        /// </summary>
        public int Textures1
        {
            get
            {
                return textures1;
            }
            set
            {
                textures1 = value;
            }
        }

        /// <summary>
        /// Getter/Setter Texture 2
        /// </summary>
        public int Textures2
        {
            get
            {
                return textures2;
            }
            set
            {
                textures2 = value;
            }
        }

        /// <summary>
        /// Getter/Setter Texture 3
        /// </summary>
        public int Textures3
        {
            get
            {
                return textures3;
            }
            set
            {
                textures3 = value;
            }
        }

        /// <summary>
        /// Getter/Setter Texture 4
        /// </summary>
        public int Textures4
        {
            get
            {
                return textures4;
            }
            set
            {
                textures4 = value;
            }
        }

        /// <summary>
        /// Read textures from database
        /// </summary>
        /// <param name="charId"></param>
        public void ReadTexturesFromDatabase(int charId)
        {
            string sqlQuery =
                "SELECT `Textures0`, `Textures1`, `Textures2`, `Textures3`, `Textures4` FROM `characters` WHERE ID = "
                + "'" + charId + "'";
            SqlWrapper ms = new SqlWrapper();
            DataTable dt = ms.ReadDatatable(sqlQuery);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    this.textures0 = (Int32)row[0];
                    this.textures1 = (Int32)row[1];
                    this.textures2 = (Int32)row[2];
                    this.textures3 = (Int32)row[3];
                    this.textures4 = (Int32)row[4];
                }
            }
        }
    }
}