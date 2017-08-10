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
    using Accord.Statistics.Distributions.Fitting;
    using Tools = Statistics.Tools;
    using Accord.Compat;

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
        IFittableDistribution<double, EmpiricalOptions>,
        ISampleableDistribution<double>
    {

        // Distribution parameters
        double[] samples;
        double smoothing;

        WeightType type;
        double[] weights;
        int[] repeats;

        double sumOfWeights;
        int numberOfSamples;


        // Derived measures
        double? mean;
        double? variance;
        double? entropy;
        double? mode;

        double constant;


        /// <summary>
        ///   Creates a new Empirical Distribution from the data samples.
        /// </summary>
        /// 
        /// <param name="samples">The data samples.</param>
        /// 
        public EmpiricalDistribution(double[] samples)
        {
            initialize(samples, null, null, null);
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
            initialize(samples, null, null, smoothing);
        }

        /// <summary>
        ///   Creates a new Empirical Distribution from the data samples.
        /// </summary>
        /// 
        /// <param name="samples">The data samples.</param>
        /// <param name="weights">The fractional weights to use for the samples.
        ///   The weights must sum up to one.</param>
        /// 
        public EmpiricalDistribution(double[] samples, double[] weights)
        {
            initialize(samples, weights, null, null);
        }

        /// <summary>
        ///   Creates a new Empirical Distribution from the data samples.
        /// </summary>
        /// 
        /// <param name="samples">The data samples.</param>
        /// <param name="weights">The number of repetition counts for each sample.</param>
        /// 
        public EmpiricalDistribution(double[] samples, int[] weights)
        {
            initialize(samples, null, weights, null);
        }

        /// <summary>
        ///   Creates a new Empirical Distribution from the data samples.
        /// </summary>
        /// 
        /// <param name="samples">The data samples.</param>
        /// <param name="weights">The fractional weights to use for the samples.
        ///   The weights must sum up to one.</param>
        /// <param name="smoothing">
        ///   The kernel smoothing or bandwidth to be used in density estimation.
        ///   By default, the normal distribution approximation will be used.</param>
        /// 
        public EmpiricalDistribution(double[] samples, double[] weights, double smoothing)
        {
            initialize(samples, weights, null, smoothing);
        }

        /// <summary>
        ///   Creates a new Empirical Distribution from the data samples.
        /// </summary>
        /// 
        /// <param name="samples">The data samples.</param>
        /// <param name="smoothing">
        ///   The kernel smoothing or bandwidth to be used in density estimation.
        ///   By default, the normal distribution approximation will be used.</param>
        /// <param name="weights">The number of repetition counts for each sample.</param>
        /// 
        public EmpiricalDistribution(double[] samples, int[] weights, double smoothing)
        {
            initialize(samples, null, weights, smoothing);
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
        ///   Gets the fractional weights associated with each sample. Note that
        ///   changing values on this array will not result int any effect in
        ///   this distribution. The distribution must be computed from scratch
        ///   with new values in case new weights needs to be used.
        /// </summary>
        /// 
        public double[] Weights
        {
            get
            {
                if (weights == null)
                {
                    weights = new double[samples.Length];
                    for (int i = 0; i < weights.Length; i++)
                        weights[i] = Counts[i] / (double)Length;
                }

                return weights;
            }
        }

        /// <summary>
        ///   Gets the repetition counts associated with each sample. Note that
        ///   changing values on this array will not result int any effect in
        ///   this distribution. The distribution must be computed from scratch
        ///   with new values in case new weights needs to be used.
        /// </summary>
        /// 
        public int[] Counts
        {
            get
            {
                if (repeats == null)
                {
                    repeats = new int[samples.Length];
                    for (int i = 0; i < repeats.Length; i++)
                        repeats[i] = 1;
                }

                return repeats;
            }
        }

        /// <summary>
        ///   Gets the total number of samples in this distribution.
        /// </summary>
        /// 
        public int Length
        {
            get { return numberOfSamples; }
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
                {
                    if (type == WeightType.None)
                        mean = Measures.Mean(samples);

                    else if (type == WeightType.Repetition)
                        mean = Measures.WeightedMean(samples, repeats);

                    else if (type == WeightType.Fraction)
                        mean = Measures.WeightedMean(samples, weights);
                }

                return mean.Value;
            }
        }

        /// <summary>
        ///   Gets the mode for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's mode value.
        /// </value>
        /// 
        public override double Mode
        {
            get
            {
                if (mode == null)
                {
                    if (type == WeightType.None)
                        mode = Measures.Mode(samples);

                    else if (type == WeightType.Repetition)
                        mode = Measures.WeightedMode(samples, repeats);

                    else if (type == WeightType.Fraction)
                        mode = Measures.WeightedMode(samples, weights);
                }

                return mode.Value;
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
                {
                    if (type == WeightType.None)
                        variance = Measures.Variance(samples);

                    else if (type == WeightType.Repetition)
                        variance = Measures.WeightedVariance(samples, repeats);

                    else if (type == WeightType.Fraction)
                        variance = Measures.WeightedVariance(samples, weights);
                }

                return variance.Value;
            }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        public override double Entropy
        {
            get
            {
                if (entropy == null)
                {
                    if (type == WeightType.None)
                        entropy = Measures.Entropy(samples, ProbabilityDensityFunction);

                    else if (type == WeightType.Repetition)
                        entropy = Measures.WeightedEntropy(samples, repeats, ProbabilityDensityFunction);

                    else if (type == WeightType.Fraction)
                        entropy = Measures.WeightedEntropy(samples, weights, ProbabilityDensityFunction);
                }

                return entropy.Value;
            }
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
        protected internal override double InnerDistributionFunction(double x)
        {
            if (type == WeightType.None)
            {
                int sum = 0; // Normal sample, no weights
                for (int i = 0; i < samples.Length; i++)
                {
                    if (samples[i] <= x)
                        sum++;
                }

                return sum / (double)numberOfSamples;
            }

            if (type == WeightType.Repetition)
            {
                int sum = 0; // Repetition counts weights
                for (int i = 0; i < samples.Length; i++)
                {
                    if (samples[i] <= x)
                        sum += repeats[i];
                }

                return sum / (double)numberOfSamples;
            }

            if (type == WeightType.Fraction)
            {
                double sum = 0; // Fractional weights
                for (int i = 0; i < samples.Length; i++)
                {
                    if (samples[i] <= x)
                        sum += weights[i];
                }

                return sum / sumOfWeights;
            }

            throw new InvalidOperationException();
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
        protected internal override double InnerProbabilityDensityFunction(double x)
        {
            // References:
            //  - Bishop, Christopher M.; Pattern Recognition and Machine Learning. 

            double p = 0;

            if (type == WeightType.None)
            {
                // Normal samples, not using any weights
                for (int i = 0; i < samples.Length; i++)
                {
                    double z = (x - samples[i]) / smoothing;
                    p += Math.Exp(-z * z * 0.5);
                }
            }

            else if (type == WeightType.Repetition)
            {
                // Weighted sample using discrete counts
                for (int i = 0; i < samples.Length; i++)
                {
                    double z = (x - samples[i]) / smoothing;
                    p += repeats[i] * Math.Exp(-z * z * 0.5);
                }
            }

            else if (type == WeightType.Fraction)
            {
                // Weighted sample using fractional weights
                for (int i = 0; i < samples.Length; i++)
                {
                    double z = (x - samples[i]) / smoothing;
                    p += weights[i] * Math.Exp(-z * z * 0.5);
                }
            }

            return p * constant;
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
        public void Fit(double[] observations, double[] weights, EmpiricalOptions options)
        {
            double? smoothing = null;
            bool inPlace = false;

            if (options != null)
            {
                smoothing = options.SmoothingRule(observations, weights, null);
                inPlace = options.InPlace;
            }

            if (!inPlace)
            {
                observations = (double[])observations.Clone();

                if (weights != null)
                    weights = (double[])weights.Clone();
            }

            initialize(observations, weights, null, smoothing);
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
        public void Fit(double[] observations, int[] weights, EmpiricalOptions options)
        {
            double? smoothing = null;
            bool inPlace = false;

            if (options != null)
            {
                smoothing = options.SmoothingRule(observations, null, weights);
                inPlace = options.InPlace;
            }

            if (!inPlace)
            {
                observations = (double[])observations.Clone();

                if (weights != null)
                    weights = (int[])weights.Clone();
            }

            initialize(observations, null, weights, smoothing);
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
            var clone = new EmpiricalDistribution();

            clone.type = type;
            clone.sumOfWeights = sumOfWeights;
            clone.numberOfSamples = numberOfSamples;
            clone.smoothing = smoothing;
            clone.constant = constant;

            clone.samples = (double[])samples.Clone();

            if (weights != null)
                clone.weights = (double[])weights.Clone();

            if (repeats != null)
                clone.repeats = (int[])repeats.Clone();

            return clone;
        }


        private EmpiricalDistribution()
        {
        }

        private void initialize(double[] observations, double[] weights, int[] repeats, double? smoothing)
        {
            if (smoothing == null)
            {
                smoothing = SmoothingRule(observations, weights, repeats);
            }

            this.samples = observations;
            this.weights = weights;
            this.repeats = repeats;
            this.smoothing = smoothing.Value;


            if (weights != null)
            {
                this.type = WeightType.Fraction;
                this.numberOfSamples = samples.Length;
                this.sumOfWeights = weights.Sum();
                this.constant = 1.0 / (Constants.Sqrt2PI * this.smoothing);
            }
            else if (repeats != null)
            {
                this.type = WeightType.Repetition;
                this.numberOfSamples = repeats.Sum();
                this.sumOfWeights = 1.0;
                this.constant = 1.0 / (Constants.Sqrt2PI * this.smoothing * numberOfSamples);
            }
            else
            {
                this.type = WeightType.None;
                this.numberOfSamples = samples.Length;
                this.sumOfWeights = 1.0;
                this.constant = 1.0 / (Constants.Sqrt2PI * this.smoothing * numberOfSamples);
            }


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
        public override string  ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format(formatProvider, "Fn(x; S)");
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
            double sigma = Measures.StandardDeviation(observations);
            return sigma * Math.Pow(4.0 / (3.0 * observations.Length), 1.0 / 5.0);
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
        /// <param name="weights">The fractional importance for each sample. Those values must sum up to one.</param>
        /// 
        /// <returns>An estimative of the smoothing parameter.</returns>
        /// 
        public static double SmoothingRule(double[] observations, double[] weights)
        {
            double N = weights.Sum();
            double sigma = Measures.WeightedStandardDeviation(observations, weights);
            return sigma * Math.Pow(4.0 / (3.0 * N), 1.0 / 5.0);
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
        /// <param name="repeats">The number of times each sample should be repeated.</param>
        /// 
        /// <returns>An estimative of the smoothing parameter.</returns>
        /// 
        public static double SmoothingRule(double[] observations, int[] repeats)
        {
            double N = repeats.Sum();
            double sigma = Measures.WeightedStandardDeviation(observations, repeats);
            return sigma * Math.Pow(4.0 / (3.0 * N), 1.0 / 5.0);
        }

        /// <summary>
        ///   Gets the default estimative of the smoothing parameter.
        /// </summary>
        /// 
        /// <remarks>
        ///   This method is based on the practical estimation of the bandwidth as
        ///   suggested in Wikipedia: http://en.wikipedia.org/wiki/Kernel_density_estimation
        /// </remarks>
        /// 
        /// <param name="observations">The observations for the empirical distribution.</param>
        /// <param name="weights">The fractional importance for each sample. Those values must sum up to one.</param>
        /// <param name="repeats">The number of times each sample should be repeated.</param>
        /// 
        /// <returns>An estimative of the smoothing parameter.</returns>
        /// 
        public static double SmoothingRule(double[] observations, double[] weights, int[] repeats)
        {
            if (weights != null)
            {
                if (repeats != null)
                    throw new ArgumentException("Either weights or repeats can be different from null.");

                return SmoothingRule(observations, weights);
            }

            if (repeats != null)
            {
                if (weights != null)
                    throw new ArgumentException("Either weights or repeats can be different from null.");

                return SmoothingRule(observations, repeats);
            }

            return SmoothingRule(observations);
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
            if (weights == null)
            {
                for (int i = 0; i < samples; i++)
                    result[i] = this.samples[source.Next(this.samples.Length)];
                return result;
            }

            double u = source.NextDouble();
            double uniform = u * sumOfWeights;

            for (int i = 0; i < samples; i++)
            {
                double cumulativeSum = 0;
                for (int j = 0; j < weights.Length; j++)
                {
                    cumulativeSum += weights[j];

                    if (uniform < cumulativeSum)
                    {
                        result[i] = this.samples[j];
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        /// 
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public override double Generate(Random source)
        {
            if (weights == null)
                return this.samples[source.Next(this.samples.Length)];


            double u = source.NextDouble();
            double uniform = u * sumOfWeights;

            double cumulativeSum = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                cumulativeSum += weights[i];

                if (uniform < cumulativeSum)
                    return this.samples[i];
            }

            throw new InvalidOperationException("Execution should never reach here.");
        }
    }
}
