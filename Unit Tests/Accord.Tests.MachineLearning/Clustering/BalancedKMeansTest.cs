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
    using Math.Distances;
    using Accord.DataSets;

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

            int[] labels2 = kmeans.Clusters.Decide(observations);
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
            BalancedKMeans kmeans = new BalancedKMeans(3, new Manhattan())
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

            int[] labels2 = kmeans.Clusters.Decide(observations);
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

            Assert.Throws<ArgumentException>(() => kmeans.Compute(observations),
                "Not enough points. There should be more points than the number K of clusters.");
        }

        [Test]
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

            Assert.Throws<InvalidOperationException>(() => kmeans.Compute(observations), "");
        }

        [Test]
        public void uniform_test()
        {
            // https://github.com/accord-net/framework/issues/451

            int numClusters = 6;

            double[][] observations =
            {
                new double[] {  10.8,   18.706148721743876 },
                new double[] { -10.8,   18.706148721743876 },
                new double[] { -21.6,   0.0 },
                new double[] { -10.8, -18.706148721743876 },
                new double[] {  10.8, -18.706148721743876 },
                new double[] {  21.6,   0.0 },
                new double[] {  32.4,  18.706148721743876 },
                new double[] {  21.6,  37.412297443487752 },
                new double[] {   0.0,  37.412297443487752 },
                new double[] { -21.6,  37.412297443487752 },
                new double[] { -32.4,  18.706148721743876 },
                new double[] { -43.2,   0.0 },
                new double[] { -32.4, -18.706148721743876 },
                new double[] { -21.6, -37.412297443487752 },
                new double[] {   0.0, -37.412297443487752 },
                new double[] {  21.6, -37.412297443487752 },
                new double[] {  32.4, -18.706148721743876 },
                new double[] {  43.2,   0.0 }
            };

            Accord.Math.Random.Generator.Seed = 0;
            var kmeans = new BalancedKMeans(numClusters)
            {
                // If a limit is not set, the following Learn call does not return....
                MaxIterations = 1000
            };

            var clusters = kmeans.Learn(observations);

            int[] labels = kmeans.Labels;
            int[] expected = new[] { 2, 4, 4, 4, 0, 3, 3, 3, 2, 2, 5, 5, 5, 0, 0, 1, 1, 1 };

            string str = labels.ToCSharp();

            Assert.IsTrue(labels.IsEqual(expected));

            int[] hist = Accord.Math.Vector.Histogram(labels);

            for (int i = 0; i < hist.Length; i++)
                Assert.AreEqual(hist[i], 3);
        }

        [Test]
        public void distances_test()
        {
            int numClusters = 6;

            double[][] observations =
            {
                new double[] {  10.8,   18.706148721743876 },
                new double[] { -10.8,   18.706148721743876 },
                new double[] { -21.6,   0.0 },
                new double[] { -10.8, -18.706148721743876 },
                new double[] {  10.8, -18.706148721743876 },
                new double[] {  21.6,   0.0 },
                new double[] {  32.4,  18.706148721743876 },
                new double[] {  21.6,  37.412297443487752 },
                new double[] {   0.0,  37.412297443487752 },
                new double[] { -21.6,  37.412297443487752 },
                new double[] { -32.4,  18.706148721743876 },
                new double[] { -43.2,   0.0 },
                new double[] { -32.4, -18.706148721743876 },
                new double[] { -21.6, -37.412297443487752 },
                new double[] {   0.0, -37.412297443487752 },
                new double[] {  21.6, -37.412297443487752 },
                new double[] {  32.4, -18.706148721743876 },
                new double[] {  43.2,   0.0 }
            };

            var distance = new SquareEuclidean();
            var centroids = new[]
            {
                new[] {   0.00000,   37.41230 },
                new[] { -32.40000,  -18.70615 },
                new[] {  10.80000,   18.70615 },
                new[] { -21.60000,   37.41230 },
                new[] { -10.80000,  -18.70615 },
                new[] {  21.60000,  -37.41230 },
            };

            double[,] expectedDistances =
            {
                { 3.2659e+003, 1.8662e+003, 4.6656e+002, 4.6656e+002, 1.8662e+003, 3.2659e+003, 5.5987e+003, 6.0653e+003, 4.1990e+003, 3.2659e+003, 1.3997e+003, 4.6656e+002, 1.6339e-012, 4.6656e+002, 1.3997e+003, 3.2659e+003, 4.1990e+003, 6.0653e+003 },
                { 1.6339e-012, 4.6656e+002, 1.3997e+003, 1.8662e+003, 1.3997e+003, 4.6656e+002, 4.6656e+002, 4.6656e+002, 4.6656e+002, 1.3997e+003, 1.8662e+003, 3.2659e+003, 3.2659e+003, 4.1990e+003, 3.2659e+003, 3.2659e+003, 1.8662e+003, 1.3997e+003 },
                { 1.3997e+003, 4.6656e+002, 1.3997e+003, 3.2659e+003, 4.1990e+003, 3.2659e+003, 3.2659e+003, 1.8662e+003, 4.6656e+002, 6.5358e-012, 4.6656e+002, 1.8662e+003, 3.2659e+003, 5.5987e+003, 6.0653e+003, 7.4650e+003, 6.0653e+003, 5.5987e+003 },
                { 1.8662e+003, 1.3997e+003, 4.6656e+002, 1.6339e-012, 4.6656e+002, 1.3997e+003, 3.2659e+003, 4.1990e+003, 3.2659e+003, 3.2659e+003, 1.8662e+003, 1.3997e+003, 4.6656e+002, 4.6656e+002, 4.6656e+002, 1.3997e+003, 1.8662e+003, 3.2659e+003 },
                { 3.2659e+003, 4.1990e+003, 3.2659e+003, 1.3997e+003, 4.6656e+002, 1.3997e+003, 3.2659e+003, 5.5987e+003, 6.0653e+003, 7.4650e+003, 6.0653e+003, 5.5987e+003, 3.2659e+003, 1.8662e+003, 4.6656e+002, 6.5358e-012, 4.6656e+002, 1.8662e+003 },
                { 4.6656e+002, 4.6656e+002, 1.8662e+003, 3.2659e+003, 3.2659e+003, 1.8662e+003, 1.3997e+003, 4.6656e+002, 6.5358e-012, 4.6656e+002, 1.3997e+003, 3.2659e+003, 4.1990e+003, 6.0653e+003, 5.5987e+003, 6.0653e+003, 4.1990e+003, 3.2659e+003 },
                { 3.2659e+003, 1.8662e+003, 4.6656e+002, 4.6656e+002, 1.8662e+003, 3.2659e+003, 5.5987e+003, 6.0653e+003, 4.1990e+003, 3.2659e+003, 1.3997e+003, 4.6656e+002, 1.6339e-012, 4.6656e+002, 1.3997e+003, 3.2659e+003, 4.1990e+003, 6.0653e+003 },
                { 1.6339e-012, 4.6656e+002, 1.3997e+003, 1.8662e+003, 1.3997e+003, 4.6656e+002, 4.6656e+002, 4.6656e+002, 4.6656e+002, 1.3997e+003, 1.8662e+003, 3.2659e+003, 3.2659e+003, 4.1990e+003, 3.2659e+003, 3.2659e+003, 1.8662e+003, 1.3997e+003 },
                { 1.3997e+003, 4.6656e+002, 1.3997e+003, 3.2659e+003, 4.1990e+003, 3.2659e+003, 3.2659e+003, 1.8662e+003, 4.6656e+002, 6.5358e-012, 4.6656e+002, 1.8662e+003, 3.2659e+003, 5.5987e+003, 6.0653e+003, 7.4650e+003, 6.0653e+003, 5.5987e+003 },
                { 1.8662e+003, 1.3997e+003, 4.6656e+002, 1.6339e-012, 4.6656e+002, 1.3997e+003, 3.2659e+003, 4.1990e+003, 3.2659e+003, 3.2659e+003, 1.8662e+003, 1.3997e+003, 4.6656e+002, 4.6656e+002, 4.6656e+002, 1.3997e+003, 1.8662e+003, 3.2659e+003 },
                { 3.2659e+003, 4.1990e+003, 3.2659e+003, 1.3997e+003, 4.6656e+002, 1.3997e+003, 3.2659e+003, 5.5987e+003, 6.0653e+003, 7.4650e+003, 6.0653e+003, 5.5987e+003, 3.2659e+003, 1.8662e+003, 4.6656e+002, 6.5358e-012, 4.6656e+002, 1.8662e+003 },
                { 4.6656e+002, 4.6656e+002, 1.8662e+003, 3.2659e+003, 3.2659e+003, 1.8662e+003, 1.3997e+003, 4.6656e+002, 6.5358e-012, 4.6656e+002, 1.3997e+003, 3.2659e+003, 4.1990e+003, 6.0653e+003, 5.5987e+003, 6.0653e+003, 4.1990e+003, 3.2659e+003 },
                { 3.2659e+003, 1.8662e+003, 4.6656e+002, 4.6656e+002, 1.8662e+003, 3.2659e+003, 5.5987e+003, 6.0653e+003, 4.1990e+003, 3.2659e+003, 1.3997e+003, 4.6656e+002, 1.6339e-012, 4.6656e+002, 1.3997e+003, 3.2659e+003, 4.1990e+003, 6.0653e+003 },
                { 1.6339e-012, 4.6656e+002, 1.3997e+003, 1.8662e+003, 1.3997e+003, 4.6656e+002, 4.6656e+002, 4.6656e+002, 4.6656e+002, 1.3997e+003, 1.8662e+003, 3.2659e+003, 3.2659e+003, 4.1990e+003, 3.2659e+003, 3.2659e+003, 1.8662e+003, 1.3997e+003 },
                { 1.3997e+003, 4.6656e+002, 1.3997e+003, 3.2659e+003, 4.1990e+003, 3.2659e+003, 3.2659e+003, 1.8662e+003, 4.6656e+002, 6.5358e-012, 4.6656e+002, 1.8662e+003, 3.2659e+003, 5.5987e+003, 6.0653e+003, 7.4650e+003, 6.0653e+003, 5.5987e+003 },
                { 1.8662e+003, 1.3997e+003, 4.6656e+002, 1.6339e-012, 4.6656e+002, 1.3997e+003, 3.2659e+003, 4.1990e+003, 3.2659e+003, 3.2659e+003, 1.8662e+003, 1.3997e+003, 4.6656e+002, 4.6656e+002, 4.6656e+002, 1.3997e+003, 1.8662e+003, 3.2659e+003 },
                { 3.2659e+003, 4.1990e+003, 3.2659e+003, 1.3997e+003, 4.6656e+002, 1.3997e+003, 3.2659e+003, 5.5987e+003, 6.0653e+003, 7.4650e+003, 6.0653e+003, 5.5987e+003, 3.2659e+003, 1.8662e+003, 4.6656e+002, 6.5358e-012, 4.6656e+002, 1.8662e+003 },
                { 4.6656e+002, 4.6656e+002, 1.8662e+003, 3.2659e+003, 3.2659e+003, 1.8662e+003, 1.3997e+003, 4.6656e+002, 6.5358e-012, 4.6656e+002, 1.3997e+003, 3.2659e+003, 4.1990e+003, 6.0653e+003, 5.5987e+003, 6.0653e+003, 4.1990e+003, 3.2659e+003 }
            };

            double[][] actualDistances = BalancedKMeans.GetDistances(distance, observations, centroids, numClusters, Jagged.Zeros(18, 18));

            for (int i = 0; i < actualDistances.Length; i++)
            {
                for (int j = 0; j < actualDistances[i].Length; j++)
                {
                    double a = actualDistances[i][j];
                    double e = expectedDistances[i, j];
                    Assert.AreEqual(e, a, 0.1);
                }
            }
        }

        [Test]
        public void uniform_test_min_variance()
        {
            // https://github.com/accord-net/framework/issues/451

            int numClusters = 6;

            double[][] observations =
            {
                new double[] {  10.8,   18.706148721743876 },
                new double[] { -10.8,   18.706148721743876 },
                new double[] { -21.6,   0.0 },
                new double[] { -10.8, -18.706148721743876 },
                new double[] {  10.8, -18.706148721743876 },
                new double[] {  21.6,   0.0 },
                new double[] {  32.4,  18.706148721743876 },
                new double[] {  21.6,  37.412297443487752 },
                new double[] {   0.0,  37.412297443487752 },
                new double[] { -21.6,  37.412297443487752 },
                new double[] { -32.4,  18.706148721743876 },
                new double[] { -43.2,   0.0 },
                new double[] { -32.4, -18.706148721743876 },
                new double[] { -21.6, -37.412297443487752 },
                new double[] {   0.0, -37.412297443487752 },
                new double[] {  21.6, -37.412297443487752 },
                new double[] {  32.4, -18.706148721743876 },
                new double[] {  43.2,   0.0 }
            };

            Accord.Math.Random.Generator.Seed = 0;
            var kmeans = new BalancedKMeans(numClusters)
            {
                Distance = new SquareEuclidean(),
                Centroids = new[]
                {
                    // C = [ 0.00000   37.41230 ; -32.40000,  -18.70615 ; 10.80000,   18.70615; -21.60000,   37.41230; -10.80000,  -18.70615; 21.60000,  -37.41230 ]
                      new[] {   0.00000,   37.41230 },
                      new[] { -32.40000,  -18.70615 },
                      new[] {  10.80000,   18.70615 },
                      new[] { -21.60000,   37.41230 },
                      new[] { -10.80000,  -18.70615 },
                      new[] {  21.60000,  -37.41230 },
                }
            };

            // If a limit is not set, the following Learn call does not return....
            kmeans.MaxIterations = 1;

            double[,] expectedCost =
           {
                { 3.2659e+003, 1.8662e+003, 4.6656e+002, 4.6656e+002, 1.8662e+003, 3.2659e+003, 5.5987e+003, 6.0653e+003, 4.1990e+003, 3.2659e+003, 1.3997e+003, 4.6656e+002, 1.6339e-012, 4.6656e+002, 1.3997e+003, 3.2659e+003, 4.1990e+003, 6.0653e+003 },
                { 1.6339e-012, 4.6656e+002, 1.3997e+003, 1.8662e+003, 1.3997e+003, 4.6656e+002, 4.6656e+002, 4.6656e+002, 4.6656e+002, 1.3997e+003, 1.8662e+003, 3.2659e+003, 3.2659e+003, 4.1990e+003, 3.2659e+003, 3.2659e+003, 1.8662e+003, 1.3997e+003 },
                { 1.3997e+003, 4.6656e+002, 1.3997e+003, 3.2659e+003, 4.1990e+003, 3.2659e+003, 3.2659e+003, 1.8662e+003, 4.6656e+002, 6.5358e-012, 4.6656e+002, 1.8662e+003, 3.2659e+003, 5.5987e+003, 6.0653e+003, 7.4650e+003, 6.0653e+003, 5.5987e+003 },
                { 1.8662e+003, 1.3997e+003, 4.6656e+002, 1.6339e-012, 4.6656e+002, 1.3997e+003, 3.2659e+003, 4.1990e+003, 3.2659e+003, 3.2659e+003, 1.8662e+003, 1.3997e+003, 4.6656e+002, 4.6656e+002, 4.6656e+002, 1.3997e+003, 1.8662e+003, 3.2659e+003 },
                { 3.2659e+003, 4.1990e+003, 3.2659e+003, 1.3997e+003, 4.6656e+002, 1.3997e+003, 3.2659e+003, 5.5987e+003, 6.0653e+003, 7.4650e+003, 6.0653e+003, 5.5987e+003, 3.2659e+003, 1.8662e+003, 4.6656e+002, 6.5358e-012, 4.6656e+002, 1.8662e+003 },
                { 4.6656e+002, 4.6656e+002, 1.8662e+003, 3.2659e+003, 3.2659e+003, 1.8662e+003, 1.3997e+003, 4.6656e+002, 6.5358e-012, 4.6656e+002, 1.3997e+003, 3.2659e+003, 4.1990e+003, 6.0653e+003, 5.5987e+003, 6.0653e+003, 4.1990e+003, 3.2659e+003 },
                { 3.2659e+003, 1.8662e+003, 4.6656e+002, 4.6656e+002, 1.8662e+003, 3.2659e+003, 5.5987e+003, 6.0653e+003, 4.1990e+003, 3.2659e+003, 1.3997e+003, 4.6656e+002, 1.6339e-012, 4.6656e+002, 1.3997e+003, 3.2659e+003, 4.1990e+003, 6.0653e+003 },
                { 1.6339e-012, 4.6656e+002, 1.3997e+003, 1.8662e+003, 1.3997e+003, 4.6656e+002, 4.6656e+002, 4.6656e+002, 4.6656e+002, 1.3997e+003, 1.8662e+003, 3.2659e+003, 3.2659e+003, 4.1990e+003, 3.2659e+003, 3.2659e+003, 1.8662e+003, 1.3997e+003 },
                { 1.3997e+003, 4.6656e+002, 1.3997e+003, 3.2659e+003, 4.1990e+003, 3.2659e+003, 3.2659e+003, 1.8662e+003, 4.6656e+002, 6.5358e-012, 4.6656e+002, 1.8662e+003, 3.2659e+003, 5.5987e+003, 6.0653e+003, 7.4650e+003, 6.0653e+003, 5.5987e+003 },
                { 1.8662e+003, 1.3997e+003, 4.6656e+002, 1.6339e-012, 4.6656e+002, 1.3997e+003, 3.2659e+003, 4.1990e+003, 3.2659e+003, 3.2659e+003, 1.8662e+003, 1.3997e+003, 4.6656e+002, 4.6656e+002, 4.6656e+002, 1.3997e+003, 1.8662e+003, 3.2659e+003 },
                { 3.2659e+003, 4.1990e+003, 3.2659e+003, 1.3997e+003, 4.6656e+002, 1.3997e+003, 3.2659e+003, 5.5987e+003, 6.0653e+003, 7.4650e+003, 6.0653e+003, 5.5987e+003, 3.2659e+003, 1.8662e+003, 4.6656e+002, 6.5358e-012, 4.6656e+002, 1.8662e+003 },
                { 4.6656e+002, 4.6656e+002, 1.8662e+003, 3.2659e+003, 3.2659e+003, 1.8662e+003, 1.3997e+003, 4.6656e+002, 6.5358e-012, 4.6656e+002, 1.3997e+003, 3.2659e+003, 4.1990e+003, 6.0653e+003, 5.5987e+003, 6.0653e+003, 4.1990e+003, 3.2659e+003 },
                { 3.2659e+003, 1.8662e+003, 4.6656e+002, 4.6656e+002, 1.8662e+003, 3.2659e+003, 5.5987e+003, 6.0653e+003, 4.1990e+003, 3.2659e+003, 1.3997e+003, 4.6656e+002, 1.6339e-012, 4.6656e+002, 1.3997e+003, 3.2659e+003, 4.1990e+003, 6.0653e+003 },
                { 1.6339e-012, 4.6656e+002, 1.3997e+003, 1.8662e+003, 1.3997e+003, 4.6656e+002, 4.6656e+002, 4.6656e+002, 4.6656e+002, 1.3997e+003, 1.8662e+003, 3.2659e+003, 3.2659e+003, 4.1990e+003, 3.2659e+003, 3.2659e+003, 1.8662e+003, 1.3997e+003 },
                { 1.3997e+003, 4.6656e+002, 1.3997e+003, 3.2659e+003, 4.1990e+003, 3.2659e+003, 3.2659e+003, 1.8662e+003, 4.6656e+002, 6.5358e-012, 4.6656e+002, 1.8662e+003, 3.2659e+003, 5.5987e+003, 6.0653e+003, 7.4650e+003, 6.0653e+003, 5.5987e+003 },
                { 1.8662e+003, 1.3997e+003, 4.6656e+002, 1.6339e-012, 4.6656e+002, 1.3997e+003, 3.2659e+003, 4.1990e+003, 3.2659e+003, 3.2659e+003, 1.8662e+003, 1.3997e+003, 4.6656e+002, 4.6656e+002, 4.6656e+002, 1.3997e+003, 1.8662e+003, 3.2659e+003 },
                { 3.2659e+003, 4.1990e+003, 3.2659e+003, 1.3997e+003, 4.6656e+002, 1.3997e+003, 3.2659e+003, 5.5987e+003, 6.0653e+003, 7.4650e+003, 6.0653e+003, 5.5987e+003, 3.2659e+003, 1.8662e+003, 4.6656e+002, 6.5358e-012, 4.6656e+002, 1.8662e+003 },
                { 4.6656e+002, 4.6656e+002, 1.8662e+003, 3.2659e+003, 3.2659e+003, 1.8662e+003, 1.3997e+003, 4.6656e+002, 6.5358e-012, 4.6656e+002, 1.3997e+003, 3.2659e+003, 4.1990e+003, 6.0653e+003, 5.5987e+003, 6.0653e+003, 4.1990e+003, 3.2659e+003 }
            };

            Assert.AreEqual(0, kmeans.Iterations);
            var clusters = kmeans.Learn(observations);
            Assert.AreEqual(1, kmeans.Iterations);

            double[][] actualCost = kmeans.munkres.CostMatrix;
            for (int i = 0; i < actualCost.Length; i++)
            {
                for (int j = 0; j < actualCost[i].Length; j++)
                {
                    double a = actualCost[i][j];
                    double e = expectedCost[i, j];
                    Assert.AreEqual(e, a, 0.1);
                }
            }

            int[] labels = kmeans.Labels;
            int[] hist = Accord.Math.Vector.Histogram(labels);
            for (int i = 0; i < hist.Length; i++)
                Assert.AreEqual(hist[i], 3);
        }

        [Test]
        public void getlabels()
        {
            // https://github.com/accord-net/framework/issues/451

            int numClusters = 6;

            double[][] observations =
            {
                new double[] {  10.8,   18.706148721743876 },
                new double[] { -10.8,   18.706148721743876 },
                new double[] { -21.6,   0.0 },
                new double[] { -10.8, -18.706148721743876 },
                new double[] {  10.8, -18.706148721743876 },
                new double[] {  21.6,   0.0 },
                new double[] {  32.4,  18.706148721743876 },
                new double[] {  21.6,  37.412297443487752 },
                new double[] {   0.0,  37.412297443487752 },
                new double[] { -21.6,  37.412297443487752 },
                new double[] { -32.4,  18.706148721743876 },
                new double[] { -43.2,   0.0 },
                new double[] { -32.4, -18.706148721743876 },
                new double[] { -21.6, -37.412297443487752 },
                new double[] {   0.0, -37.412297443487752 },
                new double[] {  21.6, -37.412297443487752 },
                new double[] {  32.4, -18.706148721743876 },
                new double[] {  43.2,   0.0 }
            };

            double[] solution = (new double[] { 3, 18, 2, 4, 17, 8, 12, 6, 10, 14, 15, 9, 13, 7, 11, 5, 16, 1 }).Subtract(1);

            int[] actualLabels = new int[observations.Length];
            BalancedKMeans.GetLabels(observations, numClusters, solution, actualLabels);

            int[] expectedLabels = (new int[] { 1, 4, 2, 5, 5, 3, 3, 1, 1, 4, 4, 2, 2, 5, 6, 6, 6, 3 }).Subtract(1);
            for (int j = 0; j < actualLabels.Length; j++)
            {
                double a = actualLabels[j];
                double e = expectedLabels[j];
                Assert.AreEqual(e, a, 0.1);
            }
        }


        [Test]
        public void yinyang_test()
        {
            // https://github.com/accord-net/framework/issues/451

            double[][] observations = new YinYang().Instances;

            Accord.Math.Random.Generator.Seed = 0;

            var kmeans = new BalancedKMeans(2)
            {

                // If a limit is not set, the following Learn call does not return....
                MaxIterations = 1000
            };

            var clusters = kmeans.Learn(observations);

            int[] labels = kmeans.Labels;
            int[] expected = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1 };

            string str = labels.ToCSharp();

            Assert.IsTrue(labels.IsEqual(expected));

            int[] hist = Accord.Math.Vector.Histogram(labels);

            for (int i = 0; i < hist.Length; i++)
                Assert.AreEqual(hist[i], 50);
        }

        [Test]
        public void gh_684()
        {
            // https://github.com/accord-net/framework/issues/684
            Accord.Math.Random.Generator.Seed = 0;

            var balancedKMeans = new Accord.MachineLearning.BalancedKMeans(3);

            balancedKMeans.MaxIterations = 1;

            var matrix = new double[34][];
            matrix[00] = new double[] { 1, 1, 26524 };
            matrix[01] = new double[] { 1, 1, 87 };
            matrix[02] = new double[] { 1, 1, 260 };
            matrix[03] = new double[] { 1, 1, 1179 };
            matrix[04] = new double[] { 1, 1, 264 };
            matrix[05] = new double[] { 1, 1, 227 };
            matrix[06] = new double[] { 1, 1, 176 };
            matrix[07] = new double[] { 1, 1, 16 };
            matrix[08] = new double[] { 1, 1, 995 };
            matrix[09] = new double[] { 1, 1, 438 };
            matrix[10] = new double[] { 1, 1, 28 };
            matrix[11] = new double[] { 1, 1, 957 };
            matrix[12] = new double[] { 1, 1, 91 };
            matrix[13] = new double[] { 1, 1, 666 };
            matrix[14] = new double[] { 1, 1, 1157 };
            matrix[15] = new double[] { 1, 1, 968 };
            matrix[16] = new double[] { 1, 1, 34 };
            matrix[17] = new double[] { 1, 1, 1385 };
            matrix[18] = new double[] { 1, 1, 430 };
            matrix[19] = new double[] { 1, 1, 1247 };
            matrix[20] = new double[] { 1, 1, 1536 };
            matrix[21] = new double[] { 1, 1, 1074 };
            matrix[22] = new double[] { 1, 1, 1316 };
            matrix[23] = new double[] { 1, 1, 217 };
            matrix[24] = new double[] { 1, 1, 475 };
            matrix[25] = new double[] { 1, 1, 1036 };
            matrix[26] = new double[] { 1, 1, 343 };
            matrix[27] = new double[] { 1, 1, 987 };
            matrix[28] = new double[] { 1, 1, 189 };
            matrix[29] = new double[] { 1, 1, 157 };
            matrix[30] = new double[] { 1, 1, 903 };
            matrix[31] = new double[] { 1, 1, 17 };
            matrix[32] = new double[] { 1, 1, 1323 };
            matrix[33] = new double[] { 1, 1, 668 };

            var clusters = balancedKMeans.Learn(matrix);

            Assert.AreEqual(3, clusters.NumberOfClasses);
            Assert.AreEqual(3, clusters.NumberOfInputs);
            Assert.AreEqual(3, clusters.NumberOfOutputs);
            Assert.AreEqual(3, clusters.Proportions.Length);
            Assert.AreEqual(0.3235294117647059, clusters.Proportions[0], 1e-6);
            Assert.AreEqual(0.35294117647058826, clusters.Proportions[1], 1e-6);
            Assert.AreEqual(0.3235294117647059, clusters.Proportions[2], 1e-6);
            Assert.IsTrue(balancedKMeans.ComputeCovariances);
            Assert.IsTrue(balancedKMeans.ComputeError);

            for (int i = 0; i < clusters.Covariances.Length; i++)
                Assert.IsNotNull(clusters.Covariances[i]);
        }
    }
}
