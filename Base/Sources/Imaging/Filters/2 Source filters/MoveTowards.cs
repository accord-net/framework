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
    /// Move towards filter.
    /// </summary>
    /// 
    /// <remarks><para>The result of this filter is an image, which is based on source image,
    /// but updated in the way to decrease diffirence with overlay image - source image is
    /// moved towards overlay image. The update equation is defined in the next way:
    /// <b>res = src + Min( Abs( ovr - src ), step ) * Sign( ovr - src )</b>.</para>
    /// 
    /// <para>The bigger is <see cref="StepSize">step size</see> value the more resulting
    /// image will look like overlay image. For example, in the case if step size is equal
    /// to 255 (or 65535 for images with 16 bits per channel), the resulting image will be
    /// equal to overlay image regardless of source image's pixel values. In the case if step
    /// size is set to 1, the resulting image will very little differ from the source image.
    /// But, in the case if the filter is applied repeatedly to the resulting image again and
    /// again, it will become equal to overlay image in maximum 255 (65535 for images with 16
    /// bits per channel) iterations.</para>
    /// 
    /// <para>The filter accepts 8 and 16 bpp grayscale images and 24, 32, 48 and 64 bpp
    /// color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// MoveTowards filter = new MoveTowards( overlayImage, 20 );
    /// // apply the filter
    /// Bitmap resultImage = filter.Apply( sourceImage );
    /// </code>
    ///
    /// <para><b>Source image:</b></para>
    /// <img src="img/imaging/sample6.png" width="320" height="240" />
    /// <para><b>Overlay image:</b></para>
    /// <img src="img/imaging/sample7.png" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/move_towards.png" width="320" height="240" />
    /// </remarks>
    /// 
    public class MoveTowards : BaseInPlaceFilter2
    {
        private int	stepSize = 1;

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
        /// Step size, [0, 65535].
        /// </summary>
        ///
        /// <remarks>
        /// <para>The property defines the maximum amount of changes per pixel in the source image.</para>
        /// 
        /// <para>Default value is set to 1.</para>
        /// </remarks>
        ///
        public int StepSize
        {
            get { return stepSize; }
            set { stepSize = Math.Max( 1, Math.Min( 65535, value ) ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveTowards"/> class
        /// </summary>
        public MoveTowards( )
        {
            InitFormatTranslations( );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveTowards"/> class.
        /// </summary>
        /// 
        /// <param name="overlayImage">Overlay image.</param>
        /// 
        public MoveTowards( Bitmap overlayImage )
            : base( overlayImage )
        {
            InitFormatTranslations( );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveTowards"/> class.
        /// </summary>
        /// 
        /// <param name="overlayImage">Overlay image.</param>
        /// <param name="stepSize">Step size.</param>
        /// 
        public MoveTowards( Bitmap overlayImage, int stepSize )
            : base( overlayImage )
        {
            InitFormatTranslations( );
            StepSize = stepSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveTowards"/> class.
        /// </summary>
        /// 
        /// <param name="unmanagedOverlayImage">Unmanaged overlay image.</param>
        /// 
        public MoveTowards( UnmanagedImage unmanagedOverlayImage )
            : base( unmanagedOverlayImage )
        {
            InitFormatTranslations( );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveTowards"/> class.
        /// </summary>
        /// 
        /// <param name="unmanagedOverlayImage">Unmanaged overlay image.</param>
        /// <param name="stepSize">Step size.</param>
        /// 
        public MoveTowards( UnmanagedImage unmanagedOverlayImage, int stepSize )
            : base( unmanagedOverlayImage )
        {
            InitFormatTranslations( );
            StepSize = stepSize;
        }

        // Initialize format translation dictionary
        private void InitFormatTranslations( )
        {
            formatTranslations[PixelFormat.Format8bppIndexed]    = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]       = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb]       = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb]      = PixelFormat.Format32bppArgb;
            formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
            formatTranslations[PixelFormat.Format48bppRgb]       = PixelFormat.Format48bppRgb;
            formatTranslations[PixelFormat.Format64bppArgb]      = PixelFormat.Format64bppArgb;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        /// <param name="overlay">Overlay image data.</param>
        ///
        protected override unsafe void ProcessFilter( UnmanagedImage image, UnmanagedImage overlay )
        {
            PixelFormat pixelFormat = image.PixelFormat;
            // get image dimension
            int width  = image.Width;
            int height = image.Height;
            // pixel value
            int v;

            if (
                ( pixelFormat == PixelFormat.Format8bppIndexed ) ||
                ( pixelFormat == PixelFormat.Format24bppRgb ) ||
                ( pixelFormat == PixelFormat.Format32bppRgb ) ||
                ( pixelFormat == PixelFormat.Format32bppArgb ) )
            {
                // initialize other variables
                int pixelSize = ( pixelFormat == PixelFormat.Format8bppIndexed ) ? 1 :
                    ( pixelFormat == PixelFormat.Format24bppRgb ) ? 3 : 4;
                int lineSize  = width * pixelSize;
                int srcOffset = image.Stride - lineSize;
                int ovrOffset = overlay.Stride - lineSize;

                // do the job
                byte * ptr = (byte*) image.ImageData.ToPointer( );
                byte * ovr = (byte*) overlay.ImageData.ToPointer( );

                // for each line
                for ( int y = 0; y < height; y++ )
                {
                    // for each pixel
                    for ( int x = 0; x < lineSize; x++, ptr++, ovr++ )
                    {
                        v = (int) *ovr - *ptr;
                        if ( v > 0 )
                        {
                            *ptr += (byte) ( ( stepSize < v ) ? stepSize : v );
                        }
                        else if ( v < 0 )
                        {
                            v = -v;
                            *ptr -= (byte) ( ( stepSize < v ) ? stepSize : v );
                        }
                    }
                    ptr += srcOffset;
                    ovr += ovrOffset;
                }
            }
            else
            {
                // initialize other variables
                int pixelSize = ( pixelFormat == PixelFormat.Format16bppGrayScale ) ? 1 :
                    ( pixelFormat == PixelFormat.Format48bppRgb ) ? 3 : 4;
                int lineSize  = width * pixelSize;
                int srcStride = image.Stride;
                int ovrStride = overlay.Stride;

                // do the job
                byte* basePtr = (byte*) image.ImageData.ToPointer( );
                byte* baseOvr = (byte*) overlay.ImageData.ToPointer( );

                // for each line
                for ( int y = 0; y < height; y++ )
                {
                    ushort * ptr = (ushort*) ( basePtr + y * srcStride );
                    ushort * ovr = (ushort*) ( baseOvr + y * ovrStride );

                    // for each pixel
                    for ( int x = 0; x < lineSize; x++, ptr++, ovr++ )
                    {
                        v = (int) *ovr - *ptr;
                        if ( v > 0 )
                        {
                            *ptr += (ushort) ( ( stepSize < v ) ? stepSize : v );
                        }
                        else if ( v < 0 )
                        {
                            v = -v;
                            *ptr -= (ushort) ( ( stepSize < v ) ? stepSize : v );
                        }
                    }
                }
            }
        }
    }
}
