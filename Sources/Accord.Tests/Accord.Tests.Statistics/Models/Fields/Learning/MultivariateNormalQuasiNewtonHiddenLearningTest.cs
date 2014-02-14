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

using Accord.Statistics.Models.Fields.Learning;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accord.Statistics.Models.Fields;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Topology;
using Accord.Statistics.Models.Fields.Functions;
using Accord.Statistics.Models.Markov.Learning;
using Accord.Math;
using Accord.Math.Differentiation;
using System;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Tests.Statistics.Models.Fields;

namespace Accord.Tests.Statistics
{


    /// <summary>
    ///This is a test class for QuasiNewtonLearningTest and is intended
    ///to contain all QuasiNewtonLearningTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MultivariateNormalQuasiNewtonHiddenLearningTest
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



        double[][][] inputs1 = new double[][][]
        {
            new double[][] 
            { 
                new double[] { 0 },
                new double[] { 1 },
                new double[] { 2 },
                new double[] { 3 },
                new double[] { 4 },
            }, 

            new double[][]
            {
                new double[] { 4 },
                new double[] { 3 },
                new double[] { 2 },
                new double[] { 1 },
                new double[] { 0 },
            }
        };

        int[] outputs1 = { 0, 1 };



        [TestMethod()]
        public void RunTest()
        {
            var hmm = MultivariateNormalHiddenMarkovClassifierPotentialFunctionTest.CreateModel1();
            var function = new MarkovMultivariateFunction(hmm);

            var model = new HiddenConditionalRandomField<double[]>(function);
            var target = new HiddenQuasiNewtonLearning<double[]>(model);

            var inputs = inputs1;
            var outputs = outputs1;

            double[] actual = new double[inputs.Length];
            double[] expected = new double[inputs.Length];

            for (int i = 0; i < inputs.Length; i++)
            {
                actual[i] = model.Compute(inputs[i]);
                expected[i] = outputs[i];
            }

            for (int i = 0; i < inputs.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);

            double llm = hmm.LogLikelihood(inputs, outputs);
            double ll0 = model.LogLikelihood(inputs, outputs);
            Assert.AreEqual(llm, ll0, 1e-10);
            Assert.IsFalse(double.IsNaN(llm));
            Assert.IsFalse(double.IsNaN(ll0));

            double error = target.Run(inputs, outputs);
            double ll1 = model.LogLikelihood(inputs, outputs);
            Assert.AreEqual(-ll1, error, 1e-10);
            Assert.IsFalse(double.IsNaN(ll1));
            Assert.IsFalse(double.IsNaN(error));


            for (int i = 0; i < inputs.Length; i++)
            {
                actual[i] = model.Compute(inputs[i]);
                expected[i] = outputs[i];
            }

            Assert.AreEqual(-0.0000041736023117522336, ll0, 1e-10);
            
            Assert.AreEqual(error, -ll1);
            Assert.IsFalse(Double.IsNaN(ll0));
            Assert.IsFalse(Double.IsNaN(error));

            for (int i = 0; i < inputs.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);

            Assert.IsTrue(ll1 > ll0);
        }


        [TestMethod()]
        public void GradientTest()
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

            FiniteDifferences diff = new FiniteDifferences(function.Weights.Length);

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

        [TestMethod()]
        public void GradientTest2()
        {
            var hmm = MultivariateNormalHiddenMarkovClassifierPotentialFunctionTest.CreateModel1();
            var function = new MarkovMultivariateFunction(hmm);

            var model = new HiddenConditionalRandomField<double[]>(function);
            var target = new ForwardBackwardGradient<double[]>(model);

            var inputs = inputs1;
            var outputs = outputs1;

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

        [TestMethod()]
        public void GradientTest3()
        {
            var hmm = MultivariateNormalHiddenMarkovClassifierPotentialFunctionTest.CreateModel1();
            var function = new MarkovMultivariateFunction(hmm);

            var model = new HiddenConditionalRandomField<double[]>(function);
            var target = new ForwardBackwardGradient<double[]>(model);
            target.Regularization = 2;

            var inputs = inputs1;
            var outputs = outputs1;



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


        private double func(HiddenConditionalRandomField<double[]> model, double[] parameters, double[][][] inputs, int[] outputs)
        {
            model.Function.Weights = parameters;
            return -model.LogLikelihood(inputs, outputs);
        }

        private double func(HiddenConditionalRandomField<double[]> model, double[] parameters, double[][][] inputs, int[] outputs, double beta)
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
