// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//
// Implemented Divide filter by HZ, March-2016
// hashemzawary@gmail.com
// https://www.linkedin.com/in/hashem-zavvari-53b01457
//

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
    /// <summary>
    /// Divide filter - divide pixel values of two images.
    /// </summary>
    /// 
    /// <remarks><para>The divide filter takes two images (source and overlay images)
    /// of the same size and pixel format and produces an image, where each pixel equals
    /// to the divide value of corresponding pixels from provided images (
    /// for 8bpp: (srcPix * 255f + 1f) / (ovrPix + 1f), 
    /// for 16bpp: (srcPix * 65535f + 1f) / (ovrPix + 1f).</para>
    /// 
    /// <para>The filter accepts 8 and 16 bpp grayscale images and 24, 32, 48 and 64 bpp
    /// color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// Divide filter = new Divide( overlayImage );
    /// // apply the filter
    /// Bitmap resultImage = filter.Apply( sourceImage );
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="Merge"/>
    /// <seealso cref="Intersect"/>
    /// <seealso cref="Add"/>
    /// <seealso cref="Difference"/>
    /// <seealso cref="Multiply"/>
    /// 
    public sealed class Divide : BaseInPlaceFilter2
    {
        // private format translation dictionary
        private readonly Dictionary<PixelFormat, PixelFormat> _formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => _formatTranslations;

        /// <summary>
        /// Initializes a new instance of the <see cref="Divide"/> class.
        /// </summary>
        public Divide()
        {
            InitFormatTranslations();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Divide"/> class.
        /// </summary>
        /// 
        /// <param name="overlayImage">Overlay image</param>
        /// 
        public Divide(Bitmap overlayImage)
            : base(overlayImage)
        {
            InitFormatTranslations();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Divide"/> class.
        /// </summary>
        /// 
        /// <param name="unmanagedOverlayImage">Unmanaged overlay image.</param>
        /// 
        public Divide(UnmanagedImage unmanagedOverlayImage)
            : base(unmanagedOverlayImage)
        {
            InitFormatTranslations();
        }

        // Initialize format translation dictionary
        private void InitFormatTranslations()
        {
            _formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            _formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            _formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            _formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
            _formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
            _formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
            _formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format64bppArgb;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        /// <param name="overlay">Overlay image data.</param>
        ///
        protected override unsafe void ProcessFilter(UnmanagedImage image, UnmanagedImage overlay)
        {
            var pixelFormat = image.PixelFormat;
            // get image dimension
            var width = image.Width;
            var height = image.Height;

            if (
                (pixelFormat == PixelFormat.Format8bppIndexed) ||
                (pixelFormat == PixelFormat.Format24bppRgb) ||
                (pixelFormat == PixelFormat.Format32bppRgb) ||
                (pixelFormat == PixelFormat.Format32bppArgb))
            {

                // initialize other variables
                var pixelSize = (pixelFormat == PixelFormat.Format8bppIndexed) ? 1 :
                    (pixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
                var lineSize = width * pixelSize;
                var srcOffset = image.Stride - lineSize;
                var ovrOffset = overlay.Stride - lineSize;
                // new pixel value

                // do the job
                var ptr = (byte*)image.ImageData.ToPointer();
                var ovr = (byte*)overlay.ImageData.ToPointer();

                // for each line
                for (var y = 0; y < height; y++)
                {
                    // for each pixel
                    for (var x = 0; x < lineSize; x++, ptr++, ovr++)
                    {
                        var v = (*ptr * 256f) / (*ovr + 1f);
                        *ptr = (v > 255) ? (byte)255 : (byte)v;
                    }
                    ptr += srcOffset;
                    ovr += ovrOffset;
                }
            }
            else
            {
                // initialize other variables
                var pixelSize = (pixelFormat == PixelFormat.Format16bppGrayScale) ? 1 :
                    (pixelFormat == PixelFormat.Format48bppRgb) ? 3 : 4;
                var lineSize = width * pixelSize;
                var srcStride = image.Stride;
                var ovrStride = overlay.Stride;
                // new pixel value

                // do the job
                var basePtr = (byte*)image.ImageData.ToPointer();
                var baseOvr = (byte*)overlay.ImageData.ToPointer();

                // for each line
                for (var y = 0; y < height; y++)
                {
                    var ptr = (ushort*)(basePtr + y * srcStride);
                    var ovr = (ushort*)(baseOvr + y * ovrStride);

                    // for each pixel
                    for (var x = 0; x < lineSize; x++, ptr++, ovr++)
                    {
                        var v = (*ptr * 65536f) / (*ovr + 1f);
                        *ptr = (v > 65535) ? (ushort)65535 : (ushort)v;
                    }
                }
            }
        }
    }
}
