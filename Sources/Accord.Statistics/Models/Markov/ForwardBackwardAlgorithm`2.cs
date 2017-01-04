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
    using Accord.Statistics.Distributions;
    using Accord.Math;

    /// <summary>
    ///   Forward-Backward algorithms for Hidden Markov Models.
    /// </summary>
    /// 
    public static partial class ForwardBackwardAlgorithm
    {

        /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        public static void Forward<TDistribution, TObservation>(HiddenMarkovModel<TDistribution, TObservation> model, TObservation[] observations, double[] scaling, double[,] fwd)
                       where TDistribution : IDistribution<TObservation>
        {
            int states = model.NumberOfStates;
            var A = Elementwise.Exp(model.LogTransitions);
            var B = model.Emissions;
            var pi = Elementwise.Exp(model.LogInitial);

            int T = observations.Length;
            double s = 0;

            // Ensures minimum requirements
            Accord.Diagnostics.Debug.Assert(fwd.GetLength(0) >= T);
            Accord.Diagnostics.Debug.Assert(fwd.GetLength(1) == states);
            Accord.Diagnostics.Debug.Assert(scaling.Length >= T);
            Array.Clear(fwd, 0, fwd.Length);


            // 1. Initialization
            for (int i = 0; i < states; i++)
                s += fwd[0, i] = pi[i] * B[i].ProbabilityFunction(observations[0]);
            scaling[0] = s;

            if (s != 0) // Scaling
            {
                for (int i = 0; i < states; i++)
                    fwd[0, i] /= s;
            }


            // 2. Induction
            for (int t = 1; t < T; t++)
            {
                TObservation obs = observations[t];
                s = 0;

                for (int i = 0; i < states; i++)
                {
                    double sum = 0.0;
                    for (int j = 0; j < states; j++)
                        sum += fwd[t - 1, j] * A[j][i];
                    fwd[t, i] = sum * B[i].ProbabilityFunction(obs);

                    s += fwd[t, i]; // scaling coefficient
                }

                scaling[t] = s;

                if (s != 0) // Scaling
                {
                    for (int i = 0; i < states; i++)
                        fwd[t, i] /= s;
                }
            }

            Accord.Diagnostics.Debug.Assert(!fwd.HasNaN());
        }

        /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        public static double[,] Forward<TDistribution, TObservation>(HiddenMarkovModel<TDistribution, TObservation> model, TObservation[] observations, out double logLikelihood)
            where TDistribution : IDistribution<TObservation>
        {
            double[,] fwd = new double[observations.Length, model.NumberOfStates];
            double[] scaling = new double[observations.Length];

            ForwardBackwardAlgorithm.Forward<TDistribution, TObservation>(model, observations, scaling, fwd);

            logLikelihood = 0;
            for (int i = 0; i < scaling.Length; i++)
                logLikelihood += Math.Log(scaling[i]);

            return fwd;
        }

        /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        public static double[,] Forward<TDistribution, TObservation>(HiddenMarkovModel<TDistribution, TObservation> model, TObservation[] observations, out double[] scaling)
            where TDistribution : IDistribution<TObservation>
        {
            double[,] fwd = new double[observations.Length, model.NumberOfStates];
            scaling = new double[observations.Length];
            Forward<TDistribution, TObservation>(model, observations, scaling, fwd);
            return fwd;
        }

        /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        public static double[,] Forward<TDistribution, TObservation>(HiddenMarkovModel<TDistribution, TObservation> model, TObservation[] observations, out double[] scaling, out double logLikelihood)
            where TDistribution : IDistribution<TObservation>
        {
            double[,] fwd = new double[observations.Length, model.NumberOfStates];
            scaling = new double[observations.Length];
            Forward<TDistribution, TObservation>(model, observations, scaling, fwd);

            logLikelihood = 0;
            for (int i = 0; i < scaling.Length; i++)
                logLikelihood += Math.Log(scaling[i]);

            return fwd;
        }



        /// <summary>
        ///   Computes Backward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        /// 
        public static void Backward<TDistribution, TObservation>(HiddenMarkovModel<TDistribution, TObservation> model, TObservation[] observations, double[] scaling, double[,] bwd)
            where TDistribution : IDistribution<TObservation>
        {
            int states = model.NumberOfStates;
            var A = Elementwise.Exp(model.LogTransitions);
            var B = model.Emissions;

            int T = observations.Length;

            // Ensures minimum requirements
            Accord.Diagnostics.Debug.Assert(bwd.GetLength(0) >= T);
            Accord.Diagnostics.Debug.Assert(bwd.GetLength(1) == states);
            Array.Clear(bwd, 0, bwd.Length);

            // For backward variables, we use the same scale factors
            //   for each time t as were used for forward variables.

            // 1. Initialization
            for (int i = 0; i < states; i++)
                bwd[T - 1, i] = 1.0 / scaling[T - 1];

            // 2. Induction
            for (int t = T - 2; t >= 0; t--)
            {
                for (int i = 0; i < states; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < states; j++)
                        sum += A[i][j] * B[j].ProbabilityFunction(observations[t + 1]) * bwd[t + 1, j];
                    bwd[t, i] = sum / scaling[t];
                }
            }

            Accord.Diagnostics.Debug.Assert(!bwd.HasNaN());
        }

        /// <summary>
        ///   Computes Backward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        /// 
        public static double[,] Backward<TDistribution, TObservation>(HiddenMarkovModel<TDistribution, TObservation> model, TObservation[] observations, double[] scaling)
             where TDistribution : IDistribution<TObservation>
        {
            int states = model.NumberOfStates;
            int T = observations.Length;
            double[,] bwd = new double[T, states];
            Backward<TDistribution, TObservation>(model, observations, scaling, bwd);
            return bwd;
        }



        /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        public static void LogForward<TDistribution, TObservation>(HiddenMarkovModel<TDistribution, TObservation> model, TObservation[] observations, double[,] lnFwd)
                       where TDistribution : IDistribution<TObservation>
        {
            int states = model.NumberOfStates;
            var logA = model.LogTransitions;
            var logB = model.Emissions;
            var logPi = model.LogInitial;

            int T = observations.Length;

            // Ensures minimum requirements
            Accord.Diagnostics.Debug.Assert(lnFwd.GetLength(0) >= T);
            Accord.Diagnostics.Debug.Assert(lnFwd.GetLength(1) == states);
            Array.Clear(lnFwd, 0, lnFwd.Length);


            // 1. Initialization
            for (int i = 0; i < states; i++)
                lnFwd[0, i] = logPi[i] + logB[i].LogProbabilityFunction(observations[0]);

            // 2. Induction
            for (int t = 1; t < T; t++)
            {
                TObservation x = observations[t];

                for (int i = 0; i < states; i++)
                {
                    double sum = Double.NegativeInfinity;
                    for (int j = 0; j < states; j++)
                        sum = Special.LogSum(sum, lnFwd[t - 1, j] + logA[j][i]);
                    lnFwd[t, i] = sum + logB[i].LogProbabilityFunction(x);
                }
            }

            Accord.Diagnostics.Debug.Assert(!lnFwd.HasNaN());
        }

        /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        public static double[,] LogForward<TDistribution, TObservation>(HiddenMarkovModel<TDistribution, TObservation> model, TObservation[] observations, out double logLikelihood)
            where TDistribution : IDistribution<TObservation>
        {
            int T = observations.Length;
            int states = model.NumberOfStates;

            double[,] lnFwd = new double[T, states];

            ForwardBackwardAlgorithm.LogForward<TDistribution, TObservation>(model, observations, lnFwd);

            logLikelihood = Double.NegativeInfinity;
            for (int i = 0; i < states; i++)
                logLikelihood = Special.LogSum(logLikelihood, lnFwd[observations.Length - 1, i]);

            return lnFwd;
        }

         /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        public static double[,] LogForward<TDistribution, TObservation>(HiddenMarkovModel<TDistribution, TObservation> model, TObservation[] observations)
                       where TDistribution : IDistribution<TObservation>
        {
            int T = observations.Length;
            int states = model.NumberOfStates;

            double[,] lnFwd = new double[T, states];

            ForwardBackwardAlgorithm.LogForward<TDistribution, TObservation>(model, observations, lnFwd);

            return lnFwd;
        }

        /// <summary>
        ///   Computes Backward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        /// 
        public static void LogBackward<TDistribution, TObservation>(HiddenMarkovModel<TDistribution, TObservation> model, TObservation[] observations, double[,] lnBwd)
            where TDistribution : IDistribution<TObservation>
        {
            int states = model.NumberOfStates;
            var logA = model.LogTransitions;
            var logB = model.Emissions;
            var logPi = model.LogInitial;

            int T = observations.Length;

            // Ensures minimum requirements
            Accord.Diagnostics.Debug.Assert(lnBwd.GetLength(0) >= T);
            Accord.Diagnostics.Debug.Assert(lnBwd.GetLength(1) == states);
            Array.Clear(lnBwd, 0, lnBwd.Length);


            // 1. Initialization
            for (int i = 0; i < states; i++)
                lnBwd[T - 1, i] = 0;

            // 2. Induction
            for (int t = T - 2; t >= 0; t--)
            {
                for (int i = 0; i < states; i++)
                {
                    double sum = Double.NegativeInfinity;
                    for (int j = 0; j < states; j++)
                        sum = Special.LogSum(sum, lnBwd[t + 1, j] + logA[i][j] + logB[j].LogProbabilityFunction(observations[t + 1]));
                    lnBwd[t, i] = sum;
                }
            }

            Accord.Diagnostics.Debug.Assert(!lnBwd.HasNaN());
        }

        /// <summary>
        ///   Computes Backward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        /// 
        public static double[,] LogBackward<TDistribution, TObservation>(HiddenMarkovModel<TDistribution, TObservation> model, TObservation[] observations)
            where TDistribution : IDistribution<TObservation>
        {
            int states = model.NumberOfStates;

            int T = observations.Length;
            double[,] lnBwd = new double[T, states];
            LogBackward(model, observations, lnBwd);
            return lnBwd;
        }

        /// <summary>
        ///   Computes Backward probabilities for a given hidden Markov model and a set of observations (no scaling).
        /// </summary>
        public static double[,] LogBackward<TDistribution, TObservation>(HiddenMarkovModel<TDistribution, TObservation> model, TObservation[] observations, out double logLikelihood)
            where TDistribution : IDistribution<TObservation>
        {
            int states = model.NumberOfStates;

            int T = observations.Length;
            double[,] lnBwd = new double[T, states];
            LogBackward(model, observations, lnBwd);

            logLikelihood = Double.NegativeInfinity;
            for (int i = 0; i < model.NumberOfStates; i++)
            {
                logLikelihood = Special.LogSum(logLikelihood, lnBwd[0, i] + model.LogInitial[i]
                    + model.Emissions[i].LogProbabilityFunction(observations[0]));
            }

            return lnBwd;
        }

    }
}
