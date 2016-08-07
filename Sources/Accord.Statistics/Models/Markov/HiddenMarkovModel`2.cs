// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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

namespace Accord.Statistics.Models.Markov
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Accord.Math;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Models.Markov.Learning;
    using Accord.Statistics.Models.Markov.Topology;
    using Accord.MachineLearning;

    /// <summary>
    ///   Arbitrary-density Hidden Markov Model.
    /// </summary>
    /// 
    [Serializable]
    public class HiddenMarkovModel<TDistribution, TObservation> : 
        LikelihoodTaggerBase<TObservation>, ICloneable
        where TDistribution : IDistribution<TObservation>
    {
        // Model is defined as M = (A, B, pi)
        private TDistribution[] B; // emission probabilities

        // Model is defined as M = (A, B, pi)
        private double[][] logA; // Transition probabilities
        private double[] logPi; // Initial state probabilities

        private int states;  // number of states
        private object tag;


        /// <summary>
        ///   Constructs a new Hidden Markov Model.
        /// </summary>
        /// 
        protected HiddenMarkovModel(ITopology topology)
        {
            double[,] a;
            this.states = topology.Create(true, out a, out logPi);
            logA = a.ToJagged();

            NumberOfInputs = 1;
        }

        /// <summary>
        ///   Constructs a new Hidden Markov Model.
        /// </summary>
        /// 
        protected HiddenMarkovModel()
        {
            NumberOfInputs = 1;
        }



        /// <summary>
        ///   Gets the number of states of this model.
        /// </summary>
        /// 
        public int States
        {
            get { return this.states; }
        }

        /// <summary>
        ///   Gets the log-initial probabilities <c>log(pi)</c> for this model.
        /// </summary>
        /// 
        public double[] LogInitial
        {
            get { return this.logPi; }
        }

        /// <summary>
        ///   Gets the log-transition matrix <c>log(A)</c> for this model.
        /// </summary>
        /// 
        public double[][] LogTransitions
        {
            get { return this.logA; }
        }

        /// <summary>
        ///   Gets or sets a user-defined tag associated with this model.
        /// </summary>
        /// 
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        /// <summary>
        ///   Constructs a new Hidden Markov Model with arbitrary-density state probabilities.
        /// </summary>
        /// 
        /// <param name="topology">
        ///   A <see cref="Topology"/> object specifying the initial values of the matrix of transition 
        ///   probabilities <c>A</c> and initial state probabilities <c>pi</c> to be used by this model.
        /// </param>
        /// <param name="emissions">
        ///   The initial emission probability distribution to be used by each of the states. This
        ///   initial probability distribution will be cloned across all states.
        /// </param>
        /// 
        public HiddenMarkovModel(ITopology topology, TDistribution emissions)
            : this(topology)
        {
            if (emissions == null)
                throw new ArgumentNullException("emissions");

            // Initialize B using the initial distribution
            B = new TDistribution[States];

            for (int i = 0; i < B.Length; i++)
                B[i] = (TDistribution)emissions.Clone();


            if (emissions is IMultivariateDistribution)
                NumberOfInputs = ((IMultivariateDistribution)B[0]).Dimension;
        }


        /// <summary>
        ///   Constructs a new Hidden Markov Model with arbitrary-density state probabilities.
        /// </summary>
        /// 
        /// <param name="topology">
        ///   A <see cref="Topology"/> object specifying the initial values of the matrix of transition 
        ///   probabilities <c>A</c> and initial state probabilities <c>pi</c> to be used by this model.
        /// </param>
        /// <param name="emissions">
        ///   The initial emission probability distributions for each state.
        /// </param>
        /// 
        public HiddenMarkovModel(ITopology topology, TDistribution[] emissions)
            : this(topology)
        {
            if (emissions == null)
                throw new ArgumentNullException("emissions");

            if (emissions.Length != States)
            {
                throw new ArgumentException(
                    "The emission matrix should have the same number of rows as the number of states in the model.",
                    "emissions");
            }

            B = emissions;

            // Assume all emissions have same form
            if (B[0] is IMultivariateDistribution)
                NumberOfInputs = ((IMultivariateDistribution)B[0]).Dimension;
        }


        /// <summary>
        ///   Constructs a new Hidden Markov Model with arbitrary-density state probabilities.
        /// </summary>
        /// 
        /// <param name="transitions">The transitions matrix A for this model.</param>
        /// <param name="emissions">The emissions matrix B for this model.</param>
        /// <param name="probabilities">The initial state probabilities for this model.</param>
        /// <param name="logarithm">Set to true if the matrices are given with logarithms of the
        /// intended probabilities; set to false otherwise. Default is false.</param>
        /// 
        public HiddenMarkovModel(double[][] transitions, TDistribution[] emissions, double[] probabilities, bool logarithm = false)
            : this(new Custom(transitions.ToMatrix(), probabilities, logarithm), emissions) { }

        /// <summary>
        ///   Constructs a new Hidden Markov Model with arbitrary-density state probabilities.
        /// </summary>
        /// 
        /// <param name="transitions">The transitions matrix A for this model.</param>
        /// <param name="emissions">The emissions matrix B for this model.</param>
        /// <param name="probabilities">The initial state probabilities for this model.</param>
        /// <param name="logarithm">Set to true if the matrices are given with logarithms of the
        /// intended probabilities; set to false otherwise. Default is false.</param>
        /// 
        public HiddenMarkovModel(double[,] transitions, TDistribution[] emissions, double[] probabilities, bool logarithm = false)
            : this(new Custom(transitions, probabilities, logarithm), emissions) { }


        /// <summary>
        ///   Constructs a new Hidden Markov Model with arbitrary-density state probabilities.
        /// </summary>
        /// 
        /// <param name="states">The number of states for the model.</param>
        /// <param name="emissions">A initial distribution to be copied to all states in the model.</param>
        /// 
        public HiddenMarkovModel(int states, TDistribution emissions)
            : this(new Topology.Ergodic(states), emissions) { }



        /// <summary>
        ///   Gets the Emission matrix (B) for this model.
        /// </summary>
        /// 
        public TDistribution[] Emissions
        {
            get { return this.B; }
            protected set { this.B = value; }
        }






        /// <summary>
        ///   Calculates the most likely sequence of hidden states
        ///   that produced the given observation sequence.
        /// </summary>
        /// 
        /// <remarks>
        ///   Decoding problem. Given the HMM M = (A, B, pi) and  the observation sequence 
        ///   O = {o1,o2, ..., oK}, calculate the most likely sequence of hidden states Si
        ///   that produced this observation sequence O. This can be computed efficiently
        ///   using the Viterbi algorithm.
        /// </remarks>
        /// 
        /// <param name="observations">A sequence of observations.</param>
        /// 
        /// <returns>The sequence of states that most likely produced the sequence.</returns>
        /// 
        public int[] Decode(TObservation[] observations)
        {
            double logLikelihood;
            return Decode(observations, out logLikelihood);
        }


        /// <summary>
        ///   Calculates the most likely sequence of hidden states
        ///   that produced the given observation sequence.
        /// </summary>
        /// 
        /// <remarks>
        ///   Decoding problem. Given the HMM M = (A, B, pi) and  the observation sequence 
        ///   O = {o1,o2, ..., oK}, calculate the most likely sequence of hidden states Si
        ///   that produced this observation sequence O. This can be computed efficiently
        ///   using the Viterbi algorithm.
        /// </remarks>
        /// 
        /// <param name="observations">A sequence of observations.</param>
        /// <param name="logLikelihood">The log-likelihood along the most likely sequence.</param>
        /// 
        /// <returns>The sequence of states that most likely produced the sequence.</returns>
        /// 
        public int[] Decode(TObservation[] observations, out double logLikelihood)
        {
            if (observations == null)
                throw new ArgumentNullException("observations");

            if (observations.Length == 0)
            {
                logLikelihood = Double.NegativeInfinity;
                return new int[0];
            }

            return viterbi(observations, out logLikelihood);
        }

        private int[] viterbi(TObservation[] x, out double logLikelihood)
        {
            // Viterbi-forward algorithm.
            int T = x.Length;
            int states = States;
            int maxState;
            double maxWeight;
            double weight;

            double[] logPi = LogInitial;
            double[][] logA = LogTransitions;

            int[,] s = new int[states, T];
            double[][] lnFwd = Jagged.Zeros(states, T);


            // Base
            for (int i = 0; i < states; i++)
                lnFwd[i][0] = logPi[i] + B[i].LogProbabilityFunction(x[0]);

            // Induction
            for (int t = 1; t < T; t++)
            {
                TObservation observation = x[t];

                for (int j = 0; j < states; j++)
                {
                    maxState = 0;
                    maxWeight = lnFwd[0][t - 1] + logA[0][j];

                    for (int i = 1; i < states; i++)
                    {
                        weight = lnFwd[i][t - 1] + logA[i][j];

                        if (weight > maxWeight)
                        {
                            maxState = i;
                            maxWeight = weight;
                        }
                    }

                    lnFwd[j][t] = maxWeight + B[j].LogProbabilityFunction(observation);
                    s[j, t] = maxState;
                }
            }

            // Find maximum value for time T-1
            maxState = 0;
            maxWeight = lnFwd[0][T - 1];

            for (int i = 1; i < states; i++)
            {
                if (lnFwd[i][T - 1] > maxWeight)
                {
                    maxState = i;
                    maxWeight = lnFwd[i][T - 1];
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
        ///   Calculates the probability of each hidden state for each
        ///   observation in the observation vector.
        /// </summary>
        /// 
        /// <remarks>
        ///   If there are 3 states in the model, and the <paramref name="observations"/>
        ///   array contains 5 elements, the resulting vector will contain 5 vectors of
        ///   size 3 each. Each vector of size 3 will contain probability values that sum
        ///   up to one. By following those probabilities in order, we may decode those
        ///   probabilities into a sequence of most likely states. However, the sequence
        ///   of obtained states may not be valid in the model.
        /// </remarks>
        /// 
        /// <param name="observations">A sequence of observations.</param>
        /// 
        /// <returns>A vector of the same size as the observation vectors, containing
        ///  the probabilities for each state in the model for the current observation.
        ///  If there are 3 states in the model, and the <paramref name="observations"/>
        ///  array contains 5 elements, the resulting vector will contain 5 vectors of
        ///  size 3 each. Each vector of size 3 will contain probability values that sum
        ///  up to one.</returns>
        /// 
        public double[][] Posterior(TObservation[] observations)
        {
            // Reference: C. S. Foo, CS262 Winter 2007, Lecture 5, Stanford
            // http://ai.stanford.edu/~serafim/CS262_2007/notes/lecture5.pdf

            if (observations == null)
                throw new ArgumentNullException("observations");

            double logLikelihood;

            // Compute forward and backward probabilities
            double[,] lnFwd = ForwardBackwardAlgorithm.LogForward(this, observations, out logLikelihood);
            double[,] lnBwd = ForwardBackwardAlgorithm.LogBackward(this, observations);

            double[][] probabilities = new double[observations.Length][];

            for (int i = 0; i < probabilities.Length; i++)
            {
                double[] states = probabilities[i] = new double[States];

                for (int j = 0; j < states.Length; j++)
                    states[j] = Math.Exp(lnFwd[i, j] + lnBwd[i, j] - logLikelihood);
            }

            return probabilities;
        }

        /// <summary>
        ///   Calculates the probability of each hidden state for each observation 
        ///   in the observation vector, and uses those probabilities to decode the
        ///   most likely sequence of states for each observation in the sequence 
        ///   using the posterior decoding method. See remarks for details.
        /// </summary>
        /// 
        /// <remarks>
        ///   If there are 3 states in the model, and the <paramref name="observations"/>
        ///   array contains 5 elements, the resulting vector will contain 5 vectors of
        ///   size 3 each. Each vector of size 3 will contain probability values that sum
        ///   up to one. By following those probabilities in order, we may decode those
        ///   probabilities into a sequence of most likely states. However, the sequence
        ///   of obtained states may not be valid in the model.
        /// </remarks>
        /// 
        /// <param name="observations">A sequence of observations.</param>
        /// <param name="path">The sequence of states most likely associated with each
        ///   observation, estimated using the posterior decoding method.</param>
        /// 
        /// <returns>A vector of the same size as the observation vectors, containing
        ///  the probabilities for each state in the model for the current observation.
        ///  If there are 3 states in the model, and the <paramref name="observations"/>
        ///  array contains 5 elements, the resulting vector will contain 5 vectors of
        ///  size 3 each. Each vector of size 3 will contain probability values that sum
        ///  up to one.</returns>
        /// 
        [Obsolete("Please use LogLikelihoods(observations) instead.")]
        public double[][] Posterior(TObservation[] observations, out int[] path)
        {
            double[][] probabilities = Posterior(observations);

            path = new int[observations.Length];
            for (int i = 0; i < path.Length; i++)
                Accord.Math.Matrix.Max(probabilities[i], out path[i]);

            return probabilities;
        }

        /// <summary>
        ///   Calculates the likelihood that this model has generated the given sequence.
        /// </summary>
        /// 
        /// <remarks>
        ///   Evaluation problem. Given the HMM  M = (A, B, pi) and  the observation
        ///   sequence O = {o1, o2, ..., oK}, calculate the probability that model
        ///   M has generated sequence O. This can be computed efficiently using the
        ///   either the Viterbi or the Forward algorithms.
        /// </remarks>
        /// 
        /// <param name="observations">
        ///   A sequence of observations.
        /// </param>
        /// <returns>
        ///   The log-likelihood that the given sequence has been generated by this model.
        /// </returns>
        /// 
        [Obsolete("Please use LogLikelihood(observations) instead.")]
        public double Evaluate(TObservation[] observations)
        {
            if (observations == null)
                throw new ArgumentNullException("observations");

            if (observations.Length == 0)
                return Double.NegativeInfinity;

            // Forward algorithm
            double logLikelihood;

            // Compute forward probabilities
            ForwardBackwardAlgorithm.LogForward(this, observations, out logLikelihood);

            // Return the sequence probability
            return logLikelihood;
        }

        /// <summary>
        ///   Calculates the log-likelihood that this model has generated the
        ///   given observation sequence along the given state path.
        /// </summary>
        /// 
        /// <param name="observations">A sequence of observations. </param>
        /// <param name="path">A sequence of states. </param>
        /// 
        /// <returns>
        ///   The log-likelihood that the given sequence of observations has
        ///   been generated by this model along the given sequence of states.
        /// </returns>
        /// 
        [Obsolete("Please use LogLikelihood(observations, path) instead.")]
        public double Evaluate(TObservation[] observations, int[] path)
        {
            if (observations == null)
                throw new ArgumentNullException("observations");

            if (path == null)
                throw new ArgumentNullException("path");

            if (observations.Length == 0)
                return Double.NegativeInfinity;

            try
            {
                double logLikelihood = LogInitial[path[0]]
                    + Emissions[path[0]].LogProbabilityFunction(observations[0]);

                for (int i = 1; i < observations.Length; i++)
                {
                    double a = LogTransitions[path[i - 1]][path[i]];
                    double b = Emissions[path[i]].LogProbabilityFunction(observations[i]);
                    logLikelihood += a + b;
                }

                // Return the sequence probability
                return logLikelihood;
            }
            catch (IndexOutOfRangeException ex)
            {
                checkHiddenStates(ex, path);
                throw;
            }
        }



        /// <summary>
        ///   Predicts the next observation occurring after a given observation sequence.
        /// </summary>
        /// 
        /// <param name="observations">A sequence of observations. Predictions will be made regarding 
        ///   the next observations that should be coming after the last observation in this sequence.</param>
        /// 
        public TObservation Predict(TObservation[] observations)
        {
            if (observations == null)
                throw new ArgumentNullException("observations");

            double logLikelihood;
            return Predict(observations, out logLikelihood);
        }

        /// <summary>
        ///   Predicts the next observation occurring after a given observation sequence.
        /// </summary>
        /// 
        /// <param name="observations">A sequence of observations. Predictions will be made regarding 
        ///   the next observations that should be coming after the last observation in this sequence.</param>
        /// <param name="logLikelihood">The log-likelihood of the given sequence, plus the predicted
        ///   next observation. Exponentiate this value (use the System.Math.Exp function) to obtain
        ///   a <c>likelihood</c> value.</param>
        /// 
        public TObservation Predict(TObservation[] observations, out double logLikelihood)
        {
            if (observations == null)
                throw new ArgumentNullException("observations");

            // Matrix to store the probabilities in assuming the next
            // observations (prediction) will belong to each state.
            double[][] weights;

            // Compute the next observation (currently only one ahead is supported).
            return predict(observations, 1, out logLikelihood, out weights)[0];
        }

        /// <summary>
        ///   Predicts the next observation occurring after a given observation sequence.
        /// </summary>
        /// 
        /// <param name="observations">A sequence of observations. Predictions will be made regarding 
        ///   the next observations that should be coming after the last observation in this sequence.</param>
        /// <param name="logLikelihood">The log-likelihood of the given sequence, plus the predicted
        ///   next observation. Exponentiate this value (use the System.Math.Exp function) to obtain
        ///   a <c>likelihood</c> value.</param>
        /// <param name="probabilities">The continuous probability distribution describing the next observations
        ///   that are likely to be generated. Taking the mode of this distribution might give the most likely
        ///   next value in the observed sequence.</param>
        /// 
        public TObservation Predict<TMultivariate>(TObservation[] observations, out double logLikelihood, out MultivariateMixture<TMultivariate> probabilities)
            where TMultivariate : DistributionBase, TDistribution, IMultivariateDistribution<double[]>
        {
            // Compute the next observation (currently only one ahead is supported)
            return predict(observations, out logLikelihood, out probabilities)[0];
        }

        /// <summary>
        ///   Predicts the next observation occurring after a given observation sequence.
        /// </summary>
        /// 
        /// <param name="observations">A sequence of observations. Predictions will be made regarding 
        ///   the next observations that should be coming after the last observation in this sequence.</param>
        /// <param name="probabilities">The continuous probability distribution describing the next observations
        ///   that are likely to be generated. Taking the mode of this distribution might give the most likely
        ///   next value in the observed sequence.</param>
        /// 
        public TObservation Predict<TMultivariate>(TObservation[] observations, out MultivariateMixture<TMultivariate> probabilities)
            where TMultivariate : DistributionBase, TDistribution, IMultivariateDistribution<double[]>
        {
            double probability;

            // Compute the next observation (currently only one ahead is supported)
            return predict(observations, out probability, out probabilities)[0];
        }


        /// <summary>
        ///   Predicts the next observation occurring after a given observation sequence.
        /// </summary>
        /// 
        /// <param name="observations">A sequence of observations. Predictions will be made regarding 
        ///   the next observations that should be coming after the last observation in this sequence.</param>
        /// <param name="probabilities">The continuous probability distribution describing the next observations
        ///   that are likely to be generated. Taking the mode of this distribution might give the most likely
        ///   next value in the observed sequence.</param>
        /// 
        public TObservation Predict<TUnivariate>(TObservation[] observations, out Mixture<TUnivariate> probabilities)
            where TUnivariate : DistributionBase, TDistribution, IUnivariateDistribution<double>
        {
            double probability;

            // Compute the next observation (as if it were multidimensional)
            return predict(observations, out probability, out probabilities)[0];
        }

        /// <summary>
        ///   Predicts the next observation occurring after a given observation sequence.
        /// </summary>
        /// 
        /// <param name="observations">A sequence of observations. Predictions will be made regarding 
        ///   the next observations that should be coming after the last observation in this sequence.</param>
        /// <param name="logLikelihood">The log-likelihood of the given sequence, plus the predicted
        ///   next observation. Exponentiate this value (use the System.Math.Exp function) to obtain
        ///   a <c>likelihood</c> value.</param>
        /// <param name="probabilities">The continuous probability distribution describing the next observations
        ///   that are likely to be generated. Taking the mode of this distribution might give the most likely
        ///   next value in the observed sequence.</param>
        /// 
        public TObservation Predict<TUnivariate>(TObservation[] observations, out double logLikelihood, out Mixture<TUnivariate> probabilities)
            where TUnivariate : DistributionBase, TDistribution, IUnivariateDistribution<double>
        {
            // Return the first (single) dimension of the next observation.
            return predict(observations, out logLikelihood, out probabilities)[0];
        }

        /// <summary>
        ///   Predicts the next observations occurring after a given observation sequence.
        /// </summary>
        /// 
        /// <param name="observations">A sequence of observations. Predictions will be made regarding 
        ///   the next observations that should be coming after the last observation in this sequence.</param>
        /// <param name="next">The number of observations to be predicted. Default is 1.</param>
        /// <param name="logLikelihood">The log-likelihood of the given sequence, plus the predicted
        ///   next observation. Exponentiate this value (use the System.Math.Exp function) to obtain
        ///   a <c>likelihood</c> value.</param>
        /// 
        public TObservation[] Predict(TObservation[] observations, int next, out double logLikelihood)
        {
            // Matrix to store the probabilities in assuming the next
            // observations (prediction) will belong to each state.
            double[][] weights;

            // Compute the next observations
            return predict(observations, next, out logLikelihood, out weights);
        }

        /// <summary>
        ///   Generates a random vector of observations from the model.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// 
        /// <returns>A random vector of observations drawn from the model.</returns>
        /// 
        public TObservation[] Generate(int samples)
        {
            int[] path; double logLikelihood;
            return Generate(samples, out path, out logLikelihood);
        }

        /// <summary>
        ///   Generates a random vector of observations from the model.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="logLikelihood">The log-likelihood of the generated observation sequence.</param>
        /// <param name="path">The Viterbi path of the generated observation sequence.</param>
        /// 
        /// <returns>A random vector of observations drawn from the model.</returns>
        /// 
        public TObservation[] Generate(int samples, out int[] path, out double logLikelihood)
        {
            double[] transitions = LogInitial;

            logLikelihood = 0; // log(1)
            path = new int[samples];

            TObservation[] observations = new TObservation[samples];


            // For each observation to be generated
            for (int t = 0; t < observations.Length; t++)
            {
                // Navigate randomly on one of the state transitions
                int state = GeneralDiscreteDistribution.Random(Elementwise.Exp(transitions));

                // Generate a sample for the state
                var sampling = (ISampleableDistribution<TObservation>)Emissions[state];
                TObservation symbol = sampling.Generate();

                // Store the sample
                observations[t] = symbol;
                path[t] = state;

                // Compute log-likelihood up to this point
                logLikelihood += transitions[state] +
                    Emissions[state].LogProbabilityFunction(symbol);

                // Continue sampling
                transitions = LogTransitions.GetRow(state);
            }

            return observations;
        }





        #region Private Methods

        /// <summary>
        ///   Predicts the next observation occurring after a given observation sequence.
        /// </summary>
        /// 
        private TObservation[] predict<TMultivariate>(TObservation[] observations, out double logLikelihood, out MultivariateMixture<TMultivariate> probabilities)
            where TMultivariate : DistributionBase, TDistribution, IMultivariateDistribution<double[]>
        {
            // Matrix to store the probabilities in assuming the next
            // observations (prediction) will belong to each state.
            double[][] weights;

            // Compute the next observation (currently only one ahead is supported).
            TObservation[] prediction = predict(observations, 1, out logLikelihood, out weights);

            // Create the mixture distribution defining the model likelihood in
            // assuming the next observation belongs will belong to each state.
            TMultivariate[] b = Array.ConvertAll(B, x => (TMultivariate)x);
            probabilities = new MultivariateMixture<TMultivariate>(weights[1].Exp(), b);

            return prediction;
        }

        /// <summary>
        ///   Predicts the next observation occurring after a given observation sequence.
        /// </summary>
        /// 
        private TObservation[] predict<TUnivariate>(TObservation[] observations, out double logLikelihood, out Mixture<TUnivariate> probabilities)
            where TUnivariate : DistributionBase, TDistribution, IUnivariateDistribution<double>
        {
            // Matrix to store the probabilities in assuming the next
            // observations (prediction) will belong to each state.
            double[][] weights;

            // Compute the next observation (currently only one ahead is supported).
            TObservation[] prediction = predict(observations, 1, out logLikelihood, out weights);

            // Create the mixture distribution defining the model likelihood in
            // assuming the next observation belongs will belong to each state.
            TUnivariate[] b = Array.ConvertAll(B, x => (TUnivariate)x);
            probabilities = new Mixture<TUnivariate>(weights[1].Exp(), b);

            return prediction;
        }

        /// <summary>
        ///   Predicts the next observation occurring after a given observation sequence.
        /// </summary>
        /// 
        private TObservation[] predict(TObservation[] observations, int next, out double logLikelihood, out double[][] lnFuture)
        {
            int states = States;
            int T = next;

            double[][] lnA = LogTransitions;

            TObservation[] prediction = new TObservation[next];
            TObservation[] expectation = new TObservation[states];

            // Compute expectations for each state
            for (int i = 0; i < states; i++)
                expectation[i] = getMode(B[i]);


            // Compute forward probabilities for the given observation sequence.
            double[,] lnFw0 = ForwardBackwardAlgorithm.LogForward(this, observations, out logLikelihood);

            // Create a matrix to store the future probabilities for the prediction
            // sequence and copy the latest forward probabilities on its first row.
            double[][] lnFwd = new double[T + 1][];
            for (int i = 0; i < lnFwd.Length; i++)
                lnFwd[i] = new double[States];


            // 1. Initialization
            for (int i = 0; i < States; i++)
                lnFwd[0][i] = lnFw0[observations.Length - 1, i];


            // 2. Induction
            for (int t = 0; t < T; t++)
            {
                double[] weights = lnFwd[t + 1];

                for (int i = 0; i < weights.Length; i++)
                {
                    double sum = Double.NegativeInfinity;
                    for (int j = 0; j < states; j++)
                        sum = Special.LogSum(sum, lnFwd[t][j] + lnA[j][i]);

                    weights[i] = sum + B[i].LogProbabilityFunction(expectation[i]);
                }

                double sumWeight = Double.NegativeInfinity;
                for (int i = 0; i < weights.Length; i++)
                    sumWeight = Special.LogSum(sumWeight, weights[i]);
                for (int i = 0; i < weights.Length; i++)
                    weights[i] -= sumWeight;

                // Select most probable value
                double maxWeight = weights[0];
                prediction[t] = expectation[0];
                for (int i = 1; i < states; i++)
                {
                    if (weights[i] > maxWeight)
                    {
                        maxWeight = weights[i];
                        prediction[t] = expectation[i];
                    }
                }

                // Recompute log-likelihood
                logLikelihood = maxWeight;
            }

            // Returns the future-forward probabilities
            lnFuture = lnFwd;

            return prediction;
        }

        private static TObservation getMode(TDistribution dist)
        {
            var uni = dist as IUnivariateDistribution<TObservation>;
            if (uni != null)
                return uni.InverseDistributionFunction(1.0);

            throw new NotSupportedException();
        }

        private void checkHiddenStates(IndexOutOfRangeException ex, int[] path)
        {
            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] < 0 || path[i] >= States)
                {
                    throw new ArgumentException("path", "The hidden states vector must "
                    + "only contain values higher than or equal to 0 and less than " + States
                    + ". The value at the position " + i + " is " + path[i] + ".", ex);
                }
            }
        }

        #endregion


        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public virtual object Clone()
        {
            double[][] A = LogTransitions.Copy();
            double[] pi = LogInitial.Copy();
            TDistribution[] B = Emissions.MemberwiseClone();

            return new HiddenMarkovModel<TDistribution, TObservation>(A, B, pi, logarithm: true);
        }



#pragma warning disable 612, 618

        /// <summary>
        /// Predicts a the probability that the sequence vector
        /// has been generated by this log-likelihood tagger along
        /// the given path of hidden states.
        /// </summary>
        /// 
        public double LogLikelihood(TObservation[] sequences, int[] path)
        {
            return Evaluate(sequences, path);
        }

        /// <summary>
        /// Predicts a the probability that the sequence vector
        /// has been generated by this log-likelihood tagger.
        /// </summary>
        /// 
        public override double[] LogLikelihood(TObservation[][] sequences, double[] result)
        {
            // TODO: Implement efficiently
            for (int i = 0; i < sequences.Length; i++)
                result[i] = Evaluate(sequences[i]);
            return result;
        }

        /// <summary>
        /// Predicts a the probability that the sequence vector
        /// has been generated by this log-likelihood tagger.
        /// </summary>
        /// 
        public override double[] LogLikelihood(TObservation[][] sequences, ref int[][] decision, double[] result)
        {
            // TODO: Implement efficiently
            for (int i = 0; i < sequences.Length; i++)
            {
                decision[i] = Decode(sequences[i]);
                result[i] = Evaluate(sequences[i]);
            }

            return result;
        }

        /// <summary>
        /// Predicts a the log-likelihood for each of the observations in
        /// the sequence vector assuming each of the possible states in the
        /// tagger model.
        /// </summary>
        public override double[][] LogLikelihoods(TObservation[] sequence, double[][] result)
        {
            return Posterior(sequence);
        }

        /// <summary>
        /// Predicts a the log-likelihood for each of the observations in
        /// the sequence vector assuming each of the possible states in the
        /// tagger model.
        /// </summary>
        public override double[][] LogLikelihoods(TObservation[] sequence, ref int[] decision, double[][] result)
        {
            // TODO: Implement efficiently
            decision = Decide(sequence);
            return Posterior(sequence);
        }


        /// <summary>
        /// Computes class-label decisions for the given <paramref name="input" />.
        /// </summary>
        /// <param name="input">The input vectors that should be classified as
        /// any of the <see cref="ITransform.NumberOfOutputs" /> possible classes.</param>
        /// <param name="result">The location where to store the class-labels.</param>
        /// <returns>
        /// A set of class-labels that best describe the <paramref name="input" />
        /// vectors according to this classifier.
        /// </returns>
        public override int[][] Decide(TObservation[][] input, int[][] result)
        {
            // TODO: Implement efficiently
            for (int i = 0; i < input.Length; i++)
                result[i] = Decode(input[i]);
            return result;
        }
#pragma warning restore 612, 618


    }
}
