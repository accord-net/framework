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
    ///   F (Fisher-Snedecor) distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In probability theory and statistics, the F-distribution is a continuous
    ///   probability distribution. It is also known as Snedecor's F distribution
    ///   or the Fisher-Snedecor distribution (after R.A. Fisher and George W. Snedecor). 
    ///   The F-distribution arises frequently as the null distribution of a test statistic,
    ///   most notably in the analysis of variance; see <see cref="Accord.Statistics.Testing.FTest"/>.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/F-distribution">
    ///       Wikipedia, The Free Encyclopedia. F-distribution. Available on: 
    ///       http://en.wikipedia.org/wiki/F-distribution </a></description></item>
    ///   </list></para>     
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example shows how to construct a Fisher-Snedecor's F-distribution
    ///   with 8 and 5 degrees of freedom, respectively.</para>
    ///
    /// <code>
    ///   // Create a Fisher-Snedecor's F distribution with 8 and 5 d.f.
    ///   FDistribution F = new FDistribution(degrees1: 8, degrees2: 5);
    /// 
    ///   // Common measures
    ///   double mean = F.Mean;     // 1.6666666666666667
    ///   double median = F.Median; // 1.0545096252132447
    ///   double var = F.Variance;  // 7.6388888888888893
    ///   
    ///   // Cumulative distribution functions
    ///   double cdf = F.DistributionFunction(x: 0.27); // 0.049463408057268315
    ///   double ccdf = F.ComplementaryDistributionFunction(x: 0.27); // 0.95053659194273166
    ///   double icdf = F.InverseDistributionFunction(p: cdf); // 0.27
    ///   
    ///   // Probability density functions
    ///   double pdf = F.ProbabilityDensityFunction(x: 0.27); // 0.45120469723580559
    ///   double lpdf = F.LogProbabilityDensityFunction(x: 0.27); // -0.79583416831212883
    ///   
    ///   // Hazard (failure rate) functions
    ///   double hf = F.HazardFunction(x: 0.27); // 0.47468419528555084
    ///   double chf = F.CumulativeHazardFunction(x: 0.27); // 0.050728620222091653
    ///   
    ///   // String representation
    ///   string str = F.ToString(CultureInfo.InvariantCulture); // F(x; df1 = 8, df2 = 5)
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class FDistribution : UnivariateContinuousDistribution,
        ISampleableDistribution<double>
    {

        // Distribution parameters
        private int d1;
        private int d2;

        // Derived values
        private double b;

        private double? mean;
        private double? variance;


        /// <summary>
        ///   Constructs a F-distribution with
        ///   the given degrees of freedom.
        /// </summary>
        /// 
        /// <param name="degrees1">The first degree of freedom.</param>
        /// <param name="degrees2">The second degree of freedom.</param>
        /// 
        public FDistribution(int degrees1, int degrees2)
        {
            if (degrees1 <= 0) 
                throw new ArgumentOutOfRangeException("degrees1", "Degrees of freedom must be positive.");

            if (degrees2 <= 0) 
                throw new ArgumentOutOfRangeException("degrees1", "Degrees of freedom must be positive.");

            this.d1 = degrees1;
            this.d2 = degrees2;

            this.b = Beta.Function(degrees1 * 0.5, degrees2 * 0.5);
        }

        /// <summary>
        ///   Gets the first degree of freedom.
        /// </summary>
        /// 
        public int DegreesOfFreedom1
        {
            get { return d1; }
        }

        /// <summary>
        ///   Gets the second degree of freedom.
        /// </summary>
        /// 
        public int DegreesOfFreedom2
        {
            get { return d2; }
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        public override double Mean
        {
            get
            {
                if (!mean.HasValue)
                {
                    if (d2 <= 2)
                    {
                        mean = Double.NaN;
                    }
                    else
                    {
                        mean = d2 / (d2 - 2.0);
                    }
                }

                return mean.Value;
            }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        public override double Variance
        {
            get
            {
                if (!variance.HasValue)
                {
                    if (d2 <= 4)
                    {
                        variance = Double.NaN;
                    }
                    else
                    {
                        variance = (2.0 * d2 * d2 * (d1 + d2 - 2)) /
                            (d1 * (d2 - 2) * (d2 - 2) * (d2 - 4));
                    }
                }

                return variance.Value;
            }
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
        public override double Entropy
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   the F-distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <remarks>
        /// <para>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.</para>
        /// <para>
        ///   The F-distribution CDF is computed in terms of the <see cref="Beta.Incomplete">
        ///   Incomplete Beta function Ix(a,b)</see> as CDF(x) = Iu(d1/2, d2/2) in which
        ///   u is given as u = (d1 * x) / (d1 * x + d2)</para>.
        /// </remarks>
        /// 
        public override double DistributionFunction(double x)
        {
            double u = (d1 * x) / (d1 * x + d2);
            return Beta.Incomplete(d1 * 0.5, d2 * 0.5, u);
        }

        /// <summary>
        ///   Gets the complementary cumulative distribution
        ///   function evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        ///   The F-distribution complementary CDF is computed in terms of the <see cref="Beta.Incomplete">
        ///   Incomplete Beta function Ix(a,b)</see> as CDFc(x) = Iu(d2/2, d1/2) in which
        ///   u is given as u = (d2 * x) / (d2 * x + d1)</para>.
        /// </remarks>
        /// 
        public override double ComplementaryDistributionFunction(double x)
        {
            double u = d2 / (d2 + d1 * x);
            return Beta.Incomplete(d2 * 0.5, d1 * 0.5, u);
        }

        /// <summary>
        ///   Gets the inverse of the cumulative distribution function (icdf) for
        ///   this distribution evaluated at probability <c>p</c>. This function
        ///   is also known as the Quantile function.
        /// </summary>
        /// 
        /// <remarks>
        ///   The Inverse Cumulative Distribution Function (ICDF) specifies, for
        ///   a given probability, the value which the random variable will be at,
        ///   or below, with that probability.
        /// </remarks>
        /// 
        public override double InverseDistributionFunction(double p)
        {
            // Cephes Math Library Release 2.8:  June, 2000
            // Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
            // Adapted under the LGPL with permission of original author.

            if (p <= 0.0 || p > 1.0)
                throw new ArgumentOutOfRangeException("p", "Input must be between zero and one.");


            double d1 = this.d1;
            double d2 = this.d2;

            double x;

            double w = Beta.Incomplete(0.5 * d2, 0.5 * d1, 0.5);

            if (w > p || p < 0.001)
            {
                w = Beta.IncompleteInverse(0.5 * d1, 0.5 * d2, p);
                x = d2 * w / (d1 * (1.0 - w));
            }
            else
            {
                w = Beta.IncompleteInverse(0.5 * d2, 0.5 * d1, 1.0 - p);
                x = (d2 - d2 * w) / (d1 * w);
            }

            return x;
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   the F-distribution evaluated at point <c>x</c>.
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
            double u = Math.Pow(d1 * x, d1) * Math.Pow(d2, d2) /
                Math.Pow(d1 * x + d2, d1 + d2);
            return Math.Sqrt(u) / (x * b);
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
        public override double LogProbabilityDensityFunction(double x)
        {
            double lnu = d1 * Math.Log(d1 * x) + d2 * Math.Log(d2) -
                (d1 + d2) * Math.Log(d1 * x + d2);
            return 0.5 * lnu - Math.Log(x * b);
        }


        /// <summary>
        ///   Not available.
        /// </summary>
        /// 
        public override void Fit(double[] observations, double[] weights, IFittingOptions options)
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
            return new FDistribution(d1, d2);
        }

        #region ISamplableDistribution<double> Members

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// 
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public double[] Generate(int samples)
        {
            return Random(d1, d2, samples);
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public double Generate()
        {
            return Random(d1, d2);
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   F-distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="d1">The first degree of freedom.</param>
        /// <param name="d2">The second degree of freedom.</param>
        /// <param name="samples">The number of samples to generate.</param>
        ///
        /// <returns>An array of double values sampled from the specified F-distribution.</returns>
        /// 
        public static double[] Random(int d1, int d2, int samples)
        {
            double[] x = GammaDistribution.Random(shape: d1 / 2.0, scale: 2, samples: samples);
            double[] y = GammaDistribution.Random(shape: d2 / 2.0, scale: 2, samples: samples);

            for (int i = 0; i < x.Length; i++)
                x[i] = x[i] / y[i];

            return x;
        }

        /// <summary>
        ///   Generates a random observation from the 
        ///   F-distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="d1">The first degree of freedom.</param>
        /// <param name="d2">The second degree of freedom.</param>
        /// 
        /// <returns>A random double value sampled from the specified F-distribution.</returns>
        /// 
        public static double Random(int d1, int d2)
        {
            double x = GammaDistribution.Random(shape: d1 / 2.0, scale: 2);
            double y = GammaDistribution.Random(shape: d2 / 2.0, scale: 2);
            return x / y;
        }


        #endregion


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
            return String.Format("F(x; df1 = {0}, df2 = {1})", d1, d2);
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
            return String.Format(formatProvider, "F(x; df1 = {0}, df2 = {1})", d1, d2);
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
            return String.Format("F(x; df1 = {0}, df2 = {1})", 
                d1.ToString(format, formatProvider), 
                d2.ToString(format, formatProvider));
        }
    }
}
