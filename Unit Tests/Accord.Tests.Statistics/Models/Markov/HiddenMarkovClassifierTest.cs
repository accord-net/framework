// Accord Unit Tests
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

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Models.Markov;
    using NUnit.Framework;
    using Accord.Statistics.Models.Markov.Learning;
    using Accord.Math;
    using System;
    using Accord.Math.Optimization.Losses;


    [TestFixture]
    public class HiddenMarkovClassifierTest
    {


        [Test]
        public void LearnTest()
        {
            #region doc_learn
            // Declare some testing data
            int[][] inputs = new int[][]
            {
                new int[] { 0,1,2,0 },   // Class 0
                new int[] { 0,0,2,0 },   // Class 0
                new int[] { 0,1,2,1,0 }, // Class 0
                new int[] { 0,1,2,0 },     // Class 0

                new int[] { 1,0,2,1 },   // Class 1
                new int[] { 1,1,2,1 },   // Class 1
                new int[] { 1,0,2,0,1 }, // Class 1
                new int[] { 1,0,2,1 },     // Class 1
            };

            int[] outputs = new int[]
            {
                0,0,0,0, // First four sequences are of class 0
                1,1,1,1, // Last four sequences are of class 1
            };

            // Create a new learning algorithm to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning()
            {
                // Train each model until the log-likelihood changes less than 0.001
                Learner = (i) => new BaumWelchLearning()
                {
                    Tolerance = 0.001,
                    Iterations = 0,
                    NumberOfStates = 2,
                }
            };

            // Train the sequence classifier 
            HiddenMarkovClassifier classifier = teacher.Learn(inputs, outputs);

            // Obtain classification labels for the output
            int[] predicted = classifier.Decide(inputs);

            // Obtain prediction scores for the outputs
            double[] lls = classifier.LogLikelihood(inputs);
            #endregion

            Assert.AreEqual(0, classifier.NumberOfInputs);
            Assert.AreEqual(2, classifier.NumberOfOutputs);
            Assert.AreEqual(2, classifier.NumberOfClasses);
            Assert.AreEqual(3, classifier.NumberOfSymbols);

            for (int i = 0; i < classifier.NumberOfClasses; i++)
            {
                Assert.AreEqual(2, classifier[i].NumberOfStates);
                Assert.AreEqual(3, classifier[i].NumberOfSymbols);
                Assert.AreEqual(1, classifier[i].NumberOfInputs);
                Assert.AreEqual(2, classifier[i].NumberOfOutputs);
            }

            Assert.AreEqual(0.5, classifier.Priors[0]);
            Assert.AreEqual(0.5, classifier.Priors[1]);

            for (int i = 0; i < inputs.Length; i++)
            {
                int expected = outputs[i];
                int actual = predicted[i];
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void LearnTest_old()
        {
            // Declare some testing data
            int[][] inputs = new int[][]
            {
                new int[] { 0,1,1,0 },   // Class 0
                new int[] { 0,0,1,0 },   // Class 0
                new int[] { 0,1,1,1,0 }, // Class 0
                new int[] { 0,1,0 },     // Class 0

                new int[] { 1,0,0,1 },   // Class 1
                new int[] { 1,1,0,1 },   // Class 1
                new int[] { 1,0,0,0,1 }, // Class 1
                new int[] { 1,0,1 },     // Class 1
            };

            int[] outputs = new int[]
            {
                0,0,0,0, // First four sequences are of class 0
                1,1,1,1, // Last four sequences are of class 1
            };


            // We are trying to predict two different classes
            int classes = 2;

            // Each sequence may have up to two symbols (0 or 1)
            int symbols = 2;

            // Nested models will have two states each
            int[] states = new int[] { 2, 2 };

            // Creates a new Hidden Markov Model Classifier with the given parameters
            HiddenMarkovClassifier classifier = new HiddenMarkovClassifier(classes, states, symbols);


            // Create a new learning algorithm to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning(classifier,

                // Train each model until the log-likelihood changes less than 0.001
                modelIndex => new BaumWelchLearning(classifier.Models[modelIndex])
                {
                    Tolerance = 0.001,
                    Iterations = 0
                }
            );

            // Train the sequence classifier 
            teacher.Learn(inputs, outputs);

            // Obtain classification labels for the output
            int[] predicted = classifier.Decide(inputs);

            // Obtain prediction scores for the outputs
            double[] lls = classifier.LogLikelihood(inputs);

            // Will assert the models have learned the sequences correctly.
            for (int i = 0; i < inputs.Length; i++)
            {
                int expected = outputs[i];
                int actual = predicted[i];
                Assert.AreEqual(expected, actual);
            }
        }


        [Test]
        public void LearnTest2()
        {
            #region doc_rejection
            // Declare some testing data
            int[][] inputs = new int[][]
            {
                new int[] { 0,0,1,2 },     // Class 0
                new int[] { 0,1,1,2 },     // Class 0
                new int[] { 0,0,0,1,2 },   // Class 0
                new int[] { 0,1,2,2,2 },   // Class 0

                new int[] { 2,2,1,0 },     // Class 1
                new int[] { 2,2,2,1,0 },   // Class 1
                new int[] { 2,2,2,1,0 },   // Class 1
                new int[] { 2,2,2,2,1 },   // Class 1
            };

            int[] outputs = new int[]
            {
                0,0,0,0, // First four sequences are of class 0
                1,1,1,1, // Last four sequences are of class 1
            };


            // Create a new learning algorithm to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning()
            {
                Learner = (i) => new BaumWelchLearning()
                {
                    NumberOfStates = 3,
                    Tolerance = 0.001,
                    Iterations = 0,
                },

                Rejection = true // Enable support for sequence rejection
            };

            // Train the sequence classifier 
            var classifier = teacher.Learn(inputs, outputs);

            // Obtain prediction classes for the outputs
            int[] prediction = classifier.Decide(inputs);

            // Obtain prediction scores for the outputs
            double[] lls = classifier.LogLikelihood(inputs);
            #endregion

            double likelihood = teacher.LogLikelihood;
            Assert.AreEqual(-24.857860924867815, likelihood, 1e-8);

            Assert.AreEqual(0, classifier.NumberOfInputs);
            Assert.AreEqual(2, classifier.NumberOfOutputs);
            Assert.AreEqual(2, classifier.NumberOfClasses);
            Assert.AreEqual(3, classifier.NumberOfSymbols);

            for (int i = 0; i < classifier.NumberOfClasses; i++)
            {
                Assert.AreEqual(3, classifier[i].NumberOfStates);
                Assert.AreEqual(3, classifier[i].NumberOfSymbols);
                Assert.AreEqual(1, classifier[i].NumberOfInputs);
                Assert.AreEqual(3, classifier[i].NumberOfOutputs);
            }

            Assert.AreEqual(0.5, classifier.Priors[0]);
            Assert.AreEqual(0.5, classifier.Priors[1]);

            likelihood = testThresholdModel(inputs, outputs, classifier, likelihood);
        }

        [Test]
        public void LearnTest2_old()
        {
            #region doc_rejection_old
            // Declare some testing data
            int[][] inputs = new int[][]
            {
                new int[] { 0,0,1,2 },     // Class 0
                new int[] { 0,1,1,2 },     // Class 0
                new int[] { 0,0,0,1,2 },   // Class 0
                new int[] { 0,1,2,2,2 },   // Class 0

                new int[] { 2,2,1,0 },     // Class 1
                new int[] { 2,2,2,1,0 },   // Class 1
                new int[] { 2,2,2,1,0 },   // Class 1
                new int[] { 2,2,2,2,1 },   // Class 1
            };

            int[] outputs = new int[]
            {
                0,0,0,0, // First four sequences are of class 0
                1,1,1,1, // Last four sequences are of class 1
            };


            // We are trying to predict two different classes
            int classes = 2;

            // Each sequence may have up to 3 symbols (0,1,2)
            int symbols = 3;

            // Nested models will have 3 states each
            int[] states = new int[] { 3, 3 };

            // Creates a new Hidden Markov Model Classifier with the given parameters
            HiddenMarkovClassifier classifier = new HiddenMarkovClassifier(classes, states, symbols);


            // Create a new learning algorithm to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning(classifier,

                // Train each model until the log-likelihood changes less than 0.001
                modelIndex => new BaumWelchLearning(classifier.Models[modelIndex])
                {
                    Tolerance = 0.001,
                    Iterations = 0
                }
            );

            // Enable support for sequence rejection
            teacher.Rejection = true;

            // Train the sequence classifier 
            teacher.Learn(inputs, outputs);

            // Obtain prediction classes for the outputs
            int[] prediction = classifier.Decide(inputs);

            // Obtain prediction scores for the outputs
            double[] lls = classifier.LogLikelihood(inputs);
            #endregion

            double likelihood = teacher.LogLikelihood;
            Assert.AreEqual(-24.857860924867815, likelihood, 1e-8);

            likelihood = testThresholdModel(inputs, outputs, classifier, likelihood);
        }

        private static double testThresholdModel(int[][] inputs, int[] outputs, HiddenMarkovClassifier classifier, double likelihood)
        {
            HiddenMarkovModel threshold = classifier.Threshold;

            Assert.AreEqual(6, threshold.States);

            Assert.AreEqual(classifier.Models[0].Transitions[0, 0], threshold.Transitions[0, 0], 1e-10);
            Assert.AreEqual(classifier.Models[0].Transitions[1, 1], threshold.Transitions[1, 1], 1e-10);
            Assert.AreEqual(classifier.Models[0].Transitions[2, 2], threshold.Transitions[2, 2], 1e-10);

            Assert.AreEqual(classifier.Models[1].Transitions[0, 0], threshold.Transitions[3, 3], 1e-10);
            Assert.AreEqual(classifier.Models[1].Transitions[1, 1], threshold.Transitions[4, 4], 1e-10);
            Assert.AreEqual(classifier.Models[1].Transitions[2, 2], threshold.Transitions[5, 5], 1e-10);

            for (int i = 0; i < 3; i++)
                for (int j = 3; j < 6; j++)
                    Assert.AreEqual(Double.NegativeInfinity, threshold.Transitions[i, j]);

            for (int i = 3; i < 6; i++)
                for (int j = 0; j < 3; j++)
                    Assert.AreEqual(Double.NegativeInfinity, threshold.Transitions[i, j]);

            Assert.IsFalse(Matrix.HasNaN(threshold.LogTransitions));

            classifier.Sensitivity = 0.5;

            // Will assert the models have learned the sequences correctly.
            for (int i = 0; i < inputs.Length; i++)
            {
                int expected = outputs[i];
                int actual = classifier.Compute(inputs[i], out likelihood);
                Assert.AreEqual(expected, actual);
            }


            int[] r0 = new int[] { 1, 1, 0, 0, 2 };


            double logRejection;
            int c = classifier.Compute(r0, out logRejection);

            Assert.AreEqual(-1, c);
            Assert.AreEqual(0.99996241769427985, logRejection, 1e-10);

            logRejection = threshold.Evaluate(r0);
            Assert.AreEqual(-5.5993214137039073, logRejection, 1e-10);

            threshold.Decode(r0, out logRejection);
            Assert.AreEqual(-9.31035541707617, logRejection, 1e-10);

            foreach (var model in classifier.Models)
            {
                double[,] A = model.Transitions;

                for (int i = 0; i < A.GetLength(0); i++)
                {
                    double[] row = A.Exp().GetRow(i);
                    double sum = row.Sum();
                    Assert.AreEqual(1, sum, 1e-10);
                }
            }
            {
                double[,] A = classifier.Threshold.Transitions;

                for (int i = 0; i < A.GetLength(0); i++)
                {
                    double[] row = A.GetRow(i);
                    double sum = row.Exp().Sum();
                    Assert.AreEqual(1, sum, 1e-6);
                }
            }
            return likelihood;
        }
    }
}
