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
    ///   One-sample Z-Test (location test).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The term Z-test is often used to refer specifically to the one-sample
    ///   location test comparing the mean of a set of measurements to a given
    ///   constant. Due to the central limit theorem, many test statistics are 
    ///   approximately normally distributed for large samples. Therefore, many
    ///   statistical tests can be performed as approximate Z-tests if the sample
    ///   size is large.</para>
    ///   
    /// <para>
    ///   If the test is <see cref="IHypothesisTest.Significant"/>, the null hypothesis 
    ///   can be rejected in favor of the <see cref="Hypothesis">alternate hypothesis</see>
    ///   specified at the creation of the test.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Z-test">
    ///        Wikipedia, The Free Encyclopedia. Z-Test. Available on:
    ///        http://en.wikipedia.org/wiki/Z-test </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   This example has been gathered from the Wikipedia's page about
    ///   the Z-Test, available from: http://en.wikipedia.org/wiki/Z-test </para>
    /// <para>
    ///   Suppose there is a text comprehension test being run across
    ///   a given demographic region. The mean score of the population
    ///   from this entire region are around 100 points, with a standard
    ///   deviation of 12 points.</para>
    /// <para>There is a local school, however, whose 55 students attained
    ///   an average score in the test of only about 96 points. Would 
    ///   their scores be surprisingly that low, or could this event
    ///   have happened due to chance?</para>
    ///   
    /// <code>
    /// // So we would like to check that a sample of
    /// // 55 students with a mean score of 96 points:
    /// 
    /// int sampleSize = 55;
    /// double sampleMean = 96;
    /// 
    /// // Was expected to have happened by chance in a population with
    /// // an hypothesized mean of 100 points and standard deviation of
    /// // about 12 points:
    /// 
    /// double standardDeviation = 12;
    /// double hypothesizedMean = 100;
    /// 
    /// 
    /// // So we start by creating the test:
    /// ZTest test = new ZTest(sampleMean, standardDeviation, sampleSize,
    ///     hypothesizedMean, OneSampleHypothesis.ValueIsSmallerThanHypothesis);
    /// 
    /// // Now, we can check whether this result would be
    /// // unlikely under a standard significance level:
    /// 
    /// bool significant  = test.Significant;
    /// 
    /// // We can also check the test statistic and its P-Value
    /// double statistic = test.Statistic;
    /// double pvalue = test.PValue;
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="TTest"/>
    /// <seealso cref="NormalDistribution"/>
    /// <seealso cref="TwoSampleTTest"/>
    /// <seealso cref="TwoSampleZTest"/>
    /// <seealso cref="TwoProportionZTest"/>
    /// 
    [Serializable]
    public class ZTest : HypothesisTest<NormalDistribution>
    {

        private ZTestPowerAnalysis powerAnalysis;

        /// <summary>
        ///   Gets the power analysis for the test, if available.
        /// </summary>
        /// 
        public IPowerAnalysis Analysis { get { return powerAnalysis; } }

        /// <summary>
        ///   Gets the standard error of the estimated value.
        /// </summary>
        /// 
        public double StandardError { get; protected set; }

        /// <summary>
        ///   Gets the estimated value, such as the mean estimated from a sample.
        /// </summary>
        /// 
        public double EstimatedValue { get; protected set; }

        /// <summary>
        ///   Gets the hypothesized value.
        /// </summary>
        /// 
        public double HypothesizedValue { get; protected set; }

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
        ///   Gets a confidence interval for the <see cref="EstimatedValue"/>
        ///   statistic within the given confidence level percentage.
        /// </summary>
        /// 
        /// <param name="percent">The confidence level. Default is 0.95.</param>
        /// 
        /// <returns>A confidence interval for the estimated value.</returns>
        /// 
        public DoubleRange GetConfidenceInterval(double percent = 0.95)
        {
            double z = PValueToStatistic(1.0 - percent);

            return new DoubleRange(
                EstimatedValue - z * StandardError,
                EstimatedValue + z * StandardError);
        }


        /// <summary>
        ///   Constructs a Z test.
        /// </summary>
        /// 
        /// <param name="samples">The data samples from which the test will be performed.</param>
        /// <param name="hypothesizedMean">The constant to be compared with the samples.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public ZTest(double[] samples, double hypothesizedMean = 0,
            OneSampleHypothesis alternate = OneSampleHypothesis.ValueIsDifferentFromHypothesis)
        {
            double mean = Tools.Mean(samples);
            double stdDev = Tools.StandardDeviation(samples, EstimatedValue);
            double stdError = Tools.StandardError(samples.Length, stdDev);

            if (samples.Length < 30)
            {
                System.Diagnostics.Trace.TraceWarning(
                    "Warning: running a Z test for less than 30 samples. Consider running a Student's T Test instead.");
            }

            this.Compute(mean, hypothesizedMean, stdError, alternate);

            this.power(stdDev, samples.Length);
        }

        /// <summary>
        ///   Constructs a Z test.
        /// </summary>
        /// 
        /// <param name="sampleMean">The sample's mean.</param>
        /// <param name="standardError">The sample's standard error.</param>
        /// <param name="hypothesizedMean">The hypothesized value for the distribution's mean.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public ZTest(double sampleMean, double standardError, double hypothesizedMean = 0,
            OneSampleHypothesis alternate = OneSampleHypothesis.ValueIsGreaterThanHypothesis)
        {
            this.Compute(sampleMean, hypothesizedMean, standardError, alternate);
        }

        /// <summary>
        ///   Constructs a Z test.
        /// </summary>
        /// 
        /// <param name="sampleMean">The sample's mean.</param>
        /// <param name="sampleStdDev">The sample's standard deviation.</param>
        /// <param name="hypothesizedMean">The hypothesized value for the distribution's mean.</param>
        /// <param name="samples">The sample's size.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public ZTest(double sampleMean, double sampleStdDev, int samples, double hypothesizedMean = 0,
            OneSampleHypothesis alternate = OneSampleHypothesis.ValueIsDifferentFromHypothesis)
        {

            if (samples < 30)
            {
                System.Diagnostics.Trace.TraceWarning(
                    "Warning: running a Z test for less than 30 samples. Consider running a Student's T Test instead.");
            }

            double stdError = Tools.StandardError(samples, sampleStdDev);
            Compute(sampleMean, hypothesizedMean, stdError, alternate);

            power(sampleStdDev, samples);
        }


        /// <summary>
        ///   Constructs a Z test.
        /// </summary>
        /// 
        /// <param name="statistic">The test statistic, as given by (x-μ)/SE.</param>
        /// <param name="alternate">The alternate hypothesis to test.</param>
        /// 
        public ZTest(double statistic, OneSampleHypothesis alternate)
        {
            this.EstimatedValue = statistic;
            this.HypothesizedValue = 0;
            this.StandardError = 1;

            this.Compute(statistic, alternate);
        }

        /// <summary>
        ///   Computes the Z test.
        /// </summary>
        /// 
        protected void Compute(double estimatedValue, double hypothesizedValue,
            double stdError, OneSampleHypothesis alternate)
        {
            this.HypothesizedValue = hypothesizedValue;
            this.EstimatedValue = estimatedValue;
            this.StandardError = stdError;

            // Compute Z statistic
            double z = (estimatedValue - hypothesizedValue) / StandardError;

            Compute(z, alternate);
        }

        /// <summary>
        ///   Computes the Z test.
        /// </summary>
        /// 
        protected void Compute(double statistic, OneSampleHypothesis alternate)
        {
            this.StatisticDistribution = NormalDistribution.Standard;
            this.Statistic = statistic;
            this.Hypothesis = alternate;
            this.Tail = (DistributionTail)alternate;
            this.PValue = StatisticToPValue(Statistic);

            this.OnSizeChanged();
        }

        /// <summary>
        ///   Constructs a T-Test.
        /// </summary>
        /// 
        protected ZTest() { }

        private void power(double stdDev, int samples)
        {
            this.powerAnalysis = new ZTestPowerAnalysis(Hypothesis)
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
            return PValueToStatistic(p, Tail);
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
            return StatisticToPValue(x, Tail);
        }

        /// <summary>
        ///   Converts a given test statistic to a p-value.
        /// </summary>
        /// 
        /// <param name="z">The value of the test statistic.</param>
        /// <param name="type">The tail of the test distribution.</param>
        /// 
        /// <returns>The p-value for the given statistic.</returns>
        /// 
        public static double StatisticToPValue(double z, DistributionTail type)
        {
            double p;
            switch (type)
            {
                case DistributionTail.TwoTail:
                    p = 2.0 * NormalDistribution.Standard.ComplementaryDistributionFunction(Math.Abs(z));
                    break;

                case DistributionTail.OneUpper:
                    p = NormalDistribution.Standard.ComplementaryDistributionFunction(z);
                    break;

                case DistributionTail.OneLower:
                    p = NormalDistribution.Standard.DistributionFunction(z);
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
        /// 
        /// <returns>The test statistic which would generate the given p-value.</returns>
        /// 
        public static double PValueToStatistic(double p, DistributionTail type)
        {
            double z;
            switch (type)
            {
                case DistributionTail.OneLower:
                    z = NormalDistribution.Standard.InverseDistributionFunction(p);
                    break;
                case DistributionTail.OneUpper:
                    z = NormalDistribution.Standard.InverseDistributionFunction(1.0 - p);
                    break;
                case DistributionTail.TwoTail:
                    z = NormalDistribution.Standard.InverseDistributionFunction(1.0 - p / 2.0);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return z;
        }


    }
}
