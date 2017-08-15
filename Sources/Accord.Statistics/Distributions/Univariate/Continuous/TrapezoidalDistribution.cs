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
    using System.ComponentModel;
    using Accord.Compat;

    /// <summary>
    ///   Trapezoidal distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Trapezoidal distributions have been used in many areas and studied under varying
    ///   scopes, such as in the excellent work of (van Dorp and Kotz, 2003), risk analysis
    ///   (Pouliquen, 1970) and (Powell and Wilson, 1997), fuzzy set theory (Chen and Hwang,
    ///   1992), applied phyisics, and biomedical applications (Flehinger and Kimmel, 1987).
    /// </para>
    /// 
    /// <para>        
    ///   Trapezoidal distributions are appropriate for modeling events that are comprised
    ///   by three different stages: one growth stage, where probability grows up until a
    ///   plateau is reached; a stability stage, where probability stays more or less the same;
    ///   and a decline stage, where probability decreases until zero (van Dorp and Kotz, 2003).
    /// </para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://www.seas.gwu.edu/~dorpjr/Publications/JournalPapers/Metrika2003VanDorp.pdf">
    ///       J. René van Dorp, Samuel Kotz, Trapezoidal distribution. Available on: 
    ///       http://www.seas.gwu.edu/~dorpjr/Publications/JournalPapers/Metrika2003VanDorp.pdf </a></description></item>
    ///     <item><description>
    ///       Powell MR, Wilson JD (1997). Risk Assessment for National Natural Resource
    ///       Conservation Programs, Discussion Paper 97-49. Resources for the Future, Washington
    ///       D.C.</description></item>
    ///     <item><description>
    ///       Chen SJ, Hwang CL (1992). Fuzzy Multiple Attribute Decision-Making: Methods and
    ///       Applications, Springer-Verlag, Berlin, New York.</description></item>
    ///     <item><description>
    ///       Flehinger BJ, Kimmel M (1987). The natural history of lung cancer in periodically 
    ///       screened population. Biometrics 1987, 43, 127-144.</description></item>
    ///   </list></para>     
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example shows how to create and test the main characteristics
    ///   of a Trapezoidal distribution given its parameters: </para>
    ///   
    /// <code>
    /// // Create a new trapezoidal distribution with linear growth between
    /// // 0 and 2, stability between 2 and 8, and decrease between 8 and 10.
    /// //
    /// //
    /// //            +-----------+
    /// //           /|           |\
    /// //          / |           | \
    /// //         /  |           |  \
    /// //  -------+---+-----------+---+-------
    /// //   ...   0   2   4   6   8   10  ...
    /// //
    /// var trapz = new TrapezoidalDistribution(a: 0, b: 2, c: 8, d: 10, n1: 1, n3: 1);
    /// 
    /// double mean = trapz.Mean;     // 2.25
    /// double median = trapz.Median; // 3.0
    /// double mode = trapz.Mode;     // 3.1353457616424696
    /// double var = trapz.Variance;  // 17.986666666666665
    /// 
    /// double cdf = trapz.DistributionFunction(x: 1.4);           // 0.13999999999999999
    /// double pdf = trapz.ProbabilityDensityFunction(x: 1.4);     // 0.10000000000000001
    /// double lpdf = trapz.LogProbabilityDensityFunction(x: 1.4); // -2.3025850929940455
    /// 
    /// double ccdf = trapz.ComplementaryDistributionFunction(x: 1.4); // 0.85999999999999999
    /// double icdf = trapz.InverseDistributionFunction(p: cdf);       // 1.3999999999999997
    /// 
    /// double hf = trapz.HazardFunction(x: 1.4);            // 0.11627906976744187
    /// double chf = trapz.CumulativeHazardFunction(x: 1.4); // 0.15082288973458366
    /// 
    /// string str = trapz.ToString(CultureInfo.InvariantCulture); // Trapezoidal(x; a=0, b=2, c=8, d=10, n1=1, n3=1, α = 1)
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class TrapezoidalDistribution : UnivariateContinuousDistribution
    {
        // distribution parameters
        double a;  // left   bottom boundary
        double b;  // left   top boundary
        double c;  // right  top boundary
        double d;  // right  bottom boundary
        double n1; // growth rate
        double n3; // decay  rate

        double alpha = 1;

        // derived measures
        double constant;

        /// <summary>
        ///   Creates a new trapezoidal distribution.
        /// </summary>
        /// 
        /// <param name="a">The minimum value a.</param>
        /// <param name="b">The beginning of the stability region b.</param>
        /// <param name="c">The end of the stability region c.</param>
        /// <param name="d">The maximum value d.</param>
        /// 
        public TrapezoidalDistribution(
            [Real(maximum: 1e+300), DefaultValue(0)] double a, [Real, DefaultValue(1)] double b,
            [Real, DefaultValue(2)] double c, [Real(minimum: 1e-300), DefaultValue(3)] double d)
            : this(a, b, c, d, 2, 2)
        {
        }

        /// <summary>
        ///   Creates a new trapezoidal distribution.
        /// </summary>
        /// 
        /// <param name="a">The minimum value a.</param>
        /// <param name="b">The beginning of the stability region b.</param>
        /// <param name="c">The end of the stability region c.</param>
        /// <param name="d">The maximum value d.</param>
        /// <param name="n1">The growth slope between points <paramref name="a"/> and <paramref name="b"/>. Default is 2.</param>
        /// <param name="n3">The growth slope between points <paramref name="c"/> and <paramref name="d"/>. Default is 2.</param>
        /// 
        public TrapezoidalDistribution(
        [Real(maximum: 1e+300), DefaultValue(0)] double a, [Real, DefaultValue(1)] double b,
        [Real, DefaultValue(2)] double c, [Real(minimum: 1e-300), DefaultValue(3)] double d,
        [Positive, DefaultValue(2)] double n1, [Positive, DefaultValue(2)] double n3)
            : this(a, b, c, d, n1, n3, 1)
        {
        }

        /// <summary>
        ///   Creates a new trapezoidal distribution.
        /// </summary>
        /// 
        /// <param name="a">The minimum value a.</param>
        /// <param name="b">The beginning of the stability region b.</param>
        /// <param name="c">The end of the stability region c.</param>
        /// <param name="d">The maximum value d.</param>
        /// <param name="n1">The growth slope between points <paramref name="a"/> and <paramref name="b"/>. Default is 2.</param>
        /// <param name="n3">The growth slope between points <paramref name="c"/> and <paramref name="d"/>. Default is 2.</param>
        /// <param name="alpha">The boundary ratio α. Default is 1.</param>
        /// 
        public TrapezoidalDistribution(
    [Real(maximum: 1e+300), DefaultValue(0)] double a, [Real, DefaultValue(1)] double b,
    [Real, DefaultValue(2)] double c, [Real(minimum: 1e-300), DefaultValue(3)] double d,
    [Positive, DefaultValue(2)] double n1, [Positive, DefaultValue(2)] double n3, [Positive, DefaultValue(1)]double alpha)
        {
            // boundary validation
            if (a > b)
                throw new ArgumentOutOfRangeException("b", "Argument b must be higher than a.");

            if (b > c)
                throw new ArgumentOutOfRangeException("c", "Argument c must be higher than b.");

            if (d < c)
                throw new ArgumentOutOfRangeException("d", "Argument d must be higher than c.");

            if (d <= a)
                throw new ArgumentOutOfRangeException("d", "The maximum value d must be higher than the minimum value a");

            if (n1 <= 0)
                throw new ArgumentOutOfRangeException("n1", "Slope n1 must be positive.");

            if (n3 <= 0)
                throw new ArgumentOutOfRangeException("n3", "Slope n3 must be positive.");

            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.n1 = n1;
            this.n3 = n3;
            this.alpha = alpha;

            double num = 2 * n1 * n3;
            double den = 2 * alpha * (b - a) * n3
                + (alpha + 1) * (c - b) * n1 * n3
                + 2 * (d - c) * n1;

            this.constant = num / den;
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
                double expectationX1 = (a + n1 * b) / (n1 + 1);
                double expectationX3 = (n3 * c + d) / (n3 + 1);

                double num = (-2 / 3.0) * (alpha - 1) * (Math.Pow(c, 3)
                    - Math.Pow(b, 3)) + (alpha * c - b) * (Math.Pow(c, 2) - Math.Pow(b, 2));
                double den = Math.Pow(c - b, 2) * (alpha + 1);

                double expectationX2 = num / den;


                num = (2 * alpha * (b - a) * n3 * expectationX1)
                   + (n1 * n3 * (expectationX2))
                   + (2 * (d - c) * n1 * expectationX3);

                den = (2 * alpha * (b - a) * n3)
                    + ((alpha + 1) * (c - b) * n1 * n3)
                    + (2 * (d - c) * n1);

                return num / den;
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
                double expectationX1_2;
                double expectationX2_2;
                double expectationX3_2;

                {
                    double num = 2 * a * a + 2 * n1 * a * b + n1 * (n1 + 1) * b * b;
                    double den = (n1 + 2) + (n1 + 1);
                    expectationX1_2 = num / den;
                }

                {
                    double num = -0.5 * (alpha - 1) * (Math.Pow(c, 4) - Math.Pow(b, 4))
                        + (2 / 3.0) * (alpha * c - b) * (Math.Pow(c, 3)
                        - Math.Pow(b, 3));

                    double den = Math.Pow(c - b, 2) * (alpha + 1);
                    expectationX2_2 = num / den;
                }

                {
                    double num = 2 * d * d + 2 * n3 * c * d + n3 * (n3 + 1) * c * c;
                    double den = (n3 + 2) * (n3 + 1);
                    expectationX3_2 = num / den;
                }

                double x = (2 * alpha * (b - a) * n3)
                    / (2 * alpha * (b - a) * n3 + (alpha + 1) * (c - b) * n1 * n3 + 2 * (d - c) * n1);

                double y = (n1 * n3)
                    / (2 * alpha * (b - a) * n3 + (alpha + 1) * (c - b) * n1 * n3 + 2 * (d - c) * n1);

                double z = (2 * (d - c) * n1)
                    / (2 * alpha * (b - a) * n3 + (alpha + 1) * (c - b) * n1 * n3 + 2 * (d - c) * n1);


                return x * expectationX1_2 + y * expectationX2_2 + z * expectationX3_2;
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
            get { return new DoubleRange(a, d); }
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
            if (x < b)
            {
                double num = 2 * alpha * (b - a) * n3;
                double den = 2 * alpha * (b - a) * n3
                    + (alpha + 1) * (c - b) * n1 * n3
                    + 2 * (d - c) * n1;
                double p = Math.Pow((x - a) / (b - a), n1);
                return (num / den) * p;
            }

            if (x < c)
            {
                double num = 2 * alpha * (b - a) * n3
                    + 2 * (x - b) * n1 * n3 * (1 + ((alpha - 1) / 2) * ((2 * c - b - x) / (c - b)));

                double den = 2 * alpha * (b - a) * n3
                    + (alpha + 1) * (c - b) * n1 * n3
                    + 2 * (d - c) * n1;

                return num / den;
            }

            if (x < d)
            {
                double num = 2 * (d - c) * n1;
                double den = 2 * alpha * (b - a) * n3
                    + (alpha + 1) * (c - b) * n1 * n3
                    + 2 * (d-c) * n1;

                double p = Math.Pow((d - x) / (d - c), n3);
                return 1 - (num / den) * p;
            }

            return 1;
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
            if (x < b)
                return constant * alpha * Math.Pow((x - a) / (b - a), n1 - 1);

            if (x < c)
                return constant * (((alpha - 1) * (c - x) / (c - b)) + 1);

            return constant * Math.Pow((d - x) / (d - c), n3 - 1);
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
            return new TrapezoidalDistribution(a, b, c, d, n1, n3);
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
            return String.Format("Trapezoidal(x; a = {0}, b = {1}, c = {2}, d = {3}, n1 = {4}, n3 = {5}, α = {6})",
                a.ToString(format, formatProvider),
                b.ToString(format, formatProvider),
                c.ToString(format, formatProvider),
                d.ToString(format, formatProvider),
                n1.ToString(format, formatProvider),
                n3.ToString(format, formatProvider),
                alpha.ToString(format, formatProvider));
        }
    }
}
