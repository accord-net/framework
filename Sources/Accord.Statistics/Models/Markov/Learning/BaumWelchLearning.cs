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
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Fitting;

    /// <summary>
    ///   Baum-Welch learning algorithm for <see cref="HiddenMarkovModel">
    ///   discrete-density Hidden Markov Models</see>.
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>
    ///   The Baum-Welch algorithm is an <see cref="IUnsupervisedLearning">unsupervised algorithm</see>
    ///   used to learn a single hidden Markov model object from a set of observation sequences. It works
    ///   by using a variant of the <see cref="Mixture{T}.Fit(double[], double[], MixtureOptions)">
    ///   Expectation-Maximization</see> algorithm to search a set of model parameters (i.e. the matrix
    ///   of <see cref="IHiddenMarkovModel.Transitions">transition probabilities A</see>, the matrix 
    ///   of <see cref="HiddenMarkovModel.Emissions">emission probabilities B</see>, and the
    ///   <see cref="IHiddenMarkovModel.Probabilities">initial probability vector π</see>) that 
    ///   would result in a model having a high likelihood of being able 
    ///   to <see cref="HiddenMarkovModel.Generate(int)">generate</see> a set of training 
    ///   sequences given to this algorithm.</para>
    ///   
    ///   <para>
    ///   For increased accuracy, this class performs all computations using log-probabilities.</para>
    ///     
    ///   <para>
    ///   For a more thorough explanation on <see cref="HiddenMarkovModel">hidden Markov models</see>
    ///   with practical examples on gesture recognition, please see 
    ///   <a href="http://www.codeproject.com/Articles/541428/Sequence-Classifiers-in-Csharp-Part-I-Hidden-Marko">
    ///   Sequence Classifiers in C#, Part I: Hidden Markov Models</a> [1].</para>
    ///     
    /// <para>
    ///   [1]: <a href="http://www.codeproject.com/Articles/541428/Sequence-Classifiers-in-Csharp-Part-I-Hidden-Marko"> 
    ///           http://www.codeproject.com/Articles/541428/Sequence-Classifiers-in-Csharp-Part-I-Hidden-Marko </a>
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    ///   <code>
    ///   // We will try to create a Hidden Markov Model which
    ///   //  can detect if a given sequence starts with a zero
    ///   //  and has any number of ones after that.
    ///   int[][] sequences = new int[][] 
    ///   {
    ///       new int[] { 0,1,1,1,1,0,1,1,1,1 },
    ///       new int[] { 0,1,1,1,0,1,1,1,1,1 },
    ///       new int[] { 0,1,1,1,1,1,1,1,1,1 },
    ///       new int[] { 0,1,1,1,1,1         },
    ///       new int[] { 0,1,1,1,1,1,1       },
    ///       new int[] { 0,1,1,1,1,1,1,1,1,1 },
    ///       new int[] { 0,1,1,1,1,1,1,1,1,1 },
    ///   };
    ///   
    ///   // Creates a new Hidden Markov Model with 3 states for
    ///   //  an output alphabet of two characters (zero and one)
    ///   HiddenMarkovModel hmm = new HiddenMarkovModel(3, 2);
    ///   
    ///   // Try to fit the model to the data until the difference in
    ///   //  the average log-likelihood changes only by as little as 0.0001
    ///   var teacher = new BaumWelchLearning(hmm) { Tolerance = 0.0001, Iterations = 0 };
    ///   double ll = teacher.Run(sequences);
    ///   
    ///   // Calculate the probability that the given
    ///   //  sequences originated from the model
    ///   double l1 = hmm.Evaluate(new int[] { 0, 1 });       // 0.999
    ///   double l2 = hmm.Evaluate(new int[] { 0, 1, 1, 1 }); // 0.916
    ///   
    ///   // Sequences which do not start with zero have much lesser probability.
    ///   double l3 = hmm.Evaluate(new int[] { 1, 1 });       // 0.000
    ///   double l4 = hmm.Evaluate(new int[] { 1, 0, 0, 0 }); // 0.000
    ///   
    ///   // Sequences which contains few errors have higher probability
    ///   //  than the ones which do not start with zero. This shows some
    ///   //  of the temporal elasticity and error tolerance of the HMMs.
    ///   double l5 = hmm.Evaluate(new int[] { 0, 1, 0, 1, 1, 1, 1, 1, 1 }); // 0.034
    ///   double l6 = hmm.Evaluate(new int[] { 0, 1, 1, 1, 1, 1, 1, 0, 1 }); // 0.034
    ///   </code>
    /// </example>
    /// 
    /// <seealso cref="HiddenMarkovModel"/>
    /// <seealso cref="HiddenMarkovModel{TDistribution}"/>
    /// <seealso cref="BaumWelchLearning{TDistribution}"/>
    /// 
    public class BaumWelchLearning : BaseBaumWelchLearning, IUnsupervisedLearning, 
        IConvergenceLearning, IUnsupervisedLearning<int[]>
    {

        private HiddenMarkovModel model;
        private int[][] discreteObservations;

        /// <summary>
        ///   Gets the model being trained.
        /// </summary>
        /// 
        public HiddenMarkovModel Model
        {
            get { return model; }
        }


        /// <summary>
        ///   Creates a new instance of the Baum-Welch learning algorithm.
        /// </summary>
        /// 
        public BaumWelchLearning(HiddenMarkovModel model)
            : base(model)
        {
            this.model = model;
        }


        /// <summary>
        ///   Runs the Baum-Welch learning algorithm for hidden Markov models.
        /// </summary>
        /// 
        /// <param name="observations">An array of observation sequences to be used to train the model.</param>
        /// 
        /// <returns>
        ///   The average log-likelihood for the observations after the model has been trained.
        /// </returns>
        /// 
        /// <remarks>
        ///   Learning problem. Given some training observation sequences O = {o1, o2, ..., oK}
        ///   and general structure of HMM (numbers of hidden and visible states), determine
        ///   HMM parameters M = (A, B, pi) that best fit training data.
        /// </remarks>
        /// 
        public double Run(params int[][] observations)
        {
            if (observations == null)
                throw new ArgumentNullException("observations");

            for (int i = 0; i < observations.Length; i++)
            {
                for (int j = 0; j < observations[i].Length; j++)
                {
                    int symbol = observations[i][j];

                    if (symbol < 0 || symbol >= model.Symbols)
                    {
                        string message = "Observation sequences should only contain symbols that are " +
                        "greater than or equal to 0, and lesser than the number of symbols passed to " +
                        "the HiddenMarkovModel. This model is expecting at most {0} symbols.";

                        throw new ArgumentOutOfRangeException("observations", String.Format(message, model.Symbols));
                    }
                }
            }

            this.discreteObservations = observations;

            return base.Run(discreteObservations);
        }


        /// <summary>
        ///   Runs the Baum-Welch learning algorithm for hidden Markov models.
        /// </summary>
        /// 
        /// <param name="observations">The sequences of univariate or multivariate observations used to train the model.
        ///   Can be either of type double[] (for the univariate case) or double[][] for the
        ///   multivariate case.</param>
        ///   
        /// <returns>
        ///   The average log-likelihood for the observations after the model has been trained.
        /// </returns>
        /// 
        /// <remarks>
        ///   Learning problem. Given some training observation sequences O = {o1, o2, ..., oK}
        ///   and general structure of HMM (numbers of hidden and visible states), determine
        ///   HMM parameters M = (A, B, pi) that best fit training data.
        /// </remarks>
        /// 
        double IUnsupervisedLearning.Run(Array[] observations)
        {
            return Run(observations as int[][]);
        }

        /// <summary>
        ///   Computes the forward and backward probabilities matrices
        ///   for a given observation referenced by its index in the
        ///   input training data.
        /// </summary>
        /// 
        /// <param name="index">The index of the observation in the input training data.</param>
        /// <param name="lnFwd">Returns the computed forward probabilities matrix.</param>
        /// <param name="lnBwd">Returns the computed backward probabilities matrix.</param>
        /// 
        protected override void ComputeForwardBackward(int index, double[,] lnFwd, double[,] lnBwd)
        {
            int states = model.States;
            int T = discreteObservations[index].Length;

            System.Diagnostics.Debug.Assert(lnBwd.GetLength(0) >= T);
            System.Diagnostics.Debug.Assert(lnBwd.GetLength(1) == states);
            System.Diagnostics.Debug.Assert(lnFwd.GetLength(0) >= T);
            System.Diagnostics.Debug.Assert(lnFwd.GetLength(1) == states);

            ForwardBackwardAlgorithm.LogForward(model, discreteObservations[index], lnFwd);
            ForwardBackwardAlgorithm.LogBackward(model, discreteObservations[index], lnBwd);
        }

        /// <summary>
        ///   Updates the emission probability matrix.
        /// </summary>
        /// 
        /// <remarks>
        ///   Implementations of this method should use the observations
        ///   in the training data and the Gamma probability matrix to
        ///   update the probability distributions of symbol emissions.
        /// </remarks>
        /// 
        protected override void UpdateEmissions()
        {
            var B = model.Emissions;
            int states = model.States;
            int symbols = model.Symbols;

            for (int i = 0; i < states; i++)
            {
                for (int j = 0; j < symbols; j++)
                {
                    double lnnum = Double.NegativeInfinity;
                    double lnden = Double.NegativeInfinity;

                    for (int k = 0; k < discreteObservations.Length; k++)
                    {
                        int T = discreteObservations[k].Length;
                        var gammak = LogGamma[k];

                        for (int t = 0; t < T; t++)
                        {
                            if (discreteObservations[k][t] == j)
                                lnnum = Special.LogSum(lnnum, gammak[t, i]);
                            lnden = Special.LogSum(lnden, gammak[t, i]);
                        }
                    }

                    // TODO: avoid locking a parameter in zero.
                    B[i, j] = lnnum - lnden;
                }
            }
        }

        /// <summary>
        ///   Computes the ksi matrix of probabilities for a given observation
        ///   referenced by its index in the input training data.
        /// </summary>
        /// 
        /// <param name="index">The index of the observation in the input training data.</param>
        /// <param name="lnFwd">The matrix of forward probabilities for the observation.</param>
        /// <param name="lnBwd">The matrix of backward probabilities for the observation.</param>
        /// 
        protected override void ComputeKsi(int index, double[,] lnFwd, double[,] lnBwd)
        {
            int states = model.States;
            var logA = model.Transitions;
            var logB = model.Emissions;

            var sequence = discreteObservations[index];
            int T = sequence.Length;
            var logKsi = LogKsi[index];
            var w = LogWeights[index];


            for (int t = 0; t < T - 1; t++)
            {
                double lnsum = Double.NegativeInfinity;
                int x = sequence[t + 1];

                for (int i = 0; i < states; i++)
                {
                    for (int j = 0; j < states; j++)
                    {
                        logKsi[t][i, j] = lnFwd[t, i] + logA[i, j] + logB[j, x] + lnBwd[t + 1, j] + w;
                        lnsum = Special.LogSum(lnsum, logKsi[t][i, j]);
                    }
                }

                for (int i = 0; i < states; i++)
                    for (int j = 0; j < states; j++)
                        logKsi[t][i, j] = logKsi[t][i, j] - lnsum;
            }
        }

        /// <summary>
        ///   Creates a Baum-Welch with default configurations for
        ///   hidden Markov models with normal mixture densities.
        /// </summary>
        /// 
        public static BaumWelchLearning<Mixture<NormalDistribution>> FromMixtureModel(
            HiddenMarkovModel<Mixture<NormalDistribution>> model, NormalOptions options)
        {
            MixtureOptions mixOptions = new MixtureOptions()
            {
                Iterations = 1,
                InnerOptions = options
            };

            return new BaumWelchLearning<Mixture<NormalDistribution>>(model)
            {
                FittingOptions = mixOptions
            };
        }

        /// <summary>
        ///   Creates a Baum-Welch with default configurations for
        ///   hidden Markov models with normal mixture densities.
        /// </summary>
        /// 
        public static BaumWelchLearning<MultivariateMixture<MultivariateNormalDistribution>> FromMixtureModel(
            HiddenMarkovModel<MultivariateMixture<MultivariateNormalDistribution>> model, NormalOptions options)
        {
            MixtureOptions mixOptions = new MixtureOptions()
            {
                Iterations = 1,
                InnerOptions = options
            };

            return new BaumWelchLearning<MultivariateMixture<MultivariateNormalDistribution>>(model)
            {
                FittingOptions = mixOptions
            };
        }
    }
}
