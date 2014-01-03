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
    using AForge;

    /// <summary>
    ///   Gompertz distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Gompertz distribution is a continuous probability distribution. The
    ///   Gompertz distribution is often applied to describe the distribution of 
    ///   adult lifespans by demographers and actuaries. Related fields of science
    ///   such as biology and gerontology also considered the Gompertz distribution
    ///   for the analysis of survival. More recently, computer scientists have also
    ///   started to model the failure rates of computer codes by the Gompertz 
    ///   distribution. In marketing science, it has been used as an individual-level 
    ///   model of customer lifetime.</para>
    /// 
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Gamma_distribution">
    ///       Wikipedia, The Free Encyclopedia. Gompertz distribution. Available on: 
    ///       http://en.wikipedia.org/wiki/Gompertz_distribution </a></description></item>
    ///   </list></para>     
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example shows how to construct a Gompertz 
    ///   distribution with <c>η = 4.2</c> and <c>b = 1.1</c>.</para>
    ///
    /// <code>
    ///   // Create a new Gompertz distribution with η = 4.2 and b = 1.1
    ///   GompertzDistribution dist = new GompertzDistribution(eta: 4.2, b: 1.1);
    ///   
    ///   // Common measures
    ///   double median = dist.Median; // 0.13886469671401389
    ///   
    ///   // Cumulative distribution functions
    ///   double cdf = dist.DistributionFunction(x: 0.27); // 0.76599768199799145
    ///   double ccdf = dist.ComplementaryDistributionFunction(x: 0.27); // 0.23400231800200855
    ///   double icdf = dist.InverseDistributionFunction(p: cdf); // 0.26999999999766749
    ///   
    ///   // Probability density functions
    ///   double pdf = dist.ProbabilityDensityFunction(x: 0.27); // 1.4549484164912097
    ///   double lpdf = dist.LogProbabilityDensityFunction(x: 0.27); // 0.37497044741163688
    ///   
    ///   // Hazard (failure rate) functions
    ///   double hf = dist.HazardFunction(x: 0.27); // 6.2176666834502088
    ///   double chf = dist.CumulativeHazardFunction(x: 0.27); // 1.4524242576820101
    ///   
    ///   // String representation
    ///   string str = dist.ToString(CultureInfo.InvariantCulture); // "Gompertz(x; η = 4.2, b = 1.1)"
    /// </code>
    /// </example>
    /// 
    public class GompertzDistribution : UnivariateContinuousDistribution
    {

        private double eta;
        private double b;


        /// <summary>
        ///   Initializes a new instance of the <see cref="GompertzDistribution"/> class.
        /// </summary>
        /// 
        /// <param name="eta">The shape parameter <c>η</c>.</param>
        /// <param name="b">The scale parameter <c>b</c>.</param>
        /// 
        public GompertzDistribution(double eta, double b)
        {
            this.eta = eta;
            this.b = b;
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
        public override double Variance
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        ///   Gets the median for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's median value.
        /// </value>
        /// 
        public override double Median
        {
            get
            {
                return (1.0 / b) * Math.Log((-1 / eta) * Math.Log(0.5) + 1);
            }
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
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        public override double DistributionFunction(double x)
        {
            return 1 - Math.Exp(-eta * (Math.Exp(b * x) - 1));
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
        public override double ProbabilityDensityFunction(double x)
        {
            return (b * eta * Math.Exp(eta)) * Math.Exp(b * x) * Math.Exp(-eta * Math.Exp(b * x));
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
        public override double LogProbabilityDensityFunction(double x)
        {
            return (Math.Log(b * eta) + eta) + b * x - eta * Math.Exp(b * x);
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
            return new GompertzDistribution(eta, b);
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
            return String.Format("Gompertz(x; η = {0}, b = {1})", eta, b);
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
            return String.Format(formatProvider, "Gompertz(x; η = {0}, b = {1})", eta, b);
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
            return String.Format("Gompertz(x; η = {0}, b = {1})",
                eta.ToString(format, formatProvider),
                b.ToString(format, formatProvider));
        }
    }
}
