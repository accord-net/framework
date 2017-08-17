﻿// Accord Statistics Library
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

namespace Accord.Statistics.Links
{
    using System;
    using Accord.Math;
    using Accord.Compat;

    /// <summary>
    ///   Natural logarithm of natural logarithm link function.
    /// </summary>
    /// 
    [Serializable]
    public class LogLogLinkFunction : ILinkFunction
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
        ///   Creates a new Log-Log link function.
        /// </summary>
        /// 
        /// <param name="beta">The beta value.</param>
        /// <param name="constant">The constant value.</param>
        /// 
        public LogLogLinkFunction(double beta, double constant)
        {
            this.B = beta;
            this.A = constant;
        }

        /// <summary>
        ///   Creates a new Log-Log link function.
        /// </summary>
        /// 
        public LogLogLinkFunction() : this(1, 0) { }

        /// <summary>
        ///   Creates a Complementary Log-Log link function.
        /// </summary>
        /// 
        public static LogLogLinkFunction Complementary()
        {
            return new LogLogLinkFunction(-1, 1);
        }

        /// <summary>
        /// The Log-log link function.
        /// </summary>
        /// 
        /// <param name="x">An input value.</param>
        /// 
        /// <returns>The transformed input value.</returns>
        /// 
        public double Function(double x)
        {
            return (Math.Log(-Math.Log(x)) - A) / B;
        }

        /// <summary>
        ///   The Log-log mean (activation) function.
        /// </summary>
        /// 
        /// <param name="x">A transformed value.</param>
        /// 
        /// <returns>The reverse transformed value.</returns>
        /// 
        public double Inverse(double x)
        {
            return Math.Exp(-Math.Exp(B * x + A));
        }

        /// <summary>The logarithm of the inverse of the link function.</summary>
        /// <param name="x">A transformed value.</param>
        /// <returns>The log of the reverse transformed value.</returns>
        public double Log(double x)
        {
            return -Math.Exp(B * x + A);
        }

        /// <summary>
        ///   First derivative of the <see cref="Inverse"/> function.
        /// </summary>
        /// 
        /// <param name="x">The input value.</param>
        /// 
        /// <returns>The first derivative of the input value.</returns>
        /// 
        public double Derivative(double x)
        {
            // -(b+1) e^(b x-e^(b x+x)+x)

            double z = B * x + A;
            double w = z - Math.Exp(z);
            return -B * Math.Exp(w);
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
        public double Derivative2(double y)
        {
            double w = Math.Log(y);
            return B * y * w;
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
