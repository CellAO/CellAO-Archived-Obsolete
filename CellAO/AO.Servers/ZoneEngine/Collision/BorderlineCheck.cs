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
        /// Default value of threshold
        /// </summary>
        private const Int32 WallCollisionThreshold = 2;

        /// <summary>
        /// Number of units we use as fuzzy detection
        /// (within 0-n to 0+n units of the client position along the axis of the client heading vector)
        /// The the server will Teleport the client with precise detection we will probably miss the client
        /// intersecting the wall, this is a rudimentary kind of sweep test.
        /// </summary>
        internal Int32 Threshold = WallCollisionThreshold;

        /// <summary>
        /// Collection of walls
        /// </summary>
        public static Dictionary<int, Walls> WallList
        {
            get
            {
                return wallList;
            }
        }

        public static Dictionary<int, Root> Destinations
        {
            get
            {
                return destinations;
            }
        }

        /// <summary>
        /// Collection of line segments built from the walls collection
        /// </summary>
        public static Dictionary<int, List<LineSegment>> Segments
        {
            get
            {
                return segments;
            }
            set
            {
                segments = value;
            }
        }

        /// <summary>
        /// Collection of line segments built from the walls collection
        /// </summary>
        private static Dictionary<Int32, List<LineSegment>> segments;

        private static Dictionary<int, Walls> wallList;

        private static Dictionary<int, Root> destinations;

        /// <summary>
        /// Constructor
        /// </summary>
        public WallCollision()
        {
            wallList = new Dictionary<Int32, Walls>();
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
            foreach (FileInfo fileInfo in fiArr)
            {
                Int32 id = Convert.ToInt32(Path.GetFileNameWithoutExtension(fileInfo.Name));
                WallList.Add(id, this.WallsLoadXml(fileInfo.FullName));
                segments.Add(id, new List<LineSegment>(WallList[id].NumWalls));
            }
        }

        public void LoadDestinations()
        {
            DirectoryInfo di = new DirectoryInfo(Path.Combine("XML Data", "Destinations"));
            FileInfo[] fiArr = di.GetFiles("*.xml");
            foreach (FileInfo fileInfo in fiArr)
            {
                Int32 id = Convert.ToInt32(Path.GetFileNameWithoutExtension(fileInfo.Name));
                Destinations.Add(id, this.PlayFieldsLoadXml(fileInfo.FullName));
            }
        }

        /// <summary>
        /// Load a specific PF wall set
        /// </summary>
        public Walls WallsLoadXml(String fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Walls));
            TextReader reader = new StreamReader(fileName);
            Walls data = (Walls)serializer.Deserialize(reader);
            reader.Close();
            return data;
        }

        public Root PlayFieldsLoadXml(String fileName)
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
            lock (WallList)
            {
                foreach (Int32 playfield in WallList.Keys)
                {
                    foreach (Wall wall in WallList[playfield].WallList)
                    {
                        Point lastPoint = wall.Points[wall.NumPoints - 1];
                        foreach (Point point in wall.Points)
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
        /// <param name="x">X position of client</param>
        /// <param name="z">Z position of client</param>
        /// <param name="playfieldId">Playfield client is currently active in</param>
        public LineSegment Test(Single x, Single z, Int32 playfieldId)
        {
            Vector2 A = new Vector2(x - this.Threshold, z - this.Threshold);
            Vector2 B = new Vector2(x + this.Threshold, z + this.Threshold);

            LineSegment clientLineSegment = new LineSegment(A, B);
            try
            {
                foreach (LineSegment zoneLineSegment in segments[playfieldId])
                {
                    if (this.Intersect(clientLineSegment, zoneLineSegment))
                    {
                        return zoneLineSegment;
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
        public Boolean Intersect(LineSegment lineSegmentA, LineSegment lineSegmentB)
        {
            Vector2 vectorA1 = lineSegmentA.VectorA;
            Vector2 vectorA2 = lineSegmentA.VectorB;
            Vector2 vectorB1 = lineSegmentB.VectorA;
            Vector2 vectorB2 = lineSegmentB.VectorB;
            Int32 intersectionA1 = this.Cross2D(Vector2.Subtract(vectorB2, vectorA1), Vector2.Subtract(vectorB1, vectorA1));
            Int32 intersectionB1 = this.Cross2D(Vector2.Subtract(vectorA1, vectorB1), Vector2.Subtract(vectorA2, vectorB1));
            Int32 intersectionA2 = this.Cross2D(Vector2.Subtract(vectorB1, vectorA2), Vector2.Subtract(vectorB2, vectorA2));
            Int32 intersectionB2 = this.Cross2D(Vector2.Subtract(vectorA2, vectorB2), Vector2.Subtract(vectorA1, vectorB2));
            if (((intersectionA1 <= 0 && intersectionA2 <= 0 && intersectionB1 <= 0 && intersectionB2 <= 0) || (intersectionA1 >= 0 && intersectionA2 >= 0 && intersectionB1 >= 0 && intersectionB2 >= 0))
                && !(intersectionA1 == 0 && intersectionA2 == 0 && intersectionB1 == 0 && intersectionB2 == 0))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get the cross product in 2D of two 2D vectors
        /// </summary>
        public Int32 Cross2D(Vector2 vectorA, Vector2 vectorB)
        {
            return (Int32)(vectorA.X * vectorB.Z - vectorA.Z * vectorB.X);
        }

        public static LineSegment WallCollisionCheck(float x, float z, int playfield)
        {
            return Program.zoneServer.ZoneBorderHandler.Test(x, z, playfield);
        }

        public static AOCoord GetCoord(LineSegment lineSegment, float x, float z, AOCoord coordinates, out Quaternion newHeading)
        {
            newHeading = new Quaternion(0, 0, 0, 0);
            foreach (Line line in Destinations[lineSegment.ZoneToPlayfield].Playfield.Lines)
            {
                if (line.ID == lineSegment.ZoneToIndex)
                {
                    int incX = 0;
                    int incZ = 0;

                    Vector3 temp = new Vector3(line.LineEndPoint.X - line.LineStartPoint.X, 0, line.LineEndPoint.Z - line.LineStartPoint.Z);

                    double factor = 1.0 / Math.Sqrt(Math.Pow(temp.x, 2) + Math.Pow(temp.z, 2));
                    temp.x = temp.x * factor;
                    temp.z = temp.z * factor;

                    if (line.LineStartPoint.X >= line.LineEndPoint.X)
                    {
                        coordinates.x = line.LineEndPoint.X;
                        if (Math.Abs(lineSegment.VectorA.X - lineSegment.VectorB.X) >= 1)
                        {
                            if (lineSegment.VectorA.X > lineSegment.VectorB.X)
                            {
                                coordinates.x += Math.Abs(line.LineEndPoint.X - line.LineStartPoint.X)
                                           * (Math.Abs(x - lineSegment.VectorB.X) / Math.Abs(lineSegment.VectorA.X - lineSegment.VectorB.X));
                                incZ = 1;
                            }
                            else
                            {
                                coordinates.x += Math.Abs(line.LineEndPoint.X - line.LineStartPoint.X)
                                           * (Math.Abs(x - lineSegment.VectorA.X) / Math.Abs(lineSegment.VectorA.X - lineSegment.VectorB.X));
                                incZ = -1;
                            }
                        }
                    }
                    else
                    {
                        coordinates.x = line.LineStartPoint.X;
                        if (Math.Abs(lineSegment.VectorA.X - lineSegment.VectorB.X) >= 1)
                        {
                            if (lineSegment.VectorA.X > lineSegment.VectorB.X)
                            {
                                coordinates.x += Math.Abs(line.LineEndPoint.X - line.LineStartPoint.X)
                                           * (Math.Abs(x - lineSegment.VectorB.X) / Math.Abs(lineSegment.VectorA.X - lineSegment.VectorB.X));
                                incZ = -1;
                            }
                            else
                            {
                                coordinates.x += Math.Abs(line.LineEndPoint.X - line.LineStartPoint.X)
                                           * (Math.Abs(x - lineSegment.VectorA.X) / Math.Abs(lineSegment.VectorA.X - lineSegment.VectorB.X));
                                incZ = 1;
                            }
                        }
                    }
                    if (line.LineStartPoint.Z >= line.LineEndPoint.Z)
                    {
                        coordinates.z = line.LineEndPoint.Z;
                        if (Math.Abs(lineSegment.VectorA.Z - lineSegment.VectorB.Z) >= 1)
                        {
                            if (lineSegment.VectorA.Z > lineSegment.VectorB.Z)
                            {
                                coordinates.z += Math.Abs(line.LineStartPoint.Z - line.LineEndPoint.Z)
                                           * (Math.Abs(z - lineSegment.VectorB.Z) / Math.Abs(lineSegment.VectorA.Z - lineSegment.VectorB.Z));
                                incX = -1;
                            }
                            else
                            {
                                coordinates.z += Math.Abs(line.LineStartPoint.Z - line.LineEndPoint.Z)
                                           * (Math.Abs(z - lineSegment.VectorA.Z) / Math.Abs(lineSegment.VectorA.Z - lineSegment.VectorB.Z));
                                incX = 1;
                            }
                        }
                    }
                    else
                    {
                        coordinates.z = line.LineStartPoint.Z;
                        if (Math.Abs(lineSegment.VectorA.Z - lineSegment.VectorB.Z) >= 1)
                        {
                            if (lineSegment.VectorA.Z > lineSegment.VectorB.Z)
                            {
                                coordinates.z += Math.Abs(line.LineStartPoint.Z - line.LineEndPoint.Z)
                                           * (Math.Abs(z - lineSegment.VectorB.Z) / Math.Abs(lineSegment.VectorA.Z - lineSegment.VectorB.Z));
                                incX = 1;
                            }
                            else
                            {
                                coordinates.z += Math.Abs(line.LineStartPoint.Z - line.LineEndPoint.Z)
                                           * (Math.Abs(z - lineSegment.VectorA.Z) / Math.Abs(lineSegment.VectorA.Z - lineSegment.VectorB.Z));
                                incX = -1;
                            }
                        }
                    }
                    if ((coordinates.y < line.LineStartPoint.Y) || (coordinates.y < line.LineEndPoint.Y))
                    {
                        if (line.LineStartPoint.Y >= line.LineEndPoint.Y)
                        {
                            coordinates.y = line.LineStartPoint.Y;
                        }
                        else
                        {
                            coordinates.y = line.LineEndPoint.Y;
                        }
                    }
                    temp.x = temp.x * incZ * 4;
                    temp.z = temp.z * incX * 4;

                    coordinates.x += Convert.ToSingle(temp.z);
                    coordinates.z += Convert.ToSingle(temp.x);

                    temp.y = temp.x;
                    temp.x = -temp.z;
                    temp.z = temp.y;
                    temp.y = 0;
                    temp = temp.Normalize();
                    newHeading = newHeading.GenerateRotationFromDirectionVector(temp);
                    break;
                }
            }
            return coordinates;
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
        public LineSegment(Point pointA, Point pointB, Int32 destinationPlayfield, Int32 destinationIndex, Int32 destinationFlags)
        {
            this.VectorA = new Vector2(pointA.X, pointA.Z);
            this.VectorB = new Vector2(pointB.X, pointB.Z);
            if (pointA.Y > pointB.Y)
            {
                this.Y = pointB.Y;
            }
            else
            {
                this.Y = pointA.Y;
            }
            this.ZoneToPlayfield = destinationPlayfield;
            this.ZoneToIndex = destinationIndex;
            this.Flags = destinationFlags;
        }

        /// <summary>
        /// 
        /// </summary>
        public LineSegment(Point pointA, Point pointB, Int32 destinationPlayfield, Int32 destinationIndex)
        {
            this.VectorA = new Vector2(pointA.X, pointA.Z);
            this.VectorB = new Vector2(pointB.X, pointB.Z);
            if (pointA.Y > pointB.Y)
            {
                this.Y = pointB.Y;
            }
            else
            {
                this.Y = pointA.Y;
            }
            this.ZoneToPlayfield = destinationPlayfield;
            this.ZoneToIndex = destinationIndex;
            this.Flags = pointA.Flags;
        }

        /// <summary>
        /// 
        /// </summary>
        public LineSegment(Point pointA, Point pointB)
        {
            this.VectorA = new Vector2(pointA.X, pointA.Z);
            this.VectorB = new Vector2(pointB.X, pointB.Z);
            if (pointA.Y > pointB.Y)
            {
                this.Y = pointB.Y;
            }
            else
            {
                this.Y = pointA.Y;
            }
            this.ZoneToPlayfield = pointA.DestinationPlayfield;
            this.ZoneToIndex = pointA.DestIdx;
            this.Flags = pointA.Flags;
        }

        /// <summary>
        /// 
        /// </summary>
        public LineSegment(Vector2 vectorA, Vector2 vectorB)
        {
            this.VectorA = new Vector2(vectorA);
            this.VectorB = new Vector2(vectorB);
            this.Y = 0;
            this.ZoneToPlayfield = 0;
            this.ZoneToIndex = 0;
            this.Flags = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public LineSegment()
        {
            this.VectorA = new Vector2();
            this.VectorB = new Vector2();
            this.Y = 0;
            this.ZoneToPlayfield = 0;
            this.ZoneToIndex = 0;
            this.Flags = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public Vector2 VectorA { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Vector2 VectorB { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Flags { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ZoneToPlayfield { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ZoneToIndex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float Y { get; set; }
    }

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
        public Vector2(Single x, Single z)
        {
            this.X = x;
            this.Z = z;
        }

        /// <summary>
        /// 
        /// </summary>
        public Vector2(Vector2 vector)
        {
            this.X = vector.X;
            this.Z = vector.Z;
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
        public static Vector2 Subtract(Vector2 vectorA, Vector2 vectorB)
        {
            Vector2 vectorC = new Vector2 { X = vectorA.X - vectorB.X, Z = vectorA.Z - vectorB.Z };
            return vectorC;
        }
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
        public Int32 DestinationPlayfield { get; set; }

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
        public List<Point> Points { get; set; }

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
        private List<Wall> wallList;

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("Wall")]
        public List<Wall> WallList
        {
            get
            {
                return this.wallList;
            }
        }

        /// <summary>
        ///
        /// </summary>
        [XmlAttribute("NumWalls")]
        public int NumWalls { get; set; }
    };

    public class LineStart
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

    public class LineEnd
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
        public LineStart LineStartPoint { get; set; }

        [XmlElement("End")]
        public LineEnd LineEndPoint { get; set; }

        /// <summary>
        ///
        /// </summary>
        [XmlAttribute("ID")]
        public int ID { get; set; }
    };

    public class PlayField
    {
        private List<Line> lines;

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("Line")]
        public List<Line> Lines
        {
            get
            {
                return this.lines;
            }
        }

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
        public PlayField Playfield;
    };

}