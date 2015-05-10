// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © AForge.NET, 2007-2014
// aforge.net@gmail.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge;

    /// <summary>
    /// Saturation adjusting in HSL color space.
    /// </summary>
    /// 
    /// <remarks><para>The filter operates in <b>HSL</b> color space and adjusts
    /// pixels' saturation value, increasing it or decreasing by specified percentage.
    /// The filters is based on <see cref="HSLLinear"/> filter, passing work to it after
    /// recalculating saturation <see cref="AdjustValue">adjust value</see> to input/output
    /// ranges of the <see cref="HSLLinear"/> filter.</para>
    /// 
    /// <para>The filter accepts 24 and 32 bpp color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// SaturationCorrection filter = new SaturationCorrection( -0.5f );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/saturation_correction.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    public class SaturationCorrection : BaseInPlacePartialFilter
    {
        private float adjustValue;	// [-1, 1]

        /// <summary>
        /// Saturation adjust value, [-1, 1].
        /// </summary>
        /// 
        /// <remarks>Default value is set to <b>0.1</b>, which corresponds to increasing
        /// saturation by 10%.</remarks>
        /// 
        public float AdjustValue
        {
            get { return adjustValue; }
            set { adjustValue = Math.Max( -1.0f, Math.Min( 1.0f, value ) ); }
        }

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
        /// Initializes a new instance of the <see cref="SaturationCorrection"/> class.
        /// </summary>
        /// 
        public SaturationCorrection( ) : this( 0.1f )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaturationCorrection"/> class.
        /// </summary>
        /// 
        /// <param name="adjustValue">Saturation adjust value.</param>
        /// 
        public SaturationCorrection( float adjustValue )
        {
            AdjustValue = adjustValue;

            formatTranslations[PixelFormat.Format24bppRgb]   = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb]   = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb]  = PixelFormat.Format32bppArgb;
            formatTranslations[PixelFormat.Format32bppPArgb] = PixelFormat.Format32bppPArgb;
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
            int pixelSize = Bitmap.GetPixelFormatSize( image.PixelFormat ) / 8;

            int startX  = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;
            int offset  = image.Stride - rect.Width * pixelSize;

            RGB rgb = new RGB( );
            HSL hsl = new HSL( );

            float desaturationChangeFactor = 1.0f + adjustValue;

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
                    rgb.Red   = ptr[RGB.R];
                    rgb.Green = ptr[RGB.G];
                    rgb.Blue  = ptr[RGB.B];

                    // convert to HSL
                    AForge.Imaging.HSL.FromRGB( rgb, hsl );

                    if ( adjustValue > 0 )
                    {
                        hsl.Saturation += ( 1.0f - hsl.Saturation ) * adjustValue * hsl.Saturation;
                    }
                    else if ( adjustValue < 0 )
                    {
                        hsl.Saturation *= desaturationChangeFactor;
                    }

                    // convert back to RGB
                    AForge.Imaging.HSL.ToRGB( hsl, rgb );

                    ptr[RGB.R] = rgb.Red;
                    ptr[RGB.G] = rgb.Green;
                    ptr[RGB.B] = rgb.Blue;
                }
                ptr += offset;
            }
        }
    }
}
