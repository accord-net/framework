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
    using Accord.Math.Distances;
    using Accord.Compat;

    /// <summary>
    ///   Sparse Laplacian Kernel.
    /// </summary>
    /// 
    [Serializable]
    [Obsolete("Please use the Laplacian kernel with Sparse<double> instead.")]
    public sealed class SparseLaplacian : KernelBase, IKernel, IDistance
    {
        private double sigma;
        private double gamma;

        /// <summary>
        ///   Constructs a new Laplacian Kernel
        /// </summary>
        /// 
        public SparseLaplacian() 
            : this(1) { }

        /// <summary>
        ///   Constructs a new Laplacian Kernel
        /// </summary>
        /// 
        /// <param name="sigma">The sigma slope value.</param>
        /// 
        public SparseLaplacian(double sigma)
        {
            this.Sigma = sigma;
        }

        /// <summary>
        ///   Gets or sets the sigma value for the kernel. When setting
        ///   sigma, gamma gets updated accordingly (gamma = 0.5*/sigma^2).
        /// </summary>
        /// 
        public double Sigma
        {
            get { return sigma; }
            set
            {
                sigma = value;
                gamma = 1.0 / sigma;
            }
        }

        /// <summary>
        ///   Gets or sets the gamma value for the kernel. When setting
        ///   gamma, sigma gets updated accordingly (gamma = 0.5*/sigma^2).
        /// </summary>
        /// 
        public double Gamma
        {
            get { return gamma; }
            set
            {
                gamma = value;
                sigma = 1.0 / gamma;
            }
        }


        /// <summary>
        ///   Laplacian Kernel function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public override double Function(double[] x, double[] y)
        {
            // Optimization in case x and y are
            // exactly the same object reference.

            if (x == y)
                return 1.0;

#pragma warning disable 0618

            double norm = SparseLinear.SquaredEuclidean(x, y);

            return Math.Exp(-gamma * Math.Sqrt(norm));
        }

        /// <summary>
        ///   Computes the distance in input space
        ///   between two points given in feature space.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in feature (kernel) space.</param>
        /// <param name="y">Vector <c>y</c> in feature (kernel) space.</param>
        /// 
        /// <returns>Distance between <c>x</c> and <c>y</c> in input space.</returns>
        /// 
        public override double Distance(double[] x, double[] y)
        {
            if (x == y)
                return 0.0;

            double norm = SparseLinear.SquaredEuclidean(x, y);

            return 2 - 2 * Math.Exp(-gamma * Math.Sqrt(norm));
        }


        /// <summary>
        ///   Estimate appropriate values for sigma given a data set.
        /// </summary>
        /// 
        /// <remarks>
        ///   This method uses a simple heuristic to obtain appropriate values
        ///   for sigma in a radial basis function kernel. The heuristic is shown
        ///   by Caputo, Sim, Furesjo and Smola, "Appearance-based object
        ///   recognition using SVMs: which kernel should I use?", 2002.
        /// </remarks>
        /// 
        /// <param name="inputs">The data set.</param>
        /// <param name="samples">The number of random samples to analyze.</param>
        /// <param name="range">The range of suitable values for sigma.</param>
        /// 
        /// <returns>A Laplacian kernel initialized with an appropriate sigma value.</returns>
        /// 
        public static SparseLaplacian Estimate(double[][] inputs, int samples, out DoubleRange range)
        {
            if (samples > inputs.Length)
                throw new ArgumentOutOfRangeException("samples");

            double[] distances = Gaussian.Distances(inputs, samples);

            double q1 = distances[(int)Math.Ceiling(0.15 * distances.Length)];
            double q9 = distances[(int)Math.Ceiling(0.85 * distances.Length)];
            double qm = distances.Median(alreadySorted: true);

            range = new DoubleRange(q1, q9);

            return new SparseLaplacian(sigma: qm);
        }
    }
}
