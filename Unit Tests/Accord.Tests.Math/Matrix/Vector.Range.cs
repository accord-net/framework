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

namespace Accord.Tests.Math
{
    using Accord.Math;
    using NUnit.Framework;
    using System.Linq;
    using System.Collections.Generic;
    using System;

    [TestFixture]
    public partial class VectorTest
    {

        [Test]
        public void gh898()
        {
            // https://github.com/accord-net/framework/issues/898
            double[] vec1 = Vector.Range(0d, 5d, 1d);                       // { 0, 1, 2, 3, 5 }
            double[] vec2 = Vector.EnumerableRange(0d, 5d).ToArray();       // { 0, 1, 2, 3, 4 }
            double[] vec3 = Vector.EnumerableRange(0d, 5d, 1d).ToArray();   // { 0, 1, 2, 3, 4, 5 }

            Assert.AreEqual(new double[] { 0, 1, 2, 3, 4 }, vec1);
            Assert.AreEqual(new double[] { 0, 1, 2, 3, 4 }, vec2);
            Assert.AreEqual(new double[] { 0, 1, 2, 3, 4 }, vec3);
        }

        [Test]
        public void range_test()
        {
            // Sanity checks
            Assert.AreEqual(new int[] { 0, 1, 2 }, Vector.Range(3));
            Assert.AreEqual(new int[] { 1, 2 }, Vector.Range(1, 3));
            Assert.AreEqual(new int[] { 3, 2 }, Vector.Range(3, 1));
            Assert.AreEqual(new int[] { 2, 1 }, Vector.Range(2, 0));
            Assert.AreEqual(new int[] { 3, 2 }, Vector.Range(3, 1, 1));
            Assert.AreEqual(new int[] { 2, 1 }, Vector.Range(2, 0, 1));

            // Deep tests
            for (int end = 0; end < 10; end++)
            {
                test(0, end, () => Vector.Range((int)end));
                test(0, end, () => Vector.Range((short)end));
                test(0, end, () => Vector.Range((byte)end));
                test(0, end, () => Vector.Range((decimal)end));
                test(0, end, () => Vector.Range((double)end));
                test(0, end, () => Vector.Range((float)end));
                test(0, end, () => Vector.Range((long)end));
                test(0, end, () => Vector.Range((ulong)end));
                test(0, end, () => Vector.Range((ushort)end));

                for (int start = 0; start < 10; start++)
                {
                    test(start, end, () => Vector.Range((int)start, (int)end, stepSize: (int)1));
                    test(start, end, () => Vector.Range((short)start, (short)end, (short)1));
                    test(start, end, () => Vector.Range((byte)start, (byte)end, stepSize: (byte)1));
                    test(start, end, () => Vector.Range((decimal)start, (decimal)end, (decimal)1));
                    test(start, end, () => Vector.Range((double)start, (double)end, (double)1));
                    test(start, end, () => Vector.Range((float)start, (float)end, (float)1));
                    test(start, end, () => Vector.Range((long)start, (long)end, (long)1));
                    test(start, end, () => Vector.Range((ulong)start, (ulong)end, (ulong)1));
                    test(start, end, () => Vector.Range((ushort)start, (ushort)end, (ushort)1));
                }
            }
        }

