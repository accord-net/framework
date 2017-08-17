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

namespace Accord.Tests.Imaging
{
    using Accord;
    using Accord.Imaging;
    using Accord.MachineLearning;
    using Accord.Math;
    using Accord.Math.Distances;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using Accord.MachineLearning.VectorMachines;
    using Accord.MachineLearning.VectorMachines.Learning;
    using Accord.Statistics.Kernels;
    using Accord.Math.Optimization.Losses;
    using Accord.Math.Metrics;
    using Accord.Tests.Vision.Properties;
#if NO_BITMAP
    using Resources = Accord.Tests.Vision.Properties.Resources_Standard;
#endif

    [TestFixture]
    public class BagOfVisualWordsTest
    {

        // Load some test images
        public static Bitmap[] GetImages()
        {
            Bitmap[] images =
            {
                Accord.Imaging.Image.Clone(Resources.flower01),
                Accord.Imaging.Image.Clone(Resources.flower02),
                Accord.Imaging.Image.Clone(Resources.flower03),
                Accord.Imaging.Image.Clone(Resources.flower04),
                Accord.Imaging.Image.Clone(Resources.flower05),
                Accord.Imaging.Image.Clone(Resources.flower06),
            };

            return images;
        }

        [Test]
        [Category("Random")]
        public void BagOfVisualWordsConstructorTest()
        {
            Accord.Math.Random.Generator.Seed = 0;

            var images = GetImages();

            BagOfVisualWords bow = new BagOfVisualWords(10);
            bow.ParallelOptions.MaxDegreeOfParallelism = 1;
#pragma warning disable 612, 618
            var points = bow.Compute(images, 1e-3);
#pragma warning restore 612, 618
            Assert.AreEqual(10, bow.NumberOfWords);
            Assert.AreEqual(6, points.Length);

            Assert.AreEqual(409, points[0].Count);
            Assert.AreEqual(727, points[1].Count);
            Assert.AreEqual(552, points[2].Count);
            Assert.AreEqual(460, points[3].Count);
            Assert.AreEqual(719, points[4].Count);
            Assert.AreEqual(1265, points[5].Count);

            double tol = 1e-5;
            Assert.AreEqual(388.04225639880224, points[0][0].X, tol);
            Assert.AreEqual(105.9954439039073, points[0][0].Y, tol);

            Assert.AreEqual(335.62395561144433, points[3][7].X, tol);
            Assert.AreEqual(152.14505651866821, points[2][3].Y, tol);

            Assert.AreEqual(573.691355494602, points[2][52].X, tol);
            Assert.AreEqual(153.6650841848263, points[1][11].Y, tol);

            Assert.AreEqual(289.54728415724327, points[0][42].X, tol);
            Assert.AreEqual(373.99402540151056, points[4][125].Y, tol);

            foreach (var point in points)
            {
                foreach (var p in point)
                {
                    Assert.IsFalse(double.IsNaN(p.X));
                    Assert.IsFalse(double.IsNaN(p.Y));
                }
            }
        }

        [Test]
        [Category("Random")]
        public void BagOfVisualWordsConstructorTest3()
        {
            Accord.Math.Random.Generator.Seed = 0;

            var images = GetImages();

            MoravecCornersDetector moravec = new MoravecCornersDetector();
            CornerFeaturesDetector detector = new CornerFeaturesDetector(moravec);

            var bow = new BagOfVisualWords<CornerFeaturePoint>(detector, numberOfWords: 10);
            bow.ParallelOptions.MaxDegreeOfParallelism = 1;

#pragma warning disable 612, 618
            var points = bow.Compute(images, 1e-3);
#pragma warning restore 612, 618
            double[] vector = bow.GetFeatureVector(images[0]);

            Assert.AreEqual(10, bow.NumberOfWords);
            Assert.AreEqual(6, points.Length);
            Assert.AreEqual(10, vector.Length);

            Assert.AreEqual(2800, points[0].Count);
            Assert.AreEqual(4532, points[1].Count);
            Assert.AreEqual(2282, points[2].Count);
            Assert.AreEqual(1173, points[3].Count);
            Assert.AreEqual(4860, points[4].Count);
            Assert.AreEqual(5730, points[5].Count);

            Assert.AreEqual(596, points[0][0].Descriptor[0]);
            Assert.AreEqual(51, points[0][0].Descriptor[1]);
            Assert.AreEqual(points[0][0].Descriptor[0], points[0][0].X);
            Assert.AreEqual(points[0][0].Descriptor[1], points[0][0].Y);

            Assert.AreEqual(461, points[3][7].Descriptor[0]);
            Assert.AreEqual(8, points[2][3].Descriptor[1]);

            Assert.AreEqual(991, points[2][52].Descriptor[0]);
            Assert.AreEqual(82, points[1][11].Descriptor[1]);

            Assert.AreEqual(430, points[0][42].Descriptor[0]);
            Assert.AreEqual(135, points[4][125].Descriptor[1]);
        }

