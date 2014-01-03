// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
//
// Copyright © Peter Kovesi, 1995-2010
// Centre for Exploration Targeting
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
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using Accord.Math;
    using AForge;
    using AForge.Imaging.Filters;

    /// <summary>
    ///   Maximum cross-correlation feature point matching algorithm.
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>
    ///     This class matches feature points by using a maximum cross-correlation measure.</para>
    ///   <para>
    ///     References:
    ///     <list type="bullet">
    ///       <item><description>
    ///         P. D. Kovesi. MATLAB and Octave Functions for Computer Vision and Image Processing.
    ///         School of Computer Science and Software Engineering, The University of Western Australia.
    ///         Available in: <a href="http://www.csse.uwa.edu.au/~pk/Research/MatlabFns/Match/matchbycorrelation.m">
    ///         http://www.csse.uwa.edu.au/~pk/Research/MatlabFns/Match/matchbycorrelation.m </a>
    ///       </description></item>
    ///       <item><description>
    ///         <a href="http://www.instructor.com.br/unesp2006/premiados/PauloHenrique.pdf">
    ///         http://www.instructor.com.br/unesp2006/premiados/PauloHenrique.pdf </a>
    ///       </description></item>
    ///       <item><description>
    ///         <a href="http://siddhantahuja.wordpress.com/2010/04/11/correlation-based-similarity-measures-summary/">
    ///         http://siddhantahuja.wordpress.com/2010/04/11/correlation-based-similarity-measures-summary/ </a>
    ///       </description></item>
    ///   </list></para>
    /// </remarks>
    ///
    /// <seealso cref="RansacHomographyEstimator"/>
    ///
    public class CorrelationMatching
    {

        private int windowSize;
        private double dmax;

        /// <summary>
        ///   Gets or sets the maximum distance to consider
        ///   points as correlated.
        /// </summary>
        /// 
        public double DistanceMax
        {
            get { return dmax; }
            set { dmax = value; }
        }

        /// <summary>
        ///   Gets or sets the size of the correlation window.
        /// </summary>
        /// 
        public int WindowSize
        {
            get { return windowSize; }
            set { windowSize = value; }
        }



        /// <summary>
        ///   Constructs a new Correlation Matching algorithm.
        /// </summary>
        /// 
        public CorrelationMatching(int windowSize)
            : this(windowSize, 0)
        {
        }

        /// <summary>
        ///   Constructs a new Correlation Matching algorithm.
        /// </summary>
        /// 
        public CorrelationMatching(int windowSize, double maxDistance)
        {
            if (windowSize % 2 == 0)
                throw new ArgumentException("Window size should be odd", "windowSize");

            this.windowSize = windowSize;
            this.dmax = maxDistance;
        }



        /// <summary>
        ///   Matches two sets of feature points computed from the given images.
        /// </summary>
        /// 
        public IntPoint[][] Match(Bitmap image1, Bitmap image2,
            IntPoint[] points1, IntPoint[] points2)
        {
            // Make sure we are dealing with grayscale images.
            Bitmap grayImage1, grayImage2;
            if (image1.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                grayImage1 = image1;
            }
            else
            {
                // create temporary grayscale image
                grayImage1 = Grayscale.CommonAlgorithms.BT709.Apply(image1);
            }

            if (image2.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                grayImage2 = image2;
            }
            else
            {
                // create temporary grayscale image
                grayImage2 = Grayscale.CommonAlgorithms.BT709.Apply(image2);
            }


            // Generate correlation matrix
            double[,] correlationMatrix =
                computeCorrelationMatrix(grayImage1, points1, grayImage2, points2, windowSize, dmax);


            // Free allocated resources
            if (image1.PixelFormat != PixelFormat.Format8bppIndexed)
                grayImage1.Dispose();

            if (image2.PixelFormat != PixelFormat.Format8bppIndexed)
                grayImage2.Dispose();


            // Select points with maximum correlation measures
            int[] colp2forp1; Matrix.Max(correlationMatrix, 1, out colp2forp1);
            int[] rowp1forp2; Matrix.Max(correlationMatrix, 0, out rowp1forp2);

            // Construct the lists of matched point indices
            int rows = correlationMatrix.GetLength(0);
            List<int> p1ind = new List<int>();
            List<int> p2ind = new List<int>();

            // For each point in the first set of points,
            for (int i = 0; i < rows; i++)
            {
                // Get the point j in the second set of points with which
                // this point i has a maximum correlation measure. (i->j)
                int j = colp2forp1[i];

                // Now, check if this point j in the second set also has
                // a maximum correlation measure with the point i. (j->i)
                if (rowp1forp2[j] == i)
                {
                    // The points are consistent. Ensure they are valid.
                    if (correlationMatrix[i, j] != Double.NegativeInfinity)
                    {
                        // We have a corresponding pair (i,j)
                        p1ind.Add(i); p2ind.Add(j);
                    }
                }
            }

            // Extract matched points from original arrays
            var m1 = points1.Submatrix(p1ind.ToArray());
            var m2 = points2.Submatrix(p2ind.ToArray());

            // Create matching point pairs
            return new IntPoint[][] { m1, m2 };
        }


        /// <summary>
        ///   Constructs the correlation matrix between selected points from two images.
        /// </summary>
        /// 
        /// <remarks>
        ///   Rows correspond to points from the first image, columns correspond to points
        ///   in the second.
        /// </remarks>
        /// 
        private static double[,] computeCorrelationMatrix(
            Bitmap image1, IntPoint[] points1,
            Bitmap image2, IntPoint[] points2,
            int windowSize, double maxDistance)
        {

            // Create the initial correlation matrix
            double[,] matrix = Matrix.Create(points1.Length, points2.Length, Double.NegativeInfinity);

            // Gather some information
            int width1 = image1.Width;
            int width2 = image2.Width;
            int height1 = image1.Height;
            int height2 = image2.Height;

            int r = (windowSize - 1) / 2; //  'radius' of correlation window
            double m = maxDistance * maxDistance; // maximum considered distance
            double[,] w1 = new double[windowSize, windowSize]; // first window
            double[,] w2 = new double[windowSize, windowSize]; // second window

            // Lock the images
            BitmapData bitmapData1 = image1.LockBits(new Rectangle(0, 0, width1, height1),
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            BitmapData bitmapData2 = image2.LockBits(new Rectangle(0, 0, width2, height2),
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);

            int stride1 = bitmapData1.Stride;
            int stride2 = bitmapData2.Stride;


            // We will ignore points at the edge
            int[] idx1 = Matrix.Find(points1,
                p => p.X >= r && p.X < width1 - r &&
                     p.Y >= r && p.Y < height1 - r);

            int[] idx2 = Matrix.Find(points2,
                p => p.X >= r && p.X < width2 - r &&
                     p.Y >= r && p.Y < height2 - r);


            // For each index in the first set of points
            foreach (int n1 in idx1)
            {
                // Get the current point
                var p1 = points1[n1];

                unsafe // Create the first window for the current point
                {
                    byte* src = (byte*)bitmapData1.Scan0 + (p1.X - r) + (p1.Y - r) * stride1;

                    for (int j = 0; j < windowSize; j++)
                    {
                        for (int i = 0; i < windowSize; i++)
                            w1[i, j] = (byte)(*(src + i));
                        src += stride1;
                    }
                }

                // Normalize the window
                double sum = 0;
                for (int i = 0; i < windowSize; i++)
                    for (int j = 0; j < windowSize; j++)
                        sum += w1[i, j] * w1[i, j];
                sum = System.Math.Sqrt(sum);
                for (int i = 0; i < windowSize; i++)
                    for (int j = 0; j < windowSize; j++)
                        w1[i, j] /= sum;


                // Identify the indices of points in p2 that we need to consider.
                int[] candidates;
                if (maxDistance == 0)
                {
                    // We should consider all points
                    candidates = idx2;
                }
                else
                {
                    // We should consider points that are within
                    //  distance maxDistance apart

                    // Compute distances from the current point
                    //  to all points in the second image.
                    double[] distances = new double[idx2.Length];
                    for (int i = 0; i < idx2.Length; i++)
                    {
                        double dx = p1.X - points2[idx2[i]].X;
                        double dy = p1.Y - points2[idx2[i]].Y;
                        distances[i] = dx * dx + dy * dy;
                    }

                    candidates = idx2.Submatrix(Matrix.Find(distances, d => d < m));
                }


                // Calculate normalized correlation measure
                foreach (int n2 in candidates)
                {
                    var p2 = points2[n2];

                    unsafe // Generate window in 2nd image
                    {
                        byte* src = (byte*)bitmapData2.Scan0 + (p2.X - r) + (p2.Y - r) * stride2;

                        for (int j = 0; j < windowSize; j++)
                        {
                            for (int i = 0; i < windowSize; i++)
                                w2[i, j] = (byte)(*(src + i));
                            src += stride2;
                        }
                    }

                    double sum1 = 0, sum2 = 0;
                    for (int i = 0; i < windowSize; i++)
                    {
                        for (int j = 0; j < windowSize; j++)
                        {
                            sum1 += w1[i, j] * w2[i, j];
                            sum2 += w2[i, j] * w2[i, j];
                        }
                    }

                    matrix[n1, n2] = sum1 / Math.Sqrt(sum2);
                }
            }

            // Release the images
            image1.UnlockBits(bitmapData1);
            image2.UnlockBits(bitmapData2);

            return matrix;
        }

    }
}
