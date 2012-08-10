#region License
/*
Copyright (c) 2005-2011, CellAO Team

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
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace LoginEngine.Packets
{
    /// <summary>
    /// 
    /// </summary>
    public class SaltAuthentication
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="ServerSalt"></param>
        public void SendPacket(Client client, out string ServerSalt)
        {
            byte[] Packet = new byte[]
            {
                0xDF, 0xDF,
                0x00, 0x01,
                0x00, 0x01,
                0x00, 0x34, // Total Length of Packet 
                0x00, 0x00, 0x00, 0x01,
                0x00, 0x00, 0x2B, 0x3F,
                0x00, 0x00, 0x00, 0x24, // Packet Type (SaltAuthentication)
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Server Salt (32 Bytes)
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };

            byte[] Salt = new byte[0x20];
            Random rand = new Random();

            rand.NextBytes(Salt);

            ServerSalt = string.Empty;

            for (int i = 0; i < 32; i++)
            {
                if (Salt[i] == 0) // 0x00 Breaks Things
                    Salt[i] = 42; // So we change it to something nicer
                Packet[20 + i] = Salt[i];

                ServerSalt += String.Format("{0:x2}", Salt[i]);
            }

            client.Send(ref Packet);
        }
    }
}
