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
    using Accord.MachineLearning;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Math;
    using Accord.Statistics;
    using Accord.MachineLearning.VectorMachines;
    using Accord.Statistics.Kernels;
    using Accord.MachineLearning.VectorMachines.Learning;


    [TestClass()]
    public class BootstrapTest
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
        public void BootstrapConstructorTest()
        {
            // Example from Masters, 1995

            // Assume a small dataset
            double[] data = { 3, 5, 2, 1, 7 };

            int size = data.Length;
            int resamplings = 3;
            int subsampleSize = data.Length;


            Bootstrap target = new Bootstrap(size, resamplings, subsampleSize);

            Assert.AreEqual(3, target.B);
            Assert.AreEqual(3, target.Subsamples.Length);

            for (int i = 0; i < target.Subsamples.Length; i++)
                Assert.AreEqual(5, target.Subsamples[i].Length);
        }

        [TestMethod()]
        public void BootstrapConstructorTest2()
        {
            // Example from Masters, 1995

            // Assume a small dataset
            double[] data = { 3, 5, 2, 1, 7 };
            // indices      { 0, 1, 2, 3, 4 }

            int[][] resamplings = 
            {
                new [] { 4, 0, 2, 0, 3 }, // indices of { 7, 3, 2, 3, 1 }
                new [] { 1, 3, 3, 0, 4 }, // indices of { 5, 1, 1, 3, 7 }
                new [] { 2, 2, 4, 3, 0 }, // indices of { 2, 2, 7, 1, 3 }
            };

            Bootstrap target = new Bootstrap(data.Length, resamplings);

            target.Fitting = (int[] trainingSamples, int[] validationSamples) =>
                {
                    double[] subsample = data.Submatrix(trainingSamples);
                    double mean = subsample.Mean();

                    return new BootstrapValues(mean, 0);
                };

            var result = target.Compute();

            double actualMean = result.Training.Mean;
            double actualVar = result.Training.Variance;

            Assert.AreEqual(3.2, actualMean, 1e-10);
            Assert.AreEqual(0.04, actualVar, 1e-10);
        }

        [TestMethod()]
        public void BootstrapConstructorTest3()
        {

            Accord.Math.Tools.SetupGenerator(0);

            // This is a sample code on how to use 0.632 Bootstrap
            // to assess the performance of Support Vector Machines.

            // Consider the example binary data. We will be trying
            // to learn a XOR problem and see how well does SVMs
            // perform on this data.

            double[][] data =
            {
                new double[] { -1, -1 }, new double[] {  1, -1 },
                new double[] { -1,  1 }, new double[] {  1,  1 },
                new double[] { -1, -1 }, new double[] {  1, -1 },
                new double[] { -1,  1 }, new double[] {  1,  1 },
                new double[] { -1, -1 }, new double[] {  1, -1 },
                new double[] { -1,  1 }, new double[] {  1,  1 },
                new double[] { -1, -1 }, new double[] {  1, -1 },
                new double[] { -1,  1 }, new double[] {  1,  1 },
            };

            int[] xor = // result of xor for the sample input data
            {
                -1,       1,
                 1,      -1,
                -1,       1,
                 1,      -1,
                -1,       1,
                 1,      -1,
                -1,       1,
                 1,      -1,
            };


            // Create a new Bootstrap algorithm passing the set size and the number of resamplings
            var bootstrap = new Bootstrap(size: data.Length, subsamples: 50);

            // Define a fitting function using Support Vector Machines. The objective of this
            // function is to learn a SVM in the subset of the data indicated by the bootstrap.

            bootstrap.Fitting = delegate(int[] indicesTrain, int[] indicesValidation)
            {
                // The fitting function is passing the indices of the original set which
                // should be considered training data and the indices of the original set
                // which should be considered validation data.

                // Lets now grab the training data:
                var trainingInputs = data.Submatrix(indicesTrain);
                var trainingOutputs = xor.Submatrix(indicesTrain);

                // And now the validation data:
                var validationInputs = data.Submatrix(indicesValidation);
                var validationOutputs = xor.Submatrix(indicesValidation);


                // Create a Kernel Support Vector Machine to operate on the set
                var svm = new KernelSupportVectorMachine(new Polynomial(2), 2);

                // Create a training algorithm and learn the training data
                var smo = new SequentialMinimalOptimization(svm, trainingInputs, trainingOutputs);

                double trainingError = smo.Run();

                // Now we can compute the validation error on the validation data:
                double validationError = smo.ComputeError(validationInputs, validationOutputs);

                // Return a new information structure containing the model and the errors achieved.
                return new BootstrapValues(trainingError, validationError);
            };


            // Compute the bootstrap estimate
            var result = bootstrap.Compute();

            // Finally, access the measured performance.
            double trainingErrors = result.Training.Mean;
            double validationErrors = result.Validation.Mean;

            // And compute the 0.632 estimate
            double estimate = result.Estimate;

            Assert.AreEqual(50, bootstrap.B);
            Assert.AreEqual(0, trainingErrors);
            Assert.AreEqual(0.021428571428571429, validationErrors);

            Assert.AreEqual(50, bootstrap.Subsamples.Length);
            Assert.AreEqual(0.013542857142857143, estimate);
        }

    }
}
