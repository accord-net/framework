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

namespace Accord.Tests.MachineLearning
{
    using System;
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Statistics.Kernels;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;
    using Accord.Math;
    using Accord.Statistics.Analysis;

    [TestClass()]
    public class SupportVectorMachineTest
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
            KernelSupportVectorMachine machine = new KernelSupportVectorMachine(new Gaussian(0.1), inputs[0].Length);

            // Instantiate a new learning algorithm for SVMs
            SequentialMinimalOptimization smo = new SequentialMinimalOptimization(machine, inputs, labels);

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

        [TestMethod()]
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
            KernelSupportVectorMachine machine = new KernelSupportVectorMachine(new Linear(), inputs[0].Length);

            // Instantiate a new learning algorithm for SVMs
            SequentialMinimalOptimization smo = new SequentialMinimalOptimization(machine, inputs, labels);

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


        [TestMethod()]
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

            KernelSupportVectorMachine machine = new KernelSupportVectorMachine(new Gaussian(0.1), inputs[0].Length);
            SequentialMinimalOptimization smo = new SequentialMinimalOptimization(machine, inputs, labels);

            smo.Complexity = 1;
            double error = smo.Run();

            Assert.AreEqual(-1, Math.Sign(machine.Compute(inputs[0])));
            Assert.AreEqual(+1, Math.Sign(machine.Compute(inputs[1])));
            Assert.AreEqual(+1, Math.Sign(machine.Compute(inputs[2])));
            Assert.AreEqual(-1, Math.Sign(machine.Compute(inputs[3])));

