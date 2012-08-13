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

#region Usings..

#endregion

namespace ZoneEngine.Misc
{
    using System;
    using System.Data;
    using System.Globalization;

    using AO.Core;

    public class DoorHandler
    {
        public static int CacheAllFromDB()
        {
            int c = 0;
            SqlWrapper ms = new SqlWrapper();
            DataTable dt = ms.ReadDT("SELECT * FROM doors");

            foreach (DataRow row in dt.Rows)
            {
                Doors door = new Doors();
                door.ID = (Int32)row["ID"];
                door.Coordinates.x = (Single)row["X"];
                door.Coordinates.y = (Single)row["Y"];
                door.Coordinates.z = (Single)row["Z"];
                door.hX = (Single)row["hx"];
                door.hY = (Single)row["hy"];
                door.hZ = (Single)row["hz"];
                door.hW = (Single)row["hw"];

                door.teleport_to_ID = (Int32)row["toid"];
                door.teleport_to_PlayField = (Int32)(UInt32)row["toplayfield"];
                door.proxy = (Boolean)row["proxy"];
                door.playfield = (Int32)(UInt32)row["playfield"];
                Program.zoneServer.Doors.Add(door);
                c++;
            }
            return c;
        }

        public static Doors DoorinRange(int playfield, AOCoord coord, float range)
        {
            int pf;
            foreach (Doors door in Program.zoneServer.Doors)
            {
                pf = door.ID & 0xffff;
                if (pf != playfield)
                {
                    continue;
                }
                if ((coord.distance2D(door.Coordinates) < range)
                    && (Math.Abs(coord.coordinate.y - door.Coordinates.y) < 3))
                {
                    return door;
                }
            }
            return null;
        }

        public static Doors FindCorrespondingDoor(Doors _door, Character ch)
        {
            if (_door.teleport_to_ID == 0)
            {
                uint lastconcrete = ch.Stats.ExtenalDoorInstance.StatBaseValue;
                foreach (Doors door in Program.zoneServer.Doors)
                {
                    if (lastconcrete == (uint)door.ID)
                    {
                        return door;
                    }
                }
            }

            foreach (Doors door in Program.zoneServer.Doors)
            {
                if (door != _door)
                {
                    if (door.ID == _door.teleport_to_ID)
                    {
                        return door;
                    }
                }
            }
            return null; // Should not happen EVER
        }

        public static void UpdateDoorHeading(Client cli)
        {
            SqlWrapper ms = new SqlWrapper();
            Doors door = DoorinRange(cli.Character.PlayField, cli.Character.Coordinates, 4.0f);
            if (door == null)
            {
                cli.SendChatText("No door in range to align");
                return;
            }
            cli.SendChatText(
                "Door " + door.ID.ToString() + " Heading before: " + door.hX.ToString() + " " + door.hY.ToString() + " "
                + door.hZ.ToString() + " " + door.hW.ToString());
            AOCoord a = new AOCoord();
            a.x = cli.Character.Coordinates.x - door.Coordinates.x;
            a.y = cli.Character.Coordinates.y - door.Coordinates.y;
            a.z = cli.Character.Coordinates.z - door.Coordinates.z;
            Quaternion q = new Quaternion(a.x, a.y, a.z, 0);
            cli.SendChatText(
                "Door " + door.ID.ToString() + " Heading now: " + q.x.ToString() + " " + q.y.ToString() + " "
                + q.z.ToString() + " " + q.w.ToString());
            ms.SqlUpdate(
                "UPDATE doors SET HX=" + String.Format(CultureInfo.InvariantCulture, "'{0}'", q.x) + ", HY="
                + String.Format(CultureInfo.InvariantCulture, "'{0}'", q.y) + ", HZ="
                + String.Format(CultureInfo.InvariantCulture, "'{0}'", q.z) + ", HW="
                + String.Format(CultureInfo.InvariantCulture, "'{0}'", q.w) + " WHERE ID=" + door.ID.ToString() + ";");
            door.hX = (float)q.x;
            door.hY = (float)q.y;
            door.hZ = (float)q.z;
            door.hW = (float)q.w;
        }
    }
}