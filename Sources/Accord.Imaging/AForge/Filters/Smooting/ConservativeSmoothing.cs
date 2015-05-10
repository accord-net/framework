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
    /// Conservative smoothing.
    /// </summary>
    /// 
    /// <remarks><para>The filter implements conservative smoothing, which is a noise reduction
    /// technique that derives its name from the fact that it employs a simple, fast filtering
    /// algorithm that sacrifices noise suppression power in order to preserve the high spatial
    /// frequency detail (e.g. sharp edges) in an image. It is explicitly designed to remove noise
    /// spikes - <b>isolated</b> pixels of exceptionally low or high pixel intensity
    /// (<see cref="SaltAndPepperNoise">salt and pepper noise</see>).</para>
    /// 
    /// <para>If the filter finds a pixel which has minimum/maximum value compared to its surrounding
    /// pixel, then its value is replaced by minimum/maximum value of those surrounding pixel.
    /// For example, lets suppose the filter uses <see cref="KernelSize">kernel size</see> of 3x3,
    /// which means each pixel has 8 surrounding pixel. If pixel's value is smaller than any value
    /// of surrounding pixels, then the value of the pixel is replaced by minimum value of those surrounding
    /// pixels.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images and 24/32 bpp
    /// color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// ConservativeSmoothing filter = new ConservativeSmoothing( );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample13.png" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/conservative_smoothing.png" width="480" height="361" />
    /// </remarks>
    /// 
    public class ConservativeSmoothing : BaseUsingCopyPartialFilter
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
        /// Kernel size, [3, 25].
        /// </summary>
        /// 
        /// <remarks><para>Determines the size of pixel's square used for smoothing.</para>
        /// 
        /// <para>Default value is set to <b>3</b>.</para>
        /// 
        /// <para><note>The value should be odd.</note></para>
        /// </remarks>
        /// 
        public int KernelSize
        {
            get { return size; }
            set { size = Math.Max( 3, Math.Min( 25, value | 1 ) ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConservativeSmoothing"/> class.
        /// </summary>
        /// 
        public ConservativeSmoothing( )
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb]    = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb]   = PixelFormat.Format32bppArgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConservativeSmoothing"/> class.
        /// </summary>
        /// 
        /// <param name="size">Kernel size.</param>
        /// 
        public ConservativeSmoothing( int size ) : this( )
        {
            KernelSize = size;
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
            // kernel's radius
            int radius = size >> 1;
            // pixel value (min and max)
            byte minR, maxR, minG, maxG, minB, maxB, v;

            byte* src = (byte*) source.ImageData.ToPointer( );
            byte* dst = (byte*) destination.ImageData.ToPointer( );
            byte* p;

            // allign pointers to the first pixel to process
            src += ( startY * srcStride + startX * pixelSize );
            dst += ( startY * dstStride + startX * pixelSize );

            if ( destination.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // Grayscale image

                // for each line
                for ( int y = startY; y < stopY; y++ )
                {
                    // for each pixel
                    for ( int x = startX; x < stopX; x++, src++, dst++ )
                    {
                        minG = 255;
                        maxG = 0;

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

                                if ( ( i != j ) && ( t < stopX ) )
                                {
                                    // find MIN and MAX values
                                    v = src[i * srcStride + j];

                                    if ( v < minG )
                                        minG = v;
                                    if ( v > maxG )
                                        maxG = v;
                                }
                            }
                        }
                        // set destination pixel
                        v = *src;
                        *dst = ( v > maxG ) ? maxG : ( ( v < minG ) ? minG : v );
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
                        minR = minG = minB = 255;
                        maxR = maxG = maxB = 0;

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

                                if ( ( i != j ) && ( t < stopX ) )
                                {
                                    p = &src[i * srcStride + j * pixelSize];

                                    // find MIN and MAX values

                                    // red
                                    v = p[RGB.R];

                                    if ( v < minR )
                                        minR = v;
                                    if ( v > maxR )
                                        maxR = v;

                                    // green
                                    v = p[RGB.G];

                                    if ( v < minG )
                                        minG = v;
                                    if ( v > maxG )
                                        maxG = v;

                                    // blue
                                    v = p[RGB.B];

                                    if ( v < minB )
                                        minB = v;
                                    if ( v > maxB )
                                        maxB = v;
                                }
                            }
                        }
                        // set destination pixel

                        // red
                        v = src[RGB.R];
                        dst[RGB.R] = ( v > maxR ) ? maxR : ( ( v < minR ) ? minR : v );
                        // green
                        v = src[RGB.G];
                        dst[RGB.G] = ( v > maxG ) ? maxG : ( ( v < minG ) ? minG : v );
                        // blue
                        v = src[RGB.B];
                        dst[RGB.B] = ( v > maxB ) ? maxB : ( ( v < minB ) ? minB : v );
                    }
                    src += srcOffset;
                    dst += dstOffset;
                }
            }
        }
    }
}
