// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@aforgenet.com
//
// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
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
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Resize image using bilinear interpolation algorithm.
    /// </summary>
    /// 
    /// <remarks><para>The class implements image resizing filter using bilinear
    /// interpolation algorithm.</para>
    /// 
    /// <para>The filter accepts 8 grayscale images and 24/32 bpp
    /// color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// ResizeBilinear filter = new ResizeBilinear( 400, 300 );
    /// // apply the filter
    /// Bitmap newImage = filter.Apply( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="..\images\imaging\sample9.png" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="..\images\imaging\resize_bilinear.png" width="400" height="300" />
    /// </remarks>
    /// 
    /// <seealso cref="ResizeNearestNeighbor"/>
    /// <seealso cref="ResizeBicubic"/>
    ///
    public class ResizeBilinear : BaseResizeFilter
    {
        // format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResizeBilinear"/> class.
        /// </summary>
        /// 
        /// <param name="newWidth">Width of the new image.</param>
        /// <param name="newHeight">Height of the new image.</param>
        /// 
		public ResizeBilinear(int newWidth, int newHeight) :
            base(newWidth, newHeight)
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
            formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
            formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
            formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format64bppArgb;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        /// 
        protected override unsafe void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
        {
            // get source image size
            int width = sourceData.Width;
            int height = sourceData.Height;

            int pixelSize = sourceData.PixelSize;
            int srcStride = sourceData.Stride;
            int dstOffset = destinationData.Offset;
            double xFactor = (double)width / newWidth;
            double yFactor = (double)height / newHeight;

            // do the job
            byte* src = (byte*)sourceData.ImageData.ToPointer();
            byte* dst = (byte*)destinationData.ImageData.ToPointer();

            // width and height decreased by 1
            int ymax = height - 1;
            int xmax = width - 1;
            // temporary pointers

            // for each line
            for (int y = 0; y < newHeight; y++)
            {
                // Y coordinates
                double oy = (double)y * yFactor;
                int oy1 = (int)oy;
                int oy2 = (oy1 == ymax) ? oy1 : oy1 + 1;
                double dy1 = oy - (double)oy1;
                double dy2 = 1.0 - dy1;

                // get temp pointers
                byte* tp1 = src + oy1 * srcStride;
                byte* tp2 = src + oy2 * srcStride;

                // for each pixel
                for (int x = 0; x < newWidth; x++)
                {
                    // X coordinates
                    double ox = (double)x * xFactor;
                    int ox1 = (int)ox;
                    int ox2 = (ox1 == xmax) ? ox1 : ox1 + 1;
                    double dx1 = ox - (double)ox1;
                    double dx2 = 1.0 - dx1;

                    // get four points
                    byte* p1 = tp1 + ox1 * pixelSize;
                    byte* p2 = tp1 + ox2 * pixelSize;
                    byte* p3 = tp2 + ox1 * pixelSize;
                    byte* p4 = tp2 + ox2 * pixelSize;

                    // interpolate using 4 points
                    for (int i = 0; i < pixelSize; i++, dst++, p1++, p2++, p3++, p4++)
                    {
                        *dst = (byte)(
                            dy2 * (dx2 * (*p1) + dx1 * (*p2)) +
                            dy1 * (dx2 * (*p3) + dx1 * (*p4)));
                    }
                }
                dst += dstOffset;
            }
        }
    }
}
