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
// This work has been inspired by the original work of Peter Kovesi,
// shared under a permissive MIT license. Details are given below:
//
//   Copyright (c) 1995-2010 Peter Kovesi
//   Centre for Exploration Targeting
//   School of Earth and Environment
//   The University of Western Australia
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy
//   of this software and associated documentation files (the "Software"), to deal
//   in the Software without restriction, including without limitation the rights 
//   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
//   of the Software, and to permit persons to whom the Software is furnished to do
//   so, subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.
//   
//   The software is provided "as is", without warranty of any kind, express or
//   implied, including but not limited to the warranties of merchantability, 
//   fitness for a particular purpose and noninfringement. In no event shall the
//   authors or copyright holders be liable for any claim, damages or other liability,
//   whether in an action of contract, tort or otherwise, arising from, out of or in
//   connection with the software or the use or other dealings in the software.
//   

namespace Accord.MachineLearning.Geometry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Accord.Math;
    using Accord.Math.Geometry;
    using AForge;

    /// <summary>
    ///   Robust circle estimator with RANSAC.
    /// </summary>
    /// 
    public class RansacCircle
    {
        private RANSAC<Circle> ransac;
        private int[] inliers;

        private Point[] points;
        private double[] d2;

        /// <summary>
        ///   Gets the RANSAC estimator used.
        /// </summary>
        /// 
        public RANSAC<Circle> Ransac
        {
            get { return this.ransac; }
        }

        /// <summary>
        ///   Gets the final set of inliers detected by RANSAC.
        /// </summary>
        /// 
        public int[] Inliers
        {
            get { return inliers; }
        }

        /// <summary>
        ///   Creates a new RANSAC 2D circle estimator.
        /// </summary>
        /// 
        /// <param name="threshold">Inlier threshold.</param>
        /// <param name="probability">Inlier probability.</param>
        /// 
        public RansacCircle(double threshold, double probability)
        {
            ransac = new RANSAC<Circle>(3, threshold, probability);
            ransac.Fitting = define;
            ransac.Distances = distance;
            ransac.Degenerate = degenerate;
        }

        /// <summary>
        ///   Produces a robust estimation of the circle
        ///   passing through the given (noisy) points.
        /// </summary>
        /// 
        /// <param name="points">A set of (possibly noisy) points.</param>
        /// 
        /// <returns>The circle passing through the points.</returns>
        /// 
        public Circle Estimate(IEnumerable<IntPoint> points)
        {
            return Estimate(points.Select(p => new Point(p.X, p.Y)).ToArray());
        }

        /// <summary>
        ///   Produces a robust estimation of the circle
        ///   passing through the given (noisy) points.
        /// </summary>
        /// 
        /// <param name="points">A set of (possibly noisy) points.</param>
        /// 
        /// <returns>The circle passing through the points.</returns>
        /// 
        public Circle Estimate(IEnumerable<Point> points)
        {
            return Estimate(points.ToArray());
        }

        /// <summary>
        ///   Produces a robust estimation of the circle
        ///   passing through the given (noisy) points.
        /// </summary>
        /// 
        /// <param name="points">A set of (possibly noisy) points.</param>
        /// 
        /// <returns>The circle passing through the points.</returns>
        /// 
        public Circle Estimate(IntPoint[] points)
        {
            return Estimate(points.Apply(p => new Point(p.X, p.Y)));
        }

        /// <summary>
        ///   Produces a robust estimation of the circle
        ///   passing through the given (noisy) points.
        /// </summary>
        /// 
        /// <param name="points">A set of (possibly noisy) points.</param>
        /// 
        /// <returns>The circle passing through the points.</returns>
        /// 
        public Circle Estimate(Point[] points)
        {
            if (points.Length < 3)
                throw new ArgumentException("At least three points are required to fit a circle");

            this.d2 = new double[points.Length];
            this.points = points;

            ransac.Compute(points.Length, out inliers);

            Circle circle = fitting(points.Submatrix(inliers));

            return circle;
        }



        private Circle define(int[] x)
        {
            System.Diagnostics.Debug.Assert(x.Length == 3);
            return new Circle(points[x[0]], points[x[1]], points[x[2]]);
        }

        private int[] distance(Circle c, double t)
        {
            for (int i = 0; i < points.Length; i++)
                d2[i] = c.DistanceToPoint(points[i]);

            return Matrix.Find(d2, z => z < t);
        }

        private bool degenerate(int[] indices)
        {
            System.Diagnostics.Debug.Assert(indices.Length == 3);

            Point p1 = points[indices[0]];
            Point p2 = points[indices[1]];
            Point p3 = points[indices[2]];

            return p1 == p2 || p2 == p3 || p1 == p3;
        }

        static Circle fitting(Point[] points)
        {
            if (points.Length == 3)
                return new Circle(points[0], points[1], points[2]);

            double[,] A = new double[points.Length, 3];
            double[,] Y = new double[points.Length, 1];

            // setup the matrices
            for (int i = 0; i < points.Length; ++i)
            {
                // we solve for [ 2*c1 2*c2 c3 ] here,
                // avoid doing 2*xn / 2*yn in the loop.
                A[i, 0] = points[i].X;
                A[i, 1] = points[i].Y;
                A[i, 2] = 1;
                Y[i, 0] = points[i].X * points[i].X + points[i].Y * points[i].Y;
            }

            // get AT * A and AT * Y
            double[,] AT = A.Transpose();
            double[,] B = AT.Multiply(A);
            double[,] Z = AT.Multiply(Y);

            // solve for c
            double[,] c = Matrix.Solve(B, Z, true);

            // okay now we get the circle :-D
            double x = c[0, 0] * 0.5;
            double y = c[1, 0] * 0.5;
            double r = System.Math.Sqrt(c[2, 0] + x * x + y * y);

            return new Circle(x, y, r);
        }
    }
}
