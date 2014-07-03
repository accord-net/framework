// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Multivariate;
    using AForge;

    /// <summary>
    ///   Triangular distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In probability theory and statistics, the triangular distribution is a continuous
    ///   probability distribution with lower limit a, upper limit b and mode c, where a &lt; 
    ///   b and a ≤ c ≤ b.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://en.wikipedia.org/wiki/Triangular_distribution">
    ///       Wikipedia, The Free Encyclopedia. Triangular distribution. Available on: 
    ///       https://en.wikipedia.org/wiki/Triangular_distribution </a></description></item>
    ///   </list></para> 
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   This example shows how to create a Triangular distribution
    ///   with minimum 1, maximum 6, and most common value 3.</para>
    ///   
    /// <code>
    /// // Create a new Triangular distribution (1, 3, 6).
    /// var trig = new TriangularDistribution(a: 1, b: 6, c: 3);
    /// 
    /// double mean = trig.Mean;     // 3.3333333333333335
    /// double median = trig.Median; // 3.2613872124741694
    /// double mode = trig.Mode;     // 3.0
    /// double var = trig.Variance;  // 1.0555555555555556
    /// 
    /// double cdf = trig.DistributionFunction(x: 2); // 0.10000000000000001
    /// double pdf = trig.ProbabilityDensityFunction(x: 2); // 0.20000000000000001
    /// double lpdf = trig.LogProbabilityDensityFunction(x: 2); // -1.6094379124341003
    /// 
    /// double ccdf = trig.ComplementaryDistributionFunction(x: 2); // 0.90000000000000002
    /// double icdf = trig.InverseDistributionFunction(p: cdf); // 2.0000000655718773
    /// 
    /// double hf = trig.HazardFunction(x: 2); // 0.22222222222222224
    /// double chf = trig.CumulativeHazardFunction(x: 2); // 0.10536051565782628
    /// 
    /// string str = trig.ToString(CultureInfo.InvariantCulture); // Triangular(x; a = 1, b = 6, c = 3)
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class TriangularDistribution : UnivariateContinuousDistribution,
        ISampleableDistribution<double>
    {

        // Distribution parameters
        private double a;
        private double b;
        private double c;

        /// <summary>
        ///   Constructs a Triangular distribution
        ///   with the given parameters a, b and c.
        /// </summary>
        /// 
        /// <param name="a">The minimum possible value in the distribution (a).</param>
        /// <param name="b">The maximum possible value in the distribution (b).</param>
        /// <param name="c">The most common value in the distribution (c).</param>
        /// 
        public TriangularDistribution(double a, double b, double c)
        {
            if (a > b)
                throw new ArgumentException();

            if (c < a || c > b)
                throw new ArgumentException();

            initialize(a, b, c);
        }


        /// <summary>
        ///   Gets the triangular parameter A (the minimum value).
        /// </summary>
        /// 
        public double A { get { return a; } }

        /// <summary>
        ///   Gets the triangular parameter B (the maximum value).
        /// </summary>
        /// 
        public double B { get { return b; } }

        /// <summary>
        ///   Gets the triangular parameter C (the most probable value).
        /// </summary>
        /// 
        public double C { get { return c; } }

        /// <summary>
        ///   Gets the mean for this distribution, 
        ///   defined as (a + b + c) / 3.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's mean value.
        /// </value>
        /// 
        public override double Mean
        {
            get { return (a + b + c) / 3.0; }
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
                double median;
                if (c >= (a + b) / 2.0)
                {
                    median = a + Math.Sqrt((b - a) * (c - a)) / Constants.Sqrt2;
                }
                else
                {
                    median = b - Math.Sqrt((b - a) * (b - c)) / Constants.Sqrt2;
                }

                System.Diagnostics.Debug.Assert(median == base.Median);

                return median;
            }
        }


        /// <summary>
        ///   Gets the variance for this distribution, defined
        ///   as (a² + b² + c² - ab - ac - bc) / 18.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's variance.
        /// </value>
        /// 
        public override double Variance
        {
            get { return (a * a + b * b + c * c - a * b - a * c - b * c) / 18; }
        }

        /// <summary>
        ///   Gets the mode for this distribution,
        ///   which is defined as <see cref="C"/>.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's mode value.
        /// </value>
        /// 
        public override double Mode
        {
            get { return c; }
        }

        /// <summary>
        ///   Gets the distribution support, defined as (<see cref="A"/>, <see cref="B"/>).
        /// </summary>
        /// 
        public override DoubleRange Support
        {
            get { return new DoubleRange(a, b); }
        }


        /// <summary>
        ///   Gets the entropy for this distribution, 
        ///   defined as 0.5 + log((b-a)/2)).
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's entropy.
        /// </value>
        /// 
        public override double Entropy
        {
            get { return 0.5 + Math.Log((b - a) / 2); }
        }


        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        public override double DistributionFunction(double x)
        {
            if (x < a)
                return 0;

            if (x >= a && x <= c)
                return ((x - a) * (x - a)) / ((b - a) * (c - a));

            if (x > c && x <= b)
                return 1 - ((b - x) * (b - x)) / ((b - a) * (b - c));

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
        public override double ProbabilityDensityFunction(double x)
        {
            if (x < a)
                return 0;

            if (x >= a && x <= c)
                return (2 * (x - a)) / ((b - a) * (c - a));

            if (x > c && x <= b)
                return (2 * (b - x)) / ((b - a) * (b - c));

            return 0;
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
            return new TriangularDistribution(a, b, c);
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
            return String.Format("Triangular(x; a = {0}, b = {1}, c = {2})", a, b, c);
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
            return String.Format(formatProvider, "Triangular(x; a = {0}, b = {1}, c = {2})", a, b, c);
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
            return String.Format(formatProvider, "Triangular(x; a = {0}, b = {1}, c = {2})",
                a.ToString(format, formatProvider),
                b.ToString(format, formatProvider),
                c.ToString(format, formatProvider));
        }

        /// <summary>
        ///   Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// 
        public string ToString(string format)
        {
            return String.Format("Triangular(x; a = {0}, b = {1}, c = {2})",
                a.ToString(format), b.ToString(format), c.ToString(format));
        }


        private void initialize(double a, double b, double c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public double[] Generate(int samples)
        {
            double Fc = DistributionFunction(c);

            double[] values = UniformContinuousDistribution.Random(samples);

            for (int i = 0; i < values.Length; i++)
            {
                double u = values[i];

                if (u < Fc)
                    values[i] = a + Math.Sqrt(u * (b - a) * (c - a));
                values[i] = b - Math.Sqrt((1 - u) * (b - a) * (b - c));
            }

            return values;
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public double Generate()
        {
            double u = UniformContinuousDistribution.Random();
            double Fc = DistributionFunction(c);

            if (u < Fc)
                return a + Math.Sqrt(u * (b - a) * (c - a));
            return b - Math.Sqrt((1 - u) * (b - a) * (b - c));
        }
    }
}
