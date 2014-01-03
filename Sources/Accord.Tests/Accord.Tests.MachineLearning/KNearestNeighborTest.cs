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
    using System;
    using Accord.Math;    
    
    [TestClass()]
    public class KNearestNeighborTest
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
        public void KNearestNeighborConstructorTest()
        {
            double[][] inputs = 
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

            int[] outputs =
            {
                0, 0,
                1, 1, 1, 1,
                2, 2, 2
            };

            int k = 3;
           
            KNearestNeighbors target = new KNearestNeighbors(k, inputs, outputs);

            for (int i = 0; i < inputs.Length; i++)
            {
                int actual = target.Compute(inputs[i]);
                int expected = outputs[i];

                Assert.AreEqual(expected, actual);
            }

            double[][] test = 
            {
                new double[] { -4, -3, -1 },
                new double[] { -5, -4, -4 },

                new double[] {  5,  3,  4 },
                new double[] {  3,  1,  6 },

                new double[] { 10,  5,  4 },
                new double[] { 13,  4,  5 },
            };

            int[] expectedOutputs =
            {
                0, 0,
                1, 1,
                2, 2,
            };

            for (int i = 0; i < test.Length; i++)
            {
                int actual = target.Compute(test[i]);
                int expected = expectedOutputs[i];

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod()]
        public void KNearestNeighborConstructorTest2()
        {
            // Create some sample learning data. In this data,
            // the first two instances belong to a class, the
            // four next belong to another class and the last
            // three to yet another.

            double[][] inputs = 
            {
                // The first two are from class 0
                new double[] { -5, -2, -1 },
                new double[] { -5, -5, -6 },

                // The next four are from class 1
                new double[] {  2,  1,  1 },
                new double[] {  1,  1,  2 },
                new double[] {  1,  2,  2 },
                new double[] {  3,  1,  2 },

                // The last three are from class 2
                new double[] { 11,  5,  4 },
                new double[] { 15,  5,  6 },
                new double[] { 10,  5,  6 },
            };

            int[] outputs =
            {
                0, 0,        // First two from class 0
                1, 1, 1, 1,  // Next four from class 1
                2, 2, 2      // Last three from class 2
            };


            // Now we will create the K-Nearest Neighbors algorithm. For this
            // example, we will be choosing k = 4. This means that, for a given
            // instance, its nearest 4 neighbors will be used to cast a decision.
            KNearestNeighbors knn = new KNearestNeighbors(k: 4, classes: 3,
                inputs: inputs, outputs: outputs);


            // After the algorithm has been created, we can classify a new instance:
            
            int answer = knn.Compute(new double[] { 11, 5, 4 }); // answer will be 2.


            Assert.AreEqual(2, answer);
        }

        [TestMethod()]
        public void KNearestNeighborGenericConstructorTest()
        {
            double[][] inputs = 
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

            int[] outputs =
            {
                0, 0,
                1, 1, 1, 1,
                2, 2, 2
            };

            int k = 3;

            KNearestNeighbors<double[]> target = new KNearestNeighbors<double[]>(k, inputs, outputs, 
                Accord.Math.Distance.SquareEuclidean);

            for (int i = 0; i < inputs.Length; i++)
            {
                int actual = target.Compute(inputs[i]);
                int expected = outputs[i];

                Assert.AreEqual(expected, actual);
            }

            double[][] test = 
            {
                new double[] { -4, -3, -1 },
                new double[] { -5, -4, -4 },

                new double[] {  5,  3,  4 },
                new double[] {  3,  1,  6 },

                new double[] { 10,  5,  4 },
                new double[] { 13,  4,  5 },
            };

            int[] expectedOutputs =
            {
                0, 0,
                1, 1,
                2, 2,
            };

            for (int i = 0; i < test.Length; i++)
            {
                int actual = target.Compute(test[i]);
                int expected = expectedOutputs[i];

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod()]
        public void KNearestNeighborGenericConstructorTest2()
        {
            // Create some sample learning data. In this data,
            // the first two instances belong to a class, the
            // four next belong to another class and the last
            // three to yet another.

            double[][] inputs = 
            {
                // The first two are from class 0
                new double[] { -5, -2, -1 },
                new double[] { -5, -5, -6 },

                // The next four are from class 1
                new double[] {  2,  1,  1 },
                new double[] {  1,  1,  2 },
                new double[] {  1,  2,  2 },
                new double[] {  3,  1,  2 },

                // The last three are from class 2
                new double[] { 11,  5,  4 },
                new double[] { 15,  5,  6 },
                new double[] { 10,  5,  6 },
            };

            int[] outputs =
            {
                0, 0,        // First two from class 0
                1, 1, 1, 1,  // Next four from class 1
                2, 2, 2      // Last three from class 2
            };


            // Now we will create the K-Nearest Neighbors algorithm. For this
            // example, we will be choosing k = 4. This means that, for a given
            // instance, its nearest 4 neighbors will be used to cast a decision.
            KNearestNeighbors<double[]> knn = new KNearestNeighbors<double[]>(k: 4, classes: 3,
                inputs: inputs, outputs: outputs, distance: Accord.Math.Distance.SquareEuclidean);


            // After the algorithm has been created, we can classify a new instance:
            int answer = knn.Compute(new double[] { 11, 5, 4 }); // answer will be 2.


            Assert.AreEqual(2, answer);
        }

        [TestMethod()]
        public void KNearestNeighborConstructorTest3()
        {

            // The k-Nearest Neighbors algorithm can be used with
            // any kind of data. In this example, we will see how
            // it can be used to compare, for example, Strings.

            string[] inputs = 
            {
                "Car",     // class 0
                "Bar",     // class 0
                "Jar",     // class 0

                "Charm",   // class 1
                "Chair"    // class 1
            };

            int[] outputs =
            {
                0, 0, 0,  // First three are from class 0
                1, 1,     // And next two are from class 1
            };


            // Now we will create the K-Nearest Neighbors algorithm. For this
            // example, we will be choosing k = 1. This means that, for a given
            // instance, only its nearest neighbor will be used to cast a new
            // decision. 
            
            // In order to compare strings, we will be using Levenshtein's string distance
            KNearestNeighbors<string> knn = new KNearestNeighbors<string>(k: 1, classes: 2,
                inputs: inputs, outputs: outputs, distance: Distance.Levenshtein);


            // After the algorithm has been created, we can use it:
            int answer = knn.Compute("Chars"); // answer should be 1.

            Assert.AreEqual(1, answer);
        }

    }
}
