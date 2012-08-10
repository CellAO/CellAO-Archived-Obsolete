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
using AO.Core;
using ZoneEngine.Packets;
#endregion

namespace ZoneEngine.PacketHandlers
{
    /// <summary>
    /// 
    /// </summary>
    internal static class Dynels
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        public static void GetDynels(Client client)
        {
            foreach (Client m_client in client.Server.Clients)
            {
                PacketWriter _writer;
                if ((m_client.Character.PlayField == client.Character.PlayField) &&
                    (m_client.Character.ID != client.Character.ID))
                {
                    SimpleCharFullUpdate.SendToOne(m_client.Character, client);

                    _writer = new PacketWriter();
                    _writer.PushByte(0xDF);
                    _writer.PushByte(0xDF);
                    _writer.PushShort(10);
                    _writer.PushShort(1);
                    _writer.PushShort(0);
                    _writer.PushInt(3086);
                    _writer.PushInt(client.Character.ID);
                    _writer.PushInt(0x570C2039);
                    _writer.PushIdentity(50000, m_client.Character.ID);
                    _writer.PushByte(0);
                    byte[] reply2 = _writer.Finish();
                    client.SendCompressed(reply2);
                }
            }
        }
    }
}