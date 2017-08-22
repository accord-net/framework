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

    /// <summary>
    ///   Common interface for loss functions, such as <see cref="SquareLoss"/>, 
    ///   <see cref="HingeLoss"/>, <see cref="CategoryCrossEntropyLoss"/> and
    ///   <see cref="BinaryCrossEntropyLoss"/>.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In mathematical optimization, statistics, decision theory and machine learning, a loss 
    ///   function or cost function is a function that maps an event or values of one or more 
    ///   variables onto a real number intuitively representing some "cost" associated with the 
    ///   event. An optimization problem seeks to minimize a loss function. An objective function
    ///   is either a loss function or its negative (sometimes called a reward function, a profit
    ///   function, a utility function, a fitness function, etc.), in which case it is to be
    ///   maximized.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://en.wikipedia.org/wiki/Loss_function">
    ///       Wikipedia contributors. "Loss function." Wikipedia, The Free Encyclopedia.
    ///       Wikipedia, The Free Encyclopedia, 18 Mar. 2016. Web.</a></description></item>
    ///   </list>
    /// </para>   
    /// </remarks>
    /// 
    /// <typeparam name="TScore">The type for the predicted score values.</typeparam>
    /// <typeparam name="TLoss">The type for the loss value. Default is double.</typeparam>
    /// 
    public interface ILoss<TScore, TLoss>
    {
        /// <summary>
        ///   Computes the loss between the expected values (ground truth) 
        ///   and the given actual values that have been predicted.
        /// </summary>
        /// 
        /// <param name="actual">The actual values that have been predicted.</param>
        /// 
        /// <returns>The loss value between the expected values and
        ///   the actual predicted values.</returns>
        /// 
        TLoss Loss(TScore actual);
    }

    /// <summary>
    ///   Common interface for differentiable loss functions, such as <see cref="SquareLoss"/>, 
    ///   <see cref="HingeLoss"/>, <see cref="CategoryCrossEntropyLoss"/> and
    ///   <see cref="BinaryCrossEntropyLoss"/>.
    /// </summary>
    /// 
    public interface IDifferentiableLoss<TInput, TScore, TLoss>
    {

        /// <summary>
        ///   Computes the derivative of the loss between the expected values (ground truth) 
        ///   and the given actual values that have been predicted.
        /// </summary>
        /// 
        /// <param name="actual">The actual values that have been predicted.</param>
        /// <param name="expected">The expected values that should have been predicted.</param>
        /// 
        /// <returns>The loss value between the expected values and
        ///   the actual predicted values.</returns>
        /// 
        TLoss Loss(TInput expected, TScore actual);

        /// <summary>
        ///   Computes the derivative of the loss between the expected values (ground truth) 
        ///   and the given actual values that have been predicted.
        /// </summary>
        /// 
        /// <param name="actual">The actual values that have been predicted.</param>
        /// <param name="expected">The expected values that should have been predicted.</param>
        /// 
        /// <returns>The loss value between the expected values and
        ///   the actual predicted values.</returns>
        /// 
        TLoss Derivative(TInput expected, TScore actual);
    }


    /// <summary>
    ///   Common interface for loss functions, such as 
    ///   <see cref="SquareLoss"/>, <see cref="HingeLoss"/> and
    ///   <see cref="CategoryCrossEntropyLoss"/>.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In mathematical optimization, statistics, decision theory and machine learning, a loss 
    ///   function or cost function is a function that maps an event or values of one or more 
    ///   variables onto a real number intuitively representing some "cost" associated with the 
    ///   event. An optimization problem seeks to minimize a loss function. An objective function
    ///   is either a loss function or its negative (sometimes called a reward function, a profit
    ///   function, a utility function, a fitness function, etc.), in which case it is to be
    ///   maximized.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://en.wikipedia.org/wiki/Loss_function">
    ///       Wikipedia contributors. "Loss function." Wikipedia, The Free Encyclopedia.
    ///       Wikipedia, The Free Encyclopedia, 18 Mar. 2016. Web.</a></description></item>
    ///   </list>
    /// </para>   
    /// </remarks>
    /// 
    /// <typeparam name="T">The type for the expected data.</typeparam>
    /// 
    public interface ILoss<T> : ILoss<T, double>
    {
    }


}
