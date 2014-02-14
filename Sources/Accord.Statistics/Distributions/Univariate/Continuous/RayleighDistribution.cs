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
    ///   Rayleigh distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In probability theory and statistics, the Rayleigh distribution is a continuous 
    ///   probability distribution. A Rayleigh distribution is often observed when the overall
    ///   magnitude of a vector is related to its directional components. </para>
    ///   
    /// <para>One example where the Rayleigh distribution naturally arises is when wind speed
    ///   is analyzed into its orthogonal 2-dimensional vector components. Assuming that the 
    ///   magnitude of each component is uncorrelated and normally distributed with equal variance,
    ///   then the overall wind speed (vector magnitude) will be characterized by a Rayleigh 
    ///   distribution.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Rayleigh_distribution">
    ///       Wikipedia, The Free Encyclopedia. Rayleigh distribution. Available on: 
    ///       http://en.wikipedia.org/wiki/Rayleigh_distribution </a></description></item>
    ///   </list></para> 
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///   // Create a new Rayleigh's distribution with σ = 0.42
    ///   var rayleigh = new RayleighDistribution(sigma: 0.42);
    ///   
    ///   // Common measures
    ///   double mean = rayleigh.Mean;     // 0.52639193767251
    ///   double median = rayleigh.Median; // 0.49451220943852386
    ///   double var = rayleigh.Variance;  // 0.075711527953380237
    ///   
    ///   // Cumulative distribution functions
    ///   double cdf = rayleigh.DistributionFunction(x: 1.4);               // 0.99613407986052716
    ///   double ccdf = rayleigh.ComplementaryDistributionFunction(x: 1.4); // 0.0038659201394728449
    ///   double icdf = rayleigh.InverseDistributionFunction(p: cdf);       // 1.4000000080222026
    ///    
    ///   // Probability density functions
    ///   double pdf = rayleigh.ProbabilityDensityFunction(x: 1.4);     // 0.030681905868831811
    ///   double lpdf = rayleigh.LogProbabilityDensityFunction(x: 1.4); // -3.4840821835248961
    ///   
    ///   // Hazard (failure rate) functions
    ///   double hf = rayleigh.HazardFunction(x: 1.4); // 7.9365079365078612
    ///   double chf = rayleigh.CumulativeHazardFunction(x: 1.4); // 5.5555555555555456
    ///   
    ///   // String representation
    ///   string str = rayleigh.ToString(CultureInfo.InvariantCulture); // Rayleigh(x; σ = 0.42)
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class RayleighDistribution : UnivariateContinuousDistribution,
        ISampleableDistribution<double>, IFormattable
    {

        // Distribution parameters
        private double sigma; // σ


        /// <summary>
        ///   Creates a new Rayleigh distribution.
        /// </summary>
        /// 
        /// <param name="sigma">The Rayleigh distribution's σ (sigma).</param>
        /// 
        public RayleighDistribution(double sigma)
        {
            this.sigma = sigma;
        }


        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mean value.</value>
        /// 
        /// <remarks>
        ///   The Rayleight's mean value is defined 
        ///   as <c>mean = σ * sqrt(π / 2)</c>.
        /// </remarks>
        /// 
        public override double Mean
        {
            get { return sigma * Math.Sqrt(Math.PI / 2.0); }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's variance.</value>
        /// 
        /// <remarks>
        ///   The Rayleight's variance value is defined 
        ///   as <c>var = (4 - π) / 2 * σ²</c>.
        /// </remarks>
        /// 
        public override double Variance
        {
            get { return (4.0 - Math.PI) / 2.0 * sigma * sigma; }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's entropy.</value>
        /// 
        public override double Entropy
        {
            get { return 1 + Math.Log(sigma / Math.Sqrt(2)) + Constants.EulerGamma / 2.0; ; }
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
        ///   See <see cref="RayleighDistribution"/>.
        /// </example>
        /// 
        public override double DistributionFunction(double x)
        {
            return 1.0 - Math.Exp(-x * x / (2 * sigma * sigma));
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
        ///   See <see cref="RayleighDistribution"/>.
        /// </example>
        ///
        public override double ProbabilityDensityFunction(double x)
        {
            return x / (sigma * sigma) * Math.Exp(-x * x / (2 * sigma * sigma));
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
        ///   See <see cref="RayleighDistribution"/>.
        /// </example>
        /// 
        public override double LogProbabilityDensityFunction(double x)
        {
            return Math.Log(x / (sigma * sigma)) + (-x * x / (2 * sigma * sigma));
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

            if (weights != null)
                throw new ArgumentException("This distribution does not support weighted samples.");

            double sum = 0;
            for (int i = 0; i < observations.Length; i++)
                sum += observations[i] * observations[i];

            sigma = Math.Sqrt(1.0 / (2.0 * observations.Length) * sum);
        }

        private RayleighDistribution() { }

        /// <summary>
        ///   Estimates a new Gamma distribution from a given set of observations.
        /// </summary>
        /// 
        public static RayleighDistribution Estimate(double[] observations)
        {
            var n = new RayleighDistribution();
            n.Fit(observations, null, null);
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
            return new RayleighDistribution(sigma);
        }

        #region ISamplableDistribution<double> Members

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public double[] Generate(int samples)
        {
            return Random(sigma, samples);
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public double Generate()
        {
            return Random(sigma);
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Rayleigh distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="sigma">The Rayleigh distribution's sigma.</param>
        /// <param name="samples">The number of samples to generate.</param>
        ///
        /// <returns>An array of double values sampled from the specified Rayleigh distribution.</returns>
        /// 
        public static double[] Random(double sigma, int samples)
        {
            double[] r = new double[samples];
            for (int i = 0; i < r.Length; i++)
            {
                double u = Accord.Math.Tools.Random.Next();
                r[i] = Math.Sqrt(-2 * sigma * sigma * Math.Log(u));
            }

            return r;
        }

        /// <summary>
        ///   Generates a random observation from the 
        ///   Rayleigh distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="sigma">The Rayleigh distribution's sigma.</param>
        /// 
        /// <returns>A random double value sampled from the specified Rayleigh distribution.</returns>
        /// 
        public static double Random(double sigma)
        {
            double u = Accord.Math.Tools.Random.Next();
            return Math.Sqrt(-2 * sigma * sigma * Math.Log(u));
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
            return String.Format("Rayleigh(x; σ = {0})", sigma);
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public string ToString(IFormatProvider formatProvider)
        {
            return String.Format(formatProvider, "Rayleigh(x; σ = {0})", sigma);
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format(formatProvider, "Rayleigh(x; σ = {0})",
                sigma.ToString(format, formatProvider));
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public string ToString(string format)
        {
            return String.Format("Rayleigh(x; σ = {0})",
                sigma.ToString(format));
        }
    }
}
