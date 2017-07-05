﻿// Accord Unit Tests
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
    using Accord.MachineLearning;
    using NUnit.Framework;
    using System;
    using Accord.Math;
    using Accord.Statistics;
    using Accord.MachineLearning.VectorMachines;
    using Accord.Statistics.Kernels;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Statistics.Analysis;
    using Accord.Math.Optimization.Losses;
    using Accord.MachineLearning.Performance;

    [TestFixture]
    public class SplitSetTest
    {

        [Test]
        public void learn_test()
        {
            #region doc_learn
            // Ensure results are reproducible
            Accord.Math.Random.Generator.Seed = 0;

            // This is a sample code on how to use Train-Val validation (split-set)
            // to assess the performance of Support Vector Machines.

            // Consider the example binary data. We will be trying to learn a XOR 
            // problem and see how well does SVMs perform on this data.

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
            var splitset = new SplitSetValidation<SupportVectorMachine<Linear, double[]>, double[]>()
            {
                Learner = (s) => new SequentialMinimalOptimization<Linear, double[]>()
                {
                    Complexity = 1000
                },

                Loss = (expected, actual, p) => new ZeroOneLoss(expected).Loss(actual),

                Stratify = false,
            };

            splitset.ParallelOptions.MaxDegreeOfParallelism = 1;

            // Compute the cross-validation
            var result = splitset.Learn(data, xor);

            // Finally, access the measured performance.
            double trainingErrors = result.Training.Value;
            double validationErrors = result.Validation.Value;
            #endregion

            Assert.AreEqual(0.2, splitset.ValidationSetProportion, 1e-10);

            Assert.AreEqual(0.2, splitset.ValidationSetProportion, 1e-6);
            Assert.AreEqual(0.8, splitset.TrainingSetProportion, 1e-6);

            Assert.AreEqual(0.5, result.Training.Value, 1e-10);
            Assert.AreEqual(0.5, result.Validation.Value, 1e-10);

            Assert.AreEqual(0, result.Training.Variance, 1e-10);
            Assert.AreEqual(0, result.Validation.Variance, 1e-10);

            Assert.AreEqual(0, result.Training.StandardDeviation, 1e-10);
            Assert.AreEqual(0, result.Validation.StandardDeviation, 1e-10);

            Assert.AreEqual(0.75, result.Training.Proportion);
            Assert.AreEqual(0.25, result.Validation.Proportion);

            Assert.AreEqual(16, result.NumberOfSamples);
            Assert.AreEqual(8, result.AverageNumberOfSamples);
        }

        [Test]
        public void SplitSetConstructorTest1()
        {

            Accord.Math.Random.Generator.Seed = 0;

            // This is a sample code on how to use two split sets
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


            // Create a new split set validation algorithm passing the set size and the split set proportion
            var splitset = new SplitSetValidation<KernelSupportVectorMachine>(size: data.Length, proportion: 0.4);

            // Define a fitting function using Support Vector Machines. The objective of this
            // function is to learn a SVM in the subset of the data indicated by the split sets.

            splitset.Fitting = delegate(int[] indicesTrain)
            {
                // The fitting function is passing the indices of the original set which
                // should be considered training data and the indices of the original set
                // which should be considered validation data.

                // Lets now grab the training data:
                var trainingInputs = data.Submatrix(indicesTrain);
                var trainingOutputs = xor.Submatrix(indicesTrain);

                // Create a Kernel Support Vector Machine to operate on the set
                var svm = new KernelSupportVectorMachine(new Polynomial(2), 2);

                // Create a training algorithm and learn the training data
                var smo = new SequentialMinimalOptimization(svm, trainingInputs, trainingOutputs);

                double trainingError = smo.Run();

                // Compute results for the training set
                int[] computedOutputs = trainingInputs.Apply(svm.Compute).Apply(Math.Sign);

                // Compute the absolute error
                int[] errors = (computedOutputs.Subtract(trainingOutputs)).Abs();

                // Retrieve error statistics
                double mean = errors.Mean();
                double variance = errors.Variance();

                // Return a new information structure containing the model and the errors.
                return SplitSetStatistics.Create(svm, trainingInputs.Length, mean, variance);

            };

            splitset.Evaluation = delegate(int[] indicesValidation, KernelSupportVectorMachine svm)
            {
                // Lets now grab the training data:
                var validationInputs = data.Submatrix(indicesValidation);
                var validationOutputs = xor.Submatrix(indicesValidation);

                // Compute results for the validation set
                int[] computedOutputs = validationInputs.Apply(svm.Compute).Apply(Math.Sign);

                // Compute the absolute error
                int[] errors = (computedOutputs.Subtract(validationOutputs)).Abs();

                // Retrieve error statistics
                double mean = errors.Mean();
                double variance = errors.Variance();

                // Return a new information structure containing the model and the errors.
                return SplitSetStatistics.Create(svm, validationInputs.Length, mean, variance);
            };


            // Compute the bootstrap estimate
            var result = splitset.Compute();

            // Finally, access the measured performance.
            double trainingErrors = result.Training.Value;
            double validationErrors = result.Validation.Value;

            Assert.AreEqual(0, trainingErrors);
            Assert.AreEqual(0, validationErrors);
        }

    }
}
