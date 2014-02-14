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
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using AForge.Imaging;
    using AForge.Imaging.Filters;

    /// <summary>
    ///   Compass convolution filter.
    /// </summary>
    /// 
    /// <seealso cref="RobinsonEdgeDetector"/>
    /// <seealso cref="KirschEdgeDetector"/>
    /// 
    public class CompassConvolution : BaseFilter
    {
        int[][,] masks;

        private Dictionary<PixelFormat, PixelFormat> formatTranslations;
        

        /// <summary>
        ///   Initializes a new instance of the <see cref="CompassConvolution"/> class.
        /// </summary>
        /// 
        public CompassConvolution(int[][,] masks)
        {
            this.masks = masks;
            formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
            formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
            formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format64bppArgb;
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
        protected unsafe override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
        {
            int width = sourceData.Width;
            int height = sourceData.Height;
            PixelFormat format = sourceData.PixelFormat;
            int pixelSize = System.Drawing.Bitmap.GetPixelFormatSize(format) / 8;

            sourceData.Clone();

            UnmanagedImage temp = UnmanagedImage.Create(width, height, format);

            int lineWidth = width * pixelSize;

            int srcStride = temp.Stride;
            int srcOffset = srcStride - lineWidth;
            int dstStride = destinationData.Stride;
            int dstOffset = dstStride - lineWidth;

            byte* srcStart = (byte*)temp.ImageData.ToPointer();
            byte* dstStart = (byte*)destinationData.ImageData.ToPointer();


            // first
            Convolution c = new Convolution(masks[0]);
            c.Apply(sourceData, destinationData);

            // others
            for (int i = 1; i < masks.Length; i++)
            {
                c.Kernel = masks[i];
                c.Apply(sourceData, temp);

                byte* src = srcStart;
                byte* dst = dstStart;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < lineWidth; x++, src++, dst++)
                    {
                        if (*src > *dst)
                            *dst = *src;
                    }

                    dst += dstOffset;
                    src += srcOffset;
                }
            }
        }

    }
}
