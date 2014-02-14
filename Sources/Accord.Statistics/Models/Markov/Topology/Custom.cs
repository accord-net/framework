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

namespace Accord.Statistics.Models.Markov.Topology
{
    using System;
    using Accord.Math;

    /// <summary>
    ///   Custom Topology for Hidden Markov Model.
    /// </summary>
    /// 
    /// <remarks>
    ///  <para>
    ///   An Hidden Markov Model Topology specifies how many states and which
    ///   initial probabilities a Markov model should have. Two common topologies
    ///   can be discussed in terms of transition state probabilities and are
    ///   available to construction through the <see cref="Ergodic"/> and
    ///   <see cref="Forward"/> classes implementing the <see cref="ITopology"/>
    ///   interface.</para>
    ///   
    ///  <para>Topology specification is important with regard to both learning and
    ///   performance: A model with too many states (and thus too many settable
    ///   parameters) will require too much training data while an model with an
    ///   insufficient number of states will prohibit the HMM from capturing subtle
    ///   statistical patterns.</para>
    ///   
    ///  <para>This custom implementation allows for arbitrarily specification of
    ///   the state transition matrix and initial state probabilities for
    ///   <see cref="IHiddenMarkovModel">hidden Markov models</see>.</para>
    ///   
    /// </remarks>
    ///   
    /// <seealso cref="HiddenMarkovModel"/>
    /// <seealso cref="Accord.Statistics.Models.Markov.Topology.ITopology"/>
    /// <seealso cref="Accord.Statistics.Models.Markov.Topology.Ergodic"/>
    /// <seealso cref="Accord.Statistics.Models.Markov.Topology.Forward"/>
    ///
    [Serializable]
    public class Custom : ITopology
    {

        private int states;
        private double[] pi;
        private double[,] transitions;

        /// <summary>
        ///   Creates a new custom topology with user-defined
        ///   transition matrix and initial state probabilities.
        /// </summary>
        /// 
        /// <param name="initial">The initial probabilities for the model.</param>
        /// <param name="transitions">The transition probabilities for the model.</param>
        /// 
        public Custom(double[,] transitions, double[] initial)
            : this(transitions, initial, false) { }

        /// <summary>
        ///   Creates a new custom topology with user-defined
        ///   transition matrix and initial state probabilities.
        /// </summary>
        /// 
        /// <param name="initial">The initial probabilities for the model.</param>
        /// <param name="transitions">The transition probabilities for the model.</param>
        /// <param name="logarithm">Set to true if the passed transitions are given 
        ///   in log-probabilities. Default is false (given values are probabilities).</param>
        /// 
        public Custom(double[,] transitions, double[] initial, bool logarithm)
        {
            if (transitions == null)
            {
                throw new ArgumentNullException("transitions");
            }

            if (initial == null)
            {
                throw new ArgumentNullException("initial");
            }

            if (transitions.GetLength(0) != transitions.GetLength(1))
            {
                if (transitions.GetLength(0) != transitions.GetLength(1))
                    throw new ArgumentException(
                        "Transition matrix should be square.",
                        "transitions");
            }

            if (initial.Length != transitions.GetLength(0))
            {
                throw new ArgumentException(
                     "Initial probabilities should have the same length as the number of states in the transition matrix.",
                     "initial");
            }



            this.states = transitions.GetLength(0);

            for (int i = 0; i < states; i++)
                for (int j = 0; j < states; j++)
                    if (Double.IsNaN(transitions[i, j]))
                        throw new ArgumentOutOfRangeException("transitions");

            if (logarithm)
            {
                this.transitions = Matrix.Exp(transitions);
                this.pi = Matrix.Exp(initial);
            }
            else
            {
                this.transitions = transitions;
                this.pi = initial;
            }
        }


        /// <summary>
        ///   Gets the number of states in this topology.
        /// </summary>
        /// 
        public int States
        {
            get { return states; }
        }

        /// <summary>
        ///   Gets the initial state probabilities.
        /// </summary>
        /// 
        public double[] Initial
        {
            get { return pi; }
        }

        /// <summary>
        ///   Gets the state-transitions matrix.
        /// </summary>
        /// 
        public double[,] Transitions
        {
            get { return transitions; }
        }



        /// <summary>
        ///   Creates the state transitions matrix and the
        ///   initial state probabilities for this topology.
        /// </summary>
        /// 
        public int Create(bool logarithm, out double[,] transitionMatrix, out double[] initialState)
        {
            if (logarithm)
            {
                transitionMatrix = Matrix.Log(transitions);
                initialState = Matrix.Log(pi);
            }
            else
            {
                transitionMatrix = (double[,])transitions.Clone();
                initialState = (double[])pi.Clone();
            }

            return states;
        }

    }
}
