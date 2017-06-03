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
    using MachineLearning.VectorMachines;
    using MachineLearning.VectorMachines.Learning;
    using Statistics.Kernels;
    using Math.Optimization.Losses;
    using Math.Metrics;

    [TestFixture]
    public class BagOfVisualWordsTest
    {

        // Load some test images
        public static Bitmap[] GetImages()
        {
            Bitmap[] images =
            {
                Accord.Imaging.Image.Clone(Accord.Tests.Vision.Properties.Resources.flower01),
                Accord.Imaging.Image.Clone(Accord.Tests.Vision.Properties.Resources.flower02),
                Accord.Imaging.Image.Clone(Accord.Tests.Vision.Properties.Resources.flower03),
                Accord.Imaging.Image.Clone(Accord.Tests.Vision.Properties.Resources.flower04),
                Accord.Imaging.Image.Clone(Accord.Tests.Vision.Properties.Resources.flower05),
                Accord.Imaging.Image.Clone(Accord.Tests.Vision.Properties.Resources.flower06),
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

            // Ensure results are reproducible
            bow.ParallelOptions.MaxDegreeOfParallelism = 1;

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
                new double[] { 4, 28, 24, 68, 51, 97, 60, 35, 18, 24 },
                new double[] { 53, 111, 89, 70, 24, 80, 130, 46, 50, 74 },
                new double[] { 31, 29, 57, 102, 63, 142, 40, 18, 37, 33 },
                new double[] { 24, 52, 57, 78, 56, 69, 65, 22, 21, 16 },
                new double[] { 124, 35, 33, 145, 90, 83, 31, 4, 95, 79 },
                new double[] { 97, 110, 127, 131, 71, 264, 139, 58, 116, 152 }
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

            // Ensure results are reproducible
            bow.ParallelOptions.MaxDegreeOfParallelism = 1;

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
                new double[] { 118, 43, 65, 87, 19, 12, 31, 14, 17, 3 },
                new double[] { 121, 67, 35, 98, 65, 113, 98, 19, 104, 7 },
                new double[] { 130, 50, 73, 120, 25, 51, 49, 16, 31, 7 },
                new double[] { 63, 35, 77, 109, 28, 37, 53, 6, 50, 2 },
                new double[] { 74, 83, 97, 198, 28, 84, 28, 48, 26, 53 },
                new double[] { 202, 134, 78, 195, 83, 182, 169, 58, 118, 46 }
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

            bow.ParallelOptions.MaxDegreeOfParallelism = 1;

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

            bow.ParallelOptions.MaxDegreeOfParallelism = 1;

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

        [Test]
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

            bow.ParallelOptions.MaxDegreeOfParallelism = 1;

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
                new double[] { 1686, 359, 307 },
                new double[] { 1689, 356, 307 },
                new double[] { 1686, 372, 294 },
                new double[] { 1676, 372, 304 },
                new double[] { 1700, 356, 296 },
                new double[] { 1670, 378, 304 }
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
            var teacher = new SequentialMinimalOptimization<Linear>()
            {
                Complexity = 10 // make a hard margin SVM
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

            bow.ParallelOptions.MaxDegreeOfParallelism = 1;

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

            double[][] expected = new double[][] {
                new double[] { 17, 81, 103, 91, 38, 63, 65, 57, 125, 40 },
                new double[] { 137, 55, 38, 63, 41, 150, 28, 67, 302, 136 },
                new double[] { 58, 96, 51, 71, 43, 128, 51, 74, 110, 69 },
                new double[] { 53, 39, 9, 9, 12, 133, 2, 39, 128, 115 },
                new double[] { 202, 45, 24, 42, 30, 156, 22, 61, 211, 153 },
                new double[] { 37, 26, 40, 23, 41, 121, 22, 63, 171, 122 }
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
            kmodes.ParallelOptions.MaxDegreeOfParallelism = 1;

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
