// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Math
{
    using System;
    using AForge.Math;

    /// <summary>
    ///   3D point structure with X, Y, and coordinates.
    /// </summary>
    /// 
    [Serializable]
    public struct Point3
    {
        [NonSerialized] // TODO: Remove when AForge releases a newer version
        Vector3 coordinates;

        /// <summary>
        ///   Creates a new <see cref="Point3"/> 
        ///   structure from the given coordinates.
        /// </summary>
        /// 
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        /// 
        public Point3(float x, float y, float z)
        {
            coordinates = new Vector3(x, y, z);
        }

        /// <summary>
        ///   Creates a new <see cref="Point3"/> 
        ///   structure from the given coordinates.
        /// </summary>
        /// 
        /// <param name="coordinates">The point coordinates.</param>
        /// 
        public Point3(Vector3 coordinates)
        {
            this.coordinates = coordinates;
        }

        /// <summary>
        ///   Gets or sets the point's X coordinate.
        /// </summary>
        /// 
        public float X
        {
            get { return coordinates.X; }
            set { coordinates.X = value; }
        }

        /// <summary>
        ///   Gets or sets the point's Y coordinate.
        /// </summary>
        /// 
        public float Y
        {
            get { return coordinates.Y; }
            set { coordinates.Y = value; }
        }

        /// <summary>
        ///   Gets or sets the point's Z coordinate.
        /// </summary>
        /// 
        public float Z
        {
            get { return coordinates.Z; }
            set { coordinates.Z = value; }
        }


        /// <summary>
        ///   Performs an implicit conversion from
        ///   <see cref="Accord.Math.Point3"/> to <see cref="AForge.Math.Vector3"/>.
        /// </summary>
        /// 
        /// <param name="point">The point to be converted.</param>
        /// 
        /// <returns>
        ///   The result of the conversion.
        /// </returns>
        /// 
        public static implicit operator Vector3(Point3 point)
        {
            return point.coordinates;
        }

        /// <summary>
        ///   Performs an implicit conversion from
        ///   <see cref="Accord.Math.Point3"/> to <see cref="Accord.Math.Point3"/>.
        /// </summary>
        /// 
        /// <param name="vector">The vector to be converted.</param>
        /// 
        /// <returns>
        ///   The result of the conversion.
        /// </returns>
        /// 
        public static implicit operator Point3(Vector3 vector)
        {
            return new Point3(vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        ///   Performs a conversion from <see cref="AForge.Math.Vector3"/>
        ///   to <see cref="Accord.Math.Point3"/>.
        /// </summary>
        /// 
        public static Point3 FromVector(Vector3 vector)
        {
            return new Point3(vector);
        }


        /// <summary>
        ///   Gets whether three points lie on the same line.
        /// </summary>
        /// 
        /// <param name="p1">The first point.</param>
        /// <param name="p2">The second point.</param>
        /// <param name="p3">The third point.</param>
        /// 
        /// <returns>True if there is a line passing through all
        ///  three points; false otherwise.</returns>
        /// 
        public static bool Collinear(Point3 p1, Point3 p2, Point3 p3)
        {
            float x1m2 = p2.X - p1.X;
            float y1m2 = p2.Y - p1.Y;
            float z1m2 = p2.Z - p1.Z;

            float x2m3 = p3.X - p1.X;
            float y2m3 = p3.Y - p1.Y;
            float z2m3 = p3.Z - p1.Z;

            float x = y1m2 * z2m3 - z1m2 * y2m3;
            float y = z1m2 * x2m3 - x1m2 * z2m3;
            float z = x1m2 * y2m3 - y1m2 * x2m3;

            float norm = x * x + y * y + z * z;

            return norm < Constants.SingleEpsilon;
        }

        /// <summary>
        ///   Gets the point at the 3D space origin: (0, 0, 0)
        /// </summary>
        /// 
        public static Point3 Origin { get { return origin; } }

        private static readonly Point3 origin = new Point3(0, 0, 0);





        /// <summary>
        ///   Implements the operator !=.
        /// </summary>
        /// 
        public static bool operator ==(Point3 a, Point3 b)
        {
            return a.coordinates == b.coordinates;
        }

        /// <summary>
        ///   Implements the operator !=.
        /// </summary>
        /// 
        public static bool operator !=(Point3 a, Point3 b)
        {
            return a.coordinates != b.coordinates;
        }

        /// <summary>
        ///   Determines whether the specified <see cref="Point3"/> is equal to this instance.
        /// </summary>
        /// 
        /// <param name="other">The <see cref="Point3"/> to compare with this instance.</param>
        /// <param name="tolerance">The acceptance tolerance threshold to consider the instances equal.</param>
        /// 
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// 
        public bool Equals(Point3 other, double tolerance)
        {
            return (Math.Abs(coordinates.X - other.coordinates.X) < tolerance)
                && (Math.Abs(coordinates.Y - other.coordinates.Y) < tolerance)
                && (Math.Abs(coordinates.Z - other.coordinates.Z) < tolerance);
        }

        /// <summary>
        ///   Determines whether the specified <see cref="Point3"/> is equal to this instance.
        /// </summary>
        /// 
        /// <param name="other">The <see cref="Point3"/> to compare with this instance.</param>
        /// 
        /// <returns>
        ///   <c>true</c> if the specified <see cref="Plane"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// 
        public bool Equals(Point3 other)
        {
            return coordinates == other.coordinates;
        }

        /// <summary>
        ///   Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// 
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// 
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// 
        public override bool Equals(object obj)
        {
            if (!(obj is Point3))
                return false;

            return Equals((Point3)obj);
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing 
        ///   algorithms and data structures like a hash table. 
        /// </returns>
        /// 
        public override int GetHashCode()
        {
            return coordinates.GetHashCode();
        }

    }
}
