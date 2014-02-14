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
    ///   Forward Topology for Hidden Markov Models.
    /// </summary>
    /// 
    /// <remarks>
    ///  <para>
    ///  Forward topologies are commonly used to initialize models in which
    ///  training sequences can be organized in samples, such as in the recognition
    ///  of spoken words. In spoken word recognition, several examples of a single
    ///  word can (and should) be used to train a single model, to achieve the most
    ///  general model able to generalize over a great number of word samples.</para>
    ///  
    ///  <para>
    ///  Forward models can typically have a large number of states.</para>
    ///  
    ///  <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Alexander Schliep, "Learning Hidden Markov Model Topology".</description></item>
    ///     <item><description>
    ///       Richard Hughey and Anders Krogh, "Hidden Markov models for sequence analysis: 
    ///       extension and analysis of the basic method", CABIOS 12(2):95-107, 1996. Available in:
    ///       http://compbio.soe.ucsc.edu/html_format_papers/hughkrogh96/cabios.html</description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <seealso cref="HiddenMarkovModel"/>
    /// <seealso cref="Accord.Statistics.Models.Markov.Topology.ITopology"/>
    /// <seealso cref="Accord.Statistics.Models.Markov.Topology.Ergodic"/>
    /// <seealso cref="Accord.Statistics.Models.Markov.Topology.Custom"/>
    ///
    /// 
    /// <example>
    ///  <para>
    ///   In the following example, we will create a Forward-only
    ///   discrete-density hidden Markov model.</para>
    ///   
    ///   <code>
    ///   // Create a new Forward-only hidden Markov model with
    ///   // three forward-only states and four sequence symbols.
    ///   var model = new HiddenMarkovModel(new Forward(3), 4);
    ///
    ///   // After creation, the state transition matrix for the model
    ///   // should be given by:
    ///   //
    ///   //       { 0.33, 0.33, 0.33 }
    ///   //       { 0.00, 0.50, 0.50 }
    ///   //       { 0.00, 0.00, 1.00 }
    ///   //       
    ///   // in which no backward transitions are allowed (have zero probability).
    ///   </code>
    /// </example>
    /// 
    [Serializable]
    public class Forward : ITopology
    {
        private int states;
        private int deepness;
        private bool random;
        private double[] pi;


        /// <summary>
        ///   Gets the number of states in this topology.
        /// </summary>
        public int States
        {
            get { return states; }
        }

        /// <summary>
        ///   Gets or sets the maximum deepness level allowed
        ///   for the forward state transition chains.
        /// </summary>
        public int Deepness
        {
            get { return deepness; }
            set { deepness = value; }
        }

        /// <summary>
        ///   Gets or sets whether the transition matrix
        ///   should be initialized with random probabilities
        ///   or not. Default is false.
        /// </summary>
        public bool Random
        {
            get { return random; }
            set { random = value; }
        }

        /// <summary>
        ///   Gets the initial state probabilities.
        /// </summary>
        public double[] Initial
        {
            get { return pi; }
        }

        /// <summary>
        ///   Creates a new Forward topology for a given number of states.
        /// </summary>
        public Forward(int states)
            : this(states, states, false)
        {
        }

        /// <summary>
        ///   Creates a new Forward topology for a given number of states.
        /// </summary>
        public Forward(int states, int deepness)
            : this(states, deepness, false)
        {
        }

        /// <summary>
        ///   Creates a new Forward topology for a given number of states.
        /// </summary>
        public Forward(int states, bool random)
            : this(states, states, random)
        {
        }

        /// <summary>
        ///   Creates a new Forward topology for a given number of states.
        /// </summary>
        public Forward(int states, int deepness, bool random)
        {
            if (states <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "states", "Number of states should be higher than zero.");
            }

            if (deepness > states)
            {
                throw new ArgumentOutOfRangeException(
                    "states", "Deepness level should be lesser or equal to the number of states.");
            }


            this.states = states;
            this.deepness = deepness;
            this.random = random;

            this.pi = new double[states];
            this.pi[0] = 1.0;
        }


        /// <summary>
        ///   Creates the state transitions matrix and the
        ///   initial state probabilities for this topology.
        /// </summary>
        public int Create(bool logarithm, out double[,] transitionMatrix, out double[] initialState)
        {
            int m = System.Math.Min(States, Deepness);
            double[,] A = new double[States, States];

            if (random)
            {
                // Create A using random uniform distribution,
                //  without allowing backward transitions

                for (int i = 0; i < states; i++)
                {
                    double sum = 0.0;
                    for (int j = i; j < m; j++)
                        sum += A[i, j] = Accord.Math.Tools.Random.NextDouble();

                    for (int j = i; j < m; j++)
                        A[i, j] /= sum;
                }
            }
            else
            {
                // Create A using equal uniform probabilities,
                //   without allowing backward transitions.

                for (int i = 0; i < states; i++)
                {
                    double d = 1.0 / Math.Min(m, states - i);
                    for (int j = i; j < states && (j - i) < m; j++)
                        A[i, j] = d;
                }
            }

            if (logarithm)
            {
                transitionMatrix = Matrix.Log(A);
                initialState = Matrix.Log(pi);
            }
            else
            {
                transitionMatrix = A;
                initialState = (double[])pi.Clone();
            }

            return States;
        }

    }
}
