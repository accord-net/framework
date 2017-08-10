// Accord Statistics Library
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

namespace Accord.Statistics.Kernels
{
    using Accord.Math.Distances;
    using System;
    using Accord.Compat;

    /// <summary>
    ///   Pearson VII universal kernel (PUK).
    /// </summary>
    /// 
    [Serializable]
    public sealed class Pearson : KernelBase, IKernel, IDistance,
        ICloneable
    {
        private double omega;
        private double sigma;

        private double constant;

        /// <summary>
        ///   Constructs a new Pearson VII universal kernel.
        /// </summary>
        /// 
        /// <param name="omega">The Pearson's omega parameter w. Default is 1.</param>
        /// <param name="sigma">The Pearson's sigma parameter s. Default is 1.</param>
        /// 
        public Pearson(double omega, double sigma)
        {
            this.omega = omega;
            this.sigma = sigma;
            this.constant = 2 * Math.Sqrt(Math.Pow(2, (1 / omega)) - 1) / sigma;
        }

        /// <summary>
        ///   Constructs a new Pearson VII universal kernel.
        /// </summary>
        /// 
        public Pearson()
            : this(1, 1) { }

        /// <summary>
        ///   Gets or sets the kernel's parameter omega. Default is 1.
        /// </summary>
        /// 
        public double Omega
        {
            get { return omega; }
            set
            {
                omega = value;
                constant = 2 * Math.Sqrt(Math.Pow(2, (1 / omega)) - 1) / sigma;
            }
        }

        /// <summary>
        ///   Gets or sets the kernel's parameter sigma. Default is 1.
        /// </summary>
        /// 
        public double Sigma
        {
            get { return sigma; }
            set
            {
                sigma = value;
                constant = 2 * Math.Sqrt(Math.Pow(2, (1 / omega)) - 1) / sigma;
            }
        }

        /// <summary>
        ///   Pearson Universal kernel function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// 
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public override double Function(double[] x, double[] y)
        {
            //Inner product
            double xx = 0;
            double yy = 0;
            double xy = 0;
            for (int i = 0; i < x.Length; i++)
            {
                double u = x[i] * x[i];
                double v = y[i] * y[i];
                double uv = x[i] * y[i];
                xx += u;
                yy += v;
                xy += uv;
            }

            double m = constant * Math.Sqrt(-2.0 * xy + xx + yy);
            return 1.0 / Math.Pow(1.0 + m * m, omega);
        }

        /// <summary>
        ///   Pearson Universal function.
        /// </summary>
        /// 
        /// <param name="z">Distance <c>z</c> in input space.</param>
        /// 
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(double z)
        {
            return 1 / Math.Pow(1 - z * constant, omega);
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
