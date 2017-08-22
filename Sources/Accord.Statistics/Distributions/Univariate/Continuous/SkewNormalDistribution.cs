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
    ///   Skew Normal distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In probability theory and statistics, the skew normal distribution is a 
    ///   <see cref="UnivariateContinuousDistribution">continuous probability distribution</see>
    ///   that generalises the <see cref="NormalDistribution">normal distribution</see> to allow 
    ///   for non-zero <see cref="NormalDistribution.Skewness">skewness</see>.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://en.wikipedia.org/wiki/Skew_normal_distribution">
    ///       Wikipedia, The Free Encyclopedia. Skew normal distribution. Available on: 
    ///       https://en.wikipedia.org/wiki/Skew_normal_distribution </a></description></item>
    ///   </list></para> 
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   This examples shows how to create a Skew normal distribution
    ///   and compute some of its properties and derived measures.</para>
    ///   
    /// <code>
    /// // Create a Skew normal distribution with location 2, scale 3 and shape 4.2
    /// var skewNormal = new SkewNormalDistribution(location: 2, scale: 3, shape: 4.2);
    /// 
    /// double mean = skewNormal.Mean;     // 4.3285611780515953
    /// double median = skewNormal.Median; // 4.0230040653062265
    /// double var = skewNormal.Variance;  // 3.5778028400709641
    /// double mode = skewNormal.Mode;     // 3.220622226764422
    /// 
    /// double cdf = skewNormal.DistributionFunction(x: 1.4); // 0.020166854942526125
    /// double pdf = skewNormal.ProbabilityDensityFunction(x: 1.4); // 0.052257431834162059
    /// double lpdf = skewNormal.LogProbabilityDensityFunction(x: 1.4); // -2.9515731621912877
    /// 
    /// double ccdf = skewNormal.ComplementaryDistributionFunction(x: 1.4); // 0.97983314505747388
    /// double icdf = skewNormal.InverseDistributionFunction(p: cdf); // 1.3999998597203041
    /// 
    /// double hf = skewNormal.HazardFunction(x: 1.4); // 0.053332990517581239
    /// double chf = skewNormal.CumulativeHazardFunction(x: 1.4); // 0.020372981958858238
    /// 
    /// string str = skewNormal.ToString(CultureInfo.InvariantCulture); // Sn(x; ξ = 2, ω = 3, α = 4.2)
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="Accord.Statistics.Distributions.Univariate.NormalDistribution"/>
    /// <seealso cref="Accord.Statistics.Distributions.Multivariate.MultivariateNormalDistribution"/>
    /// 
    [Serializable]
    public class SkewNormalDistribution : UnivariateContinuousDistribution
    {

        // Distribution parameters
        private double ksi = 0;        // ξ location (real)
        private double omega = 1;      // ω scale (positive, real)
        private double alpha = 0;      // α shape (real)


        // Derived parameters
        double delta;

        /// <summary>
        ///   Constructs a Skew normal distribution with
        ///   zero location, unit scale and zero shape.
        /// </summary>
        /// 
        public SkewNormalDistribution()
        {
            initialize(ksi, omega, alpha);
        }

        /// <summary>
        ///   Constructs a Skew normal distribution with 
        ///   given location, unit scale and zero skewness.
        /// </summary>
        /// 
        /// <param name="location">The distribution's location value ξ (ksi).</param>
        /// 
        public SkewNormalDistribution([Real] double location)
        {
            initialize(location, omega, alpha);
        }

        /// <summary>
        ///   Constructs a Skew normal distribution with 
        ///   given location and scale and zero skewness.
        /// </summary>
        /// 
        /// <param name="location">The distribution's location value ξ (ksi).</param>
        /// <param name="scale">The distribution's scale value ω (omega).</param>
        /// 
        public SkewNormalDistribution([Real] double location, [Positive] double scale)
        {
            if (scale <= 0)
                throw new ArgumentOutOfRangeException("scale", "Scale must be positive.");

            initialize(location, scale, alpha);
        }

        /// <summary>
        ///   Constructs a Skew normal distribution
        ///   with given mean and standard deviation.
        /// </summary>
        /// 
        /// <param name="location">The distribution's location value ξ (ksi).</param>
        /// <param name="scale">The distribution's scale value ω (omega).</param>
        /// <param name="shape">The distribution's shape value α (alpha).</param>
        /// 
        public SkewNormalDistribution([Real] double location, [Positive] double scale, [Real] double shape)
        {
            if (scale <= 0)
                throw new ArgumentOutOfRangeException("scale", "Scale must be positive.");

            initialize(location, scale, shape);
        }

        /// <summary>
        ///   Gets the skew-normal distribution's location value  ξ (ksi).
        /// </summary>
        /// 
        public double Location
        {
            get { return ksi; }
        }

        /// <summary>
        ///   Gets the skew-normal distribution's scale value ω (omega).
        /// </summary>
        /// 
        public double Scale
        {
            get { return omega; }
        }

        /// <summary>
        ///   Gets the skew-normal distribution's shape value α (alpha).
        /// </summary>
        /// 
        public double Shape
        {
            get { return alpha; }
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public override double Entropy
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        ///    Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's mean value.
        /// </value>
        /// 
        public override double Mean
        {
            get { return ksi + omega * delta * Math.Sqrt(2.0 / Math.PI); }
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
            get { return omega * omega * (1 - (2 * delta * delta) / Math.PI); }
        }

        /// <summary>
        ///   Gets the skewness for this distribution.
        /// </summary>
        /// 
        public double Skewness
        {
            get
            {
                double a = (4 - Math.PI) / 2.0;
                double b = delta * Math.Sqrt(2.0 / Math.PI);
                double c = 1 - 2 * delta * delta / Math.PI;
                return a * (b * b * b) / Math.Pow(c, 3 / 2);
            }
        }

        /// <summary>
        ///   Gets the excess kurtosis for this distribution.
        /// </summary>
        /// 
        public double Kurtosis
        {
            get
            {
                double a = 2 * (Math.PI * 3);
                double b = delta * Math.Sqrt(2.0 / Math.PI);
                double c = 1 - 2 * delta * delta / Math.PI;
                return a * (b * b * b * b) / (c * c);
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
            get { return new DoubleRange(Double.NegativeInfinity, Double.PositiveInfinity); }
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
            double z = (x - ksi) / omega;

            double cdf = Accord.Math.Normal.Function(z) - 2 * OwensT.Function(z, alpha);

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
            double z = (x - ksi) / omega;

            double a = Accord.Math.Normal.Derivative(z);
            double b = Accord.Math.Normal.Function(alpha * z);

            return (2 / omega) * a * b;
        }

        /// <summary>
        ///   Gets the log-probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The logarithm of the probability of <c>x</c> 
        ///   occurring in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        protected internal override double InnerLogProbabilityDensityFunction(double x)
        {
            double z = (x - ksi) / omega;

            double a = Accord.Math.Normal.LogDerivative(z);
            double b = Math.Log(Accord.Math.Normal.Function(alpha * z));

            return Math.Log(2 / omega) + a + b;
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
            return new SkewNormalDistribution(ksi, omega, alpha);
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
            return String.Format(formatProvider, "Sn(x; ξ = {0}, ω = {1}, α = {2})",
                ksi.ToString(format, formatProvider),
                omega.ToString(format, formatProvider),
                alpha.ToString(format, formatProvider));
        }


        private void initialize(double location, double scale, double shape)
        {
            this.ksi = location;
            this.omega = scale;
            this.alpha = shape;

            // Compute derived values
            this.delta = shape / Math.Sqrt(1 + shape * shape);
        }

        /// <summary>
        ///   Create a new <see cref="SkewNormalDistribution"/> that 
        ///   corresponds to a <see cref="NormalDistribution"/> with
        ///   the given mean and standard deviation.
        /// </summary>
        /// 
        /// <param name="mean">The distribution's mean value μ (mu).</param>
        /// <param name="stdDev">The distribution's standard deviation σ (sigma).</param>
        /// 
        /// <returns>A <see cref="SkewNormalDistribution"/> representing 
        /// a <see cref="NormalDistribution"/> with the given parameters.</returns>
        /// 
        public static SkewNormalDistribution Normal(int mean, double stdDev)
        {
            return new SkewNormalDistribution(mean, stdDev);
        }
    }
}
