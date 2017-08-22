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
    ///   Kumaraswamy distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In probability and statistics, the Kumaraswamy's double bounded distribution is a 
    ///   family of continuous probability distributions defined on the interval [0,1] differing 
    ///   in the values of their two non-negative shape parameters, a and b.
    ///   It is similar to the Beta distribution, but much simpler to use especially in simulation 
    ///   studies due to the simple closed form of both its probability density function and 
    ///   cumulative distribution function. This distribution was originally proposed by Poondi 
    ///   Kumaraswamy for variables that are lower and upper bounded.</para>
    ///
    /// <para>
    ///   A good example of the use of the Kumaraswamy distribution is the storage volume of a 
    ///   reservoir of capacity z<sub>max</sub> whose upper bound is z<sub>max</sub> and lower 
    ///   bound is 0 (Fletcher and Ponnambalam, 1996).</para>
    ///
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Kumaraswamy_distribution">
    ///       Wikipedia, The Free Encyclopedia. Kumaraswamy distribution. Available on: 
    ///       http://en.wikipedia.org/wiki/Kumaraswamy_distribution </a></description></item>
    ///   </list></para>     
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example shows how to create and test the main characteristics
    ///   of an Kumaraswamy distribution given its two non-negative shape parameters: </para>
    ///   
    /// <code>
    /// // Create a new Kumaraswamy distribution with shape (4,2)
    /// var kumaraswamy = new KumaraswamyDistribution(a: 4, b: 2);
    /// 
    /// double mean = kumaraswamy.Mean;     // 0.71111111111111114
    /// double median = kumaraswamy.Median; // 0.73566031573423674
    /// double mode = kumaraswamy.Mode;     // 0.80910671157022118
    /// double var = kumaraswamy.Variance;  // 0.027654320987654302
    /// 
    /// double cdf = kumaraswamy.DistributionFunction(x: 0.4);           // 0.050544639999999919
    /// double pdf = kumaraswamy.ProbabilityDensityFunction(x: 0.4);     // 0.49889280000000014
    /// double lpdf = kumaraswamy.LogProbabilityDensityFunction(x: 0.4); // -0.69536403596913343
    /// 
    /// double ccdf = kumaraswamy.ComplementaryDistributionFunction(x: 0.4); // 0.94945536000000008
    /// double icdf = kumaraswamy.InverseDistributionFunction(p: cdf);       // 0.40000011480618253
    /// 
    /// double hf = kumaraswamy.HazardFunction(x: 0.4);            // 0.52545155993431869
    /// double chf = kumaraswamy.CumulativeHazardFunction(x: 0.4); // 0.051866764053008864
    /// 
    /// string str = kumaraswamy.ToString(CultureInfo.InvariantCulture); // Kumaraswamy(x; a = 4, b = 2)
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class KumaraswamyDistribution : UnivariateContinuousDistribution
    {
        double a;
        double b;

        /// <summary>
        ///   Constructs a new Kumaraswamy's double bounded distribution with 
        ///   the given two non-negative shape parameters <c>a</c> and <c>b</c>.
        /// </summary>
        /// 
        /// <param name="a">The distribution's non-negative shape parameter a.</param>
        /// <param name="b">The distribution's non-negative shape parameter b.</param>
        /// 
        public KumaraswamyDistribution([Positive] double a, [Positive] double b)
        {
            if (a <= 0)
                throw new ArgumentOutOfRangeException("a", "The shape parameter a must be positive.");

            if (b < 0)
                throw new ArgumentOutOfRangeException("b", "The shape parameter b must be positive.");

            this.a = a;
            this.b = b;
        }

        /// <summary>
        ///   Gets the distribution's non-negative shape parameter a.
        /// </summary>
        /// 
        public double A { get { return a;  } }

        /// <summary>
        ///   Gets the distribution's non-negative shape parameter b.
        /// </summary>
        /// 
        public double B { get { return b; } }

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
                double num = b * Gamma.Function(1 + (1 / a)) * Gamma.Function(b);
                double den = Gamma.Function(1 + (1 / a) + b);

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
                double alpha = momentGeneratingFunction(2, a, b);
                double beta = Math.Pow(momentGeneratingFunction(1, a, b), 2);
                return alpha - beta;
            }
        }

        private static double momentGeneratingFunction(int n, double a, double b)
        {
            return (b * Beta.Function(1.0d + ((double)n) / a, b));
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
                return Math.Pow(1 - Math.Pow(2, -1 / b), 1 / a);
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

                if ((a >= 1) && (b >= 1) && (a != 1 && b != 1))
                {
                    double num = a - 1;
                    double den = a * b - 1;
                    return Math.Pow(num / den, 1 / a);
                }

                return Double.NaN;
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
            get { return new DoubleRange(0, 1); }
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
            double xa = Math.Pow(x, a);
            return 1 - Math.Pow(1 - xa, b);
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
            return a * b * Math.Pow(x, a - 1) * Math.Pow(1 - Math.Pow(x, a), b - 1);
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
            return new KumaraswamyDistribution(this.a, this.b);
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
            return String.Format(formatProvider, "Kumaraswamy(x; a = {0}, b = {1})",
                a.ToString(format, formatProvider),
                b.ToString(format, formatProvider));
        }

    }
}
