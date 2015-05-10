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
    /// Threshold using Simple Image Statistics (SIS).
    /// </summary>
    /// 
    /// <remarks><para>The filter performs image thresholding calculating threshold automatically
    /// using simple image statistics method. For each pixel:
    /// <list type="bullet">
    /// <item>two gradients are calculated - ex = |I(x + 1, y) - I(x - 1, y)| and
    /// |I(x, y + 1) - I(x, y - 1)|;</item>
    /// <item>weight is calculated as maximum of two gradients;</item>
    /// <item>sum of weights is updated (weightTotal += weight);</item>
    /// <item>sum of weighted pixel values is updated (total += weight * I(x, y)).</item>
    /// </list>
    /// The result threshold is calculated as sum of weighted pixel values divided by sum of weight.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// SISThreshold filter = new SISThreshold( );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample11.png" width="256" height="256" />
    /// <para><b>Result image (calculated threshold is 127):</b></para>
    /// <img src="img/imaging/sis_threshold.png" width="256" height="256" />
    /// </remarks>
    /// 
    /// <seealso cref="IterativeThreshold"/>
    /// <seealso cref="OtsuThreshold"/>
    /// 
    public class SISThreshold : BaseInPlacePartialFilter
    {
        private Threshold thresholdFilter = new Threshold( );

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
        /// Threshold value.
        /// </summary>
        /// 
        /// <remarks><para>The property is read only and represents the value, which
        /// was automaticaly calculated using image statistics.</para></remarks>
        /// 
        public int ThresholdValue
        {
            get { return thresholdFilter.ThresholdValue; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SISThreshold"/> class.
        /// </summary>
        /// 
        public SISThreshold( )
        {
            // initialize format translation dictionary
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
        }

        /// <summary>
        /// Calculate binarization threshold for the given image.
        /// </summary>
        /// 
        /// <param name="image">Image to calculate binarization threshold for.</param>
        /// <param name="rect">Rectangle to calculate binarization threshold for.</param>
        /// 
        /// <returns>Returns binarization threshold.</returns>
        /// 
        /// <remarks><para>The method is used to calculate binarization threshold only. The threshold
        /// later may be applied to the image using <see cref="Threshold"/> image processing filter.</para></remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Source pixel format is not supported by the routine. It should be
        /// 8 bpp grayscale (indexed) image.</exception>
        /// 
        public int CalculateThreshold( Bitmap image, Rectangle rect )
        {
            int calculatedThreshold = 0;

            // lock source bitmap data
            BitmapData data = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, image.PixelFormat );

            try
            {
                calculatedThreshold = CalculateThreshold( data, rect );
            }
            finally
            {
                // unlock image
                image.UnlockBits( data );
            }

            return calculatedThreshold;
        }

        /// <summary>
        /// Calculate binarization threshold for the given image.
        /// </summary>
        /// 
        /// <param name="image">Image to calculate binarization threshold for.</param>
        /// <param name="rect">Rectangle to calculate binarization threshold for.</param>
        /// 
        /// <returns>Returns binarization threshold.</returns>
        /// 
        /// <remarks><para>The method is used to calculate binarization threshold only. The threshold
        /// later may be applied to the image using <see cref="Threshold"/> image processing filter.</para></remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Source pixel format is not supported by the routine. It should be
        /// 8 bpp grayscale (indexed) image.</exception>
        /// 
        public int CalculateThreshold( BitmapData image, Rectangle rect )
        {
            return CalculateThreshold( new UnmanagedImage( image ), rect );
        }

        /// <summary>
        /// Calculate binarization threshold for the given image.
        /// </summary>
        /// 
        /// <param name="image">Image to calculate binarization threshold for.</param>
        /// <param name="rect">Rectangle to calculate binarization threshold for.</param>
        /// 
        /// <returns>Returns binarization threshold.</returns>
        /// 
        /// <remarks><para>The method is used to calculate binarization threshold only. The threshold
        /// later may be applied to the image using <see cref="Threshold"/> image processing filter.</para></remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Source pixel format is not supported by the routine. It should be
        /// 8 bpp grayscale (indexed) image.</exception>
        /// 
        public int CalculateThreshold( UnmanagedImage image, Rectangle rect )
        {
            if ( image.PixelFormat != PixelFormat.Format8bppIndexed )
                throw new UnsupportedImageFormatException( "Source pixel format is not supported by the routine." );

            int startX  = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;
            int stopXM1 = stopX - 1;
            int stopYM1 = stopY - 1;
            int stride  = image.Stride;
            int offset  = stride - rect.Width;

            // differences and weights
            double ex, ey, weight, weightTotal = 0, total = 0;

            unsafe
            {
                // do the job
                byte* ptr = (byte*) image.ImageData.ToPointer( );

                // allign pointer to the first pixel to process
                ptr += ( startY * image.Stride + startX );

                // skip the first line for the first pass
                ptr += stride;

                // for each line
                for ( int y = startY + 1; y < stopYM1; y++ )
                {
                    ptr++;
                    // for each pixels
                    for ( int x = startX + 1; x < stopXM1; x++, ptr++ )
                    {
                        // the equations are:
                        // ex = | I(x + 1, y) - I(x - 1, y) |
                        // ey = | I(x, y + 1) - I(x, y - 1) |
                        // weight = max(ex, ey)
                        // weightTotal += weight
                        // total += weight * I(x, y)

                        ex = Math.Abs( ptr[1] - ptr[-1] );
                        ey = Math.Abs( ptr[stride] - ptr[-stride] );
                        weight = ( ex > ey ) ? ex : ey;
                        weightTotal += weight;
                        total += weight * ( *ptr );
                    }
                    ptr += offset + 1;
                }
            }

            // calculate threshold
            return ( weightTotal == 0 ) ? (byte) 0 : (byte) ( total / weightTotal );
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
            // calculate threshold
            thresholdFilter.ThresholdValue = CalculateThreshold( image, rect );

            // thresholding
            thresholdFilter.ApplyInPlace( image, rect );
        }
    }
}
