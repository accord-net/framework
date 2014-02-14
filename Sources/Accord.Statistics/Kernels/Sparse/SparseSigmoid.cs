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

namespace Accord.Statistics.Kernels.Sparse
{
    using System;

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
    public sealed class SparseSigmoid : IKernel
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
        public double Function(double[] x, double[] y)
        {
            double sum = 0.0;

            int i = 0, j = 0;
            double posx, posy;

            while (i < x.Length && j < y.Length)
            {
                posx = x[i]; posy = y[j];

                if (posx == posy)
                {
                    sum += x[i + 1] * y[j + 1];

                    i += 2; j += 2;
                }
                else if (posx < posy)
                {
                    i += 2;
                }
                else if (posx > posy)
                {
                    j += 2;
                }
            }

            return System.Math.Tanh(Gamma * sum + Constant);
        }

    }
}
