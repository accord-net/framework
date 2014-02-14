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

namespace Accord.Statistics.Models.Fields
{
    using System;
    using Accord.Statistics.Models.Fields.Functions;
    using Accord.Statistics.Models.Fields.Features;
    using Accord.Math;

    /// <summary>
    ///   Forward-Backward algorithms for Conditional Random Fields.
    /// </summary>
    /// 
    public static class ForwardBackwardAlgorithm
    {


        /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        /// 
        public static void Forward<TObservation>(FactorPotential<TObservation> function,
            TObservation[] observations, int output, double[] scaling, double[,] fwd)
        {
            int states = function.States;

            int T = observations.Length;
            double s = 0;

            // Ensures minimum requirements
            System.Diagnostics.Debug.Assert(fwd.GetLength(0) >= T);
            System.Diagnostics.Debug.Assert(fwd.GetLength(1) == states);
            System.Diagnostics.Debug.Assert(scaling.Length >= T);
            Array.Clear(fwd, 0, fwd.Length);


            // 1. Initialization
            for (int i = 0; i < states; i++)
                s += fwd[0, i] = Math.Exp(function.Compute(-1, i, observations, 0, output));
            scaling[0] = s;

            if (s != 0) // Scaling
            {
                for (int i = 0; i < states; i++)
                    fwd[0, i] /= s;
            }


            // 2. Induction
            for (int t = 1; t < T; t++)
            {
                s = 0;

                for (int i = 0; i < states; i++)
                {
                    double sum = 0.0;
                    for (int j = 0; j < states; j++)
                        sum += fwd[t - 1, j] * Math.Exp(function.Compute(j, i, observations, t, output));
                    fwd[t, i] = sum;

                    s += fwd[t, i]; // scaling coefficient
                }

                scaling[t] = s;

                if (s != 0) // Scaling
                {
                    for (int i = 0; i < states; i++)
                        fwd[t, i] /= s;
                }
            }

        }

        /// <summary>
        ///   Computes Forward probabilities for a given potential function and a set of observations.
        /// </summary>
        /// 
        public static double[,] Forward<TObservation>(FactorPotential<TObservation> function,
            TObservation[] observations, int output = 0)
        {
            int states = function.States;

            int T = observations.Length;
            double[,] fwd = new double[T, states];

            // 1. Initialization
            for (int i = 0; i < states; i++)
                fwd[0, i] = Math.Exp(function.Compute(-1, i, observations, 0, output));

            // 2. Induction
            for (int t = 1; t < T; t++)
            {
                for (int i = 0; i < states; i++)
                {
                    double sum = 0.0;
                    for (int j = 0; j < states; j++)
                        sum += fwd[t - 1, j] * Math.Exp(function.Compute(j, i, observations, t, output));
                    fwd[t, i] = sum;
                }
            }

            return fwd;
        }

        /// <summary>
        ///   Computes Forward probabilities for a given potential function and a set of observations.
        /// </summary>
        /// 
        public static double[,] Forward<TObservation>(FactorPotential<TObservation> function,
            TObservation[] observations, int output, out double logLikelihood)
        {
            int states = function.States;
            double[,] fwd = new double[observations.Length, states];
            double[] coefficients = new double[observations.Length];

            ForwardBackwardAlgorithm.Forward(function, observations, output, coefficients, fwd);

            logLikelihood = 0;
            for (int i = 0; i < coefficients.Length; i++)
                logLikelihood += Math.Log(coefficients[i]);

            return fwd;
        }

        /// <summary>
        ///   Computes Forward probabilities for a given potential function and a set of observations.
        /// </summary>
        /// 
        public static double[,] Forward<TObservation>(FactorPotential<TObservation> function,
            TObservation[] observations, int output, out double[] scaling)
        {
            int states = function.States;
            double[,] fwd = new double[observations.Length, states];
            scaling = new double[observations.Length];
            Forward(function, observations, output, scaling, fwd);

            return fwd;
        }

        /// <summary>
        ///   Computes Forward probabilities for a given potential function and a set of observations.
        /// </summary>
        /// 
        public static double[,] Forward<TObservation>(FactorPotential<TObservation> function,
            TObservation[] observations, int output, out double[] scaling, out double logLikelihood)
        {
            int states = function.States;
            double[,] fwd = new double[observations.Length, states];
            scaling = new double[observations.Length];
            Forward(function, observations, output, scaling, fwd);

            logLikelihood = 0;
            for (int i = 0; i < scaling.Length; i++)
                logLikelihood += Math.Log(scaling[i]);

            return fwd;
        }



