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

namespace ZoneEngine.Collision
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    using AO.Core;

    /// <summary>
    /// Class contains a list of line segments and functions to test a client against them
    /// </summary>
    public class WallCollision
    {
        /// <summary>
        /// This holds 2 coordinates representing a 2d cartesian vector
        /// </summary>
        public class Vector2
        {
            /// <summary>
            /// 
            /// </summary>
            public Vector2()
            {
                this.X = 0.0f;
                this.Z = 0.0f;
            }

            /// <summary>
            /// 
            /// </summary>
            public Vector2(Single VX, Single VZ)
            {
                this.X = VX;
                this.Z = VZ;
            }

            /// <summary>
            /// 
            /// </summary>
            public Vector2(Vector2 V)
            {
                this.X = V.X;
                this.Z = V.Z;
            }

            /// <summary>
            /// 
            /// </summary>
            public Single X;

            /// <summary>
            /// 
            /// </summary>
            public Single Z;

            /// <summary>
            /// 
            /// </summary>
            public static Vector2 Subtract(Vector2 A, Vector2 B)
            {
                Vector2 C = new Vector2();
                C.X = A.X - B.X;
                C.Z = A.Z - B.Z;
                return C;
            }
        }

        /// <summary>
        /// This holds two 2d vectors made from 2 points that represent a line segment
        /// </summary>
        public class LineSegment
        {
            /// <summary>
            /// 
            /// </summary>
            public LineSegment(Point PA, Point PB, Int32 DestPF, Int32 DestIDX, Int32 DestFlags)
            {
                this.A = new Vector2(PA.X, PA.Z);
                this.B = new Vector2(PB.X, PB.Z);
                if (PA.Y > PB.Y)
                {
                    this.Y = PB.Y;
                }
                else
                {
                    this.Y = PA.Y;
                }
                this.ZoneToPF = DestPF;
                this.ZoneToIDX = DestIDX;
                this.Flags = DestFlags;
            }

            /// <summary>
            /// 
            /// </summary>
            public LineSegment(Point PA, Point PB, Int32 DestPF, Int32 DestIDX)
            {
                this.A = new Vector2(PA.X, PA.Z);
                this.B = new Vector2(PB.X, PB.Z);
                if (PA.Y > PB.Y)
                {
                    this.Y = PB.Y;
                }
                else
                {
                    this.Y = PA.Y;
                }
                this.ZoneToPF = DestPF;
                this.ZoneToIDX = DestIDX;
                this.Flags = PA.Flags;
            }

            /// <summary>
            /// 
            /// </summary>
            public LineSegment(Point PA, Point PB)
            {
                this.A = new Vector2(PA.X, PA.Z);
                this.B = new Vector2(PB.X, PB.Z);
                if (PA.Y > PB.Y)
                {
                    this.Y = PB.Y;
                }
                else
                {
                    this.Y = PA.Y;
                }
                this.ZoneToPF = PA.DestPF;
                this.ZoneToIDX = PA.DestIdx;
                this.Flags = PA.Flags;
            }

            /// <summary>
            /// 
            /// </summary>
            public LineSegment(Vector2 VA, Vector2 VB)
            {
                this.A = new Vector2(VA);
                this.B = new Vector2(VB);
                this.Y = 0;
                this.ZoneToPF = 0;
                this.ZoneToIDX = 0;
                this.Flags = 0;
            }

            /// <summary>
            /// 
            /// </summary>
            public LineSegment()
            {
                this.A = new Vector2();
                this.B = new Vector2();
                this.Y = 0;
                this.ZoneToPF = 0;
                this.ZoneToIDX = 0;
                this.Flags = 0;
            }

            /// <summary>
            /// 
            /// </summary>
            public Vector2 A;

            /// <summary>
            /// 
            /// </summary>
            public Vector2 B;

            /// <summary>
            /// 
            /// </summary>
            public Single Y;

            /// <summary>
            /// 
            /// </summary>
            public Int32 ZoneToPF;

            /// <summary>
            /// 
            /// </summary>
            public Int32 ZoneToIDX;

            /// <summary>
            /// 
            /// </summary>
            public Int32 Flags;
        }

        /// <summary>
        /// This holds a destination zone and border coordinate
        /// </summary>
        public class Point
        {
            /// <summary>
            ///
            /// </summary>
            [XmlAttribute("DestPF")]
            public Int32 DestPF { get; set; }

            /// <summary>
            ///
            /// </summary>
            [XmlAttribute("DestIdx")]
            public Int32 DestIdx { get; set; }

            /// <summary>
            ///
            /// </summary>
            [XmlAttribute("Flags")]
            public Int32 Flags { get; set; }

            /// <summary>
            ///
            /// </summary>
            [XmlAttribute("X")]
            public Single X { get; set; }

            /// <summary>
            ///
            /// </summary>
            [XmlAttribute("Y")]
            public Single Y { get; set; }

            /// <summary>
            ///
            /// </summary>
            [XmlAttribute("Z")]
            public Single Z { get; set; }
        };

        /// <summary>
        /// This holds a list of points
        /// </summary>
        public class Wall
        {
            /// <summary>
            /// 
            /// </summary>
            [XmlElement("Point")]
            public List<Point> points;

            /// <summary>
            ///
            /// </summary>
            [XmlAttribute("NumPoints")]
            public int NumPoints { get; set; }
        };

        /// <summary>
        /// This holds a list of walls
        /// </summary>
        [XmlRoot("Walls")]
        public class Walls
        {
            /// <summary>
            /// 
            /// </summary>
            [XmlElement("Wall")]
            public List<Wall> walls;

            /// <summary>
            ///
            /// </summary>
            [XmlAttribute("NumWalls")]
            public int NumWalls { get; set; }
        };

        public class Start
        {
            /// <summary>
            ///
            /// </summary>
            [XmlAttribute("X")]
            public Single X { get; set; }

            /// <summary>
            ///
            /// </summary>
            [XmlAttribute("Y")]
            public Single Y { get; set; }

            /// <summary>
            ///
            /// </summary>
            [XmlAttribute("Z")]
            public Single Z { get; set; }
        };

        public class End
        {
            /// <summary>
            ///
            /// </summary>
            [XmlAttribute("X")]
            public Single X { get; set; }

            /// <summary>
            ///
            /// </summary>
            [XmlAttribute("Y")]
            public Single Y { get; set; }

            /// <summary>
            ///
            /// </summary>
            [XmlAttribute("Z")]
            public Single Z { get; set; }
        };

        public class Line
        {
            /// <summary>
            /// 
            /// </summary>
            [XmlElement("Start")]
            public Start start;

            [XmlElement("End")]
            public End end;

            /// <summary>
            ///
            /// </summary>
            [XmlAttribute("ID")]
            public int ID { get; set; }
        };

        public class PlayField
        {
            /// <summary>
            /// 
            /// </summary>
            [XmlElement("Line")]
            public List<Line> lines;

            /// <summary>
            ///
            /// </summary>
            [XmlAttribute("NumLines")]
            public int NumLines { get; set; }
        };

        [XmlRoot("Root")]
        public class Root
        {
            /// <summary>
            /// 
            /// </summary>
            [XmlElement("PlayField")]
            public PlayField playfield;
        };

        /// <summary>
        /// Default value of threshold
        /// </summary>
        private const Int32 WALL_COLLISION_THRESHOLD = 2;

        /// <summary>
        /// Number of units we use as fuzzy detection
        /// (within 0-n to 0+n units of the client position along the axis of the client heading vector)
        /// The the server will Teleport the client with precise detection we will probably miss the client
        /// intersecting the wall, this is a rudimentary kind of sweep test.
        /// </summary>
        public Int32 threshold = WALL_COLLISION_THRESHOLD;

        /// <summary>
        /// Collection of walls
        /// </summary>
        public static Dictionary<Int32, Walls> walls;

        public static Dictionary<Int32, Root> destinations;

        /// <summary>
        /// Collection of line segments built from the walls collection
        /// </summary>
        public static Dictionary<Int32, List<LineSegment>> segments;

        /// <summary>
        /// Constructor
        /// </summary>
        public WallCollision()
        {
            walls = new Dictionary<Int32, Walls>();
            segments = new Dictionary<Int32, List<LineSegment>>();
            destinations = new Dictionary<Int32, Root>();

            this.LoadWalls();
            this.LoadDestinations();
            this.BuildLineSegments();
        }

        /// <summary>
        /// Iterate over all wall XML files and load them
        /// </summary>
        public void LoadWalls()
        {
            DirectoryInfo di = new DirectoryInfo(Path.Combine("XML Data", "Walls"));
            FileInfo[] fiArr = di.GetFiles("*.xml");
            foreach (FileInfo fi in fiArr)
            {
                Int32 id = Convert.ToInt32(Path.GetFileNameWithoutExtension(fi.Name));
                walls.Add(id, this.WallsLoadXML(fi.FullName));
                segments.Add(id, new List<LineSegment>(walls[id].NumWalls));
            }
        }

        public void LoadDestinations()
        {
            DirectoryInfo di = new DirectoryInfo(Path.Combine("XML Data", "Destinations"));
            FileInfo[] fiArr = di.GetFiles("*.xml");
            foreach (FileInfo fi in fiArr)
            {
                Int32 id = Convert.ToInt32(Path.GetFileNameWithoutExtension(fi.Name));
                destinations.Add(id, this.PlayFieldsLoadXML(fi.FullName));
            }
        }

        /// <summary>
        /// Load a specific PF wall set
        /// </summary>
        public Walls WallsLoadXML(String fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Walls));
            TextReader reader = new StreamReader(fileName);
            Walls data = (Walls)serializer.Deserialize(reader);
            reader.Close();
            return data;
        }

        public Root PlayFieldsLoadXML(String fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Root));
            TextReader reader = new StreamReader(fileName);
            Root data = (Root)serializer.Deserialize(reader);
            reader.Close();
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        public void BuildLineSegments()
        {
            lock (walls)
            {
                foreach (Int32 playfield in walls.Keys)
                {
                    foreach (Wall wall in walls[playfield].walls)
                    {
                        Point lastPoint = wall.points[wall.NumPoints - 1];
                        foreach (Point point in wall.points)
                        {
                            LineSegment lineSegment = new LineSegment(point, lastPoint);
                            segments[playfield].Add(lineSegment);
                            lastPoint = point;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Test of two line segments intersect
        /// </summary>
        /// <param name="X">X position of client</param>
        /// <param name="Z">Z position of client</param>
        /// <param name="PFID">Playfield client is currently active in</param>
        public LineSegment Test(Single X, Single Z, Int32 PFID)
        {
            Vector2 A = new Vector2(X - this.threshold, Z - this.threshold);
            Vector2 B = new Vector2(X + this.threshold, Z + this.threshold);

            LineSegment clientLS = new LineSegment(A, B);
            try
            {
                foreach (LineSegment zoneLS in segments[PFID])
                {
                    if (this.Intersect(clientLS, zoneLS))
                    {
                        return zoneLS;
                    }
                }
            }
            catch
            {
            }
            LineSegment empty = new LineSegment();
            return empty;
        }

        /// <summary>
        /// Test of two line segments intersect
        /// </summary>
        public Boolean Intersect(LineSegment A, LineSegment B)
        {
            Vector2 A1 = A.A;
            Vector2 A2 = A.B;
            Vector2 B1 = B.A;
            Vector2 B2 = B.B;
            Int32 IA1 = this.Cross2D(Vector2.Subtract(B2, A1), Vector2.Subtract(B1, A1));
            Int32 IB1 = this.Cross2D(Vector2.Subtract(A1, B1), Vector2.Subtract(A2, B1));
            Int32 IA2 = this.Cross2D(Vector2.Subtract(B1, A2), Vector2.Subtract(B2, A2));
            Int32 IB2 = this.Cross2D(Vector2.Subtract(A2, B2), Vector2.Subtract(A1, B2));
            if (((IA1 <= 0 && IA2 <= 0 && IB1 <= 0 && IB2 <= 0) || (IA1 >= 0 && IA2 >= 0 && IB1 >= 0 && IB2 >= 0))
                && !(IA1 == 0 && IA2 == 0 && IB1 == 0 && IB2 == 0))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get the cross product in 2D of two 2D vectors
        /// </summary>
        public Int32 Cross2D(Vector2 A, Vector2 B)
        {
            return (Int32)(A.X * B.Z - A.Z * B.X);
        }

        public static LineSegment WallCollisionCheck(float x, float z, int PF)
        {
            return Program.zoneServer.ZoneBorderHandler.Test(x, z, PF);
        }

        public static AOCoord GetCoord(LineSegment LS, float x, float z, AOCoord coord, out Quaternion newheading)
        {
            newheading = new Quaternion(0, 0, 0, 0);
            foreach (Line line in destinations[LS.ZoneToPF].playfield.lines)
            {
                if (line.ID == LS.ZoneToIDX)
                {
                    int incx = 0;
                    int incz = 0;

                    Vector3 temp = new Vector3(line.end.X - line.start.X, 0, line.end.Z - line.start.Z);

                    double factor = 1.0 / Math.Sqrt(Math.Pow(temp.x, 2) + Math.Pow(temp.z, 2));
                    temp.x = temp.x * factor;
                    temp.z = temp.z * factor;

                    if (line.start.X >= line.end.X)
                    {
                        coord.x = line.end.X;
                        if (Math.Abs(LS.A.X - LS.B.X) >= 1)
                        {
                            if (LS.A.X > LS.B.X)
                            {
                                coord.x += Math.Abs(line.end.X - line.start.X)
                                           * (Math.Abs(x - LS.B.X) / Math.Abs(LS.A.X - LS.B.X));
                                incz = 1;
                            }
                            else
                            {
                                coord.x += Math.Abs(line.end.X - line.start.X)
                                           * (Math.Abs(x - LS.A.X) / Math.Abs(LS.A.X - LS.B.X));
                                incz = -1;
                            }
                        }
                    }
                    else
                    {
                        coord.x = line.start.X;
                        if (Math.Abs(LS.A.X - LS.B.X) >= 1)
                        {
                            if (LS.A.X > LS.B.X)
                            {
                                coord.x += Math.Abs(line.end.X - line.start.X)
                                           * (Math.Abs(x - LS.B.X) / Math.Abs(LS.A.X - LS.B.X));
                                incz = -1;
                            }
                            else
                            {
                                coord.x += Math.Abs(line.end.X - line.start.X)
                                           * (Math.Abs(x - LS.A.X) / Math.Abs(LS.A.X - LS.B.X));
                                incz = 1;
                            }
                        }
                    }
                    if (line.start.Z >= line.end.Z)
                    {
                        coord.z = line.end.Z;
                        if (Math.Abs(LS.A.Z - LS.B.Z) >= 1)
                        {
                            if (LS.A.Z > LS.B.Z)
                            {
                                coord.z += Math.Abs(line.start.Z - line.end.Z)
                                           * (Math.Abs(z - LS.B.Z) / Math.Abs(LS.A.Z - LS.B.Z));
                                incx = -1;
                            }
                            else
                            {
                                coord.z += Math.Abs(line.start.Z - line.end.Z)
                                           * (Math.Abs(z - LS.A.Z) / Math.Abs(LS.A.Z - LS.B.Z));
                                incx = 1;
                            }
                        }
                    }
                    else
                    {
                        coord.z = line.start.Z;
                        if (Math.Abs(LS.A.Z - LS.B.Z) >= 1)
                        {
                            if (LS.A.Z > LS.B.Z)
                            {
                                coord.z += Math.Abs(line.start.Z - line.end.Z)
                                           * (Math.Abs(z - LS.B.Z) / Math.Abs(LS.A.Z - LS.B.Z));
                                incx = 1;
                            }
                            else
                            {
                                coord.z += Math.Abs(line.start.Z - line.end.Z)
                                           * (Math.Abs(z - LS.A.Z) / Math.Abs(LS.A.Z - LS.B.Z));
                                incx = -1;
                            }
                        }
                    }
                    if ((coord.y < line.start.Y) || (coord.y < line.end.Y))
                    {
                        if (line.start.Y >= line.end.Y)
                        {
                            coord.y = line.start.Y;
                        }
                        else
                        {
                            coord.y = line.end.Y;
                        }
                    }
                    temp.x = temp.x * incz * 4;
                    temp.z = temp.z * incx * 4;

                    coord.x += Convert.ToSingle(temp.z);
                    coord.z += Convert.ToSingle(temp.x);

                    temp.y = temp.x;
                    temp.x = -temp.z;
                    temp.z = temp.y;
                    temp.y = 0;
                    temp = temp.Normalize();
                    newheading = newheading.GenerateRotationFromDirectionVector(temp);
                    break;
                }
            }
            return coord;
        }
    };
}