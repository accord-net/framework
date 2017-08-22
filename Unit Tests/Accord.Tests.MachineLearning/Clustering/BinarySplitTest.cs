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

    [TestFixture]
    public class BinarySplitTest
    {

        [Test]
        public void BinarySplitConstructorTest()
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

            // Create a new binary split with 3 clusters 
            BinarySplit binarySplit = new BinarySplit(3);

            // Compute the algorithm, retrieving an integer array
            //  containing the labels for each of the observations
            int[] labels = binarySplit.Compute(observations);

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


            int[] labels2 = binarySplit.Clusters.Decide(observations);

            Assert.IsTrue(labels.IsEqual(labels2));

            // the data must not have changed!
            Assert.IsTrue(orig.IsEqual(observations));
        }

        [Test]
        public void binary_split_new_method()
        {
            #region doc_sample1
            // Use a fixed seed for reproducibility
            Accord.Math.Random.Generator.Seed = 0;

            // Declare some data to be clustered
            double[][] input =
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

            // Create a new binary split with 3 clusters 
            BinarySplit binarySplit = new BinarySplit(3);

            // Learn a data partitioning using the Binary Split algorithm
            KMeansClusterCollection clustering = binarySplit.Learn(input);

            // Predict group labels for each point
            int[] output = clustering.Decide(input);

            // As a result, the first two observations should belong to the
            //  same cluster (thus having the same label). The same should
            //  happen to the next four observations and to the last three.
            #endregion

            Assert.AreEqual(output[0], output[1]);

            Assert.AreEqual(output[2], output[3]);
            Assert.AreEqual(output[2], output[4]);
            Assert.AreEqual(output[2], output[5]);

            Assert.AreEqual(output[6], output[7]);
            Assert.AreEqual(output[6], output[8]);

            Assert.AreNotEqual(output[0], output[2]);
            Assert.AreNotEqual(output[2], output[6]);
            Assert.AreNotEqual(output[0], output[6]);

            int[] labels2 = binarySplit.Clusters.Decide(input);

            Assert.IsTrue(output.IsEqual(labels2));
        }

        [Test]
        public void binary_split_information_test()
        {
            // Use a fixed seed for reproducibility
            Accord.Math.Random.Generator.Seed = 0;

            // Declare some data to be clustered
            double[][] input =
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

            // Create a new binary split with 3 clusters 
            BinarySplit binarySplit = new BinarySplit(3)
            {
                ComputeProportions = true,
                ComputeCovariances = true,
                ComputeError = true,
            };

            // Learn a data partitioning using the Binary Split algorithm
            KMeansClusterCollection clustering = binarySplit.Learn(input);

            string str = clustering.Proportions.ToCSharp();
            double[] expectedProportions = new double[] { 0.333333333333333, 0.444444444444444, 0.222222222222222 };
            Assert.IsTrue(expectedProportions.IsEqual(clustering.Proportions, 1e-10));

            var strs = clustering.Covariances.Apply(x=>x.ToCSharp());
            double[][][] expectedCovar = 
            {
                new double[][] 
                {
                    new double[] { 7, 0, 1 },
                    new double[] { 0, 0, 0 },
                    new double[] { 1, 0, 1.33333333333333 }
                },
                new double[][] 
                {
                    new double[] { 0.916666666666667, -0.25, -0.0833333333333333 },
                    new double[] { -0.25, 0.25, 0.0833333333333333 },
                    new double[] { -0.0833333333333333, 0.0833333333333333, 0.25 }
                },
                new double[][]
                {
                    new double[] { 0, 0, 0 },
                    new double[] { 0, 4.5, 7.5 },
                    new double[] { 0, 7.5, 12.5 }
                }
            };

            Assert.IsTrue(expectedCovar.IsEqual(clustering.Covariances, 1e-10));
        }

        [Test]
        public void BinarySplitConstructorTest3()
        {
            // Create a new algorithm
            BinarySplit binarySplit = new BinarySplit(3);
            Assert.IsNotNull(binarySplit.Clusters);
            Assert.IsNotNull(binarySplit.Distance);
            Assert.IsNotNull(binarySplit.Clusters.Centroids);
            Assert.IsNotNull(binarySplit.Clusters.Count);
            Assert.IsNotNull(binarySplit.Clusters.Covariances);
            Assert.IsNotNull(binarySplit.Clusters.Proportions);
        }
    }
}
