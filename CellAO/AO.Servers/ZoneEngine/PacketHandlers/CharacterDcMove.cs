// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CharacterDcMove.cs" company="CellAO Team">
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
//   Defines the CharacterDCMove type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZoneEngine.PacketHandlers
{
    using System;

    using AO.Core;

    using SmokeLounge.AOtomation.Messaging.GameData;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    using ZoneEngine.Collision;
    using ZoneEngine.Misc;

    using Identity = SmokeLounge.AOtomation.Messaging.GameData.Identity;
    using Quaternion = AO.Core.Quaternion;
    using Vector3 = SmokeLounge.AOtomation.Messaging.GameData.Vector3;

    public static class CharacterDCMove
    {
        #region Public Methods and Operators

        public static void Read(CharDCMoveMessage message, Client client)
        {
            var moveType = message.MoveType;
            var heading = new Quaternion(message.Heading.X, message.Heading.Y, message.Heading.Z, message.Heading.W);
            var coordinates = new AOCoord(message.Coordinates.X, message.Coordinates.Y, message.Coordinates.Z);

            // TODO: Find out what these (tmpInt) are and name them
            var tmpInt1 = message.Unknown1;
            var tmpInt2 = message.Unknown2;
            var tmpInt3 = message.Unknown3;

            if (!client.Character.DoNotDoTimers)
            {
                var teleportPlayfield = WallCollision.WallCollisionCheck(
                    coordinates.x, coordinates.z, client.Character.PlayField);
                if (teleportPlayfield.ZoneToPlayfield >= 1)
                {
                    var coordHeading = WallCollision.GetCoord(
                        teleportPlayfield, coordinates.x, coordinates.z, coordinates);
                    if (teleportPlayfield.Flags != 1337 && client.Character.PlayField != 152
                        || Math.Abs(client.Character.Coordinates.y - teleportPlayfield.Y) <= 2
                        || teleportPlayfield.Flags == 1337
                        && Math.Abs(client.Character.Coordinates.y - teleportPlayfield.Y) <= 6)
                    {
                        client.Teleport(
                            coordHeading.Coordinates, coordHeading.Heading, teleportPlayfield.ZoneToPlayfield);
                        Program.zoneServer.Clients.Remove(client);
                    }

                    return;
                }

                if (client.Character.Stats.LastConcretePlayfieldInstance.Value != 0)
                {
                    var correspondingDoor = DoorHandler.DoorinRange(
                        client.Character.PlayField, client.Character.Coordinates, 1.0f);
                    if (correspondingDoor != null)
                    {
                        correspondingDoor = DoorHandler.FindCorrespondingDoor(correspondingDoor, client.Character);
                        client.Character.Stats.LastConcretePlayfieldInstance.Value = 0;
                        var aoc = correspondingDoor.Coordinates;
                        aoc.x += correspondingDoor.hX * 3;
                        aoc.y += correspondingDoor.hY * 3;
                        aoc.z += correspondingDoor.hZ * 3;
                        client.Teleport(aoc, client.Character.Heading, correspondingDoor.playfield);
                        Program.zoneServer.Clients.Remove(client);
                        return;
                    }
                }
            }

            client.Character.RawCoord = coordinates;
            client.Character.RawHeading = heading;
            client.Character.UpdateMoveType(moveType);

            /* Start NV Heading Testing Code
             * Yaw: 0 to 360 Degrees (North turning clockwise to a complete revolution)
             * Roll: Not sure, but is always 0 cause we can't roll in AO
             * Pitch: 90 to -90 Degrees (90 is nose in the air, 0 is level, -90 is nose to the ground)
             */
            /* Comment this line with a '//' to enable heading testing
            client.SendChatText("Raw Headings: X: " + client.Character.heading.x + " Y: " + client.Character.heading.y + " Z:" + client.Character.heading.z);
            
            client.SendChatText("Yaw:  " + Math.Round(180 * client.Character.heading.yaw / Math.PI) + " Degrees");
            client.SendChatText("Roll: " + Math.Round(180 * client.Character.heading.roll / Math.PI) + " Degrees");
            client.SendChatText("Pitch:   " + Math.Round(180 * client.Character.heading.pitch / Math.PI) + " Degrees");
            /* End NV Heading testing code */

            /* start of packet */
            var reply = new CharDCMoveMessage
                            {
                                Identity =
                                    new Identity
                                        {
                                            Type = IdentityType.CanbeAffected, 
                                            Instance = client.Character.Id
                                        }, 
                                Unknown = 0x00, 
                                MoveType = moveType, 
                                Heading =
                                    new SmokeLounge.AOtomation.Messaging.GameData.Quaternion
                                        {
                                            X =
                                                heading
                                                .xf, 
                                            Y =
                                                heading
                                                .yf, 
                                            Z =
                                                heading
                                                .zf, 
                                            W =
                                                heading
                                                .wf
                                        }, 
                                Coordinates =
                                    new Vector3 { X = heading.xf, Y = heading.yf, Z = heading.zf }, 
                                Unknown1 = tmpInt1, 
                                Unknown2 = tmpInt2, 
                                Unknown3 = tmpInt3
                            };

            Announce.Playfield(client.Character.PlayField, 0x00000C0E, reply);

            if (Statels.StatelppfonEnter.ContainsKey(client.Character.PlayField))
            {
                foreach (var s in Statels.StatelppfonEnter[client.Character.PlayField])
                {
                    if (s.onEnter(client))
                    {
                        return;
                    }

                    if (s.onTargetinVicinity(client))
                    {
                        return;
                    }
                }
            }
        }

        #endregion
    }
}