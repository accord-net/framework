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

namespace Accord.Statistics.Models.Markov
{

    using System;
    using Accord.Statistics.Distributions;
    using Accord.Math;

    /// <summary>
    ///   Forward-Backward algorithms for Hidden Markov Models.
    /// </summary>
    /// 
    public static class ForwardBackwardAlgorithm
    {

        /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        /// 
        public static void Forward(HiddenMarkovModel model, int[] observations, double[] scaling, double[,] fwd)
        {
            int states = model.States;
            var A = Matrix.Exp(model.Transitions);
            var B = Matrix.Exp(model.Emissions);
            var pi = Matrix.Exp(model.Probabilities);

            int T = observations.Length;
            double s = 0;

            // Ensures minimum requirements
            System.Diagnostics.Debug.Assert(fwd.GetLength(0) >= T);
            System.Diagnostics.Debug.Assert(fwd.GetLength(1) == states);
            System.Diagnostics.Debug.Assert(scaling.Length >= T);
            Array.Clear(fwd, 0, fwd.Length);


            // 1. Initialization
            for (int i = 0; i < states; i++)
                s += fwd[0, i] = pi[i] * B[i, observations[0]];
            scaling[0] = s;

            if (s != 0) // Scaling
            {
                for (int i = 0; i < states; i++)
                    fwd[0, i] /= s;
            }


            // 2. Induction
            for (int t = 1; t < T; t++)
            {
                int obs = observations[t];
                s = 0;

                for (int i = 0; i < states; i++)
                {
                    double sum = 0.0;
                    for (int j = 0; j < states; j++)
                        sum += fwd[t - 1, j] * A[j, i];
                    fwd[t, i] = sum * B[i, obs];

                    s += fwd[t, i]; // scaling coefficient
                }

                scaling[t] = s;

                if (s != 0) // Scaling
                {
                    for (int i = 0; i < states; i++)
                        fwd[t, i] /= s;
                }
            }

            System.Diagnostics.Debug.Assert(!fwd.HasNaN());
        }

        /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations (no scaling).
        /// </summary>
        public static double[,] Forward(HiddenMarkovModel model, int[] observations)
        {
            int states = model.States;
            var A = Matrix.Exp(model.Transitions);
            var B = Matrix.Exp(model.Emissions);
            var pi = Matrix.Exp(model.Probabilities);

            int T = observations.Length;
            double[,] fwd = new double[T, states];

            // 1. Initialization
            for (int i = 0; i < states; i++)
                fwd[0, i] = pi[i] * B[i, observations[0]];

            // 2. Induction
            for (int t = 1; t < T; t++)
            {
                int obs = observations[t];

                for (int i = 0; i < states; i++)
                {
                    double sum = 0.0;
                    for (int j = 0; j < states; j++)
                        sum += fwd[t - 1, j] * A[j, i];
                    fwd[t, i] = sum * B[i, obs];
                }
            }

            System.Diagnostics.Debug.Assert(!fwd.HasNaN());

            return fwd;
        }

        /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        /// 
        public static double[,] Forward(HiddenMarkovModel model, int[] observations, out double logLikelihood)
        {
            double[,] fwd = new double[observations.Length, model.States];
            double[] coefficients = new double[observations.Length];

            ForwardBackwardAlgorithm.Forward(model, observations, coefficients, fwd);

            logLikelihood = 0;
            for (int i = 0; i < coefficients.Length; i++)
                logLikelihood += Math.Log(coefficients[i]);

            return fwd;
        }

        /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        /// 
        public static double[,] Forward(HiddenMarkovModel model, int[] observations, out double[] scaling)
        {
            double[,] fwd = new double[observations.Length, model.States];
            scaling = new double[observations.Length];
            Forward(model, observations, scaling, fwd);
            return fwd;
        }

        /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        /// 
        public static double[,] Forward(HiddenMarkovModel model, int[] observations, out double[] scaling, out double logLikelihood)
        {
            double[,] fwd = new double[observations.Length, model.States];
            scaling = new double[observations.Length];
            Forward(model, observations, scaling, fwd);

            logLikelihood = 0;
            for (int i = 0; i < scaling.Length; i++)
                logLikelihood += Math.Log(scaling[i]);

            return fwd;
        }


