// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Pixellate filter.
    /// </summary>
    /// 
    /// <remarks><para>The filter processes an image creating the effect of an image with larger
    /// pixels - pixellated image. The effect is achieved by filling image's rectangles of the
    /// specified size by the color, which is mean color value for the corresponding rectangle.
    /// The size of rectangles to process is set by <see cref="PixelWidth"/> and <see cref="PixelHeight"/>
    /// properties.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images and 24 bpp
    /// color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// Pixellate filter = new Pixellate( );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/pixellate.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    public class Pixellate : BaseInPlacePartialFilter
    {
        private int pixelWidth  = 8;
        private int pixelHeight = 8;

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
        /// Pixel width, [2, 32].
        /// </summary>
        /// 
        /// <remarks>Default value is set to <b>8</b>.</remarks>
        /// 
        /// <seealso cref="PixelSize"/>
        /// <seealso cref="PixelHeight"/>
        /// 
        public int PixelWidth
        {
            get { return pixelWidth; }
            set { pixelWidth = Math.Max( 2, Math.Min( 32, value ) ); }
        }

        /// <summary>
        /// Pixel height, [2, 32].
        /// </summary>
        /// 
        /// <remarks>Default value is set to <b>8</b>.</remarks>
        /// 
        /// <seealso cref="PixelSize"/>
        /// <seealso cref="PixelWidth"/>
        /// 
        public int PixelHeight
        {
            get { return pixelHeight; }
            set { pixelHeight = Math.Max( 2, Math.Min( 32, value ) ); }
        }

        /// <summary>
        /// Pixel size, [2, 32].
        /// </summary>
        /// 
        /// <remarks>The property is used to set both <see cref="PixelWidth"/> and
        /// <see cref="PixelHeight"/> simultaneously.</remarks>
        /// 
        public int PixelSize
        {
            set { pixelWidth = pixelHeight = Math.Max( 2, Math.Min( 32, value ) ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pixellate"/> class.
        /// </summary>
        /// 
        public Pixellate( )
        {
            // initialize format translation dictionary
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pixellate"/> class.
        /// </summary>
        /// 
        /// <param name="pixelSize">Pixel size.</param>
        /// 
        public Pixellate( int pixelSize ) : this( )
        {
            PixelSize = pixelSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pixellate"/> class.
        /// </summary>
        /// 
        /// <param name="pixelWidth">Pixel width.</param>
        /// <param name="pixelHeight">Pixel height.</param>
        /// 
        public Pixellate( int pixelWidth, int pixelHeight )
            : this( )
        {
            PixelWidth = pixelWidth;
            PixelHeight = pixelHeight;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        ///
        protected override unsafe void ProcessFilter( UnmanagedImage image, Rectangle rect )
        {
            int pixelSize = ( image.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;

            // processing start and stop Y positions
            int startY  = rect.Top;
            int stopY   = startY + rect.Height;
            // processing width and offset
            int width   = rect.Width;
            int offset  = image.Stride - width * pixelSize;

            // loop indexes and temp vars
            int i, j, k, x, t1, t2;
            // line length to process
            int len = (int) ( ( width - 1 ) / pixelWidth ) + 1;
            // reminder
            int rem = ( ( width - 1 ) % pixelWidth ) + 1;

            // do the job
            byte* src = (byte*) image.ImageData.ToPointer( );
            // allign pointer to the first pixel to process
            src += ( startY * image.Stride + rect.Left * pixelSize );

            byte* dst = src;

            if ( image.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // Grayscale image
                int[] tmp = new int[len];

                for ( int y1 = startY, y2 = startY; y1 < stopY; )
                {
                    // collect pixels
                    Array.Clear( tmp, 0, len );

                    // calculate
                    for ( i = 0; ( i < pixelHeight ) && ( y1 < stopY ); i++, y1++ )
                    {
                        // for each pixel
                        for ( x = 0; x < width; x++, src++ )
                        {
                            tmp[(int) ( x / pixelWidth )] += (int) *src;
                        }
                        src += offset;
                    }

                    // get average values
                    t1 = i * pixelWidth;
                    t2 = i * rem;

                    for ( j = 0; j < len - 1; j++ )
                        tmp[j] /= t1;
                    tmp[j] /= t2;

                    // save average value to destination image
                    for ( i = 0; ( i < pixelHeight ) && ( y2 < stopY ); i++, y2++ )
                    {
                        // for each pixel
                        for ( x = 0; x < width; x++, dst++ )
                        {
                            *dst = (byte) tmp[(int) ( x / pixelWidth )];
                        }
                        dst += offset;
                    }
                }
            }
            else
            {
                // RGB image
                int[] tmp = new int[len * 3];

                for ( int y1 = startY, y2 = startY; y1 < stopY; )
                {
                    // collect pixels
                    Array.Clear( tmp, 0, len * 3 );

                    // calculate
                    for ( i = 0; ( i < pixelHeight ) && ( y1 < stopY ); i++, y1++ )
                    {
                        // for each pixel
                        for ( x = 0; x < width; x++, src += 3 )
                        {
                            k = ( x / pixelWidth ) * 3;
                            tmp[k]     += src[RGB.R];
                            tmp[k + 1] += src[RGB.G];
                            tmp[k + 2] += src[RGB.B];
                        }
                        src += offset;
                    }

                    // get average values
                    t1 = i * pixelWidth;
                    t2 = i * rem;

                    for ( j = 0, k = 0; j < len - 1; j++, k += 3 )
                    {
                        tmp[k]     /= t1;
                        tmp[k + 1] /= t1;
                        tmp[k + 2] /= t1;
                    }
                    tmp[k]     /= t2;
                    tmp[k + 1] /= t2;
                    tmp[k + 2] /= t2;

                    // save average value to destination image
                    for ( i = 0; ( i < pixelHeight ) && ( y2 < stopY ); i++, y2++ )
                    {
                        // for each pixel
                        for ( x = 0; x < width; x++, dst += 3 )
                        {
                            k = ( x / pixelWidth ) * 3;
                            dst[RGB.R] = (byte) tmp[k];
                            dst[RGB.G] = (byte) tmp[k + 1];
                            dst[RGB.B] = (byte) tmp[k + 2];
                        }
                        dst += offset;
                    }
                }
            }
        }
    }
}
