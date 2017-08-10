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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Euclidean loss, also known as zero-one-loss. This class
    ///   provides exactly the same functionality as <see cref="SquareLoss"/>
    ///   but has a more intuitive name. Both classes are interchangeable.
    /// </summary>
    /// 
    [Serializable]
    public class EuclideanLoss : SquareLoss
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EuclideanLoss"/> class.
        /// </summary>
        /// 
        /// <param name="expected">The expected outputs (ground truth).</param>
        /// 
        public EuclideanLoss(double[][] expected)
            : base(expected)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EuclideanLoss"/> class.
        /// </summary>
        /// 
        /// <param name="expected">The expected outputs (ground truth).</param>
        /// 
        public EuclideanLoss(double[] expected)
            : base(expected)
        {
        }
    }

    /// <summary>
    ///   Square loss, also known as L2-loss or Euclidean loss.
    /// </summary>
    /// 
    [Serializable]
    public class SquareLoss : LossBase<double[][]>
    {
        private bool mean = true;
        private bool root = false;

        /// <summary>
        ///   Gets or sets a value indicating whether the
        ///   root square loss should be computed. If <see cref="Mean"/>
        ///   is also set, computes the root mean square loss. Default is false.
        /// </summary>
        /// 
        /// <value>
        ///   <c>true</c> if the root square loss should be computed; otherwise, <c>false</c>.
        /// </value>
        /// 
        public bool Root
        {
            get { return root; }
            set { root = value; }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether the
        ///   mean square loss should be computed. If <see cref="Root"/>
        ///   is also set, computes the root mean square loss. Default is true.
        /// </summary>
        /// 
        /// <value>
        ///   <c>true</c> if the mean square loss should be computed; otherwise, <c>false</c>.
        /// </value>
        /// 
        public bool Mean
        {
            get { return mean; }
            set { mean = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SquareLoss"/> class.
        /// </summary>
        /// 
        /// <param name="expected">The expected outputs (ground truth).</param>
        /// 
        public SquareLoss(double[][] expected)
        {
            this.Expected = expected;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SquareLoss"/> class.
        /// </summary>
        /// 
        /// <param name="expected">The expected outputs (ground truth).</param>
        /// 
        public SquareLoss(double[] expected)
        {
            this.Expected = Jagged.ColumnVector(expected);
        }

        /// <summary>
        ///   Computes the loss between the expected values (ground truth)
        ///   and the given actual values that have been predicted.
        /// </summary>
        /// 
        /// <param name="actual">The actual values that have been predicted.</param>
        /// 
        /// <returns>
        ///   The loss value between the expected values and
        ///   the actual predicted values.
        /// </returns>
        /// 
        public override double Loss(double[][] actual)
        {
            double error = 0;
            for (int i = 0; i < Expected.Length; i++)
                error += Distance.SquareEuclidean(actual[i], Expected[i]);

            if (root)
                error = Math.Sqrt(error);

            if (mean)
                error = error / Expected.Length;

            return error;
        }

        /// <summary>
        ///   Computes the loss between the expected values (ground truth)
        ///   and the given actual values that have been predicted.
        /// </summary>
        /// 
        /// <param name="actual">The actual values that have been predicted.</param>
        /// 
        /// <returns>
        ///   The loss value between the expected values and
        ///   the actual predicted values.
        /// </returns>
        /// 
        public double Loss(double[] actual)
        {
            double error = 0;
            for (int i = 0; i < Expected.Length; i++)
            {
                double u = actual[i] - Expected[i][0];
                error += u * u;
            }

            if (root)
                error = Math.Sqrt(error);

            if (mean)
                error = error / Expected.Length;

            return error;
        }
    }
}
