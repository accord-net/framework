// Accord Statistics Library
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

namespace Accord.Statistics.Links
{
    using System;

    /// <summary>
    ///   Link function interface.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The link function provides the relationship between the linear predictor and the 
    ///   mean of the distribution function. There are many commonly used link functions, and 
    ///   their choice can be somewhat arbitrary. It can be convenient to match the domain of
    ///   the link function to the range of the distribution function's mean.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Generalized_linear_model#Link_function">
    ///       Wikipedia contributors. "Generalized linear model." Wikipedia, The Free Encyclopedia.</a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <seealso cref="ProbitLinkFunction"/>
    /// <seealso cref="LogitLinkFunction"/>
    /// <seealso cref="Accord.Statistics.Models.Regression.GeneralizedLinearRegression"/>
    /// 
    public interface ILinkFunction : ICloneable
    {

        /// <summary>
        ///   The link function.
        /// </summary>
        /// 
        /// <param name="x">An input value.</param>
        /// 
        /// <returns>The transformed input value.</returns>
        /// 
        double Function(double x);

        /// <summary>
        ///   The mean (activation) function.
        /// </summary>
        /// 
        /// <param name="x">A transformed value.</param>
        /// 
        /// <returns>The reverse transformed value.</returns>
        /// 
        double Inverse(double x);


        /// <summary>
        ///   First derivative of the <see cref="Inverse"/> function.
        /// </summary>
        /// 
        /// <param name="x">The input value.</param>
        /// 
        /// <returns>The first derivative of the input value.</returns>
        /// 
        double Derivative(double x);

        /// <summary>
        ///   First derivative of the <see cref="Inverse"/>
        ///   function expressed in terms of it's output.
        /// </summary>
        /// 
        /// <param name="y">The reverse transformed value.</param>
        /// 
        /// <returns>The first derivative of the input value.</returns>
        /// 
        double Derivative2(double y);

    }

}
