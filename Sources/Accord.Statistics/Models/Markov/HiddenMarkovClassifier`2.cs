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
#pragma warning disable 612, 618

namespace Accord.Statistics.Models.Markov
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Models.Markov.Topology;
    using Accord.Statistics.Models.Markov.Learning;
    using System.Collections.Generic;
    using Accord.Compat;

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
    ///   example, see <see cref="HiddenMarkovClassifierLearning{TDistribution, TObservation}"/>. </para>
    /// </example>
    /// 
    /// <seealso cref="HiddenMarkovClassifierLearning{TDistribution}"/>
    /// <seealso cref="HiddenMarkovClassifier"/>
    ///   
    [Serializable]
    public class HiddenMarkovClassifier<TDistribution, TObservation> :
        BaseHiddenMarkovClassifier<HiddenMarkovModel<TDistribution, TObservation>, TDistribution, TObservation>,
        IEnumerable<HiddenMarkovModel<TDistribution, TObservation>>
        where TDistribution : IDistribution<TObservation>
    {

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
        /// <param name="states">An array specifying the number of hidden states for each
        /// of the classifiers. By default, and Ergodic topology will be used.</param>
        /// <param name="initial">The initial probability distributions for the hidden states.
        /// For multivariate continuous density distributions, such as Normal mixtures, the
        /// choice of initial values is crucial for a good performance.</param>
        /// 
        public HiddenMarkovClassifier(int classes, int[] states, Func<int, int, TDistribution> initial)
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
        /// <param name="initial">The initial probability distributions for the hidden states.
        /// For multivariate continuous density distributions, such as Normal mixtures, the
        /// choice of initial values is crucial for a good performance.</param>
        /// 
        public HiddenMarkovClassifier(int classes, ITopology topology, Func<int, int, TDistribution> initial)
            : base(classes)
        {
            for (int i = 0; i < classes; i++)
                Models[i] = new HiddenMarkovModel<TDistribution, TObservation>(topology, (stateIndex) => initial(i, stateIndex));
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
                Models[i] = new HiddenMarkovModel<TDistribution, TObservation>(states[i], initial);
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
                Models[i] = new HiddenMarkovModel<TDistribution, TObservation>(topology, initial);
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
                Models[i] = new HiddenMarkovModel<TDistribution, TObservation>(topology, initial[i]);
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
                Models[i] = new HiddenMarkovModel<TDistribution, TObservation>(topology[i], initial[i]);
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
                Models[i] = new HiddenMarkovModel<TDistribution, TObservation>(topology[i], initial);
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
                Models[i] = new HiddenMarkovModel<TDistribution, TObservation>(topology[i], initial) { Tag = names[i] };
        }

        /// <summary>
        ///   Creates a new Sequence Classifier with the given number of classes.
        /// </summary>
        /// 
        /// <param name="models">
        ///   The models specializing in each of the classes of 
        ///   the classification problem.</param>
        /// 
        public HiddenMarkovClassifier(HiddenMarkovModel<TDistribution, TObservation>[] models)
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
                Models[i] = new HiddenMarkovModel<TDistribution, TObservation>(topology, initial) { Tag = names[i] };
        }


    }
}
