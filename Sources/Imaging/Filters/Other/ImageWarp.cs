// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2010
// andrew.kirillov@aforgenet.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Image warp effect filter.
    /// </summary>
    /// 
    /// <remarks><para>The image processing filter implements a warping filter, which
    /// sets pixels in destination image to values from source image taken with specified offset
    /// (see <see cref="WarpMap"/>).
    /// </para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images and 24/32
    /// color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // build warp map
    /// int width  = image.Width;
    /// int height = image.Height;
    /// 
    /// IntPoint[,] warpMap = new IntPoint[height, width];
    ///
    /// int size = 8;
    /// int maxOffset = -size + 1;
    ///
    /// for ( int y = 0; y &lt; height; y++ )
    /// {
    ///     for ( int x = 0; x &lt; width; x++ )
    ///     {
    ///         int dx = ( x / size ) * size - x;
    ///         int dy = ( y / size ) * size - y;
    ///
    ///         if ( dx + dy &lt;= maxOffset )
    ///         {
    ///             dx = ( x / size + 1 ) * size - 1 - x;
    ///         }
    ///
    ///         warpMap[y, x] = new IntPoint( dx, dy );
    ///     }
    /// }
    /// // create filter
    /// ImageWarp filter = new ImageWarp( warpMap );
    /// // apply the filter
    /// Bitmap newImage = filter.Apply( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/image_warp.png" width="480" height="361" />
    /// </remarks>
    /// 
    public class ImageWarp : BaseFilter
    {
        // private format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>( );

        private IntPoint[,] warpMap = null;

        /// <summary>
        /// Map used for warping images.
        /// </summary>
        /// 
        /// <remarks><para>The property sets displacement map used for warping images.
        /// The map sets offsets of pixels in source image, which are used to set values in destination
        /// image. In other words, each pixel in destination image is set to the same value
        /// as pixel in source image with corresponding offset (coordinates of pixel in source image
        /// are calculated as sum of destination coordinate and corresponding value from warp map).
        /// </para>
        /// 
        /// <para><note>The map array is accessed using [y, x] indexing, i.e.
        /// first dimension in the map array corresponds to Y axis of image.</note></para>
        /// 
        /// <para><note>If the map is smaller or bigger than the image to process, then only minimum
        /// overlapping area of the image is processed. This allows to prepare single big map and reuse
        /// it for a set of images for creating similar effects.</note></para>
        /// </remarks>
        /// 
        public IntPoint[,] WarpMap
        {
            get { return warpMap; }
            set
            {
                if ( value == null )
                    throw new NullReferenceException( "Warp map can not be set to null." );

                warpMap = value;
            }
        }

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        /// 
        /// <remarks><para>See <see cref="IFilterInformation.FormatTranslations"/>
        /// documentation for additional information.</para></remarks>
        /// 
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageWarp"/> class.
        /// </summary>
        /// 
        /// <param name="warpMap">Map used for warping images (see <see cref="WarpMap"/>).</param>
        /// 
        public ImageWarp( IntPoint[,] warpMap )
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb]    = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb]   = PixelFormat.Format32bppArgb;

            WarpMap = warpMap;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="source">Source image data.</param>
        /// <param name="destination">Destination image data.</param>
        /// 
        protected override unsafe void ProcessFilter( UnmanagedImage source, UnmanagedImage destination )
        {
            int pixelSize = Image.GetPixelFormatSize( source.PixelFormat ) / 8;

            // image width and height
            int width  = source.Width;
            int height = source.Height;

            int widthToProcess  = Math.Min( width,  warpMap.GetLength( 1 ) );
            int heightToProcess = Math.Min( height, warpMap.GetLength( 0 ) );

            int srcStride = source.Stride;
            int dstStride = destination.Stride;
            int dstOffset = dstStride - widthToProcess * pixelSize;

            // new pixel's position
            int ox, oy;

            byte* src = (byte*) source.ImageData.ToPointer( );
            byte* dst = (byte*) destination.ImageData.ToPointer( );
            byte* p;

            // for each line
            for ( int y = 0; y < heightToProcess; y++ )
            {
                // for each pixel
                for ( int x = 0; x < widthToProcess; x++ )
                {
                    // get original pixel's coordinates
                    ox = x + warpMap[y, x].X;
                    oy = y + warpMap[y, x].Y;

                    // check if the random pixel is inside of image
                    if ( ( ox >= 0 ) && ( oy >= 0 ) && ( ox < width ) && ( oy < height ) )
                    {
                        p = src + oy * srcStride + ox * pixelSize;

                        for ( int i = 0; i < pixelSize; i++, dst++, p++ )
                        {
                            *dst = *p;
                        }
                    }
                    else
                    {
                        for ( int i = 0; i < pixelSize; i++, dst++ )
                        {
                            *dst = 0;
                        }
                    }
                }

                // copy remaining pixel in the row
                if ( width != widthToProcess )
                {
                    AForge.SystemTools.CopyUnmanagedMemory( dst, src + y * srcStride + widthToProcess * pixelSize, ( width - widthToProcess ) * pixelSize );
                }

                dst += dstOffset;
            }

            // copy remaining rows of pixels
            for ( int y = heightToProcess; y < height; y++, dst += dstStride )
            {
                AForge.SystemTools.CopyUnmanagedMemory( dst, src + y * srcStride, width * pixelSize );
            }
        }
    }
}
