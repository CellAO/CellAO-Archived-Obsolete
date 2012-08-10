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


/*what does this code even do algor?*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;



namespace CreateStatelEvents
{
    class RDBIndexer
    {
        public List<RDBIndex> keys = new List<RDBIndex>();
        public RDBIndex get(string key)
        {
            foreach (RDBIndex r in keys)
            {
                if (key!=r.key)
                {
                    continue;
                }
                return r;
            }
            return null;
        }
    }
    
    class RDBIndex
    {
        public List<Int32> Value = new List<Int32>();
        public string key = "";
    }
    
    class RDBTree
    {
        private Int32 BlockOffset;
        private Int32 DataFileSize;
        private Int32 SizeFix = 12;
        public RDBIndexer IDX = new RDBIndexer();
        private List<FileStream> DAT = new List<FileStream>();
        private List<BinaryReader> BRS = new List<BinaryReader>();
        private string outputpath;
        private static int DF = 0;

        public RDBTree(string RDBPATH, string OUTPUTPATH)
        {
            outputpath = OUTPUTPATH;
            FileStream FS = new FileStream(RDBPATH + "ResourceDatabase.idx", FileMode.Open);
            BinaryReader BR = new BinaryReader(FS);
            int gu = 0;
            
            DAT.Add(new FileStream(RDBPATH+"ResourceDatabase.dat", FileMode.Open));
            BRS.Add(new BinaryReader(DAT.ElementAt(gu++)));
            DAT.Add(new FileStream(RDBPATH+"ResourceDatabase.dat.001", FileMode.Open));
            BRS.Add(new BinaryReader(DAT.ElementAt(gu++)));

            FS.Seek(12, SeekOrigin.Begin);
            BlockOffset = BR.ReadInt32();
            FS.Seek(0xb8, SeekOrigin.Begin);
            DataFileSize = BR.ReadInt32();

            if (DataFileSize == -1)
            {
                // Old Data format
                DataFileSize = (Int32)FS.Length;
            }

            FS.Seek(0x48, SeekOrigin.Begin);
            Int32 FirstBlock = BR.ReadInt32();

            FS.Seek(FirstBlock, SeekOrigin.Begin);

            do
            {
                Int32 ForwardLink = BR.ReadInt32();
                if (ForwardLink == 0)
                {
                    // Last one
                    break;
                }

                BR.ReadInt32(); // Skip Backlink
                short Entries = BR.ReadInt16(); // Number of Entries
                BR.ReadBytes(8); // Skip

                for (int i = Entries; i > 0; i--)
                {
                    int rOff = BR.ReadInt32();
                    string rType = IPAddress.HostToNetworkOrder(BR.ReadInt32()).ToString();
                    BR.ReadInt32();
                    if (IDX.get(rType) != null)
                    {
                        IDX.get(rType).Value.Add(rOff);
                    }
                    else
                    {
                        RDBIndex rtemp = new RDBIndex();
                        rtemp.key = rType;
                        rtemp.Value.Add(rOff);
                        IDX.keys.Add(rtemp);
                    }
                }
                FS.Seek(ForwardLink, SeekOrigin.Begin);
            } while (true);
            BR.Close();
            FS.Close();
        }

        public byte[] SegRead(Int32 count, long offset=-1) 
        {
            if (offset!=-1)
            {
                DF = 0;
                while (offset>DataFileSize)
                {
                    DF++;
                    offset-=DataFileSize;
                }
                if (DF>0)
                {
                    offset +=BlockOffset;
                }
                DAT.ElementAt(DF).Seek(offset, SeekOrigin.Begin);
            }


            long size = DAT.ElementAt(DF).Length;
            long here = DAT.ElementAt(DF).Position;
            if ((here+count)<=size)
            {
                return BRS.ElementAt(DF).ReadBytes(count);
            }

            byte[] buf1 = BRS.ElementAt(DF).ReadBytes((Int32)(size-here));
            byte[] buf2 = BRS.ElementAt(DF+1).ReadBytes((Int32)(count-(size-here)));
            Array.Resize(ref buf1,buf1.Length+buf2.Length);
            Array.Copy(buf2,0,buf1,(Int32)size-here,buf2.Length);
            return buf1;
        }

        public Int32 Export(Int32 RecType, bool Decode, string outputp, System.Windows.Forms.ProgressBar pb)
        {
            outputpath = outputp;
            Int32 NumExported = 0;
            string rType = RecType.ToString();
            if (IDX.get(rType) == null)
            {
                return 0; // No records found/exported
            }

            RDBIndex rdb = IDX.get(rType);

            try
            {
                Directory.CreateDirectory(outputpath);
            }
            catch { }

            foreach (Int32 rOff in rdb.Value)
            {
                byte[] header = SegRead(0x22, rOff);
                int RT = BitConverter.ToInt32(header, 0xa);
                int RN = BitConverter.ToInt32(header, 0xe);
                int RL = BitConverter.ToInt32(header, 0x12) - SizeFix;

                byte[] rData = SegRead(RL);
                if (Decode)
                {
                    DecodeItemData(ref header, ref rData);
                }
                FileStream OF = new FileStream(outputpath + @"\" + RN.ToString() + ".rdbdata", FileMode.Create);
                BinaryWriter B = new BinaryWriter(OF);
                B.Write(rData);
                B.Close();
                OF.Close();
                NumExported += 1;
                pb.PerformStep();
            }
            return NumExported;
        }

        public void DecodeItemData(ref byte[] header, ref byte[] rdata)
        {
            Int32 N = BitConverter.ToInt32(header,0xe);
            UInt64 seed = (UInt64)N;

            for (int i=0;i<rdata.Length;i++)
            {
                seed *= (UInt64)0x1012003;
                seed = (UInt64)(seed % (UInt64)0x4E512DC8F);
                rdata[i] = (byte)(rdata[i] ^ ((byte)seed & 0xff));
            }
        }
    }
}
