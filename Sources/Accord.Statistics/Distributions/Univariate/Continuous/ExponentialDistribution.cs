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
    ///   Exponential distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In probability theory and statistics, the exponential distribution (a.k.a. negative
    ///   exponential distribution) is a family of continuous probability distributions. It 
    ///   describes the time between events in a Poisson process, i.e. a process in which events 
    ///   occur continuously and independently at a constant average rate. It is the continuous 
    ///   analogue of the geometric distribution.</para>
    /// <para>
    ///   Note that the exponential distribution is not the same as the class of exponential 
    ///   families of distributions, which is a large class of probability distributions that
    ///   includes the exponential distribution as one of its members, but also includes the 
    ///   normal distribution, binomial distribution, gamma distribution, Poisson, and many 
    ///   others.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Exponential_distribution">
    ///       Wikipedia, The Free Encyclopedia. Exponential distribution. Available on: 
    ///       http://en.wikipedia.org/wiki/Exponential_distribution </a></description></item>
    ///   </list></para>     
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example shows how to create and test the main characteristics
    ///   of an Exponential distribution given a lambda (λ) rate of 0.42: </para>
    ///   
    /// <code>
    ///   // Create an Exponential distribution with λ = 0.42
    ///   var exp = new ExponentialDistribution(rate: 0.42);
    ///   
    ///   // Common measures
    ///   double mean = exp.Mean;     // 2.3809523809523809
    ///   double median = exp.Median; // 1.6503504299046317
    ///   double var = exp.Variance;  // 5.6689342403628125
    ///   
    ///   // Cumulative distribution functions
    ///   double cdf  = exp.DistributionFunction(x: 0.27);          // 0.10720652870550407
    ///   double ccdf = exp.ComplementaryDistributionFunction(x: 0.27); // 0.89279347129449593
    ///   double icdf = exp.InverseDistributionFunction(p: cdf);        // 0.27
    ///   
    ///   // Probability density functions
    ///   double pdf  = exp.ProbabilityDensityFunction(x: 0.27);    // 0.3749732579436883
    ///   double lpdf = exp.LogProbabilityDensityFunction(x: 0.27); // -0.98090056770472311
    ///   
    ///   // Hazard (failure rate) functions
    ///   double hf = exp.HazardFunction(x: 0.27);            // 0.42
    ///   double chf = exp.CumulativeHazardFunction(x: 0.27); // 0.1134
    ///   
    ///   // String representation
    ///   string str = exp.ToString(CultureInfo.InvariantCulture); // Exp(x; λ = 0.42)
    /// </code>
    /// 
    /// <para>
    ///   The following example shows how to generate random samples drawn
    ///   from an Exponential distribution and later how to re-estimate a
    ///   distribution from the generated samples.</para>
    ///   
    /// <code>
    ///   // Create an Exponential distribution with λ = 2.5
    ///   var exp = new ExponentialDistribution(rate: 2.5);
    /// 
    ///   // Generate a million samples from this distribution:
    ///   double[] samples = target.Generate(1000000);
    /// 
    ///   // Create a default exponential distribution
    ///   var newExp = new ExponentialDistribution();
    /// 
    ///   // Fit the samples
    ///   newExp.Fit(samples);
    /// 
    ///   // Check the estimated parameters
    ///   double rate = newExp.Rate; // 2.5
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class ExponentialDistribution : UnivariateContinuousDistribution,
        IFittableDistribution<double, IFittingOptions>,
        ISampleableDistribution<double>
    {

        // Distribution parameters
        double lambda; // λ 

        // Derived measures
        double lnlambda;


        /// <summary>
        ///   Creates a new Exponential distribution with the given rate.
        /// </summary>
        /// 
        /// <param name="rate">The rate parameter lambda (λ).</param>
        /// 
        public ExponentialDistribution(double rate)
        {
            init(rate);
        }

        private void init(double rate)
        {
            this.lambda = rate;

            this.lnlambda = Math.Log(rate);
        }

        /// <summary>
        ///   Gets the distribution's rate parameter lambda (λ).
        /// </summary>
        /// 
        public double Rate
        {
            get { return lambda; }
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mean value.</value>
        /// 
        /// <remarks>
        ///   In the Exponential distribution, the mean is defined as 1/λ.
        /// </remarks>
        /// 
        public override double Mean
        {
            get { return 1.0 / lambda; }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's variance.</value>
        /// 
        /// <remarks>
        ///   In the Exponential distribution, the variance is defined as 1/(λ²)
        /// </remarks>
        /// 
        public override double Variance
        {
            get { return 1.0 / (lambda * lambda); }
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
        ///   Gets the median for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's median value.</value>
        /// 
        /// <remarks>
        ///   In the Exponential distribution, the median is defined as ln(2) / λ.
        /// </remarks>
        /// 
        public override double Median
        {
            get
            {
                double median = Math.Log(2) / lambda;
                System.Diagnostics.Debug.Assert(median == base.Median);
                return median;
            }
        }

        /// <summary>
        ///   Gets the mode for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mode value.</value>
        /// 
        /// <remarks>
        ///   In the Exponential distribution, the median is defined as 0.
        /// </remarks>
        /// 
        public override double Mode
        {
            get { return 0; }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's entropy.</value>
        /// 
        /// <remarks>
        ///   In the Exponential distribution, the median is defined as 1 - ln(λ).
        /// </remarks>
        /// 
        public override double Entropy
        {
            get { return 1 - Math.Log(lambda); }
        }

        /// <summary>
        ///  Gets the cumulative distribution function (cdf) for
        ///  this distribution evaluated at point <c>x</c>.
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
        ///   The Exponential CDF is defined as CDF(x) = 1 - exp(-λ*x).</para>
        /// </remarks>
        /// 
        public override double DistributionFunction(double x)
        {
            return 1.0 - Math.Exp(-lambda * x);
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
        ///   
        /// <para>
        ///   The Exponential PDF is defined as PDF(x) = λ * exp(-λ*x).</para>
        /// </remarks>
        /// 
        public override double ProbabilityDensityFunction(double x)
        {
            return lambda * Math.Exp(-lambda * x);
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
        /// <seealso cref="ProbabilityDensityFunction"/>
        /// 
        public override double LogProbabilityDensityFunction(double x)
        {
            return lnlambda - lambda * x;
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
        /// 
        /// <para>
        ///   The Exponential ICDF is defined as ICDF(p) = -ln(1-p)/λ.</para>
        /// </remarks>
        /// 
        public override double InverseDistributionFunction(double p)
        {
            double icdf = -Math.Log(1 - p) / lambda;
            System.Diagnostics.Debug.Assert(icdf.IsRelativelyEqual(base.InverseDistributionFunction(p), 1e-6));
            return icdf;
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
        /// as regularization constants and additional parameters.</param>
        /// 
        /// <remarks>
        ///   Although both double[] and double[][] arrays are supported,
        ///   providing a double[] for a multivariate distribution or a
        ///   double[][] for a univariate distribution may have a negative
        ///   impact in performance.
        /// </remarks>
        /// 
        /// <example>
        ///   Please see <see cref="ExponentialDistribution"/>.
        /// </example>
        /// 
        public override void Fit(double[] observations, double[] weights, IFittingOptions options)
        {
            if (options != null)
                throw new ArgumentException("This method does not accept fitting options.");


            double lambda;

            if (weights == null)
            {
                lambda = 1.0 / Accord.Statistics.Tools.Mean(observations);
            }
            else
            {
                lambda = 1.0 / Accord.Statistics.Tools.WeightedMean(observations, weights);
            }

            init(lambda);
        }

        private ExponentialDistribution() { }

        /// <summary>
        ///   Estimates a new Exponential distribution from a given set of observations.
        /// </summary>
        /// 
        public static ExponentialDistribution Estimate(double[] observations)
        {
            return Estimate(observations, null);
        }

        /// <summary>
        ///   Estimates a new Exponential distribution from a given set of observations.
        /// </summary>
        /// 
        public static ExponentialDistribution Estimate(double[] observations, double[] weights)
        {
            var n = new ExponentialDistribution();
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
            return new ExponentialDistribution(lambda);
        }


        #region ISamplableDistribution<double> Members

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
            return Random(lambda, samples);
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public double Generate()
        {
            return Random(lambda);
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Exponential distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="lambda">The rate parameter lambda.</param>
        /// <param name="samples">The number of samples to generate.</param>
        ///
        /// <returns>An array of double values sampled from the specified Exponential distribution.</returns>
        /// 
        public static double[] Random(double lambda, int samples)
        {
            double[] r = new double[samples];
            for (int i = 0; i < r.Length; i++)
            {
                double u = Accord.Math.Tools.Random.NextDouble();
                r[i] = -Math.Log(u) / lambda;
            }

            return r;
        }

        /// <summary>
        ///   Generates a random observation from the 
        ///   Exponential distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="lambda">The rate parameter lambda.</param>
        /// 
        /// <returns>A random double value sampled from the specified Exponential distribution.</returns>
        /// 
        public static double Random(double lambda)
        {
            double u = Accord.Math.Tools.Random.NextDouble();
            return -Math.Log(u) / lambda;
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
            return String.Format("Exp(x; λ = {0})", lambda);
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
            return String.Format(formatProvider, "Exp(x; λ = {0})", lambda);
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
            return String.Format("Exp(x; λ = {0})",
                lambda.ToString(format, formatProvider));
        }
    }
}
