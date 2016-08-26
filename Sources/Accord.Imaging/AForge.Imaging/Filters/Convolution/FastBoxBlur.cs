// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//
// Implemented FastBoxBlur filter by HZ, March-2016
// Reference link: http://www.vcskicks.com/box-blur.php (The filter is simple and fast to BoxBlur).
// hashemzawary@gmail.com
// https://www.linkedin.com/in/hashem-zavvari-53b01457
//

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
    /// <summary>
    /// FastBoxBlur filter.
    /// </summary>
    public class FastBoxBlur : BaseInPlacePartialFilter
    {
        // private format translation dictionary
        private readonly Dictionary<PixelFormat, PixelFormat> _formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => _formatTranslations;

        private byte _horizontalKernelSize = 3;

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

        private byte _verticalKernelSize = 3;
        /// <summary>
        /// Vertical kernel size between 3 and 99.
        /// Default value is 3.
        /// </summary>
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
            _formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
            _formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FastBoxBlur"/> class.
        /// </summary>
        /// 
        /// <param name="horizontalKernelSize">Horizontal kernel size.</param>
        /// <param name="verticalKernelSize">Vertical kernel size.</param>
        public FastBoxBlur(byte horizontalKernelSize, byte verticalKernelSize) : this()
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
            if (_horizontalKernelSize != 1)
                HorizontalBoxBlur(ref image, rect, KernelSizeInRange(_horizontalKernelSize));

            if (_verticalKernelSize != 1)
                VerticalBoxBlur(ref image, rect, KernelSizeInRange(_verticalKernelSize));
        }

        static IntRange KernelSizeInRange(byte kernelSize)
        {
            kernelSize |= 1;
            var middleKernelSize = kernelSize / 2;

            return new IntRange(-middleKernelSize, middleKernelSize + 1);
        }

        static unsafe void HorizontalBoxBlur(ref UnmanagedImage image, Rectangle rect, IntRange kernelSizeRange)
        {
            var pixelSize = ((image.PixelFormat == PixelFormat.Format8bppIndexed) ||
                (image.PixelFormat == PixelFormat.Format16bppGrayScale)) ? 1 : 3;

            var startY = rect.Top;
            var stopY = startY + rect.Height;

            var startX = rect.Left * pixelSize;
            var stopX = startX + rect.Width * pixelSize;

            var basePtr = (byte*)image.ImageData.ToPointer();

            if (
                (image.PixelFormat == PixelFormat.Format8bppIndexed) ||
                (image.PixelFormat == PixelFormat.Format24bppRgb))
            {
                var offset = image.Stride - (stopX - startX);

                // allign pointer to the first pixel to process
                var ptr = basePtr + (startY * image.Stride + rect.Left * pixelSize);

                for (var y = startY; y < stopY; y++)
                {
                    for (var x = startX; x < stopX; x++, ptr++)
                    {
                        var sum = 0;

                        for (var xFilter = kernelSizeRange.Min; xFilter < kernelSizeRange.Max; xFilter++)
                        {
                            var xBound = x / pixelSize + xFilter;

                            //Only if in bounds
                            if (xBound < 0 || xBound >= image.Width) continue;

                            sum += ptr[xFilter * pixelSize];
                        }
                        *ptr = (byte)(sum / kernelSizeRange.Length);
                    }
                    ptr += offset;
                }

            }
            else
            {
                var stride = image.Stride;

                // allign pointer to the first pixel to process
                basePtr += (startY * image.Stride + rect.Left * pixelSize * 2);

                for (var y = startY; y < stopY; y++)
                {
                    var ptr = (ushort*)(basePtr);

                    for (var x = startX; x < stopX; x++, ptr++)
                    {
                        var sum = 0;

                        for (var xFilter = kernelSizeRange.Min; xFilter < kernelSizeRange.Max; xFilter++)
                        {
                            var xBound = x / pixelSize + xFilter;

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

            var startY = rect.Top;
            var stopY = startY + rect.Height;

            var startX = rect.Left * pixelSize;
            var stopX = startX + rect.Width * pixelSize;

            var basePtr = (byte*)image.ImageData.ToPointer();

            if (
                (image.PixelFormat == PixelFormat.Format8bppIndexed) ||
                (image.PixelFormat == PixelFormat.Format24bppRgb))
            {
                var offset = image.Stride - (stopX - startX);

                // allign pointer to the first pixel to process
                var ptr = basePtr + (startY * image.Stride + rect.Left * pixelSize);

                for (var y = startY; y < stopY; y++)
                {
                    for (var x = startX; x < stopX; x++, ptr++)
                    {
                        var sum = 0;

                        for (var yFilter = kernelSizeRange.Min; yFilter < kernelSizeRange.Max; yFilter++)
                        {
                            var yBound = y + yFilter;

                            //Only if in bounds
                            if (yBound < 0 || yBound >= image.Height) continue;

                            sum += ptr[yFilter * image.Stride];
                        }
                        *ptr = (byte)(sum / kernelSizeRange.Length);
                    }
                    ptr += offset;
                }

            }
            else
            {
                // allign pointer to the first pixel to process
                basePtr += (startY * image.Stride + rect.Left * pixelSize * 2);

                for (var y = startY; y < stopY; y++)
                {
                    var ptr = (ushort*)(basePtr);

                    for (var x = startX; x < stopX; x++, ptr++)
                    {
                        var sum = 0;

                        for (var yFilter = kernelSizeRange.Min; yFilter < kernelSizeRange.Max; yFilter++)
                        {
                            var yBound = y + yFilter;

                            //Only if in bounds
                            if (yBound < 0 || yBound >= image.Height) continue;

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