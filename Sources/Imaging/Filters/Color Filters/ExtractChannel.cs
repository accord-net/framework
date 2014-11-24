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
    /// Extract RGB channel from image.
    /// </summary>
    /// 
    /// <remarks><para>Extracts specified channel of color image and returns
    /// it as grayscale image.</para>
    /// 
    /// <para>The filter accepts 24, 32, 48 and 64 bpp color images and produces
    /// 8 (if source is 24 or 32 bpp image) or 16 (if source is 48 or 64 bpp image)
    /// bpp grayscale image.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// ExtractChannel filter = new ExtractChannel( RGB.G );
    /// // apply the filter
    /// Bitmap channelImage = filter.Apply( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/extract_channel.jpg" width="480" height="361" />
    /// 
    /// </remarks>
    /// 
    /// <seealso cref="ReplaceChannel"/>
    /// 
    public class ExtractChannel : BaseFilter
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
        /// ARGB channel to extract.
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
                if (
                    ( value != RGB.R ) && ( value != RGB.G ) &&
                    ( value != RGB.B ) && ( value != RGB.A )
                    )
                {
                    throw new ArgumentException( "Invalid channel is specified." );
                }
                channel = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractChannel"/> class.
        /// </summary>
        /// 
        public ExtractChannel( )
        {
            // initialize format translation dictionary
            formatTranslations[PixelFormat.Format24bppRgb]  = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format32bppRgb]  = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format48bppRgb]  = PixelFormat.Format16bppGrayScale;
            formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format16bppGrayScale;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractChannel"/> class.
        /// </summary>
        /// 
        /// <param name="channel">ARGB channel to extract.</param>
        /// 
        public ExtractChannel( short channel ) : this( )
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
        /// <exception cref="InvalidImagePropertiesException">Can not extract alpha channel from none ARGB image. The
        /// exception is throw, when alpha channel is requested from RGB image.</exception>
        /// 
        protected override unsafe void ProcessFilter( UnmanagedImage sourceData, UnmanagedImage destinationData )
        {
            // get width and height
            int width = sourceData.Width;
            int height = sourceData.Height;

            int pixelSize = Image.GetPixelFormatSize( sourceData.PixelFormat ) / 8;

            if ( ( channel == RGB.A ) && ( pixelSize != 4 ) && ( pixelSize != 8 ) )
            {
                throw new InvalidImagePropertiesException( "Can not extract alpha channel from none ARGB image." );
            }

            if ( pixelSize <= 4 )
            {
                int srcOffset = sourceData.Stride - width * pixelSize;
                int dstOffset = destinationData.Stride - width;

                // do the job
                byte * src = (byte*) sourceData.ImageData.ToPointer( );
                byte * dst = (byte*) destinationData.ImageData.ToPointer( );

                // allign source pointer to the required channel
                src += channel;

                for ( int y = 0; y < height; y++ )
                {
                    for ( int x = 0; x < width; x++, src += pixelSize, dst++ )
                    {
                        *dst = *src;
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

                    // allign source pointer to the required channel
                    src += channel;

                    // for each pixel
                    for ( int x = 0; x < width; x++, src += pixelSize, dst++ )
                    {
                        *dst = *src;
                    }
                }

            }
        }
    }
}
