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
    ///   Two sample Z-Test.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Z-test">
    ///        Wikipedia, The Free Encyclopedia. Z-Test. Available on:
    ///        http://en.wikipedia.org/wiki/Z-test </a></description></item>
    ///     </list></para>
    /// </remarks>
    /// 
    [Serializable]
    public class TwoSampleZTest : HypothesisTest<NormalDistribution>
    {

        private TwoSampleZTestPowerAnalysis powerAnalysis;

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
        ///   Constructs a Z test.
        /// </summary>
        /// 
        /// <param name="sample1">The first data sample.</param>
        /// <param name="sample2">The second data sample.</param>
        /// <param name="hypothesizedDifference">The hypothesized sample difference.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public TwoSampleZTest(double[] sample1, double[] sample2, double hypothesizedDifference = 0,
            TwoSampleHypothesis alternate = TwoSampleHypothesis.ValuesAreDifferent)
        {
            int samples1 = sample1.Length;
            int samples2 = sample2.Length;

            if (samples1 < 30 || samples2 < 30)
            {
                System.Diagnostics.Trace.TraceWarning(
                    "Warning: running a Z test for less than 30 samples. Consider running a Student's T Test instead.");
            }

            double mean1 = Tools.Mean(sample1);
            double mean2 = Tools.Mean(sample2);

            double var1 = Tools.Variance(sample1, mean1);
            double var2 = Tools.Variance(sample2, mean2);

            double sqStdError1 = var1 / sample1.Length;
            double sqStdError2 = var2 / sample2.Length;

            this.Compute(mean1, mean2, sqStdError1, sqStdError2, hypothesizedDifference, alternate);

            this.power(var1, var2, sample1.Length, sample2.Length);
        }



        /// <summary>
        ///   Constructs a Z test.
        /// </summary>
        /// 
        /// <param name="mean1">The first sample's mean.</param>
        /// <param name="mean2">The second sample's mean.</param>
        /// <param name="var1">The first sample's variance.</param>
        /// <param name="var2">The second sample's variance.</param>
        /// <param name="samples1">The number of observations in the first sample.</param>
        /// <param name="samples2">The number of observations in the second sample.</param>
        /// <param name="hypothesizedDifference">The hypothesized sample difference.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public TwoSampleZTest(
            double mean1, double var1, int samples1,
            double mean2, double var2, int samples2,
            double hypothesizedDifference = 0,
            TwoSampleHypothesis alternate = TwoSampleHypothesis.ValuesAreDifferent)
        {

            if (samples1 < 30 || samples2 < 30)
            {
                System.Diagnostics.Trace.TraceWarning(
                    "Warning: running a Z test for less than 30 samples. Consider running a Student's T Test instead.");
            }

            double sqStdError1 = var1 / samples1;
            double sqStdError2 = var2 / samples2;

            this.Compute(mean1, mean2, sqStdError1, sqStdError2, hypothesizedDifference, alternate);

            this.power(var1, var2, samples1, samples2);
        }


        /// <summary>
        ///   Constructs a Z test.
        /// </summary>
        /// 
        protected TwoSampleZTest()
        {
        }

        /// <summary>
        ///   Computes the Z test.
        /// </summary>
        ///
        protected void Compute(double value1, double value2,
            double squareStdError1, double squareStdError2,
            double hypothesizedDifference, TwoSampleHypothesis alternate)
        {
            this.EstimatedValue1 = value1;
            this.EstimatedValue2 = value2;

            double diff = value1 - value2;
            double stdError = Math.Sqrt(squareStdError1 + squareStdError2);

            Compute(diff, hypothesizedDifference, stdError, alternate);
        }

        /// <summary>
        ///   Computes the Z test.
        /// </summary>
        ///
        protected void Compute(double observedDifference, double hypothesizedDifference,
            double standardError, TwoSampleHypothesis alternate)
        {
            this.ObservedDifference = observedDifference;
            this.HypothesizedDifference = hypothesizedDifference;
            this.StandardError = standardError;

            // Compute Z statistic
            double z = (ObservedDifference - HypothesizedDifference) / StandardError;

            Compute(z, alternate);
        }

        /// <summary>
        ///   Computes the Z test.
        /// </summary>
        ///
        protected void Compute(double statistic, TwoSampleHypothesis alternate)
        {
            this.Statistic = statistic;
            this.StatisticDistribution = NormalDistribution.Standard;

            this.Hypothesis = alternate;
            this.Tail = (DistributionTail)alternate;

            this.PValue = StatisticToPValue(Statistic);

            this.OnSizeChanged();
        }


        private void power(double var1, double var2, int n1, int n2)
        {
            double stdDev = Math.Sqrt(var1 + var2);

            powerAnalysis = new TwoSampleZTestPowerAnalysis(Hypothesis)
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
            return ZTest.PValueToStatistic(p, Tail);
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
            return ZTest.StatisticToPValue(x, Tail);
        }

    }
}
