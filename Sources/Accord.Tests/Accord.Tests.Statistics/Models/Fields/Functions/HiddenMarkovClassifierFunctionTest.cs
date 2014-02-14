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

namespace Accord.Tests.Statistics.Models.Fields
{
    using Accord.Statistics.Models.Fields.Functions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Models.Fields.Features;
    using Accord.Statistics.Models.Markov.Learning;
    using System;

    /// <summary>
    ///This is a test class for HiddenMarkovModelPotentialFunctionTest and is intended
    ///to contain all HiddenMarkovModelPotentialFunctionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HiddenMarkovClassifierPotentialFunctionTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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


        public static HiddenMarkovClassifier CreateModel1()
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

            return classifier;
        }


        [TestMethod()]
        public void HiddenMarkovHiddenPotentialFunctionConstructorTest()
        {
            HiddenMarkovClassifier model = CreateModel1();

            MarkovDiscreteFunction target = new MarkovDiscreteFunction(model);

            var features = target.Features;
            double[] weights = target.Weights;

            Assert.AreEqual(22, features.Length);
            Assert.AreEqual(22, weights.Length);

            int k = 0;
            for (int c = 0; c < model.Classes; c++)
            {
                Assert.AreEqual(Math.Log(model.Priors[c]), weights[k++]);

                for (int i = 0; i < model[c].States; i++)
                    Assert.AreEqual(model[c].Probabilities[i], weights[k++]);

                for (int i = 0; i < model[c].States; i++)
                    for (int j = 0; j < model[c].States; j++)
                        Assert.AreEqual(model[c].Transitions[i, j], weights[k++]);

                for (int i = 0; i < model[c].States; i++)
                    for (int j = 0; j < model.Symbols; j++)
                        Assert.AreEqual(model[c].Emissions[i, j], weights[k++]);
            }

        }


        [TestMethod()]
        public void ComputeTest()
        {
            HiddenMarkovClassifier model = CreateModel1();

            MarkovDiscreteFunction target = new MarkovDiscreteFunction(model);

            double actual;
            double expected;

            int[] x = { 0, 1 };

            for (int c = 0; c < model.Classes; c++)
            {
                for (int i = 0; i < model[c].States; i++)
                {
                    // Check initial state transitions
                    expected = model.Priors[c] *
                        Math.Exp(model[c].Probabilities[i]) * Math.Exp(model[c].Emissions[i, x[0]]);
                    actual = Math.Exp(target.Factors[c].Compute(-1, i, x, 0, c));
                    Assert.AreEqual(expected, actual, 1e-6);
                    Assert.IsFalse(double.IsNaN(actual));
                }

                for (int t = 1; t < x.Length; t++)
                {
                    // Check normal state transitions
                    for (int i = 0; i < model[c].States; i++)
                    {
                        for (int j = 0; j < model[c].States; j++)
                        {
                            expected = model.Priors[c] * 
                                Math.Exp(model[c].Transitions[i, j]) * Math.Exp(model[c].Emissions[j, x[t]]);
                            actual = Math.Exp(target.Factors[c].Compute(i, j, x, t, c));
                            Assert.AreEqual(expected, actual, 1e-6);
                            Assert.IsFalse(double.IsNaN(actual));
                        }
                    }
                }
            }

        }
    }
}
