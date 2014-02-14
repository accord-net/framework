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
    using Accord.Statistics.Distributions.Univariate;

    /// <summary>
    ///   Absolute link function.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Link functions can be used in many models, such as in
    ///   <see cref="Accord.Statistics.Models.Regression.GeneralizedLinearRegression"/> and Support
    ///   Vector Machines.</para>    
    /// </remarks>
    /// 
    /// <seealso cref="ILinkFunction"/>
    /// <seealso cref="Accord.Statistics.Models.Regression.GeneralizedLinearRegression"/>
    /// 
    [Serializable]
    public class AbsoluteLinkFunction : ILinkFunction
    {

        /// <summary>
        ///   Linear scaling coefficient b (slope).
        /// </summary>
        /// 
        public double B { get; set; }

        /// <summary>
        ///   Creates a new Absolute link function.
        /// </summary>
        /// 
        /// <param name="beta">The beta value.</param>
        /// 
        public AbsoluteLinkFunction(double beta)
        {
            this.B = beta;
        }

        /// <summary>
        ///   Creates a new Absolute link function.
        /// </summary>
        /// 
        public AbsoluteLinkFunction() : this(1) { }

        /// <summary>
        ///   The Absolute link function.
        /// </summary>
        /// 
        /// <param name="x">An input value.</param>
        /// 
        /// <returns>The transformed input value.</returns>
        /// 
        /// <remarks>
        ///   The absolute link function is given by <c>f(x) = abs(x) / b</c>.
        /// </remarks>
        /// 
        public double Function(double x)
        {
            return Math.Abs(x) / B;
        }

        /// <summary>
        ///   The mean function.
        /// </summary>
        /// 
        /// <param name="x">A transformed value.</param>
        /// 
        /// <returns>The reverse transformed value.</returns>
        /// 
        /// <remarks>
        ///   The inverse absolute link function is given by <c>g(x) = B * abs(x)</c>.
        /// </remarks>
        /// 
        public double Inverse(double x)
        {
            return B * Math.Abs(x);
        }

        /// <summary>
        ///   First derivative of the <see cref="Inverse"/> function.
        /// </summary>
        /// 
        /// <param name="x">The input value.</param>
        /// 
        /// <returns>The first derivative of the input value.</returns>
        /// 
        /// <remarks>
        ///   The first derivative of the absolute link function 
        ///   is given by <c>f'(x) = B</c>.
        /// </remarks>
        /// 
        public double Derivative(double x)
        {
            return B;
        }

        /// <summary>
        ///   First derivative of the <see cref="Inverse"/>
        ///   function expressed in terms of it's output.
        /// </summary>
        /// 
        /// <param name="y">The reverse transformed value.</param>
        /// 
        /// <returns>The first derivative of the input value.</returns>
        /// 
        /// <remarks>
        ///   The first derivative of the absolute link function 
        ///   in terms of y = f(x) is given by <c>f'(y) = B</c>.
        /// </remarks>
        /// 
        public double Derivative2(double y)
        {
            return B;
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
