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

namespace Accord.Statistics.Models.Fields
{
    using Accord.MachineLearning;
    using Accord.Math;
    using Accord.Statistics.Models.Fields.Functions;
    using Accord.Statistics.Models.Fields.Learning;
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Accord.Compat;
    using System.Threading.Tasks;

    /// <summary>
    ///   Hidden Conditional Random Field (HCRF).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Conditional random fields (CRFs) are a class of statistical modeling method often applied 
    ///   in pattern recognition and machine learning, where they are used for structured prediction. 
    ///   Whereas an ordinary classifier predicts a label for a single sample without regard to "neighboring"
    ///   samples, a CRF can take context into account; e.g., the linear chain CRF popular in natural 
    ///   language processing predicts sequences of labels for sequences of input samples.</para>
    ///   
    /// <para>
    ///   While Conditional Random Fields can be seen as a generalization of Markov models, Hidden
    ///   Conditional Random Fields can be seen as a generalization of Hidden Markov Model Classifiers.
    ///   The (linear-chain) Conditional Random Field is the discriminative counterpart of the Markov model.
    ///   An observable Markov Model assumes the sequences of states y to be visible, rather than hidden.
    ///   Thus they can be used in a different set of problems than the hidden Markov models. Those models
    ///   are often used for sequence component labeling, also known as part-of-sequence tagging. After a model
    ///   has been trained, they are mostly used to tag parts of a sequence using the Viterbi algorithm.
    ///   This is very handy to perform, for example, classification of parts of a speech utterance, such as 
    ///   classifying phonemes inside an audio signal.  </para>
    ///   
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://www.codeproject.com/Articles/559535/Sequence-Classifiers-in-Csharp-Part-II-Hidden-Cond">
    ///       C. Souza, Sequence Classifiers in C# - Part II: Hidden Conditional Random Fields. CodeProject. Available at:
    ///       http://www.codeproject.com/Articles/559535/Sequence-Classifiers-in-Csharp-Part-II-Hidden-Cond </a></description></item>
    ///     <item><description>
    ///       Chan, Tony F.; Golub, Gene H.; LeVeque, Randall J. (1983). Algorithms for 
    ///       Computing the Sample Variance: Analysis and Recommendations. The American
    ///       Statistician 37, 242-247.</description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Fields\HiddenConditionalRandomFieldTest.cs" region="doc_learn_1" />
    ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Fields\HiddenConditionalRandomFieldTest.cs" region="doc_learn_2" />
    ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Fields\HiddenConditionalRandomFieldTest.cs" region="doc_learn_3" />
    ///   
    ///   <para>
    ///   The next example shows how to use the learning algorithms in a real-world dataset,
    ///   including training and testing in separate sets and evaluating its performance:</para>
    ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Fields\Learning\ResilientGradientHiddenLearningTest.cs" region="doc_learn_pendigits" />
    /// </example>
    /// 
    /// <typeparam name="T">The type of the observations modeled by the field.</typeparam>
    /// 
    /// <see cref="HiddenResilientGradientLearning{T}"/>
    /// 
    [Serializable]
    public class HiddenConditionalRandomField<T> : MulticlassClassifierBase<T[]>, ICloneable
    {
        // TODO: This class should inherit from MulticlassLikelihoodClassifierBase instead

        private IPotentialFunction<T> function;

        /// <summary>
        ///   Gets the number of outputs assumed by the model.
        /// </summary>
        /// 
        [Obsolete("Please use NumberOfOutputs instead.")]
        public int Outputs { get { return Function.Outputs; } }

