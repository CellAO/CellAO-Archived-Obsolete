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
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public static class FindClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Client FindClientByID(int id)
        {
            foreach (Client client in Program.zoneServer.Clients)
            {
                if (client.Character.ID != id)
                {
                    continue;
                }
                return client;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static bool FindClientByID(int id, out Client client)
        {
            foreach (Client client1 in Program.zoneServer.Clients)
            {
                if (client1.Character.ID != id)
                {
                    continue;
                }
                client = client1;
                return true;
            }
            client = null;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Client FindClientByName(string name)
        {
            foreach (Client client in Program.zoneServer.Clients)
            {
                if (client.Character.Name != name)
                {
                    continue;
                }
                return client;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static bool FindClientByName(string name, out Client client)
        {
            foreach (Client client1 in Program.zoneServer.Clients)
            {
                if (client1.Character.Name.ToLower() != name.ToLower())
                {
                    continue;
                }
                client = client1;
                return true;
            }
            client = null;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pf"></param>
        /// <returns></returns>
        public static List<Client> FindClientsByPlayfield(int pf)
        {
            List<Client> clientList = new List<Client>();
            foreach (Client client in Program.zoneServer.Clients)
            {
                if (client.Character.PlayField != pf)
                {
                    continue;
                }
                clientList.Add(client);
            }
            return clientList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pf"></param>
        /// <param name="clients"></param>
        /// <returns></returns>
        public static bool FindClientsByPlayfield(int pf, out List<Client> clients)
        {
            List<Client> clientList = new List<Client>();
            foreach (Client client in Program.zoneServer.Clients)
            {
                if (client.Character.PlayField != pf)
                {
                    continue;
                }
                clientList.Add(client);
            }
            clients = clientList;
            return clientList.Count >= 1;
        }

        /// <summary>
        /// Experimental - Check who is in the given radius
        /// </summary>
        /// <param name="client"></param>
        /// <param name="radius">RAdius</param>
        /// <returns></returns>
        public static List<Client> GetClientsInRadius(Client client, float radius)
        {
            List<Client> clientList = new List<Client>();
            //we're added ourselves, because we're included in the list too.
            //Clients.Add(this);

            // searching client as first one!
            clientList.Add(client);

            foreach (Client client1 in Program.zoneServer.Clients)
            {
                /*float DeltaX = cli.Character.coord.x - Character.coord.x;
                float DeltaY = cli.Character.coord.y - Character.coord.y;

                float DeltaRadius = (float)Math.Sqrt((double)((DeltaX * DeltaX) + (DeltaY + DeltaY)));

                if (DeltaRadius < Radius)
                {
                    Clients.Add(cli);
                }*/
                if (client1.Character.PlayField != client.Character.PlayField)
                {
                    continue;
                }
                if (client1.Character.ID == client.Character.ID)
                {
                    continue;
                }
                if (client1.Character.Coordinates.Distance2D(client.Character.Coordinates) >= radius)
                {
                    continue;
                }
                clientList.Add(client1);
            }
            return clientList;
        }

        /// <summary>
        /// Experimental - Check who is in the given radius
        /// </summary>
        /// <param name="client">Character or NPC to start from</param>
        /// <param name="radius">RAdius</param>
        /// <returns></returns>
        public static List<Dynel> GetClientsInRadius(Dynel client, float radius)
        {
            // we're added ourselves, because we're included in the list too.
            List<Dynel> clientList = new List<Dynel> { client };

            // searching client as first one!

            foreach (Client client1 in Program.zoneServer.Clients)
            {
                if (client1.Character.PlayField != client.PlayField)
                {
                    continue;
                }
                if (client1.Character.ID == client.ID)
                {
                    continue;
                }
                if (client1.Character.Coordinates.Distance2D(client.Coordinates) >= radius)
                {
                    continue;
                }
                clientList.Add(client1.Character);
            }
            return clientList;
        }
    }
}