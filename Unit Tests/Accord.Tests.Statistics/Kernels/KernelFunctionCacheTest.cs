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
    using Accord.Statistics.Kernels;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using Accord.Math;
    using System.Linq;

    [TestFixture]
    public class KernelFunctionCacheTest
    {

        double[][] inputs =
        {
            new double[] { 0 },
            new double[] { 1 },
            new double[] { 2 },
            new double[] { 3 },
            new double[] { 4 },
            new double[] { 5 },
            new double[] { 6 },
            new double[] { 7 },
            new double[] { 8 },
            new double[] { 9 },
        };


        [Test]
        public void KernelFunctionCacheConstructorTest()
        {
            IKernel kernel = new Linear(1);

            int cacheSize = 0;

            KernelFunctionCache target = new KernelFunctionCache(kernel, inputs, cacheSize);

            Assert.AreEqual(0, target.Size);
            Assert.AreEqual(0, target.Hits);
            Assert.AreEqual(0, target.Misses);
            Assert.IsFalse(target.Enabled);

            for (int i = 0; i < inputs.Length; i++)
            {
                double expected = i * i + 1;
                double actual = target.GetOrCompute(i);

                Assert.AreEqual(expected, actual);
            }

            Assert.AreEqual(0, target.Hits);

            for (int i = 0; i < inputs.Length; i++)
            {
                for (int j = 0; j < inputs.Length; j++)
                {
                    double expected = i * j + 1;
                    double actual = target.GetOrCompute(i, j);

                    Assert.AreEqual(expected, actual);
                }
            }

            Assert.AreEqual(0, target.Hits);
            Assert.AreEqual(0, target.Usage);
        }


        [Test]
        public void KernelFunctionCacheConstructorTest2()
        {
            IKernel kernel = new Linear(1);

            int cacheSize = inputs.Length - 1;

            KernelFunctionCache target = new KernelFunctionCache(kernel, inputs, cacheSize);

            Assert.AreEqual(9, target.Size);
            Assert.AreEqual(0, target.Hits);
            Assert.AreEqual(0, target.Misses);
            Assert.IsTrue(target.Enabled);

            for (int i = 0; i < inputs.Length; i++)
            {
                double expected = i * i + 1;
                double actual = target.GetOrCompute(i);

                Assert.AreEqual(expected, actual);
            }

            Assert.AreEqual(0, target.Hits);

            int[] hits = { 0, 9, 18, 27, 36, 45, 54, 63, 72, 81 };
            int[] miss = { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 };

            for (int i = 0; i < inputs.Length; i++)
            {
                for (int j = 0; j < inputs.Length; j++)
                {
                    double expected = i * j + 1;
                    double actual = target.GetOrCompute(i, j);

                    Assert.AreEqual(expected, actual);
                }

                Assert.AreEqual(hits[i], target.Hits);
                Assert.AreEqual(miss[i], target.Misses);
            }

            for (int i = 0; i < inputs.Length; i++)
            {
                for (int j = 0; j < inputs.Length; j++)
                {
                    double expected = i * j + 1;
                    double actual = target.GetOrCompute(i, j);

                    Assert.AreEqual(expected, actual);
                }
            }

            Assert.AreEqual(9, target.Misses);
            Assert.AreEqual(171, target.Hits);
            Assert.AreEqual(1.0, target.Usage);
        }

        [Test]
        public void KernelFunctionCacheConstructorTest6()
        {
            IKernel kernel = new Gaussian(0.6);

            int cacheSize = inputs.Length;

            KernelFunctionCache target = new KernelFunctionCache(kernel, inputs, cacheSize);

            Assert.AreEqual(10, target.Size);
            Assert.AreEqual(0, target.Hits);
            Assert.AreEqual(0, target.Misses);
            Assert.IsTrue(target.Enabled);

            for (int i = 0; i < inputs.Length; i++)
            {
                double expected = kernel.Function(inputs[i], inputs[i]);
                double actual = target.GetOrCompute(i);

                Assert.AreEqual(expected, actual);
            }

            Assert.AreEqual(0, target.Hits);


            for (int i = 0; i < inputs.Length; i++)
            {
                for (int j = 0; j < inputs.Length; j++)
                {
                    double expected = kernel.Function(inputs[i], inputs[j]);
                    double actual = target.GetOrCompute(i, j);

                    Assert.AreEqual(expected, actual);
                }
            }

            for (int i = 0; i < inputs.Length; i++)
            {
                for (int j = 0; j < inputs.Length; j++)
                {
                    double expected = kernel.Function(inputs[i], inputs[j]);
                    double actual = target.GetOrCompute(i, j);

                    Assert.AreEqual(expected, actual);
                }
            }

            Assert.AreEqual(0, target.Misses);
            Assert.AreEqual(0, target.Hits);
            Assert.AreEqual(0, target.Usage);
        }

        [Test]
        public void KernelFunctionCacheConstructorTest5()
        {
            IKernel kernel = new Linear(1);

            int cacheSize = inputs.Length - 1;

            KernelFunctionCache target = new KernelFunctionCache(kernel, inputs, cacheSize);

            Assert.AreEqual(9, target.Size);
            Assert.AreEqual(0, target.Hits);
            Assert.AreEqual(0, target.Misses);
            Assert.IsTrue(target.Enabled);

            // upper half
            for (int i = 0; i < inputs.Length; i++)
            {
                for (int j = i + 1; j < inputs.Length; j++)
                {
                    double expected = i * j + 1;
                    double actual = target.GetOrCompute(i, j);

                    Assert.AreEqual(expected, actual);
                }
            }


            Assert.AreEqual(9, target.Misses);
            Assert.AreEqual(36, target.Hits);
            Assert.AreEqual(1.0, target.Usage);

            // lower half
            for (int i = 0; i < inputs.Length; i++)
            {
                for (int j = i + 1; j < inputs.Length; j++)
                {
                    double expected = i * j + 1;
                    double actual = target.GetOrCompute(j, i);

                    Assert.AreEqual(expected, actual);
                }
            }


            Assert.AreEqual(9, target.Misses);
            Assert.AreEqual(81, target.Hits);
            Assert.AreEqual(1.0, target.Usage);
        }

        [Test]
        public void KernelFunctionCacheConstructorTest7()
        {
            double[][] inputs =
            {
                new double[] { 0, 1 },
                new double[] { 1, 0 },
                new double[] { 1, 1 },
                new double[] { },
            };

            IKernel kernel = new Polynomial(2);

            int cacheSize = inputs.Length - 1;

            KernelFunctionCache target = new KernelFunctionCache(kernel, inputs, cacheSize);

            Assert.AreEqual(3, target.Size);
            Assert.AreEqual(0, target.Hits);
            Assert.AreEqual(0, target.Misses);
            Assert.IsTrue(target.Enabled);

            // upper half
            for (int i = 0; i < inputs.Length - 1; i++)
            {
                for (int j = i + 1; j < inputs.Length - 1; j++)
                {
                    double expected = kernel.Function(inputs[i], inputs[j]);
                    double actual = target.GetOrCompute(i, j);

                    Assert.AreEqual(expected, actual);
                }
            }

            var lruList1 = target.GetLeastRecentlyUsedList();

            Assert.AreEqual(2, target.Misses);
            Assert.AreEqual(1, target.Hits);
            Assert.AreEqual(0.66666, target.Usage, 1e-4);

            // upper half, backwards
            for (int i = inputs.Length - 2; i >= 0; i--)
            {
                for (int j = inputs.Length - 2; j >= i; j--)
                {
                    double expected = kernel.Function(inputs[i], inputs[j]);
                    double actual = target.GetOrCompute(j, i);

                    Assert.AreEqual(expected, actual);
                }
            }

            var lruList2 = target.GetLeastRecentlyUsedList();

            Assert.IsTrue(lruList2.SequenceEqual(new[] { lruList1[1], lruList1[2], lruList1[0] }));

            Assert.AreEqual(2, target.Misses);
            Assert.AreEqual(4, target.Hits);
            Assert.AreEqual(0.666666, target.Usage, 1e-5);
        }

        [Test]
        public void KernelFunctionCacheConstructorTest8()
        {
            double[][] inputs =
            {
                new double[] { 0, 1 },
                new double[] { 1, 0 },
                new double[] { 1, 1 },
            };

            IKernel kernel = new Polynomial(2);

            int cacheSize = inputs.Length;

            KernelFunctionCache target = new KernelFunctionCache(kernel, inputs, cacheSize);

            Assert.AreEqual(3, target.Size);
            Assert.AreEqual(0, target.Hits);
            Assert.AreEqual(0, target.Misses);
            Assert.IsTrue(target.Enabled);

            // upper half
            for (int i = 0; i < inputs.Length - 1; i++)
            {
                for (int j = i + 1; j < inputs.Length - 1; j++)
                {
                    double expected = kernel.Function(inputs[i], inputs[j]);
                    double actual = target.GetOrCompute(i, j);

                    Assert.AreEqual(expected, actual);
                }
            }

            Assert.Throws<InvalidOperationException>(() => target.GetLeastRecentlyUsedList(), "The cache is not using a LRU list.");
        }

        [Test]
        public void KernelFunctionCacheConstructorTest3()
        {
            IKernel kernel = new Linear(1);

            int cacheSize = 5;

            KernelFunctionCache target = new KernelFunctionCache(kernel, inputs, cacheSize);

            Assert.AreEqual(5, target.Size);
            Assert.AreEqual(0, target.Hits);
            Assert.AreEqual(0, target.Misses);

            for (int i = 0; i < inputs.Length; i++)
            {
                double expected = i * i + 1;
                double actual = target.GetOrCompute(i);

                Assert.AreEqual(expected, actual);
            }

            Assert.AreEqual(0, target.Hits);


            for (int i = 0; i < inputs.Length; i++)
            {
                for (int j = 0; j < inputs.Length; j++)
                {
                    double expected = i * j + 1;
                    double actual = target.GetOrCompute(i, j);

                    Assert.AreEqual(expected, actual);
                }
            }

            Assert.AreEqual(51, target.Hits);
            Assert.AreEqual(39, target.Misses);

            var snapshot = target.GetDataCache();

            foreach (var entry in snapshot)
            {
                double a = target.GetOrCompute(entry.Key.Item1, entry.Key.Item2);
                double b = target.GetOrCompute(entry.Key.Item2, entry.Key.Item1);

                Assert.AreEqual(a, b);
            }


            Assert.AreEqual(39, target.Misses);
            Assert.AreEqual(121, target.Hits);
            Assert.AreEqual(1.0, target.Usage);
        }


        [Test]
        public void KernelFunctionCacheConstructorTest4()
        {
            IKernel kernel = new Linear();

            int cacheSize = 100;

            KernelFunctionCache target = new KernelFunctionCache(kernel, inputs, cacheSize);

            Assert.AreEqual(inputs.Length, target.Size);
        }


    }
}
