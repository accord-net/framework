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
    using AForge.Math.Random;

    /// <summary>
    /// Additive noise filter.
    /// </summary>
    /// 
    /// <remarks><para>The filter adds random value to each pixel of the source image.
    /// The distribution of random values can be specified by <see cref="Generator">random generator</see>.
    /// </para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images and 24 bpp
    /// color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create random generator
    /// IRandomNumberGenerator generator = new UniformGenerator( new Range( -50, 50 ) );
    /// // create filter
    /// AdditiveNoise filter = new AdditiveNoise( generator );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/additive_noise.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    public class AdditiveNoise : BaseInPlacePartialFilter
    {
        // random number generator to add noise
        IRandomNumberGenerator generator = new UniformGenerator( new Range( -10, 10 ) );

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
        /// Random number genertor used to add noise.
        /// </summary>
        /// 
        /// <remarks>Default generator is uniform generator in the range of (-10, 10).</remarks>
        /// 
        public IRandomNumberGenerator Generator
        {
            get { return generator; }
            set { generator = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdditiveNoise"/> class.
        /// </summary>
        /// 
        public AdditiveNoise( )
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdditiveNoise"/> class.
        /// </summary>
        /// 
        /// <param name="generator">Random number genertor used to add noise.</param>
        /// 
        public AdditiveNoise( IRandomNumberGenerator generator )
            : this( )
        {
            this.generator = generator;
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

            int startY  = rect.Top;
            int stopY   = startY + rect.Height;

            int startX  = rect.Left * pixelSize;
            int stopX   = startX + rect.Width * pixelSize;

            int offset  = image.Stride - ( stopX - startX );

            // do the job
            byte* ptr = (byte*) image.ImageData.ToPointer( );

            // allign pointer to the first pixel to process
            ptr += ( startY * image.Stride + rect.Left * pixelSize );

            // for each line
            for ( int y = startY; y < stopY; y++ )
            {
                // for each pixel
                for ( int x = startX; x < stopX; x++, ptr++ )
                {
                    *ptr = (byte) Math.Max( 0, Math.Min( 255, *ptr + generator.Next( ) ) );
                }
                ptr += offset;
            }
        }
    }
}
