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
//
// The methods for generating a sample from the Von-Mises Fisher distribution
// have been based on the code originally written by Yu-Hui Chen, distributed
// under a 2-clause BSD license and made available on:
//
//   - https://github.com/yuhuichen1015/SphericalDistributionsRand
// 
// The original license text is reproduced below:
//
//    Copyright (c) 2015, Yu-Hui Chen
//    All rights reserved.
//    
//    Redistribution and use in source and binary forms, with or without
//    modification, are permitted provided that the following conditions are met:
//    
//    * Redistributions of source code must retain the above copyright notice, this
//      list of conditions and the following disclaimer.
//    
//    * Redistributions in binary form must reproduce the above copyright notice,
//      this list of conditions and the following disclaimer in the documentation
//      and/or other materials provided with the distribution.
//    
//    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//    AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
//    IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//    DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
//    FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
//    DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//    SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
//    CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
//    OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
//    OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//

namespace Accord.Statistics.Distributions.Multivariate
{
    using System;
    using Accord.Math;
    using Accord.Math.Decompositions;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Math.Random;
    using Accord.Compat;

    /// <summary>
    ///   Von-Mises Fisher distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In directional statistics, the von Mises–Fisher distribution is a probability distribution 
    ///   on the (p-1)-dimensional sphere in R^p. If p = 2 the distribution reduces to the <see cref="VonMisesDistribution">
    ///   von Mises distribution on the circle</see>.</para>
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia, The Free Encyclopedia. Von Mises-Fisher Distribution. Available on:
    ///       <a href=" https://en.wikipedia.org/wiki/Von_Mises%E2%80%93Fisher_distribution">
    ///        https://en.wikipedia.org/wiki/Von_Mises%E2%80%93Fisher_distribution </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <seealso cref="VonMisesDistribution"/>
    /// 
    [Serializable]
    public class VonMisesFisherDistribution : MultivariateContinuousDistribution,
        ISampleableDistribution<double[]>
    {

        // Distribution parameters
        private double[] mean; // μ (mu)
        private double kappa;  // κ (kappa)

        private double constant;


        /// <summary>
        ///   Constructs a Von-Mises Fisher distribution with unit mean.
        /// </summary>
        /// 
        /// <param name="dimension">The number of dimensions in the distribution.</param>
        /// <param name="concentration">The concentration value κ (kappa).</param>
        /// 
        public VonMisesFisherDistribution(int dimension, double concentration)
            : base(dimension)
        {
            if (concentration < 0)
                throw new ArgumentOutOfRangeException("concentration", "Concentration parameter kappa must be non-negative.");

            for (int i = 0; i < mean.Length; i++)
                mean[i] = 1 / Math.Sqrt(mean.Length);
        }

        /// <summary>
        ///   Constructs a Von-Mises Fisher distribution with unit mean.
        /// </summary>
        /// 
        /// <param name="mean">The mean direction vector (with unit length).</param>
        /// <param name="concentration">The concentration value κ (kappa).</param>
        /// 
        public VonMisesFisherDistribution(double[] mean, double concentration)
            : base(mean.Length)
        {
            if (concentration < 0)
                throw new ArgumentOutOfRangeException("concentration", "Concentration parameter kappa must be non-negative.");

            if (!Norm.Euclidean(mean).IsEqual(1, 1e-10))
                throw new ArgumentOutOfRangeException("mean", "The mean vector must have unit length.");

            this.mean = mean;
            this.kappa = concentration;

            int p = Dimension;
            double num = Math.Pow(concentration, p / 2 - 1);
            double den = Math.Pow(2 * Math.PI, p / 2) * Bessel.I(p / 2 - 1, concentration);
            this.constant = num / den;
        }


        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   A vector containing the mean values for the distribution.
        /// </value>
        /// 
        public override double[] Mean
        {
            get { return mean; }
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public override double[] Variance
        {
            get { return null; }
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public override double[,] Covariance
        {
            get { return null; }
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        protected internal override double InnerDistributionFunction(params double[] x)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        ///   Gets the probability density function (pdf) for this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">
        ///   A single point in the distribution range. For a univariate distribution, this should be
        ///   a  single double value. For a multivariate distribution, this should be a double array.
        /// </param>
        ///   
        /// <returns>
        ///   The probability of <c>x</c> occurring in the current distribution.
        /// </returns>
        /// 
        /// <exception cref="DimensionMismatchException">x;The vector should have the same dimension as the distribution.</exception>
        /// 
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        protected internal override double InnerProbabilityDensityFunction(params double[] x)
        {
            if (x.Length != Dimension)
                throw new DimensionMismatchException("x", "The vector should have the same dimension as the distribution.");

            double[] z = x.Normalize(Norm.Euclidean);
            double d = mean.Dot(z);
            return constant * Math.Exp(kappa * d);
        }


        private VonMisesFisherDistribution(int dimension)
            : base(dimension)
        {
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public override object Clone()
        {
            var clone = new VonMisesFisherDistribution(this.Dimension);
            clone.constant = constant;
            clone.mean = (double[])mean.Clone();
            clone.kappa = kappa;

            return clone;
        }


        /// <summary>
        ///   Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// 
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// 
        /// <returns>
        ///   A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// 
        public override string ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format(formatProvider, "VonMises-Fisher(X; μ, κ = {0})", kappa);
        }

        VonMisesFisherDistribution Uniform(double[] mean)
        {
            return new VonMisesFisherDistribution(mean, concentration: 0);
        }

        private double density(double x)
        {
            // Based on the function randVMFMeanDir, available in the
            // SphericalDistributionsRand repository under BSD license,
            // originally written by Yu-Hui Chen,  University of Michigan.
            // https://github.com/yuhuichen1015/SphericalDistributionsRand

            int p = Dimension;
            double g1 = Gamma.Function((p - 1.0) / 2.0);
            double g2 = Gamma.Function(1.0 / 2.0);
            double bi = Bessel.I(p / 2 - 1, kappa);
            double num = Math.Pow(kappa / 2.0, p / 2.0 - 1.0);
            double den = g1 * g2 * bi;
            double c = num / den;

            double a = Math.Exp(kappa * x);
            double b = Math.Pow(1.0 - x * x, (p - 3.0) / 2.0);
            double y = c * a * b;
            return y;
        }

        private double[] randomDirection(int samples, double k, int dimensions, Random source)
        {
            // Based on the function randVMFMeanDir, available in the
            // SphericalDistributionsRand repository under BSD license,
            // originally written by Yu-Hui Chen,  University of Michigan.
            // https://github.com/yuhuichen1015/SphericalDistributionsRand

            double N = samples;
            double min_thresh = 1.0 / (5.0 * N);

            double[] xx = Vector.Range(-1.0, 1.0, 0.000001);
            double[] yy = xx.Apply(density);
            double[] cumyy = yy.CumulativeSum().Multiply(xx[1] - xx[0]);

            double leftBound = xx.Get(cumyy.Find(x => x > min_thresh)[0]);

            // Fin the left bound
            xx = Vector.Interval(leftBound, 1, 1000);
            yy = xx.Apply(density);

            double M = yy.Max();
            double[] t = new double[samples];

            for (int i = 0; i < N; i++)
            {
                while (true)
                {
                    double u1 = source.NextDouble();
                    double u2 = source.NextDouble();

                    double x = u1 * (1 - leftBound) + leftBound;
                    double h = density(x);

                    double draw = u2 * M;
                    if (draw <= h)
                    {
                        t[i] = x;
                        break;
                    }
                }
            }

            return t;
        }

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        /// 
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public override double[][] Generate(int samples, double[][] result, Random source)
        {
            if (mean.Length <= 2)
                throw new InvalidOperationException();

            // Based on the function randVMF, available in the
            // SphericalDistributionsRand repository under BSD license,
            // originally written by Yu-Hui Chen,  University of Michigan.
            // https://github.com/yuhuichen1015/SphericalDistributionsRand

            int p = mean.Length;
            double[] tmpMu = Vector.Create(p, new[] { 1.0 });

            double[] t = randomDirection(samples, kappa, p, source);

            double[][] RandSphere = UniformSphereDistribution.Random(samples, p - 1, source);

            for (int i = 0; i < samples; i++)
                for (int j = 0; j < tmpMu.Length; j++)
                    result[i][j] = t[i] * tmpMu[j];

            for (int i = 0; i < samples; i++)
                for (int j = 0; j < RandSphere[i].Length; j++)
                    result[i][j + 1] += Math.Sqrt(1 - t[i] * t[i]) * RandSphere[i][j];

            // Rotate the distribution to the right direction
            double[][] Otho = Matrix.Null(mean);
            double[][] Rot = Otho.InsertColumn(mean, index: 0);

            result = Rot.DotWithTransposed(result).Transpose();
            return result;
        }
    }
}
