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

    public class ChatCommandGet : AOChatCommand
    {
        public override void ExecuteCommand(Client client, Identity target, string[] args)
        {
            // Fallback to self if no target is selected
            if (target.Instance == 0)
            {
                target.Type = client.Character.Type;
                target.Instance = client.Character.ID;
            }
            if (target.Type != 50000)
            {
                client.SendChatText("Target must be player/monster/NPC");
                return;
            }
            Dynel dyn;
            dyn = FindDynel.FindDynelByID(target.Type, target.Instance);
            if (dyn != null)
            {
                Character ch = (Character)dyn;
                //ch.CalculateSkills();  // May be obsolete in the future
                int statId = StatsList.GetStatId(args[1]);
                if (statId == 1234567890)
                {
                    client.SendChatText("Unknown Stat name " + args[1]);
                    return;
                }

                uint statValue;
                int effectiveValue;
                int trickle;
                int mod;
                int perc;
                try
                {
                    statValue = ch.Stats.GetBaseValue(statId);
                    effectiveValue = ch.Stats.Get(statId);
                    trickle = ch.Stats.GetStatbyNumber(statId).Trickle;
                    mod = ch.Stats.GetStatbyNumber(statId).StatModifier;
                    perc = ch.Stats.GetStatbyNumber(statId).StatPercentageModifier;
                }
                catch
                {
                    client.SendChatText("Unknown Stat Id " + statId);
                    return;
                }

                string response = "Character " + ch.Name + " (" + ch.ID + "): Stat " + StatsList.GetStatName(statId)
                                  + " (" + statId + ") = " + statValue;

                client.SendChatText(response);
                if (statValue != ch.Stats.Get(args[1]))
                {
                    response = "Effective value Stat " + StatsList.GetStatName(statId) + " (" + statId + ") = "
                               + effectiveValue;
                    client.SendChatText(response);
                }
                response = "Trickle: " + trickle + " Modificator: " + mod + " Percentage: " + perc;
                client.SendChatText(response);
            }
            else
            {
                // Shouldnt be happen again (fallback to self)
                client.SendChatText("Unable to find target.");
            }
        }

        public override void CommandHelp(Client client)
        {
            // No help needed, no arguments can be given
            return;
        }

        public override bool CheckCommandArguments(string[] args)
        {
            // Two different checks return true: <int> <uint> and <string> <uint>
            List<Type> check = new List<Type>();
            check.Add(typeof(int));
            bool check1 = this.CheckArgumentHelper(check, args);

            check.Clear();
            check.Add(typeof(string));

            check1 |= this.CheckArgumentHelper(check, args);
            return check1;
        }

        public override int GMLevelNeeded()
        {
            // Be a GM
            return 1;
        }

        public override List<string> GetCommands()
        {
            List<string> temp = new List<string>();
            temp.Add("get");
            return temp;
        }
    }
}