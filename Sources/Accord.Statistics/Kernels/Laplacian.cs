﻿// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    using Accord.Math.Distances;

    /// <summary>
    ///   Laplacian Kernel.
    /// </summary>
    /// 
    [Serializable]
    public sealed class Laplacian : KernelBase, IKernel, 
        IRadialBasisKernel, IDistance, ICloneable, IEstimable, IReverseDistance
    {
        private double sigma;
        private double gamma;


        /// <summary>
        ///   Constructs a new Laplacian Kernel
        /// </summary>
        /// 
        public Laplacian() 
            : this(1) { }

        /// <summary>
        ///   Constructs a new Laplacian Kernel
        /// </summary>
        /// 
        /// <param name="sigma">The sigma slope value.</param>
        /// 
        public Laplacian(double sigma)
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
        /// 
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public override double Function(double[] x, double[] y)
        {
            // Optimization in case x and y are
            // exactly the same object reference.

            if (x == y) 
                return 1.0;

            double norm = 0.0;
            for (int i = 0; i < x.Length; i++)
            {
                double d = x[i] - y[i];
                norm += d * d;
            }

            norm = Math.Sqrt(norm);

            return Math.Exp(-gamma * norm);
        }

        /// <summary>
        ///   Laplacian Kernel function.
        /// </summary>
        /// 
        /// <param name="z">Distance <c>z</c> in input space.</param>
        /// 
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(double z)
        {
            return Math.Exp(-gamma * z);
        }

        /// <summary>
        ///   Computes the squared distance in input space
        ///   between two points given in feature space.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in feature (kernel) space.</param>
        /// <param name="y">Vector <c>y</c> in feature (kernel) space.</param>
        /// 
        /// <returns>Squared distance between <c>x</c> and <c>y</c> in input space.</returns>
        /// 
        public override double Distance(double[] x, double[] y)
        {
            if (x == y)
                return 0.0;

            double norm = 0.0;
            for (int i = 0; i < x.Length; i++)
            {
                double d = x[i] - y[i];
                norm += d * d;
            }

            norm = Math.Sqrt(norm);

            return 2 - 2 * Math.Exp(-gamma * norm);
        }

        /// <summary>
        ///   Computes the squared distance in input space
        ///   between two points given in feature space.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in feature (kernel) space.</param>
        /// <param name="y">Vector <c>y</c> in feature (kernel) space.</param>
        /// 
        /// <returns>
        ///   Squared distance between <c>x</c> and <c>y</c> in input space.
        /// </returns>
        /// 
        public double ReverseDistance(double[] x, double[] y)
        {
            if (x == y) 
                return 0.0;

            double norm = 0.0;
            for (int i = 0; i < x.Length; i++)
            {
                double d = x[i] - y[i];
                norm += d * d;
            }

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
        public double ReverseDistance(double df)
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
        /// <returns>A Laplacian kernel initialized with an appropriate sigma value.</returns>
        /// 
        public static Laplacian Estimate(double[][] inputs)
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
        /// <param name="samples">The number of random samples to analyze.</param>
        /// <param name="range">The range of suitable values for sigma.</param>
        /// 
        /// <returns>A Laplacian kernel initialized with an appropriate sigma value.</returns>
        /// 
        public static Laplacian Estimate(double[][] inputs, int samples, out DoubleRange range)
        {
            if (samples > inputs.Length)
                throw new ArgumentOutOfRangeException("samples");

            double[] distances = Gaussian.Distances(inputs, samples);

            double q1 = distances[(int)Math.Ceiling(0.15 * distances.Length)];
            double q9 = distances[(int)Math.Ceiling(0.85 * distances.Length)];
            double qm = Measures.Median(distances, alreadySorted: true);

            range = new DoubleRange(q1, q9);

            return new Laplacian(sigma: qm);
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



        void IEstimable<double[]>.Estimate(double[][] inputs)
        {
            var l = Estimate(inputs);
            this.Sigma = l.Sigma;
        }


    }
}
