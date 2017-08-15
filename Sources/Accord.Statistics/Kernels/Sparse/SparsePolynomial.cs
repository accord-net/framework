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
    ///   Sparse Polynomial Kernel.
    /// </summary>
    /// 
    [Serializable]
    [Obsolete("Please use Polynomial<Sparse<double>> instead.")]
    public sealed class SparsePolynomial : KernelBase, IKernel
    {
        private int degree;
        private double constant;

        /// <summary>
        ///   Constructs a new Sparse Polynomial kernel of a given degree.
        /// </summary>
        /// 
        /// <param name="degree">The polynomial degree for this kernel.</param>
        /// <param name="constant">The polynomial constant for this kernel. Default is 1.</param>
        /// 
        public SparsePolynomial(int degree, double constant)
        {
            this.degree = degree;
            this.constant = constant;
        }

        /// <summary>
        ///   Constructs a new Polynomial kernel of a given degree.
        /// </summary>
        /// 
        /// <param name="degree">The polynomial degree for this kernel.</param>
        /// 
        public SparsePolynomial(int degree)
            : this(degree, 1.0)
        {
        }

        /// <summary>
        ///   Gets or sets the kernel's polynomial degree.
        /// </summary>
        /// 
        public int Degree
        {
            get { return degree; }
            set { degree = value; }
        }

        /// <summary>
        ///   Gets or sets the kernel's polynomial constant term.
        /// </summary>
        /// 
        public double Constant
        {
            get { return constant; }
            set { constant = value; }
        }

        /// <summary>
        ///   Polynomial kernel function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public override double Function(double[] x, double[] y)
        {
#pragma warning disable 0618

            double sum = SparseLinear.Product(x,y) + constant;

            return Math.Pow(sum, Degree);
        }

    }
}
