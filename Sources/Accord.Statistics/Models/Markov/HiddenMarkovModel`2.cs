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
    using Accord.Compat;
    using Accord.Math.Optimization;

    /// <summary>
    ///   Algorithms for solving <see cref="HiddenMarkovModel"/>-related
    ///   problems, such as sequence decoding and likelihood evaluation.
    /// </summary>
    /// 
    public enum HiddenMarkovModelAlgorithm
    {
        /// <summary>
        ///   Uses the Viterbi algorithm (max-sum) to find the hidden states of a
        ///   sequence of observations and to evaluate its likelihood. The likelihood
        ///   will be computed along the Viterbi path.
        /// </summary>
        /// 
        Viterbi,

        /// <summary>
        ///   Uses the forward algorithm (sum-prod) to compute the likelihood of a sequence.
        ///   The likelihood will be computed considering every possible path in the model (default).
        ///   When set, calling LogLikelihoods will give the model's posterior distribution.
        /// </summary>
        /// 
        Forward
    }

    /// <summary>
    ///   Hidden Markov Model for any kind of observations (not only discrete).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Hidden Markov Models (HMM) are stochastic methods to model temporal and sequence
    ///   data. They are especially known for their application in temporal pattern recognition
    ///   such as speech, handwriting, gesture recognition, part-of-speech tagging, musical
    ///   score following, partial discharges and bioinformatics.</para>
    ///   
    /// <para>
    ///   This page refers to the arbitrary-density (continuous emission distributions) version
    ///   of the model. For discrete distributions, please see <see cref="HiddenMarkovModel"/>.
    /// </para>
    /// 
    /// <para>
    ///   Dynamical systems of discrete nature assumed to be governed by a Markov chain emits
    ///   a sequence of observable outputs. Under the Markov assumption, it is also assumed that
    ///   the latest output depends only on the current state of the system. Such states are often
    ///   not known from the observer when only the output values are observable.</para>
    ///   
    /// <para>
    ///   Hidden Markov Models attempt to model such systems and allow, among other things,
    ///   <list type="number">
    ///     <item><description>
    ///       To infer the most likely sequence of states that produced a given output sequence,</description></item>
    ///     <item><description>
    ///       Infer which will be the most likely next state (and thus predicting the next output),</description></item>
    ///     <item><description>
    ///       Calculate the probability that a given sequence of outputs originated from the system
    ///       (allowing the use of hidden Markov models for sequence classification).</description></item>
    ///     </list></para>
    ///     
    ///  <para>     
    ///   The “hidden” in Hidden Markov Models comes from the fact that the observer does not
    ///   know in which state the system may be in, but has only a probabilistic insight on where
    ///   it should be.</para>
    ///   
    ///  <para>
    ///   The arbitrary-density Hidden Markov Model uses any probability density function (such
    ///   as <see cref="Accord.Statistics.Distributions.Univariate.NormalDistribution">Gaussian</see>
    ///   <see cref="Accord.Statistics.Distributions.Univariate.Mixture{T}">Mixture Model</see>) for
    ///   computing the state probability. In other words, in a continuous HMM the matrix of emission
    ///   probabilities B is replaced by an array of either discrete or continuous probability density
    ///   functions.</para>
    ///  
    ///  <para>
    ///   If a <see cref="Accord.Statistics.Distributions.Univariate.GeneralDiscreteDistribution">general
    ///   discrete distribution</see> is used as the underlying probability density function, the
    ///   model becomes equivalent to the <see cref="HiddenMarkovModel">discrete Hidden Markov Model</see>.
    ///  </para>
    ///  
    /// <para>
    ///   For a more thorough explanation on some fundamentals on how Hidden Markov Models work,
    ///   please see the <see cref="HiddenMarkovModel"/> documentation page. To learn a Markov
    ///   model, you can find a list of both <see cref="ISupervisedLearning">supervised</see> and
    ///   <see cref="IUnsupervisedLearning">unsupervised</see> learning algorithms in the
    ///   <see cref="Accord.Statistics.Models.Markov.Learning"/> namespace.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia contributors. "Linear regression." Wikipedia, the Free Encyclopedia.
    ///       Available at: http://en.wikipedia.org/wiki/Hidden_Markov_model </description></item>
    ///     <item><description>
    ///       Bishop, Christopher M.; Pattern Recognition and Machine Learning. 
    ///       Springer; 1st ed. 2006.</description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    ///   <para>The example below reproduces the same example given in the Wikipedia
    ///   entry for the Viterbi algorithm (http://en.wikipedia.org/wiki/Viterbi_algorithm).
    ///   As an arbitrary density model, one can use it with any available <see cref="IDistribution">
    ///   probability distributions</see>, including with a discrete probability. In the 
    ///   following example, the generic model is used with a <see cref="GeneralDiscreteDistribution"/>
    ///   to reproduce the same example given in <see cref="HiddenMarkovModel{TDistribution, TObservation}"/>.
    ///   Below, the model's parameters are initialized manually. However, it is possible to learn
    ///   those automatically using <see cref="BaumWelchLearning{TDistribution, TObservation}"/>.</para>
    /// 
    /// <code source="Unit Tests\Accord.Tests.Statistics\Models\Markov\HiddenMarkovModel`2Test.cs" region="doc_decode" />
    /// 
    /// <para>
    ///   Examples on how to learn hidden Markov models can be found on the documentation
    ///   pages of the respective learning algorithms: <see cref="BaumWelchLearning{TDistribution, TObservation}"/>,
    ///   <see cref="ViterbiLearning{TDistribution, TObservation}"/>, <see cref="MaximumLikelihoodLearning{TDistribution, TObservation}"/>.
    ///   The simplest of such examples can be seen below:</para>
    /// 
    /// <code source="Unit Tests\Accord.Tests.Statistics\Models\Markov\HiddenMarkovModel`2Test.cs" region="doc_learn" />
    /// 
    /// <para>
    ///   Markov models can also be trained without having, in fact, "hidden" parts. The following example shows 
    ///   how hidden Markov models trained using Maximum Likelihood Learning can be used in the context of fraud 
    ///   analysis, in which we actually know in advance the class labels for each state in the sequences we are
    ///   trying to learn:</para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.Statistics\Models\Markov\MaximumLikelihoodLearning`1Test.cs" region="doc_learn_fraud_analysis"/>
    /// 
    /// <para>
    ///   Where the transform function is defined as:</para>
    ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Markov\MaximumLikelihoodLearning`1Test.cs" region="doc_learn_fraud_transform"/>
    ///   
    /// <para>Hidden Markov Models can also be used to predict the next observation in a sequence. This can be done by
    ///   inspecting the forward matrix of probabilities for the sequence and checking which would be the most likely 
    ///   state after the current one. Then, it returns the most likely value (the mode) for the distribution associated 
    ///   with that state. This limits the applicability of this model to only very short-term predictions (i.e. most likely,
    ///   only the most immediate next observation).</para>
    ///   
    /// <code source="Unit Tests\Accord.Tests.Statistics\Models\Markov\HiddenMarkovModel`2Test.cs" region="doc_predict"/>
    /// </example>
    /// 
    /// <seealso cref="BaumWelchLearning{T}">Baum-Welch, one of the most famous learning algorithms for Hidden Markov Models.</seealso>
    /// <seealso cref="HiddenMarkovModel">Discrete-density Hidden Markov Model</seealso>
    /// <seealso cref="Accord.Statistics.Models.Markov.Learning"/>
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
            : this()
        {
            double[,] a;
            this.states = topology.Create(true, out a, out logPi);
            this.logA = a.ToJagged();
            this.NumberOfOutputs = states;
        }

        /// <summary>
        ///   Constructs a new Hidden Markov Model.
        /// </summary>
        /// 
        protected HiddenMarkovModel()
        {
            this.NumberOfInputs = 1;
            this.Algorithm = HiddenMarkovModelAlgorithm.Forward;
        }

        /// <summary>
        ///   Gets or sets the <see cref="HiddenMarkovModelAlgorithm">algorithm</see>
        ///   that should be used to compute solutions to this model's LogLikelihood(T[] input)
        ///   evaluation, Decide(T[] input) decoding and LogLikelihoods(T[] input)
        ///   posterior problems.
        /// </summary>
        /// 
        public HiddenMarkovModelAlgorithm Algorithm { get; set; }

        /// <summary>
        ///   Gets the number of states of this model.
        /// </summary>
        /// 
        [Obsolete("Please use 'NumberOfStates instead.")]
        public int States
        {
            get { return this.states; }
        }

        /// <summary>
        ///   Gets the number of states of this model.
        /// </summary>
        /// 
        public int NumberOfStates
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
        public HiddenMarkovModel(ITopology topology, Func<int, TDistribution> emissions)
            : this(topology)
        {
            if (emissions == null)
                throw new ArgumentNullException("emissions");

            // Initialize B using the initial distribution
            B = new TDistribution[this.states];
            for (int i = 0; i < B.Length; i++)
                B[i] = (TDistribution)emissions(i);


            if (B[0] is IMultivariateDistribution)
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
        ///   The initial emission probability distribution to be used by each of the states. This
        ///   initial probability distribution will be cloned across all states.
        /// </param>
        /// 
        public HiddenMarkovModel(ITopology topology, TDistribution emissions)
            : this(topology, (i) => (TDistribution)emissions.Clone())
        {
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
            : this(topology, (i) => emissions[i])
        {
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
            : this(new Topology.Ergodic(states), emissions)
        {
        }

        /// <summary>
        ///   Constructs a new Hidden Markov Model with arbitrary-density state probabilities.
        /// </summary>
        /// 
        /// <param name="states">The number of states for the model.</param>
        /// <param name="emissions">A initial distribution to be copied to all states in the model.</param>
        /// 
        public HiddenMarkovModel(int states, Func<int, TDistribution> emissions)
            : this(new Topology.Ergodic(states), (i) => (TDistribution)emissions(i).Clone())
        {
        }



        /// <summary>
        ///   Gets the Emission matrix (B) for this model.
        /// </summary>
        /// 
        public TDistribution[] Emissions
        {
            get { return this.B; }
            protected set { this.B = value; }
        }




        // Main HMM algorithms
        private void emissions(TObservation[] x, double[,] output)
        {
            for (int t = 0; t < x.Length; t++)
                for (int j = 0; j < states; j++)
                    output[j, t] = B[j].LogProbabilityFunction(x[t]);
        }

        private void viterbiDecode(TObservation[] x, out double logLikelihood, double[,] lnFwd, int[,] s, ref int[] path)
        {
            int T = x.Length;
            int maxState;
            logLikelihood = viterbiLikelihood(x, lnFwd, s, out maxState);

            if (path == null)
                path = new int[T];

            // Trackback
            path[T - 1] = maxState;
            for (int t = T - 2; t >= 0; t--)
                path[t] = s[t + 1, path[t + 1]];
        }

        private double viterbiLikelihood(TObservation[] x, double[,] lnFwd, int[,] s, out int maxState)
        {
            viterbiPosterior(x, lnFwd, s, null);

            // Find maximum value for time T-1
            int T = x.Length;
            maxState = 0;
            double maxWeight = lnFwd[T - 1, 0];

            for (int i = 1; i < states; i++)
            {
                if (lnFwd[T - 1, i] > maxWeight)
                {
                    maxState = i;
                    maxWeight = lnFwd[T - 1, i];
                }
            }

            return maxWeight;
        }

        private double viterbiLikelihood(TObservation[] observations, int[] path)
        {
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


        private void viterbiPosterior(TObservation[] x, double[,] lnFwd, int[,] s, double[][] result)
        {
            // Viterbi-forward algorithm.
            int T = x.Length;
            double[] logPi = LogInitial;
            double[][] logA = LogTransitions;

            // Base
            for (int i = 0; i < states; i++)
                lnFwd[0, i] = logPi[i] + B[i].LogProbabilityFunction(x[0]);

            // Induction
            for (int t = 1; t < x.Length; t++)
            {
                TObservation observation = x[t];

                for (int j = 0; j < states; j++)
                {
                    int maxState = 0;
                    double maxWeight = lnFwd[t - 1, 0] + logA[0][j];

                    for (int i = 1; i < states; i++)
                    {
                        double weight = lnFwd[t - 1, i] + logA[i][j];

                        if (weight > maxWeight)
                        {
                            maxState = i;
                            maxWeight = weight;
                        }
                    }

                    lnFwd[t, j] = maxWeight + B[j].LogProbabilityFunction(observation);
                    s[t, j] = maxState;
                }
            }

            if (result != null)
                lnFwd.CopyTo(result, transpose: true);
        }

        private void forwardPosterior(TObservation[] observations, double[,] lnFwd, double[,] lnBwd, double[][] result)
        {
            // Reference: C. S. Foo, CS262 Winter 2007, Lecture 5, Stanford
            // http://ai.stanford.edu/~serafim/CS262_2007/notes/lecture5.pdf

            if (observations == null)
                throw new ArgumentNullException("observations");

            double logLikelihood;

            // Compute forward and backward probabilities
            ForwardBackwardAlgorithm.LogForward(this, observations, lnFwd);
            ForwardBackwardAlgorithm.LogBackward(this, observations, lnBwd);

            logLikelihood = Double.NegativeInfinity;
            for (int i = 0; i < states; i++)
                logLikelihood = Special.LogSum(logLikelihood, lnFwd[observations.Length - 1, i]);

            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < states; j++)
                    result[i][j] = Math.Exp(lnFwd[i, j] + lnBwd[i, j] - logLikelihood);
        }

        //private double[][] forwardDecode(TObservation[] observations, int[] path, double[,] lnFwd, double[,] lnBwd)
        //{
        //    double[][] probabilities = forwardPosterior(observations, lnFwd, lnBwd);

        //    path = new int[observations.Length];
        //    for (int i = 0; i < path.Length; i++)
        //        Accord.Math.Matrix.Max(probabilities[i], out path[i]);

        //    return probabilities;
        //}

        private double forwardLikelihood(TObservation[] observations, double[,] lnFwd)
        {
            // Forward algorithm
            double logLikelihood = Double.NegativeInfinity;

            // Compute forward probabilities
            ForwardBackwardAlgorithm.LogForward(this, observations, lnFwd);

            logLikelihood = Double.NegativeInfinity;
            for (int i = 0; i < states; i++)
                logLikelihood = Special.LogSum(logLikelihood, lnFwd[observations.Length - 1, i]);

            // Return the sequence probability
            return logLikelihood;
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
        [Obsolete("Please set Algorithm to Viterbi and call Decide() instead.")]
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
        [Obsolete("Please set Algorithm to Viterbi and call Decide() instead.")]
        public int[] Decode(TObservation[] observations, out double logLikelihood)
        {
            var prev = Algorithm;
            Algorithm = HiddenMarkovModelAlgorithm.Viterbi;
            int[] path = null;
            logLikelihood = LogLikelihood(observations, ref path);
            Algorithm = prev;
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
        [Obsolete("Please set Algorithm to Forward and call LogLikelihoods() instead.")]
        public double[][] Posterior(TObservation[] observations)
        {
            var prev = Algorithm;
            Algorithm = HiddenMarkovModelAlgorithm.Forward;
            double[][] r = LogLikelihoods(observations);
            Algorithm = prev;
            return r;
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
        [Obsolete("Please set Algorithm to Forward and call LogLikelihoods() instead.")]
        public double[][] Posterior(TObservation[] observations, out int[] path)
        {
            var prev = Algorithm;
            Algorithm = HiddenMarkovModelAlgorithm.Forward;
            //DecodingAlgorithm = HiddenMarkovModelDecoding.Posterior;
            path = null;
            double[][] r = LogLikelihoods(observations, ref path);
            Algorithm = prev;
            return r;
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
        [Obsolete("Please set Algorithm to Forward and call LogLikelihood() instead.")]
        public double Evaluate(TObservation[] observations)
        {
            var prev = Algorithm;
            Algorithm = HiddenMarkovModelAlgorithm.Forward;
            double r = LogLikelihood(observations);
            Algorithm = prev;
            return r;
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
            return viterbiLikelihood(observations, path);
        }




        /// <summary>
        ///   Predicts the next observation occurring after a given observation sequence.
        /// </summary>
        /// 
        /// <param name="observations">A sequence of observations. Predictions will be made regarding 
        ///   the next observations that should be coming after the last observation in this sequence.</param>
        /// 
        /// <remarks>
        ///   This method works by inspecting the forward matrix of probabilities for the sequence and 
        ///   checking which would be the most likely state after the current one. Then, it returns the 
        ///   most likely value (the mode) for the distribution associated with that state. This limits 
        ///   the applicability of this model to only very short-term predictions (i.e. most likely, only
        ///   the most immediate next observation).
        /// </remarks>
        /// 
        /// <example>
        ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Markov\HiddenMarkovModel`2Test.cs" region="doc_predict"/>
        /// </example>
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
        /// <remarks>
        ///   This method works by inspecting the forward matrix of probabilities for the sequence and 
        ///   checking which would be the most likely state after the current one. Then, it returns the 
        ///   most likely value (the mode) for the distribution associated with that state. This limits 
        ///   the applicability of this model to only very short-term predictions (i.e. most likely, only
        ///   the most immediate next observation).
        /// </remarks>
        /// 
        /// <example>
        ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Markov\HiddenMarkovModel`2Test.cs" region="doc_predict"/>
        /// </example>
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
        /// <remarks>
        ///   This method works by inspecting the forward matrix of probabilities for the sequence and 
        ///   checking which would be the most likely state after the current one. Then, it returns the 
        ///   most likely value (the mode) for the distribution associated with that state. This limits 
        ///   the applicability of this model to only very short-term predictions (i.e. most likely, only
        ///   the most immediate next observation).
        /// </remarks>
        /// 
        /// <example>
        ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Markov\HiddenMarkovModel`2Test.cs" region="doc_predict"/>
        /// </example>
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
        /// <remarks>
        ///   This method works by inspecting the forward matrix of probabilities for the sequence and 
        ///   checking which would be the most likely state after the current one. Then, it returns the 
        ///   most likely value (the mode) for the distribution associated with that state. This limits 
        ///   the applicability of this model to only very short-term predictions (i.e. most likely, only
        ///   the most immediate next observation).
        /// </remarks>
        /// 
        /// <example>
        ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Markov\HiddenMarkovModel`2Test.cs" region="doc_predict"/>
        /// </example>
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
        /// <remarks>
        ///   This method works by inspecting the forward matrix of probabilities for the sequence and 
        ///   checking which would be the most likely state after the current one. Then, it returns the 
        ///   most likely value (the mode) for the distribution associated with that state. This limits 
        ///   the applicability of this model to only very short-term predictions (i.e. most likely, only
        ///   the most immediate next observation).
        /// </remarks>
        /// 
        /// <example>
        ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Markov\HiddenMarkovModel`2Test.cs" region="doc_predict"/>
        /// </example>
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
        /// <remarks>
        ///   This method works by inspecting the forward matrix of probabilities for the sequence and 
        ///   checking which would be the most likely state after the current one. Then, it returns the 
        ///   most likely value (the mode) for the distribution associated with that state. This limits 
        ///   the applicability of this model to only very short-term predictions (i.e. most likely, only
        ///   the most immediate next observation).
        /// </remarks>
        /// 
        /// <example>
        ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Markov\HiddenMarkovModel`2Test.cs" region="doc_predict"/>
        /// </example>
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
        /// 
        /// <remarks>
        ///   This method works by inspecting the forward matrix of probabilities for the sequence and 
        ///   checking which would be the most likely state after the current one. Then, it returns the 
        ///   most likely value (the mode) for the distribution associated with that state. This limits 
        ///   the applicability of this model to only very short-term predictions (i.e. most likely, only
        ///   the most immediate next observation).
        /// </remarks>
        /// 
        /// <example>
        ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Markov\HiddenMarkovModel`2Test.cs" region="doc_predict"/>
        /// </example>
        /// 
        public virtual TObservation[] Predict(TObservation[] observations, int next)
        {
            // Matrix to store the probabilities in assuming the next
            // observations (prediction) will belong to each state.
            double[][] weights;
            double logLikelihood;

            // Compute the next observations
            return predict(observations, next, out logLikelihood, out weights);
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
        /// <remarks>
        ///   This method works by inspecting the forward matrix of probabilities for the sequence and 
        ///   checking which would be the most likely state after the current one. Then, it returns the 
        ///   most likely value (the mode) for the distribution associated with that state. This limits 
        ///   the applicability of this model to only very short-term predictions (i.e. most likely, only
        ///   the most immediate next observation).
        /// </remarks>
        /// 
        /// <example>
        ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Markov\HiddenMarkovModel`2Test.cs" region="doc_predict"/>
        /// </example>
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
            TMultivariate[] b = B.Apply(x => (TMultivariate)x);
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
            TUnivariate[] b = B.Apply(x => (TUnivariate)x);
            probabilities = new Mixture<TUnivariate>(weights[1].Exp(), b);

            return prediction;
        }

        /// <summary>
        ///   Predicts the next observation occurring after a given observation sequence.
        /// </summary>
        /// 
        private TObservation[] predict(TObservation[] observations, int next, out double logLikelihood, out double[][] lnFuture)
        {
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
                lnFwd[i] = new double[states];


            // 1. Initialization
            for (int i = 0; i < states; i++)
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
            {
                // TODO: Add support for proper mode calculation 
                // (with correct type) in IUnivariateDistribution
                return uni.Mode.To<TObservation>();
            }

            throw new NotSupportedException();
        }

        private void checkHiddenStates(IndexOutOfRangeException ex, int[] path)
        {
            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] < 0 || path[i] >= states)
                {
                    throw new ArgumentException("path", "The hidden states vector must "
                    + "only contain values higher than or equal to 0 and less than " + states
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




        /// <summary>
        /// Predicts a the probability that the sequence vector
        /// has been generated by this log-likelihood tagger along
        /// the given path of hidden states.
        /// </summary>
        /// 
        public double LogLikelihood(TObservation[] sequences, int[] path)
        {
            return viterbiLikelihood(sequences, path);
        }

        /// <summary>
        /// Predicts a the probability that the sequence vector
        /// has been generated by this log-likelihood tagger along
        /// the given path of hidden states.
        /// </summary>
        /// 
        public double[] LogLikelihood(TObservation[][] sequences, int[][] path)
        {
            return LogLikelihood(sequences, path, new double[sequences.Length]);
        }

        /// <summary>
        /// Predicts a the probability that the sequence vector
        /// has been generated by this log-likelihood tagger along
        /// the given path of hidden states.
        /// </summary>
        /// 
        public double[] LogLikelihood(TObservation[][] sequences, int[][] path, double[] result)
        {
            for (int i = 0; i < sequences.Length; i++)
                result[i] = viterbiLikelihood(sequences[i], path[i]);
            return result;
        }

        /// <summary>
        /// Predicts a the probability that the sequence vector
        /// has been generated by this log-likelihood tagger.
        /// </summary>
        /// 
        public override double[] LogLikelihood(TObservation[][] sequences, double[] result)
        {
            int T = sequences.Columns(max: true);
            var lnFwd = new double[T, states];

            if (Algorithm == HiddenMarkovModelAlgorithm.Forward)
            {
                for (int i = 0; i < sequences.Length; i++)
                    result[i] = forwardLikelihood(sequences[i], lnFwd);
            }
            else
            {
                var s = new int[T, states];
                int maxState;
                for (int i = 0; i < sequences.Length; i++)
                    result[i] = viterbiLikelihood(sequences[i], lnFwd, s, out maxState);
            }

            return result;
        }

        /// <summary>
        /// Predicts a the probability that the sequence vector
        /// has been generated by this log-likelihood tagger.
        /// </summary>
        /// 
        public override double[] LogLikelihood(TObservation[][] sequences, ref int[][] decision, double[] result)
        {
            int T = sequences.Columns(max: true);
            var lnFwd = new double[T, states];
            var s = new int[T, states];

            for (int i = 0; i < sequences.Length; i++)
                viterbiDecode(sequences[i], out result[i], lnFwd, s, ref decision[i]);

            if (Algorithm == HiddenMarkovModelAlgorithm.Forward)
            {
                for (int i = 0; i < sequences.Length; i++)
                    result[i] = forwardLikelihood(sequences[i], lnFwd); // overwrite
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
            int T = sequence.Length;
            var lnFwd = new double[T, states];

            if (Algorithm == HiddenMarkovModelAlgorithm.Forward)
            {
                var lnBwd = new double[T, states];
                forwardPosterior(sequence, lnFwd, lnBwd, result);
            }
            else
            {
                var s = new int[T, states];
                viterbiPosterior(sequence, lnFwd, s, result);
            }

            return result;
        }

        /// <summary>
        /// Predicts a the log-likelihood for each of the observations in
        /// the sequence vector assuming each of the possible states in the
        /// tagger model.
        /// </summary>
        public override double[][] LogLikelihoods(TObservation[] sequence, ref int[] decision, double[][] result)
        {
            int T = sequence.Length;
            var lnFwd = new double[T, states];
            double logLikelihood;
            var s = new int[T, states];
            viterbiDecode(sequence, out logLikelihood, lnFwd, s, ref decision);

            if (Algorithm == HiddenMarkovModelAlgorithm.Forward)
            {
                var lnBwd = new double[T, states];
                forwardPosterior(sequence, lnFwd, lnBwd, result);
            }
            else
            {
                lnFwd.CopyTo(result, transpose: true);
            }

            return result;
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
            // TODO: Use parallel for with thread-local storage
            int T = input.Columns(max: true);
            var lnFwd = new double[T, states];
            var s = new int[T, states];
            double logLikelihood;

            for (int i = 0; i < input.Length; i++)
                viterbiDecode(input[i], out logLikelihood, lnFwd, s, ref result[i]);

            return result;
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
        public override int[] Decide(TObservation[] input, int[] result)
        {
            int T = input.Length;
            var lnFwd = new double[T, states];
            var s = new int[T, states];
            double logLikelihood;
            viterbiDecode(input, out logLikelihood, lnFwd, s, ref result);
            return result;
        }

    }
}
