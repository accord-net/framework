// Accord Statistics Library
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

namespace Accord.MachineLearning
{
    using Accord.Compat;
    using System.Threading;

    /// <summary>
    ///   Common interface for unsupervised learning algorithms.
    /// </summary>
    /// 
    /// <typeparam name="TModel">The type for the model being learned.</typeparam>
    /// <typeparam name="TInput">The type for the output data that originates from the model.</typeparam>
    /// <typeparam name="TOutput">The type for the input data that enters the model.</typeparam>
    /// 
    public interface IUnsupervisedLearning<out TModel, in TInput, out TOutput>
        where TModel : ITransform<TInput, TOutput>
    {

        /// <summary>
        ///   Gets or sets a cancellation token that can be used to 
        ///   stop the learning algorithm while it is running.
        /// </summary>
        /// 
        CancellationToken Token { get; set; }

        /// <summary>
        ///   Learns a model that can map the given inputs to the desired outputs.
        /// </summary>
        /// 
        /// <param name="x">The model inputs.</param>
        /// <param name="weights">The weight of importance for each input sample.</param>
        /// 
        /// <returns>A model that has learned how to produce suitable outputs
        ///   given the input data <paramref name="x"/>.</returns>
        /// 
        TModel Learn(TInput[] x, double[] weights = null);

    }
}
