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

    /// <summary>
    ///   Wilcoxon signed-rank test for the median.
    /// </summary>
    /// 
    /// <remarks>
    ///   This test uses the positive W statistic, as explained in 
    ///   https://onlinecourses.science.psu.edu/stat414/node/319
    /// </remarks>
    /// 
    [Serializable]
    public class WilcoxonSignedRankTest : WilcoxonTest
    {

        /// <summary>
        ///   Gets the alternative hypothesis under test. If the test is
        ///   <see cref="IHypothesisTest.Significant"/>, the null hypothesis can be rejected
        ///   in favor of this alternative hypothesis.
        /// </summary>
        /// 
        public OneSampleHypothesis Hypothesis { get; protected set; }

        /// <summary>
        ///   Tests the null hypothesis that the sample median is equal to a hypothesized value.
        /// </summary>
        /// 
        /// <param name="sample">The data samples from which the test will be performed.</param>
        /// <param name="hypothesizedMedian">The constant to be compared with the samples.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public WilcoxonSignedRankTest(double[] sample, double hypothesizedMedian = 0,
            OneSampleHypothesis alternate = OneSampleHypothesis.ValueIsDifferentFromHypothesis)
        {
            int[] signs = new int[sample.Length];
            double[] diffs = new double[sample.Length];

            // 1. Compute absolute difference and sign function
            for (int i = 0; i < sample.Length; i++)
            {
                double d = sample[i] - hypothesizedMedian;
                signs[i] = Math.Sign(d);
                diffs[i] = Math.Abs(d);
            }

            this.Hypothesis = alternate;

            Compute(signs, diffs, (DistributionTail)alternate);
        }

    }
}
