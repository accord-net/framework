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
    ///   Wilcoxon signed-rank test for paired samples.
    /// </summary>
    /// 
    public class TwoSampleWilcoxonSignedRankTest : WilcoxonTest
    {

        /// <summary>
        ///   Gets the alternative hypothesis under test. If the test is
        ///   <see cref="IHypothesisTest.Significant"/>, the null hypothesis can be rejected
        ///   in favor of this alternative hypothesis.
        /// </summary>
        /// 
        public TwoSampleHypothesis Hypothesis { get; protected set; }

        /// <summary>
        ///   Tests whether the medians of two paired samples are different.
        /// </summary>
        /// 
        /// <param name="sample1">The first sample.</param>
        /// <param name="sample2">The second sample.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public TwoSampleWilcoxonSignedRankTest(double[] sample1, double[] sample2,
            TwoSampleHypothesis alternate = TwoSampleHypothesis.ValuesAreDifferent)
        {
            if (sample1.Length != sample2.Length)
                throw new DimensionMismatchException("sample2", "Both samples should be of the same size.");

            int[] signs = new int[sample1.Length];
            double[] diffs = new double[sample1.Length];

            // 1. Compute absolute difference and sign function
            for (int i = 0; i < sample1.Length; i++)
            {
                double d = sample1[i] - sample2[i];
                signs[i] = Math.Sign(d);
                diffs[i] = Math.Abs(d);
            }

            this.Hypothesis = alternate;
            Compute(signs, diffs, (DistributionTail)alternate);
        }


    }
}
