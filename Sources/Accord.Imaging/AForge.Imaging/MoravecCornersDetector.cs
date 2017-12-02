// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
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
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Collections.Generic;
    using Accord.Compat;

    /// <summary>
    /// Moravec corners detector.
    /// </summary>
    /// 
    /// <remarks><para>The class implements Moravec corners detector. For information about algorithm's
    /// details its <a href="http://www.cim.mcgill.ca/~dparks/CornerDetector/mainMoravec.htm">description</a>
    /// should be studied.</para>
    /// 
    /// <para><note>Due to limitations of Moravec corners detector (anisotropic response, etc.) its usage is limited
    /// to certain cases only.</note></para>
    /// 
    /// <para>The class processes only grayscale 8 bpp and color 24/32 bpp images.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create corner detector's instance
    /// MoravecCornersDetector mcd = new MoravecCornersDetector( );
    /// // process image searching for corners
    /// List&lt;IntPoint&gt; corners = scd.ProcessImage( image );
    /// // process points
    /// foreach ( IntPoint corner in corners )
    /// {
    ///     // ... 
    /// }
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="SusanCornersDetector"/>
    /// 
    public class MoravecCornersDetector : BaseCornersDetector
    {
        // window size
        private int windowSize = 3;

        // threshold which is used to filter interest points
        private int threshold = 500;

        /// <summary>
        /// Window size used to determine if point is interesting, [3, 15].
        /// </summary>
        /// 
        /// <remarks><para>The value specifies window size, which is used for initial searching of
        /// corners candidates and then for searching local maximums.</para>
        /// 
        /// <para>Default value is set to <b>3</b>.</para>
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">Setting value is not odd.</exception>
        /// 
        public int WindowSize
        {
            get { return windowSize; }
            set
            {
                // check if value is odd
                if ((value & 1) == 0)
                    throw new ArgumentException("The value shoule be odd.");

                windowSize = Math.Max(3, Math.Min(15, value));
            }
        }

        /// <summary>
        /// Threshold value, which is used to filter out uninteresting points.
        /// </summary>
        /// 
        /// <remarks><para>The value is used to filter uninteresting points - points which have value below
        /// specified threshold value are treated as not corners candidates. Increasing this value decreases
        /// the amount of detected point.</para>
        /// 
        /// <para>Default value is set to <b>500</b>.</para>
        /// </remarks>
        /// 
        public int Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }

        private static int[] xDelta = new int[8] { -1, 0, 1, 1, 1, 0, -1, -1 };
        private static int[] yDelta = new int[8] { -1, -1, -1, 0, 1, 1, 1, 0 };

        /// <summary>
        /// Initializes a new instance of the <see cref="MoravecCornersDetector"/> class.
        /// </summary>
        /// 
        public MoravecCornersDetector()
        {
            base.SupportedFormats.UnionWith(new[]
            {
                PixelFormat.Format8bppIndexed,
                PixelFormat.Format24bppRgb,
                PixelFormat.Format32bppRgb,
                PixelFormat.Format32bppArgb
            });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoravecCornersDetector"/> class.
        /// </summary>
        /// 
        /// <param name="threshold">Threshold value, which is used to filter out uninteresting points.</param>
        /// 
        public MoravecCornersDetector(int threshold) :
            this(threshold, 3)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoravecCornersDetector"/> class.
        /// </summary>
        /// 
        /// <param name="threshold">Threshold value, which is used to filter out uninteresting points.</param>
        /// <param name="windowSize">Window size used to determine if point is interesting.</param>
        /// 
        public MoravecCornersDetector(int threshold, int windowSize)
            : this()
        {
            this.Threshold = threshold;
            this.WindowSize = windowSize;
        }

        /// <summary>
        /// This method should be implemented by inheriting classes to implement the
        /// actual corners detection, transforming the input image into a list of points.
        /// </summary>
        /// 
        protected override List<IntPoint> InnerProcess(UnmanagedImage image)
        {
            // get source image size
            int width = image.Width;
            int height = image.Height;
            int stride = image.Stride;
            int pixelSize = Bitmap.GetPixelFormatSize(image.PixelFormat) / 8;

            // window radius
            int windowRadius = windowSize / 2;

            // offset
            int offset = stride - windowSize * pixelSize;

            // create moravec cornerness map
            int[,] moravecMap = new int[height, width];

            // do the job
            unsafe
            {
                byte* ptr = (byte*)image.ImageData.ToPointer();

                // for each row
                for (int y = windowRadius, maxY = height - windowRadius; y < maxY; y++)
                {
                    // for each pixel
                    for (int x = windowRadius, maxX = width - windowRadius; x < maxX; x++)
                    {
                        int minSum = int.MaxValue;

                        // go through 8 possible shifting directions
                        for (int k = 0; k < 8; k++)
                        {
                            // calculate center of shifted window
                            int sy = y + yDelta[k];
                            int sx = x + xDelta[k];

                            // check if shifted window is within the image
                            if (
                                (sy < windowRadius) || (sy >= maxY) ||
                                (sx < windowRadius) || (sx >= maxX)
                            )
                            {
                                // skip this shifted window
                                continue;
                            }

                            int sum = 0;

                            byte* ptr1 = ptr + (y - windowRadius) * stride + (x - windowRadius) * pixelSize;
                            byte* ptr2 = ptr + (sy - windowRadius) * stride + (sx - windowRadius) * pixelSize;

                            // for each windows' rows
                            for (int i = 0; i < windowSize; i++)
                            {
                                // for each windows' pixels
                                for (int j = 0, maxJ = windowSize * pixelSize; j < maxJ; j++, ptr1++, ptr2++)
                                {
                                    image.CheckBounds(ptr1);
                                    image.CheckBounds(ptr2);

                                    int dif = *ptr1 - *ptr2;
                                    sum += dif * dif;
                                }
                                ptr1 += offset;
                                ptr2 += offset;
                            }

                            // check if the sum is mimimal
                            if (sum < minSum)
                            {
                                minSum = sum;
                            }
                        }

                        // threshold the minimum sum
                        if (minSum < threshold)
                        {
                            minSum = 0;
                        }

                        moravecMap[y, x] = minSum;
                    }
                }
            }

            // collect interesting points - only those points, which are local maximums
            List<IntPoint> cornersList = new List<IntPoint>();

            // for each row
            for (int y = windowRadius, maxY = height - windowRadius; y < maxY; y++)
            {
                // for each pixel
                for (int x = windowRadius, maxX = width - windowRadius; x < maxX; x++)
                {
                    int currentValue = moravecMap[y, x];

                    // for each windows' rows
                    for (int i = -windowRadius; (currentValue != 0) && (i <= windowRadius); i++)
                    {
                        // for each windows' pixels
                        for (int j = -windowRadius; j <= windowRadius; j++)
                        {
                            if (moravecMap[y + i, x + j] > currentValue)
                            {
                                currentValue = 0;
                                break;
                            }
                        }
                    }

                    // check if this point is really interesting
                    if (currentValue != 0)
                    {
                        cornersList.Add(new IntPoint(x, y));
                    }
                }
            }

            return cornersList;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        protected override object Clone(ISet<PixelFormat> supportedFormats)
        {
            return new MoravecCornersDetector(threshold, windowSize)
            {
                SupportedFormats = supportedFormats
            };
        }
    }
}
