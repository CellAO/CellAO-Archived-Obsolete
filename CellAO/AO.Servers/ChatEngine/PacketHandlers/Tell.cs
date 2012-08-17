#region License
// Copyright (c) 2005-2012, CellAO Team
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//     * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace ChatEngine.PacketHandlers
{
    using ChatEngine.Packets;

    /// <summary>
    /// The tell.
    /// </summary>
    public class Tell
    {
        /// <summary>
        /// The playerId.
        /// </summary>
        private uint playerId;

        /// <summary>
        /// The message.
        /// </summary>
        private string message = string.Empty;

        /// <summary>
        /// Read and process Tell message
        /// </summary>
        /// <param name="client">
        /// Client sending
        /// </param>
        /// <param name="packet">
        /// Packet data
        /// </param>
        public void Read(Client client, byte[] packet)
        {
            PacketReader reader = new PacketReader(ref packet);

            reader.ReadUInt16();
            reader.ReadUInt16();
            this.playerId = reader.ReadUInt32();
            this.message = reader.ReadString();
            client.Server.Debug(client, "{0} >> Tell: PlayerId: {1}", client.Character.characterName, this.playerId);
            reader.Finish();
            if (client.Server.ConnectedClients.ContainsKey(this.playerId))
            {
                Client tellClient = (Client)client.Server.ConnectedClients[this.playerId];
                if (!tellClient.KnownClients.Contains(client.Character.characterId))
                {
                    byte[] pname = PlayerName.New(client, client.Character.characterId);
                    tellClient.Send(pname);
                    tellClient.KnownClients.Add(client.Character.characterId);
                }

                byte[] pgroup = new MsgPrivateGroup().Create(client.Character.characterId, this.message, string.Empty);
                tellClient.Send(pgroup);
            }
            else
            {
                byte[] sysmsg = new MsgSystem().Create("Player not online.");
                client.Send(sysmsg);
            }
        }
    }
}