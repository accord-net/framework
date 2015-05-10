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
    using AForge;

    /// <summary>
    /// Channels filters.
    /// </summary>
    /// 
    /// <remarks><para>The filter does color channels' filtering by clearing (filling with
    /// specified values) values, which are inside/outside of the specified value's
    /// range. The filter allows to fill certain ranges of RGB color channels with specified
    /// value.</para>
    /// 
    /// <para>The filter is similar to <see cref="ColorFiltering"/>, but operates with not
    /// entire pixels, but with their RGB values individually. This means that pixel itself may
    /// not be filtered (will be kept), but one of its RGB values may be filtered if they are
    /// inside/outside of specified range.</para>
    /// 
    /// <para>The filter accepts 24 and 32 bpp color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// ChannelFiltering filter = new ChannelFiltering( );
    /// // set channels' ranges to keep
    /// filter.Red   = new IntRange(   0, 255 );
    /// filter.Green = new IntRange( 100, 255 );
    /// filter.Blue  = new IntRange( 100, 255 );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/channel_filtering.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    /// <seealso cref="ColorFiltering"/>
    /// 
    public class ChannelFiltering : BaseInPlacePartialFilter
    {
        private IntRange red   = new IntRange( 0, 255 );
        private IntRange green = new IntRange( 0, 255 );
        private IntRange blue  = new IntRange( 0, 255 );

        private byte fillR = 0;
        private byte fillG = 0;
        private byte fillB = 0;

        private bool redFillOutsideRange = true;
        private bool greenFillOutsideRange = true;
        private bool blueFillOutsideRange = true;

        private byte[] mapRed   = new byte[256];
        private byte[] mapGreen = new byte[256];
        private byte[] mapBlue  = new byte[256];

        // private format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>( );

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        #region Public properties

        /// <summary>
        /// Red channel's range.
        /// </summary>
        public IntRange Red
        {
            get { return red; }
            set
            {
                red = value;
                CalculateMap( red, fillR, redFillOutsideRange, mapRed );
            }
        }

        /// <summary>
        /// Red fill value.
        /// </summary>
        public byte FillRed
        {
            get { return fillR; }
            set
            {
                fillR = value;
                CalculateMap( red, fillR, redFillOutsideRange, mapRed );
            }
        }

        /// <summary>
        /// Green channel's range.
        /// </summary>
        public IntRange Green
        {
            get { return green; }
            set
            {
                green = value;
                CalculateMap( green, fillG, greenFillOutsideRange, mapGreen );
            }
        }

        /// <summary>
        /// Green fill value.
        /// </summary>
        public byte FillGreen
        {
            get { return fillG; }
            set
            {
                fillG = value;
                CalculateMap( green, fillG, greenFillOutsideRange, mapGreen );
            }
        }

        /// <summary>
        /// Blue channel's range.
        /// </summary>
        public IntRange Blue
        {
            get { return blue; }
            set
            {
                blue = value;
                CalculateMap( blue, fillB, blueFillOutsideRange, mapBlue );
            }
        }

        /// <summary>
        /// Blue fill value.
        /// </summary>
        public byte FillBlue
        {
            get { return fillB; }
            set
            {
                fillB = value;
                CalculateMap( blue, fillB, blueFillOutsideRange, mapBlue );
            }
        }

        /// <summary>
        /// Determines, if red channel should be filled inside or outside filtering range.
        /// </summary>
        /// 
        /// <remarks>Default value is set to <see langword="true"/>.</remarks>
        /// 
        public bool RedFillOutsideRange
        {
            get { return redFillOutsideRange; }
            set
            {
                redFillOutsideRange = value;
                CalculateMap( red, fillR, redFillOutsideRange, mapRed );
            }
        }

        /// <summary>
        /// Determines, if green channel should be filled inside or outside filtering range.
        /// </summary>
        /// 
        /// <remarks>Default value is set to <see langword="true"/>.</remarks>
        /// 
        public bool GreenFillOutsideRange
        {
            get { return greenFillOutsideRange; }
            set
            {
                greenFillOutsideRange = value;
                CalculateMap( green, fillG, greenFillOutsideRange, mapGreen );
            }
        }

        /// <summary>
        /// Determines, if blue channel should be filled inside or outside filtering range.
        /// </summary>
        /// 
        /// <remarks>Default value is set to <see langword="true"/>.</remarks>
        ///
        public bool BlueFillOutsideRange
        {
            get { return blueFillOutsideRange; }
            set
            {
                blueFillOutsideRange = value;
                CalculateMap( blue, fillB, blueFillOutsideRange, mapBlue );
            }
        }

        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelFiltering"/> class.
        /// </summary>
        /// 
        public ChannelFiltering( )
            : this( new IntRange( 0, 255 ), new IntRange( 0, 255 ), new IntRange( 0, 255 ) )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelFiltering"/> class.
        /// </summary>
        /// 
        /// <param name="red">Red channel's filtering range.</param>
        /// <param name="green">Green channel's filtering range.</param>
        /// <param name="blue">Blue channel's filtering range.</param>
        /// 
        public ChannelFiltering( IntRange red, IntRange green, IntRange blue )
        {
            Red   = red;
            Green = green;
            Blue  = blue;

            formatTranslations[PixelFormat.Format24bppRgb]  = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb]  = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
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
            // get pixel size
            int pixelSize = ( image.PixelFormat == PixelFormat.Format24bppRgb ) ? 3 : 4;

            int startX  = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;
            int offset  = image.Stride - rect.Width * pixelSize;

            // do the job
            byte* ptr = (byte*) image.ImageData.ToPointer( );

            // allign pointer to the first pixel to process
            ptr += ( startY * image.Stride + startX * pixelSize );

            // for each row
            for ( int y = startY; y < stopY; y++ )
            {
                // for each pixel
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

        /// <summary>
        /// Calculate filtering map.
        /// </summary>
        /// 
        /// <param name="range">Filtering range.</param>
        /// <param name="fill">Fillter value.</param>
        /// <param name="fillOutsideRange">Fill outside or inside the range.</param>
        /// <param name="map">Filtering map.</param>
        /// 
        private void CalculateMap( IntRange range, byte fill, bool fillOutsideRange, byte[] map )
        {
            for ( int i = 0; i < 256; i++ )
            {
                if ( ( i >= range.Min ) && ( i <= range.Max ) )
                {
                    map[i] = ( fillOutsideRange ) ? (byte) i : fill;
                }
                else
                {
                    map[i] = ( fillOutsideRange ) ? fill : (byte) i;
                }
            }
        }
    }
}
