#region License
/*
Copyright (c) 2005-2012, CellAO Team

All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion

#region Usings...
using System;
using AO.Core;
using ZoneEngine.Collision;
using ZoneEngine.Misc;
#endregion

namespace ZoneEngine.PacketHandlers
{
    /// <summary>
    /// 
    /// </summary>
    public class CharDCMove
    {
        /// <summary>
        /// TODO: Add a Description of what this Class does.. -Looks at someone else-
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="client"></param>
        public static void Read(ref byte[] packet, Client client)
        {
            PacketWriter _writer = new PacketWriter();
            PacketReader _reader = new PacketReader(ref packet);

            Header header = _reader.PopHeader();
            _reader.PopByte();
            byte _movetype = _reader.PopByte();
            Quaternion _heading = _reader.PopQuat();
            AOCoord _coord = _reader.PopCoord();
            // TODO: Find out what these (tmpInt) are and name them
            int tmpInt1 = _reader.PopInt();
            int tmpInt2 = _reader.PopInt();
            int tmpInt3 = _reader.PopInt();
            _reader.Finish();

            if (!client.Character.dontdotimers)
            {
                WallCollision.LineSegment teleportPF = WallCollision.WallCollisionCheck(_coord.x, _coord.z,
                                                                                        client.Character.PlayField);
                if (teleportPF.ZoneToPF >= 1)
                {
                    Quaternion newheading = new Quaternion(0, 0, 0, 0);
                    _coord = WallCollision.GetCoord(teleportPF, _coord.x, _coord.z, _coord, out newheading);
                    if (teleportPF.Flags != 1337 && client.Character.PlayField != 152 ||
                        Math.Abs(client.Character.Coordinates.y - teleportPF.Y) <= 2 ||
                        teleportPF.Flags == 1337 && Math.Abs(client.Character.Coordinates.y - teleportPF.Y) <= 6)
                    {
                        client.Teleport(_coord, newheading, teleportPF.ZoneToPF);
                        Program.zoneServer.Clients.Remove(client);
                    }
                    return;
                }

                Doors door = null;
                if (client.Character.Stats.LastConcretePlayfieldInstance.Value != 0)
                {
                    door = DoorHandler.DoorinRange(client.Character.PlayField, client.Character.Coordinates, 1.0f);
                    if (door != null)
                    {
                        door = DoorHandler.FindCorrespondingDoor(door, client.Character);
                        client.Character.Stats.LastConcretePlayfieldInstance.Value = 0;
                        AOCoord aoc = door.Coordinates;
                        aoc.x += door.hX*3;
                        aoc.y += door.hY*3;
                        aoc.z += door.hZ*3;
                        client.Teleport(aoc, client.Character.Heading, door.playfield);
                        Program.zoneServer.Clients.Remove(client);
                        return;
                    }
                }
            }

            client.Character.rawCoord = _coord;
            client.Character.rawHeading = _heading;
            client.Character.updateMoveType(_movetype);

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
            _writer.PushByte(0xDF);
            _writer.PushByte(0xDF);
            /* packet type */
            _writer.PushShort(10);
            /* unknown */
            _writer.PushShort(1);
            /* packet length (writer takes care of this) */
            _writer.PushShort(0);
            /* server ID */
            _writer.PushInt(3086);
            /* receiver (Announce takes care of this) */
            _writer.PushInt(0);
            /* packet ID */
            _writer.PushInt(0x54111123);
            /* affected dynel identity */
            _writer.PushIdentity(50000, client.Character.ID);
            /* ? */
            _writer.PushByte(0);
            /* movement type */
            _writer.PushByte(_movetype);
            /* Heading */
            _writer.PushQuat(_heading);
            /* Coordinates */
            _writer.PushCoord(_coord);
            // see reading part for comment
            _writer.PushInt(tmpInt1);
            _writer.PushInt(tmpInt2);
            _writer.PushInt(tmpInt3);
            byte[] reply = _writer.Finish();
            Announce.Playfield(client.Character.PlayField, ref reply);

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