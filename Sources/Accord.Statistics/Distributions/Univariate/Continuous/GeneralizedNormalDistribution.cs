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
    using Accord.Compat;

    /// <summary>
    ///   Generalized Normal distribution (also known as Exponential Power distribution).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The generalized normal distribution or generalized Gaussian distribution
    ///   (GGD) is either of two families of parametric continuous probability 
    ///   distributions on the real line. Both families add a shape parameter to
    ///   the normal distribution. To distinguish the two families, they are referred
    ///   to below as "version 1" and "version 2". However this is not a standard 
    ///   nomenclature.</para>
    /// <para>
    ///   Known also as the exponential power distribution, or the generalized error
    ///   distribution, this is a parametric family of symmetric distributions. It includes
    ///   all normal and Laplace distributions, and as limiting cases it includes all 
    ///   continuous uniform distributions on bounded intervals of the real line.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://en.wikipedia.org/wiki/Generalized_normal_distribution">
    ///       Wikipedia, The Free Encyclopedia. Generalized normal distribution. Available on: 
    ///       https://en.wikipedia.org/wiki/Generalized_normal_distribution </a></description></item>
    ///   </list></para> 
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   This examples shows how to create a Generalized normal distribution
    ///   and compute some of its properties.</para>
    ///   
    /// <code>
    /// // Creates a new generalized normal distribution with the given parameters
    /// var normal = new GeneralizedNormalDistribution(location: 1, scale: 5, shape: 0.42);
    /// 
    /// double mean = normal.Mean;     // 1
    /// double median = normal.Median; // 1
    /// double mode = normal.Mode;     // 1
    /// double var = normal.Variance;  // 19200.781700666659
    /// 
    /// double cdf = normal.DistributionFunction(x: 1.4); // 0.51076148867681703
    /// double pdf = normal.ProbabilityDensityFunction(x: 1.4); // 0.024215092283124507
    /// double lpdf = normal.LogProbabilityDensityFunction(x: 1.4); // -3.7207791921441378
    /// 
    /// double ccdf = normal.ComplementaryDistributionFunction(x: 1.4); // 0.48923851132318297
    /// double icdf = normal.InverseDistributionFunction(p: cdf); // 1.4000000149740108
    /// 
    /// double hf = normal.HazardFunction(x: 1.4); // 0.049495474543966168
    /// double chf = normal.CumulativeHazardFunction(x: 1.4); // 0.7149051552030572
    /// 
    /// string str = normal.ToString(CultureInfo.InvariantCulture); // GGD(x; μ = 1, α = 5, β = 0.42)
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="NormalDistribution"/>
    /// <seealso cref="LaplaceDistribution"/>
    /// 
    [Serializable]
    public class GeneralizedNormalDistribution : UnivariateContinuousDistribution
    {

        // Distribution parameters
        private double mean = 0;  // location μ
        private double alpha = 1; // scale α
        private double beta = 0;  // shape β

        /// <summary>
        ///   Constructs a Generalized Normal distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="location">The location parameter μ.</param>
        /// <param name="scale">The scale parameter α.</param>
        /// <param name="shape">The shape parameter β.</param>
        /// 
        public GeneralizedNormalDistribution([Real] double location, 
            [Positive] double scale, [Positive] double shape)
        {
            initialize(location, scale, shape);
        }

        /// <summary>
        ///   Create an <see cref="LaplaceDistribution"/> distribution using a 
        ///   <see cref="GeneralizedNormalDistribution"/> specialization.
        /// </summary>
        /// 
        /// <param name="location">The Laplace's location parameter μ (mu).</param>
        /// <param name="scale">The Laplace's scale parameter b.</param>
        /// 
        /// <returns>A <see cref="GeneralizedNormalDistribution"/> that provides
        ///  a <see cref="LaplaceDistribution"/>.</returns>
        /// 
        public static GeneralizedNormalDistribution Laplace([Real] double location, [Positive] double scale)
        {
            return new GeneralizedNormalDistribution(location, scale, 1);
        }

        /// <summary>
        ///   Create an <see cref="NormalDistribution"/> distribution using a 
        ///   <see cref="GeneralizedNormalDistribution"/> specialization.
        /// </summary>
        /// 
        /// <param name="mean">The Normal's mean parameter μ (mu).</param>
        /// <param name="stdDev">The Normal's standard deviation σ (sigma).</param>
        /// 
        /// <returns>A <see cref="GeneralizedNormalDistribution"/> that provides
        ///  a <see cref="NormalDistribution"/> distribution.</returns>
        /// 
        public static GeneralizedNormalDistribution Normal(double mean, double stdDev)
        {
            return new GeneralizedNormalDistribution(mean, Constants.Sqrt2 * stdDev, 2);
        }

        /// <summary>
        ///   Gets the location value μ (mu) for the distribution.
        /// </summary>
        /// 
        public override double Mean
        {
            get { return mean; }
        }

        /// <summary>
        ///   Gets the median for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's median value.
        /// </value>
        /// 
        public override double Median
        {
            get
            {
                Accord.Diagnostics.Debug.Assert(mean.IsEqual(base.Median, 1e-5));
                return mean;
            }
        }

        /// <summary>
        ///   Gets the mode for this distribution.
        /// </summary>
        /// 
        /// <remarks>
        ///   In the Generalized Normal Distribution, the mode is
        ///   equal to the distribution's <see cref="Mean"/> value.
        /// </remarks>
        /// 
        /// <value>
        ///   The distribution's mode value.
        /// </value>
        /// 
        public override double Mode
        {
            get { return mean; }
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
            get { return alpha * alpha * Gamma.Function(3.0 / beta) / Gamma.Function(1.0 / beta); }
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
        ///   Gets the Entropy for this Normal distribution.
        /// </summary>
        /// 
        public override double Entropy
        {
            get
            {
                double a = 1.0 / beta;
                double b = beta / (2 * alpha * Gamma.Function(a));
                return a - Math.Log(b);
            }
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for the
        ///   Generalized Normal distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">
        ///   A single point in the distribution range.</param>
        ///   
        /// <remarks>
        /// <para>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.</para>
        /// </remarks>
        /// 
        /// <example>
        ///   See <see cref="GeneralizedNormalDistribution"/>.
        /// </example>
        /// 
        protected internal override double InnerDistributionFunction(double x)
        {
            double z = x - mean;
            double w = Math.Abs(z) / alpha;

            double u = Math.Pow(w, beta);
            double b = Gamma.LowerIncomplete(1.0 / beta, u);

            // The regularized Gamma function P is defined as the incomplete 
            // Gamma function divided by the Gamma function itself. This is 
            // exactly part of the formula indicated on Wikipedia's page on 
            // the Generalized Normal Distribution, in the CDF's section.

            double r = 0.5 + 0.5 * Math.Sign(z) * b;

            return r;
        }


        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   the Normal distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range. For a
        ///   univariate distribution, this should be a single double value.
        ///   For a multivariate distribution, this should be a double array.</param>
        ///   
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.
        /// </returns>
        /// 
        /// <example>
        ///   See <see cref="GeneralizedNormalDistribution"/>.
        /// </example> 
        ///
        protected internal override double InnerProbabilityDensityFunction(double x)
        {
            double z = Math.Abs(x - mean) / alpha;

            double a = beta / (2 * alpha * Gamma.Function(1 / beta));
            double b = Math.Pow(z, beta);

            return a * Math.Exp(-b);
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
            return new GeneralizedNormalDistribution(mean, alpha, beta);
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
            return String.Format(formatProvider, "GGD(x; μ = {0}, α = {1}, β = {2})",
                mean.ToString(format, formatProvider),
                alpha.ToString(format, formatProvider),
                beta.ToString(format, formatProvider));
        }


        private void initialize(double mu, double alpha, double beta)
        {
            this.mean = mu;
            this.alpha = alpha;
            this.beta = beta;
        }

    }
}
