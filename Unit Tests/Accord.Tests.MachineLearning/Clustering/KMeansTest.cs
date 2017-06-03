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


            int[] labels2 = kmeans.Clusters.Nearest(observations);
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

            double[][] orig = observations.MemberwiseClone();

            // Create a new K-Means algorithm with 3 clusters 
            KMeans kmeans = new KMeans(3);

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

        [Test]
        public void DeserializationTest1()
        {
            MemoryStream stream = new MemoryStream(Properties.Resources.kmeans);

            KMeans kmeans = Serializer.Load<KMeans>(stream);


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

    }
}