            Assert.AreEqual(error, 0);
        }

        [TestMethod()]
        public void ComputeTest4()
        {
            double[][] inputs = training.Submatrix(null, 0, 3);

            int[] labels = Tools.Scale(0, 1, -1, 1, training.GetColumn(4)).ToInt32();

            KernelSupportVectorMachine machine = new KernelSupportVectorMachine(
                Gaussian.Estimate(inputs), inputs[0].Length);

            SequentialMinimalOptimization smo = new SequentialMinimalOptimization(machine, inputs, labels);

            smo.Complexity = 10;

            double error = smo.Run();

            Assert.AreEqual(0.19047619047619047, error);
            Assert.AreEqual(265.78327637381551, (machine.Kernel as Gaussian).Sigma);
            Assert.AreEqual(29, machine.SupportVectors.Length);

            double[] expectedWeights =
            {
                1.65717694716503, 1.20005456611466, -5.70824245415995, 10,
                10, -2.38755497916487, 10, -8.15723436363058, 10, -10, 10,
                10, -0.188634936781317, -5.4354281009458, -8.48341139483265,
                -5.91105702760141, -5.71489190049223, 10, -2.37289205235858,
                -3.33031262413522, -1.97545116517677, 10, -10, -9.563186799279,
                -3.917941544845, -0.532584110773336, 4.81951847548326, 0.343668292727091,
                -4.34159482731336
            };

            Assert.IsTrue(expectedWeights.IsEqual(machine.Weights, 1e-6));

            int[] actual = new int[labels.Length];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = Math.Sign(machine.Compute(inputs[i]));

            ConfusionMatrix matrix = new ConfusionMatrix(actual, labels);

            Assert.AreEqual(8, matrix.FalseNegatives);
            Assert.AreEqual(0, matrix.FalsePositives);
            Assert.AreEqual(4, matrix.TruePositives);
            Assert.AreEqual(30, matrix.TrueNegatives);

            Assert.AreEqual(1 / 3.0, matrix.Sensitivity);
            Assert.AreEqual(1, matrix.Specificity);

            Assert.AreEqual(0.5, matrix.FScore);
            Assert.AreEqual(0.5129891760425771, matrix.MatthewsCorrelationCoefficient);
        }

        [TestMethod()]
        public void ComputeTest5()
        {
            double[][] inputs = training.Submatrix(null, 0, 3);

            int[] labels = Tools.Scale(0, 1, -1, 1, training.GetColumn(4)).ToInt32();

            Gaussian kernel = Gaussian.Estimate(inputs);

            var machine = new KernelSupportVectorMachine(kernel, inputs[0].Length);

            var smo = new SequentialMinimalOptimization(machine, inputs, labels);


            smo.Complexity = 1.0;
            smo.UseClassProportions = true;

            double error = smo.Run();

            Assert.AreEqual(1, smo.Complexity);
            Assert.AreEqual(0.4, smo.PositiveWeight);
            Assert.AreEqual(1.0, smo.NegativeWeight);
            Assert.AreEqual(0.4, smo.WeightRatio, 1e-10);
            Assert.AreEqual(0.38095238095238093, error);
            Assert.AreEqual(265.78327637381551, (machine.Kernel as Gaussian).Sigma);
            Assert.AreEqual(32, machine.SupportVectors.Length);


            int[] actual = new int[labels.Length];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = Math.Sign(machine.Compute(inputs[i]));

            ConfusionMatrix matrix = new ConfusionMatrix(actual, labels);

            Assert.AreEqual(7, matrix.FalseNegatives);
            Assert.AreEqual(9, matrix.FalsePositives);
            Assert.AreEqual(5, matrix.TruePositives);
            Assert.AreEqual(21, matrix.TrueNegatives);

            Assert.AreEqual(0.41666666666666669, matrix.Sensitivity);
            Assert.AreEqual(0.7, matrix.Specificity);
        }

        [TestMethod()]
        public void ComputeTest6()
        {
            double[][] inputs = training.Submatrix(null, 0, 3);

            int[] labels = Tools.Scale(0, 1, -1, 1, training.GetColumn(4)).ToInt32();

            Gaussian kernel = Gaussian.Estimate(inputs);

            {
                var machine = new KernelSupportVectorMachine(kernel, inputs[0].Length);
                var smo = new SequentialMinimalOptimization(machine, inputs, labels);

                smo.Complexity = 1.0;
                smo.WeightRatio = 30 / 12.0;

                double error = smo.Run();

                Assert.AreEqual(1, smo.PositiveWeight);
                Assert.AreEqual(0.4, smo.NegativeWeight);
                Assert.AreEqual(0.21428571428571427, error);
                Assert.AreEqual(265.78327637381551, (machine.Kernel as Gaussian).Sigma);
                Assert.AreEqual(34, machine.SupportVectors.Length);


                int[] actual = new int[labels.Length];
                for (int i = 0; i < actual.Length; i++)
                    actual[i] = Math.Sign(machine.Compute(inputs[i]));

                ConfusionMatrix matrix = new ConfusionMatrix(actual, labels);

                Assert.AreEqual(9, matrix.FalseNegatives);
                Assert.AreEqual(0, matrix.FalsePositives);
                Assert.AreEqual(3, matrix.TruePositives);
                Assert.AreEqual(30, matrix.TrueNegatives);

                Assert.AreEqual(0.25, matrix.Sensitivity);
                Assert.AreEqual(1.0, matrix.Specificity);

                Assert.AreEqual(0.4, matrix.FScore);
                Assert.AreEqual(0.4385290096535146, matrix.MatthewsCorrelationCoefficient);
            }

            {
                var machine = new KernelSupportVectorMachine(kernel, inputs[0].Length);
                var smo = new SequentialMinimalOptimization(machine, inputs, labels);

                smo.Complexity = 1.0;
                smo.WeightRatio = 12 / 30.0;

                double error = smo.Run();

                Assert.AreEqual(0.4, smo.PositiveWeight);
                Assert.AreEqual(1.0, smo.NegativeWeight);
                Assert.AreEqual(0.38095238095238093, error);
                Assert.AreEqual(265.78327637381551, (machine.Kernel as Gaussian).Sigma);
                Assert.AreEqual(32, machine.SupportVectors.Length);


                int[] actual = new int[labels.Length];
                for (int i = 0; i < actual.Length; i++)
                    actual[i] = Math.Sign(machine.Compute(inputs[i]));

                ConfusionMatrix matrix = new ConfusionMatrix(actual, labels);

                Assert.AreEqual(7, matrix.FalseNegatives);
                Assert.AreEqual(9, matrix.FalsePositives);
                Assert.AreEqual(5, matrix.TruePositives);
                Assert.AreEqual(21, matrix.TrueNegatives);

                Assert.AreEqual(0.41666666666666669, matrix.Sensitivity);
                Assert.AreEqual(0.7, matrix.Specificity);

                Assert.AreEqual(0.38461538461538458, matrix.FScore);
                Assert.AreEqual(0.11180339887498948, matrix.MatthewsCorrelationCoefficient);
            }
        }


      


        private static double[][] training = new double[][]
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
