// Accord Math Library
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

namespace Accord.Math.Optimization.Losses
{
    using Accord.Math;
    using Accord.Statistics;
    using System;
    using Accord.Compat;

    /// <summary>
    ///   Mean Accuracy loss, also known as zero-one-loss per 
    ///   class. Equivalent to <see cref="ZeroOneLoss"/> but
    ///   for multi-label classifiers.
    /// </summary>
    /// 
    [Serializable]
    public class HammingLoss : LossBase<int[][]>,
        ILoss<bool[][]>, ILoss<double[][]>, ILoss<int[]>
    {
        private bool mean = true;
        private int total;

        /// <summary>
        ///   Gets or sets a value indicating whether the mean 
        ///   accuracy loss should be computed. Default is true.
        /// </summary>
        /// 
        /// <value>
        ///   <c>true</c> if the mean accuracy loss should be computed; otherwise, <c>false</c>.
        /// </value>
        /// 
        public bool Mean
        {
            get { return mean; }
            set { mean = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HammingLoss"/> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public HammingLoss(double[][] expected)
        {
            this.Expected = expected.ToInt32();
            for (int i = 0; i < expected.Length; i++)
                total += expected[i].Length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HammingLoss"/> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public HammingLoss(int[][] expected)
        {
            this.Expected = expected;
            for (int i = 0; i < expected.Length; i++)
                total += expected[i].Length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HammingLoss"/> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public HammingLoss(int[] expected)
            : this(Jagged.OneHot(expected))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HammingLoss"/> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public HammingLoss(bool[][] expected)
        {
            this.Expected = expected.ToInt32();
            for (int i = 0; i < expected.Length; i++)
                total += expected[i].Length;
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
        public double Loss(double[][] actual)
        {
            int error = 0;
            for (int i = 0; i < Expected.Length; i++)
                for (int j = 0; j < Expected[i].Length; j++)
                    if (Classes.Decide(Expected[i][j]) != Classes.Decide(actual[i][j]))
                        error++;

            if (mean)
                return error / (double)total;
            return error;
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
        public override double Loss(int[][] actual)
        {
            int error = 0;
            for (int i = 0; i < Expected.Length; i++)
                for (int j = 0; j < Expected[i].Length; j++)
                    if (Classes.Decide(Expected[i][j]) != Classes.Decide(actual[i][j]))
                        error++;

            if (mean)
                return error / (double)total;
            return error;
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
        public double Loss(int[] actual)
        {
            int error = 0;
            for (int i = 0; i < Expected.Length; i++)
                if (Expected[i][0] != actual[i])
                    error++;

            if (mean)
                return error / (double)total;
            return error;
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
        public double Loss(bool[][] actual)
        {
            int error = 0;
            for (int i = 0; i < Expected.Length; i++)
                for (int j = 0; j < Expected[i].Length; j++)
                    if (Classes.Decide(Expected[i][j]) != actual[i][j])
                        error++;

            if (mean)
                return error / (double)total;
            return error;
        }
    }
}
