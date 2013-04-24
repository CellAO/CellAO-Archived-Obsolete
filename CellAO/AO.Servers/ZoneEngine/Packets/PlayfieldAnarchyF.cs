// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayfieldAnarchyF.cs" company="CellAO Team">
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
//   Defines the PlayfieldAnarchyF type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZoneEngine.Packets
{
    using SmokeLounge.AOtomation.Messaging.GameData;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    using ZoneEngine.Misc;
    using ZoneEngine.NonPlayerCharacter;

    public static class PlayfieldAnarchyF
    {
        #region Public Methods and Operators

        public static void Send(Client client)
        {
            var message = new PlayfieldAnarchyFMessage
                              {
                                  Identity =
                                      new Identity
                                          {
                                              IdentityType = IdentityType.Playfield2, 
                                              Instance = client.Character.PlayField
                                          }, 
                                  CharacterCoordinates =
                                      new Vector3
                                          {
                                              X = client.Character.Coordinates.x, 
                                              Y = client.Character.Coordinates.y, 
                                              Z = client.Character.Coordinates.z, 
                                          }, 
                                  PlayfieldId1 =
                                      new Identity
                                          {
                                              IdentityType = IdentityType.Playfield1, 
                                              Instance = client.Character.PlayField
                                          }, 
                                  PlayfieldId2 =
                                      new Identity
                                          {
                                              IdentityType = IdentityType.Playfield2, 
                                              Instance = client.Character.PlayField
                                          }, 
                                  PlayfieldX =
                                      Playfields.GetPlayfieldX(client.Character.PlayField), 
                                  PlayfieldZ =
                                      Playfields.GetPlayfieldZ(client.Character.PlayField)
                              };

            var vendorcount = VendorHandler.GetNumberofVendorsinPlayfield(client.Character.PlayField);
            if (vendorcount > 0)
            {
                var firstVendorId = VendorHandler.GetFirstVendor(client.Character.PlayField);
                message.PlayfieldVendorInfo = new PlayfieldVendorInfo
                                                  {
                                                      VendorCount = vendorcount, 
                                                      FirstVendorId = firstVendorId
                                                  };
            }

            client.SendCompressed(3086, client.Character.Id, message);
        }

        #endregion
    }
}