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
    using Accord.Math.Differentiation;
    using Accord.Statistics.Models.Fields;
    using Accord.Statistics.Models.Fields.Functions;
    using Accord.Statistics.Models.Fields.Learning;
    using Accord.Statistics.Models.Markov;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class QuasiNewtonHiddenLearningTest
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



        public static int[][] inputs = new int[][]
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

        public static int[] outputs = new int[]
        {
            0,0,0,0, // First four sequences are of class 0
            1,1,1,1, // Last four sequences are of class 1
        };



        [TestMethod()]
        public void RunTest()
        {
            HiddenMarkovClassifier hmm = HiddenMarkovClassifierPotentialFunctionTest.CreateModel1();
            var function = new MarkovDiscreteFunction(hmm);

            var model = new HiddenConditionalRandomField<int>(function);
            var target = new HiddenQuasiNewtonLearning<int>(model);

            double[] actual = new double[inputs.Length];
            double[] expected = new double[inputs.Length];

            for (int i = 0; i < inputs.Length; i++)
            {
                actual[i] = model.Compute(inputs[i]);
                expected[i] = outputs[i];
            }

            for (int i = 0; i < inputs.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);

            double ll0 = model.LogLikelihood(inputs, outputs);

            double error = target.Run(inputs, outputs);

            double ll1 = model.LogLikelihood(inputs, outputs);

            for (int i = 0; i < inputs.Length; i++)
            {
                actual[i] = model.Compute(inputs[i]);
                expected[i] = outputs[i];
            }

            Assert.AreEqual(-0.00046872579976353634, ll0, 1e-10);
            Assert.AreEqual(0.00012170806701733428, error, 1e-10);
            Assert.AreEqual(error, -ll1);
            Assert.IsFalse(Double.IsNaN(ll0));
            Assert.IsFalse(Double.IsNaN(error));

            for (int i = 0; i < inputs.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);

            Assert.IsTrue(ll1 > ll0);
        }

        [TestMethod()]
        public void RunTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            var function = new MarkovDiscreteFunction(2, 2, 2);

            var model = new HiddenConditionalRandomField<int>(function);
            var target = new HiddenQuasiNewtonLearning<int>(model);

            double[] actual = new double[inputs.Length];
            double[] expected = new double[inputs.Length];

            for (int i = 0; i < inputs.Length; i++)
            {
                actual[i] = model.Compute(inputs[i]);
                expected[i] = outputs[i];
            }


            double ll0 = model.LogLikelihood(inputs, outputs);
            double error = target.Run(inputs, outputs);
            double ll1 = model.LogLikelihood(inputs, outputs);

            for (int i = 0; i < inputs.Length; i++)
            {
                actual[i] = model.Compute(inputs[i]);
                expected[i] = outputs[i];
            }


            Assert.AreEqual(-5.5451774444795623, ll0, 1e-10);
            Assert.AreEqual(3.0990811490142045, error, 1e-10);
            Assert.IsFalse(double.IsNaN(error));

            for (int i = 0; i < inputs.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);

            Assert.IsTrue(ll1 > ll0);
        }




        [TestMethod()]
        public void GradientTest()
        {
            var function = new MarkovDiscreteFunction(2, 2, 2);
            var model = new HiddenConditionalRandomField<int>(function);
            var target = new ForwardBackwardGradient<int>(model);

            FiniteDifferences diff = new FiniteDifferences(function.Weights.Length);

            diff.Function = parameters => func(model, parameters);

            double[] expected = diff.Compute(function.Weights);
            double[] actual = target.Gradient(function.Weights, inputs, outputs);


            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 1e-4);
                Assert.IsFalse(double.IsNaN(actual[i]));
                Assert.IsFalse(double.IsNaN(expected[i]));
            }
        }

        [TestMethod()]
        public void GradientTest2()
        {
            HiddenMarkovClassifier hmm = HiddenMarkovClassifierPotentialFunctionTest.CreateModel1();
            var function = new MarkovDiscreteFunction(hmm);

            var model = new HiddenConditionalRandomField<int>(function);
            var target = new ForwardBackwardGradient<int>(model);

            FiniteDifferences diff = new FiniteDifferences(function.Weights.Length);

            diff.Function = parameters => func(model, parameters);

            double[] expected = diff.Compute(function.Weights);
            double[] actual = target.Gradient(function.Weights, inputs, outputs);


            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 1e-5);
                Assert.IsFalse(double.IsNaN(actual[i]));
                Assert.IsFalse(double.IsNaN(expected[i]));
            }
        }

        [TestMethod()]
        public void GradientTest3()
        {
            HiddenMarkovClassifier hmm = HiddenMarkovClassifierPotentialFunctionTest.CreateModel1();
            var function = new MarkovDiscreteFunction(hmm);

            var model = new HiddenConditionalRandomField<int>(function);
            var target = new ForwardBackwardGradient<int>(model);
            target.Regularization = 2;

            FiniteDifferences diff = new FiniteDifferences(function.Weights.Length);

            diff.Function = parameters => func(model, parameters, target.Regularization);

            double[] expected = diff.Compute(function.Weights);
            double[] actual = target.Gradient(function.Weights, inputs, outputs);


            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 1e-5);

                Assert.IsFalse(double.IsNaN(actual[i]));
                Assert.IsFalse(double.IsNaN(expected[i]));
            }
        }

        [TestMethod()]
        public void GradientTest4()
        {
            var hmm = IndependentMarkovClassifierPotentialFunctionTest.CreateModel2();
            var function = new MarkovMultivariateFunction(hmm);

            var model = new HiddenConditionalRandomField<double[]>(function);
            var target = new ForwardBackwardGradient<double[]>(model);
            target.Regularization = 0;

            FiniteDifferences diff = new FiniteDifferences(function.Weights.Length);

            diff.Function = parameters => func(model, parameters,
                IndependentMarkovClassifierPotentialFunctionTest.sequences,
                IndependentMarkovClassifierPotentialFunctionTest.labels);

            double[] expected = diff.Compute(function.Weights);
            double[] actual = target.Gradient(function.Weights,
                IndependentMarkovClassifierPotentialFunctionTest.sequences,
                IndependentMarkovClassifierPotentialFunctionTest.labels);


            for (int i = 0; i < actual.Length; i++)
            {
                if (double.IsNaN(expected[i]))
                    continue;

                Assert.AreEqual(expected[i], actual[i], 1e-5);
                Assert.IsFalse(double.IsNaN(actual[i]));
            }
        }


        private double func(HiddenConditionalRandomField<int> model, double[] parameters)
        {
            model.Function.Weights = parameters;
            return -model.LogLikelihood(inputs, outputs);
        }

        private double func(HiddenConditionalRandomField<double[]> model, double[] parameters,
            double[][][] inputs, int[] outputs)
        {
            model.Function.Weights = parameters;
            return -model.LogLikelihood(inputs, outputs);
        }

        private double func(HiddenConditionalRandomField<int> model, double[] parameters, double beta)
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
