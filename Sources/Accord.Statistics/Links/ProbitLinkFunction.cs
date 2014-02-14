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
    using Accord.Math;
    using Accord.Statistics.Distributions.Univariate;

    /// <summary>
    ///   Probit link function.
    /// </summary>
    /// 
    [Serializable]
    public class ProbitLinkFunction : ILinkFunction
    {

        private const double lnconstant = -Constants.LogSqrt2PI;


        /// <summary>
        ///   Creates a new Probit link function.
        /// </summary>
        /// 
        public ProbitLinkFunction()
        {
        }

        /// <summary>
        ///   The Probit link function.
        /// </summary>
        /// 
        /// <param name="x">An input value.</param>
        /// 
        /// <returns>The transformed input value.</returns>
        /// 
        /// <remarks>
        ///   The Probit link function is given by <c>f(x) = Phi^-1(x)</c>,
        ///   in which <see cref="Normal.Inverse">Phi^-1</see> is the
        ///   <see cref="Normal.Inverse">inverse Normal (Gaussian) cumulative 
        ///   distribution function</see>.
        /// </remarks>
        /// 
        public double Function(double x)
        {
            return Normal.Inverse(x);
        }

        /// <summary>
        ///   The Probit mean (activation) function.
        /// </summary>
        /// 
        /// <param name="x">A transformed value.</param>
        /// 
        /// <returns>The reverse transformed value.</returns>
        /// 
        /// <remarks>
        ///   The Probit link function is given by <c>g(x) = Phi(x)</c>,
        ///   in which <see cref="Normal.Function">Phi</see> is the
        ///   <see cref="Normal.Function">Normal (Gaussian) cumulative 
        ///   distribution function</see>.
        /// </remarks>
        /// 
        public double Inverse(double x)
        {
            return Normal.Function(x);
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
        ///   The first derivative of the identity link function is 
        ///   given by <c>f'(x) = exp(c - (Phi^-1(x))² * 0.5)</c> in
        ///   which <c>c = -<see cref="Constants.LogSqrt2PI">log(sqrt(2*π)</see></c>
        ///   and <see cref="Normal.Inverse">Phi^-1</see> is the <see cref="Normal.Inverse">
        ///   inverse Normal (Gaussian) cumulative distribution function</see>.
        /// </remarks>
        /// 
        public double Derivative(double x)
        {
            return Math.Exp(lnconstant - x * x * 0.5);
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
        ///   The first derivative of the identity link function in terms
        ///   of y = f(x) is given by <c>f'(y) = exp(c - x * x * 0.5)</c>
        ///   in which <c>c = -<see cref="Constants.LogSqrt2PI">log(sqrt(2*π)</see></c>
        ///   and <c>x = </c>
        /// </remarks>
        /// 
        public double Derivative2(double y)
        {
            double x = Normal.Inverse(y);
            return Math.Exp(lnconstant - x * x * 0.5);
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
