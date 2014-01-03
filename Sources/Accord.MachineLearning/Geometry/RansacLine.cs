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
    using Accord.MachineLearning;
    using Accord.Math;
    using Accord.Math.Decompositions;
    using AForge;
    using AForge.Math.Geometry;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///   Robust line estimator with RANSAC.
    /// </summary>
    /// 
    public class RansacLine
    {
        private RANSAC<Line> ransac;
        private int[] inliers;

        private Point[] points;
        private double[] d2;


        /// <summary>
        ///   Gets the RANSAC estimator used.
        /// </summary>
        /// 
        public RANSAC<Line> Ransac
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
        ///   Creates a new RANSAC line estimator.
        /// </summary>
        /// 
        /// <param name="threshold">Inlier threshold.</param>
        /// <param name="probability">Inlier probability.</param>
        /// 
        public RansacLine(double threshold, double probability)
        {
            // Create a new RANSAC with the selected threshold
            ransac = new RANSAC<Line>(2, threshold, probability);

            // Set RANSAC functions
            ransac.Fitting = defineline;
            ransac.Distances = distance;
            ransac.Degenerate = degenerate;
        }


        /// <summary>
        ///   Produces a robust estimation of the line
        ///   passing through the given (noisy) points.
        /// </summary>
        /// 
        /// <param name="points">A set of (possibly noisy) points.</param>
        /// 
        /// <returns>The line passing through the points.</returns>
        /// 
        public Line Estimate(IEnumerable<IntPoint> points)
        {
            return Estimate(points.Select(p => new Point(p.X, p.Y)).ToArray());
        }

        /// <summary>
        ///   Produces a robust estimation of the line
        ///   passing through the given (noisy) points.
        /// </summary>
        /// 
        /// <param name="points">A set of (possibly noisy) points.</param>
        /// 
        /// <returns>The line passing through the points.</returns>
        /// 
        public Line Estimate(IEnumerable<Point> points)
        {
            return Estimate(points.ToArray());
        }

        /// <summary>
        ///   Produces a robust estimation of the line
        ///   passing through the given (noisy) points.
        /// </summary>
        /// 
        /// <param name="points">A set of (possibly noisy) points.</param>
        /// 
        /// <returns>The line passing through the points.</returns>
        /// 
        public Line Estimate(IntPoint[] points)
        {
            return Estimate(points.Apply(p => new Point(p.X, p.Y)));
        }

        /// <summary>
        ///   Produces a robust estimation of the line
        ///   passing through the given (noisy) points.
        /// </summary>
        /// 
        /// <param name="points">A set of (possibly noisy) points.</param>
        /// 
        /// <returns>The line passing through the points.</returns>
        /// 
        public Line Estimate(Point[] points)
        {
            // Initial argument checks
            if (points.Length < 2)
                throw new ArgumentException("At least two points are required to fit a line");

            this.d2 = new double[points.Length];
            this.points = points;

            // Compute RANSAC and find the inlier points
            ransac.Compute(points.Length, out inliers);

            // Compute the final line
            Line line = fitting(points.Submatrix(inliers));

            return line;
        }



        private Line defineline(int[] x)
        {
            System.Diagnostics.Debug.Assert(x.Length == 2);

            Point p1 = points[x[0]];
            Point p2 = points[x[1]];

            return Line.FromPoints(p1, p2);
        }

        private int[] distance(Line p, double t)
        {
            for (int i = 0; i < points.Length; i++)
                d2[i] = p.DistanceToPoint(points[i]);

            return Matrix.Find(d2, z => z < t);
        }

        private bool degenerate(int[] indices)
        {
            System.Diagnostics.Debug.Assert(indices.Length == 2);

            Point p1 = points[indices[0]];
            Point p2 = points[indices[1]];

            return p1 == p2;
        }

        static Line fitting(Point[] points)
        {
            if (points.Length == 2)
                return Line.FromPoints(points[0], points[1]);

            float[,] A = new float[points.Length, 3];
            for (int i = 0; i < points.Length; i++)
            {
                A[i, 0] = points[i].X;
                A[i, 1] = points[i].Y;
                A[i, 2] = 1;
            }

            SingularValueDecompositionF svd = new SingularValueDecompositionF(A,
                computeLeftSingularVectors: false, computeRightSingularVectors: true,
                autoTranspose: true, inPlace: true);

            float[,] v = svd.RightSingularVectors;

            float slope = v[2, 1];
            float intercept = v[2, 0];
            float norm = (float)Math.Sqrt(slope * slope + intercept * intercept);

            return Line.FromSlopeIntercept(slope / norm, intercept / norm);
        }
    }
}
