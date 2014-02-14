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
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Math;
    using AForge;

    /// <summary>
    ///   Wilcoxon's W statistic distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This is the distribution for the positive side statistic W+ of the Wilcoxon
    ///   test. Some textbooks (and statistical packages) use alternate definitions for
    ///   W, which should be compared with the appropriate statistic tables or alternate
    ///   distributions.</para>
    /// <para>
    ///   The Wilcoxon signed-rank test is a non-parametric statistical hypothesis test
    ///   used when comparing two related samples, matched samples, or repeated measurements
    ///   on a single sample to assess whether their population mean ranks differ (i.e. it
    ///   is a paired difference test). It can be used as an alternative to the paired
    ///   Student's t-test, t-test for matched pairs, or the t-test for dependent samples
    ///   when the population cannot be assumed to be normally distributed.</para>
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Wilcoxon_signed-rank_test">
    ///       Wikipedia, The Free Encyclopedia. Wilcoxon signed-rank test. Available on:
    ///       http://en.wikipedia.org/wiki/Wilcoxon_signed-rank_test </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///   // Compute some rank statistics (see other examples below)
    ///   double[] ranks = { 1, 2, 3, 4, 5.5, 5.5, 7, 8, 9, 10, 11, 12 };
    ///   
    ///   // Create a new Wilcoxon's W distribution
    ///   WilcoxonDistribution W = new WilcoxonDistribution(ranks);
    ///   
    ///   // Common measures
    ///   double mean = W.Mean;     // 39.0
    ///   double median = W.Median; // 38.5
    ///   double var = W.Variance;  // 162.5
    ///   
    ///   // Probability density functions
    ///   double pdf = W.ProbabilityDensityFunction(w: 42);     // 0.38418508862319295
    ///   double lpdf = W.LogProbabilityDensityFunction(w: 42); // 0.38418508862319295
    ///   
    ///   // Cumulative distribution functions
    ///   double cdf = W.DistributionFunction(w: 42);               // 0.60817384423279575
    ///   double ccdf = W.ComplementaryDistributionFunction(x: 42); // 0.39182615576720425
    ///   
    ///   // Quantile function
    ///   double icdf = W.InverseDistributionFunction(p: cdf); // 42
    ///   
    ///   // Hazard (failure rate) functions
    ///   double hf = W.HazardFunction(x: 42);            // 0.98049883339449373
    ///   double chf = W.CumulativeHazardFunction(x: 42); // 0.936937017743799
    ///   
    ///   // String representation
    ///   string str = W.ToString(); // "W+(x; R)"
    /// </code>
    /// 
    /// <para>
    ///   The following example shows how to compute the W+ statistic
    ///   given a sample. The W+ statistics is given as the sum of all
    ///   positive <see cref="Accord.Statistics.Tools.Rank">signed ranks 
    ///   </see> in a sample.</para>
    ///   
    /// <code>
    ///   // Suppose we have computed a vector of differences between
    ///   // samples and an hypothesized value (as in Wilcoxon's test).
    ///   
    ///   double[] differences = ... // differences between samples and an hypothesized median
    ///   
    ///   // Compute the ranks of the absolute differences and their sign
    ///   double[] ranks = Accord.Statistics.Tools.Rank(differences.Abs());
    ///   int[] signs    = Accord.Math.Matrix.Sign(differences).ToInt32();
    ///   
    ///   // Compute the W+ statistics from the signed ranks
    ///   double W = WilcoxonDistribution.WPositive(Signs, ranks);
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="Accord.Statistics.Testing.WilcoxonSignedRankTest"/>
    /// <seealso cref="Accord.Statistics.Testing.TwoSampleWilcoxonSignedRankTest"/>
    /// 
    [Serializable]
    public class WilcoxonDistribution : UnivariateContinuousDistribution
    {
        private double[] table;


        /// <summary>
        ///   Gets the number of effective samples.
        /// </summary>
        /// 
        public int Samples { get; private set; }

        /// <summary>
        ///   Gets the rank statistics for the distribution.
        /// </summary>
        /// 
        public double[] Ranks { get; private set; }


        /// <summary>
        ///   Creates a new Wilcoxon's W+ distribution.
        /// </summary>
        /// 
        /// <param name="ranks">The rank statistics for the samples.</param>
        /// 
        public WilcoxonDistribution(double[] ranks)
            : this(ranks, false)
        {
        }

        /// <summary>
        ///   Creates a new Wilcoxon's W+ distribution.
        /// </summary>
        /// 
        /// <param name="ranks">The rank statistics for the samples.</param>
        /// <param name="forceExact">True to compute the exact test. May requires
        ///   a significant amount of processing power for large samples (n > 12).</param>
        /// 
        public WilcoxonDistribution(double[] ranks, bool forceExact)
        {
            // Remove zero elements
            int[] idx = ranks.Find(x => x != 0);
            this.Ranks = ranks.Submatrix(idx);
            this.Samples = idx.Length;

            if (forceExact || Samples < 12)
            {
                // For a small sample (i.e. n < 12)
                // the distribution will be exact.

                // Generate all possible combinations in advance
                int[][] combinations = Combinatorics.TruthTable(Samples);

                // Transform binary into sign combinations
                for (int i = 0; i < combinations.Length; i++)
                    for (int j = 0; j < combinations[i].Length; j++)
                        combinations[i][j] = Math.Sign(combinations[i][j] * 2 - 1);

                // Compute all possible values for W+ considering those signs
                this.table = new double[combinations.Length];
                for (int i = 0; i < combinations.Length; i++)
                    table[i] = WPositive(combinations[i], ranks);

                Array.Sort(table);
            }
        }


        /// <summary>
        ///   Computes the Wilcoxon's W+ statistic.
        /// </summary>
        /// 
        /// <remarks>
        ///   The W+ statistic is computed as the
        ///   sum of all positive signed ranks.
        /// </remarks>
        /// 
        public static double WPositive(int[] signs, double[] ranks)
        {
            double sum = 0;
            for (int i = 0; i < signs.Length; i++)
                if (signs[i] > 0) 
                    sum += ranks[i];

            return sum;
        }

        /// <summary>
        ///   Computes the Wilcoxon's W- statistic.
        /// </summary>
        /// 
        /// <remarks>
        ///   The W- statistic is computed as the
        ///   sum of all negative signed ranks.
        /// </remarks>
        /// 
        public static double WNegative(int[] signs, double[] ranks)
        {
            double sum = 0;
            for (int i = 0; i < signs.Length; i++)
                if (signs[i] < 0)
                    sum += ranks[i];
            return sum;
        }

        /// <summary>
        ///   Computes the Wilcoxon's W statistic.
        /// </summary>
        /// 
        /// <remarks>
        ///   The W statistic is computed as the
        ///   minimum of the W+ and W- statistics.
        /// </remarks>
        /// 
        public static double WMinimum(int[] signs, double[] ranks)
        {
            double wp = 0, wn = 0;
            for (int i = 0; i < signs.Length; i++)
            {
                if (signs[i] < 0)
                    wn += ranks[i];

                else if (signs[i] > 0)
                    wp += ranks[i];
            }

            double min = Math.Min(wp, wn);

            return min;
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
            return new WilcoxonDistribution(Ranks);
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
                double n = Samples;
                return n * (n + 1.0) / 4.0;
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
                double n = Samples;
                return (n * (n + 1.0) * (2.0 * n + 1.0)) / 24.0;
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
            get { return new DoubleRange(Double.NegativeInfinity, Double.PositiveInfinity); }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's entropy.</value>
        /// 
        public override double Entropy
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>k</c>.
        /// </summary>
        /// 
        /// <param name="w">A single point in the distribution range.</param>
        /// 
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        /// 
        public override double DistributionFunction(double w)
        {
            if (table != null) // Samples < 12
            {
                for (int i = 0; i < table.Length; i++)
                    if (w <= table[i]) 
                        return i / (double)table.Length;

                return 1;
            }

            else
            {
                // Large sample Normal distribution approximation
                double wc = w + 0.5; // continuity correction
                double z = (wc - Mean) / StandardDeviation;
                double p = NormalDistribution.Standard.DistributionFunction(z);
                return p;
            }
        }

        /// <summary>
        ///   Gets the inverse of the cumulative distribution function (icdf) for
        ///   this distribution evaluated at probability <c>p</c>. This function
        ///   is also known as the Quantile function.
        /// </summary>
        /// 
        /// <param name="p">A probability value between 0 and 1.</param>
        /// 
        /// <returns>
        ///   A sample which could original the given probability
        ///   value when applied in the <see cref="DistributionFunction"/>.
        /// </returns>
        /// 
        public override double InverseDistributionFunction(double p)
        {
            if (table != null)
            {
                double icdf = base.InverseDistributionFunction(p);
                return icdf;
            }
            else
            {
                double z = NormalDistribution.Standard.InverseDistributionFunction(p);
                double wc = z * StandardDeviation + Mean;
                double w = wc - 0.5; // reverse continuity correction
                return w;
            }
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>w</c>.
        /// </summary>
        /// 
        /// <param name="w">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The probability of <c>w</c> occurring
        ///   in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        public override double ProbabilityDensityFunction(double w)
        {
            if (table != null) // Samples < 12
            {
                // For all possible values for W, find how many
                // of them are equal to the requested value.

                int count = 0;
                for (int i = 0; i < table.Length; i++)
                    if (table[i] == w) count++;
                return count / (double)table.Length;
            }

            else
            {
                // Large sample Normal distribution approximation
                double wc = w + 0.5; // continuity correction
                double z = (wc - Mean) / StandardDeviation;
                return NormalDistribution.Standard.ProbabilityDensityFunction(z);
            }
        }

        /// <summary>
        ///   Gets the log-probability density function (pdf) for
        ///   this distribution evaluated at point <c>w</c>.
        /// </summary>
        /// 
        /// <param name="w">A single point in the distribution range.</param>
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
        public override double LogProbabilityDensityFunction(double w)
        {
            if (table != null) // Samples < 12
            {
                // For all possible values for W, find how many
                // of them are equal to the requested value.

                int count = 0;
                for (int i = 0; i < table.Length; i++)
                    if (table[i] == w) count++;
                return Math.Log(count) - Math.Log(table.Length);
            }

            else
            {
                // Large sample Normal distribution approximation
                double wc = w + 0.5; // continuity correction
                double z = (wc - Mean) / StandardDeviation;
                return NormalDistribution.Standard.LogProbabilityDensityFunction(z);
            }
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
            return String.Format("W+(x; R)");
        }
    }
}
