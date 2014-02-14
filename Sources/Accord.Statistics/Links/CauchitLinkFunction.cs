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
    ///   Cauchy link function.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Cauchy link function is associated with the
    ///   <see cref="CauchyDistribution">Cauchy distribution</see>.</para>
    ///   
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
    public class CauchitLinkFunction : ILinkFunction
    {

        /// <summary>
        ///   Linear scaling coefficient a (intercept).
        /// </summary>
        /// 
        public double A { get; private set; }

        /// <summary>
        ///   Linear scaling coefficient b (slope).
        /// </summary>
        /// 
        public double B { get; private set; }

        /// <summary>
        ///   Creates a new Cauchit link function.
        /// </summary>
        /// 
        /// <param name="beta">The beta value. Default is 1/pi.</param>
        /// <param name="constant">The constant value. Default is 0.5.</param>
        /// 
        public CauchitLinkFunction(double beta, double constant)
        {
            this.B = beta;
            this.A = constant;
        }

        /// <summary>
        ///   Creates a new Cauchit link function.
        /// </summary>
        /// 
        public CauchitLinkFunction() 
            : this(1.0 / Math.PI, 0.5) { }

        /// <summary>
        ///   The Cauchit link function.
        /// </summary>
        /// 
        /// <param name="x">An input value.</param>
        /// 
        /// <returns>The transformed input value.</returns>
        /// 
        /// <remarks>
        ///   The Cauchit link function is given by <c>f(x) = tan((x - A) / B)</c>.
        /// </remarks>
        /// 
        public double Function(double x)
        {
            return Math.Tan((x - A) / B);
        }

        /// <summary>
        ///   The Cauchit mean (activation) function.
        /// </summary>
        /// 
        /// <param name="x">A transformed value.</param>
        /// 
        /// <returns>The reverse transformed value.</returns>
        /// 
        /// <remarks>
        ///   The inverse Cauchit link function is given by <c>g(x) = tan(x) * B + A</c>.
        /// </remarks>
        /// 
        public double Inverse(double x)
        {
            return Math.Atan(x) * B + A;
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
        ///   The first derivative of the Cauchit link function 
        ///   in terms of y = f(x) is given by 
        ///   
        ///     <c>f'(y) =  B / (x * x + 1)</c>
        ///     
        /// </remarks>
        /// 
        public double Derivative(double x)
        {
            return B / (x * x + 1);
        }

        /// <summary>
        ///   First derivative of the mean function
        ///   expressed in terms of it's output.
        /// </summary>
        /// 
        /// <param name="y">The reverse transformed value.</param>
        /// 
        /// <returns>The first derivative of the input value.</returns>
        /// 
        /// <remarks>
        ///   The first derivative of the Cauchit link function 
        ///   in terms of y = f(x) is given by 
        ///   
        ///     <c>f'(y) = B / (tan((y - A) / B)² + 1)</c>
        ///     
        /// </remarks>
        /// 
        public double Derivative2(double y)
        {
            double x = Math.Tan((y - A) / B);
            return B / (x * x + 1);
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
