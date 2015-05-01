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
    /// Calculate Euclidean difference between two images and threshold it.
    /// </summary>
    /// 
    /// <remarks><para>The filter produces similar to <see cref="ThresholdedDifference"/>, however it uses
    /// Euclidean distance for finding difference between pixel values instead of Manhattan distance. Result of this
    /// image processing routine may be useful in motion detection applications or finding areas of significant
    /// difference.</para>
    /// 
    /// <para>The filter accepts 8 and 24/32color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// ThresholdedEuclideanDifference filter = new ThresholdedEuclideanDifference( 60 );
    /// // apply the filter
    /// filter.OverlayImage = backgroundImage;
    /// Bitmap resultImage = filter.Apply( sourceImage );
    /// </code>
    /// 
    /// <para><b>Source image:</b></para>
    /// <img src="img/imaging/object.jpg" width="320" height="240" />
    /// <para><b>Background image:</b></para>
    /// <img src="img/imaging/background.jpg" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/thresholded_euclidean_difference.png" width="320" height="240" />
    /// </remarks>
    /// 
    /// <seealso cref="ThresholdedDifference"/>
    /// 
    public class ThresholdedEuclideanDifference : BaseFilter2
    {
        // format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>( );

        private int threshold = 15;

        /// <summary>
        /// Difference threshold.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies difference threshold. If difference between pixels of processing image
        /// and overlay image is greater than this value, then corresponding pixel of result image is set to white; otherwise
        /// black.
        /// </para>
        /// 
        /// <para>Default value is set to <b>15</b>.</para></remarks>
        /// 
        public int Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }

        private int whitePixelsCount = 0;

        /// <summary>
        /// Number of pixels which were set to white in destination image during last image processing call.
        /// </summary>
        ///
        /// <remarks><para>The property may be useful to determine amount of difference between two images which,
        /// for example, may be treated as amount of motion in motion detection applications, etc.</para></remarks>
        ///
        public int WhitePixelsCount
        {
            get { return whitePixelsCount; }
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
        /// Initializes a new instance of the <see cref="ThresholdedEuclideanDifference"/> class.
        /// </summary>
        /// 
        public ThresholdedEuclideanDifference( )
        {
            // initialize format translation dictionary
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]    = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format32bppRgb]    = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format32bppArgb]   = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format32bppPArgb]  = PixelFormat.Format8bppIndexed;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThresholdedEuclideanDifference"/> class.
        /// </summary>
        /// 
        /// <param name="threshold">Difference threshold (see <see cref="Threshold"/>).</param>
        /// 
        public ThresholdedEuclideanDifference( int threshold ) : this( )
        {
            this.threshold = threshold;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="overlay">Overlay image data.</param>
        /// <param name="destinationData">Destination image data</param>
        /// 
        protected override unsafe void ProcessFilter( UnmanagedImage sourceData, UnmanagedImage overlay, UnmanagedImage destinationData )
        {
            whitePixelsCount = 0;

            // get source image size
            int width  = sourceData.Width;
            int height = sourceData.Height;
            int pixelSize = Bitmap.GetPixelFormatSize( sourceData.PixelFormat ) / 8;

            byte* src = (byte*) sourceData.ImageData.ToPointer( );
            byte* ovr = (byte*) overlay.ImageData.ToPointer( );
            byte* dst = (byte*) destinationData.ImageData.ToPointer( );

            if ( pixelSize == 1 )
            {
                // grayscale image
                int srcOffset = sourceData.Stride - width;
                int ovrOffset = overlay.Stride - width;
                int dstOffset = destinationData.Stride - width;

                // for each line
                for ( int y = 0; y < height; y++ )
                {
                    // for each pixel
                    for ( int x = 0; x < width; x++, src++, ovr++, dst++ )
                    {
                        int diff = *src - *ovr;

                        if ( diff < 0 )
                            diff = -diff;

                        if ( diff > threshold )
                        {
                            *dst = (byte) 255;
                            whitePixelsCount++;
                        }
                        else
                        {
                            *dst = 0;
                        }
                    }
                    src += srcOffset;
                    ovr += ovrOffset;
                    dst += dstOffset;
                }
            }
            else
            {
                // color image
                int srcOffset = sourceData.Stride - pixelSize * width;
                int ovrOffset = overlay.Stride - pixelSize * width;
                int dstOffset = destinationData.Stride - width;

                int squaredThreshold = threshold * threshold;

                // for each line
                for ( int y = 0; y < height; y++ )
                {
                    // for each pixel
                    for ( int x = 0; x < width; x++, src += pixelSize, ovr += pixelSize, dst++ )
                    {
                        int diffR = src[RGB.R] - ovr[RGB.R];
                        int diffG = src[RGB.G] - ovr[RGB.G];
                        int diffB = src[RGB.B] - ovr[RGB.B];

                        if ( diffR * diffR + diffG * diffG + diffB * diffB > squaredThreshold )
                        {
                            *dst = (byte) 255;
                            whitePixelsCount++;
                        }
                        else
                        {
                            *dst = 0;
                        }
                    }
                    src += srcOffset;
                    ovr += ovrOffset;
                    dst += dstOffset;
                }
            }
        }
    }
}
