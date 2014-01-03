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
    ///   Edge feature for Hidden Markov Model state transition probabilities.
    /// </summary>
    /// 
    [Serializable]
    public sealed class InitialFeature<T> : FeatureBase<T>, IFeature<T>
    {
        private int current;

        /// <summary>
        ///   Constructs a initial state transition feature.
        /// </summary>
        /// 
        /// <param name="owner">The potential function to which this feature belongs.</param>
        /// <param name="factorIndex">The index of the potential factor to which this feature belongs.</param>
        /// <param name="state">The destination state.</param>
        /// 
        public InitialFeature(IPotentialFunction<T> owner, int factorIndex, int state)
            : base(owner, factorIndex)
        {
            this.current = state;
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
        public override double Compute(int previousState, int currentState, T[] observations, int observationIndex, int outputClass = 0)
        {
            if (previousState == -1 && this.current == currentState && observationIndex == 0)
                return 1.0;

            return 0.0;
        }

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
        public override double Marginal(double[,] fwd, double[,] bwd, T[] x, int y)
        {
            // Assume the simplifying structure that each
            // factor is responsible for single output y.
            if (y != FactorIndex) return 0;

            return fwd[0, current] * bwd[0, current];
        }

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
        public override double LogMarginal(double[,] lnFwd, double[,] lnBwd, T[] x, int y)
        {
            // Assume the simplifying structure that each
            // factor is responsible for single output y.
            if (y != FactorIndex) return Double.NegativeInfinity;

            return lnFwd[0, current] + lnBwd[0, current];
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public IFeature<T> Clone(IPotentialFunction<T> newOwner)
        {
            var clone = (InitialFeature<T>)MemberwiseClone();
            clone.Owner = newOwner;
            return clone;
        }
    }
}
