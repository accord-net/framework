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
    ///   Base class for <see cref="ILoss{T}">loss functions</see>.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The type for the expected data.</typeparam>
    /// <typeparam name="TScore">The type for the predicted score values.</typeparam>
    /// <typeparam name="TLoss">The type for the loss value. Default is double.</typeparam>
    /// 
    [Serializable]
    public abstract class LossBase<TInput, TScore, TLoss> : ILoss<TScore, TLoss>
    {
        private TInput expected;

        /// <summary>
        ///   Gets the expected outputs (the ground truth).
        /// </summary>
        /// 
        public TInput Expected
        {
            get { return expected; }
            protected set { expected = value; }
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
        public abstract TLoss Loss(TScore actual);
    }

    /// <summary>
    ///   Base class for <see cref="ILoss{T}">loss functions</see>.
    /// </summary>
    /// 
    /// <typeparam name="T">The type for the expected data.</typeparam>
    /// 
    [Serializable]
    public abstract class LossBase<T> : LossBase<T, T, double>, ILoss<T>
    {

    }
}