        [Test]
        public void enumerable_range_test()
        {
            // Sanity checks
            Assert.AreEqual(new int[] { 0, 1, 2 }, Vector.EnumerableRange(3));
            Assert.AreEqual(new int[] { 1, 2 }, Vector.EnumerableRange(1, 3));
            Assert.AreEqual(new int[] { 3, 2 }, Vector.EnumerableRange(3, 1));
            Assert.AreEqual(new int[] { 2, 1 }, Vector.EnumerableRange(2, 0));
            Assert.AreEqual(new int[] { 3, 2 }, Vector.EnumerableRange(3, 1, 1));
            Assert.AreEqual(new int[] { 2, 1 }, Vector.EnumerableRange(2, 0, 1));

            // Deep tests
            for (int end = 0; end < 10; end++)
            {
                test(0, end, () => Vector.EnumerableRange((int)end));
                test(0, end, () => Vector.EnumerableRange((short)end));
                test(0, end, () => Vector.EnumerableRange((byte)end));
                test(0, end, () => Vector.EnumerableRange((decimal)end));
                test(0, end, () => Vector.EnumerableRange((double)end));
                test(0, end, () => Vector.EnumerableRange((float)end));
                test(0, end, () => Vector.EnumerableRange((long)end));
                test(0, end, () => Vector.EnumerableRange((ulong)end));
                test(0, end, () => Vector.EnumerableRange((ushort)end));

                for (int start = 0; start < 10; start++)
                {
                    test(start, end, () => Vector.EnumerableRange((int)start, (int)end, stepSize: (int)1));
                    test(start, end, () => Vector.EnumerableRange((short)start, (short)end, (short)1));
                    test(start, end, () => Vector.EnumerableRange((byte)start, (byte)end, stepSize: (byte)1));
                    test(start, end, () => Vector.EnumerableRange((decimal)start, (decimal)end, (decimal)1));
                    test(start, end, () => Vector.EnumerableRange((double)start, (double)end, (double)1));
                    test(start, end, () => Vector.EnumerableRange((float)start, (float)end, (float)1));
                    test(start, end, () => Vector.EnumerableRange((long)start, (long)end, (long)1));
                    test(start, end, () => Vector.EnumerableRange((ulong)start, (ulong)end, (ulong)1));
                    test(start, end, () => Vector.EnumerableRange((ushort)start, (ushort)end, (ushort)1));
                }
            }
        }

        [Test]
        public void numpy_comparison_tests()
        {
            Assert.AreEqual(new int[] { 1, 2 }, Vector.Range(1, 3, +1));
            Assert.AreEqual(new int[] { 3, 2 }, Vector.Range(3, 1, -1));
            Assert.AreEqual(new int[] { 1 }, Vector.Range(1, 3, +5));
            Assert.AreEqual(new int[] { 3 }, Vector.Range(3, 1, -5));
            Assert.IsTrue(new double[] { 0, 0.3, 0.6, 0.9 }.IsEqual(Vector.Range(0, 1, 0.3), 1e-10));
            Assert.IsTrue(new double[] { 1, 0.7, 0.4, 0.1 }.IsEqual(Vector.Range(1, 0, 0.3), 1e-10));
            Assert.IsTrue(new double[] { 0, 0.2, 0.4, 0.6, 0.8 }.IsEqual(Vector.Range(0, 1, 0.2), 1e-10));
            Assert.IsTrue(new double[] { 1, 0.8, 0.6, 0.4, 0.2 }.IsEqual(Vector.Range(1, 0, -0.2), 1e-10));

            // The framework's version differs when the intervals are inverted
            // and the step is positive. In this case, the framework will still
            // iterate over the range backwards because the third parameter is 
            // considered a step <i>size</i>, instead of a step direction.
            Assert.AreEqual(new int[] { 3, 2 }, Vector.Range(3, 1, +1));

            // However, it is not allowed to specify a negative step size when
            // a < b is not allowed since it would result in an infinite loop:
            Assert.Throws<ArgumentOutOfRangeException>(() => Vector.Range(1, 3, -1));
        }

        [Test]
        public void range_step_test()
        {
            // Sanity checks
            Assert.AreEqual(new int[] { 1, 2 }, Vector.Range(1, 3, +1));
            Assert.AreEqual(new int[] { 3, 2 }, Vector.Range(3, 1, -1));
            Assert.AreEqual(new int[] { 3, 2 }, Vector.Range(3, 1, +1));
            Assert.Throws<ArgumentOutOfRangeException>(() => Vector.Range(1, 3, -1));
            Assert.AreEqual(new int[] { 1 }, Vector.Range(1, 3, +5));
            Assert.AreEqual(new int[] { 3 }, Vector.Range(3, 1, -5));
            Assert.IsTrue(new double[] { 0, 0.3, 0.6, 0.9 }.IsEqual(Vector.Range(0, 1, 0.3), 1e-10));
            Assert.IsTrue(new double[] { 1, 0.7, 0.4, 0.1 }.IsEqual(Vector.Range(1, 0, 0.3), 1e-10));
            Assert.IsTrue(new double[] { 0, 0.2, 0.4, 0.6, 0.8 }.IsEqual(Vector.Range(0, 1, 0.2), 1e-10));
            Assert.IsTrue(new double[] { 1, 0.8, 0.6, 0.4, 0.2 }.IsEqual(Vector.Range(1, 0, -0.2), 1e-10));

            // Deep tests
            for (double step = -2; step < 2; step += 0.1)
            {
                if (Math.Abs(step) < 1e-10)
                    step = 0;

                for (int end = 0; end < 5; end++)
                {
                    for (int start = 0; start < 5; start++)
                    {
                        test(start, end, step, () => Vector.Range((decimal)start, (decimal)end, (decimal)step));
                        test(start, end, step, () => Vector.Range((double)start, (double)end, (double)step));
                        test(start, end, step, () => Vector.Range((float)start, (float)end, (float)step));
                    }
                }
            }
        }

