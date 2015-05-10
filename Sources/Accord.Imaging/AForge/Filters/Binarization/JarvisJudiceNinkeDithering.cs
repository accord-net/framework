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
    /// Dithering using Jarvis, Judice and Ninke error diffusion.
    /// </summary>
    /// 
    /// <remarks><para>The filter represents binarization filter, which is based on
    /// error diffusion dithering with Jarvis-Judice-Ninke coefficients. Error is diffused
    /// on 12 neighbor pixels with next coefficients:</para>
    /// <code lang="none">
    ///         | * | 7 | 5 |
    /// | 3 | 5 | 7 | 5 | 3 |
    /// | 1 | 3 | 5 | 3 | 1 |
    /// 
    /// / 48
    /// </code>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// JarvisJudiceNinkeDithering filter = new JarvisJudiceNinkeDithering( );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/grayscale.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/jarvis_judice_ninke.jpg" width="480" height="361" />
    /// </remarks>
    ///
    /// <seealso cref="BurkesDithering"/>
    /// <seealso cref="FloydSteinbergDithering"/>
    /// <seealso cref="SierraDithering"/>
    /// <seealso cref="StuckiDithering"/>
    /// 
    public sealed class JarvisJudiceNinkeDithering : ErrorDiffusionToAdjacentNeighbors
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JarvisJudiceNinkeDithering"/> class.
        /// </summary>
        /// 
        public JarvisJudiceNinkeDithering( ) : base( new int[3][] {
            new int[2] { 7, 5 },
            new int[5] { 3, 5, 7, 5, 3 },
            new int[5] { 1, 3, 5, 3, 1 }
        } )
        {
        }
    }
}
