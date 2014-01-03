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
    using System;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Models.Fields.Functions;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Models.Markov.Learning;
    using Accord.Statistics.Models.Markov.Topology;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class NormalHiddenMarkovClassifierPotentialFunctionTest
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


        public static HiddenMarkovClassifier<NormalDistribution> CreateModel1()
        {
            // Create a Continuous density Hidden Markov Model Sequence Classifier
            // to detect a univariate sequence and the same sequence backwards.
            double[][] sequences = new double[][] 
            {
                new double[] { 0,1,2,3,4 }, // This is the first  sequence with label = 0
                new double[] { 4,3,2,1,0 }, // This is the second sequence with label = 1
            };

            // Labels for the sequences
            int[] labels = { 0, 1 };

            // Creates a sequence classifier containing 2 hidden Markov Models
            //  with 2 states and an underlying Normal distribution as density.
            NormalDistribution density = new NormalDistribution();
            var classifier = new HiddenMarkovClassifier<NormalDistribution>(2, new Ergodic(2), density);

            // Configure the learning algorithms to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning<NormalDistribution>(classifier,

                // Train each model until the log-likelihood changes less than 0.001
                modelIndex => new BaumWelchLearning<NormalDistribution>(classifier.Models[modelIndex])
                {
                    Tolerance = 0.0001,
                    Iterations = 0
                }
            );

            // Train the sequence classifier using the algorithm
            double logLikelihood = teacher.Run(sequences, labels);


            return classifier;
        }


        [TestMethod()]
        public void HiddenMarkovHiddenPotentialFunctionConstructorTest()
        {
            HiddenMarkovClassifier<NormalDistribution> model = CreateModel1();

            var target = new MarkovContinuousFunction(model);

            var features = target.Features;
            double[] weights = target.Weights;

            Assert.AreEqual(26, features.Length);
            Assert.AreEqual(26, weights.Length);

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
                {
                    for (int j = 0; j < model[c].Dimension; j++)
                    {
                        double mean = model[c].Emissions[i].Mean;
                        double var = model[c].Emissions[i].Variance;

                        double l2ps = System.Math.Log(2 * System.Math.PI * var);

                        Assert.AreEqual(-0.5 * (l2ps + (mean * mean) / var), weights[k++]);
                        Assert.AreEqual(mean / var, weights[k++]);
                        Assert.AreEqual(-1.0 / (2 * var), weights[k++]);
                    }
                }
            }

        }


        [TestMethod()]
        public void ComputeTest()
        {
            var model = CreateModel1();

            var target = new MarkovContinuousFunction(model);

            double actual;
            double expected;

            double[] x = { 0, 1 };

            for (int c = 0; c < model.Classes; c++)
            {
                for (int i = 0; i < model[c].States; i++)
                {
                    // Check initial state transitions
                    expected = model.Priors[c] * Math.Exp(model[c].Probabilities[i]) * model[c].Emissions[i].ProbabilityDensityFunction(x[0]);
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
                            expected = model.Priors[c] * Math.Exp(model[c].Transitions[i, j]) * model[c].Emissions[j].ProbabilityDensityFunction(x[t]);
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
