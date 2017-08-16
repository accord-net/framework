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
    ///   Birnbaum-Saunders (Fatigue Life) distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Birnbaum–Saunders distribution, also known as the fatigue life distribution,
    ///   is a probability distribution used extensively in reliability applications to model 
    ///   failure times. There are several alternative formulations of this distribution in
    ///   the literature. It is named after Z. W. Birnbaum and S. C. Saunders. </para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Birnbaum%E2%80%93Saunders_distribution">
    ///       Wikipedia, The Free Encyclopedia. Birnbaum–Saunders distribution. 
    ///       Available from: http://en.wikipedia.org/wiki/Birnbaum%E2%80%93Saunders_distribution </a></description></item>
    ///     <item><description><a href="http://www.itl.nist.gov/div898/handbook/eda/section3/eda366a.htm">
    ///       NIST/SEMATECH e-Handbook of Statistical Methods, Birnbaum-Saunders (Fatigue Life) Distribution
    ///       Available from: http://www.itl.nist.gov/div898/handbook/eda/section3/eda366a.htm </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   This example shows how to create a Birnbaum-Saunders distribution
    ///   and compute some of its properties.</para>
    ///   
    /// <code>
    /// // Creates a new Birnbaum-Saunders distribution
    /// var bs = new BirnbaumSaundersDistribution(shape: 0.42);
    /// 
    /// double mean = bs.Mean;     // 1.0882000000000001
    /// double median = bs.Median; // 1.0
    /// double var = bs.Variance;  // 0.21529619999999997
    /// 
    /// double cdf = bs.DistributionFunction(x: 1.4); // 0.78956384911580346
    /// double pdf = bs.ProbabilityDensityFunction(x: 1.4); // 1.3618433601225426
    /// double lpdf = bs.LogProbabilityDensityFunction(x: 1.4); // 0.30883919386130815
    /// 
    /// double ccdf = bs.ComplementaryDistributionFunction(x: 1.4); // 0.21043615088419654
    /// double icdf = bs.InverseDistributionFunction(p: cdf); // 2.0618330099769064
    /// 
    /// double hf = bs.HazardFunction(x: 1.4); // 6.4715276077824093
    /// double chf = bs.CumulativeHazardFunction(x: 1.4); // 1.5585729930861034
    /// 
    /// string str = bs.ToString(CultureInfo.InvariantCulture); // BirnbaumSaunders(x; μ = 0, β = 1, γ = 0.42)
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class BirnbaumSaundersDistribution : UnivariateContinuousDistribution
    {

        // Distribution parameters
        private double location; // μ          > x
        private double shape;    // γ (gamma)  > 0
        private double scale;    // β          > 0


        /// <summary>
        ///   Constructs a Birnbaum-Saunders distribution
        ///   with location parameter 0, scale 1, and shape 1.
        /// </summary>
        /// 
        public BirnbaumSaundersDistribution()
            : this(0, 1, 1)
        {
        }

        /// <summary>
        ///   Constructs a Birnbaum-Saunders distribution
        ///   with location parameter 0, scale 1, and the
        ///   given shape.
        /// </summary>
        /// 
        /// <param name="shape">The shape parameter gamma (γ). Default is 1.</param>
        /// 
        public BirnbaumSaundersDistribution([Positive] double shape)
            : this(0, 1, shape)
        {
        }


        /// <summary>
        ///   Constructs a Birnbaum-Saunders distribution
        ///   with given location, shape and scale parameters.
        /// </summary>
        /// 
        /// <param name="location">The location parameter μ. Default is 0.</param>
        /// <param name="scale">The scale parameter beta (β). Default is 1.</param>
        /// <param name="shape">The shape parameter gamma (γ). Default is 1.</param>
        /// 
        public BirnbaumSaundersDistribution([Real] double location,
            [Positive] double scale, [Positive] double shape)
        {
            if (scale <= 0)
                throw new ArgumentOutOfRangeException("scale", "Scale must be greater than zero.");

            if (shape <= 0)
                throw new ArgumentOutOfRangeException("shape", "Shape must be greater than zero.");

            init(location, scale, shape);
        }

        private void init(double location, double scale, double shape)
        {
            this.location = location;
            this.scale = scale;
            this.shape = shape;
        }

        /// <summary>
        ///   Gets the distribution's location parameter μ.
        /// </summary>
        /// 
        public double Location
        {
            get { return location; }
        }

        /// <summary>
        ///   Gets the distribution's scale parameter β.
        /// </summary>
        /// 
        public double Scale
        {
            get { return scale; }
        }

        /// <summary>
        ///   Gets the distribution's shape parameter γ.
        /// </summary>
        /// 
        public double Shape
        {
            get { return shape; }
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
            get { return new DoubleRange(this.location, Double.PositiveInfinity); }
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <remarks>
        ///   The Birnbaum Saunders mean is defined as 
        ///   <c>1 + 0.5<see cref="Shape">γ</see>²</c>.
        /// </remarks>
        /// 
        /// <value>
        ///   The distribution's mean value.
        /// </value>
        /// 
        public override double Mean
        {
            get { return 1 + 0.5 * shape * shape; }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <remarks>
        ///   The Birnbaum Saunders variance is defined as 
        ///   <c><see cref="Shape">γ</see>² (1 + (5/4)<see cref="Shape">γ</see>²)</c>.
        /// </remarks>
        /// 
        /// <value>
        ///   The distribution's mean value.
        /// </value>
        /// 
        public override double Variance
        {
            get { return shape * shape * (1 + (5 * shape * shape) / 4); }
        }

        /// <summary>
        ///   This method is not supported.
        /// </summary>
        /// 
        public override double Mode
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        ///   This method is not supported.
        /// </summary>
        /// 
        public override double Entropy
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <remarks>
        /// <para>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.</para>
        /// </remarks>
        /// 
        protected internal override double InnerDistributionFunction(double x)
        {
            double c = x - location;

            double a = Math.Sqrt(c);
            double b = Math.Sqrt(1.0 / c);
            double z = (a - b) / shape;

            // Normal cumulative distribution function
            return Special.Erfc(-z / Constants.Sqrt2) * 0.5;
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
        /// <para>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.</para>
        /// </remarks>
        /// 
        protected internal override double InnerProbabilityDensityFunction(double x)
        {
            double c = x - location;

            double a = Math.Sqrt(c / scale);
            double b = Math.Sqrt(scale / c);

            double alpha = (a + b) / (2 * shape * c);
            double z = (a - b) / shape;


            // Normal cumulative distribution function
            return alpha * Special.Erfc(-z / Constants.Sqrt2) * 0.5;
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
            if (p <= 0)
                return Support.Min;
            if (p >= 1)
                return Support.Max;

            double z = Normal.Inverse(p);
            double a = z + Math.Sqrt(4 + shape * (z * z));

            return (1.0 / 4.0) * a * a + location;
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
            return new BirnbaumSaundersDistribution(location, scale, shape);
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
            return String.Format(formatProvider, "BirnbaumSaunders(x; μ = {0}, β = {1}, γ = {2})",
                location.ToString(format, formatProvider),
                scale.ToString(format, formatProvider),
                shape.ToString(format, formatProvider));
        }
    }
}
