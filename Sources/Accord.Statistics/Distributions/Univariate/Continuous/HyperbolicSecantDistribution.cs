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
    ///   Hyperbolic Secant distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In probability theory and statistics, the hyperbolic secant distribution is
    ///   a continuous probability distribution whose probability density function and
    ///   characteristic function are proportional to the hyperbolic secant function. 
    ///   The hyperbolic secant function is equivalent to the inverse hyperbolic cosine,
    ///   and thus this distribution is also called the inverse-cosh distribution.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Sech_distribution">
    ///       Wikipedia, The Free Encyclopedia. Hyperbolic secant distribution. Available on: 
    ///       http://en.wikipedia.org/wiki/Sech_distribution </a></description></item>
    ///   </list></para> 
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   This examples shows how to create a Sech distribution,
    ///   compute some of its properties and generate a number of
    ///   random samples from it.</para>
    ///   
    /// <code>
    /// // Create a new hyperbolic secant distribution
    /// var sech = new HyperbolicSecantDistribution();
    /// 
    /// double mean = sech.Mean;     // 0.0
    /// double median = sech.Median; // 0.0
    /// double mode = sech.Mode;     // 0.0
    /// double var = sech.Variance;  // 1.0
    /// 
    /// double cdf = sech.DistributionFunction(x: 1.4); // 0.92968538268895873
    /// double pdf = sech.ProbabilityDensityFunction(x: 1.4); // 0.10955386512899701
    /// double lpdf = sech.LogProbabilityDensityFunction(x: 1.4); // -2.2113389316917877
    /// 
    /// double ccdf = sech.ComplementaryDistributionFunction(x: 1.4); // 0.070314617311041272
    /// double icdf = sech.InverseDistributionFunction(p: cdf); // 1.40
    /// 
    /// double hf = sech.HazardFunction(x: 1.4); // 1.5580524977385339
    /// 
    /// string str = sech.ToString(); // Sech(x)
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class HyperbolicSecantDistribution : UnivariateContinuousDistribution
    {

        /// <summary>
        ///   Constructs a Hyperbolic Secant (Sech) distribution.
        /// </summary>
        /// 
        public HyperbolicSecantDistribution()
        {
        }


        /// <summary>
        ///   Gets the mean for this distribution (always zero).
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's mean value.
        /// </value>
        /// 
        public override double Mean
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets the median for this distribution (always zero).
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's median value.
        /// </value>
        /// 
        public override double Median
        {
            get { return 0; }
        }

        /// <summary>
        ///   Gets the variance for this distribution (always one).
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's variance.
        /// </value>
        /// 
        public override double Variance
        {
            get { return 1; }
        }


        /// <summary>
        ///   Gets the Standard Deviation (the square root of
        ///   the variance) for the current distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's standard deviation.
        /// </value>
        /// 
        public override double StandardDeviation
        {
            get { return 1; }
        }

        /// <summary>
        ///   Gets the mode for this distribution (always zero).
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's mode value.
        /// </value>
        /// 
        public override double Mode
        {
            get { return 0; }
        }

        /// <summary>
        ///   Gets the support interval for this distribution (-inf, +inf).
        /// </summary>
        /// 
        /// <value>
        ///   A <see cref="DoubleRange"/> containing
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
        /// <value>
        ///   The distribution's entropy.
        /// </value>
        /// 
        public override double Entropy
        {
            get { return (4.0 / Math.PI) * Constants.Catalan; }
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
            double angle = Math.Atan(Math.Exp(x * Math.PI / 2.0));
            return 2 * angle / Math.PI;
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
            return 0.5 * Special.Sech(x * (Math.PI / 2.0));
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
            return new HyperbolicSecantDistribution();
        }


        /// <summary>
        ///   Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// 
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// 
        /// <returns>
        ///   A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// 
        public override string ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format(formatProvider, "Sech(x)");
        }

    }
}
