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
    internal class BankOpen
    {
        public static void Send(Client cli)
        {
            PacketWriter bank = new PacketWriter();
            bank.PushByte(0xdf);
            bank.PushByte(0xdf);
            bank.PushShort(0xa);
            bank.PushShort(1);
            bank.PushShort(0); // Length
            bank.PushInt(3086);
            bank.PushInt(cli.Character.ID);
            bank.PushInt(0x343c287f);
            bank.PushIdentity(50000, cli.Character.ID);
            bank.PushByte(1);
            bank.PushInt(0);
            bank.PushInt(0);
            bank.PushInt(0);
            bank.Push3F1Count(cli.Character.Bank.Count);
            foreach (AOItem item in cli.Character.Bank)
            {
                bank.PushInt(item.flags); // misused the flags for position in the bank
                short flags = 0;
                if (item.isInstanced())
                {
                    flags |= 0xa0;
                }
                if (item.lowID == item.highID)
                {
                    flags |= 2;
                }
                else
                {
                    flags |= 1;
                }
                // perhaps there are more flags...
                bank.PushShort(flags);
                bank.PushShort((short) item.multiplecount);
                bank.PushInt(item.Type);
                bank.PushInt(item.Instance);
                bank.PushInt(item.lowID);
                bank.PushInt(item.highID);
                bank.PushInt(item.Quality);
                bank.PushInt(0); // didnt encounter any other value
            }
            byte[] reply = bank.Finish();
            cli.SendCompressed(reply);
        }
    }
}