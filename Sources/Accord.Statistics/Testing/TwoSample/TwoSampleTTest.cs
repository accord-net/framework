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
    using Accord.Statistics.Testing.Power;
    using AForge;

    /// <summary>
    ///   Two-sample Student's T test.
    /// </summary>
    /// 
    /// <remarks>
    ///  <para>
    ///   The two-sample t-test assesses whether the means of two groups are statistically 
    ///   different from each other.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Student's_t-test">
    ///       Wikipedia, The Free Encyclopedia. Student's T-Test. </a></description></item>
    ///     <item><description><a href="http://www.le.ac.uk/bl/gat/virtualfc/Stats/ttest.html">
    ///       William M.K. Trochim. The T-Test. Research methods Knowledge Base, 2009. 
    ///       Available on: http://www.le.ac.uk/bl/gat/virtualfc/Stats/ttest.html </a></description></item>
    ///     <item><description><a href="http://en.wikipedia.org/wiki/One-way_ANOVA">
    ///       Graeme D. Ruxton. The unequal variance t-test is an underused alternative to Student's
    ///       t-test and the Mann–Whitney U test. Oxford Journals, Behavioral Ecology Volume 17, Issue 4, pp.
    ///       688-690. 2006. Available on: http://beheco.oxfordjournals.org/content/17/4/688.full </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    [Serializable]
    public class TwoSampleTTest : HypothesisTest<TDistribution>
    {

        private TwoSampleTTestPowerAnalysis powerAnalysis;

        /// <summary>
        ///   Gets the power analysis for the test, if available.
        /// </summary>
        /// 
        public ITwoSamplePowerAnalysis Analysis { get { return powerAnalysis; } }

        /// <summary>
        ///   Gets the alternative hypothesis under test. If the test is
        ///   <see cref="IHypothesisTest.Significant"/>, the null hypothesis can be rejected
        ///   in favor of this alternative hypothesis.
        /// </summary>
        /// 
        public TwoSampleHypothesis Hypothesis { get; private set; }

        /// <summary>
        ///   Gets whether the test assumes equal sample variance.
        /// </summary>
        /// 
        public bool AssumeEqualVariance { get; private set; }

        /// <summary>
        ///   Gets the standard error for the difference.
        /// </summary>
        /// 
        public double StandardError { get; protected set; }

        /// <summary>
        ///   Gets the estimated value for the first sample.
        /// </summary>
        /// 
        public double EstimatedValue1 { get; protected set; }

        /// <summary>
        ///   Gets the estimated value for the second sample.
        /// </summary>
        /// 
        public double EstimatedValue2 { get; protected set; }

        /// <summary>
        ///   Gets the hypothesized difference between the two estimated values.
        /// </summary>
        /// 
        public double HypothesizedDifference { get; protected set; }

        /// <summary>
        ///   Gets the actual difference between the two estimated values.
        /// </summary>
        public double ObservedDifference { get; protected set; }

        /// <summary>
        ///   Gets the degrees of freedom for the test statistic.
        /// </summary>
        /// 
        public double DegreesOfFreedom { get { return StatisticDistribution.DegreesOfFreedom; } }

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
        ///   Tests whether the means of two samples are different.
        /// </summary>
        /// 
        /// <param name="sample1">The first sample.</param>
        /// <param name="sample2">The second sample.</param>
        /// <param name="hypothesizedDifference">The hypothesized sample difference.</param>
        /// <param name="assumeEqualVariances">True to assume equal variances, false otherwise. Default is true.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public TwoSampleTTest(double[] sample1, double[] sample2,
            bool assumeEqualVariances = true, double hypothesizedDifference = 0,
            TwoSampleHypothesis alternate = TwoSampleHypothesis.ValuesAreDifferent)
        {
            // References: http://en.wikipedia.org/wiki/Student's_t-test#Worked_examples

            double x1 = Tools.Mean(sample1);
            double x2 = Tools.Mean(sample2);

            double s1 = Tools.Variance(sample1);
            double s2 = Tools.Variance(sample2);

            int n1 = sample1.Length;
            int n2 = sample2.Length;

            Compute(x1, s1, n1, x2, s2, n2,
                hypothesizedDifference, assumeEqualVariances, alternate);

            power(s1, s2, sample1.Length, sample2.Length);
        }

        /// <summary>
        ///   Tests whether the means of two samples are different.
        /// </summary>
        /// 
        /// <param name="mean1">The first sample's mean.</param>
        /// <param name="mean2">The second sample's mean.</param>
        /// <param name="var1">The first sample's variance.</param>
        /// <param name="var2">The second sample's variance.</param>
        /// <param name="samples1">The number of observations in the first sample.</param>
        /// <param name="samples2">The number of observations in the second sample.</param>
        /// <param name="assumeEqualVariances">True assume equal variances, false otherwise. Default is true.</param>
        /// <param name="hypothesizedDifference">The hypothesized sample difference.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public TwoSampleTTest(double mean1, double var1, int samples1, double mean2, double var2, int samples2,
            bool assumeEqualVariances = true, double hypothesizedDifference = 0,
            TwoSampleHypothesis alternate = TwoSampleHypothesis.ValuesAreDifferent)
        {

            Compute(mean1, var1, samples1, mean2, var2, samples2,
                hypothesizedDifference, assumeEqualVariances, alternate);

            power(var1, var2, samples1, samples2);
        }


        /// <summary>
        ///   Creates a new two-sample T-Test.
        /// </summary>
        protected TwoSampleTTest()
        {
        }


        /// <summary>
        ///   Computes the T Test.
        /// </summary>
        /// 
        protected void Compute(
            double x1, double s1, int n1,
            double x2, double s2, int n2,
            double hypothesizedDifference, bool equalVar,
            TwoSampleHypothesis alternate)
        {
            double df;

            this.AssumeEqualVariance = equalVar;
            this.EstimatedValue1 = x1;
            this.EstimatedValue2 = x2;

            if (AssumeEqualVariance)
            {
                // Assume the two samples share the same underlying population variance.
                double Sp = Math.Sqrt(((n1 - 1) * s1 + (n2 - 1) * s2) / (n1 + n2 - 2));
                StandardError = Sp * Math.Sqrt(1.0 / n1 + 1.0 / n2);
                df = n1 + n2 - 2;
            }
            else
            {
                // Assume samples have unequal variances.
                StandardError = Math.Sqrt(s1 / n1 + s2 / n2);

                double r1 = s1 / n1, r2 = s2 / n2;
                df = ((r1 + r2) * (r1 + r2)) / ((r1 * r1) / (n1 - 1) + (r2 * r2) / (n2 - 1));
            }



            this.ObservedDifference = (x1 - x2);
            this.HypothesizedDifference = hypothesizedDifference;

            this.Statistic = (ObservedDifference - HypothesizedDifference) / StandardError;
            this.StatisticDistribution = new TDistribution(df);

            this.Hypothesis = alternate;
            this.Tail = (DistributionTail)alternate;
            this.PValue = StatisticToPValue(Statistic);

            this.OnSizeChanged();
        }

        private void power(double var1, double var2, int n1, int n2)
        {
            double stdDev = Math.Sqrt((var1 + var2) / 2.0);

            powerAnalysis = new TwoSampleTTestPowerAnalysis(Hypothesis)
            {
                Samples1 = n1,
                Samples2 = n2,
                Effect = (ObservedDifference - HypothesizedDifference) / stdDev,
                Size = Size,
            };

            powerAnalysis.ComputePower();
        }

        /// <summary>Update event.</summary>
        protected override void OnSizeChanged()
        {
            this.Confidence = GetConfidenceInterval(1.0 - Size);
            if (Analysis != null)
            {
                powerAnalysis.Size = Size;
                powerAnalysis.ComputePower();
            }
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