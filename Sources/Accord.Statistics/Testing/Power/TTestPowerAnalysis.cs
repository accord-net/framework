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
    ///   Power analysis for one-sample T-Tests.
    /// </summary>
    /// 
    [Serializable]
    public class TTestPowerAnalysis : BaseOneSamplePowerAnalysis
    {

        /// <summary>
        ///   Creates a new <see cref="TTestPowerAnalysis"/>.
        /// </summary>
        /// 
        /// <param name="hypothesis">The hypothesis tested.</param>
        /// 
        public TTestPowerAnalysis(OneSampleHypothesis hypothesis)
            : base((DistributionTail)hypothesis)
        {
        }

        /// <summary>
        ///   Creates a new <see cref="TTestPowerAnalysis"/>.
        /// </summary>
        /// 
        /// <param name="test">The test to create the analysis for.</param>
        /// 
        public TTestPowerAnalysis(TwoSampleTTest test)
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
            double delta = Effect * Math.Sqrt(Samples);
            double df = (Samples - 1);

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
        /// <param name="hypothesis">The alternative hypothesis (research hypothesis) to be tested.</param>
        /// 
        /// <returns>The required number of samples.</returns>
        /// 
        public static TTestPowerAnalysis GetSampleSize(double delta,
            double standardDeviation = 1, double power = 0.8, double alpha = 0.05,
            OneSampleHypothesis hypothesis = OneSampleHypothesis.ValueIsDifferentFromHypothesis)
        {
            TTestPowerAnalysis analysis = new TTestPowerAnalysis(hypothesis)
            {
                Effect = delta / standardDeviation,
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
        public static TTestPowerAnalysis GetEffectSize(int sampleSize, double power = 0.8, double alpha = 0.05,
            OneSampleHypothesis hypothesis = OneSampleHypothesis.ValueIsDifferentFromHypothesis)
        {
            TTestPowerAnalysis analysis = new TTestPowerAnalysis(hypothesis)
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
