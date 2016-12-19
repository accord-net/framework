// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright � AForge.NET, 2005-2010
// contacts@aforgenet.com
//

namespace Accord.Imaging.ColorReduction
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Color dithering using Jarvis, Judice and Ninke error diffusion.
    /// </summary>
    /// 
    /// <remarks><para>The image processing routine represents color dithering algorithm, which is based on
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
    /// <para>The image processing routine accepts 24/32 bpp color images for processing. As a result this routine
    /// produces 4 bpp or 8 bpp indexed image, which depends on size of the specified
    /// <see cref="ErrorDiffusionColorDithering.ColorTable">color table</see> - 4 bpp result for
    /// color tables with 16 colors or less; 8 bpp result for larger color tables.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create color image quantization routine
    /// ColorImageQuantizer ciq = new ColorImageQuantizer( new MedianCutQuantizer( ) );
    /// // create 32 colors table
    /// Color[] colorTable = ciq.CalculatePalette( image, 32 );
    /// // create dithering routine
    /// JarvisJudiceNinkeColorDithering dithering = new JarvisJudiceNinkeColorDithering( );
    /// dithering.ColorTable = colorTable;
    /// // apply the dithering routine
    /// Bitmap newImage = dithering.Apply( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/color_jarvis_judice_ninke.png" width="480" height="361" />
    /// </remarks>
    ///
    /// <seealso cref="BurkesColorDithering"/>
    /// <seealso cref="FloydSteinbergColorDithering"/>
    /// <seealso cref="SierraColorDithering"/>
    /// <seealso cref="StuckiColorDithering"/>
    /// 
    public sealed class JarvisJudiceNinkeColorDithering : ColorErrorDiffusionToAdjacentNeighbors
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JarvisJudiceNinkeColorDithering"/> class.
        /// </summary>
        /// 
        public JarvisJudiceNinkeColorDithering( )
            : base( new int[3][] {
                new int[2] { 7, 5 },
                new int[5] { 3, 5, 7, 5, 3 },
                new int[5] { 1, 3, 5, 3, 1 } } )
        {
        }
    }
}
