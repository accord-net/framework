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
    /// Dithering using Floyd-Steinberg error diffusion.
    /// </summary>
    /// 
    /// <remarks><para>The filter represents binarization filter, which is based on
    /// error diffusion dithering with <a href="http://en.wikipedia.org/wiki/Floyd%E2%80%93Steinberg_dithering">Floyd-Steinberg</a>
    /// coefficients. Error is diffused on 4 neighbor pixels with next coefficients:</para>
    /// 
    /// <code lang="none">
    ///     | * | 7 |
    /// | 3 | 5 | 1 |
    /// 
    /// / 16
    /// </code>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// FloydSteinbergDithering filter = new FloydSteinbergDithering( );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/grayscale.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/floyd_steinberg.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    /// <seealso cref="BurkesDithering"/>
    /// <seealso cref="JarvisJudiceNinkeDithering"/>
    /// <seealso cref="SierraDithering"/>
    /// <seealso cref="StuckiDithering"/>
    /// 
    public sealed class FloydSteinbergDithering : ErrorDiffusionToAdjacentNeighbors
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FloydSteinbergDithering"/> class.
        /// </summary>
        /// 
        public FloydSteinbergDithering( )
            : base( new int[2][] {
            new int[1] { 7 },
            new int[3] { 3, 5, 1 }
        } )
        {
        }
    }
}
