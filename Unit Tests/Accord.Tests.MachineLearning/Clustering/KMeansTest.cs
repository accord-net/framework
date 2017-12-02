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
    using Accord.MachineLearning;
    using Accord.Math;
    using NUnit.Framework;
    using System;
    using System.Linq;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization.Formatters.Binary;
    using Accord.IO;
    using Accord.Math.Distances;
    using Accord.Statistics.Filters;

    [TestFixture]
    public class KMeansTest
    {


        [Test]
        public void KMeansConstructorTest()
        {
            Accord.Math.Random.Generator.Seed = 0;

            // Declare some observations
            double[][] observations =
            {
                new double[] { -5, -2, -1 },
                new double[] { -5, -5, -6 },
                new double[] {  2,  1,  1 },
                new double[] {  1,  1,  2 },
                new double[] {  1,  2,  2 },
                new double[] {  3,  1,  2 },
                new double[] { 11,  5,  4 },
                new double[] { 15,  5,  6 },
                new double[] { 10,  5,  6 },
            };

            double[][] orig = observations.MemberwiseClone();

            // Create a new K-Means algorithm with 3 clusters 
            KMeans kmeans = new KMeans(3);

            // Compute the algorithm, retrieving an integer array
            //  containing the labels for each of the observations
            int[] labels = kmeans.Compute(observations);

            // As a result, the first two observations should belong to the
            //  same cluster (thus having the same label). The same should
            //  happen to the next four observations and to the last three.

            Assert.AreEqual(labels[0], labels[1]);

            Assert.AreEqual(labels[2], labels[3]);
            Assert.AreEqual(labels[2], labels[4]);
            Assert.AreEqual(labels[2], labels[5]);

            Assert.AreEqual(labels[6], labels[7]);
            Assert.AreEqual(labels[6], labels[8]);

            Assert.AreNotEqual(labels[0], labels[2]);
            Assert.AreNotEqual(labels[2], labels[6]);
            Assert.AreNotEqual(labels[0], labels[6]);


            int[] labels2 = kmeans.Clusters.Decide(observations);
            Assert.IsTrue(labels.IsEqual(labels2));

            // the data must not have changed!
            Assert.IsTrue(orig.IsEqual(observations));
        }

        [Test]
        public void learn_test()
        {
            #region doc_learn
            Accord.Math.Random.Generator.Seed = 0;

            // Declare some observations
            double[][] observations =
            {
                new double[] { -5, -2, -1 },
                new double[] { -5, -5, -6 },
                new double[] {  2,  1,  1 },
                new double[] {  1,  1,  2 },
                new double[] {  1,  2,  2 },
                new double[] {  3,  1,  2 },
                new double[] { 11,  5,  4 },
                new double[] { 15,  5,  6 },
                new double[] { 10,  5,  6 },
            };

            // Create a new K-Means algorithm
            KMeans kmeans = new KMeans(k: 3);

            // Compute and retrieve the data centroids
            var clusters = kmeans.Learn(observations);

            // Use the centroids to parition all the data
            int[] labels = clusters.Decide(observations);
            #endregion

            Assert.AreEqual(labels[0], labels[1]);

            Assert.AreEqual(labels[2], labels[3]);
            Assert.AreEqual(labels[2], labels[4]);
            Assert.AreEqual(labels[2], labels[5]);

            Assert.AreEqual(labels[6], labels[7]);
            Assert.AreEqual(labels[6], labels[8]);

            Assert.AreNotEqual(labels[0], labels[2]);
            Assert.AreNotEqual(labels[2], labels[6]);
            Assert.AreNotEqual(labels[0], labels[6]);

            int[] labels2 = kmeans.Clusters.Decide(observations);
            Assert.IsTrue(labels.IsEqual(labels2));

            // the data must not have changed!
            double[][] orig =
            {
                new double[] { -5, -2, -1 },
                new double[] { -5, -5, -6 },
                new double[] {  2,  1,  1 },
                new double[] {  1,  1,  2 },
                new double[] {  1,  2,  2 },
                new double[] {  3,  1,  2 },
                new double[] { 11,  5,  4 },
                new double[] { 15,  5,  6 },
                new double[] { 10,  5,  6 },
            };

            Assert.IsTrue(orig.IsEqual(observations));

            var c = new KMeansClusterCollection.KMeansCluster[clusters.Count];
            int i = 0;
            foreach (var cluster in clusters)
                c[i++] = cluster;

            for (i = 0; i < c.Length; i++)
                Assert.AreSame(c[i], clusters[i]);
        }

        [Test]
        public void learn_test_mixed()
        {
            #region doc_learn_mixed
            Accord.Math.Random.Generator.Seed = 0;

            // Declare some mixed discrete and continuous observations
            double[][] observations =
            {
                //             (categorical) (discrete) (continuous)
                new double[] {       1,          -1,        -2.2      },
                new double[] {       1,          -6,        -5.5      },
                new double[] {       2,           1,         1.1      },
                new double[] {       2,           2,         1.2      },
                new double[] {       2,           2,         2.6      },
                new double[] {       3,           2,         1.4      },
                new double[] {       3,           4,         5.2      },
                new double[] {       1,           6,         5.1      },
                new double[] {       1,           6,         5.9      },
            };

            // Create a new codification algorithm to convert 
            // the mixed variables above into all continuous:
            var codification = new Codification<double>()
            {
                CodificationVariable.Categorical,
                CodificationVariable.Discrete,
                CodificationVariable.Continuous
            };

            // Learn the codification from observations
            var model = codification.Learn(observations);

            // Transform the mixed observations into only continuous:
            double[][] newObservations = model.ToDouble().Transform(observations);

            // (newObservations will be equivalent to)
            double[][] expected =
            {
                //               (one hot)    (discrete)    (continuous)
                new double[] {    1, 0, 0,        -1,          -2.2      },
                new double[] {    1, 0, 0,        -6,          -5.5      },
                new double[] {    0, 1, 0,         1,           1.1      },
                new double[] {    0, 1, 0,         2,           1.2      },
                new double[] {    0, 1, 0,         2,           2.6      },
                new double[] {    0, 0, 1,         2,           1.4      },
                new double[] {    0, 0, 1,         4,           5.2      },
                new double[] {    1, 0, 0,         6,           5.1      },
                new double[] {    1, 0, 0,         6,           5.9      },
            };

            // Create a new K-Means algorithm
            KMeans kmeans = new KMeans(k: 3);

            // Compute and retrieve the data centroids
            var clusters = kmeans.Learn(observations);

            // Use the centroids to parition all the data
            int[] labels = clusters.Decide(observations);
            #endregion

            
            Assert.IsTrue(expected.IsEqual(newObservations, 1e-8));

            Assert.AreEqual(3, codification.NumberOfInputs);
            Assert.AreEqual(5, codification.NumberOfOutputs);
            Assert.AreEqual(3, codification.Columns.Count);
            Assert.AreEqual("0", codification.Columns[0].ColumnName);
            Assert.AreEqual(3, codification.Columns[0].NumberOfSymbols);
            Assert.AreEqual(1, codification.Columns[0].NumberOfInputs);
            Assert.AreEqual(3, codification.Columns[0].NumberOfOutputs);
            Assert.AreEqual(3, codification.Columns[0].NumberOfClasses);
            Assert.AreEqual(CodificationVariable.Categorical, codification.Columns[0].VariableType);
            Assert.AreEqual("1", codification.Columns[1].ColumnName);
            Assert.AreEqual(1, codification.Columns[1].NumberOfSymbols);
            Assert.AreEqual(1, codification.Columns[1].NumberOfInputs);
            Assert.AreEqual(1, codification.Columns[1].NumberOfOutputs);
            Assert.AreEqual(1, codification.Columns[1].NumberOfClasses);
            Assert.AreEqual(CodificationVariable.Discrete, codification.Columns[1].VariableType);
            Assert.AreEqual("2", codification.Columns[2].ColumnName);
            Assert.AreEqual(1, codification.Columns[2].NumberOfSymbols);
            Assert.AreEqual(1, codification.Columns[2].NumberOfInputs);
            Assert.AreEqual(1, codification.Columns[2].NumberOfOutputs);
            Assert.AreEqual(1, codification.Columns[2].NumberOfClasses);
            Assert.AreEqual(CodificationVariable.Continuous, codification.Columns[2].VariableType);

            Assert.AreEqual(labels[0], labels[2]);
            Assert.AreEqual(labels[0], labels[3]);
            Assert.AreEqual(labels[0], labels[4]);
            Assert.AreEqual(labels[0], labels[5]);

            Assert.AreEqual(labels[6], labels[7]);
            Assert.AreEqual(labels[6], labels[8]);

            Assert.AreNotEqual(labels[0], labels[1]);
            Assert.AreNotEqual(labels[0], labels[6]);

            int[] labels2 = kmeans.Clusters.Decide(observations);
            Assert.IsTrue(labels.IsEqual(labels2));

            var c = new KMeansClusterCollection.KMeansCluster[clusters.Count];
            int i = 0;
            foreach (var cluster in clusters)
                c[i++] = cluster;

            for (i = 0; i < c.Length; i++)
                Assert.AreSame(c[i], clusters[i]);
        }

        [Test]
        public void uniform_sampling_test()
        {
            Accord.Math.Random.Generator.Seed = 0;

            // Declare some observations
            double[][] observations =
            {
                new double[] { -5, -2, -1 },
                new double[] { -5, -5, -6 },
                new double[] {  2,  1,  1 },
                new double[] {  1,  1,  2 },
                new double[] {  1,  2,  2 },
                new double[] {  3,  1,  2 },
                new double[] { 11,  5,  4 },
                new double[] { 15,  5,  6 },
                new double[] { 10,  5,  6 },
            };

            // Create a new K-Means algorithm
            KMeans kmeans = new KMeans(k: 3)
            {
                UseSeeding = Seeding.Uniform
            };

            // Compute and retrieve the data centroids
            var clusters = kmeans.Learn(observations);

            int[] labels = clusters.Decide(observations);

            int[] labels2 = kmeans.Clusters.Decide(observations);

            // the data must not have changed!
            double[][] orig =
            {
                new double[] { -5, -2, -1 },
                new double[] { -5, -5, -6 },
                new double[] {  2,  1,  1 },
                new double[] {  1,  1,  2 },
                new double[] {  1,  2,  2 },
                new double[] {  3,  1,  2 },
                new double[] { 11,  5,  4 },
                new double[] { 15,  5,  6 },
                new double[] { 10,  5,  6 },
            };

            Assert.IsTrue(orig.IsEqual(observations));
        }

        [Test]
        public void learn_test_weights()
        {
            #region doc_learn_weights
            Accord.Math.Random.Generator.Seed = 0;

            // A common desire when doing clustering is to attempt to find how to 
            // weight the different components / columns of a dataset, giving them 
            // different importances depending on the end goal of the clustering task.

            // Declare some observations
            double[][] observations =
            {
                new double[] { -5, -2, -1 },
                new double[] { -5, -5, -6 },
                new double[] {  2,  1,  1 },
                new double[] {  1,  1,  2 },
                new double[] {  1,  2,  2 },
                new double[] {  3,  1,  2 },
                new double[] { 11,  5,  4 },
                new double[] { 15,  5,  6 },
                new double[] { 10,  5,  6 },
            };

            // Create a new K-Means algorithm
            KMeans kmeans = new KMeans(k: 3)
            {
                // For example, let's say we would like to consider the importance of 
                // the first column as 0.1, the second column as 0.7 and the third 0.9
                Distance = new WeightedSquareEuclidean(new double[] { 0.1, 0.7, 1.1 })
            };

            // Compute and retrieve the data centroids
            var clusters = kmeans.Learn(observations);

            // Use the centroids to parition all the data
            int[] labels = clusters.Decide(observations);
            #endregion

            Assert.AreEqual(labels[0], labels[2]);

            Assert.AreEqual(labels[0], labels[2]);
            Assert.AreEqual(labels[0], labels[3]);
            Assert.AreEqual(labels[0], labels[4]);
            Assert.AreEqual(labels[0], labels[4]);

            Assert.AreEqual(labels[6], labels[7]);
            Assert.AreEqual(labels[6], labels[8]);

            Assert.AreNotEqual(labels[0], labels[1]);
            Assert.AreNotEqual(labels[2], labels[6]);
            Assert.AreNotEqual(labels[0], labels[6]);

            int[] labels2 = kmeans.Clusters.Decide(observations);
            Assert.IsTrue(labels.IsEqual(labels2));

            var c = new KMeansClusterCollection.KMeansCluster[clusters.Count];
            int i = 0;
            foreach (var cluster in clusters)
                c[i++] = cluster;

            for (i = 0; i < c.Length; i++)
                Assert.AreSame(c[i], clusters[i]);
        }

        [Test]
        public void KMeansConstructorTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            // Declare some observations
            double[][] observations =
            {
                new double[] { -5, -2, -1 },
                new double[] { -5, -5, -6 },
                new double[] {  2,  1,  1 },
                new double[] {  1,  1,  2 },
                new double[] {  1,  2,  2 },
                new double[] {  3,  1,  2 },
                new double[] { 11,  5,  4 },
                new double[] { 15,  5,  6 },
                new double[] { 10,  5,  6 },
            };

            double error, e;

            // Create a new algorithm
            KMeans kmeans = new KMeans(3);
            kmeans.Randomize(observations);

            // Save the first initialization
            double[][] initial = kmeans.Clusters.Centroids.MemberwiseClone();

            // Compute the first K-Means
            kmeans.Compute(observations, out error);

            // Create more K-Means algorithms 
            //  with the same initializations
            for (int i = 0; i < 1000; i++)
            {
                kmeans = new KMeans(3);
                kmeans.Clusters.Centroids = initial;
                kmeans.Compute(observations, out e);

                Assert.AreEqual(error, e);
            }

            // Create more K-Means algorithms 
            //  without the same initialization
            bool differ = false;
            for (int i = 0; i < 1000; i++)
            {
                kmeans = new KMeans(3);
                kmeans.Compute(observations, out e);

                if (error != e)
                    differ = true;
            }

            Assert.IsTrue(differ);
        }

        [Test]
        public void KMeansConstructorTest3()
        {
            // Create a new algorithm
            KMeans kmeans = new KMeans(3);
            Assert.IsNotNull(kmeans.Clusters);
            Assert.IsNotNull(kmeans.Distance);
            Assert.IsNotNull(kmeans.Clusters.Centroids);
            Assert.IsNotNull(kmeans.Clusters.Count);
            Assert.IsNotNull(kmeans.Clusters.Covariances);
            Assert.IsNotNull(kmeans.Clusters.Proportions);
        }

        [Test]
        public void KMeansConstructorTest_Distance()
        {
            // Create a new algorithm
            KMeans kmeans = new KMeans(3, Distance.Manhattan);
            Assert.IsNotNull(kmeans.Distance);
            Assert.IsTrue(kmeans.Distance is Accord.Math.Distances.Manhattan);
        }

        [Test]
        public void KMeansMoreClustersThanSamples()
        {
            Accord.Math.Tools.SetupGenerator(0);


            // Declare some observations
            double[][] observations =
            {
                new double[] { -5, -2, -1 },
                new double[] { -5, -5, -6 },
                new double[] {  2,  1,  1 },
                new double[] {  1,  1,  2 },
                new double[] {  1,  2,  2 },
                new double[] {  3,  1,  2 },
                new double[] { 11,  5,  4 },
                new double[] { 15,  5,  6 },
                new double[] { 10,  5,  6 },
            };

            double[][] orig = observations.MemberwiseClone();

            KMeans kmeans = new KMeans(15);

            Assert.Throws<ArgumentException>(() => kmeans.Compute(observations), "");
        }

