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
    ///   Additive combination of kernels.
    /// </summary>
    /// 
    [Serializable]
    public sealed class Additive : IKernel
    {
        private IKernel[] kernels;
        private double[] weights;

        /// <summary>
        ///   Gets the combination of kernels to use.
        /// </summary>
        /// 
        public IKernel[] Kernels
        {
            get { return kernels; }
        }

        /// <summary>
        ///   Gets the weight array to use in the weighted kernel sum.
        /// </summary>
        /// 
        public double[] Weights
        {
            get { return weights; }
        }

        /// <summary>
        ///   Constructs a new additive kernel.
        /// </summary>
        /// 
        /// <param name="kernels">Kernels to combine.</param>
        /// 
        public Additive(params IKernel[] kernels)
        {
            this.kernels = kernels;
            this.weights = new double[kernels.Length];

            for (int i = 0; i < kernels.Length; i++) 
                weights[i] = 1;
        }

        /// <summary>
        ///   Constructs a new additive kernel.
        /// </summary>
        /// 
        /// <param name="kernels">Kernels to combine.</param>
        /// 
        /// <param name="weights">
        ///   Weight values for each of the kernels.
        ///   Default is to assign equal weights.</param>
        ///   
        public Additive(IKernel[] kernels, params double[] weights)
        {
            if (kernels.Length != weights.Length)
            {
                throw new ArgumentException("The number of weights should equals the number of kernels.",
                    "weights");
            }

            this.kernels = kernels;
            this.weights = weights;
        }

        /// <summary>
        ///   Additive Kernel Combination function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(double[] x, double[] y)
        {
            double sum = 0.0;
            for (int i = 0; i < kernels.Length; i++)
            {
                sum += weights[i] * kernels[i].Function(x, y);
            }

            return sum;
        }

    }
}
