﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnubotRejectedItems.cs" company="CellAO Team">
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
//   Defines the KnuBotRejectedItems type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZoneEngine.Packets
{
    using System.Linq;

    using AO.Core;

    using SmokeLounge.AOtomation.Messaging.GameData;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    using Identity = SmokeLounge.AOtomation.Messaging.GameData.Identity;

    public static class KnuBotRejectedItems
    {
        #region Public Methods and Operators

        public static void Send(Client cli, NonPlayerCharacterClass knubotTarget, AOItem[] items)
        {
            var message = new KnuBotRejectedItemsMessage
                              {
                                  Identity = cli.Character.Id, 
                                  Unknown = 0x00, 
                                  Unknown1 = 0x00002,
                                  Target = knubotTarget.Id, 
                                  Items =
                                      items.Select(
                                          i =>
                                          new KnuBotRejectedItem
                                              {
                                                  LowId = i.LowID, 
                                                  HighId = i.HighID, 
                                                  Quality = i.Quality, 
                                                  Unknown = 0x499602d2
                                                  
                                                  // 1234567890  ???????
                                              }).ToArray(), 
                                  Unknown2 = 0x00000000
                              };

            cli.SendCompressed(message);
        }

        #endregion
    }
}