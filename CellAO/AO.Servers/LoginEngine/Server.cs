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

namespace LoginEngine
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Threading;

    using Cell.Core;

    /// <summary>
    /// 
    /// </summary>
    public class Server : ServerBase
    {
        #region Needed overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="num_bytes"></param>
        /// <param name="buf"></param>
        /// <param name="ip"></param>
        protected override void OnReceiveUDP(int num_bytes, byte[] buf, IPEndPoint ip)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientIP"></param>
        /// <param name="num_bytes"></param>
        protected override void OnSendTo(IPEndPoint clientIP, int num_bytes)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override ClientBase CreateClient()
        {
            return new Client(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        public override void Warning(ClientBase client, Exception e)
        {
            base.Warning(client, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="msg"></param>
        /// <param name="parms"></param>
        public override void Warning(ClientBase client, string msg, params object[] parms)
        {
            base.Warning(client, msg, parms);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="msg"></param>
        /// <param name="parms"></param>
        public override void Info(ClientBase client, string msg, params object[] parms)
        {
            base.Info(client, msg, parms);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="msg"></param>
        /// <param name="parms"></param>
        public override void Debug(ClientBase client, string msg, params object[] parms)
        {
            base.Debug(client, msg, parms);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        protected override bool OnClientConnected(ClientBase client)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="forced"></param>
        protected override void OnClientDisconnected(ClientBase client, bool forced)
        {
            base.OnClientDisconnected(client, forced);
        }
        #endregion

        #region Misc overrides
        /// <summary>
        /// 
        /// </summary>
        public override void Start()
        {
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            base.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Stop()
        {
            base.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        public override int MaximumPendingConnections
        {
            get
            {
                return base.MaximumPendingConnections;
            }
            set
            {
                base.MaximumPendingConnections = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override IPAddress TcpIP
        {
            get
            {
                return base.TcpIP;
            }
            set
            {
                base.TcpIP = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override int TcpPort
        {
            get
            {
                return base.TcpPort;
            }
            set
            {
                base.TcpPort = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override IPAddress UdpIP
        {
            get
            {
                return base.UdpIP;
            }
            set
            {
                base.UdpIP = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override int UdpPort
        {
            get
            {
                return base.UdpPort;
            }
            set
            {
                base.UdpPort = value;
            }
        }
        #endregion
    }
}