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
    /// Set of Bayer patterns supported by <see cref="BayerFilterOptimized"/>.
    /// </summary>
    public enum BayerPattern
    {
        /// <summary>
        /// Pattern:<br /><br />
        /// G R<br />
        /// B G
        /// </summary>
        GRBG,

        /// <summary>
        /// Pattern:<br /><br />
        /// B G<br />
        /// G R
        /// </summary>
        BGGR
    }

    /// <summary>
    /// Optimized Bayer fileter image processing routine.
    /// </summary>
    /// 
    /// <remarks><para>The class implements <a href="http://en.wikipedia.org/wiki/Bayer_filter">Bayer filter</a>
    /// routine, which creates color image out of grayscale image produced by image sensor built with
    /// Bayer color matrix.</para>
    /// 
    /// <para>This class does all the same as <see cref="BayerFilter"/> class. However this version is
    /// optimized for some well known patterns defined in <see cref="BayerPattern"/> enumeration.
    /// Also this class processes images with even width and height only. Image size must be at least 2x2 pixels.
    /// </para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images and produces 24 bpp RGB image.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// BayerFilter filter = new BayerFilter( );
    /// // apply the filter
    /// Bitmap rgbImage = filter.Apply( image );
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="BayerFilter"/>
    /// 
    public class BayerFilterOptimized : BaseFilter
    {
        private BayerPattern bayerPattern = BayerPattern.GRBG;

        /// <summary>
        /// Bayer pattern of source images to decode.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies Bayer pattern of source images to be
        /// decoded into color images.</para>
        /// 
        /// <para>Default value is set to <see cref="BayerPattern.GRBG"/>.</para>
        /// </remarks>
        /// 
        public BayerPattern Pattern
        {
            get { return bayerPattern; }
            set { bayerPattern = value; }
        }

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
        /// Initializes a new instance of the <see cref="BayerFilterOptimized"/> class.
        /// </summary>
        /// 
        public BayerFilterOptimized( )
        {
            // initialize format translation dictionary
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format24bppRgb;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        /// 
        protected override void ProcessFilter( UnmanagedImage sourceData, UnmanagedImage destinationData )
        {
            // get width and height
            int width  = sourceData.Width;
            int height = sourceData.Height;

            if ( ( ( width & 1 ) == 1 ) || ( ( height & 1 ) == 1 ) ||
                 ( width < 2 ) || ( height < 2 ) )
            {
                throw new InvalidImagePropertiesException( "Source image must have even width and height. Width and height can not be smaller than 2." );
            }

            switch ( bayerPattern )
            {
                case BayerPattern.GRBG:
                    ApplyGRBG( sourceData, destinationData );
                    break;

                case BayerPattern.BGGR:
                    ApplyBGGR( sourceData, destinationData );
                    break;
            }
        }

        #region GRBG pattern
        private unsafe void ApplyGRBG( UnmanagedImage sourceData, UnmanagedImage destinationData )
        {
            int width  = sourceData.Width;
            int height = sourceData.Height;

            int widthM1  = width - 1;
            int heightM1 = height - 1;

            int srcStride = sourceData.Stride;
            int dstStride = destinationData.Stride;

            int srcStrideP1  = srcStride + 1;
            int srcStrideM1  = srcStride - 1;
            int srcMStride   = -srcStride;
            int srcMStrideP1 = srcMStride + 1;
            int srcMStrideM1 = srcMStride - 1;

            int srcOffset = srcStride - width;
            int dstOffset = dstStride - width * 3;

            // do the job
            byte * src = (byte*) sourceData.ImageData.ToPointer( );
            byte * dst = (byte*) destinationData.ImageData.ToPointer( );

            // --- process the first line

            // . . .
            // . G R
            // . B G
            dst[RGB.R] = src[1];
            dst[RGB.G] = (byte) ( ( *src + src[srcStrideP1] ) >> 1 );
            dst[RGB.B] = src[srcStride];

            src++;
            dst += 3;

            for ( int x = 1; x < widthM1; x += 2 )
            {
                // . . .
                // G R G
                // B G B
                dst[RGB.R] = *src;
                dst[RGB.G] = (byte) ( ( src[srcStride] + src[-1] + src[1] ) / 3 );
                dst[RGB.B] = (byte) ( ( src[srcStrideM1] + src[srcStrideP1] ) >> 1 );

                src++;
                dst += 3;

                // . . .
                // R G R
                // G B G
                dst[RGB.R] = (byte) ( ( src[-1] + src[1] ) >> 1 );
                dst[RGB.G] = (byte) ( ( *src + src[srcStrideM1] + src[srcStrideP1] ) / 3 );
                dst[RGB.B] = src[srcStride];

                src++;
                dst += 3;
            }

            // . . .
            // G R .
            // B G .
            dst[RGB.R] = *src;
            dst[RGB.G] = (byte) ( ( src[-1] + src[srcStride] ) >> 1 );
            dst[RGB.B] = src[srcStrideM1];


            // allign to the next line
            src += srcOffset + 1;
            dst += dstOffset + 3;

            // --- process all lines except the first one and the last one
            for ( int y = 1; y < heightM1; y += 2 )
            {
                // . G R
                // . B G
                // . G R
                dst[RGB.R] = (byte) ( ( src[srcMStrideP1] + src[srcStrideP1] ) >> 1 );
                dst[RGB.G] = (byte) ( ( src[srcMStride] + src[srcStride] + src[1] ) / 3 );
                dst[RGB.B] = *src;

                dst += dstStride;
                src += srcStride;

                // ( y+1 pixel )
                // . B G
                // . G R
                // . B G
                dst[RGB.R] = src[1];
                dst[RGB.G] = (byte) ( ( *src + src[srcMStrideP1] + src[srcStrideP1] ) / 3 );
                dst[RGB.B] = (byte) ( ( src[srcMStride] + src[srcStride] ) >> 1 );

                dst -= dstStride;
                src -= srcStride;

                src++;
                dst += 3;

                for ( int x = 1; x < widthM1; x += 2 )
                {
                    // G R G
                    // B G B
                    // G R G
                    dst[RGB.R] = (byte) ( ( src[srcMStride] + src[srcStride] ) >> 1 );
                    dst[RGB.G] = (byte) ( ( *src + src[srcMStrideM1] + src[srcMStrideP1] +
                                            src[srcStrideM1] + src[srcStrideP1] ) / 5 );
                    dst[RGB.B] = (byte) ( ( src[-1] + src[1] ) >> 1 );

                    // ( y+1 pixel )
                    // B G B
                    // G R G
                    // B G B
                    dst += dstStride;
                    src += srcStride;

                    dst[RGB.R] = *src;
                    dst[RGB.G] = (byte) ( ( src[srcMStride] + src[srcStride] +
                                            src[-1] + src[1] ) >> 2 );
                    dst[RGB.B] = (byte) ( ( src[srcMStrideM1] + src[srcMStrideP1] +
                                            src[srcStrideM1] + src[srcStrideP1] ) >> 2 );

                    // ( y+1 x+1 pixel )
                    // G B G
                    // R G R
                    // G B G
                    dst += 3;
                    src++;

                    dst[RGB.R] = (byte) ( ( src[-1] + src[1] ) >> 1 );
                    dst[RGB.G] = (byte) ( ( *src + src[srcMStrideM1] + src[srcMStrideP1] +
                                            src[srcStrideM1] + src[srcStrideP1] ) / 5 );
                    dst[RGB.B] = (byte) ( ( src[srcMStride] + src[srcStride] ) >> 1 );

                    // ( x+1 pixel )
                    // R G R
                    // G B G
                    // R G R
                    dst -= dstStride;
                    src -= srcStride;

                    dst[RGB.R] = (byte) ( ( src[srcMStrideM1] + src[srcMStrideP1] +
                                            src[srcStrideM1] + src[srcStrideP1] ) >> 2 );
                    dst[RGB.G] = (byte) ( ( src[srcMStride] + src[srcStride] +
                                            src[-1] + src[1] ) >> 2 );
                    dst[RGB.B] = *src;

                    dst += 3;
                    src++;
                }

                // G R .
                // B G .
                // G R .
                dst[RGB.R] = (byte) ( ( src[srcMStride] + src[srcStride] ) >> 1 );
                dst[RGB.G] = (byte) ( ( *src + src[srcMStrideM1] + src[srcStrideM1] ) / 3 );
                dst[RGB.B] = (byte) src[-1];

                src += srcStride;
                dst += dstStride;

                // ( y+1 pixel )
                // B G .
                // G R .
                // B G .
                dst[RGB.R] = *src;
                dst[RGB.G] = (byte) ( ( src[srcMStride] + src[srcStride] + src[-1] ) / 3 );
                dst[RGB.B] = (byte) ( ( src[srcMStrideM1] + src[srcStrideM1] ) >> 1 );


                // allign to the next line
                src += srcOffset + 1;
                dst += dstOffset + 3;
            }

            // --- process the first line

            // . G R
            // . B G
            // . . .
            dst[RGB.R] = src[srcMStrideP1];
            dst[RGB.G] = (byte) ( ( src[srcMStride] + src[1] ) >> 1 );
            dst[RGB.B] = *src;

            src++;
            dst += 3;

            for ( int x = 1; x < widthM1; x += 2 )
            {
                // G R G
                // B G B
                // . . .
                dst[RGB.R] = src[srcMStride];
                dst[RGB.G] = (byte) ( ( src[srcMStrideM1] + src[srcMStrideP1] + *src ) / 3 );
                dst[RGB.B] = (byte) ( ( src[-1] + src[1] ) >> 1 );

                src++;
                dst += 3;

                // R G R
                // G B G
                // . . .
                dst[RGB.R] = (byte) ( ( src[srcMStrideM1] + src[srcMStrideP1] ) >> 1 );
                dst[RGB.G] = (byte) ( ( src[srcMStride] + src[-1] + src[1] ) / 3 );
                dst[RGB.B] = *src;

                src++;
                dst += 3;
            }

            // G R .
            // B G .
            // . . .
            dst[RGB.R] = src[srcMStride];
            dst[RGB.G] = (byte) ( ( *src + src[srcMStrideM1] ) >> 1 );
            dst[RGB.B] = src[-1];
        }
        #endregion

        #region BGGR pattern
        private unsafe void ApplyBGGR( UnmanagedImage sourceData, UnmanagedImage destinationData )
        {
            int width  = sourceData.Width;
            int height = sourceData.Height;

            int widthM1  = width - 1;
            int heightM1 = height - 1;

            int srcStride = sourceData.Stride;
            int dstStride = destinationData.Stride;

            int srcStrideP1  = srcStride + 1;
            int srcStrideM1  = srcStride - 1;
            int srcMStride   = -srcStride;
            int srcMStrideP1 = srcMStride + 1;
            int srcMStrideM1 = srcMStride - 1;

            int srcOffset = srcStride - width;
            int dstOffset = dstStride - width * 3;

            // do the job
            byte * src = (byte*) sourceData.ImageData.ToPointer( );
            byte * dst = (byte*) destinationData.ImageData.ToPointer( );

            // --- process the first line

            // . . .
            // . B G 
            // . G R
            dst[RGB.R] = src[srcStrideP1];
            dst[RGB.G] = (byte) ( ( src[1] + src[srcStride] ) >> 1 );
            dst[RGB.B] = *src;

            src++;
            dst += 3;

            for ( int x = 1; x < widthM1; x += 2 )
            {
                // . . .
                // B G B 
                // G R G
                dst[RGB.R] = src[srcStride];
                dst[RGB.G] = (byte) ( ( *src + src[srcStrideM1] + src[srcStrideP1] ) / 3 );
                dst[RGB.B] = (byte) ( ( src[-1] + src[1] ) >> 1 );

                src++;
                dst += 3;

                // . . .
                // G B G
                // R G R
                dst[RGB.R] = (byte) ( ( src[srcStrideM1] + src[srcStrideP1] ) >> 1 );
                dst[RGB.G] = (byte) ( ( src[-1] + src[srcStride] + src[1] ) / 3 );
                dst[RGB.B] = *src;

                src++;
                dst += 3;
            }

            // . . .
            // B G . 
            // G R . 
            dst[RGB.R] = src[srcStride];
            dst[RGB.G] = (byte) ( ( *src + src[srcStrideM1] ) >> 1 );
            dst[RGB.B] = src[-1];


            // allign to the next line
            src += srcOffset + 1;
            dst += dstOffset + 3;

            // --- process all lines except the first one and the last one
            for ( int y = 1; y < heightM1; y += 2 )
            {
                // . B G 
                // . G R
                // . B G
                dst[RGB.R] = src[1];
                dst[RGB.G] = (byte) ( ( src[srcMStrideP1] + src[srcStrideP1] + *src ) / 3 );
                dst[RGB.B] = (byte) ( ( src[srcMStride] + src[srcStride] ) >> 1 );

                dst += dstStride;
                src += srcStride;

                // ( y+1 pixel )
                // . G R
                // . B G
                // . G R
                dst[RGB.R] = (byte) ( ( src[srcMStrideP1] + src[srcStrideP1] ) >> 1 );
                dst[RGB.G] = (byte) ( ( src[1] + src[srcMStride] + src[srcStride] ) / 3 );
                dst[RGB.B] = *src;

                dst -= dstStride;
                src -= srcStride;

                src++;
                dst += 3;

                for ( int x = 1; x < widthM1; x += 2 )
                {
                    // B G B
                    // G R G
                    // B G B
                    dst[RGB.R] = *src;
                    dst[RGB.G] = (byte) ( ( src[srcMStride] + src[srcStride] + src[-1] + src[1] ) >> 2 );
                    dst[RGB.B] = (byte) ( ( src[srcMStrideM1] + src[srcMStrideP1] + src[srcStrideM1] + src[srcStrideP1] ) >> 2 );

                    // ( y+1 pixel )
                    // G R G
                    // B G B
                    // G R G
                    dst += dstStride;
                    src += srcStride;

                    dst[RGB.R] = (byte) ( ( src[srcMStride] + src[srcStride] ) >> 1 );
                    dst[RGB.G] = (byte) ( ( *src + src[srcMStrideM1] + src[srcMStrideP1] + src[srcStrideM1] + src[srcStrideP1] ) / 5 );
                    dst[RGB.B] = (byte) ( ( src[-1] + src[1] ) >> 1 );

                    // ( y+1 x+1 pixel )
                    // R G R
                    // G B G
                    // R G R
                    dst += 3;
                    src++;

                    dst[RGB.R] = (byte) ( ( src[srcMStrideM1] + src[srcMStrideP1] + src[srcStrideM1] + src[srcStrideP1] ) >> 2 );
                    dst[RGB.G] = (byte) ( ( src[srcMStride] + src[srcStride] + src[-1] + src[1] ) >> 2 );
                    dst[RGB.B] = *src;

                    // ( x+1 pixel )
                    // G B G
                    // R G R
                    // G B G
                    dst -= dstStride;
                    src -= srcStride;

                    dst[RGB.R] = (byte) ( ( src[-1] + src[1] ) >> 1 );
                    dst[RGB.G] = (byte) ( ( src[srcMStrideM1] + src[srcMStrideP1] + src[srcStrideM1] + src[srcStrideP1] + *src ) / 5 );
                    dst[RGB.B] = (byte) ( ( src[srcMStride] + src[srcStride] ) >> 1 );

                    dst += 3;
                    src++;
                }

                // B G .
                // G R .
                // B G .
                dst[RGB.R] = *src;
                dst[RGB.G] = (byte) ( ( src[srcMStride] + src[srcStride] + src[-1] ) / 3 );
                dst[RGB.B] = (byte) ( ( src[srcMStrideM1] + src[srcStrideM1] ) >> 1 );
                src += srcStride;
                dst += dstStride;

                // ( y+1 pixel )
                // G R .
                // B G .
                // G R .
                dst[RGB.R] = (byte) ( ( src[srcMStride] + src[srcStride] ) >> 1 );
                dst[RGB.G] = (byte) ( ( src[srcMStrideM1] + src[srcStrideM1] + *src ) / 3 );
                dst[RGB.B] = src[-1];

                // align to the next line
                src += srcOffset + 1;
                dst += dstOffset + 3;
            }

            // --- process the first line

            // . B G 
            // . G R 
            // . . .
            dst[RGB.R] = src[1];
            dst[RGB.G] = (byte) ( ( src[srcMStrideP1] + *src ) >> 1 );
            dst[RGB.B] = src[srcMStride];

            src++;
            dst += 3;

            for ( int x = 1; x < widthM1; x += 2 )
            {
                // B G B 
                // G R G 
                // . . .
                dst[RGB.R] = *src;
                dst[RGB.G] = (byte) ( ( src[-1] + src[1] + src[srcMStride] ) / 3 );
                dst[RGB.B] = (byte) ( ( src[srcMStrideM1] + src[srcMStrideP1] ) >> 1 );

                src++;
                dst += 3;

                // G B G
                // R G R 
                // . . .
                dst[RGB.R] = (byte) ( ( src[-1] + src[1] ) >> 1 );
                dst[RGB.G] = (byte) ( ( *src + src[srcMStrideM1] + src[srcMStrideP1] ) / 3 );
                dst[RGB.B] = src[srcMStride];

                src++;
                dst += 3;
            }

            // B G . 
            // G R . 
            // . . .
            dst[RGB.R] = *src;
            dst[RGB.G] = (byte) ( ( src[srcMStride] + src[-1] ) >> 1 );
            dst[RGB.B] = src[srcMStrideM1];
        }
        #endregion
    }
}
