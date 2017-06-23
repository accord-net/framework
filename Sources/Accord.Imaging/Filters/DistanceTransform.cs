// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Diego Catalano, 2017
// diego.catalano at live.com
//
// Copyright © César Souza, 2009-2017
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

namespace Accord.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using Accord.Imaging;
    using Accord.Imaging.Filters;
    using Accord.Math;
    using Accord.Diagnostics;

    /// <summary>
    ///   Distance functions that can be used with <see cref="DistanceTransform"/>.
    /// </summary>
    /// 
    public enum DistanceTransformMethod
    {
        /// <summary>
        ///   Chessboard distance.
        /// </summary>
        /// 
        Chessboard,

        /// <summary>
        ///   Euclidean distance.
        /// </summary>
        Euclidean,

        /// <summary>
        ///   Manhattan distance.
        /// </summary>
        Manhattan,

        /// <summary>
        ///   Squared Euclidean distance.
        /// </summary>
        /// 
        SquaredEuclidean
    };

    /// <summary>
    ///   Distance transform filter.
    /// </summary>
    /// 
    /// <remarks>
    ///   Simple exp image filter. Applies the <see cref="System.Math.Exp"/>
    ///   function for each pixel in the image, clipping values as needed.
    ///   The resultant image can be converted back using the <see cref="Logarithm"/>
    ///   filter.
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///   Bitmap input = ... 
    /// 
    ///   // Apply the transform
    ///   DistanceTransform dt = new DistanceTransform();
    ///   Bitmap output = dt.Apply(input);
    /// 
    ///   // Show results on screen
    ///   ImageBox.Show("input", input);
    ///   ImageBox.Show("output", output);
    ///   ImageBox.Show("reconstruction", reconstruction);
    /// </code>
    /// </example>
    /// 
    public class DistanceTransform : BaseInPlaceFilter
    {
        private float max = 0;
        private IntPoint ued;
        private DistanceTransformMethod distance = DistanceTransformMethod.Euclidean;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;

        /// <summary>
        ///   Format translations dictionary.
        /// </summary>
        /// 
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistanceTransform"/> class.
        /// </summary>
        /// 
        public DistanceTransform()
        {
            formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
        }


        /// <summary>
        ///   Gets the maximum distance from the transform.
        /// </summary>
        /// 
        public float MaximumDistance
        {
            get { return max; }
        }

        /// <summary>
        ///   Gets the ultimate eroded point.
        /// </summary>
        /// 
        public IntPoint UltimateErodedPoint
        {
            get { return ued; }
        }


        /// <summary>
        ///   Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        /// 
        protected unsafe override void ProcessFilter(UnmanagedImage image)
        {
            int width = image.Width;
            int height = image.Height;
            PixelFormat format = image.PixelFormat;
            int pixelSize = System.Drawing.Bitmap.GetPixelFormatSize(format) / 8;

            Debug.Assert(pixelSize == 1);
            Debug.Assert(image.Stride == width);

            byte[] bPixels = image.ToByteArray();
            float[] fPixels = new float[bPixels.Length];

            for (int i = 0; i < bPixels.Length; i++)
            {
                if (bPixels[i] != 0)
                    fPixels[i] = float.MaxValue;
            }

            int[][] pointBufs = Jagged.Zeros<int>(2, width);

            // pass 1 & 2: increasing y
            for (int x = 0; x < width; x++)
            {
                pointBufs[0][x] = -1;
                pointBufs[1][x] = -1;
            }

            for (int y = 0; y < height; y++)
                edmLine(bPixels, fPixels, pointBufs, width, y * width, y);

            // pass 3 & 4: decreasing y
            for (int x = 0; x < width; x++)
            {
                pointBufs[0][x] = -1;
                pointBufs[1][x] = -1;
            }

            for (int y = height - 1; y >= 0; y--)
                edmLine(bPixels, fPixels, pointBufs, width, y * width, y);

            // Finish applying the distance function
            if (NeedSquareRoot(distance))
            {
                for (int i = 0, p = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++, p++)
                    {
                        float f = fPixels[p] = (fPixels[p] < 0f) ? 0 : (float)Math.Sqrt(fPixels[p]);

                        if (f > max)
                        {
                            max = f;
                            ued = new IntPoint(i, j);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0, p = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++, p++)
                    {
                        float f = fPixels[p] = (fPixels[p] < 0f) ? 0 : fPixels[p];

                        if (f > max)
                        {
                            max = f;
                            ued = new IntPoint(i, j);
                        }
                    }
                }
            }

            // Normalize and store
            fixed (float* srcPtr = fPixels)
            {
                byte* dst = (byte*)image.ImageData.ToPointer();
                float* src = srcPtr;
                for (int i = 0; i < height; i++)
                    for (int j = 0; j < width; j++, src++, dst++)
                        (*dst) = (byte)Vector.Scale(*src, 0, max, 0, 255);
            }
        }



        // Handle a line; two passes: left-to-right and right-to-left
        private void edmLine(byte[] bPixels, float[] fPixels, int[][] pointBufs, int width, int offset, int y)
        {
            int[] points = pointBufs[0]; // the buffer for the left-to-right pass

            // point at (-/+1, -/+1) to current one (-1,-1 in the first pass)
            int pPrev = -1;
            int pDiag = -1;
            int pNextDiag;

            int distSqr = Int32.MaxValue; // this value is used only if edges are not background

            for (int x = 0; x < width; x++, offset++)
            {
                pNextDiag = points[x];
                if (bPixels[offset] == 0)
                {
                    points[x] = x | y << 16; // remember coordinates as a candidate for nearest background point
                }
                else
                {
                    // foreground pixel:
                    float dist2 = minDist2(points, pPrev, pDiag, x, y, distSqr, distance);
                    if (fPixels[offset] > dist2)
                        fPixels[offset] = dist2;
                }
                pPrev = points[x];
                pDiag = pNextDiag;
            }

            offset--; //now points to the last pixel in the line
            points = pointBufs[1]; // the buffer for the right-to-left pass. Low short contains x, high short y
            pPrev = -1;
            pDiag = -1;
            for (int x = width - 1; x >= 0; x--, offset--)
            {
                pNextDiag = points[x];

                if (bPixels[offset] == 0)
                {
                    points[x] = x | y << 16; // remember coordinates as a candidate for nearest background point
                }
                else
                {
                    // foreground pixel:
                    float dist2 = minDist2(points, pPrev, pDiag, x, y, distSqr, distance);
                    if (fPixels[offset] > dist2)
                        fPixels[offset] = dist2;
                }

                pPrev = points[x];
                pDiag = pNextDiag;
            }
        }

        private float minDist2(int[] points, int pPrev, int pDiag, int x, int y, int distSqr, DistanceTransformMethod distance)
        {
            int p0 = points[x]; // the nearest background point for the same x in the previous line
            int nearestPoint = p0;

            if (p0 != -1)
            {
                int x0 = p0 & 0xffff; int y0 = (p0 >> 16) & 0xffff;
                int dist1Sqr = calcDistance(x, y, x0, y0, distance);
                if (dist1Sqr < distSqr)
                    distSqr = dist1Sqr;
            }

            if (pDiag != p0 && pDiag != -1)
            {
                int x1 = pDiag & 0xffff; int y1 = (pDiag >> 16) & 0xffff;
                int dist1Sqr = calcDistance(x, y, x1, y1, distance);
                if (dist1Sqr < distSqr)
                {
                    nearestPoint = pDiag;
                    distSqr = dist1Sqr;
                }
            }

            if (pPrev != pDiag && pPrev != -1)
            {
                int x1 = pPrev & 0xffff; int y1 = (pPrev >> 16) & 0xffff;
                int dist1Sqr = calcDistance(x, y, x1, y1, distance);
                if (dist1Sqr < distSqr)
                {
                    nearestPoint = pPrev;
                    distSqr = dist1Sqr;
                }
            }

            points[x] = nearestPoint;
            return (float)distSqr;
        }

        private int calcDistance(int x, int y, int x0, int y0, DistanceTransformMethod distance)
        {
            switch (distance)
            {
                case DistanceTransformMethod.Euclidean:
                    return (x - x0) * (x - x0) + (y - y0) * (y - y0);
                case DistanceTransformMethod.Manhattan:
                    return Math.Abs(x - x0) + Math.Abs(y - y0);
                case DistanceTransformMethod.Chessboard:
                    return Math.Max(Math.Abs(x - x0), Math.Abs(y - y0));
                case DistanceTransformMethod.SquaredEuclidean:
                    return (x - x0) * (x - x0) + (y - y0) * (y - y0);
            }

            throw new Exception();
        }

        private bool NeedSquareRoot(DistanceTransformMethod distance)
        {
            switch (distance)
            {
                case DistanceTransformMethod.Euclidean:
                    return true;
                case DistanceTransformMethod.Chessboard:
                case DistanceTransformMethod.Manhattan:
                case DistanceTransformMethod.SquaredEuclidean:
                    return false;
            }

            throw new Exception();
        }

    }
}
