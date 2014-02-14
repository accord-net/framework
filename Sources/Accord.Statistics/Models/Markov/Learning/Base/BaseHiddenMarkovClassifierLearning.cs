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

namespace Accord.Statistics.Models.Markov.Learning
{
    using System;
    using Accord.Math;
    using System.Threading.Tasks;
    using Accord.Statistics.Models.Markov.Topology;
    using System.Diagnostics;

    /// <summary>
    ///   Configuration function delegate for Sequence Classifier Learning algorithms.
    /// </summary>
    public delegate IUnsupervisedLearning ClassifierLearningAlgorithmConfiguration(int modelIndex);

    /// <summary>
    ///   Submodel learning event arguments.
    /// </summary>
    public class GenerativeLearningEventArgs : EventArgs
    {
        /// <summary>
        ///   Gets the generative class model to 
        ///   which this event refers to.
        /// </summary>
        public int Class { get; set; }

        /// <summary>
        ///   Gets the total number of models
        ///   to be learned.
        /// </summary>
        /// 
        public int Total { get; set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="GenerativeLearningEventArgs"/> class.
        /// </summary>
        /// 
        /// <param name="classLabel">The class label.</param>
        /// <param name="classes">The total number of classes.</param>
        /// 
        public GenerativeLearningEventArgs(int classLabel, int classes)
        {
            this.Class = classLabel;
            this.Total = classes;
        }

    }

