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

namespace Accord.Statistics.Models.Markov.Learning
{
#pragma warning disable 612, 618

    using System;
    using System.Threading;
    using Accord.Math;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Models.Markov.Topology;

    /// <summary>
    ///   Learning algorithm for <see cref="HiddenMarkovClassifier{TDistribution}">
    ///   arbitrary-density generative hidden Markov sequence classifiers</see>.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class acts as a teacher for <see cref="HiddenMarkovClassifier{TDistribution}">
    ///   classifiers based on arbitrary-density hidden Markov models</see>. The learning
    ///   algorithm uses a generative approach. It works by training each model in the
    ///   generative classifier separately.</para>
    ///   
    /// <para>
    ///   This can teach models that use any <see cref="IDistribution">probability
    ///   distribution</see>. Such <see cref="HiddenMarkovModel{T}">arbitrary-density models
    ///   </see> can be used for any kind of observation values or vectors. When 
    ///   
    /// 
    ///   be used whenever the sequence of observations is discrete or can be represented
    ///   by discrete symbols, such as class labels, integers, and so on. If you need
    ///   to classify sequences of other entities, such as real numbers, vectors (i.e.
    ///   multivariate observations), then you can use 
    ///   <see cref="HiddenMarkovClassifierLearning{TDistribution}">generic-density
    ///   hidden Markov models</see>. Those models can be modeled after any kind of
    ///   <see cref="IDistribution">probability distribution</see> implementing
    ///   the <see cref="IDistribution"/> interface.</para>
    ///   
    /// <para>
    ///   For a more thorough explanation on <see cref="HiddenMarkovModel">hidden Markov models</see>
    ///   with practical examples on gesture recognition, please see 
    ///   <a href="http://www.codeproject.com/Articles/541428/Sequence-Classifiers-in-Csharp-Part-I-Hidden-Marko">
    ///   Sequence Classifiers in C#, Part I: Hidden Markov Models</a> [1].</para>
    ///     
    /// <para>
    ///   [1]: <a href="http://www.codeproject.com/Articles/541428/Sequence-Classifiers-in-Csharp-Part-I-Hidden-Marko"> 
    ///           http://www.codeproject.com/Articles/541428/Sequence-Classifiers-in-Csharp-Part-I-Hidden-Marko </a>
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    ///   <para>
    ///   The following example creates a continuous-density hidden Markov model sequence
    ///   classifier to recognize two classes of univariate observation sequences.</para>
    ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Markov\HiddenMarkovClassifier`2Test.cs" region="doc_learn" />
    /// 
    ///   <para>
    ///   The following example creates a continuous-density hidden Markov model sequence
    ///   classifier to recognize two classes of multivariate sequence of observations.
    ///   This example uses multivariate Normal distributions as emission densities. </para>
    ///   
    ///   <para>
    ///   When there is insufficient training data, or one of the variables is constant,
    ///   the Normal distribution estimation may fail with a "Covariance matrix is not
    ///   positive-definite". In this case, it is possible to sidestep this issue by
    ///   specifying a small regularization constant to be added to the diagonal elements
    ///   of the covariance matrix. </para>
    ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Markov\HiddenMarkovClassifier`2Test.cs" region="doc_learn_regularization" />
    /// 
    ///   <para>
    ///   The next example shows how to use the learning algorithms in a real-world dataset,
    ///   including training and testing in separate sets and evaluating its performance:</para>
    ///   <code source="Unit Tests\Accord.Tests.Statistics\Models\Markov\HiddenMarkovClassifier`2Test.cs" region="doc_learn_pendigits" />
    /// </example>
    /// 
    /// <seealso cref="HiddenMarkovClassifier{TDistribution, TObservation}"/>
    /// <seealso cref="HiddenMarkovClassifier"/>
    /// <seealso cref="HiddenMarkovClassifierLearning"/>
    /// 
    public class HiddenMarkovClassifierLearning<TDistribution, TObservation> :
        BaseHiddenMarkovClassifierLearning<HiddenMarkovClassifier<TDistribution, TObservation>,
        HiddenMarkovModel<TDistribution, TObservation>, TDistribution, TObservation>
        where TDistribution : IDistribution<TObservation>
    {

        /// <summary>
        ///   Creates a new instance of the learning algorithm for a given 
        ///   Markov sequence classifier using the specified configuration
        ///   function.
        /// </summary>
        /// 
        [Obsolete("Please set the learning algorithm using the Learner property.")]
        public HiddenMarkovClassifierLearning(HiddenMarkovClassifier<TDistribution, TObservation> classifier,
            ClassifierLearningAlgorithmConfiguration algorithm)
            : base(classifier, algorithm)
        {
        }

        /// <summary>
        ///   Creates a new instance of the learning algorithm for a given 
        ///   Markov sequence classifier.
        /// </summary>
        /// 
        public HiddenMarkovClassifierLearning(HiddenMarkovClassifier<TDistribution, TObservation> classifier)
            : base(classifier)
        {
        }

        /// <summary>
        ///   Creates a new instance of the learning algorithm for a given 
        ///   Markov sequence classifier.
        /// </summary>
        /// 
        public HiddenMarkovClassifierLearning()
        {
        }

        /// <summary>
        ///   Creates an instance of the model to be learned. Inheritors of this abstract 
        ///   class must define this method so new models can be created from the training data.
        /// </summary>
        /// 
        protected override HiddenMarkovClassifier<TDistribution, TObservation> Create(TObservation[][] x, int[] y, int numberOfClasses)
        {
            return new HiddenMarkovClassifier<TDistribution, TObservation>(numberOfClasses);
        }

        /// <summary>
        ///   Creates a new <see cref="Threshold">threshold model</see>
        ///   for the current set of Markov models in this sequence classifier.
        /// </summary>
        /// 
        /// <returns>
        ///   A <see cref="Threshold">threshold Markov model</see>.
        /// </returns>
        /// 
        public override HiddenMarkovModel<TDistribution, TObservation> Threshold()
        {
            var models = Classifier.Models;
            var topology = CreateThresholdTopology();

            int states = topology.States;

            // Create the threshold emission matrix
            var emissions = new TDistribution[states];


            // Then, for each hidden Markov model in the classifier
            for (int i = 0, modelStartIndex = 0; i < models.Length; i++)
            {
                // Retrieve the model definition matrices
                TDistribution[] B = models[i].Emissions;

                // Now, for each state in the model
                for (int j = 0; j < models[i].States; j++)
                {
                    // Copy emissions from the model
                    emissions[j + modelStartIndex] = B[j];
                }

                // Next model starts where this ends
                modelStartIndex += models[i].States;
            }


            // Create and return the threshold hidden Markov model
            return new HiddenMarkovModel<TDistribution, TObservation>(topology, emissions)
            {
                Tag = "Non-gesture"
            };
        }

    }
}
