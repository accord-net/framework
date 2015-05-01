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
    /// Dithering using Sierra error diffusion.
    /// </summary>
    /// 
    /// <remarks><para>The filter represents binarization filter, which is based on
    /// error diffusion dithering with Sierra coefficients. Error is diffused
    /// on 10 neighbor pixels with next coefficients:</para>
    /// <code lang="none">
    ///         | * | 5 | 3 |
    /// | 2 | 4 | 5 | 4 | 2 |
    ///     | 2 | 3 | 2 |
    /// 
    /// / 32
    /// </code>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// SierraDithering filter = new SierraDithering( );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/grayscale.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/sierra.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    /// <seealso cref="BurkesDithering"/>
    /// <seealso cref="FloydSteinbergDithering"/>
    /// <seealso cref="JarvisJudiceNinkeDithering"/>
    /// <seealso cref="StuckiDithering"/>
    /// 
    public sealed class SierraDithering : ErrorDiffusionToAdjacentNeighbors
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SierraDithering"/> class.
        /// </summary>
        /// 
        public SierraDithering( ) : base( new int[3][] {
            new int[2] { 5, 3 },
            new int[5] { 2, 4, 5, 4, 2 },
            new int[3] { 2, 3, 2 }
        } )
        {
        }
    }
}
