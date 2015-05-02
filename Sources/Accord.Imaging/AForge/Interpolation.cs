// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

namespace AForge.Imaging
{
    using System;

    /// <summary>
    /// Interpolation routines.
    /// </summary>
    /// 
    internal static class Interpolation
    {
        /// <summary>
        /// Bicubic kernel.
        /// </summary>
        /// 
        /// <param name="x">X value.</param>
        /// 
        /// <returns>Bicubic cooefficient.</returns>
        /// 
        /// <remarks><para>The function implements bicubic kernel W(x) as described on
        /// <a href="http://en.wikipedia.org/wiki/Bicubic_interpolation#Bicubic_convolution_algorithm">Wikipedia</a>
        /// (coefficient <b>a</b> is set to <b>-0.5</b>).</para></remarks>
        /// 
        public static double BiCubicKernel( double x )
        {
            if ( x < 0 )
            {
                x = -x;
            }

            double biCoef = 0;

            if ( x <= 1 )
            {
                biCoef = ( 1.5 * x - 2.5 ) * x * x + 1;
            }
            else if ( x < 2 )
            {
                biCoef = ( ( -0.5 * x + 2.5 ) * x - 4 ) * x + 2;
            }

            return biCoef;
        }
    }
}
