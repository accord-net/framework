// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Template matching algorithm's interface.
    /// </summary>
    /// 
    /// <remarks><para>The interface specifies set of methods, which should be implemented by different
    /// template matching algorithms - algorithms, which search for the given template in specified
    /// image.</para></remarks>
    /// 
    public interface ITemplateMatching
    {
        /// <summary>
        /// Process image looking for matchings with specified template.
        /// </summary>
        /// 
        /// <param name="image">Source image to process.</param>
        /// <param name="template">Template image to search for.</param>
        /// <param name="searchZone">Rectangle in source image to search template for.</param>
        /// 
        /// <returns>Returns array of found matchings.</returns>
        /// 
        TemplateMatch[] ProcessImage( Bitmap image, Bitmap template, Rectangle searchZone );

        /// <summary>
        /// Process image looking for matchings with specified template.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to process.</param>
        /// <param name="templateData">Template image to search for.</param>
        /// <param name="searchZone">Rectangle in source image to search template for.</param>
        /// 
        /// <returns>Returns array of found matchings.</returns>
        /// 
        TemplateMatch[] ProcessImage( BitmapData imageData, BitmapData templateData, Rectangle searchZone );

        /// <summary>
        /// Process image looking for matchings with specified template.
        /// </summary>
        /// 
        /// <param name="image">Unmanaged source image to process.</param>
        /// <param name="template">Unmanaged template image to search for.</param>
        /// <param name="searchZone">Rectangle in source image to search template for.</param>
        /// 
        /// <returns>Returns array of found matchings.</returns>
        /// 
        TemplateMatch[] ProcessImage( UnmanagedImage image, UnmanagedImage template, Rectangle searchZone );
    }
}
