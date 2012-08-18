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

    public class ChatCommandTurnToMe : AOChatCommand
    {
        public override void ExecuteCommand(Client client, Identity target, string[] args)
        {
            Dynel _target = FindDynel.FindDynelByID(target.Type, target.Instance);
            if (_target == null)
            {
                this.CommandHelp(client);
                return;
            }

            if ((_target is Character) && (!(_target is NonPlayerCharacterClass)))
            {
                this.CommandHelp(client);
                return;
            }

            Vector3 v3 = new Vector3(0, 0, 0);
            switch (args[1].ToLower())
            {
                case "north":
                case "n":
                    v3.z = 1;
                    break;
                case "west":
                case "w":
                    v3.x = 1;
                    break;
                case "south":
                case "s":
                    v3.z = -1;
                    break;
                case "east":
                case "e":
                    v3.x = -1;
                    break;
                case "me":
                    Vector3 tempchar = new Vector3(client.Character.Coordinates.x, 0, client.Character.Coordinates.z);
                    Vector3 temptarget = new Vector3(_target.Coordinates.x, 0, _target.Coordinates.z);
                    v3 = temptarget - tempchar;
                    v3.z = -v3.z;
                    break;
                default:
                    this.CommandHelp(client);
                    return;
            }
            v3.y = 0.0000000;
            _target.rawHeading = _target.rawHeading.GenerateRotationFromDirectionVector(v3.Normalize());
            client.Teleport(client.Character.rawCoord, client.Character.rawHeading, client.Character.PlayField);
        }

        public override void CommandHelp(Client client)
        {
            client.SendChatText("Usage: /command turnto [me/north/south/west/east]");
            client.SendChatText("Lets the selected NPC target North, South, West, East or yourself (me).");
            return;
        }

        public override bool CheckCommandArguments(string[] args)
        {
            List<Type> check = new List<Type>();
            check.Add(typeof(string));
            return true;
        }

        public override int GMLevelNeeded()
        {
            return 0;
        }

        public override List<string> ListCommands()
        {
            List<string> temp = new List<string>();
            temp.Add("turnto");
            return temp;
        }
    }
}