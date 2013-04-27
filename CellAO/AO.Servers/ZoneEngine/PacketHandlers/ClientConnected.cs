﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientConnected.cs" company="CellAO Team">
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
//   Defines the ClientConnected type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZoneEngine.PacketHandlers
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    using SmokeLounge.AOtomation.Messaging.GameData;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    using ZoneEngine.NonPlayerCharacter;
    using ZoneEngine.Packets;

    public class ClientConnected
    {
        #region Public Methods and Operators

        public static byte[] StrToByteArray(string str)
        {
            var encoding = new ASCIIEncoding();
            return encoding.GetBytes(str);
        }

        [Obsolete]
        public void Read(byte[] packet, Client client)
        {
            // Don't edit anything in this region
            // unless you are 300% sure you know what you're doing
            var memoryStream = new MemoryStream(packet);
            var binaryReader = new BinaryReader(memoryStream);
            memoryStream.Position = 20;

            // we get character ID of a client and store
            // it in this ClientBase so we can use it later
            var charID = IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
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
            var chrID = new[] { packet[20], packet[21], packet[22], packet[23] };

            /* send chat server info to client */
            ChatServerInfo.Send(client);

            /* send playfield info to client */
            PlayfieldAnarchyF.Send(client);

            /* set SocialStatus to 0 */
            client.Character.Stats.SetBaseValue(521, 0);
            Stat.Send(client, 521, 0, false);

            /* Action 167 Animation and Stance Data maybe? */
            var tempBytes = new byte[]
                                {
                                    0xDF, 0xDF, 0x00, 0x0A, 0x00, 0x01, 0x00, 0x37, 0x00, 0x00, 0x0c, 0x0e, chrID[0], 
                                    chrID[1], chrID[2], chrID[3], 0x5E, 0x47, 0x77, 0x70, // CharacterAction
                                    0x00, 0x00, 0xC3, 0x50, chrID[0], chrID[1], chrID[2], chrID[3], 0x00, 0x00, 0x00, 0x00
                                    , 0xA7, // 167
                                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                    0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00
                                };
            client.SendCompressed(tempBytes);

            tempBytes = new byte[]
                            {
                                // current in game time
                                0xDF, 0xDF, 0x00, 0x0A, 0x00, 0x01, 0x00, 0x2D, 0x00, 0x00, 0x0c, 0x0e, chrID[0], 
                                chrID[1], chrID[2], chrID[3], 0x5F, 0x52, 0x41, 0x2E, // GameTime
                                0x00, 0x00, 0xC3, 0x50, chrID[0], chrID[1], chrID[2], chrID[3], 0x01, 0x46, 0xEA, 
                                0x90, 0x00, // 30024.0
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
                                0xDF, 0xDF, 0x00, 0x0A, 0x00, 0x01, 0x00, 0x65, 0x00, 0x00, 0x0c, 0x0e, chrID[0], 
                                chrID[1], chrID[2], chrID[3], 0x1D, 0x3C, 0x0F, 0x1C, // SpecialAttackWeapon
                                0x00, 0x00, 0xC3, 0x50, chrID[0], chrID[1], chrID[2], chrID[3], 0x01, 0x00, 0x00, 
                                0x0F, 0xC4, 
                                
                                
                                
                                
                                
                                
                                
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
            // Packets.WeaponItemFullUpdate.Send(client, client.Character);
            client.Character.ProcessTimers(DateTime.Now + TimeSpan.FromMilliseconds(200));

            client.Character.CalculateSkills();

            AppearanceUpdate.AnnounceAppearanceUpdate(client.Character);

            // done, so we call a hook.
            // Call all OnConnect script Methods
            Program.csc.CallMethod("OnConnect", client.Character);
        }

        public void Read(int charID, Client client)
        {
            // Don't edit anything in this region
            // unless you are 300% sure you know what you're doing
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

            /* send chat server info to client */
            ChatServerInfo.Send(client);

            /* send playfield info to client */
            PlayfieldAnarchyF.Send(client);

            /* set SocialStatus to 0 */
            client.Character.Stats.SetBaseValue(521, 0);
            Stat.Send(client, 521, 0, false);

            var identity = new Identity { Type = IdentityType.CanbeAffected, Instance = charID };

            /* Action 167 Animation and Stance Data maybe? */
            var message = new CharacterActionMessage
                              {
                                  Identity = identity, 
                                  CharacterActionType = CharacterActionType.ChangeAnimationAndStance,
                                  Target = Identity.None,
                                  ActionArgs = 
                                      new Identity
                                          {
                                              Type = IdentityType.None, 
                                              Instance = 0x00000001
                                          }
                              };
            client.SendCompressed(0x00000C0E, charID, message);

            var gameTimeMessage = new GameTimeMessage
                                      {
                                          Identity = identity, 
                                          Unknown1 = 30024.0f, 
                                          Unknown3 = 185408, 
                                          Unknown4 = 80183.3125f
                                      };
            client.SendCompressed(0x00000C0E, charID, gameTimeMessage);

            /* set SocialStatus to 0 */
            Stat.Set(client, 521, 0, false);

            /* again */
            Stat.Set(client, 521, 0, false);

            /* visual */
            SimpleCharFullUpdate.SendToPlayfield(client);

            /* inventory, items and all that */
            FullCharacter.Send(client);

            var specials = new[]
                               {
                                   new SpecialAttackInfo
                                       {
                                           Unknown1 = 0x0000AAC0, 
                                           Unknown2 = 0x00023569, 
                                           Unknown3 = 0x00000064, 
                                           Unknown4 = "MAAT"
                                       }, 
                                   new SpecialAttackInfo
                                       {
                                           Unknown1 = 0x0000A431, 
                                           Unknown2 = 0x0000A430, 
                                           Unknown3 = 0x00000090, 
                                           Unknown4 = "DIIT"
                                       }, 
                                   new SpecialAttackInfo
                                       {
                                           Unknown1 = 0x00011294, 
                                           Unknown2 = 0x00011295, 
                                           Unknown3 = 0x0000008E, 
                                           Unknown4 = "BRAW"
                                       }
                               };
            var specialAttackWeaponMessage = new SpecialAttackWeaponMessage { Identity = identity, Specials = specials };

            client.SendCompressed(0x00000C0E, charID, specialAttackWeaponMessage);

            // done

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
            // Packets.WeaponItemFullUpdate.Send(client, client.Character);
            client.Character.ProcessTimers(DateTime.Now + TimeSpan.FromMilliseconds(200));

            client.Character.CalculateSkills();

            AppearanceUpdate.AnnounceAppearanceUpdate(client.Character);

            // done, so we call a hook.
            // Call all OnConnect script Methods
            Program.csc.CallMethod("OnConnect", client.Character);
        }

        #endregion
    }
}