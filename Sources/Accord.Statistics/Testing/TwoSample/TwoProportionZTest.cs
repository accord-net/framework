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
    ///   Z-Test for two sample proportions.
    /// </summary>
    /// 
    /// <seealso cref="TwoSampleZTest"/>
    /// <seealso cref="ZTest"/>
    /// 
    [Serializable]
    public class TwoProportionZTest : TwoSampleZTest
    {

        /// <summary>
        ///   Creates a new Z-Test for two sample proportions.
        /// </summary>
        /// 
        /// <param name="proportion1">The proportion of success observations in the first sample.</param>
        /// <param name="sampleSize1">The total number of observations in the first sample.</param>
        /// <param name="proportion2">The proportion of success observations in the second sample.</param>
        /// <param name="sampleSize2">The total number of observations in the second sample.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public TwoProportionZTest(double proportion1, int sampleSize1, double proportion2, int sampleSize2,
            TwoSampleHypothesis alternate = TwoSampleHypothesis.ValuesAreDifferent)
        {
            int x1 = (int)Math.Round(proportion1 * sampleSize1);
            int x2 = (int)Math.Round(proportion2 * sampleSize2);

            Compute(x1, sampleSize1, x2, sampleSize2, alternate);
        }

        /// <summary>
        ///   Creates a new Z-Test for two sample proportions.
        /// </summary>
        /// 
        /// <param name="successes1">The number of successes in the first sample.</param>
        /// <param name="trials1">The total number of trials (observations) in the first sample.</param>
        /// <param name="successes2">The number of successes in the second sample.</param>
        /// <param name="trials2">The total number of trials (observations) in the second sample.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public TwoProportionZTest(int successes1, int trials1, int successes2, int trials2,
            TwoSampleHypothesis alternate = TwoSampleHypothesis.ValuesAreDifferent)
        {
            Compute(successes1, trials1, successes2, trials2, alternate);
        }

        /// <summary>
        ///   Computes the Z-test for two sample proportions.
        /// </summary>
        /// 
        protected void Compute(
            int successes1, int trials1,
            int successes2, int trials2,
            TwoSampleHypothesis alternate)
        {
            double p1 = successes1 / (double)trials1;
            double p2 = successes2 / (double)trials2;
            double p = (successes1 + successes2) / (double)(trials1 + trials2);

            EstimatedValue1 = p1;
            EstimatedValue2 = p2;
            ObservedDifference = p1 - p2;
            StandardError = Math.Sqrt(p * (1 - p) * (1.0 / trials1 + 1.0 / trials2));

            double z = ObservedDifference / StandardError;

            Compute(z, alternate);
        }
    }
}
