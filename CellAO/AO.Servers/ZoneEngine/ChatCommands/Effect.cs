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

namespace ZoneEngine.ChatCommands
{
    using System;
    using System.Collections.Generic;

    using AO.Core;

    using ZoneEngine.Misc;
    using ZoneEngine.Script;

    public class ChatCommandEffect : AOChatCommand
    {
        public override void ExecuteCommand(Client client, Identity target, string[] args)
        {
            // No check needed, its done by CheckCommandArguments
            int gfx = int.Parse(args[3]);

            //begin assembling packet here
            PacketWriter packet = new PacketWriter();
            packet.PushByte(0xDF);
            packet.PushByte(0xDF);
            packet.PushShort(10);
            packet.PushShort(1);
            packet.PushShort(0);
            packet.PushInt(3086);
            packet.PushInt(client.Character.ID);
            packet.PushInt(0x4D450114);
            packet.PushIdentity(target);
            packet.PushByte(0);
            packet.Push3F1Count(1); // effects count
            // effect starts
            packet.PushIdentity(53030, 0); // effect ID (53030 = GfxEffect)
            packet.PushInt(4); // ?
            packet.PushInt(0); // Criterion count
            packet.PushInt(1); // Hits
            packet.PushInt(0); // Delay
            packet.PushInt(0); // 
            packet.PushInt(0); // 
            // effect args
            packet.PushInt(gfx); // Value
            packet.PushInt(0); // GfxLife
            packet.PushInt(0); // GfxSize
            packet.PushInt(0); // GfxRed
            packet.PushInt(0); // GfxGreen
            packet.PushInt(0); // GfxBlue
            packet.PushInt(0); // GfxFade
            // effect args end
            // effect ends
            packet.PushIdentity(50000, client.Character.ID);
            byte[] reply = packet.Finish();
            //done creating the packet
            Announce.Playfield(client.Character.PlayField, ref reply);
        }

        public override void CommandHelp(Client client)
        {
            client.SendChatText("/command effect gfxeffect value <int>");
        }

        public override bool CheckCommandArguments(string[] args)
        {
            List<Type> check = new List<Type>();
            check.Add(typeof(string));
            check.Add(typeof(string));
            check.Add(typeof(int));
            return this.CheckArgumentHelper(check, args);
        }

        public override int GMLevelNeeded()
        {
            // Be a GM at least
            return 1;
        }

        public override List<string> GetCommands()
        {
            List<string> temp = new List<string>();
            temp.Add("effect");
            return temp;
        }
    }
}