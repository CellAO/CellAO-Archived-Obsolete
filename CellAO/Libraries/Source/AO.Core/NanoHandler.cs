#region License
/*
Copyright (c) 2005-2011, CellAO Team

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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ComponentAce.Compression.Libs.zlib;
using MsgPack.Serialization;
#endregion

namespace AO.Core
{
    /// <summary>
    /// Item handler class
    /// </summary>
    public class NanoHandler
    {
        private SqlWrapper mySql = new SqlWrapper();

        /// <summary>
        /// Cache of all item templates
        /// </summary>
        public static List<AONanos> NanoList = new List<AONanos>();

        /// <summary>
        /// Cache all item templates
        /// </summary>
        /// <returns>number of cached items</returns>
        public static int CacheAllNanos()
        {
            DateTime _now = DateTime.Now;
            NanoList = new List<AONanos>();
            Stream sf = new FileStream("nanos.dat", FileMode.Open);
            MemoryStream ms = new MemoryStream();

            ZOutputStream sm = new ZOutputStream(ms);
            CopyStream(sf, sm);

            ms.Seek(0, SeekOrigin.Begin);

            byte[] buffer = new byte[4];
            ms.Read(buffer, 0, 4);
            int packaged = BitConverter.ToInt32(buffer, 0);

            BinaryReader br = new BinaryReader(ms);
            var bf = MessagePackSerializer.Create<List<AONanos>>();

            while (true)
            {
                List<AONanos> templist = bf.Unpack(ms);
                NanoList.AddRange(templist);
                if (templist.Count != packaged)
                {
                    break;
                }
                Console.Write("Loaded {0} Nanos in {1}\r",
                              new object[]
                                  {NanoList.Count, new DateTime((DateTime.Now - _now).Ticks).ToString("mm:ss.ff")});
            }
            GC.Collect();
            return NanoList.Count;
        }

        /// <summary>
        /// Cache all item templates
        /// </summary>
        /// <returns>number of cached items</returns>
        public static int CacheAllNanos(string fname)
        {
            DateTime _now = DateTime.Now;
            NanoList = new List<AONanos>();
            Stream sf = new FileStream(fname, FileMode.Open);
            MemoryStream ms = new MemoryStream();

            ZOutputStream sm = new ZOutputStream(ms);
            CopyStream(sf, sm);

            ms.Seek(0, SeekOrigin.Begin);
            BinaryReader br = new BinaryReader(ms);
            var bf = MessagePackSerializer.Create<List<AONanos>>();


            byte[] buffer = new byte[4];
            ms.Read(buffer, 0, 4);
            int packaged = BitConverter.ToInt32(buffer, 0);

            while (true)
            {
                List<AONanos> templist = (List<AONanos>) bf.Unpack(ms);
                NanoList.AddRange(templist);
                if (templist.Count != packaged)
                {
                    break;
                }
                Console.Write("Loaded {0} nanos in {1}\r",
                              new object[]
                                  {NanoList.Count, new DateTime((DateTime.Now - _now).Ticks).ToString("mm:ss.ff")});
            }
            GC.Collect();
            return NanoList.Count;
        }


        private static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[2097152];
            int len;
            while ((len = input.Read(buffer, 0, 2097152)) > 0)
            {
                output.Write(buffer, 0, len);
                Console.Write("\rDeflating " + Convert.ToInt32(Math.Floor((double) input.Position/input.Length*100.0)) +
                              "%");
            }
            output.Flush();
            Console.Write("\r                                             \r");
        }


        /// <summary>
        /// Returns a nano object
        /// </summary>
        /// <param name="id">ID of the nano</param>
        /// <returns>Nano</returns>
        public static AONanos GetNano(int id)
        {
            foreach (AONanos aon in NanoList)
            {
                if (aon.ID == id)
                    return aon;
            }
            return null;
        }
    }
}