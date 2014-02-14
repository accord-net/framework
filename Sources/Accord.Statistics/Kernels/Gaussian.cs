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
    using AForge;

    /// <summary>
    ///   Gaussian Kernel.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Gaussian kernel requires tuning for the proper value of σ. Different approaches
    ///   to this problem includes the use of brute force (i.e. using a grid-search algorithm)
    ///   or a gradient ascent optimization.</para>
    ///    
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///      P. F. Evangelista, M. J. Embrechts, and B. K. Szymanski. Some Properties of the
    ///      Gaussian Kernel for One Class Learning. Available on: http://www.cs.rpi.edu/~szymansk/papers/icann07.pdf </description></item>
    ///    </list></para>
    /// </remarks>
    /// 
    [Serializable]
    public sealed class Gaussian : IKernel, IDistance, IEstimable, ICloneable
    {
        private double sigma;
        private double gamma;


        /// <summary>
        ///   Constructs a new Gaussian Kernel
        /// </summary>
        /// 
        public Gaussian() : this(1) { }

        /// <summary>
        ///   Constructs a new Gaussian Kernel
        /// </summary>
        /// 
        /// <param name="sigma">The standard deviation for the Gaussian distribution.</param>
        /// 
        public Gaussian(double sigma)
        {
            this.Sigma = sigma;
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
        public double Function(double[] x, double[] y)
        {
            // Optimization in case x and y are
            // exactly the same object reference.
            if (x == y) return 1.0;

            double norm = 0.0, d;
            for (int i = 0; i < x.Length; i++)
            {
                d = x[i] - y[i];
                norm += d * d;
            }

            return Math.Exp(-gamma * norm);
        }

        /// <summary>
        ///   Computes the distance in input space
        ///   between two points given in feature space.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in feature (kernel) space.</param>
        /// <param name="y">Vector <c>y</c> in feature (kernel) space.</param>
        /// <returns>Distance between <c>x</c> and <c>y</c> in input space.</returns>
        /// 
        public double Distance(double[] x, double[] y)
        {
            if (x == y) return 0.0;

            double norm = 0.0, d;
            for (int i = 0; i < x.Length; i++)
            {
                d = x[i] - y[i];
                norm += d * d;
            }

            // TODO: Verify the use of log1p instead
            return (1.0 / -gamma) * Math.Log(1.0 - 0.5 * norm);
        }

        /// <summary>
        ///   Computes the distance in input space given
        ///   a distance computed in feature space.
        /// </summary>
        /// 
        /// <param name="df">Distance in feature space.</param>
        /// <returns>Distance in input space.</returns>
        /// 
        public double Distance(double df)
        {
            return (1.0 / -gamma) * Math.Log(1.0 - 0.5 * df);
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
        /// 
        /// <returns>A Gaussian kernel initialized with an appropriate sigma value.</returns>
        /// 
        public static Gaussian Estimate(double[][] inputs)
        {
            DoubleRange range;
            return Estimate(inputs, inputs.Length, out range);
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
        /// <param name="range">The range of suitable values for sigma.</param>
        /// 
        /// <returns>A Gaussian kernel initialized with an appropriate sigma value.</returns>
        /// 
        public static Gaussian Estimate(double[][] inputs, out DoubleRange range)
        {
            return Estimate(inputs, inputs.Length, out range);
        }

        /// <summary>
        ///   Estimates appropriate values for sigma given a data set.
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
        /// 
        /// <returns>A Gaussian kernel initialized with an appropriate sigma value.</returns>
        /// 
        public static Gaussian Estimate(double[][] inputs, int samples)
        {
            DoubleRange range;
            return Estimate(inputs, samples, out range);
        }

        /// <summary>
        ///   Estimates appropriate values for sigma given a data set.
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
        /// <returns>A Gaussian kernel initialized with an appropriate sigma value.</returns>
        /// 
        public static Gaussian Estimate(double[][] inputs, int samples, out DoubleRange range)
        {
            if (samples > inputs.Length)
                throw new ArgumentOutOfRangeException("samples");

            double[] distances = Distances(inputs, samples);

            double q1 = Math.Sqrt(distances[(int)Math.Ceiling(0.15 * distances.Length)] / 2.0);
            double q9 = Math.Sqrt(distances[(int)Math.Ceiling(0.85 * distances.Length)] / 2.0);
            double qm = Math.Sqrt(Accord.Statistics.Tools.Median(distances, alreadySorted: true) / 2.0);

            range = new DoubleRange(q1, q9);

            return new Gaussian(sigma: qm);
        }

        /// <summary>
        ///   Computes the set of all distances between 
        ///   all points in a random subset of the data.
        /// </summary>
        /// 
        /// <param name="inputs">The inputs points.</param>
        /// <param name="samples">The number of samples.</param>
        /// 
        public static double[] Distances(double[][] inputs, int samples)
        {
            int[] idx = Accord.Statistics.Tools.RandomSample(inputs.Length, samples);
            int[] idy = Accord.Statistics.Tools.RandomSample(inputs.Length, samples);

            double[] distances = new double[samples * samples];

            for (int i = 0; i < idx.Length; i++)
            {
                double[] x = inputs[idx[i]];

                for (int j = 0; j < idy.Length; j++)
                {
                    double[] y = inputs[idy[j]];

                    double norm = 0.0, d;
                    for (int k = 0; k < x.Length; k++)
                    {
                        d = x[k] - y[k];
                        norm += d * d;
                    }

                    distances[i * samples + j] = norm;
                }
            }

            Array.Sort(distances);

            return distances;
        }



        /// <summary>
        ///   Estimates kernel parameters from the data.
        /// </summary>
        /// 
        /// <param name="inputs">The input data.</param>
        /// 
        void IEstimable.Estimate(double[][] inputs)
        {
            var g = Gaussian.Estimate(inputs);
            this.Gamma = g.Gamma;
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
