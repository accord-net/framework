// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@aforgenet.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Sobel edge detector.
    /// </summary>
    /// 
    /// <remarks><para>The filter searches for objects' edges by applying Sobel operator.</para>
    /// 
    /// <para>Each pixel of the result image is calculated as approximated absolute gradient
    /// magnitude for corresponding pixel of the source image:
    /// <code lang="none">
    /// |G| = |Gx| + |Gy] ,
    /// </code>
    /// where Gx and Gy are calculate utilizing Sobel convolution kernels:
    /// <code lang="none">
    ///    Gx         Gy
    /// -1 0 +1    +1 +2 +1
    /// -2 0 +2     0  0  0
    /// -1 0 +1    -1 -2 -1
    /// </code>
    /// Using the above kernel the approximated magnitude for pixel <b>x</b> is calculate using
    /// the next equation:
    /// <code lang="none">
    /// P1 P2 P3
    /// P8  x P4
    /// P7 P6 P5
    /// 
    /// |G| = |P1 + 2P2 + P3 - P7 - 2P6 - P5| +
    ///       |P3 + 2P4 + P5 - P1 - 2P8 - P7|
    /// </code>
    /// </para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// SobelEdgeDetector filter = new SobelEdgeDetector( );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample2.jpg" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/sobel_edges.png" width="320" height="240" />
    /// </remarks>
    /// 
    /// <seealso cref="DifferenceEdgeDetector"/>
    /// <seealso cref="HomogenityEdgeDetector"/>
    /// 
    public class SobelEdgeDetector : BaseUsingCopyPartialFilter
    {
        private bool scaleIntensity = true;

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
        /// Scale intensity or not.
        /// </summary>
        /// 
        /// <remarks><para>The property determines if edges' pixels intensities of the result image
        /// should be scaled in the range of the lowest and the highest possible intensity
        /// values.</para>
        /// 
        /// <para>Default value is set to <see langword="true"/>.</para>
        /// </remarks>
        /// 
        public bool ScaleIntensity
        {
            get { return scaleIntensity; }
            set { scaleIntensity = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SobelEdgeDetector"/> class.
        /// </summary>
        /// 
        public SobelEdgeDetector( )
        {
            // initialize format translation dictionary
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
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
            // processing start and stop X,Y positions
            int startX  = rect.Left + 1;
            int startY  = rect.Top + 1;
            int stopX   = startX + rect.Width - 2;
            int stopY   = startY + rect.Height - 2;

            int dstStride = destination.Stride;
            int srcStride = source.Stride;

            int dstOffset = dstStride - rect.Width + 2;
            int srcOffset = srcStride - rect.Width + 2;

            // data pointers
            byte* src = (byte*) source.ImageData.ToPointer( );
            byte* dst = (byte*) destination.ImageData.ToPointer( );

            // allign pointers
            src += srcStride * startY + startX;
            dst += dstStride * startY + startX;

            // variables for gradient calculation
            double g, max = 0;

            // for each line
            for ( int y = startY; y < stopY; y++ )
            {
                // for each pixel
                for ( int x = startX; x < stopX; x++, src++, dst++ )
                {
                    g = Math.Min( 255,
                        Math.Abs( src[-srcStride - 1] + src[-srcStride + 1]
                                - src[ srcStride - 1] - src[ srcStride + 1]
                                + 2 * ( src[-srcStride] - src[srcStride] ) )
                      + Math.Abs( src[-srcStride + 1] + src[srcStride + 1]
                                - src[-srcStride - 1] - src[srcStride - 1]
                                + 2 * ( src[1] - src[-1] ) ) );

                    if ( g > max )
                        max = g;
                    *dst = (byte) g;
                }
                src += srcOffset;
                dst += dstOffset;
            }

            
            // do we need scaling
            if ( ( scaleIntensity ) && ( max != 255 ) )
            {
                // make the second pass for intensity scaling
                double factor = 255.0 / (double) max;
                dst = (byte*) destination.ImageData.ToPointer( );
                dst += dstStride * startY + startX;

                // for each line
                for ( int y = startY; y < stopY; y++ )
                {
                    // for each pixel
                    for ( int x = startX; x < stopX; x++, dst++ )
                    {
                        *dst = (byte) ( factor * ( *dst ) );
                    }
                    dst += dstOffset;
                }
            }
            
            // draw black rectangle to remove those pixels, which were not processed
            // (this needs to be done for those cases, when filter is applied "in place" -
            // source image is modified instead of creating new copy)
            Drawing.Rectangle( destination, rect, Color.Black );
        }
    }
}
