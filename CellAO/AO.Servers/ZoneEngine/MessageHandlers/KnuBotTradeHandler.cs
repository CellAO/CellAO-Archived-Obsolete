// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnuBotTradeHandler.cs" company="CellAO Team">
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
//   Defines the KnuBotTradeHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZoneEngine.MessageHandlers
{
    using System.ComponentModel.Composition;

    using AO.Core.Components;

    using SmokeLounge.AOtomation.Messaging.Messages;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    using ZoneEngine.Misc;

    [Export(typeof(IHandleMessage))]
    public class KnuBotTradeHandler : IHandleMessage<KnuBotTradeMessage>
    {
        #region Public Methods and Operators

        public void Handle(object sender, Message message)
        {
            var client = (Client)sender;
            var knuBotTradeMessage = (KnuBotTradeMessage)message.Body;

            var target = knuBotTradeMessage.Target;
            var npc = (NonPlayerCharacterClass)FindDynel.FindDynelById((int)target.Type, target.Instance);
            var character = FindClient.FindClientById(message.Header.Sender).Character;

            var ie = character.GetInventoryAt(knuBotTradeMessage.Container.Instance);
            var aoi = ie.Item.ShallowCopy();
            character.Inventory.Remove(ie); // Silent remove, no message given to client. Client handles this on its own
            npc.KnuBotTrade(character, aoi);
        }

        #endregion
    }
}