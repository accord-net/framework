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
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Image processing filter interface.
    /// </summary>
    /// 
    /// <remarks>The interface defines the set of methods, which should be
    /// provided by all image processing filters. Methods of this interface
    /// keep the source image unchanged and returt the result of image processing
    /// filter as new image.</remarks>
    /// 
    /// <seealso cref="IInPlaceFilter"/>
    /// <seealso cref="IInPlacePartialFilter"/>
    /// 
    public interface IFilter
    {
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
        Bitmap Apply( Bitmap image );

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
        Bitmap Apply( BitmapData imageData );

        /// <summary>
        /// Apply filter to an image.
        /// </summary>
        /// 
        /// <param name="image">Image in unmanaged memory.</param>
        /// 
        /// <returns>Returns filter's result obtained by applying the filter to
        /// the source image.</returns>
        /// 
        /// <remarks>The method keeps the source image unchanged and returns
        /// the result of image processing filter as new image.</remarks> 
        /// 
        UnmanagedImage Apply( UnmanagedImage image );

        /// <summary>
        /// Apply filter to an image.
        /// </summary>
        /// 
        /// <param name="sourceImage">Source image to be processed.</param>
        /// <param name="destinationImage">Destination image to store filter's result.</param>
        /// 
        /// <remarks><para>The method keeps the source image unchanged and puts the
        /// the result of image processing filter into destination image.</para>
        /// 
        /// <para><note>The destination image must have the size, which is expected by
        /// the filter.</note></para>
        /// </remarks>
        /// 
        /// <exception cref="InvalidImagePropertiesException">In the case if destination image has incorrect
        /// size.</exception>
        /// 
        void Apply( UnmanagedImage sourceImage, UnmanagedImage destinationImage );
    }
}
