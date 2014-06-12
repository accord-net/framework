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
    ///   Pareto's Distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Pareto distribution, named after the Italian economist Vilfredo Pareto, is a power law
    ///   probability distribution that coincides with social, scientific, geophysical, actuarial, 
    ///   and many other types of observable phenomena. Outside the field of economics it is sometimes
    ///   referred to as the Bradford distribution.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Pareto_distribution">
    ///       Wikipedia, The Free Encyclopedia. Pareto distribution. 
    ///       Available from: http://en.wikipedia.org/wiki/Pareto_distribution </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///    // Creates a new Pareto's distribution with xm = 0.42, α = 3
    ///    var pareto = new ParetoDistribution(scale: 0.42, shape: 3);
    ///    
    ///    // Common measures
    ///    double mean   = pareto.Mean;     // 0.63
    ///    double median = pareto.Median;   // 0.52916684095584676
    ///    double var    = pareto.Variance; // 0.13229999999999997
    ///    
    ///    // Cumulative distribution functions
    ///    double cdf = pareto.DistributionFunction(x: 1.4);           // 0.973
    ///    double ccdf = pareto.ComplementaryDistributionFunction(x: 1.4); // 0.027000000000000024
    ///    double icdf = pareto.InverseDistributionFunction(p: cdf);       // 1.4000000446580794
    ///    
    ///    // Probability density functions
    ///    double pdf = pareto.ProbabilityDensityFunction(x: 1.4);     // 0.057857142857142857
    ///    double lpdf = pareto.LogProbabilityDensityFunction(x: 1.4); // -2.8497783609309111
    ///    
    ///    // Hazard (failure rate) functions
    ///    double hf = pareto.HazardFunction(x: 1.4);            // 2.142857142857141
    ///    double chf = pareto.CumulativeHazardFunction(x: 1.4); // 3.6119184129778072
    ///    
    ///    // String representation
    ///    string str = pareto.ToString(CultureInfo.InvariantCulture); // Pareto(x; xm = 0.42, α = 3)
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class ParetoDistribution : UnivariateContinuousDistribution,
        IFormattable, ISampleableDistribution<double>
    {

        double xm; // x_m
        double alpha;  // alpha

        /// <summary>
        ///   Creates new Pareto distribution.
        /// </summary>
        /// 
        /// <param name="scale">The scale parameter x<sub>m</sub>.</param>
        /// <param name="shape">The shape parameter α (alpha).</param>
        /// 
        public ParetoDistribution(double scale, double shape)
        {
            init(scale, shape);
        }

        private void init(double scale, double shape)
        {
            this.xm = scale;
            this.alpha = shape;
        }

        public double Scale
        {
            get { return xm; }
        }

        public double Alpha
        {
            get { return alpha; }
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mean value.</value>
        /// 
        /// <remarks>
        ///   The Pareto distribution's mean is defined as
        ///   <c>α * x<sub>m</sub> / (α - 1)</c>.
        /// </remarks>
        /// 
        public override double Mean
        {
            get { return (alpha * xm) / (alpha - 1); }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's variance.</value>
        /// 
        /// <remarks>
        ///   The Pareto distribution's mean is defined as
        ///   <c>α * x<sub>m</sub>² / ((α - 1)² * (α - 2)</c>.
        /// </remarks>
        /// 
        public override double Variance
        {
            get { return (xm * xm * alpha) / ((alpha - 1) * (alpha - 1) * (alpha - 2)); }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's entropy.</value>
        /// 
        /// <remarks>
        ///   The Pareto distribution's Entropy is defined as
        ///   <c>ln(x<sub>m</sub> / α) + 1 / α + 1</c>.
        /// </remarks>
        /// 
        public override double Entropy
        {
            get { return Math.Log(xm / alpha) + 1.0 / alpha + 1.0; }
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
            get { return new DoubleRange(xm, Double.PositiveInfinity); }
        }

        /// <summary>
        ///   Gets the mode for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mode value.</value>
        /// 
        /// <remarks>
        ///   The Pareto distribution's Entropy is defined as <c>x<sub>m</sub></c>.
        /// </remarks>
        /// 
        public override double Mode
        {
            get { return xm; }
        }

        /// <summary>
        ///   Gets the median for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's median value.</value>
        /// 
        /// <remarks>
        ///   The Pareto distribution's median is defined
        ///   as <c>x<sub>m</sub> * 2^(1 / α)</c>.
        /// </remarks>
        /// 
        public override double Median
        {
            get { return xm * Math.Pow(2, 1.0 / alpha); }
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
        ///   See <see cref="ParetoDistribution"/>.
        /// </example>
        /// 
        public override double DistributionFunction(double x)
        {
            if (x >= xm)
                return 1 - Math.Pow(xm / x, alpha);
            return 0;
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
        ///   See <see cref="ParetoDistribution"/>.
        /// </example>
        /// 
        public override double ProbabilityDensityFunction(double x)
        {
            if (x >= xm)
                return (alpha * Math.Pow(xm, alpha)) / Math.Pow(x, alpha + 1);
            return 0;
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
        ///   See <see cref="ParetoDistribution"/>.
        /// </example>
        /// 
        public override double LogProbabilityDensityFunction(double x)
        {
            if (x >= xm)
                return Math.Log(alpha) + alpha * Math.Log(xm) - (alpha + 1) * Math.Log(x);
            return 0;
        }


        public override void Fit(double[] observations, double[] weights, IFittingOptions options)
        {
            double xm = observations.Min();

            double lnx = Math.Log(xm);

            double den = 0;
            for (int i = 0; i < observations.Length; i++)
                den += Math.Log(observations[i]) - lnx;

            double alpha = observations.Length / den;

            init(xm, alpha);
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
            return new ParetoDistribution(xm, alpha);
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
            return String.Format("Pareto(x; xm = {0}, α = {1})", xm, alpha);
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
            return String.Format(formatProvider, "Pareto(x; xm = {0}, α = {1})", xm, alpha);
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
            return String.Format(formatProvider, "Pareto(x; xm = {0}, α = {1})",
                xm.ToString(format, formatProvider),
                alpha.ToString(format, formatProvider));
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
            return String.Format("Pareto(x; xm = {0}, α = {1})",
                xm.ToString(format), alpha.ToString(format));
        }

        public double[] Generate(int samples)
        {
            double[] U = UniformContinuousDistribution.Standard.Generate(samples);

            for (int i = 0; i < U.Length; i++)
                U[i] = xm / Math.Pow(U[i], 1.0 / alpha);

            return U;
        }

        public double Generate()
        {
            double U = UniformContinuousDistribution.Standard.Generate();

            return xm / Math.Pow(U, 1.0 / alpha);
        }
    }
}
