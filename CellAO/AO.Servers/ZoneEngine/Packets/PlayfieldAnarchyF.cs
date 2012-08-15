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
            PacketWriter writer = new PacketWriter();

            writer.PushBytes(new byte[] { 0xDF, 0xDF });
            writer.PushShort(10);
            writer.PushShort(1);
            writer.PushShort(0);
            writer.PushInt(3086);
            writer.PushInt(client.Character.ID);
            writer.PushInt(0x5F4B1A39);
            writer.PushIdentity(40016, client.Character.PlayField);
            writer.PushByte(0);

            writer.PushInt(4);
            writer.PushCoord(client.Character.Coordinates);
            writer.PushByte(97);
            writer.PushIdentity(51100, client.Character.PlayField);
            writer.PushInt(0);
            writer.PushInt(0);
            writer.PushIdentity(40016, client.Character.PlayField);
            writer.PushInt(0);
            writer.PushInt(0);

            int vendorcount = VendorHandler.GetNumberofVendorsinPlayfield(client.Character.PlayField);
            if (vendorcount > 0)
            {
                writer.PushInt(51035);
                writer.PushInt(1);
                writer.PushInt(1);
                writer.PushInt(vendorcount);
                writer.PushInt(VendorHandler.GetFirstVendor(client.Character.PlayField));
            }
            // TODO: Use correct World Position for each "outdoors" playfield -Suiv-
            // Playfield WorldPos X
            writer.PushInt(Playfields.GetPlayfieldX(client.Character.PlayField));
            // Playfield WorldPos Z
            writer.PushInt(Playfields.GetPlayfieldZ(client.Character.PlayField));

            byte[] reply = writer.Finish();
            client.SendCompressed(reply);
        }
    }
}