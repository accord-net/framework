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
    using AForge;

    /// <summary>
    /// Linear correction of RGB channels.
    /// </summary>
    /// 
    /// <remarks><para>The filter performs linear correction of RGB channels by mapping specified
    /// channels' input ranges to output ranges. It is similar to the
    /// <see cref="ColorRemapping"/>, but the remapping is linear.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale and 24/32 bpp color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// LevelsLinear filter = new LevelsLinear( );
    /// // set ranges
    /// filter.InRed   = new IntRange( 30, 230 );
    /// filter.InGreen = new IntRange( 50, 240 );
    /// filter.InBlue  = new IntRange( 10, 210 );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/levels_linear.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    /// <seealso cref="HSLLinear"/>
    /// <seealso cref="YCbCrLinear"/>
    /// 
    public class LevelsLinear : BaseInPlacePartialFilter
    {
        private IntRange inRed      = new IntRange( 0, 255 );
        private IntRange inGreen    = new IntRange( 0, 255 );
        private IntRange inBlue     = new IntRange( 0, 255 );

        private IntRange outRed     = new IntRange( 0, 255 );
        private IntRange outGreen   = new IntRange( 0, 255 );
        private IntRange outBlue    = new IntRange( 0, 255 );

        private byte[] mapRed       = new byte[256];
        private byte[] mapGreen     = new byte[256];
        private byte[] mapBlue      = new byte[256];

        // private format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>( );

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        #region Public Propertis

        /// <summary>
        /// Red component's input range.
        /// </summary>
        public IntRange InRed
        {
            get { return inRed; }
            set
            {
                inRed = value;
                CalculateMap( inRed, outRed, mapRed );
            }
        }

        /// <summary>
        /// Green component's input range.
        /// </summary>
        public IntRange InGreen
        {
            get { return inGreen; }
            set
            {
                inGreen = value;
                CalculateMap( inGreen, outGreen, mapGreen );
            }
        }

        /// <summary>
        /// Blue component's input range.
        /// </summary>
        public IntRange InBlue
        {
            get { return inBlue; }
            set
            {
                inBlue = value;
                CalculateMap( inBlue, outBlue, mapBlue );
            }
        }

        /// <summary>
        /// Gray component's input range.
        /// </summary>
        public IntRange InGray
        {
            get { return inGreen; }
            set
            {
                inGreen = value;
                CalculateMap( inGreen, outGreen, mapGreen );
            }
        }

        /// <summary>
        /// Input range for RGB components.
        /// </summary>
        /// 
        /// <remarks>The property allows to set red, green and blue input ranges to the same value.</remarks>
        /// 
        public IntRange Input
        {
            set
            {
                inRed = inGreen = inBlue = value;
                CalculateMap( inRed, outRed, mapRed );
                CalculateMap( inGreen, outGreen, mapGreen );
                CalculateMap( inBlue, outBlue, mapBlue );
            }
        }

        /// <summary>
        /// Red component's output range.
        /// </summary>
        public IntRange OutRed
        {
            get { return outRed; }
            set
            {
                outRed = value;
                CalculateMap( inRed, outRed, mapRed );
            }
        }

        /// <summary>
        /// Green component's output range.
        /// </summary>
        public IntRange OutGreen
        {
            get { return outGreen; }
            set
            {
                outGreen = value;
                CalculateMap( inGreen, outGreen, mapGreen );
            }
        }

        /// <summary>
        /// Blue component's output range.
        /// </summary>
        public IntRange OutBlue
        {
            get { return outBlue; }
            set
            {
                outBlue = value;
                CalculateMap( inBlue, outBlue, mapBlue );
            }
        }

        /// <summary>
        /// Gray component's output range.
        /// </summary>
        public IntRange OutGray
        {
            get { return outGreen; }
            set
            {
                outGreen = value;
                CalculateMap( inGreen, outGreen, mapGreen );
            }
        }

        /// <summary>
        /// Output range for RGB components.
        /// </summary>
        /// 
        /// <remarks>The property allows to set red, green and blue output ranges to the same value.</remarks>
        /// 
        public IntRange Output
        {
            set
            {
                outRed = outGreen = outBlue = value;
                CalculateMap( inRed, outRed, mapRed );
                CalculateMap( inGreen, outGreen, mapGreen );
                CalculateMap( inBlue, outBlue, mapBlue );
            }
        }

        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="LevelsLinear"/> class.
        /// </summary>
        public LevelsLinear( )
        {
            CalculateMap( inRed, outRed, mapRed );
            CalculateMap( inGreen, outGreen, mapGreen );
            CalculateMap( inBlue, outBlue, mapBlue );

            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb]    = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb]   = PixelFormat.Format32bppArgb;
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
            int pixelSize = Image.GetPixelFormatSize( image.PixelFormat ) / 8;

            // processing start and stop X,Y positions
            int startX  = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;
            int offset  = image.Stride - rect.Width * pixelSize;

            // do the job
            byte* ptr = (byte*) image.ImageData.ToPointer( );

            // allign pointer to the first pixel to process
            ptr += ( startY * image.Stride + startX * pixelSize );

            if ( image.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // grayscale image
                for ( int y = startY; y < stopY; y++ )
                {
                    for ( int x = startX; x < stopX; x++, ptr++ )
                    {
                        // gray
                        *ptr = mapGreen[*ptr];
                    }
                    ptr += offset;
                }
            }
            else
            {
                // RGB image
                for ( int y = startY; y < stopY; y++ )
                {
                    for ( int x = startX; x < stopX; x++, ptr += pixelSize )
                    {
                        // red
                        ptr[RGB.R] = mapRed[ptr[RGB.R]];
                        // green
                        ptr[RGB.G] = mapGreen[ptr[RGB.G]];
                        // blue
                        ptr[RGB.B] = mapBlue[ptr[RGB.B]];
                    }
                    ptr += offset;
                }
            }
        }


        /// <summary>
        /// Calculate conversion map.
        /// </summary>
        /// 
        /// <param name="inRange">Input range.</param>
        /// <param name="outRange">Output range.</param>
        /// <param name="map">Conversion map.</param>
        /// 
        private void CalculateMap( IntRange inRange, IntRange outRange, byte[] map )
        {
            double k = 0, b = 0;

            if ( inRange.Max != inRange.Min )
            {
                k = (double) ( outRange.Max - outRange.Min ) / (double) ( inRange.Max - inRange.Min );
                b = (double) ( outRange.Min ) - k * inRange.Min;
            }

            for ( int i = 0; i < 256; i++ )
            {
                byte v = (byte) i;

                if ( v >= inRange.Max )
                    v = (byte) outRange.Max;
                else if ( v <= inRange.Min )
                    v = (byte) outRange.Min;
                else
                    v = (byte) ( k * v + b );

                map[i] = v;
            }
        }
    }
}
