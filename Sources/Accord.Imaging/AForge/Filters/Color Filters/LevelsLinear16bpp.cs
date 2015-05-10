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
    /// Linear correction of RGB channels for images, which have 16 bpp planes (16 bit gray images or 48/64 bit colour images).
    /// </summary>
    /// 
    /// <remarks><para>The filter performs linear correction of RGB channels by mapping specified
    /// channels' input ranges to output ranges. This version of the filter processes only images
    /// with 16 bpp colour planes. See <see cref="LevelsLinear"/> for 8 bpp version.</para>
    /// 
    /// <para>The filter accepts 16 bpp grayscale and 48/64 bpp colour images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// LevelsLinear16bpp filter = new LevelsLinear16bpp( );
    /// // set ranges
    /// filter.InRed   = new IntRange( 3000, 42000 );
    /// filter.InGreen = new IntRange( 5000, 37500 );
    /// filter.InBlue  = new IntRange( 1000, 60000 );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="LevelsLinear"/>
    /// 
    public class LevelsLinear16bpp : BaseInPlacePartialFilter
    {
        private IntRange inRed   = new IntRange( 0, 65535 );
        private IntRange inGreen = new IntRange( 0, 65535 );
        private IntRange inBlue  = new IntRange( 0, 65535 );

        private IntRange outRed   = new IntRange( 0, 65535 );
        private IntRange outGreen = new IntRange( 0, 65535 );
        private IntRange outBlue  = new IntRange( 0, 65535 );

        private ushort[] mapRed   = new ushort[65536];
        private ushort[] mapGreen = new ushort[65536];
        private ushort[] mapBlue  = new ushort[65536];

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
        public LevelsLinear16bpp( )
        {
            CalculateMap( inRed, outRed, mapRed );
            CalculateMap( inGreen, outGreen, mapGreen );
            CalculateMap( inBlue, outBlue, mapBlue );

            formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
            formatTranslations[PixelFormat.Format48bppRgb]       = PixelFormat.Format48bppRgb;
            formatTranslations[PixelFormat.Format64bppArgb]      = PixelFormat.Format64bppArgb;
            formatTranslations[PixelFormat.Format64bppPArgb]     = PixelFormat.Format64bppPArgb;
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
            int pixelSize = Image.GetPixelFormatSize( image.PixelFormat ) / 16;

            // processing start and stop X,Y positions
            int startX  = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;
            int offset  = image.Stride - rect.Width * pixelSize;

            // do the job
            byte* basePtr =(byte*) image.ImageData.ToPointer( );

            if ( image.PixelFormat == PixelFormat.Format16bppGrayScale )
            {
                // grayscale image
                for ( int y = startY; y < stopY; y++ )
                {
                    ushort* ptr = (ushort*) ( basePtr + y * image.Stride ) + startX;

                    for ( int x = startX; x < stopX; x++, ptr++ )
                    {
                        // gray
                        *ptr = mapGreen[*ptr];
                    }
                }
            }
            else
            {
                // RGB image
                for ( int y = startY; y < stopY; y++ )
                {
                    ushort* ptr = (ushort*) ( basePtr + y * image.Stride ) + startX * pixelSize;

                    for ( int x = startX; x < stopX; x++, ptr += pixelSize )
                    {
                        // red
                        ptr[RGB.R] = mapRed[ptr[RGB.R]];
                        // green
                        ptr[RGB.G] = mapGreen[ptr[RGB.G]];
                        // blue
                        ptr[RGB.B] = mapBlue[ptr[RGB.B]];
                    }
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
        private void CalculateMap( IntRange inRange, IntRange outRange, ushort[] map )
        {
            double k = 0, b = 0;

            if ( inRange.Max != inRange.Min )
            {
                k = (double) ( outRange.Max - outRange.Min ) / (double) ( inRange.Max - inRange.Min );
                b = (double) ( outRange.Min ) - k * inRange.Min;
            }

            for ( int i = 0; i < 65536; i++ )
            {
                ushort v = (ushort) i;

                if ( v >= inRange.Max )
                    v = (ushort) outRange.Max;
                else if ( v <= inRange.Min )
                    v = (ushort) outRange.Min;
                else
                    v = (ushort) ( k * v + b );

                map[i] = v;
            }
        }
    }
}
