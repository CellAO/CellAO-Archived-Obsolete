// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatCommandHandler.cs" company="CellAO Team">
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
//   Defines the ChatCommandHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZoneEngine.PacketHandlers
{
    using AO.Core;

    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    public static class ChatCommandHandler
    {
        // TODO: Move this to character class
        #region Public Methods and Operators

        public static bool HasNano(int nanoId, Client client)
        {
            var found = false;
            foreach (var uploadedNanos in client.Character.UploadedNanos)
            {
                if (uploadedNanos.Nano != nanoId)
                {
                    continue;
                }

                found = true;
                break;
            }

            return found;
        }

        public static bool ItemExists(int placement, Client client)
        {
            return client.Character.GetInventoryAt(placement) != null;
        }

        public static void Read(ChatCmdMessage message, Client client)
        {
            var fullArgs = message.Command.TrimEnd(char.MinValue);
            var temp = string.Empty;
            do
            {
                temp = fullArgs;
                fullArgs = fullArgs.Replace("  ", " ");
            }
            while (temp != fullArgs);

            var cmdArgs = fullArgs.Trim().Split(' ');

            Program.csc.CallChatCommand(cmdArgs[0].ToLower(), client, message.Target, cmdArgs);
        }

        #endregion
    }
}