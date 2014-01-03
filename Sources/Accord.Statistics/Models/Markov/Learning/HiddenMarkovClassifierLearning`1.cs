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
    ///   
    ///   <code>
    ///   // Create a Continuous density Hidden Markov Model Sequence Classifier
    ///   // to detect a univariate sequence and the same sequence backwards.
    ///   double[][] sequences = new double[][] 
    ///   {
    ///       new double[] { 0,1,2,3,4 }, // This is the first  sequence with label = 0
    ///       new double[] { 4,3,2,1,0 }, // This is the second sequence with label = 1
    ///   };
    ///   
    ///   // Labels for the sequences
    ///   int[] labels = { 0, 1 };
    ///
    ///   // Creates a new Continuous-density Hidden Markov Model Sequence Classifier
    ///   //  containing 2 hidden Markov Models with 2 states and an underlying Normal
    ///   //  distribution as the continuous probability density.
    ///   NormalDistribution density = new NormalDistribution();
    ///   var classifier = new HiddenMarkovClassifier&lt;NormalDistribution&gt;(2, new Ergodic(2), density);
    ///
    ///   // Create a new learning algorithm to train the sequence classifier
    ///   var teacher = new HiddenMarkovClassifierLearning&lt;NormalDistribution&gt;(classifier,
    ///
    ///       // Train each model until the log-likelihood changes less than 0.001
    ///       modelIndex => new BaumWelchLearning&lt;NormalDistribution&gt;(classifier.Models[modelIndex])
    ///       {
    ///           Tolerance = 0.0001,
    ///           Iterations = 0
    ///       }
    ///   );
    ///   
    ///   // Train the sequence classifier using the algorithm
    ///   teacher.Run(sequences, labels);
    ///   
    ///   
    ///   // Calculate the probability that the given
    ///   //  sequences originated from the model
    ///   double likelihood;
    ///   
    ///   // Try to classify the first sequence (output should be 0)
    ///   int c1 = classifier.Compute(sequences[0], out likelihood);
    ///   
    ///   // Try to classify the second sequence (output should be 1)
    ///   int c2 = classifier.Compute(sequences[1], out likelihood);
    ///   </code>
    ///   
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
    ///   
    ///   <code>
    ///   // Create a Continuous density Hidden Markov Model Sequence Classifier
    ///   // to detect a multivariate sequence and the same sequence backwards.
    ///   
    ///   double[][][] sequences = new double[][][]
    ///   {
    ///       new double[][] 
    ///       { 
    ///           // This is the first  sequence with label = 0
    ///           new double[] { 0, 1 },
    ///           new double[] { 1, 2 },
    ///           new double[] { 2, 3 },
    ///           new double[] { 3, 4 },
    ///           new double[] { 4, 5 },
    ///       }, 
    ///   
    ///       new double[][]
    ///       {
    ///               // This is the second sequence with label = 1
    ///           new double[] { 4,  3 },
    ///           new double[] { 3,  2 },
    ///           new double[] { 2,  1 },
    ///           new double[] { 1,  0 },
    ///           new double[] { 0, -1 },
    ///       }
    ///   };
    ///   
    ///   // Labels for the sequences
    ///   int[] labels = { 0, 1 };
    ///   
    ///   
    ///   var initialDensity = new MultivariateNormalDistribution(2);
    ///   
    ///   // Creates a sequence classifier containing 2 hidden Markov Models with 2 states
    ///   // and an underlying multivariate mixture of Normal distributions as density.
    ///   var classifier = new HiddenMarkovClassifier&lt;MultivariateNormalDistribution>(
    ///       classes: 2, topology: new Forward(2), initial: initialDensity);
    ///   
    ///   // Configure the learning algorithms to train the sequence classifier
    ///   var teacher = new HiddenMarkovClassifierLearning&lt;MultivariateNormalDistribution>(
    ///       classifier,
    ///   
    ///       // Train each model until the log-likelihood changes less than 0.0001
    ///       modelIndex => new BaumWelchLearning&lt;MultivariateNormalDistribution>(
    ///           classifier.Models[modelIndex])
    ///       {
    ///           Tolerance = 0.0001,
    ///           Iterations = 0,
    ///   
    ///           FittingOptions = new NormalOptions()
    ///           {
    ///               Diagonal = true,      // only diagonal covariance matrices
    ///               Regularization = 1e-5 // avoid non-positive definite errors
    ///           }
    ///       }
    ///   );
    ///   
    ///   // Train the sequence classifier using the algorithm
    ///   double logLikelihood = teacher.Run(sequences, labels);
    ///   
    ///   
    ///   // Calculate the probability that the given
    ///   //  sequences originated from the model
    ///   double likelihood, likelihood2;
    ///   
    ///   // Try to classify the 1st sequence (output should be 0)
    ///   int c1 = classifier.Compute(sequences[0], out likelihood);
    ///   
    ///   // Try to classify the 2nd sequence (output should be 1)
    ///   int c2 = classifier.Compute(sequences[1], out likelihood2);
    ///   </code>
    /// </example>
    /// 
    /// <see cref="HiddenMarkovClassifierLearning"/>
    /// <see cref="HiddenMarkovClassifier{TDistribution}"/>
    /// 
    public class HiddenMarkovClassifierLearning<TDistribution> :
        BaseHiddenMarkovClassifierLearning<HiddenMarkovClassifier<TDistribution>,
        HiddenMarkovModel<TDistribution>>
        where TDistribution : IDistribution
    {


        /// <summary>
        ///   Creates a new instance of the learning algorithm for a given 
        ///   Markov sequence classifier using the specified configuration
        ///   function.
        /// </summary>
        /// 
        public HiddenMarkovClassifierLearning(HiddenMarkovClassifier<TDistribution> classifier,
            ClassifierLearningAlgorithmConfiguration algorithm)
            : base(classifier, algorithm) { }


        /// <summary>
        ///   Trains each model to recognize each of the output labels.
        /// </summary>
        /// 
        /// <returns>The sum log-likelihood for all models after training.</returns>
        /// 
        public double Run(Array[] inputs, int[] outputs)
        {
            return base.Run<Array>(inputs, outputs);
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
        public double ComputeError(Array[] inputs, int[] outputs)
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
        public override HiddenMarkovModel<TDistribution> Threshold()
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
            return new HiddenMarkovModel<TDistribution>(topology, emissions)
            { 
                Tag = "Non-gesture"
            };
        }

    }
}
