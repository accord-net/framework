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
    ///   Power Normal distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://www.itl.nist.gov/div898/handbook/eda/section3/eda366d.htm">
    ///       NIST/SEMATECH e-Handbook of Statistical Methods. Power Normal distribution. Available on: 
    ///       http://www.itl.nist.gov/div898/handbook/eda/section3/eda366d.htm </a></description></item>
    ///   </list></para> 
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   This example shows how to create a Power Normal distribution
    ///   and compute some of its properties.</para>
    ///   
    /// <code>
    /// // Create a new Power-Normal distribution with p = 4.2
    /// var pnormal = new PowerNormalDistribution(power: 4.2);
    /// 
    /// double cdf = pnormal.DistributionFunction(x: 1.4); // 0.99997428721920678
    /// double pdf = pnormal.ProbabilityDensityFunction(x: 1.4); // 0.00020022645890003279
    /// double lpdf = pnormal.LogProbabilityDensityFunction(x: 1.4); // -0.20543269836728234
    /// 
    /// double ccdf = pnormal.ComplementaryDistributionFunction(x: 1.4); // 0.000025712780793218926
    /// double icdf = pnormal.InverseDistributionFunction(p: cdf); // 1.3999999999998953
    /// 
    /// double hf = pnormal.HazardFunction(x: 1.4); // 7.7870402470368854
    /// double chf = pnormal.CumulativeHazardFunction(x: 1.4); // 10.568522382550167
    /// 
    /// string str = pnormal.ToString(); // PND(x; p = 4.2)
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
        public PowerNormalDistribution([Positive] double power)
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
        /// <param name="x">
        ///   A single point in the distribution range.</param>
        ///   
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        /// 
        protected internal override double InnerDistributionFunction(double x)
        {
            double phi = Normal.Function(-x);
            return 1.0 - Math.Pow(phi, power);
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
        ///   value when applied in the <see cref="UnivariateContinuousDistribution.DistributionFunction(double)"/>.</returns>
        /// 
        protected internal override double InnerInverseDistributionFunction(double p)
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
        protected internal override double InnerProbabilityDensityFunction(double x)
        {
            double pdf = Normal.Derivative(x);
            double cdf = Normal.Function(-x);
            return power * pdf * Math.Pow(cdf, power - 1);
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
        protected internal override double InnerLogProbabilityDensityFunction(double x)
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
        public override string ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format(formatProvider, "PND(x; p = {0})",
                power.ToString(format, formatProvider));
        }



        private void initialize(double power)
        {
            this.power = power;
        }

    }
}
