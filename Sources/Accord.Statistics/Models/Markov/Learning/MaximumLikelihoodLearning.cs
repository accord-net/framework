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
    using Accord.MachineLearning;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Compat;
    using System.Threading;

#pragma warning disable 612, 618

    /// <summary>
    ///    Maximum Likelihood learning algorithm for <see cref="HiddenMarkovModel">
    ///    discrete-density Hidden Markov Models</see>.
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
    /// <code source="Unit Tests\Accord.Tests.Statistics\Models\Markov\MaximumLikelihoodLearningTest.cs" region="doc_learn"/>
    /// </example>
    /// 
    /// <seealso cref="MaximumLikelihoodLearning"/>
    /// <seealso cref="ViterbiLearning"/>
    /// <seealso cref="HiddenMarkovModel"/>
    /// 
    public class MaximumLikelihoodLearning :
        BaseMaximumLikelihoodLearning<HiddenMarkovModel, GeneralDiscreteDistribution, int, GeneralDiscreteOptions>,
        ISupervisedLearning, ISupervisedLearning<HiddenMarkovModel, int[], int[]>
    {

        /// <summary>
        ///   Creates a new instance of the Maximum Likelihood learning algorithm.
        /// </summary>
        /// 
        public MaximumLikelihoodLearning(HiddenMarkovModel model)
        {
            this.Model = model;
        }

        /// <summary>
        ///   Creates a new instance of the Maximum Likelihood learning algorithm.
        /// </summary>
        /// 
        public MaximumLikelihoodLearning()
        {
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
        [Obsolete("Please use Learn(x, y) instead.")]
        public double Run(int[][] observations, int[][] paths)
        {
            run(observations, paths);

            // 5. Compute log-likelihood
            double logLikelihood = Double.NegativeInfinity;
            for (int i = 0; i < observations.Length; i++)
                logLikelihood = Special.LogSum(logLikelihood, Model.Evaluate(observations[i]));

            return logLikelihood;
        }

        void run(int[][] observations, int[][] paths)
        {
            // Grab model information
            var model = Model;
            int N = observations.Length;
            int states = model.States;
            int symbols = model.Symbols;

            int[] initial = new int[states];
            int[,] transitions = new int[states, states];
            int[,] emissions = new int[states, symbols];

            // 1. Count first state occurrences
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

            if (UseLaplaceRule)
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

            if (initialCount == 0)
                initialCount = 1;

            for (int i = 0; i < transitionCount.Length; i++)
            {
                if (transitionCount[i] == 0)
                    transitionCount[i] = 1;
            }

            for (int i = 0; i < emissionCount.Length; i++)
            {
                if (emissionCount[i] == 0)
                    emissionCount[i] = 1;
            }

            for (int i = 0; i < initial.Length; i++)
                model.LogInitial[i] = Math.Log(initial[i] / (double)initialCount);

            for (int i = 0; i < transitionCount.Length; i++)
                for (int j = 0; j < states; j++)
                    model.LogTransitions[i][j] = Math.Log(transitions[i, j] / (double)transitionCount[i]);

            for (int i = 0; i < emissionCount.Length; i++)
                for (int j = 0; j < symbols; j++)
                    model.LogEmissions[i][j] = Math.Log(emissions[i, j] / (double)emissionCount[i]);

            Accord.Diagnostics.Debug.Assert(!model.LogInitial.HasNaN());
            Accord.Diagnostics.Debug.Assert(!model.LogTransitions.HasNaN());
            Accord.Diagnostics.Debug.Assert(!model.Emissions.HasNaN());
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
        ///   Creates an instance of the model to be learned. Inheritors of this abstract 
        ///   class must define this method so new models can be created from the training data.
        /// </summary>
        /// 
        protected override HiddenMarkovModel Create(int[][] x, int numberOfClasses)
        {
            return new HiddenMarkovModel(states: numberOfClasses, symbols: x.Max() + 1);
        }
    }
}
