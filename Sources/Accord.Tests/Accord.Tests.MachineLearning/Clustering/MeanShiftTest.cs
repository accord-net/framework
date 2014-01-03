// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Math;
    using Accord.Statistics.Distributions.DensityKernels;

    [TestClass()]
    public class MeanShiftTest
    {

        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }



        [TestMethod()]
        public void MeanShiftConstructorTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            // Test Samples
            double[][] samples =
            {
                new double[] { 0, 1 },
                new double[] { 1, 2 }, 
                new double[] { 1, 1 },
                new double[] { 0, 7 },
                new double[] { 1, 1 },
                new double[] { 6, 2 },
                new double[] { 6, 5 },
                new double[] { 5, 1 },
                new double[] { 7, 1 },
                new double[] { 5, 1 }
            };


            var kernel = new GaussianKernel(dimension: 2);
            MeanShift meanShift = new MeanShift(2, kernel, 3);

            // Compute the model (estimate)
            int[] labels = meanShift.Compute(samples);

            int a = 0;
            int b = 1;

            if (0.2358896594197982.IsRelativelyEqual(meanShift.Clusters.Modes[1][0], 1e-10))
            {
                a = 1;
                b = 0;
            }

            for (int i = 0; i < 5; i++)
                Assert.AreEqual(a, labels[i]);

            for (int i = 5; i < samples.Length; i++)
                Assert.AreEqual(b, labels[i]);

            Assert.AreEqual(0.2358896594197982, meanShift.Clusters.Modes[a][0], 1e-10);
            Assert.AreEqual(1.0010865560750339, meanShift.Clusters.Modes[a][1], 1e-10);

            Assert.AreEqual(6.7284908155626031, meanShift.Clusters.Modes[b][0], 1e-10);
            Assert.AreEqual(1.2713970467590967, meanShift.Clusters.Modes[b][1], 1e-10);

            Assert.AreEqual(2, meanShift.Clusters.Count);
            Assert.AreEqual(2, meanShift.Clusters.Modes.Length);
        }

        [TestMethod()]
        public void MeanShiftConstructorTest2()
        {

            Accord.Math.Tools.SetupGenerator(1);


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

            // Create a uniform kernel density function
            UniformKernel kernel = new UniformKernel();

            // Create a new Mean-Shift algorithm for 3 dimensional samples
            MeanShift meanShift = new MeanShift(dimension: 3, kernel: kernel, bandwidth: 1.5 );

            // Compute the algorithm, retrieving an integer array
            //  containing the labels for each of the observations
            int[] labels = meanShift.Compute(observations);

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


            int[] labels2 = meanShift.Clusters.Nearest(observations);
            Assert.IsTrue(labels.IsEqual(labels2));

            // the data must not have changed!
            Assert.IsTrue(orig.IsEqual(observations));
        }


    }
}
