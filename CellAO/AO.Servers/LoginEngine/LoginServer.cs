// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginServer.cs" company="CellAO Team">
//   Copyright © 2005-2013 CellAO Team.
//   
//   All rights reserved.
//   
//   Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//   
//       * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//       * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//       * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//   
//   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//   A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//   CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//   EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//   PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//   LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//   NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//   SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// </copyright>
// <summary>
//   Defines the LoginServer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoginEngine
{
    using System.ComponentModel.Composition;
    using System.Globalization;
    using System.Net;
    using System.Threading;

    using Cell.Core;

    using LoginEngine.Component;

    /// <summary>
    /// </summary>
    [Export]
    public class LoginServer : ServerBase
    {
        #region Fields

        private readonly ClientFactory clientFactory;

        #endregion

        #region Constructors and Destructors

        [ImportingConstructor]
        public LoginServer(ClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <summary>
        /// </summary>
        public override void Start()
        {
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            base.Start();
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <returns>
        ///     The <see cref="ClientBase" />.
        /// </returns>
        protected override ClientBase CreateClient()
        {
            return this.clientFactory.Create(this);
        }

        /// <summary>
        /// </summary>
        /// <param name="client">
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected override bool OnClientConnected(ClientBase client)
        {
            return true;
        }

        protected override void OnReceiveUDP(int num_bytes, byte[] buf, IPEndPoint ip)
        {
        }

        protected override void OnSendTo(IPEndPoint clientIP, int num_bytes)
        {
        }

        #endregion
    }
}