        /// <summary>
        ///   Gets the potential function encompassing
        ///   all feature functions for this model.
        /// </summary>
        /// 
        public IPotentialFunction<T> Function
        {
            get { return function; }
            set
            {
                this.function = value;
                this.NumberOfOutputs = function.Outputs;
                this.NumberOfClasses = function.Outputs;
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="HiddenConditionalRandomField{T}"/> class.
        /// </summary>
        /// 
        public HiddenConditionalRandomField()
        {
        }

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
        [Obsolete("Please use the Decide() method instead.")]
        public int Compute(T[] observations)
        {
            double[] logLikelihoods;
            return compute(observations, out logLikelihoods);
        }

        /// <summary>
        ///   Computes the most likely output for the given observations.
        /// </summary>
        /// 
        [Obsolete("Please use the Decide() method instead.")]
        public int Compute(T[] observations, out double[] logLikelihoods)
        {
            return compute(observations, out logLikelihoods);
        }

        private int compute(T[] observations, out double[] logLikelihoods)
        {
            // Compute log-likelihoods for all possible outputs
            logLikelihoods = LogLikelihood(observations);

            // The logLikelihoods array now stores the unnormalized
            // log-likelihoods for each of the possible outputs. We
            // should now normalize them.

            double sum = Double.NegativeInfinity;

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
        [Obsolete("Please use the Decide() method instead.")]
        public int Compute(T[] observations, out double logLikelihood)
        {
            double[] logLikelihoods;
            int i = compute(observations, out logLikelihoods);
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
            logLikelihoods = LogLikelihood(observations);

            // The logLikelihoods array now stores the unormalized
            // log-likelihoods for each of the possible outputs. We
            // should now normalize the one we are interested.

            // Compute the marginal likelihood
            double logZx = Double.NegativeInfinity;
            for (int i = 0; i < logLikelihoods.Length; i++)
                logZx = Special.LogSum(logZx, logLikelihoods[i]);

            double logZxy = logLikelihoods[output];

            Accord.Diagnostics.Debug.Assert(!Double.IsNaN(logZx));
            Accord.Diagnostics.Debug.Assert(!Double.IsNaN(logZxy));
            Accord.Diagnostics.Debug.Assert(!Double.IsNaN(logLikelihoods[output]));
            // Accord.Diagnostics.Debug.Assert(!Double.IsInfinity(logZx));

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

            Accord.Diagnostics.Debug.Assert(!Double.IsNaN(z));

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

            Accord.Diagnostics.Debug.Assert(!Double.IsNaN(z));

            return z;
        }

        /// <summary>
        ///   Computes the partition function Z(x).
        /// </summary>
        /// 
        public double Partition(T[] x)
        {
            double sum = 0;

            for (int j = 0; j < NumberOfOutputs; j++)
            {
                double logLikelihood;

                ForwardBackwardAlgorithm.Forward(Function.Factors[j], x, j, out logLikelihood);

                sum += Math.Exp(logLikelihood);
            }

            Accord.Diagnostics.Debug.Assert(!Double.IsNaN(sum));

            return sum;
        }

        /// <summary>
        ///   Computes the log-partition function ln Z(x).
        /// </summary>
        /// 
        public double LogPartition(T[] x)
        {
            double sum = 0;

            for (int j = 0; j < NumberOfOutputs; j++)
            {
                double logLikelihood;

                ForwardBackwardAlgorithm.Forward(Function.Factors[j], x, j, out logLikelihood);

                sum += Math.Exp(logLikelihood);
            }

            Accord.Diagnostics.Debug.Assert(!Double.IsNaN(sum));

            return Math.Log(sum);
        }

        /// <summary>
        ///   Computes the log-likelihood of the observations given this model.
        /// </summary>
        /// 
        public double[][] LogLikelihood(T[][] observations)
        {
            var result = new double[observations.Length][];
            for (int i = 0; i < result.Length; i++)
                result[i] = LogLikelihood(observations[i]);
            return result;
        }

        /// <summary>
        ///   Computes the log-likelihood of the observations given this model.
        /// </summary>
        /// 
        public double[] LogLikelihood(T[] observations)
        {
            double[] logLikelihoods = new double[NumberOfOutputs];


#if SERIAL || DEBUG  // For all possible outputs for the model,

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

                Accord.Diagnostics.Debug.Assert(!Double.IsNaN(logLikelihood));
            }
#if !(SERIAL || DEBUG)
);
#endif

            return logLikelihoods;
        }

#if !NETSTANDARD1_4
        /// <summary>
        ///   Saves the random field to a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to which the random field is to be serialized.</param>
        /// 
        [Obsolete("Please use Accord.IO.Serializer.Save() instead.")]
        public void Save(Stream stream)
        {
            Accord.IO.Serializer.Save(this, stream);
        }

        /// <summary>
        ///   Saves the random field to a stream.
        /// </summary>
        /// 
        /// <param name="path">The stream to which the random field is to be serialized.</param>
        /// 
        [Obsolete("Please use Accord.IO.Serializer.Save() instead.")]
        public void Save(string path)
        {
            Accord.IO.Serializer.Save(this, path);
        }

        /// <summary>
        ///   Loads a random field from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from which the random field is to be deserialized.</param>
        /// 
        /// <returns>The deserialized random field.</returns>
        /// 
        [Obsolete("Please use Accord.IO.Serializer.Load<HiddenConditionalRandomField<T>>() instead.")]
        public static HiddenConditionalRandomField<T> Load(Stream stream)
        {
            return Accord.IO.Serializer.Load<HiddenConditionalRandomField<T>>(stream);
        }

        /// <summary>
        ///   Loads a random field from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the random field is to be deserialized.</param>
        /// 
        /// <returns>The deserialized random field.</returns>
        /// 
        [Obsolete("Please use Accord.IO.Serializer.Load<HiddenConditionalRandomField<T>>() instead.")]
        public static HiddenConditionalRandomField<T> Load(string path)
        {
            return Accord.IO.Serializer.Load<HiddenConditionalRandomField<T>>(path);
        }
#endif

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

        /// <summary>
        /// Computes a class-label decision for a given <paramref name="input" />.
        /// </summary>
        /// <param name="input">The input vector that should be classified into
        /// one of the <see cref="ITransform.NumberOfOutputs" /> possible classes.</param>
        /// <returns>
        /// A class-label that best described <paramref name="input" /> according
        /// to this classifier.
        /// </returns>
        public override int Decide(T[] input)
        {
            double[] v;
            return compute(input, out v);
        }
    }
}
