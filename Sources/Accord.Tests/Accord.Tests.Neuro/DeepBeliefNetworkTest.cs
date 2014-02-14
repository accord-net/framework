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
    using Accord.Neuro.ActivationFunctions;
    using Accord.Neuro.Learning;
    using Accord.Neuro.Layers;

    [TestClass()]
    public class DeepBeliefNetworkTest
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

            DeepBeliefNetwork network = createNetwork(inputs);

            ParallelResilientBackpropagationLearning teacher = new ParallelResilientBackpropagationLearning(network);

            for (int i = 0; i < 100; i++)
            {
                teacher.RunEpoch(inputs, outputs);
            }

            double[] actual = new double[outputs.Length];
            for (int i = 0; i < inputs.Length; i++)
                actual[i] = network.Compute(inputs[i])[0];

            Assert.AreEqual(0, actual[0], 1e-10);
            Assert.AreEqual(0, actual[1], 1e-10);
            Assert.AreEqual(0, actual[2], 1e-10);
            Assert.AreEqual(1, actual[3], 1e-10);
            Assert.AreEqual(1, actual[4], 1e-10);
            Assert.AreEqual(1, actual[5], 1e-10);
        }

        [TestMethod()]
        public void PushPopTest()
        {
            DeepBeliefNetwork network = new DeepBeliefNetwork(6, 2, 9);

            Assert.AreEqual(2, network.Machines.Count);
            Assert.AreEqual(6, network.InputsCount);
            Assert.AreEqual(9, network.OutputCount);
            Assert.AreEqual(2, network.Machines[0].Hidden.Neurons.Length);
            Assert.AreEqual(9, network.Machines[1].Hidden.Neurons.Length);
            Assert.AreEqual(6, network.Machines[0].Visible.Neurons.Length);
            Assert.AreEqual(2, network.Machines[1].Visible.Neurons.Length);

            network.Pop();

            Assert.AreEqual(1, network.Machines.Count);
            Assert.AreEqual(6, network.InputsCount);
            Assert.AreEqual(2, network.OutputCount);
            Assert.AreEqual(2, network.Machines[0].Hidden.Neurons.Length);
            Assert.AreEqual(6, network.Machines[0].Visible.Neurons.Length);

            network.Push(4);
            network.Push(10);

            Assert.AreEqual(3, network.Machines.Count);
            Assert.AreEqual(6, network.InputsCount);
            Assert.AreEqual(10, network.OutputCount);
            Assert.AreEqual(2, network.Machines[0].Hidden.Neurons.Length);
            Assert.AreEqual(4, network.Machines[1].Hidden.Neurons.Length);
            Assert.AreEqual(10, network.Machines[2].Hidden.Neurons.Length);

            Assert.AreEqual(6, network.Machines[0].Visible.Neurons.Length);
            Assert.AreEqual(2, network.Machines[1].Visible.Neurons.Length);
            Assert.AreEqual(4, network.Machines[2].Visible.Neurons.Length);

            network.Pop();

            Assert.AreEqual(2, network.Machines.Count);
            Assert.AreEqual(6, network.InputsCount);
            Assert.AreEqual(4, network.OutputCount);
            Assert.AreEqual(2, network.Machines[0].Hidden.Neurons.Length);
            Assert.AreEqual(4, network.Machines[1].Hidden.Neurons.Length);

            Assert.AreEqual(6, network.Machines[0].Visible.Neurons.Length);
            Assert.AreEqual(2, network.Machines[1].Visible.Neurons.Length);
        }

        [TestMethod()]
        public void ConstructorTest()
        {
            DeepBeliefNetwork network = new DeepBeliefNetwork(6, 2, 1);

            Assert.AreEqual(2, network.Machines.Count);
            Assert.AreEqual(6, network.InputsCount);
            Assert.AreEqual(1, network.OutputCount);
            Assert.AreEqual(2, network.Machines[0].Hidden.Neurons.Length);
            Assert.AreEqual(1, network.Machines[1].Hidden.Neurons.Length);
            Assert.AreEqual(6, network.Machines[0].Visible.Neurons.Length);
            Assert.AreEqual(2, network.Machines[1].Visible.Neurons.Length);
        }

        private static DeepBeliefNetwork createNetwork(double[][] inputs)
        {
            DeepBeliefNetwork network = new DeepBeliefNetwork(6, 2, 1);

            network.Machines[0].Hidden.Neurons[0].Weights[0] = 0.00461421;
            network.Machines[0].Hidden.Neurons[0].Weights[1] = 0.04337112;
            network.Machines[0].Hidden.Neurons[0].Weights[2] = -0.10839599;
            network.Machines[0].Hidden.Neurons[0].Weights[3] = -0.06234004;
            network.Machines[0].Hidden.Neurons[0].Weights[4] = -0.03017057;
            network.Machines[0].Hidden.Neurons[0].Weights[5] = 0.09520391;
            network.Machines[0].Hidden.Neurons[0].Threshold = 0;

            network.Machines[0].Hidden.Neurons[1].Weights[0] = 0.08263872;
            network.Machines[0].Hidden.Neurons[1].Weights[1] = -0.118437;
            network.Machines[0].Hidden.Neurons[1].Weights[2] = -0.21710971;
            network.Machines[0].Hidden.Neurons[1].Weights[3] = 0.02332903;
            network.Machines[0].Hidden.Neurons[1].Weights[4] = 0.00953116;
            network.Machines[0].Hidden.Neurons[1].Weights[5] = 0.09870652;
            network.Machines[0].Hidden.Neurons[1].Threshold = 0;

            network.Machines[0].Visible.Neurons[0].Threshold = 0;
            network.Machines[0].Visible.Neurons[1].Threshold = 0;
            network.Machines[0].Visible.Neurons[2].Threshold = 0;
            network.Machines[0].Visible.Neurons[3].Threshold = 0;
            network.Machines[0].Visible.Neurons[4].Threshold = 0;
            network.Machines[0].Visible.Neurons[5].Threshold = 0;

            network.UpdateVisibleWeights();


            DeepBeliefNetworkLearning target = new DeepBeliefNetworkLearning(network)
            {
                Algorithm = (h, v, i) => new ContrastiveDivergenceLearning(h, v)
            };

            for (int layer = 0; layer < 2; layer++)
            {

                target.LayerIndex = layer;

                double[][] layerInputs = target.GetLayerInput(inputs);

                int iterations = 5000;
                double[] errors = new double[iterations];
                for (int i = 0; i < iterations; i++)
                    errors[i] = target.RunEpoch(layerInputs);
            }

            return network;
        }
    }
}
