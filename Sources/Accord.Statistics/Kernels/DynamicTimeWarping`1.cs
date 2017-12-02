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
    using System.Runtime.Serialization;
    using Accord.Math.Distances;
    using Accord.Compat;
    using System.Threading;

    /// <summary>
    ///   Radial Basis Function Dynamic Time Warping Sequence Kernel.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Dynamic Time Warping Sequence Kernel is a sequence kernel, accepting
    ///   vector sequences of variable size as input. Despite the sequences being
    ///   variable in size, the vectors contained in such sequences should have its
    ///   size fixed and should be informed at the construction of this kernel.</para>
    /// <para>
    ///   The conversion of the DTW global distance to a dot product uses a combination
    ///   of a technique known as spherical normalization and the polynomial kernel. The
    ///   degree of the polynomial kernel and the alpha for the spherical normalization
    ///   should be given at the construction of the kernel. For more information,
    ///   please see the referenced papers shown below.</para>
    ///   
    /// <para>
    ///   The use of a <see cref="KernelFunctionCache">cache</see> is highly advisable
    ///   when using this kernel.</para>
    ///   
    /// <para>
    ///   <list type="bullet">
    ///   References:
    ///     <item><description>
    ///     V. Wan, J. Carmichael; Polynomial Dynamic Time Warping Kernel Support
    ///     Vector Machines for Dysarthric Speech Recognition with Sparse Training
    ///     Data. Interspeech'2005 - Eurospeech - 9th European Conference on Speech
    ///     Communication and Technology. Lisboa, 2005.</description></item>
    ///   </list></para>
    /// 
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example demonstrates how to create and learn a Support Vector
    ///   Machine (SVM) to recognize sequences of univariate observations using the 
    ///   Dynamic Time Warping kernel.</para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\Kernels\DynamicTimeWarpingTest.cs" region="doc_learn" />
    ///
    /// <para>
    ///   Now, instead of having univariate observations, the following example 
    ///   demonstrates how to create and learn a sequences of multivariate (or
    ///   n-dimensional) observations.</para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.MachineLearning\Kernels\DynamicTimeWarpingTest.cs" region="doc_learn_multivariate" />
    /// </example>
    /// 
    [Serializable]
    public struct DynamicTimeWarping<TDistance, TInput>
        : IKernel<TInput[]>, ICloneable, IDistance<TInput[]>
        where TDistance : struct, IDistance<TInput>
    {
        private double sigma;
        private double gamma;

        private TDistance distance; // inner kernel function

        [NonSerialized]
        private ThreadLocal<Locals> locals;

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
        ///   Constructs a new Dynamic Time Warping kernel.
        /// </summary>
        /// 
        /// <param name="innerKernel">The inner kernel function of the composite kernel.</param>
        /// 
        public DynamicTimeWarping(TDistance innerKernel)
            : this(innerKernel, 1)
        {
        }

        /// <summary>
        ///   Constructs a new Dynamic Time Warping kernel.
        /// </summary>
        /// 
        /// <param name="innerKernel">The inner kernel function of the composite kernel.</param>
        /// <param name="sigma">The kernel's sigma parameter.</param>
        /// 
        public DynamicTimeWarping(TDistance innerKernel, double sigma)
        {
            this.distance = innerKernel;
            this.sigma = sigma;
            this.gamma = 1.0 / (2.0 * sigma * sigma);
            this.locals = new ThreadLocal<Locals>(() => new Locals());
        }

        /// <summary>
        ///   Constructs a new Dynamic Time Warping kernel.
        /// </summary>
        /// 
        /// <param name="sigma">The kernel's sigma parameter.</param>
        /// 
        public DynamicTimeWarping(double sigma)
        {
            this.distance = default(TDistance);
            this.sigma = sigma;
            this.gamma = 1.0 / (2.0 * sigma * sigma);
            this.locals = new ThreadLocal<Locals>(() => new Locals());
        }


        /// <summary>
        ///   Dynamic Time Warping kernel function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// 
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(TInput[] x, TInput[] y)
        {
            if (x == y)
                return 1.0;

            double distance = k(x, y);

            return Math.Exp(-gamma * distance);
        }

        /// <summary>
        ///   Computes the squared distance in feature space
        ///   between two points given in input space.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// 
        /// <returns>
        ///   Squared distance between <c>x</c> and <c>y</c> in feature (kernel) space.
        /// </returns>
        /// 
        public double Distance(TInput[] x, TInput[] y)
        {
            return Function(x, x) + Function(y, y) - 2 * Function(x, y);
        }

        internal double k(TInput[] x, TInput[] y)
        {
            if (this.locals == null)
                this.locals = new ThreadLocal<Locals>(() => new Locals());

            return D(locals.Value, x, y);
        }

        /// <summary>
        ///   Global distance D(X,Y) between two sequences of vectors.
        /// </summary>
        /// 
        /// <param name="locals">The current thread local storage.</param>
        /// <param name="sequence1">A sequence of vectors.</param>
        /// <param name="sequence2">A sequence of vectors.</param>
        /// 
        /// <returns>The global distance between X and Y.</returns>
        /// 
        private double D(Locals locals, TInput[] sequence1, TInput[] sequence2)
        {
            // Get the number of vectors in each sequence. The vectors
            // have been projected, so the length is augmented by one.
            int vectorCount1 = sequence1.Length;
            int vectorCount2 = sequence2.Length;

            // Application of the Dynamic Time Warping
            // algorithm by using dynamic programming.
            if (locals.m < vectorCount2 || locals.n < vectorCount1)
                locals.Create(vectorCount1, vectorCount2);

            double[,] DTW = locals.DTW;

            for (int i = 0; i < sequence1.Length; i++)
            {
                for (int j = 0; j < sequence2.Length; j++)
                {
                    // Compute the distance between the sequences
                    double cost = distance.Distance(sequence1[i], sequence2[j]);

                    double insertion = DTW[i, j + 1];
                    double deletion = DTW[i + 1, j];
                    double match = DTW[i, j];

                    double min = (insertion < deletion
                        ? (insertion < match ? insertion : match)
                        : (deletion < match ? deletion : match));

                    DTW[i + 1, j + 1] = cost + min;
                }
            }

            return DTW[vectorCount1, vectorCount2]; // return the minimum global distance
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


        [OnDeserialized]
        private void onDeserialized(StreamingContext context)
        {
            this.locals = new ThreadLocal<Locals>(() => new Locals());
        }

        private class Locals
        {
            public double[,] DTW;
            public int m;
            public int n;

            public Locals()
            {
            }

            public void Create(int n, int m)
            {
                this.n = n;
                this.m = m;
                this.DTW = new double[n + 1, m + 1];

                for (int i = 1; i <= n; i++)
                    DTW[i, 0] = double.PositiveInfinity;

                for (int i = 1; i <= m; i++)
                    DTW[0, i] = double.PositiveInfinity;
            }

        }

    }
}
