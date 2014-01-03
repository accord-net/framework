// Accord Imaging Library
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

namespace Accord.Imaging
{
    using System.Drawing;
    using System;

    /// <summary>
    ///   Represents an ordered pair of real x- and y-coordinates and scalar w that defines
    ///   a point in a two-dimensional plane using homogeneous coordinates.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In mathematics, homogeneous coordinates are a system of coordinates used in
    ///   projective geometry much as Cartesian coordinates are used in Euclidean geometry.</para>
    /// <para>
    ///   They have the advantage that the coordinates of a point, even those at infinity,
    ///   can be represented using finite coordinates. Often formulas involving homogeneous
    ///   coordinates are simpler and more symmetric than their Cartesian counterparts.</para>
    /// <para>
    ///   Homogeneous coordinates have a range of applications, including computer graphics,
    ///   where they allow affine transformations and, in general, projective transformations
    ///   to be easily represented by a matrix.</para>
    ///   
    /// <para>
    ///   References: 
    ///   <list type="bullet">
    ///     <item><description>
    ///       http://alumnus.caltech.edu/~woody/docs/3dmatrix.html</description></item>
    ///     <item><description>
    ///       http://simply3d.wordpress.com/2009/05/29/homogeneous-coordinates/</description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    public struct PointH
    {

        private float px, py, pw;

        /// <summary>
        ///   The first coordinate.
        /// </summary>
        /// 
        public float X
        {
            get { return px; }
            set { px = value; }
        }

        /// <summary>
        ///   The second coordinate.
        /// </summary>
        /// 
        public float Y
        {
            get { return py; }
            set { py = value; }
        }

        /// <summary>
        ///   The inverse scaling factor for X and Y.
        /// </summary>
        /// 
        public float W
        {
            get { return pw; }
            set { pw = value; }
        }

        /// <summary>
        ///   Creates a new point.
        /// </summary>
        /// 
        public PointH(float x, float y)
        {
            px = x;
            py = y;
            pw = 1;
        }

        /// <summary>
        ///   Creates a new point.
        /// </summary>
        /// 
        public PointH(double x, double y)
        {
            px = (float)x;
            py = (float)y;
            pw = 1;
        }

        /// <summary>
        ///   Creates a new point.
        /// </summary>
        /// 
        public PointH(float x, float y, float w)
        {
            px = x;
            py = y;
            pw = w;
        }

        /// <summary>
        ///   Creates a new point.
        /// </summary>
        /// 
        public PointH(double x, double y, double w)
        {
            px = (float)x;
            py = (float)y;
            pw = (float)w;
        }

        /// <summary>
        ///   Transforms a point using a projection matrix.
        /// </summary>
        /// 
        public void Transform(float[,] matrix)
        {
            float x = matrix[0, 0] * px + matrix[0, 1] * py + matrix[0, 2] * pw;
            float y = matrix[1, 0] * px + matrix[1, 1] * py + matrix[1, 2] * pw;
            float w = matrix[2, 0] * px + matrix[2, 1] * py + matrix[2, 2] * pw;

            px = x;
            py = y;
            pw = w;
        }

        /// <summary>
        ///   Normalizes the point to have unit scale.
        /// </summary>
        /// 
        public void Normalize()
        {
            px = px / pw;
            py = py / pw;
            pw = 1;
        }

        /// <summary>
        ///   Gets whether this point is normalized (w = 1).
        /// </summary>
        /// 
        public bool IsNormalized
        {
            get { return pw == 1f; }
        }

        /// <summary>
        ///   Gets whether this point is at infinity (w = 0).
        /// </summary>
        /// 
        public bool IsAtInfinity
        {
            get { return pw == 0f; }
        }

        /// <summary>
        ///   Gets whether this point is at the origin.
        /// </summary>
        /// 
        public bool IsEmpty
        {
            get { return px == 0 && py == 0; }
        }

        /// <summary>
        ///   Converts the point to a array representation.
        /// </summary>
        /// 
        public double[] ToArray()
        {
            return new double[] { px, py, pw };
        }

        /// <summary>
        ///   Multiplication by scalar.
        /// </summary>
        /// 
        public static PointH operator *(PointH point, float scalar)
        {
            return new PointH(scalar * point.X, scalar * point.Y, scalar * point.W);
        }

        /// <summary>
        ///   Multiplication by scalar.
        /// </summary>
        /// 
        public static PointH operator *(float scalar, PointH point)
        {
            return point * scalar;
        }

        /// <summary>
        ///   Multiplies the point by a scalar.
        /// </summary>
        /// 
        public PointH Multiply(float value)
        {
            return this * value;
        }

        /// <summary>
        ///   Subtraction.
        /// </summary>
        /// 
        public static PointH operator -(PointH left, PointH right)
        {
            return new PointH(left.X - right.X, left.Y - right.Y, left.W - right.W);
        }

        /// <summary>
        ///   Subtracts the values of two points.
        /// </summary>
        /// 
        public PointH Subtract(PointH value)
        {
            return this - value;
        }

        /// <summary>
        ///   Addition.
        /// </summary>
        /// 
        public static PointH operator +(PointH left, PointH right)
        {
            return new PointH(left.X + right.X, left.Y + right.Y, left.W + right.W);
        }

        /// <summary>
        ///   Add the values of two points.
        /// </summary>
        /// 
        public PointH Add(PointH value)
        {
            return this + value;
        }

        /// <summary>
        ///   Equality.
        /// </summary>
        /// 
        public static bool operator ==(PointH left, PointH right)
        {
            return (left.px / left.pw == right.px / right.pw && left.py / left.pw == right.py / right.pw);
        }

        /// <summary>
        ///   Inequality
        /// </summary>
        /// 
        public static bool operator !=(PointH left, PointH right)
        {
            return (left.px / left.pw != right.px / right.pw
                || left.py / left.pw != right.py / right.pw);
        }

        /// <summary>
        ///   PointF Conversion.
        /// </summary>
        /// 
        public static implicit operator PointF(PointH point)
        {
            return new PointF((float)(point.px / point.pw), (float)(point.py / point.pw));
        }

        /// <summary>
        ///   Converts to a Integer point by computing the ceiling of the point coordinates. 
        /// </summary>
        /// 
        public static Point Ceiling(PointH point)
        {
            return new Point(
                (int)System.Math.Ceiling(point.px / point.pw),
                (int)System.Math.Ceiling(point.py / point.pw));
        }

        /// <summary>
        ///   Converts to a Integer point by rounding the point coordinates. 
        /// </summary>
        /// 
        public static Point Round(PointH point)
        {
            return new Point(
                (int)System.Math.Round(point.px / point.pw),
                (int)System.Math.Round(point.py / point.pw));
        }

        /// <summary>
        ///   Converts to a Integer point by truncating the point coordinates. 
        /// </summary>
        /// 
        public static Point Truncate(PointH point)
        {
            return new Point(
                (int)System.Math.Truncate(point.px / point.pw),
                (int)System.Math.Truncate(point.py / point.pw));
        }

        /// <summary>
        ///   Compares two objects for equality.
        /// </summary>
        /// 
        public override bool Equals(object obj)
        {
            if (obj is PointH)
            {
                PointH p = (PointH)obj;
                if (px / pw == p.px / p.pw &&
                    py / pw == p.py / p.pw)
                    return true;
            }

            return false;
        }

        /// <summary>
        ///   Returns the hash code for this instance.
        /// </summary>
        /// 
        public override int GetHashCode()
        {
            return px.GetHashCode() ^ py.GetHashCode() ^ pw.GetHashCode();
        }



        /// <summary>
        ///   Returns the empty point.
        /// </summary>
        /// 
        public static readonly PointH Empty = new PointH(0, 0, 1);
    }
}
