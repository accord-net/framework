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

namespace Accord.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using AForge.Imaging;
    using AForge.Imaging.Filters;

    /// <summary>
    ///   Variance filter.
    /// </summary>
    /// 
    /// <remarks>
    ///   The Variance filter replaces each pixel in an image by its
    ///   neighborhood variance. The end result can be regarded as an
    ///   border enhancement, making the Variance filter suitable to
    ///   be used as an edge detection mechanism.
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// Bitmap image = ... // Lena's picture
    /// 
    /// // Create a new Variance filter:
    /// var variance = new VarianceFilter();
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
    public class Variance : BaseFilter
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
        public Variance()
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
        public Variance(int radius)
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
                        long sum = 0;
                        int count = 0;

                        for (int i = 0; i < radius; i++)
                        {
                            int ir = i - radius;
                            int t = y + ir;

                            if (t < 0) continue;
                            if (t >= height) break;

                            for (int j = 0; j < size; j++)
                            {
                                int jr = j - radius;
                                t = x + jr;

                                if (t < 0) continue;
                                if (t >= width) continue;

                                sum += src[ir * srcStride + jr];
                                count++;
                            }
                        }

                        double mean = sum / (double)count;
                        double variance = 0;

                        for (int i = 0; i < radius; i++)
                        {
                            int ir = i - radius;
                            int t = y + ir;

                            if (t < 0) continue;
                            if (t >= height) break;

                            for (int j = 0; j < size; j++)
                            {
                                int jr = j - radius;
                                t = x + jr;

                                if (t < 0) continue;
                                if (t >= width) continue;

                                byte val = src[ir * srcStride + jr];
                                variance += (val - mean) * (val - mean);
                            }
                        }

                        variance /= count - 1;

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
                        long sumR = 0;
                        long sumG = 0;
                        long sumB = 0;
                        int count = 0;

                        for (int i = 0; i < size; i++)
                        {
                            int ir = i - radius;
                            int t = y + ir;

                            if (t < 0) continue;
                            if (t >= height) break;

                            for (int j = 0; j < size; j++)
                            {
                                int jr = j - radius;
                                t = x + jr;

                                if (t < 0) continue;
                                if (t >= height) continue;

                                byte* p = &src[ir * srcStride + jr * pixelSize];

                                count++;

                                sumR += p[RGB.R];
                                sumG += p[RGB.G];
                                sumB += p[RGB.B];
                            }
                        }

                        double meanR = sumR / (double)count;
                        double meanG = sumG / (double)count;
                        double meanB = sumB / (double)count;

                        double varR = 0;
                        double varG = 0;
                        double varB = 0;

                        for (int i = 0; i < size; i++)
                        {
                            int ir = i - radius;
                            int t = y + ir;

                            if (t < 0) continue;
                            if (t >= height) break;

                            // for each kernel column
                            for (int j = 0; j < size; j++)
                            {
                                int jr = j - radius;
                                t = x + jr;

                                if (t < 0) continue;
                                if (t >= height) continue;

                                byte* p = &src[ir * srcStride + jr * pixelSize];

                                count++;

                                varR += (p[RGB.R] - meanR) * (p[RGB.R] - meanR);
                                varG += (p[RGB.G] - meanG) * (p[RGB.G] - meanG);
                                varB += (p[RGB.B] - meanB) * (p[RGB.B] - meanB);
                            }
                        }

                        varR /= count - 1;
                        varG /= count - 1;
                        varB /= count - 1;

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
