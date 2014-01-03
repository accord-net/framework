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

namespace Accord.Statistics.Distributions.Univariate
{
    using System;
    using Accord.Math;
    using Accord.Statistics.Distributions.Fitting;
    using AForge;

    /// <summary>
    ///   Inverse Gaussian (Normal) Distribution, also known as the Wald distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Inverse Gaussian distribution is a two-parameter family of continuous probability
    ///   distributions with support on (0,∞). As λ tends to infinity, the inverse Gaussian distribution
    ///   becomes more like a normal (Gaussian) distribution. The inverse Gaussian distribution has
    ///   several properties analogous to a Gaussian distribution. The name can be misleading: it is
    ///   an "inverse" only in that, while the Gaussian describes a Brownian Motion's level at a fixed
    ///   time, the inverse Gaussian describes the distribution of the time a Brownian Motion with positive
    ///   drift takes to reach a fixed positive level.</para>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Inverse_Gaussian_distribution">
    ///       Wikipedia, The Free Encyclopedia. Inverse Gaussian distribution. Available on: 
    ///       http://en.wikipedia.org/wiki/Inverse_Gaussian_distribution </a></description></item>
    ///   </list></para> 
    /// </remarks>
    ///
    /// <example>
    /// <code>
    ///   // Create a new inverse Gaussian distribution with μ = 0.42 and λ = 1.2
    ///   var invGaussian = new InverseGaussianDistribution(mean: 0.42, shape: 1.2);
    ///   
    ///   // Common measures
    ///   double mean = invGaussian.Mean;     // 0.42
    ///   double median = invGaussian.Median; // 0.35856861093990083
    ///   double var = invGaussian.Variance;  // 0.061739999999999989
    ///   
    ///   // Cumulative distribution functions
    ///   double cdf = invGaussian.DistributionFunction(x: 0.27);               // 0.30658791274125458
    ///   double ccdf = invGaussian.ComplementaryDistributionFunction(x: 0.27); // 0.69341208725874548
    ///   double icdf = invGaussian.InverseDistributionFunction(p: cdf);        // 0.26999999957543408
    ///   
    ///   // Probability density functions
    ///   double pdf = invGaussian.ProbabilityDensityFunction(x: 0.27);     // 2.3461495925760354
    ///   double lpdf = invGaussian.LogProbabilityDensityFunction(x: 0.27); // 0.85277551314980737
    ///   
    ///   // Hazard (failure rate) functions
    ///   double hf = invGaussian.HazardFunction(x: 0.27);            // 3.383485283406336
    ///   double chf = invGaussian.CumulativeHazardFunction(x: 0.27); // 0.36613081401302111
    ///   
    ///   // String representation
    ///   string str = invGaussian.ToString(CultureInfo.InvariantCulture); // "N^-1(x; μ = 0.42, λ = 1.2)"
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="NormalDistribution"/>
    ///
    [Serializable]
    public class InverseGaussianDistribution : UnivariateContinuousDistribution,
        ISampleableDistribution<double>, IFormattable
    {

        // Distribution parameters
        private double mean;
        private double lambda;


        /// <summary>
        ///   Constructs a new Inverse Gaussian distribution.
        /// </summary>
        /// 
        /// <param name="mean">The mean parameter mu.</param>
        /// <param name="shape">The shape parameter lambda.</param>
        /// 
        public InverseGaussianDistribution(double mean, double shape)
        {
            if (mean <= 0)
                throw new ArgumentOutOfRangeException("mean");

            if (shape <= 0) 
                throw new ArgumentOutOfRangeException("shape");

            init(mean, shape);
        }

        private void init(double mean, double shape)
        {
            this.mean = mean;
            this.lambda = shape;
        }

        /// <summary>
        ///   Gets the shape parameter (lambda)
        ///   for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's lambda value.</value>
        /// 
        public double Shape
        {
            get { return lambda; }
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mean value.</value>
        /// 
        public override double Mean
        {
            get { return mean; }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's variance.</value>
        /// 
        public override double Variance
        {
            get { return (mean * mean * mean) / lambda; }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's entropy.</value>
        /// 
        public override double Entropy
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        ///   Gets the support interval for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   A <see cref="AForge.DoubleRange" /> containing
        ///   the support interval for this distribution.
        /// </value>
        /// 
        public override DoubleRange Support
        {
            get { return new DoubleRange(0, Double.PositiveInfinity); }
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        /// 
        /// <example>
        ///   See <see cref="InverseGaussianDistribution"/>.
        /// </example>
        /// 
        public override double DistributionFunction(double x)
        {
            double sqrt = Math.Sqrt(lambda / x);

            double a = 0.5 * Special.Erfc(sqrt * (mean - x) / (Constants.Sqrt2 * mean));
            double b = 0.5 * Special.Erfc(sqrt * (mean + x) / (Constants.Sqrt2 * mean));
            double c = Math.Exp((2.0 * lambda) / mean);

            return a + b * c;
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        /// <example>
        ///   See <see cref="InverseGaussianDistribution"/>.
        /// </example>
        /// 
        public override double ProbabilityDensityFunction(double x)
        {
            double a = Math.Sqrt(lambda / (2.0 * Math.PI * x * x * x));
            double b = -lambda * ((x - mean) * (x - mean)) / (2.0 * mean * mean * x);

            return a * Math.Exp(b);
        }

        /// <summary>
        ///   Gets the log-probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The logarithm of the probability of <c>x</c>
        ///   occurring in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        /// <example>
        ///   See <see cref="InverseGaussianDistribution"/>.
        /// </example>
        /// 
        public override double LogProbabilityDensityFunction(double x)
        {
            double a = Math.Sqrt(lambda / (2.0 * Math.PI * x * x * x));
            double b = -lambda * ((x - mean) * (x - mean)) / (2.0 * mean * mean * x);

            return Math.Log(a) + b;
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting, such
        ///   as regularization constants and additional parameters.</param>
        ///   
        /// <remarks>
        ///   Although both double[] and double[][] arrays are supported,
        ///   providing a double[] for a multivariate distribution or a
        ///   double[][] for a univariate distribution may have a negative
        ///   impact in performance.
        /// </remarks>
        /// 
        public override void Fit(double[] observations, double[] weights, IFittingOptions options)
        {
            if (options != null)
                throw new ArgumentException("This method does not accept fitting options.");

            double mean;
            double lambda;
            int n = observations.Length;

            if (weights == null)
            {
                mean = observations.Mean();

                double sum = 0;
                for (int i = 0; i < observations.Length; i++)
                    sum += (1.0 / observations[i] - 1.0 / mean);
                lambda = n / sum;
            }
            else
            {
                mean = observations.WeightedMean(observations);

                double sum = 0;
                for (int i = 0; i < observations.Length; i++)
                    sum += weights[i] * (1.0 / observations[i] - 1.0 / mean);
                lambda = 1.0 / sum;
            }

            init(mean, lambda);
        }


        private InverseGaussianDistribution() { }

        /// <summary>
        ///   Estimates a new Normal distribution from a given set of observations.
        /// </summary>
        /// 
        public static InverseGaussianDistribution Estimate(double[] observations)
        {
            return Estimate(observations, null);
        }

        /// <summary>
        ///   Estimates a new Normal distribution from a given set of observations.
        /// </summary>
        /// 
        public static InverseGaussianDistribution Estimate(double[] observations, double[] weights)
        {
            var n = new InverseGaussianDistribution();
            n.Fit(observations, weights, null);
            return n;
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
            return new InverseGaussianDistribution(mean, lambda);
        }

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// 
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public double[] Generate(int samples)
        {
            return Random(mean, lambda, samples);
        }

        #region ISampleableDistribution<double> Members

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public double Generate()
        {
            return Random(mean, lambda);
        }

        /// <summary>
        ///   Generates a random observation from the 
        ///   Inverse Gaussian distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="mean">The mean parameter mu.</param>
        /// <param name="shape">The shape parameter lambda.</param>
        /// 
        /// <returns>A random double value sampled from the specified Uniform distribution.</returns>
        /// 
        public static double Random(double mean, double shape)
        {
            var u = Accord.Math.Tools.Random;
            var g = new AForge.Math.Random.GaussianGenerator(0, 1, u.Next());

            double v = g.Next();
            double y = v * v;
            double x = mean + (mean * mean * y) / (2 * shape) - (mean / (2 * shape)) * Math.Sqrt(4 * mean * shape * y + mean * mean * y * y);

            double t = u.NextDouble();

            if (t <= (mean) / (mean + x))
                return x;

            return (mean * mean) / x;
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Inverse Gaussian distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="mean">The mean parameter mu.</param>
        /// <param name="shape">The shape parameter lambda.</param>
        /// <param name="samples">The number of samples to generate.</param>
        ///
        /// <returns>An array of double values sampled from the specified Uniform distribution.</returns>
        /// 
        public static double[] Random(double mean, double shape, int samples)
        {
            var u = Accord.Math.Tools.Random;
            var g = new AForge.Math.Random.GaussianGenerator(0, 1, u.Next());

            double[] r = new double[samples];
            for (int i = 0; i < r.Length; i++)
            {
                double v = g.Next();
                double y = v * v;
                double x = mean + (mean * mean * y) / (2 * shape) - (mean / (2 * shape)) * Math.Sqrt(4 * mean * shape * y + mean * mean * y * y);

                double t = u.NextDouble();

                if (t <= (mean) / (mean + x))
                    r[i] = x;
                else
                    r[i] = (mean * mean) / x;
            }

            return r;
        }

        #endregion


        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public override string ToString()
        {
            return String.Format("N^-1(x; μ = {0}, λ = {1})", mean, lambda);
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <param name="formatProvider">The format provider.</param>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public string ToString(IFormatProvider formatProvider)
        {
            return String.Format(formatProvider, "N^-1(x; μ = {0}, λ = {1})", mean, lambda);
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format("N^-1(x; μ = {0}, λ = {1})", 
                mean.ToString(format, formatProvider),
                lambda.ToString(format, formatProvider));
        }
    }
}

