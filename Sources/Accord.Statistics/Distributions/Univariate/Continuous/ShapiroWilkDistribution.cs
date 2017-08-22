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
    using Accord.Statistics.Testing;
    using Accord.Math;
    using Accord.Compat;

    /// <summary>
    ///   Shapiro-Wilk distribution.
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    ///   The Shapiro-Wilk distribution models the distribution of <see cref="ShapiroWilkTest">
    ///   Shapiro-Wilk's</see> <see cref="IHypothesisTest">test statistic</see>. </para>  
    ///   
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://sci2s.ugr.es/keel/pdf/algorithm/articulo/royston1982.pdf">
    ///       Royston, P. "Algorithm AS 181: The W test for Normality", Applied Statistics (1982),
    ///       Vol. 31, pp. 176–180. </a></description></item>    
    ///     <item><description><a href="http://lib.stat.cmu.edu/apstat/R94">
    ///       Royston, P. "Remark AS R94", Applied Statistics (1995), Vol. 44, No. 4, pp. 547-551.
    ///       Available at http://lib.stat.cmu.edu/apstat/R94 </a></description></item>
    ///     <item><description>
    ///       Royston, P. "Approximating the Shapiro-Wilk W-test for non-normality",
    ///       Statistics and Computing (1992), Vol. 2, pp. 117-119. </description></item>
    ///     <item><description>
    ///       Royston, P. "An Extension of Shapiro and Wilk's W Test for Normality to Large
    ///       Samples", Journal of the Royal Statistical Society Series C (1982a), Vol. 31,
    ///       No. 2, pp. 115-124. </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// // Create a new Shapiro-Wilk's W for 5 samples
    /// var sw = new ShapiroWilkDistribution(samples: 5);
    /// 
    /// double mean = sw.Mean;     // 0.81248567196628929
    /// double median = sw.Median; // 0.81248567196628929
    /// double mode = sw.Mode;     // 0.81248567196628929
    /// 
    /// double cdf = sw.DistributionFunction(x: 0.84); // 0.83507812080728383
    /// double pdf = sw.ProbabilityDensityFunction(x: 0.84); // 0.82021062372326459
    /// double lpdf = sw.LogProbabilityDensityFunction(x: 0.84); // -0.1981941135071546
    /// 
    /// double ccdf = sw.ComplementaryDistributionFunction(x: 0.84); // 0.16492187919271617
    /// double icdf = sw.InverseDistributionFunction(p: cdf); // 0.84000000194587177
    /// 
    /// double hf = sw.HazardFunction(x: 0.84); // 4.9733281462602292
    /// double chf = sw.CumulativeHazardFunction(x: 0.84); // 1.8022833766369502
    /// 
    /// string str = sw.ToString(CultureInfo.InvariantCulture); // W(x; n = 12)
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class ShapiroWilkDistribution : UnivariateContinuousDistribution, IFormattable
    {

        Func<double, double> g; // forward transformation
        Func<double, double> h; // inverse transformation
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
        public ShapiroWilkDistribution([PositiveInteger(minimum: 4)] int samples)
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

                double alpha = 0.459 * n - 2.273;
                this.g = w => -Math.Log(alpha - Special.Log1m(w));
                this.h = w => Math.Exp(-Math.Exp(-alpha)) * (Math.Exp(Math.Exp(-alpha)) - Math.Exp(w));
                double mean = -0.0006714 * n3 + 0.0250540 * n2 - 0.39978 * n + 0.54400;
                double sigma = Math.Exp(-0.0020322 * n3 + 0.0627670 * n2 - 0.77857 * n + 1.38220);

                this.normal = new NormalDistribution(mean, sigma);
            }
            else
            {
                double u = Math.Log(samples);
                double u2 = u * u;
                double u3 = u2 * u;

                this.g = w => Special.Log1m(w);
                this.h = w => 1 - Math.Exp(w);
                double mean = 0.00389150 * u3 - 0.083751 * u2 - 0.31082 * u - 1.5861;
                double sigma = Math.Exp(0.00303020 * u2 - 0.082676 * u - 0.48030);

                this.normal = new NormalDistribution(mean, sigma);
            }
        }


        /// <summary>
        ///   Gets the support interval for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   A <see cref="DoubleRange" /> containing
        ///   the support interval for this distribution.
        /// </value>
        /// 
        public override DoubleRange Support
        {
            get { return new DoubleRange(0, 1); }
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
            get { return h(normal.Mean); }
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
            get { return h(normal.Mode); }
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
            get { return h(normal.Median); }
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
        ///   See <see cref="ShapiroWilkDistribution"/>.
        /// </example>
        /// 
        protected internal override double InnerDistributionFunction(double x)
        {
            double z = g(x);
            double cdf = normal.ComplementaryDistributionFunction(z);
            return cdf;
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
        protected internal override double InnerComplementaryDistributionFunction(double x)
        {
            double z = g(x);
            return normal.DistributionFunction(z);
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
        protected internal override double InnerProbabilityDensityFunction(double x)
        {
            double z = g(x);
            return normal.ProbabilityDensityFunction(z);
        }



        /// <summary>
        /// Gets the log-probability density function (pdf) for
        /// this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        /// <returns>The logarithm of the probability of <c>x</c>
        /// occurring in the current distribution.</returns>
        /// <remarks>The Probability Density Function (PDF) describes the
        /// probability that a given value <c>x</c> will occur.</remarks>
        protected internal override double InnerLogProbabilityDensityFunction(double x)
        {
            double z = g(x);
            return normal.LogProbabilityDensityFunction(z);
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
        public override string ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format("W(x; n = {0})",
                NumberOfSamples.ToString(format, formatProvider));
        }

    }
}