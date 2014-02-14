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
    ///   Cauchy Kernel.
    /// </summary>
    /// 
    /// <remarks>
    ///   The Cauchy kernel comes from the Cauchy distribution (Basak, 2008). It is a
    ///   long-tailed kernel and can be used to give long-range influence and sensitivity
    ///   over the high dimension space.
    /// </remarks>
    /// 
    [Serializable]
    public sealed class Cauchy : IKernel, ICloneable
    {
        private double sigma;

        /// <summary>
        ///   Gets or sets the kernel's sigma value.
        /// </summary>
        /// 
        public double Sigma
        {
            get { return sigma; }
            set { sigma = value; }
        }

        /// <summary>
        ///   Constructs a new Cauchy Kernel.
        /// </summary>
        /// 
        /// <param name="sigma">The value for sigma.</param>
        /// 
        public Cauchy(double sigma)
        {
            this.sigma = sigma;
        }

        /// <summary>
        ///   Cauchy Kernel Function
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(double[] x, double[] y)
        {
            // Optimization in case x and y are
            // exactly the same object reference.
            if (x == y) return 1.0;

            double norm = 0.0, d;
            for (int i = 0; i < x.Length; i++)
            {
                d = x[i] - y[i];
                norm += d * d;
            }

            return (1.0 / (1.0 + norm / sigma));
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
