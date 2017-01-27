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
    using Accord.MachineLearning;
using System.Threading;

    /// <summary>
    ///    Maximum Likelihood learning algorithm for discrete-density Hidden Markov Models.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The maximum likelihood estimate is a <see cref="ISupervisedLearning">
    ///   supervised learning algorithm</see>. It considers both the sequence
    ///   of observations as well as the sequence of states in the Markov model
    ///   are visible and thus during training. </para>
    ///   
    /// <para>
    ///   Often, the Maximum Likelihood Estimate can be used to give a starting
    ///   point to a unsupervised algorithm, making possible to use semi-supervised
    ///   techniques with HMMs. It is possible, for example, to use MLE to guess
    ///   initial values for an HMM given a small set of manually labeled labels,
    ///   and then further estimate this model using the <see cref="ViterbiLearning">
    ///   Viterbi learning algorithm</see>.</para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   The following example comes from Prof. Yechiam Yemini slides on Hidden Markov
    ///   Models, available at http://www.cs.columbia.edu/4761/notes07/chapter4.3-HMM.pdf.
    ///   In this example, we will be specifying both the sequence of observations and
    ///   the sequence of states assigned to each observation in each sequence to learn
    ///   our Markov model.
    /// </para>
    /// 
    /// <code source="Unit Tests\Accord.Tests.Statistics\Models\Markov\MaximumLikelihoodLearning`1Test.cs" region="doc_learn"/>
    /// </example>
    /// 
    /// <seealso cref="MaximumLikelihoodLearning"/>
    /// <seealso cref="ViterbiLearning{TDistribution}"/>
    /// <seealso cref="HiddenMarkovModel{TDistribution}"/>
    /// 
    public class MaximumLikelihoodLearning<TDistribution, TObservation> :
        ISupervisedLearning<HiddenMarkovModel<TDistribution, TObservation>, TObservation[], int[]>
        where TDistribution : IFittableDistribution<TObservation>
    {
        [NonSerialized]
        CancellationToken token = new CancellationToken();

        private HiddenMarkovModel<TDistribution, TObservation> model;
        private bool useLaplaceRule = true;
        private bool useWeights = false;

        private int[] initial;
        private int[,] transitions;


        private IFittingOptions fittingOptions;

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
        public HiddenMarkovModel<TDistribution, TObservation> Model
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
        public MaximumLikelihoodLearning(HiddenMarkovModel<TDistribution, TObservation> model)
        {
            this.model = model;

            int states = model.NumberOfStates;

            initial = new int[states];
            transitions = new int[states, states];
        }


        /// <summary>
        /// Learns a model that can map the given inputs to the given outputs.
        /// </summary>
        /// <param name="x">The model inputs.</param>
        /// <param name="y">The desired outputs associated with each <paramref name="x">inputs</paramref>.</param>
        /// <param name="weights">The weight of importance for each input-output pair.</param>
        /// <returns>A model that has learned how to produce <paramref name="y" /> given <paramref name="x" />.</returns>
        public HiddenMarkovModel<TDistribution, TObservation> Learn(TObservation[][] x, int[][] y, double[] weights = null)
        {
            // Grab model information
            int N = x.Length;
            int states = model.NumberOfStates;

            Array.Clear(initial, 0, initial.Length);
            Array.Clear(transitions, 0, transitions.Length);


            // 1. Count first state occurrences
            for (int i = 0; i < y.Length; i++)
                initial[y[i][0]]++;

            // 2. Count all state transitions
            foreach (int[] path in y)
                for (int j = 1; j < path.Length; j++)
                    transitions[path[j - 1], path[j]]++;

            if (useWeights)
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
            Accord.Diagnostics.Debug.Assert(!model.LogTransitions.HasNaN());

            return model;
        }


    }
}
