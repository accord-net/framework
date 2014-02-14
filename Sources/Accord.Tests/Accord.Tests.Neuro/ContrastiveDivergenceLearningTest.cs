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

namespace Accord.Tests.Neuro
{
    using Accord.Neuro.Learning;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Neuro.Networks;
    using Accord.Neuro.Neurons;
    using AForge;
    using Accord.Neuro.ActivationFunctions;
    using Accord.Neuro;


    [TestClass()]
    public class ContrastiveDivergenceLearningTest
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
#if !DEBUG
        [Ignore]
#endif
        public void RunTest()
        {
            // Example from Edwin Chen, Introduction to Restricted Boltzmann Machines
            // http://blog.echen.me/2011/07/18/introduction-to-restricted-Boltzmann-machines/

            double[][] inputs =
            {
                new double[] { 1,1,1,0,0,0 },
                new double[] { 1,0,1,0,0,0 },
                new double[] { 1,1,1,0,0,0 },
                new double[] { 0,0,1,1,1,0 },
                new double[] { 0,0,1,1,0,0 },
                new double[] { 0,0,1,1,1,0 }
            };

            BernoulliFunction activation = new BernoulliFunction();
            BernoulliFunction.Random = new ThreadSafeRandom(2);
            RestrictedBoltzmannMachine network = new RestrictedBoltzmannMachine(activation, 6, 2);

            network.Hidden.Neurons[0].Weights[0] = 0.00461421;
            network.Hidden.Neurons[0].Weights[1] = 0.04337112;
            network.Hidden.Neurons[0].Weights[2] = -0.10839599;
            network.Hidden.Neurons[0].Weights[3] = -0.06234004;
            network.Hidden.Neurons[0].Weights[4] = -0.03017057;
            network.Hidden.Neurons[0].Weights[5] = 0.09520391;
            network.Hidden.Neurons[0].Threshold = 0;

            network.Hidden.Neurons[1].Weights[0] = 0.08263872;
            network.Hidden.Neurons[1].Weights[1] = -0.118437;
            network.Hidden.Neurons[1].Weights[2] = -0.21710971;
            network.Hidden.Neurons[1].Weights[3] = 0.02332903;
            network.Hidden.Neurons[1].Weights[4] = 0.00953116;
            network.Hidden.Neurons[1].Weights[5] = 0.09870652;
            network.Hidden.Neurons[1].Threshold = 0;

            network.Visible.Neurons[0].Threshold = 0;
            network.Visible.Neurons[1].Threshold = 0;
            network.Visible.Neurons[2].Threshold = 0;
            network.Visible.Neurons[3].Threshold = 0;
            network.Visible.Neurons[4].Threshold = 0;
            network.Visible.Neurons[5].Threshold = 0;

            network.Visible.CopyReversedWeightsFrom(network.Hidden);

            ContrastiveDivergenceLearning target = new ContrastiveDivergenceLearning(network);

            target.Momentum = 0;
            target.LearningRate = 0.1;
            target.Decay = 0;

            int iterations = 5000;
            double[] errors = new double[iterations];
            for (int i = 0; i < iterations; i++)
                errors[i] = target.RunEpoch(inputs);

            double startError = errors[0];
            double lastError = errors[iterations - 1];
            Assert.IsTrue(startError > lastError);

            Assert.AreEqual(9.5400234262580224, startError);
            Assert.AreEqual(1.3364496250348414, lastError, 1e-10);

            {
                double[] output = network.GenerateOutput(new double[] { 0, 0, 0, 1, 1, 0 });
                Assert.AreEqual(2, output.Length);
                Assert.AreEqual(0, output[0]);
                Assert.AreEqual(1, output[1]);
            }

            {
                double[] output = network.GenerateOutput(new double[] { 1, 1, 1, 0, 0, 0 });
                Assert.AreEqual(2, output.Length);
                Assert.AreEqual(1, output[0]);
                Assert.AreEqual(0, output[1]);
            }
        }

        [TestMethod()]
        public void RunTest2()
        {
            // Example from Edwin Chen, Introduction to Restricted Boltzmann Machines
            // http://blog.echen.me/2011/07/18/introduction-to-restricted-Boltzmann-machines/

            double[][] inputs =
            {
                new double[] { 1,1,1,0,0,0 },
                new double[] { 1,0,1,0,0,0 },
                new double[] { 1,1,1,0,0,0 },
                new double[] { 0,0,1,1,1,0 },
                new double[] { 0,0,1,1,0,0 },
                new double[] { 0,0,1,1,1,0 }
            };

            Accord.Math.Tools.SetupGenerator(0);
            BernoulliFunction.Random = new ThreadSafeRandom(0);
            GaussianFunction.Random.SetSeed(0);

            RestrictedBoltzmannMachine network = 
                RestrictedBoltzmannMachine.CreateGaussianBernoulli(6, 2);

            new GaussianWeights(network).Randomize();
            network.UpdateVisibleWeights();
            

            ContrastiveDivergenceLearning target = new ContrastiveDivergenceLearning(network);

            target.Momentum = 0;
            target.LearningRate = 0.1;
            target.Decay = 0;

            int iterations = 5000;
            double[] errors = new double[iterations];
            for (int i = 0; i < iterations; i++)
                errors[i] = target.RunEpoch(inputs);

            double startError = errors[0];
            double lastError = errors[iterations - 1];
            Assert.IsTrue(startError > lastError);

            {
                double[] output = network.GenerateOutput(new double[] { 0, 0, 0, 1, 1, 0 });
                Assert.AreEqual(2, output.Length);
                Assert.AreEqual(1, output[0]);
                Assert.AreEqual(0, output[1]);
            }

            {
                double[] output = network.GenerateOutput(new double[] { 1, 1, 1, 0, 0, 0 });
                Assert.AreEqual(2, output.Length);
                Assert.AreEqual(1, output[0]);
                Assert.AreEqual(1, output[1]);
            }


        }

    }
}
