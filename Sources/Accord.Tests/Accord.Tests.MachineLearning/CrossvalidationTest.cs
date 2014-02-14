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
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Math;
    using Accord.Statistics.Kernels;

    [TestClass()]
    public class CrossvalidationTest
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
        public void SplittingsTest()
        {

            int[] folds = CrossValidation.Splittings(100, 10);

            for (int i = 0; i < 10; i++)
            {
                int actual = folds.Count(x => x == i);
                int expected = 10;

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod()]
        public void FittingTest()
        {

            int[] folds = CrossValidation.Splittings(100, 10);

            int[] samples = Matrix.Indices(0, 100);

            CrossValidation val = new CrossValidation(folds, 10);

            val.RunInParallel = false;

            int current = 0;
            val.Fitting = (k, trainingSamples, validationSamples) =>
            {
                Assert.AreEqual(current, k);
                Assert.AreEqual(90, trainingSamples.Length);
                Assert.AreEqual(10, validationSamples.Length);

                int[] trainingSet = samples.Submatrix(trainingSamples);
                int[] validationSet = samples.Submatrix(validationSamples);

                for (int i = 0; i < trainingSet.Length; i++)
                    Assert.AreEqual(samples[trainingSamples[i]], trainingSet[i]);

                for (int i = 0; i < validationSet.Length; i++)
                    Assert.AreEqual(samples[validationSamples[i]], validationSet[i]);

                current++;

                return new CrossValidationValues<object>(new object(), k, 2 * k);
            };

            var result = val.Compute();

            Assert.AreEqual(10, current);
            Assert.AreEqual(4.5, result.Training.Mean);
            Assert.AreEqual(9.0, result.Validation.Mean);
            Assert.AreEqual(
                2 * result.Training.StandardDeviation,      
                result.Validation.StandardDeviation);

            Assert.AreEqual(val.Folds.Length, result.Training.Sizes.Length);
            Assert.AreEqual(val.Folds.Length, result.Validation.Sizes.Length);

            for (int i = 0; i < result.Training.Sizes.Length; i++)
                Assert.AreEqual(90, result.Training.Sizes[i]);

            for (int i = 0; i < result.Validation.Sizes.Length; i++)
                Assert.AreEqual(10, result.Validation.Sizes[i]);
        }

        [TestMethod()]
        public void CrossvalidationConstructorTest()
        {

            Accord.Math.Tools.SetupGenerator(0);

            // This is a sample code on how to use Cross-Validation
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


            // Create a new Cross-validation algorithm passing the data set size and the number of folds
            var crossvalidation = new CrossValidation<KernelSupportVectorMachine>(size: data.Length, folds: 3);

            // Define a fitting function using Support Vector Machines. The objective of this
            // function is to learn a SVM in the subset of the data indicated by cross-validation.

            crossvalidation.Fitting = delegate(int k, int[] indicesTrain, int[] indicesValidation)
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
                return new CrossValidationValues<KernelSupportVectorMachine>(svm, trainingError, validationError);
            };


            // Compute the cross-validation
            var result = crossvalidation.Compute();

            // Finally, access the measured performance.
            double trainingErrors = result.Training.Mean;
            double validationErrors = result.Validation.Mean;

            Assert.AreEqual(3, crossvalidation.K);
            Assert.AreEqual(0, result.Training.Mean);
            Assert.AreEqual(0, result.Validation.Mean);

            Assert.AreEqual(3, crossvalidation.Folds.Length);
            Assert.AreEqual(3, result.Models.Length);
        }

    }
}
