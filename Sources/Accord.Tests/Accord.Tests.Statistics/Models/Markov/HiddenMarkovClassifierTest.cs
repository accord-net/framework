// Accord Unit Tests
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

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Models.Markov;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Statistics.Models.Markov.Learning;
    using Accord.Math;
    using System;


    [TestClass()]
    public class HiddenMarkovClassifierTest
    {


        private TestContext testContextInstance;


        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }



        [TestMethod()]
        public void LearnTest()
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

            // Train the sequence classifier using the algorithm
            double likelihood = teacher.Run(inputs, outputs);


            // Will assert the models have learned the sequences correctly.
            for (int i = 0; i < inputs.Length; i++)
            {
                int expected = outputs[i];
                int actual = classifier.Compute(inputs[i], out likelihood);
                Assert.AreEqual(expected, actual);
            }
        }


        [TestMethod()]
        public void LearnTest2()
        {
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

            // Train the sequence classifier using the algorithm
            double likelihood = teacher.Run(inputs, outputs);

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

            Assert.IsFalse(Matrix.HasNaN(threshold.Transitions));

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
            Assert.AreEqual(0.99906957195279988, logRejection);
            Assert.IsFalse(double.IsNaN(logRejection));

            logRejection = threshold.Evaluate(r0);
            Assert.AreEqual(-4.5653702970734793, logRejection, 1e-10);
            Assert.IsFalse(double.IsNaN(logRejection));

            threshold.Decode(r0, out logRejection);
            Assert.AreEqual(-8.21169955167614, logRejection, 1e-10);
            Assert.IsFalse(double.IsNaN(logRejection));

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
        }
    }
}
