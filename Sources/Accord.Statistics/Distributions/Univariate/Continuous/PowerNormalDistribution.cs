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
    ///   Power Normal distribution.
    /// </summary>
    /// 
    /// <example>
    /// <para>
    ///   This example shows how to create a Power Normal distribution
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
    /// int icdf1 = dist.InverseDistributionFunction(p: 0.0); // 3
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
    public class PowerNormalDistribution : UnivariateContinuousDistribution
    {

        // Distribution parameters
        private double power = 1; // power (p)

        /// <summary>
        ///   Constructs a Power Normal distribution
        ///   with given power (shape) parameter.
        /// </summary>
        /// 
        /// <param name="power">The distribution's power p.</param>
        /// 
        public PowerNormalDistribution(double power)
        {
            if (power <= 0)
                throw new ArgumentOutOfRangeException("power", "Power must be positive.");

            initialize(power);
        }

        /// <summary>
        ///   Gets the distribution shape (power) parameter.
        /// </summary>
        /// 
        public double Power
        {
            get { return power; }
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public override double Mean
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public override double Median
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public override double Mode
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public override double Variance
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public override double StandardDeviation
        {
            get { throw new NotSupportedException(); }
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
            get { return new DoubleRange(0, Double.PositiveInfinity); }
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">
        ///   A single point in the distribution range.</param>
        ///   
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        /// 
        public override double DistributionFunction(double x)
        {
            return 1.0 - Math.Pow(Normal.Function(-x), power);
        }

        /// <summary>
        ///   Gets the inverse of the cumulative distribution function (icdf) for
        ///   this distribution evaluated at probability <c>p</c>. This function 
        ///   is also known as the Quantile function.
        /// </summary>
        /// 
        /// <remarks>
        ///   The Inverse Cumulative Distribution Function (ICDF) specifies, for
        ///   a given probability, the value which the random variable will be at,
        ///   or below, with that probability.
        /// </remarks>
        /// 
        /// <param name="p">A probability value between 0 and 1.</param>
        /// 
        /// <returns>A sample which could original the given probability 
        ///   value when applied in the <see cref="DistributionFunction"/>.</returns>
        /// 
        public override double InverseDistributionFunction(double p)
        {
            return Normal.Inverse(1.0 - Math.Pow(1.0 - p, 1.0 / power));
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">
        ///   A single point in the distribution range.</param>
        ///   
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.</returns>
        ///   
        public override double ProbabilityDensityFunction(double x)
        {
            return power * Normal.Derivative(x) * Math.Pow(Normal.Function(-x), power - 1);
        }

        /// <summary>
        ///   Gets the log-probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">
        ///   A single point in the distribution range.</param>
        ///   
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        /// <returns>
        ///   The logarithm of the probability of <c>x</c> 
        ///   occurring in the current distribution.</returns>
        ///   
        public override double LogProbabilityDensityFunction(double x)
        {
            return Math.Log(power) + Normal.LogDerivative(x) + (power - 1) * Normal.Function(-x);
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
            return new PowerNormalDistribution(power);
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
            return String.Format("PND(x; p = {0})", power);
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
            return String.Format(formatProvider, "PND(x; p = {0})", power);
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
            return String.Format(formatProvider, "PND(x; p = {0})",
                power.ToString(format, formatProvider));
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
            return String.Format("PND(x; p = {0})", power.ToString(format));
        }


        private void initialize(double power)
        {
            this.power = power;
        }

    }
}
