// AForge Neural Net Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2012
// contacts@aforgenet.com
//

namespace AForge.Neuro
{
    using System;

    /// <summary>
    /// Threshold activation function.
    /// </summary>
    ///
    /// <remarks><para>The class represents threshold activation function with
    /// the next expression:
    /// <code lang="none">
    /// f(x) = 1, if x >= 0, otherwise 0
    /// </code>
    /// </para>
    /// 
    /// <para>Output range of the function: <b>[0, 1]</b>.</para>
    /// 
    /// <para>Functions graph:</para>
    /// <img src="img/neuro/threshold.bmp" width="242" height="172" />
    /// </remarks>
    ///
    [Serializable]
    public class ThresholdFunction : IActivationFunction, ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThresholdFunction"/> class.
        /// </summary>
        public ThresholdFunction( ) { }

        /// <summary>
        /// Calculates function value.
        /// </summary>
        ///
        /// <param name="x">Function input value.</param>
        /// 
        /// <returns>Function output value, <i>f(x)</i>.</returns>
        ///
        /// <remarks>The method calculates function value at point <paramref name="x"/>.</remarks>
        ///
        public double Function( double x )
        {
            return ( x >= 0 ) ? 1 : 0;
        }

        /// <summary>
        /// Calculates function derivative (not supported).
        /// </summary>
        /// 
        /// <param name="x">Input value.</param>
        /// 
        /// <returns>Always returns 0.</returns>
        /// 
        /// <remarks><para><note>The method is not supported, because it is not possible to
        /// calculate derivative of the function.</note></para></remarks>
        ///
        public double Derivative( double x )
        {
            return 0;
        }

        /// <summary>
        /// Calculates function derivative (not supported).
        /// </summary>
        /// 
        /// <param name="y">Input value.</param>
        /// 
        /// <returns>Always returns 0.</returns>
        /// 
        /// <remarks><para><note>The method is not supported, because it is not possible to
        /// calculate derivative of the function.</note></para></remarks>
        /// 
        public double Derivative2( double y )
        {
            return 0;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// 
        public object Clone( )
        {
            return new ThresholdFunction( );
        }
    }
}
