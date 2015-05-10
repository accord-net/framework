// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.comm
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Convert grayscale image to RGB.
    /// </summary>
    /// 
    /// <remarks><para>The filter creates color image from specified grayscale image
    /// initializing all RGB channels to the same value - pixel's intensity of grayscale image.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images and produces
    /// 24 bpp RGB image.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// GrayscaleToRGB filter = new GrayscaleToRGB( );
    /// // apply the filter
    /// Bitmap rgbImage = filter.Apply( image );
    /// </code>
    /// 
    /// </remarks>
    /// 
    public sealed class GrayscaleToRGB : BaseFilter
    {
        // private format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>( );

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GrayscaleToRGB"/> class.
        /// </summary>
        /// 
        public GrayscaleToRGB( )
        {
            // initialize format translation dictionary
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format24bppRgb;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        /// 
        protected override unsafe void ProcessFilter( UnmanagedImage sourceData, UnmanagedImage destinationData )
        {
            // get width and height
            int width = sourceData.Width;
            int height = sourceData.Height;

            int srcOffset = sourceData.Stride - width;
            int dstOffset = destinationData.Stride - width * 3;

            // do the job
            byte * src = (byte*) sourceData.ImageData.ToPointer( );
            byte * dst = (byte*) destinationData.ImageData.ToPointer( );

            // for each line
            for ( int y = 0; y < height; y++ )
            {
                // for each pixel
                for ( int x = 0; x < width; x++, src++, dst += 3 )
                {
                    dst[RGB.R] = dst[RGB.G] = dst[RGB.B] = *src;
                }
                src += srcOffset;
                dst += dstOffset;
            }
        }
    }
}
