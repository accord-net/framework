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
    using Accord.Statistics.Analysis;

    /// <summary>
    ///   Two-Sample (Goodness-of-fit) Chi-Square Test (Upper Tail)
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   A chi-square test (also chi-squared or χ2  test) is any statistical
    ///   hypothesis test in which the sampling distribution of the test statistic
    ///   is a <see cref="ChiSquareDistribution">chi-square distribution</see> when
    ///   the null hypothesis is true, or any in which this is asymptotically true,
    ///   meaning that the sampling distribution (if the null hypothesis is true) 
    ///   can be made to approximate a chi-square distribution as closely as desired
    ///   by making the sample size large enough.</para>
    /// <para>
    ///   The chi-square test is used whenever one would like to test whether the
    ///   actual data differs from a random distribution. </para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Chi-square_test">
    ///        Wikipedia, The Free Encyclopedia. Chi-Square Test. Available on:
    ///        http://en.wikipedia.org/wiki/Chi-square_test </a></description></item>
    ///   
    ///     <item><description><a href="http://www2.lv.psu.edu/jxm57/irp/chisquar.html">
    ///        J. S. McLaughlin. Chi-Square Test. Available on:
    ///        http://www2.lv.psu.edu/jxm57/irp/chisquar.html </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    [Serializable]
    public class ChiSquareTest : HypothesisTest<ChiSquareDistribution>
    {


        /// <summary>
        ///   Gets the degrees of freedom for the Chi-Square distribution.
        /// </summary>
        /// 
        public int DegreesOfFreedom
        {
            get { return StatisticDistribution.DegreesOfFreedom; }
        }


        /// <summary>
        ///   Constructs a Chi-Square Test.
        /// </summary>
        /// 
        /// <param name="statistic">The test statistic.</param>
        /// <param name="degreesOfFreedom">The chi-square distribution degrees of freedom.</param>
        /// 
        public ChiSquareTest(double statistic, int degreesOfFreedom)
        {
            Compute(statistic, degreesOfFreedom);
        }


        /// <summary>
        ///   Constructs a Chi-Square Test.
        /// </summary>
        /// 
        /// <param name="expected">The expected variable values.</param>
        /// <param name="observed">The observed variable values.</param>
        /// <param name="degreesOfFreedom">The chi-square distribution degrees of freedom.</param>
        /// 
        public ChiSquareTest(double[] expected, double[] observed, int degreesOfFreedom)
        {
            if (expected == null)
                throw new ArgumentNullException("expected");

            if (observed == null)
                throw new ArgumentNullException("observed");


            // X² = sum(o - e)²
            //          -----
            //            e

            double sum = 0.0;
            for (int i = 0; i < observed.Length; i++)
            {
                double d = observed[i] - expected[i];
                sum += (d * d) / expected[i];
            }


            Compute(sum, degreesOfFreedom);
        }

        /// <summary>
        ///   Constructs a Chi-Square Test.
        /// </summary>
        /// 
        public ChiSquareTest(ConfusionMatrix matrix, bool yatesCorrection = false)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");


            int[] row = matrix.RowTotals;
            int[] col = matrix.ColumnTotals;
            int[,] values = matrix.Matrix;
            int samples = matrix.Samples;

            double chiSquare = compute(values, row, col, samples, yatesCorrection);

            Compute(chiSquare, degreesOfFreedom: 1);
        }

        /// <summary>
        ///   Constructs a Chi-Square Test.
        /// </summary>
        /// 
        public ChiSquareTest(GeneralConfusionMatrix matrix, bool yatesCorrection = false)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");


            int df = (matrix.Classes - 1) * (matrix.Classes - 1);

            int[] row = matrix.RowTotals;
            int[] col = matrix.ColumnTotals;
            int[,] values = matrix.Matrix;
            int samples = matrix.Samples;

            double chiSquare = compute(values, row, col, samples, yatesCorrection);

            Compute(chiSquare, degreesOfFreedom: df);
        }



        /// <summary>
        ///   Constructs a Chi-Square Test.
        /// </summary>
        protected ChiSquareTest() { }

        /// <summary>
        ///   Computes the Chi-Square Test.
        /// </summary>
        /// 
        protected void Compute(double statistic, int degreesOfFreedom)
        {
            this.Statistic = statistic;
            this.StatisticDistribution = new ChiSquareDistribution(degreesOfFreedom);

            this.Tail = DistributionTail.OneUpper;
            this.PValue = StatisticDistribution.ComplementaryDistributionFunction(Statistic);
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
            return StatisticDistribution.ComplementaryDistributionFunction(x);
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
            throw new NotSupportedException();
        }







        private static double compute(int[,] values, int[] row, int[] col, int samples, bool yatesCorrection)
        {
            // X² = sum(o - e)²
            //          -----
            //            e

            double x = 0;

            for (int i = 0; i < row.Length; i++)
            {
                for (int j = 0; j < col.Length; j++)
                {
                    double e = (row[i] * col[j]) / (double)samples;
                    double o = values[i, j];

                    double u = o - e;

                    if (yatesCorrection)
                        u = Math.Abs(u) - 0.5;

                    x += (u * u) / e;
                }
            }

            return x;
        }
    }
}
