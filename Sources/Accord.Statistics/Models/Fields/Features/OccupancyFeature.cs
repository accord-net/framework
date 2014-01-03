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
    using Accord.Math;
    using Accord.Statistics.Models.Fields.Functions;

    /// <summary>
    ///   State occupancy function for modeling continuous-
    ///   density Hidden Markov Model state emission features.
    /// </summary>
    /// 
    /// <typeparam name="T"></typeparam>
    /// 
    [Serializable]
    public sealed class OccupancyFeature<T> : FeatureBase<T>, IFeature<T>
    {
        private int current;

        /// <summary>
        ///   Constructs a state occupancy feature.
        /// </summary>
        /// 
        /// <param name="owner">The potential function to which this feature belongs.</param>
        /// <param name="factorIndex">The index of the potential factor to which this feature belongs.</param>
        /// <param name="state">The current state.</param>
        /// 
        public OccupancyFeature(IPotentialFunction<T> owner, int factorIndex, int state)
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
        public override double Compute(int previousState, int currentState, T[] observations,
            int observationIndex, int outputClass = 0)
        {
            return (this.current == currentState) ? 1 : 0;
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
            if (y != FactorIndex)
                return 0;

            double marginal = 0;
            for (int t = 0; t < x.Length; t++)
            {
                marginal += fwd[t, current] * bwd[t, current]; ;
            }

            return marginal;
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

            double marginal = double.NegativeInfinity;
            for (int t = 0; t < x.Length; t++)
                marginal = Special.LogSum(marginal, lnFwd[t, current] + lnBwd[t, current]);

            return marginal;
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
            var clone = (OccupancyFeature<T>)MemberwiseClone();
            clone.Owner = newOwner;
            return clone;
        }

    }
}
