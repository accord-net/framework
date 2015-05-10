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
    /// Base class for filters, which require source image backup to make them applicable to
    /// source image (or its part) directly.
    /// </summary>
    /// 
    /// <remarks><para>The base class is used for filters, which can not do
    /// direct manipulations with source image. To make effect of in-place filtering,
    /// these filters create a background copy of the original image (done by this
    /// base class) and then do manipulations with it putting result back to the original
    /// source image.</para>
    /// 
    /// <para><note>The background copy of the source image is created only in the case of in-place
    /// filtering. Otherwise background copy is not created - source image is processed and result is
    /// put to destination image.</note></para>
    /// 
    /// <para>The base class is for those filters, which support as filtering entire image, as
    /// partial filtering of specified rectangle only.</para>
    /// </remarks>
    ///
    public abstract class BaseUsingCopyPartialFilter : IFilter, IInPlaceFilter, IInPlacePartialFilter, IFilterInformation
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
            // check pixel format of the source image
            CheckSourceFormat( imageData.PixelFormat );

            // get width and height
            int width  = imageData.Width;
            int height = imageData.Height;

            // destination image format
            PixelFormat dstPixelFormat = FormatTranslations[imageData.PixelFormat];

            // create new image of required format
            Bitmap dstImage = ( dstPixelFormat == PixelFormat.Format8bppIndexed ) ?
                AForge.Imaging.Image.CreateGrayscaleImage( width, height ) :
                new Bitmap( width, height, dstPixelFormat );

            // lock destination bitmap data
            BitmapData dstData = dstImage.LockBits(
                new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadWrite, dstPixelFormat );

            try
            {
                // process the filter
                ProcessFilter( new UnmanagedImage( imageData ), new UnmanagedImage( dstData ), new Rectangle( 0, 0, width, height ) );
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
            UnmanagedImage dstImage = UnmanagedImage.Create( image.Width, image.Height, FormatTranslations[image.PixelFormat] );

            // process the filter
            ProcessFilter( image, dstImage, new Rectangle( 0, 0, image.Width, image.Height ) );

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
            // check pixel format of the source and destination images
            CheckSourceFormat( sourceImage.PixelFormat );

            // ensure destination image has correct format
            if ( destinationImage.PixelFormat != FormatTranslations[sourceImage.PixelFormat] )
            {
                throw new InvalidImagePropertiesException( "Destination pixel format is specified incorrectly." );
            }

            // ensure destination image has correct size
            if ( ( destinationImage.Width != sourceImage.Width ) || ( destinationImage.Height != sourceImage.Height ) )
            {
                throw new InvalidImagePropertiesException( "Destination image must have the same width and height as source image." );
            }

            // process the filter
            ProcessFilter( sourceImage, destinationImage, new Rectangle( 0, 0, sourceImage.Width, sourceImage.Height ) );
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
            // apply the filter
            ApplyInPlace( image, new Rectangle( 0, 0, image.Width, image.Height ) );
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
            // apply the filter
            ApplyInPlace( new UnmanagedImage( imageData ), new Rectangle( 0, 0, imageData.Width, imageData.Height ) );
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
            // apply the filter
            ApplyInPlace( image, new Rectangle( 0, 0, image.Width, image.Height ) );
        }

        /// <summary>
        /// Apply filter to an image or its part.
        /// </summary>
        /// 
        /// <param name="image">Image to apply filter to.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        /// <remarks>The method applies the filter directly to the provided source image.</remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        /// 
        public void ApplyInPlace( Bitmap image, Rectangle rect )
        {
            // lock source bitmap data
            BitmapData data = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadWrite, image.PixelFormat );

            try
            {
                // apply the filter
                ApplyInPlace( new UnmanagedImage( data ), rect );
            }
            finally
            {
                // unlock image
                image.UnlockBits( data );
            }
        }

        /// <summary>
        /// Apply filter to an image or its part.
        /// </summary>
        /// 
        /// <param name="imageData">Image data to apply filter to.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        /// <remarks>The method applies the filter directly to the provided source image.</remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        ///
        public void ApplyInPlace( BitmapData imageData, Rectangle rect )
        {
            // apply the filter
            ApplyInPlace( new UnmanagedImage( imageData ), rect );
        }

        /// <summary>
        /// Apply filter to an unmanaged image or its part.
        /// </summary>
        /// 
        /// <param name="image">Unmanaged image to apply filter to.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        /// <remarks>The method applies the filter directly to the provided source image.</remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        /// 
        public void ApplyInPlace( UnmanagedImage image, Rectangle rect )
        {
            // check pixel format of the source image
            CheckSourceFormat( image.PixelFormat );

            // validate rectangle
            rect.Intersect( new Rectangle( 0, 0, image.Width, image.Height ) );

            // process the filter if rectangle is not empty
            if ( ( rect.Width | rect.Height ) != 0 )
            {
                // create a copy of the source image
                int size = image.Stride * image.Height;

                IntPtr imageCopy = MemoryManager.Alloc( size );
                AForge.SystemTools.CopyUnmanagedMemory( imageCopy, image.ImageData, size );

                // process the filter
                ProcessFilter(
                    new UnmanagedImage( imageCopy, image.Width, image.Height, image.Stride, image.PixelFormat ),
                    image, rect );

                MemoryManager.Free( imageCopy );
            }
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        protected abstract unsafe void ProcessFilter( UnmanagedImage sourceData, UnmanagedImage destinationData, Rectangle rect );

        // Check pixel format of the source image
        private void CheckSourceFormat( PixelFormat pixelFormat )
        {
            if ( !FormatTranslations.ContainsKey( pixelFormat ) )
                throw new UnsupportedImageFormatException( "Source pixel format is not supported by the filter." );
        }
    }
}
