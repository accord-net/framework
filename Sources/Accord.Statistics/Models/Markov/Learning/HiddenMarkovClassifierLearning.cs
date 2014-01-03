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
    using System.Threading;
    using System.Threading.Tasks;
    using Accord.Math;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Models.Markov.Topology;

    /// <summary>
    ///   Learning algorithm for discrete-density <see cref="HiddenMarkovClassifier">
    ///   generative hidden Markov sequence classifiers</see>.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class acts as a teacher for <see cref="HiddenMarkovClassifier">
    ///   classifiers based on discrete hidden Markov models</see>. The learning
    ///   algorithm uses a generative approach. It works by training each model in
    ///   the generative classifier separately.</para>
    ///   
    /// <para>
    ///   This class implements discrete classifiers only. Discrete classifiers can
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
    /// <para>
    ///   The following example shows how to create a hidden Markov model sequence classifier
    ///   to classify discrete sequences into two disjoint labels: labels for class 0 and 
    ///   labels for class 1. The training data is separated in inputs and outputs. The
    ///   inputs are the sequences we are trying to learn, and the outputs are the labels
    ///   associated with each input sequence.</para>
    ///   
    /// <para>
    ///   In this example we will be using the <see cref="BaumWelchLearning">Baum-Welch</see>
    ///   algorithm to learn each model in our generative classifier; however, any other
    ///   <see cref="IUnsupervisedLearning">unsupervised learning algorithm</see> could be used.
    /// </para>
    ///   
    ///   <code>
    ///   // Declare some testing data
    ///   int[][] inputs = new int[][]
    ///   {
    ///       new int[] { 0,1,1,0 },   // Class 0
    ///       new int[] { 0,0,1,0 },   // Class 0
    ///       new int[] { 0,1,1,1,0 }, // Class 0
    ///       new int[] { 0,1,0 },     // Class 0
    ///   
    ///       new int[] { 1,0,0,1 },   // Class 1
    ///       new int[] { 1,1,0,1 },   // Class 1
    ///       new int[] { 1,0,0,0,1 }, // Class 1
    ///       new int[] { 1,0,1 },     // Class 1
    ///   };
    ///   
    ///   int[] outputs = new int[]
    ///   {
    ///       0,0,0,0, // First four sequences are of class 0
    ///       1,1,1,1, // Last four sequences are of class 1
    ///   };
    ///   
    ///   
    ///   // We are trying to predict two different classes
    ///   int classes = 2;
    ///
    ///   // Each sequence may have up to two symbols (0 or 1)
    ///   int symbols = 2;
    ///
    ///   // Nested models will have two states each
    ///   int[] states = new int[] { 2, 2 };
    ///
    ///   // Creates a new Hidden Markov Model Sequence Classifier with the given parameters
    ///   HiddenMarkovClassifier classifier = new HiddenMarkovClassifier(classes, states, symbols);
    ///   
    ///   // Create a new learning algorithm to train the sequence classifier
    ///   var teacher = new HiddenMarkovClassifierLearning(classifier,
    ///   
    ///       // Train each model until the log-likelihood changes less than 0.001
    ///       modelIndex => new BaumWelchLearning(classifier.Models[modelIndex])
    ///       {
    ///           Tolerance = 0.001,
    ///           Iterations = 0
    ///       }
    ///   );
    ///   
    ///   // Train the sequence classifier using the algorithm
    ///   double likelihood = teacher.Run(inputs, outputs);
    ///   
    ///   </code>
    /// </example>
    /// 
    /// <see cref="HiddenMarkovClassifier"/>
    /// <see cref="HiddenMarkovClassifierLearning{TDistribution}"/>
    /// 
    public class HiddenMarkovClassifierLearning :
        BaseHiddenMarkovClassifierLearning<HiddenMarkovClassifier, HiddenMarkovModel>
    {

        private int smoothingKernelSize = 3;
        private double smoothingSigma = 1.0;
        private double[] gaussianKernel;

        /// <summary>
        ///   Gets or sets the smoothing kernel's sigma
        ///   for the threshold model.
        /// </summary>
        /// 
        /// <value>The smoothing kernel's sigma.</value>
        /// 
        public double Smoothing
        {
            get { return smoothingSigma; }
            set
            {
                smoothingSigma = value;
                createSmoothingKernel();
            }
        }

        /// <summary>
        ///   Creates a new instance of the learning algorithm for a given 
        ///   Markov sequence classifier using the specified configuration
        ///   function.
        /// </summary>
        /// 
        public HiddenMarkovClassifierLearning(HiddenMarkovClassifier classifier,
            ClassifierLearningAlgorithmConfiguration algorithm)
            : base(classifier, algorithm)
        {
            createSmoothingKernel();
        }


        /// <summary>
        ///   Trains each model to recognize each of the output labels.
        /// </summary>
        /// 
        /// <returns>The sum log-likelihood for all models after training.</returns>
        /// 
        public double Run(int[][] inputs, int[] outputs)
        {
            if (inputs == null)
                throw new ArgumentNullException("inputs");

            for (int i = 0; i < inputs.Length; i++)
            {
                for (int j = 0; j < inputs[i].Length; j++)
                {
                    int symbol = inputs[i][j];

                    if (symbol < 0 || symbol >= Classifier.Symbols)
                    {
                        string message = "Observation sequences should only contain symbols that are " +
                        "greater than or equal to 0, and lesser than the number of symbols passed to " +
                        "the HiddenMarkovClassifier. This classifier is expecting at most {0} symbols.";

                        throw new ArgumentOutOfRangeException("inputs", String.Format(message, Classifier.Symbols));
                    }
                }
            }

            return base.Run<int[]>(inputs, outputs);
        }


        /// <summary>
        ///   Compute model error for a given data set.
        /// </summary>
        /// 
        /// <param name="inputs">The input points.</param>
        /// <param name="outputs">The output points.</param>
        /// 
        /// <returns>The percent of misclassification errors for the data.</returns>
        /// 
        public double ComputeError(int[][] inputs, int[] outputs)
        {
            int errors = 0;
            Parallel.For(0, inputs.Length, i =>
            {
                int expectedOutput = outputs[i];
                int actualOutput = Classifier.Compute(inputs[i]);

                if (expectedOutput != actualOutput)
                    Interlocked.Increment(ref errors);
            });

            return errors / (double)inputs.Length;
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
        public override HiddenMarkovModel Threshold()
        {
            var models = Classifier.Models;
            var topology = CreateThresholdTopology();

            int states = topology.States;
            int symbols = models[0].Symbols;

            // Create the threshold emission matrix
            double[,] emissions = new double[states, symbols];


            // Then, for each hidden Markov model in the classifier
            for (int i = 0, modelStartIndex = 0; i < models.Length; i++)
            {
                // Retrieve the model definition matrices
                var B = Matrix.Exp(models[i].Emissions);

                // Now, for each state 'j' in the model
                for (int j = 0; j < models[i].States; j++)
                {
                    int stateIndex = j + modelStartIndex;

                    // Copy state emissions from the model
                    emissions.SetRow(stateIndex, B.GetRow(j));
                }

                // Next model starts where this ends
                modelStartIndex += models[i].States;
            }


            // Apply smoothing
            if (smoothingSigma > 0)
            {
                for (int i = 0; i < states; i++)
                {
                    double[] e = emissions.GetRow(i);
                    double[] g = e.Convolve(gaussianKernel, true);
                    g = g.Divide(g.Sum()); // Make probabilities
                    emissions.SetRow(i, g);
                }
            }


            System.Diagnostics.Debug.Assert(!emissions.HasNaN());


            // Create and return the threshold hidden Markov model
            return new HiddenMarkovModel(topology, emissions, logarithm: false)
            {
                Tag = "Non-gesture"
            };
        }


        private void createSmoothingKernel()
        {
            AForge.Math.Gaussian g = new AForge.Math.Gaussian(smoothingSigma);
            gaussianKernel = g.Kernel(smoothingKernelSize);

            // Normalize
            double norm = gaussianKernel.Euclidean();
            gaussianKernel = gaussianKernel.Divide(norm);
        }

    }
}
