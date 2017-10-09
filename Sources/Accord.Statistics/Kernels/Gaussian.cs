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
    using AForge;
    using Accord.Math;
    using Accord.Math.Distances;
    using Accord.Compat;

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
    ///      P. F. Evangelista, M. J. Embrechts, and B. K. Szymanski. Some Properties
    ///      of the Gaussian Kernel for One Class Learning. Available on: 
    ///      http://www.cs.rpi.edu/~szymansk/papers/icann07.pdf </description></item>
    ///    </list></para>
    /// </remarks>
    /// 
    [Serializable]
    public struct Gaussian : IKernel, IRadialBasisKernel,
        IDistance, IEstimable, ICloneable, IReverseDistance,
        IKernel<Sparse<double>>, IEstimable<Sparse<double>>, IDistance<Sparse<double>>
    {
        private double sigma;
        private double gamma;




        /// <summary>
        ///   Constructs a new Gaussian Kernel with a given sigma value. To 
        ///   construct from a gamma value, use the <see cref="FromGamma(double)"/> 
        ///   named constructor instead.
        /// </summary>
        /// 
        /// <param name="sigma">The kernel's sigma parameter.</param>
        /// 
        public Gaussian(double sigma)
        {
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
        public double Function(double[] x, double[] y)
        {
            if (sigma == gamma)
                Sigma = 1.0; // TODO: Remove if using VS 2015/C# 6

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

            return Math.Exp(-gamma * norm);
        }

        /// <summary>
        ///   Gaussian Kernel function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(Sparse<double> x, Sparse<double> y)
        {
            // Optimization in case x and y are
            // exactly the same object reference.

            if (x == y)
                return 1.0;

            double norm = Accord.Math.Distance.SquareEuclidean(x, y);

            return Math.Exp(-gamma * norm);
        }


        /// <summary>
        ///   Gaussian Kernel function.
        /// </summary>
        /// 
        /// <param name="z">Distance <c>z</c> in input space.</param>
        /// 
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(double z)
        {
            if (sigma == gamma)
                Sigma = 1.0; // TODO: Remove if using VS 2015/C# 6

            return Math.Exp(-gamma * z * z);
        }

        /// <summary>
        ///   Computes the squared distance in feature space
        ///   between two points given in input space.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// 
        /// <returns>Squared distance between <c>x</c> and <c>y</c> in feature (kernel) space.</returns>
        /// 
        public double Distance(double[] x, double[] y)
        {
            if (sigma == gamma)
                Sigma = 1.0; // TODO: Remove if using VS 2015/C# 6

            if (x == y)
                return 0.0;

            double norm = 0.0;
            for (int i = 0; i < x.Length; i++)
            {
                double d = x[i] - y[i];
                norm += d * d;
            }

            return 2 - 2 * Math.Exp(-gamma * norm);
        }

        /// <summary>
        ///   Computes the squared distance in feature space
        ///   between two points given in input space.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// 
        /// <returns>Squared distance between <c>x</c> and <c>y</c> in feature (kernel) space.</returns>
        /// 
        public double Distance(Sparse<double> x, Sparse<double> y)
        {
            if (sigma == gamma)
                Sigma = 1.0; // TODO: Remove if using VS 2015/C# 6

            if (x == y)
                return 0.0;

            double norm = Accord.Math.Distance.SquareEuclidean(x, y);

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
            if (sigma == gamma)
                Sigma = 1.0; // TODO: Remove if using VS 2015/C# 6

            if (x == y)
                return 0.0;

            double norm = 0.0;
            for (int i = 0; i < x.Length; i++)
            {
                double d = x[i] - y[i];
                norm += d * d;
            }

            return -(1.0 / gamma) * Math.Log(1.0 - 0.5 * norm);
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
            if (sigma == gamma)
                Sigma = 1.0; // TODO: Remove if using VS 2015/C# 6

            return (1.0 / -gamma) * Math.Log(1.0 - 0.5 * df);
        }



        /// <summary>
        ///   Constructs a new Gaussian Kernel with a given gamma value. To 
        ///   construct from a sigma value, use the <see cref="Gaussian(double)"/> 
        ///   constructor instead.
        /// </summary>
        /// 
        /// <param name="gamma">The kernel's gamma parameter.</param>
        /// 
        public static Gaussian FromGamma(double gamma)
        {
            return new Gaussian() { Gamma = gamma };
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
            return Estimate(inputs, samples, new SquareEuclidean(), out range);
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
        public static Gaussian Estimate(Sparse<double>[] inputs)
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
        public static Gaussian Estimate(Sparse<double>[] inputs, out DoubleRange range)
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
        public static Gaussian Estimate(Sparse<double>[] inputs, int samples)
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
        public static Gaussian Estimate(Sparse<double>[] inputs, int samples, out DoubleRange range)
        {
            return Estimate(inputs, samples, new SquareEuclidean(), out range);
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
        /// <param name="distance">The distance function to be used in the Gaussian kernel. Default is <see cref="SquareEuclidean"/>.</param>
        /// 
        /// <returns>A Gaussian kernel initialized with an appropriate sigma value.</returns>
        /// 
        public static Gaussian Estimate<TInput, TDistance>(TInput[] inputs, TDistance distance)
            where TDistance : IDistance<TInput>
        {
            DoubleRange range;
            return Estimate(inputs, inputs.Length, distance, out range);
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
        /// <param name="distance">The distance function to be used in the Gaussian kernel. Default is <see cref="SquareEuclidean"/>.</param>
        /// 
        /// <returns>A Gaussian kernel initialized with an appropriate sigma value.</returns>
        /// 
        public static Gaussian Estimate<TInput, TDistance>(TInput[] inputs, TDistance distance, out DoubleRange range)
            where TDistance : IDistance<TInput>
        {
            return Estimate(inputs, inputs.Length, distance, out range);
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
        /// <param name="distance">The distance function to be used in the Gaussian kernel. Default is <see cref="SquareEuclidean"/>.</param>
        /// 
        /// <returns>A Gaussian kernel initialized with an appropriate sigma value.</returns>
        /// 
        public static Gaussian Estimate<TInput, TDistance>(TInput[] inputs, int samples, TDistance distance)
            where TDistance : IDistance<TInput>
        {
            DoubleRange range;
            return Estimate(inputs, samples, distance, out range);
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
        /// <param name="distance">The distance function to be used in the Gaussian kernel. Default is <see cref="SquareEuclidean"/>.</param>
        /// 
        /// <returns>A Gaussian kernel initialized with an appropriate sigma value.</returns>
        /// 
        public static Gaussian Estimate<TInput, TDistance>(TInput[] inputs, int samples, TDistance distance, out DoubleRange range)
            where TDistance : IDistance<TInput>
        {
            if (samples > inputs.Length)
                throw new ArgumentOutOfRangeException("samples");

            double[] distances = Distances(inputs, samples, distance);

            double q1 = Math.Sqrt(distances[(int)Math.Ceiling(0.15 * distances.Length)] / 2.0);
            double q9 = Math.Sqrt(distances[(int)Math.Ceiling(0.85 * distances.Length)] / 2.0);
            double qm = Math.Sqrt(Measures.Median(distances, alreadySorted: true) / 2.0);

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
            return Distances<SquareEuclidean, double[]>(inputs, samples, new SquareEuclidean());
        }

        /// <summary>
        ///   Computes the set of all distances between 
        ///   all points in a random subset of the data.
        /// </summary>
        /// 
        /// <param name="inputs">The inputs points.</param>
        /// <param name="samples">The number of samples.</param>
        /// 
        public static double[] Distances(Sparse<double>[] inputs, int samples)
        {
            return Distances<SquareEuclidean, Sparse<double>>(inputs, samples, new SquareEuclidean());
        }

        /// <summary>
        ///   Computes the set of all distances between 
        ///   all points in a random subset of the data.
        /// </summary>
        /// 
        /// <param name="inputs">The inputs points.</param>
        /// <param name="samples">The number of samples.</param>
        /// <param name="distance">The distance function to be used in the Gaussian kernel. Default is <see cref="SquareEuclidean"/>.</param>
        /// 
        public static double[] Distances<TDistance, TInput>(TInput[] inputs, int samples, TDistance distance)
            where TDistance : IDistance<TInput>
        {
            int[] idx = Vector.Sample(samples, inputs.Length);
            int[] idy = Vector.Sample(samples, inputs.Length);

            var distances = new double[samples * samples];
            for (int i = 0; i < idx.Length; i++)
                for (int j = 0; j < idy.Length; j++)
                    distances[i * samples + j] = distance.Distance(inputs[idx[i]], inputs[idy[j]]);

            Array.Sort(distances);

            return distances;
        }



        void IEstimable<double[]>.Estimate(double[][] inputs)
        {
            this.Gamma = Gaussian.Estimate(inputs).Gamma;
        }

        void IEstimable<Sparse<double>>.Estimate(Sparse<double>[] inputs)
        {
            this.Gamma = Gaussian.Estimate(inputs).Gamma;
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




        #region Gaussian<T> static methods (for composite Gaussian kernels)

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
        /// <param name="kernel">The inner kernel.</param>
        /// <param name="inputs">The data set.</param>
        /// 
        /// <returns>A Gaussian kernel initialized with an appropriate sigma value.</returns>
        /// 
        public static Gaussian<T> Estimate<T>(T kernel, double[][] inputs)
            where T : IDistance, ICloneable
        {
            DoubleRange range;
            return Estimate(kernel, inputs, inputs.Length, out range);
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
        /// <param name="kernel">The inner kernel.</param>
        /// <param name="inputs">The data set.</param>
        /// <param name="range">The range of suitable values for sigma.</param>
        /// 
        /// <returns>A Gaussian kernel initialized with an appropriate sigma value.</returns>
        /// 
        public static Gaussian<T> Estimate<T>(T kernel, double[][] inputs, out DoubleRange range)
            where T : IDistance, IKernel, ICloneable
        {
            return Estimate(kernel, inputs, inputs.Length, out range);
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
        /// <param name="kernel">The inner kernel.</param>
        /// <param name="inputs">The data set.</param>
        /// <param name="samples">The number of random samples to analyze.</param>
        /// 
        /// <returns>A Gaussian kernel initialized with an appropriate sigma value.</returns>
        /// 
        public static Gaussian<T> Estimate<T>(T kernel, double[][] inputs, int samples)
            where T : IDistance, IKernel, ICloneable
        {
            DoubleRange range;
            return Estimate(kernel, inputs, samples, out range);
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
        /// <param name="kernel">The inner kernel.</param>
        /// <param name="inputs">The data set.</param>
        /// <param name="samples">The number of random samples to analyze.</param>
        /// <param name="range">The range of suitable values for sigma.</param>
        /// 
        /// <returns>A Gaussian kernel initialized with an appropriate sigma value.</returns>
        /// 
        public static Gaussian<T> Estimate<T>(T kernel, double[][] inputs, int samples, out DoubleRange range)
            where T : IDistance, ICloneable
        {
            if (samples > inputs.Length)
                throw new ArgumentOutOfRangeException("samples");

            double[] distances = kernel.Distances(inputs, samples);

            double q1 = Math.Sqrt(distances[(int)Math.Ceiling(0.15 * distances.Length)] / 2.0);
            double q9 = Math.Sqrt(distances[(int)Math.Ceiling(0.85 * distances.Length)] / 2.0);
            double qm = Math.Sqrt(Measures.Median(distances, alreadySorted: true) / 2.0);

            range = new DoubleRange(q1, q9);

            return new Gaussian<T>(kernel, sigma: qm);
        }

        #endregion




    }
}