        [Test]
        [Category("Random")]
#if NET35
        [Ignore("Random behaviour differs in net35.")]
#endif
        public void GetFeatureVectorTest()
        {
            var images = GetImages();

            Accord.Math.Random.Generator.Seed = 0;

            // The Bag-of-Visual-Words model converts arbitrary-size images 
            // into fixed-length feature vectors. In this example, we will
            // be setting the codebook size to 10. This means all generated
            // feature vectors will have the same length of 10.

            // Create a new Bag-of-Visual-Words (BoW) model
            BagOfVisualWords bow = new BagOfVisualWords(10);

            bow.ParallelOptions.MaxDegreeOfParallelism = 1;

            // Compute the model using
            // a set of training images
            bow.Compute(images);

            // After this point, we will be able to translate
            // images into double[] feature vectors using
            double[] feature = bow.GetFeatureVector(images[0]);

            Assert.AreEqual(10, feature.Length);


            double[][] expected = new double[][]
            {
                new double[] { 4, 28, 24, 68, 51, 97, 60, 35, 18, 24 },
                new double[] { 53, 111, 89, 70, 24, 80, 130, 46, 50, 74 },
                new double[] { 31, 29, 57, 102, 63, 142, 40, 18, 37, 33 }
            };

            double[][] actual = new double[expected.Length][];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = bow.GetFeatureVector(images[i]);

            string str = actual.ToCSharp();

            for (int i = 0; i < actual.Length; i++)
                for (int j = 0; j < actual[i].Length; j++)
                    Assert.IsTrue(expected[i].Contains(actual[i][j]));
        }

