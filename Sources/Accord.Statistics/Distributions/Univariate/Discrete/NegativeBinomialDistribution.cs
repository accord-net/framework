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

    /// <summary>
    ///   Negative Binomial distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The negative binomial distribution is a discrete probability distribution of the number 
    ///   of successes in a sequence of Bernoulli trials before a specified (non-random) number of
    ///   failures (denoted r) occur. For example, if one throws a die repeatedly until the third 
    ///   time “1” appears, then the probability distribution of the number of non-“1”s that had 
    ///   appeared will be negative binomial.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Negative_binomial_distribution">
    ///       Wikipedia, The Free Encyclopedia. Negative Binomial distribution. 
    ///       Available from: http://en.wikipedia.org/wiki/Negative_binomial_distribution </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///    // Create a new Negative Binomial distribution with r = 7 and p = 0.42
    ///    var dist = new NegativeBinomialDistribution(failures: 7, probability: 0.42);
    ///    
    ///    // Common measures
    ///    double mean = dist.Mean;     // 5.068965517241379
    ///    double median = dist.Median; // 5.0
    ///    double var = dist.Variance;  // 8.7395957193816862
    ///    
    ///    // Cumulative distribution functions
    ///    double cdf = dist.DistributionFunction(k: 2);               // 0.19605133020527743
    ///    double ccdf = dist.ComplementaryDistributionFunction(k: 2); // 0.80394866979472257
    ///    
    ///    // Probability mass functions
    ///    double pmf1 = dist.ProbabilityMassFunction(k: 4); // 0.054786846293416853
    ///    double pmf2 = dist.ProbabilityMassFunction(k: 5); // 0.069908015870399909
    ///    double pmf3 = dist.ProbabilityMassFunction(k: 6); // 0.0810932984096639
    ///    double lpmf = dist.LogProbabilityMassFunction(k: 2); // -2.3927801721315989
    ///    
    ///    // Quantile function
    ///    int icdf1 = dist.InverseDistributionFunction(p: 0.17); // 2
    ///    int icdf2 = dist.InverseDistributionFunction(p: 0.46); // 4
    ///    int icdf3 = dist.InverseDistributionFunction(p: 0.87); // 8
    ///    
    ///    // Hazard (failure rate) functions
    ///    double hf = dist.HazardFunction(x: 4); // 0.10490438293398294
    ///    double chf = dist.CumulativeHazardFunction(x: 4); // 0.64959916255036043
    ///    
    ///    // String representation
    ///    string str = dist.ToString(CultureInfo.InvariantCulture); // "NegativeBinomial(x; r = 7, p = 0.42)"
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="BinomialDistribution"/>
    /// 
    [Serializable]
    public class NegativeBinomialDistribution : UnivariateDiscreteDistribution
    {

        int r;    // number of failures
        double p; // success probability

        /// <summary>
        ///   Creates a new Negative Binomial distribution.
        /// </summary>
        /// 
        /// <param name="failures">Number of failures <c>r</c>.</param>
        /// <param name="probability">Success probability in each experiment.</param>
        /// 
        public NegativeBinomialDistribution(int failures, double probability)
        {
            if (failures <= 0)
                throw new ArgumentOutOfRangeException("failures");

            if (probability < 0 || probability > 1)
                throw new ArgumentOutOfRangeException("probability");

            this.r = failures;
            this.p = probability;
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mean value.</value>
        /// 
        public override double Mean
        {
            get { return (p * r) / (1 - p); }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's variance.</value>
        /// 
        public override double Variance
        {
            get { return (p * r) / ((1 - p) * (1 - p)); }
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
        ///   Gets P( X&lt;= k), the cumulative distribution function
        ///   (cdf) for this distribution evaluated at point <c>k</c>.
        /// </summary>
        /// 
        /// <param name="k">A single point in the distribution range.</param>
        /// 
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        /// 
        public override double DistributionFunction(int k)
        {
            if (k < 0) 
                return 0;

            return 1.0 - Beta.Incomplete(k + 1, r, p);
        }

        /// <summary>
        ///   Gets the probability mass function (pmf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="k">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The probability of <c>k</c> occurring
        ///   in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Mass Function (PMF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        public override double ProbabilityMassFunction(int k)
        {
            if (k < 0) return 0;
            return Special.Binomial(k + r - 1, r - 1) * Math.Pow(1 - p, k) * Math.Pow(p, r);
        }

        /// <summary>
        ///   Gets the log-probability mass function (pmf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="k">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The logarithm of the probability of <c>x</c>
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
            if (k <= 0) return Double.NegativeInfinity;
            return Special.LogBinomial(k + r - 1, r - 1) + k * Math.Log(1 - p) + r * Math.Log(p);
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
            return new NegativeBinomialDistribution(r, p);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        /// elements can be either of type double (for univariate data) or
        /// type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting, such
        /// as regularization constants and additional parameters.</param>
        /// 
        public override void Fit(double[] observations, double[] weights, Fitting.IFittingOptions options)
        {
            throw new NotSupportedException();
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
            return String.Format("NegativeBinomial(x; r = {0}, p = {1})", r, p);
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
            return String.Format(formatProvider, "NegativeBinomial(x; r = {0}, p = {1})", r, p);
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
            return String.Format("NegativeBinomial(x; r = {0}, p = {1})",
                r.ToString(format, formatProvider),
                p.ToString(format, formatProvider));
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
            return String.Format("NegativeBinomial(x; r = {0}, p = {1})",
                r.ToString(format), p.ToString(format));
        }
    }
}
