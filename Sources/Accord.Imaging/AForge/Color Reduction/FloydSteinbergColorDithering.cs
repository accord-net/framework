// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2010
// contacts@aforgenet.com
//

namespace AForge.Imaging.ColorReduction
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Color dithering using Floyd-Steinberg error diffusion.
    /// </summary>
    /// 
    /// <remarks><para>The image processing routine represents color dithering algorithm, which is based on
    /// error diffusion dithering with <a href="http://en.wikipedia.org/wiki/Floyd%E2%80%93Steinberg_dithering">Floyd-Steinberg</a>
    /// coefficients. Error is diffused on 4 neighbor pixels with the next coefficients:</para>
    /// 
    /// <code lang="none">
    ///     | * | 7 |
    /// | 3 | 5 | 1 |
    /// 
    /// / 16
    /// </code>
    /// 
    /// <para>The image processing routine accepts 24/32 bpp color images for processing. As a result this routine
    /// produces 4 bpp or 8 bpp indexed image, which depends on size of the specified
    /// <see cref="ErrorDiffusionColorDithering.ColorTable">color table</see> - 4 bpp result for
    /// color tables with 16 colors or less; 8 bpp result for larger color tables.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create color image quantization routine
    /// ColorImageQuantizer ciq = new ColorImageQuantizer( new MedianCutQuantizer( ) );
    /// // create 16 colors table
    /// Color[] colorTable = ciq.CalculatePalette( image, 16 );
    /// // create dithering routine
    /// FloydSteinbergColorDithering dithering = new FloydSteinbergColorDithering( );
    /// dithering.ColorTable = colorTable;
    /// // apply the dithering routine
    /// Bitmap newImage = dithering.Apply( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/color_floyd_steinberg.png" width="480" height="361" />
    /// </remarks>
    /// 
    /// <seealso cref="BurkesColorDithering"/>
    /// <seealso cref="JarvisJudiceNinkeColorDithering"/>
    /// <seealso cref="SierraColorDithering"/>
    /// <seealso cref="StuckiColorDithering"/>
    /// 
    public sealed class FloydSteinbergColorDithering : ColorErrorDiffusionToAdjacentNeighbors
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FloydSteinbergColorDithering"/> class.
        /// </summary>
        /// 
        public FloydSteinbergColorDithering( )
            : base( new int[2][] {
                new int[1] { 7 },
                new int[3] { 3, 5, 1 } } )
        {
        }
    }
}
