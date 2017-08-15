// Accord Statistics Library
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

namespace Accord.Statistics.Testing.Power
{
    using System;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Compat;

    /// <summary>
    ///   Power analysis for two-sample T-Tests.
    /// </summary>
    /// 
    /// <example>
    /// <para>
    ///   There are different ways a power analysis test can be conducted.</para>
    ///   
    /// <code>
    /// // Let's say we have two samples, and we would like to know whether those
    /// // samples have the same mean. For this, we can perform a two sample T-Test:
    /// double[] A = { 5.0, 6.0, 7.9, 6.95, 5.3, 10.0, 7.48, 9.4, 7.6, 8.0, 6.22 };
    /// double[] B = { 5.0, 1.6, 5.75, 5.80, 2.9, 8.88, 4.56, 2.4, 5.0, 10.0 };
    /// 
    /// // Perform the test, assuming the samples have unequal variances
    /// var test = new TwoSampleTTest(A, B, assumeEqualVariances: false);
    /// 
    /// double df = test.DegreesOfFreedom;   // d.f. = 14.351
    /// double t = test.Statistic;           // t    = 2.14
    /// double p = test.PValue;              // p    = 0.04999
    /// bool significant = test.Significant; // true
    /// 
    /// // The test gave us an indication that the samples may
    /// // indeed have come from different distributions (whose
    /// // mean value is actually distinct from each other).
    /// 
    /// // Now, we would like to perform an _a posteriori_ analysis of the 
    /// // test. When doing an a posteriori analysis, we can not change some
    /// // characteristics of the test (because it has been already done), 
    /// // but we can measure some important features that may indicate 
    /// // whether the test is trustworthy or not.
    ///
    /// // One of the first things would be to check for the test's power.
    /// // A test's power is 1 minus the probability of rejecting the null
    /// // hypothesis when the null hypothesis is actually false. It is
    /// // the other side of the coin when we consider that the P-value
    /// // is the probability of rejecting the null hypothesis when the
    /// // null hypothesis is actually true.
    /// 
    /// // Ideally, this should be a high value:
    /// double power = test.Analysis.Power; // 0.5376260
    /// 
    /// // Check how much effect we are trying to detect
    /// double effect = test.Analysis.Effect; // 0.94566
    /// 
    /// // With this power, that is the minimal difference we can spot?
    /// double sigma = Math.Sqrt(test.Variance);
    /// double thres = test.Analysis.Effect * sigma; // 2.0700909090909
    /// 
    /// // This means that, using our test, the smallest difference that
    /// // we could detect with some confidence would be something around
    /// // 2 standard deviations. If we would like to say the samples are
    /// // different when they are less than 2 std. dev. apart, we would
    /// // need to do repeat our experiment differently.
    /// </code>
    /// 
    /// <para>
    ///   Another way to create the power analysis is to pass the
    ///   hypothesis test to the t-test power analysis constructor.</para>
    ///   
    /// <code>
    /// // Create an a posteriori analysis of the experiment
    /// var analysis = new TwoSampleTTestPowerAnalysis(test);
    /// 
    /// // When creating a power analysis, we have three things we can
    /// // change. We can always freely configure two of those things
    /// // and then ask the analysis to give us the third.
    /// 
    /// // Those are:
    /// double e = analysis.Effect;       // the test's minimum detectable effect size (0.94566)
    /// double n = analysis.TotalSamples; // the number of samples in the test (21 or (11 + 10))
    /// double b = analysis.Power;        // the probability of committing a type-2 error (0.53)
    /// 
    /// // Let's say we would like to create a test with 80% power.
    /// analysis.Power = 0.8;
    /// analysis.ComputeEffect(); // what effect could we detect?
    /// 
    /// double detectableEffect = analysis.Effect; // we would detect a difference of 1.290514
    /// </code>
    /// 
    /// <para>
    ///   However, to achieve this 80%, we would need to redo our experiment
    ///   more carefully. Assuming we are going to redo our experiment, we will
    ///   have more freedom about what we can change and what we can not. For 
    ///   better addressing those points, we will create an a priori analysis 
    ///   of the experiment:</para>
    ///   
    /// <code>
    /// // We would like to know how many samples we would need to gather in
    /// // order to achieve a 80% power test which can detect an effect size
    /// // of one standard deviation:
    /// //
    /// analysis = TwoSampleTTestPowerAnalysis.GetSampleSize
    /// (
    ///     variance1: A.Variance(),
    ///     variance2: B.Variance(),     
    ///     delta: 1.0, // the minimum detectable difference we want
    ///     power: 0.8  // the test power that we want
    /// );
    /// 
    /// // How many samples would we need in order to see the effect we need?
    /// int n1 = (int)Math.Ceiling(analysis.Samples1); // 77
    /// int n2 = (int)Math.Ceiling(analysis.Samples2); // 77
    /// 
    /// // According to our power analysis, we would need at least 77 
    /// // observations in each sample in order to see the effect we
    /// // need with the required 80% power.
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="IPowerAnalysis"/>
    /// <seealso cref="TTestPowerAnalysis"/>
    /// <seealso cref="TwoSampleZTestPowerAnalysis"/>
    /// 
    [Serializable]
    public class TwoSampleTTestPowerAnalysis : BaseTwoSamplePowerAnalysis
    {

