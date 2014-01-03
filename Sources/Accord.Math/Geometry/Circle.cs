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

namespace Accord.Math.Geometry
{
    using System;
    using AForge;

    /// <summary>
    ///   2D circle class.
    /// </summary>
    /// 
    public class Circle
    {

        /// <summary>
        ///   Gets the area of the circle (πR²).
        /// </summary>
        /// 
        public double Area
        {
            get { return Radius * Radius * Math.PI; }
        }

        /// <summary>
        ///   Gets the circumference of the circle (2πR).
        /// </summary>
        /// 
        public double Circumference
        {
            get { return 2 * Radius * Math.PI; }
        }

        /// <summary>
        ///   Gets the diameter of the circle (2R).
        /// </summary>
        /// 
        public double Diameter
        {
            get { return 2 * Radius; }
        }

        /// <summary>
        ///   Gets or sets the radius for this circle.
        /// </summary>
        /// 
        public double Radius { get; set; }

        /// <summary>
        ///   Gets or sets the origin (center) of this circle.
        /// </summary>
        /// 
        public Point Origin { get; set; }

        /// <summary>
        ///   Creates a new unit <see cref="Circle"/> at the origin.
        /// </summary>
        /// 
        public Circle()
        {
            Origin = new Point(0, 0);
            Radius = 1;
        }


        /// <summary>
        ///   Creates a new <see cref="Circle"/> with the given radius 
        ///   centered at the given <c>x</c> and <c>y</c> coordinates.
        /// </summary>
        /// 
        /// <param name="x">The x-coordinate of the circle's center.</param>
        /// <param name="y">The y-coordinate of the circle's center.</param>
        /// <param name="radius">The circle radius.</param>
        /// 
        public Circle(float x, float y, double radius)
        {
            Origin = new Point(x, y);
            Radius = radius;
        }

        /// <summary>
        ///   Creates a new <see cref="Circle"/> with the given radius 
        ///   centered at the given <c>x</c> and <c>y</c> coordinates.
        /// </summary>
        /// 
        /// <param name="x">The x-coordinate of the circle's center.</param>
        /// <param name="y">The y-coordinate of the circle's center.</param>
        /// <param name="radius">The circle radius.</param>
        /// 
        public Circle(double x, double y, double radius)
        {
            Origin = new Point((float)x, (float)y);
            Radius = radius;
        }

        /// <summary>
        ///   Creates a new <see cref="Circle"/> with the given radius 
        ///   centered at the given center point coordinates.
        /// </summary>
        /// 
        /// <param name="origin">The point at the circle's center.</param>
        /// <param name="radius">The circle radius.</param>
        /// 
        public Circle(Point origin, double radius)
        {
            Origin = origin;
            Radius = radius;
        }

        /// <summary>
        ///   Creates a new <see cref="Circle"/> from three non-linear points.
        /// </summary>
        /// 
        /// <param name="p1">The first point.</param>
        /// <param name="p2">The second point.</param>
        /// <param name="p3">The third point.</param>
        /// 
        public Circle(Point p1, Point p2, Point p3)
        {
            // ya = ma * (x - x1) + y1
            // yb = mb * (x - x2) + y2
            //
            // ma = (y2 - y1) / (x2 - x1)
            // mb = (y3 - y2) / (x3 - x2)
            double ma = (p2.Y - p1.Y) / (p2.X - p1.X);
            double mb = (p3.Y - p2.Y) / (p3.X - p2.X);

            //       (ma * mb * (y1 - y3) + mb * (x1 + x2) - ma * (x2 + x3)
            // x = ----------------------------------------------------------
            //                          2 * (mb - ma)
            double x = (ma * mb * (p1.Y - p3.Y) + mb * (p1.X + p2.Y) - ma * (p2.X + p3.X)) / (2 * (mb - ma));
            double y = ma * (x - p1.X) + p1.Y;

            Origin = new Point((float)x, (float)y);
            Radius = Origin.DistanceTo(p1);
        }

        /// <summary>
        ///   Computes the distance from circle to point.
        /// </summary>
        /// 
        /// <param name="point">The point to have its distance from the circle computed.</param>
        /// 
        /// <returns>The distance from <paramref name="point"/> to this circle.</returns>
        /// 
        public double DistanceToPoint(Point point)
        {
            return Math.Abs(Origin.DistanceTo(point) - Radius);
        }
    }
}
