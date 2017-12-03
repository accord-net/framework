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

namespace Accord.Tests.MachineLearning
{
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Statistics.Kernels;
    using Accord.Statistics.Kernels.Sparse;
    using NUnit.Framework;
    using Accord.Math;
    using System;

    [TestFixture]
    public class KernelSupportVectorMachineTest
    {


        [Test]
        public void ComputeTest()
        {
            // Example AND problem
            double[][] inputs =
            {
                new double[] { 0, 0 }, // 0 and 0: 0 (label -1)
                new double[] { 0, 1 }, // 0 and 1: 0 (label -1)
                new double[] { 1, 0 }, // 1 and 0: 0 (label -1)
                new double[] { 1, 1 }  // 1 and 1: 1 (label +1)
            };

            // Dichotomy SVM outputs should be given as [-1;+1]
            int[] labels =
            {
                // 0,  0,  0, 1
                  -1, -1, -1, 1
            };

            // Create a Support Vector Machine for the given inputs
            var machine = new KernelSupportVectorMachine(new Gaussian(0.1), inputs[0].Length);

            // Instantiate a new learning algorithm for SVMs
            var smo = new SequentialMinimalOptimization(machine, inputs, labels);

            // Set up the learning algorithm
            smo.Complexity = 1.0;

            // Run
            double error = smo.Run();

            Assert.AreEqual(-1, Math.Sign(machine.Compute(inputs[0])));
            Assert.AreEqual(-1, Math.Sign(machine.Compute(inputs[1])));
            Assert.AreEqual(-1, Math.Sign(machine.Compute(inputs[2])));
            Assert.AreEqual(+1, Math.Sign(machine.Compute(inputs[3])));

            Assert.AreEqual(error, 0);

            Assert.AreEqual(-0.6640625, machine.Threshold);
            Assert.AreEqual(1, machine.Weights[0]);
            Assert.AreEqual(-0.34375, machine.Weights[1]);
            Assert.AreEqual(-0.328125, machine.Weights[2]);
            Assert.AreEqual(-0.328125, machine.Weights[3]);
        }

        [Test]
        public void ComputeTest3()
        {
            // Example AND problem
            double[][] inputs =
            {
                new double[] { 0, 0 }, // 0 and 0: 0 (label -1)
                new double[] { 0, 1 }, // 0 and 1: 0 (label -1)
                new double[] { 1, 0 }, // 1 and 0: 0 (label -1)
                new double[] { 1, 1 }  // 1 and 1: 1 (label +1)
            };

            // Dichotomy SVM outputs should be given as [-1;+1]
            int[] labels =
            {
                // 0,  0,  0, 1
                  -1, -1, -1, 1
            };

            // Create a Support Vector Machine for the given inputs
            var machine = new KernelSupportVectorMachine(new Linear(), inputs[0].Length);

            // Instantiate a new learning algorithm for SVMs
            var smo = new SequentialMinimalOptimization(machine, inputs, labels);

            // Set up the learning algorithm
            smo.Complexity = 100000.0;

            // Run
            double error = smo.Run();

            Assert.AreEqual(-1, Math.Sign(machine.Compute(inputs[0])));
            Assert.AreEqual(-1, Math.Sign(machine.Compute(inputs[1])));
            Assert.AreEqual(-1, Math.Sign(machine.Compute(inputs[2])));
            Assert.AreEqual(+1, Math.Sign(machine.Compute(inputs[3])));

            Assert.AreEqual(error, 0);

            Assert.AreEqual(-3.0, machine.Threshold);
            Assert.AreEqual(4, machine.Weights[0]);
            Assert.AreEqual(-2, machine.Weights[1]);
            Assert.AreEqual(-2, machine.Weights[2]);
        }


        [Test]
        public void SparseComputeTest()
        {
            // Example AND problem
            double[][] inputs =
            {
                new double[] {          }, // 0 and 0: 0 (label -1)
                new double[] {      2,1 }, // 0 and 1: 0 (label -1)
                new double[] { 1,1      }, // 1 and 0: 0 (label -1)
                new double[] { 1,1, 2,1 }  // 1 and 1: 1 (label +1)
            };

            // Dichotomy SVM outputs should be given as [-1;+1]
            int[] labels =
            {
                // 0,  0,  0, 1
                    -1, -1, -1, 1
            };

            // Create a Support Vector Machine for the given inputs
            // (sparse machines should use 0 as the number of inputs)
            var machine = new KernelSupportVectorMachine(new SparseLinear(), inputs: 0); 

            // Instantiate a new learning algorithm for SVMs
            var smo = new SequentialMinimalOptimization(machine, inputs, labels);

            // Set up the learning algorithm
            smo.Complexity = 100000.0;

            // Run
            double error = smo.Run(); // should be zero

            double[] predicted = inputs.Apply(machine.Compute).Sign();

            // Outputs should be -1, -1, -1, +1
            Assert.AreEqual(-1, predicted[0]);
            Assert.AreEqual(-1, predicted[1]);
            Assert.AreEqual(-1, predicted[2]);
            Assert.AreEqual(+1, predicted[3]);

            Assert.AreEqual(error, 0);

            // Same as ComputeTest3 test above
            Assert.AreEqual(-3.0, machine.Threshold);
            Assert.AreEqual(4, machine.Weights[0]);
            Assert.AreEqual(-2, machine.Weights[1]);
            Assert.AreEqual(-2, machine.Weights[2]);
        }


