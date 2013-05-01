// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemText.cs" company="CellAO Team">
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
//   Defines the SystemText type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZoneEngine.Packets
{
    using SmokeLounge.AOtomation.Messaging.GameData;

    public static class SystemText
    {
        #region Public Methods and Operators

        public static void Send(Client client, string text, int color)
        {
            var message = new SmokeLounge.AOtomation.Messaging.Messages.N3Messages.FormatFeedbackMessage
                              {
                                  Identity =
                                      new Identity
                                          {
                                              Type
                                                  =
                                                  IdentityType
                                                  .CanbeAffected, 
                                              Instance
                                                  =
                                                  client
                                                  .Character
                                                  .Id
                                          }, 
                                  Unknown =
                                      0x01, 
                                  Unknown1 =
                                      0x00000000, 
                                  Unknown2 =
                                      0x00557E26, 
                                  Unknown3 =
                                      0x21212122, 
                                  Unknown4 =
                                      0x3a212121, 
                                  Unknown5 =
                                      0x293C, 
                                  Unknown6 =
                                      0x73, 
                                  Message =
                                      text, 
                                  Unknown7 =
                                      0x00000000
                              };

            client.SendCompressed(message);
        }

        #endregion
    }
}