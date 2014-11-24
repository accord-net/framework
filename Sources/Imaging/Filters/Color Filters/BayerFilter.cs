// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Generic Bayer fileter image processing routine.
    /// </summary>
    /// 
    /// <remarks><para>The class implements <a href="http://en.wikipedia.org/wiki/Bayer_filter">Bayer filter</a>
    /// routine, which creates color image out of grayscale image produced by image sensor built with
    /// Bayer color matrix.</para>
    /// 
    /// <para>This Bayer filter implementation is made generic by allowing user to specify used
    /// <see cref="BayerPattern">Bayer pattern</see>. This makes it slower. For optimized version
    /// of the Bayer filter see <see cref="BayerFilterOptimized"/> class, which implements Bayer filter
    /// specifically optimized for some well known patterns.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images and produces 24 bpp RGB image.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// BayerFilter filter = new BayerFilter( );
    /// // apply the filter
    /// Bitmap rgbImage = filter.Apply( image );
    /// </code>
    /// 
    /// <para><b>Source image:</b></para>
    /// <img src="img/imaging/sample23.png" width="640" height="480" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/bayer_filter.jpg" width="640" height="480" />
    /// </remarks>
    /// 
    /// <see cref="BayerFilterOptimized"/>
    /// 
    public class BayerFilter : BaseFilter
    {
        // private format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>( );

        private bool performDemosaicing = true;
        private int[,] bayerPattern = new int[2, 2] { { RGB.G, RGB.R }, { RGB.B, RGB.G } };

        /// <summary>
        /// Specifies if demosaicing must be done or not.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies if color demosaicing must be done or not.
        /// If the property is set to <see langword="false"/>, then pixels of the result color image
        /// are colored according to the <see cref="BayerPattern">Bayer pattern</see> used, i.e. every pixel
        /// of the source grayscale image is copied to corresponding color plane of the result image.
        /// If the property is set to <see langword="true"/>, then pixels of the result image
        /// are set to color, which is obtained by averaging color components from the 3x3 window - pixel
        /// itself plus 8 surrounding neighbors.</para>
        /// 
        /// <para>Default value is set to <see langword="true"/>.</para>
        /// </remarks>
        /// 
        public bool PerformDemosaicing
        {
            get { return performDemosaicing; }
            set { performDemosaicing = value; }
        }

        /// <summary>
        /// Specifies Bayer pattern used for decoding color image.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies 2x2 array of RGB color indexes, which set the
        /// Bayer patter used for decoding color image.</para>
        /// 
        /// <para>By default the property is set to:
        /// <code>
        /// new int[2, 2] { { RGB.G, RGB.R }, { RGB.B, RGB.G } }
        /// </code>,
        /// which corresponds to
        /// <code lang="none">
        /// G R
        /// B G
        /// </code>
        /// pattern.
        /// </para>
        /// </remarks>
        /// 
        public int[,] BayerPattern
        {
            get { return bayerPattern; }
            set
            {
                bayerPattern = value;
            }
        }

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        /// 
        /// <remarks><para>See <see cref="IFilterInformation.FormatTranslations"/>
        /// documentation for additional information.</para></remarks>
        /// 
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BayerFilter"/> class.
        /// </summary>
        /// 
        public BayerFilter( )
        {
            // initialize format translation dictionary
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format24bppRgb;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        /// 
        protected override unsafe void ProcessFilter( UnmanagedImage sourceData, UnmanagedImage destinationData )
        {
            // get width and height
            int width  = sourceData.Width;
            int height = sourceData.Height;

            int widthM1  = width - 1;
            int heightM1 = height - 1;

            int srcStride = sourceData.Stride;

            int srcOffset = srcStride - width;
            int dstOffset = destinationData.Stride - width * 3;

            // do the job
            byte * src = (byte*) sourceData.ImageData.ToPointer( );
            byte * dst = (byte*) destinationData.ImageData.ToPointer( );

            int[] rgbValues = new int[3];
            int[] rgbCounters = new int[3];

            if ( !performDemosaicing )
            {
                // for each line
                for ( int y = 0; y < height; y++ )
                {
                    // for each pixel
                    for ( int x = 0; x < width; x++, src++, dst += 3 )
                    {
                        dst[RGB.R] = dst[RGB.G] = dst[RGB.B] = 0;
                        dst[bayerPattern[y & 1, x & 1]] = *src;
                    }
                    src += srcOffset;
                    dst += dstOffset;
                }
            }
            else
            {
                // for each line
                for ( int y = 0; y < height; y++ )
                {
                    // for each pixel
                    for ( int x = 0; x < width; x++, src++, dst += 3 )
                    {
                        rgbValues[0] = rgbValues[1] = rgbValues[2] = 0;
                        rgbCounters[0] = rgbCounters[1] = rgbCounters[2] = 0;

                        int bayerIndex = bayerPattern[y & 1, x & 1];

                        rgbValues[bayerIndex] += *src;
                        rgbCounters[bayerIndex]++;

                        if ( x != 0 )
                        {
                            bayerIndex = bayerPattern[y & 1, ( x - 1 ) & 1];

                            rgbValues[bayerIndex] += src[-1];
                            rgbCounters[bayerIndex]++;
                        }

                        if ( x != widthM1 )
                        {
                            bayerIndex = bayerPattern[y & 1, ( x + 1 ) & 1];

                            rgbValues[bayerIndex] += src[1];
                            rgbCounters[bayerIndex]++;
                        }

                        if ( y != 0 )
                        {
                            bayerIndex = bayerPattern[( y - 1 ) & 1, x & 1];

                            rgbValues[bayerIndex] += src[-srcStride];
                            rgbCounters[bayerIndex]++;

                            if ( x != 0 )
                            {
                                bayerIndex = bayerPattern[( y - 1 ) & 1, ( x - 1 ) & 1];

                                rgbValues[bayerIndex] += src[-srcStride - 1];
                                rgbCounters[bayerIndex]++;
                            }

                            if ( x != widthM1 )
                            {
                                bayerIndex = bayerPattern[( y - 1 ) & 1, ( x + 1 ) & 1];

                                rgbValues[bayerIndex] += src[-srcStride + 1];
                                rgbCounters[bayerIndex]++;
                            }
                        }

                        if ( y != heightM1 )
                        {
                            bayerIndex = bayerPattern[( y + 1 ) & 1, x & 1];

                            rgbValues[bayerIndex] += src[srcStride];
                            rgbCounters[bayerIndex]++;

                            if ( x != 0 )
                            {
                                bayerIndex = bayerPattern[( y + 1 ) & 1, ( x - 1 ) & 1];

                                rgbValues[bayerIndex] += src[srcStride - 1];
                                rgbCounters[bayerIndex]++;
                            }

                            if ( x != widthM1 )
                            {
                                bayerIndex = bayerPattern[( y + 1 ) & 1, ( x + 1 ) & 1];

                                rgbValues[bayerIndex] += src[srcStride + 1];
                                rgbCounters[bayerIndex]++;
                            }
                        }

                        dst[RGB.R] = (byte) ( rgbValues[RGB.R] / rgbCounters[RGB.R] );
                        dst[RGB.G] = (byte) ( rgbValues[RGB.G] / rgbCounters[RGB.G] );
                        dst[RGB.B] = (byte) ( rgbValues[RGB.B] / rgbCounters[RGB.B] );
                    }
                    src += srcOffset;
                    dst += dstOffset;
                }
            }
        }
    }
}
