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
    /// <para>
    ///   In this example, we will create a sequence classifier using a hidden Markov
    ///   classifier. Afterwards, we will transform this Markov classifier into an
    ///   equivalent Hidden Conditional Random Field by choosing a suitable feature
    ///   function.</para>
    ///   
    /// <code>
    /// // Let's say we would like to do a very simple mechanism for
    /// // gesture recognition. In this example, we will be trying to
    /// // create a classifier that can distinguish between the words
    /// // "hello", "car", and "wardrobe". 
    ///             
    /// // Let's say we decided to acquire some data, and we asked some
    /// // people to perform those words in front of a Kinect camera, and,
    /// // using Microsoft's SDK, we were able to captured the x and y
    /// // coordinates of each hand while the word was being performed.
    /// 
    /// // Let's say we decided to represent our frames as:
    /// // 
    /// //    double[] frame = { leftHandX, leftHandY, rightHandX, rightHandY };
    /// //
    /// // Since we captured words, this means we captured sequences of
    /// // frames as we described above. Let's write some of those as 
    /// // rough examples to explain how gesture recognition can be done:
    /// 
    /// double[][] hello =
    /// {
    ///     new double[] { 1.0, 0.1, 0.0, 0.0 }, // let's say the word
    ///     new double[] { 0.0, 1.0, 0.1, 0.1 }, // hello took 6 frames
    ///     new double[] { 0.0, 1.0, 0.1, 0.1 }, // to be recorded.
    ///     new double[] { 0.0, 0.0, 1.0, 0.0 },
    ///     new double[] { 0.0, 0.0, 1.0, 0.0 },
    ///     new double[] { 0.0, 0.0, 0.1, 1.1 },
    /// };
    /// 
    /// double[][] car =
    /// {
    ///     new double[] { 0.0, 0.0, 0.0, 1.0 }, // the car word
    ///     new double[] { 0.1, 0.0, 1.0, 0.1 }, // took only 4.
    ///     new double[] { 0.0, 0.0, 0.1, 0.0 },
    ///     new double[] { 1.0, 0.0, 0.0, 0.0 },
    /// };
    /// 
    /// double[][] wardrobe =
    /// {
    ///     new double[] { 0.0, 0.0, 1.0, 0.0 }, // same for the
    ///     new double[] { 0.1, 0.0, 1.0, 0.1 }, // wardrobe word.
    ///     new double[] { 0.0, 0.1, 1.0, 0.0 },
    ///     new double[] { 0.1, 0.0, 1.0, 0.1 },
    /// };
    /// 
    /// // Here, please note that a real-world example would involve *lots*
    /// // of samples for each word. Here, we are considering just one from
    /// // each class which is clearly sub-optimal and should _never_ be done
    /// // on practice. For example purposes, however, please disregard this.
    /// 
    /// // Those are the words we have in our vocabulary:
    /// //
    /// double[][][] words = { hello, car, wardrobe }; 
    /// 
    /// // Now, let's associate integer labels with them. This is needed
    /// // for the case where there are multiple samples for each word.
    /// //
    /// int[] labels = { 0, 1, 2 };
    /// 
    /// 
    /// // We will create our classifiers assuming an independent
    /// // Gaussian distribution for each component in our feature
    /// // vectors (like assuming a Naive Bayes assumption).
    /// 
    /// var initial = new Independent&lt;NormalDistribution>
    /// (
    ///     new NormalDistribution(0, 1), 
    ///     new NormalDistribution(0, 1), 
    ///     new NormalDistribution(0, 1), 
    ///     new NormalDistribution(0, 1)  
    /// );
    /// 
    /// 
    /// // Now, we can proceed and create our classifier. 
    /// //
    /// int numberOfWords = 3;  // we are trying to distinguish between 3 words
    /// int numberOfStates = 5; // this value can be found by trial-and-error
    /// 
    /// var hmm = new HiddenMarkovClassifier&lt;Independent&lt;NormalDistribution>>
    /// (
    ///     classes: numberOfWords, 
    ///     topology: new Forward(numberOfStates), // word classifiers should use a forward topology
    ///     initial: initial
    /// );
    /// 
    /// // Create a new learning algorithm to train the sequence classifier
    /// var teacher = new HiddenMarkovClassifierLearning&lt;Independent&lt;NormalDistribution>>(hmm,
    /// 
    ///     // Train each model until the log-likelihood changes less than 0.001
    ///     modelIndex => new BaumWelchLearning&lt;Independent&lt;NormalDistribution>>(hmm.Models[modelIndex])
    ///     {
    ///         Tolerance = 0.001,
    ///         Iterations = 100,
    /// 
    ///         // This is necessary so the code doesn't blow up when it realize
    ///         // there is only one sample per word class. But this could also be
    ///         // needed in normal situations as well.
    ///         //
    ///         FittingOptions = new IndependentOptions()
    ///         {
    ///             InnerOption = new NormalOptions() { Regularization = 1e-5 }
    ///         }
    ///     }
    /// );
    /// 
    /// // Finally, we can run the learning algorithm!
    /// double logLikelihood = teacher.Run(words, labels);
    /// 
    /// // At this point, the classifier should be successfully 
    /// // able to distinguish between our three word classes:
    /// //
    /// int tc1 = hmm.Compute(hello);    // should be 0
    /// int tc2 = hmm.Compute(car);      // should be 1
    /// int tc3 = hmm.Compute(wardrobe); // should be 2
    /// 
    /// 
    /// // Now, we can use the Markov classifier to initialize a HCRF
    /// var function = new MarkovMultivariateFunction(hmm);
    /// var hcrf = new HiddenConditionalRandomField&lt;double[]>(function);
    /// 
    /// // We can check that both are equivalent, although they have
    /// // formulations that can be learned with different methods
    /// //
    /// for (int i = 0; i &lt; words.Length; i++)
    /// {
    ///     // Should be the same
    ///     int expected = hmm.Compute(words[i]);
    ///     int actual = hcrf.Compute(words[i]);
    /// 
    ///     // Should be the same
    ///     double h0 = hmm.LogLikelihood(words[i], 0);
    ///     double c0 = hcrf.LogLikelihood(words[i], 0);
    /// 
    ///     double h1 = hmm.LogLikelihood(words[i], 1);
    ///     double c1 = hcrf.LogLikelihood(words[i], 1);
    /// 
    ///     double h2 = hmm.LogLikelihood(words[i], 2);
    ///     double c2 = hcrf.LogLikelihood(words[i], 2);
    /// }
    /// 
    ///
    /// // Now we can learn the HCRF using one of the best learning
    /// // algorithms available, Resilient Backpropagation learning:
    /// 
    /// // Create a learning algorithm
    /// var rprop = new HiddenResilientGradientLearning&lt;double[]>(hcrf)
    /// {
    ///     Iterations = 50,
    ///     Tolerance = 1e-5
    /// };
    ///
    /// // Run the algorithm and learn the models
    /// double error = rprop.Run(words, labels);
    ///
    /// // At this point, the HCRF should be successfully 
    /// // able to distinguish between our three word classes:
    /// //
    /// int hc1 = hcrf.Compute(hello);    // Should be 0
    /// int hc2 = hcrf.Compute(car);      // Should be 1
    /// int hc3 = hcrf.Compute(wardrobe); // Should be 2
    /// </code>
    /// 
    /// <para>
    ///   In order to see how this HCRF can be trained to the data, please take a look 
    ///   at the <see cref="HiddenResilientGradientLearning{T}"/> page. Resilient Propagation
    ///   is one of the best algorithms for HCRF training.</para>
    /// </example>
    /// 
    /// <typeparam name="T">The type of the observations modeled by the field.</typeparam>
    /// 
    /// <see cref="HiddenResilientGradientLearning{T}"/>
    /// 
    [Serializable]
    public class HiddenConditionalRandomField<T> : MulticlassClassifierBase<T[]>,
        ICloneable
    {

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
            this.NumberOfOutputs = function.Outputs;
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
            logLikelihoods = computeLogLikelihood(observations);

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
            logLikelihoods = computeLogLikelihood(observations);

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


        private double[] computeLogLikelihood(T[] observations)
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
