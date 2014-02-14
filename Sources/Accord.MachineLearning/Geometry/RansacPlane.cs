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
    using AForge;
    using Accord.Math.Geometry;
    using Accord.Math.Decompositions;

    /// <summary>
    ///   Robust plane estimator with RANSAC.
    /// </summary>
    /// 
    public class RansacPlane
    {
        private RANSAC<Plane> ransac;
        private int[] inliers;

        private Point3[] points;
        private double[] d2;


        /// <summary>
        ///   Gets the RANSAC estimator used.
        /// </summary>
        /// 
        public RANSAC<Plane> Ransac
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
        ///   Creates a new RANSAC 3D plane estimator.
        /// </summary>
        /// 
        /// <param name="threshold">Inlier threshold.</param>
        /// <param name="probability">Inlier probability.</param>
        /// 
        public RansacPlane(double threshold, double probability)
        {
            // Create a new RANSAC with the selected threshold
            ransac = new RANSAC<Plane>(3, threshold, probability);

            // Set RANSAC functions
            ransac.Fitting = define;
            ransac.Distances = distance;
            ransac.Degenerate = degenerate;
        }


        /// <summary>
        ///   Produces a robust estimation of the plane
        ///   passing through the given (noisy) points.
        /// </summary>
        /// 
        /// <param name="points">A set of (possibly noisy) points.</param>
        /// 
        /// <returns>The plane passing through the points.</returns>
        /// 
        public Plane Estimate(Point3[] points)
        {
            // Initial argument checks
            if (points.Length < 3)
                throw new ArgumentException("At least three points are required to fit a plane");

            this.d2 = new double[points.Length];
            this.points = points;

            // Compute RANSAC and find the inlier points
            ransac.Compute(points.Length, out inliers);

            if (inliers.Length == 0)
                return null;

            // Compute the final plane
            Plane plane = fitting(points.Submatrix(inliers));

            return plane;
        }



        private Plane define(int[] x)
        {
            Point3 p1 = points[x[0]];
            Point3 p2 = points[x[1]];
            Point3 p3 = points[x[2]];

            return Plane.FromPoints(p1, p2, p3);
        }

        private int[] distance(Plane p, double t)
        {
            for (int i = 0; i < points.Length; i++)
                d2[i] = p.DistanceToPoint(points[i]);

            return Matrix.Find(d2, z => z < t);
        }

        private bool degenerate(int[] indices)
        {
            Point3 p1 = points[indices[0]];
            Point3 p2 = points[indices[1]];
            Point3 p3 = points[indices[2]];

            return Point3.Collinear(p1, p2, p3);
        }

        static Plane fitting(Point3[] points)
        {
            // Set up constraint equations of the form  AB = 0,
            // where B is a column vector of the plane coefficients
            // in the form   b(1)*X + b(2)*Y +b(3)*Z + b(4) = 0.
            //
            // A = [XYZ' ones(npts,1)]; % Build constraint matrix
            if (points.Length < 3)
                return null;

            if (points.Length == 3)
                return Plane.FromPoints(points[0], points[1], points[2]);

            float[,] A = new float[points.Length, 4];
            for (int i = 0; i < points.Length; i++)
            {
                A[i, 0] = points[i].X;
                A[i, 1] = points[i].Y;
                A[i, 2] = points[i].Z;
                A[i, 3] = -1;
            }

            SingularValueDecompositionF svd = new SingularValueDecompositionF(A,
                computeLeftSingularVectors: false, computeRightSingularVectors: true,
                autoTranspose: true, inPlace: true);

            float[,] v = svd.RightSingularVectors;

            float a = v[0, 3];
            float b = v[1, 3];
            float c = v[2, 3];
            float d = v[3, 3];

            float norm = (float)Math.Sqrt(a * a + b * b + c * c);

            a /= norm;
            b /= norm;
            c /= norm;
            d /= norm;

            return new Plane(a, b, c, -d);
        }

    }
}
