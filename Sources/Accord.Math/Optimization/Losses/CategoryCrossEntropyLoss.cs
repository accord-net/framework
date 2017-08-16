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
    using Statistics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Categorical cross-entropy loss for multi-class problems,
    ///   also known as the logistic loss for softmax (categorical) outputs.
    /// </summary>
    /// 
    /// <seealso cref="BinaryCrossEntropyLoss"/>
    /// 
    [Serializable]
    public class CategoryCrossEntropyLoss : LossBase<bool[][]>, ILoss<int[]>, ILoss<double[][]>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryCrossEntropyLoss"/> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public CategoryCrossEntropyLoss(double[][] expected)
        {
            this.Expected = Classes.Decide(expected);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryCrossEntropyLoss"/> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public CategoryCrossEntropyLoss(int[] expected)
        {
            this.Expected = Jagged.OneHot<bool>(expected);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryCrossEntropyLoss"/> class.
        /// </summary>
        /// <param name="expected">The expected outputs (ground truth).</param>
        public CategoryCrossEntropyLoss(bool[][] expected)
        {
            this.Expected = expected;
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
        public override double Loss(bool[][] actual)
        {
            double sum = 0;
            for (int i = 0; i < actual.Length; i++)
                for (int j = 0; j < actual[i].Length; j++)
                    sum -= Measures.Entropy(Expected[i][j], actual[i][j]);
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
        public double Loss(double[][] actual)
        {
            double sum = 0;
            for (int i = 0; i < actual.Length; i++)
                for (int j = 0; j < actual[i].Length; j++)
                    sum -= Measures.Entropy(Expected[i][j], actual[i][j]);
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
        public double Loss(int[] actual)
        {
            return Loss(Jagged.OneHot<bool>(actual));
        }
    }
}
