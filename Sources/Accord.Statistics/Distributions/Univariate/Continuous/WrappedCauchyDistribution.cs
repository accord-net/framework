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
    using Accord.Statistics.Distributions.Fitting;
    using AForge;
    using AForge.Math;

    /// <summary>
    ///   Wrapped Cauchy Distribution.
    /// </summary>
    /// 
    /// <remarks>
    ///  <para>
    ///   In probability theory and directional statistics, a wrapped Cauchy distribution
    ///   is a wrapped probability distribution that results from the "wrapping" of the 
    ///   Cauchy distribution around the unit circle. The Cauchy distribution is sometimes
    ///   known as a Lorentzian distribution, and the wrapped Cauchy distribution may 
    ///   sometimes be referred to as a wrapped Lorentzian distribution.</para>
    ///   
    ///  <para>
    ///   The wrapped Cauchy distribution is often found in the field of spectroscopy where
    ///   it is used to analyze diffraction patterns (e.g. see Fabry–Pérot interferometer)</para>.
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Directional_statistics">
    ///       Wikipedia, The Free Encyclopedia. Directional statistics. Available on:
    ///       http://en.wikipedia.org/wiki/Directional_statistics </a></description></item>
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Wrapped_Cauchy_distribution">
    ///       Wikipedia, The Free Encyclopedia. Wrapped Cauchy distribution. Available on:
    ///       http://en.wikipedia.org/wiki/Wrapped_Cauchy_distribution </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///   // Create a Wrapped Cauchy distribution with μ = 0.42, γ = 3
    ///   var dist = new WrappedCauchyDistribution(mu: 0.42, gamma: 3);
    ///   
    ///   // Common measures
    ///   double mean = dist.Mean;     // 0.42
    ///   double var = dist.Variance;  // 0.950212931632136
    ///    
    ///   // Probability density functions
    ///   double pdf = dist.ProbabilityDensityFunction(x: 0.42); // 0.1758330112785475
    ///   double lpdf = dist.LogProbabilityDensityFunction(x: 0.42); // -1.7382205338929015
    ///   
    ///   // String representation
    ///   string str = dist.ToString(); // "WrappedCauchy(x; μ = 0,42, γ = 3)"
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="CauchyDistribution"/>
    /// 
    public class WrappedCauchyDistribution : UnivariateContinuousDistribution,
        IFittableDistribution<double, CauchyOptions>
    {

        private double mu;
        private double gamma;

        /// <summary>
        ///   Initializes a new instance of the <see cref="WrappedCauchyDistribution"/> class.
        /// </summary>
        /// 
        /// <param name="mu">The mean resultant parameter μ.</param>
        /// <param name="gamma">The gamma parameter γ.</param>
        /// 
        public WrappedCauchyDistribution(double mu, double gamma)
        {
            if (gamma <= 0)
                throw new ArgumentOutOfRangeException("gamma", "Gamma must be positive.");

            this.mu = mu;
            this.gamma = gamma;
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's mean value.
        /// </value>
        /// 
        public override double Mean
        {
            get { return mu; }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's variance.
        /// </value>
        /// 
        public override double Variance
        {
            get { return 1 - Math.Exp(-gamma); }
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
            get { return new DoubleRange(-Math.PI, Math.PI); }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's entropy.
        /// </value>
        /// 
        public override double Entropy
        {
            get { return Math.Log(2 * Math.PI * (1 - Math.Exp(-2 * gamma))); }
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public override double DistributionFunction(double x)
        {
            throw new NotSupportedException();
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
            double constant = (1.0 / (2 * Math.PI));
            return constant * Math.Sinh(gamma) / (Math.Cosh(gamma) - Math.Cos(x - mu));
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
            return Math.Log(ProbabilityDensityFunction(x));
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
            return new WrappedCauchyDistribution(mu, gamma);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting, such
        ///   as regularization constants and additional parameters.</param>
        ///   
        public void Fit(double[] observations, double[] weights = null, CauchyOptions options = null)
        {
            double sin = 0, cos = 0;
            for (int i = 0; i < observations.Length; i++)
            {
                sin += Math.Sin(observations[i]);
                cos += Math.Cos(observations[i]);
            }

            mu = new Complex(sin, cos).Phase;

            double N = observations.Length;
            double R2 = (cos / N) * (cos / N) + (sin / N) * (sin / N);
            double R2e = N / (N - 1) * (R2 - 1 / N);

            gamma = Math.Log(1 / R2e) / 2;
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
            return String.Format("WrappedCauchy(x; μ = {0}, γ = {1})", mu, gamma);
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
            return String.Format(formatProvider, "WrappedCauchy(x; μ = {0}, γ = {1})", mu, gamma);
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
            return String.Format(formatProvider, "WrappedCauchy(x; μ = {0}, γ = {1})",
                mu.ToString(format, formatProvider),
                gamma.ToString(format, formatProvider));
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
            return String.Format("WrappedCauchy(x; μ = {0}, γ = {1})",
                mu.ToString(format), gamma.ToString(format));
        }
    }
}
