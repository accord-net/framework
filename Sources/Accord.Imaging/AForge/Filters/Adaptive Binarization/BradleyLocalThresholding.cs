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
    /// Adaptive thresholding using the internal image.
    /// </summary>
    /// 
    /// <remarks><para>The image processing routine implements local thresholding technique described
    /// by Derek Bradley and Gerhard Roth in the "Adaptive Thresholding Using the Integral Image" paper.
    /// </para>
    /// 
    /// <para>The brief idea of the algorithm is that every image's pixel is set to black if its brightness
    /// is <i>t</i> percent lower (see <see cref="PixelBrightnessDifferenceLimit"/>) than the average brightness
    /// of surrounding pixels in the window of the specified size (see <see cref="WindowSize"/>), othwerwise it is set
    /// to white.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create the filter
    /// BradleyLocalThresholding filter = new BradleyLocalThresholding( );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample20.png" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/bradley_local_thresholding.png" width="320" height="240" />
    /// </remarks>
    /// 
    public class BradleyLocalThresholding : BaseInPlaceFilter
    {
        // private format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>( );

        private int windowSize = 41;
        private float pixelBrightnessDifferenceLimit = 0.15f;

        /// <summary>
        /// Window size to calculate average value of pixels for.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies window size around processing pixel, which determines number of
        /// neighbor pixels to use for calculating their average brightness.</para>
        /// 
        /// <para>Default value is set to <b>41</b>.</para>
        /// 
        /// <para><note>The value should be odd.</note></para>
        /// </remarks>
        /// 
        public int WindowSize
        {
            get { return windowSize; }
            set { windowSize = Math.Max( 3, value | 1 ); }
        }

        /// <summary>
        /// Brightness difference limit between processing pixel and average value across neighbors.
        /// </summary>
        ///
        /// <remarks><para>The property specifies what is the allowed difference percent between processing pixel
        /// and average brightness of neighbor pixels in order to be set white. If the value of the
        /// current pixel is <i>t</i> percent (this property value) lower than the average then it is set
        /// to black, otherwise it is set to white. </para>
        /// 
        /// <para>Default value is set to <b>0.15</b>.</para>
        /// </remarks>
        ///
        public float PixelBrightnessDifferenceLimit
        {
            get { return pixelBrightnessDifferenceLimit; }
            set { pixelBrightnessDifferenceLimit = Math.Max( 0.0f, Math.Min( 1.0f, value ) ); }
        }

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        /// 
        /// <remarks><para>See <see cref="IFilterInformation.FormatTranslations"/> for more information.</para></remarks>
        ///
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>   
        /// Initializes a new instance of the <see cref="BradleyLocalThresholding"/> class.
        /// </summary>
        /// 
        public BradleyLocalThresholding( )
        {
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        ///
        protected override unsafe void ProcessFilter( UnmanagedImage image )
        {
            // create integral image
            IntegralImage im = IntegralImage.FromBitmap( image );

            int width    = image.Width;
            int height   = image.Height;
            int widthM1  = width - 1;
            int heightM1 = height - 1;

            int offset = image.Stride - width;
            int radius = windowSize / 2;

            float avgBrightnessPart = 1.0f - pixelBrightnessDifferenceLimit;

            byte* ptr = (byte*) image.ImageData.ToPointer( );

            for ( int y = 0; y < height; y++ )
            {
                // rectangle's Y coordinates
                int y1 = y - radius;
                int y2 = y + radius;

                if ( y1 < 0 )
                    y1 = 0;
                if ( y2 > heightM1 )
                    y2 = heightM1;

                for ( int x = 0; x < width; x++, ptr++ )
                {
                    // rectangle's X coordinates
                    int x1 = x - radius;
                    int x2 = x + radius;

                    if ( x1 < 0 )
                        x1 = 0;
                    if ( x2 > widthM1 )
                        x2 = widthM1;

                    *ptr = (byte) ( ( *ptr < (int) ( im.GetRectangleMeanUnsafe( x1, y1, x2, y2 ) * avgBrightnessPart ) ) ? 0 : 255 );
                }

                ptr += offset;
            }
        }
    }
}
