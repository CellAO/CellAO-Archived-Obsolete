#region License
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnubotStartTrade.cs" company="CellAO Team">
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
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace ZoneEngine.PacketHandlers
{
    #region Usings ...

    using SmokeLounge.AOtomation.Messaging.GameData;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    using ZoneEngine.Misc;

    #endregion

    /// <summary>
    /// </summary>
    public static class KnuBotStartTrade
    {
        #region Public Methods and Operators

        /// <summary>
        /// Reads the KnuBot Start Trade Message from the client and dispatches it to the npc for event handling.
        /// </summary>
        /// <param name="message">
        /// Deserialized Message
        /// </param>
        /// <param name="client">
        /// Client who was asking for trade
        /// </param>
        public static void Read(KnuBotStartTradeMessage message, Client client)
        {
            var npc =
                (NonPlayerCharacterClass)FindDynel.FindDynelById((int)message.Target.Type, message.Target.Instance);
            if (npc != null)
            {
                npc.KnuBotStartTrade(client.Character);
            }
        }

        /// <summary>
        /// Sends back a reply to the KnuBot Start Trade Message
        /// </summary>
        /// <param name="client">
        /// Client who started the trade
        /// </param>
        /// <param name="knubotTarget">
        /// NPC who answers on the trade message
        /// </param>
        /// <param name="message">
        /// Textmessage to display in the chat window
        /// </param>
        /// <param name="numberOfItemSlots">
        /// Number of item slots displayed (1-8?)
        /// </param>
        public static void Send(Client client, NonPlayerCharacterClass knubotTarget, string message, int numberOfItemSlots)
        {
            var knuBotStartTradeMessage = new KnuBotStartTradeMessage
                                              {
                                                  Identity =
                                                  {
                                                      Type = IdentityType.CanbeAffected,
                                                      Instance = client.Character.Id
                                                  },
                                                  Target =
                                                      {
                                                          Type = IdentityType.CanbeAffected,
                                                          Instance = knubotTarget.Id
                                                      },
                                                  NumberOfItemSlotsInTradeWindow = numberOfItemSlots,
                                                  Message = message,
                                                  Unknown1 = 2
                                              };
            client.SendCompressed(knuBotStartTradeMessage);
        }

        #endregion
    }
}