        /// <summary>
        ///   Computes Backward probabilities for a given potential function and a set of observations.
        /// </summary>
        /// 
        public static void Backward<TObservation>(FactorPotential<TObservation> function,
            TObservation[] observations, int output, double[] scaling, double[,] bwd)
        {
            int states = function.States;
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
                        sum += bwd[t + 1, j] * Math.Exp(function.Compute(i, j, observations, t + 1, output));
                    bwd[t, i] += sum / scaling[t];
                }
            }

        }

        /// <summary>
        ///   Computes Backward probabilities for a given potential function and a set of observations.
        /// </summary>
        /// 
        public static double[,] Backward<TObservation>(FactorPotential<TObservation> function,
            TObservation[] observations, int output = 0)
        {
            int states = function.States;

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
                        sum += bwd[t + 1, j] * Math.Exp(function.Compute(i, j, observations, t + 1, output));
                    bwd[t, i] += sum;
                }
            }

            return bwd;
        }

        /// <summary>
        ///   Computes Backward probabilities for a given potential function and a set of observations.
        /// </summary>
        public static double[,] Backward<TObservation>(FactorPotential<TObservation> function,
            TObservation[] observations, int output, double[] scaling)
        {
            int states = function.States;

            double[,] bwd = new double[observations.Length, states];
            Backward(function, observations, output, scaling, bwd);
            return bwd;
        }

        /// <summary>
        ///   Computes Backward probabilities for a given potential function and a set of observations(no scaling).
        /// </summary>
        public static double[,] Backward<TObservation>(FactorPotential<TObservation> function,
            TObservation[] observations, int output, out double logLikelihood)
        {
            int states = function.States;

            var bwd = Backward(function, observations, output);

            double likelihood = 0;
            for (int i = 0; i < states; i++)
                likelihood += bwd[0, i] * Math.Exp(function.Compute(-1, i, observations, 0, output));
            logLikelihood = Math.Log(likelihood);

            return bwd;
        }


        /// <summary>
        ///   Computes Forward probabilities for a given hidden Markov model and a set of observations.
        /// </summary>
        /// 
        public static void LogForward<TObservation>(FactorPotential<TObservation> function,
            TObservation[] observations, int output, double[,] lnFwd)
        {
            int states = function.States;

            int T = observations.Length;

            // Ensures minimum requirements
            System.Diagnostics.Debug.Assert(lnFwd.GetLength(0) >= T);
            System.Diagnostics.Debug.Assert(lnFwd.GetLength(1) == states);
            Array.Clear(lnFwd, 0, lnFwd.Length);


            // 1. Initialization
            for (int i = 0; i < states; i++)
                lnFwd[0, i] = function.Compute(-1, i, observations, 0, output);

            // 2. Induction
            for (int t = 1; t < T; t++)
            {
                for (int i = 0; i < states; i++)
                {
                    double sum = Double.NegativeInfinity;
                    for (int j = 0; j < states; j++)
                        sum = Special.LogSum(sum, lnFwd[t - 1, j] + function.Compute(j, i, observations, t, output));
                    lnFwd[t, i] = sum;
                }
            }
        }

        /// <summary>
        ///   Computes Forward probabilities for a given potential function and a set of observations.
        /// </summary>
        /// 
        public static double[,] LogForward<TObservation>(FactorPotential<TObservation> function,
            TObservation[] observations, int output)
        {
            int states = function.States;
            double[,] lnFwd = new double[observations.Length, states];
            ForwardBackwardAlgorithm.LogForward(function, observations, output, lnFwd);
            return lnFwd;
        }

        /// <summary>
        ///   Computes Forward probabilities for a given potential function and a set of observations.
        /// </summary>
        /// 
        public static double[,] LogForward<TObservation>(FactorPotential<TObservation> function,
            TObservation[] observations, int output, out double logLikelihood)
        {
            int states = function.States;
            double[,] lnFwd = new double[observations.Length, states];
            int T = observations.Length;

            ForwardBackwardAlgorithm.LogForward(function, observations, output, lnFwd);

            logLikelihood = Double.NegativeInfinity;
            for (int j = 0; j < states; j++)
                logLikelihood = Special.LogSum(logLikelihood, lnFwd[T - 1, j]);

            System.Diagnostics.Debug.Assert(!Double.IsNaN(logLikelihood));

            return lnFwd;
        }



        /// <summary>
        ///   Computes Backward probabilities for a given potential function and a set of observations.
        /// </summary>
        /// 
        public static void LogBackward<TObservation>(FactorPotential<TObservation> function,
            TObservation[] observations, int output, double[,] lnBwd)
        {
            int states = function.States;
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
                        sum = Special.LogSum(sum, lnBwd[t + 1, j] + function.Compute(i, j, observations, t + 1, output));
                    lnBwd[t, i] += sum;
                }
            }

        }


        /// <summary>
        ///   Computes Backward probabilities for a given potential function and a set of observations.
        /// </summary>
        public static double[,] LogBackward<TObservation>(FactorPotential<TObservation> function,
            TObservation[] observations, int output)
        {
            int states = function.States;

            double[,] lnBwd = new double[observations.Length, states];
            LogBackward(function, observations, output, lnBwd);
            return lnBwd;
        }

        /// <summary>
        ///   Computes Backward probabilities for a given potential function and a set of observations(no scaling).
        /// </summary>
        public static double[,] LogBackward<TObservation>(FactorPotential<TObservation> function,
            TObservation[] observations, int output, out double logLikelihood)
        {
            int states = function.States;

            var lnBwd = LogBackward(function, observations, output);

            logLikelihood = double.NegativeInfinity;
            for (int i = 0; i < states; i++)
                logLikelihood = Special.LogSum(logLikelihood, lnBwd[0, i] + function.Compute(-1, i, observations, 0, output));

            return lnBwd;
        }


    }
}

