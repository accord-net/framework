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
    using Range = System.ComponentModel.DataAnnotations.RangeAttribute;

    /// <summary>
    ///   Shapiro-Wilk distribution.
    /// </summary>
    /// 

    [Serializable]
    public class ShapiroWilkDistribution : UnivariateContinuousDistribution, IFormattable
    {

        Func<double, double> g;
        NormalDistribution normal;


        /// <summary>
        ///   Gets the number of samples distribution parameter.
        /// </summary>
        /// 
        public int NumberOfSamples { get; private set; }

        /// <summary>
        ///   Creates a new Shapiro-Wilk distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples.</param>
        /// 
        public ShapiroWilkDistribution([Range(4, 5000)] int samples)
        {
            if (samples < 4)
            {
                throw new ArgumentOutOfRangeException("samples",
                    "The number of samples must be higher than 3.");
            }

            this.NumberOfSamples = samples;

            if (samples <= 11)
            {
                double n = samples;
                double n2 = n * n;
                double n3 = n2 * n;

                this.g = w => -Math.Log(0.459 * n - 2.273 - Math.Log(1 - w));
                double mean = -0.0006714 * n3 + 0.0250540 * n2 - 0.39978 * n + 0.54400;
                double sigma = Math.Exp(-0.0020322 * n3 + 0.0627670 * n2 - 0.77857 * n + 1.38220);

                this.normal = new NormalDistribution(mean, sigma);
            }
            else
            {
                double u = Math.Log(samples);
                double u2 = u * u;
                double u3 = u2 * u;

                this.g = w => Math.Log(1.0 - w);
                double mean = 0.00389150 * u3 - 0.083751 * u2 - 0.31082 * u - 1.5861; // 1.5861?
                double sigma = Math.Exp(0.00303020 * u2 - 0.082676 * u - 0.48030);

                this.normal = new NormalDistribution(mean, sigma);
            }
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public override DoubleRange Support
        {
            get { throw new NotSupportedException(); }
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
        public override double Mode
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
        ///   See <see cref="KolmogorovSmirnovDistribution"/>.
        /// </example>
        /// 
        public override double DistributionFunction(double x)
        {
            return normal.DistributionFunction(g(x));
        }

        /// <summary>
        ///   Gets the complementary cumulative distribution function
        ///   (ccdf) for this distribution evaluated at point <c>x</c>.
        ///   This function is also known as the Survival function.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <remarks>
        ///   The Complementary Cumulative Distribution Function (CCDF) is
        ///   the complement of the Cumulative Distribution Function, or 1
        ///   minus the CDF.
        /// </remarks>
        /// 
        public override double ComplementaryDistributionFunction(double x)
        {
            return normal.ComplementaryDistributionFunction(g(x));
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
        public override double ProbabilityDensityFunction(double x)
        {
            return normal.ProbabilityDensityFunction(g(x));
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public override double LogProbabilityDensityFunction(double x)
        {
            return normal.LogProbabilityDensityFunction(g(x));
        }


        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public override void Fit(double[] observations, double[] weights, Fitting.IFittingOptions options)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public override object Clone()
        {
            return new ShapiroWilkDistribution(NumberOfSamples);
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
            return String.Format("SW(x; n = {0})", NumberOfSamples);
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
            return String.Format(formatProvider, "SW(x; n = {0})", NumberOfSamples);
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
            return String.Format("SW(x; n = {0})",
                NumberOfSamples.ToString(format, formatProvider));
        }


    }
}