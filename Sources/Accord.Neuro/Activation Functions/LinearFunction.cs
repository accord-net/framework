// Accord Neural Net Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Neuro
{
    using System;
    using AForge;
    using AForge.Neuro;

    /// <summary>
    ///   Linear activation function.
    /// </summary>
    ///
    /// <remarks>
    ///   <para>This class implements a linear activation function bounded
    ///   in the interval (a,b), as given by the piecewise formula:</para>
    /// 
    ///   <code lang="none">
    ///   f(x) = alpha*x, if a > x*alpha > b
    ///   f(x) = a,       if a > x*alpha;
    ///   f(x) = b,       if     x*alpha > b;
    ///   </code>
    ///   
    /// <para>
    ///   In which, by default, a = -1 and b = +1.</para>
    /// 
    /// <para>
    ///   This function is continuous only in the interval (a/alpha, b/alpha). This is similar
    ///   to the threshold function but with a linear growth component. If alpha is set to a 
    ///   very high value (such as infinity), the function behaves as a threshold function.
    /// </para>
    /// 
    /// <para>The output range of the function can be set to an arbitrary
    /// value. The default output range is <b>[-1, +1]</b>.</para>
    /// 
    /// </remarks>
    ///
    [Serializable]
    public class LinearFunction : IActivationFunction
    {
        // linear slope value
        private double alpha = 1;

        // function output range
        private DoubleRange range = new DoubleRange(-1, +1);

        /// <summary>
        /// Linear slope value.
        /// </summary>
        /// 
        /// <remarks>
        ///   <para>Default value is set to <b>1</b>.</para>
        /// </remarks>
        /// 
        public double Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }

        /// <summary>
        ///   Function output range.
        /// </summary>
        ///
        /// <remarks>
        ///   <para>Default value is set to [-1;+1]</para>
        /// </remarks>
        ///
        public DoubleRange Range
        {
            get { return range; }
            set { range = value; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LinearFunction"/> class.
        /// </summary>
        /// 
        public LinearFunction(double alpha)
        {
            this.Alpha = alpha;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LinearFunction"/> class.
        /// </summary>
        /// 
        public LinearFunction(DoubleRange range)
        {
            this.Range = range;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LinearFunction"/> class.
        /// </summary>
        /// 
        public LinearFunction(double alpha, DoubleRange range)
        {
            this.Alpha = alpha;
            this.Range = range;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LinearFunction"/> class.
        /// </summary>
        /// 
        public LinearFunction() { }

        /// <summary>
        ///   Calculates function value.
        /// </summary>
        ///
        /// <param name="x">Function input value.</param>
        /// 
        /// <returns>Function output value, <i>f(x)</i>.</returns>
        ///
        /// <remarks>The method calculates function value at point <paramref name="x"/>.</remarks>
        ///
        public double Function(double x)
        {
            double y = alpha * x;

            if (y > range.Max)
                return range.Max;
            else if (y < range.Min)
                return range.Min;
            return y;
        }

        /// <summary>
        ///   Calculates function derivative.
        /// </summary>
        /// 
        /// <param name="x">Function input value.</param>
        /// 
        /// <returns>Function derivative, <i>f'(x)</i>.</returns>
        /// 
        /// <remarks>The method calculates function derivative at point <paramref name="x"/>.</remarks>
        ///
        public double Derivative(double x)
        {
            double y = alpha * x;

            if (y <= range.Min || y >= range.Max)
                return 0;
            return alpha;
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
        public double Derivative2(double y)
        {
            if (y <= range.Min || y >= range.Max) 
                return 0;
            return alpha;
        }

    }
}