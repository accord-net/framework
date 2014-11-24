// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2012
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
    /// Contrast adjusting in RGB color space.
    /// </summary>
    /// 
    /// <remarks><para>The filter operates in <b>RGB</b> color space and adjusts
    /// pixels' contrast value by increasing RGB values of bright pixel and decreasing
    /// RGB values of dark pixels (or vise versa if contrast needs to be decreased).
    /// The filter is based on <see cref="LevelsLinear"/>
    /// filter and simply sets all input ranges to (<see cref="Factor"/>, 255-<see cref="Factor"/>) and
    /// all output range to (0, 255) in the case if the factor value is positive.
    /// If the factor value is negative, then all input ranges are set to
    /// (0, 255 ) and all output ranges are set to
    /// (-<see cref="Factor"/>, 255_<see cref="Factor"/>).</para>
    /// 
    /// <para>See <see cref="LevelsLinear"/> documentation forr more information about the base filter.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale and 24/32 bpp color images for processing.</para>
    ///
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// ContrastCorrection filter = new ContrastCorrection( 15 );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/contrast_correction.jpg" width="480" height="361" />
    /// </remarks>
    ///
    /// <seealso cref="LevelsLinear"/>
    /// 
    public class ContrastCorrection : BaseInPlacePartialFilter
    {
        private LevelsLinear baseFilter = new LevelsLinear( );
        private int factor;

        /// <summary>
        /// Contrast adjusting factor, [-127, 127].
        /// </summary>
        /// 
        /// <remarks><para>Factor which is used to adjust contrast. Factor values greater than
        /// 0 increase contrast making light areas lighter and dark areas darker. Factor values
        /// less than 0 decrease contrast - decreasing variety of contrast.</para>
        /// 
        /// <para>Default value is set to <b>10</b>.</para></remarks>
        /// 
        public int Factor
        {
            get { return factor; }
            set
            {
                factor = Math.Max( -127, Math.Min( 127, value ) );


                if ( factor > 0 )
                {
                    baseFilter.InRed = baseFilter.InGreen = baseFilter.InBlue = baseFilter.InGray =
                        new IntRange( factor, 255 - factor );

                    baseFilter.OutRed = baseFilter.OutGreen = baseFilter.OutBlue = baseFilter.OutGray =
                        new IntRange( 0, 255 );
                }
                else
                {
                    baseFilter.OutRed = baseFilter.OutGreen = baseFilter.OutBlue = baseFilter.OutGray =
                        new IntRange( -factor, 255 + factor );

                    baseFilter.InRed = baseFilter.InGreen = baseFilter.InBlue = baseFilter.InGray =
                        new IntRange( 0, 255 );
                }
            }
        }

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        /// 
        /// <remarks><para>See <see cref="IFilterInformation.FormatTranslations"/>
        /// documentation for additional information.</para></remarks>
        ///
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return baseFilter.FormatTranslations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContrastCorrection"/> class.
        /// </summary>
        /// 
        public ContrastCorrection( )
        {
            Factor = 10;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContrastCorrection"/> class.
        /// </summary>
        /// 
        /// <param name="factor">Contrast <see cref="Factor">adjusting factor</see>.</param>
        /// 
        public ContrastCorrection( int factor )
        {
            Factor = factor;
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
            baseFilter.ApplyInPlace( image, rect );
        }
    }
}
