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
    /// Apply mask to the specified image.
    /// </summary>
    /// 
    /// <remarks><para>The filter applies mask to the specified image - keeps all pixels
    /// in the image if corresponding pixels/values of the mask are not equal to 0. For all
    /// 0 pixels/values in mask, corresponding pixels in the source image are set to 0.</para>
    /// 
    /// <para>Mask can be specified as <see cref="MaskImage">.NET's managed Bitmap</see>, as
    /// <see cref="UnmanagedMaskImage">UnmanagedImage</see> or as <see cref="Mask">byte array</see>.
    /// In the case if mask is specified as image, it must be 8 bpp grayscale image. In all case
    /// mask size must be the same as size of the image to process.</para>
    /// 
    /// <para>The filter accepts 8/16 bpp grayscale and 24/32/48/64 bpp color images for processing.</para>
    /// </remarks>
    /// 
    public class ApplyMask : BaseInPlacePartialFilter
    {
        private Bitmap maskImage;
        private UnmanagedImage unmanagedMaskImage;
        private byte[,] mask;

        /// <summary>
        /// Mask image to apply.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies mask image to use. The image must be grayscale
        /// (8 bpp format) and have the same size as the source image to process.</para>
        /// 
        /// <para>When the property is set, both <see cref="UnmanagedMaskImage"/> and
        /// <see cref="Mask"/> properties are set to <see langword="null"/>.</para>
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">The mask image must be 8 bpp grayscale image.</exception>
        /// 
        public Bitmap MaskImage
        {
            get { return maskImage; }
            set
            {
                if ( ( maskImage != null ) && ( maskImage.PixelFormat != PixelFormat.Format8bppIndexed ) )
                {
                    throw new ArgumentException( "The mask image must be 8 bpp grayscale image." );
                }

                maskImage = value;
                unmanagedMaskImage = null;
                mask = null;
            }
        }

        /// <summary>
        /// Unmanaged mask image to apply.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies unmanaged mask image to use. The image must be grayscale
        /// (8 bpp format) and have the same size as the source image to process.</para>
        /// 
        /// <para>When the property is set, both <see cref="MaskImage"/> and
        /// <see cref="Mask"/> properties are set to <see langword="null"/>.</para>
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">The mask image must be 8 bpp grayscale image.</exception>
        /// 
        public UnmanagedImage UnmanagedMaskImage
        {
            get { return unmanagedMaskImage; }
            set
            {
                if ( ( unmanagedMaskImage != null ) && ( unmanagedMaskImage.PixelFormat != PixelFormat.Format8bppIndexed ) )
                {
                    throw new ArgumentException( "The mask image must be 8 bpp grayscale image." );
                }

                unmanagedMaskImage = value;
                maskImage = null;
                mask = null;
            }
        }

        /// <summary>
        /// Mask to apply.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies mask array to use. Size of the array must
        /// be the same size as the size of the source image to process - its 0<sup>th</sup> dimension
        /// must be equal to image's height and its 1<sup>st</sup> dimension must be equal to width. For
        /// example, for 640x480 image, the mask array must be defined as:
        /// <code>
        /// byte[,] mask = new byte[480, 640];
        /// </code>
        /// </para></remarks>
        /// 
        public byte[,] Mask
        {
            get { return mask; }
            set
            {
                mask = value;
                maskImage = null;
                unmanagedMaskImage = null;
            }
        }

        // private format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>( );

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

        private ApplyMask( )
        {
            formatTranslations[PixelFormat.Format8bppIndexed]    = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb]       = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb]      = PixelFormat.Format32bppArgb;
            formatTranslations[PixelFormat.Format32bppRgb]       = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppPArgb]     = PixelFormat.Format32bppPArgb;
            formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
            formatTranslations[PixelFormat.Format48bppRgb]       = PixelFormat.Format48bppRgb;
            formatTranslations[PixelFormat.Format64bppArgb]      = PixelFormat.Format64bppArgb;
            formatTranslations[PixelFormat.Format64bppPArgb]     = PixelFormat.Format64bppPArgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyMask"/> class.
        /// </summary>
        /// 
        /// <param name="maskImage"><see cref="MaskImage">Mask image</see> to use.</param>
        /// 
        public ApplyMask( Bitmap maskImage ) : this( )
        {
            MaskImage = maskImage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyMask"/> class.
        /// </summary>
        /// 
        /// <param name="unmanagedMaskImage"><see cref="UnmanagedMaskImage">Unmanaged mask image</see> to use.</param>
        /// 
        public ApplyMask( UnmanagedImage unmanagedMaskImage ) : this( )
        {
            UnmanagedMaskImage = unmanagedMaskImage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyMask"/> class.
        /// </summary>
        /// 
        /// <param name="mask"><see cref="Mask"/> to use.</param>
        /// 
        public ApplyMask( byte[,] mask ) : this( )
        {
            Mask = mask;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        /// <exception cref="NullReferenceException">None of the possible mask properties were set. Need to provide mask before applying the filter.</exception>
        /// <exception cref="ArgumentException">Invalid size of provided mask. Its size must be the same as the size of the image to mask.</exception>
        ///
        protected override unsafe void ProcessFilter( UnmanagedImage image, Rectangle rect )
        {
            if ( mask != null )
            {
                if ( ( image.Width  != mask.GetLength( 1 ) ) ||
                     ( image.Height != mask.GetLength( 0 ) ) )
                {
                    throw new ArgumentException( "Invalid size of mask array. Its size must be the same as the size of the image to mask." );
                }

                fixed ( byte* maskPtr = mask )
                {
                    ProcessImage( image, rect, maskPtr, mask.GetLength( 1 ) );
                }
            }
            else if ( unmanagedMaskImage != null )
            {
                if ( ( image.Width  != unmanagedMaskImage.Width ) ||
                     ( image.Height != unmanagedMaskImage.Height ) )
                {
                    throw new ArgumentException( "Invalid size of unmanaged mask image. Its size must be the same as the size of the image to mask." );
                }

                ProcessImage( image, rect, (byte*) unmanagedMaskImage.ImageData.ToPointer( ),
                              unmanagedMaskImage.Stride );
            }
            else if ( maskImage != null )
            {
                if ( ( image.Width  != maskImage.Width ) ||
                     ( image.Height != maskImage.Height ) )
                {
                    throw new ArgumentException( "Invalid size of mask image. Its size must be the same as the size of the image to mask." );
                }

                BitmapData maskData = maskImage.LockBits( new Rectangle( 0, 0, image.Width, image.Height ),
                    ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );

                try
                {
                    ProcessImage( image, rect, (byte*) maskData.Scan0.ToPointer( ),
                                  maskData.Stride );
                }
                finally
                {
                    maskImage.UnlockBits( maskData );
                }
            }
            else
            {
                throw new NullReferenceException( "None of the possible mask properties were set. Need to provide mask before applying the filter." );
            }
        }

        private unsafe void ProcessImage( UnmanagedImage image, Rectangle rect, byte* mask, int maskLineSize )
        {
            int pixelSize = Bitmap.GetPixelFormatSize( image.PixelFormat ) / 8;

            int startY  = rect.Top;
            int stopY   = startY + rect.Height;

            int startX  = rect.Left;
            int stopX   = startX + rect.Width;

            int stride = image.Stride;
            int maskOffset = maskLineSize - rect.Width;

            // allign mask to the first pixel
            mask += maskLineSize * startY + startX;

            if ( ( pixelSize <= 4 ) && ( pixelSize != 2 ) )
            {
                // 8 bits per channel
                byte* imagePtr = (byte*) image.ImageData.ToPointer( ) +
                                 stride * startY + pixelSize * startX;
                int offset = stride - rect.Width * pixelSize;

                #region 8 bit cases
                switch ( pixelSize )
                {
                    case 1:
                        // 8 bpp grayscale
                        for ( int y = startY; y < stopY; y++ )
                        {
                            for ( int x = startX; x < stopX; x++, imagePtr++, mask++ )
                            {
                                if ( *mask == 0 )
                                {
                                    *imagePtr = 0;
                                }
                            }
                            imagePtr += offset;
                            mask += maskOffset;
                        }
                        break;

                    case 3:
                        // 24 bpp color
                        for ( int y = startY; y < stopY; y++ )
                        {
                            for ( int x = startX; x < stopX; x++, imagePtr += 3, mask++ )
                            {
                                if ( *mask == 0 )
                                {
                                    imagePtr[RGB.R] = 0;
                                    imagePtr[RGB.G] = 0;
                                    imagePtr[RGB.B] = 0;
                                }
                            }
                            imagePtr += offset;
                            mask += maskOffset;
                        }
                        break;

                    case 4:
                        // 32 bpp color
                        for ( int y = startY; y < stopY; y++ )
                        {
                            for ( int x = startX; x < stopX; x++, imagePtr += 4, mask++ )
                            {
                                if ( *mask == 0 )
                                {
                                    imagePtr[RGB.R] = 0;
                                    imagePtr[RGB.G] = 0;
                                    imagePtr[RGB.B] = 0;
                                    imagePtr[RGB.A] = 0;
                                }
                            }
                            imagePtr += offset;
                            mask += maskOffset;
                        }
                        break;
                }
                #endregion
            }
            else
            {
                // 16 bits per channel
                byte* imagePtrBase = (byte*) image.ImageData.ToPointer( ) +
                                     stride * startY + pixelSize * startX;

                #region 16 bit cases
                switch ( pixelSize )
                {
                    case 2:
                        // 16 bpp grayscale
                        for ( int y = startY; y < stopY; y++ )
                        {
                            ushort* imagePtr = (ushort*) imagePtrBase;

                            for ( int x = startX; x < stopX; x++, imagePtr++, mask++ )
                            {
                                if ( *mask == 0 )
                                {
                                    *imagePtr = 0;
                                }
                            }
                            imagePtrBase += stride;
                            mask += maskOffset;
                        }
                        break;

                    case 6:
                        // 16 bpp grayscale
                        for ( int y = startY; y < stopY; y++ )
                        {
                            ushort* imagePtr = (ushort*) imagePtrBase;

                            for ( int x = startX; x < stopX; x++, imagePtr += 3, mask++ )
                            {
                                if ( *mask == 0 )
                                {
                                    imagePtr[RGB.R] = 0;
                                    imagePtr[RGB.G] = 0;
                                    imagePtr[RGB.B] = 0;
                                }
                            }
                            imagePtrBase += stride;
                            mask += maskOffset;
                        }
                        break;

                    case 8:
                        // 16 bpp grayscale
                        for ( int y = startY; y < stopY; y++ )
                        {
                            ushort* imagePtr = (ushort*) imagePtrBase;

                            for ( int x = startX; x < stopX; x++, imagePtr += 4, mask++ )
                            {
                                if ( *mask == 0 )
                                {
                                    imagePtr[RGB.R] = 0;
                                    imagePtr[RGB.G] = 0;
                                    imagePtr[RGB.B] = 0;
                                    imagePtr[RGB.A] = 0;
                                }
                            }
                            imagePtrBase += stride;
                            mask += maskOffset;
                        }
                        break;
                }
                #endregion
            }
        }
    }
}
