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
    ///   Laplace's Distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In probability theory and statistics, the Laplace distribution is a continuous
    ///   probability distribution named after Pierre-Simon Laplace. It is also sometimes called
    ///   the double exponential distribution.</para>
    /// 
    /// <para>
    ///   The difference between two independent identically distributed exponential random
    ///   variables is governed by a Laplace distribution, as is a Brownian motion evaluated at an
    ///   exponentially distributed random time. Increments of Laplace motion or a variance gamma 
    ///   process evaluated over the time scale also have a Laplace distribution.</para>
    /// 
    /// <para>
    ///    The probability density function of the Laplace distribution is also reminiscent of the
    ///    normal distribution; however, whereas the normal distribution is expressed in terms of 
    ///    the squared difference from the mean μ, the Laplace density is expressed in terms of the 
    ///    absolute difference from the mean. Consequently the Laplace distribution has fatter tails
    ///    than the normal distribution.</para>
    ///    
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Laplace_distribution">
    ///       Wikipedia, The Free Encyclopedia. Laplace distribution. 
    ///       Available from: http://en.wikipedia.org/wiki/Laplace_distribution </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///    // Create a new Laplace distribution with μ = 4 and b = 2
    ///    var laplace = new LaplaceDistribution(location: 4, scale: 2);
    ///    
    ///    // Common measures
    ///    double mean = laplace.Mean;     // 4.0
    ///    double median = laplace.Median; // 4.0
    ///    double var = laplace.Variance;  // 8.0
    ///    
    ///    // Cumulative distribution functions
    ///    double cdf = laplace.DistributionFunction(x: 0.27);               // 0.077448104942453522
    ///    double ccdf = laplace.ComplementaryDistributionFunction(x: 0.27); // 0.92255189505754642
    ///    double icdf = laplace.InverseDistributionFunction(p: cdf);        // 0.27
    ///    
    ///    // Probability density functions
    ///    double pdf = laplace.ProbabilityDensityFunction(x: 0.27);     //  0.038724052471226761
    ///    double lpdf = laplace.LogProbabilityDensityFunction(x: 0.27); // -3.2512943611198906
    ///    
    ///    // Hazard (failure rate) functions
    ///    double hf = laplace.HazardFunction(x: 0.27); // 0.041974931360160776
    ///    double chf = laplace.CumulativeHazardFunction(x: 0.27); // 0.080611649844768624
    ///     
    ///    // String representation
    ///    string str = laplace.ToString(CultureInfo.InvariantCulture); // Laplace(x; μ = 4, b = 2)
    /// </code>
    /// </example>
    /// 
    public class LaplaceDistribution : UnivariateContinuousDistribution, IFormattable
    {

        double u; // location parameter μ (mu)
        double b; // scale parameter b

        double constant; // 1 / (2 * b)

        /// <summary>
        ///   Creates a new Laplace distribution.
        /// </summary>
        /// 
        /// <param name="location">The location parameter μ (mu).</param>
        /// <param name="scale">The scale parameter b.</param>
        /// 
        public LaplaceDistribution(double location, double scale)
        {
            if (scale <= 0) throw new ArgumentOutOfRangeException(
                "scale", "Scale must be non-negative.");

            this.u = location;
            this.b = scale;

            this.constant = 1.0 / (2.0 * b);
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mean value.</value>
        /// 
        /// <remarks>
        ///   The Laplace's distribution mean has the 
        ///   same value as the location parameter μ.
        /// </remarks>
        /// 
        public override double Mean
        {
            get { return u; }
        }

        /// <summary>
        ///   Gets the median for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's median value.
        /// </value>
        /// 
        /// <remarks>
        ///   The Laplace's distribution median has the 
        ///   same value as the location parameter μ.
        /// </remarks>
        /// 
        public override double Median
        {
            get { return u; }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's variance.</value>
        /// 
        /// <remarks>
        ///   The Laplace's variance is computed as 2*b².
        /// </remarks>
        /// 
        public override double Variance
        {
            get { return 2 * b * b; }
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
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's entropy.</value>
        /// 
        /// <remarks>
        ///   The Laplace's entropy is defined as ln(2*e*b), in which
        ///   <c>e</c> is the <see cref="Math.E">Euler constant</see>.
        /// </remarks>
        /// 
        public override double Entropy
        {
            get { return Math.Log(2 * Math.E * b); }
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
        ///   See <see cref="LaplaceDistribution"/>.
        /// </example>
        /// 
        public override double DistributionFunction(double x)
        {
            return 0.5 * (1 + Math.Sign(x - u) * (1 - Math.Exp(-Math.Abs(x - u) / b)));
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
        ///   See <see cref="LaplaceDistribution"/>.
        /// </example>
        /// 
        public override double ProbabilityDensityFunction(double x)
        {
            return constant * Math.Exp(-Math.Abs(x - u) / b);
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
        ///   See <see cref="LaplaceDistribution"/>.
        /// </example>
        /// 
        public override double LogProbabilityDensityFunction(double x)
        {
            return Math.Log(constant) - Math.Abs(x - u) / b;
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
            return new LaplaceDistribution(u, b);
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
            return String.Format("Laplace(x; μ = {0}, b = {1})", u, b);
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
            return String.Format(formatProvider, "Laplace(x; μ = {0}, b = {1})", u, b);
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
            return String.Format("Laplace(x; μ = {0}, b = {1})", 
                u.ToString(format, formatProvider),
                b.ToString(format, formatProvider));
        }
    }
}
