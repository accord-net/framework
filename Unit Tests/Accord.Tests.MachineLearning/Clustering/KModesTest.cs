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
    using NUnit.Framework;
    using System;
    using Accord.Math;
    using Accord.Statistics;
    using Accord.Math.Distances;


    [TestFixture]
    public class KModesTest
    {

        [Test]
        public void KModesConstructorTest()
        {
            Accord.Math.Random.Generator.Seed = 0;

            // Declare some observations
            int[][] observations =
            {
                new int[] { 0, 0   }, // a
                new int[] { 0, 1   }, // a
                new int[] { 0, 1   }, // a
                new int[] { 1, 1   }, // a
 
                new int[] { 5, 3   }, // b
                new int[] { 6, 8   }, // b
                new int[] { 6, 8   }, // b
                new int[] { 6, 7   }, // b
                new int[] { 5, 8   }, // b

                new int[] { 12, 14 }, // c
                new int[] { 12, 14 }, // c
                new int[] { 13, 14 }, // c
            };

            int[][] orig = observations.MemberwiseClone();

            // Create a new K-Modes algorithm with 3 clusters 
            KModes kmodes = new KModes(3);

            // Compute the algorithm, retrieving an integer array
            //  containing the labels for each of the observations
            int[] labels = kmodes.Compute(observations);

            // As a result, the first three observations should belong to the
            //  same cluster (thus having the same label). The same should
            //  happen to the next four observations and to the last two.

            Assert.AreEqual(labels[0], labels[1]);
            Assert.AreEqual(labels[0], labels[2]);
            Assert.AreEqual(labels[0], labels[3]);

            Assert.AreEqual(labels[4], labels[5]);
            Assert.AreEqual(labels[4], labels[6]);
            Assert.AreEqual(labels[4], labels[7]);
            Assert.AreEqual(labels[4], labels[8]);

            Assert.AreEqual(labels[9], labels[10]);
            Assert.AreEqual(labels[9], labels[11]);

            Assert.AreNotEqual(labels[0], labels[4]);
            Assert.AreNotEqual(labels[0], labels[9]);
            Assert.AreNotEqual(labels[4], labels[9]);


            int[] labels2 = kmodes.Clusters.Decide(observations);
            Assert.IsTrue(labels.IsEqual(labels2));

            // the data must not have changed!
            Assert.IsTrue(orig.IsEqual(observations));
        }


        [Test]
        public void KModesConstructorTestHamming()
        {
            Accord.Math.Random.Generator.Seed = 0;

            // Declare some observations
            byte[][] observations = new int[][]
            {
                new int[] { 0, 0   }, // a
                new int[] { 0, 1   }, // a
                new int[] { 0, 1   }, // a
                new int[] { 1, 1   }, // a
 
                new int[] { 5, 3   }, // b
                new int[] { 6, 8   }, // b
                new int[] { 6, 8   }, // b
                new int[] { 6, 7   }, // b
                new int[] { 5, 8   }, // b

                new int[] { 12, 14 }, // c
                new int[] { 12, 14 }, // c
                new int[] { 13, 14 }, // c
            }.To<byte[][]>();

            byte[][] orig = observations.MemberwiseClone();

            // Create a new K-Modes algorithm with 3 clusters 
            var kmodes = new KModes<byte>(3, Distance.BitwiseHamming);

            Assert.IsTrue(kmodes.Distance is Hamming);

            int[] labels = kmodes.Compute(observations);

            Assert.AreEqual(labels[0], labels[1]);
            Assert.AreEqual(labels[0], labels[2]);
            Assert.AreEqual(labels[0], labels[3]);
            Assert.AreEqual(labels[0], labels[4]);
            Assert.AreEqual(labels[0], labels[7]);

            Assert.AreEqual(labels[5], labels[6]);
            Assert.AreEqual(labels[5], labels[8]);

            Assert.AreEqual(labels[9], labels[10]);
            Assert.AreEqual(labels[9], labels[11]);

            Assert.AreNotEqual(labels[0], labels[5]);
            Assert.AreNotEqual(labels[5], labels[7]);
            Assert.AreNotEqual(labels[0], labels[9]);



            // the data must not have changed!
            Assert.IsTrue(orig.IsEqual(observations));
        }


        [Test]
        public void doc_learn()
        {
            #region doc_learn
            Accord.Math.Random.Generator.Seed = 0;

            // Declare some observations
            byte[][] observations = new[]
            {
                new byte[] { 0, 0   }, // a
                new byte[] { 0, 1   }, // a
                new byte[] { 0, 1   }, // a
                new byte[] { 1, 1   }, // a
 
                new byte[] { 5, 3   }, // b
                new byte[] { 6, 8   }, // b
                new byte[] { 6, 8   }, // b
                new byte[] { 6, 7   }, // b
                new byte[] { 5, 8   }, // b

                new byte[] { 12, 14 }, // c
                new byte[] { 12, 14 }, // c
                new byte[] { 13, 14 }, // c
            };

            // Create a new 3-Modes algorithm using the Hamming distance
            var kmodes = new KModes<byte>(k: 3, distance: new Hamming())
            {
                MaxIterations = 100
            };

            // Compute and retrieve the data centroids
            var clusters = kmodes.Learn(observations);

            // Use the centroids to parition all the data
            int[] labels = clusters.Decide(observations);
            #endregion



            Assert.AreEqual(labels[0], labels[1]);
            Assert.AreEqual(labels[0], labels[2]);
            Assert.AreEqual(labels[0], labels[3]);
            Assert.AreEqual(labels[0], labels[4]);
            Assert.AreEqual(labels[0], labels[7]);

            Assert.AreEqual(labels[5], labels[6]);
            Assert.AreEqual(labels[5], labels[8]);

            Assert.AreEqual(labels[9], labels[10]);
            Assert.AreEqual(labels[9], labels[11]);

            Assert.AreNotEqual(labels[0], labels[5]);
            Assert.AreNotEqual(labels[5], labels[7]);
            Assert.AreNotEqual(labels[0], labels[9]);
        }

    }
}
