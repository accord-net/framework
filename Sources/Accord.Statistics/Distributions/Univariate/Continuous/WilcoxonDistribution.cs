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
    using System.Linq;
    using Accord.Math;
    using Accord.Compat;
    using System.Threading.Tasks;

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
    ///   given a sample. The Statsstics is given as the sum of all
    ///   positive <see cref="Accord.Statistics.Tools.Rank(double[], bool, bool)">signed ranks 
    ///   </see> in a sample.</para>
    ///   
    /// <code>
    ///   // Suppose we have computed a vector of differences between
    ///   // samples and an hypothesized value (as in Wilcoxon's test).
    ///   
    ///   double[] differences = ... // differences between samples and an hypothesized median
    ///   
    ///   // Compute the ranks of the absolute differences and their sign
    ///   double[] ranks = Measures.Rank(differences.Abs());
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
        private bool exact;
        private double[] table;

        private int n;
        private NormalDistribution approximation;


        /// <summary>
        ///   Gets the number of effective samples.
        /// </summary>
        /// 
        public int NumberOfSamples { get { return n; } }

        /// <summary>
        ///   Gets whether this distribution computes the exact probabilities
        ///   (by searching all possible sign combinations) or gives fast 
        ///   approximations.
        /// </summary>
        /// 
        /// <value><c>true</c> if this distribution is exact; otherwise, <c>false</c>.</value>
        /// 
        public bool Exact { get { return exact; } }

        /// <summary>
        ///   Gets the statistic values for all possible combinations
        ///   of ranks. This is used to compute the exact distribution.
        /// </summary>
        /// 
        public double[] Table { get { return table; } }

        /// <summary>
        ///   Gets or sets the <see cref="ContinuityCorrection">continuity correction</see>
        ///   to be applied when using the Normal approximation to this distribution.
        /// </summary>
        /// 
        public ContinuityCorrection Correction { get; set; }

        /// <summary>
        ///   Creates a new Wilcoxon's W+ distribution.
        /// </summary>
        /// 
        /// <param name="n">The number of observations.</param>
        /// 
        public WilcoxonDistribution([PositiveInteger] int n)
        {
            init(n, null, null);
        }

        /// <summary>
        ///   Creates a new Wilcoxon's W+ distribution.
        /// </summary>
        /// 
        /// <param name="ranks">The rank statistics for the samples.</param>
        /// <param name="exact">True to compute the exact distribution. May require a significant 
        ///   amount of processing power for large samples (n > 12). If left at null, whether to
        ///   compute the exact or approximate distribution will depend on the number of samples.
        /// </param>
        /// 
        public WilcoxonDistribution(double[] ranks, bool? exact = null)
        {
            init(ranks.Length, ranks, exact);
        }

        private void init(int n, double[] ranks, bool? exact)
        {
            if (n <= 0)
                throw new ArgumentOutOfRangeException("n", "The number of samples must be positive.");

            this.n = n;

            double mean = n * (n + 1.0) / 4.0;
            double stdDev = Math.Sqrt((n * (n + 1.0) * (2.0 * n + 1.0)) / 24.0);

            bool hasVectors = ranks != null;

            // For small samples (< 12) the distribution can be exact
            this.exact = hasVectors && (n <= 12);

            if (exact.HasValue)
            {
                if (exact.Value && !hasVectors)
                    throw new ArgumentException("exact", "Cannot use exact method if rank vectors are not specified.");
                this.exact = exact.Value; // force
            }

            if (hasVectors)
            {
                if (this.exact)
                    initExactMethod(ranks);
            }

            this.approximation = new NormalDistribution(mean, stdDev);
        }

        private void initExactMethod(double[] ranks)
        {
            ranks = ranks.Get(ranks.Find(x => x != 0));

            long combinations = (long)Math.Pow(2, ranks.Length);

            // Compute W+ for all combinations of signs
#if NET35
            var seq = EnumerableEx.Zip<int[], long, Tuple<int[], long>>(
                Combinatorics.Sequences(ranks.Length), Vector.EnumerableRange(combinations),
                (int[] c, long i) => new Tuple<int[], long>(c, i));
#else
            var seq = Enumerable.Zip<int[], long, Tuple<int[], long>>(
                Combinatorics.Sequences(ranks.Length), Vector.EnumerableRange(combinations),
                (int[] c, long i) => new Tuple<int[], long>(c, i));
#endif

            this.table = new double[combinations];

            Parallel.ForEach(seq, item =>
            {
                int[] c = item.Item1;
                long i = item.Item2;

                // Transform binary into sign combinations
                for (int j = 0; j < c.Length; j++)
                    c[j] = Math.Sign(c[j] * 2 - 1);

                // Compute W+
                table[i] = WPositive(c, ranks);
            });

            Array.Sort(table);
        }


        /// <summary>
        ///   Computes the Wilcoxon's W+ statistic.
        /// </summary>
        /// 
        /// <remarks>
        ///   The W+ statistic is computed as the sum of all positive signed ranks.
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
        ///   The W- statistic is computed as the sum of all negative signed ranks.
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
        ///   Computes the Wilcoxon's W statistic (equivalent to 
        ///   Mann-Whitney U when used in two-sample tests).
        /// </summary>
        /// 
        /// <remarks>
        ///   The W statistic is computed as the minimum of the W+ and W- statistics.
        /// </remarks>
        /// 
        public static double WMinimum(int[] signs, double[] ranks)
        {
            double wp = 0;
            double wn = 0;
            for (int i = 0; i < signs.Length; i++)
            {
                if (signs[i] < 0)
                    wn += ranks[i];

                else if (signs[i] > 0)
                    wp += ranks[i];
            }

            return Math.Min(wp, wn);
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
            var clone = new WilcoxonDistribution(n);
            clone.exact = exact;
            clone.table = table;
            clone.n = n;
            clone.approximation = (NormalDistribution)approximation.Clone();
            return clone;
        }



        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mean value.</value>
        /// 
        public override double Mean
        {
            get { return approximation.Mean; }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's variance.</value>
        /// 
        public override double Variance
        {
            get { return approximation.Variance; }
        }

        /// <summary>
        ///   Gets the mode for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's mode value. In the current
        ///   implementation, returns the same as the <see cref="Mean"/>.
        /// </value>
        /// 
        public override double Mode
        {
            get { return approximation.Mode; }
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
            get
            {
                if (exact)
                    return new DoubleRange(0, Double.PositiveInfinity);
                return approximation.Support;
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
            get { return approximation.Entropy; }
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>k</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        /// 
        protected internal override double InnerDistributionFunction(double x)
        {
            if (this.exact)
                return exactMethod(x, table);

            if (Correction == ContinuityCorrection.Midpoint)
            {
                if (x > Mean)
                {
                    x = x - 0.5;
                }
                else
                {
                    x = x + 0.5;
                }
            }
            else if (Correction == ContinuityCorrection.KeepInside)
            {
                x = x + 0.5;
            }

            return approximation.DistributionFunction(x);
        }

        /// <summary>
        /// Gets the complementary cumulative distribution function
        /// (ccdf) for this distribution evaluated at point <c>x</c>.
        /// This function is also known as the Survival function.
        /// </summary>
        /// <param name="x">A single point in the distribution range.</param>
        /// <returns>System.Double.</returns>
        /// <remarks>The Complementary Cumulative Distribution Function (CCDF) is
        /// the complement of the Cumulative Distribution Function, or 1
        /// minus the CDF.</remarks>
        protected internal override double InnerComplementaryDistributionFunction(double x)
        {
            if (this.exact)
                return exactComplement(x, table);

            if (Correction == ContinuityCorrection.Midpoint)
            {
                if (x > Mean)
                {
                    x = x - 0.5;
                }
                else
                {
                    x = x + 0.5;
                }
            }
            else if (Correction == ContinuityCorrection.KeepInside)
            {
                x = x - 0.5;
            }

            return approximation.ComplementaryDistributionFunction(x);
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
        ///   value when applied in the <see cref="UnivariateContinuousDistribution.DistributionFunction(double)"/>.
        /// </returns>
        /// 
        protected internal override double InnerInverseDistributionFunction(double p)
        {
            if (this.exact)
                return base.InnerInverseDistributionFunction(p);

            return approximation.InverseDistributionFunction(p);
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>w</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
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
        protected internal override double InnerProbabilityDensityFunction(double x)
        {
            if (this.exact)
                return count(x, table) / (double)table.Length;

            return approximation.ProbabilityDensityFunction(x);
        }

        /// <summary>
        ///   Gets the log-probability density function (pdf) for
        ///   this distribution evaluated at point <c>w</c>.
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
        protected internal override double InnerLogProbabilityDensityFunction(double x)
        {
            if (this.exact)
                return Math.Log(count(x, table)) - Math.Log(table.Length);

            return approximation.LogProbabilityDensityFunction(x);
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
            return String.Format(formatProvider, "W+(x; R)");
        }






        // TODO: This is a general method. It should be moved to
        // a more appropriate place and be changed to public
        internal static double exactMethod(double x, double[] table)
        {
#if DEBUG
            int count = 0;
            for (int i = 0; i < table.Length; i++)
                if (table[i] <= x)
                    count++;
            double p = count / (double)table.Length;
#endif

            for (int i = 0; i < table.Length; i++)
            {
                if (x < table[i])
                {
#if DEBUG
                    if (i != count)
                        throw new Exception();
#endif
                    return i / (double)table.Length;
                }
            }

            return 1;
        }

        // TODO: This is a general method. It should be moved to
        // a more appropriate place and be changed to public
        internal static double exactComplement(double x, double[] table)
        {
#if DEBUG
            int count = 0;
            for (int i = 0; i < table.Length; i++)
                if (table[i] >= x)
                    count++;
            double p = count / (double)table.Length;
#endif

            for (int i = table.Length - 1; i >= 0; i--)
            {
                if (table[i] < x)
                {
                    int j = table.Length - i - 1;
#if DEBUG
                    if (j != count)
                        throw new Exception();
#endif
                    return j / (double)table.Length;
                }
            }

            return 1;
        }

        internal static int count(double x, double[] table)
        {
            // For all possible values for W, find how many
            // of them are equal to the requested value.

            int count = 0;
            for (int i = 0; i < table.Length; i++)
                if (table[i] == x)
                    count++;
            return count;
        }
    }
}
