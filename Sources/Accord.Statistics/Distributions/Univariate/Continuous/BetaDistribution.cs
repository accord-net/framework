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
    using Accord.Math.Optimization;
    using System.ComponentModel;
    using Accord.Compat;

    /// <summary>
    ///   Beta Distribution (of the first kind).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The beta distribution is a family of continuous probability distributions
    ///   defined on the interval (0, 1) parameterized by two positive shape parameters,
    ///   typically denoted by α and β. The beta distribution can be suited to the 
    ///   statistical modeling of proportions in applications where values of proportions
    ///   equal to 0 or 1 do not occur. One theoretical case where the beta distribution 
    ///   arises is as the distribution of the ratio formed by one random variable having
    ///   a Gamma distribution divided by the sum of it and another independent random 
    ///   variable also having a Gamma distribution with the same scale parameter (but 
    ///   possibly different shape parameter).</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Beta_distribution">
    ///       Wikipedia, The Free Encyclopedia. Beta distribution. 
    ///       Available from: http://en.wikipedia.org/wiki/Beta_distribution </a></description></item>
    ///   </list></para>
    /// </remarks>
    ///
    /// <example>
    /// <para>
    ///   Note: More advanced examples, including distribution estimation and random number
    ///   generation are also available at the <see cref="GeneralizedBetaDistribution"/>
    ///   page.</para>
    ///   
    /// <para>
    ///   The following example shows how to instantiate and use a Beta 
    ///   distribution given its alpha and beta parameters: </para>
    /// 
    /// <code>
    ///  double alpha = 0.42;
    ///  double beta = 1.57;
    /// 
    ///  // Create a new Beta distribution with α = 0.42 and β = 1.57
    ///  BetaDistribution distribution = new BetaDistribution(alpha, beta);
    /// 
    ///  // Common measures
    ///  double mean   = distribution.Mean;      // 0.21105527638190955
    ///  double median = distribution.Median;    // 0.11577711097114812
    ///  double var    = distribution.Variance;  // 0.055689279830523512
    ///  
    ///  // Cumulative distribution functions
    ///  double cdf    = distribution.DistributionFunction(x: 0.27);          // 0.69358638272337991
    ///  double ccdf   = distribution.ComplementaryDistributionFunction(x: 0.27); // 0.30641361727662009
    ///  double icdf   = distribution.InverseDistributionFunction(p: cdf);        // 0.26999999068687469
    ///  
    ///  // Probability density functions
    ///  double pdf    = distribution.ProbabilityDensityFunction(x: 0.27);    // 0.94644031936694828
    ///  double lpdf   = distribution.LogProbabilityDensityFunction(x: 0.27); // -0.055047364344046057
    ///  
    ///  // Hazard (failure rate) functions
    ///  double hf     = distribution.HazardFunction(x: 0.27);           // 3.0887671630877072
    ///  double chf    = distribution.CumulativeHazardFunction(x: 0.27); // 1.1828193992944409
    ///  
    ///  // String representation
    ///  string str = distribution.ToString(); // B(x; α = 0.42, β = 1.57)
    /// </code>
    /// 
    /// <para>
    ///   The following example shows to create a Beta distribution
    ///   given a discrete number of trials and the number of successes
    ///   within those trials. It also shows how to compute the 2.5 and
    ///   97.5 percentiles of the distribution:</para>
    ///   
    /// <code>
    ///  int trials = 100;
    ///  int successes = 78;
    ///  
    ///  BetaDistribution distribution = new BetaDistribution(successes, trials);
    ///  
    ///  double mean   = distribution.Mean; // 0.77450980392156865
    ///  double median = distribution.Median; // 0.77630912598534851
    ///  
    ///  double p025   = distribution.InverseDistributionFunction(p: 0.025); // 0.68899653915764347
    ///  double p975   = distribution.InverseDistributionFunction(p: 0.975); // 0.84983461640764513
    /// </code>
    /// 
    /// <para>
    ///   The next example shows how to generate 1000 new samples from a Beta distribution:</para>
    /// 
    /// <code>
    ///   // Using the distribution's parameters
    ///   double[] samples = GeneralizedBetaDistribution
    ///     .Random(alpha: 2, beta: 3, min: 0, max: 1, samples: 1000);
    ///     
    ///   // Using an existing distribution
    ///   var b = new GeneralizedBetaDistribution(alpha: 1, beta: 2);
    ///   double[] new_samples = b.Generate(1000);
    /// </code>
    /// 
    /// <para>
    ///   And finally, how to estimate the parameters of a Beta distribution from
    ///   a set of observations, using either the Method-of-moments or the Maximum 
    ///   Likelihood Estimate.</para>
    ///   
    /// <code>
    /// // Draw 100000 observations from a Beta with α = 2, β = 3:
    /// double[] samples = GeneralizedBetaDistribution
    ///     .Random(alpha: 2, beta: 3, samples: 100000);
    /// 
    /// // Estimate a distribution from the data
    /// var B = BetaDistribution.Estimate(samples);
    /// 
    /// // Explicitly using Method-of-moments estimation
    /// var mm = BetaDistribution.Estimate(samples,
    ///     new BetaOptions { Method = BetaEstimationMethod.Moments });
    ///     
    /// // Explicitly using Maximum Likelihood estimation
    /// var mle = BetaDistribution.Estimate(samples,
    ///     new BetaOptions { Method = BetaEstimationMethod.MaximumLikelihood });
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="Accord.Math.Beta"/>
    /// <seealso cref="GeneralizedBetaDistribution"/>
    ///
    [Serializable]
    public class BetaDistribution : UnivariateContinuousDistribution, IFormattable,
        IFittableDistribution<double, BetaOptions>, ISampleableDistribution<double>
    {
        double alpha;
        double beta;

        double constant;
        double? entropy;


        /// <summary>
        ///   Creates a new Beta distribution.
        /// </summary>
        /// 
        public BetaDistribution()
            : this(0, 1)
        {
        }

        /// <summary>
        ///   Creates a new Beta distribution.
        /// </summary>
        /// 
        /// <param name="successes">The number of success <c>r</c>. Default is 0.</param>
        /// <param name="trials">The number of trials <c>n</c>. Default is 1.</param>
        /// 
        public BetaDistribution([NonnegativeInteger(maximum: Int32.MaxValue - 1)] int successes, [PositiveInteger] int trials)
        {
            if (successes < 0)
                throw new ArgumentOutOfRangeException("successes", "The number of success must be positive");
            if (successes == Int32.MaxValue)
                throw new ArgumentOutOfRangeException("successes", "The number of success must be less than Int32.MaxValue");

            if (trials <= 0)
                throw new ArgumentOutOfRangeException("trials", "The number of trials must be positive");

            if (successes > trials)
            {
                throw new ArgumentOutOfRangeException("successes",
                    "The number of successes should be lesser than or equal to the number of trials");
            }

            initialize(successes + 1, trials - successes + 1);
        }

        /// <summary>
        ///   Creates a new Beta distribution.
        /// </summary>
        /// 
        /// <param name="alpha">The shape parameter α (alpha).</param>
        /// <param name="beta">The shape parameter β (beta).</param>
        /// 
        public BetaDistribution([Positive, DefaultValue(1)] double alpha, [Positive, DefaultValue(1)] double beta)
        {
            if (alpha <= 0)
                throw new ArgumentOutOfRangeException("alpha", "The shape parameter alpha must be positive.");

            if (beta <= 0)
                throw new ArgumentOutOfRangeException("beta", "The shape parameter beta must be positive.");

            initialize(alpha, beta);
        }

        private void initialize(double alpha, double beta)
        {
            this.alpha = alpha;
            this.beta = beta;

            this.constant = 1.0 / Accord.Math.Beta.Function(alpha, beta);
            this.entropy = null;
        }

        /// <summary>
        ///   Gets the shape parameter α (alpha)
        /// </summary>
        /// 
        public double Alpha
        {
            get { return alpha; }
        }

        /// <summary>
        ///   Gets the shape parameter β (beta).
        /// </summary>
        /// 
        public double Beta
        {
            get { return beta; }
        }

        /// <summary>
        ///   Gets the number of successes <c>r</c>.
        /// </summary>
        /// 
        public double Successes
        {
            get { return alpha - 1; }
        }

        /// <summary>
        ///   Gets the number of trials <c>n</c>.
        /// </summary>
        /// 
        public double Trials
        {
            get { return beta + Successes - 1; }
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
            get { return new DoubleRange(0, 1); }
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mean value.</value>
        /// 
        /// <remarks>The Beta's mean is computed as μ = a / (a + b).</remarks>
        /// 
        public override double Mean
        {
            get { return alpha / (alpha + beta); }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's variance.</value>
        /// 
        /// <remarks>The Beta's variance is computed as σ² = (a * b) / ((a + b)² * (a + b + 1)).</remarks>
        /// 
        public override double Variance
        {
            get { return (alpha * beta) / ((alpha + beta) * (alpha + beta) * (alpha + beta + 1)); }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's entropy.</value>
        /// 
        public override double Entropy
        {
            get
            {
                if (entropy == null)
                {
                    double lnBab = Math.Log(Accord.Math.Beta.Function(alpha, beta));
                    double da = Gamma.Digamma(alpha);
                    double db = Gamma.Digamma(beta);
                    double dab = Gamma.Digamma(alpha + beta);
                    entropy = lnBab - (alpha - 1) * da - (beta - 1) * db + (alpha + beta - 2) * dab;
                }

                return entropy.Value;
            }
        }

        /// <summary>
        ///   Gets the mode for this distribution.
        /// </summary>
        /// 
        /// <remarks>
        ///  The beta distribution's mode is given
        ///  by <c>(a - 1) / (a + b - 2).</c>
        /// </remarks>
        /// 
        /// <value>
        ///   The distribution's mode value.
        /// </value>
        /// 
        public override double Mode
        {
            get { return (alpha - 1.0) / (alpha + beta - 2.0); }
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <remarks>
        /// <para>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.</para>
        ///   
        /// <para>
        ///   The Beta's CDF is computed using the <see cref="Accord.Math.Beta.Incomplete">Incomplete 
        ///   (regularized) Beta function I_x(a,b)</see> as CDF(x) = I_x(a,b)</para>
        /// </remarks>
        /// 
        protected internal override double InnerDistributionFunction(double x)
        {
            return Accord.Math.Beta.Incomplete(alpha, beta, x);
        }

        /// <summary>
        ///   Gets the inverse of the cumulative distribution function (icdf) for
        ///   this distribution evaluated at probability <c>p</c>. This function
        ///   is also known as the Quantile function.
        /// </summary>
        /// 
        /// <param name="p">A probability value between 0 and 1.</param>
        /// 
        /// <returns>A sample which could original the given probability 
        ///   value when applied in the <see cref="UnivariateContinuousDistribution.DistributionFunction(double)"/>.</returns>
        /// 
        protected internal override double InnerInverseDistributionFunction(double p)
        {
            return Accord.Math.Beta.IncompleteInverse(alpha, beta, p);
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
        /// <para>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.</para>
        /// <para>
        ///   The Beta's PDF is computed as pdf(x) = c * x^(a - 1) * (1 - x)^(b - 1)
        ///   where constant c is c = 1.0 / <see cref="Accord.Math.Beta.Function">Beta.Function(a, b)</see></para>
        /// </remarks>
        /// 
        protected internal override double InnerProbabilityDensityFunction(double x)
        {
            return constant * Math.Pow(x, alpha - 1) * Math.Pow(1 - x, beta - 1);
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
        /// <para>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.</para>
        /// </remarks>
        /// 
        /// <seealso cref="UnivariateContinuousDistribution.ProbabilityDensityFunction(double)"/>
        /// 
        protected internal override double InnerLogProbabilityDensityFunction(double x)
        {
            return Math.Log(constant) + (alpha - 1) * Math.Log(x) + (beta - 1) * Math.Log(1 - x);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">
        ///   The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting,
        ///   such as regularization constants and additional parameters.</param>
        /// 
        public override void Fit(double[] observations, double[] weights, IFittingOptions options)
        {
            Fit(observations, weights, options as BetaOptions);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">
        ///   The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting,
        ///   such as regularization constants and additional parameters.</param>
        /// 
        public override void Fit(double[] observations, int[] weights, IFittingOptions options)
        {
            Fit(observations, weights, options as BetaOptions);
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
        public void Fit(double[] observations, double[] weights, BetaOptions options)
        {
            bool useMLE = false;
            if (options != null)
                useMLE = options.Method == BetaEstimationMethod.MaximumLikelihood;

            double mean;
            double var;

            if (weights == null)
            {
                mean = observations.Mean();
                var = observations.Variance(mean);
            }
            else
            {
                mean = observations.WeightedMean(weights);
                var = observations.WeightedVariance(weights, mean);
            }

            fitMoments(mean, var);

            if (useMLE)
            {
                if (weights == null)
                {
                    double sum1 = 0, sum2 = 0;
                    for (int i = 0; i < observations.Length; i++)
                    {
                        sum1 += Math.Log(observations[i]);
                        sum2 += Math.Log(1 - observations[i]);
                    }

                    fitMLE(sum1, sum2, observations.Length);
                }
                else
                {
                    double sum1 = 0, sum2 = 0, sumw = 0;
                    for (int i = 0; i < observations.Length; i++)
                    {
                        sum1 += Math.Log(observations[i]) * weights[i];
                        sum2 += Math.Log(1 - observations[i]) * weights[i];
                        sumw += weights[i];
                    }

                    fitMLE(sum1, sum2, sumw);
                }
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
        public void Fit(double[] observations, int[] weights, BetaOptions options)
        {
            bool useMLE = false;
            if (options != null)
                useMLE = options.Method == BetaEstimationMethod.MaximumLikelihood;

            double mean;
            double var;

            if (weights == null)
            {
                mean = observations.Mean();
                var = observations.Variance(mean);
            }
            else
            {
                mean = observations.WeightedMean(weights);
                var = observations.WeightedVariance(weights, mean);
            }

            fitMoments(mean, var);

            if (useMLE)
            {
                if (weights == null)
                {
                    double sum1 = 0, sum2 = 0;
                    for (int i = 0; i < observations.Length; i++)
                    {
                        sum1 += Math.Log(observations[i]);
                        sum2 += Math.Log(1 - observations[i]);
                    }

                    fitMLE(sum1, sum2, observations.Length);
                }
                else
                {
                    double sum1 = 0, sum2 = 0, sumw = 0;
                    for (int i = 0; i < observations.Length; i++)
                    {
                        sum1 += Math.Log(observations[i]) * weights[i];
                        sum2 += Math.Log(1 - observations[i]) * weights[i];
                        sumw += weights[i];
                    }

                    fitMLE(sum1, sum2, sumw);
                }
            }
        }

        private void fitMoments(double mean, double var)
        {
            if (var >= mean * (1.0 - mean))
                throw new NotSupportedException();

            double u = (mean * (1 - mean) / var) - 1.0;
            double alpha = mean * u;
            double beta = (1 - mean) * u;
            initialize(alpha, beta);
        }

        private void fitMLE(double sum1, double sum2, double n)
        {
            double[] gradient = new double[2];

            var bfgs = new BoundedBroydenFletcherGoldfarbShanno(numberOfVariables: 2);
            bfgs.LowerBounds[0] = 1e-100;
            bfgs.LowerBounds[1] = 1e-100;
            bfgs.Solution[0] = this.alpha;
            bfgs.Solution[1] = this.beta;

            bfgs.Function = (double[] parameters) =>
                LogLikelihood(sum1, sum2, n, parameters[0], parameters[1]);

            bfgs.Gradient = (double[] parameters) =>
                Gradient(sum1, sum2, n, parameters[0], parameters[1], gradient);

            if (!bfgs.Minimize())
                throw new ConvergenceException();

            this.alpha = bfgs.Solution[0];
            this.beta = bfgs.Solution[1];
        }


        /// <summary>
        ///   Computes the Gradient of the Log-Likelihood function for estimating Beta distributions.
        /// </summary>
        /// 
        /// <param name="observations">The observed values.</param>
        /// <param name="alpha">The current alpha value.</param>
        /// <param name="beta">The current beta value.</param>
        /// 
        /// <returns>
        ///   A bi-dimensional value containing the gradient w.r.t to alpha in its
        ///   first position, and the gradient w.r.t to be in its second position.
        /// </returns>
        /// 
        public static double[] Gradient(double[] observations, double alpha, double beta)
        {
            double sum1 = 0, sum2 = 0;
            for (int i = 0; i < observations.Length; i++)
            {
                sum1 += Math.Log(observations[i]);
                sum2 += Math.Log(1 - observations[i]);
            }

            double[] g = new double[2];
            Gradient(sum1, sum2, observations.Length, alpha, beta, g);
            return g;
        }

        /// <summary>
        ///   Computes the Gradient of the Log-Likelihood function for estimating Beta distributions.
        /// </summary>
        /// 
        /// <param name="sum1">The sum of log(y), where y refers to all observed values.</param>
        /// <param name="sum2">The sum of log(1 - y), where y refers to all observed values.</param>
        /// <param name="n">The total number of observed values.</param>
        /// <param name="alpha">The current alpha value.</param>
        /// <param name="beta">The current beta value.</param>
        /// <param name="g">A bi-dimensional vector to store the gradient.</param>
        /// 
        /// <returns>
        ///   A bi-dimensional vector containing the gradient w.r.t to alpha in its
        ///   first position, and the gradient w.r.t to be in its second position.
        /// </returns>
        /// 
        public static double[] Gradient(double sum1, double sum2, double n, double alpha, double beta, double[] g)
        {
            double dab = Gamma.Digamma(alpha + beta);
            double da = Gamma.Digamma(alpha);
            double db = Gamma.Digamma(beta);

            double ga = sum1 - n * (da - dab);
            double gb = sum2 - n * (db - dab);

            g[0] = -ga;
            g[1] = -gb;

            return g;
        }

        /// <summary>
        ///   Computes the Log-Likelihood function for estimating Beta distributions.
        /// </summary>
        /// 
        /// <param name="observations">The observed values.</param>
        /// <param name="alpha">The current alpha value.</param>
        /// <param name="beta">The current beta value.</param>
        /// 
        /// <returns>The log-likelihood value for the given observations and given Beta parameters.</returns>
        /// 
        public static double LogLikelihood(double[] observations, double alpha, double beta)
        {
            double sum1 = 0, sum2 = 0;
            for (int i = 0; i < observations.Length; i++)
            {
                sum1 += Math.Log(observations[i]);
                sum2 += Math.Log(1 - observations[i]);
            }

            return LogLikelihood(sum1, sum2, observations.Length, alpha, beta);
        }

        /// <summary>
        ///   Computes the Log-Likelihood function for estimating Beta distributions.
        /// </summary>
        /// 
        /// <param name="sum1">The sum of log(y), where y refers to all observed values.</param>
        /// <param name="sum2">The sum of log(1 - y), where y refers to all observed values.</param>
        /// <param name="n">The total number of observed values.</param>
        /// <param name="alpha">The current alpha value.</param>
        /// <param name="beta">The current beta value.</param>
        /// 
        /// <returns>The log-likelihood value for the given observations and given Beta parameters.</returns>
        /// 
        public static double LogLikelihood(double sum1, double sum2, double n, double alpha, double beta)
        {
            double t1 = (alpha - 1) * sum1;
            double t2 = (beta - 1) * sum2;
            double t3 = Accord.Math.Beta.Log(alpha, beta);

            double sum = t1 + t2 - n * t3;

            return -sum;
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
            return new BetaDistribution(alpha, beta);
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
            return String.Format(formatProvider, "B(x; α = {0}, β = {1})",
                alpha.ToString(format, formatProvider),
                beta.ToString(format, formatProvider));
        }


        #region ISampleableDistribution<double> Members

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
            return Random(alpha, beta, samples, result, source);
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
            return Random(alpha, beta, source);
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Beta distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="alpha">The shape parameter α (alpha).</param>
        /// <param name="beta">The shape parameter β (beta).</param>
        /// <param name="samples">The number of samples to generate.</param>
        ///
        /// <returns>An array of double values sampled from the specified Beta distribution.</returns>
        /// 
        public static double[] Random(double alpha, double beta, int samples)
        {
            return Random(alpha, beta, samples, new double[samples], Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Beta distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="alpha">The shape parameter α (alpha).</param>
        /// <param name="beta">The shape parameter β (beta).</param>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        ///
        /// <returns>An array of double values sampled from the specified Beta distribution.</returns>
        /// 
        public static double[] Random(double alpha, double beta, int samples, Random source)
        {
            return Random(alpha, beta, samples, new double[samples], source);
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Beta distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="alpha">The shape parameter α (alpha).</param>
        /// <param name="beta">The shape parameter β (beta).</param>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        ///
        /// <returns>An array of double values sampled from the specified Beta distribution.</returns>
        /// 
        public static double[] Random(double alpha, double beta, int samples, double[] result)
        {
            return Random(alpha, beta, samples, result, Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Beta distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="alpha">The shape parameter α (alpha).</param>
        /// <param name="beta">The shape parameter β (beta).</param>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        ///
        /// <returns>An array of double values sampled from the specified Beta distribution.</returns>
        /// 
        public static double[] Random(double alpha, double beta, int samples, double[] result, Random source)
        {
            double[] x = GammaDistribution.Random(alpha, 1, samples, result, source);
            double[] y = GammaDistribution.Random(beta, 1, samples, source);

            for (int i = 0; i < x.Length; i++)
                result[i] = x[i] / (x[i] + y[i]);

            return result;
        }

        /// <summary>
        ///   Generates a random observation from the 
        ///   Beta distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="alpha">The shape parameter α (alpha).</param>
        /// <param name="beta">The shape parameter β (beta).</param>
        /// 
        /// <returns>A random double value sampled from the specified Beta distribution.</returns>
        /// 
        public static double Random(double alpha, double beta)
        {
            return Random(alpha, beta, Accord.Math.Random.Generator.Random);
        }

        /// <summary>
        ///   Generates a random observation from the 
        ///   Beta distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="alpha">The shape parameter α (alpha).</param>
        /// <param name="beta">The shape parameter β (beta).</param>
        /// <param name="source">The random number generator to use as a source of randomness. 
        ///   Default is to use <see cref="Accord.Math.Random.Generator.Random"/>.</param>
        /// 
        /// <returns>A random double value sampled from the specified Beta distribution.</returns>
        /// 
        public static double Random(double alpha, double beta, Random source)
        {
            double x = GammaDistribution.Random(alpha, 1, source);
            double y = GammaDistribution.Random(beta, 1, source);

            return x / (x + y);
        }

        #endregion


        /// <summary>
        ///   Estimates a new Beta distribution from a set of observations.
        /// </summary>
        /// 
        public static BetaDistribution Estimate(double[] samples)
        {
            var beta = new BetaDistribution(1, 1);
            beta.Fit(samples);
            return beta;
        }

        /// <summary>
        ///   Estimates a new Beta distribution from a set of weighted observations.
        /// </summary>
        /// 
        public static BetaDistribution Estimate(double[] samples, double[] weights)
        {
            var beta = new BetaDistribution(1, 1);
            beta.Fit(samples, weights, null);
            return beta;
        }

        /// <summary>
        ///   Estimates a new Beta distribution from a set of weighted observations.
        /// </summary>
        /// 
        public static BetaDistribution Estimate(double[] samples, double[] weights, BetaOptions options)
        {
            var beta = new BetaDistribution(1, 1);
            beta.Fit(samples, weights, options);
            return beta;
        }

        /// <summary>
        ///   Estimates a new Beta distribution from a set of observations.
        /// </summary>
        /// 
        public static BetaDistribution Estimate(double[] samples, BetaOptions options)
        {
            var beta = new BetaDistribution(1, 1);
            beta.Fit(samples, (double[])null, options);
            return beta;
        }
    }
}
