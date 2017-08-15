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
    ///   Power Lognormal distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://www.itl.nist.gov/div898/handbook/eda/section3/eda366e.htm">
    ///       NIST/SEMATECH e-Handbook of Statistical Methods. Power Lognormal distribution. Available on: 
    ///       http://www.itl.nist.gov/div898/handbook/eda/section3/eda366e.htm </a></description></item>
    ///   </list></para> 
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   This example shows how to create a Power Lognormal 
    ///   distribution and compute some of its properties.</para>
    ///   
    /// <code>
    /// // Create a Power-Lognormal distribution with p = 4.2 and s = 1.2
    /// var plog = new PowerLognormalDistribution(power: 4.2, shape: 1.2);
    /// 
    /// double cdf = plog.DistributionFunction(x: 1.4); // 0.98092157745191766
    /// double pdf = plog.ProbabilityDensityFunction(x: 1.4); // 0.046958580233533977
    /// double lpdf = plog.LogProbabilityDensityFunction(x: 1.4); // -3.0584893374471496
    /// 
    /// double ccdf = plog.ComplementaryDistributionFunction(x: 1.4); // 0.019078422548082351
    /// double icdf = plog.InverseDistributionFunction(p: cdf); // 1.4
    /// 
    /// double hf = plog.HazardFunction(x: 1.4); // 10.337649063164642
    /// double chf = plog.CumulativeHazardFunction(x: 1.4); // 3.9591972920568446
    /// 
    /// string str = plog.ToString(CultureInfo.InvariantCulture); // PLD(x; p = 4.2, σ = 1.2)
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class PowerLognormalDistribution : UnivariateContinuousDistribution
    {

        // Distribution parameters
        private double power = 1; // power (p)
        private double sigma = 1; // sigma (σ)

        /// <summary>
        ///   Constructs a Power Lognormal distribution
        ///   with the given power and shape parameters.
        /// </summary>
        /// 
        /// <param name="power">The distribution's power p.</param>
        /// <param name="shape">The distribution's shape σ.</param>
        /// 
        public PowerLognormalDistribution([Positive] double power, [Positive] double shape)
        {
            if (power <= 0)
                throw new ArgumentOutOfRangeException("power", "Power must be positive.");

            if (shape <= 0)
                throw new ArgumentOutOfRangeException("shape", "Shape must be positive.");

            initialize(power, shape);
        }

        /// <summary>
        ///   Gets the distribution's power parameter (p).
        /// </summary>
        /// 
        public double Power
        {
            get { return power; }
        }

        /// <summary>
        ///   Gets the distribution's shape parameter sigma (σ). 
        /// </summary>
        /// 
        public double Shape
        {
            get { return sigma; }
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
            get { return new DoubleRange(0, Double.PositiveInfinity); }
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        protected internal override double InnerDistributionFunction(double x)
        {
            double z = Math.Log(x) / sigma;
            return 1.0 - Math.Pow(Normal.Function(-z), power);
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
            return Math.Exp(Normal.Inverse(1.0 - Math.Pow(1.0 - p, 1.0 / power)) * sigma);
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
        protected internal override double InnerProbabilityDensityFunction(double x)
        {
            double a = power / (x * sigma);
            double z = Math.Log(x) / sigma;

            return a * Normal.Derivative(z) * Math.Pow(Normal.Function(-z), power - 1);
        }

        /// <summary>
        ///   Gets the hazard function, also known as the failure rate or
        ///   the conditional failure density function for this distribution
        ///   evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The conditional failure density function <c>h(x)</c>
        ///   evaluated at <c>x</c> in the current distribution.
        /// </returns>
        /// 
        public override double HazardFunction(double x)
        {
            double a = power / (x * sigma);
            double z = Math.Log(x) / sigma;

            double num = power * a * Normal.Derivative(z);
            double den = Normal.Function(-z);
            return num / den;
        }

        /// <summary>
        ///   Gets the cumulative hazard function for this
        ///   distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The cumulative hazard function <c>H(x)</c>
        ///   evaluated at <c>x</c> in the current distribution.
        /// </returns>
        /// 
        public override double CumulativeHazardFunction(double x)
        {
            double z = Math.Log(x) / sigma;
            return -Math.Log(Math.Pow(Normal.Function(-z), power));
        }

        /// <summary>
        ///   Gets the complementary cumulative distribution function
        ///   (ccdf) for this distribution evaluated at point <c>x</c>.
        ///   This function is also known as the Survival function.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        protected internal override double InnerComplementaryDistributionFunction(double x)
        {
            double z = Math.Log(x) / sigma;
            return Math.Pow(Normal.Function(-z), power);
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
            return new PowerLognormalDistribution(power, sigma);
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
            return String.Format(formatProvider, "PLD(x; p = {0}, σ = {1})",
                power.ToString(format, formatProvider),
                sigma.ToString(format, formatProvider));
        }



        private void initialize(double power, double sigma)
        {
            this.power = power;
            this.sigma = sigma;
        }

    }
}
