// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// In-place partial filter interface.
    /// </summary>
    /// 
    /// <remarks><para>The interface defines the set of methods, which should be
    /// implemented by filters, which are capable to do image processing
    /// directly on the source image. Not all image processing filters
    /// can be applied directly to the source image - only filters, which do not
    /// change image dimension and pixel format, can be applied directly to the
    /// source image.</para>
    /// 
    /// <para>The interface also supports partial image filtering, allowing to specify
    /// image rectangle, which should be filtered.</para>
    /// </remarks>
    /// 
    /// <seealso cref="IFilter"/>
    /// <seealso cref="IInPlaceFilter"/>
    /// 
    public interface IInPlacePartialFilter
    {
        /// <summary>
        /// Apply filter to an image or its part.
        /// </summary>
        /// 
        /// <param name="image">Image to apply filter to.</param>
        /// <param name="rect">Image rectangle for processing by filter.</param>
        /// 
        /// <remarks>The method applies filter directly to the provided image data.</remarks>
        /// 
        void ApplyInPlace( Bitmap image, Rectangle rect );

        /// <summary>
        /// Apply filter to an image or its part.
        /// </summary>
        /// 
        /// <param name="imageData">Image to apply filter to.</param>
        /// <param name="rect">Image rectangle for processing by filter.</param>
        /// 
        /// <remarks>The method applies filter directly to the provided image data.</remarks>
        /// 
        void ApplyInPlace( BitmapData imageData, Rectangle rect );

        /// <summary>
        /// Apply filter to an image in unmanaged memory.
        /// </summary>
        /// 
        /// <param name="image">Image in unmanaged memory.</param>
        /// <param name="rect">Image rectangle for processing by filter.</param>
        /// 
        /// <remarks>The method applies filter directly to the provided image.</remarks>
        /// 
        void ApplyInPlace( UnmanagedImage image, Rectangle rect );
    }
}
