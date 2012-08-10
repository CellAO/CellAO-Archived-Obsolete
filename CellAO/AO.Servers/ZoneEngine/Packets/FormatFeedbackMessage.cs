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
using AO.Core;
#endregion

namespace ZoneEngine.Packets
{
    public static class FormatFeedbackMessage
    {
        public static void Send(Client client, int category, int instance, object[] args)
        {
            PacketWriter FFM = new PacketWriter();
            FFM.PushByte(0xDF);
            FFM.PushByte(0xDF);
            FFM.PushShort(10);
            FFM.PushShort(1);
            FFM.PushShort(0);
            FFM.PushInt(3086);
            FFM.PushInt(0);
            FFM.PushInt(0x206B4B73);
            FFM.PushIdentity(50000, client.Character.ID);
            FFM.PushByte(1);
            FFM.PushInt(0);

            string message = "&~" + b85(category) + "&:" + b85(instance);

            foreach (object arg in args)
            {
                if (arg is Int32)
                {
                    message = message + "i" + b85_5((Int32) arg);
                }
                if (arg is string)
                {
                    if (((string) arg).Length > 255)
                    {
                        message = message + "S";
                        Int16 len = (Int16) ((string) arg).Length;
                        message = message + shorttochar(len) + (string) arg;
                    }
                    else
                    {
                        message = message + "s" + bytetochar((byte) (((string) arg).Length));
                    }
                }
            }

            Int16 mlen = (Int16) (message.Length);
            FFM.PushShort(mlen);
            FFM.PushString(message);
            FFM.PushInt(1);

            byte[] FFMA = FFM.Finish();
            client.SendCompressed(FFMA);
        }

        public static char bytetochar(byte b)
        {
            byte[] buf = new byte[1];
            buf[0] = b;
            return BitConverter.ToChar(buf, 0);
        }

        public static string shorttochar(Int16 value)
        {
            return bytetochar((byte) ((Int16) (value >> 8))) + "" + bytetochar((byte) ((Int16) (value & 0xff)));
        }

        public static string b85(int value)
        {
            string b85conv = "";
            int i = 0;
            byte[] n85 = new byte[1];
            while (i < 4)
            {
                n85[0] = (byte) ((value%85) + 33);
                b85conv = BitConverter.ToChar(n85, 0) + b85conv;
                value = (value - (value%85))/85;
            }
            return b85conv;
        }

        public static string b85_5(int value)
        {
            string b85conv = "";
            int i = 0;
            byte[] n85 = new byte[1];
            while (i < 5)
            {
                n85[0] = (byte) ((value%85) + 33);
                b85conv = BitConverter.ToChar(n85, 0) + b85conv;
                value = (value - (value%85))/85;
            }
            return b85conv;
        }
    }
}