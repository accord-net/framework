// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Diego Catalano, 2013
// diego.catalano at live.com
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
    using System;
    using AForge.Imaging;

    /// <summary>
    ///   Gray-Level Difference Method (GLDM).
    /// </summary>
    /// 
    /// <remarks>
    ///   Computes an gray-level histogram of difference 
    ///   values between adjacent pixels in an image.
    /// </remarks>
    /// 
    public class GrayLevelDifferenceMethod
    {

        private CooccurrenceDegree degree;
        private bool autoGray = true;


        /// <summary>
        ///   Gets or sets whether the maximum value of gray should be
        ///   automatically computed from the image. If set to false,
        ///   the maximum gray value will be assumed 255.
        /// </summary>
        /// 
        public bool AutoGray
        {
            get { return autoGray; }
            set { autoGray = value; }
        }

        /// <summary>
        ///   Gets or sets the direction at which the co-occurrence should be found.
        /// </summary>
        /// 
        public CooccurrenceDegree Degree
        {
            get { return degree; }
            set { degree = value; }
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="GrayLevelDifferenceMethod"/> class.
        /// </summary>
        /// 
        /// <param name="degree">The direction at which the co-occurrence should be found.</param>
        /// 
        public GrayLevelDifferenceMethod(CooccurrenceDegree degree)
        {
            this.degree = degree;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="GrayLevelDifferenceMethod"/> class.
        /// </summary>
        /// 
        /// <param name="degree">The direction at which the co-occurrence should be found.</param>
        /// <param name="autoGray">Whether the maximum value of gray should be
        ///   automatically computed from the image. Default is true.</param>
        /// 
        public GrayLevelDifferenceMethod(CooccurrenceDegree degree, bool autoGray)
        {
            this.degree = degree;
            this.autoGray = autoGray;
        }

        /// <summary>
        ///   Computes the Gray-level Difference Method (GLDM)
        ///   Histogram for the given source image.
        /// </summary>
        /// 
        /// <param name="source">The source image.</param>
        /// 
        /// <returns>An histogram containing co-occurrences 
        /// for every gray level in <paramref name="source"/>.</returns>
        /// 
        public unsafe int[] Compute(UnmanagedImage source)
        {
            int width = source.Width;
            int height = source.Height;
            int stride = source.Stride;
            int offset = stride - width;
            int maxGray = 255;

            byte* src = (byte*)source.ImageData.ToPointer();

            if (autoGray)
                maxGray = max(width, height, offset, src);


            int[] hist = new int[maxGray + 1];

            switch (degree)
            {
                case CooccurrenceDegree.Degree0:
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 1; x < width; x++)
                        {
                            byte a = src[stride * y + (x - 1)];
                            byte b = src[stride * y + x];
                            int bin = Math.Abs(a - b);
                            hist[bin]++;
                        }
                    }
                    break;

                case CooccurrenceDegree.Degree45:
                    for (int y = 1; y < height; y++)
                    {
                        for (int x = 0; x < width - 1; x++)
                        {
                            byte a = src[stride * y + x];
                            byte b = src[stride * (y - 1) + (x + 1)];
                            int bin = Math.Abs(a - b);
                            hist[bin]++;
                        }
                    }
                    break;

                case CooccurrenceDegree.Degree90:
                    for (int y = 1; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            byte a = src[stride * (y - 1) + x];
                            byte b = src[stride * y + x];
                            int bin = Math.Abs(a - b);
                            hist[bin]++;
                        }
                    }
                    break;

                case CooccurrenceDegree.Degree135:
                    for (int y = 1; y < height; y++)
                    {
                        int steps = width - 1;
                        for (int x = 0; x < width - 1; x++)
                        {
                            byte a = src[stride * y + (steps - x)];
                            byte b = src[stride * (y - 1) + (steps - 1 - x)];
                            int bin = Math.Abs(a - b);
                            hist[bin]++;
                        }
                    }
                    break;
            }

            return hist;
        }

        unsafe private static int max(int width, int height, int offset, byte* src)
        {
            int maxGray = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++, src++)
                    if (*src > maxGray) maxGray = *src;
                src += offset;
            }

            return maxGray;
        }

    }
}