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

namespace ZoneEngine.Packets
{
    using AO.Core;

    public class VendingMachineFullUpdate
    {
        public static void Send(Client client, VendingMachine vm)
        {
            PacketWriter pw = new PacketWriter();

            pw.PushByte(0xdf);
            pw.PushByte(0xdf);
            pw.PushShort(0xa);
            pw.PushShort(1);
            pw.PushShort(0);
            pw.PushInt(3086);
            pw.PushInt(client.Character.ID);
            pw.PushInt(0x7f544905); // 20
            pw.PushIdentity(vm.Type, vm.ID);
            pw.PushByte(0);
            pw.PushInt(0xb); // Counter??
            pw.PushInt(0);
            pw.PushInt(0); // 41
            pw.PushCoord(vm.Coordinates);
            pw.PushQuat(vm.Heading); // 69
            pw.PushInt(vm.PlayField);
            pw.PushInt(1000015);
            pw.PushInt(0);
            pw.PushShort(0x6f);
            pw.PushInt(0x2379);
            pw.PushInt(0); // 91
            pw.PushByte(0x80);
            pw.PushByte(2);
            pw.PushShort(0x3603);
            pw.PushInt(0x17);
            pw.PushInt(vm.TemplateID);
            pw.PushInt(0x2bd);
            pw.PushInt(0); // 111
            pw.PushInt(0x2be);
            pw.PushInt(0);
            pw.PushInt(0x2bf);
            pw.PushInt(0);
            pw.PushInt(0x19c); // 131
            pw.PushInt(1);
            pw.PushInt(0x1f5);
            pw.PushInt(2);
            pw.PushInt(0x1f4);
            pw.PushInt(0);
            pw.PushInt(0);
            pw.PushInt(2);
            pw.PushInt(0x32); // 147
            pw.Push3F1Count(0);
            pw.PushInt(3); // 155<

            byte[] packet = pw.Finish();
            client.SendCompressed(packet);
        }
    }
}