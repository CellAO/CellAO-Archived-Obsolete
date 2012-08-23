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
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The channels.
    /// </summary>
    public static class ChatChannels
    {
        /// <summary>
        /// The channel names.
        /// </summary>
        public static readonly List<ChannelsEntry> ChannelNames = new List<ChannelsEntry>
            {
                new ChannelsEntry("Global", 0x0400002328, 0x8044),
                new ChannelsEntry("CellAO News", 0x0c000007d0, 0x8044)
                
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

            byte[] temp = new byte[8];
            Array.Copy(packet, 4, temp, 0, 5);

            ulong chanid = BitConverter.ToUInt64(temp, 0);

            foreach (ChannelsEntry ce in ChannelNames)
            {

                if (ce.Id != chanid)
                {
                    continue;
                }
                return ce;
            }

            return null;
        }

        public static void CreateTeamChannel(int teamID, uint[] teamMemberIds)
        {
            ChannelsEntry channelsEntry = new ChannelsEntry();

        }
    }
    public enum ChannelType : ulong
    {
        Unknown = 0,
        Admin = 1,
        Team = 2 | 0x80,
        Organization = 3,
        Leaders = 4,
        GM = 5,
        Shopping = 6 | 0x80,
        General = 7 | 0x80,
        Towers = 10,
        Announcements = 12,
        Raid = 15 | 0x80,
        Battlestation = 16 | 0x80
    }

    public enum ChannelFlags : uint
    {
        None = 0,
        CantIgnore = 0x1,
        CantSend = 0x2,
        NoInternational = 0x10,
        NoVoice = 0x20,
        SendCriteria = 0x40,
        GroupOnName = 0x80,
        Muted = 0x1000000,
    }
}