        /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        public static void Forward<TDistribution>(HiddenMarkovModel<TDistribution> model, double[][] observations, double[] scaling, double[,] fwd)
                       where TDistribution : IDistribution
        {
            int states = model.States;
            var A = Matrix.Exp(model.Transitions);
            var B = model.Emissions;
            var pi = Matrix.Exp(model.Probabilities);

            int T = observations.Length;
            double s = 0;

            // Ensures minimum requirements
            System.Diagnostics.Debug.Assert(fwd.GetLength(0) >= T);
            System.Diagnostics.Debug.Assert(fwd.GetLength(1) == states);
            System.Diagnostics.Debug.Assert(scaling.Length >= T);
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
                double[] obs = observations[t];
                s = 0;

                for (int i = 0; i < states; i++)
                {
                    double sum = 0.0;
                    for (int j = 0; j < states; j++)
                        sum += fwd[t - 1, j] * A[j, i];
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

            System.Diagnostics.Debug.Assert(!fwd.HasNaN());
        }

        /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        public static double[,] Forward<TDistribution>(HiddenMarkovModel<TDistribution> model, double[][] observations, out double logLikelihood)
            where TDistribution : IDistribution
        {
            double[,] fwd = new double[observations.Length, model.States];
            double[] scaling = new double[observations.Length];

            ForwardBackwardAlgorithm.Forward<TDistribution>(model, observations, scaling, fwd);

            logLikelihood = 0;
            for (int i = 0; i < scaling.Length; i++)
                logLikelihood += Math.Log(scaling[i]);

            return fwd;
        }

        /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        public static double[,] Forward<TDistribution>(HiddenMarkovModel<TDistribution> model, double[][] observations, out double[] scaling)
            where TDistribution : IDistribution
        {
            double[,] fwd = new double[observations.Length, model.States];
            scaling = new double[observations.Length];
            Forward<TDistribution>(model, observations, scaling, fwd);
            return fwd;
        }

        /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        public static double[,] Forward<TDistribution>(HiddenMarkovModel<TDistribution> model, double[][] observations, out double[] scaling, out double logLikelihood)
            where TDistribution : IDistribution
        {
            double[,] fwd = new double[observations.Length, model.States];
            scaling = new double[observations.Length];
            Forward<TDistribution>(model, observations, scaling, fwd);

            logLikelihood = 0;
            for (int i = 0; i < scaling.Length; i++)
                logLikelihood += Math.Log(scaling[i]);

            return fwd;
        }



        /// <summary>
        ///   Computes Backward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        /// 
        public static void Backward(HiddenMarkovModel model, int[] observations, double[] scaling, double[,] bwd)
        {
            int states = model.States;
            var A = Matrix.Exp(model.Transitions);
            var B = Matrix.Exp(model.Emissions);
            var pi = Matrix.Exp(model.Probabilities);

            int T = observations.Length;

            // Ensures minimum requirements
            System.Diagnostics.Debug.Assert(bwd.GetLength(0) >= T);
            System.Diagnostics.Debug.Assert(bwd.GetLength(1) == states);
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
                        sum += A[i, j] * B[j, observations[t + 1]] * bwd[t + 1, j];
                    bwd[t, i] += sum / scaling[t];
                }
            }

            System.Diagnostics.Debug.Assert(!bwd.HasNaN());
        }

        /// <summary>
        ///   Computes Backward probabilities for a given hidden Markov model and a set of observations (no scaling).
        /// </summary>
        public static double[,] Backward(HiddenMarkovModel model, int[] observations)
        {
            int states = model.States;
            var A = Matrix.Exp(model.Transitions);
            var B = Matrix.Exp(model.Emissions);
            var pi = Matrix.Exp(model.Probabilities);

            int T = observations.Length;
            double[,] bwd = new double[T, states];

            // 1. Initialization
            for (int i = 0; i < states; i++)
                bwd[T - 1, i] = 1.0;

            // 2. Induction
            for (int t = T - 2; t >= 0; t--)
            {
                for (int i = 0; i < states; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < states; j++)
                        sum += A[i, j] * B[j, observations[t + 1]] * bwd[t + 1, j];
                    bwd[t, i] += sum;
                }
            }

            System.Diagnostics.Debug.Assert(!bwd.HasNaN());

