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
using AO.Core;
#endregion

namespace ZoneEngine.Packets
{
    public class SimpleItemFullUpdate
    {
        public static void UpdateItemsCache(Character ch)
        {
            int CurrentSlot = 64;

            while (CurrentSlot < 109)
            {
                CurrentSlot++;
            }
        }

        public void SendPlayField(Client cli, VendingMachine vm, int itemnum)
        {
            PacketWriter pw = new PacketWriter();

            pw.PushByte(0xdf);
            pw.PushByte(0xdf);
            pw.PushShort(0xa);
            pw.PushShort(1);
            pw.PushShort(0);
            pw.PushInt(3086);
            pw.PushInt(cli.Character.ID);
            pw.PushInt(0x3b11256f);
            pw.PushIdentity(0xc76e, 0x021fa86f); // whats this one???
            pw.PushByte(0);
            pw.PushInt(11);
            pw.PushIdentity(cli.Character.Type, cli.Character.ID);
            pw.PushInt(cli.Character.PlayField);
            pw.PushInt(0x0f424f);
            pw.PushInt(0);
            pw.PushShort(0x656f); // ??????
            pw.Push3F1Count(6);
            pw.PushInt(0);
            pw.PushByte(0x80);
            pw.PushByte(0);
            pw.PushShort(0x0203);
            pw.PushInt(0x17);
            pw.PushInt(vm.Inventory[itemnum].Item.lowID); // TODO: 3 times lowID and no highID?
            pw.PushInt(0x2bd);
            pw.PushInt(1);
            pw.PushInt(0x2be);
            pw.PushInt(vm.Inventory[itemnum].Item.lowID);
            pw.PushInt(0x2bf);
            pw.PushInt(vm.Inventory[itemnum].Item.lowID);
            pw.PushInt(0x19c);
            pw.PushInt(1);
            pw.PushInt(0);
        }

        public void SendOwner(Client cli, VendingMachine vm, int itemnum)
        {
            PacketWriter pw = new PacketWriter();

            pw.PushByte(0xdf);
            pw.PushByte(0xdf);
            pw.PushShort(0xa);
            pw.PushShort(1);
            pw.PushShort(0);
            pw.PushInt(3086);
            pw.PushInt(cli.Character.ID);
            pw.PushInt(0x3b11256f);
            pw.PushIdentity(0xc76e, 0x021fa86f); // whats this one???
            pw.PushByte(0);
            pw.PushInt(11);
            pw.PushIdentity(cli.Character.Type, cli.Character.ID);
            pw.PushInt(cli.Character.PlayField);
            pw.PushInt(0x0f424f);
            pw.PushInt(0);
            pw.PushShort(0x656f); // ??????
            pw.Push3F1Count(6);
            pw.PushInt(0);
            pw.PushByte(0x80);
            pw.PushByte(0);
            pw.PushShort(0x0203);
            pw.PushInt(0x17);
            pw.PushInt(vm.Inventory[itemnum].Item.lowID); // TODO: Three times low id and no high id?
            pw.PushInt(0x2bd);
            pw.PushInt(1);
            pw.PushInt(0x2be);
            pw.PushInt(vm.Inventory[itemnum].Item.lowID);
            pw.PushInt(0x2bf);
            pw.PushInt(vm.Inventory[itemnum].Item.lowID);
            pw.PushInt(0x19c);
            pw.PushInt(1);
            pw.PushInt(0);
        }
    }
}