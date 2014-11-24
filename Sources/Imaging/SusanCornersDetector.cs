// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//
// Copyright © Frank Nagl, 2007
// admin@franknagl.de
//
namespace AForge.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Collections.Generic;
    using AForge.Imaging.Filters;

    /// <summary>
    /// Susan corners detector.
    /// </summary>
    /// 
    /// <remarks><para>The class implements Susan corners detector, which is described by
    /// S.M. Smith in: <b>S.M. Smith, "SUSAN - a new approach to low level image processing",
    /// Internal Technical Report TR95SMS1, Defense Research Agency, Chobham Lane, Chertsey,
    /// Surrey, UK, 1995</b>.</para>
    /// 
    /// <para><note>Some implementation notes:
    /// <list type="bullet">
    /// <item>Analyzing each pixel and searching for its USAN area, the 7x7 mask is used,
    /// which is comprised of 37 pixels. The mask has circle shape:
    /// <code lang="none">
    ///   xxx
    ///  xxxxx
    /// xxxxxxx
    /// xxxxxxx
    /// xxxxxxx
    ///  xxxxx
    ///   xxx
    /// </code>
    /// </item>
    /// <item>In the case if USAN's center of mass has the same coordinates as nucleus
    /// (central point), the pixel is not a corner.</item>
    /// <item>For noise suppression the 5x5 square window is used.</item></list></note></para>
    /// 
    /// <para>The class processes only grayscale 8 bpp and color 24/32 bpp images.
    /// In the case of color image, it is converted to grayscale internally using
    /// <see cref="GrayscaleBT709"/> filter.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create corners detector's instance
    /// SusanCornersDetector scd = new SusanCornersDetector( );
    /// // process image searching for corners
    /// List&lt;IntPoint&gt; corners = scd.ProcessImage( image );
    /// // process points
    /// foreach ( IntPoint corner in corners )
    /// {
    ///     // ... 
    /// }
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="MoravecCornersDetector"/>
    /// 
    public class SusanCornersDetector : ICornersDetector
    {
        // brightness difference threshold
        private int differenceThreshold = 25;
        // geometrical threshold
        private int geometricalThreshold = 18;

        /// <summary>
        /// Brightness difference threshold.
        /// </summary>
        /// 
        /// <remarks><para>The brightness difference threshold controls the amount
        /// of pixels, which become part of USAN area. If difference between central
        /// pixel (nucleus) and surrounding pixel is not higher than difference threshold,
        /// then that pixel becomes part of USAN.</para>
        /// 
        /// <para>Increasing this value decreases the amount of detected corners.</para>
        /// 
        /// <para>Default value is set to <b>25</b>.</para>
        /// </remarks>
        /// 
        public int DifferenceThreshold
        {
            get { return differenceThreshold; }
            set { differenceThreshold = value; }
        }

        /// <summary>
        /// Geometrical threshold.
        /// </summary>
        /// 
        /// <remarks><para>The geometrical threshold sets the maximum number of pixels
        /// in USAN area around corner. If potential corner has USAN with more pixels, than
        /// it is not a corner.</para>
        /// 
        /// <para> Decreasing this value decreases the amount of detected corners - only sharp corners
        /// are detected. Increasing this value increases the amount of detected corners, but
        /// also increases amount of flat corners, which may be not corners at all.</para>
        /// 
        /// <para>Default value is set to <b>18</b>, which is half of maximum amount of pixels in USAN.</para>
        /// </remarks>
        /// 
        public int GeometricalThreshold
        {
            get { return geometricalThreshold; }
            set { geometricalThreshold = value; }
        }
        
        private static int[] rowRadius = new int[7] { 1, 2, 3, 3, 3, 2, 1 };

        /// <summary>
        /// Initializes a new instance of the <see cref="SusanCornersDetector"/> class.
        /// </summary>
        public SusanCornersDetector( )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SusanCornersDetector"/> class.
        /// </summary>
        /// 
        /// <param name="differenceThreshold">Brightness difference threshold.</param>
        /// <param name="geometricalThreshold">Geometrical threshold.</param>
        /// 
        public SusanCornersDetector( int differenceThreshold, int geometricalThreshold )
        {
            this.differenceThreshold  = differenceThreshold;
            this.geometricalThreshold = geometricalThreshold;
        }

        /// <summary>
        /// Process image looking for corners.
        /// </summary>
        /// 
        /// <param name="image">Source image to process.</param>
        /// 
        /// <returns>Returns list of found corners (X-Y coordinates).</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
        /// 
        public List<IntPoint> ProcessImage( Bitmap image )
        {
            // check image format
            if (
                ( image.PixelFormat != PixelFormat.Format8bppIndexed ) &&
                ( image.PixelFormat != PixelFormat.Format24bppRgb ) &&
                ( image.PixelFormat != PixelFormat.Format32bppRgb ) &&
                ( image.PixelFormat != PixelFormat.Format32bppArgb )
                )
            {
                throw new UnsupportedImageFormatException( "Unsupported pixel format of the source image." );
            }

            // lock source image
            BitmapData imageData = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, image.PixelFormat );

            List<IntPoint> corners;

            try
            {
                // process the image
                corners = ProcessImage( new UnmanagedImage( imageData ) );
            }
            finally
            {
                // unlock image
                image.UnlockBits( imageData );
            }

            return corners;
        }

        /// <summary>
        /// Process image looking for corners.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found corners (X-Y coordinates).</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
        /// 
        public List<IntPoint> ProcessImage( BitmapData imageData )
        {
            return ProcessImage( new UnmanagedImage( imageData ) );
        }

        /// <summary>
        /// Process image looking for corners.
        /// </summary>
        /// 
        /// <param name="image">Unmanaged source image to process.</param>
        /// 
        /// <returns>Returns array of found corners (X-Y coordinates).</returns>
        ///
        /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
        /// 
        public List<IntPoint> ProcessImage( UnmanagedImage image )
        {
            // check image format
            if (
                ( image.PixelFormat != PixelFormat.Format8bppIndexed ) &&
                ( image.PixelFormat != PixelFormat.Format24bppRgb ) &&
                ( image.PixelFormat != PixelFormat.Format32bppRgb ) &&
                ( image.PixelFormat != PixelFormat.Format32bppArgb )
                )
            {
                throw new UnsupportedImageFormatException( "Unsupported pixel format of the source image." );
            }

            // get source image size
            int width  = image.Width;
            int height = image.Height;

            // make sure we have grayscale image
            UnmanagedImage grayImage = null;

            if ( image.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                grayImage = image;
            }
            else
            {
                // create temporary grayscale image
                grayImage = Grayscale.CommonAlgorithms.BT709.Apply( image );
            }

            int[,] susanMap = new int[height, width];

            // do the job
            unsafe
            {
                int stride = grayImage.Stride;
                int offset = stride - width;

                byte* src = (byte*) grayImage.ImageData.ToPointer( ) + stride * 3 + 3;

			    // for each row
                for ( int y = 3, maxY = height - 3; y < maxY; y++ )
                {
                    // for each pixel
                    for ( int x = 3, maxX = width - 3; x < maxX; x++, src++ )
                    {
                        // get value of the nucleus
                        byte nucleusValue = *src;
                        // usan - number of pixels with similar brightness
                        int usan = 0;
                        // center of gravity
                        int cx = 0, cy = 0;

                        // for each row of the mask
                        for ( int i = -3; i <= 3; i++ )
                        {
                            // determine row's radius
                            int r = rowRadius[i + 3];

                            // get pointer to the central pixel of the row
                            byte* ptr = src + stride * i;

                            // for each element of the mask's row
                            for ( int j = -r; j <= r; j++ )
                            {
                                // differenceThreshold
                                if ( System.Math.Abs( nucleusValue - ptr[j] ) <= differenceThreshold )
                                {
                                    usan++;

                                    cx += x + j;
                                    cy += y + i;
                                }
                            }
                        }

                        // check usan size
                        if ( usan < geometricalThreshold )
                        {
                            cx /= usan;
                            cy /= usan;

                            if ( ( x != cx ) || ( y != cy ) )
                            {
                                // cornersList.Add( new Point( x, y ) );
                                usan = ( geometricalThreshold - usan );
                            }
                            else
                            {
                                usan = 0;
                            }
                        }
                        else
                        {
                            usan = 0;
                        }

                        // usan = ( usan < geometricalThreshold ) ? ( geometricalThreshold - usan ) : 0;
                        susanMap[y, x] = usan;
                    }

                    src += 6 + offset;
                }
            }

            if ( image.PixelFormat != PixelFormat.Format8bppIndexed )
            {
                // free grayscale image
                grayImage.Dispose( );
            }

            // collect interesting points - only those points, which are local maximums
            List<IntPoint> cornersList = new List<IntPoint>( );

            // for each row
            for ( int y = 2, maxY = height - 2; y < maxY; y++ )
            {
                // for each pixel
                for ( int x = 2, maxX = width - 2; x < maxX; x++ )
                {
                    int currentValue = susanMap[y, x];

                    // for each windows' row
                    for ( int i = -2; ( currentValue != 0 ) && ( i <= 2 ); i++ )
                    {
                        // for each windows' pixel
                        for ( int j = -2; j <= 2; j++ )
                        {
                            if ( susanMap[y + i, x + j] > currentValue )
                            {
                                currentValue = 0;
                                break;
                            }
                        }
                    }

                    // check if this point is really interesting
                    if ( currentValue != 0 )
                    {
                        cornersList.Add( new IntPoint( x, y ) );
                    }
                }
            }

            return cornersList;
        }
    }
}