        [Test]
        [Category("Random")]
#if NET35
        [Ignore("Random behaviour differs in net35.")]
#endif
        public void learn_new()
        {
            #region doc_learn
            // Ensure results are reproducible
            Accord.Math.Random.Generator.Seed = 0;

            // The Bag-of-Visual-Words model converts images of arbitrary 
            // size into fixed-length feature vectors. In this example, we
            // will be setting the codebook size to 10. This means all feature
            // vectors that will be generated will have the same length of 10.

            // By default, the BoW object will use the sparse SURF as the 
            // feature extractor and K-means as the clustering algorithm.

            // Create a new Bag-of-Visual-Words (BoW) model
            BagOfVisualWords bow = new BagOfVisualWords(10);
            // Note: the BoW model can also be created using
            // var bow = BagOfVisualWords.Create(10);

            // Get some training images
            Bitmap[] images = GetImages();

            // Compute the model
            bow.Learn(images);

            // After this point, we will be able to translate
            // images into double[] feature vectors using
            double[][] features = bow.Transform(images);
            #endregion

            var kmeans = bow.Clustering as KMeans;
            Assert.AreEqual(64, kmeans.Clusters.NumberOfInputs);
            Assert.AreEqual(10, kmeans.Clusters.NumberOfOutputs);
            Assert.AreEqual(10, kmeans.Clusters.NumberOfClasses);

            string str = kmeans.Clusters.Proportions.ToCSharp();
            double[] expectedProportions = new double[] { 0.0960793804453049, 0.0767182962245886, 0.103823814133591, 0.0738141335914811, 0.0997095837366893, 0.0815585672797677, 0.0788964181994192, 0.090513068731849, 0.117376573088093, 0.181510164569216 };

            Assert.IsTrue(kmeans.Clusters.Proportions.IsEqual(expectedProportions, 1e-10));
            Assert.IsTrue(kmeans.Clusters.Covariances.All(x => x == null));

            Assert.AreEqual(features.GetLength(), new[] { 6, 10 });

            str = features.ToCSharp();

            double[][] expected = new double[][]
            {
                new double[] { 47, 44, 42, 4, 23, 22, 28, 53, 50, 96 },
                new double[] { 26, 91, 71, 49, 99, 70, 59, 28, 155, 79 },
                new double[] { 71, 34, 51, 33, 53, 25, 44, 64, 32, 145 },
                new double[] { 49, 41, 31, 24, 54, 19, 41, 63, 66, 72 },
                new double[] { 137, 16, 92, 115, 39, 75, 24, 92, 41, 88 },
                new double[] { 67, 91, 142, 80, 144, 126, 130, 74, 141, 270 }
            };

            for (int i = 0; i < features.Length; i++)
                for (int j = 0; j < features[i].Length; j++)
                    Assert.IsTrue(expected[i].Contains(features[i][j]));

            #region doc_classification

            // Now, the features can be used to train any classification
            // algorithm as if they were the images themselves. For example,
            // let's assume the first three images belong to a class and
            // the second three to another class. We can train an SVM using

            int[] labels = { -1, -1, -1, +1, +1, +1 };

            // Create the SMO algorithm to learn a Linear kernel SVM
            var teacher = new SequentialMinimalOptimization<Linear>()
            {
                Complexity = 10000 // make a hard margin SVM
            };

            // Obtain a learned machine
            var svm = teacher.Learn(features, labels);

            // Use the machine to classify the features
            bool[] output = svm.Decide(features);

            // Compute the error between the expected and predicted labels
            double error = new ZeroOneLoss(labels).Loss(output);
            #endregion

            Assert.IsTrue(new ZeroOneLoss(labels).IsBinary);
            Assert.AreEqual(error, 0);
        }


        [Test]
        [Category("Random")]
#if NET35
        [Ignore("Random behaviour differs in net35.")]
#endif
        public void custom_clustering_test()
        {
            #region doc_clustering
            // Ensure results are reproducible
            Accord.Math.Random.Generator.Seed = 0;

            // The Bag-of-Visual-Words model converts images of arbitrary 
            // size into fixed-length feature vectors. In this example, we
            // will be setting the codebook size to 10. This means all feature
            // vectors that will be generated will have the same length of 10.

            // By default, the BoW object will use the sparse SURF as the 
            // feature extractor and K-means as the clustering algorithm.
            // In this example, we will use the Binary-Split clustering
            // algorithm instead.

            // Create a new Bag-of-Visual-Words (BoW) model
            var bow = BagOfVisualWords.Create(new BinarySplit(10));

            // Since we are using generics, we can setup properties 
            // of the binary split clustering algorithm directly:
            bow.Clustering.ComputeProportions = true;
            bow.Clustering.ComputeCovariances = false;

            // Get some training images
            Bitmap[] images = GetImages();

            // Compute the model
            bow.Learn(images);

            // After this point, we will be able to translate
            // images into double[] feature vectors using
            double[][] features = bow.Transform(images);
            #endregion

            Assert.AreEqual(-1, bow.NumberOfInputs);
            Assert.AreEqual(10, bow.NumberOfOutputs);
            Assert.AreEqual(10, bow.NumberOfWords);
            Assert.AreEqual(64, bow.Clustering.Clusters.NumberOfInputs);
            Assert.AreEqual(10, bow.Clustering.Clusters.NumberOfOutputs);
            Assert.AreEqual(10, bow.Clustering.Clusters.NumberOfClasses);

            BinarySplit binarySplit = bow.Clustering;

            string str = binarySplit.Clusters.Proportions.ToCSharp();
            double[] expectedProportions = new double[] { 0.158034849951597, 0.11810261374637, 0.0871248789932236, 0.116408518877057, 0.103581800580833, 0.192642787996128, 0.0365440464666021, 0.0716360116166505, 0.0575992255566312, 0.058325266214908 };

            Assert.IsTrue(binarySplit.Clusters.Proportions.IsEqual(expectedProportions, 1e-10));
            Assert.IsTrue(binarySplit.Clusters.Covariances.All(x => x == null));

            Assert.AreEqual(features.GetLength(), new[] { 6, 10 });

            str = features.ToCSharp();

            double[][] expected = new double[][]
            {
                new double[] { 73, 36, 41, 50, 7, 106, 23, 22, 22, 29 },
                new double[] { 76, 93, 25, 128, 86, 114, 20, 91, 22, 72 },
                new double[] { 106, 47, 67, 57, 37, 131, 33, 31, 22, 21 },
                new double[] { 84, 41, 49, 59, 33, 73, 32, 50, 6, 33 },
                new double[] { 169, 105, 92, 47, 95, 67, 16, 25, 83, 20 },
                new double[] { 145, 166, 86, 140, 170, 305, 27, 77, 83, 66 }
            };

            for (int i = 0; i < features.Length; i++)
                for (int j = 0; j < features[i].Length; j++)
                    Assert.IsTrue(expected[i].Contains(features[i][j]));

            #region doc_classification_clustering

            // Now, the features can be used to train any classification
            // algorithm as if they were the images themselves. For example,
            // let's assume the first three images belong to a class and
            // the second three to another class. We can train an SVM using

            int[] labels = { -1, -1, -1, +1, +1, +1 };

            // Create the SMO algorithm to learn a Linear kernel SVM
            var teacher = new SequentialMinimalOptimization<Linear>()
            {
                Complexity = 10000 // make a hard margin SVM
            };

            // Obtain a learned machine
            var svm = teacher.Learn(features, labels);

            // Use the machine to classify the features
            bool[] output = svm.Decide(features);

            // Compute the error between the expected and predicted labels
            double error = new ZeroOneLoss(labels).Loss(output); // should be 0
            #endregion

            Assert.AreEqual(error, 0);
        }

