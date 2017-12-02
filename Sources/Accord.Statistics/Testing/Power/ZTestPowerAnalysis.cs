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

namespace Accord.Statistics.Testing.Power
{
    using System;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Compat;

    /// <summary>
    ///   Power analysis for one-sample Z-Tests.
    /// </summary>
    /// 
    /// <example>
    /// // When creating a power analysis, we have three things we can
    /// // change. We can always freely configure two of those things
    /// // and then ask the analysis to give us the third.
    /// 
    /// var analysis = new ZTestPowerAnalysis(OneSampleHypothesis.ValueIsDifferentFromHypothesis);
    /// 
    /// // Those are:
    /// double e = analysis.Effect;   // the test's minimum detectable effect size
    /// double n = analysis.Samples;  // the number of samples in the test
    /// double p = analysis.Power;    // the probability of committing a type-2 error
    /// 
    /// // Let's set the desired effect size and the 
    /// // number of samples so we can get the power
    /// 
    /// analysis.Effect = 0.2; // we would like to detect at least 0.2 std. dev. apart
    /// analysis.Samples = 60; // we would like to use at most 60 samples
    /// analysis.ComputePower(); // what will be the power of this test?
    /// 
    /// double power = analysis.Power; // The power is going to be 0.34 (or 34%)
    /// 
    /// // Let's set the desired power and the number 
    /// // of samples so we can get the effect size
    /// 
    /// analysis.Power = 0.8;  // we would like to create a test with 80% power
    /// analysis.Samples = 60; // we would like to use at most 60 samples
    /// analysis.ComputeEffect(); // what would be the minimum effect size we can detect?
    /// 
    /// double effect = analysis.Effect; // The effect will be 0.36 standard deviations.
    /// 
    /// // Let's set the desired power and the effect
    /// // size so we can get the number of samples
    /// 
    /// analysis.Power = 0.8;  // we would like to create a test with 80% power
    /// analysis.Effect = 0.2; // we would like to detect at least 0.2 std. dev. apart
    /// analysis.ComputeSamples();
    /// 
    /// double samples = analysis.Samples; // We would need around 197 samples.
    /// </example>
    /// 
    /// <seealso cref="ZTest"/>
    /// <seealso cref="TwoSampleZTestPowerAnalysis"/>
    /// <seealso cref="TTestPowerAnalysis"/>
    /// 
    [Serializable]
    public class ZTestPowerAnalysis : BaseOneSamplePowerAnalysis
    {

        /// <summary>
        ///   Creates a new <see cref="ZTestPowerAnalysis"/>.
        /// </summary>
        /// 
        /// <param name="hypothesis">The hypothesis tested.</param>
        /// 
        public ZTestPowerAnalysis(OneSampleHypothesis hypothesis)
            : base((DistributionTail)hypothesis)
        {
        }

