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
    /// Subtract filter - subtract pixel values of two images.
    /// </summary>
    /// 
    /// <remarks><para>The subtract filter takes two images (source and overlay images)
    /// of the same size and pixel format and produces an image, where each pixel equals
    /// to the difference value of corresponding pixels from provided images (if difference is less
    /// than minimum allowed value, 0, then it is truncated to that minimum value).</para>
    /// 
    /// <para>The filter accepts 8 and 16 bpp grayscale images and 24, 32, 48 and 64 bpp
    /// color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// Subtract filter = new Subtract( overlayImage );
    /// // apply the filter
    /// Bitmap resultImage = filter.Apply( sourceImage );
    /// </code>
    /// 
    /// <para><b>Source image:</b></para>
    /// <img src="img/imaging/sample6.png" width="320" height="240" />
    /// <para><b>Overlay image:</b></para>
    /// <img src="img/imaging/sample7.png" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/subtract.png" width="320" height="240" />
    /// </remarks>
    /// 
    /// <seealso cref="Merge"/>
    /// <seealso cref="Intersect"/>
    /// <seealso cref="Add"/>
    /// <seealso cref="Difference"/>
    /// 
    public sealed class Subtract : BaseInPlaceFilter2
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
        /// Initializes a new instance of the <see cref="Subtract"/> class.
        /// </summary>
        public Subtract( )
        {
            InitFormatTranslations( );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Subtract"/> class.
        /// </summary>
        /// 
        /// <param name="overlayImage">Overlay image</param>
        /// 
        public Subtract( Bitmap overlayImage )
            : base( overlayImage )
        {
            InitFormatTranslations( );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Subtract"/> class.
        /// </summary>
        /// 
        /// <param name="unmanagedOverlayImage">Unmanaged overlay image.</param>
        /// 
        public Subtract( UnmanagedImage unmanagedOverlayImage )
            : base( unmanagedOverlayImage )
        {
            InitFormatTranslations( );
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
                // new pixel value
                int v;

                // do the job
                byte * ptr = (byte*) image.ImageData.ToPointer( );
                byte * ovr = (byte*) overlay.ImageData.ToPointer( );

                // for each line
                for ( int y = 0; y < height; y++ )
                {
                    // for each pixel
                    for ( int x = 0; x < lineSize; x++, ptr++, ovr++ )
                    {
                        v = (int) *ptr - (int) *ovr;
                        *ptr = ( v < 0 ) ? (byte) 0 : (byte) v;
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
                // new pixel value
                int v;

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
                        v = (int) *ptr - (int) *ovr;
                        *ptr = ( v < 0 ) ? (ushort) 0 : (ushort) v;
                    }
                }
            }
        }
    }
}