        [Test]
        [Category("Random")]
#if NET35
        [Ignore("Random behaviour differs in net35.")]
#endif
        public void custom_feature_test()
        {
            #region doc_feature
            Accord.Math.Random.Generator.Seed = 0;

            // The Bag-of-Visual-Words model converts images of arbitrary 
            // size into fixed-length feature vectors. In this example, we
            // will be setting the codebook size to 10. This means all feature
            // vectors that will be generated will have the same length of 10.

            // By default, the BoW object will use the sparse SURF as the 
            // feature extractor and K-means as the clustering algorithm.
            // In this example, we will use the HOG feature extractor
            // and the Binary-Split clustering algorithm instead.

            // Create a new Bag-of-Visual-Words (BoW) model using HOG features
            var bow = BagOfVisualWords.Create(new HistogramsOfOrientedGradients(), new BinarySplit(10));

            // Get some training images
            Bitmap[] images = GetImages();

            // Compute the model
            bow.Learn(images);

            // After this point, we will be able to translate
            // images into double[] feature vectors using
            double[][] features = bow.Transform(images);
            #endregion

            Assert.AreEqual(features.GetLength(), new[] { 6, 10 });

            string str = features.ToCSharp();

            double[][] expected = new double[][]
            {
                new double[] { 53, 285, 317, 292, 389, 264, 127, 250, 283, 92 },
                new double[] { 64, 326, 267, 418, 166, 241, 160, 237, 324, 149 },
                new double[] { 63, 234, 229, 221, 645, 178, 226, 178, 218, 160 },
                new double[] { 87, 322, 324, 295, 180, 276, 219, 218, 247, 184 },
                new double[] { 60, 312, 285, 285, 352, 274, 166, 226, 290, 102 },
                new double[] { 110, 292, 299, 324, 72, 208, 317, 248, 252, 230 }
            };

            for (int i = 0; i < features.Length; i++)
                for (int j = 0; j < features[i].Length; j++)
                    Assert.IsTrue(expected[i].Contains(features[i][j]));

            #region doc_classification_feature

            // Now, the features can be used to train any classification
            // algorithm as if they were the images themselves. For example,
            // let's assume the first three images belong to a class and
            // the second three to another class. We can train an SVM using

            int[] labels = { -1, -1, -1, +1, +1, +1 };

            // Create the SMO algorithm to learn a Linear kernel SVM
            var teacher = new SequentialMinimalOptimization<Linear>()
            {
                Complexity = 100 // make a hard margin SVM
            };

            // Obtain a learned machine
            var svm = teacher.Learn(features, labels);

            // Use the machine to classify the features
            bool[] output = svm.Decide(features);

            // Compute the error between the expected and predicted labels
            double error = new ZeroOneLoss(labels).Loss(output); // should be 0
            #endregion

            Assert.AreEqual(error, 0);
        }

