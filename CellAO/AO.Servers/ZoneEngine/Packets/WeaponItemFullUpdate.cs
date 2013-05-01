// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeaponItemFullUpdate.cs" company="CellAO Team">
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
//   Defines the WeaponItemFullUpdate type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZoneEngine.Packets
{
    using SmokeLounge.AOtomation.Messaging.GameData;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    public static class WeaponItemFullUpdate
    {
        #region Public Methods and Operators

        public static void SendOwner(Client client, Character character)
        {
            // ## ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ ##
            // ## Weapon Item Full Update - Test ## Do Not attempt to send this packet until all values have been Verified ##
            // ## ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ ##

            // notes: one packet has to be sent for each hand!!!!!!! (more for each weapon, see below)
            // Packet only sent when Weapons are in Posession or being equipped the first time
            var message = new WeaponItemFullUpdateMessage
                              {
                                  Identity = new Identity
                                                 {
                                                     // Type Weapon (always instanced)
                                                     Type =
                                                         IdentityType.WeaponInstance, 

                                                     // Instance of Weapon, hardcoding this causes a crash if you want to equip more than one weapon
                                                     Instance = 0x45AEE789
                                                 }, 
                                  Unknown = 0x00, 
                                  Unknown1 = 0x0000000B, 
                                  Character = new Identity
                                                  {
                                                      // Reciever
                                                      Type =
                                                          (IdentityType)
                                                          character.Type, 
                                                      Instance = character.Id
                                                  }, 
                                  Unknown2 = 0x00000C0A, 
                                  
                                  
                                  
                                  // constant
                                  Unknown3 = 0x000F424F, 
                                  Unknown4 = 0x0000, 

                                  // Placement/Location Maybe?
                                  Unknown5 = 0x00000006, 

                                  // Another Constant
                                  Unknown6 = 0x00001F88, 

                                  // constant Flags
                                  Flags = 0x00000000, 
                                  
                                  
                                  
                                  // Item Flag
                                  ItemFlags = 0x00000403, 

                                  // constant Static Instance?
                                  StaticInstance = 0x00000017, 

                                  // Weapon ITEM ID
                                  WeaponItemId = 0x0001E6D0, 
                                  
                                  
                                  
                                  // constant ACG Item Level
                                  AcgItemLevel = 0x000002BD, 

                                  // Quality-Level
                                  QualityLevel = 0x00000005, 

                                  // constant ACGItemTemplateID
                                  AcgItemTemplateId = 0x000002BE, 

                                  // Weapon ITEM ID (low ID)
                                  WeaponItemLowId = 0x0001E6D0, 

                                  // constant ACGItemTemplateID2
                                  AcgItemTemplateId2 = 0x000002BF, 

                                  // High ID
                                  WeaponItemHighId = 0x0001E6D1, 

                                  // Constant MultipleCount
                                  MultipleCount = 0x0000019C, 

                                  // Amount
                                  Amount = 0x00000001, 

                                  // Constant Energy
                                  Energy = 0x0000001A, 

                                  // Ammo
                                  Ammo = 0x00000028, 

                                  // Empty fill
                                  Unknown7 = 0x00000000
                              };

            client.SendCompressed(message);
        }

        public static void SendPlayfield(Client client, Character character)
        {
            // ## ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ ##
            // ## Weapon Item Full Update - Test ## Do Not attempt to send this packet until all values have been Verified ##
            // ## ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ ##

            // notes: one packet has to be sent for each hand!!!!!!!
            // Packet only sent when Weapons are in Posession!
            var message = new WeaponItemFullUpdateMessage
                              {
                                  Identity = new Identity
                                                 {
                                                     // Type Weapon (always instanced)
                                                     Type =
                                                         IdentityType.WeaponInstance, 

                                                     // Instance of Weapon, hardcoding this causes a crash if you want to equip more than one weapon
                                                     Instance = 0x45AEE789
                                                 }, 
                                  Unknown = 0x00, 
                                  Unknown1 = 0x0000000B, 
                                  Character = new Identity
                                                  {
                                                      // Reciever
                                                      Type =
                                                          (IdentityType)
                                                          character.Type, 
                                                      Instance = character.Id
                                                  }, 
                                  Unknown2 = 0x00000C0A, 


                                  // constant
                                  Unknown3 = 0x000F424F, 
                                  Unknown4 = 0x0000, 

                                  // Placement/Location Maybe?
                                  Unknown5 = 0x00000006, 

                                  // Another Constant
                                  Unknown6 = 0x00001F88, 

                                  // constant Flags
                                  Flags = 0x00000000, 


                                  // Item Flag
                                  ItemFlags = 0x00000403, 

                                  // constant Static Instance?
                                  StaticInstance = 0x00000017, 

                                  // Weapon ITEM ID
                                  WeaponItemId = 0x0001E6D0, 


                                  // constant ACG Item Level
                                  AcgItemLevel = 0x000002BD, 

                                  // Quality-Level
                                  QualityLevel = 0x00000005, 

                                  // constant ACGItemTemplateID
                                  AcgItemTemplateId = 0x000002BE, 

                                  // Weapon ITEM ID (low ID)
                                  WeaponItemLowId = 0x0001E6D0, 

                                  // constant ACGItemTemplateID2
                                  AcgItemTemplateId2 = 0x000002BF, 

                                  // High ID
                                  WeaponItemHighId = 0x0001E6D1, 

                                  // Constant MultipleCount
                                  MultipleCount = 0x0000019C, 

                                  // Amount
                                  Amount = 0x00000001, 

                                  // Constant Energy
                                  Energy = 0x0000001A, 

                                  // Ammo
                                  Ammo = 0x00000028, 

                                  // Empty fill
                                  Unknown7 = 0x00000000
                              };

            client.SendCompressed(message);
        }

        #endregion
    }
}