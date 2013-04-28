// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TradeskillPacket.cs" company="CellAO Team">
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
//   Defines the TradeskillPacket type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZoneEngine.Packets
{
    using SmokeLounge.AOtomation.Messaging.GameData;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    public static class TradeskillPacket
    {
        #region Public Methods and Operators

        public static void SendNotTradeskill(Character character)
        {
            var message = new CharacterActionMessage
                              {
                                  Identity =
                                      new Identity
                                          {
                                              Type = IdentityType.CanbeAffected, 
                                              Instance = character.Id
                                          }, 
                                  Unknown = 0x00, 
                                  Action = CharacterActionType.TradeskillNotValid, 
                                  Unknown1 = 0x00000000, 
                                  Target = Identity.None, 
                                  Parameter1 = 0x00000000, 
                                  Parameter2 = 0x00000000, 
                                  Unknown2 = 0x0000
                              };

            character.Client.SendCompressed(message);
        }

        public static void SendOutOfRange(Character character, int min)
        {
            var message = new CharacterActionMessage
                              {
                                  Identity =
                                      new Identity
                                          {
                                              Type = IdentityType.CanbeAffected, 
                                              Instance = character.Id
                                          }, 
                                  Unknown = 0x00, 
                                  Action = CharacterActionType.TradeskillOutOfRange, 
                                  Unknown1 = 0x00000000, 
                                  Target = Identity.None, 
                                  Parameter1 = 0x00000000, 
                                  Parameter2 = min, 
                                  Unknown2 = 0x0000
                              };

            character.Client.SendCompressed(message);
        }

        public static void SendRequirement(Character character, TradeSkillSkillInfo tradeSkillSkillInfo)
        {
            var message = new CharacterActionMessage
                              {
                                  Identity =
                                      new Identity
                                          {
                                              Type = IdentityType.CanbeAffected, 
                                              Instance = character.Id
                                          }, 
                                  Unknown = 0x00, 
                                  Action = CharacterActionType.TradeskillRequirement, 
                                  Unknown1 = 0x00000000, 
                                  Target = Identity.None, 
                                  Parameter1 = tradeSkillSkillInfo.Skill, 
                                  Parameter2 = tradeSkillSkillInfo.Requirement, 
                                  Unknown2 = 0x0000
                              };

            character.Client.SendCompressed(message);
        }

        public static void SendResult(Character character, int min, int max, int low, int high)
        {
            var message = new CharacterActionMessage
                              {
                                  Identity =
                                      new Identity
                                          {
                                              Type = IdentityType.CanbeAffected, 
                                              Instance = character.Id
                                          }, 
                                  Unknown = 0x00, 
                                  Action = CharacterActionType.TradeskillResult, 
                                  Unknown1 = 0x00000000, 
                                  Target =
                                      new Identity { Type = (IdentityType)max, Instance = high }, 
                                  Parameter1 = min, 
                                  Parameter2 = low, 
                                  Unknown2 = 0x0000
                              };

            character.Client.SendCompressed(message);
        }

        public static void SendSource(Character character, int count)
        {
            var message = new CharacterActionMessage
                              {
                                  Identity =
                                      new Identity
                                          {
                                              Type = IdentityType.CanbeAffected, 
                                              Instance = character.Id
                                          }, 
                                  Unknown = 0x00, 
                                  Action = CharacterActionType.TradeskillSource, 
                                  Unknown1 = 0x00000000, 
                                  Target = Identity.None, 
                                  Parameter1 = 0x00000000, 
                                  Parameter2 = count, 
                                  Unknown2 = 0x0000
                              };

            character.Client.SendCompressed(message);
        }

        public static void SendTarget(Character character, int count)
        {
            var message = new CharacterActionMessage
                              {
                                  Identity =
                                      new Identity
                                          {
                                              Type = IdentityType.CanbeAffected, 
                                              Instance = character.Id
                                          }, 
                                  Unknown = 0x00, 
                                  Action = CharacterActionType.TradeskillTarget, 
                                  Unknown1 = 0x00000000, 
                                  Target = Identity.None, 
                                  Parameter1 = 0x00000000, 
                                  Parameter2 = count, 
                                  Unknown2 = 0x0000
                              };

            character.Client.SendCompressed(message);
        }

        #endregion
    }
}