        [Test, Ignore("Haralick does not extract good features in this dataset")]
        public void custom_feature_test_haralick()
        {
            #region doc_feature_haralick
            Accord.Math.Random.Generator.Seed = 0;

            // The Bag-of-Visual-Words model converts images of arbitrary 
            // size into fixed-length feature vectors. In this example, we
            // will be setting the codebook size to 3. This means all feature
            // vectors that will be generated will have the same length of 10.

            // By default, the BoW object will use the sparse SURF as the 
            // feature extractor and K-means as the clustering algorithm.
            // In this example, we will use the Haralick feature extractor
            // and the GMM clustering algorithm instead.

            // Create a new Bag-of-Visual-Words (BoW) model using HOG features
            var bow = BagOfVisualWords.Create(new Haralick(), new GaussianMixtureModel(3));

            // Get some training images
            Bitmap[] images = GetImages();

            // Compute the model
            bow.Learn(images);

            // After this point, we will be able to translate
            // images into double[] feature vectors using
            double[][] features = bow.Transform(images);
            #endregion

            Assert.AreEqual(features.GetLength(), new[] { 6, 10 });

            string str = features.ToCSharp();

            double[][] expected = new double[][]
            {
                new double[] { 141, 332, 240, 88, 363, 238, 282, 322, 114, 232 },
                new double[] { 103, 452, 195, 140, 158, 260, 283, 368, 163, 230 },
                new double[] { 88, 231, 185, 172, 631, 189, 219, 241, 237, 159 },
                new double[] { 106, 318, 262, 212, 165, 276, 264, 275, 244, 230 },
                new double[] { 143, 302, 231, 113, 332, 241, 273, 320, 157, 240 },
                new double[] { 87, 347, 248, 249, 63, 227, 292, 288, 339, 212 }
            };

            for (int i = 0; i < features.Length; i++)
                for (int j = 0; j < features[i].Length; j++)
                    Assert.IsTrue(expected[i].Contains(features[i][j]));

            #region doc_classification_feature_haralick

            // Now, the features can be used to train any classification
            // algorithm as if they were the images themselves. For example,
            // let's assume the first three images belong to a class and
            // the second three to another class. We can train an SVM using

            int[] labels = { -1, -1, -1, +1, +1, +1 };

            // Create the SMO algorithm to learn a Linear kernel SVM
            var teacher = new SequentialMinimalOptimization<Linear>()
            {
                Complexity = 100 // make a hard margin SVM
            };

            // Obtain a learned machine
            var svm = teacher.Learn(features, labels);

            // Use the machine to classify the features
            bool[] output = svm.Decide(features);

            // Compute the error between the expected and predicted labels
            double error = new ZeroOneLoss(labels).Loss(output); // should be 0
            #endregion

            Assert.AreEqual(error, 0);
        }

