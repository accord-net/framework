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

namespace Accord.Statistics.Distributions.Univariate.Continuous
{
    using System;
    using Accord.Math;

    /// <summary>
    ///   Beta prime distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    /// the beta prime distribution (also known as inverted beta distribution or beta distribution of the second kind[1]) is an absolutely continuous probability distribution defined for x > 0 with two parameters α and β, having the probability density function:
    /// f(x) = \frac{x^{\alpha-1} (1+x)^{-\alpha -\beta}}{B(\alpha,\beta)}
    /// where B is a Beta function. While the related beta distribution is the conjugate prior distribution of the parameter of a Bernoulli distribution expressed as a probability, the beta prime distribution is the conjugate prior distribution of the parameter of a Bernoulli distribution expressed in odds. The distribution is a Pearson type VI distribution.
    ///
    /// <para>
    ///   A good example of the use of the Beta Prime distribution is the storage volume of a reservoir of capacity zmax whose upper bound is zmax and lower bound is 0 (Fletcher & Ponnambalam, 1996).</para>
    ///
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
    /// 
    ///     double alpha = 4.0d;
    ///     double beta = 6.0d;
    ///     
    ///     var betaPrimeDist = new BetaPrimeDistribution(alpha, beta);
    ///     double mean = betaPrimeDist.Mean; // 0.8
    ///     double variance = betaPrimeDist.Variance; //0.36
    ///     double mode = betaPrimeDist.Mode; //0.42857142857142855
    ///     double pdf = betaPrimeDist.ProbabilityDensityFunction(4.0d); //0.0033030143999999975
    ///     double cdf = betaPrimeDist.DistributionFunction(4.0d); //0.996933632
    ///     string tostr = betaPrimeDist.ToString(); //BetaPrime(x; α = 4, β = 6)
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class BetaPrimeDistribution : UnivariateContinuousDistribution
    {


        // Distribution parameters
        double alpha; // shape (α)
        double beta; // shape (β)

        public BetaPrimeDistribution([Positive] double alpha, [Positive] double beta)
        {
            if (alpha <= 0.0d || beta <= 0.0d)
            {
                throw new ArgumentOutOfRangeException(string.Format("α and β must be positive: α={0}, β={1}", alpha, beta));
            }
            this.alpha = alpha;
            this.beta = beta;
        }


        public override double Mean
        {
            get 
            {
                if (beta > 1.0d) { 
                        return ( alpha / ( beta - 1.0d ) );
                }
                return double.PositiveInfinity;
            }
        }

        public override double Mode
        {
            get
            {
                if (alpha >= 1.0d) {
                    return ( alpha - 1.0d ) /
                           ( beta + 1.0d );
                }
                return 0.0d;
            }
        }

        public override double Variance
        {
            get {

                if (beta > 2.0)
                {
                    return  alpha * (alpha + beta - 1.0d) /
                            ((beta - 2.0d) * Math.Pow(beta - 1.0, 2.0d));
                }
                else if (beta > 1.0d) {
                    return double.PositiveInfinity;
                }

                return double.NaN;
            }
        }

        public override double Entropy
        {
            get { return double.NaN; }
        }

        public override AForge.DoubleRange Support
        {
            get { return new AForge.DoubleRange(0.0d, double.MaxValue); }
        }

        public override double DistributionFunction([Positive] double x)
        {
            if (x <= 0.0d) { throw new ArgumentOutOfRangeException(string.Format("x must be strictly positive: x={0}", x)); }
            return Beta.Incomplete( alpha, beta, x / ( 1.0d + x ) );
        }

        public override double ProbabilityDensityFunction([Positive] double x)
        {
            if (x <= 0.0d) { throw new ArgumentOutOfRangeException(string.Format("x must be strictly positive: x={0}", x)); }
            return 
                ( Math.Pow(x, alpha - 1.0d) * Math.Pow( 1.0d + x, -1.0d * alpha - beta ) ) /
                ( Beta.Function( alpha, beta) );
        }

        public override object Clone()
        {
            return new BetaPrimeDistribution( this.alpha, this.beta );
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
            return String.Format("BetaPrime(x; α = {0}, β = {1})", alpha, beta);
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
            return String.Format(formatProvider, "BetaPrime(x; α = {0}, β = {1})",
                alpha, beta);
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
            return String.Format("BetaPrime(x; α = {0}, β = {1})",
                alpha.ToString(format, formatProvider),
                beta.ToString(format, formatProvider));
        }

    }
}
