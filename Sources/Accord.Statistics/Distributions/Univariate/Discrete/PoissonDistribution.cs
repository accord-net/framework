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
    ///   Poisson probability distribution.
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>The Poisson distribution is a discrete probability distribution that
    ///   expresses the probability of a number of events occurring in a fixed
    ///   period of time if these events occur with a known average rate and
    ///   independently of the time since the last event.</para>
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Poisson_distribution">
    ///       Wikipedia, The Free Encyclopedia. Poisson distribution. Available on:
    ///       http://en.wikipedia.org/wiki/Poisson_distribution </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///    // Create a Poisson distribution with λ = 4.2
    ///    var dist = new PoissonDistribution(lambda: 4.2);
    ///    
    ///    // Common measures
    ///    double mean = dist.Mean;     // 4.2
    ///    double median = dist.Median; // 4.0
    ///    double var = dist.Variance;  // 4.2
    ///    
    ///    // Cumulative distribution functions
    ///    double cdf = dist.DistributionFunction(k: 2);               // 0.39488100648845126
    ///    double ccdf = dist.ComplementaryDistributionFunction(k: 2); // 0.60511899351154874
    ///    
    ///    // Probability mass functions
    ///    double pmf1 = dist.ProbabilityMassFunction(k: 4); // 0.19442365170822165
    ///    double pmf2 = dist.ProbabilityMassFunction(k: 5); // 0.1633158674349062
    ///    double pmf3 = dist.ProbabilityMassFunction(k: 6); // 0.11432110720443435
    ///    double lpmf = dist.LogProbabilityMassFunction(k: 2); // -2.0229781299813
    ///    
    ///    // Quantile function
    ///    int icdf1 = dist.InverseDistributionFunction(p: 0.17); // 2
    ///    int icdf2 = dist.InverseDistributionFunction(p: 0.46); // 4
    ///    int icdf3 = dist.InverseDistributionFunction(p: 0.87); // 7
    ///    
    ///    // Hazard (failure rate) functions
    ///    double hf = dist.HazardFunction(x: 4); // 0.19780423301883465
    ///    double chf = dist.CumulativeHazardFunction(x: 4); // 0.017238269667812049
    ///    
    ///    // String representation
    ///    string str = dist.ToString(CultureInfo.InvariantCulture); // "Poisson(x; λ = 4.2)"
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class PoissonDistribution : UnivariateDiscreteDistribution,
        IFittableDistribution<double, IFittingOptions>
    {

        // Distribution parameters
        private double lambda; // λ

        // Derived values
        private double epml;

        // Distribution measures
        private double? entropy;


        /// <summary>
        ///   Creates a new Poisson distribution with the given λ (lambda).
        /// </summary>
        /// 
        /// <param name="lambda">The Poisson's λ (lambda) parameter.</param>
        /// 
        public PoissonDistribution(double lambda)
        {
            if (lambda <= 0)
                throw new ArgumentOutOfRangeException("lambda", 
                    "Poisson's λ must be greater than zero.");

            initialize(lambda);
        }

        private void initialize(double lm)
        {
            this.lambda = lm;
            this.epml = Math.Exp(-lm);

            this.entropy = null;
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        public override double Mean
        {
            get { return lambda; }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        public override double Variance
        {
            get { return lambda; }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <remarks>
        ///   A closed form expression for the entropy of a Poisson
        ///   distribution is unknown. This property returns an approximation
        ///   for large lambda.
        /// </remarks>
        /// 
        public override double Entropy
        {
            get
            {
                if (!entropy.HasValue)
                {
                    entropy = 0.5 * System.Math.Log(2.0 * System.Math.PI * lambda)
                        - 1 / (12 * lambda)
                        - 1 / (24 * lambda * lambda)
                        - 19 / (360 * lambda * lambda * lambda);
                }

                return entropy.Value;
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
            get { return new DoubleRange(Double.Epsilon, Double.PositiveInfinity); }
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>k</c>.
        /// </summary>
        /// 
        /// <param name="k">
        ///   A single point in the distribution range.</param>
        ///   
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        /// 
        public override double DistributionFunction(int k)
        {
            return Gamma.LowerIncomplete(k + 1, lambda) / Special.Factorial(k);
        }

        /// <summary>
        ///   Gets the probability mass function (pmf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="k">
        ///   A single point in the distribution range.</param>
        ///   
        /// <remarks>
        ///   The Probability Mass Function (PMF) describes the
        ///   probability that a given value <c>k</c> will occur.
        /// </remarks>
        /// 
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.</returns>
        ///   
        public override double ProbabilityMassFunction(int k)
        {
            return (Math.Pow(lambda, k) / Special.Factorial(k)) * epml;
        }

        /// <summary>
        ///   Gets the log-probability mass function (pmf) for
        ///   this distribution evaluated at point <c>k</c>.
        /// </summary>
        /// 
        /// <param name="k">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The logarithm of the probability of <c>k</c>
        ///   occurring in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Mass Function (PMF) describes the
        ///   probability that a given value <c>k</c> will occur.
        /// </remarks>
        /// 
        public override double LogProbabilityMassFunction(int k)
        {
            return (k * Math.Log(lambda) - Special.LogFactorial(k)) - lambda;
        }

        /// <summary>
        ///   Gets the inverse of the cumulative distribution function (icdf) for
        ///   this distribution evaluated at probability <c>p</c>. This function
        ///   is also known as the Quantile function.
        /// </summary>
        /// 
        /// <param name="p">A probability value between 0 and 1.</param>
        /// 
        /// <returns>The observation which most likely generated <paramref name="p"/>.</returns>
        /// 
        /// <remarks>
        /// <para>
        ///   The Inverse Cumulative Distribution Function (ICDF) specifies, for
        ///   a given probability, the value which the random variable will be at,
        ///   or below, with that probability.</para>
        /// <para>
        ///   In Poisson's distribution, the Inverse CDF can be computed using
        ///   the <see cref="Gamma.Inverse">inverse Gamma function Γ'(a, x)</see>
        ///   as 
        ///             <code>icdf(p) = Γ'(λ, 1 - p)</code>
        ///   .</para>
        /// </remarks>
        public override int InverseDistributionFunction(double p)
        {
            double result = Gamma.Inverse(lambda, 1.0 - p);

            return (int)Math.Round(result);
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
                throw new ArgumentException("No options may be specified.");

            double mean;

            if (weights == null)
                mean = observations.Mean();
            else
                mean = observations.WeightedMean(weights);

            initialize(mean);
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
            return new PoissonDistribution(lambda);
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
            return String.Format("Poisson(x; λ = {0})", lambda);
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
            return String.Format(formatProvider, "Poisson(x; λ = {0})", lambda);
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
            return String.Format("Poisson(x; λ = {0})",
                lambda.ToString(format, formatProvider));
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
            return String.Format("Poisson(x; λ = {0})", lambda.ToString(format));
        }
    }
}
