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
    using Math.Distances;

    [TestFixture]
    public class BalancedKMeansTest
    {


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

            double[][] orig = observations.MemberwiseClone();

            // Create a new K-Means algorithm with 3 clusters 
            BalancedKMeans kmeans = new BalancedKMeans(3)
            {
                // Note: in balanced k-means the chances of the algorithm oscillating
                // between two solutions increases considerably. For this reason, we 
                // set a max-iterations limit to avoid iterating indefinitely.
                MaxIterations = 100
            };

            // Compute the algorithm, retrieving an integer array
            //  containing the labels for each of the observations
            KMeansClusterCollection clusters = kmeans.Learn(observations);

            // As a result, the first two observations should belong to the
            //  same cluster (thus having the same label). The same should
            //  happen to the next four observations and to the last three.
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

            int[] labels2 = kmeans.Clusters.Nearest(observations);
            Assert.IsTrue(labels.IsEqual(labels2));

            // the data must not have changed!
            Assert.IsTrue(orig.IsEqual(observations));

            var c = new KMeansClusterCollection.KMeansCluster[clusters.Count];
            int i = 0;
            foreach (var cluster in clusters)
                c[i++] = cluster;

            for (i = 0; i < c.Length; i++)
                Assert.AreSame(c[i], clusters[i]);
        }

        [Test]
        public void learn_test_new_distance()
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

            double[][] orig = observations.MemberwiseClone();

            // Create a new K-Means algorithm with 3 clusters 
            BalancedKMeans kmeans = new BalancedKMeans(3, new Manhattan());

            // Note: in balanced k-means the chances of the algorithm oscillating
            // between two solutions increases considerably. For this reason, we 
            // set a max-iterations limit to avoid iterating indefinitely.
            kmeans.MaxIterations = 100;

            // Compute the algorithm, retrieving an integer array
            //  containing the labels for each of the observations
            KMeansClusterCollection clusters = kmeans.Learn(observations);

            // As a result, the first two observations should belong to the
            //  same cluster (thus having the same label). The same should
            //  happen to the next four observations and to the last three.
            int[] labels = clusters.Decide(observations);

            // In case we would like equilibrated class labels, we can get the
            // balanced class labels directly from the learning algorithm:
            int[] balanced = kmeans.Labels;
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


            Assert.AreEqual(balanced[0], balanced[1]);
            Assert.AreEqual(balanced[0], balanced[2]);

            Assert.AreEqual(balanced[3], balanced[4]);
            Assert.AreEqual(balanced[3], balanced[5]);

            Assert.AreEqual(balanced[6], balanced[7]);
            Assert.AreEqual(balanced[6], balanced[8]);

            Assert.AreNotEqual(balanced[0], balanced[3]);
            Assert.AreNotEqual(balanced[3], balanced[6]);
            Assert.AreNotEqual(balanced[0], balanced[6]);

            int[] labels2 = kmeans.Clusters.Nearest(observations);
            Assert.IsTrue(labels.IsEqual(labels2));

            // the data must not have changed!
            Assert.IsTrue(orig.IsEqual(observations));

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
            BalancedKMeans kmeans = new BalancedKMeans(3);

            // note: in balanced k-means the chances of the algorithm
            // oscillating between two solutions increases considerably
            kmeans.MaxIterations = 100;

            kmeans.Randomize(observations);

            // Save the first initialization
            double[][] initial = kmeans.Clusters.Centroids.MemberwiseClone();

            // Compute the first K-Means
            kmeans.Compute(observations, out error);

            // Create more K-Means algorithms 
            //  without the same initialization
            bool differ = false;
            for (int i = 0; i < 1000; i++)
            {
                kmeans = new BalancedKMeans(3);
                kmeans.MaxIterations = 100;
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
            BalancedKMeans kmeans = new BalancedKMeans(3);
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
            BalancedKMeans kmeans = new BalancedKMeans(3, new Manhattan());
            Assert.IsNotNull(kmeans.Distance);
            Assert.IsTrue(kmeans.Distance is Accord.Math.Distances.Manhattan);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
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

            BalancedKMeans kmeans = new BalancedKMeans(15)
            {
                MaxIterations = 10
            };

            int[] labels = kmeans.Compute(observations);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MaxIterationsZero()
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

            var kmeans = new BalancedKMeans(2);

            int[] labels = kmeans.Compute(observations);
        }

        [Test]
        public void uniform_test()
        {
            // https://github.com/accord-net/framework/issues/451

            int numClusters = 6;

            double[][] observations =
            {
                new double[] {  10.8,   18.706148721743876 }, // 5
                new double[] { -10.8,   18.706148721743876 }, // 0
                new double[] { -21.6,   0.0 },                // 3
                new double[] { -10.8, -18.706148721743876 },  // 3
                new double[] {  10.8, -18.706148721743876 },  // 5
                new double[] {  21.6,   0.0 },                // 1
                new double[] {  32.4,  18.706148721743876 },  // 4
                new double[] {  21.6,  37.412297443487752 },  // 4
                new double[] {   0.0,  37.412297443487752 },  // 2
                new double[] { -21.6,  37.412297443487752 },  // 2
                new double[] { -32.4,  18.706148721743876 },  // 4
                new double[] { -43.2,   0.0 },                // 5
                new double[] { -32.4, -18.706148721743876 },  // 2
                new double[] { -21.6, -37.412297443487752 },  // 3
                new double[] {   0.0, -37.412297443487752 },  // 1
                new double[] {  21.6, -37.412297443487752 },  // 1
                new double[] {  32.4, -18.706148721743876 },  // 0
                new double[] {  43.2,   0.0 }                 // 0
            };

            Accord.Math.Random.Generator.Seed = 0;
            var kmeans = new BalancedKMeans(numClusters);

            // If a limit is not set, the following Learn call does not return....
            kmeans.MaxIterations = 1000;

            var clusters = kmeans.Learn(observations);

            int[] labels = kmeans.Labels;
            int[] expected = new [] { 5, 0, 3, 3, 5, 1, 4, 4, 2, 2, 4, 5, 2, 3, 1, 1, 0, 0 };

            string str = labels.ToCSharp();

            Assert.IsTrue(labels.IsEqual(expected));

            int[] hist = Accord.Math.Vector.Histogram(labels);

            for (int i = 0; i < hist.Length; i++)
                Assert.AreEqual(hist[i], 3);
        }

    }
}
