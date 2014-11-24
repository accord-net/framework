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
    /// In-place filter interface.
    /// </summary>
    /// 
    /// <remarks>The interface defines the set of methods, which should be
    /// implemented by filters, which are capable to do image processing
    /// directly on the source image. Not all image processing filters
    /// can be applied directly to the source image - only filters, which do not
    /// change image's dimension and pixel format, can be applied directly to the
    /// source image.</remarks>
    /// 
    /// <seealso cref="IFilter"/>
    /// <seealso cref="IInPlacePartialFilter"/>
    /// 
    public interface IInPlaceFilter
    {
        /// <summary>
        /// Apply filter to an image.
        /// </summary>
        /// 
        /// <param name="image">Image to apply filter to.</param>
        /// 
        /// <remarks>The method applies filter directly to the provided image data.</remarks>
        /// 
        void ApplyInPlace( Bitmap image );

        /// <summary>
        /// Apply filter to an image.
        /// </summary>
        /// 
        /// <param name="imageData">Image to apply filter to.</param>
        /// 
        /// <remarks>The method applies filter directly to the provided image data.</remarks>
        /// 
        void ApplyInPlace( BitmapData imageData );

        /// <summary>
        /// Apply filter to an image in unmanaged memory.
        /// </summary>
        /// 
        /// <param name="image">Image in unmanaged memory.</param>
        /// 
        /// <remarks>The method applies filter directly to the provided image data.</remarks>
        /// 
        void ApplyInPlace( UnmanagedImage image );
    }
}

