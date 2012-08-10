#region License
/*
Copyright (c) 2005-2012, CellAO Team

All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion

#region Usings...
using System.Collections.Generic;
#endregion

namespace ZoneEngine.Misc
{
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
            foreach (Client mClient in Program.zoneServer.Clients)
            {
                if (mClient.Character.ID != id) continue;
                return mClient;
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
            foreach (Client mClient in Program.zoneServer.Clients)
            {
                if (mClient.Character.ID != id) continue;
                client = mClient;
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
            foreach (Client mClient in Program.zoneServer.Clients)
            {
                if (mClient.Character.Name != name) continue;
                return mClient;
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
            foreach (Client mClient in Program.zoneServer.Clients)
            {
                if (mClient.Character.Name.ToLower() != name.ToLower()) continue;
                client = mClient;
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
        public static List<Client> FindClientsByPF(int pf)
        {
            List<Client> mClients = new List<Client>();
            foreach (Client mClient in Program.zoneServer.Clients)
            {
                if (mClient.Character.PlayField != pf) continue;
                mClients.Add(mClient);
            }
            return mClients;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pf"></param>
        /// <param name="clients"></param>
        /// <returns></returns>
        public static bool FindClientsByPF(int pf, out List<Client> clients)
        {
            List<Client> mClients = new List<Client>();
            foreach (Client mClient in Program.zoneServer.Clients)
            {
                if (mClient.Character.PlayField != pf) continue;
                mClients.Add(mClient);
            }
            clients = mClients;
            return mClients.Count >= 1;
        }

        /// <summary>
        /// Experimental - Check who is in the given radius
        /// </summary>
        /// <param name="client"></param>
        /// <param name="radius">RAdius</param>
        /// <returns></returns>
        public static List<Client> GetClientsInRadius(Client client, float radius)
        {
            List<Client> mClients = new List<Client>();
            //we're added ourselves, because we're included in the list too.
            //Clients.Add(this);


            // searching client as first one!
            mClients.Add(client);

            foreach (Client mClient in Program.zoneServer.Clients)
            {
                /*float DeltaX = cli.Character.coord.x - Character.coord.x;
                float DeltaY = cli.Character.coord.y - Character.coord.y;

                float DeltaRadius = (float)Math.Sqrt((double)((DeltaX * DeltaX) + (DeltaY + DeltaY)));

                if (DeltaRadius < Radius)
                {
                    Clients.Add(cli);
                }*/
                if (mClient.Character.PlayField != client.Character.PlayField) continue;
                if (mClient.Character.Coordinates.distance2D(client.Character.Coordinates) >= radius) continue;
                if (mClient.Character.ID == client.Character.ID) continue;
                mClients.Add(mClient);
            }
            return mClients;
        }

        /// <summary>
        /// Experimental - Check who is in the given radius
        /// </summary>
        /// <param name="cli">Character or NPC to start from</param>
        /// <param name="radius">RAdius</param>
        /// <returns></returns>
        public static List<Dynel> GetClientsInRadius(Dynel cli, float radius)
        {
            List<Dynel> mClients = new List<Dynel>();
            //we're added ourselves, because we're included in the list too.
            //Clients.Add(this);


            // searching client as first one!
            mClients.Add(cli);

            foreach (Client mClient in Program.zoneServer.Clients)
            {
                /*float DeltaX = cli.Character.coord.x - Character.coord.x;
                float DeltaY = cli.Character.coord.y - Character.coord.y;

                float DeltaRadius = (float)Math.Sqrt((double)((DeltaX * DeltaX) + (DeltaY + DeltaY)));

                if (DeltaRadius < Radius)
                {
                    Clients.Add(cli);
                }*/
                if (mClient.Character.PlayField != cli.PlayField) continue;
                if (mClient.Character.Coordinates.distance2D(cli.Coordinates) >= radius) continue;
                if (mClient.Character.ID == cli.ID) continue;
                mClients.Add(mClient.Character);
            }
            return mClients;
        }
    }
}