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
    /// Grayscale image using R-Y algorithm.
    /// </summary>
    /// 
    /// <remarks>The class uses <b>R-Y</b> algorithm to convert color image
    /// to grayscale. The conversion coefficients are:
    /// <list type="bullet">
    /// <item>Red: 0.5;</item>
    /// <item>Green: 0.419;</item>
    /// <item>Blue: 0.081.</item>
    /// </list>
    /// </remarks>
    /// 
    /// <seealso cref="Grayscale"/>
    /// <seealso cref="GrayscaleBT709"/>
    /// <seealso cref="GrayscaleY"/>
    ///
    [Obsolete( "Use Grayscale.CommonAlgorithms.RMY object instead" )]
    public sealed class GrayscaleRMY : Grayscale
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrayscaleRMY"/> class.
        /// </summary>
        public GrayscaleRMY( ) : base( 0.5, 0.419, 0.081 ) { }
    }
}
