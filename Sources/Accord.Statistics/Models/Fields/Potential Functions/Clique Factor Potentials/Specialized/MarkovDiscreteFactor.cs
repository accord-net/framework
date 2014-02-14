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

namespace Accord.Statistics.Models.Fields.Functions.Specialized
{
    using System;
    using System.Collections.Generic;
    using Accord.Statistics.Models.Fields.Features;
    using System.Runtime.Serialization;

    /// <summary>
    ///   Discrete-density Markov Factor Potential (Clique Potential) function.
    /// </summary>
    /// 
    [Serializable]
    public class MarkovDiscreteFactor : FactorPotential<int>
    {

        /// <summary>
        ///   Gets the number of symbols in the discrete
        ///   alphabet used by this Markov model factor.
        /// </summary>
        /// 
        public int Symbols { get; private set; }


        /// <summary>
        ///   Creates a new factor (clique) potential function.
        /// </summary>
        /// 
        /// <param name="owner">The owner <see cref="IPotentialFunction{T}"/>.</param>
        /// <param name="states">The number of states in this clique potential.</param>
        /// <param name="factorIndex">The index of this factor potential in the <paramref name="owner"/>.</param>
        /// <param name="symbols">The number of symbols in the discrete alphabet.</param>
        /// <param name="classIndex">The index of the first class label feature in the <paramref name="owner"/>'s parameter vector.</param>
        /// <param name="classCount">The number of class label features in this factor.</param>
        /// <param name="edgeIndex">The index of the first edge feature in the <paramref name="owner"/>'s parameter vector.</param>
        /// <param name="edgeCount">The number of edge features in this factor.</param>
        /// <param name="stateIndex">The index of the first state feature in the <paramref name="owner"/>'s parameter vector.</param>
        /// <param name="stateCount">The number of state features in this factor.</param>
        /// 
        public MarkovDiscreteFactor(IPotentialFunction<int> owner, int states, int factorIndex, int symbols,
            int edgeIndex, int edgeCount,
            int stateIndex, int stateCount,
            int classIndex=0, int classCount=0)
            : base(owner, states, factorIndex, edgeIndex, edgeCount, stateIndex, stateCount, classIndex, classCount)
        {
            this.Symbols = symbols;
        }


        /// <summary>
        ///   Computes the factor potential function for the given parameters.
        /// </summary>
        /// 
        /// <param name="previousState">The previous state in a given sequence of states.</param>
        /// <param name="currentState">The current state in a given sequence of states.</param>
        /// <param name="observations">The observation vector.</param>
        /// <param name="index">The index of the observation in the current state of the sequence.</param>
        /// <param name="outputClass">The output class label for the sequence.</param>
        /// <returns>The value of the factor potential function evaluated for the given parameters.</returns>
        /// 
        public override double Compute(int previousState, int currentState, int[] observations, int index, int outputClass = 0)
        {
            if (outputClass != this.Index)
                return Double.NegativeInfinity;

            double[] parameters = Owner.Weights;

            double sum = 0;


            // If a output feature exists,
            if (OutputParameters.Count != 0)
            {
                // it will be activated for every state
                //   in the state/observation sequence
                int i = OutputParameters.Offset;
                double w = parameters[i];

                if (Double.IsNaN(w) || Double.IsNegativeInfinity(w))
                    return parameters[i] = Double.NegativeInfinity;

                sum += w;
            }

            // The state occupancy feature is activated for the current state.
            {
                int i = StateParameters.Offset + currentState * Symbols + observations[index];
                double b = parameters[i];

                if (Double.IsNaN(b) || Double.IsNegativeInfinity(b))
                    return parameters[i] = Double.NegativeInfinity;

                sum += b;
            }

            // If are just starting the sequence
            if (previousState == -1)
            {
                // Compute the initial transition feature
                int i = EdgeParameters.Offset + currentState;
                double p = parameters[i];

                if (Double.IsNaN(p))
                    return parameters[i] = Double.NegativeInfinity;

                sum += p;
            }
            else
            {
                // Compute the transition feature
                int i = EdgeParameters.Offset + States + previousState * States + currentState;
                double a = parameters[i];

                if (Double.IsNaN(a))
                    return parameters[i] = Double.NegativeInfinity;

                sum += a;
            }

            System.Diagnostics.Debug.Assert(!Double.IsNaN(sum));

            return sum;
        }


    }
}
