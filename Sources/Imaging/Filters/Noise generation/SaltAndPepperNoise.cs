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

    /// <summary>
    /// Salt and pepper noise.
    /// </summary>
    /// 
    /// <remarks><para>The filter adds random salt and pepper noise - sets
    /// maximum or minimum values to randomly selected pixels.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images and 24/32 bpp
    /// color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// SaltAndPepperNoise filter = new SaltAndPepperNoise( 10 );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/salt_noise.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    public class SaltAndPepperNoise : BaseInPlacePartialFilter
    {
        // noise amount in percents
        private double noiseAmount = 10;

        // random number generator
        private Random rand = new Random( );

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
        /// Amount of noise to generate in percents, [0, 100].
        /// </summary>
        /// 
        public double NoiseAmount
        {
            get { return noiseAmount; }
            set { noiseAmount = Math.Max( 0, Math.Min( 100, value ) ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaltAndPepperNoise"/> class.
        /// </summary>
        /// 
        public SaltAndPepperNoise( )
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb]    = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb]   = PixelFormat.Format32bppArgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaltAndPepperNoise"/> class.
        /// </summary>
        /// 
        /// <param name="noiseAmount">Amount of noise to generate in percents, [0, 100].</param>
        /// 
        public SaltAndPepperNoise( double noiseAmount )
            : this( )
        {
            this.noiseAmount = noiseAmount;
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
            int startX  = rect.Left;
            int startY  = rect.Top;
            int width   = rect.Width;
            int height  = rect.Height;
            int stride  = image.Stride;

            int noisyPixels = (int) ( ( width * height * noiseAmount ) / 100 );

            // values to set
            byte[] values = new byte[2] { 0, 255 };

            // do the job
            byte* ptr = (byte*) image.ImageData.ToPointer( );

            if ( image.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // grayscale image
                for ( int i = 0; i < noisyPixels; i++ )
                {
                    int x = startX + rand.Next( width );
                    int y = startY + rand.Next( height );

                    ptr[y * stride + x] = values[rand.Next( 2 )];
                }
            }
            else
            {
                int pixelSize = ( image.PixelFormat == PixelFormat.Format24bppRgb ) ? 3 : 4;

                // color image
                for ( int i = 0; i < noisyPixels; i++ )
                {
                    int x = startX + rand.Next( width );
                    int y = startY + rand.Next( height );
                    int colorPlane = rand.Next( 3 );

                    ptr[y * stride + x * pixelSize + colorPlane] = values[rand.Next( 2 )];
                }
            }
        }
    }
}
