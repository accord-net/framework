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
    /// Dithering using Burkes error diffusion.
    /// </summary>
    /// 
    /// <remarks><para>The filter represents binarization filter, which is based on
    /// error diffusion dithering with Burkes coefficients. Error is diffused
    /// on 7 neighbor pixels with next coefficients:</para>
    /// <code lang="none">
    ///         | * | 8 | 4 |
    /// | 2 | 4 | 8 | 4 | 2 |
    /// 
    /// / 32
    /// </code>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// BurkesDithering filter = new BurkesDithering( );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/grayscale.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/burkes.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    /// <seealso cref="FloydSteinbergDithering"/>
    /// <seealso cref="JarvisJudiceNinkeDithering"/>
    /// <seealso cref="SierraDithering"/>
    /// <seealso cref="StuckiDithering"/>
    /// 
    public sealed class BurkesDithering : ErrorDiffusionToAdjacentNeighbors
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BurkesDithering"/> class.
        /// </summary>
        /// 
        public BurkesDithering( ) : base( new int[2][] {
            new int[2] { 8, 4 },
            new int[5] { 2, 4, 8, 4, 2 }
        } )
        {
        }
    }
}
