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
    using Accord.Math;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Models.Markov.Learning;
    using Accord.Statistics.Models.Markov.Topology;
    using Accord.Compat;

    /// <summary>
    ///   Discrete-density Hidden Markov Model Set for Sequence Classification.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class uses a set of <see cref="HiddenMarkovModel">discrete hidden Markov models</see>
    ///   to classify sequences of integer symbols. Each model will try to learn and recognize each 
    ///   of the different output classes. For examples and details on how to learn such models,
    ///   please take a look on the documentation for <see cref="HiddenMarkovClassifierLearning"/>.</para>
    /// <para>
    ///   For other type of sequences, such as discrete sequences (not necessarily symbols) or even
    ///   continuous and multivariate variables, please see use the generic classifier counterpart 
    ///   <see cref="HiddenMarkovClassifier{TDistribution}"/></para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   Examples are available at the respective learning algorithm pages. For 
    ///   example, see <see cref="HiddenMarkovClassifierLearning"/>. </para>
    /// </example>
    /// 
    /// <seealso cref="HiddenMarkovClassifier{TDistribution}"/>
    /// <seealso cref="HiddenMarkovClassifierLearning"/>
    /// 
#pragma warning disable 612, 618
    [Serializable]
    public class HiddenMarkovClassifier : BaseHiddenMarkovClassifier<
            HiddenMarkovModel, GeneralDiscreteDistribution, int>,
        IEnumerable<HiddenMarkovModel>,
        IHiddenMarkovClassifier
