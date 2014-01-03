// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
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
    using AForge.Imaging;
    using AForge.Imaging.Filters;

    /// <summary>
    ///   Kuwahara filter.
    /// </summary>
    /// 
    /// <example>
    /// <code>
    ///   Bitmap image = ... // Lena's famous picture
    /// 
    ///   // Create a new Kuwahara filter
    ///   Kuwahara kuwahara = new Kuwahara();
    /// 
    ///   // Apply the Kuwahara filter
    ///   Bitmap result = kuwahara.Apply(image);
    ///   
    ///   // Show on the screen
    ///   ImageBox.Show(result);
    /// </code>
    /// </example>
    /// 
    public class Kuwahara : BaseInPlaceFilter
    {
        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        private int kernelSize = 5;
        private int blockSize = 2;

        /// <summary>
        ///   Gets the size of the kernel used in the Kuwahara filter. This
        ///   should be odd and greater than or equal to five. Default is 5.
        /// </summary>
        /// 
        public int Size
        {
            get { return kernelSize; }
            set
            {
                if (value % 2 == 0 || value < 5)
                    throw new ArgumentOutOfRangeException("value", 
                        "Size must be odd and greater than or equal to 5.");

                kernelSize = value;
                blockSize = kernelSize / 2;
            }
        }

        /// <summary>
        ///   Gets the size of each of the four inner blocks used in the
        ///   Kuwahara filter. This is always half the <see cref="Size">
        ///   kernel size</see> minus one.
        /// </summary>
        /// 
        /// <value>
        ///   The size of the each inner block, or <c>k / 2 - 1</c> 
        ///   where <c>k</c> is the kernel size.
        /// </value>
        /// 
        public int BlockSize
        {
            get { return blockSize; }
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
        ///   Initializes a new instance of the <see cref="Kuwahara"/> class.
        /// </summary>
        /// 
        public Kuwahara()
        {
            formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
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

            int stride = image.Stride;
            int offset = stride - width;


            int b = blockSize;
            int blocksX = width - b;
            int blocksY = height - b;
            double count = b * b;

            double[,] mean = new double[blocksY, blocksX];
            double[,] var = new double[blocksY, blocksX];

            byte* src = (byte*)image.ImageData.ToPointer();
            for (int y = 0; y < height - b; y++)
            {
                for (int x = 0; x < width - b; x++, src++)
                {
                    mean[y, x] = UnsafeTools.Sum(src, b, b, stride) / count;
                    var[y, x] = UnsafeTools.Scatter(src, b, b, stride, mean[y, x]);
                }
                src += offset + b;
            }


            src = (byte*)image.ImageData.ToPointer() + b * stride + b;
            for (int y = b; y < height - b - 1; y++)
            {
                for (int x = b; x < width - b - 1; x++, src++)
                {
                    // variances
                    double va = var[y - b, x - b], vb = var[y - b, x + 1];
                    double vc = var[y + 1, x - b], vd = var[y + 1, x + 1];

                    // means
                    double ma = mean[y - b, x - b], mb = mean[y - b, x + 1];
                    double mc = mean[y + 1, x - b], md = mean[y + 1, x + 1];

                    double value = min(va, vb, vc, vd,
                                       ma, mb, mc, md);

                    *src = (byte)(value > 255 ? 255 : (value < 0 ? 0 : value));
                }

                src += offset + 2 * b + 1;
            }
        }

        unsafe private static double min(
            double varA, double varB, double varC, double varD,
            double meanA, double meanB, double meanC, double meanD)
        {
            double varMin = varA;
            double mean = meanA;

            if (varB < varMin)
            {
                varMin = varB;
                mean = meanB;
            }

            if (varC < varMin)
            {
                varMin = varC;
                mean = meanC;
            }

            if (varD < varMin)
            {
                varMin = varD;
                mean = meanD;
            }

            return mean;
        }
    }
}
