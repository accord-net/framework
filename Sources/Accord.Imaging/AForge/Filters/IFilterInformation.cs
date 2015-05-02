// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Interface which provides information about image processing filter.
    /// </summary>
    /// 
    /// <remarks><para>The interface defines set of properties, which provide different type
    /// of information about image processing filters implementing <see cref="IFilter"/> interface
    /// or another filter's interface.</para></remarks>
    /// 
    public interface IFilterInformation
    {
        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        /// 
        /// <remarks><para>The dictionary defines, which pixel formats are supported for
        /// source images and which pixel format will be used for resulting image.
        /// </para>
        /// 
        /// <para>Keys of this dictionary defines all pixel formats which are supported for source
        /// images, but corresponding values define what will be resulting pixel format. For
        /// example, if value <see cref="System.Drawing.Imaging.PixelFormat">Format16bppGrayScale</see>
        /// is put into the dictionary with the
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format48bppRgb</see> key, then it means
        /// that the filter accepts color 48 bpp image and produces 16 bpp grayscale image as a result
        /// of image processing.</para>
        /// 
        /// <para>The information provided by this property is mostly actual for filters, which can not
        /// be applied directly to the source image, but provide new image a result. Since usually all
        /// filters implement <see cref="IFilter"/> interface, the information provided by this property
        /// (if filter also implements <see cref="IFilterInformation"/> interface) may be useful to
        /// user to resolve filter's capabilities.</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // get filter's IFilterInformation interface
        /// IFilterInformation info = (IFilterInformation) filter;
        /// // check if the filter supports our image's format
        /// if ( info.FormatTranslations.ContainsKey( image.PixelFormat )
        /// {
        ///     // format is supported, check what will be result of image processing
        ///     PixelFormat resultingFormat = info.FormatTranslations[image.PixelFormat];
        /// }
        /// /// </code>
        /// </remarks>
        /// 
        Dictionary<PixelFormat, PixelFormat> FormatTranslations { get; }
    }
}
