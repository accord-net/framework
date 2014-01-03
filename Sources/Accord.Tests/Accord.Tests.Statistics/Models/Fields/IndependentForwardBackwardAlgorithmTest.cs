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
    using Accord.Statistics.Models.Fields;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Statistics.Models.Fields.Functions;
    using Accord.Statistics.Models.Markov;
    using Accord.Math;
    using System;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Models.Markov.Topology;

    [TestClass()]
    public class IndependentForwardBackwardAlgorithmTest
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
        public void ForwardTest()
        {
            HiddenMarkovClassifier<Independent> hmm = IndependentMarkovClassifierPotentialFunctionTest
                .CreateModel2();

            var function = new MarkovMultivariateFunction(hmm, includePriors: false);
            double[][][] observations = IndependentMarkovClassifierPotentialFunctionTest.sequences;
            int[] labels = IndependentMarkovClassifierPotentialFunctionTest.labels;

            foreach (double[][] x in observations)
            {
                foreach (int y in labels)
                {
                    double[] scaling1;
                    double logLikelihood1;

                    double[,] actual = Accord.Statistics.Models.Fields.
                        ForwardBackwardAlgorithm.Forward(function.Factors[y], x, y, out scaling1, out logLikelihood1);

                    double[] scaling2;
                    double logLikelihood2;
                    double[,] expected = Accord.Statistics.Models.Markov.
                        ForwardBackwardAlgorithm.Forward(hmm.Models[y], x, out scaling2, out logLikelihood2);

                    for (int i = 0; i < actual.GetLength(0); i++)
                        for (int j = 0; j < actual.GetLength(1); j++)
                        {
                            Assert.AreEqual(expected[i, j], actual[i, j], 1e-10);
                            Assert.IsFalse(Double.IsNaN(actual[i, j]));
                        }

                    Assert.AreEqual(logLikelihood1, logLikelihood2, 1e-10);

                    for (int i = 0; i < scaling1.Length; i++)
                        Assert.AreEqual(scaling1[i], scaling2[i], 1e-10);
                }
            }
        }

        [TestMethod()]
        public void LogForwardTest()
        {
            HiddenMarkovClassifier<Independent> hmm = IndependentMarkovClassifierPotentialFunctionTest
                .CreateModel2();

            MarkovMultivariateFunction function = new MarkovMultivariateFunction(hmm, includePriors: false);

            double[][][] observations = IndependentMarkovClassifierPotentialFunctionTest.sequences;
            int[] labels = IndependentMarkovClassifierPotentialFunctionTest.labels;

            foreach (double[][] x in observations)
            {
                foreach (int y in labels)
                {
                    double[,] actual = new double[5, 2];
                    Accord.Statistics.Models.Fields.
                        ForwardBackwardAlgorithm.LogForward(function.Factors[y], x, y, actual);

                    double[,] expected = new double[5, 2];
                    Accord.Statistics.Models.Markov.
                        ForwardBackwardAlgorithm.LogForward(hmm.Models[y], x, expected);

                    for (int i = 0; i < actual.GetLength(0); i++)
                        for (int j = 0; j < actual.GetLength(1); j++)
                            Assert.AreEqual(expected[i, j], actual[i, j], 1e-10);
                }
            }
        }

        [TestMethod()]
        public void LogBackwardTest()
        {
            HiddenMarkovClassifier<Independent> hmm = IndependentMarkovClassifierPotentialFunctionTest
                .CreateModel2();

            MarkovMultivariateFunction function = new MarkovMultivariateFunction(hmm);

            double[][][] observations = IndependentMarkovClassifierPotentialFunctionTest.sequences;
            int[] labels = IndependentMarkovClassifierPotentialFunctionTest.labels;

            foreach (double[][] x in observations)
            {
                foreach (int y in labels)
                {
                    double[,] actual = new double[5, 2];
                    Accord.Statistics.Models.Fields.
                        ForwardBackwardAlgorithm.LogBackward(function.Factors[y], x, y, actual);

                    double[,] expected = new double[5, 2];
                    Accord.Statistics.Models.Markov.
                        ForwardBackwardAlgorithm.LogBackward(hmm.Models[y], x, expected);

                    for (int i = 0; i < actual.GetLength(0); i++)
                        for (int j = 0; j < actual.GetLength(1); j++)
                            Assert.AreEqual(expected[i, j], actual[i, j], 1e-10);
                }
            }
        }


        [TestMethod()]
        public void ForwardTest2()
        {
            HiddenMarkovClassifier<Independent> hmm = IndependentMarkovClassifierPotentialFunctionTest
                .CreateModel3();

            var function = new MarkovMultivariateFunction(hmm, includePriors: false);
            double[][][] observations = IndependentMarkovClassifierPotentialFunctionTest.sequences2;
            int[] labels = IndependentMarkovClassifierPotentialFunctionTest.labels2;

            foreach (double[][] x in observations)
            {
                foreach (int y in labels)
                {
                    double[] scaling1;
                    double logLikelihood1;

                    double[,] actual = Accord.Statistics.Models.Fields.
                        ForwardBackwardAlgorithm.Forward(function.Factors[y], x, y, out scaling1, out logLikelihood1);

                    double[] scaling2;
                    double logLikelihood2;
                    double[,] expected = Accord.Statistics.Models.Markov.
                        ForwardBackwardAlgorithm.Forward(hmm.Models[y], x, out scaling2, out logLikelihood2);

                    for (int i = 0; i < actual.GetLength(0); i++)
                        for (int j = 0; j < actual.GetLength(1); j++)
                        {
                            Assert.AreEqual(expected[i, j], actual[i, j], 1e-10);
                            Assert.IsFalse(Double.IsNaN(actual[i, j]));
                        }

                    Assert.AreEqual(logLikelihood1, logLikelihood2, 1e-10);

                    for (int i = 0; i < scaling1.Length; i++)
                    {
                        Assert.AreEqual(scaling1[i], scaling2[i], 1e-8);
                        Assert.IsFalse(Double.IsNaN(scaling1[i]));
                        Assert.IsFalse(Double.IsNaN(scaling2[i]));
                    }
                }
            }
        }

        [TestMethod()]
        public void LogForwardTest2()
        {
            HiddenMarkovClassifier<Independent> hmm = IndependentMarkovClassifierPotentialFunctionTest
                .CreateModel3();

            MarkovMultivariateFunction function = new MarkovMultivariateFunction(hmm, includePriors: false);

            double[][][] observations = IndependentMarkovClassifierPotentialFunctionTest.sequences2;
            int[] labels = IndependentMarkovClassifierPotentialFunctionTest.labels2;

            foreach (double[][] x in observations)
            {
                foreach (int y in labels)
                {
                    double[,] actual = new double[x.Length, 5];
                    Accord.Statistics.Models.Fields.
                        ForwardBackwardAlgorithm.LogForward(function.Factors[y], x, y, actual);

                    double[,] expected = new double[x.Length, 5];
                    Accord.Statistics.Models.Markov.
                        ForwardBackwardAlgorithm.LogForward(hmm.Models[y], x, expected);

                    for (int i = 0; i < actual.GetLength(0); i++)
                        for (int j = 0; j < actual.GetLength(1); j++)
                        {
                            Assert.AreEqual(expected[i, j], actual[i, j], 1e-9);
                            Assert.IsFalse(Double.IsNaN(actual[i, j]));
                        }
                }
            }
        }

        [TestMethod()]
        public void LogBackwardTest2()
        {
            HiddenMarkovClassifier<Independent> hmm = IndependentMarkovClassifierPotentialFunctionTest
                .CreateModel3();

            MarkovMultivariateFunction function = new MarkovMultivariateFunction(hmm);

            double[][][] observations = IndependentMarkovClassifierPotentialFunctionTest.sequences2;
            int[] labels = IndependentMarkovClassifierPotentialFunctionTest.labels2;

            foreach (double[][] x in observations)
            {
                foreach (int y in labels)
                {
                    double[,] actual = new double[x.Length, 5];
                    Accord.Statistics.Models.Fields.
                        ForwardBackwardAlgorithm.LogBackward(function.Factors[y], x, y, actual);

                    double[,] expected = new double[x.Length, 5];
                    Accord.Statistics.Models.Markov.
                        ForwardBackwardAlgorithm.LogBackward(hmm.Models[y], x, expected);

                    for (int i = 0; i < actual.GetLength(0); i++)
                        for (int j = 0; j < actual.GetLength(1); j++)
                        {
                            Assert.AreEqual(expected[i, j], actual[i, j], 1e-10);
                            Assert.IsFalse(Double.IsNaN(actual[i, j]));
                        }
                }
            }
        }


    }
}
