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
using System;
using System.IO;
using System.Text;
#endregion

namespace ZoneEngine.PacketHandlers
{
    /// <summary>
    /// 
    /// </summary>
    public class SystemMessage
    {
        #region Constructors
        #endregion

        #region Functions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="packet"></param>
        /// <param name="messageNumber"></param>
        public void Parse(Client client, ref byte[] packet, int messageNumber)
        {
            switch (messageNumber)
            {
                case 27:
                    // client connected, lets send first message (not compressed)
                    {
                        Byte[] connected = new byte[]
                                               {
                                                   0xDF, 0xDF,
                                                   0x7F, 0x00,
                                                   0x00, 0x01,
                                                   0x00, 0x10,
                                                   0x03, 0x00, 0x00, 0x00,
                                                   0x00, 0x00, 0x00, 0x00
                                               };
                        client.Send(ref connected);

                        // and off we go to ClientConnected
                        ClientConnected tmp_ClientConnected = new ClientConnected();
                        tmp_ClientConnected.Read(ref packet, client);
                    }
                    break;
                default:
                    client.Server.Warning(client, "Client sent unknown SystemMessage {0:x8}", messageNumber.ToString());
                    TextWriter tw = new StreamWriter("System Message Debug output.txt", true,
                                                     Encoding.GetEncoding("windows-1252"));
                    tw.WriteLine("System Message " + messageNumber.ToString());
                    string line = "";
                    string asc = "";
                    foreach (byte b in packet)
                    {
                        line = line + b.ToString("X2") + " ";
                        if ((b >= 32) && (b <= 127))
                        {
                            asc = asc + (Char) b;
                        }
                        else
                        {
                            asc = asc + ".";
                        }
                        if (asc.Length == 16)
                        {
                            tw.WriteLine(line + asc);
                            line = "";
                            asc = "";
                        }
                    }
                    if (line != "")
                    {
                        tw.WriteLine(line.PadRight(16*3), asc);
                    }
                    tw.Close();
                    break;
            }
        }
        #endregion
    }
}