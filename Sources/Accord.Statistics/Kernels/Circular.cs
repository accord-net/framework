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
    ///   Circular Kernel.
    /// </summary>
    /// 
    /// <remarks>
    ///   The circular kernel comes from a statistics perspective. It is an example
    ///   of an isotropic stationary kernel and is positive definite in R^2.
    /// </remarks>
    /// 
    [Serializable]
    public sealed class Circular : IKernel, ICloneable
    {
        private const double c2dPI = 2.0 / System.Math.PI;

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
        ///   Constructs a new Circular Kernel.
        /// </summary>
        /// 
        /// <param name="sigma">Value for sigma.</param>
        /// 
        public Circular(double sigma)
        {
            this.sigma = sigma;
        }

        /// <summary>
        ///   Circular Kernel Function
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(double[] x, double[] y)
        {
            double norm = 0.0, a, b, c;
            for (int i = 0; i < x.Length; i++)
            {
                double d = x[i] - y[i];
                norm += d * d;
            }

            norm = System.Math.Sqrt(norm);

            if (norm >= sigma)
            {
                return 0;
            }
            else
            {
                norm = norm / sigma;
                a = c2dPI * System.Math.Acos(-norm);
                b = c2dPI * norm;
                c = 1.0 - norm * norm;

                return a - b * System.Math.Sqrt(c);
            }
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
