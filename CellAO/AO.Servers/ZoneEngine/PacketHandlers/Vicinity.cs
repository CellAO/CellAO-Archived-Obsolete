// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vicinity.cs" company="CellAO Team">
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
//   Defines the Vicinity type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZoneEngine.PacketHandlers
{
    using System;

    using AO.Core;

    using SmokeLounge.AOtomation.Messaging.GameData;

    using ZoneEngine.Misc;

    public static class Vicinity
    {
        #region Public Methods and Operators

        public static void Read(SmokeLounge.AOtomation.Messaging.Messages.TextMessage textMessage, Client client)
        {
#if DEBUG
            Console.WriteLine("Vicinity: " + textMessage.Message.Text);
#endif
            var range = 0f;
            switch (textMessage.Message.Type)
            {
                case ChatMessageType.Say:

                    // Say
                    range = 10.0f;
                    break;
                case ChatMessageType.Whisper:

                    // Whisper
                    range = 1.5f;
                    break;
                case ChatMessageType.Shout:

                    // Shout
                    range = 60.0f;
                    break;
            }

            var clients = FindClient.GetClientsInRadius(client, range);
            var recvers = new uint[clients.Count];
            var index = 0;

            foreach (var child in clients)
            {
                recvers[index] = (uint)child.Character.Id.Instance;
                index++;
            }

            ChatCom.SendVicinity(
                (uint)client.Character.Id.Instance, (byte)textMessage.Message.Type, recvers, textMessage.Message.Text);
        }

        #endregion
    }
}