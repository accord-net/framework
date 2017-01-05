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

namespace Accord.Tests.Statistics.Models.Fields
{
    using Accord.Statistics.Models.Fields;
    using NUnit.Framework;
    using Accord.Statistics.Models.Fields.Functions;
    using Accord.Statistics.Models.Markov;
    using Accord.Math;
    using System;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Models.Markov.Topology;

    [TestFixture]
    public class IndependentForwardBackwardAlgorithmTest
    {


        [Test]
        public void ForwardTest()
        {
            double[][][] observations;
            int[] labels;

            var hmm = IndependentMarkovFunctionTest.CreateModel2(out observations, out labels);

            var function = new MarkovMultivariateFunction(hmm, includePriors: false);


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

        [Test]
        public void LogForwardTest()
        {
            double[][][] observations;
            int[] labels;
            var hmm = IndependentMarkovFunctionTest.CreateModel2(out observations, out labels);

            MarkovMultivariateFunction function = new MarkovMultivariateFunction(hmm, includePriors: false);

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

        [Test]
        public void LogBackwardTest()
        {
            double[][][] observations;
            int[] labels;
            var hmm = IndependentMarkovFunctionTest.CreateModel2(out observations, out labels);

            MarkovMultivariateFunction function = new MarkovMultivariateFunction(hmm);


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


        [Test]
        public void ForwardTest2()
        {
            double[][][] observations;
            int[] labels;

            var hmm = IndependentMarkovFunctionTest.CreateModel3(out observations, out labels);

            var function = new MarkovMultivariateFunction(hmm, includePriors: false);


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
                        Assert.IsTrue(scaling1[i].IsRelativelyEqual(scaling2[i], 1e-10));
                        Assert.IsFalse(Double.IsNaN(scaling1[i]));
                        Assert.IsFalse(Double.IsNaN(scaling2[i]));
                    }
                }
            }
        }

        [Test]
        public void LogForwardTest2()
        {
            double[][][] observations;
            int[] labels;

            var hmm = IndependentMarkovFunctionTest.CreateModel3(out observations, out labels);

            MarkovMultivariateFunction function = new MarkovMultivariateFunction(hmm, includePriors: false);


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

        [Test]
        public void LogBackwardTest2()
        {
            double[][][] observations;
            int[] labels;

            var hmm = IndependentMarkovFunctionTest.CreateModel3(out observations, out labels);

            MarkovMultivariateFunction function = new MarkovMultivariateFunction(hmm);


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

        [Test]
        public void LogForwardGesturesTest()
        {
            int[] labels;
            double[][][] words;
            var classifier = IndependentMarkovFunctionTest.CreateModel4(out words, out labels, false);

            var function = new MarkovMultivariateFunction(classifier, includePriors: false);
            var target = new HiddenConditionalRandomField<double[]>(function);

            foreach (var word in words)
            {
                for (int c = 0; c < 3; c++)
                {
                    var actual = Accord.Statistics.Models.Fields.ForwardBackwardAlgorithm.LogForward(
                        target.Function.Factors[c], word, c);

                    var expected = Accord.Statistics.Models.Markov.ForwardBackwardAlgorithm.LogForward(
                        classifier[c], word);

                    for (int i = 0; i < actual.GetLength(0); i++)
                    {
                        for (int j = 0; j < actual.GetLength(1); j++)
                        {
                            double a = actual[i, j];
                            double e = expected[i, j];

                            Assert.IsTrue(e.IsRelativelyEqual(a, 1e-10)); 
                        }
                    }
                }
            }
        }

        [Test]
        public void LogForwardGesturesPriorsTest()
        {
            int[] labels;
            double[][][] words;
            var classifier = IndependentMarkovFunctionTest.CreateModel4(out words, out labels, true);

            var function = new MarkovMultivariateFunction(classifier);
            var target = new HiddenConditionalRandomField<double[]>(function);

            foreach (var word in words)
            {
                for (int c = 0; c < 3; c++)
                {
                    var actual = Accord.Statistics.Models.Fields.ForwardBackwardAlgorithm.LogForward(
                        target.Function.Factors[c], word, c);

                    var expected = Accord.Statistics.Models.Markov.ForwardBackwardAlgorithm.LogForward(
                        classifier[c], word);

                    for (int i = 0; i < actual.GetLength(0); i++)
                    {
                        for (int j = 0; j < actual.GetLength(1); j++)
                        {
                            double a = actual[i, j];
                            double e = expected[i, j];

                            // TODO: Verify if is possible to reduce this tolerance
                            Assert.IsTrue(e.IsRelativelyEqual(a, 0.1));
                        }
                    }
                }
            }
        }


#pragma warning disable 0618
        [Test]
        public void LogForwardGesturesDeoptimizedTest()
        {
            int[] labels;
            double[][][] words;
            var classifier = IndependentMarkovFunctionTest.CreateModel4(out words, out labels, false);

            var function = new MarkovMultivariateFunction(classifier);

            function.Deoptimize();

            var target = new HiddenConditionalRandomField<double[]>(function);

            foreach (var word in words)
            {
                for (int c = 0; c < 3; c++)
                {
                    var actual = Accord.Statistics.Models.Fields.ForwardBackwardAlgorithm.LogForward(
                        target.Function.Factors[c], word, c);

                    var expected = Accord.Statistics.Models.Markov.ForwardBackwardAlgorithm.LogForward(
                        classifier[c], word);

                    for (int i = 0; i < actual.GetLength(0); i++)
                    {
                        for (int j = 0; j < actual.GetLength(1); j++)
                        {
                            double a = actual[i, j];
                            double e = expected[i, j];
                            Assert.IsTrue(e.IsRelativelyEqual(a, 0.1));
                        }
                    }
                }
            }
        }

        [Test]
        public void LogForwardGesturesPriorsDeoptimizedTest()
        {
            int[] labels;
            double[][][] words;
            var classifier = IndependentMarkovFunctionTest.CreateModel4(out words, out labels, true);

            var deopFun = new MarkovMultivariateFunction(classifier);
            deopFun.Deoptimize();
            var target1 = new HiddenConditionalRandomField<double[]>(deopFun);

            var function = new MarkovMultivariateFunction(classifier);
            var target2 = new HiddenConditionalRandomField<double[]>(function);

            foreach (var word in words)
            {
                for (int c = 0; c < 3; c++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        var actual = Accord.Statistics.Models.Fields.ForwardBackwardAlgorithm
                            .LogForward(target1.Function.Factors[c], word, y);

                        var expected = Accord.Statistics.Models.Fields.ForwardBackwardAlgorithm
                            .LogForward(target2.Function.Factors[c], word, y);

                        for (int i = 0; i < actual.GetLength(0); i++)
                        {
                            for (int j = 0; j < actual.GetLength(1); j++)
                            {
                                double a = actual[i, j];
                                double e = expected[i, j];
                                Assert.IsTrue(e.IsRelativelyEqual(a, 0.1));
                            }

                        }
                    }
                }
            }
        }

#pragma warning restore 0618
    }
}
