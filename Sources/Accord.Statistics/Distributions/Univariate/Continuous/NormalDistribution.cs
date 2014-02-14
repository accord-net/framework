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
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Multivariate;
    using AForge;

    /// <summary>
    ///   Normal (Gaussian) distribution.
    /// </summary>
    /// 
    /// <remarks>
    ///   The Gaussian is the most widely used distribution for continuous
    ///   variables. In the case of a single variable, it is governed by
    ///   two parameters, the mean and the variance.
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   This examples shows how to create a Normal distribution,
    ///   compute some of its properties and generate a number of
    ///   random samples from it.</para>
    ///   
    /// <code>
    ///   // Create a normal distribution with mean 2 and sigma 3
    ///   var normal = new NormalDistribution(mean: 2, stdDev: 3);
    /// 
    ///   // In a normal distribution, the median and
    ///   // the mode coincide with the mean, so
    /// 
    ///   double mean = normal.Mean;     // 2
    ///   double mode = normal.Mode;     // 2
    ///   double median = normal.Median; // 2
    /// 
    ///   // The variance is the square of the standard deviation
    ///   double variance = normal.Variance; // 3² = 9
    ///   
    ///   // Let's check what is the cumulative probability of
    ///   // a value less than 3 occurring in this distribution:
    ///   double cdf = normal.DistributionFunction(3); // 0.63055
    /// 
    ///   // Finally, let's generate 1000 samples from this distribution
    ///   // and check if they have the specified mean and standard devs
    /// 
    ///   double[] samples = normal.Generate(1000);
    /// 
    ///   double sampleMean = samples.Mean();             // 1.92
    ///   double sampleDev = samples.StandardDeviation(); // 3.00
    /// </code>
    /// 
    /// <para>
    ///   This example further demonstrates how to compute
    ///   derived measures from a Normal distribution: </para>
    ///   
    /// <code>
    ///   var normal = new NormalDistribution(mean: 4, stdDev: 4.2);
    ///   
    ///   double mean = normal.Mean;     // 4.0
    ///   double median = normal.Median; // 4.0
    ///   double var = normal.Variance;  // 17.64
    ///   
    ///   double cdf = normal.DistributionFunction(x: 1.4);           // 0.26794249453351904
    ///   double pdf = normal.ProbabilityDensityFunction(x: 1.4);     // 0.078423391448155175
    ///   double lpdf = normal.LogProbabilityDensityFunction(x: 1.4); // -2.5456330358182586
    ///   
    ///   double ccdf = normal.ComplementaryDistributionFunction(x: 1.4); // 0.732057505466481
    ///   double icdf = normal.InverseDistributionFunction(p: cdf);       // 1.4
    ///   
    ///   double hf = normal.HazardFunction(x: 1.4);            // 0.10712736480747137
    ///   double chf = normal.CumulativeHazardFunction(x: 1.4); // 0.31189620872601354
    ///   
    ///   string str = normal.ToString(CultureInfo.InvariantCulture); // N(x; μ = 4, σ² = 17.64)
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="Accord.Statistics.Testing.ZTest"/>
    /// <seealso cref="Accord.Statistics.Testing.TTest"/>
    /// 
    [Serializable]
    public class NormalDistribution : UnivariateContinuousDistribution,
        IFittableDistribution<double, NormalOptions>,
        ISampleableDistribution<double>, IFormattable
    {

        // Distribution parameters
        private double mean = 0;   // mean μ
        private double stdDev = 1; // standard deviation σ

        // Distribution measures
        private double? entropy;

        // Derived measures
        private double variance = 1; // σ²
        private double lnconstant;   // log(1/sqrt(2*pi*variance))

        private bool immutable;

        // 97.5 percentile of standard normal distribution
        private const double p95 = 1.95996398454005423552;

        /// <summary>
        ///   Constructs a Normal (Gaussian) distribution
        ///   with zero mean and unit standard deviation.
        /// </summary>
        /// 
        public NormalDistribution()
        {
            initialize(mean, stdDev, stdDev * stdDev);
        }

        /// <summary>
        ///   Constructs a Normal (Gaussian) distribution
        ///   with given mean and unit standard deviation.
        /// </summary>
        /// 
        /// <param name="mean">The distribution's mean value μ (mu).</param>
        /// 
        public NormalDistribution(double mean)
        {
            initialize(mean, stdDev, stdDev * stdDev);
        }

        /// <summary>
        ///   Constructs a Normal (Gaussian) distribution
        ///   with given mean and standard deviation.
        /// </summary>
        /// 
        /// <param name="mean">The distribution's mean value μ (mu).</param>
        /// <param name="stdDev">The distribution's standard deviation σ (sigma).</param>
        /// 
        public NormalDistribution(double mean, double stdDev)
        {
            if (stdDev <= 0)
                throw new ArgumentOutOfRangeException("stdDev", "Standard deviation must be positive.");

            initialize(mean, stdDev, stdDev * stdDev);
        }



        /// <summary>
        ///   Gets the Mean value μ (mu) for 
        ///   this Normal distribution.
        /// </summary>
        /// 
        public override double Mean
        {
            get { return mean; }
        }

        /// <summary>
        ///   Gets the median for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's median value.
        /// </value>
        /// 
        public override double Median
        {
            get
            {
                System.Diagnostics.Debug.Assert(mean == base.Median);
                return mean;
            }
        }

        /// <summary>
        ///   Gets the Variance σ² (sigma-squared), which is the square
        ///   of the standard deviation σ for this Normal distribution.
        /// </summary>
        /// 
        public override double Variance
        {
            get { return variance; }
        }

        /// <summary>
        ///   Gets the Standard Deviation σ (sigma), which is the 
        ///   square root of the variance for this Normal distribution.
        /// </summary>
        /// 
        public override double StandardDeviation
        {
            get { return stdDev; }
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
            get { return new DoubleRange(Double.NegativeInfinity, Double.PositiveInfinity); }
        }

        /// <summary>
        ///   Gets the Entropy for this Normal distribution.
        /// </summary>
        /// 
        public override double Entropy
        {
            get
            {
                if (!entropy.HasValue)
                    entropy = 0.5 * (Math.Log(2.0 * Math.PI * variance) + 1);

                return entropy.Value;
            }
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   the this Normal distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">
        ///   A single point in the distribution range.</param>
        ///   
        /// <remarks>
        /// <para>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.</para>
        /// <para>
        ///  The calculation is computed through the relationship to the error function
        ///  as <see cref="Accord.Math.Special.Erfc">erfc</see>(-z/sqrt(2)) / 2.</para>  
        ///  
        /// <para>    
        ///   References:
        ///   <list type="bullet">
        ///     <item><description>
        ///       Weisstein, Eric W. "Normal Distribution." From MathWorld--A Wolfram Web Resource.
        ///       Available on: http://mathworld.wolfram.com/NormalDistribution.html </description></item>
        ///     <item><description><a href="http://en.wikipedia.org/wiki/Normal_distribution#Cumulative_distribution_function">
        ///       Wikipedia, The Free Encyclopedia. Normal distribution. Available on:
        ///       http://en.wikipedia.org/wiki/Normal_distribution#Cumulative_distribution_function </a></description></item>
        ///   </list></para>
        /// </remarks>
        /// 
        /// <example>
        ///   See <see cref="NormalDistribution"/>.
        /// </example>
        /// 
        public override double DistributionFunction(double x)
        {
            double z = (x - mean) / stdDev;
            double cdf = Special.Erfc(-z / Constants.Sqrt2) * 0.5;

            /*
                // For a normal distribution with zero variance, the cdf is the Heaviside
                // step function (Wikipedia, http://en.wikipedia.org/wiki/Normal_distribution)
                return (x >= mean) ? 1.0 : 0.0;
            */

            return cdf;
        }


        /// <summary>
        ///   Gets the inverse of the cumulative distribution function (icdf) for
        ///   this distribution evaluated at probability <c>p</c>. This function
        ///   is also known as the Quantile function.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        ///   The Inverse Cumulative Distribution Function (ICDF) specifies, for
        ///   a given probability, the value which the random variable will be at,
        ///   or below, with that probability.</para>
        /// <para>
        ///   The Normal distribution's ICDF is defined in terms of the
        ///   <see cref="Normal.Inverse">standard normal inverse cumulative
        ///   distribution function I</see> as <c>ICDF(p) = μ + σ * I(p)</c>.
        /// </para>
        /// </remarks>
        ///
        /// <example>
        ///   See <see cref="NormalDistribution"/>.
        /// </example>
        ///
        public override double InverseDistributionFunction(double p)
        {
            double icdf = mean + stdDev * Normal.Inverse(p);

            System.Diagnostics.Debug.Assert(icdf.IsRelativelyEqual(base.InverseDistributionFunction(p), 1e-6));

            return icdf;
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   the Normal distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range. For a
        ///   univariate distribution, this should be a single
        ///   double value. For a multivariate distribution,
        ///   this should be a double array.</param>
        ///   
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        /// <para>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.</para>
        /// <para>
        ///   The Normal distribution's PDF is defined as
        ///   <c>PDF(x) = c * exp((x - μ / σ)²/2)</c>.</para>
        /// </remarks>
        /// 
        /// <example>
        ///   See <see cref="NormalDistribution"/>.
        /// </example> 
        ///
        public override double ProbabilityDensityFunction(double x)
        {
            double z = (x - mean) / stdDev;
            double lnp = lnconstant - z * z * 0.5;

            return Math.Exp(lnp);

            /*
                 // In the case the variance is zero, return the weak limit function 
                 // of the sequence of Gaussian functions with variance towards zero.

                 // In this case, the pdf can be seen as a Dirac delta function
                 // (Wikipedia, http://en.wikipedia.org/wiki/Dirac_delta_function).

                 return (x == mean) ? Double.PositiveInfinity : 0.0;
             */
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   the Normal distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range. For a
        ///   univariate distribution, this should be a single
        ///   double value. For a multivariate distribution,
        ///   this should be a double array.</param>
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
        ///   See <see cref="NormalDistribution"/>.
        /// </example>
        /// 
        public override double LogProbabilityDensityFunction(double x)
        {
            double z = (x - mean) / stdDev;
            double lnp = lnconstant - z * z * 0.5;

            return lnp;
        }

        /// <summary>
        ///   Gets the Z-Score for a given value.
        /// </summary>
        /// 
        public double ZScore(double x)
        {
            return (x - mean) / stdDev;
        }



        /// <summary>
        ///   Gets the Standard Gaussian Distribution,
        ///   with zero mean and unit variance.
        /// </summary>
        /// 
        public static NormalDistribution Standard { get { return standard; } }

        private static readonly NormalDistribution standard = new NormalDistribution() { immutable = true };

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
        public override void Fit(double[] observations, double[] weights, IFittingOptions options)
        {
            NormalOptions normalOptions = options as NormalOptions;
            if (options != null && normalOptions == null)
                throw new ArgumentException("The specified options' type is invalid.", "options");

            Fit(observations, weights, normalOptions);
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
        public void Fit(double[] observations, double[] weights, NormalOptions options)
        {
            if (immutable) throw new InvalidOperationException();

            double mu, var;

            if (weights != null)
            {
#if DEBUG
                for (int i = 0; i < weights.Length; i++)
                    if (Double.IsNaN(weights[i]) || Double.IsInfinity(weights[i]))
                        throw new ArgumentException("Invalid numbers in the weight vector.", "weights");
#endif

                // Compute weighted mean
                mu = Statistics.Tools.WeightedMean(observations, weights);

                // Compute weighted variance
                var = Statistics.Tools.WeightedVariance(observations, weights, mu);
            }
            else
            {
                // Compute weighted mean
                mu = Statistics.Tools.Mean(observations);

                // Compute weighted variance
                var = Statistics.Tools.Variance(observations, mu);
            }

            if (options != null)
            {
                // Parse optional estimation options
                double regularization = options.Regularization;

                if (var == 0 || Double.IsNaN(var) || Double.IsInfinity(var))
                    var = regularization;
            }

            if (var <= 0)
            {
                throw new ArgumentException("Variance is zero. Try specifying "
                    + "a regularization constant in the fitting options.");
            }

            initialize(mu, Math.Sqrt(var), var);
        }


        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public override object Clone()
        {
            return new NormalDistribution(mean, stdDev);
        }

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
            return String.Format("N(x; μ = {0}, σ² = {1})", mean, variance);
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
            return String.Format(formatProvider, "N(x; μ = {0}, σ² = {1})", mean, variance);
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
            return String.Format(formatProvider, "N(x; μ = {0}, σ² = {1})",
                mean.ToString(format, formatProvider),
                variance.ToString(format, formatProvider));
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
            return String.Format("N(x; μ = {0}, σ² = {1})",
                mean.ToString(format), variance.ToString(format));
        }


        private void initialize(double mu, double dev, double var)
        {
            this.mean = mu;
            this.stdDev = dev;
            this.variance = var;

            // Compute derived values
            this.lnconstant = -Math.Log(Constants.Sqrt2PI * dev);
        }


        /// <summary>
        ///   Estimates a new Normal distribution from a given set of observations.
        /// </summary>
        /// 
        public static NormalDistribution Estimate(double[] observations)
        {
            return Estimate(observations, null, null);
        }

        /// <summary>
        ///   Estimates a new Normal distribution from a given set of observations.
        /// </summary>
        /// 
        public static NormalDistribution Estimate(double[] observations, NormalOptions options)
        {
            return Estimate(observations, null, options);
        }

        /// <summary>
        ///   Estimates a new Normal distribution from a given set of observations.
        /// </summary>
        /// 
        public static NormalDistribution Estimate(double[] observations, double[] weights, NormalOptions options)
        {
            NormalDistribution n = new NormalDistribution();
            n.Fit(observations, weights, options);
            return n;
        }


        /// <summary>
        ///   Converts this univariate distribution into a
        ///   1-dimensional multivariate distribution.
        /// </summary>
        /// 
        public MultivariateNormalDistribution ToMultivariateDistribution()
        {
            return new MultivariateNormalDistribution(
                new double[] { mean }, new double[,] { { variance } });
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
            double[] r = new double[samples];

            var g = new AForge.Math.Random.GaussianGenerator(
                (float)mean, (float)stdDev, Accord.Math.Tools.Random.Next());

            for (int i = 0; i < r.Length; i++)
                r[i] = g.Next();

            return r;
        }

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public double Generate()
        {
            var g = new AForge.Math.Random.GaussianGenerator(
                (float)mean, (float)stdDev, Accord.Math.Tools.Random.Next());

            return g.Next();
        }

    }
}
