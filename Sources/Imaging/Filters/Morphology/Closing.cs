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
	/// Closing operator from Mathematical Morphology.
	/// </summary>
	/// 
    /// <remarks><para>Closing morphology operator equals to <see cref="Dilatation">dilatation</see> followed
    /// by <see cref="Erosion">erosion</see>.</para>
    /// 
    /// <para>Applied to binary image, the filter may be used connect or fill objects. Since dilatation is used
    /// first, it may connect/fill object areas. Then erosion restores objects. But since dilatation may connect
    /// something before, erosion may not remove after that because of the formed connection.</para>
    /// 
    /// <para>See documentation to <see cref="Erosion"/> and <see cref="Dilatation"/> classes for more
    /// information and list of supported pixel formats.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// Closing filter = new Closing( );
    /// // apply the filter
    /// filter.Apply( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample12.png" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/closing.png" width="320" height="240" />
    /// </remarks>
    /// 
    /// <seealso cref="Erosion"/>
    /// <seealso cref="Dilatation"/>
    /// <seealso cref="Opening"/>
    /// 
    public class Closing : IFilter, IInPlaceFilter, IInPlacePartialFilter, IFilterInformation
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
		/// Initializes a new instance of the <see cref="Closing"/> class.
		/// </summary>
        /// 
        /// <remarks><para>Initializes new instance of the <see cref="Closing"/> class using
        /// default structuring element for both <see cref="Erosion"/> and <see cref="Dilatation"/>
        /// classes - 3x3 structuring element with all elements equal to 1.
        /// </para></remarks>
        /// 
        public Closing( ) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Closing"/> class.
		/// </summary>
		/// 
		/// <param name="se">Structuring element.</param>
		/// 
        /// <remarks><para>See documentation to <see cref="Erosion"/> and <see cref="Dilatation"/>
        /// classes for information about structuring element constraints.</para></remarks>
        /// 
        public Closing( short[,] se )
		{
			errosion = new Erosion( se );
			dilatation = new Dilatation(se);
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
            Bitmap tempImage = dilatation.Apply( image );
            Bitmap destImage = errosion.Apply( tempImage );

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
            Bitmap tempImage = dilatation.Apply( imageData );
            Bitmap destImage = errosion.Apply( tempImage );

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
            UnmanagedImage destImage = dilatation.Apply( image );
            errosion.ApplyInPlace( destImage );

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
            dilatation.Apply( sourceImage, destinationImage );
            errosion.ApplyInPlace( destinationImage );
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
            dilatation.ApplyInPlace( image );
            errosion.ApplyInPlace( image );
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
            dilatation.ApplyInPlace( imageData );
            errosion.ApplyInPlace( imageData );
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
            dilatation.ApplyInPlace( image );
            errosion.ApplyInPlace( image );
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
            dilatation.ApplyInPlace( image, rect );
            errosion.ApplyInPlace( image, rect );
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
            dilatation.ApplyInPlace( imageData, rect );
            errosion.ApplyInPlace( imageData, rect );
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
            dilatation.ApplyInPlace( image, rect );
            errosion.ApplyInPlace( image, rect );
        }
    }
}
