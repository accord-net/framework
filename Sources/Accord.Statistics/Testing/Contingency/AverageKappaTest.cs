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
    ///   Kappa Test for multiple contingency tables.
    /// </summary>
    ///
    /// <remarks>
    ///   <para>
    ///   The multiple-matrix Kappa test tries to assert whether the Kappa measure 
    ///   of many contingency tables, each of which created by a different rater
    ///   or classification model, differs significantly. The computations are
    ///   based on the pages 607, 608 of  (Fleiss, 2003).</para>
    ///   
    /// <para>
    ///   This is a <see cref="ChiSquareTest">Chi-square kind of test</see>.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>J. L. Fleiss. Statistical methods for rates and proportions.
    ///     Wiley-Interscience; 3rd edition (September 5, 2003) </description></item>
    ///   </list></para>
    /// </remarks>
    ///
    [Serializable]
    public class AverageKappaTest : ChiSquareTest
    {

        /// <summary>
        ///   Gets the overall Kappa value
        ///   for the many contingency tables.
        /// </summary>
        /// 
        public double OverallKappa { get; private set; }

        /// <summary>
        ///   Gets the overall Kappa variance
        ///   for the many contingency tables.
        /// </summary>
        /// 
        public double OverallVariance { get; private set; }

        /// <summary>
        ///   Gets the variance for each kappa value.
        /// </summary>
        /// 
        public double[] Variances { get; private set; }

        /// <summary>
        ///   Gets the kappa for each contingency table.
        /// </summary>
        /// 
        public double[] EstimatedValues { get; private set; }



        /// <summary>
        ///   Creates a new multiple table Kappa test.
        /// </summary>
        /// 
        /// <param name="kappas">The kappa values.</param>
        /// <param name="variances">The variance for each kappa value.</param>
        /// 
        public AverageKappaTest(double[] kappas, double[] variances)
        {
            if (kappas.Length != variances.Length)
                throw new DimensionMismatchException("variances", 
                    "The number of variance estimates and kappa estimates must match.");

            this.Compute(kappas, variances);
        }



        /// <summary>
        ///   Creates a new multiple table Kappa test.
        /// </summary>
        /// 
        /// <param name="matrices">The contingency tables.</param>
        /// 
        public AverageKappaTest(GeneralConfusionMatrix[] matrices)
        {

            double[] kappas = new double[matrices.Length];
            double[] variances = new double[matrices.Length];

            for (int i = 0; i < matrices.Length; i++)
            {
                kappas[i] = matrices[i].Kappa;
                variances[i] = matrices[i].Variance;
            }

            this.Compute(kappas, variances);
        }

        /// <summary>
        ///   Computes the multiple matrix Kappa test.
        /// </summary>
        /// 
        protected void Compute(double[] kappas, double[] variances)
        {
            this.EstimatedValues = kappas;
            this.Variances = variances;

            double num = 0;
            for (int i = 0; i < kappas.Length; i++)
                num += kappas[i] / variances[i];

            double den = 0;
            for (int i = 0; i < variances.Length; i++)
                den += 1.0 / variances[i];

            this.OverallKappa = num / den;
            this.OverallVariance = den;

            double chiSquare = 0;
            for (int i = 0; i < kappas.Length; i++)
                chiSquare += Math.Pow(kappas[i] - OverallKappa, 2) / variances[i];

            int df = kappas.Length - 1;

            base.Compute(chiSquare, df);
        }

    }
}