#pragma warning restore 612, 618
    {

        /// <summary>
        ///   Obsolete. Please use <see cref="NumberOfSymbols"/> instead.
        /// </summary>
        /// 
        [Obsolete("Please use NumberOfSymbols instead.")]
        public int Symbols
        {
            get { return NumberOfSymbols; }
        }

        /// <summary>
        ///   Gets the number of symbols
        ///   recognizable by the models.
        /// </summary>
        /// 
        public int NumberOfSymbols
        {
            get { return this[0].NumberOfSymbols; }
        }


        #region Constructors

        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        /// 
        /// <param name="classes">The number of classes in the classifier.</param>
        /// 
        public HiddenMarkovClassifier(int classes)
            : base(classes)
        {
        }

        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        /// 
        /// <param name="classes">The number of classes in the classifier.</param>
        /// <param name="topology">The topology of the hidden states. A forward-only topology
        /// is indicated to sequence classification problems, such as speech recognition.</param>
        /// <param name="symbols">The number of symbols in the models' discrete alphabet.</param>
        /// <param name="names">The optional class names for each of the classifiers.</param>
        /// 
        public HiddenMarkovClassifier(int classes, ITopology topology, int symbols, string[] names)
            : base(classes)
        {
            for (int i = 0; i < classes; i++)
                Models[i] = new HiddenMarkovModel(topology, symbols) { Tag = names[i] };
        }

        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        /// 
        /// <param name="classes">The number of classes in the classifier.</param>
        /// <param name="topology">The topology of the hidden states. A forward-only topology
        /// is indicated to sequence classification problems, such as speech recognition.</param>
        /// <param name="symbols">The number of symbols in the models' discrete alphabet.</param>
        /// 
        public HiddenMarkovClassifier(int classes, ITopology topology, int symbols)
            : base(classes)
        {
            for (int i = 0; i < classes; i++)
                Models[i] = new HiddenMarkovModel(topology, symbols);
        }

        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        /// 
        /// <param name="classes">The number of classes in the classifier.</param>
        /// <param name="topology">The topology of the hidden states. A forward-only topology
        /// is indicated to sequence classification problems, such as speech recognition.</param>
        /// <param name="symbols">The number of symbols in the models' discrete alphabet.</param>
        /// 
        public HiddenMarkovClassifier(int classes, ITopology[] topology, int symbols)
            : base(classes)
        {
            if (topology.Length != classes)
                throw new ArgumentException("The number of topology specifications should equal the number of classes", "classes");

            for (int i = 0; i < classes; i++)
                Models[i] = new HiddenMarkovModel(topology[i], symbols);
        }

        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        /// 
        /// <param name="classes">The number of classes in the classifier.</param>
        /// <param name="topology">The topology of the hidden states. A forward-only topology
        /// is indicated to sequence classification problems, such as speech recognition.</param>
        /// <param name="symbols">The number of symbols in the models' discrete alphabet.</param>
        /// <param name="names">The optional class names for each of the classifiers.</param>
        /// 
        public HiddenMarkovClassifier(int classes, ITopology[] topology, int symbols, string[] names)
            : base(classes)
        {
            if (topology.Length != classes)
                throw new ArgumentException("The number of topology specifications should equal the number of classes", "classes");

            for (int i = 0; i < classes; i++)
                Models[i] = new HiddenMarkovModel(topology[i], symbols) { Tag = names[i] };
        }

        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        /// 
        /// <param name="classes">The number of classes in the classifier.</param>
        /// <param name="states">An array specifying the number of hidden states for each
        /// of the classifiers. By default, and Ergodic topology will be used.</param>
        /// <param name="symbols">The number of symbols in the models' discrete alphabet.</param>
        /// <param name="names">The optional class names for each of the classifiers.</param>
        /// 
        public HiddenMarkovClassifier(int classes, int[] states, int symbols, string[] names)
            : base(classes)
        {
            if (states.Length != classes)
                throw new ArgumentException("The number of state specifications should equal the number of classes.", "classes");

            for (int i = 0; i < classes; i++)
                Models[i] = new HiddenMarkovModel(new Ergodic(states[i]), symbols) { Tag = names[i] };
        }

        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        /// 
        /// <param name="classes">The number of classes in the classifier.</param>
        /// <param name="states">An array specifying the number of hidden states for each
        /// of the classifiers. By default, and Ergodic topology will be used.</param>
        /// <param name="symbols">The number of symbols in the models' discrete alphabet.</param>
        /// 
        public HiddenMarkovClassifier(int classes, int[] states, int symbols)
            : base(classes)
        {
            if (states.Length != classes)
                throw new ArgumentException("The number of state specifications should equal the number of classes.", "classes");

            for (int i = 0; i < classes; i++)
                Models[i] = new HiddenMarkovModel(new Ergodic(states[i]), symbols);
        }
        #endregion


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
        [Obsolete("Please use Decide(input) instead.")]
        public int Compute(int[] sequence)
        {
            return base.Decide(sequence);
        }

        /// <summary>
        ///   Computes the most likely class for a given sequence.
        /// </summary>
        /// 
        /// <param name="sequence">The sequence of observations.</param>
        /// <param name="response">The class responsibilities (or
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
        [Obsolete("Please use Decide(input) or Probability(input) instead.")]
        public int Compute(int[] sequence, out double response)
        {
            int decision;
            response = base.Probability(sequence, out decision);
            return decision;
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
        [Obsolete("Please use Decide(input) instead.")]
        public int Compute(int[] sequence, out double[] responsibilities)
        {
            int decision;
            responsibilities = base.Probabilities(sequence, out decision);
            return decision;
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
        public new double LogLikelihood(int[][] sequences, int[] outputs)
        {
            // for backwards compatibility
            return base.LogLikelihood(sequences, outputs).Sum();
        }


#pragma warning disable 612, 618

        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        /// 
        /// <param name="classes">The number of classes in the classifier.</param>
        /// <param name="states">An array specifying the number of hidden states for each
        /// of the classifiers. By default, and Ergodic topology will be used.</param>
        /// <param name="symbols">The number of symbols in the models' discrete alphabet.</param>
        /// 
        public static HiddenMarkovClassifier<GeneralDiscreteDistribution> CreateGeneric(
            int classes, int[] states, int symbols)
        {
            var classifier = new HiddenMarkovClassifier<GeneralDiscreteDistribution>(
                classes, states, new GeneralDiscreteDistribution(symbols));

            return classifier;
        }

#pragma warning restore 612, 618


        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        /// 
        /// <param name="classes">The number of classes in the classifier.</param>
        /// <param name="states">An array specifying the number of hidden states for each
        /// of the classifiers. By default, and Ergodic topology will be used.</param>
        /// <param name="symbols">The number of symbols in the models' discrete alphabet.</param>
        /// 
        public static HiddenMarkovClassifier<GeneralDiscreteDistribution, int> CreateGeneric2(
            int classes, int[] states, int symbols)
        {
            var classifier = new HiddenMarkovClassifier<GeneralDiscreteDistribution, int>(
                classes, states, new GeneralDiscreteDistribution(symbols));

            return classifier;
        }

        #region ISequenceClassifier implementation

#pragma warning disable 612, 618
        /// <summary>
        ///   Computes the most likely class for a given sequence.
        /// </summary>
        /// 
        int IHiddenMarkovClassifier.Compute(Array sequence, out double[] likelihoods)
        {
            int decision;
            likelihoods = base.LogLikelihoods(sequence as int[], out decision);
            return decision;
        }
#pragma warning restore 612, 618
        #endregion


        #region Save & Load methods
#pragma warning disable 612, 618

        /// <summary>
        ///   Saves the classifier to a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to which the classifier is to be serialized.</param>
        /// 
        [Obsolete("Please use Accord.Serializer.Save instead.")]
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
        [Obsolete("Please use Accord.Serializer.Save instead.")]
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
        [Obsolete("Please use Accord.Serializer.Load instead.")]
        public static HiddenMarkovClassifier Load(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            return (HiddenMarkovClassifier)b.Deserialize(stream);
        }

        /// <summary>
        ///   Loads a classifier from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the classifier is to be deserialized.</param>
        /// 
        /// <returns>The deserialized classifier.</returns>
        /// 
        [Obsolete("Please use Accord.Serializer.Load instead.")]
        public static HiddenMarkovClassifier Load(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                return Load(fs);
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
        [Obsolete("Please use Accord.Serializer.Load instead.")]
        public static HiddenMarkovClassifier<TDistribution> Load<TDistribution>(Stream stream)
            where TDistribution : IDistribution
        {
            return HiddenMarkovClassifier<TDistribution>.Load(stream);
        }

        /// <summary>
        ///   Loads a classifier from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the classifier is to be deserialized.</param>
        /// 
        /// <returns>The deserialized classifier.</returns>
        /// 
        [Obsolete("Please use Accord.Serializer.Load instead.")]
        public static HiddenMarkovClassifier<TDistribution> Load<TDistribution>(string path)
            where TDistribution : IDistribution
        {
            return HiddenMarkovClassifier<TDistribution>.Load(path);
        }
#pragma warning restore 612, 618

        #endregion

    }
}
