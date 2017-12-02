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

namespace Accord.Statistics.Distributions.Univariate
{
    using System;
    using Accord.Math;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Multivariate;
    using AForge;
    using Accord.Compat;

    /// <summary>
    ///   Normal (Gaussian) distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In probability theory, the normal (or Gaussian) distribution is a very 
    ///   commonly occurring continuous probability distribution—a function that
    ///   tells the probability that any real observation will fall between any two
    ///   real limits or real numbers, as the curve approaches zero on either side.
    ///   Normal distributions are extremely important in statistics and are often 
    ///   used in the natural and social sciences for real-valued random variables
    ///   whose distributions are not known.</para>
    /// <para>
    ///   The normal distribution is immensely useful because of the central limit 
    ///   theorem, which states that, under mild conditions, the mean of many random 
    ///   variables independently drawn from the same distribution is distributed 
    ///   approximately normally, irrespective of the form of the original distribution:
    ///   physical quantities that are expected to be the sum of many independent processes
    ///   (such as measurement errors) often have a distribution very close to the normal.
    ///   Moreover, many results and methods (such as propagation of uncertainty and least
    ///   squares parameter fitting) can be derived analytically in explicit form when the
    ///   relevant variables are normally distributed.</para>
    /// <para>
    ///   The Gaussian distribution is sometimes informally called the bell curve. However,
    ///   many other distributions are bell-shaped (such as Cauchy's, Student's, and logistic).
    ///   The terms Gaussian function and Gaussian bell curve are also ambiguous because they
    ///   sometimes refer to multiples of the normal distribution that cannot be directly
    ///   interpreted in terms of probabilities.</para>
    ///   
    /// <para>
    ///   The Gaussian is the most widely used distribution for continuous
    ///   variables. In the case of a single variable, it is governed by
    ///   two parameters, the mean and the variance.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://en.wikipedia.org/wiki/Normal_distribution">
    ///       Wikipedia, The Free Encyclopedia. Normal distribution. Available on: 
    ///       https://en.wikipedia.org/wiki/Normal_distribution </a></description></item>
    ///   </list></para> 
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
    ///   double mode = normal.Mode;     // 4.0
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
    /// <seealso cref="Accord.Statistics.Distributions.Univariate.SkewNormalDistribution"/>
    /// <seealso cref="Accord.Statistics.Distributions.Multivariate.MultivariateNormalDistribution"/>
    /// 
    /// <seealso cref="Accord.Statistics.Testing.ZTest"/>
    /// <seealso cref="Accord.Statistics.Testing.TTest"/>
    /// 
    [Serializable]
    public class NormalDistribution : UnivariateContinuousDistribution,
        IFittableDistribution<double, NormalOptions>,
        ISampleableDistribution<double>, IFormattable,
        IUnivariateFittableDistribution
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
        public NormalDistribution([Real] double mean)
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
        public NormalDistribution([Real] double mean, [Positive] double stdDev)
        {
            if (stdDev <= 0)
            {
                throw new ArgumentOutOfRangeException("stdDev",
                    "Standard deviation must be positive.");
            }

            initialize(mean, stdDev, stdDev * stdDev);
        }