        [Test]
        public void ComputeTest2()
        {
            // XOR
            double[][] inputs =
            {
                new double[] { 0, 0 },
                new double[] { 0, 1 },
                new double[] { 1, 0 },
                new double[] { 1, 1 }
            };

            int[] labels =
            {
                -1,
                 1,
                 1,
                -1
            };

            var machine = new KernelSupportVectorMachine(new Gaussian(0.1), inputs[0].Length);
            var smo = new SequentialMinimalOptimization(machine, inputs, labels);

            smo.Complexity = 1;
            double error = smo.Run();

            Assert.AreEqual(-1, Math.Sign(machine.Compute(inputs[0])));
            Assert.AreEqual(+1, Math.Sign(machine.Compute(inputs[1])));
            Assert.AreEqual(+1, Math.Sign(machine.Compute(inputs[2])));
            Assert.AreEqual(-1, Math.Sign(machine.Compute(inputs[3])));

            Assert.AreEqual(error, 0);
        }



        public static double[][] training = new double[][]
        {
                #region sample data
                new double[] { 0567, 0568, 0001, 0002,    0 },
                new double[] { 0839, 1043, 0204, 0011,    1 },
                new double[] { 0506, 1400, 0894, 0020,    1 },
                new double[] { 0066, 0066, 0000, 0001,    0 },
                new double[] { 0208, 0223, 0015, 0005,    1 },
                new double[] { 0069, 0069, 0000, 0001,    0 },
                new double[] { 0417, 0458, 0041, 0008,    1 },
                new double[] { 0078, 0078, 0000, 0001,    0 },
                new double[] { 0137, 0150, 0013, 0004,    1 },
                new double[] { 0108, 0136, 0028, 0002,    0 },
                new double[] { 0235, 0294, 0059, 0005,    0 },
                new double[] { 0350, 0511, 0161, 0010,    1 },
                new double[] { 0271, 0418, 0147, 0003,    0 },
                new double[] { 0195, 0217, 0022, 0010,    1 },
                new double[] { 0259, 0267, 0008, 0006,    1 },
                new double[] { 0298, 0372, 0074, 0007,    1 },
                new double[] { 0709, 0994, 0285, 0016,    1 },
                new double[] { 1041, 1348, 0307, 0039,    1 },
                new double[] { 0075, 0075, 0000, 0001,    0 },
                new double[] { 0529, 0597, 0068, 0002,    0 },
                new double[] { 0509, 0584, 0075, 0002,    0 },
                new double[] { 0289, 0289, 0000, 0001,    0 },
                new double[] { 0110, 0125, 0015, 0004,    0 },
                new double[] { 0020, 0020, 0000, 0001,    0 },
                new double[] { 0295, 0295, 0000, 0001,    0 },
                new double[] { 0250, 0283, 0033, 0002,    0 },
                new double[] { 0031, 0044, 0013, 0002,    0 },
                new double[] { 0178, 0198, 0020, 0003,    0 },
                new double[] { 0835, 0848, 0013, 0005,    0 },
                new double[] { 0132, 0178, 0046, 0002,    0 },
                new double[] { 0429, 0632, 0203, 0004,    0 },
                new double[] { 0740, 0894, 0154, 0005,    0 },
                new double[] { 0056, 0065, 0009, 0003,    1 },
                new double[] { 0071, 0071, 0000, 0001,    0 },
                new double[] { 0248, 0321, 0073, 0004,    0 },
                new double[] { 0034, 0034, 0000, 0001,    0 },
                new double[] { 0589, 0652, 0063, 0004,    0 },
                new double[] { 0124, 0134, 0010, 0002,    0 },
                new double[] { 0426, 0427, 0001, 0002,    0 },
                new double[] { 0030, 0030, 0000, 0001,    0 },
                new double[] { 0023, 0023, 0000, 0001,    0 },
                new double[] { 0499, 0499, 0000, 0001,    0 },
                #endregion
        };

    }
}
