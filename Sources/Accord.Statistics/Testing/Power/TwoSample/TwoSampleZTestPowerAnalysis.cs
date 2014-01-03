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

namespace Accord.Statistics.Testing.Power
{
    using System;
    using Accord.Statistics.Distributions.Univariate;

    /// <summary>
    ///   Power analysis for two-sample Z-Tests.
    /// </summary>
    /// 
    [Serializable]
    public class TwoSampleZTestPowerAnalysis : BaseTwoSamplePowerAnalysis
    {

        /// <summary>
        ///   Creates a new <see cref="ZTestPowerAnalysis"/>.
        /// </summary>
        /// 
        /// <param name="hypothesis">The hypothesis tested.</param>
        /// 
        public TwoSampleZTestPowerAnalysis(TwoSampleHypothesis hypothesis)
            : base((DistributionTail)hypothesis)
        {
        }

        /// <summary>
        ///   Creates a new <see cref="ZTestPowerAnalysis"/>.
        /// </summary>
        /// 
        /// <param name="test">The test to create the analysis for.</param>
        /// 
        public TwoSampleZTestPowerAnalysis(TwoSampleZTest test)
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
            double samples = (Samples1 * Samples2) / (double)(Samples1 + Samples2);

            double Za = ZTest.PValueToStatistic(Size, Tail);
            double Zb = Za - Math.Sqrt(samples) * Effect;

            Power = NormalDistribution.Standard.ComplementaryDistributionFunction(Zb);
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
        public override void ComputeSamples(double proportion = 1)
        {
            double Za = ZTest.PValueToStatistic(Size, Tail);
            double Zb = NormalDistribution.Standard
                .InverseDistributionFunction(Power);

            double n = (Za + Zb) / Effect;

            Samples1 = n * n;
            Samples2 = Samples1 * proportion;
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

            double n = Math.Sqrt(Samples1 + Samples2);
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
        /// <param name="proportion">The proportion of observations in the second group
        /// when compared to the first group. A proportion of 2:1 results in twice more
        /// samples in the second group than in the first. Default is 1.</param>
        /// 
        /// <returns>The required number of samples.</returns>
        /// 
        public static TwoSampleZTestPowerAnalysis GetSampleSize(double delta,
            double standardDeviation = 1, double proportion = 1.0, double power = 0.8, double alpha = 0.05,
            TwoSampleHypothesis hypothesis = TwoSampleHypothesis.ValuesAreDifferent)
        {
            var analysis = new TwoSampleZTestPowerAnalysis(hypothesis)
            {
                Effect = (delta) / standardDeviation,
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
        /// <param name="sampleSize1">The number of observations in the first sample.</param>
        /// <param name="sampleSize2">The number of observations in the second sample.</param>
        /// <param name="power">The desired power level. Default is 0.8.</param>
        /// <param name="alpha">The desired significance level. Default is 0.05.</param>
        /// <param name="hypothesis">The alternative hypothesis (research hypothesis) to be tested.</param>
        /// 
        /// <returns>The required number of samples.</returns>
        /// 
        public static TwoSampleZTestPowerAnalysis GetEffectSize(int sampleSize1, int sampleSize2,
            double power = 0.8, double alpha = 0.05,
            TwoSampleHypothesis hypothesis = TwoSampleHypothesis.ValuesAreDifferent)
        {
            var analysis = new TwoSampleZTestPowerAnalysis(hypothesis)
            {
                Samples1 = sampleSize1,
                Samples2 = sampleSize2,
                Size = alpha,
                Power = power,
            };

            analysis.ComputeEffect();

            return analysis;
        }

    }
}
