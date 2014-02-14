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

namespace Accord.Statistics.Models.Markov.Hybrid
{
    using System;
    using Accord.Math;

    /// <summary>
    ///   General Markov function for arbitrary state-emission density definitions.
    /// </summary>
    /// 
    /// <param name="previous">The previous state index.</param>
    /// <param name="observation">The observation at the current state.</param>
    /// <returns>An array containing the values for the observations in each next possible state.</returns>
    /// 
    public delegate double[] GeneralMarkovFunction(int previous, double[] observation);

    /// <summary>
    ///   Hybrid Markov model for arbitrary state-observation functions.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class can be used to implement HMM hybrids such as ANN-HMM
    ///   or SVM-HMMs through the specification of a custom <see cref="GeneralMarkovFunction"/>.
    /// </remarks>
    /// 
    public class HybridMarkovModel
    {

        /// <summary>
        ///   Gets the Markov function, which takes the previous state, the
        ///   next state and a observation and produces a probability value.
        /// </summary>
        /// 
        public GeneralMarkovFunction Function { get; private set; }

        /// <summary>
        ///   Gets the number of states in the model.
        /// </summary>
        /// 
        public int States { get; private set; }

        /// <summary>
        ///   Gets the number of dimensions of the 
        ///   observations handled by this model.
        /// </summary>
        /// 
        public int Dimension { get; private set; }

        /// <summary>
        ///   Gets or sets an user-defined object associated with this model.
        /// </summary>
        /// 
        public object Tag { get; set; }

        /// <summary>
        ///    Initializes a new instance of the <see cref="HybridMarkovModel"/> class.
        /// </summary>
        /// 
        /// <param name="function">A function specifying a probability for a transition-emission pair.</param>
        /// <param name="states">The number of states in the model.</param>
        /// <param name="dimension">The number of dimensions in the model.</param>
        /// 
        public HybridMarkovModel(GeneralMarkovFunction function, int states, int dimension)
        {
            this.Function = function;
            this.States = states;
            this.Dimension = dimension;
        }



        /// <summary>
        ///   Calculates the most likely sequence of hidden states
        ///   that produced the given observation sequence.
        /// </summary>
        /// <remarks>
        ///   Decoding problem. Given the HMM M = (A, B, pi) and  the observation sequence 
        ///   O = {o1,o2, ..., oK}, calculate the most likely sequence of hidden states Si
        ///   that produced this observation sequence O. This can be computed efficiently
        ///   using the Viterbi algorithm.
        /// </remarks>
        /// <param name="observations">
        ///   A sequence of observations.</param>
        /// <param name="logLikelihood">
        ///   The state optimized probability.</param>
        /// <returns>
        ///   The sequence of states that most likely produced the sequence.
        /// </returns>
        /// 
        public int[] Decode(double[][] observations, out double logLikelihood)
        {
            if (observations == null)
                throw new ArgumentNullException("observations");

            if (observations.Length == 0)
            {
                logLikelihood = Double.NegativeInfinity;
                return new int[0];
            }


            // Viterbi-forward algorithm.
            int T = observations.Length;
            int states = States;
            int maxState;
            double maxWeight;
            double weight;

            int[,] s = new int[states, T];
            double[,] lnFwd = new double[states, T];
            double[][] output = new double[states][];


            // Base
            output[0] = Function(-1, observations[0]);
            for (int i = 0; i < states; i++)
                lnFwd[i, 0] = output[0][i];

            // Induction
            for (int t = 1; t < T; t++)
            {
                double[] observation = observations[t];

                for (int i = 0; i < States; i++)
                    output[i] = Function(i, observation);

                for (int j = 0; j < states; j++)
                {
                    maxState = 0;
                    maxWeight = lnFwd[0, t - 1] + output[0][j];

                    for (int i = 1; i < states; i++)
                    {
                        weight = lnFwd[i, t - 1] + output[i][j];

                        if (weight > maxWeight)
                        {
                            maxState = i;
                            maxWeight = weight;
                        }
                    }

                    lnFwd[j, t] = maxWeight;
                    s[j, t] = maxState;
                }
            }


            // Find maximum value for time T-1
            maxState = 0;
            maxWeight = lnFwd[0, T - 1];

            for (int i = 1; i < states; i++)
            {
                if (lnFwd[i, T - 1] > maxWeight)
                {
                    maxState = i;
                    maxWeight = lnFwd[i, T - 1];
                }
            }


            // Trackback
            int[] path = new int[T];
            path[T - 1] = maxState;

            for (int t = T - 2; t >= 0; t--)
                path[t] = s[path[t + 1], t + 1];


            // Returns the sequence probability as an out parameter
            logLikelihood = maxWeight;

            // Returns the most likely (Viterbi path) for the given sequence
            return path;
        }

        /// <summary>
        ///   Calculates the probability that this model has generated the given sequence.
        /// </summary>
        /// <remarks>
        ///   Evaluation problem. Given the HMM  M = (A, B, pi) and  the observation
        ///   sequence O = {o1, o2, ..., oK}, calculate the probability that model
        ///   M has generated sequence O. This can be computed efficiently using the
        ///   Forward algorithm. </remarks>
        /// <param name="observations">
        ///   A sequence of observations. </param>
        /// <returns>
        ///   The probability that the given sequence has been generated by this model.
        /// </returns>
        /// 
        public double Evaluate(double[][] observations)
        {
            int T = observations.Length;
            double[,] lnFwd = new double[States, T];
            double[][] output = new double[States][];

            // 1. Initialization
            output[0] = Function(-1, observations[0]);
            for (int i = 0; i < States; i++)
                lnFwd[0, i] = Math.Log(output[0][i]);

            // 2. Induction
            for (int t = 1; t < T; t++)
            {
                double[] x = observations[t];

                for (int i = 0; i < States; i++)
                    output[i] = Function(i, x);

                for (int i = 0; i < States; i++)
                {
                    double sum = Double.NegativeInfinity;
                    for (int j = 0; j < States; j++)
                        sum = Special.LogSum(sum, lnFwd[t - 1, j] + output[j][i]);
                    lnFwd[t, i] = sum;
                }
            }

            double logLikelihood = Double.NegativeInfinity;
            for (int i = 0; i < States; i++)
                logLikelihood = Special.LogSum(logLikelihood, lnFwd[T - 1, i]);

            return logLikelihood;
        }

    }
}
