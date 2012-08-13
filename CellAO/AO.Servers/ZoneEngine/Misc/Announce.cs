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

#region Usings...

#endregion

namespace ZoneEngine.Misc
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class Announce
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data"></param>
        public static void Playfield(int playfield, ref byte[] data)
        {
            lock (Program.zoneServer.Clients)
            {
                for (int i = Program.zoneServer.Clients.Count - 1; i >= 0; i--)
                {
                    Client mClient = (Client)Program.zoneServer.Clients[i];

                    if (mClient.Character.PlayField != playfield)
                    {
                        continue;
                    }
                    byte[] mID = BitConverter.GetBytes(mClient.Character.ID);
                    data[12] = mID[3];
                    data[13] = mID[2];
                    data[14] = mID[1];
                    data[15] = mID[0];
                    mClient.SendCompressed(data);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data"></param>
        public static void PlayfieldOthers(Client client, ref Byte[] data)
        {
            foreach (Client mClient in Program.zoneServer.Clients)
            {
                if ((mClient.Character.PlayField != client.Character.PlayField)
                    || (mClient.Character.ID == client.Character.ID))
                {
                    continue;
                }
                Byte[] mID = BitConverter.GetBytes(mClient.Character.ID);
                Array.Reverse(mID);
                data[12] = mID[0];
                data[13] = mID[1];
                data[14] = mID[2];
                data[15] = mID[3];
                mClient.SendCompressed(data);
            }
        }

        /// <summary>
        /// NPC's send their stats to playfield too
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data"></param>
        public static void PlayfieldOthers(int playfield, ref Byte[] data)
        {
            foreach (Client mClient in Program.zoneServer.Clients)
            {
                if ((mClient.Character.PlayField != playfield))
                {
                    continue;
                }
                Byte[] mID = BitConverter.GetBytes(mClient.Character.ID);
                Array.Reverse(mID);
                data[12] = mID[0];
                data[13] = mID[1];
                data[14] = mID[2];
                data[15] = mID[3];
                mClient.SendCompressed(data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data"></param>
        public static void Global(Client client, byte[] data)
        {
            foreach (Client mClient in Program.zoneServer.Clients)
            {
                byte[] mID = BitConverter.GetBytes(mClient.Character.ID);
                Array.Reverse(mID);
                data[12] = mID[0];
                data[13] = mID[1];
                data[14] = mID[2];
                data[15] = mID[3];
                mClient.SendCompressed(data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data"></param>
        public static void GlobalOthers(Client client, byte[] data)
        {
            foreach (Client mClient in Program.zoneServer.Clients)
            {
                if (mClient.Character.ID == client.Character.ID)
                {
                    continue;
                }
                byte[] mID = BitConverter.GetBytes(mClient.Character.ID);
                Array.Reverse(mID);
                data[12] = mID[0];
                data[13] = mID[1];
                data[14] = mID[2];
                data[15] = mID[3];
                mClient.SendCompressed(data);
            }
        }

        /* Moved this here so I can delete ChatText.cs */

        /// <summary>
        /// Broadcasts chat message to ALL users on the server
        /// </summary>
        /// <param name="b">Base Client to extract the ServerBase from. Like, who designed this :P</param>
        /// <param name="msg">...</param>
        public static void Broadcast(Client b, string msg)
        {
            foreach (Client c in b.Server.Clients)
            {
                c.SendChatText("System Message: " + msg);
            }
        }
    }
}