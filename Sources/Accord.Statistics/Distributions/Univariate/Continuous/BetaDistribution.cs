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
    ///  string str = distribution.ToString(CultureInfo.InvariantCulture); // "B(x; α = 0.42, β = 1.57)
    /// </code>
    /// 
    /// <para>
    ///   The following example shows to create a Beta distribution
    ///   given a discrete number of trials and the number of successess
    ///   within those trials. It also shows how to compute the 2.5 and
    ///   97.5 percentiles of the distribution:
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
    /// </para>
    /// </example>
    /// 
    /// <seealso cref="Accord.Math.Beta"/>
    ///
    [Serializable]
    public class BetaDistribution : UnivariateContinuousDistribution, IFormattable
    {
        double a; // alpha
        double b; // beta

        double constant;
        double? entropy;

        /// <summary>
        ///   Creates a new Beta distribution.
        /// </summary>
        /// 
        /// <param name="success">The number of success <c>r</c>.</param>
        /// <param name="trials">The number of trials <c>n</c>.</param>
        /// 
        public BetaDistribution(int success, int trials)
        {
            if (success < 0)
                throw new ArgumentOutOfRangeException("success", "The number of success must be positive");
            if (trials <= 0)
                throw new ArgumentOutOfRangeException("success", "The number of trials must be positive");

            if (success > trials)
                throw new ArgumentOutOfRangeException("success",
                    "The number of successes should be lesser than or equal to the number of trials");

            init(success + 1, trials - success + 1);
        }

        /// <summary>
        ///   Creates a new Beta distribution.
        /// </summary>
        /// 
        /// <param name="alpha">The shape parameter α (alpha).</param>
        /// <param name="beta">The shape parameter β (beta).</param>
        /// 
        public BetaDistribution(double alpha, double beta)
        {
            if (alpha <= 0) throw new ArgumentOutOfRangeException("alpha");
            if (beta <= 0) throw new ArgumentOutOfRangeException("beta");

            init(alpha, beta);
        }

        private void init(double alpha, double beta)
        {
            this.a = alpha;
            this.b = beta;

            constant = 1.0 / Accord.Math.Beta.Function(a, b);
        }

        /// <summary>
        ///   Gets the shape parameter α (alpha)
        /// </summary>
        /// 
        public double Alpha
        {
            get { return a; }
        }

        /// <summary>
        ///   Gets the shape parameter β (beta).
        /// </summary>
        /// 
        public double Beta
        {
            get { return b; }
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
            get { return a / (a + b); }
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
            get { return (a * b) / ((a + b) * (a + b) * (a + b + 1)); }
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
                    double lnBab = Math.Log(Accord.Math.Beta.Function(a, b));
                    double da = Gamma.Digamma(a);
                    double db = Gamma.Digamma(b);
                    double dab = Gamma.Digamma(a + b);
                    entropy = lnBab - (a - 1) * da - (b - 1) * db + (a + b - 2) * dab;
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
        /// <para>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.</para>
        ///   
        /// <para>
        ///   The Beta's CDF is computed using the <see cref="Accord.Math.Beta.Incomplete">Incomplete 
        ///   (regularized) Beta function I_x(a,b)</see> as CDF(x) = I_x(a,b)</para>
        /// </remarks>
        /// 
        public override double DistributionFunction(double x)
        {
            return Accord.Math.Beta.Incomplete(a, b, x);
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
        ///   value when applied in the <see cref="DistributionFunction"/>.</returns>
        /// 
        public override double InverseDistributionFunction(double p)
        {
            return Accord.Math.Beta.IncompleteInverse(a, b, p);
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
        public override double ProbabilityDensityFunction(double x)
        {
            if (x <= 0 || x >= 1) return 0;
            return constant * Math.Pow(x, a - 1) * Math.Pow(1 - x, b - 1);
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
        /// <seealso cref="ProbabilityDensityFunction"/>
        /// 
        public override double LogProbabilityDensityFunction(double x)
        {
            if (x <= 0 || x >= 1) return Double.NegativeInfinity;
            return Math.Log(constant) + (a - 1) * Math.Log(x) + (b - 1) * Math.Log(1 - x);
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
            return new BetaDistribution(a, b);
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
            return String.Format("B(x; α = {0}, β = {1})", a, b);
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
            return String.Format(formatProvider, "B(x; α = {0}, β = {1})", a, b);
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
            return String.Format("B(x; α = {0}, β = {1})",
                a.ToString(format, formatProvider), b.ToString(format, formatProvider));
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
            return String.Format("B(x; α = {0}, β = {1})",
                a.ToString(format), b.ToString(format));
        }
    }
}
