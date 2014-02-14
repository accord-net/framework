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
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading.Tasks;
    using Accord.Math;
    using Accord.Statistics.Models.Fields.Functions;

    /// <summary>
    ///   Hidden Conditional Random Field (HCRF).
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the observations modeled by the field.</typeparam>
    /// 
    [Serializable]
    public class HiddenConditionalRandomField<T> : ICloneable
    {

        /// <summary>
        ///   Gets the number of outputs assumed by the model.
        /// </summary>
        /// 
        public int Outputs { get { return Function.Outputs; } }

        /// <summary>
        ///   Gets the potential function encompassing
        ///   all feature functions for this model.
        /// </summary>
        /// 
        public IPotentialFunction<T> Function { get; private set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="HiddenConditionalRandomField{T}"/> class.
        /// </summary>
        /// 
        /// <param name="function">The potential function to be used by the model.</param>
        /// 
        public HiddenConditionalRandomField(IPotentialFunction<T> function)
        {
            this.Function = function;
        }

        /// <summary>
        ///   Computes the most likely output for the given observations.
        /// </summary>
        /// 
        public int Compute(T[] observations)
        {
            double[] logLikelihoods;
            return Compute(observations, out logLikelihoods);
        }

        /// <summary>
        ///   Computes the most likely output for the given observations.
        /// </summary>
        /// 
        public int Compute(T[] observations, out double[] logLikelihoods)
        {
            // Compute log-likelihoods for all possible outputs
            logLikelihoods = computeLogLikelihood(observations);

            // The logLikelihoods array now stores the unormalized
            // log-likelihoods for each of the possible outputs. We
            // should now normalize them.

            double sum = 0;

            // Compute the marginal log-likelihood
            for (int i = 0; i < logLikelihoods.Length; i++)
                sum = Special.LogSum(sum, logLikelihoods[i]);

            // Normalize all log-likelihoods
            for (int i = 0; i < logLikelihoods.Length; i++)
                logLikelihoods[i] -= sum;

            // Choose the class with maximum likelihood
            int imax; logLikelihoods.Max(out imax);

            return imax;
        }

        /// <summary>
        ///   Computes the most likely output for the given observations.
        /// </summary>
        /// 
        public int Compute(T[] observations, out double logLikelihood)
        {
            double[] logLikelihoods;
            int i = Compute(observations, out logLikelihoods);
            logLikelihood = logLikelihoods[i];
            return i;
        }

        /// <summary>
        ///   Computes the most likely state labels for the given observations,
        ///   returning the overall sequence probability for this model.
        /// </summary>
        /// 
        public int[] Decode(T[] observations, int output)
        {
            double logLikelihood;
            return Decode(observations, output, out logLikelihood);
        }

        /// <summary>
        ///   Computes the most likely state labels for the given observations,
        ///   returning the overall sequence probability for this model.
        /// </summary>
        /// 
        public int[] Decode(T[] observations, int output, out double logLikelihood)
        {
            // Viterbi-forward algorithm.
            var factor = Function.Factors[output];
            int states = factor.States;
            int maxState;
            double maxWeight;
            double weight;

            int[,] s = new int[states, observations.Length];
            double[,] lnFwd = new double[states, observations.Length];


            // Base
            for (int i = 0; i < states; i++)
                lnFwd[i, 0] = factor.Compute(-1, i, observations, 0, output);

            // Induction
            for (int t = 1; t < observations.Length; t++)
            {
                T observation = observations[t];

                for (int j = 0; j < states; j++)
                {
                    maxState = 0;
                    maxWeight = lnFwd[0, t - 1] + factor.Compute(0, j, observations, t, output);

                    for (int i = 1; i < states; i++)
                    {
                        weight = lnFwd[i, t - 1] + factor.Compute(i, j, observations, t, output);

                        if (weight > maxWeight)
                        {
                            maxState = i;
                            maxWeight = weight;
                        }
                    }

                    lnFwd[j, t] = maxWeight;
                    s[j, t] = maxState;
                }
            }


            // Find maximum value for time T-1
            maxState = 0;
            maxWeight = lnFwd[0, observations.Length - 1];

            for (int i = 1; i < states; i++)
            {
                if (lnFwd[i, observations.Length - 1] > maxWeight)
                {
                    maxState = i;
                    maxWeight = lnFwd[i, observations.Length - 1];
                }
            }


            // Trackback
            int[] path = new int[observations.Length];
            path[path.Length - 1] = maxState;

            for (int t = path.Length - 2; t >= 0; t--)
                path[t] = s[path[t + 1], t + 1];


            // Returns the sequence probability as an out parameter
            logLikelihood = maxWeight;

            // Returns the most likely (Viterbi path) for the given sequence
            return path;
        }

        /// <summary>
        ///   Computes the log-likelihood that the given 
        ///   observations belong to the desired output.
        /// </summary>
        /// 
        public double LogLikelihood(T[] observations, int output)
        {
            double[] logLikelihoods;
            return LogLikelihood(observations, output, out logLikelihoods);
        }

        /// <summary>
        ///   Computes the log-likelihood that the given 
        ///   observations belong to the desired output.
        /// </summary>
        /// 
        public double LogLikelihood(T[] observations, int output, out double[] logLikelihoods)
        {
            // Compute the marginal likelihood as Z(y,x)
            //                                    ------
            //                                     Z(x)

            // Compute log-likelihoods for all possible outputs
            logLikelihoods = computeLogLikelihood(observations);

            // The logLikelihoods array now stores the unormalized
            // log-likelihoods for each of the possible outputs. We
            // should now normalize the one we are interested.

            // Compute the marginal likelihood
            double logZx = Double.NegativeInfinity;
            for (int i = 0; i < logLikelihoods.Length; i++)
                logZx = Special.LogSum(logZx, logLikelihoods[i]);

            double logZxy = logLikelihoods[output];

            System.Diagnostics.Debug.Assert(!Double.IsNaN(logZx));
            System.Diagnostics.Debug.Assert(!Double.IsNaN(logZxy));
            System.Diagnostics.Debug.Assert(!Double.IsNaN(logLikelihoods[output]));
           // System.Diagnostics.Debug.Assert(!Double.IsInfinity(logZx));

            // Return the marginal
            if (logZx == logZxy)
                return 0;
            return logZxy - logZx;
        }

        /// <summary>
        ///   Computes the log-likelihood that the given 
        ///   observations belong to the desired outputs.
        /// </summary>
        /// 
        public double LogLikelihood(T[][] observations, int[] output)
        {
            double sum = 0;

            double[] logLikelihoods;
            for (int i = 0; i < observations.Length; i++)
                sum += LogLikelihood(observations[i], output[i], out logLikelihoods);

            return sum;
        }

        /// <summary>
        ///   Computes the log-likelihood that the given 
        ///   observations belong to the desired outputs.
        /// </summary>
        /// 
        public double LogLikelihood(T[][] observations, int[] output, out double[][] logLikelihoods)
        {
            double sum = 0;

            logLikelihoods = new double[observations.Length][];
            for (int i = 0; i < observations.Length; i++)
                sum += LogLikelihood(observations[i], output[i], out logLikelihoods[i]);

            return sum;
        }

        /// <summary>
        ///   Computes the partition function Z(x,y).
        /// </summary>
        /// 
        public double Partition(T[] x, int y)
        {
            double logLikelihood;

            ForwardBackwardAlgorithm.Forward(Function.Factors[y], x, y, out logLikelihood);

            double z = Math.Exp(logLikelihood);

            System.Diagnostics.Debug.Assert(!Double.IsNaN(z));

            return z;
        }

        /// <summary>
        ///   Computes the log-partition function ln Z(x,y).
        /// </summary>
        /// 
        public double LogPartition(T[] x, int y)
        {
            double logLikelihood;

            ForwardBackwardAlgorithm.Forward(Function.Factors[y], x, y, out logLikelihood);

            double z = logLikelihood;

            System.Diagnostics.Debug.Assert(!Double.IsNaN(z));

            return z;
        }

        /// <summary>
        ///   Computes the partition function Z(x).
        /// </summary>
        /// 
        public double Partition(T[] x)
        {
            double sum = 0;

            for (int j = 0; j < Outputs; j++)
            {
                double logLikelihood;

                ForwardBackwardAlgorithm.Forward(Function.Factors[j], x, j, out logLikelihood);

                sum += Math.Exp(logLikelihood);
            }

            System.Diagnostics.Debug.Assert(!Double.IsNaN(sum));

            return sum;
        }

        /// <summary>
        ///   Computes the log-partition function ln Z(x).
        /// </summary>
        /// 
        public double LogPartition(T[] x)
        {
            double sum = 0;

            for (int j = 0; j < Outputs; j++)
            {
                double logLikelihood;

                ForwardBackwardAlgorithm.Forward(Function.Factors[j], x, j, out logLikelihood);

                sum += Math.Exp(logLikelihood);
            }

            System.Diagnostics.Debug.Assert(!Double.IsNaN(sum));

            return Math.Log(sum);
        }


        private double[] computeLogLikelihood(T[] observations)
        {
            double[] logLikelihoods = new double[Outputs];


#if SERIAL  // For all possible outputs for the model,

            for (int y = 0; y < logLikelihoods.Length; y++)
#else
            Parallel.For(0, logLikelihoods.Length, y =>
#endif
            {
                double logLikelihood;

                // Compute the factor log-likelihood for the output
                ForwardBackwardAlgorithm.LogForward(Function.Factors[y],
                    observations, y, out logLikelihood);

                // Accumulate output's likelihood
                logLikelihoods[y] = logLikelihood;

                System.Diagnostics.Debug.Assert(!Double.IsNaN(logLikelihood));
            }
#if !SERIAL
);
#endif

            return logLikelihoods;
        }


        /// <summary>
        ///   Saves the random field to a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to which the random field is to be serialized.</param>
        /// 
        public void Save(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(stream, this);
        }

        /// <summary>
        ///   Saves the random field to a stream.
        /// </summary>
        /// 
        /// <param name="path">The stream to which the random field is to be serialized.</param>
        /// 
        public void Save(string path)
        {
            Save(new FileStream(path, FileMode.Create));
        }

        /// <summary>
        ///   Loads a random field from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from which the random field is to be deserialized.</param>
        /// 
        /// <returns>The deserialized random field.</returns>
        /// 
        public static HiddenConditionalRandomField<T> Load(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            return (HiddenConditionalRandomField<T>)b.Deserialize(stream);
        }

        /// <summary>
        ///   Loads a random field from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the random field is to be deserialized.</param>
        /// 
        /// <returns>The deserialized random field.</returns>
        /// 
        public static HiddenConditionalRandomField<T> Load(string path)
        {
            return Load(new FileStream(path, FileMode.Open));
        }

        #region ICloneable Members

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public object Clone()
        {
            return new HiddenConditionalRandomField<T>((IPotentialFunction<T>)Function.Clone());
        }

        #endregion
    }
}
