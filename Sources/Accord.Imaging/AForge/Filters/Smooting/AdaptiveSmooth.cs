// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//
// Found description in
// "An Edge Detection Technique Using the Facet
// Model and Parameterized Relaxation Labeling"
// by Ioannis Matalas, Student Member, IEEE, Ralph Benjamin, and Richard Kitney
//
namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Adaptive Smoothing - noise removal with edges preserving.
    /// </summary>
    /// 
    /// <remarks><para>The filter is aimed to perform image smoothing, but keeping sharp edges.
    /// This makes it applicable to additive noise removal and smoothing objects' interiors, but
    /// not applicable for spikes (salt and pepper noise) removal.</para>
    /// 
    /// <para>The next calculations are done for each pixel:
    /// <list type="bullet">
    /// <item>weights are calculate for 9 pixels - pixel itself and 8 neighbors:
    /// <code lang="none">
    /// w(x, y) = exp( -1 * (Gx^2 + Gy^2) / (2 * factor^2) )
    /// Gx(x, y) = (I(x + 1, y) - I(x - 1, y)) / 2
    /// Gy(x, y) = (I(x, y + 1) - I(x, y - 1)) / 2
    /// </code>,
    /// where <see cref="Factor">factor</see> is a configurable value determining smoothing's quality.</item>
    /// <item>sum of 9 weights is calclated (weightTotal);</item>
    /// <item>sum of 9 weighted pixel values is calculatd (total);</item>
    /// <item>destination pixel is calculated as <b>total / weightTotal</b>.</item>
    /// </list></para>
    /// 
    /// <para>Description of the filter was found in <b>"An Edge Detection Technique Using
    /// the Facet Model and Parameterized Relaxation Labeling" by Ioannis Matalas, Student Member,
    /// IEEE, Ralph Benjamin, and Richard Kitney</b>.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images and 24 bpp
    /// color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// AdaptiveSmoothing filter = new AdaptiveSmoothing( );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample13.png" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/adaptive_smooth.png" width="480" height="361" />
    /// </remarks>
    /// 
    public class AdaptiveSmoothing : BaseUsingCopyPartialFilter
    {
        private double factor = 3.0;

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
        /// Factor value.
        /// </summary>
        /// 
        /// <remarks><para>Factor determining smoothing quality (see <see cref="AdaptiveSmoothing"/>
        /// documentation).</para>
        /// 
        /// <para>Default value is set to <b>3</b>.</para>
        /// </remarks>
        /// 
        public double Factor
        {
            get { return factor; }
            set { factor = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdaptiveSmoothing"/> class.
        /// </summary>
        /// 
        public AdaptiveSmoothing( )
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdaptiveSmoothing"/> class.
        /// </summary>
        /// 
        /// <param name="factor">Factor value.</param>
        /// 
        public AdaptiveSmoothing( double factor )
            : this( )
        {
            this.factor = factor;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="source">Source image data.</param>
        /// <param name="destination">Destination image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        protected override unsafe void ProcessFilter( UnmanagedImage source, UnmanagedImage destination, Rectangle rect )
        {
            int pixelSize  = Image.GetPixelFormatSize( source.PixelFormat ) / 8;
            int pixelSize2 = pixelSize * 2;

            // processing start and stop X,Y positions
            int startX  = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;

            int startXP2    = startX + 2;
            int startYP2    = startY + 2;
            int stopXM2     = stopX - 2;
            int stopYM2     = stopY - 2;

            int srcStride = source.Stride;
            int dstStride = destination.Stride;
            int srcOffset = srcStride - rect.Width * pixelSize;
            int dstOffset = dstStride - rect.Width * pixelSize;

            // gradient and weights
            double gx, gy, weight, weightTotal, total;
            // precalculated factor value
            double f = -8 * factor * factor;

            // do the job
            byte* src = (byte*) source.ImageData.ToPointer( ) + srcStride * 2;
            byte* dst = (byte*) destination.ImageData.ToPointer( ) + dstStride * 2;

            // allign pointers to the first pixel to process
            src += ( startY * srcStride + startX * pixelSize );
            dst += ( startY * dstStride + startX * pixelSize );

            for ( int y = startYP2; y < stopYM2; y++ )
            {
                src += pixelSize2;
                dst += pixelSize2;

                for ( int x = startXP2; x < stopXM2; x++ )
                {
                    for ( int i = 0; i < pixelSize; i++, src++, dst++ )
                    {
                        weightTotal = 0;
                        total = 0;

                        // original formulas for weight calculation:
                        // w(x, y) = exp( -1 * (Gx^2 + Gy^2) / (2 * factor^2) )
                        // Gx(x, y) = (I(x + 1, y) - I(x - 1, y)) / 2
                        // Gy(x, y) = (I(x, y + 1) - I(x, y - 1)) / 2
                        //
                        // here is a little bit optimized version

                        // x - 1, y - 1
                        gx = src[-srcStride] - src[-pixelSize2 - srcStride];
                        gy = src[-pixelSize] - src[-pixelSize - 2 * srcStride];
                        weight = System.Math.Exp( ( gx * gx + gy * gy ) / f );
                        total += weight * src[-pixelSize - srcStride];
                        weightTotal += weight;

                        // x, y - 1
                        gx = src[pixelSize - srcStride] - src[-pixelSize - srcStride];
                        gy = *src - src[-2 * srcStride];
                        weight = System.Math.Exp( ( gx * gx + gy * gy ) / f );
                        total += weight * src[-srcStride];
                        weightTotal += weight;

                        // x + 1, y - 1
                        gx = src[pixelSize2 - srcStride] - src[-srcStride];
                        gy = src[pixelSize] - src[pixelSize - 2 * srcStride];
                        weight = System.Math.Exp( ( gx * gx + gy * gy ) / f );
                        total += weight * src[pixelSize - srcStride];
                        weightTotal += weight;

                        // x - 1, y
                        gx = *src - src[-pixelSize2];
                        gy = src[-pixelSize + srcStride] - src[-pixelSize - srcStride];
                        weight = System.Math.Exp( ( gx * gx + gy * gy ) / f );
                        total += weight * src[-pixelSize];
                        weightTotal += weight;

                        // x, y
                        gx = src[pixelSize] - src[-pixelSize];
                        gy = src[srcStride] - src[-srcStride];
                        weight = System.Math.Exp( ( gx * gx + gy * gy ) / f );
                        total += weight * ( *src );
                        weightTotal += weight;

                        // x + 1, y
                        gx = src[pixelSize2] - *src;
                        gy = src[pixelSize + srcStride] - src[pixelSize - srcStride];
                        weight = System.Math.Exp( ( gx * gx + gy * gy ) / f );
                        total += weight * src[pixelSize];
                        weightTotal += weight;

                        // x - 1, y + 1
                        gx = src[srcStride] - src[-pixelSize2 + srcStride];
                        gy = src[-pixelSize + 2 * srcStride] - src[-pixelSize];
                        weight = System.Math.Exp( ( gx * gx + gy * gy ) / f );
                        total += weight * src[-pixelSize + srcStride];
                        weightTotal += weight;

                        // x, y + 1
                        gx = src[pixelSize + srcStride] - src[-pixelSize + srcStride];
                        gy = src[2 * srcStride] - *src;
                        weight = System.Math.Exp( ( gx * gx + gy * gy ) / f );
                        total += weight * src[srcStride];
                        weightTotal += weight;

                        // x + 1, y + 1
                        gx = src[pixelSize2 + srcStride] - src[srcStride];
                        gy = src[pixelSize + 2 * srcStride] - src[pixelSize];
                        weight = System.Math.Exp( ( gx * gx + gy * gy ) / f );
                        total += weight * src[pixelSize + srcStride];
                        weightTotal += weight;

                        // save destination value
                        *dst = ( weightTotal == 0.0 ) ? *src : (byte) System.Math.Min( total / weightTotal, 255.0 );
                    }
                }
                src += srcOffset + pixelSize2;
                dst += dstOffset + pixelSize2;
            }
        }
    }
}
