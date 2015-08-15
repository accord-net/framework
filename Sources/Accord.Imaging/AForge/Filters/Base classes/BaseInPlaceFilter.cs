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
    /// Base class for filters, which may be applied directly to the source image.
    /// </summary>
    /// 
    /// <remarks><para>The abstract class is the base class for all filters, which can
    /// be applied to an image producing new image as a result of image processing or
    /// applied directly to the source image without changing its size and pixel format.</para>
    /// </remarks>
    /// 
    public abstract class BaseInPlaceFilter : IFilter, IInPlaceFilter, IFilterInformation
    {
        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        /// 
        /// <remarks><para>The dictionary defines, which pixel formats are supported for
        /// source images and which pixel format will be used for resulting image.
        /// </para>
        /// 
        /// <para>See <see cref="IFilterInformation.FormatTranslations"/> for more information.</para>
        /// </remarks>
        ///
        public abstract Dictionary<PixelFormat, PixelFormat> FormatTranslations { get; }

        /// <summary>
        /// Apply filter to an image.
        /// </summary>
        /// 
        /// <param name="image">Source image to apply filter to.</param>
        /// 
        /// <returns>Returns filter's result obtained by applying the filter to
        /// the source image.</returns>
        /// 
        /// <remarks>The method keeps the source image unchanged and returns
        /// the result of image processing filter as new image.</remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        ///
        public Bitmap Apply( Bitmap image )
        {
            // lock source bitmap data
            BitmapData srcData = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, image.PixelFormat );

            Bitmap dstImage = null;

            try
            {
                // apply the filter
                dstImage = Apply( srcData );
                if ( ( image.HorizontalResolution > 0 ) && ( image.VerticalResolution > 0 ) )
                {
                    dstImage.SetResolution( image.HorizontalResolution, image.VerticalResolution );
                }
            }
            finally
            {
                // unlock source image
                image.UnlockBits( srcData );
            }

            return dstImage;
        }

        /// <summary>
        /// Apply filter to an image.
        /// </summary>
        /// 
        /// <param name="imageData">Source image to apply filter to.</param>
        /// 
        /// <returns>Returns filter's result obtained by applying the filter to
        /// the source image.</returns>
        /// 
        /// <remarks>The filter accepts bitmap data as input and returns the result
        /// of image processing filter as new image. The source image data are kept
        /// unchanged.</remarks>
        ///
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        ///
        public Bitmap Apply( BitmapData imageData )
        {
            // destination image format
            PixelFormat dstPixelFormat = imageData.PixelFormat;

            // check pixel format of the source image
            CheckSourceFormat( dstPixelFormat );

            // get image dimension
            int width  = imageData.Width;
            int height = imageData.Height;

            // create new image of required format
            Bitmap dstImage = ( dstPixelFormat == PixelFormat.Format8bppIndexed ) ?
                AForge.Imaging.Image.CreateGrayscaleImage( width, height ) :
                new Bitmap( width, height, dstPixelFormat );

            // lock destination bitmap data
            BitmapData dstData = dstImage.LockBits(
                new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadWrite, dstPixelFormat );

            // copy image
            AForge.SystemTools.CopyUnmanagedMemory( dstData.Scan0, imageData.Scan0, imageData.Stride * height );

            try
            {
                // process the filter
                ProcessFilter( new UnmanagedImage( dstData ) );
            }
            finally
            {
                // unlock destination images
                dstImage.UnlockBits( dstData );
            }

            return dstImage;
        }

        /// <summary>
        /// Apply filter to an image in unmanaged memory.
        /// </summary>
        /// 
        /// <param name="image">Source image in unmanaged memory to apply filter to.</param>
        /// 
        /// <returns>Returns filter's result obtained by applying the filter to
        /// the source image.</returns>
        /// 
        /// <remarks>The method keeps the source image unchanged and returns
        /// the result of image processing filter as new image.</remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        ///
        public UnmanagedImage Apply( UnmanagedImage image )
        {
            // check pixel format of the source image
            CheckSourceFormat( image.PixelFormat );

            // create new destination image
            UnmanagedImage dstImage = UnmanagedImage.Create( image.Width, image.Height, image.PixelFormat );

            Apply( image, dstImage );

            return dstImage;
        }

        /// <summary>
        /// Apply filter to an image in unmanaged memory.
        /// </summary>
        /// 
        /// <param name="sourceImage">Source image in unmanaged memory to apply filter to.</param>
        /// <param name="destinationImage">Destination image in unmanaged memory to put result into.</param>
        /// 
        /// <remarks><para>The method keeps the source image unchanged and puts result of image processing
        /// into destination image.</para>
        /// 
        /// <para><note>The destination image must have the same width and height as source image. Also
        /// destination image must have pixel format, which is expected by particular filter (see
        /// <see cref="FormatTranslations"/> property for information about pixel format conversions).</note></para>
        /// </remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        /// <exception cref="InvalidImagePropertiesException">Incorrect destination pixel format.</exception>
        /// <exception cref="InvalidImagePropertiesException">Destination image has wrong width and/or height.</exception>
        ///
        public void Apply( UnmanagedImage sourceImage, UnmanagedImage destinationImage )
        {
            // check pixel format of the source image
            CheckSourceFormat( sourceImage.PixelFormat );

            // ensure destination image has correct format
            if ( destinationImage.PixelFormat != sourceImage.PixelFormat )
            {
                throw new InvalidImagePropertiesException( "Destination pixel format must be the same as pixel format of source image." );
            }

            // ensure destination image has correct size
            if ( ( destinationImage.Width != sourceImage.Width ) || ( destinationImage.Height != sourceImage.Height ) )
            {
                throw new InvalidImagePropertiesException( "Destination image must have the same width and height as source image." );
            }

            // usually stride will be the same for 2 images of the size size/format,
            // but since this a public a method and users may provide any evil, we need to check it
            int dstStride = destinationImage.Stride;
            int srcStride = sourceImage.Stride;
            int lineSize  = Math.Min( srcStride, dstStride );

            unsafe
            {
                byte* dst = (byte*) destinationImage.ImageData.ToPointer( );
                byte* src = (byte*) sourceImage.ImageData.ToPointer( );

                // copy image
                for ( int y = 0, height = sourceImage.Height; y < height; y++ )
                {
                    AForge.SystemTools.CopyUnmanagedMemory( dst, src, lineSize );
                    dst += dstStride;
                    src += srcStride;
                }
            }

            // process the filter
            ProcessFilter( destinationImage );
        }

        /// <summary>
        /// Apply filter to an image.
        /// </summary>
        /// 
        /// <param name="image">Image to apply filter to.</param>
        /// 
        /// <remarks>The method applies the filter directly to the provided source image.</remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        /// 
        public void ApplyInPlace( Bitmap image )
        {
            // check pixel format of the source image
            CheckSourceFormat( image.PixelFormat );

            // lock source bitmap data
            BitmapData data = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadWrite, image.PixelFormat );

            try
            {
                // process the filter
                ProcessFilter( new UnmanagedImage( data ) );
            }
            finally
            {
                // unlock image
                image.UnlockBits( data );
            }
        }

        /// <summary>
        /// Apply filter to an image.
        /// </summary>
        /// 
        /// <param name="imageData">Image data to apply filter to.</param>
        /// 
        /// <remarks>The method applies the filter directly to the provided source image.</remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        ///
        public void ApplyInPlace( BitmapData imageData )
        {
            // check pixel format of the source image
            CheckSourceFormat( imageData.PixelFormat );

            // process the filter
            ProcessFilter( new UnmanagedImage( imageData ) );
        }

        /// <summary>
        /// Apply filter to an unmanaged image.
        /// </summary>
        /// 
        /// <param name="image">Unmanaged image to apply filter to.</param>
        /// 
        /// <remarks>The method applies the filter directly to the provided source unmanaged image.</remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        ///
        public void ApplyInPlace( UnmanagedImage image )
        {
            // check pixel format of the source image
            CheckSourceFormat( image.PixelFormat );

            // process the filter
            ProcessFilter( image );
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        ///
        protected abstract unsafe void ProcessFilter( UnmanagedImage image );

        // Check pixel format of the source image
        private void CheckSourceFormat( PixelFormat pixelFormat )
        {
            if ( !FormatTranslations.ContainsKey( pixelFormat ) )
                throw new UnsupportedImageFormatException( "Source pixel format is not supported by the filter." );
        }
    }
}