        /// <summary>
        ///   Creates a new <see cref="TTestPowerAnalysis"/>.
        /// </summary>
        /// 
        /// <param name="hypothesis">The hypothesis tested.</param>
        /// 
        public TwoSampleTTestPowerAnalysis(TwoSampleHypothesis hypothesis)
            : base((DistributionTail)hypothesis) { }

        /// <summary>
        ///   Creates a new <see cref="TTestPowerAnalysis"/>.
        /// </summary>
        /// 
        /// <param name="test">The test to create the analysis for.</param>
        /// 
        public TwoSampleTTestPowerAnalysis(TwoSampleTTest test)
            : base(test.Tail)
        {
            this.Power = test.Analysis.Power;
            this.Size = test.Analysis.Size;
            this.Effect = test.Analysis.Effect;
            this.Samples1 = test.Analysis.Samples1;
            this.Samples2 = test.Analysis.Samples2;
        }

        /// <summary>
        ///  Computes the power for a test with givens values of
        ///  <see cref="IPowerAnalysis.Effect">effect size</see> and <see cref="IPowerAnalysis.Samples">
        ///  number of samples</see> under <see cref="IPowerAnalysis.Size"/>.
        /// </summary>
        /// 
        /// <returns>
        ///  The power for the test under the given conditions.
        /// </returns>
        /// 
        public override void ComputePower()
        {
            double delta = Effect / Math.Sqrt(1.0 / Samples1 + 1.0 / Samples2);
            double df = Samples1 + Samples2 - 2;

            TDistribution td = new TDistribution(df);
            NoncentralTDistribution nt = new NoncentralTDistribution(df, delta);

            switch (Tail)
            {
                case DistributionTail.TwoTail:
                    {
                        double Ta = td.InverseDistributionFunction(1.0 - Size / 2);
                        double pa = nt.ComplementaryDistributionFunction(+Ta);
                        double pb = nt.DistributionFunction(-Ta);
                        Power = pa + pb;
                        break;
                    }

                case DistributionTail.OneLower:
                    {
                        double Ta = td.InverseDistributionFunction(Size);
                        Power = nt.DistributionFunction(Ta);
                        break;
                    }

                case DistributionTail.OneUpper:
                    {
                        double Ta = td.InverseDistributionFunction(1.0 - Size);
                        Power = nt.ComplementaryDistributionFunction(Ta);
                        break;
                    }

                default:
                    throw new InvalidOperationException();
            }
        }


        /// <summary>
        ///   Estimates the number of samples necessary to attain the
        ///   required power level for the given effect size.
        /// </summary>
        /// 
        /// <param name="delta">The minimum detectable difference.</param>
        /// <param name="standardDeviation">The difference standard deviation.</param>
        /// <param name="power">The desired power level. Default is 0.8.</param>
        /// <param name="alpha">The desired significance level. Default is 0.05.</param>
        /// <param name="proportion">The proportion of observations in the second group
        /// when compared to the first group. A proportion of 2:1 results in twice more
        /// samples in the second group than in the first. Default is 1.</param>
        /// <param name="hypothesis">The alternative hypothesis (research hypothesis) to be tested.</param>
        /// 
        /// <returns>The required number of samples.</returns>
        /// 
        public static TwoSampleTTestPowerAnalysis GetSampleSize(double delta,
            double standardDeviation = 1, double proportion = 1.0, double power = 0.8, double alpha = 0.05,
            TwoSampleHypothesis hypothesis = TwoSampleHypothesis.ValuesAreDifferent)
        {
            var analysis = new TwoSampleTTestPowerAnalysis(hypothesis)
            {
                Effect = delta / standardDeviation,
                Size = alpha,
                Power = power,
            };

            analysis.ComputeSamples(proportion);

            return analysis;
        }

        /// <summary>
        ///   Estimates the number of samples necessary to attain the
        ///   required power level for the given effect size.
        /// </summary>
        /// 
        /// <param name="delta">The minimum detectable difference.</param>
        /// <param name="variance1">The first sample variance.</param>
        /// <param name="variance2">The second sample variance.</param>
        /// <param name="power">The desired power level. Default is 0.8.</param>
        /// <param name="alpha">The desired significance level. Default is 0.05.</param>
        /// <param name="proportion">The proportion of observations in the second group
        /// when compared to the first group. A proportion of 2:1 results in twice more
        /// samples in the second group than in the first. Default is 1.</param>
        /// <param name="hypothesis">The alternative hypothesis (research hypothesis) to be tested.</param>
        /// 
        /// <returns>The required number of samples.</returns>
        /// 
        public static TwoSampleTTestPowerAnalysis GetSampleSize(double delta,
            double variance1, double variance2, double proportion = 1.0, double power = 0.8,
            double alpha = 0.05, TwoSampleHypothesis hypothesis = TwoSampleHypothesis.ValuesAreDifferent)
        {
            double standardDeviation = Math.Sqrt((variance1 + variance2) / 2.0);

            var analysis = new TwoSampleTTestPowerAnalysis(hypothesis)
            {
                Effect = delta / standardDeviation,
                Size = alpha,
                Power = power,
            };

            analysis.ComputeSamples(proportion);

            return analysis;
        }

    }
}
