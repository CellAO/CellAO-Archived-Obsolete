// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginServer.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the LoginServer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoginEngine
{
    using System.Globalization;
    using System.Net;
    using System.Threading;

    using AO.Core.Components;

    using Cell.Core;

    /// <summary>
    /// </summary>
    public class LoginServer : ServerBase
    {
        #region Fields

        private readonly IMessageSerializer messageSerializer;

        #endregion

        #region Constructors and Destructors

        public LoginServer()
        {
            this.messageSerializer = new MessageSerializer();
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
            return new Client(this, this.messageSerializer);
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