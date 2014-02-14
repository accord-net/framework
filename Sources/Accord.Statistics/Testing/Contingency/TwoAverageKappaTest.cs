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
    using Accord.Statistics.Analysis;

    /// <summary>
    ///   Kappa test for the average of two groups of contingency tables.
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    ///   The two-matrix Kappa test tries to assert whether the Kappa measure 
    ///   of two groups of contingency tables, each group created by a different
    ///   rater or classification model and measured repeatedly, differs significantly. </para>
    ///   
    /// <para>
    ///   This is a <see cref="TwoSampleTTest">two sample t-test kind of test</see>.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>J. L. Fleiss. Statistical methods for rates and proportions.
    ///     Wiley-Interscience; 3rd edition (September 5, 2003) </description></item>
    ///    </list></para>
    /// </remarks>
    ///
    [Serializable]
    public class TwoAverageKappaTest : TwoSampleTTest
    {


        /// <summary>
        ///   Gets the variance for the first Kappa value.
        /// </summary>
        /// 
        public double Variance1 { get; private set; }

        /// <summary>
        ///   Gets the variance for the second Kappa value.
        /// </summary>
        /// 
        public double Variance2 { get; private set; }


        /// <summary>
        ///   Creates a new Two-Table Mean Kappa test.
        /// </summary>
        /// 
        /// <param name="meanKappa1">The average kappa value for the first group of contingency tables.</param>
        /// <param name="meanKappa2">The average kappa value for the second group of contingency tables.</param>
        /// <param name="varKappa1">The kappa's variance in the first group of tables.</param>
        /// <param name="varKappa2">The kappa's variance in the first group of tables.</param>
        /// <param name="kappaSamples1">The number of contingency tables averaged in the first group.</param>
        /// <param name="kappaSamples2">The number of contingency tables averaged in the second group.</param>
        /// <param name="assumeEqualVariances">True to assume equal variances, false otherwise. Default is true.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// <param name="hypothesizedDifference">The hypothesized difference between the two Kappa values.</param>
        /// 
        public TwoAverageKappaTest(double meanKappa1, double varKappa1, int kappaSamples1,
            double meanKappa2, double varKappa2, int kappaSamples2,
            double hypothesizedDifference = 0, bool assumeEqualVariances = true,
            TwoSampleHypothesis alternate = TwoSampleHypothesis.ValuesAreDifferent)
        {

            Variance1 = varKappa1;
            Variance2 = varKappa2;

            base.Compute(
                meanKappa1, varKappa1, kappaSamples1,
                meanKappa2, varKappa2, kappaSamples2,
                hypothesizedDifference, assumeEqualVariances, alternate);
        }

        /// <summary>
        ///   Creates a new Two-Table Mean Kappa test.
        /// </summary>
        /// 
        /// <param name="matrices1">The first group of contingency tables.</param>
        /// <param name="matrices2">The second  group of contingency tables.</param>
        /// <param name="assumeEqualVariances">True to assume equal variances, false otherwise. Default is true.</param>
        /// <param name="hypothesizedDifference">The hypothesized difference between the two average Kappa values.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public TwoAverageKappaTest(GeneralConfusionMatrix[] matrices1, GeneralConfusionMatrix[] matrices2,
            double hypothesizedDifference = 0, bool assumeEqualVariances = true,
            TwoSampleHypothesis alternate = TwoSampleHypothesis.ValuesAreDifferent)
        {

            double[] kappas1 = new double[matrices1.Length];
            for (int i = 0; i < matrices1.Length; i++)
                kappas1[i] = matrices1[i].Kappa;

            double[] kappas2 = new double[matrices1.Length];
            for (int i = 0; i < matrices2.Length; i++)
                kappas2[i] = matrices2[i].Kappa;

            double meanKappa1 = kappas1.Mean();
            double meanKappa2 = kappas2.Mean();

            Variance1 = kappas1.Variance(meanKappa1);
            Variance2 = kappas2.Variance(meanKappa2);

            int kappaSamples1 = matrices1.Length;
            int kappaSamples2 = matrices2.Length;

            base.Compute(
               meanKappa1, Variance1, kappaSamples1,
               meanKappa2, Variance2, kappaSamples2,
               hypothesizedDifference, assumeEqualVariances, alternate);
        }


    }
}
