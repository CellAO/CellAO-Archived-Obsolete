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
using Cell.Core;
using System.IO;
using System.Net.Sockets;
#endregion

namespace LoginEngine
{
    /// <summary>
    /// 
    /// </summary>
    public class Client : ClientBase
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srvr"></param>
        public Client(Server srvr)
            : base(srvr)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public Client()
            : base(null)
        {
        }

        #endregion

        #region Needed overrides

        /// <summary>
        /// 
        /// </summary>
        /// <param name="num_bytes"></param>
        protected override void OnReceive(int num_bytes)
        {
            byte[] packet = new byte[num_bytes];
            Array.Copy(m_readBuffer.Array, m_readBuffer.Offset, packet, 0, num_bytes);
            uint messageNumber = GetMessageNumber(packet);
            Parser myParser = new Parser();
            myParser.Parse(this, ref packet, messageNumber);
        }

        #endregion

        #region Misc overrides

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packet"></param>
        public override void Send(ref byte[] packet)
        {
            // 18.1 Fix - Dont ask why its not in network byte order like ZoneEngine packets, its too early in the morning
            byte[] pn = BitConverter.GetBytes(packetNumber++);
            packet[0] = pn[0];
            packet[1] = pn[1];

            base.Send(ref packet);
        }


        public void senddirect(ref byte[] packet)
        {
            if (m_tcpSock.Connected)
            {
                using (SocketAsyncEventArgs args = new SocketAsyncEventArgs())
                {
                    args.Completed += SendAsyncComplete2;
                    args.SetBuffer(packet, 0, packet.Length);
                    args.UserToken = this;
                    m_tcpSock.SendAsync(args);
                }
            }
        }


        private static void SendAsyncComplete2(object sender, SocketAsyncEventArgs args)
        {
            ClientBase client = args.UserToken as ClientBase;

            try
            {
            }
            catch
            {
                // Don't do anything because all errors in the 
                // network stream are handled in the receive code.
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public override void Cleanup()
        {
            base.Cleanup();
        }

        #endregion

        #region Our own stuff

        /// <summary>
        /// 
        /// </summary>
        public UInt16 packetNumber = 1;

        /// <summary>
        /// 
        /// </summary>
        public string accountName = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string clientVersion = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string serverSalt = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        protected uint GetMessageNumber(byte[] packet)
        {
            byte[] messageNumberArray = new byte[4];
            messageNumberArray[3] = packet[16];
            messageNumberArray[2] = packet[17];
            messageNumberArray[1] = packet[18];
            messageNumberArray[0] = packet[19];
            uint reply = BitConverter.ToUInt32(messageNumberArray, 0);
            return reply;
        }

        #endregion
    }
}
