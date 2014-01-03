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
    using AForge;

    /// <summary>
    ///   T-Test for two paired samples.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Paired T-test can be used when the samples are dependent; that is, when there
    ///   is only one sample that has been tested twice (repeated measures) or when there are
    ///   two samples that have been matched or "paired". This is an example of a paired difference
    ///   test.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Student%27s_t-test#Dependent_t-test_for_paired_samples">
    ///       Wikipedia, The Free Encyclopedia. Student's t-test. 
    ///       Available from: http://en.wikipedia.org/wiki/Student%27s_t-test#Dependent_t-test_for_paired_samples </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   Suppose we would like to know the effect of a treatment (such
    ///   as a new drug) in improving the well-being of 9 patients. The
    ///   well-being is measured in a discrete scale, going from 0 to 10.</para>
    /// <code>
    /// // To do so, we need to register the initial state of each patient
    /// // and then register their state after a given time under treatment.
    /// 
    /// double[,] patients =
    /// {
    ///         //                 before      after
    ///         //                treatment  treatment
    ///         /* Patient 1.*/ {     0,         1     },
    ///         /* Patient 2.*/ {     6,         5     },
    ///         /* Patient 3.*/ {     4,         9     },
    ///         /* Patient 4.*/ {     8,         6     },
    ///         /* Patient 5.*/ {     1,         6     },
    ///         /* Patient 6.*/ {     6,         7     },
    ///         /* Patient 7.*/ {     3,         4     },
    ///         /* Patient 8.*/ {     8,         7     },
    ///         /* Patient 9.*/ {     6,         5     },
    /// };
    /// 
    /// // Extract the before and after columns
    /// double[] before = patients.GetColumn(0);
    /// double[] after = patients.GetColumn(1);
    /// 
    /// // Create the paired-sample T-test. Our research hypothesis is
    /// // that the treatment does improve the patient's well-being. So
    /// // we will be testing the hypothesis that the well-being of the
    /// // "before" sample, the first sample, is "smaller" in comparison
    /// // to the "after" treatment group.
    /// 
    /// PairedTTest test = new PairedTTest(before, after,
    ///     TwoSampleHypothesis.FirstValueIsSmallerThanSecond);
    /// 
    /// bool significant = test.Significant; //   not significant
    /// double pvalue = test.PValue;         //  p-value =  0.1650
    /// double tstat  = test.Statistic;      //  t-stat  = -1.0371
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class PairedTTest : HypothesisTest<TDistribution>
    {

        /// <summary>
        ///   Gets the alternative hypothesis under test. If the test is
        ///   <see cref="IHypothesisTest.Significant"/>, the null hypothesis can be rejected
        ///   in favor of this alternative hypothesis.
        /// </summary>
        /// 
        public TwoSampleHypothesis Hypothesis { get; private set; }

        /// <summary>
        ///   Gets the first sample's mean.
        /// </summary>
        /// 
        public double Mean1 { get; private set; }

        /// <summary>
        ///   Gets the second sample's mean.
        /// </summary>
        /// 
        public double Mean2 { get; private set; }

        /// <summary>
        ///   Gets the observed mean difference between the two samples.
        /// </summary>
        /// 
        public double ObservedDifference { get; private set; }

        /// <summary>
        ///   Gets the standard error of the difference.
        /// </summary>
        /// 
        public double StandardError { get; private set; }

        /// <summary>
        ///   Gets the size of a sample. 
        ///   Both samples have equal size.
        /// </summary>
        /// 
        public int SampleSize { get; private set; }

        /// <summary>
        ///   Gets the 95% confidence interval for the
        ///   <see cref="ObservedDifference"/> statistic.
        /// </summary>
        /// 
        public DoubleRange Confidence { get; protected set; }

        /// <summary>
        ///   Gets a confidence interval for the <see cref="ObservedDifference"/>
        ///   statistic within the given confidence level percentage.
        /// </summary>
        /// 
        /// <param name="percent">The confidence level. Default is 0.95.</param>
        /// 
        /// <returns>A confidence interval for the estimated value.</returns>
        /// 
        public DoubleRange GetConfidenceInterval(double percent = 0.95)
        {
            double u = PValueToStatistic(1.0 - percent);

            return new DoubleRange(
                ObservedDifference - u * StandardError,
                ObservedDifference + u * StandardError);
        }

        /// <summary>
        ///   Creates a new paired t-test.
        /// </summary>
        /// 
        /// <param name="sample1">The observations in the first sample.</param>
        /// <param name="sample2">The observations in the second sample.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public PairedTTest(double[] sample1, double[] sample2,
            TwoSampleHypothesis alternate = TwoSampleHypothesis.ValuesAreDifferent)
        {
            if (sample1.Length != sample2.Length)
                throw new DimensionMismatchException("sample2", "Paired samples must have the same size.");

            int n = sample1.Length;

            double[] delta = new double[sample1.Length];
            for (int i = 0; i < delta.Length; i++)
                delta[i] = sample1[i] - sample2[i];

            double mean = delta.Mean();
            double std = delta.StandardDeviation();

            StandardError = std / Math.Sqrt(n);
            ObservedDifference = mean;
            Mean1 = sample1.Mean();
            Mean2 = sample2.Mean();

            SampleSize = n;
            Statistic = ObservedDifference / StandardError;
            StatisticDistribution = new TDistribution(n - 1);

            this.Hypothesis = alternate;
            this.Tail = (DistributionTail)alternate;
            this.PValue = StatisticToPValue(Statistic);

            this.OnSizeChanged();
        }

        /// <summary>Update event.</summary>
        protected override void OnSizeChanged()
        {
            this.Confidence = GetConfidenceInterval(1.0 - Size);
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
            return TTest.PValueToStatistic(p, StatisticDistribution, Tail);
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
            return TTest.StatisticToPValue(x, StatisticDistribution, Tail);
        }
    }
}
