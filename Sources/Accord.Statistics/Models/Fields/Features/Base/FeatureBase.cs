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
    using System;
    using Accord.Statistics.Models.Fields.Functions;

    /// <summary>
    ///   Base implementation for <see cref="ConditionalRandomField{T}">Conditional Random Fields</see>
    ///   <see cref="IFeature{T}">feature functions</see>.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the observations being modeled.</typeparam>
    /// 
    [Serializable]
    public abstract class FeatureBase<T>
    {
        /// <summary>
        ///   Gets the potential function containing this feature.
        /// </summary>
        /// 
        public IPotentialFunction<T> Owner { get; protected set; }

        /// <summary>
        ///   Gets the potential factor to which this feature belongs.
        /// </summary>
        /// 
        public int FactorIndex { get; private set; }

        /// <summary>
        ///   Creates a new feature.
        /// </summary>
        /// 
        /// <param name="owner">The potential function to which this feature belongs.</param>
        /// <param name="factorIndex">The index of the potential factor to which this feature belongs.</param>
        /// 
        protected FeatureBase(IPotentialFunction<T> owner, int factorIndex)
        {
            this.Owner = owner;
            this.FactorIndex = factorIndex;
        }


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
        public virtual double Compute(int[] states, T[] observations, int output)
        {
            double z = Compute(-1, states[0], observations, 0, output);
            for (int t = 1; t < observations.Length; t++)
                z += Compute(states[t - 1], states[t], observations, t, output);

            return z;
        }

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
        public abstract double Compute(int previousState, int currentState, T[] observations, int observationIndex, int outputClass);

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
        public abstract double Marginal(double[,] fwd, double[,] bwd, T[] x, int y);

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
        public abstract double LogMarginal(double[,] lnFwd, double[,] lnBwd, T[] x, int y);

    }
}
