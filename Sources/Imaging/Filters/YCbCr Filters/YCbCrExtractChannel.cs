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
	/// Extract YCbCr channel from image.
	/// </summary>
	/// 
	/// <remarks><para>The filter extracts specified YCbCr channel of color image and returns
    /// it in the form of grayscale image.</para>
    /// 
    /// <para>The filter accepts 24 and 32 bpp color images and produces
    /// 8 bpp grayscale images.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// YCbCrExtractChannel filter = new YCbCrExtractChannel( YCbCr.CrIndex );
    /// // apply the filter
    /// Bitmap crChannel = filter.Apply( image );
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="YCbCrReplaceChannel"/>
	/// 
    public class YCbCrExtractChannel : BaseFilter
	{
		private short channel = YCbCr.YIndex;

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
		/// YCbCr channel to extract.
		/// </summary>
        /// 
        /// <remarks><para>Default value is set to <see cref="YCbCr.YIndex"/> (Y channel).</para></remarks>
        /// 
        /// <exception cref="ArgumentException">Invalid channel was specified.</exception>
        /// 
		public short Channel
		{
			get { return channel; }
			set
			{
				if (
					( value != YCbCr.YIndex ) &&
					( value != YCbCr.CbIndex ) &&
					( value != YCbCr.CrIndex )
					)
				{
					throw new ArgumentException( "Invalid channel was specified." );
				}
				channel = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="YCbCrExtractChannel"/> class.
		/// </summary>
		/// 
		public YCbCrExtractChannel( )
        {
            // initialize format translation dictionary
            formatTranslations[PixelFormat.Format24bppRgb]  = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format32bppRgb]  = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format8bppIndexed;
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="YCbCrExtractChannel"/> class.
		/// </summary>
		/// 
		/// <param name="channel">YCbCr channel to extract.</param>
		/// 
		public YCbCrExtractChannel( short channel ) : this( )
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
            int pixelSize = Image.GetPixelFormatSize( sourceData.PixelFormat ) / 8;

			// get width and height
			int width = sourceData.Width;
			int height = sourceData.Height;

            int srcOffset = sourceData.Stride - width * pixelSize;
			int dstOffset = destinationData.Stride - width;
			RGB rgb = new RGB( );
			YCbCr ycbcr = new YCbCr( );

			// do the job
			byte * src = (byte *) sourceData.ImageData.ToPointer( );
			byte * dst = (byte *) destinationData.ImageData.ToPointer( );
			byte v = 0;

			// for each row
			for ( int y = 0; y < height; y++ )
			{
				// for each pixel
                for ( int x = 0; x < width; x++, src += pixelSize, dst++ )
				{
					rgb.Red		= src[RGB.R];
					rgb.Green	= src[RGB.G];
					rgb.Blue	= src[RGB.B];

					// convert to YCbCr
                    AForge.Imaging.YCbCr.FromRGB( rgb, ycbcr );

					switch ( channel )
					{
						case YCbCr.YIndex:
							v = (byte) ( ycbcr.Y * 255 );
							break;

						case YCbCr.CbIndex:
							v = (byte) ( ( ycbcr.Cb + 0.5 ) * 255 );
							break;

						case YCbCr.CrIndex:
							v = (byte) ( ( ycbcr.Cr + 0.5 ) * 255 );
							break;
					}

					*dst = v;
				}
				src += srcOffset;
				dst += dstOffset;
			}
		}
	}
}
