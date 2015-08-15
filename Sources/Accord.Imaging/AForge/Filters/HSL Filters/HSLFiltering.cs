// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge;

    /// <summary>
    /// Color filtering in HSL color space.
    /// </summary>
    /// 
    /// <remarks><para>The filter operates in <b>HSL</b> color space and filters
    /// pixels, which color is inside/outside of the specified HSL range -
    /// it keeps pixels with colors inside/outside of the specified range and fills the
    /// rest with <see cref="FillColor">specified color</see>.</para>
    /// 
    /// <para>The filter accepts 24 and 32 bpp color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// HSLFiltering filter = new HSLFiltering( );
    /// // set color ranges to keep
    /// filter.Hue = new IntRange( 335, 0 );
    /// filter.Saturation = new Range( 0.6f, 1 );
    /// filter.Luminance = new Range( 0.1f, 1 );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/hsl_filtering.jpg" width="480" height="361" />
    /// 
    /// <para>Sample usage with saturation update only:</para>
    /// <code>
    /// // create filter
    /// HSLFiltering filter = new HSLFiltering( );
    /// // configure the filter
    /// filter.Hue = new IntRange( 340, 20 );
    /// filter.UpdateLuminance = false;
    /// filter.UpdateHue = false;
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/hsl_filtering2.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    /// <seealso cref="ColorFiltering"/>
    /// <seealso cref="YCbCrFiltering"/>
    /// 
    public class HSLFiltering : BaseInPlacePartialFilter
    {
        private IntRange hue = new IntRange( 0, 359 );
        private Range saturation = new Range( 0.0f, 1.0f );
        private Range luminance = new Range( 0.0f, 1.0f );

        private int   fillH = 0;
        private float fillS = 0.0f;
        private float fillL = 0.0f;
        private bool  fillOutsideRange = true;

        private bool updateH = true;
        private bool updateS = true;
        private bool updateL = true;

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
        /// Range of hue component, [0, 359].
        /// </summary>
        /// 
        /// <remarks><note>Because of hue values are cycled, the minimum value of the hue
        /// range may have bigger integer value than the maximum value, for example [330, 30].</note></remarks>
        /// 
        public IntRange Hue
        {
            get { return hue; }
            set { hue = value; }
        }

        /// <summary>
        /// Range of saturation component, [0, 1].
        /// </summary>
        public Range Saturation
        {
            get { return saturation; }
            set { saturation = value; }
        }

        /// <summary>
        /// Range of luminance component, [0, 1].
        /// </summary>
        public Range Luminance
        {
            get { return luminance; }
            set { luminance = value; }
        }

        /// <summary>
        /// Fill color used to fill filtered pixels.
        /// </summary>
        public HSL FillColor
        {
            get { return new HSL( fillH, fillS, fillL ); }
            set
            {
                fillH = value.Hue;
                fillS = value.Saturation;
                fillL = value.Luminance;
            }
        }

        /// <summary>
        /// Determines, if pixels should be filled inside or outside specified
        /// color range.
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

        /// <summary>
        /// Determines, if hue value of filtered pixels should be updated.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies if hue of filtered pixels should be
        /// updated with value from <see cref="FillColor">fill color</see> or not.</para>
        /// 
        /// <para>Default value is set to <see langword="true"/>.</para></remarks>
        /// 
        public bool UpdateHue
        {
            get { return updateH; }
            set { updateH = value; }
        }

        /// <summary>
        /// Determines, if saturation value of filtered pixels should be updated.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies if saturation of filtered pixels should be
        /// updated with value from <see cref="FillColor">fill color</see> or not.</para>
        /// 
        /// <para>Default value is set to <see langword="true"/>.</para></remarks>
        /// 
        public bool UpdateSaturation
        {
            get { return updateS; }
            set { updateS = value; }
        }

        /// <summary>
        /// Determines, if luminance value of filtered pixels should be updated.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies if luminance of filtered pixels should be
        /// updated with value from <see cref="FillColor">fill color</see> or not.</para>
        /// 
        /// <para>Default value is set to <see langword="true"/>.</para></remarks>
        /// 
        public bool UpdateLuminance
        {
            get { return updateL; }
            set { updateL = value; }
        }

        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="HSLFiltering"/> class.
        /// </summary>
        public HSLFiltering( )
        {
            formatTranslations[PixelFormat.Format24bppRgb]  = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb]  = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HSLFiltering"/> class.
        /// </summary>
        /// 
        /// <param name="hue">Range of hue component.</param>
        /// <param name="saturation">Range of saturation component.</param>
        /// <param name="luminance">Range of luminance component.</param>
        /// 
        public HSLFiltering( IntRange hue, Range saturation, Range luminance ) :
            this( )
        {
            this.hue = hue;
            this.saturation = saturation;
            this.luminance = luminance;
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

            RGB rgb = new RGB( );
            HSL hsl = new HSL( );

            bool updated;

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
                    updated   = false;
                    rgb.Red   = ptr[RGB.R];
                    rgb.Green = ptr[RGB.G];
                    rgb.Blue  = ptr[RGB.B];

                    // convert to HSL
                    AForge.Imaging.HSL.FromRGB( rgb, hsl );

                    // check HSL values
                    if (
                        ( hsl.Saturation >= saturation.Min ) && ( hsl.Saturation <= saturation.Max ) &&
                        ( hsl.Luminance >= luminance.Min ) && ( hsl.Luminance <= luminance.Max ) &&
                        (
                        ( ( hue.Min < hue.Max ) && ( hsl.Hue >= hue.Min ) && ( hsl.Hue <= hue.Max ) ) ||
                        ( ( hue.Min > hue.Max ) && ( ( hsl.Hue >= hue.Min ) || ( hsl.Hue <= hue.Max ) ) )
                        )
                        )
                    {
                        if ( !fillOutsideRange )
                        {
                            if ( updateH ) hsl.Hue = fillH;
                            if ( updateS ) hsl.Saturation = fillS;
                            if ( updateL ) hsl.Luminance = fillL;

                            updated = true;
                        }
                    }
                    else
                    {
                        if ( fillOutsideRange )
                        {
                            if ( updateH ) hsl.Hue = fillH;
                            if ( updateS ) hsl.Saturation = fillS;
                            if ( updateL ) hsl.Luminance = fillL;

                            updated = true;
                        }
                    }

                    if ( updated )
                    {
                        // convert back to RGB
                        AForge.Imaging.HSL.ToRGB( hsl, rgb );

                        ptr[RGB.R] = rgb.Red;
                        ptr[RGB.G] = rgb.Green;
                        ptr[RGB.B] = rgb.Blue;
                    }
                }
                ptr += offset;
            }
        }
    }
}
