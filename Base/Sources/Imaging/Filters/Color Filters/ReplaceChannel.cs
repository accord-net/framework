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
    /// Replace RGB channel of color imgae.
    /// </summary>
    /// 
    /// <remarks><para>Replaces specified RGB channel of color image with
    /// specified grayscale image.</para>
    /// 
    /// <para>The filter is quite useful in conjunction with <see cref="ExtractChannel"/> filter
    /// (however may be used alone in some cases). Using the <see cref="ExtractChannel"/> filter
    /// it is possible to extract one of RGB channel, perform some image processing with it and then
    /// put it back into the original color image.</para>
    /// 
    /// <para>The filter accepts 24, 32, 48 and 64 bpp color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // extract red channel
    /// ExtractChannel extractFilter = new ExtractChannel( RGB.R );
    /// Bitmap channel = extractFilter.Apply( image );
    /// // threshold channel
    /// Threshold thresholdFilter = new Threshold( 230 );
    /// thresholdFilter.ApplyInPlace( channel );            
    /// // put the channel back
    /// ReplaceChannel replaceFilter = new ReplaceChannel( RGB.R, channel );
    /// replaceFilter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/replace_channel.jpg" width="480" height="361" />
    /// 
    /// </remarks>
    /// 
    /// <seealso cref="ExtractChannel"/>
    /// 
    public class ReplaceChannel : BaseInPlacePartialFilter
    {
        private short channel = RGB.R;
        private Bitmap channelImage;
        private UnmanagedImage unmanagedChannelImage;

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
        /// ARGB channel to replace.
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
        /// Grayscale image to use for channel replacement.
        /// </summary>
        /// 
        /// <remarks>
        /// <para><note>Setting this property will clear the <see cref="UnmanagedChannelImage"/> property -
        /// only one channel image is allowed: managed or unmanaged.</note></para>
        /// </remarks>
        /// 
        /// <exception cref="InvalidImagePropertiesException">Channel image should be 8 bpp indexed or 16 bpp grayscale image.</exception>
        ///
        public Bitmap ChannelImage
        {
            get { return channelImage; }
            set
            {
                if ( value != null )
                {
                    // check for valid format
                    if ( ( value.PixelFormat != PixelFormat.Format8bppIndexed ) &&
                         ( value.PixelFormat != PixelFormat.Format16bppGrayScale ) )
                        throw new InvalidImagePropertiesException( "Channel image should be 8 bpp indexed or 16 bpp grayscale image." );
                }

                channelImage = value;
                unmanagedChannelImage = null;
            }
        }

        /// <summary>
        /// Unmanaged grayscale image to use for channel replacement.
        /// </summary>
        /// 
        /// <remarks>
        /// <para><note>Setting this property will clear the <see cref="ChannelImage"/> property -
        /// only one channel image is allowed: managed or unmanaged.</note></para>
        /// </remarks>
        /// 
        /// <exception cref="InvalidImagePropertiesException">Channel image should be 8 bpp indexed or 16 bpp grayscale image.</exception>
        /// 
        public UnmanagedImage UnmanagedChannelImage
        {
            get { return unmanagedChannelImage; }
            set
            {
                if ( value != null )
                {
                    // check for valid format
                    if ( ( value.PixelFormat != PixelFormat.Format8bppIndexed ) &&
                         ( value.PixelFormat != PixelFormat.Format16bppGrayScale ) )
                        throw new InvalidImagePropertiesException( "Channel image should be 8 bpp indexed or 16 bpp grayscale image." );
                }

                channelImage = null;
                unmanagedChannelImage = value;
            }
        }

        // private constructor
        private ReplaceChannel( )
        {
            // initialize format translation dictionary
            formatTranslations[PixelFormat.Format24bppRgb]  = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb]  = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
            formatTranslations[PixelFormat.Format48bppRgb]  = PixelFormat.Format16bppGrayScale;
            formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format16bppGrayScale;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ReplaceChannel"/> class.
        /// </summary>
        /// 
        /// <param name="channel">ARGB channel to replace.</param>
        /// 
        public ReplaceChannel( short channel ) : this( )
        {
            this.Channel = channel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplaceChannel"/> class.
        /// </summary>
        /// 
        /// <param name="channel">ARGB channel to replace.</param>
        /// <param name="channelImage">Channel image to use for replacement.</param>
        /// 
        public ReplaceChannel( short channel, Bitmap channelImage ) : this( )
        {
            this.Channel = channel;
            this.ChannelImage = channelImage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YCbCrReplaceChannel"/> class.
        /// </summary>
        /// 
        /// <param name="channel">RGB channel to replace.</param>
        /// <param name="channelImage">Unmanaged channel image to use for replacement.</param>
        /// 
        public ReplaceChannel( short channel, UnmanagedImage channelImage )
            : this( )
        {
            this.Channel = channel;
            this.UnmanagedChannelImage = channelImage;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        /// <exception cref="NullReferenceException">Channel image was not specified.</exception>
        /// <exception cref="InvalidImagePropertiesException">Channel image size does not match source
        /// image size.</exception>
        /// <exception cref="InvalidImagePropertiesException">Channel image's format does not correspond to format of the source image.</exception>
        ///
        /// <exception cref="InvalidImagePropertiesException">Can not replace alpha channel of none ARGB image. The
        /// exception is throw, when alpha channel is requested to be replaced in RGB image.</exception>
        /// 
        protected override unsafe void ProcessFilter( UnmanagedImage image, Rectangle rect )
        {
            if ( ( channelImage == null ) && ( unmanagedChannelImage == null ) )
            {
                throw new NullReferenceException( "Channel image was not specified." );
            }

            int pixelSize = Image.GetPixelFormatSize( image.PixelFormat ) / 8;

            if ( ( channel == RGB.A ) && ( pixelSize != 4 ) && ( pixelSize != 8 ) )
            {
                throw new InvalidImagePropertiesException( "Can not replace alpha channel of none ARGB image." );
            }

            int width   = image.Width;
            int height  = image.Height;
            int startX  = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;
            int offset  = image.Stride - rect.Width * pixelSize;

            BitmapData chData = null;
            // pointer to channel's data
            byte* ch;
            // channel's image stride
            int chStride = 0;
            PixelFormat chFormat = PixelFormat.Format16bppGrayScale;

            // check channel's image type
            if ( channelImage != null )
            {
                // check channel's image dimension
                if ( ( width != channelImage.Width ) || ( height != channelImage.Height ) )
                    throw new InvalidImagePropertiesException( "Channel image size does not match source image size." );

                // lock channel image
                chData = channelImage.LockBits(
                    new Rectangle( 0, 0, width, height ),
                    ImageLockMode.ReadOnly, channelImage.PixelFormat );

                ch = (byte*) chData.Scan0.ToPointer( );
                chStride = chData.Stride;
                chFormat = chData.PixelFormat;
            }
            else
            {
                // check channel's image dimension
                if ( ( width != unmanagedChannelImage.Width ) || ( height != unmanagedChannelImage.Height ) )
                    throw new InvalidImagePropertiesException( "Channel image size does not match source image size." );

                ch = (byte*) unmanagedChannelImage.ImageData;
                chStride = unmanagedChannelImage.Stride;
                chFormat = unmanagedChannelImage.PixelFormat;
            }

            if ( pixelSize <= 4 )
            {
                // check channel image's format
                if ( chFormat != PixelFormat.Format8bppIndexed )
                    throw new InvalidImagePropertiesException( "Channel image's format does not correspond to format of the source image." );

                int offsetCh = chData.Stride - rect.Width;

                // do the job
                byte* dst = (byte*) image.ImageData.ToPointer( );

                // allign pointers to the first pixel to process
                dst += ( startY * image.Stride + startX * pixelSize );
                ch  += ( startY * chStride + startX );

                // for each line
                for ( int y = startY; y < stopY; y++ )
                {
                    // for each pixel
                    for ( int x = startX; x < stopX; x++, dst += pixelSize, ch++ )
                    {
                        dst[channel] = *ch;
                    }
                    dst += offset;
                    ch += offsetCh;
                }
            }
            else
            {
                // check channel image's format
                if ( chFormat != PixelFormat.Format16bppGrayScale )
                    throw new InvalidImagePropertiesException( "Channel image's format does not correspond to format of the source image." );

                int stride = image.Stride;

                // do the job
                byte* baseDst = (byte*) image.ImageData.ToPointer( );
                // allign pointers for X coordinate
                baseDst += startX * pixelSize;
                ch += startX * 2;

                pixelSize /= 2;

                // for each line
                for ( int y = startY; y < stopY; y++ )
                {
                    ushort* dst = (ushort*) ( baseDst + y * stride );
                    ushort* chPtr = (ushort*) ( ch + y * chStride );

                    // for each pixel
                    for ( int x = startX; x < stopX; x++, dst += pixelSize, chPtr++ )
                    {
                        dst[channel] = *chPtr;
                    }
                }
            }

            if ( chData != null )
            {
                // unlock
                channelImage.UnlockBits( chData );
            }
        }
    }
}
