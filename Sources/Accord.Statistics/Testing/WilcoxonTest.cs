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

namespace Accord.Statistics.Testing
{
    using System;
    using Accord.Statistics.Distributions.Univariate;

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
    /// <see cref="WilcoxonSignedRankTest"/>
    /// <seealso cref="TwoSampleWilcoxonSignedRankTest"/>
    /// 
    public class WilcoxonTest : HypothesisTest<WilcoxonDistribution>
    {

        /// <summary>
        ///   Gets the number of samples being tested.
        /// </summary>
        /// 
        public int Samples { get { return StatisticDistribution.Samples; } }

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
        ///   Creates a new Wilcoxon's W+ test.
        /// </summary>
        /// 
        /// <param name="signs">The signs for the sample differences.</param>
        /// <param name="diffs">The differences between samples.</param>
        /// <param name="tail">The distribution tail to test.</param>
        /// 
        public WilcoxonTest(int[] signs, double[] diffs, DistributionTail tail)
        {
            Compute(signs, diffs, tail);
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
        protected void Compute(int[] signs, double[] diffs, DistributionTail tail)
        {
            Signs = signs;
            Delta = diffs;
            Ranks = Accord.Statistics.Tools.Rank(Delta);

            double W = WilcoxonDistribution.WPositive(Signs, Ranks);

            this.Compute(W, Ranks, tail);
        }

        /// <summary>
        ///   Computes the Wilcoxon Signed-Rank test.
        /// </summary>
        /// 
        protected void Compute(double statistic, double[] ranks, DistributionTail tail)
        {
            this.Statistic = statistic;
            this.StatisticDistribution = new WilcoxonDistribution(ranks);
            this.Tail = tail;
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
            double p;
            switch (Tail)
            {
                case DistributionTail.TwoTail:
                    double a = StatisticDistribution.DistributionFunction(x);
                    double b = StatisticDistribution.ComplementaryDistributionFunction(x);
                    p = 2 * Math.Min(a, b);
                    break;

                case DistributionTail.OneUpper:
                    p = StatisticDistribution.DistributionFunction(x);
                    break;

                case DistributionTail.OneLower:
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
                default: throw new InvalidOperationException();
            }

            return b;
        }

    }
}
