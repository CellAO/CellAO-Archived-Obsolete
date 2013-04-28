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
    using System.Collections.ObjectModel;

    using Cell.Core;

    /// <summary>
    /// The client.
    /// </summary>
    public class Client : ClientBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class. 
        /// The client.
        /// </summary>
        /// <param name="srvr">
        /// </param>
        public Client(ChatServer srvr)
            : base(srvr)
        {
            this.Character = new Character(0, null);
            this.ServerSalt = string.Empty;
            this.knownClients = new Collection<uint>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class. 
        /// The client.
        /// </summary>
        public Client()
            : base(null)
        {
        }
        #endregion

        #region Needed overrides
        /// <summary>
        /// The on receive.
        /// </summary>
        /// <param name="numBytes">
        /// </param>
        protected override bool OnReceive(BufferSegment segment)
        {
            if (segment.Length>=4)
            {
                byte[] packet = new byte[segment.Length];
                Array.Copy(segment.SegmentData, 0, packet, 0, segment.Length);
                ushort messageNumber = this.GetMessageNumber(packet);
                Parser m_parser = new Parser();
                m_parser.Parse(this, packet, messageNumber);
                return true;
            }
            return false;
        }
        #endregion

        #region Misc overrides

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="packet">
        /// </param>
        public void Send(byte[] packet)
        {
            BufferSegment toSend = BufferManager.GetSegment(packet.Length);
            toSend.CopyFrom(packet, 0);
            base.Send(toSend);
        }

        /// <summary>
        /// The cleanup.
        /// </summary>
        public void Cleanup()
        {
            base.Dispose(true);
        }
        #endregion

        #region Our own stuff
        /// <summary>
        /// Private known clients collection
        /// </summary>
        private readonly Collection<uint> knownClients;

        /// <summary>
        /// The known clients.
        /// </summary>
        public Collection<uint> KnownClients
        {
            get
            {
                return this.knownClients;
            }
        }

        /// <summary>
        /// The character.
        /// </summary>
        public Character Character { get; set; }

        /// <summary>
        /// The server salt.
        /// </summary>
        public string ServerSalt { get; set; }

        // NV: Should this be here or inside Character...

        /// <summary>
        /// The get message number.
        /// </summary>
        /// <param name="packet">
        /// </param>
        /// <returns>
        /// The get message number.
        /// </returns>
        protected ushort GetMessageNumber(byte[] packet)
        {
            ushort reply = BitConverter.ToUInt16(new[] { packet[1], packet[0] }, 0);
            return reply;
        }
        #endregion
    }
}