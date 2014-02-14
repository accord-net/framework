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
    using Accord.Math;
    using Accord.Statistics.Analysis;

    /// <summary>
    ///   Stuart-Maxwell test of homogeneity for <c>K x K</c> contingency tables.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Stuart-Maxwell test is a generalization of <see cref="McNemarTest">
    ///   McNemar's test</see> for multiple categories. </para>
    ///   
    /// <para>
    ///   This is a <see cref="ChiSquareTest">Chi-square kind of test</see>.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Uebersax, John (2006). "McNemar Tests of Marginal Homogeneity".
    ///       Available on: http://www.john-uebersax.com/stat/mcnemar.htm </description></item>
    ///     <item><description>
    ///       Sun, Xuezheng; Yang, Zhao (2008). "Generalized McNemar's Test for Homogeneity of the Marginal
    ///       Distributions". Available on: http://www2.sas.com/proceedings/forum2008/382-2008.pdf  </description></item>
    ///    </list></para>
    /// </remarks>
    /// 
    [Serializable]
    public class StuartMaxwellTest : ChiSquareTest
    {

        double[] d;
        double[,] S;
        double[,] invS;


        /// <summary>
        ///   Gets the delta vector <c>d</c> used
        ///   in the test calculations.
        /// </summary>
        /// 
        public double[] Delta
        {
            get { return d; }
        }

        /// <summary>
        ///   Gets the covariance matrix <c>S</c>
        ///   used in the test calculations.
        /// </summary>
        /// 
        public double[,] Covariance
        {
            get { return S; }
        }

        /// <summary>
        ///   Gets the inverse covariance matrix
        ///   <c>S^-1</c> used in the calculations.
        /// </summary>
        /// 
        public double[,] Precision
        {
            get { return invS; }
        }


        /// <summary>
        ///   Creates a new Stuart-Maxwell test.
        /// </summary>
        /// 
        /// <param name="matrix">The contingency table to test.</param>
        /// 
        public StuartMaxwellTest(GeneralConfusionMatrix matrix)
        {
            int classes = matrix.Classes;
            int samples = matrix.Samples;

            int df = classes - 1;

            int[] rowMarginals = matrix.RowTotals;
            int[] colMarginals = matrix.ColumnTotals;

            d = new double[df];
            for (int i = 0; i < d.Length; i++)
                d[i] = rowMarginals[i] - colMarginals[i];

            S = new double[df, df];

            for (int i = 0; i < df; i++)
            {
                for (int j = 0; j < df; j++)
                {
                    if (i == j)
                    {
                        double u = (rowMarginals[i] - colMarginals[i]);
                        double pii = matrix.Matrix[i, i];

                        S[i, i] = rowMarginals[i] + colMarginals[i] - 2.0 * pii;
                    }
                    else
                    {
                        double pij = matrix.Matrix[i, j];
                        double pji = matrix.Matrix[j, i];

                        S[i, j] = -(pij + pji);
                    }
                }
            }

            invS = S.PseudoInverse();

            double chiSquare = d.Multiply(invS).InnerProduct(d);

            Compute(chiSquare, df);
        }

    }
}
