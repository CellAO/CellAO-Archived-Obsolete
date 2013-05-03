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

namespace ZoneEngine.Packets
{
    using System;

    using AO.Core;

    [Obsolete]
    public static class FormatFeedbackMessage
    {
        public static void Send(Client client, int category, int instance, object[] args)
        {
            PacketWriter packetWriter = new PacketWriter();
            packetWriter.PushByte(0xDF);
            packetWriter.PushByte(0xDF);
            packetWriter.PushShort(10);
            packetWriter.PushShort(1);
            packetWriter.PushShort(0);
            packetWriter.PushInt(3086);
            packetWriter.PushInt(0);
            packetWriter.PushInt(0x206B4B73);
            packetWriter.PushIdentity(client.Character.Id);
            packetWriter.PushByte(1);
            packetWriter.PushInt(0);

            string message = "&~" + Encode85By4(category) + "&:" + Encode85By4(instance);

            foreach (object arg in args)
            {
                if (arg is Int32)
                {
                    message = message + "i" + Encode85By5((Int32)arg);
                }
                string stringArg = arg as string;
                if (stringArg != null)
                {
                    if (stringArg.Length > 255)
                    {
                        message = message + "S";
                        Int16 len = (Int16)stringArg.Length;
                        message = message + ShortToChar(len) + stringArg;
                    }
                    else
                    {
                        message = message + "s" + ByteToChar((byte)(stringArg.Length));
                    }
                }
            }

            Int16 mlen = (Int16)(message.Length);
            packetWriter.PushShort(mlen);
            packetWriter.PushString(message);
            packetWriter.PushInt(1);

            byte[] packet = packetWriter.Finish();
            client.SendCompressed(packet);
        }

        public static char ByteToChar(byte value)
        {
            byte[] buffer = new byte[1];
            buffer[0] = value;
            return BitConverter.ToChar(buffer, 0);
        }

        public static string ShortToChar(Int16 value)
        {
            return ByteToChar((byte)((Int16)(value >> 8))) + "" + ByteToChar((byte)((Int16)(value & 0xff)));
        }

        public static string Encode85By4(int value)
        {
            string b85Conv = "";
            int i = 0;
            byte[] n85 = new byte[1];
            while (i < 4)
            {
                n85[0] = (byte)((value % 85) + 33);
                b85Conv = BitConverter.ToChar(n85, 0) + b85Conv;
                value = (value - (value % 85)) / 85;
            }
            return b85Conv;
        }

        public static string Encode85By5(int value)
        {
            string b85Conv = "";
            int i = 0;
            byte[] n85 = new byte[1];
            while (i < 5)
            {
                n85[0] = (byte)((value % 85) + 33);
                b85Conv = BitConverter.ToChar(n85, 0) + b85Conv;
                value = (value - (value % 85)) / 85;
            }
            return b85Conv;
        }
    }
}