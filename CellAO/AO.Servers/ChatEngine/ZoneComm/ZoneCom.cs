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

namespace ChatEngine
{
    using System;

    /// <summary>
    /// Gateway to the Client, because of staticness.
    /// will contain funcs for doing stuff
    /// </summary>
    public static class ZoneCom
    {
        /// <summary>
        /// Link this client to the server Engine
        /// </summary>
        /// <param name="ip">
        /// The ip.
        /// </param>
        /// <param name="port">
        /// The port.
        /// </param>
        /// <param name="srv">
        /// The srv.
        /// </param>
        /// <returns>
        /// The link.
        /// </returns>
        public static bool Link(string ip, int port, Server srv)
        {
            try
            {
                Client = new IscClient(ip, port, srv);
            }
            catch (Exception e)
            {
                Console.WriteLine("[ISComm] Failure: " + e);
                return false;
            }

            return true;
        }

        /// <summary>
        /// The client.
        /// </summary>
        public static IscClient Client { get; set; }
    }
}