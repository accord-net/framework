// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Ashley Messer, 2014
// glyphard at gmail.com
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
    using Accord.Compat;

    /// <summary>
    ///   Beta prime distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In probability theory and statistics, the beta prime distribution (also known as inverted 
    ///   beta distribution or beta distribution of the second kind) is an absolutely continuous 
    ///   probability distribution defined for <c>x > 0</c> with two parameters α and β, having the
    ///   probability density function:</para>
    ///   
    /// <code>
    ///           x^(α-1) (1+x)^(-α-β)
    ///   f(x) =  --------------------
    ///                 B(α,β)
    /// </code>
    /// 
    /// <para>
    ///   where B is the <see cref="Beta">Beta function</see>. While the related beta distribution is
    ///   the conjugate prior distribution of the parameter of a <see cref="BernoulliDistribution">Bernoulli
    ///   distribution</see> expressed as a probability, the beta prime distribution is the conjugate prior
    ///   distribution of the parameter of a Bernoulli distribution expressed in odds. The distribution is 
    ///   a Pearson type VI distribution.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Beta_prime_distribution">
    ///       Wikipedia, The Free Encyclopedia. Beta Prime distribution. Available on: 
    ///       http://en.wikipedia.org/wiki/Beta_prime_distribution </a></description></item>
    ///   </list></para>     
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example shows how to create and test the main characteristics
    ///   of an Beta prime distribution given its two non-negative shape parameters: </para>
    ///   
    /// <code>
    /// // Create a new Beta-Prime distribution with shape (4,2)
    /// var betaPrime = new BetaPrimeDistribution(alpha: 4, beta: 2);
    /// 
    /// double mean = betaPrime.Mean;     // 4.0
    /// double median = betaPrime.Median; // 2.1866398762435981
    /// double mode = betaPrime.Mode;     // 1.0
    /// double var = betaPrime.Variance;  // +inf
    /// 
    /// double cdf = betaPrime.DistributionFunction(x: 0.4);           // 0.02570357589099781
    /// double pdf = betaPrime.ProbabilityDensityFunction(x: 0.4);     // 0.16999719504628183
    /// double lpdf = betaPrime.LogProbabilityDensityFunction(x: 0.4); // -1.7719733417957513
    /// 
    /// double ccdf = betaPrime.ComplementaryDistributionFunction(x: 0.4); // 0.97429642410900219
    /// double icdf = betaPrime.InverseDistributionFunction(p: cdf);       // 0.39999982363709291
    /// 
    /// double hf = betaPrime.HazardFunction(x: 0.4);            // 0.17448200654307533
    /// double chf = betaPrime.CumulativeHazardFunction(x: 0.4); // 0.026039684773113869
    /// 
    /// string str = betaPrime.ToString(CultureInfo.InvariantCulture); // BetaPrime(x; α = 4, β = 2)
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class BetaPrimeDistribution : UnivariateContinuousDistribution
    {

        // Distribution parameters
        double alpha; // shape (α)
        double beta;  // shape (β)

        /// <summary>
        ///   Constructs a new Beta-Prime distribution with the given 
        ///   two non-negative shape parameters <c>a</c> and <c>b</c>.
        /// </summary>
        /// 
        /// <param name="alpha">The distribution's non-negative shape parameter a.</param>
        /// <param name="beta">The distribution's non-negative shape parameter b.</param>
        /// 
        public BetaPrimeDistribution([Positive] double alpha, [Positive] double beta)
        {
            if (alpha <= 0)
                throw new ArgumentOutOfRangeException("alpha", "The shape parameter alpha must be positive.");

            if (beta < 0)
                throw new ArgumentOutOfRangeException("beta", "The shape parameter beta must be positive.");


            this.alpha = alpha;
            this.beta = beta;
        }

        /// <summary>
        ///   Gets the distribution's non-negative shape parameter a.
        /// </summary>
        /// 
        public double Alpha
        {
            get { return alpha; }
        }

        /// <summary>
        ///   Gets the distribution's non-negative shape parameter b.
        /// </summary>
        /// 
        public double Beta
        {
            get { return beta; }
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's mean value.
        /// </value>
        /// 
        public override double Mean
        {
            get
            {
                if (beta > 1)
                    return alpha / (beta - 1);

                return Double.PositiveInfinity;
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
                if (alpha >= 1)
                    return (alpha - 1) / (beta + 1);

                return 0.0;
            }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's variance.
        /// </value>
        /// 
        public override double Variance
        {
            get
            {
                if (beta > 2.0)
                {
                    double num = alpha * (alpha + beta - 1);
                    double den = (beta - 2) * Math.Pow(beta - 1, 2);
                    return num / den;
                }
                else if (beta > 1.0)
                {
                    return Double.PositiveInfinity;
                }

                return double.NaN;
            }
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public override double Entropy
        {
            get { return double.NaN; }
        }

        /// <summary>
        ///   Gets the support interval for this distribution, which 
        ///   for the Beta- Prime distribution ranges from 0 to all 
        ///   positive numbers.
        /// </summary>
        /// 
        /// <value>
        ///   A <see cref="DoubleRange" /> containing
        ///   the support interval for this distribution.
        /// </value>
        /// 
        public override DoubleRange Support
        {
            get { return new DoubleRange(0, Double.PositiveInfinity); }
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
        protected internal override double InnerDistributionFunction([Positive] double x)
        {
            return Accord.Math.Beta.Incomplete(alpha, beta, x / (1 + x));
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
        protected internal override double InnerProbabilityDensityFunction([Positive] double x)
        {
            double num = Math.Pow(x, alpha - 1) * Math.Pow(1 + x, -alpha - beta);
            double den = Accord.Math.Beta.Function(alpha, beta);
            return num / den;
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
        protected internal override double InnerLogProbabilityDensityFunction([Positive] double x)
        {
            double num = (alpha - 1) * Math.Log(x) + (-alpha - beta) * Math.Log(1 + x);
            double den = Accord.Math.Beta.Log(alpha, beta);
            return num - den;
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
            return new BetaPrimeDistribution(this.alpha, this.beta);
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
            return String.Format(formatProvider, "BetaPrime(x; α = {0}, β = {1})",
                alpha.ToString(format, formatProvider),
                beta.ToString(format, formatProvider));
        }

    }
}
