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
    using Accord.Statistics.Distributions.Multivariate;

    /// <summary>
    ///   Logit link function.
    /// </summary>
    /// 
    /// <remarks>
    ///   The Logit link function is associated with the
    ///   <see cref="BinomialDistribution">Binomial</see> and
    ///   <see cref="MultinomialDistribution">Multinomial</see> distributions.
    /// </remarks>
    /// 
    [Serializable]
    public class LogitLinkFunction : ILinkFunction
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
        ///   Creates a new Logit link function.
        /// </summary>
        /// 
        /// <param name="beta">The beta value. Default is 1.</param>
        /// <param name="constant">The constant value. Default is 0.</param>
        /// 
        public LogitLinkFunction(double beta, double constant)
        {
            this.B = beta;
            this.A = constant;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LogitLinkFunction"/> class.
        /// </summary>
        /// 
        public LogitLinkFunction()
            : this(1, 0) { }


        /// <summary>
        ///   The Logit link function.
        /// </summary>
        /// 
        /// <param name="x">An input value.</param>
        /// 
        /// <returns>The transformed input value.</returns>
        /// 
        /// <remarks>
        ///   The inverse Logit link function is given by
        ///   <c>f(x) = (Math.Log(x / (1.0 - x)) - A) / B</c>.
        /// </remarks>
        /// 
        public double Function(double x)
        {
            return (Math.Log(x / (1.0 - x)) - A) / B;
        }

        /// <summary>
        ///   The Logit mean (activation) function.
        /// </summary>
        /// 
        /// <param name="x">A transformed value.</param>
        /// 
        /// <returns>The reverse transformed value.</returns>
        /// 
        /// <remarks>
        ///   The inverse Logit link function is given by
        ///   <c>g(x) = 1.0 / (1.0 + Math.Exp(-z)</c> in
        ///   which <c>z =  B * x + A</c>.
        /// </remarks>
        /// 
        public double Inverse(double x)
        {
            double z = B * x + A;
            return 1.0 / (1.0 + Math.Exp(-z));
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
        ///   The first derivative of the identity link 
        ///   function is given by <c>f'(x) = y * (1.0 - y)</c>
        ///   where <c>y = f(x)</c> is the <see cref="Function">
        ///   Logit function</see>.
        /// </remarks>
        /// 
        public double Derivative(double x)
        {
            double z = B * x + A;
            double y = 1.0 / (1.0 + Math.Exp(-z));

            return y * (1.0 - y);
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
        ///   The first derivative of the Logit link function 
        ///   in terms of y = f(x) is given by <c>y * (1.0 - y)</c>.
        /// </remarks>
        /// 
        public double Derivative2(double y)
        {
            return y * (1.0 - y);
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
