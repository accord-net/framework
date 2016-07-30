// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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


namespace Accord.Math.Optimization.Losses
{
    using System;
    using Accord.Math;
    using Accord.Statistics;

    /// <summary>
    ///   R² (r-squared) loss.
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>
    ///    The coefficient of determination is used in the context of statistical models
    ///    whose main purpose is the prediction of future outcomes on the basis of other
    ///    related information. It is the proportion of variability in a data set that
    ///    is accounted for by the statistical model. It provides a measure of how well
    ///    future outcomes are likely to be predicted by the model.</para>
    ///   <para>
    ///    The R² coefficient of determination is a statistical measure of how well the
    ///    regression line approximates the real data points. An R² of 1.0 indicates
    ///    that the regression line perfectly fits the data.</para> 
    /// </remarks>
    /// 
    [Serializable]
    public class RSquaredLoss : LossBase<double[][], double[]>, ILoss<double[]>
    {

        /// <summary>
        ///   Gets or sets the number of variables being fit in the problem.
        /// </summary>
        /// 
        public int NumberOfInputs { get; set; }

        /// <summary>
        ///   Gets whether the adjusted version of the R² 
        ///   measure should be computed instead.
        /// </summary>
        /// 
        public bool Adjust { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RSquaredLoss"/> class.
        /// </summary>
        /// 
        /// <param name="expected">The expected outputs (ground truth).</param>
        /// <param name="numberOfInputs">The number if variables being fit.</param>
        /// 
        public RSquaredLoss(int numberOfInputs, double[] expected)
            : base(Jagged.ColumnVector(expected))
        {
            NumberOfInputs = numberOfInputs;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RSquaredLoss"/> class.
        /// </summary>
        /// 
        /// <param name="expected">The expected outputs (ground truth).</param>
        /// <param name="numberOfInputs">The number if variables being fit.</param>
        /// 
        public RSquaredLoss(int numberOfInputs, double[][] expected)
            : base(expected)
        {
            NumberOfInputs = numberOfInputs;
        }

        /// <summary>
        /// Computes the loss between the expected values (ground truth)
        /// and the given actual values that have been predicted.
        /// </summary>
        /// <param name="actual">The actual values that have been predicted.</param>
        /// <returns>
        /// The loss value between the expected values and
        /// the actual predicted values.
        /// </returns>
        public double Loss(double[] actual)
        {
            // R-squared = 100 * SS(regression) / SS(total)

            int p = NumberOfInputs;
            int n = actual.Length;
            double SSe = 0.0;
            double SSt = 0.0;

            // Calculate output mean
            double avg = Expected.Mean();

            // Calculate SSe and SSt
            for (int i = 0; i < Expected.Length; i++)
            {
                double d = Expected[i][0] - actual[i];
                SSe += d * d;

                d = Expected[i][0] - avg;
                SSt += d * d;
            }

            // Calculate R-Squared
            double r2 = 1 - (((Math.Abs(SSt - SSe) / n) > 1e-16) ? SSe / SSt : 0);

            if (!Adjust)
            {
                // Return ordinary R-Squared
                return r2;
            }

            if (r2 == 1)
                return 1;

            if (n - p == 1.0)
                return double.NaN;

            // Return adjusted R-Squared
            return 1.0 - (1.0 - r2) * ((n - 1.0) / (n - p - 1.0));
        }

        /// <summary>
        /// Computes the loss between the expected values (ground truth)
        /// and the given actual values that have been predicted.
        /// </summary>
        /// <param name="actual">The actual values that have been predicted.</param>
        /// <returns>
        /// The loss value between the expected values and
        /// the actual predicted values.
        /// </returns>
        public override double[] Loss(double[][] actual)
        {
            int N = actual.Length;
            int M = actual.Columns();
            int P = NumberOfInputs;
            double[] SSe = new double[M];
            double[] SSt = new double[M];
            double[] avg = new double[M];
            double[] r2 = new double[M];

            // For each output variable
            avg = Expected.Mean(dimension: 0);

            // Calculate SSe and SSt
            for (int i = 0; i < N; i++)
            {
                for (int c = 0; c < M; c++)
                {
                    double d;
                    d = Expected[i][c] - actual[i][c];
                    SSe[c] += d * d;

                    d = Expected[i][c] - avg[c];
                    SSt[c] += d * d;
                }
            }

            // Calculate R-Squared
            for (int c = 0; c < M; c++)
                r2[c] = 1 - (((Math.Abs(SSt[c] - SSe[c]) / N) > 1e-16) ? SSe[c] / SSt[c] : 0);

            if (Adjust)
            {
                // Return adjusted R-Squared
                for (int c = 0; c < M; c++)
                {
                    if (r2[c] == 1.0)
                        continue;

                    if (N == P + 1)
                    {
                        r2[c] = double.NaN;
                    }
                    else
                    {
                        r2[c] = 1.0 - (1.0 - r2[c]) * ((N - 1.0) / (N - P - 1.0));
                    }
                }
            }

            return r2;
        }
    }
}
