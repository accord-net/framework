// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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

    /// <summary>
    ///   Binary cross-entropy loss for multi-label problems, also
    ///   known as logistic loss per output of a multi-label classifier.
    /// </summary>
    /// 
    [Serializable]
    public class BinaryCrossEntropyLoss : LossBase<double[][]>, ILoss<double[]>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryCrossEntropyLoss"/> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public BinaryCrossEntropyLoss(double[][] expected)
            : base(expected)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryCrossEntropyLoss"/> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public BinaryCrossEntropyLoss(double[] expected)
            : base(Jagged.ColumnVector(expected))
        {
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
        public override double Loss(double[][] actual)
        {
            double sum = 0;
            for (int i = 0; i < actual.Length; i++)
                for (int j = 0; j < actual[i].Length; j++)
                    sum -= Expected[i][j] * Math.Log(actual[i][j])
                        + (1.0 - Expected[i][j]) * Math.Log(1.0 - actual[i][j]);
            return sum;
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
            double sum = 0;
            for (int i = 0; i < actual.Length; i++)
                sum -= Expected[i][0] * Math.Log(actual[i])
                    + (1.0 - Expected[i][0]) * Math.Log(1.0 - actual[i]);
            return sum;
        }
    }
}
