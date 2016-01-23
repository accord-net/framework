// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Diego Catalano, 2015
// diego.catalano at live.com
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

    /// <summary>
    ///   Fast Variance filter.
    /// </summary>
    /// 
    /// <remarks>
    ///   The Fast Variance filter replaces each pixel in an image by its
    ///   neighborhood online variance. This filter differs from the
    ///   <see cref="Variance" />filter because it uses only a single pass
    ///   over the image.
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// Bitmap image = ... // Lena's picture
    /// 
    /// // Create a new Variance filter:
    /// var variance = new FastVariance();
    /// 
    /// // Compute the filter
    /// Bitmap result = variance.Apply(image);
    /// 
    /// // Show on the screen
    /// ImageBox.Show(result);
    /// </code>
    /// 
    /// <para>
    ///   The resulting image is shown below:</para>
    /// 
    ///   <img src="..\images\variance.png" />
    /// 
    /// </example>
    /// 
    public class FastVariance : BaseFilter
    {

        private int radius = 2;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;

        /// <summary>
        ///   Gets or sets the radius of the neighborhood
        ///   used to compute a pixel's local variance.
        /// </summary>
        /// 
        public int Radius
        {
            get { return radius; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value", "Radius must be higher than zero.");
                radius = value;
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Variance"/> class.
        /// </summary>
        /// 
        public FastVariance()
        {
            formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Variance"/> class.
        /// </summary>
        /// 
        /// <param name="radius">The radius neighborhood used to compute a pixel's local variance.</param>
        /// 
        public FastVariance(int radius)
            : this()
        {
            if (radius < 1)
                throw new ArgumentOutOfRangeException("radius", "Radius must be higher than zero.");

            this.radius = radius;
        }


        /// <summary>
        ///   Format translations dictionary.
        /// </summary>
        /// 
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }


        /// <summary>
        ///   Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        /// 
        protected override unsafe void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
        {
            int width = sourceData.Width;
            int height = sourceData.Height;
            int size = radius * 2;

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(sourceData.PixelFormat) / 8;

            int srcStride = sourceData.Stride;
            int dstStride = destinationData.Stride;

            int srcOffset = srcStride - width * pixelSize;
            int dstOffset = dstStride - width * pixelSize;

            byte* src = (byte*)sourceData.ImageData.ToPointer();
            byte* dst = (byte*)destinationData.ImageData.ToPointer();


            // do the processing job
            if (destinationData.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                // for each line
                for (int y = 0; y < height; y++)
                {
                    // for each pixel
                    for (int x = 0; x < width; x++, src++, dst++)
                    {
                        double mean = 0;
                        double m2 = 0;
                        int n = 0;

                        for (int i = 0; i < radius; i++)
                        {
                            int ir = i - radius;
                            int t = y + ir;

                            if (t < 0) 
                                continue;
                            if (t >= height) 
                                break;

                            for (int j = 0; j < size; j++)
                            {
                                int jr = j - radius;
                                t = x + jr;

                                if (t < 0) 
                                    continue;
                                if (t >= width) 
                                    continue;

                                double delta = src[ir * srcStride + jr] - mean;

                                n++; // update statistics
                                mean += delta / (double)n;
                                m2 += delta * (src[ir * srcStride + jr] - mean);
                            }
                        }


                        double variance = m2 / ((double)n - 1);

                        *dst = (byte)((variance > 255) ? 255 : ((variance < 0) ? 0 : variance));
                    }

                    src += srcOffset;
                    dst += dstOffset;
                }
            }
            else
            {
                // for each line
                for (int y = 0; y < height; y++)
                {
                    // for each pixel
                    for (int x = 0; x < width; x++, src += pixelSize, dst += pixelSize)
                    {
                        double meanR = 0;
                        double meanG = 0;
                        double meanB = 0;

                        double m2R = 0;
                        double m2G = 0;
                        double m2B = 0;
                        int n = 0;

                        for (int i = 0; i < size; i++)
                        {
                            int ir = i - radius;
                            int t = y + ir;

                            if (t < 0) 
                                continue;
                            if (t >= height) 
                                break;

                            for (int j = 0; j < size; j++)
                            {
                                int jr = j - radius;
                                t = x + jr;

                                if (t < 0) 
                                    continue;
                                if (t >= width) 
                                    continue;

                                byte* p = &src[ir * srcStride + jr * pixelSize];

                                n++; // Update statistics
                                double deltaR = p[RGB.R] - meanR;
                                double deltaG = p[RGB.G] - meanG;
                                double deltaB = p[RGB.B] - meanB;

                                meanR += deltaR / (double)n;
                                meanG += deltaG / (double)n;
                                meanB += deltaB / (double)n;

                                m2R += deltaR * (p[RGB.R] - meanR);
                                m2G += deltaG * (p[RGB.G] - meanG);
                                m2B += deltaB * (p[RGB.B] - meanB);

                            }
                        }

                        double varR = m2R / ((double)n - 1);
                        double varG = m2G / ((double)n - 1);
                        double varB = m2B / ((double)n - 1);

                        dst[RGB.R] = (byte)((varR > 255) ? 255 : ((varR < 0) ? 0 : varR));
                        dst[RGB.G] = (byte)((varG > 255) ? 255 : ((varG < 0) ? 0 : varG));
                        dst[RGB.B] = (byte)((varB > 255) ? 255 : ((varB < 0) ? 0 : varB));

                        // take care of alpha channel
                        if (pixelSize == 4)
                            dst[RGB.A] = src[RGB.A];
                    }

                    src += srcOffset;
                    dst += dstOffset;
                }
            }
        }
    }
}