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
        double[] weights;
        int[] repeats;
        double smoothing;

        double numberOfSamples;
        double sumOfWeights;


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
        /// <param name="smoothing">
        ///   The kernel smoothing or bandwidth to be used in density estimation.
        ///   By default, the normal distribution approximation will be used.</param>
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
        /// <param name="smoothing">
        ///   The kernel smoothing or bandwidth to be used in density estimation.
        ///   By default, the normal distribution approximation will be used.</param>
        /// 
        public EmpiricalDistribution(double[] samples, int[] repeats)
        {
            initialize(samples, null, repeats, null);
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
        /// 
        public EmpiricalDistribution(double[] samples, int[] repeats, double smoothing)
        {
            initialize(samples, null, repeats, smoothing);
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
        ///   Gets the weights associated with each sample. In case all
        ///   samples have the same weight, this vector can be null.
        /// </summary>
        /// 
        public double[] Weights
        {
            get { return weights; }
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

        public double Length
        {
            get { return numberOfSamples; }
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
                    if (repeats != null)
                        mean = Statistics.Tools.WeightedMean(samples, repeats);
                    else if (weights != null)
                        mean = Statistics.Tools.WeightedMean(samples, weights);
                    else mean = Statistics.Tools.Mean(samples);
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
                    if (repeats != null)
                        mode = Statistics.Tools.WeightedMode(samples, repeats);
                    else if (weights != null)
                        mode = Statistics.Tools.WeightedMode(samples, weights);
                    else
                        mode = Statistics.Tools.Mode(samples);
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
                    if (repeats != null)
                        variance = Statistics.Tools.WeightedVariance(samples, repeats);
                    else if (weights != null)
                        variance = Statistics.Tools.WeightedVariance(samples, weights);
                    else
                        variance = Statistics.Tools.Variance(samples);
                }

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
                    if (repeats != null)
                    {
                        double sum = 0;
                        for (int i = 0; i < samples.Length; i++)
                        {
                            double p = ProbabilityDensityFunction(samples[i]);
                            sum += repeats[i] * (p * Math.Log(p));
                        }
                        this.entropy = sum;
                    }

                    else if (weights != null)
                    {
                        double sum = 0;
                        for (int i = 0; i < samples.Length; i++)
                        {
                            double p = ProbabilityDensityFunction(samples[i]);
                            sum += weights[i] * (p * Math.Log(p));
                        }
                        this.entropy = sum;
                    }

                    else
                    {
                        double sum = 0;
                        for (int i = 0; i < samples.Length; i++)
                        {
                            double p = ProbabilityDensityFunction(samples[i]);
                            sum += p * Math.Log(p);
                        }
                        this.entropy = sum;
                    }
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
            if (repeats != null)
            {
                int sum = 0;
                for (int i = 0; i < samples.Length; i++)
                {
                    if (samples[i] <= x)
                        sum += repeats[i];
                }

                return sum / numberOfSamples;
            }
            else if (weights != null)
            {
                double sum = 0;
                for (int i = 0; i < samples.Length; i++)
                {
                    if (samples[i] <= x)
                        sum += weights[i];
                }

                return sum / sumOfWeights;
            }
            else
            {
                int sum = 0;
                for (int i = 0; i < samples.Length; i++)
                {
                    if (samples[i] <= x)
                        sum++;
                }

                return sum / numberOfSamples;
            }
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

            if (repeats != null)
            {
                double p = 0;
                for (int i = 0; i < samples.Length; i++)
                {
                    double z = (x - samples[i]) / smoothing;
                    p += repeats[i] * Math.Exp(-z * z * 0.5);
                }

                return p * constant;
            }

            else if (weights != null)
            {
                double p = 0;
                for (int i = 0; i < samples.Length; i++)
                {
                    double z = (x - samples[i]) / smoothing;
                    p += weights[i] * Math.Exp(-z * z * 0.5);
                }

                return p * constant;
            }

            else
            {
                double p = 0;
                for (int i = 0; i < samples.Length; i++)
                {
                    double z = (x - samples[i]) / smoothing;
                    p += Math.Exp(-z * z * 0.5);
                }
                return p * constant;
            }
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

            if (options != null)
                smoothing = options.SmoothingRule(observations, weights, null);

            double[] newSamples = (double[])observations.Clone();
            double[] newWeights = weights != null ? (double[])weights.Clone() : null;

            initialize(newSamples, newWeights, null, smoothing);
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

            if (options != null)
                smoothing = options.SmoothingRule(observations, null, weights);

            double[] newSamples = (double[])observations.Clone();
            int[] newWeights = weights != null ? (int[])weights.Clone() : null;

            initialize(newSamples, null, newWeights, smoothing);
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

            clone.numberOfSamples = numberOfSamples;
            clone.sumOfWeights = sumOfWeights;
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
                this.numberOfSamples = samples.Length;
                this.sumOfWeights = weights.Sum();
                this.constant = 1.0 / (Constants.Sqrt2PI * this.smoothing);
            }
            else if (repeats != null)
            {
                this.numberOfSamples = repeats.Sum();
                this.weights = repeats.Divide(numberOfSamples);
                this.sumOfWeights = 1.0;
                this.constant = 1.0 / (Constants.Sqrt2PI * this.smoothing * numberOfSamples);
            }
            else
            {
                this.numberOfSamples = samples.Length;
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
        /// 
        /// <returns>An estimative of the smoothing parameter.</returns>
        /// 
        public static double SmoothingRule(double[] observations, double[] weights)
        {
            double N = weights.Sum();
            double sigma = Statistics.Tools.WeightedStandardDeviation(observations, weights);
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
        /// 
        /// <returns>An estimative of the smoothing parameter.</returns>
        /// 
        public static double SmoothingRule(double[] observations, int[] repeats)
        {
            double N = repeats.Sum();
            double sigma = Statistics.Tools.WeightedStandardDeviation(observations, repeats);
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
        /// 
        /// <returns>An estimative of the smoothing parameter.</returns>
        /// 
        public static double SmoothingRule(double[] observations, double[] weights, int[] repeats)
        {
            if (weights != null)
                return SmoothingRule(observations, weights);

            if (repeats != null)
                return SmoothingRule(observations, repeats);

            return SmoothingRule(observations);
        }

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public double[] Generate(int samples)
        {
            var generator = Accord.Math.Tools.Random;

            double[] s = new double[samples];

            if (weights == null)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    int index = generator.Next(this.samples.Length);
                    s[i] = this.samples[index];
                }
            }
            else
            {
                double u = generator.NextDouble();
                double uniform = u * numberOfSamples;

                for (int i = 0; i < s.Length; i++)
                {
                    double cumulativeSum = 0;
                    for (int j = 0; j < weights.Length; j++)
                    {
                        cumulativeSum += weights[j];

                        if (uniform < cumulativeSum)
                        {
                            s[i] = this.samples[j];
                            break;
                        }
                    }
                }
            }

            return s;
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public double Generate()
        {
            var generator = Accord.Math.Tools.Random;

            if (weights == null)
            {
                int index = generator.Next(this.samples.Length);
                return this.samples[index];
            }
            else
            {
                double u = generator.NextDouble();

                double uniform = u * numberOfSamples;

                double cumulativeSum = 0;
                for (int i = 0; i < weights.Length; i++)
                {
                    cumulativeSum += weights[i];

                    if (uniform < cumulativeSum)
                        return this.samples[i];
                }

                throw new Exception();
            }
        }
    }
}
