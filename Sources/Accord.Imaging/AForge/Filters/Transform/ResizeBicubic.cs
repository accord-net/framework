// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Resize image using bicubic interpolation algorithm.
    /// </summary>
    /// 
    /// <remarks><para>The class implements image resizing filter using bicubic
    /// interpolation algorithm. It uses bicubic kernel W(x) as described on
    /// <a href="http://en.wikipedia.org/wiki/Bicubic_interpolation#Bicubic_convolution_algorithm">Wikipedia</a>
    /// (coefficient <b>a</b> is set to <b>-0.5</b>).</para>
    /// 
    /// <para>The filter accepts 8 grayscale images and 24 bpp
    /// color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// ResizeBicubic filter = new ResizeBicubic( 400, 300 );
    /// // apply the filter
    /// Bitmap newImage = filter.Apply( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample9.png" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/resize_bicubic.png" width="400" height="300" />
    /// </remarks>
    /// 
    /// <seealso cref="ResizeNearestNeighbor"/>
    /// <seealso cref="ResizeBilinear"/>
    ///
    public class ResizeBicubic : BaseResizeFilter
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
        /// Initializes a new instance of the <see cref="ResizeBicubic"/> class.
        /// </summary>
        /// 
        /// <param name="newWidth">Width of new image.</param>
        /// <param name="newHeight">Height of new image.</param>
        /// 
		public ResizeBicubic( int newWidth, int newHeight ) :
            base( newWidth, newHeight )
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
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

            int pixelSize = ( sourceData.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;
            int srcStride = sourceData.Stride;
            int dstOffset = destinationData.Stride - pixelSize * newWidth;
            double xFactor = (double) width / newWidth;
            double yFactor = (double) height / newHeight;

            // do the job
            byte* src = (byte*) sourceData.ImageData.ToPointer( );
            byte* dst = (byte*) destinationData.ImageData.ToPointer( );

            // coordinates of source points and cooefficiens
            double  ox, oy, dx, dy, k1, k2;
            int     ox1, oy1, ox2, oy2;
            // destination pixel values
            double r, g, b;
            // width and height decreased by 1
            int ymax = height - 1;
            int xmax = width - 1;
            // temporary pointer
            byte* p;

            // check pixel format
            if ( destinationData.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // grayscale
                for ( int y = 0; y < newHeight; y++ )
                {
                    // Y coordinates
                    oy  = (double) y * yFactor - 0.5;
                    oy1 = (int) oy;
                    dy  = oy - (double) oy1;

                    for ( int x = 0; x < newWidth; x++, dst++ )
                    {
                        // X coordinates
                        ox  = (double) x * xFactor - 0.5f;
                        ox1 = (int) ox;
                        dx  = ox - (double) ox1;

                        // initial pixel value
                        g = 0;

                        for ( int n = -1; n < 3; n++ )
                        {
                            // get Y cooefficient
                            k1 = Interpolation.BiCubicKernel( dy - (double) n );

                            oy2 = oy1 + n;
                            if ( oy2 < 0 )
                                oy2 = 0;
                            if ( oy2 > ymax )
                                oy2 = ymax;

                            for ( int m = -1; m < 3; m++ )
                            {
                                // get X cooefficient
                                k2 = k1 * Interpolation.BiCubicKernel( (double) m - dx );

                                ox2 = ox1 + m;
                                if ( ox2 < 0 )
                                    ox2 = 0;
                                if ( ox2 > xmax )
                                    ox2 = xmax;

                                g += k2 * src[oy2 * srcStride + ox2];
                            }
                        }
                        *dst = (byte) Math.Max( 0, Math.Min( 255, g ) );
                    }
                    dst += dstOffset;
                }
            }
            else
            {
                // RGB
                for ( int y = 0; y < newHeight; y++ )
                {
                    // Y coordinates
                    oy  = (double) y * yFactor - 0.5f;
                    oy1 = (int) oy;
                    dy  = oy - (double) oy1;

                    for ( int x = 0; x < newWidth; x++, dst += 3 )
                    {
                        // X coordinates
                        ox  = (double) x * xFactor - 0.5f;
                        ox1 = (int) ox;
                        dx  = ox - (double) ox1;

                        // initial pixel value
                        r = g = b = 0;

                        for ( int n = -1; n < 3; n++ )
                        {
                            // get Y cooefficient
                            k1 = Interpolation.BiCubicKernel( dy - (double) n );

                            oy2 = oy1 + n;
                            if ( oy2 < 0 )
                                oy2 = 0;
                            if ( oy2 > ymax )
                                oy2 = ymax;

                            for ( int m = -1; m < 3; m++ )
                            {
                                // get X cooefficient
                                k2 = k1 * Interpolation.BiCubicKernel( (double) m - dx );

                                ox2 = ox1 + m;
                                if ( ox2 < 0 )
                                    ox2 = 0;
                                if ( ox2 > xmax )
                                    ox2 = xmax;

                                // get pixel of original image
                                p = src + oy2 * srcStride + ox2 * 3;

                                r += k2 * p[RGB.R];
                                g += k2 * p[RGB.G];
                                b += k2 * p[RGB.B];
                            }
                        }

                        dst[RGB.R] = (byte) Math.Max( 0, Math.Min( 255, r ) );
                        dst[RGB.G] = (byte) Math.Max( 0, Math.Min( 255, g ) );
                        dst[RGB.B] = (byte) Math.Max( 0, Math.Min( 255, b ) );
                    }
                    dst += dstOffset;
                }
            }
        }
    }
}
