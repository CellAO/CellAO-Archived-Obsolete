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

    using AO.Core;

    using ZoneEngine.Collision;
    using ZoneEngine.Misc;

    /// <summary>
    /// 
    /// </summary>
    public static class CharacterDcMove
    {
        /// <summary>
        /// TODO: Add a Description of what this Class does.. -Looks at someone else-
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="client"></param>
        public static void Read(byte[] packet, Client client)
        {
            PacketWriter packetWriter = new PacketWriter();
            PacketReader packetReader = new PacketReader(packet);

            Header header = packetReader.PopHeader();
            packetReader.PopByte();
            byte moveType = packetReader.PopByte();
            Quaternion heading = packetReader.PopQuat();
            AOCoord coordinates = packetReader.PopCoord();
            // TODO: Find out what these (tmpInt) are and name them
            int tmpInt1 = packetReader.PopInt();
            int tmpInt2 = packetReader.PopInt();
            int tmpInt3 = packetReader.PopInt();
            packetReader.Finish();

            if (!client.Character.DoNotDoTimers)
            {
                LineSegment teleportPlayfield = WallCollision.WallCollisionCheck(
                    coordinates.x, coordinates.z, client.Character.PlayField);
                if (teleportPlayfield.ZoneToPlayfield >= 1)
                {
                    Quaternion newHeading = new Quaternion(0, 0, 0, 0);
                    coordinates = WallCollision.GetCoord(teleportPlayfield, coordinates.x, coordinates.z, coordinates, out newHeading);
                    if (teleportPlayfield.Flags != 1337 && client.Character.PlayField != 152
                        || Math.Abs(client.Character.Coordinates.y - teleportPlayfield.Y) <= 2
                        || teleportPlayfield.Flags == 1337 && Math.Abs(client.Character.Coordinates.y - teleportPlayfield.Y) <= 6)
                    {
                        client.Teleport(coordinates, newHeading, teleportPlayfield.ZoneToPlayfield);
                        Program.zoneServer.Clients.Remove(client);
                    }
                    return;
                }

                Doors correspondingDoor = null;
                if (client.Character.Stats.LastConcretePlayfieldInstance.Value != 0)
                {
                    correspondingDoor = DoorHandler.DoorinRange(client.Character.PlayField, client.Character.Coordinates, 1.0f);
                    if (correspondingDoor != null)
                    {
                        correspondingDoor = DoorHandler.FindCorrespondingDoor(correspondingDoor, client.Character);
                        client.Character.Stats.LastConcretePlayfieldInstance.Value = 0;
                        AOCoord aoc = correspondingDoor.Coordinates;
                        aoc.x += correspondingDoor.hX * 3;
                        aoc.y += correspondingDoor.hY * 3;
                        aoc.z += correspondingDoor.hZ * 3;
                        client.Teleport(aoc, client.Character.Heading, correspondingDoor.playfield);
                        Program.zoneServer.Clients.Remove(client);
                        return;
                    }
                }
            }

            client.Character.rawCoord = coordinates;
            client.Character.rawHeading = heading;
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
            packetWriter.PushByte(0xDF);
            packetWriter.PushByte(0xDF);
            /* packet type */
            packetWriter.PushShort(10);
            /* unknown */
            packetWriter.PushShort(1);
            /* packet length (writer takes care of this) */
            packetWriter.PushShort(0);
            /* server ID */
            packetWriter.PushInt(3086);
            /* receiver (Announce takes care of this) */
            packetWriter.PushInt(0);
            /* packet ID */
            packetWriter.PushInt(0x54111123);
            /* affected dynel identity */
            packetWriter.PushIdentity(50000, client.Character.ID);
            /* ? */
            packetWriter.PushByte(0);
            /* movement type */
            packetWriter.PushByte(moveType);
            /* Heading */
            packetWriter.PushQuat(heading);
            /* Coordinates */
            packetWriter.PushCoord(coordinates);
            // see reading part for comment
            packetWriter.PushInt(tmpInt1);
            packetWriter.PushInt(tmpInt2);
            packetWriter.PushInt(tmpInt3);
            byte[] reply = packetWriter.Finish();
            Announce.Playfield(client.Character.PlayField, reply);

            if (Statels.StatelppfonEnter.ContainsKey(client.Character.PlayField))
            {
                foreach (Statels.Statel s in Statels.StatelppfonEnter[client.Character.PlayField])
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
    }
}