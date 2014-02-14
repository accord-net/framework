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
    ///   Sign test for the median.
    /// </summary>
    /// 
    [Serializable]
    public class SignTest : BinomialTest
    {

        /// <summary>
        ///   Gets the alternative hypothesis under test. If the test is
        ///   <see cref="IHypothesisTest.Significant"/>, the null hypothesis
        ///   can be rejected in favor of this alternative hypothesis.
        /// </summary>
        /// 
        public new OneSampleHypothesis Hypothesis { get; protected set; }

        /// <summary>
        ///   Tests the null hypothesis that the sample median is equal to a hypothesized value.
        /// </summary>
        /// 
        /// <param name="positiveSamples">The number of positive samples.</param>
        /// <param name="totalSamples">The total number of samples.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public SignTest(int positiveSamples, int totalSamples, OneSampleHypothesis alternate)
        {
            Compute(positiveSamples, totalSamples, alternate);
        }

        /// <summary>
        ///   Tests the null hypothesis that the sample median is equal to a hypothesized value.
        /// </summary>
        /// 
        /// <param name="sample">The data samples from which the test will be performed.</param>
        /// <param name="hypothesizedMedian">The constant to be compared with the samples.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public SignTest(double[] sample, double hypothesizedMedian = 0, 
            OneSampleHypothesis alternate = OneSampleHypothesis.ValueIsDifferentFromHypothesis)
        {
            int positive = 0;
            int negative = 0;

            for (int i = 0; i < sample.Length; i++)
            {
                double d = sample[i] - hypothesizedMedian;

                if (d > 0)
                    positive++;
                else if (d < 0)
                    negative++;
            }


            Compute(positive, positive + negative, alternate);
        }

        /// <summary>
        ///   Computes the one sample sign test.
        /// </summary>
        /// 
        protected void Compute(int positive, int total, OneSampleHypothesis alternate)
        {
            this.Hypothesis = alternate;

            // The underlying test is to check whether the probability
            // value from the samples are higher than or lesser than 0.5,
            // thus the actual Binomial test hypothesis is inverted:

            OneSampleHypothesis binomialHypothesis;

            switch (alternate)
            {
                case OneSampleHypothesis.ValueIsDifferentFromHypothesis:
                    binomialHypothesis = OneSampleHypothesis.ValueIsDifferentFromHypothesis; break;
                case OneSampleHypothesis.ValueIsGreaterThanHypothesis:
                    binomialHypothesis = OneSampleHypothesis.ValueIsSmallerThanHypothesis; break;
                case OneSampleHypothesis.ValueIsSmallerThanHypothesis:
                    binomialHypothesis = OneSampleHypothesis.ValueIsGreaterThanHypothesis; break;
                default: throw new InvalidOperationException();
            }

            base.Compute(positive, total, 0.5, binomialHypothesis);
        }


    }
}
