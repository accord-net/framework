// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2010
// contacts@aforgenet.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Contrast stretching filter.
    /// </summary>
    /// 
    /// <remarks><para>Contrast stretching (or as it is often called normalization) is a simple image enhancement
    /// technique that attempts to improve the contrast in an image by 'stretching' the range of intensity values
    /// it contains to span a desired range of values, e.g. the full range of pixel values that the image type
    /// concerned allows. It differs from the more sophisticated <see cref="HistogramEqualization">histogram equalization</see>
    /// in that it can only apply a linear scaling function to the image pixel values.</para>
    /// 
    /// <para>The result of this filter may be achieved by using <see cref="ImageStatistics"/> class, which allows to
    /// get pixels' intensities histogram, and <see cref="LevelsLinear"/> filter, which does linear correction
    /// of pixel's intensities.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale and 24 bpp color images.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// ContrastStretch filter = new ContrastStretch( );
    /// // process image
    /// filter.ApplyInPlace( sourceImage );
    /// </code>
    /// 
    /// <para><b>Source image:</b></para>
    /// <img src="img/imaging/sample5.jpg" width="480" height="387" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/contrast_stretch.jpg" width="480" height="387" />
    /// </remarks>
    /// 
    public class ContrastStretch : BaseInPlacePartialFilter
    {
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
        /// Initializes a new instance of the <see cref="ContrastStretch"/> class.
        /// </summary>
        public ContrastStretch( )
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb]    = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb]   = PixelFormat.Format32bppArgb;
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
            int pixelSize = Image.GetPixelFormatSize( image.PixelFormat ) / 8;

            int startX = rect.Left;
            int startY = rect.Top;
            int stopX  = startX + rect.Width;
            int stopY  = startY + rect.Height;
            int stride = image.Stride;
            int offset = stride - rect.Width * pixelSize;

            // levels linear correction filter is going to be used on STEP 2
            LevelsLinear levelsLinear = new LevelsLinear( );

            // STEP 1 - search for min and max pixel values
            byte* ptr = (byte*) image.ImageData.ToPointer( );

            // check image format
            if ( image.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // allign pointer to the first pixel to process
                ptr += ( startY * stride + startX );

                byte min = 255;
                byte max = 0;

                for ( int y = startY; y < stopY; y++ )
                {
                    for ( int x = startX; x < stopX; x++, ptr++ )
                    {
                        byte value = *ptr;

                        if ( value < min )
                            min = value;

                        if ( value > max )
                            max = value;
                    }
                    ptr += offset;
                }

                levelsLinear.InGray = new IntRange( min, max );
            }
            else
            {
                // allign pointer to the first pixel to process
                ptr += ( startY * stride + startX * pixelSize );

                byte minR = 255, minG = 255, minB = 255;
                byte maxR = 0,   maxG = 0,   maxB = 0;

                for ( int y = startY; y < stopY; y++ )
                {
                    for ( int x = startX; x < stopX; x++, ptr += pixelSize )
                    {
                        // red
                        byte value = ptr[RGB.R];

                        if ( value < minR )
                            minR = value;

                        if ( value > maxR )
                            maxR = value;

                        // green
                        value = ptr[RGB.G];

                        if ( value < minG )
                            minG = value;

                        if ( value > maxG )
                            maxG = value;

                        // blue
                        value = ptr[RGB.B];

                        if ( value < minB )
                            minB = value;

                        if ( value > maxB )
                            maxB = value;
                    }
                    ptr += offset;
                }

                levelsLinear.InRed   = new IntRange( minR, maxR );
                levelsLinear.InGreen = new IntRange( minG, maxG );
                levelsLinear.InBlue  = new IntRange( minB, maxB );
            }

            // STEP 2 - run levels linear correction
            levelsLinear.ApplyInPlace( image, rect );
        }
    }
}
