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

namespace Accord.Tests.Math
{
    using Accord.Math;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System;
    using System.Data;
    using AForge;

    public partial class MatrixTest
    {

        
        [TestMethod()]
        public void MatrixSubmatrix()
        {
            double[,] value = new double[,]
            { 
              { 1.000, 1.000, 1.000 },
              { 2.000, 2.000, 2.000 },
              { 3.000, 3.000, 3.000 }
            };

            double[,] expected = new double[,]
            { 
              { 1.000, 1.000, 1.000 },
              { 3.000, 3.000, 3.000 }
            };

            double[,] actual = Matrix.Submatrix(value, new int[] { 0, 2 });

            Assert.IsTrue(Matrix.IsEqual(actual, expected));
        }

        [TestMethod()]
        public void MatrixSubmatrix2()
        {
            double[,] value = new double[,]
            { 
              { 1.000, 1.000, 1.000 },
              { 2.000, 2.000, 2.000 },
              { 3.000, 3.000, 3.000 }
            };

            double[,] expected = new double[,]
            { 
              { 1.000, 1.000, 1.000 },
              { 3.000, 3.000, 3.000 }
            };

            double[,] actual;

            actual = Matrix.Submatrix(value, new int[] { 0, 2 });
            Assert.IsTrue(Matrix.IsEqual(actual, expected));

            actual = Matrix.Submatrix(value, new int[] { 0, 2 }, null);
            Assert.IsTrue(Matrix.IsEqual(actual, expected));

            actual = Matrix.Submatrix(value, null, null);
            Assert.IsTrue(Matrix.IsEqual(actual, value));
        }

        [TestMethod()]
        public void SubmatrixTest()
        {
            double[][] data = 
            {
                new double[] { 1, 2, 3 },
                new double[] { 4, 5, 6 },
                new double[] { 7, 8, 9 },
            };

            int[] rowIndexes = { 1, 2 };
            int j0 = 0;
            int j1 = 1;

            double[][] expected = 
            {
                //new double[] { 1, 2, 3 },
                new double[] { 4, 5/*, 6*/ },
                new double[] { 7, 8/*, 9*/ },
            };

            double[][] actual = Matrix.Submatrix(data, rowIndexes, j0, j1);

            Assert.IsTrue(Matrix.IsEqual(expected, actual));



            double[][] expected2 = 
            {
                new double[] { 1, 2/*, 3*/ },
                new double[] { 4, 5/*, 6*/ },
                new double[] { 7, 8/*, 9*/ },
            };

            double[][] actual2 = Matrix.Submatrix(data, null, j0, j1);

            Assert.IsTrue(Matrix.IsEqual(expected2, actual2));
        }

        [TestMethod()]
        public void SubmatrixTest1()
        {
            double[,] value = new double[,]
            { 
                { 1.000, 1.000, 1.000 },
                { 2.000, 2.000, 2.000 },
                { 3.000, 3.000, 3.000 }
            };

            double[,] expected = new double[,]
            { 
                {        1.000, 1.000 },
                {        3.000, 3.000 }
            };

            int[] rowIndexes = { 0, 2 };
            int j0 = 1;
            int j1 = 2;

            double[,] actual;

            actual = Matrix.Submatrix(value, rowIndexes, j0, j1);
            Assert.IsTrue(Matrix.IsEqual(actual, expected));


            double[,] expected2 = new double[,]
            { 
                {        1.000, 1.000 },
                {        2.000, 2.000 },
                {        3.000, 3.000 }
            };

            actual = Matrix.Submatrix(value, null, j0, j1);
            Assert.IsTrue(Matrix.IsEqual(actual, expected2));

            actual = Matrix.Submatrix(value, null, null);
            Assert.IsTrue(Matrix.IsEqual(actual, value));
        }


        [TestMethod()]
        public void SubgroupTest1()
        {
            Accord.Math.Tools.SetupGenerator(0);

            double[] value = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };

            int[] idx = Accord.Statistics.Tools.RandomGroups(value.Length, 4);

            double[][] groups = value.Subgroups(idx);

            Assert.AreEqual(4, groups.Length);
            Assert.AreEqual(3, groups[0].Length);
            Assert.AreEqual(4, groups[1].Length);
            Assert.AreEqual(3, groups[2].Length);
            Assert.AreEqual(3, groups[3].Length);

            for (int i = 0; i < groups.Length; i++)
            {
                for (int j = 0; j < groups[i].Length; j++)
                {
                    double e = groups[i][j];

                    for (int k = 0; k < groups.Length; k++)
                    {
                        for (int l = 0; l < groups[k].Length; l++)
                        {
                            double a = groups[k][l];

                            if (k != i && l != j)
                                Assert.AreNotEqual(a, l);
                        }
                    }
                }
            }

        }

        [TestMethod()]
        public void SubgroupTest2()
        {
            double[] value = { 1, 2, 3, 4, 5, 6, 7 };
            int[] idx = { 0, 0, 0, 5, 5, 5, 5 };


            double[][] groups = value.Subgroups(idx);

            Assert.AreEqual(2, groups.Length);
            Assert.AreEqual(3, groups[0].Length);
            Assert.AreEqual(4, groups[1].Length);

            Assert.AreEqual(groups[0][0], 1);
            Assert.AreEqual(groups[0][1], 2);
            Assert.AreEqual(groups[0][2], 3);

            Assert.AreEqual(groups[1][0], 4);
            Assert.AreEqual(groups[1][1], 5);
            Assert.AreEqual(groups[1][2], 6);
            Assert.AreEqual(groups[1][3], 7);
        }

        [TestMethod()]
        public void SubgroupTest3()
        {
            double[] value = { 1, 2, 3, 4, 5, 6, 7 };
            int[] idx = { 0, 0, 0, 4, 4, 4, 4 };


            double[][] groups = value.Subgroups(idx, 5);

            Assert.AreEqual(5, groups.Length);

            Assert.AreEqual(3, groups[0].Length);
            Assert.AreEqual(0, groups[1].Length);
            Assert.AreEqual(0, groups[2].Length);
            Assert.AreEqual(0, groups[3].Length);
            Assert.AreEqual(4, groups[4].Length);

            Assert.AreEqual(groups[0][0], 1);
            Assert.AreEqual(groups[0][1], 2);
            Assert.AreEqual(groups[0][2], 3);

            Assert.AreEqual(groups[4][0], 4);
            Assert.AreEqual(groups[4][1], 5);
            Assert.AreEqual(groups[4][2], 6);
            Assert.AreEqual(groups[4][3], 7);
        }
    }
}
