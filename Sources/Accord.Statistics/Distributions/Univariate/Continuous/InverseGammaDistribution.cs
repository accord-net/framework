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
    ///   Inverse Gamma Distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The inverse gamma distribution is a two-parameter family of continuous probability
    ///   distributions on the positive real line, which is the distribution of the reciprocal
    ///   of a variable distributed according to the gamma distribution. Perhaps the chief use
    ///   of the inverse gamma distribution is in Bayesian statistics, where it serves as the 
    ///   conjugate prior of the variance of a normal distribution. However, it is common among
    ///   Bayesians to consider an alternative parameterization of the normal distribution in 
    ///   terms of the precision, defined as the reciprocal of the variance, which allows the
    ///   gamma distribution to be used directly as a conjugate prior.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Inverse-gamma_distribution">
    ///       Wikipedia, The Free Encyclopedia. Inverse Gamma Distribution. 
    ///       Available from: http://en.wikipedia.org/wiki/Inverse-gamma_distribution </a></description></item>
    ///     <item><description><a href="http://www.johndcook.com/inverse gamma.pdf">
    ///       John D. Cook. (2008). The Inverse Gamma Distribution. </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///   // Create a new inverse Gamma distribution with α = 0.42 and β = 0.5
    ///   var invGamma = new InverseGammaDistribution(shape: 0.42, scale: 0.5);
    /// 
    ///   // Common measures
    ///   double mean   = invGamma.Mean;     // -0.86206896551724133
    ///   double median = invGamma.Median;   // 3.1072323347401709
    ///   double var    = invGamma.Variance; // -0.47035626665061164
    ///    
    ///   // Cumulative distribution functions
    ///   double cdf = invGamma.DistributionFunction(x: 0.27);           // 0.042243552114989695
    ///   double ccdf = invGamma.ComplementaryDistributionFunction(x: 0.27); // 0.95775644788501035
    ///   double icdf = invGamma.InverseDistributionFunction(p: cdf);        // 0.26999994629410995
    ///   
    ///   // Probability density functions
    ///   double pdf = invGamma.ProbabilityDensityFunction(x: 0.27);     // 0.35679850067181362
    ///   double lpdf = invGamma.LogProbabilityDensityFunction(x: 0.27); // -1.0305840804381006
    ///   
    ///   // Hazard (failure rate) functions
    ///   double hf = invGamma.HazardFunction(x: 0.27); // 0.3725357333377633
    ///   double chf = invGamma.CumulativeHazardFunction(x: 0.27); // 0.043161763098266373
    ///   
    ///   // String representation
    ///   string str = invGamma.ToString(); // Γ^(-1)(x; α = 0.42, β = 0.5)
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="GammaDistribution"/>
    /// 
    [Serializable]
    public class InverseGammaDistribution : UnivariateContinuousDistribution
    {
        double a;
        double b;

        double constant;

        /// <summary>
        ///   Creates a new Inverse Gamma Distribution.
        /// </summary>
        /// 
        /// <param name="shape">The shape parameter α (alpha).</param>
        /// <param name="scale">The scale parameter β (beta).</param>
        /// 
        public InverseGammaDistribution(double shape, double scale)
        {
            if (shape <= 0)
                throw new ArgumentOutOfRangeException("shape");

            if (scale <= 0) 
                throw new ArgumentOutOfRangeException("scale");

            this.a = shape;
            this.b = scale;

            this.constant = Math.Pow(b, a) / Gamma.Function(a);
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mean value.</value>
        /// 
        /// <remarks>
        ///   In the Inverse Gamma distribution, the Mean is given as b / (a - 1).
        /// </remarks>
        /// 
        public override double Mean
        {
            get { return b / (a - 1); }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's variance.</value>
        /// 
        /// <remarks>
        ///   In the Inverse Gamma distribution, the Variance is given as b² / ((a - 1)² * (a - 2)).
        /// </remarks>
        /// 
        public override double Variance
        {
            get { return (b * b) / ((a - 1) * (a - 1) * (a - 2)); }
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
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's entropy.</value>
        /// 
        public override double Entropy
        {
            get { return a + Math.Log(b * Gamma.Function(a)) - (1 + a) * Gamma.Digamma(a); }
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <remarks>
        /// <para>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.</para>
        ///   
        /// <para>
        ///   In the Inverse Gamma CDF is computed in terms of the <see cref="Gamma.UpperIncomplete">
        ///   Upper Incomplete Regularized Gamma Function Q</see> as CDF(x) = Q(a, b / x).</para>
        /// </remarks>
        /// 
        /// <example>
        ///   See <see cref="InverseGammaDistribution"/>.
        /// </example>
        /// 
        public override double DistributionFunction(double x)
        {
            if (x == 0) return 0;
            return Gamma.UpperIncomplete(a, b / x);
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
        ///   See <see cref="InverseGammaDistribution"/>.
        /// </example>
        /// 
        public override double ProbabilityDensityFunction(double x)
        {
            if (x <= 0) return 0;

            return constant * Math.Pow(x, -a - 1) * Math.Exp(-b / x);
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
        ///   See <see cref="InverseGammaDistribution"/>.
        /// </example>
        /// 
        public override double LogProbabilityDensityFunction(double x)
        {
            if (x <= 0) return 0;

            double lnx = Math.Log(x);
            return Math.Log(constant) + (-a - 1) * lnx + (-b / x);
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
            return new InverseGammaDistribution(a, b);
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
            return String.Format("Γ^(-1)(x; α = {0}, β = {1})", a, b);
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
            return String.Format(formatProvider, "Γ^(-1)(x; α = {0}, β = {1})", a, b);
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
            return String.Format("Γ^(-1)(x; α = {0}, β = {1})",
                a.ToString(format, formatProvider),
                b.ToString(format, formatProvider));
        }
    }
}
