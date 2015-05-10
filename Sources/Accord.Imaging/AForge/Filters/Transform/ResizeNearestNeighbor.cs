// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@aforgenet.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Resize image using nearest neighbor algorithm.
    /// </summary>
    /// 
    /// <remarks><para>The class implements image resizing filter using nearest
    /// neighbor algorithm, which does not assume any interpolation.</para>
    /// 
    /// <para>The filter accepts 8 and 16 bpp grayscale images and 24, 32, 48 and 64 bpp
    /// color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// ResizeNearestNeighbor filter = new ResizeNearestNeighbor( 400, 300 );
    /// // apply the filter
    /// Bitmap newImage = filter.Apply( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample9.png" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/resize_nearest.png" width="400" height="300" />
    /// </remarks>
    /// 
    /// <seealso cref="ResizeBilinear"/>
    /// <seealso cref="ResizeBicubic"/>
    /// 
    public class ResizeNearestNeighbor : BaseResizeFilter
    {
        // format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>( );

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResizeNearestNeighbor"/> class.
        /// </summary>
        /// 
        /// <param name="newWidth">Width of the new image.</param>
        /// <param name="newHeight">Height of the new image.</param>
        /// 
		public ResizeNearestNeighbor( int newWidth, int newHeight ) :
            base( newWidth, newHeight )
		{
            formatTranslations[PixelFormat.Format8bppIndexed]    = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]       = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb]       = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb]      = PixelFormat.Format32bppArgb;
            formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
            formatTranslations[PixelFormat.Format48bppRgb]       = PixelFormat.Format48bppRgb;
            formatTranslations[PixelFormat.Format64bppArgb]      = PixelFormat.Format64bppArgb;
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
            // get source image size
            int width   = sourceData.Width;
            int height  = sourceData.Height;

            int pixelSize = Image.GetPixelFormatSize( sourceData.PixelFormat ) / 8;
            int srcStride = sourceData.Stride;
            int dstStride = destinationData.Stride;
            double xFactor = (double) width / newWidth;
            double yFactor = (double) height / newHeight;

            // do the job
            byte* baseSrc = (byte*) sourceData.ImageData.ToPointer( );
            byte* baseDst = (byte*) destinationData.ImageData.ToPointer( );

            // for each line
            for ( int y = 0; y < newHeight; y++ )
            {
                byte* dst = baseDst + dstStride * y;
                byte* src = baseSrc + srcStride * ( (int) ( y * yFactor ) );
                byte* p;

                // for each pixel
                for ( int x = 0; x < newWidth; x++ )
                {
                    p = src + pixelSize * ( (int) ( x * xFactor ) );

                    for ( int i = 0; i < pixelSize; i++, dst++, p++ )
                    {
                        *dst = *p;
                    }
                }
            }
        }
    }
}
