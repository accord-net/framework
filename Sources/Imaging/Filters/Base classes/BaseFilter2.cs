// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2010
// contacts@aforgenet.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Base class for filters, which operate with two images of the same size and format and
    /// produce new image as a result.
    /// </summary>
    /// 
    /// <remarks><para>The abstract class is the base class for all filters, which can
    /// be applied to an image producing new image as a result of image processing.</para>
    /// 
    /// <para>The base class is aimed for such type of filters, which require additional image
    /// to process the source image. The additional image is set by <see cref="OverlayImage"/>
    /// or <see cref="UnmanagedOverlayImage"/> property and must have the same size and pixel format
    /// as source image. See documentation of particular inherited class for information
    /// about overlay image purpose.
    /// </para>
    /// </remarks>
    /// 
    public abstract class BaseFilter2 : BaseFilter
    {
        private Bitmap overlayImage;
        private UnmanagedImage unmanagedOverlayImage;

        /// <summary>
        /// Overlay image.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>The property sets an overlay image, which will be used as the second image required
        /// to process source image. See documentation of particular inherited class for information
        /// about overlay image purpose.
        /// </para>
        /// 
        /// <para><note>Overlay image must have the same size and pixel format as source image.
        /// Otherwise exception will be generated when filter is applied to source image.</note></para>
        /// 
        /// <para><note>Setting this property will clear the <see cref="UnmanagedOverlayImage"/> property -
        /// only one overlay image is allowed: managed or unmanaged.</note></para>
        /// </remarks>
        ///
        public Bitmap OverlayImage
        {
            get { return overlayImage; }
            set
            {
                overlayImage = value;

                if ( value != null )
                    unmanagedOverlayImage = null;
            }
        }

        /// <summary>
        /// Unmanaged overlay image.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>The property sets an overlay image, which will be used as the second image required
        /// to process source image. See documentation of particular inherited class for information
        /// about overlay image purpose.
        /// </para>
        /// 
        /// <para><note>Overlay image must have the same size and pixel format as source image.
        /// Otherwise exception will be generated when filter is applied to source image.</note></para>
        /// 
        /// <para><note>Setting this property will clear the <see cref="OverlayImage"/> property -
        /// only one overlay image is allowed: managed or unmanaged.</note></para>
        /// </remarks>
        ///
        public UnmanagedImage UnmanagedOverlayImage
        {
            get { return unmanagedOverlayImage; }
            set
            {
                unmanagedOverlayImage = value;

                if ( value != null )
                    overlayImage = null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFilter2"/> class.
        /// </summary>
        /// 
        protected BaseFilter2( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFilter2"/> class.
        /// </summary>
        /// 
        /// <param name="overlayImage">Overlay image.</param>
        /// 
        protected BaseFilter2( Bitmap overlayImage )
        {
            this.overlayImage = overlayImage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFilter2"/> class.
        /// </summary>
        /// 
        /// <param name="unmanagedOverlayImage">Unmanaged overlay image.</param>
        /// 
        protected BaseFilter2( UnmanagedImage unmanagedOverlayImage )
        {
            this.unmanagedOverlayImage = unmanagedOverlayImage;
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
            PixelFormat pixelFormat = sourceData.PixelFormat;
            // get image dimension
            int width  = sourceData.Width;
            int height = sourceData.Height;

            // check overlay type
            if ( overlayImage != null )
            {
                // source image and overlay must have same pixel format
                if ( pixelFormat != overlayImage.PixelFormat )
                    throw new InvalidImagePropertiesException( "Source and overlay images must have same pixel format." );

                // check overlay image size
                if ( ( width != overlayImage.Width ) || ( height != overlayImage.Height ) )
                    throw new InvalidImagePropertiesException( "Overlay image size must be equal to source image size." );

                // lock overlay image
                BitmapData ovrData = overlayImage.LockBits(
                    new Rectangle( 0, 0, width, height ),
                    ImageLockMode.ReadOnly, pixelFormat );

                try
                {
                    ProcessFilter( sourceData, new UnmanagedImage( ovrData ), destinationData );
                }
                finally
                {
                    // unlock overlay image
                    overlayImage.UnlockBits( ovrData );
                }
            }
            else if ( unmanagedOverlayImage != null )
            {
                // source image and overlay must have same pixel format
                if ( pixelFormat != unmanagedOverlayImage.PixelFormat )
                    throw new InvalidImagePropertiesException( "Source and overlay images must have same pixel format." );

                // check overlay image size
                if ( ( width != unmanagedOverlayImage.Width ) || ( height != unmanagedOverlayImage.Height ) )
                    throw new InvalidImagePropertiesException( "Overlay image size must be equal to source image size." );

                ProcessFilter( sourceData, unmanagedOverlayImage, destinationData );
            }
            else
            {
                throw new NullReferenceException( "Overlay image is not set." );
            }
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="overlay">Overlay image data.</param>
        /// <param name="destinationData">Destination image data</param>
        /// 
        /// <remarks><para>Overlay image size and pixel format is checked by this base class, before
        /// passing execution to inherited class.</para></remarks>
        ///
        protected abstract unsafe void ProcessFilter( UnmanagedImage sourceData, UnmanagedImage overlay, UnmanagedImage destinationData );
    }
}