        /// <summary>
        ///   Gets the Mean value μ (mu) for this Normal distribution.
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
        /// <remarks>
        ///   The normal distribution's median value 
        ///   equals its <see cref="Mean"/> value μ.
        /// </remarks>
        /// 
        /// <value>
        ///   The distribution's median value.
        /// </value>
        /// 
        public override double Median
        {
            get
            {
                Accord.Diagnostics.Debug.Assert(mean.IsEqual(base.Median, 1e-10));
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
        ///   Gets the mode for this distribution.
        /// </summary>
        /// 
        /// <remarks>
        ///   The normal distribution's mode value 
        ///   equals its <see cref="Mean"/> value μ.
        /// </remarks>
        /// 
        /// <value>
        ///   The distribution's mode value.
        /// </value>
        /// 
        public override double Mode
        {
            get { return mean; }
        }

        /// <summary>
        ///   Gets the skewness for this distribution. In 
        ///   the Normal distribution, this is always 0.
        /// </summary>
        /// 
        public double Skewness
        {
            get { return 0; }
        }

        /// <summary>
        ///   Gets the excess kurtosis for this distribution. 
        ///   In the Normal distribution, this is always 0.
        /// </summary>
        /// 
        public double Kurtosis
        {
            get { return 0; }
        }

        /// <summary>
        ///   Gets the support interval for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   A <see cref="DoubleRange" /> containing
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
        protected internal override double InnerDistributionFunction(double x)
        {
            return Normal.Function((x - mean) / stdDev);
        }

        /// <summary>
        ///   Gets the complementary cumulative distribution function
        ///   (ccdf) for this distribution evaluated at point <c>x</c>.
        ///   This function is also known as the Survival function.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        protected internal override double InnerComplementaryDistributionFunction(double x)
        {
            return Normal.Complemented((x - mean) / stdDev);
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
        protected internal override double InnerInverseDistributionFunction(double p)
        {
            double inv = Normal.Inverse(p);

            double icdf = mean + stdDev * inv;

#if DEBUG
            double baseValue;
            if (p < 0.0 || p > 1.0)
                throw new ArgumentOutOfRangeException("p", "Value must be between 0 and 1.");

            if (Double.IsNaN(p))
                throw new ArgumentOutOfRangeException("p", "Value is Not-a-Number (NaN).");

            if (p == 0)
                baseValue = Support.Min;

            if (p == 1)
                baseValue = Support.Max;

            baseValue = base.InnerInverseDistributionFunction(p);
            double r1 = DistributionFunction(baseValue);
            double r2 = DistributionFunction(icdf);

            bool close = r1.IsEqual(r2, 1e-6);

            if (!close)
            {
                throw new Exception();
            }
#endif

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
        protected internal override double InnerProbabilityDensityFunction(double x)
        {
            double z = (x - mean) / stdDev;
            double lnp = lnconstant - z * z * 0.5;

            return Math.Exp(lnp);
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
        protected internal override double InnerLogProbabilityDensityFunction(double x)
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
        ///   Gets the Standard Gaussian Distribution, with zero mean and unit variance.
        /// </summary>
        /// 
        public static NormalDistribution Standard
        {
            get { return standard; }
        }

        private static readonly NormalDistribution standard = new NormalDistribution()
        {
            immutable = true
        };

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
            if (immutable)
                throw new InvalidOperationException("NormalDistribution.Standard is immutable.");

            double mu, var;

            if (weights != null)
            {
#if DEBUG
                for (int i = 0; i < weights.Length; i++)
                    if (Double.IsNaN(weights[i]) || Double.IsInfinity(weights[i]))
                        throw new ArgumentException("Invalid numbers in the weight vector.", "weights");
#endif

                // Compute weighted mean
                mu = Measures.WeightedMean(observations, weights);

                // Compute weighted variance
                var = Measures.WeightedVariance(observations, weights, mu);
            }
            else
            {
                // Compute weighted mean
                mu = Measures.Mean(observations);

                // Compute weighted variance
                var = Measures.Variance(observations, mu);
            }

            if (options != null)
            {
                if (options.Robust)
                {
                    initialize(mu, Math.Sqrt(var), var);
                    return;
                }

                // Parse optional estimation options
                double regularization = options.Regularization;

                if (var == 0 || Double.IsNaN(var) || Double.IsInfinity(var))
                    var = regularization;
            }

            if (Double.IsNaN(var) || var <= 0)
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
        public override string ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format(formatProvider, "N(x; μ = {0}, σ² = {1})",
                mean.ToString(format, formatProvider),
                variance.ToString(format, formatProvider));
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
        /// <param name="result">The location where to store the samples.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        /// 
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public override double[] Generate(int samples, double[] result, Random source)
        {
            return Random(mean, stdDev, samples, result, source);
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        ///
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        ///   
        /// <returns>A observation drawn from this distribution.</returns>
        /// 
        public override double Generate(Random source)
        {
            return Random(mean, stdDev, source);
        }

        /// <summary>
        ///   Generates a single random observation from the 
        ///   Normal distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="mean">The mean value μ (mu).</param>
        /// <param name="stdDev">The standard deviation σ (sigma).</param>
        ///
        /// <returns>An double value sampled from the specified Normal distribution.</returns>
        /// 
        public static double Random(double mean, double stdDev)
        {
            return Random(mean, stdDev, Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        ///   Generates a single random observation from the 
        ///   Normal distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="mean">The mean value μ (mu).</param>
        /// <param name="stdDev">The standard deviation σ (sigma).</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        ///
        /// <returns>An double value sampled from the specified Normal distribution.</returns>
        /// 
        public static double Random(double mean, double stdDev, Random source)
        {
            return Random(source) * stdDev + mean;
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Normal distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="mean">The mean value μ (mu).</param>
        /// <param name="stdDev">The standard deviation σ (sigma).</param>
        /// <param name="samples">The number of samples to generate.</param>
        ///
        /// <returns>An array of double values sampled from the specified Normal distribution.</returns>
        /// 
        public static double[] Random(double mean, double stdDev, int samples)
        {
            return Random(mean, stdDev, samples, new double[samples], Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Normal distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="mean">The mean value μ (mu).</param>
        /// <param name="stdDev">The standard deviation σ (sigma).</param>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        ///   
        /// <returns>An array of double values sampled from the specified Normal distribution.</returns>
        /// 
        public static double[] Random(double mean, double stdDev, int samples, Random source)
        {
            return Random(mean, stdDev, samples, new double[samples], source);
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Normal distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="mean">The mean value μ (mu).</param>
        /// <param name="stdDev">The standard deviation σ (sigma).</param>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        ///
        /// <returns>An array of double values sampled from the specified Normal distribution.</returns>
        /// 
        public static double[] Random(double mean, double stdDev, int samples, double[] result)
        {
            return Random(mean, stdDev, samples, new double[samples], Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Normal distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="mean">The mean value μ (mu).</param>
        /// <param name="stdDev">The standard deviation σ (sigma).</param>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        ///
        /// <returns>An array of double values sampled from the specified Normal distribution.</returns>
        /// 
        public static double[] Random(double mean, double stdDev, int samples, double[] result, Random source)
        {
            Random(samples, result, source);
            for (int i = 0; i < samples; i++)
                result[i] = result[i] * stdDev + mean;
            return result;
        }



        [ThreadStatic]
        private static bool useSecond = false;

        [ThreadStatic]
        private static double secondValue = 0;

        /// <summary>
        ///   Generates a random vector of observations from the standard
        ///   Normal distribution (zero mean and unit standard deviation).
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        ///
        /// <returns>An array of double values sampled from the specified Normal distribution.</returns>
        /// 
        public static double[] Random(int samples, double[] result)
        {
            return Random(samples, result, Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        ///   Generates a random vector of observations from the standard
        ///   Normal distribution (zero mean and unit standard deviation).
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        ///
        /// <returns>An array of double values sampled from the specified Normal distribution.</returns>
        /// 
        public static double[] Random(int samples, double[] result, Random source)
        {
            bool useSecond = NormalDistribution.useSecond;
            double secondValue = NormalDistribution.secondValue;

            for (int i = 0; i < samples; i++)
            {
                // check if we can use second value
                if (useSecond)
                {
                    // return the second number
                    useSecond = false;
                    result[i] = secondValue;
                    continue;
                }

                // Polar form of the Box-Muller transformation
                // http://www.design.caltech.edu/erik/Misc/Gaussian.html

                double x1, x2, w, firstValue;

                // generate new numbers
                do
                {
                    x1 = source.NextDouble() * 2.0 - 1.0;
                    x2 = source.NextDouble() * 2.0 - 1.0;
                    w = x1 * x1 + x2 * x2;
                }
                while (w >= 1.0);

                w = Math.Sqrt((-2.0 * Math.Log(w)) / w);

                // get two standard random numbers
                firstValue = x1 * w;
                secondValue = x2 * w;

                useSecond = true;

                // return the first number
                result[i] = firstValue;
            }

            NormalDistribution.useSecond = useSecond;
            NormalDistribution.secondValue = secondValue;

            return result;
        }

        /// <summary>
        ///   Generates a random value from a standard Normal 
        ///   distribution (zero mean and unit standard deviation).
        /// </summary>
        /// 
        public static double Random()
        {
            return Random(Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        ///   Generates a random value from a standard Normal 
        ///   distribution (zero mean and unit standard deviation).
        /// </summary>
        /// 
        public static double Random(Random source)
        {
            // check if we can use second value
            if (useSecond)
            {
                // return the second number
                useSecond = false;
                return secondValue;
            }

            double x1, x2, w, firstValue;

            // generate new numbers
            do
            {
                x1 = source.NextDouble() * 2.0 - 1.0;
                x2 = source.NextDouble() * 2.0 - 1.0;
                w = x1 * x1 + x2 * x2;
            }
            while (w >= 1.0);

            w = Math.Sqrt((-2.0 * Math.Log(w)) / w);

            // get two standard random numbers
            firstValue = x1 * w;
            secondValue = x2 * w;

            useSecond = true;

            // return the first number
            return firstValue;
        }
    }
}
