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
    /// Otsu thresholding.
    /// </summary>
    /// 
    /// <remarks><para>The class implements Otsu thresholding, which is described in
    /// <b>N. Otsu, "A threshold selection method from gray-level histograms", IEEE Trans. Systems,
    /// Man and Cybernetics 9(1), pp. 62–66, 1979.</b></para>
    /// 
    /// <para>This implementation instead of minimizing the weighted within-class variance
    /// does maximization of between-class variance, what gives the same result. The approach is
    /// described in <a href="http://sampl.ece.ohio-state.edu/EE863/2004/ECE863-G-segclust2.ppt">this presentation</a>.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// OtsuThreshold filter = new OtsuThreshold( );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// // check threshold value
    /// byte t = filter.ThresholdValue;
    /// // ...
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample11.png" width="256" height="256" />
    /// <para><b>Result image (calculated threshold is 97):</b></para>
    /// <img src="img/imaging/otsu_threshold.png" width="256" height="256" />
    /// </remarks>
    /// 
    /// <seealso cref="IterativeThreshold"/>
    /// <seealso cref="SISThreshold"/>
    /// 
    public class OtsuThreshold : BaseInPlacePartialFilter
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
        /// <remarks>The property is read only and represents the value, which
        /// was automaticaly calculated using Otsu algorithm.</remarks>
        /// 
        public int ThresholdValue
        {
            get { return thresholdFilter.ThresholdValue; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OtsuThreshold"/> class.
        /// </summary>
        /// 
        public OtsuThreshold( )
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

            int calculatedThreshold = 0;

            // get start and stop X-Y coordinates
            int startX  = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;
            int offset  = image.Stride - rect.Width;

            // histogram array
            int[] integerHistogram = new int[256];
            double[] histogram = new double[256];

            unsafe
            {
                // collect histogram first
                byte* ptr = (byte*) image.ImageData.ToPointer( );

                // allign pointer to the first pixel to process
                ptr += ( startY * image.Stride + startX );

                // for each line	
                for ( int y = startY; y < stopY; y++ )
                {
                    // for each pixel
                    for ( int x = startX; x < stopX; x++, ptr++ )
                    {
                        integerHistogram[*ptr]++;
                    }
                    ptr += offset;
                }

                // pixels count in the processing region
                int pixelCount = ( stopX - startX ) * ( stopY - startY );
                // mean value of the processing region
                double imageMean = 0;

                for ( int i = 0; i < 256; i++ )
                {
                    histogram[i] = (double) integerHistogram[i] / pixelCount;
                    imageMean += histogram[i] * i;
                }

                double max = double.MinValue;

                // initial class probabilities
                double class1Probability = 0;
                double class2Probability = 1;

                // initial class 1 mean value
                double class1MeanInit = 0;

                // check all thresholds
                for ( int t = 0; ( t < 256 ) && ( class2Probability > 0 ); t++ )
                {
                    // calculate class means for the given threshold
                    double class1Mean = class1MeanInit;
                    double class2Mean = ( imageMean - ( class1Mean * class1Probability ) ) / class2Probability;

                    // calculate between class variance
                    double betweenClassVariance = ( class1Probability ) * ( 1.0 - class1Probability ) * Math.Pow( class1Mean - class2Mean, 2 );

                    // check if we found new threshold candidate
                    if ( betweenClassVariance > max )
                    {
                        max = betweenClassVariance;
                        calculatedThreshold = t;
                    }

                    // update initial probabilities and mean value
                    class1MeanInit *= class1Probability;

                    class1Probability += histogram[t];
                    class2Probability -= histogram[t];

                    class1MeanInit += (double) t * (double) histogram[t];

                    if ( class1Probability != 0 )
                    {
                        class1MeanInit /= class1Probability;
                    }
                }
            }

            return calculatedThreshold;
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
            // calculate threshold for the given image
            thresholdFilter.ThresholdValue = CalculateThreshold( image, rect );

            // thresholding
            thresholdFilter.ApplyInPlace( image, rect );
        }
    }
}
