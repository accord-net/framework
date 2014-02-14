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
    using Accord.Statistics.Distributions.Fitting;
    using AForge;

    /// <summary>
    ///   Chi-Square (χ²) probability distribution
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In probability theory and statistics, the chi-square distribution (also chi-squared
    ///   or χ²-distribution) with k degrees of freedom is the distribution of a sum of the 
    ///   squares of k independent standard normal random variables. It is one of the most 
    ///   widely used probability distributions in inferential statistics, e.g. in hypothesis 
    ///   testing, or in construction of confidence intervals.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Chi-square_distribution">
    ///       Wikipedia, The Free Encyclopedia. Chi-square distribution. Available on: 
    ///       http://en.wikipedia.org/wiki/Chi-square_distribution </a></description></item>
    ///   </list></para>     
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example demonstrates how to create a new χ² 
    ///   distribution with the given degrees of freedom. </para>
    ///   
    /// <code>
    ///   // Create a new χ² distribution with 7 d.f.
    ///   var chisq = new ChiSquareDistribution(degreesOfFreedom: 7);
    /// 
    ///   // Common measures
    ///   double mean = chisq.Mean;     //  7
    ///   double median = chisq.Median; //  6.345811195595612
    ///   double var = chisq.Variance;  // 14
    /// 
    ///   // Cumulative distribution functions
    ///   double cdf = chisq.DistributionFunction(x: 6.27);           // 0.49139966433823956
    ///   double ccdf = chisq.ComplementaryDistributionFunction(x: 6.27); // 0.50860033566176044
    ///   double icdf = chisq.InverseDistributionFunction(p: cdf);        // 6.2700000000852318
    ///   
    ///   // Probability density functions
    ///   double pdf = chisq.ProbabilityDensityFunction(x: 6.27);     // 0.11388708001184455
    ///   double lpdf = chisq.LogProbabilityDensityFunction(x: 6.27); // -2.1725478476948092
    /// 
    ///   // Hazard (failure rate) functions
    ///   double hf = chisq.HazardFunction(x: 6.27);            // 0.22392254197721179
    ///   double chf = chisq.CumulativeHazardFunction(x: 6.27); // 0.67609276602233315
    ///   
    ///   // String representation
    ///   string str = chisq.ToString(); // "χ²(x; df = 7)
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class ChiSquareDistribution : UnivariateContinuousDistribution,
        ISampleableDistribution<double>
    {
        // Distribution parameters
        private int degreesOfFreedom;

        // Distribution measures
        private double? entropy;


        /// <summary>
        ///   Constructs a new Chi-Square distribution
        ///   with given degrees of freedom.
        /// </summary>
        /// 
        /// <param name="degreesOfFreedom">The degrees of freedom for the distribution.</param>
        /// 
        public ChiSquareDistribution(int degreesOfFreedom)
        {
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
        /// <para>
        ///   References:
        ///   <list type="bullet">
        ///     <item><description>
        ///       <a href="http://en.wikipedia.org/wiki/Chi-square_distribution#Probability_density_function">
        ///       Wikipedia, the free encyclopedia. Chi-square distribution. </a></description></item>
        ///   </list></para>
        /// </remarks>
        /// 
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.</returns>
        ///   
        public override double ProbabilityDensityFunction(double x)
        {
            double v = degreesOfFreedom;
            double m1 = Math.Pow(x, (v - 2.0) / 2.0);
            double m2 = Math.Exp(-x / 2.0);
            double m3 = Math.Pow(2, v / 2.0) * Gamma.Function(v / 2.0);
            return (m1 * m2) / m3;
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
        public override double LogProbabilityDensityFunction(double x)
        {
            double v = degreesOfFreedom;
            double m1 = ((v - 2.0) / 2.0) * Math.Log(x);
            double m2 = (-x / 2.0);
            double m3 = (v / 2.0) * Math.Log(2) + Gamma.Log(v / 2.0);
            return (m1 + m2) - m3;
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
        ///   
        /// <para>
        ///   The χ² distribution function is defined in terms of the <see cref="Gamma.LowerIncomplete">
        ///   Incomplete Gamma Function Γ(a, x)</see> as CDF(x; df) = Γ(df/2, x/d). </para>
        /// </remarks>
        /// 
        public override double DistributionFunction(double x)
        {
            return Gamma.LowerIncomplete(degreesOfFreedom / 2.0, x / 2.0);
        }

        /// <summary>
        ///   Gets the complementary cumulative distribution function
        ///   (ccdf) for the χ² distribution evaluated at point <c>x</c>.
        ///   This function is also known as the Survival function.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   The Complementary Cumulative Distribution Function (CCDF) is
        ///   the complement of the Cumulative Distribution Function, or 1
        ///   minus the CDF.</para>
        ///   
        /// <para>
        ///   The χ² complementary distribution function is defined in terms of the 
        ///   <see cref="Gamma.UpperIncomplete">Complemented Incomplete Gamma 
        ///   Function Γc(a, x)</see> as CDF(x; df) = Γc(df/2, x/d). </para>
        /// </remarks>
        /// 
        public override double ComplementaryDistributionFunction(double x)
        {
            return Gamma.UpperIncomplete(degreesOfFreedom / 2.0, x / 2.0);
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
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <remarks>
        ///   The χ² distribution mean is the number of degrees of freedom.
        /// </remarks>
        /// 
        public override double Mean
        {
            get { return degreesOfFreedom; }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <remarks>
        ///   The χ² distribution variance is twice its degrees of freedom.
        /// </remarks>
        /// 
        public override double Variance
        {
            get { return 2.0 * degreesOfFreedom; }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        public override double Entropy
        {
            get
            {
                if (!entropy.HasValue)
                {
                    double kd2 = degreesOfFreedom / 2.0;
                    double m1 = Math.Log(2.0 * Gamma.Function(kd2));
                    double m2 = (1.0 - kd2) * Gamma.Digamma(kd2);
                    entropy = kd2 + m1 + m2;
                }

                return entropy.Value;
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
            return new ChiSquareDistribution(degreesOfFreedom);
        }

        #region ISamplableDistribution<double> Members

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public double[] Generate(int samples)
        {
            return GammaDistribution.Random(shape: degreesOfFreedom / 2.0, scale: 2, samples: samples);
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public double Generate()
        {
            return GammaDistribution.Random(shape: degreesOfFreedom / 2.0, scale: 2);
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Chi-Square distribution with the given parameters.
        /// </summary>
        /// 
        /// <returns>An array of double values sampled from the specified Chi-Square distribution.</returns>
        /// 
        public static double[] Random(int degreesOfFreedom, int samples)
        {
            return GammaDistribution.Random(shape: degreesOfFreedom / 2.0, scale: 2, samples: samples);
        }


        /// <summary>
        ///   Generates a random observation from the 
        ///   Chi-Square distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="degreesOfFreedom">The degrees of freedom for the distribution.</param>
        /// 
        /// <returns>A random double value sampled from the specified Chi-Square distribution.</returns>
        /// 
        public static double Random(int degreesOfFreedom)
        {
            return GammaDistribution.Random(shape: degreesOfFreedom / 2.0, scale: 2);
        }

        #endregion

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
            return String.Format("χ²(x; df = {0})", degreesOfFreedom);
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
            return String.Format(formatProvider, "χ²(x; df = {0})", degreesOfFreedom);
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
            return String.Format("χ²(x; df = {0})", 
                degreesOfFreedom.ToString(format, formatProvider));
        }
    }

}