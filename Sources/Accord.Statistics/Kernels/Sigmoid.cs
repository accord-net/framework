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
    using Accord.Math;
    using AForge;

    /// <summary>
    ///   Sigmoid Kernel.
    /// </summary>
    /// 
    /// <remarks>
    ///   Sigmoid kernel of the form k(x,z) = tanh(a * x'z + c). Sigmoid kernels are only
    ///   conditionally positive definite for some values of a and c, and therefore may not
    ///   induce a reproducing kernel Hilbert space. However, they have been successfully 
    ///   used in practice (Schölkopf and Smola, 2002).
    /// </remarks>
    /// 
    [Serializable]
    public sealed class Sigmoid : IKernel, ICloneable, IEstimable
    {
        private double alpha;
        private double constant;

        /// <summary>
        ///   Estimates suitable values for the sigmoid kernel
        ///   by exploring the response area of the tanh function.
        /// </summary>
        /// 
        /// <param name="inputs">An input data set.</param>
        /// 
        /// <returns>A Sigmoid kernel initialized with appropriate values.</returns>
        /// 
        public static Sigmoid Estimate(double[][] inputs)
        {
            double[] norm = new double[inputs.Length];
            for (int i = 0; i < inputs.Length; i++)
                norm[i] = Norm.SquareEuclidean(inputs[i]);

            double max = Matrix.Max(norm);

            double r = -Math.E;
            double a = -r / max;

            return new Sigmoid(a, r);
        }

        /// <summary>
        ///   Estimates suitable values for the sigmoid kernel
        ///   by exploring the response area of the tanh function.
        /// </summary>
        /// 
        /// <param name="inputs">An input data set.</param>
        /// <param name="samples">The size of the subset to use in the estimation.</param>
        /// <param name="range">The interquartile range for the data.</param>
        /// 
        /// <returns>A Sigmoid kernel initialized with appropriate values.</returns>
        /// 
        public static Sigmoid Estimate(double[][] inputs, int samples, out DoubleRange range)
        {
            if (samples > inputs.Length)
                throw new ArgumentOutOfRangeException("samples");

            double[] distances = Products(inputs, samples);

            double q1 = Math.Sqrt(distances[(int)Math.Ceiling(0.15 * distances.Length)] / 2.0);
            double q9 = Math.Sqrt(distances[(int)Math.Ceiling(0.85 * distances.Length)] / 2.0);
            double qm = Math.Sqrt(Accord.Statistics.Tools.Median(distances, alreadySorted: true) / 2.0);

            range = new DoubleRange(q1, q9);

            double max = qm;

            double r = -Math.E;
            double a = -r / max;

            return new Sigmoid(a, r);
        }

        /// <summary>
        ///   Computes the set of all distances between 
        ///   all points in a random subset of the data.
        /// </summary>
        /// 
        /// <param name="inputs">The inputs points.</param>
        /// <param name="samples">The number of samples.</param>
        /// 
        public static double[] Products(double[][] inputs, int samples)
        {
            int[] idx = Accord.Statistics.Tools.RandomSample(inputs.Length, samples);
            int[] idy = Accord.Statistics.Tools.RandomSample(inputs.Length, samples);

            double[] products = new double[samples * samples];

            for (int i = 0; i < idx.Length; i++)
            {
                double[] x = inputs[idx[i]];

                for (int j = 0; j < idy.Length; j++)
                {
                    double[] y = inputs[idy[j]];

                    double sum = 0;
                    for (int k = 0; k < x.Length; k++)
                        sum = x[k] * y[k];

                    products[i * samples + j] = sum;
                }
            }

            Array.Sort(products);

            return products;
        }

        /// <summary>
        ///   Constructs a Sigmoid kernel.
        /// </summary>
        /// 
        public Sigmoid()
            : this(0.01, -Math.E) { }

        /// <summary>
        ///   Constructs a Sigmoid kernel.
        /// </summary>
        /// 
        /// <param name="alpha">
        ///   Alpha parameter. Typically should be set to
        ///   a small positive value. Default is 0.01.</param>
        /// <param name="constant">
        ///   Constant parameter. Typically should be set to
        ///   a negative value. Default is -e (Euler's constant).</param>
        /// 
        public Sigmoid(double alpha, double constant)
        {
            this.alpha = alpha;
            this.constant = constant;
        }

        /// <summary>
        ///   Gets or sets the kernel's alpha parameter.
        /// </summary>
        /// 
        /// <remarks>
        ///   In a sigmoid kernel, alpha is a inner product
        ///   coefficient for the hyperbolic tangent function.
        /// </remarks>
        /// 
        public double Alpha
        {
            get { return alpha; }
            set { alpha = value; }
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
            for (int i = 0; i < x.Length; i++)
                sum += x[i] * y[i];

            double value = Math.Tanh(alpha * sum + constant);

            return value;
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

        void IEstimable.Estimate(double[][] inputs)
        {
            var s = Estimate(inputs);
            this.Alpha = s.Alpha;
            this.Constant = s.Constant;
        }

    }
}
