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
    /// Bipolar sigmoid activation function.
    /// </summary>
    ///
    /// <remarks><para>The class represents bipolar sigmoid activation function with
    /// the next expression:
    /// <code lang="none">
    ///                2
    /// f(x) = ------------------ - 1
    ///        1 + exp(-alpha * x)
    ///
    ///           2 * alpha * exp(-alpha * x )
    /// f'(x) = -------------------------------- = alpha * (1 - f(x)^2) / 2
    ///           (1 + exp(-alpha * x))^2
    /// </code>
    /// </para>
    /// 
    /// <para>Output range of the function: <b>[-1, 1]</b>.</para>
    /// 
    /// <para>Functions graph:</para>
    /// <img src="img/neuro/sigmoid_bipolar.bmp" width="242" height="172" />
    /// </remarks>
    /// 
    [Serializable]
    public class BipolarSigmoidFunction : IActivationFunction, ICloneable
    {
        // sigmoid's alpha value
        private double alpha = 2;

        /// <summary>
        /// Sigmoid's alpha value.
        /// </summary>
        ///
        /// <remarks><para>The value determines steepness of the function. Increasing value of
        /// this property changes sigmoid to look more like a threshold function. Decreasing
        /// value of this property makes sigmoid to be very smooth (slowly growing from its
        /// minimum value to its maximum value).</para>
        ///
        /// <para>Default value is set to <b>2</b>.</para>
        /// </remarks>
        /// 
        public double Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SigmoidFunction"/> class.
        /// </summary>
        public BipolarSigmoidFunction( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BipolarSigmoidFunction"/> class.
        /// </summary>
        /// 
        /// <param name="alpha">Sigmoid's alpha value.</param>
        /// 
        public BipolarSigmoidFunction( double alpha )
        {
            this.alpha = alpha;
        }

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
            return ( ( 2 / ( 1 + Math.Exp( -alpha * x ) ) ) - 1 );
        }

        /// <summary>
        /// Calculates function derivative.
        /// </summary>
        /// 
        /// <param name="x">Function input value.</param>
        /// 
        /// <returns>Function derivative, <i>f'(x)</i>.</returns>
        /// 
        /// <remarks>The method calculates function derivative at point <paramref name="x"/>.</remarks>
        ///
        public double Derivative( double x )
        {
            double y = Function( x );

            return ( alpha * ( 1 - y * y ) / 2 );
        }

        /// <summary>
        /// Calculates function derivative.
        /// </summary>
        /// 
        /// <param name="y">Function output value - the value, which was obtained
        /// with the help of <see cref="Function"/> method.</param>
        /// 
        /// <returns>Function derivative, <i>f'(x)</i>.</returns>
        ///
        /// <remarks><para>The method calculates the same derivative value as the
        /// <see cref="Derivative"/> method, but it takes not the input <b>x</b> value
        /// itself, but the function value, which was calculated previously with
        /// the help of <see cref="Function"/> method.</para>
        /// 
        /// <para><note>Some applications require as function value, as derivative value,
        /// so they can save the amount of calculations using this method to calculate derivative.</note></para>
        /// </remarks>
        /// 
        public double Derivative2( double y )
        {
            return ( alpha * ( 1 - y * y ) / 2 );
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
            return new BipolarSigmoidFunction( alpha );
        }
    }
}
