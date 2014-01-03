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
    using System.Collections.Generic;
    using Accord.Math;
    using AForge;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class ToolsTest
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
        public void ScaleTest1()
        {
            double fromMin = 0;
            double fromMax = 1;
            double toMin = 0;
            double toMax = 100;
            double x = 0.2;

            double actual = Tools.Scale(fromMin, fromMax, toMin, toMax, x);
            Assert.AreEqual(20.0, actual);

            float actualF = Tools.Scale((float)fromMin, (float)fromMax, (float)toMin, (float)toMax, (float)x);
            Assert.AreEqual(20f, actualF);
        }


        [TestMethod()]
        public void ScaleTest()
        {
            IntRange from = new IntRange(0, 100);
            IntRange to = new IntRange(0, 50);
            int x = 50;
            int expected = 25;
            int actual = Tools.Scale(from, to, x);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod()]
        public void ScaleTest2()
        {
            DoubleRange from = new DoubleRange(-100, 100);
            DoubleRange to = new DoubleRange(0, 50);
            double x = 0;
            double expected = 25;
            double actual = Tools.Scale(from, to, x);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod()]
        public void ScaleTest3()
        {
            double toMin = 0;
            double toMax = 100;

            double[][] x = 
            { 
                new double[] { -1.0,  1.0 },
                new double[] { -0.2,  0.0 },
                new double[] { -0.6,  0.0 },
                new double[] {  0.0, -1.0 },
            };

            double[][] expected = 
            { 
                new double[] {    0, 100 },
                new double[] {   80,  50 },
                new double[] {   40,  50 },
                new double[] {  100,   0 },
            };

            var actual = Tools.Scale(toMin, toMax, x);

            Assert.IsTrue(Matrix.IsEqual(expected, actual));

        }


        [TestMethod()]
        public void ScaleTest4()
        {
            float fromMin = 0f;
            float fromMax = 1f;
            float toMin = 0f;
            float toMax = 100f;
            float[] x = { 0.2f, 0.34f };

            float[] actual = Tools.Scale(fromMin, fromMax, toMin, toMax, x);
            Assert.AreEqual(20.0, actual[0]);
            Assert.AreEqual(34.0, actual[1]);

            float[] actualF = Tools.Scale(fromMin, fromMax, toMin, toMax, x);
            Assert.AreEqual(20f, actualF[0]);
            Assert.AreEqual(34f, actualF[1]);
        }


        [TestMethod()]
        public void AtanhTest()
        {
            double d = 0.42;
            double expected = 0.447692023527421;
            double actual = Tools.Atanh(d);
            Assert.AreEqual(expected, actual, 1e-10);
        }


        [TestMethod()]
        public void AsinhTest()
        {
            double d = 0.42;
            double expected = 0.408540207829808;
            double actual = Tools.Asinh(d);
            Assert.AreEqual(expected, actual, 1e-10);
        }


        [TestMethod()]
        public void AcoshTest()
        {
            double x = 3.14;
            double expected = 1.810991348900196;
            double actual = Tools.Acosh(x);
            Assert.AreEqual(expected, actual, 1e-10);
        }


        [TestMethod()]
        public void InvSqrtTest()
        {
            float f = 42f;

            float expected = 1f / (float)System.Math.Sqrt(f);
            float actual = Tools.InvSqrt(f);

            Assert.AreEqual(expected, actual, 0.001);
        }

        [TestMethod()]
        public void DirectionTest()
        {
            IntPoint center = new IntPoint(0, 0);

            IntPoint w = new IntPoint(1, 0);
            IntPoint nw = new IntPoint(1, 1);
            IntPoint n = new IntPoint(0, 1);
            IntPoint ne = new IntPoint(-1, 1);
            IntPoint e = new IntPoint(-1, 0);
            IntPoint se = new IntPoint(-1, -1);
            IntPoint s = new IntPoint(0, -1);
            IntPoint sw = new IntPoint(1, -1);


            int actual;
            int expected;

            actual = Accord.Math.Tools.Direction(center, w);
            expected = (int)System.Math.Floor(0 / 18.0);
            Assert.AreEqual(expected, actual);

            actual = Accord.Math.Tools.Direction(center, nw);
            expected = (int)System.Math.Floor(45 / 18.0);
            Assert.AreEqual(expected, actual);

            actual = Accord.Math.Tools.Direction(center, n);
            expected = (int)System.Math.Floor(90 / 18.0);
            Assert.AreEqual(expected, actual);

            actual = Accord.Math.Tools.Direction(center, ne);
            expected = (int)System.Math.Floor(135 / 18.0);
            Assert.AreEqual(expected, actual);

            actual = Accord.Math.Tools.Direction(center, e);
            expected = (int)System.Math.Floor(180 / 18.0);
            Assert.AreEqual(expected, actual);

            actual = Accord.Math.Tools.Direction(center, se);
            expected = (int)System.Math.Floor(225 / 18.0);
            Assert.AreEqual(expected, actual);

            actual = Accord.Math.Tools.Direction(center, s);
            expected = (int)System.Math.Floor(270 / 18.0);
            Assert.AreEqual(expected, actual);

            actual = Accord.Math.Tools.Direction(center, sw);
            expected = (int)System.Math.Floor(315 / 18.0);
            Assert.AreEqual(expected, actual);
        }



        [TestMethod()]
        public void CombinationsTest()
        {
            {
                int symbols = 2;
                int length = 3;

                int[][] expected =
                {
                    new int[] { 0, 0, 0 },
                    new int[] { 0, 0, 1 },
                    new int[] { 0, 1, 0 },
                    new int[] { 0, 1, 1 },
                    new int[] { 1, 0, 0 },
                    new int[] { 1, 0, 1 },
                    new int[] { 1, 1, 0 },
                    new int[] { 1, 1, 1 },
                };

                int[][] actual = Combinatorics.TruthTable(symbols, length);

                Assert.IsTrue(expected.IsEqual(actual));
            }

            {
                int symbols = 3;
                int length = 3;

                int[][] expected =
                {
                    new int[] { 0, 0, 0 },
                    new int[] { 0, 0, 1 },
                    new int[] { 0, 0, 2 },
                    new int[] { 0, 1, 0 },
                    new int[] { 0, 1, 1 },
                    new int[] { 0, 1, 2 },
                    new int[] { 0, 2, 0 },
                    new int[] { 0, 2, 1 },
                    new int[] { 0, 2, 2 },
                    new int[] { 1, 0, 0 },
                    new int[] { 1, 0, 1 },
                    new int[] { 1, 0, 2 },
                    new int[] { 1, 1, 0 },
                    new int[] { 1, 1, 1 },
                    new int[] { 1, 1, 2 },
                    new int[] { 1, 2, 0 },
                    new int[] { 1, 2, 1 },
                    new int[] { 1, 2, 2 },
                    new int[] { 2, 0, 0 },
                    new int[] { 2, 0, 1 },
                    new int[] { 2, 0, 2 },
                    new int[] { 2, 1, 0 },
                    new int[] { 2, 1, 1 },
                    new int[] { 2, 1, 2 },
                    new int[] { 2, 2, 0 },
                    new int[] { 2, 2, 1 },
                    new int[] { 2, 2, 2 },
                };

                int[][] actual = Combinatorics.TruthTable(symbols, length);

                Assert.IsTrue(expected.IsEqual(actual));
            }
        }

        [TestMethod()]
        public void PermutationsTest()
        {
            int[] values = { 1, 2, 3 };

            List<int[]> permutations = new List<int[]>();
            foreach (var p in Combinatorics.Permutations(values))
                permutations.Add(p);

            Assert.AreEqual(5, permutations.Count);
            Assert.IsTrue(permutations[0].IsEqual(new[] { 1, 3, 2 }));
            Assert.IsTrue(permutations[1].IsEqual(new[] { 2, 1, 3 }));
            Assert.IsTrue(permutations[2].IsEqual(new[] { 2, 3, 1 }));
            Assert.IsTrue(permutations[3].IsEqual(new[] { 3, 1, 2 }));
            Assert.IsTrue(permutations[4].IsEqual(new[] { 3, 2, 1 }));
        }

    }
}
