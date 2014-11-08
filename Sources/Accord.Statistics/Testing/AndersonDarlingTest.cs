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
    using Accord.Math;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Distributions;

    /// <summary>
    ///   One-sample Anderson-Darling (AD) test.
    /// </summary>
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
        public IUnivariateDistribution TheoreticalDistribution { get; private set; }

        /// <summary>
        ///   Creates a new Anderson-Darling test.
        /// </summary>
        /// 
        /// <param name="sample">The sample we would like to test as belonging to the <paramref name="hypothesizedDistribution"/>.</param>
        /// <param name="hypothesizedDistribution">A fully specified distribution.</param>
        /// 
        public AndersonDarlingTest(double[] sample, IUnivariateDistribution hypothesizedDistribution)
        {
            double N = sample.Length;

            // Create the test statistic distribution with given degrees of freedom

            this.TheoreticalDistribution = hypothesizedDistribution;
            if (hypothesizedDistribution is UniformContinuousDistribution)
                StatisticDistribution = new AndersonDarlingDistribution(AndersonDarlingDistributionType.Uniform, sample.Length);
            else if (hypothesizedDistribution is NormalDistribution)
                StatisticDistribution = new AndersonDarlingDistribution(AndersonDarlingDistributionType.Normal, sample.Length);

            // Create a copy of the samples to prevent altering the
            // constructor's original arguments in the sorting step 
            double[] Y = (double[])sample.Clone();

            // Sort sample
            Array.Sort(Y);

            // Create the theoretical and empirical distributions
            this.TheoreticalDistribution = hypothesizedDistribution;


            double S = 0;
            int n = Y.Length;

            // Finally, compute the test statistic.
            for (int i = 0; i < Y.Length; i++)
            {
                double a = 2.0 * (i + 1) - 1;
                double b = TheoreticalDistribution.DistributionFunction(Y[i]);
                double c = TheoreticalDistribution.ComplementaryDistributionFunction(Y[n - i - 1]);

                S += a * (Math.Log(b) + Math.Log(c));
            }

            this.Statistic = -n - S / n;
            this.PValue = StatisticToPValue(Statistic);
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