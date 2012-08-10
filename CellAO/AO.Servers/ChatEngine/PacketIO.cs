#region License
// Copyright (c) 2005-2012, CellAO Team
// All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//     * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
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

namespace ChatEngine
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;

    /// <summary>
    /// The packet writer.
    /// </summary>
    public class PacketWriter
    {
        /// <summary>
        /// The _packet.
        /// </summary>
        private readonly List<byte> _packet = new List<byte>();

        /// <summary>
        /// The _data.
        /// </summary>
        private readonly List<byte> _data = new List<byte>();

        /// <summary>
        /// The _type.
        /// </summary>
        private readonly ushort _type;

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketWriter"/> class. 
        /// The packet writer.
        /// </summary>
        /// <param name="type">
        /// </param>
        public PacketWriter(ushort type)
        {
            this._type = type;
        }

        /// <summary>
        /// The write byte.
        /// </summary>
        /// <param name="input">
        /// </param>
        public void WriteByte(byte input)
        {
            this._data.Add(input);
        }

        /// <summary>
        /// The write u int 16.
        /// </summary>
        /// <param name="input">
        /// </param>
        public void WriteUInt16(ushort input)
        {
            this._data.AddRange(BitConverter.GetBytes((UInt16)IPAddress.HostToNetworkOrder((Int16)input)));
        }

        /// <summary>
        /// The write u int 32.
        /// </summary>
        /// <param name="input">
        /// </param>
        public void WriteUInt32(uint input)
        {
            this._data.AddRange(BitConverter.GetBytes((UInt32)IPAddress.HostToNetworkOrder((Int32)input)));
        }

        /// <summary>
        /// The write bytes.
        /// </summary>
        /// <param name="input">
        /// </param>
        public void WriteBytes(byte[] input)
        {
            this._data.AddRange(input);
        }

        /// <summary>
        /// The write string.
        /// </summary>
        /// <param name="input">
        /// </param>
        public void WriteString(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                this._data.AddRange(new byte[] { 0x00, 0x01, 0x00 });
            }
            else
            {
                short length = IPAddress.HostToNetworkOrder((Int16)Encoding.UTF8.GetByteCount(input));
                this._data.AddRange(BitConverter.GetBytes((UInt16)length));
                this._data.AddRange(Encoding.UTF8.GetBytes(input));
            }
        }

        /// <summary>
        /// The finish.
        /// </summary>
        /// <returns>
        /// </returns>
        public byte[] Finish()
        {
            this._packet.AddRange(BitConverter.GetBytes((ushort)IPAddress.HostToNetworkOrder((short)this._type)));
            this._packet.AddRange(BitConverter.GetBytes((ushort)IPAddress.HostToNetworkOrder((short)this._data.Count)));
            this._packet.AddRange(this._data);
            return this._packet.ToArray();
        }
    }

    /// <summary>
    /// The packet reader.
    /// </summary>
    public class PacketReader : IDisposable
    {
        /// <summary>
        /// The _stream.
        /// </summary>
        private readonly MemoryStream stream;

        /// <summary>
        /// The _reader.
        /// </summary>
        private readonly BinaryReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketReader"/> class. 
        /// The packet reader.
        /// </summary>
        /// <param name="packet">
        /// </param>
        public PacketReader(ref byte[] packet)
        {
            this.stream = new MemoryStream(packet);
            this.reader = new BinaryReader(this.stream);
        }

        /// <summary>
        /// Read one byte
        /// </summary>
        /// <returns>
        /// Next byte
        /// </returns>
        public byte ReadByte()
        {
            return this.reader.ReadByte();
        }

        /// <summary>
        /// Read one UInt16
        /// </summary>
        /// <returns>
        /// Next UInt16
        /// </returns>
        public ushort ReadUInt16()
        {
            return (ushort)IPAddress.HostToNetworkOrder((Int16)this.reader.ReadUInt16());
        }

        /// <summary>
        /// Read one UInt32
        /// </summary>
        /// <returns>
        /// Next UInt32
        /// </returns>
        public uint ReadUInt32()
        {
            return (uint)IPAddress.HostToNetworkOrder((Int32)this.reader.ReadUInt32());
        }

        /// <summary>
        /// Read number of bytes
        /// </summary>
        /// <param name="count">
        /// number of bytes to read
        /// </param>
        /// <returns>
        /// Byte array
        /// </returns>
        public byte[] ReadBytes(int count)
        {
            return this.reader.ReadBytes(count);
        }

        /// <summary>
        /// Read a string
        /// </summary>
        /// <returns>
        /// The string
        /// </returns>
        public string ReadString()
        {
            ushort strlen = this.ReadUInt16();
            return Encoding.UTF8.GetString(this.ReadBytes(strlen));
        }

        /// <summary>
        /// The finish.
        /// </summary>
        public void Finish()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Dispose object
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose object
        /// </summary>
        /// <param name="disposing">
        /// The dispose Managed objects
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.reader != null)
                {
                    this.reader.Dispose();
                }

                if (this.stream != null)
                {
                    this.stream.Dispose();
                }
            }
        }
    }
}