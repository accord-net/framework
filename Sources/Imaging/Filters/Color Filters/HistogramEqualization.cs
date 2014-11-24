// AForge Image Processing Library
// AForge.NET framework
//
// Copyright ©
//   Andrew Kirillov (andrew.kirillov@gmail.com),
//   Mladen Prajdic  (spirit1_fe@yahoo.com)
// 2005-2008
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Histogram equalization filter.
    /// </summary>
    ///
    /// <remarks><para>The filter does histogram equalization increasing local contrast in images. The effect
    /// of histogram equalization can be better seen on images, where pixel values have close contrast values.
    /// Through this adjustment, pixels intensities can be better distributed on the histogram. This allows for
    /// areas of lower local contrast to gain a higher contrast without affecting the global contrast.
    /// </para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images and 24/32 bpp
    /// color images for processing.</para>
    /// 
    /// <para><note>For color images the histogram equalization is applied to each color plane separately.</note></para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// HistogramEqualization filter = new HistogramEqualization( );
    /// // process image
    /// filter.ApplyInPlace( sourceImage );
    /// </code>
    /// 
    /// <para><b>Source image:</b></para>
    /// <img src="img/imaging/sample5.jpg" width="480" height="387" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/equalized.jpg" width="480" height="387" />
    /// </remarks>
    ///
    public class HistogramEqualization : BaseInPlacePartialFilter
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
        /// Initializes a new instance of the <see cref="HistogramEqualization"/> class.
        /// </summary>
        public HistogramEqualization( )
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
            int pixelSize = ( image.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 :
                ( image.PixelFormat == PixelFormat.Format24bppRgb ) ? 3 : 4;

            int startX = rect.Left;
            int startY = rect.Top;
            int stopX  = startX + rect.Width;
            int stopY  = startY + rect.Height;
            int stride = image.Stride;
            int offset = stride - rect.Width * pixelSize;

            int numberOfPixels = ( stopX - startX ) * ( stopY - startY );

            // check image format
            if ( image.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // grayscale image
                byte* ptr = (byte*) image.ImageData.ToPointer( );
                // allign pointer to the first pixel to process
                ptr += ( startY * stride + startX );

                // calculate histogram
                int[] histogram = new int[256];
                for ( int y = startY; y < stopY; y++ )
                {
                    for ( int x = startX; x < stopX; x++, ptr++ )
                    {
                        histogram[*ptr]++;
                    }
                    ptr += offset;
                }

                // calculate new intensity levels
                byte[] equalizedHistogram = Equalize( histogram, numberOfPixels );

                // update pixels' intensities
                ptr = (byte*) image.ImageData.ToPointer( );
                // allign pointer to the first pixel to process
                ptr += ( startY * stride + startX );

                for ( int y = startY; y < stopY; y++ )
                {
                    for ( int x = startX; x < stopX; x++, ptr++ )
                    {
                        *ptr = equalizedHistogram[*ptr];
                    }
                    ptr += offset;
                }
            }
            else
            {
                // color image
                byte* ptr = (byte*) image.ImageData.ToPointer( );
                // allign pointer to the first pixel to process
                ptr += ( startY * stride + startX * pixelSize );

                // calculate histogram
                int[] histogramR = new int[256];
                int[] histogramG = new int[256];
                int[] histogramB = new int[256];

                for ( int y = startY; y < stopY; y++ )
                {
                    for ( int x = startX; x < stopX; x++, ptr += pixelSize )
                    {
                        histogramR[ptr[RGB.R]]++;
                        histogramG[ptr[RGB.G]]++;
                        histogramB[ptr[RGB.B]]++;
                    }
                    ptr += offset;
                }

                // calculate new intensity levels
                byte[] equalizedHistogramR = Equalize( histogramR, numberOfPixels );
                byte[] equalizedHistogramG = Equalize( histogramG, numberOfPixels );
                byte[] equalizedHistogramB = Equalize( histogramB, numberOfPixels );

                // update pixels' intensities
                ptr = (byte*) image.ImageData.ToPointer( );
                // allign pointer to the first pixel to process
                ptr += ( startY * stride + startX * pixelSize );

                for ( int y = startY; y < stopY; y++ )
                {
                    for ( int x = startX; x < stopX; x++, ptr += pixelSize )
                    {
                        ptr[RGB.R] = equalizedHistogramR[ptr[RGB.R]];
                        ptr[RGB.G] = equalizedHistogramG[ptr[RGB.G]];
                        ptr[RGB.B] = equalizedHistogramB[ptr[RGB.B]];
                    }
                    ptr += offset;
                }
            }
        }

        
        // Histogram 
        private byte[] Equalize( int[] histogram, long numPixel )
        {
            byte[] equalizedHistogram = new byte[256];
            float coef = 255.0f / numPixel;

            // calculate the first value
            float prev = histogram[0] * coef;
            equalizedHistogram[0] = (byte) prev;

            // calcualte the rest of values
            for ( int i = 1; i < 256; i++ )
            {
                prev += histogram[i] * coef;
                equalizedHistogram[i] = (byte) prev;
            }

            return equalizedHistogram;
        }
    }
}
