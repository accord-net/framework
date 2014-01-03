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
// Some functions adapted from the original work of Peter Kovesi,
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
    using System.Drawing.Imaging;
    using Accord.Math;
    using Accord.Math.Decompositions;
    using AForge.Imaging;
    using Accord.Math.Geometry;
    using AForge.Math;

    /// <summary>
    ///   Static tool functions for imaging.
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>
    ///     References:
    ///     <list type="bullet">
    ///       <item><description>
    ///         P. D. Kovesi. MATLAB and Octave Functions for Computer Vision and Image Processing.
    ///         School of Computer Science and Software Engineering, The University of Western Australia.
    ///         Available in: <a href="http://www.csse.uwa.edu.au/~pk/Research/MatlabFns/Match/matchbycorrelation.m">
    ///         http://www.csse.uwa.edu.au/~pk/Research/MatlabFns/Match/matchbycorrelation.m </a>
    ///       </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    public static class Tools
    {

        private const double SQRT2 = 1.4142135623730951;


        #region Algebra and geometry tools

        /// <summary>
        ///   Computes the center of a given rectangle.
        /// </summary>
        public static Point Center(this Rectangle rectangle)
        {
            return new Point(
                (int)(rectangle.X + rectangle.Width / 2f),
                (int)(rectangle.Y + rectangle.Height / 2f));
        }

        /// <summary>
        ///   Compares two rectangles for equality, considering an acceptance threshold.
        /// </summary>
        public static bool IsEqual(this Rectangle objA, Rectangle objB, int threshold)
        {
            return (Math.Abs(objA.X - objB.X) < threshold) &&
                   (Math.Abs(objA.Y - objB.Y) < threshold) &&
                   (Math.Abs(objA.Width - objB.Width) < threshold) &&
                   (Math.Abs(objA.Height - objB.Height) < threshold);
        }


        #region Homography Matrix
        /// <summary>
        ///   Creates an homography matrix matching points
        ///   from a set of points to another.
        /// </summary>
        public static MatrixH Homography(PointH[] points1, PointH[] points2)
        {
            // Initial argument checks
            if (points1.Length != points2.Length)
                throw new ArgumentException("The number of points should be equal.");

            if (points1.Length < 4)
                throw new ArgumentException("At least four points are required to fit an homography");


            int N = points1.Length;

            MatrixH T1, T2; // Normalize input points
            points1 = Tools.Normalize(points1, out T1);
            points2 = Tools.Normalize(points2, out T2);

            // Create the matrix A
            float[,] A = new float[3 * N, 9];
            for (int i = 0; i < N; i++)
            {
                PointH X = points1[i];
                float x = points2[i].X;
                float y = points2[i].Y;
                float w = points2[i].W;
                int r = 3 * i;

                A[r, 0] = 0;
                A[r, 1] = 0;
                A[r, 2] = 0;
                A[r, 3] = -w * X.X;
                A[r, 4] = -w * X.Y;
                A[r, 5] = -w * X.W;
                A[r, 6] = y * X.X;
                A[r, 7] = y * X.Y;
                A[r, 8] = y * X.W;

                r++;
                A[r, 0] = w * X.X;
                A[r, 1] = w * X.Y;
                A[r, 2] = w * X.W;
                A[r, 3] = 0;
                A[r, 4] = 0;
                A[r, 5] = 0;
                A[r, 6] = -x * X.X;
                A[r, 7] = -x * X.Y;
                A[r, 8] = -x * X.W;

                r++;
                A[r, 0] = -y * X.X;
                A[r, 1] = -y * X.Y;
                A[r, 2] = -y * X.W;
                A[r, 3] = x * X.X;
                A[r, 4] = x * X.Y;
                A[r, 5] = x * X.W;
                A[r, 6] = 0;
                A[r, 7] = 0;
                A[r, 8] = 0;
            }


            // Create the singular value decomposition
            var svd = new SingularValueDecompositionF(A,
                computeLeftSingularVectors: false, computeRightSingularVectors: true,
                autoTranspose: false, inPlace: true);

            float[,] V = svd.RightSingularVectors;


            // Extract the homography matrix
            MatrixH H = new MatrixH(V[0, 8], V[1, 8], V[2, 8],
                                    V[3, 8], V[4, 8], V[5, 8],
                                    V[6, 8], V[7, 8], V[8, 8]);

            // Denormalize
            H = T2.Inverse().Multiply(H.Multiply(T1));

            return H;
        }

        /// <summary>
        ///   Creates an homography matrix matching points
        ///   from a set of points to another.
        /// </summary>
        /// 
        public static MatrixH Homography(PointF[] points1, PointF[] points2)
        {
            // Initial argument checks
            if (points1.Length != points2.Length)
                throw new ArgumentException("The number of points should be equal.");

            if (points1.Length < 4)
                throw new ArgumentException("At least four points are required to fit an homography");


            int N = points1.Length;

            MatrixH T1, T2; // Normalize input points
            points1 = Tools.Normalize(points1, out T1);
            points2 = Tools.Normalize(points2, out T2);

            // Create the matrix A
            var A = new float[3 * N, 9];
            for (int i = 0; i < N; i++)
            {
                PointF X = points1[i];
                float x = points2[i].X;
                float y = points2[i].Y;
                int r = 3 * i;

                A[r, 0] = 0;
                A[r, 1] = 0;
                A[r, 2] = 0;
                A[r, 3] = -X.X;
                A[r, 4] = -X.Y;
                A[r, 5] = -1;
                A[r, 6] = y * X.X;
                A[r, 7] = y * X.Y;
                A[r, 8] = y;

                r++;
                A[r, 0] = X.X;
                A[r, 1] = X.Y;
                A[r, 2] = 1;
                A[r, 3] = 0;
                A[r, 4] = 0;
                A[r, 5] = 0;
                A[r, 6] = -x * X.X;
                A[r, 7] = -x * X.Y;
                A[r, 8] = -x;

                r++;
                A[r, 0] = -y * X.X;
                A[r, 1] = -y * X.Y;
                A[r, 2] = -y;
                A[r, 3] = x * X.X;
                A[r, 4] = x * X.Y;
                A[r, 5] = x;
                A[r, 6] = 0;
                A[r, 7] = 0;
                A[r, 8] = 0;
            }


            // Create the singular value decomposition
            SingularValueDecompositionF svd = new SingularValueDecompositionF(A,
                computeLeftSingularVectors: false, computeRightSingularVectors: true,
                autoTranspose: false, inPlace: true);

            float[,] V = svd.RightSingularVectors;


            // Extract the homography matrix
            MatrixH H = new MatrixH(V[0, 8], V[1, 8], V[2, 8],
                                    V[3, 8], V[4, 8], V[5, 8],
                                    V[6, 8], V[7, 8], V[8, 8]);

            // Denormalize
            H = T2.Inverse().Multiply(H.Multiply(T1));

            return H;
        }
        #endregion


        #region Fundamental Matrix
        /// <summary>
        ///   Creates the fundamental matrix between two
        ///   images from a set of points from each image.
        /// </summary>
        /// 
        public static float[,] Fundamental(PointH[] points1, PointH[] points2, out PointH[] epipoles)
        {
            var F = Fundamental(points1, points2);

            SingularValueDecompositionF svd = new SingularValueDecompositionF(F,
                computeLeftSingularVectors: true, computeRightSingularVectors: true,
                autoTranspose: true, inPlace: false);

            var U = svd.LeftSingularVectors;
            var V = svd.RightSingularVectors;

            PointH e1 = new PointH(V[0, 2] / V[2, 2], V[1, 2] / V[2, 2], 1);
            PointH e2 = new PointH(U[0, 2] / U[2, 2], U[1, 2] / U[2, 2], 1);

            epipoles = new PointH[] { e1, e2 };

            return F;
        }

        /// <summary>
        ///   Creates the fundamental matrix between two
        ///   images from a set of points from each image.
        /// </summary>
        /// 
        public static float[,] Fundamental(PointH[] points1, PointH[] points2)
        {
            int N = points1.Length;

            float[,] T1, T2; // Normalize input points
            points1 = Tools.Normalize(points1, out T1);
            points2 = Tools.Normalize(points2, out T2);


            float[,] A = new float[N, 9];
            for (int i = 0; i < N; i++)
            {
                float x1 = points1[i].X;
                float y1 = points1[i].Y;

                float x2 = points2[i].X;
                float y2 = points2[i].Y;

                A[i, 0] = x2 * x1;
                A[i, 1] = x2 * y1;
                A[i, 2] = x2;

                A[i, 3] = y2 * x1;
                A[i, 4] = y2 * y2;
                A[i, 5] = y2;

                A[i, 6] = x1;
                A[i, 7] = y1;
                A[i, 8] = 1;
            }

            float[,] F = createFundamentalMatrix(A);

            // Denormalize
            F = T2.Transpose().Multiply(F.Multiply(T1));

            return F;
        }

        /// <summary>
        ///   Creates the fundamental matrix between two
        ///   images from a set of points from each image.
        /// </summary>
        /// 
        public static float[,] Fundamental(PointF[] points1, PointF[] points2)
        {
            int N = points1.Length;

            float[,] T1, T2; // Normalize input points
            points1 = Tools.Normalize(points1, out T1);
            points2 = Tools.Normalize(points2, out T2);


            float[,] A = new float[N, 9];
            for (int i = 0; i < N; i++)
            {
                float x1 = points1[i].X;
                float y1 = points1[i].Y;

                float x2 = points2[i].X;
                float y2 = points2[i].Y;

                A[i, 0] = x2 * x1;
                A[i, 1] = x2 * y1;
                A[i, 2] = x2;

                A[i, 3] = y2 * x1;
                A[i, 4] = y2 * y2;
                A[i, 5] = y2;

                A[i, 6] = x1;
                A[i, 7] = y1;
                A[i, 8] = 1;
            }

            float[,] F = createFundamentalMatrix(A);

            // Denormalize
            F = T2.Transpose().Multiply(F.Multiply(T1));

            return F;
        }

        private static float[,] createFundamentalMatrix(float[,] A)
        {
            float[,] U, V;
            float[] D;

            SingularValueDecompositionF svd = new SingularValueDecompositionF(A,
                computeLeftSingularVectors: false, computeRightSingularVectors: true,
                autoTranspose: true, inPlace: true);

            V = svd.RightSingularVectors;

            int s = svd.RightSingularVectors.GetLength(1) - 1;

            float[,] F = 
            {
                { V[0, s], V[1, s], V[2, s] },
                { V[3, s], V[4, s], V[5, s] },
                { V[6, s], V[7, s], V[8, s] },
            };

            svd = new SingularValueDecompositionF(F,
                computeLeftSingularVectors: true, computeRightSingularVectors: true,
                autoTranspose: true, inPlace: false);

            U = svd.LeftSingularVectors;
            D = svd.Diagonal;
            V = svd.RightSingularVectors;

            D[2] = 0;

            // Reconstruct with rank 2 approximation
            var newF = U.MultiplyByDiagonal(D).Multiply(V.Transpose());

            F = newF;
            return F;
        }
        #endregion


        /// <summary>
        ///   Normalizes a set of homogeneous points so that the origin is located
        ///   at the centroid and the mean distance to the origin is sqrt(2).
        /// </summary>
        public static PointH[] Normalize(this PointH[] points, out MatrixH transformation)
        {
            float[,] H;
            var result = Normalize(points, out H);
            transformation = new MatrixH(H);
            return result;
        }

        /// <summary>
        ///   Normalizes a set of homogeneous points so that the origin is located
        ///   at the centroid and the mean distance to the origin is sqrt(2).
        /// </summary>
        public static PointF[] Normalize(this PointF[] points, out MatrixH transformation)
        {
            float[,] H;
            var result = Normalize(points, out H);
            transformation = new MatrixH(H);
            return result;
        }

        /// <summary>
        ///   Normalizes a set of homogeneous points so that the origin is located
        ///   at the centroid and the mean distance to the origin is sqrt(2).
        /// </summary>
        public static PointF[] Normalize(this PointF[] points, out float[,] transformation)
        {
            float n = points.Length;
            float xmean = 0, ymean = 0;
            for (int i = 0; i < points.Length; i++)
            {
                xmean += points[i].X;
                ymean += points[i].Y;
            }
            xmean /= n; ymean /= n;


            float scale = 0;
            for (int i = 0; i < points.Length; i++)
            {
                float x = points[i].X - xmean;
                float y = points[i].Y - ymean;

                scale += (float)System.Math.Sqrt(x * x + y * y);
            }

            scale = (float)(SQRT2 * n / scale);


            transformation = new float[,]
            {
               { scale,       0,  -scale * xmean },
               { 0,       scale,  -scale * ymean },
               { 0,           0,            1    }
            };

            return new MatrixH(transformation).TransformPoints(points);
        }

        /// <summary>
        ///   Normalizes a set of homogeneous points so that the origin is located
        ///   at the centroid and the mean distance to the origin is sqrt(2).
        /// </summary>
        public static PointH[] Normalize(this PointH[] points, out float[,] transformation)
        {
            float n = points.Length;
            float xmean = 0, ymean = 0;
            for (int i = 0; i < points.Length; i++)
            {
                points[i].X = points[i].X / points[i].W;
                points[i].Y = points[i].Y / points[i].W;
                points[i].W = 1;

                xmean += points[i].X;
                ymean += points[i].Y;
            }
            xmean /= n; ymean /= n;


            float scale = 0;
            for (int i = 0; i < points.Length; i++)
            {
                float x = points[i].X - xmean;
                float y = points[i].Y - ymean;

                scale += (float)System.Math.Sqrt(x * x + y * y);
            }

            scale = (float)(SQRT2 * n / scale);


            transformation = new float[,]
            {
               { scale,       0,  -scale * xmean },
               { 0,       scale,  -scale * ymean },
               { 0,           0,            1    }
            };

            return new MatrixH(transformation).TransformPoints(points);
        }

        /// <summary>
        ///   Detects if three points are collinear.
        /// </summary>
        /// 
        public static bool Collinear(PointF pt1, PointF pt2, PointF pt3)
        {
            return Math.Abs(
                 (pt1.Y - pt2.Y) * pt3.X +
                 (pt2.X - pt1.X) * pt3.Y +
                 (pt1.X * pt2.Y - pt1.Y * pt2.X)) < Constants.SingleEpsilon;
        }

        /// <summary>
        ///   Detects if three points are collinear.
        /// </summary>
        /// 
        public static bool Collinear(PointH pt1, PointH pt2, PointH pt3)
        {
            return Math.Abs(
             (pt1.Y * pt2.W - pt1.W * pt2.Y) * pt3.X +
             (pt1.W * pt2.X - pt1.X * pt2.W) * pt3.Y +
             (pt1.X * pt2.Y - pt1.Y * pt2.X) * pt3.W) < Constants.SingleEpsilon;
        }

       
        #endregion


        #region Image tools

        #region Sum


        /// <summary>
        ///   Computes the sum of the pixels in a given image.
        /// </summary>
        public static int Sum(this Bitmap image)
        {
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, image.PixelFormat);

            int sum = Sum(data);

            image.UnlockBits(data);

            return sum;
        }

        /// <summary>
        ///   Computes the sum of the pixels in a given image.
        /// </summary>
        /// 
        public unsafe static int Sum(this BitmapData image)
        {
            return Sum(new UnmanagedImage(image));
        }

        /// <summary>
        ///   Computes the sum of the pixels in a given image.
        /// </summary>
        public unsafe static int Sum(this UnmanagedImage image)
        {
            if (image.PixelFormat != PixelFormat.Format8bppIndexed &&
                image.PixelFormat != PixelFormat.Format16bppGrayScale)
                throw new UnsupportedImageFormatException("Only grayscale images are supported");

            int width = image.Width;
            int height = image.Height;
            int offset = image.Stride - image.Width;

            int sum = 0;

            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                byte* src = (byte*)image.ImageData.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, src++)
                        sum += (*src);
                    src += offset;
                }
            }
            else
            {
                short* src = (short*)image.ImageData.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, src++)
                        sum += (*src);
                    src += offset;
                }
            }

            return sum;
        }



        /// <summary>
        ///   Computes the sum of the pixels in a given image.
        /// </summary>
        public static long Sum(this Bitmap image, Rectangle rectangle)
        {
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, image.PixelFormat);

            int sum = Sum(data, rectangle);

            image.UnlockBits(data);

            return sum;
        }

        /// <summary>
        ///   Computes the sum of the pixels in a given image.
        /// </summary>
        public static int Sum(this BitmapData image, Rectangle rectangle)
        {
            return Sum(new UnmanagedImage(image), rectangle);
        }

        /// <summary>
        ///   Computes the sum of the pixels in a given image.
        /// </summary>
        public unsafe static int Sum(this UnmanagedImage image, Rectangle rectangle)
        {
            if ((image.PixelFormat != PixelFormat.Format8bppIndexed) &&
                (image.PixelFormat != PixelFormat.Format16bppGrayScale))
                throw new UnsupportedImageFormatException("Only grayscale images are supported");

            int width = image.Width;
            int height = image.Height;
            int stride = image.Stride;
            int offset = image.Stride - image.Width;

            int rwidth = rectangle.Width;
            int rheight = rectangle.Height;
            int rx = rectangle.X;
            int ry = rectangle.Y;
            int sum = 0;

            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                byte* src = (byte*)image.ImageData.ToPointer();

                for (int y = 0; y < rheight; y++)
                {
                    byte* p = src + stride * (ry + y) + rx;

                    for (int x = 0; x < rwidth; x++)
                        sum += (*p++);
                }
            }
            else
            {
                ushort* src = (ushort*)image.ImageData.ToPointer();

                for (int y = 0; y < rheight; y++)
                {
                    ushort* p = src + stride * (ry + y) + rx;

                    for (int x = 0; x < rwidth; x++)
                        sum += (*p++);
                }
            }

            return sum;
        }


        #endregion

        #region Mean
        /// <summary>
        ///   Computes the arithmetic mean of the pixels in a given image.
        /// </summary>
        /// 
        public static double Mean(this Bitmap image, Rectangle rectangle)
        {
            return (double)Sum(image, rectangle) / (rectangle.Width * rectangle.Height);
        }

        /// <summary>
        ///   Computes the arithmetic mean of the pixels in a given image.
        /// </summary>
        /// 
        public static double Mean(this BitmapData image, Rectangle rectangle)
        {
            return (double)Sum(image, rectangle) / (rectangle.Width * rectangle.Height);
        }

        /// <summary>
        ///   Computes the arithmetic mean of the pixels in a given image.
        /// </summary>
        /// 
        public static double Mean(this UnmanagedImage image, Rectangle rectangle)
        {
            return (double)Sum(image, rectangle) / (rectangle.Width * rectangle.Height);
        }

        /// <summary>
        ///   Computes the arithmetic mean of the pixels in a given image.
        /// </summary>
        /// 
        public static double Mean(this Bitmap image)
        {
            return (double)Sum(image) / (image.Width * image.Height);
        }

        /// <summary>
        ///   Computes the arithmetic mean of the pixels in a given image.
        /// </summary>
        /// 
        public static double Mean(this BitmapData image)
        {
            return (double)Sum(image) / (image.Width * image.Height);
        }

        /// <summary>
        ///   Computes the arithmetic mean of the pixels in a given image.
        /// </summary>
        /// 
        public static double Mean(this UnmanagedImage image)
        {
            return (double)Sum(image) / (image.Width * image.Height);
        }

        /// <summary>
        ///   Computes the standard deviation of image pixels.
        /// </summary>
        /// 
        public static double StandardDeviation(this Bitmap image, double mean)
        {
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                       ImageLockMode.ReadOnly, image.PixelFormat);

            double dev = StandardDeviation(data, mean);

            image.UnlockBits(data);

            return dev;
        }

        /// <summary>
        ///   Computes the standard deviation of image pixels.
        /// </summary>
        /// 
        public static double StandardDeviation(this BitmapData image, double mean)
        {
            return StandardDeviation(new UnmanagedImage(image), mean);
        }

        /// <summary>
        ///   Computes the standard deviation of image pixels.
        /// </summary>
        /// 
        public unsafe static double StandardDeviation(this UnmanagedImage image, double mean)
        {
            if (image.PixelFormat != PixelFormat.Format8bppIndexed &&
                image.PixelFormat != PixelFormat.Format16bppGrayScale)
                throw new UnsupportedImageFormatException("Only grayscale images are supported");

            int width = image.Width;
            int height = image.Height;
            int offset = image.Stride - image.Width;

            double sum = 0;

            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                byte* src = (byte*)image.ImageData.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, src++)
                    {
                        double u = (*src) - mean;
                        sum += u * u;
                    }
                    src += offset;
                }
            }
            else
            {
                short* src = (short*)image.ImageData.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, src++)
                    {
                        double u = (*src) - mean;
                        sum += u * u;
                    }
                    src += offset;
                }
            }

            return Math.Sqrt(sum / (width * height - 1));
        }

        /// <summary>
        ///   Computes the standard deviation of image pixels.
        /// </summary>
        /// 
        public static double StandardDeviation(this Bitmap image, Rectangle rectangle, double mean)
        {
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                       ImageLockMode.ReadOnly, image.PixelFormat);

            double dev = StandardDeviation(data, rectangle, mean);

            image.UnlockBits(data);

            return dev;
        }

        /// <summary>
        ///   Computes the standard deviation of image pixels.
        /// </summary>
        /// 
        public static double StandardDeviation(this BitmapData image, Rectangle rectangle, double mean)
        {
            return StandardDeviation(new UnmanagedImage(image), rectangle, mean);
        }

        /// <summary>
        ///   Computes the standard deviation of image pixels.
        /// </summary>
        /// 
        public unsafe static double StandardDeviation(this UnmanagedImage image, Rectangle rectangle, double mean)
        {
            if (image.PixelFormat != PixelFormat.Format8bppIndexed &&
                image.PixelFormat != PixelFormat.Format16bppGrayScale)
                throw new UnsupportedImageFormatException("Only grayscale images are supported");

            int width = image.Width;
            int height = image.Height;
            int stride = image.Stride;
            int offset = image.Stride - image.Width;

            int rwidth = rectangle.Width;
            int rheight = rectangle.Height;
            int rx = rectangle.X;
            int ry = rectangle.Y;

            double sum = 0;

            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                byte* src = (byte*)image.ImageData.ToPointer();

                for (int y = 0; y < rheight; y++)
                {
                    byte* p = src + stride * (ry + y) + rx;

                    for (int x = 0; x < rwidth; x++)
                    {
                        double u = (*p++) - mean;
                        sum += u * u;
                    }
                    src += offset;
                }
            }
            else
            {
                short* src = (short*)image.ImageData.ToPointer();

                for (int y = 0; y < rheight; y++)
                {
                    short* p = src + stride * (ry + y) + rx;

                    for (int x = 0; x < rwidth; x++)
                    {
                        double u = (*p++) - mean;
                        sum += u * u;
                    }
                    src += offset;
                }
            }

            return Math.Sqrt(sum / (rwidth * rheight - 1));
        }
        #endregion

        #region Maximum & Minimum
        /// <summary>
        ///   Computes the maximum pixel value in the given image.
        /// </summary>
        /// 
        public static int Max(this BitmapData image, Rectangle rectangle)
        {
            if ((image.PixelFormat != PixelFormat.Format8bppIndexed) &&
                (image.PixelFormat != PixelFormat.Format16bppGrayScale))
                throw new UnsupportedImageFormatException("Only grayscale images are supported");

            int width = image.Width;
            int height = image.Height;
            int stride = image.Stride;
            int offset = image.Stride - image.Width;

            int rwidth = rectangle.Width;
            int rheight = rectangle.Height;
            int rx = rectangle.X;
            int ry = rectangle.Y;

            int max = 0;

            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                unsafe
                {
                    byte* src = (byte*)image.Scan0.ToPointer();

                    for (int y = 0; y < rheight; y++)
                    {
                        byte* p = src + stride * (ry + y) + rx;

                        for (int x = 0; x < rwidth; x++, p++)
                            if (*p > max) max = *p;
                    }
                }
            }
            else
            {
                unsafe
                {
                    ushort* src = (ushort*)image.Scan0.ToPointer();

                    for (int y = 0; y < rheight; y++)
                    {
                        ushort* p = src + stride * (ry + y) + rx;

                        for (int x = 0; x < rwidth; x++, p++)
                            if (*p > max) max = *p;
                    }
                }
            }

            return max;
        }

        /// <summary>
        ///   Computes the maximum pixel value in the given image.
        /// </summary>
        /// 
        public unsafe static int Max(this UnmanagedImage image, Rectangle rectangle)
        {
            if ((image.PixelFormat != PixelFormat.Format8bppIndexed) &&
                (image.PixelFormat != PixelFormat.Format16bppGrayScale))
                throw new UnsupportedImageFormatException("Only grayscale images are supported");

            int width = image.Width;
            int height = image.Height;
            int stride = image.Stride;
            int offset = image.Stride - image.Width;

            int rwidth = rectangle.Width;
            int rheight = rectangle.Height;
            int rx = rectangle.X;
            int ry = rectangle.Y;

            int max = 0;

            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                byte* src = (byte*)image.ImageData.ToPointer();

                for (int y = 0; y < rheight; y++)
                {
                    byte* p = src + stride * (ry + y) + rx;

                    for (int x = 0; x < rwidth; x++, p++)
                        if (*p > max) max = *p;
                }
            }
            else
            {
                ushort* src = (ushort*)image.ImageData.ToPointer();

                for (int y = 0; y < rheight; y++)
                {
                    ushort* p = src + 2 * stride * (ry + y) + 2 * rx;

                    for (int x = 0; x < rwidth; x++, p++)
                        if (*p > max) max = *p;
                }
            }

            return max;
        }

        /// <summary>
        ///   Computes the maximum pixel value in the given image.
        /// </summary>
        /// 
        public unsafe static int Max(this UnmanagedImage image)
        {
            if ((image.PixelFormat != PixelFormat.Format8bppIndexed) &&
                (image.PixelFormat != PixelFormat.Format16bppGrayScale))
                throw new UnsupportedImageFormatException("Only grayscale images are supported");

            int width = image.Width;
            int height = image.Height;
            int stride = image.Stride;
            int offset = image.Stride - image.Width;

            int max = 0;

            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                byte* src = (byte*)image.ImageData.ToPointer();

                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++, src++)
                        if (*src > max) max = *src;
            }
            else
            {
                ushort* src = (ushort*)image.ImageData.ToPointer();

                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++, src++)
                        if (*src > max) max = *src;
            }

            return max;
        }

        /// <summary>
        ///   Computes the maximum pixel value in the given image.
        /// </summary>
        /// 
        public unsafe static int Max(this UnmanagedImage image, int channel)
        {
            if ((image.PixelFormat != PixelFormat.Format32bppArgb) &&
                (image.PixelFormat != PixelFormat.Format24bppRgb))
                throw new UnsupportedImageFormatException("Only 32 and 24-bit images are supported");

            int width = image.Width;
            int height = image.Height;
            int stride = image.Stride;
            int offset = image.Stride - image.Width;
            int pixelSize = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;

            int max = 0;

            byte* src = (byte*)image.ImageData.ToPointer() + channel;

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++, src += pixelSize)
                    if (*src > max) max = *src;


            return max;
        }

        /// <summary>
        ///   Computes the maximum pixel value in the given image.
        /// </summary>
        /// 
        public unsafe static int Min(this UnmanagedImage image, int channel)
        {
            if ((image.PixelFormat != PixelFormat.Format32bppArgb) &&
                (image.PixelFormat != PixelFormat.Format24bppRgb))
                throw new UnsupportedImageFormatException("Only 32 and 24-bit images are supported");

            int width = image.Width;
            int height = image.Height;
            int stride = image.Stride;
            int offset = image.Stride - image.Width;
            int pixelSize = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;

            int min = int.MaxValue;

            byte* src = (byte*)image.ImageData.ToPointer() + channel;

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++, src += pixelSize)
                    if (*src < min) min = *src;


            return min;
        }

        /// <summary>
        ///   Computes the maximum pixel value in the given image.
        /// </summary>
        /// 
        public static int Max(this Bitmap image)
        {
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, image.PixelFormat);

            int max = Max(data, new Rectangle(0, 0, image.Width, image.Height));

            image.UnlockBits(data);

            return max;
        }

        /// <summary>
        ///   Computes the maximum pixel value in the given image.
        /// </summary>
        /// 
        public static int Max(this Bitmap image, int channel)
        {
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, image.PixelFormat);

            int max = Max(new UnmanagedImage(data), channel);

            image.UnlockBits(data);

            return max;
        }

        /// <summary>
        ///   Computes the maximum pixel value in the given image.
        /// </summary>
        /// 
        public static int Max(this Bitmap image, Rectangle rectangle)
        {
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, image.PixelFormat);

            int max = Max(data, rectangle);

            image.UnlockBits(data);

            return max;
        }

        /// <summary>
        ///   Computes the minimum pixel value in the given image.
        /// </summary>
        /// 
        public static int Min(this BitmapData image, Rectangle rectangle)
        {
            if ((image.PixelFormat != PixelFormat.Format8bppIndexed) &&
                (image.PixelFormat != PixelFormat.Format16bppGrayScale))
                throw new UnsupportedImageFormatException("Only grayscale images are supported");

            int width = image.Width;
            int height = image.Height;
            int stride = image.Stride;
            int offset = image.Stride - image.Width;

            int rwidth = rectangle.Width;
            int rheight = rectangle.Height;
            int rx = rectangle.X;
            int ry = rectangle.Y;

            int min;

            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                min = byte.MaxValue;

                unsafe
                {
                    byte* src = (byte*)image.Scan0.ToPointer();

                    for (int y = 0; y < rheight; y++)
                    {
                        byte* p = src + stride * (ry + y) + rx;

                        for (int x = 0; x < rwidth; x++, p++)
                            if (*p < min) min = *p;
                    }
                }
            }
            else
            {
                min = ushort.MaxValue;

                unsafe
                {
                    ushort* src = (ushort*)image.Scan0.ToPointer();

                    for (int y = 0; y < rheight; y++)
                    {
                        ushort* p = src + stride * (ry + y) + rx;

                        for (int x = 0; x < rwidth; x++, p++)
                            if (*p < min) min = *p;
                    }
                }
            }

            return min;
        }

        /// <summary>
        ///   Computes the minimum pixel value in the given image.
        /// </summary>
        /// 
        public static int Min(this UnmanagedImage image)
        {
            return Min(image, new Rectangle(0, 0, image.Width, image.Height));
        }

        /// <summary>
        ///   Computes the minimum pixel value in the given image.
        /// </summary>
        /// 
        public static int Min(this UnmanagedImage image, Rectangle rectangle)
        {
            if ((image.PixelFormat != PixelFormat.Format8bppIndexed) &&
                (image.PixelFormat != PixelFormat.Format16bppGrayScale))
                throw new UnsupportedImageFormatException("Only grayscale images are supported");

            int width = image.Width;
            int height = image.Height;
            int stride = image.Stride;
            int offset = image.Stride - image.Width;

            int rwidth = rectangle.Width;
            int rheight = rectangle.Height;
            int rx = rectangle.X;
            int ry = rectangle.Y;

            int min;

            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                min = byte.MaxValue;

                unsafe
                {
                    byte* src = (byte*)image.ImageData.ToPointer();

                    for (int y = 0; y < rheight; y++)
                    {
                        byte* p = src + stride * (ry + y) + rx;

                        for (int x = 0; x < rwidth; x++, p++)
                            if (*p < min) min = *p;
                    }
                }
            }
            else
            {
                min = ushort.MaxValue;

                unsafe
                {
                    ushort* src = (ushort*)image.ImageData.ToPointer();

                    for (int y = 0; y < rheight; y++)
                    {
                        ushort* p = src + stride * (ry + y) + rx;

                        for (int x = 0; x < rwidth; x++, p++)
                            if (*p < min) min = *p;
                    }
                }
            }

            return min;
        }

        /// <summary>
        ///   Computes the maximum pixel value in the given image.
        /// </summary>
        /// 
        public static int Min(this Bitmap image)
        {
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, image.PixelFormat);

            int min = Min(data, new Rectangle(0, 0, data.Width, data.Height));

            image.UnlockBits(data);

            return min;
        }

        /// <summary>
        ///   Computes the maximum pixel value in the given image.
        /// </summary>
        /// 
        public static int Min(this Bitmap image, int channel)
        {
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, image.PixelFormat);

            int min = Min(new UnmanagedImage(data), channel);

            image.UnlockBits(data);

            return min;
        }

        /// <summary>
        ///   Computes the maximum pixel value in the given image.
        /// </summary>
        /// 
        public static int Min(this Bitmap image, Rectangle rectangle)
        {
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, image.PixelFormat);

            int min = Min(data, rectangle);

            image.UnlockBits(data);

            return min;
        }

        #endregion


        #region ToDoubleArray
        /// <summary>
        ///   Converts a given image into a array of double-precision
        ///   floating-point numbers scaled between -1 and 1.
        /// </summary>
        /// 
        [Obsolete("Please use the converters in the Imaging.Converters namespace.")]
        public static double[] ToDoubleArray(this Bitmap image, int channel)
        {
            return ToDoubleArray(image, channel, -1, 1);
        }

        /// <summary>
        ///   Converts a given image into a array of double-precision
        ///   floating-point numbers scaled between the given range.
        /// </summary>
        /// 
        [Obsolete("Please use the converters in the Imaging.Converters namespace.")]
        public static double[] ToDoubleArray(this Bitmap image, int channel, double min, double max)
        {
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, image.PixelFormat);

            double[] array = ToDoubleArray(data, channel, min, max);

            image.UnlockBits(data);

            return array;
        }

        /// <summary>
        ///   Converts a given image into a array of double-precision
        ///   floating-point numbers scaled between -1 and 1.
        /// </summary>
        /// 
        [Obsolete("Please use the converters in the Imaging.Converters namespace.")]
        public static double[] ToDoubleArray(this BitmapData image, int channel)
        {
            return ToDoubleArray(new UnmanagedImage(image), channel, -1, 1);
        }

        /// <summary>
        ///   Converts a given image into a array of double-precision
        ///   floating-point numbers scaled between the given range.
        /// </summary>
        /// 
        [Obsolete("Please use the converters in the Imaging.Converters namespace.")]
        public static double[] ToDoubleArray(this BitmapData image, int channel, double min, double max)
        {
            return ToDoubleArray(new UnmanagedImage(image), channel, min, max);
        }

        /// <summary>
        ///   Converts a given image into a array of double-precision
        ///   floating-point numbers scaled between the given range.
        /// </summary>
        /// 
        [Obsolete("Please use the converters in the Imaging.Converters namespace.")]
        public static double[] ToDoubleArray(this UnmanagedImage image, int channel)
        {
            return ToDoubleArray(image, channel, -1, 1);
        }

        /// <summary>
        ///   Converts a given image into a array of double-precision
        ///   floating-point numbers scaled between the given range.
        /// </summary>
        /// 
        [Obsolete("Please use the converters in the Imaging.Converters namespace.")]
        public static double[] ToDoubleArray(this UnmanagedImage image, int channel, double min, double max)
        {
            int width = image.Width;
            int height = image.Height;
            int offset = image.Stride - image.Width;

            double[] data = new double[width * height];
            int dst = 0;

            unsafe
            {
                byte* src = (byte*)image.ImageData.ToPointer() + channel;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, src++, dst++)
                        data[dst] = Accord.Math.Tools.Scale(0, 255, min, max, *src);
                    src += offset;
                }
            }

            return data;
        }

        /// <summary>
        ///   Converts a given image into a array of double-precision
        ///   floating-point numbers scaled between -1 and 1.
        /// </summary>
        /// 
        [Obsolete("Please use the converters in the Imaging.Converters namespace.")]
        public static double[][] ToDoubleArray(this Bitmap image)
        {
            return ToDoubleArray(image, -1, 1);
        }

        /// <summary>
        ///   Converts a given image into a array of double-precision
        ///   floating-point numbers scaled between the given range.
        /// </summary>
        /// 
        [Obsolete("Please use the converters in the Imaging.Converters namespace.")]
        public static double[][] ToDoubleArray(this Bitmap image, double min, double max)
        {
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, image.PixelFormat);

            double[][] array = ToDoubleArray(data, min, max);

            image.UnlockBits(data);

            return array;
        }

        /// <summary>
        ///   Converts a given image into a array of double-precision
        ///   floating-point numbers scaled between the given range.
        /// </summary>
        /// 
        [Obsolete("Please use the converters in the Imaging.Converters namespace.")]
        public static double[][] ToDoubleArray(this BitmapData image, double min, double max)
        {
            int width = image.Width;
            int height = image.Height;
            int pixelSize = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
            int offset = image.Stride - image.Width * pixelSize;

            double[][] data = new double[width * height][];
            int dst = 0;

            unsafe
            {
                byte* src = (byte*)image.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, dst++)
                    {
                        double[] pixel = data[dst] = new double[pixelSize];
                        for (int i = pixel.Length - 1; i >= 0; i--, src++)
                            pixel[i] = Accord.Math.Tools.Scale(0, 255, min, max, *src);
                    }
                    src += offset;
                }
            }

            return data;
        }
        #endregion

        #region ToDoubleMatrix
        /// <summary>
        ///   Converts a given image into a array of double-precision
        ///   floating-point numbers scaled between -1 and 1.
        /// </summary>
        /// 
        [Obsolete("Please use the converters in the Imaging.Converters namespace.")]
        public static double[,] ToDoubleMatrix(this Bitmap image, int channel)
        {
            return ToDoubleMatrix(image, channel, -1, 1);
        }

        /// <summary>
        ///   Converts a given image into a array of double-precision
        ///   floating-point numbers scaled between the given range.
        /// </summary>
        /// 
        [Obsolete("Please use the converters in the Imaging.Converters namespace.")]
        public static double[,] ToDoubleMatrix(this Bitmap image, int channel, double min, double max)
        {
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, image.PixelFormat);

            double[,] array = ToDoubleMatrix(data, channel, min, max);

            image.UnlockBits(data);

            return array;
        }

        /// <summary>
        ///   Converts a given image into a array of double-precision
        ///   floating-point numbers scaled between -1 and 1.
        /// </summary>
        /// 
        [Obsolete("Please use the converters in the Imaging.Converters namespace.")]
        public static double[,] ToDoubleMatrix(this BitmapData image, int channel)
        {
            return ToDoubleMatrix(image, channel, -1, 1);
        }

        /// <summary>
        ///   Converts a given image into a array of double-precision
        ///   floating-point numbers scaled between the given range.
        /// </summary>
        /// 
        [Obsolete("Please use the converters in the Imaging.Converters namespace.")]
        public static double[,] ToDoubleMatrix(this BitmapData image, int channel, double min, double max)
        {
            int width = image.Width;
            int height = image.Height;
            int offset = image.Stride - image.Width;

            double[,] data = new double[height, width];

            unsafe
            {
                fixed (double* ptrData = data)
                {
                    double* dst = ptrData;
                    byte* src = (byte*)image.Scan0.ToPointer() + channel;

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, src++, dst++)
                        {
                            *dst = Accord.Math.Tools.Scale(0, 255, min, max, *src);
                        }
                        src += offset;
                    }
                }
            }

            return data;
        }
        #endregion

        #region ToBitmap
        /// <summary>
        ///   Converts an image given as a matrix of pixel values into
        ///   a <see cref="System.Drawing.Bitmap"/>.
        /// </summary>
        /// 
        /// <param name="pixels">A matrix containing the grayscale pixel
        /// values as <see cref="System.Double">bytes</see>.</param>
        /// <returns>A <see cref="System.Drawing.Bitmap"/> of the same width
        /// and height as the pixel matrix containing the given pixel values.</returns>
        /// 
        [Obsolete("Please use the converters in the Imaging.Converters namespace.")]
        public static Bitmap ToBitmap(this byte[,] pixels)
        {
            int width = pixels.GetLength(1);
            int height = pixels.GetLength(0);

            Bitmap bitmap = AForge.Imaging.Image.CreateGrayscaleImage(width, height);

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly, bitmap.PixelFormat);

            int offset = data.Stride - width;

            unsafe
            {
                byte* dst = (byte*)data.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, dst++)
                    {
                        *dst = (byte)pixels[y, x];
                    }
                    dst += offset;
                }
            }

            bitmap.UnlockBits(data);

            return bitmap;
        }

        /// <summary>
        ///   Converts an image given as a matrix of pixel values into
        ///   a <see cref="System.Drawing.Bitmap"/>.
        /// </summary>
        /// 
        /// <param name="pixels">A matrix containing the grayscale pixel
        /// values as <see cref="System.Double">bytes</see>.</param>
        /// <returns>A <see cref="System.Drawing.Bitmap"/> of the same width
        /// and height as the pixel matrix containing the given pixel values.</returns>
        /// 
        [Obsolete("Please use the converters in the Imaging.Converters namespace.")]
        public static Bitmap ToBitmap(this short[,] pixels)
        {
            int width = pixels.GetLength(1);
            int height = pixels.GetLength(0);

            Bitmap bitmap = AForge.Imaging.Image.CreateGrayscaleImage(width, height);
            bitmap = AForge.Imaging.Image.Convert8bppTo16bpp(bitmap);

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly, bitmap.PixelFormat);

            int offset = data.Stride - width;

            unsafe
            {
                short* dst = (short*)data.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, dst++)
                    {
                        *dst = (short)pixels[y, x];
                    }
                    dst += offset;
                }
            }

            bitmap.UnlockBits(data);

            return bitmap;
        }

        /// <summary>
        ///   Converts an image given as a array of pixel values into
        ///   a <see cref="System.Drawing.Bitmap"/>.
        /// </summary>
        /// 
        /// <param name="pixels">An array containing the grayscale pixel
        /// values as <see cref="System.Double">doubles</see>.</param>
        /// <param name="width">The width of the resulting image.</param>
        /// <param name="height">The height of the resulting image.</param>
        /// <param name="min">The minimum value representing a color value of 0.</param>
        /// <param name="max">The maximum value representing a color value of 255. </param>
        /// 
        /// <returns>A <see cref="System.Drawing.Bitmap"/> of given width and height
        /// containing the given pixel values.</returns>
        /// 
        [Obsolete("Please use the converters in the Imaging.Converters namespace.")]
        public static Bitmap ToBitmap(this double[] pixels, int width, int height, double min, double max)
        {
            Bitmap bitmap = AForge.Imaging.Image.CreateGrayscaleImage(width, height);

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly, bitmap.PixelFormat);

            int offset = data.Stride - width;
            int src = 0;

            unsafe
            {
                byte* dst = (byte*)data.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, src++, dst++)
                    {
                        *dst = (byte)Accord.Math.Tools.Scale(min, max, 0, 255, pixels[src]);
                    }
                    dst += offset;
                }
            }

            bitmap.UnlockBits(data);

            return bitmap;
        }

        /// <summary>
        ///   Converts an image given as a array of pixel values into
        ///   a <see cref="System.Drawing.Bitmap"/>.
        /// </summary>
        /// 
        /// <param name="pixels">An jagged array containing the pixel values
        /// as double arrays. Each element of the arrays will be converted to
        /// a R, G, B, A value. The bits per pixel of the resulting image
        /// will be set according to the size of these arrays.</param>
        /// <param name="width">The width of the resulting image.</param>
        /// <param name="height">The height of the resulting image.</param>
        /// <param name="min">The minimum value representing a color value of 0.</param>
        /// <param name="max">The maximum value representing a color value of 255. </param>
        /// 
        /// <returns>A <see cref="System.Drawing.Bitmap"/> of given width and height
        /// containing the given pixel values.</returns>
        /// 
        [Obsolete("Please use the converters in the Imaging.Converters namespace.")]
        public static Bitmap ToBitmap(this double[][] pixels, int width, int height, double min, double max)
        {
            PixelFormat format;
            int channels = pixels[0].Length;

            switch (channels)
            {
                case 1:
                    format = PixelFormat.Format8bppIndexed;
                    break;

                case 3:
                    format = PixelFormat.Format24bppRgb;
                    break;

                case 4:
                    format = PixelFormat.Format32bppArgb;
                    break;

                default:
                    throw new ArgumentException("Unsupported image pixel format.", "pixels");
            }


            Bitmap bitmap = new Bitmap(width, height, format);

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly, format);

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(format) / 8;
            int offset = data.Stride - width * pixelSize;
            int src = 0;

            unsafe
            {
                byte* dst = (byte*)data.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, src++)
                    {
                        for (int c = channels - 1; c >= 0; c--, dst++)
                        {
                            *dst = (byte)Accord.Math.Tools.Scale(min, max, 0, 255, pixels[src][c]);
                        }
                    }
                    dst += offset;
                }
            }

            bitmap.UnlockBits(data);

            return bitmap;
        }

        /// <summary>
        ///   Converts an image given as a array of pixel values into
        ///   a <see cref="System.Drawing.Bitmap"/>.
        /// </summary>
        /// 
        /// <param name="pixels">An jagged array containing the pixel values
        /// as double arrays. Each element of the arrays will be converted to
        /// a R, G, B, A value. The bits per pixel of the resulting image
        /// will be set according to the size of these arrays.</param>
        /// <param name="width">The width of the resulting image.</param>
        /// <param name="height">The height of the resulting image.</param>
        /// 
        /// <returns>A <see cref="System.Drawing.Bitmap"/> of given width and height
        /// containing the given pixel values.</returns>
        /// 
        [Obsolete("Please use the converters in the Imaging.Converters namespace.")]
        public static Bitmap ToBitmap(this double[][] pixels, int width, int height)
        {
            return ToBitmap(pixels, width, height, -1, 1);
        }
        #endregion

        #endregion


        /// <summary>
        ///   Multiplies a point by a transformation matrix.
        /// </summary>
        /// 
        public static float[] Multiply(this PointF point, float[,] transformationMatrix)
        {
            float[] x = new float[] { point.X, point.Y, 1 };
            return Matrix.Multiply(x, transformationMatrix);
        }

        /// <summary>
        ///   Multiplies a transformation matrix and a point.
        /// </summary>
        /// 
        public static float[] Multiply(this  float[,] transformationMatrix, PointF point)
        {
            float[] x = new float[] { point.X, point.Y, 1 };
            return Matrix.Multiply(transformationMatrix, x);
        }

        /// <summary>
        ///   Computes the inner product of two points.
        /// </summary>
        /// 
        public static float InnerProduct(this  PointF a, PointF b)
        {
            return a.X * b.X + a.Y * b.Y + 1;
        }

        /// <summary>
        ///   Transforms the given points using this transformation matrix.
        /// </summary>
        /// 
        public static PointF[] TransformPoints(this float[,] fundamentalMatrix, params PointF[] points)
        {
            PointF[] r = new PointF[points.Length];

            for (int j = 0; j < points.Length; j++)
            {
                float[] a = new float[] { points[j].X, points[j].Y, 1 };
                float[] b = fundamentalMatrix.Multiply(a);
                r[j] = new PointF(b[0] / b[2], b[1] / b[2]);
            }

            return r;
        }

    }
}
