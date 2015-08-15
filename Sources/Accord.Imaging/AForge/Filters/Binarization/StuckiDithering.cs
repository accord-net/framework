// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//
// Original idea from CxImage
// http://www.codeproject.com/bitmap/cximage.asp
//
namespace AForge.Imaging.Filters
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Dithering using Stucki error diffusion.
    /// </summary>
    /// 
    /// <remarks><para>The filter represents binarization filter, which is based on
    /// error diffusion dithering with Stucki coefficients. Error is diffused
    /// on 12 neighbor pixels with next coefficients:</para>
    /// <code lang="none">
    ///         | * | 8 | 4 |
    /// | 2 | 4 | 8 | 4 | 2 |
    /// | 1 | 2 | 4 | 2 | 1 |
    /// 
    /// / 42
    /// </code>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// StuckiDithering filter = new StuckiDithering( );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/grayscale.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/stucki.jpg" width="480" height="361" />
    /// </remarks>
    ///
    /// <seealso cref="BurkesDithering"/>
    /// <seealso cref="FloydSteinbergDithering"/>
    /// <seealso cref="JarvisJudiceNinkeDithering"/>
    /// <seealso cref="SierraDithering"/>
    /// 
    public sealed class StuckiDithering : ErrorDiffusionToAdjacentNeighbors
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StuckiDithering"/> class.
        /// </summary>
        /// 
        public StuckiDithering( ) : base( new int[3][] {
            new int[2] { 8, 4 },
            new int[5] { 2, 4, 8, 4, 2 },
            new int[5] { 1, 2, 4, 2, 1 }
        } )
        {
        }
    }
}
