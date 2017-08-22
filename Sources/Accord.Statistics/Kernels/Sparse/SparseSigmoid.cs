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

namespace Accord.Statistics.Kernels.Sparse
{
    using System;
    using Accord.Compat;

    /// <summary>
    ///   Sparse Sigmoid Kernel.
    /// </summary>
    /// 
    /// <remarks>
    ///   Sigmoid kernels are not positive definite and therefore do not induce
    ///   a reproducing kernel Hilbert space. However, they have been successfully
    ///   used in practice (Schölkopf and Smola, 2002).
    /// </remarks>
    /// 
    [Serializable]
    [Obsolete("Please use the Sigmoid kernel with Sparse<double> instead.")]
    public sealed class SparseSigmoid : KernelBase, IKernel
    {
        private double gamma;
        private double constant;

        /// <summary>
        ///   Constructs a Sparse Sigmoid kernel.
        /// </summary>
        /// 
        /// <param name="alpha">Alpha parameter.</param>
        /// <param name="constant">Constant parameter.</param>
        /// 
        public SparseSigmoid(double alpha, double constant)
        {
            this.gamma = alpha;
            this.constant = constant;
        }

        /// <summary>
        ///   Constructs a Sparse Sigmoid kernel.
        /// </summary>
        /// 
        public SparseSigmoid()
            : this(0.01, -Math.E) { }

        /// <summary>
        ///   Gets or sets the kernel's gamma parameter.
        /// </summary>
        /// 
        /// <remarks>
        ///   In a sigmoid kernel, gamma is a inner product
        ///   coefficient for the hyperbolic tangent function.
        /// </remarks>
        /// 
        public double Gamma
        {
            get { return gamma; }
            set { gamma = value; }
        }

        /// <summary>
        ///   Gets or sets the kernel's constant term.
        /// </summary>
        /// 
        public double Constant
        {
            get { return constant; }
            set { constant = value; }
        }

        /// <summary>
        ///   Sigmoid kernel function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public override double Function(double[] x, double[] y)
        {
#pragma warning disable 0618

            double sum = SparseLinear.Product(x, y);

            return System.Math.Tanh(Gamma * sum + Constant);
        }

    }
}
