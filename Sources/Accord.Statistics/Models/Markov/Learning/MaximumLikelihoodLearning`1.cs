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

namespace Accord.Statistics.Models.Markov.Learning
{
    using System;
    using Accord.Math;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using System.Collections.Generic;

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
    /// <code>
    /// // Those are the observation sequences. Each sequence contains a variable number
    /// // of observation (although in this example they have all the same length, this
    /// // is just a coincidence and not something required).
    /// 
    /// int[][] observations = 
    /// {
    ///     new int[] { 0,0,0,1,0,0 }, 
    ///     new int[] { 1,0,0,1,0,0 },
    ///     new int[] { 0,0,1,0,0,0 },
    ///     new int[] { 0,0,0,0,1,0 },
    ///     new int[] { 1,0,0,0,1,0 },
    ///     new int[] { 0,0,0,1,1,0 },
    ///     new int[] { 1,0,0,0,0,0 },
    ///     new int[] { 1,0,1,0,0,0 },
    /// };
    /// 
    /// // Now those are the visible states associated with each observation in each 
    /// // observation sequence above. Note that there is always one state assigned
    /// // to each observation, so the lengths of the sequence of observations and 
    /// // the sequence of states must always match.
    /// 
    /// int[][] paths = 
    /// {
    ///     new int[] { 0,0,1,0,1,0 },
    ///     new int[] { 1,0,1,0,1,0 },
    ///     new int[] { 1,0,0,1,1,0 },
    ///     new int[] { 1,0,1,1,1,0 },
    ///     new int[] { 1,0,0,1,0,1 },
    ///     new int[] { 0,0,1,0,0,1 },
    ///     new int[] { 0,0,1,1,0,1 },
    ///     new int[] { 0,1,1,1,0,0 },
    /// };
    /// 
    /// // Since the observation sequences are composed of discrete symbols, we can specify
    /// // a GeneralDiscreteDistribution to simulate a standard discrete HiddenMarkovModel.
    /// var initial = new GeneralDiscreteDistribution(symbols: 2);
    /// 
    /// // Create our Markov model with two states (0, 1) and two symbols (0, 1)
    /// HiddenMarkovModel model = new HiddenMarkovModel&lt;(states: 2, symbols: 2);
    /// 
    /// // Now we can create our learning algorithm
    /// MaximumLikelihoodLearning teacher = new MaximumLikelihoodLearning(model)
    /// {
    ///    // Set some options
    ///    UseLaplaceRule = false
    /// };
    /// 
    /// // and finally learn a model using the algorithm
    /// double logLikelihood = teacher.Run(observations, paths);
    /// 
    /// 
    /// // To check what has been learned, we can extract the emission
    /// // and transition matrices, as well as the initial probability
    /// // vector from the HMM to compare against expected values:
    /// 
    /// var pi = Matrix.Exp(model.Probabilities); // { 0.5, 0.5 }
    /// var A = Matrix.Exp(model.Transitions);    // { { 7/20, 13/20 }, { 14/20, 6/20 } }
    /// var B = Matrix.Exp(model.Emissions);      // { { 17/25, 8/25 }, { 19/23, 4/23 } }
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="MaximumLikelihoodLearning"/>
    /// <seealso cref="ViterbiLearning{TDistribution}"/>
    /// <seealso cref="HiddenMarkovModel{TDistribution}"/>
    /// 
    public class MaximumLikelihoodLearning<TDistribution> : ISupervisedLearning
                where TDistribution : IDistribution
    {

        private HiddenMarkovModel<TDistribution> model;
        private bool useLaplaceRule = true;

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

            int[] initial = new int[states];
            int[,] transitions = new int[states, states];

            // 1. Count first state occurrences
            for (int i = 0; i < paths.Length; i++)
                initial[paths[i][0]]++;

            // 2. Count all state transitions
            foreach (int[] path in paths)
                for (int j = 1; j < path.Length; j++)
                    transitions[path[j - 1], path[j]]++;

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
                    model.Emissions[i].Fit(clusters[i].ToArray(), null, fittingOptions);

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

            for (int i = 0; i < initial.Length; i++)
                model.Probabilities[i] = Math.Log(initial[i] / (double)initialCount);

            for (int i = 0; i < transitionCount.Length; i++)
                for (int j = 0; j < states; j++)
                    model.Transitions[i, j] = Math.Log(transitions[i, j] / (double)transitionCount[i]);

            System.Diagnostics.Debug.Assert(!model.Probabilities.HasNaN());
            System.Diagnostics.Debug.Assert(!model.Transitions.HasNaN());


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
