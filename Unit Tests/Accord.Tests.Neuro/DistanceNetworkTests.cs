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

namespace Accord.Tests.Neuro
{
    using Accord.IO;
    using Accord.Math;
    using Accord.Neuro;
    using Accord.Neuro.Learning;
    using AForge;
    using NUnit.Framework;
    using System;
    using System.IO;

    [TestFixture]
    public class DistanceNetworkTest
    {

        [Test]
        public void DistanceNetworkTest1()
        {
            string basePath = Path.Combine(NUnit.Framework.TestContext.CurrentContext.TestDirectory, "dn");

            #region doc_example
            // Assure results are reproducible
            Accord.Math.Tools.SetupGenerator(0);

            int numberOfInputs = 3;
            int hiddenNeurons = 25;

            // Create some example inputs
            double[][] input =
            {
                new double[] { -1, -1, -1 },
                new double[] { -1,  1, -1 },
                new double[] {  1, -1, -1 },
                new double[] {  1,  1, -1 },
                new double[] { -1, -1,  1 },
                new double[] { -1,  1,  1 },
                new double[] {  1, -1,  1 },
                new double[] {  1,  1,  1 },
            };

            // Create a new network
            var network = new DistanceNetwork(numberOfInputs, hiddenNeurons);

            // Create a teaching algorithm
            var teacher = new SOMLearning(network);

            // Use the teacher to learn the network 
            double error = Double.PositiveInfinity;
            for (int i = 0; i < 10; i++)
                error = teacher.RunEpoch(input);

            string fileName = Path.Combine(basePath, "ann.bin");

            // Save the network to a file path:
            Serializer.Save(network, fileName);

            // Load the network back from the stream
            DistanceNetwork target = Serializer.Load<DistanceNetwork>(fileName);
            #endregion

            // Make sure the network we loaded is exactly the same
            Assert.AreEqual(network.InputsCount, target.InputsCount);
            for (int i = 0; i < network.Layers.Length; i++)
            {
                Assert.AreEqual(network.Layers[i].InputsCount, target.Layers[i].InputsCount);
                for (int j = 0; j < network.Layers[i].Neurons.Length; j++)
                {
                    Assert.AreEqual(network.Layers[i].Neurons[j].InputsCount, target.Layers[i].Neurons[j].InputsCount);
                    Assert.AreEqual(network.Layers[i].Neurons[j].Weights, target.Layers[i].Neurons[j].Weights);
                }
            }
        }

    }
}