        [Test, Category("Random")]
#if NET35
        [Ignore("Random")]
#endif
        public void custom_feature_test_lbp()
        {
            #region doc_feature_lbp
            Accord.Math.Random.Generator.Seed = 0;

            // The Bag-of-Visual-Words model converts images of arbitrary 
            // size into fixed-length feature vectors. In this example, we
            // will be setting the codebook size to 3. This means all feature
            // vectors that will be generated will have the same length of 10.

            // By default, the BoW object will use the sparse SURF as the 
            // feature extractor and K-means as the clustering algorithm.
            // In this example, we will use the Local Binary Pattern (LBP) 
            // feature extractor and the Binary-Split clustering algorithm.

            // Create a new Bag-of-Visual-Words (BoW) model using LBP features
            var bow = BagOfVisualWords.Create(new LocalBinaryPattern(), new BinarySplit(3));

            // Since we are using generics, we can setup properties 
            // of the binary split clustering algorithm directly:
            bow.Clustering.ComputeCovariances = false;
            bow.Clustering.ComputeProportions = false;
            bow.Clustering.ComputeError = false;

            // Get some training images
            Bitmap[] images = GetImages();

            // Compute the model
            bow.Learn(images);

            // After this point, we will be able to translate
            // images into double[] feature vectors using
            double[][] features = bow.Transform(images);
            #endregion

            Assert.AreEqual(features.GetLength(), new[] { 6, 3 });

            string str = features.ToCSharp();

            double[][] expected = new double[][]
            {
                new double[] { 1608, 374, 370 },
                new double[] { 1508, 337, 507 },
                new double[] { 1215, 343, 794 },
                new double[] { 782, 550, 1020 },
                new double[] { 1480, 360, 512 },
                new double[] { 15, 724, 1613 }
            };

            for (int i = 0; i < features.Length; i++)
                for (int j = 0; j < features[i].Length; j++)
                    Assert.IsTrue(expected[i].Contains(features[i][j]));

            #region doc_classification_feature_lbp

            // Now, the features can be used to train any classification
            // algorithm as if they were the images themselves. For example,
            // let's assume the first three images belong to a class and
            // the second three to another class. We can train an SVM using

            int[] labels = { -1, -1, +1, +1, +1, +1 };

            // Create the SMO algorithm to learn a Linear kernel SVM
            var teacher = new SequentialMinimalOptimization<Gaussian>()
            {
                Complexity = 100 // make a hard margin SVM
            };

            // Obtain a learned machine
            var svm = teacher.Learn(features, labels);

            // Use the machine to classify the features
            bool[] output = svm.Decide(features);

            // Compute the error between the expected and predicted labels
            double error = new ZeroOneLoss(labels).Loss(output); // should be 0
            #endregion

            Assert.AreEqual(error, 0);
        }


        [Test]
        [Category("Random")]
#if NET35
        [Ignore("Random behaviour differs in net35.")]
#endif
        public void custom_data_type_test()
        {
            #region doc_datatype
            Accord.Math.Random.Generator.Seed = 0;

            // The Bag-of-Visual-Words model converts images of arbitrary 
            // size into fixed-length feature vectors. In this example, we
            // will be setting the codebook size to 10. This means all feature
            // vectors that will be generated will have the same length of 10.

            // By default, the BoW object will use the sparse SURF as the 
            // feature extractor and K-means as the clustering algorithm.
            // In this example, we will use the FREAK feature extractor
            // and the K-Modes clustering algorithm instead.

            // Create a new Bag-of-Visual-Words (BoW) model using FREAK binary features
            var bow = BagOfVisualWords.Create<FastRetinaKeypointDetector, KModes<byte>, byte[]>(
                new FastRetinaKeypointDetector(), new KModes<byte>(10, new Hamming()));

            // Get some training images
            Bitmap[] images = GetImages();

            // Compute the model
            bow.Learn(images);

            // After this point, we will be able to translate
            // images into double[] feature vectors using
            double[][] features = bow.Transform(images);
            #endregion

            Assert.AreEqual(features.GetLength(), new[] { 6, 10 });

            string str = features.ToCSharp();

            double[][] expected = new double[][]
            {
                new double[] { 33, 58, 19, 35, 112, 67, 70, 155, 86, 45 },
                new double[] { 130, 91, 74, 114, 200, 90, 136, 37, 53, 92 },
                new double[] { 45, 49, 68, 55, 123, 142, 40, 100, 92, 37 },
                new double[] { 25, 17, 89, 136, 138, 59, 33, 7, 23, 12 },
                new double[] { 186, 78, 86, 133, 198, 60, 65, 25, 38, 77 },
                new double[] { 45, 33, 10, 131, 192, 26, 99, 20, 82, 28 }
            };

            for (int i = 0; i < features.Length; i++)
                for (int j = 0; j < features[i].Length; j++)
                    Assert.IsTrue(expected[i].Contains(features[i][j]));

            #region doc_classification_datatype

            // Now, the features can be used to train any classification
            // algorithm as if they were the images themselves. For example,
            // let's assume the first three images belong to a class and
            // the second three to another class. We can train an SVM using

            int[] labels = { -1, -1, -1, +1, +1, +1 };

            // Create the SMO algorithm to learn a Linear kernel SVM
            var teacher = new SequentialMinimalOptimization<Linear>()
            {
                Complexity = 1000 // make a hard margin SVM
            };

            // Obtain a learned machine
            var svm = teacher.Learn(features, labels);

            // Use the machine to classify the features
            bool[] output = svm.Decide(features);

            // Compute the error between the expected and predicted labels
            double error = new ZeroOneLoss(labels).Loss(output); // should be 0
            #endregion

            Assert.AreEqual(error, 0);
        }


