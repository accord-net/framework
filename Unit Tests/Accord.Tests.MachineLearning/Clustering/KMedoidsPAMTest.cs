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
    using NUnit.Framework;
    using Accord.MachineLearning;
    using Accord.Math;

    [TestFixture]
    public class KMedoidsTest
    {
        [Test]
        public void KMedoidsConstructorTest()
        {
            Accord.Math.Random.Generator.Seed = 0;

            // Declare some observations
            int[][] observations = new int[][]
            {
                new[] { 2, 6 }, // a
                new[] { 3, 4 }, // a
                new[] { 3, 8 }, // a
                new[] { 4, 7 }, // a
                new[] { 6, 2 }, // b
                new[] { 6, 4 }, // b
                new[] { 7, 3 }, // b
                new[] { 7, 4 }, // b
                new[] { 8, 5 }, // b
                new[] { 7, 6 }  // b 
            };

            int[][] orig = observations.MemberwiseClone();

            // Create a new K-Modes algorithm with 2 clusters 
            PartitioningAroundMedoids kmedoidsPam = new PartitioningAroundMedoids(2);

            // Compute the algorithm, retrieving an integer array
            //  containing the labels for each of the observations
            int[] labels = kmedoidsPam.Compute(observations);

            // As a result, the first four observations should belong to the
            //  same cluster (thus having the same label). The same should
            //  happen to the next five observations.

            Assert.AreEqual(labels[0], labels[2]);
            Assert.AreEqual(labels[0], labels[3]);

            Assert.AreEqual(labels[4], labels[1]);
            Assert.AreEqual(labels[4], labels[5]);
            Assert.AreEqual(labels[4], labels[6]);
            Assert.AreEqual(labels[4], labels[7]);
            Assert.AreEqual(labels[4], labels[8]);
            Assert.AreEqual(labels[4], labels[9]);

            int[] labels2 = kmedoidsPam.Clusters.Decide(observations);
            Assert.IsTrue(labels.IsEqual(labels2));

            // the data must not have changed!
            Assert.IsTrue(orig.IsEqual(observations));
        }


        [Test]
        public void doc_learn()
        {
            #region doc_learn
            Accord.Math.Random.Generator.Seed = 0;

            // Declare some observations
            int[][] observations = new int[][]
            {
                new[] { 2, 6 }, // a
                new[] { 3, 4 }, // a
                new[] { 3, 8 }, // a
                new[] { 4, 7 }, // a
                new[] { 6, 2 }, // b
                new[] { 6, 4 }, // b
                new[] { 7, 3 }, // b
                new[] { 7, 4 }, // b
                new[] { 8, 5 }, // b
                new[] { 7, 6 }  // b
            };

            // Create a new 2-Medoids algorithm.
            var kmedoidsPam = new PartitioningAroundMedoids(2);
            kmedoidsPam.MaxIterations = 100;

            // Compute and retrieve the data centroids
            var clusters = kmedoidsPam.Learn(observations);

            // Use the centroids to parition all the data
            int[] labels = clusters.Decide(observations);
            #endregion

            Assert.AreEqual(labels[0], labels[2]);
            Assert.AreEqual(labels[0], labels[3]);

            Assert.AreEqual(labels[4], labels[1]);
            Assert.AreEqual(labels[4], labels[5]);
            Assert.AreEqual(labels[4], labels[6]);
            Assert.AreEqual(labels[4], labels[7]);
            Assert.AreEqual(labels[4], labels[8]);
            Assert.AreEqual(labels[4], labels[9]);
        }
    }
}
