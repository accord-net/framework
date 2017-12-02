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
    using Accord.Math;
    using Accord.Statistics.Testing;
    using Accord.Statistics;
    using System.Linq;
    using System.Diagnostics;
    using Accord.Compat;
    using System.Threading.Tasks;
    using Accord.Math.Optimization;

    /// <summary>
    ///   Continuity correction to be used when aproximating
    ///   discrete values through a continuous distribution.
    /// </summary>
    /// 
    public enum ContinuityCorrection
    {
        /// <summary>
        ///   No correction for continuity should be applied.
        /// </summary>
        /// 
        None,

        /// <summary>
        ///   The correction for continuity is -0.5 when the statistic is 
        ///   greater than the mean and +0.5 when it is less than the mean.
        /// </summary>
        /// 
        /// <remarks>
        ///   This correction is used/described in http://vassarstats.net/textbook/ch12a.html.
        /// </remarks>
        /// 
        Midpoint,

        /// <summary>
        ///   The correction for continuity will be -0.5 when computing values at the 
        ///   <see cref="DistributionTail.OneUpper">right (upper) tail</see> of the 
        ///   distribution, and +0.5 when computing at the <see cref="DistributionTail.OneLower">
        ///   left (lower) tail</see>.
        /// </summary>
        /// 
        KeepInside
    }

    /// <summary>
    ///   Mann-Whitney's U statistic distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This is the distribution for <see cref="MannWhitneyWilcoxonTest">Mann-Whitney's U</see>
    ///   statistic used in <see cref="MannWhitneyWilcoxonTest"/>. This distribution is based on
    ///   sample <see cref="Accord.Statistics.Tools.Rank(double[], bool, bool)"/> statistics.</para>
    /// <para>
    ///   This is the distribution for the first sample statistic, U1. Some textbooks
    ///   (and statistical packages) use alternate definitions for U, which should be
    ///   compared with the appropriate statistic tables or alternate distributions.</para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///   // Consider the following rank statistics
    ///   double[] ranks = { 1, 2, 3, 4, 5 };
    ///   
    ///   // Create a new Mann-Whitney U's distribution with n1 = 2 and n2 = 3
    ///   var mannWhitney = new MannWhitneyDistribution(ranks, n1: 2, n2: 3);
    ///   
    ///   // Common measures
    ///   double mean = mannWhitney.Mean;     // 2.7870954605658511
    ///   double median = mannWhitney.Median; // 1.5219615583481305
    ///   double var = mannWhitney.Variance;  // 18.28163603621158
    ///   
    ///   // Cumulative distribution functions
    ///   double cdf = mannWhitney.DistributionFunction(x: 4);               // 0.6
    ///   double ccdf = mannWhitney.ComplementaryDistributionFunction(x: 4); // 0.4
    ///   double icdf = mannWhitney.InverseDistributionFunction(p: cdf);     // 3.6666666666666661
    ///   
    ///   // Probability density functions
    ///   double pdf = mannWhitney.ProbabilityDensityFunction(x: 4);     // 0.2
    ///   double lpdf = mannWhitney.LogProbabilityDensityFunction(x: 4); // -1.6094379124341005
    ///   
    ///   // Hazard (failure rate) functions
    ///   double hf = mannWhitney.HazardFunction(x: 4); // 0.5
    ///   double chf = mannWhitney.CumulativeHazardFunction(x: 4); // 0.916290731874155
    ///     
    ///   // String representation
    ///   string str = mannWhitney.ToString(); // MannWhitney(u; n1 = 2, n2 = 3)
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="MannWhitneyWilcoxonTest"/>
    /// <seealso cref="Accord.Statistics.Tools.Rank(Double[], bool, bool)"/>
    /// <seealso cref="WilcoxonDistribution"/>
    /// 
    [Serializable]
    public class MannWhitneyDistribution : UnivariateContinuousDistribution
    {
        private bool exact;
        private double[] table;

        private int n1;
        private int n2;

        private NormalDistribution approximation;


        /// <summary>
        ///   Gets the number of observations in the first sample. 
        /// </summary>
        /// 
        public int NumberOfSamples1 { get { return n1; } }

        /// <summary>
        ///   Gets the number of observations in the second sample.
        /// </summary>
        /// 
        public int NumberOfSamples2 { get { return n2; } }

        /// <summary>
        ///   Gets or sets the <see cref="ContinuityCorrection">continuity correction</see>
        ///   to be applied when using the Normal approximation to this distribution.
        /// </summary>
        /// 
        public ContinuityCorrection Correction { get; set; }

        /// <summary>
        ///   Gets whether this distribution computes the exact probabilities
        ///   (by searching all possible rank combinations) or gives fast 
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
        ///   Constructs a Mann-Whitney's U-statistic distribution.
        /// </summary>
        /// 
        /// <param name="n1">The number of observations in the first sample.</param>
        /// <param name="n2">The number of observations in the second sample.</param>
        /// 
        public MannWhitneyDistribution([PositiveInteger] int n1, [PositiveInteger] int n2)
        {
            init(n1, n2, null, null);
        }

        /// <summary>
        ///   Constructs a Mann-Whitney's U-statistic distribution.
        /// </summary>
        /// 
        /// <param name="ranks">The rank statistics.</param>
        /// <param name="n1">The number of observations in the first sample.</param>
        /// <param name="n2">The number of observations in the second sample.</param>
        /// <param name="exact">True to compute the exact distribution. May require a significant 
        ///   amount of processing power for large samples (n > 30). If left at null, whether to
        ///   compute the exact or approximate distribution will depend on the number of samples.
        /// </param>
        /// 
        public MannWhitneyDistribution(double[] ranks, [PositiveInteger]int n1, [PositiveInteger]int n2, bool? exact = null)
        {
            init(n1, n2, ranks, exact);
        }

        /// <summary>
        ///   Constructs a Mann-Whitney's U-statistic distribution.
        /// </summary>
        /// 
        /// <param name="ranks1">The global rank statistics for the first sample.</param>
        /// <param name="ranks2">The global rank statistics for the second sample.</param>
        /// <param name="exact">True to compute the exact distribution. May require a significant 
        ///   amount of processing power for large samples (n > 30). If left at null, whether to
        ///   compute the exact or approximate distribution will depend on the number of samples.
        /// </param>
        /// 
        public MannWhitneyDistribution(double[] ranks1, double[] ranks2, bool? exact = null)
        {
            double[] ranks = ranks1.Concatenate(ranks2);
            init(ranks1.Length, ranks2.Length, ranks, exact);
        }


        private void init(int n1, int n2, double[] ranks, bool? exact)
        {
            if (n1 <= 0)
                throw new ArgumentOutOfRangeException("n1", "The number of observations in the first sample (n1) must be higher than zero.");

            if (n2 <= 0)
                throw new ArgumentOutOfRangeException("n2", "The number of observations in the second sample (n2) must be higher than zero.");

            if (n1 > n2)
                Trace.TraceWarning("Creating a MannWhitneyDistribution where the first sample is larger than the second sample. If possible, please re-organize your samples such that the first sample is smaller than the second sample.");

            if (ranks != null)
            {
                if (ranks.Length <= 1)
                    throw new ArgumentOutOfRangeException("ranks", "The rank vector must contain a minimum of 2 elements.");

                for (int i = 0; i < ranks.Length; i++)
                    if (ranks[i] < 0)
                        throw new ArgumentOutOfRangeException("The rank values cannot be negative.");
            }

            int n = n1 + n2;
            this.n1 = n1;
            this.n2 = n2;

            // From https://en.wikipedia.org/wiki/Mann%E2%80%93Whitney_U_test: For large samples, U 
            // is approximately normally distributed. In that case, the standardized value is given 
            // by z = (U - mean) / stdDev where: 
            double mean = (n1 * n2) / 2.0;
            double stdDev = Math.Sqrt((n1 * n2 * (n + 1)) / 12.0);

            bool hasVectors = ranks != null;

            // For small samples (< 30) the distribution can be exact
            this.exact = hasVectors && (n1 <= 30 && n2 <= 30);

            if (exact.HasValue)
            {
                if (exact.Value && !hasVectors)
                    throw new ArgumentException("exact", "Cannot use exact method if rank vectors are not specified.");
                this.exact = exact.Value; // force
            }

            if (hasVectors)
            {
                // Apply correction to the variance
                double correction = MannWhitneyDistribution.correction(ranks);
                double a = (n1 * n2) / 12.0;
                double b = (n + 1.0) - correction;
                stdDev = Math.Sqrt(a * b);

                if (this.exact)
                    initExactMethod(ranks);
            }

            this.approximation = new NormalDistribution(mean, stdDev);
        }

        private static double correction(double[] ranks)
        {
            // Computes the tie correction term for the variance as described in
            // https://en.wikipedia.org/wiki/Mann%E2%80%93Whitney_U_test#Normal_approximation_and_tie_correction

            int n = ranks.Length;

            if (n <= 1)
                throw new ArgumentOutOfRangeException("ranks");

            // Compute number of ties
            int[] ties = ranks.Ties();

            double sum = 0;
            for (int i = 0; i < ties.Length; i++)
            {
                double t3 = ties[i] * ties[i] * ties[i];
                sum += t3 - ties[i];
            }

            return sum / (n * (n - 1));
        }

        private void initExactMethod(double[] ranks)
        {
            int min = Math.Min(n1, n2);
            long combinations = (long)Special.Binomial(n1 + n2, min);

            this.table = new double[combinations];

#if NET35
            var seq = EnumerableEx.Zip<double[], long, Tuple<double[], long>>(
                Combinatorics.Combinations(ranks, min), Vector.Range(combinations),
                (double[] c, long i) => new Tuple<double[], long>(c, i));
#else
            var seq = Enumerable.Zip<double[], long, Tuple<double[], long>>(
                Combinatorics.Combinations(ranks, min), Vector.Range(combinations),
                (double[] c, long i) => new Tuple<double[], long>(c, i));
#endif

            Parallel.ForEach(seq, i =>
            {
                this.table[i.Item2] = MannWhitneyU(i.Item1);
            });

            Array.Sort(table);
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
            if (NumberOfSamples1 <= NumberOfSamples2)
                return distributionFunction(x);

            Trace.TraceWarning("Warning: Using a MannWhitneyDistribution where the first sample is larger than the second.");
            return complementaryDistributionFunction(x);
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
            if (NumberOfSamples1 <= NumberOfSamples2)
                return complementaryDistributionFunction(x);

            Trace.TraceWarning("Warning: Using a MannWhitneyDistribution where the first sample is larger than the second.");
            return distributionFunction(x);
        }



        private double distributionFunction(double x)
        {
            if (exact)
            {
                // For small samples (< 30) and if there are not very large
                // differences in samples sizes, this distribution is exact.
                return WilcoxonDistribution.exactMethod(x, table);
            }

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



        private double complementaryDistributionFunction(double x)
        {
            if (exact)
            {
                // For small samples (< 30) and if there are not very large
                // differences in samples sizes, this distribution is exact.
                return WilcoxonDistribution.exactComplement(x, table);
            }

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
        ///   Gets the Mann-Whitney's U statistic for the first sample.
        /// </summary>
        /// 
        public static double MannWhitneyU(double[] ranks)
        {
            int n = ranks.Length;
            double rankSum = ranks.Sum();
            double u = rankSum - (n * (n + 1.0)) / 2.0;
            return u;
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
            var clone = new MannWhitneyDistribution(NumberOfSamples1, NumberOfSamples2);
            clone.exact = exact;
            clone.table = table;
            clone.n1 = n1;
            clone.n2 = n2;
            clone.Correction = Correction;
            clone.approximation = (NormalDistribution)approximation.Clone();
            return clone;
        }


        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mean value.</value>
        /// 
        /// <remarks>
        ///   The mean of Mann-Whitney's U distribution
        ///   is defined as <c>(n1 * n2) / 2</c>.
        /// </remarks>
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
        /// <remarks>
        ///   The variance of Mann-Whitney's U distribution
        ///   is defined as <c>(n1 * n2 * (n1 + n2 + 1)) / 12</c>.
        /// </remarks>
        /// 
        public override double Variance
        {
            get { return approximation.Variance; }
        }

        /// <summary>
        ///   This method is not supported.
        /// </summary>
        /// 
        public override double Mode
        {
            get { return approximation.Mode; }
        }

        /// <summary>
        ///   This method is not supported.
        /// </summary>
        /// 
        public override double Entropy
        {
            get { return approximation.Entropy; }
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
                return new DoubleRange(Double.NegativeInfinity, Double.PositiveInfinity);
            }
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>u</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The probability of <c>u</c> occurring
        ///   in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>u</c> will occur.
        /// </remarks>
        /// 
        /// <example>
        ///   See <see cref="MannWhitneyDistribution"/>.
        /// </example>
        /// 
        protected internal override double InnerProbabilityDensityFunction(double x)
        {
            if (this.exact)
                return WilcoxonDistribution.count(x, table) / (double)table.Length;

            return approximation.ProbabilityDensityFunction(x);
        }

        /// <summary>
        ///   Gets the log-probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The logarithm of the probability of <c>u</c>
        ///   occurring in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>u</c> will occur.
        /// </remarks>
        /// 
        /// <example>
        ///   See <see cref="MannWhitneyDistribution"/>.
        /// </example>
        /// 
        protected internal override double InnerLogProbabilityDensityFunction(double x)
        {
            if (exact)
                return Math.Log(WilcoxonDistribution.count(x, table)) - Math.Log(table.Length);

            return approximation.ProbabilityDensityFunction(x);
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
            {
                if (n1 > n2)
                {
                    Trace.TraceWarning("Warning: Using a MannWhitneyDistribution where the first sample is larger than the second.");
                    double lower = 0;
                    double upper = 0;

                    double f = DistributionFunction(0);

                    if (f > p)
                    {
                        while (f > p && !double.IsInfinity(lower))
                        {
                            lower = upper;
                            upper = 2 * upper + 1;
                            f = DistributionFunction(upper);
                        }
                    }
                    else if (f < p)
                    {
                        while (f < p && !double.IsInfinity(upper))
                        {
                            upper = lower;
                            lower = 2 * lower - 1;
                            f = DistributionFunction(lower);
                        }
                    }
                    else
                    {
                        return 0;
                    }

                    if (double.IsNegativeInfinity(lower))
                        lower = double.MinValue;

                    if (double.IsPositiveInfinity(upper))
                        upper = double.MaxValue;

                    double value = BrentSearch.Find(DistributionFunction, p, lower, upper);

                    return value;
                }

                return base.InnerInverseDistributionFunction(p);
            }

            return approximation.InverseDistributionFunction(p);
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
            return String.Format(formatProvider, "MannWhitney(u; n1 = {0}, n2 = {1})",
                NumberOfSamples1.ToString(format, formatProvider),
                NumberOfSamples2.ToString(format, formatProvider));
        }
    }
}
