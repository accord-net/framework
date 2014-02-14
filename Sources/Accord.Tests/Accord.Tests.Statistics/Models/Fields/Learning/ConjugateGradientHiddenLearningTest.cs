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
    using Accord.Statistics.Models.Fields;
    using Accord.Statistics.Models.Fields.Functions;
    using Accord.Statistics.Models.Fields.Learning;
    using Accord.Statistics.Models.Markov;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class ConjugateGradientHiddenLearningTest
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
        public void RunTest()
        {
            var inputs = QuasiNewtonHiddenLearningTest.inputs;
            var outputs = QuasiNewtonHiddenLearningTest.outputs;

            HiddenMarkovClassifier hmm = HiddenMarkovClassifierPotentialFunctionTest.CreateModel1();
            var function = new MarkovDiscreteFunction(hmm);

            var model = new HiddenConditionalRandomField<int>(function);
            var target = new HiddenConjugateGradientLearning<int>(model);

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
            Assert.AreEqual(0.0001043282389863176, error, 1e-6);
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
            var inputs = QuasiNewtonHiddenLearningTest.inputs;
            var outputs = QuasiNewtonHiddenLearningTest.outputs;


            Accord.Math.Tools.SetupGenerator(0);

            var function = new MarkovDiscreteFunction(2, 2, 2);

            var model = new HiddenConditionalRandomField<int>(function);
            var target = new HiddenConjugateGradientLearning<int>(model);

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
            Assert.AreEqual(0, error, 1e-10);
            Assert.IsFalse(double.IsNaN(error));

            for (int i = 0; i < inputs.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);

            Assert.IsTrue(ll1 > ll0);
        }
    }
}