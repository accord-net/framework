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

namespace Accord.Statistics.Kernels
{
    using System;

    /// <summary>
    ///   Bessel Kernel.
    /// </summary>
    /// 
    /// <remarks>
    ///   The Bessel kernel is well known in the theory of function spaces of fractional smoothness. 
    /// </remarks>
    /// 
    [Serializable]
    public sealed class Bessel : IKernel, ICloneable
    {
        private int order;
        private double sigma;

        /// <summary>
        ///   Gets or sets the order of the Bessel function.
        /// </summary>
        /// 
        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        /// <summary>
        ///   Gets or sets the sigma constant for this kernel.
        /// </summary>
        /// 
        public double Sigma
        {
            get { return sigma; }
            set { sigma = value; }
        }

        /// <summary>
        ///   Constructs a new Bessel Kernel.
        /// </summary>
        /// 
        /// <param name="order">The order for the Bessel function.</param>
        /// <param name="sigma">The value for sigma.</param>
        /// 
        public Bessel(int order, double sigma)
        {
            this.order = order;
            this.sigma = sigma;
        }

        /// <summary>
        ///   Bessel Kernel Function
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(double[] x, double[] y)
        {
            double norm = 0.0;

            for (int k = 0; k < x.Length; k++)
            {
                double d = x[k] - y[k];
                norm += d * d;
            }
            norm = System.Math.Sqrt(norm);

            return Accord.Math.Bessel.J(order, sigma * norm) /
                System.Math.Pow(norm, -norm * order);
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
