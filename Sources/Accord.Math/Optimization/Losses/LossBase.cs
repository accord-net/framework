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

    /// <summary>
    ///   Base class for <see cref="ILoss{T}">loss functions</see>.
    /// </summary>
    /// 
    /// <typeparam name="TInput">The type for the expected data.</typeparam>
    /// <typeparam name="TOutput">The type for the loss value. Default is double.</typeparam>
    /// 
    [Serializable]
    public abstract class LossBase<TInput, TOutput> : ILoss<TInput, TOutput>
    {
        private TInput expected;

        /// <summary>
        ///   Gets the expected outputs (the ground truth).
        /// </summary>
        /// 
        public TInput Expected { get { return expected; } }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LossBase{TInput, TOutput}"/> class.
        /// </summary>
        /// 
        /// <param name="expected">The expected outputs (ground truth).</param>
        /// 
        protected LossBase(TInput expected)
        {
            this.expected = expected;
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
        public abstract TOutput Loss(TInput actual);
    }

    /// <summary>
    ///   Base class for <see cref="ILoss{T}">loss functions</see>.
    /// </summary>
    /// 
    /// <typeparam name="T">The type for the expected data.</typeparam>
    /// 
    [Serializable]
    public abstract class LossBase<T> : LossBase<T, double>, ILoss<T>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="LossBase{TInput, TOutput}"/> class.
        /// </summary>
        /// 
        /// <param name="expected">The expected outputs (ground truth).</param>
        /// 
        protected LossBase(T expected)
            : base(expected)
        {
        }
    }
}
