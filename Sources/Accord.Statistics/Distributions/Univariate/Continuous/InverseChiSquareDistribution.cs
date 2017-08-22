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
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Compat;

    /// <summary>
    ///   Inverse chi-Square (χ²) probability distribution
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In probability and statistics, the inverse-chi-squared distribution (or 
    ///   inverted-chi-square distribution) is a continuous probability distribution
    ///   of a positive-valued random variable. It is closely related to the 
    ///   <see cref="ChiSquareDistribution">chi-squared distribution</see> and its 
    ///   specific importance is that it arises in the application of Bayesian 
    ///   inference to the normal distribution, where it can be used as the 
    ///   prior and posterior distribution for an unknown variance.</para>
    ///   
    /// <para>
    ///   The inverse-chi-squared distribution (or inverted-chi-square distribution) is
    ///   the probability distribution of a random variable whose multiplicative inverse
    ///   (reciprocal) has a <see cref="ChiSquareDistribution">chi-squared distribution</see>.
    ///   It is also often defined as the distribution of a random variable whose reciprocal
    ///   divided by its degrees of freedom is a chi-squared distribution. That is, if X has
    ///   the chi-squared distribution with <c>v</c> degrees of freedom, then according to 
    ///   the first definition, 1/X has the inverse-chi-squared distribution with <c>v</c>
    ///   degrees of freedom; while according to the second definition, <c>v</c>X has the 
    ///   inverse-chi-squared distribution with <c>v</c> degrees of freedom. Only the first 
    ///   definition is covered by this class.
    /// </para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Inverse-chi-squared_distribution">
    ///       Wikipedia, The Free Encyclopedia. Inverse-chi-square distribution. Available on: 
    ///       http://en.wikipedia.org/wiki/Inverse-chi-squared_distribution </a></description></item>
    ///   </list></para>     
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example demonstrates how to create a new inverse
    ///   χ² distribution with the given degrees of freedom. </para>
    ///   
    /// <code>
    ///   // Create a new inverse χ² distribution with 7 d.f.
    ///   var invchisq = new InverseChiSquareDistribution(degreesOfFreedom: 7);
    ///   double mean = invchisq.Mean;     // 0.2
    ///   double median = invchisq.Median; // 6.345811068141737
    ///   double var = invchisq.Variance;  // 75
    ///   
    ///   double cdf = invchisq.DistributionFunction(x: 6.27);           // 0.50860033566176044
    ///   double pdf = invchisq.ProbabilityDensityFunction(x: 6.27);     // 0.0000063457380298844403
    ///   double lpdf = invchisq.LogProbabilityDensityFunction(x: 6.27); // -11.967727146795536
    ///   
    ///   double ccdf = invchisq.ComplementaryDistributionFunction(x: 6.27); // 0.49139966433823956
    ///   double icdf = invchisq.InverseDistributionFunction(p: cdf);        // 6.2699998329362963
    ///   
    ///   double hf = invchisq.HazardFunction(x: 6.27);            // 0.000012913598625327002
    ///   double chf = invchisq.CumulativeHazardFunction(x: 6.27); // 0.71049750196765715
    ///   
    ///   string str = invchisq.ToString(); // "Inv-χ²(x; df = 7)"
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="ChiSquareDistribution"/>
    /// 
    [Serializable]
    public class InverseChiSquareDistribution : UnivariateContinuousDistribution
    {
        // Distribution parameters
        private int degreesOfFreedom;


        /// <summary>
        ///   Constructs a new Inverse Chi-Square distribution 
        ///   with the given degrees of freedom.
        /// </summary>
        /// 
        /// <param name="degreesOfFreedom">The degrees of freedom for the distribution.</param>
        /// 
        public InverseChiSquareDistribution([PositiveInteger] int degreesOfFreedom)
        {
            if (degreesOfFreedom <= 0)
            {
                throw new ArgumentOutOfRangeException("degreesOfFreedom",
                    "The number of degrees of freedom must be higher than zero.");
            }

            this.degreesOfFreedom = degreesOfFreedom;
        }

        /// <summary>
        ///   Gets the Degrees of Freedom for this distribution.
        /// </summary>
        /// 
        public int DegreesOfFreedom
        {
            get { return degreesOfFreedom; }
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   the χ² distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.</para>
        /// </remarks>
        /// 
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.</returns>
        ///   
        protected internal override double InnerProbabilityDensityFunction(double x)
        {
            double v = degreesOfFreedom;
            double a = Math.Pow(2, -v / 2);
            double b = Math.Pow(x, -v / 2 - 1);
            double c = Math.Exp(-1 / (2 * x));
            double d = Gamma.Function(v / 2);
            return (a * b * c) / d;
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   the χ² distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.</para>
        /// </remarks>
        /// 
        protected internal override double InnerDistributionFunction(double x)
        {
            double result = Gamma.UpperIncomplete(degreesOfFreedom / 2.0, (1 / (2.0 * x)));
            return result;
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
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        public override double Mean
        {
            get
            {
                if (degreesOfFreedom > 2)
                    return 1.0 / (degreesOfFreedom - 2.0);
                return Double.NaN;
            }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        public override double Variance
        {
            get
            {
                double v = degreesOfFreedom;

                if (v > 4)
                    return (v - 2) * (v - 2) * (v - 4);
                return Double.NaN;
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
            get { return 1.0 / (degreesOfFreedom + 2); }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        public override double Entropy
        {
            get
            {
                double v = degreesOfFreedom;

                return v / 2
                    + Math.Log(0.5 * Gamma.Function(v / 2))
                    - (1 - v / 2) * Gamma.Digamma(v / 2);
            }
        }

        /// <summary>
        ///   This method is not supported.
        /// </summary>
        /// 
        public override void Fit(double[] observations, double[] weights, IFittingOptions options)
        {
            throw new NotSupportedException();
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
            return new InverseChiSquareDistribution(degreesOfFreedom);
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
            return String.Format(formatProvider, "Inv-χ²(x; df = {0})",
                degreesOfFreedom.ToString(format, formatProvider));
        }
    }

}