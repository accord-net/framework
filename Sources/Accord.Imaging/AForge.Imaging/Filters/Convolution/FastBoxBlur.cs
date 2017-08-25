// Accord Imaging Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Hashem Zawary, 2016
// hashemzawary@gmail.com
// https://www.linkedin.com/in/hashem-zavvari-53b01457
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
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    ///   Fast Box Blur filter.
    /// </summary>
    /// 
    /// <remarks>
    ///   Reference: http://www.vcskicks.com/box-blur.php
    /// </remarks>
    /// 
    public class FastBoxBlur : BaseInPlacePartialFilter
    {
        private byte _horizontalKernelSize = 3;
        private byte _verticalKernelSize = 3;
        private readonly Dictionary<PixelFormat, PixelFormat> _formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        /// 
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return _formatTranslations; }
        }


        /// <summary>
        /// Horizontal kernel size between 3 and 99.
        /// Default value is 3.
        /// </summary>
        /// 
        public byte HorizontalKernelSize
        {
            get { return _horizontalKernelSize; }
            set
            {
                _horizontalKernelSize = System.Math.Max((byte)3, System.Math.Min((byte)99, value));
            }
        }

        /// <summary>
        /// Vertical kernel size between 3 and 99.
        /// Default value is 3.
        /// </summary>
        /// 
        public byte VerticalKernelSize
        {
            get { return _verticalKernelSize; }
            set
            {
                _verticalKernelSize = System.Math.Max((byte)3, System.Math.Min((byte)99, value));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FastBoxBlur"/> class.
        /// </summary>
        /// 
        public FastBoxBlur()
        {
            // initialize format translation dictionary
            _formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            _formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            _formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            _formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
            _formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
            _formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FastBoxBlur"/> class.
        /// </summary>
        /// 
        /// <param name="horizontalKernelSize">Horizontal kernel size.</param>
        /// <param name="verticalKernelSize">Vertical kernel size.</param>
        /// 
        public FastBoxBlur(byte horizontalKernelSize, byte verticalKernelSize)
            : this()
        {
            HorizontalKernelSize = horizontalKernelSize;
            VerticalKernelSize = verticalKernelSize;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        protected override unsafe void ProcessFilter(UnmanagedImage image, Rectangle rect)
        {
            HorizontalBoxBlur(ref image, rect, KernelSizeInRange(_horizontalKernelSize));

            VerticalBoxBlur(ref image, rect, KernelSizeInRange(_verticalKernelSize));
        }

        static IntRange KernelSizeInRange(byte kernelSize)
        {
            kernelSize |= 1;
            int middleKernelSize = kernelSize / 2;

            return new IntRange(-middleKernelSize, middleKernelSize + 1);
        }

        static unsafe void HorizontalBoxBlur(ref UnmanagedImage image, Rectangle rect, IntRange kernelSizeRange)
        {
            var pixelSize = ((image.PixelFormat == PixelFormat.Format8bppIndexed) ||
                (image.PixelFormat == PixelFormat.Format16bppGrayScale)) ? 1 : 3;

            int startY = rect.Top;
            int stopY = startY + rect.Height;

            int startX = rect.Left * pixelSize;
            int stopX = startX + rect.Width * pixelSize;

            byte* basePtr = (byte*)image.ImageData.ToPointer();

            if (image.PixelFormat == PixelFormat.Format8bppIndexed
                || image.PixelFormat == PixelFormat.Format24bppRgb
                || image.PixelFormat == PixelFormat.Format32bppRgb
                || image.PixelFormat == PixelFormat.Format32bppArgb)
            {
                int offset = image.Stride - (stopX - startX);

                // align pointer to the first pixel to process
                byte* ptr = basePtr + (startY * image.Stride + rect.Left * pixelSize);

                for (int y = startY; y < stopY; y++)
                {
                    for (int x = startX; x < stopX; x++, ptr++)
                    {
                        int sum = 0;

                        for (int xFilter = kernelSizeRange.Min; xFilter < kernelSizeRange.Max; xFilter++)
                        {
                            int xBound = x / pixelSize + xFilter;

                            // Only if in bounds
                            if (xBound < 0 || xBound >= image.Width)
                                continue;

                            sum += ptr[xFilter * pixelSize];
                        }

                        *ptr = (byte)(sum / kernelSizeRange.Length);
                    }

                    ptr += offset;
                }

            }
            else // 16bpp per channel (ushort*)
            {
                int stride = image.Stride;

                // align pointer to the first pixel to process
                basePtr += (startY * image.Stride + rect.Left * pixelSize * 2);

                for (int y = startY; y < stopY; y++)
                {
                    ushort* ptr = (ushort*)(basePtr);

                    for (var x = startX; x < stopX; x++, ptr++)
                    {
                        int sum = 0;

                        for (var xFilter = kernelSizeRange.Min; xFilter < kernelSizeRange.Max; xFilter++)
                        {
                            int xBound = x / pixelSize + xFilter;

                            //Only if in bounds
                            if (xBound < 0 || xBound >= image.Width) continue;

                            sum += ptr[xFilter * pixelSize];
                        }

                        *ptr = (ushort)(sum / kernelSizeRange.Length);
                    }

                    basePtr += stride;
                }
            }
        }

        static unsafe void VerticalBoxBlur(ref UnmanagedImage image, Rectangle rect, IntRange kernelSizeRange)
        {
            var pixelSize = ((image.PixelFormat == PixelFormat.Format8bppIndexed) ||
                (image.PixelFormat == PixelFormat.Format16bppGrayScale)) ? 1 : 3;

            int startY = rect.Top;
            int stopY = startY + rect.Height;

            int startX = rect.Left * pixelSize;
            int stopX = startX + rect.Width * pixelSize;

            byte* basePtr = (byte*)image.ImageData.ToPointer();

            if (image.PixelFormat == PixelFormat.Format8bppIndexed ||
                image.PixelFormat == PixelFormat.Format24bppRgb ||
                image.PixelFormat == PixelFormat.Format32bppArgb ||
                image.PixelFormat == PixelFormat.Format32bppRgb)
            {
                int offset = image.Stride - (stopX - startX);

                // align pointer to the first pixel to process
                var ptr = basePtr + (startY * image.Stride + rect.Left * pixelSize);

                for (int y = startY; y < stopY; y++)
                {
                    for (int x = startX; x < stopX; x++, ptr++)
                    {
                        int sum = 0;

                        for (int yFilter = kernelSizeRange.Min; yFilter < kernelSizeRange.Max; yFilter++)
                        {
                            int yBound = y + yFilter;

                            //Only if in bounds
                            if (yBound < 0 || yBound >= image.Height)
                                continue;

                            sum += ptr[yFilter * image.Stride];
                        }

                        *ptr = (byte)(sum / kernelSizeRange.Length);
                    }

                    ptr += offset;
                }
            }
            else // 16bpp per channel (ushort*)
            {
                // align pointer to the first pixel to process
                basePtr += (startY * image.Stride + rect.Left * pixelSize * 2);

                for (int y = startY; y < stopY; y++)
                {
                    ushort* ptr = (ushort*)(basePtr);

                    for (int x = startX; x < stopX; x++, ptr++)
                    {
                        int sum = 0;

                        for (int yFilter = kernelSizeRange.Min; yFilter < kernelSizeRange.Max; yFilter++)
                        {
                            int yBound = y + yFilter;

                            //Only if in bounds
                            if (yBound < 0 || yBound >= image.Height)
                                continue;

                            sum += ptr[yFilter * image.Stride / 2];
                        }

                        *ptr = (ushort)(sum / kernelSizeRange.Length);
                    }

                    basePtr += image.Stride;
                }
            }

        }
    }
}
