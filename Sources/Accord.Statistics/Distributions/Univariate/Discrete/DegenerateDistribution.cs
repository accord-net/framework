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
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Compat;

    /// <summary>
    ///   Degenerate distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In mathematics, a degenerate distribution or deterministic distribution is
    ///   the probability distribution of a random variable which only takes a single
    ///   value. Examples include a two-headed coin and rolling a die whose sides all 
    ///   show the same number. While this distribution does not appear random in the
    ///   everyday sense of the word, it does satisfy the definition of random variable. </para>
    /// <para> 
    ///   The degenerate distribution is localized at a point k0 on the real line. The
    ///   probability mass function is a Delta function at k0.</para>
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Degenerate_distribution">
    ///       Wikipedia, The Free Encyclopedia. Degenerate distribution. Available on:
    ///       http://en.wikipedia.org/wiki/Degenerate_distribution </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   This example shows how to create a Degenerate distribution
    ///   and compute some of its properties.</para>
    ///   
    /// <code>
    /// var dist = new DegenerateDistribution(value: 2);
    /// 
    /// double mean = dist.Mean;     // 2
    /// double median = dist.Median; // 2
    /// double mode = dist.Mode;     // 2
    /// double var = dist.Variance;  // 1
    /// 
    /// double cdf1 = dist.DistributionFunction(k: 1);    // 0
    /// double cdf2 = dist.DistributionFunction(k: 2);   // 1
    /// 
    /// double pdf1 = dist.ProbabilityMassFunction(k: 1); // 0
    /// double pdf2 = dist.ProbabilityMassFunction(k: 2); // 1
    /// double pdf3 = dist.ProbabilityMassFunction(k: 3); // 0
    /// 
    /// double lpdf = dist.LogProbabilityMassFunction(k: 2); // 0
    /// double ccdf = dist.ComplementaryDistributionFunction(k: 2); // 0.0
    /// 
    /// int icdf1 = dist.InverseDistributionFunction(p: 0.0); // 1
    /// int icdf2 = dist.InverseDistributionFunction(p: 0.5); // 3
    /// int icdf3 = dist.InverseDistributionFunction(p: 1.0); // 2
    /// 
    /// double hf = dist.HazardFunction(x: 0); // 0.0
    /// double chf = dist.CumulativeHazardFunction(x: 0); // 0.0
    /// 
    /// string str = dist.ToString(CultureInfo.InvariantCulture); // Degenerate(x; k0 = 2)
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class DegenerateDistribution : UnivariateDiscreteDistribution
    {

        private int k0;

        /// <summary>
        ///   Gets the unique value whose probability is different from zero.
        /// </summary>
        /// 
        public int Value { get { return k0; } }

        /// <summary>
        ///   Initializes a new instance of the <see cref="DegenerateDistribution"/> class.
        /// </summary>
        /// 
        public DegenerateDistribution()
            : this(0)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="DegenerateDistribution"/> class.
        /// </summary>
        /// 
        /// <param name="value">
        ///   The only value whose probability is different from zero. Default is zero.
        /// </param>
        /// 
        public DegenerateDistribution([Integer] int value)
        {
            this.k0 = value;
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <remarks>
        ///   In the Degenerate distribution, the mean is equal to the
        ///   <see cref="Value">unique value</see> within its domain.
        /// </remarks>
        /// 
        /// <value>
        ///   The distribution's mean value, which should equal <see cref="Value"/>.
        /// </value>
        /// 
        public override double Mean
        {
            get { return k0; }
        }

        /// <summary>
        ///   Gets the median for this distribution, which should equal <see cref="Value"/>.
        /// </summary>
        /// 
        /// <remarks>
        ///   In the Degenerate distribution, the mean is equal to the
        ///   <see cref="Value">unique value</see> within its domain.
        /// </remarks>
        /// 
        /// <value>
        ///   The distribution's median value.
        /// </value>
        /// 
        public override double Median
        {
            get { return k0; }
        }

        /// <summary>
        ///   Gets the variance for this distribution, which should equal 0.
        /// </summary>
        /// 
        /// <remarks>
        ///   In the Degenerate distribution, the variance equals 0.
        /// </remarks>
        /// 
        /// <value>
        ///   The distribution's variance.
        /// </value>
        /// 
        public override double Variance
        {
            get { return 0; }
        }

        /// <summary>
        ///   Gets the mode for this distribution, which should equal <see cref="Value"/>.
        /// </summary>
        /// 
        /// <remarks>
        ///   In the Degenerate distribution, the mean is equal to the
        ///   <see cref="Value">unique value</see> within its domain.
        /// </remarks>
        /// 
        /// <value>
        ///   The distribution's mode value.
        /// </value>
        /// 
        public override double Mode
        {
            get { return k0; }
        }

        /// <summary>
        ///   Gets the entropy for this distribution, which is zero.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's entropy.
        /// </value>
        /// 
        public override double Entropy
        {
            get { return 0; }
        }

        /// <summary>
        ///   Gets the support interval for this distribution.
        /// </summary>
        /// 
        /// <remarks>
        ///   The degenerate distribution's support is given only on the 
        ///   point interval (<see cref="Value"/>, <see cref="Value"/>).
        /// </remarks>
        /// 
        /// <value>
        ///   A <see cref="IntRange"/> containing
        ///   the support interval for this distribution.
        /// </value>
        /// 
        public override IntRange Support
        {
            get { return new IntRange(k0, k0); }
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>k</c>.
        /// </summary>
        /// 
        /// <param name="k">A single point in the distribution range.</param>
        /// 
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        /// 
        protected internal override double InnerDistributionFunction(int k)
        {
            if (k < k0)
                return 0;
            return 1;
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
        protected internal override double InnerProbabilityMassFunction(int k)
        {
            if (k == k0)
                return 1;
            return 0;
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
        ///   value when applied in the <see cref="UnivariateDiscreteDistribution.DistributionFunction(int)"/>.
        /// </returns>
        /// 
        protected override int InnerInverseDistributionFunction(double p)
        {
            if (p > 0.5)
                return k0 + 1;
            if (p < 0.5)
                return k0 - 1;
            return k0;
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
            return new DegenerateDistribution(k0);
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
            return String.Format(formatProvider, "Degenerate(x; k0 = {0})",
                k0.ToString(format, formatProvider));
        }

        /// <summary>
        ///   The <see cref="DegenerateDistribution"/> does not support fitting.
        /// </summary>
        /// 
        public override void Fit(double[] observations, double[] weights, IFittingOptions options)
        {
            throw new NotSupportedException();
        }
    }

}
