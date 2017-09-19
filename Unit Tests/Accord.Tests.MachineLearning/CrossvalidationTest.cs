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
    using System;
    using Accord.MachineLearning;
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Math;
    using Accord.Statistics.Kernels;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Models.Markov.Learning;
    using NUnit.Framework;
    using Accord.Math.Optimization.Losses;
    using Accord.Statistics.Analysis;
    using Accord.MachineLearning.Performance;

    [TestFixture]
    public class CrossvalidationTest
    {

        [Test]
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

        [Test]
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

                return new CrossValidationValues(k, 2 * k);
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

        [Test]
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

        [Test]
        public void learn_test_simple()
        {
            #region doc_learn_simple
            // Ensure results are reproducible
            Accord.Math.Random.Generator.Seed = 0;

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
            var crossvalidation = CrossValidation.Create(
                k: 3, // Use 3 folds in cross-validation

                // Indicate how learning algorithms for the models should be created
                learner: (s) => new SequentialMinimalOptimization<Linear>()
                {
                    Complexity = 100
                },

                // Indicate how the performance of those models will be measured
                loss: (expected, actual, p) => new ZeroOneLoss(expected).Loss(actual),
                
                fit: (teacher, x, y, w) => teacher.Learn(x, y, w),
                x: data, 
                y: xor
            );

            // If needed, control the parallelization degree
            crossvalidation.ParallelOptions.MaxDegreeOfParallelism = 1;

            var result = crossvalidation.Learn(data, xor);

            // Finally, access the measured performance.
            double trainingErrors = result.Training.Mean; // 0.30606060606060609 (+/- var. 0.083498622589531682)
            double validationErrors = result.Validation.Mean; // 0.3666666666666667 (+/- var. 0.023333333333333334)

            // If desired, compute an aggregate confusion matrix for the validation sets:
            GeneralConfusionMatrix gcm = result.ToConfusionMatrix(data, xor);
            #endregion

            Assert.AreEqual(3, crossvalidation.K);
            Assert.AreEqual(0.30606060606060609, result.Training.Mean, 1e-10);
            Assert.AreEqual(0.3666666666666667, result.Validation.Mean, 1e-10);

            Assert.AreEqual(0.083498622589531682, result.Training.Variance, 1e-10);
            Assert.AreEqual(0.023333333333333334, result.Validation.Variance, 1e-10);

            Assert.AreEqual(0.28896128216342704, result.Training.StandardDeviation, 1e-10);
            Assert.AreEqual(0.15275252316519467, result.Validation.StandardDeviation, 1e-10);

            Assert.AreEqual(0, result.Training.PooledStandardDeviation);
            Assert.AreEqual(0, result.Validation.PooledStandardDeviation);

            Assert.AreEqual(3, crossvalidation.Folds.Length);
            Assert.AreEqual(3, result.Models.Length);
        }

        [Test]
        public void learn_test()
        {
            #region doc_learn
            // Ensure results are reproducible
            Accord.Math.Random.Generator.Seed = 0;

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
            var crossvalidation = new CrossValidation<SupportVectorMachine<Linear, double[]>, double[]>()
            {
                K = 3, // Use 3 folds in cross-validation

                // Indicate how learning algorithms for the models should be created
                Learner = (s) => new SequentialMinimalOptimization<Linear, double[]>()
                {
                    Complexity = 100
                },

                // Indicate how the performance of those models will be measured
                Loss = (expected, actual, p) => new ZeroOneLoss(expected).Loss(actual),

                Stratify = false, // do not force balancing of classes
            };

            // If needed, control the parallelization degree
            crossvalidation.ParallelOptions.MaxDegreeOfParallelism = 1;

            // Compute the cross-validation
            var result = crossvalidation.Learn(data, xor);

            // Finally, access the measured performance.
            double trainingErrors = result.Training.Mean; // should be 0.30606060606060609 (+/- var. 0.083498622589531682)
            double validationErrors = result.Validation.Mean; // should be 0.3666666666666667 (+/- var. 0.023333333333333334)

            // If desired, compute an aggregate confusion matrix for the validation sets:
            GeneralConfusionMatrix gcm = result.ToConfusionMatrix(data, xor);
            double accuracy = gcm.Accuracy; // should be 0.625
            double error = gcm.Error; // should be 0.375
            #endregion

            Assert.AreEqual(16, gcm.Samples);
            Assert.AreEqual(0.625, accuracy);
            Assert.AreEqual(0.375, error);
            Assert.AreEqual(2, gcm.Classes);

            Assert.AreEqual(3, crossvalidation.K);
            Assert.AreEqual(0.30606060606060609, result.Training.Mean, 1e-10);
            Assert.AreEqual(0.3666666666666667, result.Validation.Mean, 1e-10);

            Assert.AreEqual(0.083498622589531682, result.Training.Variance, 1e-10);
            Assert.AreEqual(0.023333333333333334, result.Validation.Variance, 1e-10);

            Assert.AreEqual(0.28896128216342704, result.Training.StandardDeviation, 1e-10);
            Assert.AreEqual(0.15275252316519467, result.Validation.StandardDeviation, 1e-10);

            Assert.AreEqual(0, result.Training.PooledStandardDeviation);
            Assert.AreEqual(0, result.Validation.PooledStandardDeviation);

            Assert.AreEqual(3, crossvalidation.Folds.Length);
            Assert.AreEqual(3, result.Models.Length);
        }

        [Test]
        public void learn_hmm()
        {
            #region doc_learn_hmm
            // Ensure results are reproducible
            Accord.Math.Random.Generator.Seed = 0;

            // This is a sample code on how to use Cross-Validation
            // to assess the performance of Hidden Markov Models.

            // Declare some testing data
            int[][] inputs = new int[][]
            {
                new int[] { 0,1,1,0 },   // Class 0
                new int[] { 0,0,1,0 },   // Class 0
                new int[] { 0,1,1,1,0 }, // Class 0
                new int[] { 0,1,1,1,0 }, // Class 0
                new int[] { 0,1,1,0 },   // Class 0

                new int[] { 0,0,0,0,0 }, // Class 1
                new int[] { 0,0,0,1,0 }, // Class 1
                new int[] { 0,0,0,0,0 }, // Class 1
                new int[] { 0,0,0 },     // Class 1
                new int[] { 0,0,0,0 },   // Class 1

                new int[] { 1,0,0,1 },   // Class 2
                new int[] { 1,1,0,1 },   // Class 2
                new int[] { 1,0,0,0,1 }, // Class 2
                new int[] { 1,0,1 },     // Class 2
                new int[] { 1,1,0,1 },   // Class 2
            };

            int[] outputs = new int[]
            {
                0,0,0,0,0, // First  5 sequences are of class 0
                1,1,1,1,1, // Middle 5 sequences are of class 1
                2,2,2,2,2, // Last   5 sequences are of class 2
            };

            // Create a new Cross-validation algorithm passing the data set size and the number of folds
            var crossvalidation = new CrossValidation<HiddenMarkovClassifier, int[]>()
            {
                K = 3, // Use 3 folds in cross-validation
                Learner = (s) => new HiddenMarkovClassifierLearning()
                {
                    Learner = (p) => new BaumWelchLearning()
                    {
                        NumberOfStates = 3
                    }
                },

                Loss = (expected, actual, p) => 
                {
                    var cm = new GeneralConfusionMatrix(classes: p.Model.NumberOfClasses, expected: expected, predicted: actual);
                    p.Variance = cm.Variance;
                    return p.Value = cm.Kappa;
                },

                Stratify = false,
            };

            // If needed, control the parallelization degree
            crossvalidation.ParallelOptions.MaxDegreeOfParallelism = 1;

            // Compute the cross-validation
            var result = crossvalidation.Learn(inputs, outputs);

            // If desired, compute an aggregate confusion matrix for the validation sets:
            GeneralConfusionMatrix gcm = result.ToConfusionMatrix(inputs, outputs);

            // Finally, access the measured performance.
            double trainingErrors = result.Training.Mean;
            double validationErrors = result.Validation.Mean;

            double trainingErrorVar = result.Training.Variance;
            double validationErrorVar = result.Validation.Variance;

            double trainingErrorPooledVar = result.Training.PooledVariance;
            double validationErrorPooledVar = result.Validation.PooledVariance;
            #endregion

            Assert.AreEqual(5, result.AverageNumberOfSamples);
            Assert.AreEqual(15, result.NumberOfSamples);

            Assert.AreEqual(0, result.NumberOfInputs);
            Assert.AreEqual(3, result.NumberOfOutputs);
            Assert.AreEqual(3, result.Models.Length);

            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(0, result.Models[i].NumberOfInputs);
                Assert.AreEqual(3, result.Models[i].NumberOfOutputs);
                Assert.AreEqual(3, result.Models[i].Model.NumberOfClasses);
                Assert.AreEqual(0, result.Models[i].Model.NumberOfInputs);
                Assert.AreEqual(3, result.Models[i].Model.NumberOfOutputs);

                Assert.AreEqual(10, result.Models[i].Training.NumberOfSamples);
                Assert.AreEqual(5, result.Models[i].Validation.NumberOfSamples);
            }

            Assert.AreEqual(3, crossvalidation.K);
            Assert.AreEqual(1.0, result.Training.Mean, 1e-10);
            Assert.AreEqual(0.78472222222222232, result.Validation.Mean, 1e-10);

            Assert.AreEqual(0, result.Training.Variance, 1e-10);
            Assert.AreEqual(0.034866898148148126, result.Validation.Variance, 1e-10);

            Assert.AreEqual(0, result.Training.StandardDeviation, 1e-10);
            Assert.AreEqual(0.18672680082984372, result.Validation.StandardDeviation, 1e-10);

            Assert.AreEqual(0, result.Training.PooledVariance);
            Assert.AreEqual(0.045256528501157391, result.Validation.PooledVariance);

            Assert.AreEqual(0, result.Training.PooledStandardDeviation);
            Assert.AreEqual(0.21273581856649668, result.Validation.PooledStandardDeviation);

            Assert.AreEqual(3, crossvalidation.Folds.Length);
            Assert.AreEqual(3, result.Models.Length);
        }

        [Test]
        public void CrossvalidationConstructorTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            // This is a sample code on how to use Cross-Validation
            // to assess the performance of Hidden Markov Models.

            // Declare some testing data
            int[][] inputs = new int[][]
            {
                new int[] { 0,1,1,0 },   // Class 0
                new int[] { 0,0,1,0 },   // Class 0
                new int[] { 0,1,1,1,0 }, // Class 0
                new int[] { 0,1,1,1,0 }, // Class 0
                new int[] { 0,1,1,0 },   // Class 0
                new int[] { 0,1,1,1,0 }, // Class 0
                new int[] { 0,1,1,1,0 }, // Class 0
                new int[] { 0,1,0,1,0 }, // Class 0
                new int[] { 0,1,0 },     // Class 0
                new int[] { 0,1,1,0 },   // Class 0

                new int[] { 1,0,0,1 },   // Class 1
                new int[] { 1,1,0,1 },   // Class 1
                new int[] { 1,0,0,0,1 }, // Class 1
                new int[] { 1,0,1 },     // Class 1
                new int[] { 1,1,0,1 },   // Class 1
                new int[] { 1,0,1 },     // Class 1
                new int[] { 1,0,0,1 },   // Class 1
                new int[] { 1,0,0,0,1 }, // Class 1
                new int[] { 1,0,1 },     // Class 1
                new int[] { 1,0,0,0,1 }, // Class 1
            };

            int[] outputs = new int[]
            {
                0,0,0,0,0,0,0,0,0,0, // First 10 sequences are of class 0
                1,1,1,1,1,1,1,1,1,1, // Last 10 sequences are of class 1
            };


            // Create a new Cross-validation algorithm passing the data set size and the number of folds
            var crossvalidation = new CrossValidation<HiddenMarkovClassifier>(size: inputs.Length, folds: 3);

            // Define a fitting function using Support Vector Machines. The objective of this
            // function is to learn a SVM in the subset of the data indicated by cross-validation.

            crossvalidation.Fitting = delegate(int k, int[] indicesTrain, int[] indicesValidation)
            {
                // The fitting function is passing the indices of the original set which
                // should be considered training data and the indices of the original set
                // which should be considered validation data.

                // Lets now grab the training data:
                var trainingInputs = inputs.Submatrix(indicesTrain);
                var trainingOutputs = outputs.Submatrix(indicesTrain);

                // And now the validation data:
                var validationInputs = inputs.Submatrix(indicesValidation);
                var validationOutputs = outputs.Submatrix(indicesValidation);


                // We are trying to predict two different classes
                int classes = 2;

                // Each sequence may have up to two symbols (0 or 1)
                int symbols = 2;

                // Nested models will have two states each
                int[] states = new int[] { 2, 2 };

                // Creates a new Hidden Markov Model Classifier with the given parameters
                HiddenMarkovClassifier classifier = new HiddenMarkovClassifier(classes, states, symbols);


                // Create a new learning algorithm to train the sequence classifier
                var teacher = new HiddenMarkovClassifierLearning(classifier,

                    // Train each model until the log-likelihood changes less than 0.001
                    modelIndex => new BaumWelchLearning(classifier.Models[modelIndex])
                    {
                        Tolerance = 0.001,
                        Iterations = 0
                    }
                );

                // Train the sequence classifier using the algorithm
                double likelihood = teacher.Run(trainingInputs, trainingOutputs);

                double trainingError = teacher.ComputeError(trainingInputs, trainingOutputs);

                // Now we can compute the validation error on the validation data:
                double validationError = teacher.ComputeError(validationInputs, validationOutputs);

                // Return a new information structure containing the model and the errors achieved.
                return new CrossValidationValues<HiddenMarkovClassifier>(classifier, trainingError, validationError);
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

        [Test]
        public void NotEnoughSamplesTest1()
        {
            Accord.Math.Random.Generator.Seed = 0;

            int[] labels = Matrix.Vector(10, 1).Concatenate(Matrix.Vector(30, 0));

            Vector.Shuffle(labels);

            var crossvalidation = new CrossValidation<MulticlassSupportVectorMachine>(size: 40, folds: 10)
            {
                RunInParallel = false,

                Fitting = (int index, int[] indicesTrain, int[] indicesValidation) =>
                {
                    var labelsValidation = labels.Submatrix(indicesValidation);
                    int countValidation = labelsValidation.Count(x => x == 1);
                    Assert.AreEqual(2, countValidation);

                    var labelsTraining = labels.Submatrix(indicesTrain);
                    int countTraining = labelsTraining.Count(x => x == 1);
                    Assert.AreEqual(9 * 2, countTraining);

                    return new CrossValidationValues<MulticlassSupportVectorMachine>(null, 0, 0);
                }
            };

            bool thrown = false;
            try { crossvalidation.Compute(); }
            catch (Exception) { thrown = true; }
            Assert.IsTrue(thrown);

            crossvalidation = new CrossValidation<MulticlassSupportVectorMachine>(labels, 2, folds: 10)
            {
                RunInParallel = false,

                Fitting = (int index, int[] indicesTrain, int[] indicesValidation) =>
                {
                    var labelsValidation = labels.Submatrix(indicesValidation);
                    int countValidation = labelsValidation.Count(x => x == 1);
                    Assert.AreEqual(1, countValidation);

                    var labelsTraining = labels.Submatrix(indicesTrain);
                    int countTraining = labelsTraining.Count(x => x == 1);
                    Assert.AreEqual(9, countTraining);

                    return new CrossValidationValues<MulticlassSupportVectorMachine>(null, 0, 0);
                }
            };

            crossvalidation.Compute();
        }

        [Test]
        public void NotEnoughSamplesTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            int[] labels = Matrix.Vector(10, 1).Concatenate(Matrix.Vector(30, 0));

            Vector.Shuffle(labels);

            var crossvalidation = new CrossValidation<MulticlassSupportVectorMachine>(labels, 2, folds: 10)
            {
                RunInParallel = false,

                Fitting = (int index, int[] indicesTrain, int[] indicesValidation) =>
                {
                    var labelsValidation = labels.Submatrix(indicesValidation);
                    int countValidation = labelsValidation.Count(x => x == 1);
                    Assert.AreEqual(1, countValidation);

                    var labelsTraining = labels.Submatrix(indicesTrain);
                    int countTraining = labelsTraining.Count(x => x == 1);
                    Assert.AreEqual(9, countTraining);

                    return new CrossValidationValues<MulticlassSupportVectorMachine>(null, 0, 0);
                }
            };

            crossvalidation.Compute();
        }
    }
}
