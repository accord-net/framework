﻿// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Fredrik Enqvist, 2016
// fredrikenqvist at hotmail.com
//
// Copyright © César Souza, 2009-2016
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
    using Accord.Math;
    using Accord.Math.Optimization;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using AForge;
    using System;
    using System.ComponentModel;

    /// <summary>
    ///   Generalized Pareto distribution (three parameters).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In statistics, the generalized Pareto distribution (GPD) is a family of 
    ///   continuous probability distributions. It is often used to model the tails
    ///   of another distribution. It is specified by three parameters: location μ,
    ///   scale σ, and shape ξ. Sometimes it is specified by only scale and shape
    ///   and sometimes only by its shape parameter. Some references give the shape 
    ///   parameter as κ = − ξ.
    /// </para>
    /// 
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://en.wikipedia.org/wiki/Generalized_Pareto_distribution">
    ///       Wikipedia, The Free Encyclopedia. Generalized Pareto distribution. 
    ///       Available from: https://en.wikipedia.org/wiki/Generalized_Pareto_distribution </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code source="Unit Tests\Accord.Tests.Statistics\Distributions\Univariate\Continuous\GeneralizedParetoDistributionTest.cs" region="doc_example1" />
    /// </example>
    /// 
    /// <seealso cref="Accord.Statistics.Distributions.Univariate.ParetoDistribution" />
    /// <seealso cref="Accord.Statistics.Distributions.Univariate.UnivariateContinuousDistribution" />
    /// 
    [Serializable]
    public class GeneralizedParetoDistribution : UnivariateContinuousDistribution, IFormattable
    {

        double mu;
        double sigma;
        double ksi;


        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralizedParetoDistribution"/> class.
        /// </summary>
        /// 
        public GeneralizedParetoDistribution()
            : this(1, 1, 1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralizedParetoDistribution"/> class.
        /// </summary>
        /// 
        /// <param name="location">The location parameter μ (mu).</param>
        /// <param name="scale">The scale parameter σ (sigma). Must be > 0.</param>
        /// <param name="shape">The shape parameter ξ (Xi).</param>
        /// 
        public GeneralizedParetoDistribution([Real] double location, [Positive] double scale, [Real] double shape)
        {
            if (scale <= 0)
                throw new ArgumentOutOfRangeException("scale", "Scale must be positive.");

            init(location, scale, shape);
        }


        private void init(double location, double scale, double shape)
        {
            this.mu = location;
            this.sigma = scale;
            this.ksi = shape;
        }

        /// <summary>
        ///   Gets the scale parameter σ (sigma).
        /// </summary>
        /// 
        public double Scale
        {
            get { return sigma; }
        }

        /// <summary>
        ///   Gets shape parameter ξ (Xi).
        /// </summary>
        /// 
        public double Shape
        {
            get { return ksi; }
        }

        /// <summary>
        ///   Gets the location parameter μ (mu).
        /// </summary>
        /// 
        public double Location
        {
            get { return mu; }
        }

        /// <summary>
        /// Gets the variance for this distribution.
        /// </summary>
        /// <value>The distribution's variance.</value>
        public override double Variance
        {
            get { return (sigma * sigma) / ((1 - ksi) * (1 - ksi) * (1 - 2 * ksi)); }
        }


        /// <summary>
        /// Gets the entropy for this distribution.
        /// </summary>
        /// <value>The distribution's entropy.</value>
        public override double Entropy
        {
            get { return Double.NaN; }
        }


        /// <summary>
        /// Gets the support interval for this distribution.
        /// </summary>
        /// <value>A <see cref="DoubleRange" /> containing
        /// the support interval for this distribution.</value>
        public override DoubleRange Support
        {
            get
            {
                if (ksi >= 0)
                    return new DoubleRange(mu, Double.PositiveInfinity);
                return new DoubleRange(mu, mu - sigma / ksi);
            }
        }


        /// <summary>
        /// Gets the mean for this distribution.
        /// </summary>
        /// <value>The distribution's mean value.</value>
        public override double Mean
        {
            get { return mu + (sigma / (1 - ksi)); }
        }


        /// <summary>
        /// Gets the median for this distribution.
        /// </summary>
        /// <value>The distribution's median value.</value>
        public override double Median
        {
            get { return mu + (sigma * (Math.Pow(2, ksi) - 1) / ksi); }
        }


        /// <summary>
        /// Gets the cumulative distribution function (cdf) for
        /// this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        /// <returns>System.Double.</returns>
        /// <remarks>The Cumulative Distribution Function (CDF) describes the cumulative
        /// probability that a given value or any value smaller than it will occur.</remarks>
        public override double DistributionFunction(double x)
        {
            // PDF components
            double m = (x - mu) / sigma;
            double k = 1 + (ksi * m);
            double l = -1 / ksi;

            // domain logic
            bool shapePos = ksi > 0;
            bool shapeNeg = ksi < 0;
            bool shapeZero = ksi == 0; // special case 

            bool xA = x >= mu;
            bool xB = x <= (mu - (sigma / ksi));

            bool A = shapePos && xA;
            bool B = shapeNeg && xA && xB;
            bool C = shapeZero && xA; // special case

            // CDF function
            if (A || B)
                return 1 - Math.Pow(k, l);
            if (C)
                return 1 - Math.Exp(-1 * m);

            return 0;
        }


        /// <summary>
        /// Gets the probability density function (pdf) for
        /// this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        /// <returns>The probability of <c>x</c> occurring
        /// in the current distribution.</returns>
        /// <remarks>The Probability Density Function (PDF) describes the
        /// probability that a given value <c>x</c> will occur.</remarks>
        public override double ProbabilityDensityFunction(double x)
        {
            // PDF components
            double m = (x - mu) / sigma;
            double k = 1 + (ksi * m);
            double l = -1 * ((1 / ksi) + 1);

            // domain logic
            bool shapePos = ksi > 0;
            bool shapeNeg = ksi < 0;
            bool shapeZero = ksi == 0; // special case 

            bool xA = x >= mu;
            bool xB = x <= (mu - (sigma / ksi));

            bool A = shapePos && xA;
            bool B = shapeNeg && xA && xB;
            bool C = shapeZero && xA; // special case

            // PDF function
            if (A || B)
                return Math.Pow(k, l) / sigma;
            if (C)
                return Math.Exp(-1 * m) / sigma;

            return 0;
        }





        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            return new GeneralizedParetoDistribution(mu, sigma, ksi);
        }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format(formatProvider, "Pareto(x; μ = {0}, σ = {1}, ξ = {2})",
                mu.ToString(format, formatProvider),
                sigma.ToString(format, formatProvider),
                ksi.ToString(format, formatProvider));
        }

        /// <summary>
        /// Generates a random vector of observations from the current distribution.
        /// </summary>
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="result">The location where to store the samples.</param>
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        public override double[] Generate(int samples, double[] result)
        {
            UniformContinuousDistribution.Standard.Generate(samples, result);

            if (ksi == 0)
            {
                for (int i = 0; i < result.Length; i++)
                    result[i] = mu - sigma * Math.Log(result[i]);
            }
            else
            {
                for (int i = 0; i < result.Length; i++)
                    result[i] = mu + sigma * (Math.Pow(result[i], -ksi) - 1) / ksi;
            }

            return result;
        }

        /// <summary>
        /// Generates a random observation from the current distribution.
        /// </summary>
        /// <returns>A random observations drawn from this distribution.</returns>
        public override double Generate()
        {
            double U = UniformContinuousDistribution.Standard.Generate();

            if (ksi == 0)
                return mu - sigma * Math.Log(U);
            return mu + sigma * (Math.Pow(U, -ksi) - 1) / ksi;
        }
    }
}