        /// <summary>
        ///   Creates a new <see cref="ZTestPowerAnalysis"/>.
        /// </summary>
        /// 
        /// <param name="test">The test to create the analysis for.</param>
        /// 
        public ZTestPowerAnalysis(TwoSampleZTest test)
            : base(test.Tail)
        {
            this.Power = test.Analysis.Power;
            this.Size = test.Analysis.Size;
            this.Effect = test.Analysis.Effect;
            this.Samples = test.Analysis.Samples;
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
            switch (Tail)
            {
                case DistributionTail.TwoTail:
                    {
                        double Za = NormalDistribution.Standard.InverseDistributionFunction(1.0 - Size / 2.0);
                        double Zb = Za - Math.Sqrt(Samples) * Effect;

                        Power = NormalDistribution.Standard.ComplementaryDistributionFunction(Zb);
                        break;
                    }

                case DistributionTail.OneLower:
                    {
                        double Za = NormalDistribution.Standard.InverseDistributionFunction(Size);
                        double Zb = Za - Math.Sqrt(Samples) * Effect;

                        double beta = NormalDistribution.Standard.ComplementaryDistributionFunction(Zb);
                        Power = 1.0 - beta;
                        break;
                    }

                case DistributionTail.OneUpper:
                    {
                        double Za = NormalDistribution.Standard.InverseDistributionFunction(1.0 - Size);
                        double Zb = Za - Math.Sqrt(Samples) * Effect;

                        Power = NormalDistribution.Standard.ComplementaryDistributionFunction(Zb);
                        break;
                    }

                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        ///   Gets the recommended sample size for the test to attain
        ///   the power indicating in <see cref="IPowerAnalysis.Power"/> considering
        ///   values of <see cref="IPowerAnalysis.Effect"/> and <see cref="IPowerAnalysis.Size"/>.
        /// </summary>
        /// 
        /// <returns>
        ///   Recommended sample size for attaining the given
        ///   <see cref="IPowerAnalysis.Power"/> for size effect <see cref="IPowerAnalysis.Effect"/>
        ///   under the given <see cref="IPowerAnalysis.Size"/>.
        /// </returns>
        /// 
        public override void ComputeSamples()
        {
            double Za = ZTest.PValueToStatistic(Size, Tail);
            double Zb = NormalDistribution.Standard
                .InverseDistributionFunction(Power);

            double n = (Za + Zb) / Effect;

            Samples = n * n;
        }

        /// <summary>
        ///   Computes the minimum detectable effect size for the test
        ///   considering the power given in <see cref="IPowerAnalysis.Power"/>, the
        ///   number of samples in <see cref="IPowerAnalysis.Samples"/> and the significance
        ///   level <see cref="IPowerAnalysis.Size"/>.
        /// </summary>
        /// 
        /// <returns>
        ///   The minimum detectable <see cref="IPowerAnalysis.Effect">effect
        ///   size</see> for the test under the given conditions.
        /// </returns>
        /// 
        public override void ComputeEffect()
        {
            double Za = ZTest.PValueToStatistic(Size, Tail);
            double Zb = NormalDistribution.Standard
                    .InverseDistributionFunction(Power);

            double n = Math.Sqrt(Samples);
            double d = (Za + Zb) / n;

            Effect = d;
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
        /// <param name="hypothesis">The alternative hypothesis (research hypothesis) to be tested.</param>
        /// 
        /// <returns>The required number of samples.</returns>
        /// 
        public static ZTestPowerAnalysis GetSampleSize(double delta,
            double standardDeviation = 1, double power = 0.8, double alpha = 0.05,
           OneSampleHypothesis hypothesis = OneSampleHypothesis.ValueIsDifferentFromHypothesis)
        {
            ZTestPowerAnalysis analysis = new ZTestPowerAnalysis(hypothesis)
            {
                Effect = (delta) / standardDeviation,
                Size = alpha,
                Power = power,
            };

            analysis.ComputeSamples();

            return analysis;
        }

        /// <summary>
        ///   Estimates the number of samples necessary to attain the
        ///   required power level for the given effect size.
        /// </summary>
        /// 
        /// <param name="sampleSize">The number of observations in the sample.</param>
        /// <param name="power">The desired power level. Default is 0.8.</param>
        /// <param name="alpha">The desired significance level. Default is 0.05.</param>
        /// <param name="hypothesis">The alternative hypothesis (research hypothesis) to be tested.</param>
        /// 
        /// <returns>The required number of samples.</returns>
        /// 
        public static ZTestPowerAnalysis GetEffectSize(int sampleSize, double power = 0.8, double alpha = 0.05,
            OneSampleHypothesis hypothesis = OneSampleHypothesis.ValueIsDifferentFromHypothesis)
        {
            var analysis = new ZTestPowerAnalysis(hypothesis)
            {
                Samples = sampleSize,
                Size = alpha,
                Power = power,
            };

            analysis.ComputeEffect();

            return analysis;
        }
    }
}
