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
    using AForge;
    using Accord.Statistics.Distributions.Fitting;

    /// <summary>
    ///   Empirical distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Empirical distributions are based solely on the data. This class
    ///   uses the empirical distribution function and the Gaussian kernel
    ///   density estimation to provide an univariate continuous distribution
    ///   implementation which depends only on sampled data.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia, The Free Encyclopedia. Empirical Distribution Function. Available on:
    ///       <a href=" http://en.wikipedia.org/wiki/Empirical_distribution_function">
    ///        http://en.wikipedia.org/wiki/Empirical_distribution_function </a></description></item>
    ///     <item><description>
    ///       PlanetMath. Empirical Distribution Function. Available on:
    ///       <a href="http://planetmath.org/encyclopedia/EmpiricalDistributionFunction.html">
    ///       http://planetmath.org/encyclopedia/EmpiricalDistributionFunction.html </a></description></item>
    ///     <item><description>
    ///       Wikipedia, The Free Encyclopedia. Kernel Density Estimation. Available on:
    ///       <a href="http://en.wikipedia.org/wiki/Kernel_density_estimation">
    ///       http://en.wikipedia.org/wiki/Kernel_density_estimation </a></description></item>
    ///     <item><description>
    ///       Bishop, Christopher M.; Pattern Recognition and Machine Learning. 
    ///       Springer; 1st ed. 2006.</description></item>
    ///  </list></para>  
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example shows how to build an empirical distribution directly from a sample: </para>
    ///   
    /// <code>
    ///   // Consider the following univariate samples
    ///   double[] samples = { 5, 5, 1, 4, 1, 2, 2, 3, 3, 3, 4, 3, 3, 3, 4, 3, 2, 3 };
    ///   
    ///   // Create a non-parametric, empirical distribution using those samples:
    ///   EmpiricalDistribution distribution = new EmpiricalDistribution(samples);
    ///     
    ///   // Common measures
    ///   double mean   = distribution.Mean;     // 3
    ///   double median = distribution.Median;   // 2.9999993064186787
    ///   double var    = distribution.Variance; // 1.2941176470588236
    ///   
    ///   // Cumulative distribution function
    ///   double cdf  = distribution.DistributionFunction(x: 4.2);          // 0.88888888888888884
    ///   double ccdf = distribution.ComplementaryDistributionFunction(x: 4.2); //0.11111111111111116
    ///   double icdf = distribution.InverseDistributionFunction(p: cdf);       // 4.1999999999999993
    ///   
    ///   // Probability density functions
    ///   double pdf  = distribution.ProbabilityDensityFunction(x: 4.2);    // 0.15552784414141974
    ///   double lpdf = distribution.LogProbabilityDensityFunction(x: 4.2); // -1.8609305013898356
    ///   
    ///   // Hazard (failure rate) functions
    ///   double hf  = distribution.HazardFunction(x: 4.2);           // 1.3997505972727771
    ///   double chf = distribution.CumulativeHazardFunction(x: 4.2); // 2.1972245773362191
    ///
    ///   // Automatically estimated smooth parameter (gamma)
    ///   double smoothing = distribution.Smoothing; // 1.9144923416414432
    /// 
    ///   // String representation
    ///   string str = distribution.ToString(CultureInfo.InvariantCulture); // Fn(x; S)
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="EmpiricalHazardDistribution"/>
    /// 
    [Serializable]
    public class EmpiricalDistribution : UnivariateContinuousDistribution,
        IFittableDistribution<double, EmpiricalOptions>
    {

        // Distribution parameters
        double[] samples;
        double smoothing;

        // Derived measures
        double? mean;
        double? variance;
        double? entropy;


        /// <summary>
        ///   Creates a new Empirical Distribution from the data samples.
        /// </summary>
        /// 
        /// <param name="samples">The data samples.</param>
        /// 
        public EmpiricalDistribution(double[] samples)
        {
            initialize(samples, null);
        }

        /// <summary>
        ///   Creates a new Empirical Distribution from the data samples.
        /// </summary>
        /// 
        /// <param name="samples">The data samples.</param>
        /// <param name="smoothing">
        ///   The kernel smoothing or bandwidth to be used in density estimation.
        ///   By default, the normal distribution approximation will be used.</param>
        /// 
        public EmpiricalDistribution(double[] samples, double smoothing)
        {
            initialize(samples, smoothing);
        }

        /// <summary>
        ///   Gets the samples giving this empirical distribution.
        /// </summary>
        /// 
        public double[] Samples
        {
            get { return samples; }
        }

        /// <summary>
        ///   Gets the bandwidth smoothing parameter
        ///   used in the kernel density estimation.
        /// </summary>
        /// 
        public double Smoothing
        {
            get { return smoothing; }
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <example>
        ///   See <see cref="EmpiricalDistribution"/>.
        /// </example>
        /// 
        public override double Mean
        {
            get
            {
                if (mean == null)
                    mean = Statistics.Tools.Mean(samples);
                return mean.Value;
            }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <example>
        ///   See <see cref="EmpiricalDistribution"/>.
        /// </example>
        /// 
        public override double Variance
        {
            get
            {
                if (variance == null)
                    variance = Statistics.Tools.Variance(samples, Mean);
                return variance.Value;
            }
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
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        public override double Entropy
        {
            get
            {
                if (!entropy.HasValue)
                {
                    double h = 0;
                    for (int i = 0; i < samples.Length; i++)
                    {
                        double p = ProbabilityDensityFunction(samples[i]);
                        h += p * Math.Log(p);
                    }

                    this.entropy = h;
                }
                return entropy.Value;
            }
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
        ///   See <see cref="EmpiricalDistribution"/>.
        /// </example>
        /// 
        public override double DistributionFunction(double x)
        {
            int sum = 0;

            for (int i = 0; i < samples.Length; i++)
            {
                if (samples[i] <= x)
                    sum++;
            }

            return sum / (double)samples.Length;
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
        ///   See <see cref="EmpiricalDistribution"/>.
        /// </example>
        /// 
        public override double ProbabilityDensityFunction(double x)
        {
            // References:
            //  - Bishop, Christopher M.; Pattern Recognition and Machine Learning. 

            double p = 0;

            for (int i = 0; i < samples.Length; i++)
            {
                double z = (x - samples[i]) / smoothing;
                p += Math.Exp(-z * z * 0.5);
            }

            p *= 1.0 / (Constants.Sqrt2PI * smoothing);

            return p / samples.Length;
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
        ///   See <see cref="EmpiricalDistribution"/>.
        /// </example>
        /// 
        public override double LogProbabilityDensityFunction(double x)
        {
            // References:
            //  - Bishop, Christopher M.; Pattern Recognition and Machine Learning. 

            double p = 0;

            for (int i = 0; i < samples.Length; i++)
            {
                double z = (x - samples[i]) / smoothing;
                p += Math.Exp(-z * z * 0.5);
            }

            double logp = Math.Log(p) - Math.Log(Constants.Sqrt2PI * smoothing);

            return logp - Math.Log(samples.Length);
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
        public override void Fit(double[] observations, double[] weights, IFittingOptions options)
        {
            Fit(observations, weights, options as EmpiricalOptions);
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
        public void Fit(double[] observations, double[] weights = null, EmpiricalOptions options = null)
        {
            if (weights != null)
                throw new ArgumentException("This distribution does not support weighted samples.");

            if (options != null)
                throw new ArgumentException("This method does not accept fitting options.");

            double? smoothing = null;

            if (options != null)
                smoothing = options.SmoothingRule(observations);

            initialize((double[])observations.Clone(), smoothing);
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
            EmpiricalDistribution e = new EmpiricalDistribution();
            e.samples = (double[])samples.Clone();
            e.smoothing = smoothing;

            return e;
        }


        private EmpiricalDistribution()
        {
        }

        private void initialize(double[] observations, double? smoothing)
        {
            if (smoothing == null)
                smoothing = SmoothingRule(observations);

            this.samples = observations;
            this.smoothing = smoothing.Value;

            this.mean = null;
            this.variance = null;
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
            return String.Format("Fn(x; S)");
        }


        /// <summary>
        ///   Gets the default estimative of the smoothing parameter.
        /// </summary>
        /// <remarks>
        ///   This method is based on the practical estimation of the bandwidth as
        ///   suggested in Wikipedia: http://en.wikipedia.org/wiki/Kernel_density_estimation
        /// </remarks>
        /// 
        /// <param name="observations">The observations for the empirical distribution.</param>
        /// 
        /// <returns>An estimative of the smoothing parameter.</returns>
        /// 
        public static double SmoothingRule(double[] observations)
        {
            double sigma = Statistics.Tools.StandardDeviation(observations);
            return sigma * Math.Pow(4.0 / (3.0 * observations.Length), -1 / 5.0);
        }

    }
}
