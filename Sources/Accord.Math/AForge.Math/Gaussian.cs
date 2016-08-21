// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge.Math
{
    using Accord.Math;
    using System;

    /// <summary>
    /// Gaussian function.
    /// </summary>
    /// 
    /// <remarks><para>The class is used to calculate 1D and 2D Gaussian functions for
    /// specified <see cref="Sigma"/> (s) value:</para>
    /// 
    /// <code lang="none">
    /// 1-D: f(x) = exp( x * x / ( -2 * s * s ) ) / ( s * sqrt( 2 * PI ) )
    /// 
    /// 2-D: f(x, y) = exp( x * x + y * y / ( -2 * s * s ) ) / ( s * s * 2 * PI )
    /// </code>
    /// 
    /// </remarks>
    /// 
    [Obsolete("Please use Accord.Math.Normal instead.")]
    public class Gaussian
    {
        // sigma value
        private double sigma = 1.0;
        // squared sigma
        private double sqrSigma = 1.0;

        /// <summary>
        /// Sigma value.
        /// </summary>
        /// 
        /// <remarks><para>Sigma property of Gaussian function.</para>
        /// 
        /// <para>Default value is set to <b>1</b>. Minimum allowed value is <b>0.00000001</b>.</para>
        /// </remarks>
        /// 
        public double Sigma
        {
            get { return sigma; }
            set
            {
                sigma = Math.Max(0.00000001, value);
                sqrSigma = sigma * sigma;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gaussian"/> class.
        /// </summary>
        /// 
        public Gaussian() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gaussian"/> class.
        /// </summary>
        /// 
        /// <param name="sigma">Sigma value.</param>
        /// 
        public Gaussian(double sigma)
        {
            Sigma = sigma;
        }

        /// <summary>
        /// 1-D Gaussian function.
        /// </summary>
        /// 
        /// <param name="x">x value.</param>
        /// 
        /// <returns>Returns function's value at point <paramref name="x"/>.</returns>
        /// 
        /// <remarks><para>The function calculates 1-D Gaussian function:</para>
        /// 
        /// <code lang="none">
        /// f(x) = exp( x * x / ( -2 * s * s ) ) / ( s * sqrt( 2 * PI ) )
        /// </code>
        /// </remarks>
        /// 
        [Obsolete("Please use Normal.Gaussian instead.")]
        public double Function(double x)
        {
            return Normal.Gaussian(sqrSigma, x);
        }

        /// <summary>
        /// 2-D Gaussian function.
        /// </summary>
        /// 
        /// <param name="x">x value.</param>
        /// <param name="y">y value.</param>
        /// 
        /// <returns>Returns function's value at point (<paramref name="x"/>, <paramref name="y"/>).</returns>
        /// 
        /// <remarks><para>The function calculates 2-D Gaussian function:</para>
        /// 
        /// <code lang="none">
        /// f(x, y) = exp( x * x + y * y / ( -2 * s * s ) ) / ( s * s * 2 * PI )
        /// </code>
        /// </remarks>
        /// 
        [Obsolete("Please use Normal.Gaussian2D instead.")]
        public double Function2D(double x, double y)
        {
            return Normal.Gaussian2D(sqrSigma, x, y);
        }

        /// <summary>
        /// 1-D Gaussian kernel.
        /// </summary>
        /// 
        /// <param name="size">Kernel size (should be odd), [3, 101].</param>
        /// 
        /// <returns>Returns 1-D Gaussian kernel of the specified size.</returns>
        /// 
        /// <remarks><para>The function calculates 1-D Gaussian kernel, which is array
        /// of Gaussian function's values in the [-r, r] range of x value, where
        /// r=floor(<paramref name="size"/>/2).
        /// </para></remarks>
        /// 
        /// <exception cref="ArgumentException">Wrong kernel size.</exception>
        /// 
        [Obsolete("Please use Normal.Kernel instead.")]
        public double[] Kernel(int size)
        {
            return Normal.Kernel(sqrSigma, size);
        }

        /// <summary>
        /// 2-D Gaussian kernel.
        /// </summary>
        /// 
        /// <param name="size">Kernel size (should be odd), [3, 101].</param>
        /// 
        /// <returns>Returns 2-D Gaussian kernel of specified size.</returns>
        /// 
        /// <remarks><para>The function calculates 2-D Gaussian kernel, which is array
        /// of Gaussian function's values in the [-r, r] range of x,y values, where
        /// r=floor(<paramref name="size"/>/2).
        /// </para></remarks>
        /// 
        /// <exception cref="ArgumentException">Wrong kernel size.</exception>
        /// 
        [Obsolete("Please use Normal.Kernel2D instead.")]
        public double[,] Kernel2D(int size)
        {
            return Normal.Kernel2D(sqrSigma, size);
        }
    }
}
