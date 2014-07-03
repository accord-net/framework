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
    ///   Gumbel distribution (as known as the Extreme Value Type I distribution).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In probability theory and statistics, the Gumbel distribution is used to model
    ///   the distribution of the maximum (or the minimum) of a number of samples of various
    ///   distributions. Such a distribution might be used to represent the distribution of 
    ///   the maximum level of a river in a particular year if there was a list of maximum 
    ///   values for the past ten years. It is useful in predicting the chance that an extreme 
    ///   earthquake, flood or other natural disaster will occur.</para>
    ///
    /// <para>
    ///   The potential applicability of the Gumbel distribution to represent the distribution 
    ///   of maxima relates to extreme value theory which indicates that it is likely to be useful
    ///   if the distribution of the underlying sample data is of the normal or exponential type.</para>
    ///
    /// <para>
    ///   The Gumbel distribution is a particular case of the generalized extreme value
    ///   distribution (also known as the Fisher-Tippett distribution). It is also known 
    ///   as the log-Weibull distribution and the double exponential distribution (a term
    ///   that is alternatively sometimes used to refer to the Laplace distribution). It 
    ///   is related to the Gompertz distribution[citation needed]: when its density is 
    ///   first reflected about the origin and then restricted to the positive half line,
    ///   a Gompertz function is obtained.</para>
    ///
    /// <para>
    ///   In the latent variable formulation of the multinomial logit model — common in 
    ///   discrete choice theory — the errors of the latent variables follow a Gumbel 
    ///   distribution. This is useful because the difference of two Gumbel-distributed 
    ///   random variables has a logistic distribution.</para>
    ///
    /// <para>
    ///   The Gumbel distribution is named after Emil Julius Gumbel (1891–1966), based on
    ///   his original papers describing the distribution.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Gumbel_distribution">
    ///       Wikipedia, The Free Encyclopedia. Gumbel distribution. Available on: 
    ///       http://en.wikipedia.org/wiki/Gumbel_distribution </a></description></item>
    ///   </list></para>     
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example shows how to create and test the main characteristics
    ///   of an Gumbel distribution given its location and scale parameters: </para>
    ///   
    /// <code>
    /// var gumbel = new GumbelDistribution(location: 4.795, scale: 1 / 0.392);
    /// 
    /// double mean = gumbel.Mean;     // 6.2674889410753387
    /// double median = gumbel.Median; // 5.7299819402593481
    /// double mode = gumbel.Mode;     // 4.7949999999999999
    /// double var = gumbel.Variance;  // 10.704745853604138
    /// 
    /// double cdf = gumbel.DistributionFunction(x: 3.4); // 0.17767760424788051
    /// double pdf = gumbel.ProbabilityDensityFunction(x: 3.4); // 0.12033954114322486
    /// double lpdf = gumbel.LogProbabilityDensityFunction(x: 3.4); // -2.1174380222001519
    /// 
    /// double ccdf = gumbel.ComplementaryDistributionFunction(x: 3.4); // 0.82232239575211952
    /// double icdf = gumbel.InverseDistributionFunction(p: cdf); // 3.3999999904866245
    /// 
    /// double hf = gumbel.HazardFunction(x: 1.4); // 0.03449691276402958
    /// double chf = gumbel.CumulativeHazardFunction(x: 1.4); // 0.022988793482259906
    /// 
    /// string str = gumbel.ToString(CultureInfo.InvariantCulture); // Gumbel(x; μ = 4.795, β = 2.55)
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class GumbelDistribution : UnivariateContinuousDistribution
    {

        // Distribution parameters
        double mean; // location (μ)
        double beta; // scale (β)


        /// <summary>
        ///   Creates a new Gumbel distribution 
        ///   with the given location and scale.
        /// </summary>
        /// 
        /// <param name="location">The location parameter μ (mu).</param>
        /// <param name="scale">The scale parameter β (beta).</param>
        /// 
        public GumbelDistribution(double location, double scale)
        {
            init(location, scale);
        }

        private void init(double location, double scale)
        {
            this.mean = location;
            this.beta = scale;
        }

        /// <summary>
        ///   Gets the distribution's location parameter mu (μ).
        /// </summary>
        /// 
        public double Location
        {
            get { return mean; }
        }

        /// <summary>
        ///   Gets the distribution's scale parameter beta (β).
        /// </summary>
        /// 
        public double Shape
        {
            get { return beta; }
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mean value.</value>
        /// 
        public override double Mean
        {
            get { return mean + beta * Constants.EulerGamma; }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's variance.</value>
        /// 
        public override double Variance
        {
            get { return ((Math.PI * Math.PI) / 6.0) * beta * beta; }
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
        ///   Gets the median for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's median value.</value>
        /// 
        public override double Median
        {
            get
            {
                double median = mean - beta * Math.Log(Constants.Log2);
                System.Diagnostics.Debug.Assert(median.IsRelativelyEqual(base.Median, 1e-6));
                return median;
            }
        }

        /// <summary>
        ///   Gets the mode for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mode value.</value>
        /// 
        public override double Mode
        {
            get { return mean; }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's entropy.</value>
        /// 
        public override double Entropy
        {
            get { return Math.Log(beta) + Constants.EulerGamma + 1; }
        }

        /// <summary>
        ///  Gets the cumulative distribution function (cdf) for
        ///  this distribution evaluated at point <c>x</c>.
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
            double z = (x - mean) / beta;
            return Math.Exp(-Math.Exp(-z));
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
            double z = (x - mean) / beta;
            return (1 / beta) * Math.Exp(-(z + Math.Exp(-z)));
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
        /// <seealso cref="ProbabilityDensityFunction"/>
        /// 
        public override double LogProbabilityDensityFunction(double x)
        {
            double z = (x - mean) / beta;
            return Math.Log(1 / beta) - (z + Math.Exp(-z));
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
            return new GumbelDistribution(mean, beta);
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
            return String.Format("Gumbel(x; μ = {0}, β = {1})", mean, beta);
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
            return String.Format(formatProvider, "Gumbel(x; μ = {0}, β = {1})",
                mean, beta);
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
            return String.Format("Gumbel(x; μ = {0}, β = {1})",
                mean.ToString(format, formatProvider),
                beta.ToString(format, formatProvider));
        }
    }
}
