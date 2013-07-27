// Accord Statistics Library
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
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

namespace Accord.Statistics.Models.Markov.Learning
{
    using System;
    using Accord.Math;

    /// <summary>
    ///    Maximum Likelihood learning algorithm for discrete-density Hidden Markov Models.
    /// </summary>
    /// 
    public class MaximumLikelihoodLearning :  ISupervisedLearning
    {

        private HiddenMarkovModel model;
        private bool useLaplaceRule = true;

        /// <summary>
        ///   Gets the model being trained.
        /// </summary>
        /// 
        public HiddenMarkovModel Model
        {
            get { return model; }
        }

        /// <summary>
        ///   Gets or sets whether to use Laplace's rule
        ///   of succession to avoid zero probabilities.
        /// </summary>
        /// 
        public bool UseLaplaceRule
        {
            get { return useLaplaceRule; }
            set { useLaplaceRule = value; }
        }

        /// <summary>
        ///   Creates a new instance of the Maximum Likelihood learning algorithm.
        /// </summary>
        /// 
        public MaximumLikelihoodLearning(HiddenMarkovModel model)
        {
            this.model = model;
        }


        /// <summary>
        ///   Runs the Maximum Likelihood learning algorithm for hidden Markov models.
        /// </summary>
        /// 
        /// <param name="observations">An array of observation sequences to be used to train the model.</param>
        /// <param name="paths">An array of state labels associated to each observation sequence.</param>
        /// 
        /// <returns>
        ///   The average log-likelihood for the observations after the model has been trained.
        /// </returns>
        /// 
        /// <remarks>
        ///   Supervised learning problem. Given some training observation sequences O = {o1, o2, ..., oK},
        ///   known training state paths H = {h1, h2, ..., hK} and general structure of HMM (numbers of 
        ///   hidden and visible states), determine HMM parameters M = (A, B, pi) that best fit training data.
        /// </remarks>
        /// 
        public double Run(int[][] observations, int[][] paths)
        {
            // Grab model information
            int N = observations.Length;
            int states = model.States;
            int symbols = model.Symbols;

            int[] initial = new int[states];
            int[,] transitions = new int[states, states];
            int[,] emissions = new int[states, symbols];

            // 1. Count first state occurances
            for (int i = 0; i < paths.Length; i++)
                initial[paths[i][0]]++;

            // 2. Count all state transitions
            foreach (int[] path in paths)
                for (int j = 1; j < path.Length; j++)
                    transitions[path[j - 1], path[j]]++;

            // 3. Count emissions for each state
            for (int i = 0; i < observations.Length; i++)
                for (int j = 0; j < observations[i].Length; j++)
                    emissions[paths[i][j], observations[i][j]]++;

            // 4. Form log-probabilities, using the Laplace
            //    correction to avoid zero probabilities

            if (useLaplaceRule)
            {
                // Use Laplace's rule of succession correction
                // http://en.wikipedia.org/wiki/Rule_of_succession

                for (int i = 0; i < initial.Length; i++)
                {
                    initial[i]++;

                    for (int j = 0; j < states; j++)
                        transitions[i, j]++;

                    for (int k = 0; k < symbols; k++)
                        emissions[i, k]++;
                }
            }

            // Form probabilities
            int initialCount = initial.Sum();
            int[] transitionCount = transitions.Sum(1);
            int[] emissionCount = emissions.Sum(1);

            for (int i = 0; i < initial.Length; i++)
                model.Probabilities[i] = Math.Log(initial[i] / (double)initialCount);

            for (int i = 0; i < transitionCount.Length; i++)
                for (int j = 0; j < states; j++)
                    model.Transitions[i, j] = Math.Log(transitions[i, j] / (double)transitionCount[i]);

            for (int i = 0; i < emissionCount.Length; i++)
                for (int j = 0; j < symbols; j++)
                    model.Emissions[i, j] = Math.Log(emissions[i, j] / (double)emissionCount[i]);

            System.Diagnostics.Debug.Assert(!model.Probabilities.HasNaN());
            System.Diagnostics.Debug.Assert(!model.Transitions.HasNaN());
            System.Diagnostics.Debug.Assert(!model.Emissions.HasNaN());


            // 5. Compute log-likelihood
            double logLikelihood = Double.NegativeInfinity;
            for (int i = 0; i < observations.Length; i++)
                logLikelihood = Special.LogSum(logLikelihood, model.Evaluate(observations[i]));

            return logLikelihood;
        }

        /// <summary>
        ///   Runs the Maximum Likelihood learning algorithm for hidden Markov models.
        /// </summary>
        /// 
        /// <param name="observations">An array of observation sequences to be used to train the model.</param>
        /// <param name="paths">An array of state labels associated to each observation sequence.</param>
        /// 
        /// <returns>
        ///   The average log-likelihood for the observations after the model has been trained.
        /// </returns>
        /// 
        /// <remarks>
        ///   Supervised learning problem. Given some training observation sequences O = {o1, o2, ..., oK},
        ///   known training state paths H = {h1, h2, ..., hK} and general structure of HMM (numbers of 
        ///   hidden and visible states), determine HMM parameters M = (A, B, pi) that best fit training data.
        /// </remarks>
        /// 
        double ISupervisedLearning.Run(Array[] observations, int[][] paths)
        {
            return Run(observations as int[][], paths);
        }


    }
}
