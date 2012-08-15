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

namespace ZoneEngine.Packets
{
    using AO.Core;

    using ZoneEngine.Misc;
    using ZoneEngine.NonPlayerCharacter;

    /// <summary>
    /// 
    /// </summary>
    public static class PlayfieldAnarchyF
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        public static void Send(Client client)
        {
            PacketWriter packetWriter = new PacketWriter();

            packetWriter.PushBytes(new byte[] { 0xDF, 0xDF });
            packetWriter.PushShort(10);
            packetWriter.PushShort(1);
            packetWriter.PushShort(0);
            packetWriter.PushInt(3086);
            packetWriter.PushInt(client.Character.ID);
            packetWriter.PushInt(0x5F4B1A39);
            packetWriter.PushIdentity(40016, client.Character.PlayField);
            packetWriter.PushByte(0);

            packetWriter.PushInt(4);
            packetWriter.PushCoord(client.Character.Coordinates);
            packetWriter.PushByte(97);
            packetWriter.PushIdentity(51100, client.Character.PlayField);
            packetWriter.PushInt(0);
            packetWriter.PushInt(0);
            packetWriter.PushIdentity(40016, client.Character.PlayField);
            packetWriter.PushInt(0);
            packetWriter.PushInt(0);

            int vendorcount = VendorHandler.GetNumberofVendorsinPlayfield(client.Character.PlayField);
            if (vendorcount > 0)
            {
                packetWriter.PushInt(51035);
                packetWriter.PushInt(1);
                packetWriter.PushInt(1);
                packetWriter.PushInt(vendorcount);
                packetWriter.PushInt(VendorHandler.GetFirstVendor(client.Character.PlayField));
            }
            // TODO: Use correct World Position for each "outdoors" playfield -Suiv-
            // Playfield WorldPos X
            packetWriter.PushInt(Playfields.GetPlayfieldX(client.Character.PlayField));
            // Playfield WorldPos Z
            packetWriter.PushInt(Playfields.GetPlayfieldZ(client.Character.PlayField));

            byte[] packet = packetWriter.Finish();
            client.SendCompressed(packet);
        }
    }
}