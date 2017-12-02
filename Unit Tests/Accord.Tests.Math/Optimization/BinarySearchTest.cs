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

namespace Accord.Tests.Math
{
    using System.Linq;
    using Accord.Math.Optimization;
    using NUnit.Framework;
    using System;
    using Accord.Math;
    using Accord;

    [TestFixture]
    public class BinarySearchTest2
    {
        [Datapoint]
        public int[] a = { -3, -2, -1, 0, 1, 2, 3 };
        [Datapoint]
        public int[] b = { -3, -2, -1, 0, 1, 2, 3, 4, 5 };
        [Datapoint]
        public int[] c = { -5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5 };
        [Datapoint]
        public int[] d = { 3, 2, 1, 0, -1, -2, -3 };
        [Datapoint]
        public int[] e = { 3, 2, 1, 0, -1, -2, -3, -4, -5 };
        [Datapoint]
        public int[] f = { 5, 4, 3, 2, 1, 0, -1, -2, -3 };
        [Theory]
        public void AccordBinarySearchShouldWorkWithArray(int[] data)
        {
            var zeroIndex = data.ToList().FindIndex(v => v == 0);
            var index = new BinarySearch(i => data[i], 0, data.Length - 1).FindRoot();
            Assert.AreEqual(zeroIndex, index, String.Format("For {0}", String.Join(",", data)));
        }
    }


    [TestFixture]
    public class BinarySearchTest
    {
        double[] elements;
        int[] idx;

        public BinarySearchTest()
        {
            elements = new double[] { 5.2, 2.7, 8, 6.1, 21, 9, -1, 2, 0 };
            idx = Matrix.Indices(0, elements.Length);

            Array.Sort(elements, idx);
        }

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


        [Test]
        public void ConstructorTest()
        {
            // https://www.wolframalpha.com/input/?i=%28x%2B1%29+*+%28x%2B1%29+*+%28x%2B1%29+%2B+2+*+%28x%2B1%29+*+%28x%2B1%29+%3D+0%2C+x+is+integer

            Func<int, double> function = x => (x + 1) * (x + 1) * (x + 1) + 2 * (x + 1) * (x + 1);

            // Possible roots are -3 or -1

            BinarySearch search;
            search = new BinarySearch(function, -2, 3);
            double r1 = search.FindRoot();
            Assert.AreEqual(-1, r1);

            search = new BinarySearch(function, -10, -2);
            double r2 = search.FindRoot();
            Assert.AreEqual(-3, r2);
        }

        [Test]
        public void ConstructorTest4()
        {
            // (x+5)^3 + 2(x+5)^2 - 10(x+5)
            Func<int, double> function = x =>
            {
                int y = (x + 5);
                return y * y * y + 2 * y * y - 10 * y;
            };

            BinarySearch search = new BinarySearch(function, -6, -4);
            double root = search.FindRoot();
            Assert.AreEqual(-5, root);
        }

        [Test]
        public void ConstructorTest1()
        {
            Func<int, double> function = x => elements[x];
            BinarySearch search = new BinarySearch(function, 0, elements.Length);

            int a1 = search.FindRoot();
            Assert.AreEqual(1, a1);

            for (int i = 0; i < elements.Length; i++)
            {
                int a2 = search.Find(elements[i]);
                Assert.AreEqual(i, a2);
            }
        }

        [Test]
        public void ConstructorTest2()
        {
            Func<int, double> function = x => elements[x];
            BinarySearch search = new BinarySearch(function, 0, elements.Length - 1);

            int a1 = search.Find(5);
            int a2 = search.Find(6);

            int a3 = search.Find(elements.Max() + 1);
            int a4 = search.Find(elements.Max() - 1);
            int a5 = search.Find(elements.Max());

            int a6 = search.Find(elements.Min() + 1);
            int a7 = search.Find(elements.Min() - 1);
            int a8 = search.Find(elements.Min());

            Assert.AreEqual(a1, 4);
            Assert.AreEqual(a2, 5);

            Assert.AreEqual(a3, 8);
            Assert.AreEqual(a4, 8);
            Assert.AreEqual(a5, 8);

            Assert.AreEqual(a6, 1);
            Assert.AreEqual(a7, 0);
            Assert.AreEqual(a8, 0);
        }

    }
}
