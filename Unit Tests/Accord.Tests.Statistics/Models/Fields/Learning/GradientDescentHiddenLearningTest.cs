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
    using Accord.Statistics.Models.Fields;
    using Accord.Statistics.Models.Fields.Functions;
    using Accord.Statistics.Models.Fields.Learning;
    using Accord.Statistics.Models.Markov;
    using NUnit.Framework;

    [TestFixture]
    public class GradientDescentHiddenLearningTest
    {

        [Test]
        public void RunTest()
        {
            var inputs = QuasiNewtonHiddenLearningTest.inputs;
            var outputs = QuasiNewtonHiddenLearningTest.outputs;

            HiddenMarkovClassifier hmm = DiscreteHiddenMarkovClassifierPotentialFunctionTest.CreateModel1();
            var function = new MarkovDiscreteFunction(hmm);

            var model = new HiddenConditionalRandomField<int>(function);
            var target = new HiddenGradientDescentLearning<int>(model);

#if DEBUG
            target.ParallelOptions.MaxDegreeOfParallelism = 1;
#endif

            target.LearningRate = 1000;

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

            double error = Double.NegativeInfinity;
            for (int i = 0; i < 50; i++)
                error = target.RunEpoch(inputs, outputs);

            double ll1 = model.LogLikelihood(inputs, outputs);

            for (int i = 0; i < inputs.Length; i++)
            {
                actual[i] = model.Compute(inputs[i]);
                expected[i] = outputs[i];
            }

            Assert.AreEqual(-0.00046872579975998363, ll0, 1e-10);
            Assert.AreEqual(0.00027018722449545507, error, 1e-10);

            for (int i = 0; i < inputs.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);

            Assert.IsTrue(ll1 > ll0);
        }

        [Test]
        public void RunTest2()
        {
            var inputs = QuasiNewtonHiddenLearningTest.inputs;
            var outputs = QuasiNewtonHiddenLearningTest.outputs;


            Accord.Math.Tools.SetupGenerator(0);

            var function = new MarkovDiscreteFunction(2, 2, 2);

            var model = new HiddenConditionalRandomField<int>(function);
            var target = new HiddenGradientDescentLearning<int>(model);

            double[] actual = new double[inputs.Length];
            double[] expected = new double[inputs.Length];

            for (int i = 0; i < inputs.Length; i++)
            {
                actual[i] = model.Compute(inputs[i]);
                expected[i] = outputs[i];
            }


            double ll0 = model.LogLikelihood(inputs, outputs);

            double error = Double.PositiveInfinity;
            for (int i = 0; i < 50; i++)
            {
                error = target.RunEpoch(inputs, outputs);
            }

            double ll1 = model.LogLikelihood(inputs, outputs);

            for (int i = 0; i < inputs.Length; i++)
            {
                actual[i] = model.Compute(inputs[i]);
                expected[i] = outputs[i];
            }


            Assert.AreEqual(-5.5451774444795623, ll0, 1e-10);
            Assert.AreEqual(0, error, 1e-5);
            Assert.IsFalse(double.IsNaN(error));

            for (int i = 0; i < inputs.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);

            Assert.IsTrue(ll1 > ll0);
        }

    }
}