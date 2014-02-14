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
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Models.Markov.Topology;
    using Accord.Statistics.Models.Markov.Learning;
    using System.Collections.Generic;

    /// <summary>
    ///   Arbitrary-density Hidden Markov Model Set for Sequence Classification.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class uses a set of <see cref="HiddenMarkovModel{TDistribution}">density hidden
    ///   Markov models</see> to classify sequences of real (double-precision floating point)
    ///   numbers or arrays of those numbers. Each model will try to learn and recognize each 
    ///   of the different output classes. For examples and details on how to learn such models,
    ///   please take a look on the documentation for 
    ///   <see cref="HiddenMarkovClassifierLearning{TDistribution}"/>.</para>
    ///   
    /// <para>
    ///   For the discrete version of this classifier, please see its non-generic counterpart 
    ///   <see cref="HiddenMarkovClassifier"/>.</para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   Examples are available at the respective learning algorithm pages. For 
    ///   example, see <see cref="HiddenMarkovClassifierLearning{TDistribution}"/>. </para>
    /// </example>
    /// 
    /// <seealso cref="HiddenMarkovClassifierLearning{TDistribution}"/>
    /// <seealso cref="HiddenMarkovClassifier"/>
    ///   
    [Serializable]
    public class HiddenMarkovClassifier<TDistribution> : BaseHiddenMarkovClassifier<HiddenMarkovModel<TDistribution>>,
        IEnumerable<HiddenMarkovModel<TDistribution>>, IHiddenMarkovClassifier where TDistribution : IDistribution
    {

        /// <summary>
        ///   Gets the number of dimensions of the 
        ///   observations handled by this classifier.
        /// </summary>
        /// 
        public int Dimension
        {
            get { return Models[0].Dimension; }
        }

        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        /// 
        /// <param name="classes">The number of classes in the classifier.</param>
        /// <param name="states">An array specifying the number of hidden states for each
        /// of the classifiers. By default, and Ergodic topology will be used.</param>
        /// <param name="initial">The initial probability distributions for the hidden states.
        /// For multivariate continuous density distributions, such as Normal mixtures, the
        /// choice of initial values is crucial for a good performance.</param>
        /// 
        public HiddenMarkovClassifier(int classes, int[] states, TDistribution initial)
            : base(classes)
        {
            for (int i = 0; i < classes; i++)
                Models[i] = new HiddenMarkovModel<TDistribution>(states[i], initial);
        }

        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        /// 
        /// <param name="classes">The number of classes in the classifier.</param>
        /// <param name="topology">The topology of the hidden states. A forward-only topology
        /// is indicated to sequence classification problems, such as speech recognition.</param>
        /// <param name="initial">The initial probability distributions for the hidden states.
        /// For multivariate continuous density distributions, such as Normal mixtures, the
        /// choice of initial values is crucial for a good performance.</param>
        /// 
        public HiddenMarkovClassifier(int classes, ITopology topology, TDistribution initial)
            : base(classes)
        {
            for (int i = 0; i < classes; i++)
                Models[i] = new HiddenMarkovModel<TDistribution>(topology, initial);
        }

        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        /// 
        /// <param name="classes">The number of classes in the classifier.</param>
        /// <param name="topology">The topology of the hidden states. A forward-only topology
        /// is indicated to sequence classification problems, such as speech recognition.</param>
        /// <param name="initial">The initial probability distributions for the hidden states.
        /// For multivariate continuous density distributions, such as Normal mixtures, the
        /// choice of initial values is crucial for a good performance.</param>
        /// 
        public HiddenMarkovClassifier(int classes, ITopology topology, TDistribution[] initial)
            : base(classes)
        {
            for (int i = 0; i < classes; i++)
                Models[i] = new HiddenMarkovModel<TDistribution>(topology, initial[i]);
        }


        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        /// 
        /// <param name="classes">The number of classes in the classifier.</param>
        /// <param name="topology">The topology of the hidden states. A forward-only topology
        /// is indicated to sequence classification problems, such as speech recognition.</param>
        /// <param name="initial">The initial probability distributions for the hidden states.
        /// For multivariate continuous density distributions, such as Normal mixtures, the
        /// choice of initial values is crucial for a good performance.</param>
        /// 
        public HiddenMarkovClassifier(int classes, ITopology[] topology, TDistribution[] initial)
            : base(classes)
        {
            for (int i = 0; i < classes; i++)
                Models[i] = new HiddenMarkovModel<TDistribution>(topology[i], initial[i]);
        }

        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        /// 
        /// <param name="classes">The number of classes in the classifier.</param>
        /// <param name="topology">The topology of the hidden states. A forward-only topology
        /// is indicated to sequence classification problems, such as speech recognition.</param>
        /// <param name="initial">The initial probability distributions for the hidden states.
        /// For multivariate continuous density distributions, such as Normal mixtures, the
        /// choice of initial values is crucial for a good performance.</param>
        /// 
        public HiddenMarkovClassifier(int classes, ITopology[] topology, TDistribution initial)
            : base(classes)
        {
            for (int i = 0; i < classes; i++)
                Models[i] = new HiddenMarkovModel<TDistribution>(topology[i], initial);
        }

        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        /// 
        /// <param name="classes">The number of classes in the classifier.</param>
        /// <param name="topology">The topology of the hidden states. A forward-only topology
        /// is indicated to sequence classification problems, such as speech recognition.</param>
        /// <param name="initial">The initial probability distributions for the hidden states.
        /// For multivariate continuous density distributions, such as Normal mixtures, the
        /// choice of initial values is crucial for a good performance.</param>
        /// <param name="names">The class labels for each of the models.</param>
        /// 
        public HiddenMarkovClassifier(int classes, ITopology[] topology, TDistribution initial, string[] names)
            : base(classes)
        {
            for (int i = 0; i < classes; i++)
                Models[i] = new HiddenMarkovModel<TDistribution>(topology[i], initial) { Tag = names[i] };
        }

        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        /// 
        /// <param name="models">
        ///   The models specializing in each of the classes of 
        ///   the classification problem.</param>
        /// 
        public HiddenMarkovClassifier(HiddenMarkovModel<TDistribution>[] models)
            : base(models) { }

        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        /// 
        /// <param name="classes">The number of classes in the classifier.</param>
        /// <param name="topology">The topology of the hidden states. A forward-only topology
        /// is indicated to sequence classification problems, such as speech recognition.</param>
        /// <param name="initial">The initial probability distributions for the hidden states.
        /// For multivariate continuous density distributions, such as Normal mixtures, the
        /// choice of initial values is crucial for a good performance.</param>
        /// <param name="names">The class labels for each of the models.</param>
        /// 
        public HiddenMarkovClassifier(int classes, ITopology topology, TDistribution initial, string[] names)
            : base(classes)
        {
            for (int i = 0; i < classes; i++)
                Models[i] = new HiddenMarkovModel<TDistribution>(topology, initial) { Tag = names[i] };
        }


        /// <summary>
        ///   Computes the most likely class for a given sequence.
        /// </summary>
        /// 
        /// <param name="sequence">The sequence of observations.</param>
        /// 
        /// <returns>Return the label of the given sequence, or -1 if it has
        /// been rejected by the <see cref="BaseHiddenMarkovClassifier{T}.Threshold">
        /// threshold model</see>.</returns>
        /// 
        public new int Compute(Array sequence)
        {
            return base.Compute(sequence);
        }

        /// <summary>
        ///   Computes the most likely class for a given sequence.
        /// </summary>
        /// 
        /// <param name="sequence">The sequence of observations.</param>
        /// <param name="response">The probability of the assigned class.</param>
        /// 
        /// <returns>Return the label of the given sequence, or -1 if it has
        /// been rejected by the <see cref="BaseHiddenMarkovClassifier{T}.Threshold">
        /// threshold model</see>.</returns>
        /// 
        public new int Compute(Array sequence, out double response)
        {
            return base.Compute(sequence, out response);
        }

        /// <summary>
        ///   Computes the most likely class for a given sequence.
        /// </summary>
        /// 
        /// <param name="sequence">The sequence of observations.</param>
        /// <param name="responsibilities">The class responsibilities (or
        /// the probability of the sequence to belong to each class). When
        /// using threshold models, the sum of the probabilities will not
        /// equal one, and the amount left was the threshold probability.
        /// If a threshold model is not being used, the array should sum to
        /// one.</param>
        /// 
        /// <returns>Return the label of the given sequence, or -1 if it has
        /// been rejected by the <see cref="BaseHiddenMarkovClassifier{T}.Threshold">
        /// threshold model</see>.</returns>
        /// 
        public new int Compute(Array sequence, out double[] responsibilities)
        {
            return base.Compute(sequence, out responsibilities);
        }

        /// <summary>
        ///   Computes the log-likelihood of a sequence
        ///   belong to a given class according to this
        ///   classifier.
        /// </summary>
        /// <param name="sequence">The sequence of observations.</param>
        /// <param name="output">The output class label.</param>
        /// 
        /// <returns>The log-likelihood of the sequence belonging to the given class.</returns>
        /// 
        public new double LogLikelihood(Array sequence, int output)
        {
            double[] responsabilities;
            base.Compute(sequence, out responsabilities);
            return Math.Log(responsabilities[output]);
        }

        /// <summary>
        ///   Computes the log-likelihood that a sequence
        ///   belongs any of the classes in the classifier.
        /// </summary>
        /// <param name="sequence">The sequence of observations.</param>
        /// 
        /// <returns>The log-likelihood of the sequence belonging to the classifier.</returns>
        /// 
        public new double LogLikelihood(Array sequence)
        {
            return base.LogLikelihood(sequence);
        }

        /// <summary>
        ///   Computes the log-likelihood of a set of sequences
        ///   belonging to their given respective classes according
        ///   to this classifier.
        /// </summary>
        /// <param name="sequences">A set of sequences of observations.</param>
        /// <param name="outputs">The output class label for each sequence.</param>
        /// 
        /// <returns>The log-likelihood of the sequences belonging to the given classes.</returns>
        /// 
        public new double LogLikelihood(Array[] sequences, int[] outputs)
        {
            double[] responsabilities;

            double logLikelihood = 0;
            for (int i = 0; i < sequences.Length; i++)
            {
                base.Compute(sequences[i], out responsabilities);
                logLikelihood += Math.Log(responsabilities[outputs[i]]);
            }
            return logLikelihood;
        }



        #region Load & Save methods

        /// <summary>
        ///   Saves the classifier to a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to which the classifier is to be serialized.</param>
        /// 
        public void Save(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(stream, this);
        }

        /// <summary>
        ///   Saves the classifier to a stream.
        /// </summary>
        /// 
        /// <param name="path">The stream to which the classifier is to be serialized.</param>
        /// 
        public void Save(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                Save(fs);
            }
        }

        /// <summary>
        ///   Loads a classifier from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from which the classifier is to be deserialized.</param>
        /// 
        /// <returns>The deserialized classifier.</returns>
        /// 
        public static HiddenMarkovClassifier<TDistribution> Load(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            return (HiddenMarkovClassifier<TDistribution>)b.Deserialize(stream);
        }

        /// <summary>
        ///   Loads a classifier from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the classifier is to be deserialized.</param>
        /// 
        /// <returns>The deserialized classifier.</returns>
        /// 
        public static HiddenMarkovClassifier<TDistribution> Load(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                return Load(fs);
            }
        }

        #endregion

    }
}