        [Test, Category("Serialization")]
        public void SerializeTest()
        {
            var images = GetImages();

            Accord.Math.Random.Generator.Seed = 0;

            BagOfVisualWords bow = new BagOfVisualWords(10);

            bow.Compute(images);

            double[][] expected = new double[images.Length][];
            for (int i = 0; i < expected.Length; i++)
                expected[i] = bow.GetFeatureVector(images[i]);

            MemoryStream stream = new MemoryStream();
            BinaryFormatter fmt = new BinaryFormatter();
            fmt.Serialize(stream, bow);
            stream.Seek(0, SeekOrigin.Begin);
            bow = (BagOfVisualWords)fmt.Deserialize(stream);

            double[][] actual = new double[expected.Length][];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = bow.GetFeatureVector(images[i]);


            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        [Category("Serialization")]
        public void SerializeTest2()
        {
            var images = GetImages();

            Accord.Math.Random.Generator.Seed = 0;

            FastCornersDetector fast = new FastCornersDetector();

            FastRetinaKeypointDetector freak = new FastRetinaKeypointDetector(fast);

            var kmodes = new KModes<byte>(5, new Hamming());
            //kmodes.ParallelOptions.MaxDegreeOfParallelism = 1;

            var bow = new BagOfVisualWords<FastRetinaKeypoint, byte[]>(freak, kmodes);
            bow.Compute(images);

            double[][] expected = new double[images.Length][];
            for (int i = 0; i < expected.Length; i++)
                expected[i] = bow.GetFeatureVector(images[i]);

            MemoryStream stream = new MemoryStream();
            BinaryFormatter fmt = new BinaryFormatter();
            fmt.Serialize(stream, bow);
            stream.Seek(0, SeekOrigin.Begin);
            bow = (BagOfVisualWords<FastRetinaKeypoint, byte[]>)fmt.Deserialize(stream);

            double[][] actual = new double[expected.Length][];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = bow.GetFeatureVector(images[i]);

            Assert.IsTrue(expected.IsEqual(actual));
        }


        [Test, Ignore("Test writing has not been finished")]
        public void LargeTest()
        {
            // Requires data from the National Data Science bowl
            // https://github.com/accord-net/framework/issues/58
            // TODO: Add some code to download and cache the data

            var trainingDirectory = @"C:\Users\CésarRoberto\Downloads\train\train\";

            var images = LabeledImages.FromDirectory(trainingDirectory).ToArray();

            var binarySplit = new BinarySplit(32);

            var bow = new BagOfVisualWords(binarySplit);

            bow.Compute(images.Select(i => i.Item).Take(50).ToArray());

        }

        public interface ILabeledItem<T>
        {
            string Category { get; }

            T Item { get; }
        }

        public class LabeledImages : ILabeledItem<Bitmap>
        {
            public LabeledImages(string category, string filename)
            {
                Category = category;
                Filename = filename;
            }

            public string Category { get; private set; }

            public string Filename { get; private set; }

            public Bitmap Item
            {
                get
                {
                    using (var sourceImage = System.Drawing.Image.FromFile(Filename))
                    {
                        var targetImage = new Bitmap(sourceImage.Width, sourceImage.Height, PixelFormat.Format32bppArgb);
                        using (var canvas = Graphics.FromImage(targetImage))
                        {
                            canvas.DrawImageUnscaled(sourceImage, 0, 0);
                        }
                        return targetImage;
                    }
                }
            }

            public static IEnumerable<LabeledImages> FromDirectory(string path)
            {
                return Directory.EnumerateDirectories(path)
                    .SelectMany(dir => Directory.EnumerateFiles(dir)
                        .Select(file => new LabeledImages(Path.GetFileName(dir), file)));
            }
        }

    }
}
