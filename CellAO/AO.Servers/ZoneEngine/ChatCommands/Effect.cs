// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Effect.cs" company="CellAO Team">
//   Copyright © 2005-2013 CellAO Team.
//   
//   All rights reserved.
//   
//   Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//   
//       * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//       * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//       * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//   
//   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//   A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//   CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//   EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//   PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//   LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//   NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//   SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// </copyright>
// <summary>
//   Defines the ChatCommandEffect type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZoneEngine.ChatCommands
{
    using System;
    using System.Collections.Generic;

    using SmokeLounge.AOtomation.Messaging.GameData;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    using ZoneEngine.Misc;
    using ZoneEngine.Script;

    public class ChatCommandEffect : AOChatCommand
    {
        #region Public Methods and Operators

        public override bool CheckCommandArguments(string[] args)
        {
            var check = new List<Type>();
            check.Add(typeof(string));
            check.Add(typeof(string));
            check.Add(typeof(int));
            return CheckArgumentHelper(check, args);
        }

        public override void CommandHelp(Client client)
        {
            client.SendChatText("/command effect gfxeffect value <int>");
        }

        public override void ExecuteCommand(Client client, Identity target, string[] args)
        {
            // No check needed, its done by CheckCommandArguments
            var gfx = int.Parse(args[3]);

            var nanoEffect = new NanoEffect
                                 {
                                     Effect = new Identity { Type = IdentityType.GfxEffect, Instance = 0 }, 
                                     Unknown1 = 0x00000004, 
                                     CriterionCount = 0x00000000, 
                                     Hits = 0x00000001, 
                                     Delay = 0x00000000, 
                                     Unknown2 = 0x00000000, 
                                     Unknown3 = 0x00000000, 
                                     GfxValue = gfx, 
                                     GfxLife = 0x00000000, 
                                     GfxSize = 0x00000000, 
                                     GfxRed = 0x00000000, 
                                     GfxGreen = 0x00000000, 
                                     GfxBlue = 0x00000000, 
                                     GfxFade = 0x00000000
                                 };
            var msg = new SpellListMessage
                          {
                              Identity = target, 
                              Unknown = 0x00, 
                              NanoEffects = new[] { nanoEffect },
                              Character = client.Character.Id 
                          };

            Announce.Playfield(client.Character.PlayField, msg);
        }

        public override int GMLevelNeeded()
        {
            // Be a GM at least
            return 1;
        }

        public override List<string> ListCommands()
        {
            var temp = new List<string>();
            temp.Add("effect");
            return temp;
        }

        #endregion
    }
}