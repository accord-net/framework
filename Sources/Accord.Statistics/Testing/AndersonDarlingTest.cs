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
    using Accord.Math;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Distributions;
    using System.Diagnostics;
    using Accord.Compat;

    /// <summary>
    ///   One-sample Anderson-Darling (AD) test.
    /// </summary>
    /// 
    /// <example>
    /// <code source="Unit Tests\Accord.Tests.Statistics\Testing\AndersonDarlingTestTest.cs" region="doc_test_uniform" />
    /// <code source="Unit Tests\Accord.Tests.Statistics\Testing\AndersonDarlingTestTest.cs" region="doc_test_normal" />
    /// </example>
    /// 
    [Serializable]
    public class AndersonDarlingTest : HypothesisTest<AndersonDarlingDistribution>,
        IHypothesisTest<AndersonDarlingDistribution>
    {

        /// <summary>
        ///   Gets the theoretical, hypothesized distribution for the samples,
        ///   which should have been stated <i>before</i> any measurements.
        /// </summary>
        /// 
        public IUnivariateDistribution<double> TheoreticalDistribution { get; private set; }

        /// <summary>
        ///   Creates a new Anderson-Darling test.
        /// </summary>
        /// 
        /// <param name="sample">The sample we would like to test as belonging to the <paramref name="hypothesizedDistribution"/>.</param>
        /// <param name="hypothesizedDistribution">A fully specified distribution.</param>
        /// 
        public AndersonDarlingTest(double[] sample, IUnivariateDistribution<double> hypothesizedDistribution)
        {
            // Create the test statistic distribution 
            this.TheoreticalDistribution = hypothesizedDistribution;
            if (hypothesizedDistribution is UniformContinuousDistribution)
            {
                StatisticDistribution = new AndersonDarlingDistribution(AndersonDarlingDistributionType.Uniform, sample.Length);
            }
            else if (hypothesizedDistribution is NormalDistribution)
            {
                StatisticDistribution = new AndersonDarlingDistribution(AndersonDarlingDistributionType.Normal, sample.Length);
            }
            else
            {
                Trace.WriteLine(String.Format("Unsupported distribution in AndersonDarling: {0}. P-values will not be computed, but test statistic may be useful.",
                    hypothesizedDistribution.ToString(), hypothesizedDistribution.GetType().ToString()));
            }

            // Create a copy of the samples to prevent altering the
            // constructor's original arguments in the sorting step 
            double[] sortedSamples = sample.Sorted();

            // Create the theoretical and empirical distributions
            this.TheoreticalDistribution = hypothesizedDistribution;

            this.Statistic = GetStatistic(sortedSamples, TheoreticalDistribution);
            this.PValue = StatisticToPValue(Statistic);
        }

        /// <summary>
        ///   Gets the Anderson-Darling statistic for the samples and target distribution.
        /// </summary>
        /// 
        /// <param name="sortedSamples">The sorted samples.</param>
        /// <param name="distribution">The target distribution.</param>
        /// 
        public static double GetStatistic(double[] sortedSamples, IUnivariateDistribution<double> distribution)
        {
            double N = sortedSamples.Length;
            double S = 0;
            int n = sortedSamples.Length;

            // Finally, compute the test statistic.
            for (int i = 0; i < sortedSamples.Length; i++)
            {
                double a = 2.0 * (i + 1) - 1;
                double b = distribution.DistributionFunction(sortedSamples[i]);
                double c = distribution.ComplementaryDistributionFunction(sortedSamples[n - i - 1]);

                S += a * (Math.Log(b) + Math.Log(c));
            }

            return -n - S / n;
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public override double PValueToStatistic(double p)
        {
            throw new NotSupportedException();
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
                return Double.NaN;

            return StatisticDistribution.ComplementaryDistributionFunction(x);
        }
    }
}