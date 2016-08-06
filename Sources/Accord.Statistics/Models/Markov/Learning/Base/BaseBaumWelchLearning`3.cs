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

namespace Accord.Statistics.Models.Markov.Learning
{
    using System;
    using Accord.Math;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.MachineLearning;
    using Accord.Statistics.Models.Markov.Topology;

    /// <summary>
    ///   Base class for implementations of the Baum-Welch learning algorithm.
    ///   This class cannot be instantiated.
    /// </summary>
    /// 
    public abstract class BaseBaumWelchLearningOptions<TModel, TDistribution, TObservation, TOptions> :
        BaseBaumWelchLearning<TModel, TDistribution, TObservation, TOptions>,
        IUnsupervisedLearning<TModel, TObservation[], int[]>,
        IConvergenceLearning
        where TModel : HiddenMarkovModel<TDistribution, TObservation>
        where TDistribution : IFittableDistribution<TObservation, TOptions>
        where TOptions : class, IFittingOptions
    {


        /// <summary>
        ///   Creates a new instance of the Baum-Welch learning algorithm.
        /// </summary>
        /// 
        public BaseBaumWelchLearningOptions(TModel model)
            : base(model)
        {
        }

        /// <summary>
        /// Fits one emission distribution. This method can be override in a
        /// base class in order to implement special fitting options.
        /// </summary>
        protected override void Fit(int index, TObservation[] values, double[] weights)
        {
            Model.Emissions[index].Fit(values, weights, FittingOptions);
        }
    }

    /// <summary>
    ///   Base class for implementations of the Baum-Welch learning algorithm.
    ///   This class cannot be instantiated.
    /// </summary>
    /// 
    public abstract class BaseBaumWelchLearning<TModel, TDistribution, TObservation, TOptions> :
        BaseHiddenMarkovModelLearning<TModel, TObservation>,
        IUnsupervisedLearning<TModel, TObservation[], int[]>,
        IConvergenceLearning
        where TOptions : class, IFittingOptions
        where TModel : HiddenMarkovModel<TDistribution, TObservation>
        where TDistribution : IFittableDistribution<TObservation>
    {

        private AbsoluteConvergence convergence = new AbsoluteConvergence(); // TODO: change to relative

        private TObservation[][] vectorObservations;
        private TObservation[] samples;
        private double[] sampleWeights;

        private ITopology topology;

        /// <summary>
        ///   Gets or sets the distribution fitting options
        ///   to use when estimating distribution densities
        ///   during learning.
        /// </summary>
        /// <value>The distribution fitting options.</value>
        /// 
        public TOptions FittingOptions { get; set; }

        ///// <summary>
        /////   Gets or sets the function that initializes the emission
        /////   distributions in the hidden Markov Models.
        ///// </summary>
        ///// 
        //public Func<int, TDistribution> Emissions { get; set; }

        /// <summary>
        ///   Gets the log-likelihood of the model at the last iteration.
        /// </summary>
        /// 
        public double LogLikelihood { get; set; }

        /// <summary>
        ///   Gets or sets the state-transition topology to be used.
        ///   Default is <see cref="Ergodic"/>.
        /// </summary>
        /// 
        public ITopology Topology
        {
            get { return topology; }
            set { topology = value; }
        }

        /// <summary>
        ///   Gets or sets the number of states in the models to be learned.
        /// </summary>
        /// 
        public int NumberOfStates
        {
            get { return topology.States; }
            set { topology.States = value; }
        }

        //protected TDistribution[] CreateEmissions()
        //{
        //    var emissions = new TDistribution[NumberOfStates];
        //    for (int i = 0; i < emissions.Length; i++)
        //    {
        //        emissions[i] = Emissions(i);
        //        emissions[i].Fit(samples);
        //    }
        //    return emissions;
        //}

        /// <summary>
        ///   Creates a new instance of the Baum-Welch learning algorithm.
        /// </summary>
        /// 
        public BaseBaumWelchLearning(TModel model)
            : base(model)
        {
            init();
        }

        //public BaseBaumWelchLearning()
        //{
        //    init();
        //}

        private void init()
        {
            Topology = new Ergodic(5);
        }

        /// <summary>
        ///   Gets or sets the maximum change in the average log-likelihood
        ///   after an iteration of the algorithm used to detect convergence.
        /// </summary>
        /// 
        /// <remarks>
        ///   This is the likelihood convergence limit L between two iterations of the algorithm. The
        ///   algorithm will stop when the change in the likelihood for two consecutive iterations
        ///   has not changed by more than L percent of the likelihood. If left as zero, the
        ///   algorithm will ignore this parameter and iterate over a number of fixed iterations
        ///   specified by the previous parameter.
        /// </remarks>
        /// 
        public double Tolerance
        {
            get { return convergence.Tolerance; }
            set { convergence.Tolerance = value; }
        }

        /// <summary>
        ///   Gets or sets the maximum number of iterations
        ///   performed by the learning algorithm.
        /// </summary>
        /// 
        /// <remarks>
        ///   This is the maximum number of iterations to be performed by the learning algorithm. If
        ///   specified as zero, the algorithm will learn until convergence of the model average
        ///   likelihood respecting the desired limit.
        /// </remarks>
        /// 
        public int Iterations
        {
            get { return convergence.Iterations; }
            set { convergence.Iterations = value; }
        }

        /// <summary>
        ///   Gets the Ksi matrix of log probabilities created during
        ///   the last iteration of the Baum-Welch learning algorithm.
        /// </summary>
        /// 
        public double[][][,] LogKsi { get; protected set; }

        /// <summary>
        ///   Gets the Gamma matrix of log probabilities created during
        ///   the last iteration of the Baum-Welch learning algorithm.
        /// </summary>
        /// 
        public double[][,] LogGamma { get; protected set; }

        /// <summary>
        ///   Gets the sample weights in the last iteration of the
        ///   Baum-Welch learning algorithm.
        /// </summary>
        /// 
        public double[] LogWeights { get; protected set; }


        /// <summary>
        ///   Learns a model that can map the given inputs to the desired outputs.
        /// </summary>
        /// 
        /// <param name="x">The model inputs.</param>
        /// <param name="weights">The weight of importance for each input sample.</param>
        /// 
        /// <returns>A model that has learned how to produce suitable outputs
        ///   given the input data <paramref name="x"/>.</returns>
        /// 
        public TModel Learn(TObservation[][] x, double[] weights = null)
        {
            // Initial argument checks
            CheckArgs(x, weights);

            // Baum-Welch algorithm.

            // The Baum–Welch algorithm is a particular case of a generalized expectation-maximization
            // (GEM) algorithm. It can compute maximum likelihood estimates and posterior mode estimates
            // for the parameters (transition and emission probabilities) of an HMM, when given only
            // emissions as training data.

            // The algorithm has two steps:
            //  - Calculating the forward probability and the backward probability for each HMM state;
            //  - On the basis of this, determining the frequency of the transition-emission pair values
            //    and dividing it by the probability of the entire string. This amounts to calculating
            //    the expected count of the particular transition-emission pair. Each time a particular
            //    transition is found, the value of the quotient of the transition divided by the probability
            //    of the entire string goes up, and this value can then be made the new value of the transition.


            samples = x.Concatenate();
            vectorObservations = x;
            sampleWeights = new double[samples.Length];

            if (Model == null)
                throw new InvalidOperationException("The model must have been created first.");
                //Model = CreateModel(observations);

            // Grab model information
            int states = Model.States;
            var logA = Model.LogTransitions;
            var logP = Model.LogInitial;


            // Initialize the algorithm
            int N = x.Length;
            double logN = Math.Log(N);
            LogKsi = new double[N][][,];
            LogGamma = new double[N][,];
            LogWeights = new double[N];
            if (weights != null)
                weights.Log(result: LogWeights);


            for (int i = 0; i < x.Length; i++)
            {
                int T = x[i].Length;

                LogKsi[i] = new double[T][,];
                LogGamma[i] = new double[T, states];

                for (int t = 0; t < LogKsi[i].Length; t++)
                    LogKsi[i][t] = new double[states, states];
            }


            int TMax = x.Apply(x_i => x_i.Length).Max();
            double[,] lnFwd = new double[TMax, states];
            double[,] lnBwd = new double[TMax, states];

            // Initialize the model log-likelihoods
            double newLogLikelihood = Double.NegativeInfinity;
            convergence.NewValue = Double.NegativeInfinity;


            while (true) // Until convergence or max iterations is reached
            {
                // For each sequence in the observations input
                for (int i = 0; i < x.Length; i++)
                {
                    int T = x[i].Length;
                    double[,] logGamma = LogGamma[i];
                    double w = LogWeights[i];


                    // 1st step - Calculating the forward probability and the
                    //            backward probability for each HMM state.
                    ComputeForwardBackward(i, lnFwd, lnBwd);


                    // 2nd step - Determining the frequency of the transition-emission pair values
                    //            and dividing it by the probability of the entire string.

                    // Calculate gamma values for next computations
                    for (int t = 0; t < T; t++)
                    {
                        double lnsum = Double.NegativeInfinity;
                        for (int k = 0; k < states; k++)
                        {
                            logGamma[t, k] = lnFwd[t, k] + lnBwd[t, k] + w;
                            lnsum = Special.LogSum(lnsum, logGamma[t, k]);
                        }

                        Accord.Diagnostics.Debug.Assert(!Double.IsNaN(lnsum));

                        // Normalize if different from zero
                        if (lnsum != Double.NegativeInfinity)
                            for (int k = 0; k < states; k++)
                                logGamma[t, k] = logGamma[t, k] - lnsum;
                    }

                    // Calculate ksi values for next computations
                    ComputeKsi(i, lnFwd, lnBwd);

                    // Compute log-likelihood for the given sequence
                    for (int j = 0; j < states; j++)
                        newLogLikelihood = Special.LogSum(newLogLikelihood, lnFwd[T - 1, j]);
                }


                // Average the likelihood for all sequences
                newLogLikelihood /= x.Length;
                convergence.NewValue = newLogLikelihood;
                LogLikelihood = newLogLikelihood;

                // Check for convergence
                if (convergence.HasConverged)
                    break;

                // 3. Continue with parameter re-estimation
                newLogLikelihood = Double.NegativeInfinity;

                // 3.1 Re-estimation of initial state probabilities 
                for (int i = 0; i < logP.Length; i++)
                {
                    double lnsum = Double.NegativeInfinity;
                    for (int k = 0; k < LogGamma.Length; k++)
                        lnsum = Special.LogSum(lnsum, LogGamma[k][0, i]);
                    logP[i] = lnsum - logN;
                }

                // 3.2 Re-estimation of transition probabilities 
                for (int i = 0; i < states; i++)
                {
                    for (int j = 0; j < states; j++)
                    {
                        double lnnum = Double.NegativeInfinity;
                        double lnden = Double.NegativeInfinity;

                        for (int k = 0; k < LogGamma.Length; k++)
                        {
                            int T = x[k].Length;

                            for (int t = 0; t < T - 1; t++)
                            {
                                lnnum = Special.LogSum(lnnum, LogKsi[k][t][i, j]);
                                lnden = Special.LogSum(lnden, LogGamma[k][t, i]);
                            }
                        }

                        logA[i][j] = (lnnum == lnden) ? 0 : lnnum - lnden;

                        Accord.Diagnostics.Debug.Assert(!Double.IsNaN(logA[i][j]));
                    }
                }

                // 3.3 Re-estimation of emission probabilities
                UpdateEmissions(); // discrete and continuous
            }

            // Returns the model average log-likelihood
            return Model;
        }

        private static void CheckArgs(TObservation[][] observations, double[] weights)
        {
            for (int i = 0; i < observations.Length; i++)
                if (observations[i].Length == 0)
                    throw new ArgumentException("observations");

            if (weights != null)
                if (weights.Length != observations.Length)
                    throw new DimensionMismatchException("weights");
        }


        /// <summary>
        ///   Computes the ksi matrix of probabilities for a given observation
        ///   referenced by its index in the input training data.
        /// </summary>
        /// <param name="index">The index of the observation in the input training data.</param>
        /// <param name="lnFwd">The matrix of forward probabilities for the observation.</param>
        /// <param name="lnBwd">The matrix of backward probabilities for the observation.</param>
        /// 
        protected void ComputeKsi(int index, double[,] lnFwd, double[,] lnBwd)
        {
            int states = Model.States;
            double[][] logA = Model.LogTransitions;
            TDistribution[] B = Model.Emissions;

            var sequence = vectorObservations[index];

            int T = sequence.Length;
            var logKsi = LogKsi[index];
            var w = LogWeights[index];


            for (int t = 0; t < T - 1; t++)
            {
                double lnsum = Double.NegativeInfinity;
                TObservation x = sequence[t + 1];

                for (int i = 0; i < states; i++)
                {
                    for (int j = 0; j < states; j++)
                    {
                        double b = B[j].LogProbabilityFunction(x);
                        logKsi[t][i, j] = lnFwd[t, i] + lnBwd[t + 1, j] + logA[i][j] + b + w;
                        lnsum = Special.LogSum(lnsum, logKsi[t][i, j]);
                    }
                }

                Accord.Diagnostics.Debug.Assert(!Double.IsNaN(lnsum));

                // Normalize if different from zero
                if (lnsum != Double.NegativeInfinity)
                    for (int i = 0; i < states; i++)
                        for (int j = 0; j < states; j++)
                            logKsi[t][i, j] = logKsi[t][i, j] - lnsum;
            }
        }

        /// <summary>
        ///   Updates the emission probability matrix.
        /// </summary>
        /// <remarks>
        ///   Implementations of this method should use the observations
        ///   in the training data and the Gamma probability matrix to
        ///   update the probability distributions of symbol emissions.
        /// </remarks>
        /// 
        protected void UpdateEmissions()
        {
            var B = Model.Emissions;

            // For each state i in the model
            for (int i = 0; i < B.Length; i++)
            {
                double lnsum = Double.NegativeInfinity;

                // For each observation sequence k
                for (int k = 0, w = 0; k < vectorObservations.Length; k++)
                {
                    int T = vectorObservations[k].Length;

                    // For each observation t in k
                    for (int t = 0; t < T; t++, w++)
                    {
                        sampleWeights[w] = LogGamma[k][t, i];
                        lnsum = Special.LogSum(lnsum, sampleWeights[w]);
                    }
                }

                Accord.Diagnostics.Debug.Assert(!Double.IsNaN(lnsum));

                if (lnsum != Double.NegativeInfinity)
                    for (int w = 0; w < sampleWeights.Length; w++)
                        sampleWeights[w] = sampleWeights[w] - lnsum;


                // Convert to probabilities
                for (int w = 0; w < sampleWeights.Length; w++)
                {
                    double p = Math.Exp(sampleWeights[w]);
                    sampleWeights[w] = (Double.IsNaN(p) || Double.IsInfinity(p)) ? 0.0 : p;
                }

                // Estimate the distribution for state i
                Fit(i, samples, sampleWeights);
            }
        }

        /// <summary>
        ///   Fits one emission distribution. This method can be override in a
        ///   base class in order to implement special fitting options.
        /// </summary>
        /// 
        protected virtual void Fit(int index, TObservation[] values, double[] weights)
        {
            if (FittingOptions == null)
                Model.Emissions[index].Fit(values, weights);
            else
                Model.Emissions[index].Fit(values, weights, FittingOptions);
        }


        /// <summary>
        ///   Computes the forward and backward probabilities matrices
        ///   for a given observation referenced by its index in the
        ///   input training data.
        /// </summary>
        /// <param name="index">The index of the observation in the input training data.</param>
        /// <param name="lnFwd">Returns the computed forward probabilities matrix.</param>
        /// <param name="lnBwd">Returns the computed backward probabilities matrix.</param>
        /// 
        protected void ComputeForwardBackward(int index, double[,] lnFwd, double[,] lnBwd)
        {
            int states = Model.States;
            int T = vectorObservations[index].Length;

            Accord.Diagnostics.Debug.Assert(lnBwd.GetLength(0) >= T);
            Accord.Diagnostics.Debug.Assert(lnBwd.GetLength(1) == states);
            Accord.Diagnostics.Debug.Assert(lnFwd.GetLength(0) >= T);
            Accord.Diagnostics.Debug.Assert(lnFwd.GetLength(1) == states);

            ForwardBackwardAlgorithm.LogForward(Model, vectorObservations[index], lnFwd);
            ForwardBackwardAlgorithm.LogBackward(Model, vectorObservations[index], lnBwd);
        }

    }
}
