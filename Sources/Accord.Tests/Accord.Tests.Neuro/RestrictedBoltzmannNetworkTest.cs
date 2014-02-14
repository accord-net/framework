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
    using Accord.Neuro.Networks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using AForge.Neuro;
    using Accord.Neuro.Learning;
    using Accord.Neuro.ActivationFunctions;

    [TestClass()]
    public class RestrictedBoltzmannNetworkTest
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
        public void CreateActivationNetworkTest()
        {
            double[][] inputs =
            {
                new double[] { 1,1,1,0,0,0 },
                new double[] { 1,0,1,0,0,0 },
                new double[] { 1,1,1,0,0,0 },
                new double[] { 0,0,1,1,1,0 },
                new double[] { 0,0,1,1,0,0 },
                new double[] { 0,0,1,1,1,0 }
            };

            double[][] outputs =
            {
                new double[] { 0 },
                new double[] { 0 },
                new double[] { 0 },
                new double[] { 1 },
                new double[] { 1 },
                new double[] { 1 },
            };

            RestrictedBoltzmannMachine network = createNetwork(inputs);

            ActivationNetwork ann = network.ToActivationNetwork(new SigmoidFunction(1), outputs: 1);

            ParallelResilientBackpropagationLearning teacher = new ParallelResilientBackpropagationLearning(ann);

            for (int i = 0; i < 100; i++)
            {
                teacher.RunEpoch(inputs, outputs);
            }

            double[] actual = new double[outputs.Length];
            for (int i = 0; i < inputs.Length; i++)
                actual[i] = ann.Compute(inputs[i])[0];

            Assert.AreEqual(0, actual[0], 1e-10);
            Assert.AreEqual(0, actual[1], 1e-10);
            Assert.AreEqual(0, actual[2], 1e-10);
            Assert.AreEqual(1, actual[3], 1e-10);
            Assert.AreEqual(1, actual[4], 1e-10);
            Assert.AreEqual(1, actual[5], 1e-10);
        }

        private static RestrictedBoltzmannMachine createNetwork(double[][] inputs)
        {
            RestrictedBoltzmannMachine network = new RestrictedBoltzmannMachine(6, 2);

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

            int iterations = 5000;
            double[] errors = new double[iterations];
            for (int i = 0; i < iterations; i++)
                errors[i] = target.RunEpoch(inputs);

            return network;
        }

        [TestMethod()]
        public void NetworkTest2()
        {
            // Create some sample inputs and outputs. Note that the
            // first four vectors belong to one class, and the other
            // four belong to another (you should see that the 1s
            // accumulate on the beginning for the first four vectors
            // and on the end for the second four).

            double[][] inputs =
            {
                new double[] { 1,1,1,0,0,0 }, // class a
                new double[] { 1,0,1,0,0,0 }, // class a
                new double[] { 1,1,1,0,0,0 }, // class a
                new double[] { 0,0,1,1,1,0 }, // class b
                new double[] { 0,0,1,1,0,0 }, // class b
                new double[] { 0,0,1,1,1,0 }, // class b
            };

            double[][] outputs =
            {
                new double[] { 1, 0 }, // indicates the inputs at this
                new double[] { 1, 0 }, // position belongs to class a
                new double[] { 1, 0 },
                new double[] { 0, 1 }, // indicates the inputs at this
                new double[] { 0, 1 }, // position belongs to class b
                new double[] { 0, 1 },
            };

            // Create a Bernoulli activation function
            var function = new BernoulliFunction(alpha: 0.5);

            // Create a Restricted Boltzmann Machine for 6 inputs and with 1 hidden neuron
            var rbm = new RestrictedBoltzmannMachine(function, inputsCount: 6, hiddenNeurons: 2);

            // Create the learning algorithm for RBMs
            var teacher = new ContrastiveDivergenceLearning(rbm)
            {
                Momentum = 0,
                LearningRate = 0.1,
                Decay = 0
            };

            // learn 5000 iterations
            for (int i = 0; i < 5000; i++)
                teacher.RunEpoch(inputs);

            // Compute the machine answers for the given inputs:
            double[] a = rbm.Compute(new double[] { 1, 1, 1, 0, 0, 0 }); // { 0.99, 0.00 }
            double[] b = rbm.Compute(new double[] { 0, 0, 0, 1, 1, 1 }); // { 0.00, 0.99 }

            // As we can see, the first neuron responds to vectors belonging
            // to the first class, firing 0.99 when we feed vectors which 
            // have 1s at the beginning. Likewise, the second neuron fires 
            // when the vector belongs to the second class.

            // We can also generate input vectors given the classes:
            double[] xa = rbm.GenerateInput(new double[] { 1, 0 }); // { 1, 1, 1, 0, 0, 0 }
            double[] xb = rbm.GenerateInput(new double[] { 0, 1 }); // { 0, 0, 1, 1, 1, 0 }

            // As we can see, if we feed an output pattern where the first neuron
            // is firing and the second isn't, the network generates an example of
            // a vector belonging to the first class. The same goes for the second
            // neuron and the second class.

            Assert.IsTrue(((a[0] > a[1]) && (b[0] < b[1])) 
                        ^ ((a[0] < a[1]) && (b[0] > b[1])));
        }
    }
}
