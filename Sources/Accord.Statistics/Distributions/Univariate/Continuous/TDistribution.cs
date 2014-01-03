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
    using AForge;

    /// <summary>
    ///   Student's t-distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In probability and statistics, Student's t-distribution (or simply the
    ///   t-distribution) is a family of continuous probability distributions that
    ///   arises when estimating the mean of a normally distributed population in 
    ///   situations where the sample size is small and population standard deviation
    ///   is unknown. It plays a role in a number of widely used statistical analyses,
    ///   including the Student's t-test for assessing the statistical significance of
    ///   the difference between two sample means, the construction of confidence intervals 
    ///   for the difference between two population means, and in linear regression 
    ///   analysis. The Student's t-distribution also arises in the Bayesian analysis of 
    ///   data from a normal family.</para>
    /// <para>
    ///   If we take <c>k</c> samples from a normal distribution with fixed unknown mean and 
    ///   variance, and if we compute the sample mean and sample variance for these k 
    ///   samples, then the t-distribution (for k) can be defined as the distribution 
    ///   of the location of the true mean, relative to the sample mean and divided by
    ///   the sample standard deviation, after multiplying by the normalizing term 
    ///   <c>sqrt(n)</c>, where <c>n</c> is the sample size. In this way the t-distribution
    ///   can be used to estimate how likely it is that the true mean lies in any given
    ///   range.</para>
    /// <para>
    ///   The t-distribution is symmetric and bell-shaped, like the normal distribution,
    ///   but has heavier tails, meaning that it is more prone to producing values that 
    ///   fall far from its mean. This makes it useful for understanding the statistical 
    ///   behavior of certain types of ratios of random quantities, in which variation in
    ///   the denominator is amplified and may produce outlying values when the denominator
    ///   of the ratio falls close to zero. The Student's t-distribution is a special case 
    ///   of the generalized hyperbolic distribution.</para>
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Student's_t-distribution">
    ///       Wikipedia, The Free Encyclopedia. Student's t-distribution. Available on:
    ///       http://en.wikipedia.org/wiki/Student's_t-distribution </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///   // Create a new Student's T distribution with d.f = 4.2
    ///   TDistribution t = new TDistribution(degreesOfFreedom: 4.2);
    ///   
    ///   // Common measures
    ///   double mean = t.Mean;     // 0.0
    ///   double median = t.Median; // 0.0
    ///   double var = t.Variance;  // 1.9090909090909089
    ///   
    ///   // Cumulative distribution functions
    ///   double cdf = t.DistributionFunction(x: 1.4);           // 0.88456136730659074
    ///   double pdf = t.ProbabilityDensityFunction(x: 1.4);     // 0.13894002185341031
    ///   double lpdf = t.LogProbabilityDensityFunction(x: 1.4); // -1.9737129364307417
    ///   
    ///   // Probability density functions
    ///   double ccdf = t.ComplementaryDistributionFunction(x: 1.4); // 0.11543863269340926
    ///   double icdf = t.InverseDistributionFunction(p: cdf);       // 1.4000000000000012
    ///   
    ///   // Hazard (failure rate) functions
    ///   double hf = t.HazardFunction(x: 1.4);            // 1.2035833984833988
    ///   double chf = t.CumulativeHazardFunction(x: 1.4); // 2.1590162088918525
    ///   
    ///   // String representation
    ///   string str = t.ToString(CultureInfo.InvariantCulture); // T(x; df = 4.2)
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="Accord.Statistics.Testing.TTest"/>
    /// <seealso cref="NoncentralTDistribution"/>
    /// 
    [Serializable]
    public class TDistribution : UnivariateContinuousDistribution, IFormattable
    {

        private double constant;


        /// <summary>
        ///   Gets the degrees of freedom for the distribution.
        /// </summary>
        /// 
        public double DegreesOfFreedom { get; private set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="TDistribution"/> class.
        /// </summary>
        /// 
        /// <param name="degreesOfFreedom">The degrees of freedom.</param>
        /// 
        public TDistribution(double degreesOfFreedom)
        {
            if (degreesOfFreedom < 1)
                throw new ArgumentOutOfRangeException("degreesOfFreedom");

            this.DegreesOfFreedom = degreesOfFreedom;

            double v = degreesOfFreedom;

            // TODO: Use LogGamma instead.
            this.constant = Gamma.Function((v + 1) / 2.0) / (Math.Sqrt(v * Math.PI) * Gamma.Function(v / 2.0));
        }


        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        public override double Mean
        {
            get { return (DegreesOfFreedom > 1) ? 0 : Double.NaN; }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        public override double Variance
        {
            get
            {
                if (DegreesOfFreedom > 2)
                    return DegreesOfFreedom / (DegreesOfFreedom - 2);
                else if (DegreesOfFreedom > 1)
                    return Double.PositiveInfinity;
                return Double.NaN;
            }
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
            get { return new DoubleRange(Double.NegativeInfinity, Double.PositiveInfinity); }
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
        /// <example>
        ///   See <see cref="TDistribution"/>.
        /// </example>
        /// 
        public override double DistributionFunction(double x)
        {
            double v = DegreesOfFreedom;
            double sqrt = Math.Sqrt(x * x + v);
            double u = (x + sqrt) / (2 * sqrt);
            return Beta.Incomplete(v / 2.0, v / 2.0, u);
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
        /// <example>
        ///   See <see cref="TDistribution"/>.
        /// </example>
        /// 
        public override double ProbabilityDensityFunction(double x)
        {
            double v = DegreesOfFreedom;
            return constant * Math.Pow(1 + (x * x) / DegreesOfFreedom, -(v + 1) / 2.0);
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
        /// <example>
        ///   See <see cref="TDistribution"/>.
        /// </example>
        /// 
        public override double LogProbabilityDensityFunction(double x)
        {
            double v = DegreesOfFreedom;
            return Math.Log(constant) - ((v + 1) / 2.0) * Math.Log(1 + (x * x) / DegreesOfFreedom);
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
        /// <example>
        ///   See <see cref="TDistribution"/>.
        /// </example>
        /// 
        public override double InverseDistributionFunction(double p)
        {
            return inverseDistributionLeftTail(DegreesOfFreedom, p);
        }

        /// <summary>
        ///  Not supported.
        /// </summary>
        /// 
        public override void Fit(double[] observations, double[] weights, Fitting.IFittingOptions options)
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
            return new TDistribution(DegreesOfFreedom);
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
            return String.Format("T(x; df = {0})", DegreesOfFreedom);
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
            return String.Format(formatProvider, "T(x; df = {0})", DegreesOfFreedom);
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
            return String.Format(formatProvider, "T(x; df = {0})",
                DegreesOfFreedom.ToString(format, formatProvider));
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
            return String.Format("T(x; df = {0})",
                DegreesOfFreedom.ToString(format));
        }


        /// <summary>
        ///   Gets the inverse of the cumulative distribution function (icdf) for
        ///   the left tail T-distribution evaluated at probability <c>p</c>.
        /// </summary>
        /// 
        /// <remarks>
        ///   Based on the stdtril function from the Cephes Math Library
        ///   Release 2.8, adapted with permission of Stephen L. Moshier.
        /// </remarks>
        /// 
        private static double inverseDistributionLeftTail(double df, double p)
        {
            if (p > 0.25 && p < 0.75)
            {
                if (p == 0.5)
                    return 0;

                double u = 1.0 - 2.0 * p;
                double z = Beta.IncompleteInverse(0.5, 0.5 * df, Math.Abs(u));
                double t = Math.Sqrt(df * z / (1.0 - z));

                if (p < 0.5)
                    t = -t;

                return t;
            }
            else
            {
                int rflg = -1;

                if (p >= 0.5)
                {
                    p = 1.0 - p;
                    rflg = 1;
                }

                double z = Beta.IncompleteInverse(0.5 * df, 0.5, 2.0 * p);

                if ((Double.MaxValue * z) < df)
                    return rflg * Double.MaxValue;

                double t = Math.Sqrt(df / z - df);
                return rflg * t;
            }
        }

    }
}
