// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

namespace AForge.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge.Math;

    /// <summary>
    /// Horizontal intensity statistics.
    /// </summary>
    /// 
    /// <remarks><para>The class provides information about horizontal distribution
    /// of pixel intensities, which may be used to locate objects, their centers, etc.
    /// </para>
    /// 
    /// <para>The class accepts grayscale (8 bpp indexed and 16 bpp) and color (24, 32, 48 and 64 bpp) images.
    /// In the case of 32 and 64 bpp color images, the alpha channel is not processed - statistics is not
    /// gathered for this channel.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // collect statistics
    /// HorizontalIntensityStatistics his = new HorizontalIntensityStatistics( sourceImage );
    /// // get gray histogram (for grayscale image)
    /// Histogram histogram = his.Gray;
    /// // output some histogram's information
    /// System.Diagnostics.Debug.WriteLine( "Mean = " + histogram.Mean );
    /// System.Diagnostics.Debug.WriteLine( "Min = " + histogram.Min );
    /// System.Diagnostics.Debug.WriteLine( "Max = " + histogram.Max );
    /// </code>
    /// 
    /// <para><b>Sample grayscale image with its horizontal intensity histogram:</b></para>
    /// <img src="img/imaging/hor_histogram.jpg" width="320" height="338" />
    /// </remarks>
    /// 
    /// <seealso cref="VerticalIntensityStatistics"/>
    /// 
    public class HorizontalIntensityStatistics
    {
        // histograms for RGB channgels
        private Histogram red   = null;
        private Histogram green = null;
        private Histogram blue  = null;
        // grayscale histogram
        private Histogram gray  = null;

        /// <summary>
        /// Histogram for red channel.
        /// </summary>
        /// 
        public Histogram Red
        {
            get
            {
                if ( red == null )
                {
                    throw new InvalidImagePropertiesException( ExceptionMessage.ColorHistogramException );
                }
                return red;
            }
        }

        /// <summary>
        /// Histogram for green channel.
        /// </summary>
        /// 
        public Histogram Green
        {
            get
            {
                if ( green == null )
                {
                    throw new InvalidImagePropertiesException( ExceptionMessage.ColorHistogramException );
                }
                return green;
            }
        }

        /// <summary>
        /// Histogram for blue channel.
        /// </summary>
        /// 
        public Histogram Blue
        {
            get
            {
                if ( blue == null )
                {
                    throw new InvalidImagePropertiesException( ExceptionMessage.ColorHistogramException );
                }
                return blue;
            }
        }

        /// <summary>
        /// Histogram for gray channel (intensities).
        /// </summary>
        /// 
        public Histogram Gray
        {
            get
            {
                if ( gray == null )
                {
                    throw new InvalidImagePropertiesException( ExceptionMessage.GrayHistogramException );
                }
                return gray;
            }
        }

        /// <summary>
        /// Value wich specifies if the processed image was color or grayscale.
        /// </summary>
        /// 
        /// <remarks><para>If the property equals to <b>true</b>, then the <see cref="Gray"/>
        /// property should be used to retrieve histogram for the processed grayscale image.
        /// Otherwise <see cref="Red"/>, <see cref="Green"/> and <see cref="Blue"/> property
        /// should be used to retrieve histogram for particular RGB channel of the processed
        /// color image.</para></remarks>
        /// 
        public bool IsGrayscale
        {
            get { return ( gray != null ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalIntensityStatistics"/> class.
        /// </summary>
        /// 
        /// <param name="image">Source image.</param>
        ///
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        /// 
        public HorizontalIntensityStatistics( Bitmap image )
        {
            // check image format
            if (
                ( image.PixelFormat != PixelFormat.Format8bppIndexed ) &&
                ( image.PixelFormat != PixelFormat.Format16bppGrayScale ) &&
                ( image.PixelFormat != PixelFormat.Format24bppRgb ) &&
                ( image.PixelFormat != PixelFormat.Format32bppRgb ) &&
                ( image.PixelFormat != PixelFormat.Format32bppArgb ) &&
                ( image.PixelFormat != PixelFormat.Format48bppRgb ) &&
                ( image.PixelFormat != PixelFormat.Format64bppArgb )
                )
            {
                throw new UnsupportedImageFormatException( "Unsupported pixel format of the source image." );
            }

            // lock bitmap data
            BitmapData imageData = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, image.PixelFormat );

            try
            {
                // gather statistics
                ProcessImage( new UnmanagedImage( imageData ) );
            }
            finally
            {
                // unlock image
                image.UnlockBits( imageData );
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalIntensityStatistics"/> class.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data.</param>
        ///
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        /// 
        public HorizontalIntensityStatistics( BitmapData imageData )
            : this( new UnmanagedImage( imageData ) )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalIntensityStatistics"/> class.
        /// </summary>
        /// 
        /// <param name="image">Source unmanaged image.</param>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        /// 
        public HorizontalIntensityStatistics( UnmanagedImage image )
        {
            // check image format
            if (
                ( image.PixelFormat != PixelFormat.Format8bppIndexed ) &&
                ( image.PixelFormat != PixelFormat.Format16bppGrayScale ) &&
                ( image.PixelFormat != PixelFormat.Format24bppRgb ) &&
                ( image.PixelFormat != PixelFormat.Format32bppRgb ) &&
                ( image.PixelFormat != PixelFormat.Format32bppArgb ) &&
                ( image.PixelFormat != PixelFormat.Format48bppRgb ) &&
                ( image.PixelFormat != PixelFormat.Format64bppArgb )
                )
            {
                throw new UnsupportedImageFormatException( "Unsupported pixel format of the source image." );
            }

            // gather statistics
            ProcessImage( image );
        }

        /// <summary>
        /// Gather horizontal intensity statistics for specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image.</param>
        /// 
        private void ProcessImage( UnmanagedImage image )
        {
            PixelFormat pixelFormat = image.PixelFormat;
            // get image dimension
            int width  = image.Width;
            int height = image.Height;

            red = green = blue = gray = null;

            // do the job
            unsafe
            {
                // check pixel format
                if ( pixelFormat == PixelFormat.Format8bppIndexed )
                {
                    // 8 bpp grayscale image
                    byte* p = (byte*) image.ImageData.ToPointer( );
                    int offset = image.Stride - width;

                    // histogram array
                    int[] g = new int[width];

                    // for each pixel
                    for ( int y = 0; y < height; y++ )
                    {
                        // for each pixel
                        for ( int x = 0; x < width; x++, p++ )
                        {
                            g[x] += *p;
                        }
                        p += offset;
                    }

                    // create historgram for gray level
                    gray = new Histogram( g );
                }
                else if ( pixelFormat == PixelFormat.Format16bppGrayScale )
                {
                    // 16 bpp grayscale image
                    byte* basePtr = (byte*) image.ImageData.ToPointer( );
                    int stride = image.Stride;

                    // histogram array
                    int[] g = new int[width];

                    // for each pixel
                    for ( int y = 0; y < height; y++ )
                    {
                        ushort* p = (ushort*) ( basePtr + stride * y );

                        // for each pixel
                        for ( int x = 0; x < width; x++, p++ )
                        {
                            g[x] += *p;
                        }
                    }

                    // create historgram for gray level
                    gray = new Histogram( g );
                }
                else if (
                    ( pixelFormat == PixelFormat.Format24bppRgb ) ||
                    ( pixelFormat == PixelFormat.Format32bppRgb ) ||
                    ( pixelFormat == PixelFormat.Format32bppArgb ) )
                {
                    // 24/32 bpp color image
                    byte* p = (byte*) image.ImageData.ToPointer( );
                    int pixelSize = ( pixelFormat == PixelFormat.Format24bppRgb ) ? 3 : 4;
                    int offset = image.Stride - width * pixelSize;

                    // histogram arrays
                    int[] r = new int[width];
                    int[] g = new int[width];
                    int[] b = new int[width];

                    // for each line
                    for ( int y = 0; y < height; y++ )
                    {
                        // for each pixel
                        for ( int x = 0; x < width; x++, p += pixelSize )
                        {
                            r[x] += p[RGB.R];
                            g[x] += p[RGB.G];
                            b[x] += p[RGB.B];
                        }
                        p += offset;
                    }

                    // create histograms
                    red   = new Histogram( r );
                    green = new Histogram( g );
                    blue  = new Histogram( b );
                }
                else if (
                    ( pixelFormat == PixelFormat.Format48bppRgb ) ||
                    ( pixelFormat == PixelFormat.Format64bppArgb ) )
                {
                    // 48/64 bpp color image
                    byte* basePtr = (byte*) image.ImageData.ToPointer( );
                    int stride = image.Stride;
                    int pixelSize = ( pixelFormat == PixelFormat.Format48bppRgb ) ? 3 : 4;

                    // histogram arrays
                    int[] r = new int[width];
                    int[] g = new int[width];
                    int[] b = new int[width];

                    // for each line
                    for ( int y = 0; y < height; y++ )
                    {
                        ushort* p = (ushort*) ( basePtr + stride * y );

                        // for each pixel
                        for ( int x = 0; x < width; x++, p += pixelSize )
                        {
                            r[x] += p[RGB.R];
                            g[x] += p[RGB.G];
                            b[x] += p[RGB.B];
                        }
                    }

                    // create histograms
                    red   = new Histogram( r );
                    green = new Histogram( g );
                    blue  = new Histogram( b );
                }
            }
        }
    }
}
