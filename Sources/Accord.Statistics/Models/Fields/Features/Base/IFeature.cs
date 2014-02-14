// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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

namespace Accord.Statistics.Models.Fields.Features
{
    using Accord.Statistics.Models.Fields.Functions;

    /// <summary>
    ///   Common interface for <see cref="ConditionalRandomField{T}">Conditional Random Fields</see>
    ///   <see cref="IFeature{T}">feature functions</see>
    /// </summary>
    /// 
    /// <typeparam name="TObservation">The type of the observations being modeled.</typeparam>
    /// 
    public interface IFeature<TObservation>
    {

        /// <summary>
        ///   Gets the potential function containing this feature.
        /// </summary>
        /// 
        IPotentialFunction<TObservation> Owner { get; }


        /// <summary>
        ///   Computes the feature for the given parameters.
        /// </summary>
        /// 
        /// <param name="previousState">The previous state.</param>
        /// <param name="currentState">The current state.</param>
        /// <param name="observations">The observations.</param>
        /// <param name="observationIndex">The index of the current observation.</param>
        /// <param name="outputClass">The output class label for the sequence.</param>
        /// 
        double Compute(int previousState, int currentState, TObservation[] observations, int observationIndex, int outputClass = 0);

        /// <summary>
        ///   Computes the feature for the given parameters.
        /// </summary>
        /// 
        /// <param name="states">The sequence of states.</param>
        /// <param name="observations">The sequence of observations.</param>
        /// <param name="output">The output class label for the sequence.</param>
        /// 
        /// <returns>The result of the feature.</returns>
        /// 
        double Compute(int[] states, TObservation[] observations, int output);

        /// <summary>
        ///   Computes the probability of occurrence of this 
        ///   feature given a sequence of observations.
        /// </summary>
        /// 
        /// <param name="fwd">The matrix of forward state probabilities.</param>
        /// <param name="bwd">The matrix of backward state probabilities.</param>
        /// <param name="x">The observation sequence.</param>
        /// <param name="y">The output class label for the sequence.</param>
        /// 
        /// <returns>The probability of occurrence of this feature.</returns>
        /// 
        double Marginal(double[,] fwd, double[,] bwd, TObservation[] x, int y);

        /// <summary>
        ///   Computes the log-probability of occurrence of this 
        ///   feature given a sequence of observations.
        /// </summary>
        /// 
        /// <param name="lnFwd">The matrix of forward state log-probabilities.</param>
        /// <param name="lnBwd">The matrix of backward state log-probabilities.</param>
        /// <param name="x">The observation sequence.</param>
        /// <param name="y">The output class label for the sequence.</param>
        /// 
        /// <returns>The probability of occurrence of this feature.</returns>
        /// 
        double LogMarginal(double[,] lnFwd, double[,] lnBwd, TObservation[] x, int y);

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        IFeature<TObservation> Clone(IPotentialFunction<TObservation> newOwner);

    }
}
