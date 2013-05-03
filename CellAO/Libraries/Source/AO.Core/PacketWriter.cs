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
#endregion

#pragma warning disable 1591

namespace AO.Core
{
    using SmokeLounge.AOtomation.Messaging.GameData;

    /// <summary>
    /// Class for writing packets
    /// </summary>
    public class PacketWriter
    {
        private readonly MemoryStream _stream;
        private readonly BinaryWriter _writer;
        private byte[] _packet;

        public PacketWriter()
        {
            _stream = new MemoryStream();
            _writer = new BinaryWriter(_stream);
        }

        public bool PushByte(byte _byte)
        {
            _writer.Write(_byte);
            return true;
        }

        public bool PushShort(short _short)
        {
            _writer.Write(IPAddress.HostToNetworkOrder(_short));
            return true;
        }

        public bool PushInt(int _int)
        {
            _writer.Write(IPAddress.HostToNetworkOrder(_int));
            return true;
        }

        public bool PushUInt(uint _uint)
        {
            _writer.Write(IPAddress.HostToNetworkOrder((Int32) _uint));
            return true;
        }

        public bool PushUInt(int _int)
        {
            _writer.Write(IPAddress.HostToNetworkOrder(_int));
            return true;
        }

        public bool PushLong(long _long)
        {
            _writer.Write(IPAddress.HostToNetworkOrder(_long));
            return true;
        }

        public bool PushFloat(float _float)
        {
            byte[] _floata = BitConverter.GetBytes(_float);
            Array.Reverse(_floata);
            _writer.Write(_floata);
            return true;
        }

        public bool PushCoord(AOCoord _coord)
        {
            PushFloat(_coord.x);
            PushFloat(_coord.y);
            PushFloat(_coord.z);
            return true;
        }

        public bool PushQuat(Quaternion _quat)
        {
            PushFloat(_quat.xf);
            PushFloat(_quat.yf);
            PushFloat(_quat.zf);
            PushFloat(_quat.wf);
            return true;
        }

        public bool Push3F1Count(int _count)
        {
            int _mcount = ((_count + 1)*1009);
            PushInt(_mcount);
            return true;
        }

        public bool PushIdentity(Identity id)
        {
            PushInt((int)id.Type);
            PushInt(id.Instance);
            return true;
        }

        public bool PushBytes(byte[] _bytes)
        {
            _writer.Write(_bytes);
            return true;
        }

        public bool PushString(string _text)
        {
            char[] temp = _text.ToCharArray();
            bool b = true;
            foreach (char c in temp)
            {
                b = b & PushByte((byte) c);
            }
            return b;
        }

        public bool PushACGItem(int _LID, int _HID, int _QL)
        {
            PushInt(_LID);
            PushInt(_HID);
            PushInt(_QL);
            return true;
        }

        public byte[] Finish()
        {
            _stream.Capacity = (int) _stream.Length;
            _packet = _stream.GetBuffer();
            _writer.Close();
            _stream.Dispose();
            byte[] _length = BitConverter.GetBytes((short) _packet.Length);
            _packet[6] = _length[1];
            _packet[7] = _length[0];
            return _packet;
        }
    }
}