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

namespace ChatEngine.Lists
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// The channels.
    /// </summary>
    public static class Channels
    {
        /// <summary>
        /// The channel names.
        /// </summary>
        public static readonly Collection<ChannelsEntry> ChannelNames = new Collection<ChannelsEntry>
            {
                new ChannelsEntry("Global", new byte[] { 0x04, 0x00, 0x00, 0x23, 0x28 }, 0x8044),
                new ChannelsEntry("CellAO News", new byte[] { 0x0c, 0x00, 0x00, 0x07, 0xd0 }, 0x8044)
                
                // need to change the flags soo only GMs can broadcast to this channel..
            };

        /// <summary>
        /// Get the channel entry
        /// </summary>
        /// <param name="packet">
        /// The packet.
        /// </param>
        /// <returns>
        /// Returns ChannelsEntry or null
        /// </returns>
        public static ChannelsEntry GetChannel(byte[] packet)
        {
            foreach (ChannelsEntry ce in ChannelNames)
            {
                if (packet[4] != ce.Id[0])
                {
                    continue;
                }

                if (packet[5] != ce.Id[1])
                {
                    continue;
                }

                if (packet[6] != ce.Id[2])
                {
                    continue;
                }

                if (packet[7] != ce.Id[3])
                {
                    continue;
                }

                if (packet[8] != ce.Id[4])
                {
                    continue;
                }

                return ce;
            }

            return null;
        }
    }
}