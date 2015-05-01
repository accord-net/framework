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
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge;

    /// <summary>
    /// Corners detector's interface.
    /// </summary>
    /// 
    /// <remarks><para>The interface specifies set of methods, which should be implemented by different
    /// corners detection algorithms.</para></remarks>
    /// 
    public interface ICornersDetector
    {
        /// <summary>
        /// Process image looking for corners.
        /// </summary>
        /// 
        /// <param name="image">Source image to process.</param>
        /// 
        /// <returns>Returns list of found corners (X-Y coordinates).</returns>
        /// 
        List<IntPoint> ProcessImage( Bitmap image );

        /// <summary>
        /// Process image looking for corners.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found corners (X-Y coordinates).</returns>
        /// 
        List<IntPoint> ProcessImage( BitmapData imageData );

        /// <summary>
        /// Process image looking for corners.
        /// </summary>
        /// 
        /// <param name="image">Unmanaged source image to process.</param>
        /// 
        /// <returns>Returns list of found corners (X-Y coordinates).</returns>
        /// 
        List<IntPoint> ProcessImage( UnmanagedImage image );
    }
}
