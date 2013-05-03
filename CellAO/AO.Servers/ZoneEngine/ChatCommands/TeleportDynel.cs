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
    using System.Globalization;

    using AO.Core;

    using SmokeLounge.AOtomation.Messaging.GameData;

    using ZoneEngine.Misc;
    using ZoneEngine.Script;

    public class ChatCommandteleportdynel : AOChatCommand
    {
        public override void ExecuteCommand(Client client, Identity target, string[] args)
        {
            List<Type> check = new List<Type>();
            check.Add(typeof(float));
            check.Add(typeof(float));
            check.Add(typeof(int));

            AOCoord coord = new AOCoord();
            int pf = client.Character.PlayField;
            if (CheckArgumentHelper(check, args))
            {
                coord = new AOCoord(
                    float.Parse(args[1], NumberStyles.Any, CultureInfo.InvariantCulture),
                    client.Character.Coordinates.y,
                    float.Parse(args[2], NumberStyles.Any, CultureInfo.InvariantCulture));
                pf = int.Parse(args[3]);
            }

            check.Clear();
            check.Add(typeof(float));
            check.Add(typeof(float));
            check.Add(typeof(string));
            check.Add(typeof(float));
            check.Add(typeof(int));

            if (CheckArgumentHelper(check, args))
            {
                coord = new AOCoord(
                    float.Parse(args[1], NumberStyles.Any, CultureInfo.InvariantCulture),
                    float.Parse(args[4], NumberStyles.Any, CultureInfo.InvariantCulture),
                    float.Parse(args[2], NumberStyles.Any, CultureInfo.InvariantCulture));
                pf = int.Parse(args[5]);
            }

            if (!Playfields.ValidPlayfield(pf))
            {
                client.SendFeedback(110, 188845972);
                return;
            }

            Client mClient = null;
            if ((mClient = FindClient.FindClientById(target.Instance)) == null)
            {
                client.SendChatText("Target not found");
                return;
            }

            mClient.Teleport(coord, mClient.Character.Heading, pf);
        }

        public override void CommandHelp(Client client)
        {
            // No help needed, no arguments can be given
            client.SendChatText("Teleports selected char to another position");
            client.SendChatText("Usage: /command teleportdynel [float] [float] [int] (X, Z, Playfield)");
            client.SendChatText("Or:    /command teleportdynel [float] [float] y [float] [int] (X, Z, Y, Playfield)");
            return;
        }

        public override bool CheckCommandArguments(string[] args)
        {
            List<Type> check = new List<Type>();
            check.Add(typeof(float));
            check.Add(typeof(float));
            check.Add(typeof(int));
            bool check1 = CheckArgumentHelper(check, args);

            check.Clear();
            check.Add(typeof(float));
            check.Add(typeof(float));
            check.Add(typeof(string));
            check.Add(typeof(float));
            check.Add(typeof(int));
            check1 |= CheckArgumentHelper(check, args);

            return check1;
        }

        public override int GMLevelNeeded()
        {
            return 1;
        }

        public override List<string> ListCommands()
        {
            List<string> temp = new List<string>();
            temp.Add("teleportdynel");
            return temp;
        }
    }
}