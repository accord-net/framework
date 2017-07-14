// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Diego Catalano, 2013
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

    /// <summary>
    ///   Niblack Threshold.
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>
    ///   The Niblack filter is a local thresholding algorithm that separates
    ///   white and black pixels given the local mean and standard deviation
    ///   for the current window.</para>
    ///   
    /// <para>
    ///  This filter implementation has been contributed by Diego Catalano.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///        W. Niblack, An Introduction to Digital Image Processing, pp. 115-116.
    ///        Prentice Hall, 1986.</description></item>
    ///   </list></para>   
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// Bitmap image = ... // Lena's picture
    /// 
    /// // Create a new Niblack threshold:
    /// var niblack = new NiblackThreshold();
    /// 
    /// // Compute the filter
    /// Bitmap result = niblack.Apply(image);
    /// 
    /// // Show on the screen
    /// ImageBox.Show(result);
    /// </code>
    /// 
    /// <para>
    ///   The resulting image is shown below:</para>
    /// 
    ///   <img src="..\images\niblack.png" />
    /// 
    /// </example>
    /// 
    /// <seealso cref="SauvolaThreshold"/>
    /// 
    public class NiblackThreshold : BaseFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;

        private int radius = 15;
        private double k = 0.2D;
        private double c = 0;

        /// <summary>
        ///   Gets or sets the filter convolution
        ///   radius. Default is 15.
        /// </summary>
        /// 
        public int Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        /// <summary>
        ///   Gets or sets the user-defined 
        ///   parameter k. Default is 0.2.
        /// </summary>
        /// 
        public double K
        {
            get { return k; }
            set { k = value; }
        }

        /// <summary>
        ///   Gets or sets the mean offset C. This value should
        ///   be between 0 and 255. The default value is 0.
        /// </summary>
        /// 
        public double C
        {
            get { return c; }
            set { c = value; }
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
        ///   Initializes a new instance of the <see cref="NiblackThreshold"/> class.
        /// </summary>
        /// 
        public NiblackThreshold()
        {
            formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
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
            if (sourceData.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                // for each line
                for (int y = 0; y < height; y++)
                {
                    // for each pixel
                    for (int x = 0; x < width; x++, src++, dst++)
                    {
                        long sum = 0;
                        int count = 0;

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

                                sum += src[ir * srcStride + jr];
                                count++;
                            }
                        }

                        double mean = sum / (double)count;
                        double variance = 0;

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

                                byte val = src[ir * srcStride + jr];
                                variance += (val - mean) * (val - mean);
                            }
                        }

                        variance /= count - 1;

                        double cut = mean + k * Math.Sqrt(variance) - c;

                        *dst = (*src > cut) ? (byte)255 : (byte)0;
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

                            if (t < 0) 
                                continue;
                            if (t >= height) 
                                break;

                            // for each kernel column
                            for (int j = 0; j < size; j++)
                            {
                                int jr = j - radius;
                                t = x + jr;

                                if (t < 0) 
                                    continue;
                                if (t >= width) 
                                    continue;

                                byte* p = &src[ir * srcStride + jr * pixelSize];

                                varR += (p[RGB.R] - meanR) * (p[RGB.R] - meanR);
                                varG += (p[RGB.G] - meanG) * (p[RGB.G] - meanG);
                                varB += (p[RGB.B] - meanB) * (p[RGB.B] - meanB);
                            }
                        }

                        varR /= count - 1;
                        varG /= count - 1;
                        varB /= count - 1;

                        double cutR = (meanR + k * Math.Sqrt(varR) - c);
                        double cutG = (meanG + k * Math.Sqrt(varG) - c);
                        double cutB = (meanB + k * Math.Sqrt(varB) - c);

                        dst[RGB.R] = (src[RGB.R] > cutR) ? (byte)255 : (byte)0;
                        dst[RGB.G] = (src[RGB.G] > cutG) ? (byte)255 : (byte)0;
                        dst[RGB.B] = (src[RGB.B] > cutB) ? (byte)255 : (byte)0;

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

