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
    ///   Kappa Test for two contingency tables.
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    ///   The two-matrix Kappa test tries to assert whether the Kappa measure 
    ///   of two contingency tables, each of which created by a different rater
    ///   or classification model, differs significantly. </para>
    ///   
    /// <para>
    ///   This is a <see cref="TwoSampleZTest">two sample z-test kind of test</see>.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>J. L. Fleiss. Statistical methods for rates and proportions.
    ///     Wiley-Interscience; 3rd edition (September 5, 2003) </description></item>
    ///     <item><description>
    ///     Ientilucci, Emmett (2006). "On Using and Computing the Kappa Statistic".
    ///     Available on: http://www.cis.rit.edu/~ejipci/Reports/On_Using_and_Computing_the_Kappa_Statistic.pdf </description></item>
    ///   </list></para>
    /// </remarks>
    ///
    [Serializable]
    public class TwoMatrixKappaTest : TwoSampleZTest
    {


        /// <summary>
        ///   Gets the summed Kappa variance
        ///   for the two contingency tables.
        /// </summary>
        /// 
        public double OverallVariance { get; private set; }

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
        ///   Creates a new Two-Table Kappa test.
        /// </summary>
        /// 
        /// <param name="kappa1">The kappa value for the first contingency table to test.</param>
        /// <param name="kappa2">The kappa value for the second contingency table to test.</param>
        /// <param name="var1">The variance of the kappa value for the first contingency table to test.</param>
        /// <param name="var2">The variance of the kappa value for the second contingency table to test.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// <param name="hypothesizedDifference">The hypothesized difference between the two Kappa values.</param>
        /// 
        public TwoMatrixKappaTest(double kappa1, double var1, double kappa2, double var2, double hypothesizedDifference = 0,
            TwoSampleHypothesis alternate = TwoSampleHypothesis.ValuesAreDifferent)
        {
            this.EstimatedValue1 = kappa1;
            this.EstimatedValue2 = kappa2;

            this.Variance1 = var1;
            this.Variance2 = var2;

            this.OverallVariance = Variance1 + Variance2;

            double diff = Math.Abs(EstimatedValue1 - EstimatedValue2);
            double stdError = Math.Sqrt(OverallVariance);

            Compute(diff, hypothesizedDifference, stdError, alternate);
        }

        /// <summary>
        ///   Creates a new Two-Table Kappa test.
        /// </summary>
        /// 
        /// <param name="matrix1">The first contingency table to test.</param>
        /// <param name="matrix2">The second contingency table to test.</param>
        /// <param name="hypothesizedDifference">The hypothesized difference between the two Kappa values.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public TwoMatrixKappaTest(GeneralConfusionMatrix matrix1, GeneralConfusionMatrix matrix2, double hypothesizedDifference = 0,
            TwoSampleHypothesis alternate = TwoSampleHypothesis.ValuesAreDifferent)
        {
            this.EstimatedValue1 = matrix1.Kappa;
            this.EstimatedValue2 = matrix2.Kappa;

            this.Variance1 = matrix1.Variance;
            this.Variance2 = matrix2.Variance;

            this.OverallVariance = Variance1 + Variance2;

            double diff = Math.Abs(EstimatedValue1 - EstimatedValue2);
            double stdError = Math.Sqrt(OverallVariance);

            Compute(diff, hypothesizedDifference, stdError, alternate);
        }


    }
}
