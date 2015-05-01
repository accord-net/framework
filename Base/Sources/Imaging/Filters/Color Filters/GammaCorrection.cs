// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Gamma correction filter.
    /// </summary>
    /// 
    /// <remarks><para>The filter performs <a href="http://en.wikipedia.org/wiki/Gamma_correction">gamma correction</a>
    /// of specified image in RGB color space. Each pixels' value is converted using the V<sub>out</sub>=V<sub>in</sub><sup>g</sup>
    /// equation, where <b>g</b> is <see cref="Gamma">gamma value</see>.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale and 24 bpp color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// GammaCorrection filter = new GammaCorrection( 0.5 );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/gamma.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    public class GammaCorrection : BaseInPlacePartialFilter
    {
        private double gamma;
        private byte[] table = new byte[256];

        // private format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>( );

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        /// Gamma value, [0.1, 5.0].
        /// </summary>
        /// 
        /// <remarks>Default value is set to <b>2.2</b>.</remarks>
        /// 
        public double Gamma
        {
            get { return gamma; }
            set
            {
                // get gamma value
                gamma = Math.Max( 0.1, Math.Min( 5.0, value ) );

                // calculate tranformation table
                double g = 1 / gamma;
                for ( int i = 0; i < 256; i++ )
                {
                    table[i] = (byte) Math.Min( 255, (int) ( Math.Pow( i / 255.0, g ) * 255 + 0.5 ) );
                }
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="GammaCorrection"/> class.
        /// </summary>
        public GammaCorrection( ) : this ( 2.2 )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GammaCorrection"/> class.
        /// </summary>
        /// 
        /// <param name="gamma">Gamma value.</param>
        /// 
        public GammaCorrection( double gamma )
        {
            Gamma = gamma;

            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
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
            int pixelSize = ( image.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;

            // processing start and stop X,Y positions
            int startX  = rect.Left * pixelSize;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width * pixelSize;
            int stopY   = startY + rect.Height;
            int offset  = image.Stride - rect.Width * pixelSize;

            // do the job
            byte* ptr = (byte*) image.ImageData.ToPointer( );

            // allign pointer to the first pixel to process
            ptr += ( startY * image.Stride + startX );

            // gamma correction
            for ( int y = startY; y < stopY; y++ )
            {
                for ( int x = startX; x < stopX; x++, ptr++ )
                {
                    // process each pixel
                    *ptr = table[*ptr];
                }
                ptr += offset;
            }
        }
    }
}
