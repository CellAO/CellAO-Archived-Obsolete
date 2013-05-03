// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BankOpen.cs" company="CellAO Team">
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
//   Defines the BankOpen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZoneEngine.Packets
{
    using System.Collections.Generic;

    using SmokeLounge.AOtomation.Messaging.GameData;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    public static class BankOpen
    {
        #region Public Methods and Operators

        public static void Send(Client client)
        {
            var message = new BankMessage
                              {
                                  Identity = client.Character.Id, 
                                  Unknown = 0x01, 
                                  Unknown1 = 0, 
                                  Unknown2 = 0, 
                                  Unknown3 = 0
                              };

            var bankSlots = new List<BankSlot>();
            foreach (var item in client.Character.Bank)
            {
                short flags = 0;
                if (item.isInstanced())
                {
                    flags |= 0xa0;
                }

                if (item.LowID == item.HighID)
                {
                    flags |= 2;
                }
                else
                {
                    flags |= 1;
                }

                var bankSlot = new BankSlot
                                   {
                                       ItemFlags = item.Flags, 
                                       Flags = flags, 
                                       Count = (short)item.MultipleCount, 
                                       Identity =
                                           new Identity
                                               {
                                                   Type = (IdentityType)item.Type, 
                                                   Instance = item.Instance
                                               }, 
                                       ItemLowId = item.LowID, 
                                       ItemHighId = item.HighID, 
                                       Quality = item.Quality, 
                                       Unknown = 0
                                   };
                bankSlots.Add(bankSlot);
            }

            message.BankSlots = bankSlots.ToArray();
            client.SendCompressed(message);
        }

        #endregion
    }
}