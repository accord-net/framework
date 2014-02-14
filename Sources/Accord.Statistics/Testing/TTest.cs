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
    ///   One-sample Student's T test.
    /// </summary>
    /// 
    /// <remarks>
    ///  <para>
    ///   The one-sample t-test assesses whether the mean of a sample is
    ///   statistically different from a hypothesized value.</para>
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
    /// <example>
    ///   <code>
    ///   // Consider a sample generated from a Gaussian
    ///   // distribution with mean 0.5 and unit variance.
    ///   
    ///   double[] sample = 
    ///   { 
    ///       -0.849886940156521,	3.53492346633185,  1.22540422494611, 0.436945126810344, 1.21474290382610,
    ///        0.295033941700225, 0.375855651783688, 1.98969760778547, 1.90903448980048,	1.91719241342961
    ///   };
    ///
    ///   // One may rise the hypothesis that the mean of the sample is not
    ///   // significantly different from zero. In other words, the fact that
    ///   // this particular sample has mean 0.5 may be attributed to chance.
    ///
    ///   double hypothesizedMean = 0;
    ///
    ///   // Create a T-Test to check this hypothesis
    ///   TTest test = new TTest(sample, hypothesizedMean,
    ///          OneSampleHypothesis.ValueIsDifferentFromHypothesis);
    ///
    ///   // Check if the mean is significantly different
    ///   test.Significant should be true
    ///
    ///   // Now, we would like to test if the sample mean is
    ///   // significantly greater than the hypothesized zero.
    ///
    ///   // Create a T-Test to check this hypothesis
    ///   TTest greater = new TTest(sample, hypothesizedMean,
    ///          OneSampleHypothesis.ValueIsGreaterThanHypothesis);
    ///
    ///   // Check if the mean is significantly larger
    ///   greater.Significant should be true
    ///
    ///   // Now, we would like to test if the sample mean is
    ///   // significantly smaller than the hypothesized zero.
    ///
    ///   // Create a T-Test to check this hypothesis
    ///   TTest smaller = new TTest(sample, hypothesizedMean,
    ///          OneSampleHypothesis.ValueIsSmallerThanHypothesis);
    ///
    ///   // Check if the mean is significantly smaller
    ///   smaller.Significant should be false
    ///   </code>
    /// </example>
    /// 
    /// <seealso cref="ZTest"/>
    /// <seealso cref="NormalDistribution"/>
    /// <seealso cref="TwoSampleTTest"/>
    /// <seealso cref="TwoSampleZTest"/>
    /// <seealso cref="PairedTTest"/>
    /// 
    [Serializable]
    public class TTest : HypothesisTest<TDistribution>
    {

        private TTestPowerAnalysis powerAnalysis;

        /// <summary>
        ///   Gets the power analysis for the test, if available.
        /// </summary>
        /// 
        public IPowerAnalysis Analysis { get { return powerAnalysis; } }

        /// <summary>
        ///   Gets the standard error of the estimated value.
        /// </summary>
        /// 
        public double StandardError { get; private set; }

        /// <summary>
        ///   Gets the estimated parameter value, such as the sample's mean value.
        /// </summary>
        /// 
        public double EstimatedValue { get; private set; }

        /// <summary>
        ///   Gets the hypothesized parameter value.
        /// </summary>
        /// 
        public double HypothesizedValue { get; private set; }

        /// <summary>
        ///   Gets the 95% confidence interval for the <see cref="EstimatedValue"/>.
        /// </summary>
        /// 
        public DoubleRange Confidence { get; protected set; }

        /// <summary>
        ///   Gets the alternative hypothesis under test. If the test is
        ///   <see cref="IHypothesisTest.Significant"/>, the null hypothesis can be rejected
        ///   in favor of this alternative hypothesis.
        /// </summary>
        /// 
        public OneSampleHypothesis Hypothesis { get; protected set; }

        /// <summary>
        ///   Gets a confidence interval for the estimated value
        ///   within the given confidence level percentage.
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
                EstimatedValue - u * StandardError,
                EstimatedValue + u * StandardError);
        }

        /// <summary>
        ///   Tests the null hypothesis that the population mean is equal to a specified value.
        /// </summary>
        /// 
        /// <param name="statistic">The test statistic.</param>
        /// <param name="degreesOfFreedom">The degrees of freedom for the test distribution.</param>
        /// <param name="hypothesis">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public TTest(double statistic, double degreesOfFreedom,
            OneSampleHypothesis hypothesis = OneSampleHypothesis.ValueIsDifferentFromHypothesis)
        {
            Compute(statistic, degreesOfFreedom, hypothesis);
        }

        /// <summary>
        ///   Tests the null hypothesis that the population mean is equal to a specified value.
        /// </summary>
        /// 
        /// <param name="estimatedValue">The estimated value (θ).</param>
        /// <param name="standardError">The standard error of the estimation (SE).</param>
        /// <param name="hypothesizedValue">The hypothesized value (θ').</param>
        /// <param name="degreesOfFreedom">The degrees of freedom for the test distribution.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public TTest(double estimatedValue, double standardError, double degreesOfFreedom, double hypothesizedValue = 0,
            OneSampleHypothesis alternate = OneSampleHypothesis.ValueIsDifferentFromHypothesis)
        {
            Compute(estimatedValue, standardError, degreesOfFreedom, hypothesizedValue, alternate);
        }

        /// <summary>
        ///   Tests the null hypothesis that the population mean is equal to a specified value.
        /// </summary>
        /// 
        /// <param name="sample">The data samples from which the test will be performed.</param>
        /// <param name="hypothesizedMean">The constant to be compared with the samples.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public TTest(double[] sample, double hypothesizedMean = 0,
            OneSampleHypothesis alternate = OneSampleHypothesis.ValueIsDifferentFromHypothesis)
        {
            int n = sample.Length;

            double mean = Accord.Statistics.Tools.Mean(sample);
            double stdDev = Accord.Statistics.Tools.StandardDeviation(sample, mean);
            double stdError = Accord.Statistics.Tools.StandardError(n, stdDev);

            Compute(n, mean, hypothesizedMean, stdError, alternate);

            power(stdDev, n);
        }

        /// <summary>
        ///   Tests the null hypothesis that the population mean is equal to a specified value.
        /// </summary>
        /// 
        /// <param name="mean">The sample's mean value.</param>
        /// <param name="stdDev">The standard deviation for the samples.</param>
        /// <param name="samples">The number of observations in the sample.</param>
        /// <param name="hypothesizedMean">The constant to be compared with the samples.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public TTest(double mean, double stdDev, int samples, double hypothesizedMean = 0,
            OneSampleHypothesis alternate = OneSampleHypothesis.ValueIsDifferentFromHypothesis)
        {
            double stdError = Accord.Statistics.Tools.StandardError(samples, stdDev);

            Compute(samples, mean, hypothesizedMean, stdError, alternate);

            power(stdDev, samples);
        }

        /// <summary>
        ///   Computes the T-Test.
        /// </summary>
        /// 
        protected void Compute(int n, double mean, double hypothesizedMean, double stdError, OneSampleHypothesis hypothesis)
        {
            this.EstimatedValue = mean;
            this.StandardError = stdError;
            this.HypothesizedValue = hypothesizedMean;

            double df = n - 1;
            double t = (EstimatedValue - hypothesizedMean) / StandardError;

            Compute(t, df, hypothesis);
        }

        /// <summary>
        ///   Computes the T-test.
        /// </summary>
        /// 
        protected void Compute(double statistic, double df, OneSampleHypothesis alternate)
        {
            this.Statistic = statistic;
            this.StatisticDistribution = new TDistribution(df);
            this.Hypothesis = alternate;
            this.Tail = (DistributionTail)alternate;
            this.PValue = StatisticToPValue(Statistic);

            this.OnSizeChanged();
        }

        /// <summary>
        ///   Computes the T-test.
        /// </summary>
        /// 
        private void Compute(double estimatedValue, double stdError, double degreesOfFreedom,
            double hypothesizedValue, OneSampleHypothesis alternate)
        {
            this.EstimatedValue = estimatedValue;
            this.StandardError = stdError;
            this.HypothesizedValue = hypothesizedValue;

            double df = degreesOfFreedom;
            double t = (EstimatedValue - hypothesizedValue) / StandardError;

            Compute(t, df, alternate);
        }

        private void power(double stdDev, int samples)
        {
            this.powerAnalysis = new TTestPowerAnalysis(Hypothesis)
            {
                Samples = samples,
                Effect = (EstimatedValue - HypothesizedValue) / stdDev,
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
            return PValueToStatistic(p, StatisticDistribution, Tail);
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
            return StatisticToPValue(x, StatisticDistribution, Tail);
        }

        /// <summary>
        ///   Converts a given test statistic to a p-value.
        /// </summary>
        /// 
        /// <param name="t">The value of the test statistic.</param>
        /// <param name="type">The tail of the test distribution.</param>
        /// <param name="distribution">The test distribution.</param>
        /// 
        /// <returns>The p-value for the given statistic.</returns>
        /// 
        public static double StatisticToPValue(double t, TDistribution distribution, DistributionTail type)
        {
            double p;
            switch (type)
            {
                case DistributionTail.TwoTail:
                    p = 2.0 * distribution.ComplementaryDistributionFunction(Math.Abs(t));
                    break;

                case DistributionTail.OneUpper:
                    p = distribution.ComplementaryDistributionFunction(t);
                    break;

                case DistributionTail.OneLower:
                    p = distribution.DistributionFunction(t);
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
        /// <param name="type">The tail of the test distribution.</param>
        /// <param name="distribution">The test distribution.</param>
        /// 
        /// <returns>The test statistic which would generate the given p-value.</returns>
        /// 
        public static double PValueToStatistic(double p, TDistribution distribution, DistributionTail type)
        {
            double t;
            switch (type)
            {
                case DistributionTail.OneLower:
                    t = distribution.InverseDistributionFunction(p);
                    break;
                case DistributionTail.OneUpper:
                    t = distribution.InverseDistributionFunction(1.0 - p);
                    break;
                case DistributionTail.TwoTail:
                    t = distribution.InverseDistributionFunction(1.0 - p / 2.0);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return t;
        }

    
    }
}
