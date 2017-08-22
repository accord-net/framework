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
    using Accord.Compat;

    /// <summary>
    ///   Squared Hinge loss.
    /// </summary>
    /// 
    [Serializable]
    public struct SquaredHingeLoss : ILoss<double[]>,
        IDifferentiableLoss<bool, double, double>,
        IDifferentiableLoss<double, double, double>
    {
        HingeLoss hinge;

        /// <summary>
        ///   Initializes a new instance of the <see cref="SquaredHingeLoss"/> class.
        /// </summary>
        /// 
        /// <param name="expected">The expected outputs (ground truth).</param>
        /// 
        public SquaredHingeLoss(double[][] expected)
        {
            hinge = new Losses.HingeLoss(expected);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SquaredHingeLoss"/> class.
        /// </summary>
        /// 
        /// <param name="expected">The expected outputs (ground truth).</param>
        /// 
        public SquaredHingeLoss(double[] expected)
        {
            hinge = new Losses.HingeLoss(expected);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SquaredHingeLoss"/> class.
        /// </summary>
        /// 
        /// <param name="expected">The expected outputs (ground truth).</param>
        /// 
        public SquaredHingeLoss(int[] expected)
        {
            hinge = new Losses.HingeLoss(expected);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SquaredHingeLoss"/> class.
        /// </summary>
        /// 
        /// <param name="expected">The expected outputs (ground truth).</param>
        /// 
        public SquaredHingeLoss(bool[] expected)
        {
            hinge = new Losses.HingeLoss(expected);
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
            for (int i = 0; i < hinge.Expected.Length; i++)
                error += Loss(hinge.Expected[i][0], actual[i]);
            return error;
        }

        /// <summary>
        /// Computes the derivative of the loss between the expected values (ground truth)
        /// and the given actual values that have been predicted.
        /// </summary>
        /// <param name="expected">The expected values that should have been predicted.</param>
        /// <param name="actual">The actual values that have been predicted.</param>
        /// <returns>The loss value between the expected values and
        /// the actual predicted values.</returns>
        public double Loss(bool expected, double actual)
        {
            if (expected)
            {
                if (actual > 1)
                    return 0;
                double d = 1 - actual;
                return 0.5 * d * d;
            }
            else
            {
                if (actual < -1)
                    return 0;
                double d = 1 + actual;
                return 0.5 * d * d;
            }
        }

        /// <summary>
        /// Computes the derivative of the loss between the expected values (ground truth)
        /// and the given actual values that have been predicted.
        /// </summary>
        /// <param name="expected">The expected values that should have been predicted.</param>
        /// <param name="actual">The actual values that have been predicted.</param>
        /// <returns>The loss value between the expected values and
        /// the actual predicted values.</returns>
        public double Derivative(bool expected, double actual)
        {
            if (expected)
            {
                if (actual > 1)
                    return 0;
                return actual * (1 - actual);
            }
            else
            {
                if (actual < -1)
                    return 0;
                return -actual * (1 + actual);
            }
        }

        /// <summary>
        /// Computes the derivative of the loss between the expected values (ground truth)
        /// and the given actual values that have been predicted.
        /// </summary>
        /// <param name="expected">The expected values that should have been predicted.</param>
        /// <param name="actual">The actual values that have been predicted.</param>
        /// <returns>The loss value between the expected values and
        /// the actual predicted values.</returns>
        public double Loss(double expected, double actual)
        {
            // TODO: Use multiplication instead of conditionals
            if (expected > 0)
            {
                if (actual > 1)
                    return 0;
                double d = 1 - actual;
                return 0.5 * d * d;
            }
            else
            {
                if (actual < -1)
                    return 0;
                double d = 1 + actual;
                return 0.5 * d * d;
            }
        }

        /// <summary>
        /// Computes the derivative of the loss between the expected values (ground truth)
        /// and the given actual values that have been predicted.
        /// </summary>
        /// <param name="expected">The expected values that should have been predicted.</param>
        /// <param name="actual">The actual values that have been predicted.</param>
        /// <returns>The loss value between the expected values and
        /// the actual predicted values.</returns>
        public double Derivative(double expected, double actual)
        {
            // TODO: Use multiplication instead of conditionals
            if (expected > 0)
            {
                if (actual > 1)
                    return 0;
                return actual * (1 - actual);
            }
            else
            {
                if (actual < -1)
                    return 0;
                return -actual * (1 + actual);
            }
        }
    }
}
