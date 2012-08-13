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

    internal class SwitchItem
    {
        public static void Send(Client cli, int frompage, int fromplacement, Identity to_identity, int toplacement)
        {
            PacketWriter xx = new PacketWriter();

            // Send Switch place ACK
            xx.PushByte(0xDF);
            xx.PushByte(0xDF);
            xx.PushShort(0x000a);
            xx.PushShort(0x0001);
            xx.PushShort(0); // LENGTH
            xx.PushInt(3086); // Server ID
            xx.PushInt(cli.Character.ID);
            xx.PushInt(0x47537a24);
            xx.PushInt(50000);
            xx.PushInt(cli.Character.ID);
            xx.PushByte(0);
            xx.PushInt(frompage); // Send Container ID
            xx.PushInt(fromplacement);
            xx.PushInt(to_identity.Type);
            xx.PushInt(to_identity.Instance);
            xx.PushInt(toplacement); // changed toplacement
            byte[] reply = xx.Finish();
            cli.SendCompressed(reply);
        }
    }
}