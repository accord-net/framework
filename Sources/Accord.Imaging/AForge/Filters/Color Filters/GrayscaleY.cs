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
    /// Grayscale image using Y algorithm.
    /// </summary>
    /// 
    /// <remarks>The class uses <b>Y</b> algorithm to convert color image
    /// to grayscale. The conversion coefficients are:
    /// <list type="bullet">
    /// <item>Red: 0.299;</item>
    /// <item>Green: 0.587;</item>
    /// <item>Blue: 0.114.</item>
    /// </list>
    /// </remarks>
    /// 
    /// <seealso cref="Grayscale"/>
    /// <seealso cref="GrayscaleBT709"/>
    /// <seealso cref="GrayscaleRMY"/>
    ///
    [Obsolete( "Use Grayscale.CommonAlgorithms.Y object instead" )]
    public sealed class GrayscaleY : Grayscale
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrayscaleY"/> class.
        /// </summary>
        public GrayscaleY( ) : base( 0.299, 0.587, 0.114 ) { }
    }
}
