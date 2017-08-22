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
    ///   Logistic distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In probability theory and statistics, the logistic distribution is a continuous
    ///   probability distribution. Its cumulative distribution function is the logistic 
    ///   function, which appears in logistic regression and feedforward neural networks.
    ///   It resembles the normal distribution in shape but has heavier tails (higher 
    ///   kurtosis). The <see cref="TukeyLambdaDistribution">Tukey lambda distribution</see>
    ///   can be considered a generalization of the logistic distribution since it adds a
    ///   shape parameter, λ (the Tukey distribution becomes logistic when λ is zero).</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Logistic_distribution">
    ///       Wikipedia, The Free Encyclopedia. Logistic distribution. Available on: 
    ///       http://en.wikipedia.org/wiki/Logistic_distribution </a></description></item>
    ///   </list></para> 
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   This examples shows how to create a Logistic distribution,
    ///   compute some of its properties and generate a number of
    ///   random samples from it.</para>
    ///   
    /// <code>
    /// // Create a logistic distribution with μ = 0.42 and scale = 3
    /// var log = new LogisticDistribution(location: 0.42, scale: 1.2);
    /// 
    /// double mean = log.Mean;     // 0.42
    /// double median = log.Median; // 0.42
    /// double mode = log.Mode;     // 0.42
    /// double var = log.Variance;  // 4.737410112522892
    /// 
    /// double cdf = log.DistributionFunction(x: 1.4); // 0.693528308197921
    /// double pdf = log.ProbabilityDensityFunction(x: 1.4); // 0.17712232827170876
    /// double lpdf = log.LogProbabilityDensityFunction(x: 1.4); // -1.7309146649427332
    /// 
    /// double ccdf = log.ComplementaryDistributionFunction(x: 1.4); // 0.306471691802079
    /// double icdf = log.InverseDistributionFunction(p: cdf); // 1.3999999999999997
    /// 
    /// double hf = log.HazardFunction(x: 1.4); // 0.57794025683160088
    /// double chf = log.CumulativeHazardFunction(x: 1.4); // 1.1826298874077226
    /// 
    /// string str = log.ToString(CultureInfo.InvariantCulture); // Logistic(x; μ = 0.42, scale = 1.2)
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="TukeyLambdaDistribution"/>
    /// 
    [Serializable]
    public class LogisticDistribution : UnivariateContinuousDistribution
    {

        // Distribution parameters
        private double mu;   // location μ
        private double s;    // scale s

        /// <summary>
        ///   Constructs a Logistic distribution
        ///   with zero location and unit scale.
        /// </summary>
        /// 
        public LogisticDistribution()
        {
            initialize(0, 1);
        }

        /// <summary>
        ///   Constructs a Logistic distribution
        ///   with given location and unit scale.
        /// </summary>
        /// 
        /// <param name="location">The distribution's location value μ (mu).</param>
        /// 
        public LogisticDistribution([Real] double location)
        {
            initialize(location, 1);
        }

        /// <summary>
        ///   Constructs a Logistic distribution
        ///   with given location and scale parameters.
        /// </summary>
        /// 
        /// <param name="location">The distribution's location value μ (mu).</param>
        /// <param name="scale">The distribution's scale value s.</param>
        /// 
        public LogisticDistribution([Real] double location, [Positive] double scale)
        {
            if (scale <= 0)
                throw new ArgumentOutOfRangeException("scale", "Scale must be positive.");

            initialize(location, scale);
        }

        /// <summary>
        ///   Gets the location value μ (mu).
        /// </summary>
        /// 
        public double Location { get { return mu; } }

        /// <summary>
        ///   Gets the location value μ (mu).
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's mean value.
        /// </value>
        /// 
        public override double Mean
        {
            get { return mu; }
        }

        /// <summary>
        ///   Gets the distribution's scale value (s).
        /// </summary>
        /// 
        public double Scale
        {
            get { return s; }
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
                Accord.Diagnostics.Debug.Assert(mu == base.Median);
                return mu;
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
            get { return (s * s * Math.PI * Math.PI) / 3.0; }
        }

        /// <summary>
        ///   Gets the mode for this distribution.
        /// </summary>
        /// 
        /// <remarks>
        ///   In the logistic distribution, the mode is equal
        ///   to the distribution <see cref="Mean"/> value.
        /// </remarks>
        /// 
        /// <value>
        ///   The distribution's mode value.
        /// </value>
        /// 
        public override double Mode
        {
            get { return mu; }
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
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <remarks>
        ///   In the logistic distribution, the entropy is 
        ///   equal to <c>ln(<see cref="Scale">s</see>) + 2</c>.
        /// </remarks>
        /// 
        /// <value>
        ///   The distribution's entropy.
        /// </value>
        /// 
        public override double Entropy
        {
            get { return Math.Log(s) + 2; }
        }


        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        protected internal override double InnerDistributionFunction(double x)
        {
            double z = (x - mu) / s;

            return 1.0 / (1 + Math.Exp(-z));
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
        protected internal override double InnerProbabilityDensityFunction(double x)
        {
            double z = (x - mu) / s;

            double num = Math.Exp(-z);
            double a = (1 + num);
            double den = s * a * a;

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
        protected internal override double InnerLogProbabilityDensityFunction(double x)
        {
            double z = (x - mu) / s;

            double result = -z - (Math.Log(s) + 2 * Special.Log1p(Math.Exp(-z)));

            return result;
        }

        /// <summary>
        ///   Gets the inverse of the cumulative distribution function (icdf) for
        ///   this distribution evaluated at probability <c>p</c>. This function
        ///   is also known as the Quantile function.
        /// </summary>
        /// 
        /// <param name="p">A probability value between 0 and 1.</param>
        /// 
        /// <returns>
        ///   A sample which could original the given probability
        ///   value when applied in the <see cref="UnivariateContinuousDistribution.DistributionFunction(double)"/>.
        /// </returns>
        /// 
        protected internal override double InnerInverseDistributionFunction(double p)
        {
            return mu + s * Math.Log(p / (1 - p));
        }

        /// <summary>
        ///   Gets the first derivative of the <see cref="UnivariateContinuousDistribution.InverseDistributionFunction(double)">
        ///   inverse distribution function</see> (icdf) for this distribution evaluated
        ///   at probability <c>p</c>. 
        /// </summary>
        /// 
        /// <param name="p">A probability value between 0 and 1.</param>
        /// 
        public override double QuantileDensityFunction(double p)
        {
            return s / (p * (1 - p));
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
            return new LogisticDistribution(mu, s);
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
            return String.Format(formatProvider, "Logistic(x; μ = {0}, s = {1})",
                mu.ToString(format, formatProvider),
                s.ToString(format, formatProvider));
        }



        private void initialize(double mean, double scale)
        {
            this.mu = mean;
            this.s = scale;
        }
    }
}
