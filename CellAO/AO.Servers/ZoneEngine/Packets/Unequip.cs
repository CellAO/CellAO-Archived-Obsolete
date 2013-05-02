// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Unequip.cs" company="CellAO Team">
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
//   Defines the Unequip type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZoneEngine.Packets
{
    using AO.Core;

    using SmokeLounge.AOtomation.Messaging.GameData;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    using Identity = SmokeLounge.AOtomation.Messaging.GameData.Identity;

    public static class Unequip
    {
        #region Public Methods and Operators

        public static void Send(Client client, AOItem item, int page, int placement)
        {
            // tell the client to remove (07) the item modifiers (AC, skills and so on)

            // if (placement == 6)
            switch (placement)
            {
                case 6: // Right Hand

                    // Action 97
                    var action97 = new CharacterActionMessage
                                       {
                                           Identity =
                                               new Identity
                                                   {
                                                       Type = IdentityType.CanbeAffected, 
                                                       Instance = client.Character.Id
                                                   }, 
                                           Unknown = 0x00, 
                                           Action = CharacterActionType.Unknown3, 
                                           Unknown1 = 0x00000000, 
                                           Target = Identity.None, 
                                           Parameter1 = 0x00000000, 
                                           Parameter2 = 0x00000006, 
                                           Unknown2 = 0x0000
                                       };
                    client.SendCompressed(action97);

                    break;

                default:
                    var message = new TemplateActionMessage
                                      {
                                          Identity =
                                              new Identity
                                                  {
                                                      Type = IdentityType.CanbeAffected, 
                                                      Instance = client.Character.Id
                                                  }, 
                                          Unknown = 0x00, 
                                          ItemLowId = item.LowID, 
                                          ItemHighId = item.HighID, 
                                          Quality = item.Quality, 
                                          Unknown1 = 1, 
                                          Unknown2 = 7, 
                                          Placement =
                                              new Identity
                                                  {
                                                      Type = (IdentityType)page, 
                                                      Instance = placement
                                                  }, 
                                          Unknown3 = 0x00000000, 
                                          Unknown4 = 0x00000000
                                      };

                    client.SendCompressed(message);
                    break;
            }
        }

        #endregion
    }
}