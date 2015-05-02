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
    /// Base class for filters, which produce new image of the same size as a
    /// result of image processing.
    /// </summary>
    /// 
    /// <remarks><para>The abstract class is the base class for all filters, which
    /// do image processing creating new image with the same size as source.
    /// Filters based on this class cannot be applied directly to the source
    /// image, which is kept unchanged.</para>
    /// 
    /// <para>The base class itself does not define supported pixel formats of source
    /// image and resulting pixel formats of destination image. Filters inheriting from
    /// this base class, should specify supported pixel formats and their transformations
    /// overriding abstract <see cref="FormatTranslations"/> property.</para>
    /// </remarks>
    /// 
    public abstract class BaseFilter : IFilter, IFilterInformation
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
                ProcessFilter( new UnmanagedImage( imageData ), new UnmanagedImage( dstData ) );
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
            ProcessFilter( image, dstImage );

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
            ProcessFilter( sourceImage, destinationImage );
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        /// 
        protected abstract unsafe void ProcessFilter( UnmanagedImage sourceData, UnmanagedImage destinationData );

        // Check pixel format of the source image
        private void CheckSourceFormat( PixelFormat pixelFormat )
        {
            if ( !FormatTranslations.ContainsKey( pixelFormat ) )
                throw new UnsupportedImageFormatException( "Source pixel format is not supported by the filter." );
        }
    }
}
