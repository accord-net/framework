// AForge XIMEA Video Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

namespace AForge.Video.Ximea
{
    using System;

    /// <summary>
    /// Enumeration of image formats supported by XIMEA cameras.
    /// </summary>
    public enum ImageFormat : int
    {
        /// <summary>
        /// 8 bits per pixel.
        /// </summary>
        Grayscale8 = 0,

        /// <summary>
        /// 16 bits per pixel.
        /// </summary>
        Grayscale16 = 1,

        /// <summary>
        /// RGB data format.
        /// </summary>
        RGB24 = 2,

        /// <summary>
        /// RGBA data format.
        /// </summary>
        RGB32 = 3
    }
}
