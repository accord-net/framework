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

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Analysis;
    using NUnit.Framework;
    using Accord.Math;
    using Accord.MachineLearning;

    [TestFixture]
    public class MinimumMeanDistanceClassifierTest
    {

        [Test]
        public void ComputeTest()
        {
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
            var mmdc = new MinimumMeanDistanceClassifier(inputs, output);

            double[][] projection = mmdc.Scores(inputs);

            // Or perform classification using:
            int[] results = mmdc.Decide(inputs);


            Assert.IsTrue(results.IsEqual(output));
            
            //string str = projection.ToCSharp();

            double[][] expected = new double[][] {
                new double[] { -7.76, -62.92 },
                new double[] { -1.16, -53.92 },
                new double[] { -1.36, -62.12 },
                new double[] { -5.76, -31.72 },
                new double[] { -1.16, -32.32 },
                new double[] { -76.96, -6.12 },
                new double[] { -28.36, -5.92 },
                new double[] { -37.96, -7.12 },
                new double[] { -36.56, -0.52 },
                new double[] { -68.36, -2.72 } 
            };

            Assert.IsTrue(expected.IsEqual(projection, 1e-6));
        }

        [Test]
        public void new_method()
        {
            #region doc_learn
            // Create some sample input data instances. 

            double[][] inputs = 
            {
                // Class 0
                new double[] {  4,  1 }, 
                new double[] {  2,  4 },
                new double[] {  2,  3 },

                // Class 1
                new double[] {  5,  5 },
                new double[] {  5,  6 },

                // Class 2
                new double[] { 10,  8 }
            };

            int[] output = 
            {
                0, 0, 0, // The first three are from class 0
                1, 1,    // The second two are from class 1
                2        // The last is from class 2
            };

            // We will create a MMDC object for the data
            var mmdc = new MinimumMeanDistanceClassifier();

            // Compute the analysis and create a classifier
            mmdc.Learn(inputs, output);

            // Now we can project the data into mean distance space:
            double[][] projection = mmdc.Scores(inputs);

            // Or perform classification using:
            int[] results = mmdc.Decide(inputs);
            #endregion

            Assert.IsTrue(results.IsEqual(output));

            string str = projection.ToCSharp();

            double[][] expected = new double[][] 
            {
                new double[] { -4.55555555555556, -21.25, -85 },
                new double[] { -2.22222222222222, -11.25, -80 },
                new double[] { -0.555555555555555, -15.25, -89 },
                new double[] { -10.8888888888889, -0.25, -34 },
                new double[] { -16.5555555555556, -0.25, -29 },
                new double[] { -82.2222222222222, -31.25, 0 } 
            };

            Assert.IsTrue(expected.IsEqual(projection, 1e-6));
        }

    }
}
