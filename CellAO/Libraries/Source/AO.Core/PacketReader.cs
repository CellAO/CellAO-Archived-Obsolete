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
using System.IO;
using System.Net;
using System.Text;
#endregion

#pragma warning disable 1591

namespace AO.Core
{
    /// <summary>
    /// Class for reading packets
    /// </summary>
    public class PacketReader
    {
        private readonly MemoryStream stream;
        private readonly BinaryReader reader;

        /// <summary>
        /// Initializes new packet reader
        /// </summary>
        /// <param name="packet">byte array to read from</param>
        public PacketReader(byte[] packet)
        {
            this.stream = new MemoryStream(packet);
            this.reader = new BinaryReader(this.stream);
        }

        /// <summary>
        /// Reads byte from packet
        /// </summary>
        /// <returns></returns>
        public byte PopByte()
        {
            return this.reader.ReadByte();
        }

        /// <summary>
        /// Reads short (Int16) from packet
        /// </summary>
        /// <returns></returns>
        public short PopShort()
        {
            return IPAddress.NetworkToHostOrder(this.reader.ReadInt16());
        }

        /// <summary>
        /// Reads int (Int32) from packet
        /// </summary>
        /// <returns></returns>
        public int PopInt()
        {
            return IPAddress.NetworkToHostOrder(this.reader.ReadInt32());
        }

        /// <summary>
        /// Reads uint (UInt32) from packet
        /// </summary>
        /// <returns></returns>
        public uint PopUInt()
        {
            return (uint) IPAddress.NetworkToHostOrder(this.reader.ReadInt32());
        }

        /// <summary>
        /// Reads long (Int64) from packet
        /// </summary>
        /// <returns></returns>
        public long PopLong()
        {
            return IPAddress.NetworkToHostOrder(this.reader.ReadInt64());
        }

        /// <summary>
        /// Reads float (single) from packet
        /// </summary>
        /// <returns></returns>
        public float PopFloat()
        {
            byte[] _float = ReadBytes(4);
            Array.Reverse(_float);
            return BitConverter.ToSingle(_float, 0);
        }

        /// <summary>
        /// Reads set of coordinates (3 floats) from packet
        /// </summary>
        /// <returns></returns>
        public AOCoord PopCoord()
        {
            byte[] x, y, z;

            x = ReadBytes(4);
            Array.Reverse(x);
            y = ReadBytes(4);
            Array.Reverse(y);
            z = ReadBytes(4);
            Array.Reverse(z);

            return new AOCoord(BitConverter.ToSingle(x, 0), BitConverter.ToSingle(y, 0), BitConverter.ToSingle(z, 0));
        }

        /// <summary>
        /// Reads Quaternion (4 floats) from packet
        /// </summary>
        /// <returns></returns>
        public Quaternion PopQuat()
        {
            byte[] x, y, z, w;

            x = ReadBytes(4);
            Array.Reverse(x);
            y = ReadBytes(4);
            Array.Reverse(y);
            z = ReadBytes(4);
            Array.Reverse(z);
            w = ReadBytes(4);
            Array.Reverse(w);

            return new Quaternion(BitConverter.ToSingle(x, 0), BitConverter.ToSingle(y, 0), BitConverter.ToSingle(z, 0),
                                  BitConverter.ToSingle(w, 0));
        }

        /// <summary>
        /// Reads string from packet
        /// </summary>
        /// <param name="length">Length of string to read</param>
        /// <returns></returns>
        public string PopString(int length)
        {
            byte[] _string = ReadBytes(length);
            return Encoding.ASCII.GetString(_string);
        }

        /// <summary>
        /// Reads 3F1 count and converts it to normal int
        /// </summary>
        /// <returns></returns>
        public int Pop3F1Count()
        {
            int count = PopInt();
            return ((count/1009) - 1);
        }

        /// <summary>
        /// Reads identity (int32 Type, int32 Instance) from packet
        /// </summary>
        /// <returns></returns>
        public Identity PopIdentity()
        {
            Identity identity = new Identity();
            identity.Type = PopInt();
            identity.Instance = PopInt();
            return identity;
        }

        /// <summary>
        /// Skips specified number of bytes
        /// </summary>
        /// <param name="count">Number of bytes to skip</param>
        public void SkipBytes(int count)
        {
            this.reader.ReadBytes(count);
        }

        /// <summary>
        /// Reads specified number of bytes
        /// </summary>
        /// <param name="count">Number of bytes to read</param>
        /// <returns></returns>
        public byte[] ReadBytes(int count)
        {
            return this.reader.ReadBytes(count);
        }

        /// <summary>
        /// Reads packet header. Use only for zone server packets
        /// </summary>
        /// <returns></returns>
        public Header PopHeader()
        {
            Header header = new Header();
            header.Start = PopShort();
            header.Type = PopShort();
            header.Unknown = PopShort();
            header.Size = PopShort();
            header.Sender = PopInt();
            header.Receiver = PopInt();
            header.ID = PopInt();
            header.AffectedId = PopIdentity();
            return header;
        }

        /// <summary>
        /// Reads ACGItem (Uint32 LID, Uint32 HID, Uint32 QL) from packet.
        /// </summary>
        /// <returns></returns>
        public ACGItem PopACGItem()
        {
            ACGItem acgItem = new ACGItem();
            acgItem.LID = (uint) PopInt();
            acgItem.HID = (uint) PopInt();
            acgItem.QL = (uint) PopInt();
            return acgItem;
        }

        /// <summary>
        /// Closes and disposes used stream/reader
        /// </summary>
        public void Finish()
        {
            this.reader.Close();
            this.stream.Dispose();
        }

        /// <summary>
        /// Returns position of Reader
        /// </summary>
        /// <returns>position</returns>
        public long Position()
        {
            return this.reader.BaseStream.Position;
        }

        /// <summary>
        /// Returns length of Reader
        /// </summary>
        /// <returns></returns>
        public long Length()
        {
            return this.reader.BaseStream.Length;
        }
    }
}