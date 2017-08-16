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
    ///   Lévy distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In probability theory and statistics, the Lévy distribution, named after Paul Lévy, is a continuous 
    ///   probability distribution for a non-negative random variable. In spectroscopy, this distribution, with 
    ///   frequency as the dependent variable, is known as a van der Waals profile. It is a special case of the
    ///   inverse-gamma distribution.</para>
    ///
    /// <para>
    ///   It is one of the few distributions that are stable and that have probability density functions that can
    ///   be expressed analytically, the others being the normal distribution and the Cauchy distribution. All three
    ///   are special cases of the stable distributions, which do not generally have a probability density function
    ///   which can be expressed analytically.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://en.wikipedia.org/wiki/L%C3%A9vy_distribution">
    ///       Wikipedia, The Free Encyclopedia. Lévy distribution. Available on: 
    ///       https://en.wikipedia.org/wiki/L%C3%A9vy_distribution </a></description></item>
    ///   </list></para> 
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   This examples shows how to create a Lévy distribution
    ///   and how to compute some of its measures and properties.
    /// </para>
    ///   
    /// <code>
    /// // Create a new Lévy distribution on 1 with scale 4.2:
    /// var levy = new LevyDistribution(location: 1, scale: 4.2);
    /// 
    /// double mean = levy.Mean;     // +inf
    /// double median = levy.Median; // 10.232059220934481
    /// double mode = levy.Mode;     // NaN
    /// double var = levy.Variance;  // +inf
    /// 
    /// double cdf = levy.DistributionFunction(x: 1.4); // 0.0011937454448720029
    /// double pdf = levy.ProbabilityDensityFunction(x: 1.4); // 0.016958939623898304
    /// double lpdf = levy.LogProbabilityDensityFunction(x: 1.4); // -4.0769601727487803
    /// 
    /// double ccdf = levy.ComplementaryDistributionFunction(x: 1.4); // 0.99880625455512795
    /// double icdf = levy.InverseDistributionFunction(p: cdf); // 1.3999999
    /// 
    /// double hf = levy.HazardFunction(x: 1.4); // 0.016979208476674869
    /// double chf = levy.CumulativeHazardFunction(x: 1.4); // 0.0011944585265140923
    /// 
    /// string str = levy.ToString(CultureInfo.InvariantCulture); // Lévy(x; μ = 1, c = 4.2)
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class LevyDistribution : UnivariateContinuousDistribution
    {

        // Distribution parameters
        private double location = 0;   // location μ
        private double scale = 1;      // scale c


        // Derived measures
        private double? median;

        /// <summary>
        ///   Constructs a new <see cref="LevyDistribution"/> 
        ///   with zero location and unit scale.
        /// </summary>
        /// 
        public LevyDistribution()
        {
            initialize(0, 1);
        }

        /// <summary>
        ///   Constructs a new <see cref="LevyDistribution"/> in
        ///   the given <paramref name="location"/> and with unit scale.
        /// </summary>
        /// 
        /// <param name="location">The distribution's location.</param>
        /// 
        public LevyDistribution([Real] double location)
        {
            initialize(location, 1);
        }

        /// <summary>
        ///   Constructs a new <see cref="LevyDistribution"/> in the 
        ///   given <paramref name="location"/> and <paramref name="scale"/>.
        /// </summary>
        /// 
        /// <param name="location">The distribution's location.</param>
        /// <param name="scale">The distribution's scale.</param>
        /// 
        public LevyDistribution([Real] double location, [Positive] double scale)
        {
            if (scale <= 0)
                throw new ArgumentOutOfRangeException("scale", "Scale must be positive.");

            initialize(location, scale);
        }


        /// <summary>
        ///   Gets the location μ (mu) for this distribution.
        /// </summary>
        /// 
        public double Location
        {
            get { return location; }
        }

        /// <summary>
        ///   Gets the location c for this distribution.
        /// </summary>
        /// 
        public double Scale
        {
            get { return scale; }
        }

        /// <summary>
        ///   Gets the mean for this distribution, which for
        ///   the Levy distribution is always positive infinity.
        /// </summary>
        /// 
        /// <value>
        ///   This property always returns <c>Double.PositiveInfinity</c>.
        /// </value>
        /// 
        public override double Mean
        {
            get { return Double.PositiveInfinity; }
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
                if (!median.HasValue)
                {
                    double a = Normal.Inverse(0.75);
                    double m = location + scale / (a * a);

#if DEBUG
                    double baseMedian = base.Median;
                    if (!m.IsEqual(baseMedian, 1e-10))
                        throw new Exception();
#endif

                    median = m;
                }

                return median.Value;
            }
        }

        /// <summary>
        ///   Gets the variance for this distribution, which for
        ///   the Levy distribution is always positive infinity.
        /// </summary>
        /// 
        /// <value>
        ///   This property always returns <c>Double.PositiveInfinity</c>.
        /// </value>
        /// 
        public override double Variance
        {
            get { return Double.PositiveInfinity; }
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
                if (location == 0)
                    return scale / 3.0;
                return Double.NaN;
            }
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
            get { return new DoubleRange(location, Double.PositiveInfinity); }
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
            get
            {
                return (1.0 + 3.0 * Constants.EulerGamma + Math.Log(16 * Math.PI * scale * scale)) / 2.0;
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
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        /// 
        protected internal override double InnerDistributionFunction(double x)
        {
            double cdf = Special.Erfc(Math.Sqrt(scale / (2 * (x - location))));

            return cdf;
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The probability of <c>x</c> occurring in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        protected internal override double InnerProbabilityDensityFunction(double x)
        {
            double z = x - location;
            double a = Math.Sqrt(scale / (2.0 * Math.PI));
            double b = Math.Exp(-(scale / (2 * z)));
            double c = Math.Pow(z, 3.0 / 2.0);

            return a * b / c;
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
        ///   value when applied in the <see cref="UnivariateContinuousDistribution.DistributionFunction(double)" />.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Inverse Cumulative Distribution Function (ICDF) specifies, for
        ///   a given probability, the value which the random variable will be at,
        ///   or below, with that probability.
        /// </remarks>
        /// 
        protected internal override double InnerInverseDistributionFunction(double p)
        {
            double a = Normal.Inverse(1.0 - p / 2.0);
            double icdf = location + scale / (a * a);

#if DEBUG
            double baseValue = base.InnerInverseDistributionFunction(p);
            if (!baseValue.IsEqual(icdf, 1e-5))
                throw new Exception();
#endif

            return icdf;
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
            return new LevyDistribution(location, scale);
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
            return String.Format(formatProvider, "Lévy(x; μ = {0}, c = {1})",
                location.ToString(format, formatProvider),
                scale.ToString(format, formatProvider));
        }


        private void initialize(double mu, double c)
        {
            this.location = mu;
            this.scale = c;
        }

    }
}
