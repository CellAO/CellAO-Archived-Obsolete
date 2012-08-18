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

namespace ZoneEngine
{
    using System;

    using ISComm;
    using ISComm.EventArgs;

    public class ISCServer : ServerBaseClass
    {
        public void Ping()
        {
            try
            {
                byte[] Data = new byte[1];
                Data[0] = 0x00;

                this.SendMessage(0x01, Data);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: " + e.Message);
                Console.ResetColor();
            }
        }

        private void ISCServer_OnConnect(object s, EventArgs a)
        {
            Console.WriteLine("[ISComm] Link established!");
        }

        private void ISCServer_OnMessage(object s, OnMessageArgs e)
        {
            if (e.ID == 99)
            {
                int charid = BitConverter.ToInt32(e.Data, 0);
                foreach (Client c in Program.zoneServer.Clients)
                {
                    if (c.Character.Id == charid)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Unauthorized access detected: \r\nCharacterID: " + charid);
                        Console.WriteLine("From IP: " + c.TcpIP);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Program.zoneServer.DisconnectClient(c);
                        break;
                    }
                }
            }
        }

        private void ISCServer_OnDisconnect(object s, EventArgs a)
            //Andyzweb: I added a \n to this write line so it appeared on a seprate line from the prompt
            //It just looks nice.
        {
            Console.WriteLine("\n[ISComm] Link lost with ChatEngine");
            Console.Write("\nServer Command >>");
        }

        public ISCServer(int Port)
            : base(Port)
        {
            this.OnMessage += this.ISCServer_OnMessage;
            this.OnDisconnect += this.ISCServer_OnDisconnect;
            this.OnConnect += this.ISCServer_OnConnect;
        }
    }
}