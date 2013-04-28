// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Equip.cs" company="CellAO Team">
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
//   Defines the Equip type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZoneEngine.Packets
{
    using AO.Core;

    using SmokeLounge.AOtomation.Messaging.GameData;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    using Identity = SmokeLounge.AOtomation.Messaging.GameData.Identity;

    public static class Equip
    {
        #region Public Methods and Operators

        public static void Send(Client cli, AOItem it, int page, int placement)
        {
            // if (placement == 6)
            switch (placement)
            {
                case 6: // Right Hand

                    /*
                    // ContainerAddItem Reply
                    equippacket.PushByte(0xdf);
                    equippacket.PushByte(0xdf);
                    equippacket.PushShort(0xa);
                    equippacket.PushShort(0x1);
                    equippacket.PushShort(0);
                    equippacket.PushInt(3086);
                    equippacket.PushInt(cli.Character.ID);
                    equippacket.PushInt(0x47537A24);
                    equippacket.PushInt(50000);
                    equippacket.PushInt(cli.Character.ID);
                    equippacket.PushByte(0);
                    equippacket.PushInt(0x60);
                    equippacket.PushInt(0x40);
                    equippacket.PushInt(50000);
                    equippacket.PushInt(cli.Character.ID);
                    equippacket.PushInt(0x6);
                    
                    byte[] reply = equippacket.Finish();
                    cli.SendCompressed(reply);
                    */

                    // Action 167 Reply
                    var action167 = new CharacterActionMessage
                                        {
                                            Identity =
                                                new Identity
                                                    {
                                                        Type = IdentityType.CanbeAffected, 
                                                        Instance = cli.Character.Id
                                                    }, 
                                            Unknown = 0x00, 
                                            Action = CharacterActionType.ChangeAnimationAndStance, 
                                            Unknown1 = 0x00000000, 
                                            Target = Identity.None, 
                                            Parameter1 = 0x00000000, 
                                            Parameter2 = 0x00000000, 
                                            Unknown2 = 0x0000
                                        };
                    cli.SendCompressed(action167);

                    // Action 131 Reply
                    var action131 = new CharacterActionMessage
                                        {
                                            Identity =
                                                new Identity
                                                    {
                                                        Type = IdentityType.CanbeAffected, 
                                                        Instance = cli.Character.Id
                                                    }, 
                                            Unknown = 0x00, 
                                            Action = CharacterActionType.Equip, 
                                            Unknown1 = 0x00000000, 
                                            Target =
                                                new Identity
                                                    {
                                                        Type = (IdentityType)0xC74A, 
                                                        Instance = 0x4598815B
                                                    }, 
                                            Parameter1 = 0x00000000, 
                                            Parameter2 = 0x00000006, 
                                            Unknown2 = 0x0000
                                        };
                    cli.SendCompressed(action131);

                    break;

                default:
                    var message = new TemplateActionMessage
                                      {
                                          Identity =
                                              new Identity
                                                  {
                                                      Type = IdentityType.CanbeAffected, 
                                                      Instance = cli.Character.Id
                                                  }, 
                                          Unknown = 0x00, 
                                          ItemLowId = it.LowID, 
                                          ItemHighId = it.HighID, 
                                          Quality = it.Quality, 
                                          Unknown1 = 1, 
                                          Unknown2 =
                                              (placement >= 49) && (placement <= 63) ? 7 : 6, 
                                          Placement =
                                              new Identity
                                                  {
                                                      Type = (IdentityType)page, 
                                                      Instance = placement
                                                  }, 
                                          Unknown3 = 0x00000000, 
                                          Unknown4 = 0x00000000
                                      };

                    if (!((placement >= 49) && (placement <= 63)))
                    {
                        cli.SendCompressed(message);
                    }

                    break;
            }
        }

        #endregion
    }
}