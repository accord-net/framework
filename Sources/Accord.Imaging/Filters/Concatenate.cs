// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
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
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge.Imaging;
    using AForge.Imaging.Filters;

    /// <summary>
    ///   Concatenation filter.
    /// </summary>
    /// 
    /// <remarks>
    ///   Concatenates two images side by side in a single image.
    /// </remarks>
    /// 
    public class Concatenate : BaseTransformationFilter
    {
       
        private Bitmap overlayImage;
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        
        /// <summary>
        ///   Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        ///   Creates a new concatenation filter.
        /// </summary>
        /// <param name="overlayImage">The first image to concatenate.</param>
        public Concatenate(Bitmap overlayImage)
        {
            this.overlayImage = overlayImage;
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
            formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
            formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
            formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format64bppArgb;
        }

        /// <summary>
        ///   Calculates new image size.
        /// </summary>
        protected override Size CalculateNewImageSize(UnmanagedImage sourceData)
        {
            int finalWidth = overlayImage.Width + sourceData.Width;
            int finalHeight = System.Math.Max(overlayImage.Height, sourceData.Height);
            return new Size(finalWidth, finalHeight);
        }


        /// <summary>
        ///   Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        ///
        protected override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
        {
            // Lock the overlay image (left image)
            BitmapData overlayData = overlayImage.LockBits(
                new Rectangle(0, 0, overlayImage.Width, overlayImage.Height),
                ImageLockMode.ReadOnly, overlayImage.PixelFormat);

            int dstHeight = destinationData.Height;
            int src1Height = overlayData.Height;
            int src2Height = sourceData.Height;

            int src1Stride = overlayData.Stride;
            int src2Stride = sourceData.Stride;
            int dstStride = destinationData.Stride;

             int pixelSize = System.Drawing.Image.GetPixelFormatSize(sourceData.PixelFormat) / 8;
             int copySize1 = overlayData.Width * pixelSize;
             int copySize2 = sourceData.Width * pixelSize;

            // do the job
            unsafe
            {
                byte* src1 = (byte*)overlayData.Scan0.ToPointer();
                byte* src2 = (byte*)sourceData.ImageData.ToPointer();
                byte* dst = (byte*)destinationData.ImageData.ToPointer();

                // for each line
                for (int y = 0; y < dstHeight; y++)
                {
                    if (y < src1Height)
                        AForge.SystemTools.CopyUnmanagedMemory(dst, src1, copySize1);
                    if (y < src2Height)
                        AForge.SystemTools.CopyUnmanagedMemory(dst + copySize1, src2, copySize2);
                    
                    src1 += src1Stride;
                    src2 += src2Stride;
                    dst += dstStride;
                }
            }

            // Release
            overlayImage.UnlockBits(overlayData);
        }
    }
}