            return bwd;
        }

        /// <summary>
        ///   Computes Backward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        /// 
        public static double[,] Backward(HiddenMarkovModel model, int[] observations, double[] scaling)
        {
            int states = model.States;

            int T = observations.Length;
            double[,] bwd = new double[T, states];
            Backward(model, observations, scaling, bwd);
            return bwd;
        }

        /// <summary>
        ///   Computes Backward probabilities for a given hidden Markov model and a set of observations (no scaling).
        /// </summary>
        public static double[,] Backward(HiddenMarkovModel model, int[] observations, out double logLikelihood)
        {
            var bwd = Backward(model, observations);

            double likelihood = 0;
            for (int i = 0; i < model.States; i++)
                likelihood += bwd[0, i] * Math.Exp(model.Probabilities[i]) * Math.Exp(model.Emissions[i, observations[0]]);
            logLikelihood = Math.Log(likelihood);

            return bwd;
        }

        /// <summary>
        ///   Computes Backward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        /// 
        public static void Backward<TDistribution>(HiddenMarkovModel<TDistribution> model, double[][] observations, double[] scaling, double[,] bwd)
            where TDistribution : IDistribution
        {
            int states = model.States;
            var A = Matrix.Exp(model.Transitions);
            var B = model.Emissions;
            var pi = Matrix.Exp(model.Probabilities);

            int T = observations.Length;

            // Ensures minimum requirements
            System.Diagnostics.Debug.Assert(bwd.GetLength(0) >= T);
            System.Diagnostics.Debug.Assert(bwd.GetLength(1) == states);
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
                        sum += A[i, j] * B[j].ProbabilityFunction(observations[t + 1]) * bwd[t + 1, j];
                    bwd[t, i] += sum / scaling[t];
                }
            }

            System.Diagnostics.Debug.Assert(!bwd.HasNaN());
        }

        /// <summary>
        ///   Computes Backward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        /// 
        public static double[,] Backward<TDistribution>(HiddenMarkovModel<TDistribution> model, double[][] observations, double[] scaling)
             where TDistribution : IDistribution
        {
            int states = model.States;
            int T = observations.Length;
            double[,] bwd = new double[T, states];
            Backward<TDistribution>(model, observations, scaling, bwd);
            return bwd;
        }


        /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        /// 
        public static void LogForward(HiddenMarkovModel model, int[] observations, double[,] lnFwd)
        {
            int states = model.States;
            var logA = model.Transitions;
            var logB = model.Emissions;
            var logPi = model.Probabilities;

            int T = observations.Length;

            // Ensures minimum requirements
            System.Diagnostics.Debug.Assert(lnFwd.GetLength(0) >= T);
            System.Diagnostics.Debug.Assert(lnFwd.GetLength(1) == states);
            Array.Clear(lnFwd, 0, lnFwd.Length);


            // 1. Initialization
            for (int i = 0; i < states; i++)
                lnFwd[0, i] = logPi[i] + logB[i, observations[0]];

            // 2. Induction
            for (int t = 1; t < T; t++)
            {
                int x = observations[t];

                for (int i = 0; i < states; i++)
                {
                    double sum = Double.NegativeInfinity;
                    for (int j = 0; j < states; j++)
                        sum = Special.LogSum(sum, lnFwd[t - 1, j] + logA[j, i]);
                    lnFwd[t, i] = sum + logB[i, x];
                }
            }

            System.Diagnostics.Debug.Assert(!lnFwd.HasNaN());
        }

        /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        /// 
        public static double[,] LogForward(HiddenMarkovModel model, int[] observations, out double logLikelihood)
        {
            int states = model.States;

            double[,] lnFwd = new double[observations.Length, model.States];
            ForwardBackwardAlgorithm.LogForward(model, observations, lnFwd);

            logLikelihood = Double.NegativeInfinity;
            for (int i = 0; i < states; i++)
                logLikelihood = Special.LogSum(logLikelihood, lnFwd[observations.Length - 1, i]);

            return lnFwd;
        }

        /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        /// 
        public static double[,] LogForward(HiddenMarkovModel model, int[] observations)
        {
            int states = model.States;

            double[,] lnFwd = new double[observations.Length, model.States];
            ForwardBackwardAlgorithm.LogForward(model, observations, lnFwd);

            return lnFwd;
        }

        /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        public static void LogForward<TDistribution>(HiddenMarkovModel<TDistribution> model, double[][] observations, double[,] lnFwd)
                       where TDistribution : IDistribution
        {
            int states = model.States;
            var logA = model.Transitions;
            var logB = model.Emissions;
            var logPi = model.Probabilities;

            int T = observations.Length;

            // Ensures minimum requirements
            System.Diagnostics.Debug.Assert(lnFwd.GetLength(0) >= T);
            System.Diagnostics.Debug.Assert(lnFwd.GetLength(1) == states);
            Array.Clear(lnFwd, 0, lnFwd.Length);


            // 1. Initialization
            for (int i = 0; i < states; i++)
                lnFwd[0, i] = logPi[i] + logB[i].LogProbabilityFunction(observations[0]);

            // 2. Induction
            for (int t = 1; t < T; t++)
            {
                double[] x = observations[t];

                for (int i = 0; i < states; i++)
                {
                    double sum = Double.NegativeInfinity;
                    for (int j = 0; j < states; j++)
                        sum = Special.LogSum(sum, lnFwd[t - 1, j] + logA[j, i]);
                    lnFwd[t, i] = sum + logB[i].LogProbabilityFunction(x);
                }
            }

            System.Diagnostics.Debug.Assert(!lnFwd.HasNaN());
        }

        /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        public static double[,] LogForward<TDistribution>(HiddenMarkovModel<TDistribution> model, double[][] observations, out double logLikelihood)
            where TDistribution : IDistribution
        {
            int T = observations.Length;
            int states = model.States;

            double[,] lnFwd = new double[T, states];

            ForwardBackwardAlgorithm.LogForward<TDistribution>(model, observations, lnFwd);

            logLikelihood = Double.NegativeInfinity;
            for (int i = 0; i < states; i++)
                logLikelihood = Special.LogSum(logLikelihood, lnFwd[observations.Length - 1, i]);

            return lnFwd;
        }

        /// <summary>
        ///   Computes Backward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        /// 
        public static void LogBackward(HiddenMarkovModel model, int[] observations, double[,] lnBwd)
        {
            int states = model.States;
            var logA = model.Transitions;
            var logB = model.Emissions;
            var logPi = model.Probabilities;

            int T = observations.Length;

            // Ensures minimum requirements
            System.Diagnostics.Debug.Assert(lnBwd.GetLength(0) >= T);
            System.Diagnostics.Debug.Assert(lnBwd.GetLength(1) == states);
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
                        sum = Special.LogSum(sum, lnBwd[t + 1, j] + logA[i, j] + logB[j, observations[t + 1]]);
                    lnBwd[t, i] += sum;
                }
            }

            System.Diagnostics.Debug.Assert(!lnBwd.HasNaN());
        }

        /// <summary>
        ///   Computes Backward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        /// 
        public static double[,] LogBackward(HiddenMarkovModel model, int[] observations)
        {
            int states = model.States;

            int T = observations.Length;
            double[,] bwd = new double[T, states];
            LogBackward(model, observations, bwd);
            return bwd;
        }

        /// <summary>
        ///   Computes Backward probabilities for a given hidden Markov model and a set of observations (no scaling).
        /// </summary>
        public static double[,] LogBackward(HiddenMarkovModel model, int[] observations, out double logLikelihood)
        {
            int states = model.States;

            int T = observations.Length;
            double[,] lnBwd = new double[T, states];
            LogBackward(model, observations, lnBwd);

            logLikelihood = Double.NegativeInfinity;
            for (int i = 0; i < model.States; i++)
                logLikelihood = Special.LogSum(logLikelihood, lnBwd[0, i] +
                    model.Probabilities[i] + model.Emissions[i, observations[0]]);

            return lnBwd;
        }

        /// <summary>
        ///   Computes Backward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        /// 
        public static void LogBackward<TDistribution>(HiddenMarkovModel<TDistribution> model, double[][] observations, double[,] lnBwd)
            where TDistribution : IDistribution
        {
            int states = model.States;
            var logA = model.Transitions;
            var logB = model.Emissions;
            var logPi = model.Probabilities;

            int T = observations.Length;

            // Ensures minimum requirements
            System.Diagnostics.Debug.Assert(lnBwd.GetLength(0) >= T);
            System.Diagnostics.Debug.Assert(lnBwd.GetLength(1) == states);
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
                        sum = Special.LogSum(sum, lnBwd[t + 1, j] + logA[i, j]
                            + logB[j].LogProbabilityFunction(observations[t + 1]));
                    lnBwd[t, i] += sum;
                }
            }

            System.Diagnostics.Debug.Assert(!lnBwd.HasNaN());
        }

        /// <summary>
        ///   Computes Backward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        /// 
        public static double[,] LogBackward<TDistribution>(HiddenMarkovModel<TDistribution> model, double[][] observations)
            where TDistribution : IDistribution
        {
            int states = model.States;

            int T = observations.Length;
            double[,] lnBwd = new double[T, states];
            LogBackward(model, observations, lnBwd);
            return lnBwd;
        }

        /// <summary>
        ///   Computes Backward probabilities for a given hidden Markov model and a set of observations (no scaling).
        /// </summary>
        public static double[,] LogBackward<TDistribution>(HiddenMarkovModel<TDistribution> model, double[][] observations, out double logLikelihood)
            where TDistribution : IDistribution
        {
            int states = model.States;

            int T = observations.Length;
            double[,] lnBwd = new double[T, states];
            LogBackward(model, observations, lnBwd);

            logLikelihood = Double.NegativeInfinity;
            for (int i = 0; i < model.States; i++)
                logLikelihood = Special.LogSum(logLikelihood, lnBwd[0, i] + model.Probabilities[i]
                    + model.Emissions[i].LogProbabilityFunction(observations[0]));

            return lnBwd;
        }
    }
}
