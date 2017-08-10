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
    using Accord.Compat;
    using System.Threading;

    /// <summary>
    ///   Base class for observable Markov model learning algorithms.
    /// </summary>
    /// 
    public abstract class BaseMaximumLikelihoodLearning<TModel, TDistribution, TObservation, TOptions>
        where TDistribution : IFittableDistribution<TObservation>
        where TModel : HiddenMarkovModel<TDistribution, TObservation>
        where TOptions : IFittingOptions
    {
        [NonSerialized]
        CancellationToken token = new CancellationToken();

        private TModel model;
        private bool useLaplaceRule = true;
        private bool useWeights = false;

        private TOptions fittingOptions;

        /// <summary>
        ///   Gets or sets a cancellation token that can be used to
        ///   stop the learning algorithm while it is running.
        /// </summary>
        /// 
        public CancellationToken Token
        {
            get { return token; }
            set { token = value; }
        }

        /// <summary>
        ///   Gets the model being trained.
        /// </summary>
        /// 
        public TModel Model
        {
            get { return model; }
            set { model = value; }
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
        ///   Gets or sets the function that initializes the emission
        ///   distributions in the hidden Markov Models.
        /// </summary>
        /// 
        public Func<int, TDistribution> Emissions { get; set; }

        /// <summary>
        ///   Gets or sets the distribution fitting options
        ///   to use when estimating distribution densities
        ///   during learning.
        /// </summary>
        /// <value>The distribution fitting options.</value>
        /// 
        public TOptions FittingOptions
        {
            get { return fittingOptions; }
            set { fittingOptions = value; }
        }

        /// <summary>
        ///   Creates a new instance of the Maximum Likelihood learning algorithm.
        /// </summary>
        /// 
        public BaseMaximumLikelihoodLearning()
        {
        }


        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair (if supported by the learning algorithm).</param>
        /// <returns>A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.</returns>
        public TModel Learn(TObservation[][] x, int[][] y, double[] weights = null)
        {
            if (weights != null)
                throw new ArgumentException(Accord.Properties.Resources.NotSupportedWeights, "weights");

            if (Model == null)
                Model = Create(x, numberOfClasses: y.Max() + 1);

            MarkovHelperMethods.CheckObservationDimensions(x, Model);

            // Grab model information
            var model = Model;
            var fittingOptions = FittingOptions;

            int N = x.Length;
            int states = model.NumberOfStates;

            int[] initial = new int[states];
            int[,] transitions = new int[states, states];


            // 1. Count first state occurrences
            for (int i = 0; i < y.Length; i++)
                initial[y[i][0]]++;

            // 2. Count all state transitions
            foreach (int[] path in y)
                for (int j = 1; j < path.Length; j++)
                    transitions[path[j - 1], path[j]]++;

            if (UseWeights)
            {
                int totalObservations = 0;
                for (int i = 0; i < x.Length; i++)
                    totalObservations += x[i].Length;

                double[][] totalWeights = new double[states][];
                for (int i = 0; i < totalWeights.Length; i++)
                    totalWeights[i] = new double[totalObservations];

                var all = new TObservation[totalObservations];

                for (int i = 0, c = 0; i < y.Length; i++)
                {
                    for (int t = 0; t < y[i].Length; t++, c++)
                    {
                        int state = y[i][t];
                        all[c] = x[i][t];
                        totalWeights[state][c] = 1;
                    }
                }

                for (int i = 0; i < model.NumberOfStates; i++)
                    model.Emissions[i].Fit(all, totalWeights[i], fittingOptions);
            }
            else
            {
                // 3. Count emissions for each state
                var clusters = new List<TObservation>[model.NumberOfStates];
                for (int i = 0; i < clusters.Length; i++)
                    clusters[i] = new List<TObservation>();

                // Count symbol frequencies per state
                for (int i = 0; i < y.Length; i++)
                {
                    for (int t = 0; t < y[i].Length; t++)
                    {
                        int state = y[i][t];
                        var symbol = x[i][t];

                        clusters[state].Add(symbol);
                    }
                }

                // Estimate probability distributions
                for (int i = 0; i < model.NumberOfStates; i++)
                    if (clusters[i].Count > 0)
                        model.Emissions[i].Fit(clusters[i].ToArray(), fittingOptions);
            }

            // 4. Form log-probabilities, using the Laplace
            //    correction to avoid zero probabilities

            if (UseLaplaceRule)
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
            Accord.Diagnostics.Debug.Assert(!model.LogTransitions.HasNaN());

            return Model;
        }

        /// <summary>
        ///   Creates an instance of the model to be learned. Inheritors of this abstract 
        ///   class must define this method so new models can be created from the training data.
        /// </summary>
        /// 
        protected abstract TModel Create(TObservation[][] x, int numberOfClasses);
    }
}
