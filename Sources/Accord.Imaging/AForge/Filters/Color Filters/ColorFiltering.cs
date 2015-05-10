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
    /// Color filtering.
    /// </summary>
    /// 
    /// <remarks><para>The filter filters pixels inside/outside of specified RGB color range -
    /// it keeps pixels with colors inside/outside of specified range and fills the rest with
    /// <see cref="FillColor">specified color</see>.</para>
    /// 
    /// <para>The filter accepts 24 and 32 bpp color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// ColorFiltering filter = new ColorFiltering( );
    /// // set color ranges to keep
    /// filter.Red   = new IntRange( 100, 255 );
    /// filter.Green = new IntRange( 0, 75 );
    /// filter.Blue  = new IntRange( 0, 75 );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/color_filtering.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    /// <seealso cref="ChannelFiltering"/>
    /// <seealso cref="EuclideanColorFiltering"/>
    /// <seealso cref="HSLFiltering"/>
    /// <seealso cref="YCbCrFiltering"/>
    /// 
    public class ColorFiltering : BaseInPlacePartialFilter
    {
        private IntRange red   = new IntRange( 0, 255 );
        private IntRange green = new IntRange( 0, 255 );
        private IntRange blue  = new IntRange( 0, 255 );

        private byte fillR = 0;
        private byte fillG = 0;
        private byte fillB = 0;
        private bool fillOutsideRange = true;

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
        /// Range of red color component.
        /// </summary>
        public IntRange Red
        {
            get { return red; }
            set { red = value; }
        }

        /// <summary>
        /// Range of green color component.
        /// </summary>
        public IntRange Green
        {
            get { return green; }
            set { green = value; }
        }

        /// <summary>
        /// Range of blue color component.
        /// </summary>
        public IntRange Blue
        {
            get { return blue; }
            set { blue = value; }
        }

        /// <summary>
        /// Fill color used to fill filtered pixels.
        /// </summary>
        public RGB FillColor
        {
            get { return new RGB( fillR, fillG, fillB ); }
            set
            {
                fillR = value.Red;
                fillG = value.Green;
                fillB = value.Blue;
            }
        }

        /// <summary>
        /// Determines, if pixels should be filled inside or outside of specified
        /// color ranges.
        /// </summary>
        /// 
        /// <remarks><para>Default value is set to <see langword="true"/>, which means
        /// the filter removes colors outside of the specified range.</para></remarks>
        /// 
        public bool FillOutsideRange
        {
            get { return fillOutsideRange; }
            set { fillOutsideRange = value; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorFiltering"/> class.
        /// </summary>
        /// 
        public ColorFiltering( )
        {
            formatTranslations[PixelFormat.Format24bppRgb]  = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb]  = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorFiltering"/> class.
        /// </summary>
        /// 
        /// <param name="red">Red components filtering range.</param>
        /// <param name="green">Green components filtering range.</param>
        /// <param name="blue">Blue components filtering range.</param>
        /// 
        public ColorFiltering( IntRange red, IntRange green, IntRange blue ) :
            this( )
        {
            this.red   = red;
            this.green = green;
            this.blue  = blue;
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
            byte r, g, b;

            // allign pointer to the first pixel to process
            ptr += ( startY * image.Stride + startX * pixelSize );

            // for each row
            for ( int y = startY; y < stopY; y++ )
            {
                // for each pixel
                for ( int x = startX; x < stopX; x++, ptr += pixelSize )
                {
                    r = ptr[RGB.R];
                    g = ptr[RGB.G];
                    b = ptr[RGB.B];

                    // check pixel
                    if (
                        ( r >= red.Min )   && ( r <= red.Max ) &&
                        ( g >= green.Min ) && ( g <= green.Max ) &&
                        ( b >= blue.Min )  && ( b <= blue.Max )
                        )
                    {
                        if ( !fillOutsideRange )
                        {
                            ptr[RGB.R] = fillR;
                            ptr[RGB.G] = fillG;
                            ptr[RGB.B] = fillB;
                        }
                    }
                    else
                    {
                        if ( fillOutsideRange )
                        {
                            ptr[RGB.R] = fillR;
                            ptr[RGB.G] = fillG;
                            ptr[RGB.B] = fillB;
                        }
                    }
                }
                ptr += offset;
            }
        }
    }
}
