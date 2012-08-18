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

namespace ZoneEngine.ChatCommands
{
    using System;
    using System.Collections.Generic;

    using AO.Core;

    using ZoneEngine.Misc;
    using ZoneEngine.Packets;
    using ZoneEngine.Script;

    public class ChatCommandNanoUpload : AOChatCommand
    {
        public override void ExecuteCommand(Client client, Identity target, string[] args)
        {
            Client targetClient;
            if (FindClient.FindClientByName(args[1], out targetClient))
            {
                bool hasNano = (this.HasNano(Convert.ToInt32(args[2]), targetClient));
                if (hasNano == false)
                {
                    targetClient.Character.UploadNano(Convert.ToInt32(args[2]));
                    UploadNanoupdate.Send(targetClient.Character, 53019, Convert.ToInt32(args[2]));
                    return;
                }
                targetClient.SendChatText("The nano already exists in nano programs..");
            }
        }

        public bool HasNano(int nanoId, Client client)
        {
            bool found = false;
            foreach (AOUploadedNanos uploadedNano in client.Character.UploadedNanos)
            {
                if (uploadedNano.Nano != nanoId)
                {
                    continue;
                }
                found = true;
                break;
            }
            return found;
        }

        public override void CommandHelp(Client client)
        {
            client.SendChatText("Usage: /command nanoupload charname nanoId");
            return;
        }

        public override bool CheckCommandArguments(string[] args)
        {
            // Always true, only string arguments
            List<Type> check = new List<Type> { typeof(string), typeof(int) };

            return CheckArgumentHelper(check, args);
        }

        public override int GMLevelNeeded()
        {
            return 1;
        }

        public override List<string> ListCommands()
        {
            List<string> temp = new List<string> { "nanoupload" };
            return temp;
        }
    }
}