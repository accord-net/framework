// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Median filter.
    /// </summary>
    /// 
    /// <remarks><para>The median filter is normally used to reduce noise in an image, somewhat like
    /// the <see cref="Mean">mean filter</see>. However, it often does a better job than the mean
    /// filter of preserving useful detail in the image.</para>
    /// 
    /// <para>Each pixel of the original source image is replaced with the median of neighboring pixel
    /// values. The median is calculated by first sorting all the pixel values from the surrounding
    /// neighborhood into numerical order and then replacing the pixel being considered with the
    /// middle pixel value.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images and 24/32 bpp
    /// color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// Median filter = new Median( );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample13.png" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/median.png" width="480" height="361" />
    /// </remarks>
    /// 
    public class Median : BaseUsingCopyPartialFilter
    {
        private int size = 3;

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
        /// Processing square size for the median filter, [3, 25].
        /// </summary>
        /// 
        /// <remarks><para>Default value is set to <b>3</b>.</para>
        /// 
        /// <para><note>The value should be odd.</note></para>
        /// </remarks>
        /// 
        public int Size
        {
            get { return size; }
            set { size = Math.Max( 3, Math.Min( 25, value | 1 ) ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Median"/> class.
        /// </summary>
        public Median( )
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb]    = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb]   = PixelFormat.Format32bppArgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Median"/> class.
        /// </summary>
        /// 
        /// <param name="size">Processing square size.</param>
        /// 
        public Median( int size ) : this( )
        {
            Size = size;
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
            int srcOffset = srcStride - rect.Width * pixelSize;
            int dstOffset = dstStride - rect.Width * pixelSize;

            // loop and array indexes
            int i, j, t;
            // processing square's radius
            int radius = size >> 1;
            // number of elements
            int c;

            // array to hold pixel values (R, G, B)
            byte[] r = new byte[size * size];
            byte[] g = new byte[size * size];
            byte[] b = new byte[size * size];

            byte* src = (byte*) source.ImageData.ToPointer( );
            byte* dst = (byte*) destination.ImageData.ToPointer( );
            byte* p;

            // allign pointers to the first pixel to process
            src += ( startY * srcStride + startX * pixelSize );
            dst += ( startY * dstStride + startX * pixelSize );

            // do the processing job
            if ( destination.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // grayscale image

                // for each line
                for ( int y = startY; y < stopY; y++ )
                {
                    // for each pixel
                    for ( int x = startX; x < stopX; x++, src++, dst++ )
                    {
                        c = 0;

                        // for each kernel row
                        for ( i = -radius; i <= radius; i++ )
                        {
                            t = y + i;

                            // skip row
                            if ( t < startY )
                                continue;
                            // break
                            if ( t >= stopY )
                                break;

                            // for each kernel column
                            for ( j = -radius; j <= radius; j++ )
                            {
                                t = x + j;

                                // skip column
                                if ( t < startX )
                                    continue;

                                if ( t < stopX )
                                {
                                    g[c++] = src[i * srcStride + j];
                                }
                            }
                        }
                        // sort elements
                        Array.Sort( g, 0, c );
                        // get the median
                        *dst = g[c >> 1];
                    }
                    src += srcOffset;
                    dst += dstOffset;
                }
            }
            else
            {
                // RGB image

                // for each line
                for ( int y = startY; y < stopY; y++ )
                {
                    // for each pixel
                    for ( int x = startX; x < stopX; x++, src += pixelSize, dst += pixelSize )
                    {
                        c = 0;

                        // for each kernel row
                        for ( i = -radius; i <= radius; i++ )
                        {
                            t = y + i;

                            // skip row
                            if ( t < startY )
                                continue;
                            // break
                            if ( t >= stopY )
                                break;

                            // for each kernel column
                            for ( j = -radius; j <= radius; j++ )
                            {
                                t = x + j;

                                // skip column
                                if ( t < startX )
                                    continue;

                                if ( t < stopX )
                                {
                                    p = &src[i * srcStride + j * pixelSize];

                                    r[c] = p[RGB.R];
                                    g[c] = p[RGB.G];
                                    b[c] = p[RGB.B];
                                    c++;
                                }
                            }
                        }

                        // sort elements
                        Array.Sort( r, 0, c );
                        Array.Sort( g, 0, c );
                        Array.Sort( b, 0, c );
                        // get the median
                        t = c >> 1;
                        dst[RGB.R] = r[t];
                        dst[RGB.G] = g[t];
                        dst[RGB.B] = b[t];
                    }
                    src += srcOffset;
                    dst += dstOffset;
                }
            }
        }
    }
}
