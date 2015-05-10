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
    /// Opening operator from Mathematical Morphology.
    /// </summary>
    /// 
    /// <remarks><para>Opening morphology operator equals to <see cref="Erosion">erosion</see> followed
    /// by <see cref="Dilatation">dilatation</see>.</para>
    /// 
    /// <para>Applied to binary image, the filter may be used for removing small object keeping big objects
    /// unchanged. Since erosion is used first, it removes all small objects. Then dilatation restores big
    /// objects, which were not removed by erosion.</para>
    /// 
    /// <para>See documentation to <see cref="Erosion"/> and <see cref="Dilatation"/> classes for more
    /// information and list of supported pixel formats.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// Opening filter = new Opening( );
    /// // apply the filter
    /// filter.Apply( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample12.png" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/opening.png" width="320" height="240" />
    /// </remarks>
    ///
    /// <seealso cref="Erosion"/>
    /// <seealso cref="Dilatation"/>
    /// <seealso cref="Closing"/>
    /// 
    public class Opening : IFilter, IInPlaceFilter, IInPlacePartialFilter, IFilterInformation
    {
        private Erosion     errosion = new Erosion( );
        private Dilatation  dilatation = new Dilatation( );

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return errosion.FormatTranslations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Opening"/> class.
        /// </summary>
        /// 
        /// <remarks><para>Initializes new instance of the <see cref="Opening"/> class using
        /// default structuring element for both <see cref="Erosion"/> and <see cref="Dilatation"/>
        /// classes - 3x3 structuring element with all elements equal to 1.
        /// </para></remarks>
        /// 
        public Opening( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Opening"/> class.
        /// </summary>
        /// 
        /// <param name="se">Structuring element.</param>
        /// 
        /// <remarks><para>See documentation to <see cref="Erosion"/> and <see cref="Dilatation"/>
        /// classes for information about structuring element constraints.</para></remarks>
        /// 
        public Opening( short[,] se )
        {
            errosion   = new Erosion( se );
            dilatation = new Dilatation( se );
        }

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
            Bitmap tempImage = errosion.Apply( image );
            Bitmap destImage = dilatation.Apply( tempImage );

            tempImage.Dispose( );

            return destImage;
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
        public Bitmap Apply( BitmapData imageData )
        {
            Bitmap tempImage = errosion.Apply( imageData );
            Bitmap destImage = dilatation.Apply( tempImage );

            tempImage.Dispose( );

            return destImage;
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
            UnmanagedImage destImage = errosion.Apply( image );
            dilatation.ApplyInPlace( destImage );

            return destImage;
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
            errosion.Apply( sourceImage, destinationImage );
            dilatation.ApplyInPlace( destinationImage );
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
            errosion.ApplyInPlace( image );
            dilatation.ApplyInPlace( image );
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
            errosion.ApplyInPlace( imageData );
            dilatation.ApplyInPlace( imageData );
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
            errosion.ApplyInPlace( image );
            dilatation.ApplyInPlace( image );
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
            errosion.ApplyInPlace( image, rect );
            dilatation.ApplyInPlace( image, rect );
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
            errosion.ApplyInPlace( imageData, rect );
            dilatation.ApplyInPlace( imageData, rect );
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
            errosion.ApplyInPlace( image, rect );
            dilatation.ApplyInPlace( image, rect );
        }
    }
}
