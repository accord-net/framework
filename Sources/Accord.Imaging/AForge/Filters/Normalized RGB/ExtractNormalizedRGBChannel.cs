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
    /// Extract normalized RGB channel from color image.
    /// </summary>
    /// 
    /// <remarks><para>Extracts specified normalized RGB channel of color image and returns
    /// it as grayscale image.</para>
    /// 
    /// <para><note>Normalized RGB color space is defined as:
    /// <code lang="none">
    /// r = R / (R + G + B ),
    /// g = G / (R + G + B ),
    /// b = B / (R + G + B ),
    /// </code>
    /// where <b>R</b>, <b>G</b> and <b>B</b> are components of RGB color space and
    /// <b>r</b>, <b>g</b> and <b>b</b> are components of normalized RGB color space.
    /// </note></para>
    /// 
    /// <para>The filter accepts 24, 32, 48 and 64 bpp color images and produces
    /// 8 (if source is 24 or 32 bpp image) or 16 (if source is 48 or 64 bpp image)
    /// bpp grayscale image.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// ExtractNormalizedRGBChannel filter = new ExtractNormalizedRGBChannel( RGB.G );
    /// // apply the filter
    /// Bitmap channelImage = filter.Apply( image );
    /// </code>
    /// 
    /// </remarks>
    /// 
    /// <seealso cref="ExtractChannel"/>
    /// 
    public class ExtractNormalizedRGBChannel : BaseFilter
    {
        private short channel = RGB.R;

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
        /// Normalized RGB channel to extract.
        /// </summary>
        /// 
        /// <remarks><para>Default value is set to <see cref="AForge.Imaging.RGB.R"/>.</para></remarks>
        /// 
        /// <exception cref="ArgumentException">Invalid channel is specified.</exception>
        /// 
        public short Channel
        {
            get { return channel; }
            set
            {
                if ( ( value != RGB.R ) && ( value != RGB.G ) && ( value != RGB.B ) )
                {
                    throw new ArgumentException( "Invalid channel is specified." );
                }
                channel = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractNormalizedRGBChannel"/> class.
        /// </summary>
        /// 
        public ExtractNormalizedRGBChannel( )
        {
            // initialize format translation dictionary
            formatTranslations[PixelFormat.Format24bppRgb]  = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format32bppRgb]  = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format48bppRgb]  = PixelFormat.Format16bppGrayScale;
            formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format16bppGrayScale;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractNormalizedRGBChannel"/> class.
        /// </summary>
        /// 
        /// <param name="channel">Normalized RGB channel to extract.</param>
        /// 
        public ExtractNormalizedRGBChannel( short channel )
            : this( )
        {
            this.Channel = channel;
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

            int pixelSize = Image.GetPixelFormatSize( sourceData.PixelFormat ) / 8;
            int sum;

            if ( pixelSize <= 4 )
            {
                int srcOffset = sourceData.Stride - width * pixelSize;
                int dstOffset = destinationData.Stride - width;

                // do the job
                byte * src = (byte*) sourceData.ImageData.ToPointer( );
                byte * dst = (byte*) destinationData.ImageData.ToPointer( );

                for ( int y = 0; y < height; y++ )
                {
                    for ( int x = 0; x < width; x++, src += pixelSize, dst++ )
                    {
                        sum = ( src[RGB.R] + src[RGB.G] + src[RGB.B] );

                        *dst = ( sum != 0 ) ? (byte) ( 255 * src[channel] / sum ) : (byte) 0;
                    }
                    src += srcOffset;
                    dst += dstOffset;
                }
            }
            else
            {
                pixelSize /= 2;

                byte* srcBase   = (byte*) sourceData.ImageData.ToPointer( );
                byte* dstBase   = (byte*) destinationData.ImageData.ToPointer( );
                int srcStride = sourceData.Stride;
                int dstStride = destinationData.Stride;

                // for each line
                for ( int y = 0; y < height; y++ )
                {
                    ushort* src = (ushort*) ( srcBase + y * srcStride );
                    ushort* dst = (ushort*) ( dstBase + y * dstStride );

                    // for each pixel
                    for ( int x = 0; x < width; x++, src += pixelSize, dst++ )
                    {
                        sum = ( src[RGB.R] + src[RGB.G] + src[RGB.B] );

                        *dst = ( sum != 0 ) ? (ushort) ( 65535 * src[channel] / sum ) : (ushort) 0;
                    }
                }
            }
        }
    }
}
