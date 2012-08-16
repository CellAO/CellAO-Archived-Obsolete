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

#region Using
using System;
using System.Globalization;
#endregion

namespace AO.Core
{
    /// <summary>
    /// AO Coordinates Class
    /// </summary>
    public class AOCoord
    {
        #region Coordinate Representations
        /// <summary>
        /// Vector representation of Coordinates
        /// </summary>
        public Vector3 coordinate;

        /// <summary>
        /// Quaterion representation of Coordinates (w is 0)
        /// </summary>
        public Quaternion QuatCoordinate
        {
            get { return new Quaternion(coordinate); }
            set { coordinate = value.VectorRepresentation(); }
        }

        /// <summary>
        /// Component representation of X Coordinate
        /// </summary>
        public float x
        {
            get { return (float) coordinate.x; }
            set { coordinate.x = value; }
        }

        /// <summary>
        /// Component representation of Y Coordinate
        /// </summary>
        public float y
        {
            get { return (float) coordinate.y; }
            set { coordinate.y = value; }
        }

        /// <summary>
        /// Component representation of Z Coordinate
        /// </summary>
        public float z
        {
            get { return (float) coordinate.z; }
            set { coordinate.z = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public AOCoord()
        {
            this.Update(0, 0, 0);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="aoCoord">AOCoord coordinate is at</param>
        public AOCoord(AOCoord aoCoord)
        {
            this.Update(aoCoord);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coordinate">Vector coordinate is at</param>
        public AOCoord(Vector3 coordinate)
        {
            this.Update(coordinate);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x">x component of the Coordinate</param>
        /// <param name="y">y component of the Coordinate</param>
        /// <param name="z">z component of the Coordinate</param>
        public AOCoord(float x, float y, float z)
        {
            this.Update(x, y, z);
        }
        #endregion

        #region Update
        /// <summary>
        /// Update the Coordinate using components
        /// </summary>
        /// <param name="x">x component of the Coordinate</param>
        /// <param name="y">y component of the Coordinate</param>
        /// <param name="z">z component of the Coordinate</param>
        public void Update(float x, float y, float z)
        {
            coordinate = new Vector3(x, y, z);
        }

        /// <summary>
        /// Update the Coordinate using a Vector
        /// </summary>
        /// <param name="coordinate">Vector coordinate is at</param>
        public void Update(Vector3 coordinate)
        {
            this.coordinate = coordinate;
        }

        /// <summary>
        /// Update the Coordinate using a AOCoord
        /// </summary>
        /// <param name="aoCoord">AOCoord coordinate is at</param>
        public void Update(AOCoord aoCoord)
        {
            coordinate = aoCoord.coordinate;
        }
        #endregion

        #region Distance Calculations
        /// <summary>
        /// Calculate the Distance between two Coordinates in 3 Dimensions
        /// </summary>
        /// <param name="c1">Coordinate 1</param>
        /// <param name="c2">Coordinate 2</param>
        public static double Distance3D(AOCoord c1, AOCoord c2)
        {
            Vector3 difference = c1.coordinate - c2.coordinate;

            return difference.Magnitude;
        }

        /// <summary>
        /// Calculate the Distance between two Coordinates in 3 Dimensions
        /// </summary>
        /// <param name="c1">Other Coordinate</param>
        public double Distance3D(AOCoord c1)
        {
            return Distance3D(this, c1);
        }

        /// <summary>
        /// Calculate the Distance between two Coordinates in 2 Dimensions
        /// </summary>
        /// <param name="c1">Coordinate 1</param>
        /// <param name="c2">Coordinate 2</param>
        public static double Distance2D(AOCoord c1, AOCoord c2)
        {
            Vector3 difference = c1.coordinate - c2.coordinate;

            return Math.Sqrt((difference.x*difference.x) + (difference.z*difference.z));
        }

        /// <summary>
        /// Calculate the Distance between two Coordinates in 2 Dimensions
        /// </summary>
        /// <param name="c1">Other Coordinate</param>
        public double Distance2D(AOCoord c1)
        {
            return Distance2D(this, c1);
        }
        #endregion

        #region ToString
        /// <summary>
        /// Converts the coordinates to a string representation
        /// </summary>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0:F1} {2:F1} y {1:F1}", coordinate.x, coordinate.y,
                                 coordinate.z);
        }
        #endregion
    }
}