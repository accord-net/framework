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
    using Accord.Math.Differentiation;
    using Accord.Math.Integration;
    using Accord.Math.Optimization;
    using AForge;

    /// <summary>
    ///   General continuous distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The general continuous distribution provides the automatic calculation for 
    ///   a variety of distribution functions and measures given only definitions for
    ///   the Probability Density Function (PDF) or the Cumulative Distribution Function
    ///   (CDF). Values such as the Expected value, Variance, Entropy and others are
    ///   computed through numeric integration.</para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// // Let's suppose we have a formula that defines a probability distribution
    /// // but we dont know much else about it. We don't know the form of its cumulative
    /// // distribution function, for example. We would then like to know more about
    /// // it, such as the underlying distribution's moments, characteristics, and 
    /// // properties.
    /// 
    /// // Let's suppose the formula we have is this one:
    /// double mu = 5;
    /// double sigma = 4.2;
    /// 
    /// Func&gt;double, double> df = x => 1.0 / (sigma * Math.Sqrt(2 * Math.PI))
    ///                 * Math.Exp(-Math.Pow(x - mu, 2) / (2 * sigma * sigma));
    /// 
    /// // And for the moment, let's also pretend we don't know it is actually the
    /// // p.d.f. of a Gaussian distribution with mean 5 and std. deviation of 4.2.
    /// 
    /// // So, let's create a distribution based _solely_ on the formula we have:
    /// var distribution = GeneralContinuousDistribution.FromDensityFunction(df);
    /// 
    /// // Now, we can check everything that we can know about it:
    /// 
    /// double mean = distribution.Mean;     // 5      (note that all of those have been
    /// double median = distribution.Median; // 5       detected automatically simply from
    /// double var = distribution.Variance;  // 17.64   the given density formula through
    /// double mode = distribution.Mode;     // 5       numerical methods)
    /// 
    /// double cdf = distribution.DistributionFunction(x: 1.4);           // 0.19568296915377595
    /// double pdf = distribution.ProbabilityDensityFunction(x: 1.4);     // 0.065784567984404935
    /// double lpdf = distribution.LogProbabilityDensityFunction(x: 1.4); // -2.7213699972695058
    /// 
    /// double ccdf = distribution.ComplementaryDistributionFunction(x: 1.4); // 0.80431703084622408
    /// double icdf = distribution.InverseDistributionFunction(p: cdf);       // 1.3999999997024655
    /// 
    /// double hf = distribution.HazardFunction(x: 1.4);            // 0.081789351041333558
    /// double chf = distribution.CumulativeHazardFunction(x: 1.4); // 0.21776177055276186
    /// </code>
    /// </example>
    /// 
    public class GeneralContinuousDistribution : UnivariateContinuousDistribution
    {

        // distribution parameters
        private IUnivariateIntegration method;
        private Func<double, double> pdf;
        private Func<double, double> cdf;
        private DoubleRange support;

        private double? mean;
        private double? variance;
        private double? entropy;
        private double? mode;


        /// <summary>
        ///   Creates a new <see cref="GeneralContinuousDistribution"/> with the given PDF and CDF functions.
        /// </summary>
        /// 
        /// <param name="support">The distribution's support over the real line.</param>
        /// <param name="density">A probability density function.</param>
        /// <param name="distribution">A cumulative distribution function.</param>
        /// 
        public GeneralContinuousDistribution(DoubleRange support,
            Func<double, double> density, Func<double, double> distribution)
        {
            if (density == null)
                throw new ArgumentNullException("density");

            if (distribution == null)
                throw new ArgumentNullException("distribution");

            this.method = new InfiniteAdaptiveGaussKronrod(100);
            this.pdf = density;
            this.cdf = distribution;
            this.support = support;
        }

        /// <summary>
        ///   Creates a new <see cref="GeneralContinuousDistribution"/> with the given PDF and CDF functions.
        /// </summary>
        /// 
        /// <param name="distribution">A distribution whose properties will be numerically estimated.</param>
        /// 
        public GeneralContinuousDistribution(UnivariateContinuousDistribution distribution)
        {
            if (distribution == null)
                throw new ArgumentNullException("distribution");

            this.method = new InfiniteAdaptiveGaussKronrod(100);
            this.pdf = distribution.ProbabilityDensityFunction;
            this.cdf = distribution.DistributionFunction;
            this.support = distribution.Support;
        }

        /// <summary>
        ///   Creates a new <see cref="GeneralContinuousDistribution"/> 
        ///   from an existing <see cref="UnivariateContinuousDistribution">
        ///   continuous distribution</see>.
        /// </summary>
        /// 
        /// <param name="distribution">The distribution.</param>
        /// 
        /// <returns>A <see cref="GeneralContinuousDistribution"/> representing the same
        /// <paramref name="distribution"/> but whose measures and functions are computed
        /// using numerical integration and differentiation.</returns>
        /// 
        public static GeneralContinuousDistribution FromDistribution(UnivariateContinuousDistribution distribution)
        {
            GeneralContinuousDistribution dist = new GeneralContinuousDistribution();
            dist.support = distribution.Support;
            dist.pdf = distribution.ProbabilityDensityFunction;
            dist.cdf = distribution.DistributionFunction;
            return dist;
        }

        /// <summary>
        ///   Creates a new <see cref="GeneralContinuousDistribution"/> 
        ///   using only a probability density function definition.
        /// </summary>
        /// 
        /// <param name="pdf">A probability density function.</param>
        /// 
        /// <returns>A <see cref="GeneralContinuousDistribution"/> created from the 
        /// <paramref name="pdf"/> whose measures and functions are computed using 
        /// numerical integration and differentiation.</returns>
        /// 
        public static GeneralContinuousDistribution FromDensityFunction(Func<double, double> pdf)
        {
            var range = new DoubleRange(double.NegativeInfinity, double.PositiveInfinity);
            var method = createDefaultIntegrationMethod();
            return FromDensityFunction(range, pdf, method);
        }

        /// <summary>
        ///   Creates a new <see cref="GeneralContinuousDistribution"/> 
        ///   using only a probability density function definition.
        /// </summary>
        /// 
        /// <param name="support">The distribution's support over the real line.</param>
        /// <param name="pdf">A probability density function.</param>
        /// 
        /// <returns>A <see cref="GeneralContinuousDistribution"/> created from the 
        /// <paramref name="pdf"/> whose measures and functions are computed using 
        /// numerical integration and differentiation.</returns>
        /// 
        public static GeneralContinuousDistribution FromDensityFunction(
            DoubleRange support, Func<double, double> pdf)
        {
            var method = createDefaultIntegrationMethod();
            return FromDensityFunction(support, pdf, method);
        }

        /// <summary>
        ///   Creates a new <see cref="GeneralContinuousDistribution"/> 
        ///   using only a cumulative distribution function definition.
        /// </summary>
        /// 
        /// <param name="cdf">A cumulative distribution function.</param>
        /// 
        /// <returns>A <see cref="GeneralContinuousDistribution"/> created from the 
        /// <paramref name="cdf"/> whose measures and functions are computed using 
        /// numerical integration and differentiation.</returns>
        /// 
        public static GeneralContinuousDistribution FromDistributionFunction(Func<double, double> cdf)
        {
            var range = new DoubleRange(double.NegativeInfinity, double.PositiveInfinity);
            var method = createDefaultIntegrationMethod();
            return FromDistributionFunction(range, cdf, method);
        }

        /// <summary>
        ///   Creates a new <see cref="GeneralContinuousDistribution"/> 
        ///   using only a cumulative distribution function definition.
        /// </summary>
        /// 
        /// <param name="support">The distribution's support over the real line.</param>
        /// <param name="cdf">A cumulative distribution function.</param>
        /// 
        /// <returns>A <see cref="GeneralContinuousDistribution"/> created from the 
        /// <paramref name="cdf"/> whose measures and functions are computed using 
        /// numerical integration and differentiation.</returns>
        /// 
        public static GeneralContinuousDistribution FromDistributionFunction(
            DoubleRange support, Func<double, double> cdf)
        {
            var method = createDefaultIntegrationMethod();
            return FromDistributionFunction(support, cdf, method);
        }

        /// <summary>
        ///   Creates a new <see cref="GeneralContinuousDistribution"/> 
        ///   using only a probability density function definition.
        /// </summary>
        /// 
        /// <param name="support">The distribution's support over the real line.</param>
        /// <param name="pdf">A probability density function.</param>
        /// <param name="method">The integration method to use for numerical computations.</param>
        /// 
        /// <returns>A <see cref="GeneralContinuousDistribution"/> created from the 
        /// <paramref name="pdf"/> whose measures and functions are computed using 
        /// numerical integration and differentiation.</returns>
        /// 
        public static GeneralContinuousDistribution FromDensityFunction(
            DoubleRange support, Func<double, double> pdf, IUnivariateIntegration method)
        {
            GeneralContinuousDistribution dist = new GeneralContinuousDistribution();
            dist.support = support;
            dist.pdf = pdf;
            dist.method = method;
            return dist;
        }

        /// <summary>
        ///   Creates a new <see cref="GeneralContinuousDistribution"/> 
        ///   using only a cumulative distribution function definition.
        /// </summary>
        /// 
        /// <param name="support">The distribution's support over the real line.</param>
        /// <param name="cdf">A cumulative distribution function.</param>
        /// <param name="method">The integration method to use for numerical computations.</param>
        /// 
        /// <returns>A <see cref="GeneralContinuousDistribution"/> created from the 
        /// <paramref name="cdf"/> whose measures and functions are computed using 
        /// numerical integration and differentiation.</returns>
        /// 
        public static GeneralContinuousDistribution FromDistributionFunction(
            DoubleRange support, Func<double, double> cdf, IUnivariateIntegration method)
        {
            GeneralContinuousDistribution dist = new GeneralContinuousDistribution();
            dist.support = support;
            dist.cdf = cdf;
            dist.method = method;
            return dist;
        }

        /// <summary>
        ///   Gets the support interval for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   A <see cref="DoubleRange"/> containing
        ///   the support interval for this distribution.
        /// </value>
        /// 
        public override DoubleRange Support
        {
            get { return support; }
        }


        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mean value.</value>
        /// 
        public override double Mean
        {
            get
            {
                if (mean == null)
                {
                    method.Function = (x) =>
                    {
                        double p = ProbabilityDensityFunction(x);
                        return x * p;
                    };

                    method.Range = support;
                    method.Compute();
                    mean = method.Area;
                }

                return mean.Value;
            }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's variance.</value>
        /// 
        public override double Variance
        {
            get
            {
                if (variance == null)
                {
                    double u = Mean;
                    method.Function = (x) => (x * x) * ProbabilityDensityFunction(x);
                    method.Range = support;
                    method.Compute();
                    variance = method.Area - u * u;
                }

                return variance.Value;
            }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's entropy.</value>
        /// 
        public override double Entropy
        {
            get
            {
                if (entropy == null)
                {
                    method.Function = (x) => ProbabilityDensityFunction(x) * LogProbabilityDensityFunction(x);
                    method.Range = support;
                    method.Compute();
                    entropy = method.Area;
                }

                return entropy.Value;
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
            get
            {
                if (mode == null)
                {
                    var range = GetRange(0.99);
                    double lower = range.Min;
                    double upper = range.Max;
                    mode = BrentSearch.Maximize(ProbabilityDensityFunction, lower, upper);
                }

                return mode.Value;
            }
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
            if (cdf != null)
                return cdf(x);

            method.Function = pdf;
            method.Range = new DoubleRange(Double.NegativeInfinity, x);
            method.Compute();
            return method.Area;
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
            if (pdf != null)
                return pdf(x);

            return FiniteDifferences.Derivative(cdf, x, 1, 1e-6);
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
            GeneralContinuousDistribution c = new GeneralContinuousDistribution();

            c.pdf = pdf;
            c.cdf = cdf;
            c.method = (IUnivariateIntegration)method.Clone();

            return c;
        }

        /// <summary>
        ///   Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// 
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// 
        /// <returns>
        ///   A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// 
        public override string ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format(formatProvider, "Continuous(x)");
        }

        private GeneralContinuousDistribution()
        {
        }


        private static InfiniteAdaptiveGaussKronrod createDefaultIntegrationMethod()
        {
            var method = new InfiniteAdaptiveGaussKronrod(100);
            method.ToleranceRelative = 1e-5;
            return method;
        }
    }
}
