// Accord Neural Net Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
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
    using Accord.Compat;

    /// <summary>
    ///   Rectified linear activation function.
    /// </summary>
    ///
    /// <remarks>
    ///   <para>This class implements a rectified linear activation 
    ///   function as given by the piecewise formula:</para>
    /// 
    ///   <code lang="none">
    ///   f(x) = x, if x > 0
    ///   f(x) = 0, otherwise
    ///   </code>
    ///   
    /// <para>
    ///   This function is non-differentiable at zero.
    /// </para>
    /// </remarks>
    ///
    [Serializable]
    public class RectifiedLinearFunction : IActivationFunction
    {

        /// <summary>
        ///   Initializes a new instance of the <see cref="RectifiedLinearFunction"/> class.
        /// </summary>
        /// 
        public RectifiedLinearFunction()
        {
        }

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
            if (x > 0)
                return x;
            return 0;
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
            if (x > 0)
                return 1;
            return 0;
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
            if (y > 0)
                return 1;
            return 0;
        }

    }
}