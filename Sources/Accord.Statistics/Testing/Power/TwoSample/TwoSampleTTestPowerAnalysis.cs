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
    ///   Power analysis for two-sample T-Tests.
    /// </summary>
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
