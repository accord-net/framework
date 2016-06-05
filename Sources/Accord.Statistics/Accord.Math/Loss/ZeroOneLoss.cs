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
    using Accord.Math;
    using Accord.Statistics;
    using System;

    /// <summary>
    ///   Accuracy loss, also known as zero-one-loss.
    /// </summary>
    /// 
    [Serializable]
    public class ZeroOneLoss : LossBase<int[]>, ILoss<bool[]>, ILoss<double[][]>
    {
        private bool mean;

        /// <summary>
        ///   Gets or sets a value indicating whether the
        ///   mean accuracy loss should be computed.
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
        /// Initializes a new instance of the <see cref="ZeroOneLoss"/> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public ZeroOneLoss(double[][] expected)
            : base(expected.ArgMax(dimension: 0))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZeroOneLoss"/> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public ZeroOneLoss(int[] expected)
            : base(expected)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZeroOneLoss"/> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public ZeroOneLoss(bool[] expected)
            : base(expected.ToInt32())
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
        public double Loss(double[][] actual)
        {
            int error = 0;
            for (int i = 0; i < Expected.Length; i++)
                if (Expected[i] != actual[i].ArgMax())
                    error++;

            if (mean)
                return error / (double)Expected.Length;
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
        public override double Loss(int[] actual)
        {
            int error = 0;
            for (int i = 0; i < Expected.Length; i++)
                if (Expected[i] != actual[i])
                    error++;

            if (mean)
                return error / (double)Expected.Length;
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
        public double Loss(bool[] actual)
        {
            int error = 0;
            for (int i = 0; i < Expected.Length; i++)
                if ((Classes.Decide(Expected[i])) != actual[i])
                    error++;

            if (mean)
                return error / (double)Expected.Length;
            return error;
        }
    }
}
