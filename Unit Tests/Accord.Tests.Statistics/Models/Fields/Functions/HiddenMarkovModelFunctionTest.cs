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
    using Accord.Statistics.Models.Markov.Topology;
    using Accord.Statistics.Models.Markov.Learning;
    using System;

    /// <summary>
    ///This is a test class for HiddenMarkovModelPotentialFunctionTest and is intended
    ///to contain all HiddenMarkovModelPotentialFunctionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HiddenMarkovModelFunctionTest
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


        public static HiddenMarkovModel CreateModel1()
        {
            double[] initial = { 1.0, 0.0 };

            double[,] transitions = 
            {
                { 0.33, 0.66 },
                { 0.00, 1.00 },

            };

            double[,] emissions =
            {
                { 0.25, 0.25, 0.50 },
                { 0.05, 0.05, 0.90 }
            };

            HiddenMarkovModel model = new HiddenMarkovModel(transitions, emissions, initial);
            return model;
        }

        public static HiddenMarkovModel CreateModel2()
        {
            int states = 3;
            int symbols = 3;

            int[][] sequences = new int[][] 
            {
                new int[] { 0, 1, 1, 1, 2 },
                new int[] { 0, 1, 1, 1, 2, 2, 2 },
                new int[] { 0, 0, 1, 1, 2, 2 },
                new int[] { 0, 1, 1, 1, 2, 2, 2 },
                new int[] { 0, 1, 1, 1, 2, 1 },
                new int[] { 0, 1, 1, 2, 2 },
                new int[] { 0, 0, 1, 1, 1, 2, 1 },
                new int[] { 0, 0, 0, 1, 1, 1, 2, 1 },
                new int[] { 0, 1, 1, 2, 2, 2 },
            };

            HiddenMarkovModel hmm = new HiddenMarkovModel(new Forward(states), symbols);

            var teacher = new BaumWelchLearning(hmm) { Iterations = 100, Tolerance = 0 };

            double ll = teacher.Run(sequences);

            return hmm;
        }

        [TestMethod()]
        public void HiddenMarkovModelFunctionConstructorTest()
        {
            HiddenMarkovModel model = CreateModel1();

            MarkovDiscreteFunction target = new MarkovDiscreteFunction(model);

            var features = target.Features;
            double[] weights = target.Weights;

            Assert.AreEqual(features.Length, 12);
            Assert.AreEqual(weights.Length, 12);

            int k = 0;

            for (int i = 0; i < model.States; i++)
                Assert.AreEqual(model.Probabilities[i], weights[k++]);

            for (int i = 0; i < model.States; i++)
                for (int j = 0; j < model.States; j++)
                    Assert.AreEqual(model.Transitions[i, j], weights[k++]);

            for (int i = 0; i < model.States; i++)
                for (int j = 0; j < model.Symbols; j++)
                    Assert.AreEqual(model.Emissions[i, j], weights[k++]);
        }


        [TestMethod()]
        public void ComputeTest()
        {
            HiddenMarkovModel model = CreateModel1();

            MarkovDiscreteFunction target = new MarkovDiscreteFunction(model);

            double actual;
            double expected;

            int[] x = { 0, 1 };


            for (int i = 0; i < model.States; i++)
            {
                // Check initial state transitions
                expected = Math.Exp(model.Probabilities[i]) * Math.Exp(model.Emissions[i, x[0]]);
                actual = Math.Exp(target.Factors[0].Compute(-1, i, x, 0));
                Assert.AreEqual(expected, actual, 1e-6);
            }

            for (int t = 0; t < x.Length; t++)
            {
                for (int i = 0; i < model.States; i++)
                {
                    // Check initial state transitions
                    expected = Math.Exp(model.Probabilities[i]) * Math.Exp(model.Emissions[i, x[0]]);
                    actual = Math.Exp(target.Factors[0].Compute(-1, i, x, 0));
                    Assert.AreEqual(expected, actual, 1e-6);

                    // Check normal state transitions
                    for (int j = 0; j < model.States; j++)
                    {
                        expected = Math.Exp(model.Transitions[i, j]) * Math.Exp(model.Emissions[j, x[t]]);
                        actual = Math.Exp(target.Factors[0].Compute(i, j, x, t));
                        Assert.AreEqual(expected, actual, 1e-6);
                    }
                }
            }

        }
    }
}
