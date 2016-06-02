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
    using Accord.Imaging;
    using Accord.Imaging.Filters;
    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;

    /// <summary>
    ///   Wolf Jolion Threshold.
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>
    ///   The Wolf-Jolion threshold filter is a variation 
    ///   of the <see cref="SauvolaThreshold"/> filter.</para>
    ///   
    /// <para>
    ///  This filter implementation has been contributed by Diego Catalano.</para>
    ///  
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///     <a href="http://liris.cnrs.fr/christian.wolf/papers/icpr2002v.pdf">
    ///         C. Wolf, J.M. Jolion, F. Chassaing. "Text Localization, Enhancement and 
    ///         Binarization in Multimedia Documents." Proceedings of the 16th International 
    ///         Conference on Pattern Recognition, 2002. 
    ///         Available in http://liris.cnrs.fr/christian.wolf/papers/icpr2002v.pdf </a></description></item>
    ///   </list></para>   
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// Bitmap image = ... // Lena's picture
    /// 
    /// // Create a new Wolf-Joulion threshold:
    /// var wolfJoulion = new WolfJoulionThreshold();
    /// 
    /// // Compute the filter
    /// Bitmap result = wolfJoulion.Apply(image);
    /// 
    /// // Show on the screen
    /// ImageBox.Show(result);
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="NiblackThreshold"/>
    /// <seealso cref="SauvolaThreshold"/>
    /// 
    public class WolfJolionThreshold : BaseFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;

        private int radius = 15;
        private double k = 0.5D;
        private double r = 128;

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
        ///   parameter k. Default is 0.5.
        /// </summary>
        /// 
        public double K
        {
            get { return k; }
            set { k = value; }
        }

        /// <summary>
        ///   Gets or sets the dynamic range of the 
        ///   standard deviation, R. Default is 128.
        /// </summary>
        /// 
        public double R
        {
            get { return r; }
            set { r = value; }
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
        ///   Initializes a new instance of the <see cref="WolfJolionThreshold"/> class.
        /// </summary>
        /// 
        public WolfJolionThreshold()
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

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(sourceData.PixelFormat) / 8;

            int srcStride = sourceData.Stride;
            int dstStride = destinationData.Stride;

            int srcOffset = srcStride - width * pixelSize;
            int dstOffset = dstStride - width * pixelSize;

            byte* src = (byte*)sourceData.ImageData.ToPointer();
            byte* dst = (byte*)destinationData.ImageData.ToPointer();

            // TODO: Move or cache the creation of those filters
            int[,] kernel = Accord.Math.Matrix.Create(radius * 2 + 1, radius * 2 + 1, 1);
            Convolution conv = new Convolution(kernel);
            FastVariance fv = new FastVariance(radius);

            // Mean filter
            UnmanagedImage mean = conv.Apply(sourceData);

            // Variance filter
            UnmanagedImage var = fv.Apply(sourceData);

            // do the processing job
            if (sourceData.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                byte* srcVar = (byte*)var.ImageData.ToPointer();
                byte* srcMean = (byte*)mean.ImageData.ToPointer();

                // Store maximum value from variance.
                int maxV = Max(width, height, srcVar, srcOffset);

                // Store minimum value from image.
                int minG = Min(width, height, src, srcOffset);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, src++, srcMean++, srcVar++, dst++)
                    {
                        double mP = *srcMean;
                        double vP = *srcVar;

                        double threshold = (mP + k * ((Math.Sqrt(vP) / (double)maxV - 1.0) * (mP - (double)minG)));

                        *dst = (byte)(*src > threshold ? 255 : 0);
                    }

                    src += srcOffset;
                    srcMean += srcOffset;
                    srcVar += srcOffset;
                    dst += dstOffset;
                }
            }
        }

        unsafe private static int Max(int width, int height, byte* src, int offset)
        {
            int max = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++, src++)
                {
                    if (*src > max)
                        max = *src;
                }

                src += offset;
            }

            return max;
        }

        unsafe private static int Min(int width, int height, byte* src, int offset)
        {
            int max = 255;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++, src++)
                {
                    if (*src > max)
                        max = *src;
                }

                src += offset;
            }

            return max;
        }
    }
}