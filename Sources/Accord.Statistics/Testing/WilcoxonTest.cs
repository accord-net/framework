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

namespace Accord.Statistics.Testing
{
    using System;
    using Accord.Statistics.Distributions.Univariate;
    using System.Linq;
    using Accord.Math;
    using System.Collections.Generic;

    /// <summary>
    ///   Base class for Wilcoxon's W tests.
    /// </summary>
    /// 
    /// <remarks>
    ///   This is a base class which doesn't need to be used directly.
    ///   Instead, you may wish to call <see cref="WilcoxonSignedRankTest"/>
    ///   and <see cref="TwoSampleWilcoxonSignedRankTest"/>.
    /// </remarks>
    /// 
    /// <seealso cref="WilcoxonSignedRankTest"/>
    /// <seealso cref="TwoSampleWilcoxonSignedRankTest"/>
    /// 
    /// <seealso cref="Accord.Statistics.Distributions.Univariate.WilcoxonDistribution"/>
    /// 
    public class WilcoxonTest : HypothesisTest<WilcoxonDistribution>
    {
        private bool hasTies;

        /// <summary>
        ///   Gets the number of samples being tested.
        /// </summary>
        /// 
        public int NumberOfSamples { get { return StatisticDistribution.NumberOfSamples; } }

        /// <summary>
        ///   Gets the signs for each of the <see cref="Delta"/> differences.
        /// </summary>
        /// 
        public int[] Signs { get; protected set; }

        /// <summary>
        ///   Gets the differences between the samples.
        /// </summary>
        /// 
        public double[] Delta { get; protected set; }

        /// <summary>
        ///   Gets the rank statistics for the differences.
        /// </summary>
        /// 
        public double[] Ranks { get; protected set; }

        /// <summary>
        ///   Gets wether the samples to be ranked contain zeros.
        /// </summary>
        /// 
        public bool HasZeros { get; private set; }

        /// <summary>
        ///   Gets wether the samples to be ranked contain ties.
        /// </summary>
        /// 
        public bool HasTies { get { return hasTies; } }

        /// <summary>
        ///   Gets whether we are using a exact test.
        /// </summary>
        /// 
        public bool IsExact { get; private set; }

        /// <summary>
        ///   Creates a new Wilcoxon's W+ test.
        /// </summary>
        /// 
        /// <param name="signs">The signs for the sample differences.</param>
        /// <param name="diffs">The differences between samples.</param>
        /// <param name="tail">The distribution tail to test.</param>
        /// 
        public WilcoxonTest(int[] signs, double[] diffs, DistributionTail tail)
        {
            Compute(signs, diffs, tail, null, adjustForTies: true);
        }

        /// <summary>
        ///   Creates a new Wilcoxon's W+ test.
        /// </summary>
        /// 
        protected WilcoxonTest()
        {
        }

        /// <summary>
        ///   Computes the Wilcoxon Signed-Rank test.
        /// </summary>
        /// 
        protected void Compute(int[] signs, double[] diffs, DistributionTail tail, 
            bool? exact, bool adjustForTies)
        {
            int[] nonZeros = diffs.Find(x => x != 0);
            this.HasZeros = nonZeros.Length != diffs.Length;

            if (HasZeros)
            {
                // It is actually necessary to discard zeros before the test procedure
                // https://en.wikipedia.org/wiki/Wilcoxon_signed-rank_test#Test_procedure
                // http://vassarstats.net/textbook/ch12a.html
                signs = signs.Get(nonZeros);
                diffs = diffs.Get(nonZeros);
            }

            this.Signs = signs;
            this.Delta = diffs;
            this.Ranks = Delta.Rank(hasTies: out hasTies, adjustForTies: adjustForTies);

            double W = WilcoxonDistribution.WPositive(Signs, Ranks);

            this.Compute(W, Ranks, tail, exact);
        }

        /// <summary>
        ///   Computes the Wilcoxon Signed-Rank test.
        /// </summary>
        /// 
        protected void Compute(double statistic, double[] ranks, DistributionTail tail, bool? exact)
        {
            if (this.HasZeros)
            {
                if (!exact.HasValue)
                    exact = false;

                if (exact == true)
                    throw new ArgumentException("An exact test cannot be computed when there are zeros in the samples (or when paired samples are the same in a paired test).");
            }

            this.Statistic = statistic;
            this.Tail = tail;

            if (ranks.Length != 0)
            {
                this.StatisticDistribution = new WilcoxonDistribution(ranks, exact)
                {
                    Correction = (Tail == DistributionTail.TwoTail) ? ContinuityCorrection.Midpoint : ContinuityCorrection.KeepInside
                };

                this.IsExact = this.StatisticDistribution.Exact;
            }
            else
            {
                this.StatisticDistribution = null;
                this.IsExact = exact.GetValueOrDefault(false);
            }

            this.PValue = StatisticToPValue(Statistic);

            this.OnSizeChanged();
        }

        /// <summary>
        ///   Converts a given test statistic to a p-value.
        /// </summary>
        /// 
        /// <param name="x">The value of the test statistic.</param>
        /// 
        /// <returns>The p-value for the given statistic.</returns>
        /// 
        public override double StatisticToPValue(double x)
        {
            if (StatisticDistribution == null)
                return Double.NaN; // return NaN to match R's behavior

            double p;
            switch (Tail)
            {
                case DistributionTail.TwoTail:
                    double a = StatisticDistribution.DistributionFunction(x);
                    double b = StatisticDistribution.ComplementaryDistributionFunction(x);
                    double c = Math.Min(a, b);
                    p = Math.Min(2 * c, 1);
                    break;

                case DistributionTail.OneLower:
                    p = StatisticDistribution.DistributionFunction(x);
                    break;

                case DistributionTail.OneUpper:
                    p = StatisticDistribution.ComplementaryDistributionFunction(x);
                    break;

                default:
                    throw new InvalidOperationException();
            }
            return p;
        }

        /// <summary>
        ///   Converts a given p-value to a test statistic.
        /// </summary>
        /// 
        /// <param name="p">The p-value.</param>
        /// 
        /// <returns>The test statistic which would generate the given p-value.</returns>
        /// 
        public override double PValueToStatistic(double p)
        {
            double b;
            switch (Tail)
            {
                case DistributionTail.OneLower:
                    b = StatisticDistribution.InverseDistributionFunction(p);
                    break;
                case DistributionTail.OneUpper:
                    b = StatisticDistribution.InverseDistributionFunction(1.0 - p);
                    break;
                case DistributionTail.TwoTail:
                    b = StatisticDistribution.InverseDistributionFunction(1.0 - p / 2.0);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return b;
        }

    }
}
