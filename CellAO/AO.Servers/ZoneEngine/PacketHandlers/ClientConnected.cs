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

namespace ZoneEngine.PacketHandlers
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    using ZoneEngine.NonPlayerCharacter;
    using ZoneEngine.Packets;

    /// <summary>
    /// 
    /// </summary>
    public class ClientConnected
    {
        //public byte[] GameTime = BitConverter.GetBytes(Convert.ToInt64(ConfigReadWrite.Instance.CurrentConfig.GameTime));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] StrToByteArray(string str)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetBytes(str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="client"></param>
        public void Read(byte[] packet, Client client)
        {
            // Don't edit anything in this region
            // unless you are 300% sure you know what you're doing

            #region Do not edit
            MemoryStream memoryStream = new MemoryStream(packet);
            BinaryReader binaryReader = new BinaryReader(memoryStream);
            memoryStream.Position = 20;
            // we get character ID of a client and store
            // it in this ClientBase so we can use it later
            int charID = IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
            binaryReader.Close();
            memoryStream.Dispose();

            client.Character = new Character(charID, 0);
            client.Character.Client = client;
            client.Character.ReadNames();

            client.Server.Info(
                client,
                "Client connected. ID: {0} IP: {1}",
                client.Character.Id,
                client.TcpIP + " Character name: " + client.Character.Name);

            // now we have to start sending packets like 
            // character stats, inventory, playfield info
            // and so on. I will put some packets here just 
            // to get us in game. We have to start moving
            // these packets somewhere else and make packet 
            // builders instead of sending (half) hardcoded
            // packets.

            // lets get char ID as byte array
            byte[] chrID = new[] { packet[20], packet[21], packet[22], packet[23] };

            /* send chat server info to client */
            ChatServerInfo.Send(client);

            /* send playfield info to client */
            PlayfieldAnarchyF.Send(client);

            /* set SocialStatus to 0 */

            client.Character.Stats.SetBaseValue(521, 0);
            Stat.Send(client, 521, 0, false);

            /* Action 167 Animation and Stance Data maybe? */
            byte[] tempBytes = new byte[]
                {
                    0xDF, 0xDF, 0x00, 0x0A, 0x00, 0x01, 0x00, 0x37, 0x00, 0x00, 0x0c, 0x0e, chrID[0], chrID[1], chrID[2],
                    chrID[3], 0x5E, 0x47, 0x77, 0x70, // CharacterAction
                    0x00, 0x00, 0xC3, 0x50, chrID[0], chrID[1], chrID[2], chrID[3], 0x00, 0x00, 0x00, 0x00, 0xA7, // 167
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                    , 0x00, 0x00, 0x01, 0x00, 0x00
                };
            client.SendCompressed(tempBytes);

            tempBytes = new byte[]
                {
                    // current in game time

                    0xDF, 0xDF, 0x00, 0x0A, 0x00, 0x01, 0x00, 0x2D, 0x00, 0x00, 0x0c, 0x0e, chrID[0], chrID[1], chrID[2],
                    chrID[3], 0x5F, 0x52, 0x41, 0x2E, // GameTime
                    0x00, 0x00, 0xC3, 0x50, chrID[0], chrID[1], chrID[2], chrID[3], 0x01, 0x46, 0xEA, 0x90, 0x00, // 30024.0
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0xD4, 0x40, // 185408
                    0x47, 0x9C, 0x9B, 0xA8 // 80183.3125
                };
            client.SendCompressed(tempBytes);

            /* set SocialStatus to 0 */
            Stat.Set(client, 521, 0, false);

            /* again */
            Stat.Set(client, 521, 0, false);

            /* visual */

            SimpleCharFullUpdate.SendToPlayfield(client);

            /* inventory, items and all that */
            FullCharacter.Send(client);

            tempBytes = new byte[]
                {
                    // this packet gives you (or anyone else)
                    // special attacks like brawl, fling shot and so

                    0xDF, 0xDF, 0x00, 0x0A, 0x00, 0x01, 0x00, 0x65, 0x00, 0x00, 0x0c, 0x0e, chrID[0], chrID[1], chrID[2],
                    chrID[3], 0x1D, 0x3C, 0x0F, 0x1C, // SpecialAttackWeapon
                    0x00, 0x00, 0xC3, 0x50, chrID[0], chrID[1], chrID[2], chrID[3], 0x01, 0x00, 0x00, 0x0F, 0xC4,
                    // (4036/1009)-1 = 3 special attacks
                    0x00, 0x00, 0xAA, 0xC0, // 43712
                    0x00, 0x02, 0x35, 0x69, // 144745
                    0x00, 0x00, 0x00, 0x64, // 100
                    0x4D, 0x41, 0x41, 0x54, // "MAAT"
                    0x00, 0x00, 0xA4, 0x31, // 42033
                    0x00, 0x00, 0xA4, 0x30, // 42032
                    0x00, 0x00, 0x00, 0x90, // 144
                    0x44, 0x49, 0x49, 0x54, // "DIIT"
                    0x00, 0x01, 0x12, 0x94, // 70292
                    0x00, 0x01, 0x12, 0x95, // 70293
                    0x00, 0x00, 0x00, 0x8E, // 142
                    0x42, 0x52, 0x41, 0x57, // "BRAW"
                    0x00, 0x00, 0x00, 0x07, // 7
                    0x00, 0x00, 0x00, 0x07, // 7
                    0x00, 0x00, 0x00, 0x07, // 7
                    0x00, 0x00, 0x00, 0x0E, // 14
                    0x00, 0x00, 0x00, 0x64 // 100
                };

            client.SendCompressed(tempBytes);
            // done
            #endregion

            // Timers are allowed to update client stats now.
            client.Character.DoNotDoTimers = false;

            // spawn all active monsters to client
            NonPlayerCharacterHandler.SpawnMonstersInPlayfieldToClient(client, client.Character.PlayField);

            if (VendorHandler.GetNumberofVendorsinPlayfield(client.Character.PlayField) > 0)
            {
                /* Shops */
                VendorHandler.GetVendorsInPF(client);
            }

            // WeaponItemFullCharUpdate  Maybe the right location , First Check if weapons present usually in equipment
            //Packets.WeaponItemFullUpdate.Send(client, client.Character);

            client.Character.ProcessTimers(DateTime.Now + TimeSpan.FromMilliseconds(200));

            client.Character.CalculateSkills();

            AppearanceUpdate.AnnounceAppearanceUpdate(client.Character);

            // done, so we call a hook.
            // Call all OnConnect script Methods
            Program.csc.CallMethod("OnConnect", client.Character);
        }
    }
}