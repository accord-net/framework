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
    ///   This examples shows how to create a Triangular distribution
    ///   and compute some of its properties.</para>
    ///   
    /// <code>

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
        /// <param name="a">The triangular distribution parameter a.</param>
        /// <param name="b">The triangular distribution parameter b.</param>
        /// <param name="c">The triangular distribution parameter c.</param>
        /// 
        public TriangularDistribution(double a, double b, double c)
        {
            if (a > b)
                throw new ArgumentException();

            if (c < a || c > b)
                throw new ArgumentException();

            initialize(a, b, c);
        }




        public override double Mean
        {
            get { return (a + b + c) / 3.0; }
        }


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


        public override double Variance
        {
            get { return (a * a + b * b + c * c - a * b - a * c - b * c) / 18; }
        }

        public override double Mode
        {
            get { return c; }
        }

        public override DoubleRange Support
        {
            get { return new DoubleRange(a, b); }
        }


        public override double Entropy
        {
            get { return 0.5 + Math.Log((b - a) / 2); }
        }


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
