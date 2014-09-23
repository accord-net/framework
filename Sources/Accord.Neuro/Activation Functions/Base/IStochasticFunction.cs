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

namespace Accord.Neuro.ActivationFunctions
{
    using AForge.Neuro;

    /// <summary>
    ///   Common interface for stochastic activation functions.
    /// </summary>
    /// 
    /// <seealso cref="BernoulliFunction"/>
    /// <seealso cref="GaussianFunction"/>
    /// 
    public interface IStochasticFunction : IActivationFunction
    {
        /// <summary>
        ///   Samples a value from the function given a input value.
        /// </summary>
        /// 
        /// <param name="x">Function input value.</param>
        /// 
        /// <returns>Draws a random value from the function.</returns>
        /// 
         double Generate(double x);

         /// <summary>
         ///   Samples a value from the function given a function output value.
         /// </summary>
         /// 
         /// <param name="y">The function output value. This is the value which was obtained
         /// with the help of the <see cref="IActivationFunction.Function(double)"/> method.</param>
         /// 
         /// <remarks><para>The method calculates the same output value as the
         /// <see cref="Generate"/> method, but it takes not the input <b>x</b> value
         /// itself, but the function value, which was calculated previously with help
         /// of the <see cref="IActivationFunction.Function(double)"/> method.</para>
         /// </remarks>
         /// 
         /// <returns>Draws a random value from the function.</returns>
         /// 
         double Generate2(double y);
    }
}
