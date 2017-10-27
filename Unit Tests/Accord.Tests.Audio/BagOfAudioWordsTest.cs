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

namespace Accord.Tests.Audio
{
    using Accord.Audio;
    using NUnit.Framework;
    using Accord.Audio.Windows;
    using Accord.Math;
    using System.IO;
    using Accord.DataSets;
    using Accord.Audition;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.MachineLearning.VectorMachines;
    using Accord.Statistics.Analysis;
    using Accord.MachineLearning;
    using Accord.Statistics.Kernels;

    [TestFixture]
    public class BagOfAudioWordsTest
    {
        [Test]
        public void learn()
        {
            string basePath = Path.Combine(NUnit.Framework.TestContext.CurrentContext.TestDirectory, "learn");

            #region doc_learn
            // Ensure results are reproducible
            Accord.Math.Random.Generator.Seed = 0;

            // The Bag-of-Audio-Words model converts audio signals of arbitrary 
            // size into fixed-length feature vectors. In this example, we
            // will be setting the codebook size to 10. This means all feature
            // vectors that will be generated will have the same length of 10.

            // By default, the BoW object will use the MFCC extractor as the 
            // feature extractor and K-means as the clustering algorithm.

            // Create a new Bag-of-Audio-Words (BoW) model
            var bow = BagOfAudioWords.Create(numberOfWords: 32);
            // Note: a simple BoW model can also be created using
            // var bow = new BagOfAudioWords(numberOfWords: 10);

            // Get some training images
            FreeSpokenDigitsDataset fsdd = new FreeSpokenDigitsDataset(basePath);
            string[] trainFileNames = fsdd.Training.LocalPaths;
            int[] trainOutputs = fsdd.Training.Digits;

            // Compute the model
            bow.Learn(trainFileNames);

            // After this point, we will be able to translate
            // the signals into double[] feature vectors using
            double[][] trainInputs = bow.Transform(trainFileNames);

            // We can also check some statistics about the dataset:
            int numberOfSignals = bow.Statistics.TotalNumberOfInstances; // 1350

            // Statistics about all the descriptors that have been extracted:
            int totalDescriptors = bow.Statistics.TotalNumberOfDescriptors; // 29106
            double totalMean = bow.Statistics.TotalNumberOfDescriptorsPerInstance.Mean; // 21.56
            double totalVar = bow.Statistics.TotalNumberOfDescriptorsPerInstance.Variance; // 52.764002965159314
            IntRange totalRange = bow.Statistics.TotalNumberOfDescriptorsPerInstanceRange; // [8, 115]

            // Statistics only about the descriptors that have been actually used:
            int takenDescriptors = bow.Statistics.NumberOfDescriptorsTaken; // 29106
            double takenMean = bow.Statistics.NumberOfDescriptorsTakenPerInstance.Mean; // 21.56
            double takenVar = bow.Statistics.NumberOfDescriptorsTakenPerInstance.Variance; // 52.764002965159314
            IntRange takenRange = bow.Statistics.NumberOfDescriptorsTakenPerInstanceRange; // [8, 115]
            #endregion

            Assert.AreEqual(1350, numberOfSignals);

            Assert.AreEqual(29106, totalDescriptors);
            Assert.AreEqual(21.56, totalMean);
            Assert.AreEqual(52.764002965159314, totalVar, 1e-8);
            Assert.AreEqual(new IntRange(8, 115), totalRange);

            Assert.AreEqual(29106, takenDescriptors);
            Assert.AreEqual(21.56, takenMean);
            Assert.AreEqual(52.764002965159314, takenVar, 1e-8);
            Assert.AreEqual(new IntRange(8, 115), takenRange);


            var kmeans = bow.Clustering as KMeans;
            Assert.AreEqual(13, kmeans.Clusters.NumberOfInputs);
            Assert.AreEqual(32, kmeans.Clusters.NumberOfOutputs);
            Assert.AreEqual(32, kmeans.Clusters.NumberOfClasses);

            #region doc_classification

            // Now, the features can be used to train any classification
            // algorithm as if they were the signals themselves. For example,
            // we can use them to train an Chi-square SVM as shown below:

            // Create the SMO algorithm to learn a Chi-Square kernel SVM
            var teacher = new MulticlassSupportVectorLearning<ChiSquare>()
            {
                Learner = (p) => new SequentialMinimalOptimization<ChiSquare>()
            };

            // Obtain a learned machine
            var svm = teacher.Learn(trainInputs, trainOutputs);

            // Use the machine to classify the features
            int[] output = svm.Decide(trainInputs);

            // Compute the error between the expected and predicted labels for the training set:
            var trainMetrics = GeneralConfusionMatrix.Estimate(svm, trainInputs, trainOutputs);
            double trainAcc = trainMetrics.Accuracy; // should be around 0.97259259259259256

            // Now, we can evaluate the performance of the model on the testing set:
            string[] testFileNames = fsdd.Testing.LocalPaths;
            int[] testOutputs = fsdd.Testing.Digits;

            // First we transform the testing set to double[]:
            double[][] testInputs = bow.Transform(testFileNames);

            // Then we compute the error between expected and predicted for the testing set:
            var testMetrics = GeneralConfusionMatrix.Estimate(svm, testInputs, testOutputs);
            double testAcc = testMetrics.Accuracy; // should be around 0.8666666666666667
            #endregion

            Assert.AreEqual(0.97259259259259256, trainAcc, 1e-8);
            Assert.AreEqual(0.8666666666666667, testAcc, 1e-8);
        }
    }
}
