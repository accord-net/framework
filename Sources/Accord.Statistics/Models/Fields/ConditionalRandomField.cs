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
    using Accord.Statistics.Models.Fields.Functions;

    /// <summary>
    ///   Linear-Chain Conditional Random Field (CRF).
    /// </summary>
    /// <remarks>
    ///   <para>A conditional random field (CRF) is a type of discriminative undirected
    ///   probabilistic graphical model. It is most often used for labeling or parsing
    ///   of sequential data, such as natural language text or biological sequences
    ///   and computer vision.</para>
    ///   
    ///   <para>This implementation is currently experimental.</para>
    /// </remarks>
    /// 
    [Serializable]
    public class ConditionalRandomField<T> : ICloneable
    {
        

        /// <summary>
        ///   Gets the number of states in this
        ///   linear-chain Conditional Random Field.
        /// </summary>
        /// 
        public int States { get; private set; }

        /// <summary>
        ///   Gets the potential function encompassing
        ///   all feature functions for this model.
        /// </summary>
        /// 
        public IPotentialFunction<T> Function { get; private set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="ConditionalRandomField{T}"/> class.
        /// </summary>
        /// 
        /// <param name="states">The number of states for the model.</param>
        /// <param name="function">The potential function to be used by the model.</param>
        /// 
        public ConditionalRandomField(int states, IPotentialFunction<T> function)
        {
            this.States = states;
            this.Function = function;
        }

        /// <summary>
        ///   Computes the partition function, as known as Z(x),
        ///   for the specified observations.
        /// </summary>
        /// 
        public double Partition(T[] observations)
        {
            return Math.Exp(LogPartition(observations));
        }

        /// <summary>
        ///   Computes the Log of the partition function.
        /// </summary>
        /// 
        public double LogPartition(T[] observations)
        {
            double logLikelihood;
            double[,] fwd = ForwardBackwardAlgorithm.LogForward(Function.Factors[0], observations, 0, out logLikelihood);
            return logLikelihood;
        }


        /// <summary>
        ///   Computes the log-likelihood of the model for the given observations.
        /// </summary>
        /// 
        public double LogLikelihood(T[] observations, int[] labels)
        {
            double p = Function.Factors[0].Compute(-1, labels[0], observations, 0);
            for (int t = 1; t < observations.Length; t++)
                p += Function.Factors[0].Compute(labels[t - 1], labels[t], observations, t);

            double z = LogPartition(observations);

            if (p == z)
                return 0;

            if (double.IsInfinity(p))
                return 0;

            if (double.IsInfinity(z))
                return 0;

            System.Diagnostics.Debug.Assert(!Double.IsNaN(p));
            System.Diagnostics.Debug.Assert(!Double.IsInfinity(p));

            System.Diagnostics.Debug.Assert(!Double.IsNaN(z));
            System.Diagnostics.Debug.Assert(!Double.IsInfinity(z));

            return p - z;
        }

        /// <summary>
        ///   Computes the most likely state labels for the given observations,
        ///   returning the overall sequence probability for this model.
        /// </summary>
        /// 
        public int[] Compute(T[] observations, out double logLikelihood)
        {
            return viterbi(Function.Factors[0], observations, out logLikelihood);
        }

        private int[] viterbi(FactorPotential<T> factor, T[] observations, out double logLikelihood)
        {
            // Viterbi-forward algorithm.
            int states = factor.States;
            int maxState;
            double maxWeight;
            double weight;

            int[,] s = new int[states, observations.Length];
            double[,] lnFwd = new double[states, observations.Length];


            // Base
            for (int i = 0; i < states; i++)
                lnFwd[i, 0] = Function.Factors[0].Compute(-1, i, observations, 0);

            // Induction
            for (int t = 1; t < observations.Length; t++)
            {
                T observation = observations[t];

                for (int j = 0; j < states; j++)
                {
                    maxState = 0;
                    maxWeight = lnFwd[0, t - 1] + Function.Factors[0].Compute(0, j, observations, t);

                    for (int i = 1; i < states; i++)
                    {
                        weight = lnFwd[i, t - 1] + Function.Factors[0].Compute(i, j, observations, t);

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


            // Find minimum value for time T-1
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
        ///   Computes the most likely state labels for the given observations,
        ///   returning the overall sequence log-likelihood for this model.
        /// </summary>
        /// 
        public double LogLikelihood(T[][] observations, int[][] labels)
        {
            double logLikelihood = 0;
            for (int i = 0; i < observations.Length; i++)
                logLikelihood += LogLikelihood(observations[i], labels[i]);

            System.Diagnostics.Debug.Assert(!Double.IsNaN(logLikelihood));
            System.Diagnostics.Debug.Assert(!Double.IsInfinity(logLikelihood));

            return logLikelihood;
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
        public static ConditionalRandomField<T> Load(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            return (ConditionalRandomField<T>)b.Deserialize(stream);
        }

        /// <summary>
        ///   Loads a random field from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the random field is to be deserialized.</param>
        /// 
        /// <returns>The deserialized random field.</returns>
        /// 
        public static ConditionalRandomField<T> Load(string path)
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

        public object Clone()
        {
            return new ConditionalRandomField<T>(States, (IPotentialFunction<T>)Function.Clone());
        }

        #endregion
    }

}
