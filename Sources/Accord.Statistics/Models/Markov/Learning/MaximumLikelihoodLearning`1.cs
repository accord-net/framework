// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
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
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using System.Collections.Generic;
#pragma warning disable 612, 618

    /// <summary>
    ///    Obsolete. Please use <see cref="MaximumLikelihoodLearning{TDistribution, TObservation}"/> instead.
    /// </summary>
    /// 
    [Obsolete("Please use MaximumLikelihoodLearning<TDistribution, TObservation> instead.")]
    public class MaximumLikelihoodLearning<TDistribution> : ISupervisedLearning
                where TDistribution : IDistribution
    {

        private HiddenMarkovModel<TDistribution> model;
        private bool useLaplaceRule = true;
        private bool useWeights = false;

        private int[] initial;
        private int[,] transitions;


        private IFittingOptions fittingOptions;


        /// <summary>
        ///   Gets the model being trained.
        /// </summary>
        /// 
        public HiddenMarkovModel<TDistribution> Model
        {
            get { return model; }
        }

        /// <summary>
        ///   Gets or sets whether the emission fitting algorithm should
        ///   present weighted samples or simply the clustered samples to
        ///   the <see cref="IDistribution.Fit(System.Array)">density estimation 
        ///   methods</see>.
        /// </summary>
        /// 
        public bool UseWeights
        {
            get { return useWeights; }
            set { useWeights = value; }
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
        ///   Gets or sets the distribution fitting options
        ///   to use when estimating distribution densities
        ///   during learning.
        /// </summary>
        /// <value>The distribution fitting options.</value>
        /// 
        public IFittingOptions FittingOptions
        {
            get { return fittingOptions; }
            set { fittingOptions = value; }
        }

        /// <summary>
        ///   Creates a new instance of the Maximum Likelihood learning algorithm.
        /// </summary>
        /// 
        public MaximumLikelihoodLearning(HiddenMarkovModel<TDistribution> model)
        {
            this.model = model;

            int states = model.States;

            initial = new int[states];
            transitions = new int[states, states];
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
        public double Run(Array[] observations, int[][] paths)
        {
            // Convert the generic representation to a vector of multivariate sequences
            double[][][] obs = observations as double[][][];

            if (obs == null)
            {
                obs = new double[observations.Length][][];
                for (int i = 0; i < observations.Length; i++)
                    obs[i] = convert(observations[i], model.Dimension);
            }

            // Grab model information
            int N = observations.Length;
            int states = model.States;

            Array.Clear(initial, 0, initial.Length);
            Array.Clear(transitions, 0, transitions.Length);


            // 1. Count first state occurrences
            for (int i = 0; i < paths.Length; i++)
                initial[paths[i][0]]++;

            // 2. Count all state transitions
            foreach (int[] path in paths)
                for (int j = 1; j < path.Length; j++)
                    transitions[path[j - 1], path[j]]++;

            if (useWeights)
            {
                int totalObservations = 0;
                for (int i = 0; i < obs.Length; i++)
                    totalObservations += obs[i].Length;

                double[][] weights = new double[states][];
                for (int i = 0; i < weights.Length; i++)
                    weights[i] = new double[totalObservations];

                double[][] all = new double[totalObservations][];

                for (int i = 0, c = 0; i < paths.Length; i++)
                {
                    for (int t = 0; t < paths[i].Length; t++, c++)
                    {
                        int state = paths[i][t];
                        all[c] = obs[i][t];
                        weights[state][c] = 1;
                    }
                }

                for (int i = 0; i < model.States; i++)
                    model.Emissions[i].Fit(all, weights[i], fittingOptions);
            }
            else
            {
                // 3. Count emissions for each state
                List<double[]>[] clusters = new List<double[]>[model.States];
                for (int i = 0; i < clusters.Length; i++)
                    clusters[i] = new List<double[]>();

                // Count symbol frequencies per state
                for (int i = 0; i < paths.Length; i++)
                {
                    for (int t = 0; t < paths[i].Length; t++)
                    {
                        int state = paths[i][t];
                        double[] symbol = obs[i][t];

                        clusters[state].Add(symbol);
                    }
                }

                // Estimate probability distributions
                for (int i = 0; i < model.States; i++)
                    if (clusters[i].Count > 0)
                        model.Emissions[i].Fit(clusters[i].ToArray(), fittingOptions);
            }

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
                }
            }

            // Form probabilities
            int initialCount = initial.Sum();
            int[] transitionCount = transitions.Sum(1);

            if (initialCount == 0)
                initialCount = 1;

            for (int i = 0; i < transitionCount.Length; i++)
                if (transitionCount[i] == 0)
                    transitionCount[i] = 1;


            for (int i = 0; i < initial.Length; i++)
                model.LogInitial[i] = Math.Log(initial[i] / (double)initialCount);

            for (int i = 0; i < transitionCount.Length; i++)
                for (int j = 0; j < states; j++)
                    model.LogTransitions[i][j] = Math.Log(transitions[i, j] / (double)transitionCount[i]);

            Accord.Diagnostics.Debug.Assert(!model.LogInitial.HasNaN());
            Accord.Diagnostics.Debug.Assert(!model.Transitions.HasNaN());


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

        /// <summary>
        ///   Converts a univariate or multivariate array
        ///   of observations into a two-dimensional jagged array.
        /// </summary>
        /// 
        private static double[][] convert(Array array, int dimension)
        {
            double[][] multivariate = array as double[][];
            if (multivariate != null) return multivariate;

            double[] univariate = array as double[];
            if (univariate != null) return Accord.Math.Matrix.Split(univariate, dimension);

            throw new ArgumentException("Invalid array argument type.", "array");
        }

    }
}
