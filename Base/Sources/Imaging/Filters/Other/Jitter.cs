// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//
// Original idea from CxImage
// http://www.codeproject.com/bitmap/cximage.asp
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Jitter filter.
    /// </summary>
    /// 
    /// <remarks><para>The filter moves each pixel of a source image in
    /// random direction within a window of specified <see cref="Radius">radius</see>.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images and 24/32
    /// color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// Jitter filter = new Jitter( 4 );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/jitter.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    public class Jitter : BaseUsingCopyPartialFilter
    {
        private int radius = 2;

        // random number generator
        private Random rand = new Random( );

        // private format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>( );

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
        /// Jittering radius, [1, 10]
        /// </summary>
        /// 
        /// <remarks><para>Determines radius in which pixels can move.</para>
        /// 
        /// <para>Default value is set to <b>2</b>.</para>
        /// </remarks>
        /// 
        public int Radius
        {
            get { return radius; }
            set { radius = Math.Max( 1, Math.Min( 10, value ) ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jitter"/> class.
        /// </summary>
        public Jitter( )
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb]    = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb]   = PixelFormat.Format32bppArgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jitter"/> class.
        /// </summary>
        /// 
        /// <param name="radius">Jittering radius.</param>
        /// 
        public Jitter( int radius ) : this( )
        {
            Radius = radius;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="source">Source image data.</param>
        /// <param name="destination">Destination image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        protected override unsafe void ProcessFilter( UnmanagedImage source, UnmanagedImage destination, Rectangle rect )
        {
            int pixelSize = Image.GetPixelFormatSize( source.PixelFormat ) / 8;

            // processing start and stop X,Y positions
            int startX  = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;

            int srcStride = source.Stride;
            int dstStride = destination.Stride;
            int dstOffset = dstStride - rect.Width * pixelSize;

            // new pixel's position
            int ox, oy;

            // maximum value for random number generator
            int max = radius * 2 + 1;

            byte* src = (byte*) source.ImageData.ToPointer( );
            byte* dst = (byte*) destination.ImageData.ToPointer( );
            byte* p;

            // copy source to destination before
            if ( srcStride == dstStride )
            {
                AForge.SystemTools.CopyUnmanagedMemory( dst, src, srcStride * source.Height );
            }
            else
            {
                int len = source.Width * pixelSize;

                for ( int y = 0, heigh = source.Height; y < heigh; y++ )
                {
                    AForge.SystemTools.CopyUnmanagedMemory(
                        dst + dstStride * y, src + srcStride * y, len );
                }
            }

            // allign pointer to the first pixel to process
            dst += ( startY * dstStride + startX * pixelSize );

            // Note:
            // It is possible to speed-up this filter creating separate
            // loops for RGB and grayscale images.

            // for each line
            for ( int y = startY; y < stopY; y++ )
            {
                // for each pixel
                for ( int x = startX; x < stopX; x++ )
                {
                    // generate radnom pixel's position
                    ox = x + rand.Next( max ) - radius;
                    oy = y + rand.Next( max ) - radius;

                    // check if the random pixel is inside our image
                    if ( ( ox >= startX ) && ( oy >= startY ) && ( ox < stopX ) && ( oy < stopY ) )
                    {
                        p = src + oy * srcStride + ox * pixelSize;

                        for ( int i = 0; i < pixelSize; i++, dst++, p++ )
                        {
                            *dst = *p;
                        }
                    }
                    else
                    {
                        dst += pixelSize;
                    }
                }
                dst += dstOffset;
            }
        }
    }
}