        [Test]
        public void enumerable_range_step_test()
        {
            // Sanity checks
            Assert.AreEqual(new int[] { 1, 2 }, Vector.EnumerableRange(1, 3, +1));
            Assert.AreEqual(new int[] { 3, 2 }, Vector.EnumerableRange(3, 1, -1));
            Assert.AreEqual(new int[] { 3, 2 }, Vector.EnumerableRange(3, 1, +1));
            Assert.Throws<ArgumentOutOfRangeException>(() => Vector.EnumerableRange(1, 3, -1).ToArray());
            Assert.AreEqual(new int[] { 1 }, Vector.EnumerableRange(1, 3, +5));
            Assert.AreEqual(new int[] { 3 }, Vector.EnumerableRange(3, 1, -5));
            Assert.IsTrue(new double[] { 0, 0.3, 0.6, 0.9 }.IsEqual(Vector.EnumerableRange(0, 1, 0.3).ToArray(), 1e-10));
            Assert.IsTrue(new double[] { 1, 0.7, 0.4, 0.1 }.IsEqual(Vector.EnumerableRange(1, 0, 0.3).ToArray(), 1e-10));
            Assert.IsTrue(new double[] { 0, 0.2, 0.4, 0.6, 0.8 }.IsEqual(Vector.EnumerableRange(0, 1, 0.2).ToArray(), 1e-10));
            Assert.IsTrue(new double[] { 1, 0.8, 0.6, 0.4, 0.2 }.IsEqual(Vector.EnumerableRange(1, 0, -0.2).ToArray(), 1e-10));

            // Deep tests
            for (double step = -2; step < 2; step += 0.1)
            {
                if (Math.Abs(step) < 1e-10)
                    step = 0;

                for (int end = 0; end < 5; end++)
                {
                    for (int start = 0; start < 5; start++)
                    {
                        test(start, end, step, () => Vector.EnumerableRange((decimal)start, (decimal)end, (decimal)step));
                        test(start, end, step, () => Vector.EnumerableRange((double)start, (double)end, (double)step));
                        test(start, end, step, () => Vector.EnumerableRange((float)start, (float)end, (float)step));
                    }
                }
            }
        }




        private static void test<T>(int start, int end, Func<IEnumerable<T>> func)
        {
            test(start, end, () => func().ToArray());
        }

        private static void test<T>(int start, int end, double step, Func<IEnumerable<T>> func)
        {
            test(start, end, step, () => func().ToArray());
        }

        private static void test<T>(int start, int end, Func<T[]> func)
        {
            T[] values = func();

            if (start == end)
            {
                Assert.AreEqual(0, values.Length);
            }
            else if (start < end)
            {
                Assert.AreEqual(start, values.Get(0));
                Assert.AreEqual(end - 1, values.Get(-1));
            }
            else
            {
                Assert.AreEqual(start, values.Get(0));
                Assert.AreEqual(end + 1, values.Get(-1));
            }
        }

        private static void test<T>(int start, int end, double step, Func<T[]> func)
        {
            double tol = 1e-10;
            if (typeof(T) == typeof(float))
                tol = 1e-6;

            if (start == end)
            {
                T[] values = func();
                Assert.AreEqual(0, values.Length);
            }
            else
            {
                if (step == 0)
                {
                    Assert.Throws<ArgumentOutOfRangeException>(() => func());
                }
                else
                {
                    if (start < end)
                    {
                        if (step > 0)
                        {
                            T[] values = func();
                            Assert.AreEqual(start, values.Get(0));
                            Assert.AreEqual(start + (values.Length - 1) * step, values.Get(-1).To<double>(), tol);
                        }
                        else
                        {
                            Assert.Throws<ArgumentOutOfRangeException>(() => func());
                        }
                    }
                    else
                    {
                        T[] values = func();
                        Assert.AreEqual(start, values.Get(0));
                        if ((start - end) <= step)
                            Assert.AreEqual(start - (values.Length - 1) * step, values.Get(-1).To<double>(), tol);
                    }
                }
            }
        }

    }
}
