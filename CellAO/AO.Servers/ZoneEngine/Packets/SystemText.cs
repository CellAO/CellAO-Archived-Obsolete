﻿#region License
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
using AO.Core;
#endregion

namespace ZoneEngine.Packets
{
    internal class SystemText
    {
        public static void Send(Client cli, string text, int color)
        {
            PacketWriter pw = new PacketWriter();
            pw.PushByte(0xdf);
            pw.PushByte(0xdf);
            pw.PushShort(0xa);
            pw.PushShort(1);
            pw.PushShort(0);
            pw.PushInt(3086);
            pw.PushInt(cli.Character.ID);
            pw.PushInt(0x206b4b73);
            pw.PushInt(50000);
            pw.PushInt(cli.Character.ID);
            pw.PushByte(1);
            pw.PushInt(0);
            pw.PushInt(0x557e26);
            pw.PushInt(0x21212122);
            pw.PushInt(0x3a212121);
            pw.PushShort(0x293c);
            pw.PushByte(0x73);
            pw.PushByte((byte) text.Length);
            pw.PushString(text);
            pw.PushInt(0);

            byte[] packet = pw.Finish();

            cli.SendCompressed(packet);
        }
    }
}