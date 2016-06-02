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

    /// <summary>
    ///   Negative log-likelihood loss. 
    /// </summary>
    /// 
    [Serializable]
    public class LogLikelihoodLoss : ILoss<double[][]>, ILoss<double[]>
    {


        /// <summary>
        /// Initializes a new instance of the <see cref="LogLikelihoodLoss"/> class.
        /// </summary>
        /// 
        public LogLikelihoodLoss()
        {
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
        public double Loss(double[][] actual)
        {
            double error = 0;
            for (int i = 0; i < actual.Length; i++)
                for (int j = 0; j < actual[i].Length; j++)
                    error += actual[i][j];
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
        public double Loss(double[] actual)
        {
            double error = 0;
            for (int i = 0; i < actual.Length; i++)
                error += actual[i];
            return error;
        }


    }
}
