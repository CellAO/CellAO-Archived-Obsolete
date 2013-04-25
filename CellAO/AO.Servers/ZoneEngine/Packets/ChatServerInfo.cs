// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatServerInfo.cs" company="CellAO Team">
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
//   Chat server info packet writer
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZoneEngine.Packets
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    using AO.Core.Config;

    using SmokeLounge.AOtomation.Messaging.Messages.SystemMessages;

    /// <summary>
    ///     Chat server info packet writer
    /// </summary>
    public static class ChatServerInfo
    {
        #region Public Methods and Operators

        /// <summary>
        /// Sends chat server info to client
        /// </summary>
        /// <param name="client">
        /// Client that gets the info
        /// </param>
        public static void Send(Client client)
        {
            /* get chat settings from config */
            var chatServerIp = string.Empty;
            IPAddress tempIp;
            if (IPAddress.TryParse(ConfigReadWrite.Instance.CurrentConfig.ChatIP, out tempIp))
            {
                chatServerIp = ConfigReadWrite.Instance.CurrentConfig.ChatIP;
            }
            else
            {
                var chatHost = Dns.GetHostEntry(ConfigReadWrite.Instance.CurrentConfig.ChatIP);
                foreach (var ip in chatHost.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        chatServerIp = ip.ToString();
                        break;
                    }
                }
            }

            var chatPort = Convert.ToInt32(ConfigReadWrite.Instance.CurrentConfig.ChatPort);

            var chatServerInfoMessage = new ChatServerInfoMessage { HostName = chatServerIp, Port = chatPort };
            client.SendCompressed(0x00000C0E, client.Character.Id, chatServerInfoMessage);
        }

        #endregion
    }
}