    /// <summary>
    ///   Abstract base class for Sequence Classifier learning algorithms.
    /// </summary>
    /// 
    public abstract class BaseHiddenMarkovClassifierLearning<TClassifier, TModel>
        where TClassifier : BaseHiddenMarkovClassifier<TModel>
        where TModel : IHiddenMarkovModel
    {


        /// <summary>
        ///   Gets the classifier being trained by this instance.
        /// </summary>
        /// <value>The classifier being trained by this instance.</value>
        /// 
        public TClassifier Classifier { get; private set; }

        /// <summary>
        ///   Gets or sets the configuration function specifying which
        ///   training algorithm should be used for each of the models
        ///   in the hidden Markov model set.
        /// </summary>
        /// 
        public ClassifierLearningAlgorithmConfiguration Algorithm { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether a threshold model
        ///   should be created or updated after training to support rejection.
        /// </summary>
        /// <value><c>true</c> to update the threshold model after training;
        /// otherwise, <c>false</c>.</value>
        /// 
        public bool Rejection { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether the class priors
        ///   should be estimated from the data, as in an empirical Bayes method.
        /// </summary>
        /// 
        public bool Empirical { get; set; }

        /// <summary>
        ///   Occurs when the learning of a class model has started.
        /// </summary>
        /// 
        public event EventHandler<GenerativeLearningEventArgs> ClassModelLearningStarted;

        /// <summary>
        ///   Occurs when the learning of a class model has finished.
        /// </summary>
        /// 
        public event EventHandler<GenerativeLearningEventArgs> ClassModelLearningFinished;

        /// <summary>
        ///   Creates a new instance of the learning algorithm for a given 
        ///   Markov sequence classifier using the specified configuration
        ///   function.
        /// </summary>
        /// 
        protected BaseHiddenMarkovClassifierLearning(TClassifier classifier,
            ClassifierLearningAlgorithmConfiguration algorithm)
        {
            this.Classifier = classifier;
            this.Algorithm = algorithm;
        }



        /// <summary>
        ///   Trains each model to recognize each of the output labels.
        /// </summary>
        /// <returns>The sum log-likelihood for all models after training.</returns>
        /// 
        protected double Run<T>(T[] inputs, int[] outputs)
        {
            if (inputs == null) 
                throw new ArgumentNullException("inputs");

            if (outputs == null) 
                throw new ArgumentNullException("outputs");

            if (inputs.Length != outputs.Length)
                throw new DimensionMismatchException("outputs",
                    "The number of inputs and outputs does not match.");

            for (int i = 0; i < outputs.Length; i++)
                if (outputs[i] < 0 || outputs[i] >= Classifier.Classes)
                    throw new ArgumentOutOfRangeException("outputs");


            int classes = Classifier.Classes;
            double[] logLikelihood = new double[classes];
            int[] classCounts = new int[classes];


            // For each model,
            Parallel.For(0, classes, i =>
            {
                // We will start the class model learning problem
                var args = new GenerativeLearningEventArgs(i, classes);
                OnGenerativeClassModelLearningStarted(args);

                // Select the input/output set corresponding
                //  to the model's specialization class
                int[] inx = outputs.Find(y => y == i);
                T[] observations = inputs.Submatrix(inx);

                classCounts[i] = observations.Length;

                if (observations.Length > 0)
                {
                    // Create and configure the learning algorithm
                    IUnsupervisedLearning teacher = Algorithm(i);

                    // Train the current model in the input/output subset
                    logLikelihood[i] = teacher.Run(observations as Array[]);
                }

                // Update and report progress
                OnGenerativeClassModelLearningFinished(args);
            });

            if (Empirical)
            {
                for (int i = 0; i < classes; i++)
                    Classifier.Priors[i] = (double)classCounts[i] / inputs.Length;
            }

            if (Rejection)
            {
                Classifier.Threshold = Threshold();
            }

            // Returns the sum log-likelihood for all models.
            return logLikelihood.Sum();
        }


        /// <summary>
        ///   Creates a new <see cref="Threshold">threshold model</see>
        ///   for the current set of Markov models in this sequence classifier.
        /// </summary>
        /// <returns>A <see cref="Threshold">threshold Markov model</see>.</returns>
        /// 
        public abstract TModel Threshold();

        /// <summary>
        ///   Creates the state transition topology for the threshold model. This
        ///   method can be used to help in the implementation of the <see cref="Threshold"/>
        ///   abstract method which has to be defined for implementers of this class.
        /// </summary>
        /// 
        protected ITopology CreateThresholdTopology()
        {
            TModel[] models = Classifier.Models;

            int states = 0;

            // Get the total number of states
            for (int i = 0; i < models.Length; i++)
                states += models[i].States;

            // Create the threshold model transition matrix
            double[,] transitions = new double[states, states];

            // Set the initial probabilities
            double[] initial = new double[states];
            for (int i = 0; i < initial.Length; i++)
                initial[i] = 1.0 / models.Length;


            // Then, for each hidden Markov model in the classifier
            for (int i = 0, modelStartIndex = 0; i < models.Length; i++)
            {

                // Now, for each state 'j' in the model
                for (int j = 0; j < models[i].States; j++)
                {
                    // Retrieve the state self-transition probability
                    double self = Math.Exp(models[i].Transitions[j, j]);

                    // Make sure the exp-log conversion was within limits
                    if (self < 0) self = 0; else if (self > 1) self = 1;

                    // Check where we should write it
                    int stateIndex = modelStartIndex + j;

                    // Copy the self-transition probability
                    transitions[stateIndex, stateIndex] = self;

                    // And normalize all others to sum up to one
                    double pinv = (1.0 - self) / (models[i].States - 1);

                    for (int k = 0; k < models[i].States; k++)
                        if (j != k) transitions[stateIndex, modelStartIndex + k] = pinv;

#if DEBUG
                    // Rows should sum up to one.
                    check(transitions, stateIndex);
#endif
                }

                // Next model starts where this ends
                modelStartIndex += models[i].States;
            }


            // Create and return the custom threshold topology
            return new Custom(transitions, initial, logarithm: false);
        }

        
        private static void check(double[,] transitions, int index)
        {
            // Check if they indeed sum up to one
            var modelRow = transitions.GetRow(index);
            var modelRowSum = modelRow.Sum();
            if (Math.Abs(modelRowSum - 1.0) >= 1e-4)
                throw new InvalidOperationException("Rows do not sum to one.");
        }


        /// <summary>
        ///   Raises the <see cref="E:GenerativeClassModelLearningFinished"/> event.
        /// </summary>
        /// 
        /// <param name="args">The <see cref="Accord.Statistics.Models.Markov.Learning.GenerativeLearningEventArgs"/> instance containing the event data.</param>
        /// 
        protected void OnGenerativeClassModelLearningFinished(GenerativeLearningEventArgs args)
        {
            if (ClassModelLearningFinished != null)
                ClassModelLearningFinished(this, args);
        }

        /// <summary>
        ///   Raises the <see cref="E:GenerativeClassModelLearningStarted"/> event.
        /// </summary>
        /// 
        /// <param name="args">The <see cref="Accord.Statistics.Models.Markov.Learning.GenerativeLearningEventArgs"/> instance containing the event data.</param>
        /// 
        protected void OnGenerativeClassModelLearningStarted(GenerativeLearningEventArgs args)
        {
            if (ClassModelLearningStarted != null)
                ClassModelLearningStarted(this, args);
        }
    }
}
