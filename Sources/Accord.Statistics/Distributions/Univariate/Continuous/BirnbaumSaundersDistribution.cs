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
    using Accord.Math.Optimization;
    using Accord.Statistics.Distributions.Fitting;
    using AForge;

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
    ///       
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>

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
        ///   with location parameter 0, scale 1, and the
        ///   given shape.
        /// </summary>
        /// 
        public BirnbaumSaundersDistribution(double shape)
            : this(0, 1, shape)
        {
        }


        /// <summary>
        ///   Constructs a Birnbaum-Saunders distribution
        ///   with given location, shape and scale parameters.
        /// </summary>
        /// 
        /// <param name="location">The location parameter μ.</param>
        /// <param name="scale">The scale parameter beta (β).</param>
        /// <param name="shape">The shape parameter gamma (γ).</param>
        /// 
        public BirnbaumSaundersDistribution(double location, double scale, double shape)
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
        ///   Gets the distribution's scale parameter  β.
        /// </summary>
        /// 
        public double Scale
        {
            get { return scale; }
        }

        /// <summary>
        ///   Gets the distribution's shape parameter  γ.
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
        ///   A <see cref="AForge.DoubleRange" /> containing
        ///   the support interval for this distribution.
        /// </value>
        /// 
        public override DoubleRange Support
        {
            get { return new DoubleRange(this.location, Double.PositiveInfinity); }
        }

        public DoubleRange Range
        {
            get { return new DoubleRange(0, Double.PositiveInfinity); }
        }


        public override double Mean
        {
            get { return 1 + 0.5 * shape * shape; }
        }


        public override double Variance
        {
            get { return shape * shape * (1 + (5 * shape * shape) / 4); }
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
        public override double DistributionFunction(double x)
        {
            double a = Math.Sqrt(x);
            double b = Math.Sqrt(1.0 / x);
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
        public override double ProbabilityDensityFunction(double x)
        {
            double c = x - location;

            double a = Math.Sqrt(c / scale);
            double b = Math.Sqrt(scale / c);

            double alpha = (a + b) / (2 * shape * c);
            double z = (a - b) / shape;


            // Normal cumulative distribution function
            return alpha * Special.Erfc(-z / Constants.Sqrt2) * 0.5;
        }

        public override double InverseDistributionFunction(double p)
        {
            double z = Normal.Inverse(p);
            double a = z + Math.Sqrt(4 + shape * (z * z));

            return (1.0 / 4.0) * a * a;
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
        public override string ToString()
        {
            return String.Format("BirnbaumSaunders(x; μ = {0}, β = {1}, γ = {2})", 
                location, scale, shape);
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
            return String.Format(formatProvider, "BirnbaumSaunders(x; μ = {0}, β = {1}, γ = {2})",
                location, scale, shape);
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
            return String.Format("BirnbaumSaunders(x; μ = {0}, β = {1}, γ = {2})",
                location.ToString(format, formatProvider),
                scale.ToString(format, formatProvider),
                shape.ToString(format, formatProvider));
        }
    }
}
