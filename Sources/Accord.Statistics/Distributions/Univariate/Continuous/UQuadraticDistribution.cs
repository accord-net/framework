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
    ///   U-quadratic distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In probability theory and statistics, the U-quadratic distribution is a continuous
    ///   probability distribution defined by a unique quadratic function with lower limit a 
    ///   and upper limit b. This distribution is a useful model for symmetric bimodal processes.
    ///   Other continuous distributions allow more flexibility, in terms of relaxing the symmetry
    ///   and the quadratic shape of the density function, which are enforced in the U-quadratic 
    ///   distribution - e.g., Beta distribution, Gamma distribution, etc. </para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/U-quadratic_distribution">
    ///       Wikipedia, The Free Encyclopedia. U-quadratic distribution. Available on: 
    ///       http://en.wikipedia.org/wiki/U-quadratic_distribution </a></description></item>
    ///   </list></para>     
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example shows how to create and test the main characteristics
    ///   of an U-quadratic distribution given its two parameters: </para>
    ///   
    /// <code>
    /// // Create a new U-quadratic distribution with values
    /// var u2 = new UQuadraticDistribution(a: 0.42, b: 4.2);
    /// 
    /// double mean = u2.Mean;     // 2.3100000000000001
    /// double median = u2.Median; // 2.3100000000000001
    /// double mode = u2.Mode;     // 0.8099060089153145
    /// double var = u2.Variance;  // 2.1432600000000002
    /// 
    /// double cdf = u2.DistributionFunction(x: 1.4);           // 0.44419041812731797
    /// double pdf = u2.ProbabilityDensityFunction(x: 1.4);     // 0.18398763254730335
    /// double lpdf = u2.LogProbabilityDensityFunction(x: 1.4); // -1.6928867380489712
    /// 
    /// double ccdf = u2.ComplementaryDistributionFunction(x: 1.4); // 0.55580958187268203
    /// double icdf = u2.InverseDistributionFunction(p: cdf);       // 1.3999998213768274
    /// 
    /// double hf = u2.HazardFunction(x: 1.4);            // 0.3310263776442936
    /// double chf = u2.CumulativeHazardFunction(x: 1.4); // 0.58732952203701494
    /// 
    /// string str = u2.ToString(CultureInfo.InvariantCulture); // "UQuadratic(x; a = 0.42, b = 4.2)"
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class UQuadraticDistribution : UnivariateContinuousDistribution
    {
        // Distribution parameters

        double a;
        double b;
        double alpha;
        double beta;

        /// <summary>
        ///   Constructs a new U-quadratic distribution.
        /// </summary>
        /// 
        /// <param name="a">Parameter a.</param>
        /// <param name="b">Parameter b.</param>
        /// 
        public UQuadraticDistribution([Nonnegative] double a, [Positive] double b)
        {
            if (b < a)
            {
                throw new ArgumentOutOfRangeException("b",
                    "Parameter b must be equal to or greater than parameter a.");
            }

            this.a = a;
            this.b = b;

            this.alpha = 12 / Math.Pow(b - a, 3);
            this.beta = (b + a) / 2;
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
            get { return (a + b) / 2.0d; }
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
            get { return (a + b) / 2.0d; }
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
            get { return (3.0d / 20.0d) * Math.Pow(b - a, 2.0d); }
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
            get { return new DoubleRange(a, b); }
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
        protected internal override double InnerDistributionFunction(double x)
        {
            return (alpha / 3) * (Math.Pow(x - beta, 3) + Math.Pow(beta - a, 3));
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
        protected internal override double InnerProbabilityDensityFunction(double x)
        {
            return alpha * Math.Pow(x - beta, 2);
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
            return new UQuadraticDistribution(this.a, this.b);
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
            return String.Format(formatProvider, "U-Quadratic(x; a = {0}, b = {1})",
                a.ToString(format, formatProvider),
                b.ToString(format, formatProvider));
        }

    }
}
