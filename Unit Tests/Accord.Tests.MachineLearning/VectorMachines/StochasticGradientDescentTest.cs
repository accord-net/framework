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
    using Accord.Math;
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Kernels;
    using NUnit.Framework;
    using System;
    using Accord.Statistics.Models.Regression.Linear;
    using Math.Optimization.Losses;
    using Accord.DataSets;
    using Accord.Statistics.Filters;
    using Accord.MachineLearning;

    [TestFixture]
    public class StochasticGradientDescentTest
    {

        [Test]
        public void LearnTest()
        {

            double[][] inputs =
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            int[] xor =
            {
                -1,
                 1,
                 1,
                -1
            };

            var kernel = new Polynomial(2, 0.0);

            double[][] augmented = new double[inputs.Length][];
            for (int i = 0; i < inputs.Length; i++)
                augmented[i] = kernel.Transform(inputs[i]);

            // Create the Least Squares Support Vector Machine teacher
            var learn = new StochasticGradientDescent()
            {
                LearningRate = 1e-3
            };

            // Run the learning algorithm
            var svm = learn.Learn(augmented, xor);

            bool[] predicted = svm.Decide(augmented);
            double error = new ZeroOneLoss(xor).Loss(predicted);
            Assert.AreEqual(0, error);

            int[] output = augmented.Apply(p => Math.Sign(svm.Compute(p)));
            for (int i = 0; i < output.Length; i++)
                Assert.AreEqual(System.Math.Sign(xor[i]), System.Math.Sign(output[i]));
        }


        [Test]
        public void learn_linear_multiclass()
        {
            #region doc_learn_multiclass
            // In this example, we will learn a multi-class SVM using the one-vs-one (OvO)
            // approach. The OvO approacbh can decompose decision problems involving multiple 
            // classes into a series of binary ones, which can then be solved using SVMs.

            // Ensure we have reproducible results
            Accord.Math.Random.Generator.Seed = 0;

            // We will try to learn a classifier
            // for the Fisher Iris Flower dataset
            var iris = new Iris();
            double[][] inputs = iris.Instances; // get the flower characteristics
            int[] outputs = iris.ClassLabels;   // get the expected flower classes

            // We will use mini-batches of size 32 to learn a SVM using SGD
            var batches = MiniBatches.Create(batchSize: 32, maxIterations: 1000,
               shuffle: ShuffleMethod.EveryEpoch, input: inputs, output: outputs);

            // Now, we can create a multi-class teaching algorithm for the SVMs
            var teacher = new MulticlassSupportVectorLearning<Linear, double[]>
            {
                // We will use SGD to learn each of the binary problems in the multi-class problem
                Learner = (p) => new StochasticGradientDescent<Linear, double[], LogisticLoss>()
                {
                    LearningRate = 1e-3, 
                    MaxIterations = 1 // so the gradient is only updated once after each mini-batch
                }
            };

            // The following line is only needed to ensure reproducible results. Please remove it to enable full parallelization
            teacher.ParallelOptions.MaxDegreeOfParallelism = 1; // (Remove, comment, or change this line to enable full parallelism)

            // Now, we can start training the model on mini-batches:
            foreach (var batch in batches)
            {
                teacher.Learn(batch.Inputs, batch.Outputs);
            }

            // Get the final model:
            var svm = teacher.Model;

            // Now, we should be able to use the model to predict 
            // the classes of all flowers in Fisher's Iris dataset:
            int[] prediction = svm.Decide(inputs);

            // And from those predictions, we can compute the model accuracy:
            var cm = new GeneralConfusionMatrix(expected: outputs, predicted: prediction);
            double accuracy = cm.Accuracy; // should be approximately 0.973
            #endregion

            Assert.AreEqual(0.97333333333333338, cm.Accuracy);
            Assert.AreEqual(150, batches.NumberOfSamples);
            Assert.AreEqual(32, batches.MiniBatchSize);
            Assert.AreEqual(213, batches.CurrentEpoch);
            Assert.AreEqual(1001, batches.CurrentIteration);
            Assert.AreEqual(82, batches.CurrentSample);
        }

        [Test]
        public void learn_linear_multilabel()
        {
            #region doc_learn_multilabel
            // In this example, we will learn a multi-class SVM using the one-vs-rest (OvR)
            // approach. The OvR approacbh can decompose decision problems involving multiple 
            // classes into a series of binary ones, which can then be solved using SVMs.

            // Ensure we have reproducible results
            Accord.Math.Random.Generator.Seed = 0;

            // We will try to learn a classifier
            // for the Fisher Iris Flower dataset
            var iris = new Iris();
            double[][] inputs = iris.Instances; // get the flower characteristics
            int[] outputs = iris.ClassLabels;   // get the expected flower classes

            // We will use mini-batches of size 32 to learn a SVM using SGD
            var batches = MiniBatches.Create(batchSize: 32, maxIterations: 1000,
               shuffle: ShuffleMethod.EveryEpoch, input: inputs, output: outputs);

            // Now, we can create a multi-label teaching algorithm for the SVMs
            var teacher = new MultilabelSupportVectorLearning<Linear, double[]>
            {
                // We will use SGD to learn each of the binary problems in the multi-class problem
                Learner = (p) => new StochasticGradientDescent<Linear, double[], LogisticLoss>()
                {
                    LearningRate = 1e-3,
                    MaxIterations = 1 // so the gradient is only updated once after each mini-batch
                }
            };

            // The following line is only needed to ensure reproducible results. Please remove it to enable full parallelization
            teacher.ParallelOptions.MaxDegreeOfParallelism = 1; // (Remove, comment, or change this line to enable full parallelism)

            // Now, we can start training the model on mini-batches:
            foreach (var batch in batches)
            {
                teacher.Learn(batch.Inputs, batch.Outputs);
            }

            // Get the final model:
            var svm = teacher.Model;

            // Now, we should be able to use the model to predict 
            // the classes of all flowers in Fisher's Iris dataset:
            int[] prediction = svm.ToMulticlass().Decide(inputs);

            // And from those predictions, we can compute the model accuracy:
            var cm = new GeneralConfusionMatrix(expected: outputs, predicted: prediction);
            double accuracy = cm.Accuracy; // should be approximately 0.913
            #endregion

            Assert.AreEqual(0.91333333333333333, cm.Accuracy);
            Assert.AreEqual(150, batches.NumberOfSamples);
            Assert.AreEqual(32, batches.MiniBatchSize);
            Assert.AreEqual(213, batches.CurrentEpoch);
            Assert.AreEqual(1001, batches.CurrentIteration);
            Assert.AreEqual(82, batches.CurrentSample);
        }

    }
}
