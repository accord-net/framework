// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//
// Original idea found in Paint.NET project
// http://www.eecs.wsu.edu/paint.net/
//
namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Oil painting filter.
    /// </summary>
    /// 
    /// <remarks><para>Processing source image the filter changes each pixels' value
    /// to the value of pixel with the most frequent intensity within window of the
    /// <see cref="BrushSize">specified size</see>. Going through the window the filters
    /// finds which intensity of pixels is the most frequent. Then it updates value
    /// of the pixel in the center of the window to the value with the most frequent
    /// intensity. The update procedure creates the effect of oil painting.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images and 24/32
    /// color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// OilPainting filter = new OilPainting( 15 );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/oil_painting.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    public class OilPainting : BaseUsingCopyPartialFilter
    {
        private int brushSize = 5;

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
        /// Brush size, [3, 21].
        /// </summary>
        /// 
        /// <remarks><para>Window size to search for most frequent pixels' intensity.</para>
        /// 
        /// <para>Default value is set to <b>5</b>.</para></remarks>
        /// 
        public int BrushSize
        {
            get { return brushSize; }
            set { brushSize = Math.Max( 3, Math.Min( 21, value | 1 ) ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OilPainting"/> class.
        /// </summary>
        public OilPainting( )
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb]    = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb]   = PixelFormat.Format32bppArgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OilPainting"/> class.
        /// </summary>
        /// 
        /// <param name="brushSize">Brush size.</param>
        /// 
        public OilPainting( int brushSize ) : this( )
        {
            BrushSize = brushSize;
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
            int startX = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;

            int srcStride = source.Stride;
            int dstStride = destination.Stride;
            int srcOffset = srcStride - rect.Width * pixelSize;
            int dstOffset = srcStride - rect.Width * pixelSize;

            // loop and array indexes
            int i, j, t;
            // brush radius
            int radius = brushSize >> 1;

            // intensity values
            byte intensity, maxIntensity;
            int[] intensities = new int[256];

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
                        // clear arrays
                        Array.Clear( intensities, 0, 256 );

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
                                    intensity = src[i * srcStride + j];
                                    intensities[intensity]++;
                                }
                            }
                        }

                        // get most frequent intesity
                        maxIntensity = 0;
                        j = 0;

                        for ( i = 0; i < 256; i++ )
                        {
                            if ( intensities[i] > j )
                            {
                                maxIntensity = (byte) i;
                                j = intensities[i];
                            }
                        }

                        // set destination pixel
                        *dst = maxIntensity;
                    }
                    src += srcOffset;
                    dst += dstOffset;
                }
            }
            else
            {
                // RGB image
                int[] red   = new int[256];
                int[] green = new int[256];
                int[] blue  = new int[256];

                // for each line
                for ( int y = startY; y < stopY; y++ )
                {
                    // for each pixel
                    for ( int x = startX; x < stopX; x++, src += pixelSize, dst += pixelSize )
                    {
                        // clear arrays
                        Array.Clear( intensities, 0, 256 );
                        Array.Clear( red, 0, 256 );
                        Array.Clear( green, 0, 256 );
                        Array.Clear( blue, 0, 256 );

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

                                    // grayscale value using BT709
                                    intensity = (byte) ( 0.2125 * p[RGB.R] + 0.7154 * p[RGB.G] + 0.0721 * p[RGB.B] );

                                    //
                                    intensities[intensity]++;
                                    // red
                                    red[intensity] += p[RGB.R];
                                    // green
                                    green[intensity] += p[RGB.G];
                                    // blue
                                    blue[intensity] += p[RGB.B];
                                }
                            }
                        }

                        // get most frequent intesity
                        maxIntensity = 0;
                        j = 0;

                        for ( i = 0; i < 256; i++ )
                        {
                            if ( intensities[i] > j )
                            {
                                maxIntensity = (byte) i;
                                j = intensities[i];
                            }
                        }

                        // set destination pixel
                        dst[RGB.R] = (byte) ( red[maxIntensity] / intensities[maxIntensity] );
                        dst[RGB.G] = (byte) ( green[maxIntensity] / intensities[maxIntensity] );
                        dst[RGB.B] = (byte) ( blue[maxIntensity] / intensities[maxIntensity] );
                    }
                    src += srcOffset;
                    dst += dstOffset;
                }
            }
        }
    }
}
