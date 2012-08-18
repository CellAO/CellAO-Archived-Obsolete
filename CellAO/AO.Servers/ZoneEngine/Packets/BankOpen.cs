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

    public static class BankOpen
    {
        public static void Send(Client client)
        {
            PacketWriter packetWriter = new PacketWriter();
            packetWriter.PushByte(0xdf);
            packetWriter.PushByte(0xdf);
            packetWriter.PushShort(0xa);
            packetWriter.PushShort(1);
            packetWriter.PushShort(0); // Length
            packetWriter.PushInt(3086);
            packetWriter.PushInt(client.Character.Id);
            packetWriter.PushInt(0x343c287f);
            packetWriter.PushIdentity(50000, client.Character.Id);
            packetWriter.PushByte(1);
            packetWriter.PushInt(0);
            packetWriter.PushInt(0);
            packetWriter.PushInt(0);
            packetWriter.Push3F1Count(client.Character.Bank.Count);
            foreach (AOItem item in client.Character.Bank)
            {
                packetWriter.PushInt(item.Flags); // misused the flags for position in the bank
                short flags = 0;
                if (item.isInstanced())
                {
                    flags |= 0xa0;
                }
                if (item.LowID == item.HighID)
                {
                    flags |= 2;
                }
                else
                {
                    flags |= 1;
                }
                // perhaps there are more flags...
                packetWriter.PushShort(flags);
                packetWriter.PushShort((short)item.MultipleCount);
                packetWriter.PushInt(item.Type);
                packetWriter.PushInt(item.Instance);
                packetWriter.PushInt(item.LowID);
                packetWriter.PushInt(item.HighID);
                packetWriter.PushInt(item.Quality);
                packetWriter.PushInt(0); // didnt encounter any other value
            }
            byte[] reply = packetWriter.Finish();
            client.SendCompressed(reply);
        }
    }
}