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
    using System;
    using Accord.Math;
    using Accord.Math.Distances;
    using Accord.Compat;

    /// <summary>
    ///   Composite Gaussian Kernel.
    /// </summary>
    /// 
    [Serializable]
    public sealed class Gaussian<TDistance> : KernelBase, IKernel, 
        IEstimable, ICloneable 
        where TDistance : IDistance, ICloneable
    {
        private double sigma;
        private double gamma;

        private TDistance distance;

        /// <summary>
        ///   Constructs a new Composite Gaussian Kernel
        /// </summary>
        /// 
        /// <param name="innerKernel">The inner kernel function of the composite kernel.</param>
        /// 
        public Gaussian(TDistance innerKernel)
            : this(innerKernel, 1)
        {
        }

        /// <summary>
        ///   Constructs a new Composite Gaussian Kernel
        /// </summary>
        /// 
        /// <param name="innerKernel">The inner kernel function of the composite kernel.</param>
        /// <param name="sigma">The kernel's sigma parameter.</param>
        /// 
        public Gaussian(TDistance innerKernel, double sigma)
        {
            this.distance = innerKernel;
            this.sigma = sigma;
            this.gamma = 1.0 / (2.0 * sigma * sigma);
        }

        /// <summary>
        ///   Gets or sets the sigma value for the kernel. When setting
        ///   sigma, gamma gets updated accordingly (gamma = 0.5/sigma^2).
        /// </summary>
        /// 
        public double Sigma
        {
            get { return sigma; }
            set
            {
                sigma = value;
                gamma = 1.0 / (2.0 * sigma * sigma);
            }
        }

        /// <summary>
        ///   Gets or sets the sigma² value for the kernel. When setting
        ///   sigma², gamma gets updated accordingly (gamma = 0.5/sigma²).
        /// </summary>
        /// 
        public double SigmaSquared
        {
            get { return sigma * sigma; }
            set
            {
                sigma = Math.Sqrt(value);
                gamma = 1.0 / (2.0 * value);
            }
        }

        /// <summary>
        ///   Gets or sets the gamma value for the kernel. When setting
        ///   gamma, sigma gets updated accordingly (gamma = 0.5/sigma^2).
        /// </summary>
        /// 
        public double Gamma
        {
            get { return gamma; }
            set
            {
                gamma = value;
                sigma = Math.Sqrt(1.0 / (gamma * 2.0));
            }
        }

        /// <summary>
        ///   Gaussian Kernel function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public override double Function(double[] x, double[] y)
        {
            if (x == y)
                return 1.0;

            double distance = this.distance.Distance(x, y);

            return Math.Exp(-gamma * distance);
        }



        void IEstimable<double[]>.Estimate(double[][] inputs)
        {
            this.Gamma = Gaussian.Estimate(distance, inputs).Gamma;
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
            Gaussian<TDistance> clone = (Gaussian<TDistance>)MemberwiseClone();
            clone.distance = (TDistance)distance.Clone();
            return clone;
        }

    }
}