#if !NO_BINARY_SERIALIZATION
        [Test]
#if NETCORE
        [Ignore("Models created in .NET desktop cannot be de-serialized in .NET Core/Standard (yet)")]
#endif
        public void DeserializationTest1()
        {
            string fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "kmeans.bin");

            KMeans kmeans = Serializer.Load<KMeans>(fileName);


            KMeans kbase = new KMeans(3);

            Assert.AreEqual(kbase.Iterations, kmeans.Iterations);
            Assert.AreEqual(kbase.MaxIterations, kmeans.MaxIterations);
            Assert.AreEqual(kbase.Tolerance, kmeans.Tolerance);

            Assert.AreEqual(kbase.UseSeeding, kmeans.UseSeeding);
            Assert.AreEqual(kbase.ComputeCovariances, kmeans.ComputeCovariances);

            Assert.AreEqual(kbase.ComputeError, kmeans.ComputeError);
            Assert.AreEqual(kbase.ComputeCovariances, kmeans.ComputeCovariances);
            Assert.AreEqual(kbase.Error, kmeans.Error);

            Assert.IsTrue(kbase.ComputeError);
            Assert.IsTrue(kbase.ComputeCovariances);
            Assert.AreEqual(kbase.Distance.GetType(), kmeans.Distance.GetType());
        }
#endif
    }
}
