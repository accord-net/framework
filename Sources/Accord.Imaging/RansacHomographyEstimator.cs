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

namespace Accord.Imaging
{
    using System;
    using System.Drawing;
    using Accord.MachineLearning;
    using Accord.Math;
    using AForge;
    using Point = System.Drawing.Point;

    /// <summary>
    ///   RANSAC Robust Homography Matrix Estimator.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Fitting a homography using RANSAC is pretty straightforward. Being a iterative method,
    ///   in a single iteration a random sample of four correspondences is selected from the 
    ///   given correspondence points and a homography H is then computed from those points.</para>
    /// <para>
    ///   The original points are then transformed using this homography and their distances to
    ///   where those transforms should be is then computed and matching points can classified
    ///   as inliers and non-matching points as outliers.</para>  
    /// <para>
    ///   After a given number of iterations, the iteration which produced the largest number
    ///   of inliers is then selected as the best estimation for H.</para>  
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       E. Dubrofsky. Homography Estimation. Master thesis. Available on:
    ///       http://www.cs.ubc.ca/~dubroe/courses/MastersEssay.pdf </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    public class RansacHomographyEstimator
    {
        private RANSAC<MatrixH> ransac;
        private int[] inliers;

        private PointF[] pointSet1;
        private PointF[] pointSet2;
        double[] d2;


        /// <summary>
        ///   Gets the RANSAC estimator used.
        /// </summary>
        /// 
        public RANSAC<MatrixH> Ransac
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
        ///   Creates a new RANSAC homography estimator.
        /// </summary>
        /// 
        /// <param name="threshold">Inlier threshold.</param>
        /// <param name="probability">Inlier probability.</param>
        /// 
        public RansacHomographyEstimator(double threshold, double probability)
        {
            // Create a new RANSAC with the selected threshold
            ransac = new RANSAC<MatrixH>(4, threshold, probability);

            // Set RANSAC functions
            ransac.Fitting = homography;
            ransac.Degenerate = degenerate;
            ransac.Distances = distance;
        }

        /// <summary>
        ///   Matches two sets of points using RANSAC.
        /// </summary>
        /// 
        /// <returns>The homography matrix matching x1 and x2.</returns>
        /// 
        public MatrixH Estimate(AForge.Point[] points1, AForge.Point[] points2)
        {
            // Initial argument checks
            if (points1.Length != points2.Length)
                throw new ArgumentException("The number of points should be equal.");

            if (points1.Length < 4)
                throw new ArgumentException("At least four points are required to fit an homography");

            PointF[] p1 = new PointF[points1.Length];
            PointF[] p2 = new PointF[points2.Length];
            for (int i = 0; i < points1.Length; i++)
            {
                p1[i] = new PointF(points1[i].X, points1[i].Y);
                p2[i] = new PointF(points2[i].X, points2[i].Y);
            }

            return Estimate(p1, p2);
        }

        /// <summary>
        ///   Matches two sets of points using RANSAC.
        /// </summary>
        /// 
        /// <returns>The homography matrix matching x1 and x2.</returns>
        /// 
        public MatrixH Estimate(IntPoint[] points1, IntPoint[] points2)
        {
            // Initial argument checks
            if (points1.Length != points2.Length)
                throw new ArgumentException("The number of points should be equal.");

            if (points1.Length < 4)
                throw new ArgumentException("At least four points are required to fit an homography");

            PointF[] p1 = new PointF[points1.Length];
            PointF[] p2 = new PointF[points2.Length];
            for (int i = 0; i < points1.Length; i++)
            {
                p1[i] = new PointF(points1[i].X, points1[i].Y);
                p2[i] = new PointF(points2[i].X, points2[i].Y);
            }

            return Estimate(p1, p2);
        }

        /// <summary>
        ///   Matches two sets of points using RANSAC.
        /// </summary>
        /// 
        /// <returns>The homography matrix matching x1 and x2.</returns>
        /// 
        public MatrixH Estimate(PointF[] points1, PointF[] points2)
        {
            // Initial argument checks
            if (points1.Length != points2.Length)
                throw new ArgumentException("The number of points should be equal.");

            if (points1.Length < 4)
                throw new ArgumentException("At least four points are required to fit an homography");


            // Normalize each set of points so that the origin is
            //  at centroid and mean distance from origin is sqrt(2).
            MatrixH T1, T2;
            this.pointSet1 = Tools.Normalize(points1, out T1);
            this.pointSet2 = Tools.Normalize(points2, out T2);
            d2 = new double[points1.Length];


            // Compute RANSAC and find the inlier points
            MatrixH H = ransac.Compute(points1.Length, out inliers);

            if (inliers == null || inliers.Length < 4)
                //throw new Exception("RANSAC could not find enough points to fit an homography.");
                return null;


            // Compute the final homography considering all inliers
            H = homography(inliers);

            // Denormalize
            H = T2.Inverse() * (H * T1);

            return H;
        }

        /// <summary>
        ///   Estimates a homography with the given points.
        /// </summary>
        /// 
        private MatrixH homography(int[] points)
        {
            // Retrieve the original points
            PointF[] x1 = this.pointSet1.Submatrix(points);
            PointF[] x2 = this.pointSet2.Submatrix(points);

            // Compute the homography
            return Tools.Homography(x1, x2);
        }

        /// <summary>
        ///   Compute inliers using the Symmetric Transfer Error,
        /// </summary>
        /// 
        private int[] distance(MatrixH H, double t)
        {
            // Compute the projections (both directions)
            PointF[] p1 = H.TransformPoints(pointSet1);
            PointF[] p2 = H.Inverse().TransformPoints(pointSet2);

            // Compute the distances
            for (int i = 0; i < pointSet1.Length; i++)
            {
                // Compute the distance as
                float ax = pointSet1[i].X - p2[i].X;
                float ay = pointSet1[i].Y - p2[i].Y;
                float bx = pointSet2[i].X - p1[i].X;
                float by = pointSet2[i].Y - p1[i].Y;
                d2[i] = (ax * ax) + (ay * ay) + (bx * bx) + (by * by);
            }

            // Find and return the inliers
            return Matrix.Find(d2, z => z < t);
        }

        /// <summary>
        ///   Checks if the selected points will result in a degenerate homography.
        /// </summary>
        /// 
        private bool degenerate(int[] points)
        {
            PointF[] x1 = this.pointSet1.Submatrix(points);
            PointF[] x2 = this.pointSet2.Submatrix(points);

            // If any three of the four points in each set is collinear,
            //  the resulting homography matrix will be degenerate.

            return Tools.Collinear(x1[0], x1[1], x1[2]) ||
                   Tools.Collinear(x1[0], x1[1], x1[3]) ||
                   Tools.Collinear(x1[0], x1[2], x1[3]) ||
                   Tools.Collinear(x1[1], x1[2], x1[3]) ||

                   Tools.Collinear(x2[0], x2[1], x2[2]) ||
                   Tools.Collinear(x2[0], x2[1], x2[3]) ||
                   Tools.Collinear(x2[0], x2[2], x2[3]) ||
                   Tools.Collinear(x2[1], x2[2], x2[3]);
        }

    }
}
