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
    /// Threshold binarization with error carry.
    /// </summary>
    /// 
    /// <remarks><para>The filter is similar to <see cref="Threshold"/> filter in the way,
    /// that it also uses threshold value for image binarization. Unlike regular threshold
    /// filter, this filter uses cumulative pixel value in comparing with threshold value.
    /// If cumulative pixel value is below threshold value, then image pixel becomes black.
    /// If cumulative pixel value is equal or higher than threshold value, then image pixel
    /// becomes white and cumulative pixel value is decreased by 255. In the beginning of each
    /// image line the cumulative value is reset to 0.
    /// </para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// Threshold filter = new Threshold( 100 );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/grayscale.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/threshold_carry.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    public class ThresholdWithCarry : BaseInPlacePartialFilter
    {
        private byte threshold = 128;

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
        /// <remarks>Default value is 128.</remarks>
        /// 
        public byte ThresholdValue
        {
            get { return threshold; }
            set { threshold = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThresholdWithCarry"/> class.
        /// </summary>
        /// 
        public ThresholdWithCarry( )
        {
            // initialize format translation dictionary
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThresholdWithCarry"/> class.
        /// </summary>
        /// 
        /// <param name="threshold">Threshold value.</param>
        /// 
        public ThresholdWithCarry( byte threshold )
            : this( )
        {
            this.threshold = threshold;
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
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;
            int offset  = image.Stride - rect.Width;

            // value which is caried from pixel to pixel
            short carry = 0;

            // do the job
            byte* ptr = (byte*) image.ImageData.ToPointer( );

            // allign pointer to the first pixel to process
            ptr += ( startY * image.Stride + startX );

            // for each line	
            for ( int y = startY; y < stopY; y++ )
            {
                carry = 0;

                // for each pixel
                for ( int x = startX; x < stopX; x++, ptr++ )
                {
                    carry += *ptr;

                    if ( carry >= threshold )
                    {
                        *ptr = (byte) 255;
                        carry -= 255;
                    }
                    else
                    {
                        *ptr = (byte) 0;
                    }
                }
                ptr += offset;
            }
        }
    }
}
