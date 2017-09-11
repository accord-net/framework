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
    using Accord.Math;
    using Accord.Neuro;
    using Accord.Neuro.Learning;
    using Accord.Statistics;
    using AForge;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class ResilientBackPropagationLearningTest
    {


#if !NET35
        [Test]
        public void RunEpochTest1()
        {
            Accord.Math.Tools.SetupGenerator(0);

            double[][] input = 
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            double[][] output =
            {
                new double[] { -1 },
                new double[] {  1 },
                new double[] {  1 },
                new double[] { -1 }
            };

            //Neuron.RandGenerator = new ThreadSafeRandom(0);
            ActivationNetwork network = new ActivationNetwork(
                   new BipolarSigmoidFunction(2), 2, 2, 1);

            var teacher = new ParallelResilientBackpropagationLearning(network);

            double error = 1.0;
            while (error > 1e-5)
                error = teacher.RunEpoch(input, output);

            for (int i = 0; i < input.Length; i++)
            {
                double actual = network.Compute(input[i])[0];
                double expected = output[i][0];

                Assert.AreEqual(expected, actual, 0.01);
                Assert.IsFalse(Double.IsNaN(actual));
            }
        }

        [Test]
        [Category("Slow")]
        public void MulticlassTest1()
        {
            Accord.Math.Tools.SetupGenerator(0);

            // Suppose we would like to teach a network to recognize 
            // the following input vectors into 3 possible classes:
            //
            double[][] inputs =
            {
                new double[] { 0, 1, 1, 0 }, // 0
                new double[] { 0, 1, 0, 0 }, // 0
                new double[] { 0, 0, 1, 0 }, // 0
                new double[] { 0, 1, 1, 0 }, // 0
                new double[] { 0, 1, 0, 0 }, // 0
                new double[] { 1, 0, 0, 0 }, // 1
                new double[] { 1, 0, 0, 0 }, // 1
                new double[] { 1, 0, 0, 1 }, // 1
                new double[] { 0, 0, 0, 1 }, // 1
                new double[] { 0, 0, 0, 1 }, // 1
                new double[] { 1, 1, 1, 1 }, // 2
                new double[] { 1, 0, 1, 1 }, // 2
                new double[] { 1, 1, 0, 1 }, // 2
                new double[] { 0, 1, 1, 1 }, // 2
                new double[] { 1, 1, 1, 1 }, // 2
            };

            int[] classes =
            {
                0, 0, 0, 0, 0,
                1, 1, 1, 1, 1,
                2, 2, 2, 2, 2,
            };

            // First we have to convert this problem into a way that  the neural
            // network can handle. The first step is to expand the classes into 
            // indicator vectors, where a 1 into a position signifies that this
            // position indicates the class the sample belongs to.
            //
            double[][] outputs = Statistics.Tools.Expand(classes, -1, +1);

            // Create an activation function for the net
            var function = new BipolarSigmoidFunction();

            // Create an activation network with the function and
            //  4 inputs, 5 hidden neurons and 3 possible outputs:
            var network = new ActivationNetwork(function, 4, 5, 3);

            // Randomly initialize the network
            new NguyenWidrow(network).Randomize();

            // Teach the network using parallel Rprop:
            var teacher = new ParallelResilientBackpropagationLearning(network);

            double error = 1.0;
            while (error > 1e-5)
                error = teacher.RunEpoch(inputs, outputs);


            // Checks if the network has learned
            for (int i = 0; i < inputs.Length; i++)
            {
                double[] answer = network.Compute(inputs[i]);

                int expected = classes[i];
                int actual; answer.Max(out actual);

                Assert.AreEqual(expected, actual, 0.01);
            }
        }
#endif


    }
}
