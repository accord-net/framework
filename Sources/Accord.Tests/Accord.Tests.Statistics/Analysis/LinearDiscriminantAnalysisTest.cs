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

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Analysis;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Math;

    [TestClass()]
    public class LinearDiscriminantAnalysisTest
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



        // This is the same data used in the example by Gutierrez-Osuna
        // http://research.cs.tamu.edu/prism/lectures/pr/pr_l10.pdf

        private static double[,] inputs =
        {
            {  4,  1 }, // Class 1
            {  2,  4 },
            {  2,  3 },
            {  3,  6 },
            {  4,  4 },

            {  9, 10 }, // Class 2
            {  6,  8 },
            {  9,  5 },
            {  8,  7 },
            { 10,  8 }
        };

        private static int[] output = 
        {
            1, 1, 1, 1, 1, // Class labels for the input vectors
            2, 2, 2, 2, 2
        };


        [TestMethod()]
        public void ComputeTest()
        {
            LinearDiscriminantAnalysis lda = new LinearDiscriminantAnalysis(inputs, output);

            // Compute the analysis
            lda.Compute();

            double[,] expectedScatter1 = 
            {
                {  0.80, -0.40 }, 
                { -0.40,  2.64 } 
            };

            double[,] expectedScatter2 = 
            {
                {  1.84, -0.04 }, 
                { -0.04,  2.64 }
            };

            double[,] expectedBetween = 
            {
                { 29.16, 21.60 },
                { 21.60, 16.00 },
            };

            double[,] expectedWithin = 
            {
                {  2.64, -0.44 },
                { -0.44,  5.28 }
            };

            Assert.IsTrue(Matrix.IsEqual(lda.Classes[0].Scatter, expectedScatter1, 0.01));
            Assert.IsTrue(Matrix.IsEqual(lda.Classes[1].Scatter, expectedScatter2, 0.01));

            Assert.IsTrue(Matrix.IsEqual(lda.ScatterBetweenClass, expectedBetween, 0.01));
            Assert.IsTrue(Matrix.IsEqual(lda.ScatterWithinClass, expectedWithin, 0.01));
        }

        [TestMethod()]
        public void ClassifyTest1()
        {
            // Create some sample input data instances. This is the same
            // data used in the Gutierrez-Osuna's example available on:
            // http://research.cs.tamu.edu/prism/lectures/pr/pr_l10.pdf

            double[][] inputs = 
            {
                // Class 0
                new double[] {  4,  1 }, 
                new double[] {  2,  4 },
                new double[] {  2,  3 },
                new double[] {  3,  6 },
                new double[] {  4,  4 },

                // Class 1
                new double[] {  9, 10 },
                new double[] {  6,  8 },
                new double[] {  9,  5 },
                new double[] {  8,  7 },
                new double[] { 10,  8 }
            };

            int[] output = 
            {
                0, 0, 0, 0, 0, // The first five are from class 0
                1, 1, 1, 1, 1  // The last five are from class 1
            };

            // Then, we will create a LDA for the given instances.
            var lda = new LinearDiscriminantAnalysis(inputs, output);

            lda.Compute(); // Compute the analysis


            // Now we can project the data into KDA space:
            double[][] projection = lda.Transform(inputs);

            // Or perform classification using:
            int[] results = lda.Classify(inputs);


            // Test the classify method
            for (int i = 0; i < 5; i++)
            {
                int expected = 0;
                int actual = results[i];
                Assert.AreEqual(expected, actual);
            }

            for (int i = 5; i < 10; i++)
            {
                int expected = 1;
                int actual = results[i];
                Assert.AreEqual(expected, actual);
            }

        }


        [TestMethod()]
        public void ComputeTest2()
        {
            LinearDiscriminantAnalysis lda = new LinearDiscriminantAnalysis(inputs, output);

            // Compute the analysis
            lda.Compute();

            Assert.AreEqual(2, lda.Classes.Count);
            Assert.AreEqual(3.0, lda.Classes[0].Mean[0]);
            Assert.AreEqual(3.6, lda.Classes[0].Mean[1]);
            Assert.AreEqual(5, lda.Classes[0].Indices.Length);

            Assert.AreEqual(0, lda.Classes[0].Indices[0]);
            Assert.AreEqual(1, lda.Classes[0].Indices[1]);
            Assert.AreEqual(2, lda.Classes[0].Indices[2]);
            Assert.AreEqual(3, lda.Classes[0].Indices[3]);
            Assert.AreEqual(4, lda.Classes[0].Indices[4]);

            Assert.AreEqual(5, lda.Classes[1].Indices[0]);
            Assert.AreEqual(6, lda.Classes[1].Indices[1]);
            Assert.AreEqual(7, lda.Classes[1].Indices[2]);
            Assert.AreEqual(8, lda.Classes[1].Indices[3]);
            Assert.AreEqual(9, lda.Classes[1].Indices[4]);

            Assert.AreEqual(2, lda.Discriminants.Count);
            Assert.AreEqual(15.65685019206146, lda.Discriminants[0].Eigenvalue);
            Assert.AreEqual(-0.00000000000000, lda.Discriminants[1].Eigenvalue, 1e-15);

            Assert.AreEqual(5.7, lda.Means[0]);
            Assert.AreEqual(5.6, lda.Means[1]);
        }

        [TestMethod()]
        public void ProjectionTest()
        {
            LinearDiscriminantAnalysis lda = new LinearDiscriminantAnalysis(inputs, output);

            // Compute the analysis
            lda.Compute();

            // Project the input data into discriminant space
            double[,] projection = lda.Transform(inputs);

            Assert.AreEqual(projection[0, 0], 4.4273255813953485);
            Assert.AreEqual(projection[0, 1], 1.9629629629629628);
            Assert.AreEqual(projection[1, 0], 3.7093023255813953);
            Assert.AreEqual(projection[1, 1], -2.5185185185185186);
            Assert.AreEqual(projection[2, 0], 3.2819767441860463);
            Assert.AreEqual(projection[2, 1], -1.5185185185185186);
            Assert.AreEqual(projection[3, 0], 5.5639534883720927);
            Assert.AreEqual(projection[3, 1], -3.7777777777777777);
            Assert.AreEqual(projection[4, 0], 5.7093023255813957);
            Assert.AreEqual(projection[4, 1], -1.0370370370370372);
            Assert.AreEqual(projection[5, 0], 13.273255813953488);
            Assert.AreEqual(projection[5, 1], -3.3333333333333339);
            Assert.AreEqual(projection[6, 0], 9.4186046511627914);
            Assert.AreEqual(projection[6, 1], -3.5555555555555554);
            Assert.AreEqual(projection[7, 0], 11.136627906976745);
            Assert.AreEqual(projection[7, 1], 1.6666666666666661);
            Assert.AreEqual(projection[8, 0], 10.991279069767442);
            Assert.AreEqual(projection[8, 1], -1.0740740740740744);
            Assert.AreEqual(projection[9, 0], 13.418604651162791);
            Assert.AreEqual(projection[9, 1], -0.59259259259259345);

            // Assert the result equals the transformation of the input
            double[,] result = lda.Result;
            Assert.IsTrue(Matrix.IsEqual(result, projection));
        }

        [TestMethod()]
        public void ClassifyTest()
        {
            LinearDiscriminantAnalysis lda = new LinearDiscriminantAnalysis(inputs, output);

            // Compute the analysis
            lda.Compute();

            for (int i = 0; i < output.Length; i++)
                Assert.AreEqual(lda.Classify(inputs.GetRow(i)), output[i]);
        }

    }
}
