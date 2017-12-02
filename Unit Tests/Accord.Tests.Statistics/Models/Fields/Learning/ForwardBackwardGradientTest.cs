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
    using System;
    using Accord.Math.Differentiation;
    using Accord.Statistics.Models.Fields;
    using Accord.Statistics.Models.Fields.Functions;
    using Accord.Statistics.Models.Fields.Learning;
    using Accord.Statistics.Models.Markov;
    using NUnit.Framework;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Models.Markov.Topology;

    [TestFixture]
    public class ForwardBackwardLearningTest
    {

        #region Discrete

        [Test]
        public void GradientTest_DiscreteMarkov()
        {
            var function = new MarkovDiscreteFunction(2, 2, 2);
            var model = new HiddenConditionalRandomField<int>(function);
            var target = new ForwardBackwardGradient<int>(model);

            FiniteDifferences diff = new FiniteDifferences(function.Weights.Length)
            {
                StepSize = 1e-5
            };

            var inputs = QuasiNewtonHiddenLearningTest.inputs;
            var outputs = QuasiNewtonHiddenLearningTest.outputs;

            diff.Function = parameters => func(model, parameters, inputs, outputs);

            double[] expected = diff.Compute(function.Weights);
            double[] actual = target.Gradient(function.Weights, inputs, outputs);


            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 1e-4);
            }
        }

        [Test]
        public void GradientTest_DiscreteMarkov2()
        {
            HiddenMarkovClassifier hmm = DiscreteHiddenMarkovClassifierPotentialFunctionTest.CreateModel1();
            var function = new MarkovDiscreteFunction(hmm);

            var model = new HiddenConditionalRandomField<int>(function);
            var target = new ForwardBackwardGradient<int>(model);

            FiniteDifferences diff = new FiniteDifferences(function.Weights.Length);

            var inputs = QuasiNewtonHiddenLearningTest.inputs;
            var outputs = QuasiNewtonHiddenLearningTest.outputs;

            diff.Function = parameters => func(model, parameters, inputs, outputs);

            double[] expected = diff.Compute(function.Weights);
            double[] actual = target.Gradient(function.Weights, inputs, outputs);

            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected[i], actual[i], 1e-4);
        }

        [Test]
        public void GradientTest_DiscreteMarkov3()
        {
            HiddenMarkovClassifier hmm = DiscreteHiddenMarkovClassifierPotentialFunctionTest.CreateModel1();
            var function = new MarkovDiscreteFunction(hmm);

            var model = new HiddenConditionalRandomField<int>(function);
            var target = new ForwardBackwardGradient<int>(model);
            target.Regularization = 2;

            FiniteDifferences diff = new FiniteDifferences(function.Weights.Length);

            var inputs = QuasiNewtonHiddenLearningTest.inputs;
            var outputs = QuasiNewtonHiddenLearningTest.outputs;

            diff.Function = parameters => func(model, parameters, inputs, outputs, target.Regularization);

            double[] expected = diff.Compute(function.Weights);
            double[] actual = target.Gradient(function.Weights, inputs, outputs);

            for (int i = 0; i < actual.Length; i++)
                Assert.AreEqual(expected[i], actual[i], 1e-4);
        }



        #endregion

        #region Markov Independent

        [Test]
        public void GradientTest_MarkovIndependent()
        {
            double[][][] observations;
            int[] labels;
            HiddenMarkovClassifier<Independent> hmm =
                IndependentMarkovFunctionTest.CreateModel2(out observations, out labels);

            var function = new MarkovMultivariateFunction(hmm);

            var model = new HiddenConditionalRandomField<double[]>(function);
            var target = new ForwardBackwardGradient<double[]>(model);
            target.Regularization = 0;

            FiniteDifferences diff = new FiniteDifferences(function.Weights.Length);

            diff.Function = parameters => func(model, parameters,
                observations,
                labels);

            double[] expected = diff.Compute(function.Weights);
            double[] actual = target.Gradient(function.Weights, observations, labels);


            for (int i = 0; i < actual.Length; i++)
            {
                if (double.IsNaN(expected[i]))
                    continue;

                Assert.AreEqual(expected[i], actual[i], 1e-5);
                Assert.IsFalse(double.IsNaN(actual[i]));
            }
        }

        [Test]
        public void GradientTest_MarkovIndependentNormal_Priors()
        {
            double[][][] observations;
            int[] labels;
            HiddenMarkovClassifier<Independent<NormalDistribution>> hmm =
                IndependentMarkovFunctionTest.CreateModel4(out observations, out labels, usePriors: true);

            var function = new MarkovMultivariateFunction(hmm);

            var model = new HiddenConditionalRandomField<double[]>(function);
            var target = new ForwardBackwardGradient<double[]>(model);
            target.Regularization = 0;

            FiniteDifferences diff = new FiniteDifferences(function.Weights.Length);

            diff.Function = parameters => func(model, parameters,
                observations,
                labels);

            double[] expected = diff.Compute(function.Weights);
            double[] actual = target.Gradient(function.Weights, observations, labels);


            for (int i = 0; i < actual.Length; i++)
            {
                if (double.IsNaN(expected[i]))
                    continue;

                Assert.AreEqual(expected[i], actual[i], 1e-5);
                Assert.IsFalse(double.IsNaN(actual[i]));
            }
        }

        [Test]
        public void GradientTest_MarkovIndependentNormal_NoPriors()
        {
            double[][][] observations;
            int[] labels;
            HiddenMarkovClassifier<Independent<NormalDistribution>> hmm =
                IndependentMarkovFunctionTest.CreateModel4(out observations, out labels, usePriors: false);

            var function = new MarkovMultivariateFunction(hmm);

            var model = new HiddenConditionalRandomField<double[]>(function);
            var target = new ForwardBackwardGradient<double[]>(model);
            target.Regularization = 0;

            FiniteDifferences diff = new FiniteDifferences(function.Weights.Length);

            diff.Function = parameters => func(model, parameters,
                observations,
                labels);

            double[] expected = diff.Compute(function.Weights);
            double[] actual = target.Gradient(function.Weights, observations, labels);


            for (int i = 0; i < actual.Length; i++)
            {
                if (double.IsNaN(expected[i]))
                    continue;

                Assert.AreEqual(expected[i], actual[i], 1e-5);
                Assert.IsFalse(double.IsNaN(actual[i]));
            }
        }

        #endregion

        [Test]
        public void GradientTest_MarkovMultivariate()
        {
            // Creates a sequence classifier containing 2 hidden Markov Models
            //  with 2 states and an underlying Normal distribution as density.
            MultivariateNormalDistribution density = new MultivariateNormalDistribution(3);
            var hmm = new HiddenMarkovClassifier<MultivariateNormalDistribution>(2, new Ergodic(2), density);

            double[][][] inputs =
            {
                new [] { new double[] { 0, 1, 0 }, new double[] { 0, 1, 0 }, new double[] { 0, 1, 0 } },
                new [] { new double[] { 1, 6, 2 }, new double[] { 2, 1, 6 }, new double[] { 1, 1, 0 } },
                new [] { new double[] { 9, 1, 0 }, new double[] { 0, 1, 5 }, new double[] { 0, 0, 0 } },
            };

            int[] outputs =
            {
                0, 0, 1
            };

            var function = new MarkovMultivariateFunction(hmm);

            var model = new HiddenConditionalRandomField<double[]>(function);
            var target = new ForwardBackwardGradient<double[]>(model);

            FiniteDifferences diff = new FiniteDifferences(function.Weights.Length)
            {
                StepSize = 1e-5
            };

            diff.Function = parameters => func(model, parameters, inputs, outputs);

            double[] expected = diff.Compute(function.Weights);
            double[] actual = target.Gradient(function.Weights, inputs, outputs);


            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 0.05);
                Assert.IsFalse(double.IsNaN(actual[i]));
                Assert.IsFalse(double.IsNaN(expected[i]));
            }
        }

        [Test]
        public void GradientTest_MarkovMultivariate2()
        {
            var hmm = MultivariateMarkovFunctionTest.CreateModel1();
            var function = new MarkovMultivariateFunction(hmm);

            var model = new HiddenConditionalRandomField<double[]>(function);
            var target = new ForwardBackwardGradient<double[]>(model);

            var inputs = MultivariateNormalQuasiNewtonHiddenLearningTest.inputs1;
            var outputs = MultivariateNormalQuasiNewtonHiddenLearningTest.outputs1;

            FiniteDifferences diff = new FiniteDifferences(function.Weights.Length);

            diff.Function = parameters => func(model, parameters, inputs, outputs);

            double[] expected = diff.Compute(function.Weights);
            double[] actual = target.Gradient(function.Weights, inputs, outputs);


            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 1e-3);
                Assert.IsFalse(double.IsNaN(actual[i]));
                Assert.IsFalse(double.IsNaN(expected[i]));
            }
        }

        [Test]
        public void GradientTest_MarkovMultivariate3()
        {
            var hmm = MultivariateMarkovFunctionTest.CreateModel1();
            var function = new MarkovMultivariateFunction(hmm);

            var model = new HiddenConditionalRandomField<double[]>(function);
            var target = new ForwardBackwardGradient<double[]>(model);
            target.Regularization = 2;

            var inputs = MultivariateNormalQuasiNewtonHiddenLearningTest.inputs1;
            var outputs = MultivariateNormalQuasiNewtonHiddenLearningTest.outputs1;


            FiniteDifferences diff = new FiniteDifferences(function.Weights.Length);

            diff.Function = parameters => func(model, parameters, inputs, outputs, target.Regularization);

            double[] expected = diff.Compute(function.Weights);
            double[] actual = target.Gradient(function.Weights, inputs, outputs);


            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 1e-3);

                Assert.IsFalse(double.IsNaN(actual[i]));
                Assert.IsFalse(double.IsNaN(expected[i]));
            }
        }






        [Test]
        public void GradientTest_MarkovNormal()
        {
            var hmm = MarkovContinuousFunctionTest.CreateModel1();
            var function = new MarkovContinuousFunction(hmm);

            var model = new HiddenConditionalRandomField<double>(function);
            var target = new ForwardBackwardGradient<double>(model);

            FiniteDifferences diff = new FiniteDifferences(function.Weights.Length);

            var inputs = NormalQuasiNewtonHiddenLearningTest.inputs;
            var outputs = NormalQuasiNewtonHiddenLearningTest.outputs;


            diff.Function = parameters => func(model, parameters, inputs, outputs);

            double[] expected = diff.Compute(function.Weights);
            double[] actual = target.Gradient(function.Weights, inputs, outputs);


            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 1e-3);
                Assert.IsFalse(double.IsNaN(actual[i]));
                Assert.IsFalse(double.IsNaN(expected[i]));
            }
        }

        [Test]
        public void GradientTest_MarkovNormal_Regularization()
        {
            var hmm = MarkovContinuousFunctionTest.CreateModel1();
            var function = new MarkovContinuousFunction(hmm);

            var model = new HiddenConditionalRandomField<double>(function);
            var target = new ForwardBackwardGradient<double>(model);
            target.Regularization = 2;

            var inputs = NormalQuasiNewtonHiddenLearningTest.inputs;
            var outputs = NormalQuasiNewtonHiddenLearningTest.outputs;

            FiniteDifferences diff = new FiniteDifferences(function.Weights.Length);

            diff.Function = parameters => func(model, parameters, inputs, outputs, target.Regularization);

            double[] expected = diff.Compute(function.Weights);
            double[] actual = target.Gradient(function.Weights, inputs, outputs);


            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 1e-2);
                Assert.IsFalse(double.IsNaN(actual[i]));
                Assert.IsFalse(double.IsNaN(expected[i]));
            }
        }


        private double func<T>(HiddenConditionalRandomField<T> model, double[] parameters, T[][] inputs, int[] outputs)
        {
            model.Function.Weights = parameters;
            return -model.LogLikelihood(inputs, outputs);
        }

        private double func<T>(HiddenConditionalRandomField<T> model, double[] parameters, T[][] inputs, int[] outputs, double beta)
        {
            model.Function.Weights = parameters;

            // Regularization
            double sumSquaredWeights = 0;
            if (beta != 0)
            {
                for (int i = 0; i < parameters.Length; i++)
                    if (!(Double.IsInfinity(parameters[i]) || Double.IsNaN(parameters[i])))
                        sumSquaredWeights += parameters[i] * parameters[i];
                sumSquaredWeights = sumSquaredWeights * 0.5 / beta;
            }

            double logLikelihood = model.LogLikelihood(inputs, outputs);

            // Maximize the log-likelihood and minimize the sum of squared weights
            return -logLikelihood + sumSquaredWeights;
        }

    }
}
