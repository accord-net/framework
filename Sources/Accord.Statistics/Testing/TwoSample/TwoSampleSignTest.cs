﻿// Accord Statistics Library
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
    using Accord.Compat;

    /// <summary>
    ///   Sign test for two paired samples.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This is a <see cref="BinomialTest">Binomial kind of test</see>.</para>
    /// </remarks>
    /// 
    /// <seealso cref="SignTest"/>
    /// <seealso cref="BinomialTest"/>
    /// <seealso cref="Accord.Statistics.Distributions.Univariate.BinomialDistribution"/>
    /// 
    [Serializable]
    public class TwoSampleSignTest : BinomialTest
    {

        /// <summary>
        ///   Gets the alternative hypothesis under test. If the test is
        ///   <see cref="IHypothesisTest.Significant"/>, the null hypothesis
        ///   can be rejected in favor of this alternative hypothesis.
        /// </summary>
        /// 
        public new TwoSampleHypothesis Hypothesis { get; protected set; }


        /// <summary>
        ///   Creates a new sign test for two samples.
        /// </summary>
        /// 
        /// <param name="positiveSamples">The number of positive samples (successes).</param>
        /// <param name="totalSamples">The total number of samples (trials).</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public TwoSampleSignTest(int positiveSamples, int totalSamples, TwoSampleHypothesis alternate)
        {
            Compute(positiveSamples, totalSamples, alternate);
        }

        /// <summary>
        ///   Creates a new sign test for two samples.
        /// </summary>
        /// 
        /// <param name="sample1">The first sample of observations.</param>
        /// <param name="sample2">The second sample of observations.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public TwoSampleSignTest(double[] sample1, double[] sample2, TwoSampleHypothesis alternate)
        {
            if (sample1.Length != sample2.Length)
                throw new DimensionMismatchException("sample2", "Both samples should be of the same size.");

            int positive = 0;
            int negative = 0;

            for (int i = 0; i < sample1.Length; i++)
            {
                double d = sample2[i] - sample1[i];

                if (d > 0) positive++;
                else if (d < 0) negative++;
            }


            Compute(positive, positive + negative, alternate);
        }


        /// <summary>
        ///   Computes the two sample sign test.
        /// </summary>
        /// 
        protected void Compute(int positive, int total, TwoSampleHypothesis alternate)
        {
            this.Hypothesis = alternate;

            // The underlying test is to check whether the probability
            // value from the samples are higher than or lesser than 0.5,
            // thus the actual Binomial test hypothesis is inverted:

            OneSampleHypothesis binomialHypothesis;

            switch (alternate)
            {
                case TwoSampleHypothesis.ValuesAreDifferent:
                    binomialHypothesis = OneSampleHypothesis.ValueIsDifferentFromHypothesis; break;
                case TwoSampleHypothesis.FirstValueIsGreaterThanSecond:
                    binomialHypothesis = OneSampleHypothesis.ValueIsSmallerThanHypothesis; break;
                case TwoSampleHypothesis.FirstValueIsSmallerThanSecond:
                    binomialHypothesis = OneSampleHypothesis.ValueIsGreaterThanHypothesis; break;
                default: throw new InvalidOperationException();
            }


            base.Compute(positive, total, 0.5, binomialHypothesis);
        }

    }
}
