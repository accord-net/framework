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
    using Accord.Math;

    /// <summary>
    ///   State feature for Hidden Markov Model output class symbol probabilities.
    /// </summary>
    /// 
    [Serializable]
    public sealed class OutputFeature<T> : FeatureBase<T>, IFeature<T>
    {

        int classSymbol;

        /// <summary>
        ///   Constructs a new output class symbol feature.
        /// </summary>
        /// 
        /// <param name="owner">The potential function to which this feature belongs.</param>
        /// <param name="factorIndex">The index of the potential factor to which this feature belongs.</param>
        /// <param name="classSymbol">The emission symbol.</param>
        /// 
        public OutputFeature(IPotentialFunction<T> owner, int factorIndex, int classSymbol)
            : base(owner, factorIndex)
        {
            this.classSymbol = classSymbol;
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
            int observationIndex, int outputClass)
        {
            return (outputClass == classSymbol) ? 1 : 0;
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
            double likelihood = 0;

            int states = fwd.GetLength(1);
            for (int j = 0; j < states; j++)
                likelihood += fwd[x.Length - 1, j];

            return likelihood;
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
            double logLikelihood = Double.NegativeInfinity;

            int states = lnFwd.GetLength(1);
            for (int j = 0; j < states; j++)
                logLikelihood = Special.LogSum(logLikelihood, lnFwd[x.Length - 1, j]);

            return logLikelihood;
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
            var clone = (OutputFeature<T>)MemberwiseClone();
            clone.Owner = newOwner;
            return clone;
        }

    }
}
