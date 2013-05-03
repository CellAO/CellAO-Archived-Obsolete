// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VendingMachineFullUpdate.cs" company="CellAO Team">
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
//   Defines the VendingMachineFullUpdate type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZoneEngine.Packets
{
    using SmokeLounge.AOtomation.Messaging.GameData;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    public static class VendingMachineFullUpdate
    {
        #region Public Methods and Operators

        public static void Send(Client client, VendingMachine vendingMachine)
        {
            var message = new VendingMachineFullUpdateMessage
                              {
                                  Identity = vendingMachine.Id, 
                                  Unknown = 0x00, 
                                  Unknown1 = 0x0000000B, 
                                  Unknown2 = 0x00000000, 
                                  Unknown3 = 0x00000000, 
                                  Coordinates =
                                      new Vector3
                                          {
                                              X = vendingMachine.Coordinates.x, 
                                              Y = vendingMachine.Coordinates.y, 
                                              Z = vendingMachine.Coordinates.z, 
                                          }, 
                                  Heading =
                                      new Quaternion
                                          {
                                              X = vendingMachine.Heading.xf, 
                                              Y = vendingMachine.Heading.yf, 
                                              Z = vendingMachine.Heading.zf, 
                                              W = vendingMachine.Heading.wf, 
                                          }, 
                                  PlayfieldId = vendingMachine.PlayField, 
                                  Unknown4 = 0x000F424F, 
                                  Unknown5 = 0, 
                                  Unknown6 = 0x006F, 
                                  Unknown7 = 0x00002379, 
                                  Unknown8 = 0x00000000, 
                                  Unknown9 = 0x80, 
                                  Unknown10 = 0x02, 
                                  Unknown11 = 0x3603, 
                                  Unknown12 = 0x00000017, 
                                  TemplateId = vendingMachine.TemplateId, 
                                  Unknown13 = 0x000002BD, 
                                  Unknown14 = 0x00000000, 
                                  Unknown15 = 0x000002BE, 
                                  Unknown16 = 0x00000000, 
                                  Unknown17 = 0x000002BF, 
                                  Unknown18 = 0x00000000, 
                                  Unknown19 = 0x0000019C, 
                                  Unknown20 = 0x00000001, 
                                  Unknown21 = 0x000001F5, 
                                  Unknown22 = 0x00000002, 
                                  Unknown23 = 0x000001F4, 
                                  Unknown24 = 0x00000000, 
                                  Unknown25 = 0x00000000, 
                                  Unknown26 = 0x00000002, 
                                  Unknown27 = 0x00000032, 
                                  Unknown28 = new object[] { }, 
                                  Unknown29 = 0x00000003
                              };

            client.SendCompressed(message);
        }

        #endregion
    }
}