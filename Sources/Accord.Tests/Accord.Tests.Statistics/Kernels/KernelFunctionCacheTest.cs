

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Kernels;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using Accord.Math;
    using System.Linq;

    [TestClass()]
    public class KernelFunctionCacheTest
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


        [TestMethod()]
        public void KernelFunctionCacheConstructorTest()
        {
            IKernel kernel = new Linear();

            int cacheSize = 0;

            KernelFunctionCache target = new KernelFunctionCache(kernel, inputs, cacheSize);

            Assert.AreEqual(0, target.Size);
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

            Assert.AreEqual(0, target.Hits);
            Assert.AreEqual(0, target.Usage);
        }


        [TestMethod()]
        public void KernelFunctionCacheConstructorTest2()
        {
            IKernel kernel = new Linear();

            int cacheSize = inputs.Length;

            KernelFunctionCache target = new KernelFunctionCache(kernel, inputs, cacheSize);

            Assert.AreEqual(10, target.Size);
            Assert.AreEqual(0, target.Hits);
            Assert.AreEqual(0, target.Misses);

            for (int i = 0; i < inputs.Length; i++)
            {
                double expected = i * i + 1;
                double actual = target.GetOrCompute(i);

                Assert.AreEqual(expected, actual);
            }

            Assert.AreEqual(0, target.Hits);

            int[] hits = { 0, 1, 3, 6, 10, 15, 21, 28, 36, 45 };
            int[] miss = { 9, 17, 24, 30, 35, 39, 42, 44, 45, 45 };

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

            Assert.AreEqual(45, target.Misses);
            Assert.AreEqual(135, target.Hits);
            Assert.AreEqual(1.0, target.Usage);
        }

        [TestMethod()]
        public void KernelFunctionCacheConstructorTest6()
        {
            IKernel kernel = new Gaussian(0.6);

            int cacheSize = inputs.Length;

            KernelFunctionCache target = new KernelFunctionCache(kernel, inputs, cacheSize);

            Assert.AreEqual(10, target.Size);
            Assert.AreEqual(0, target.Hits);
            Assert.AreEqual(0, target.Misses);

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

            Assert.AreEqual(45, target.Misses);
            Assert.AreEqual(135, target.Hits);
            Assert.AreEqual(1.0, target.Usage);
        }

        [TestMethod()]
        public void KernelFunctionCacheConstructorTest5()
        {
            IKernel kernel = new Linear();

            int cacheSize = inputs.Length;

            KernelFunctionCache target = new KernelFunctionCache(kernel, inputs, cacheSize);

            Assert.AreEqual(10, target.Size);
            Assert.AreEqual(0, target.Hits);
            Assert.AreEqual(0, target.Misses);

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


            Assert.AreEqual(45, target.Misses);
            Assert.AreEqual(0, target.Hits);
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


            Assert.AreEqual(45, target.Misses);
            Assert.AreEqual(45, target.Hits);
            Assert.AreEqual(1.0, target.Usage);
        }

        [TestMethod()]
        public void KernelFunctionCacheConstructorTest7()
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

            // upper half
            for (int i = 0; i < inputs.Length; i++)
            {
                for (int j = i + 1; j < inputs.Length; j++)
                {
                    double expected = kernel.Function(inputs[i], inputs[j]);
                    double actual = target.GetOrCompute(i, j);

                    Assert.AreEqual(expected, actual);
                }
            }

            var lruList1 = target.GetLeastRecentlyUsedList();

            Assert.AreEqual(3, target.Misses);
            Assert.AreEqual(0, target.Hits);
            Assert.AreEqual(1.0, target.Usage);

            // upper half, backwards
            for (int i = inputs.Length - 1; i >= 0; i--)
            {
                for (int j = inputs.Length - 1; j >= i; j--)
                {
                    double expected = kernel.Function(inputs[i], inputs[j]);
                    double actual = target.GetOrCompute(j, i);

                    Assert.AreEqual(expected, actual);
                }
            }

            var lruList2 = target.GetLeastRecentlyUsedList();

            Assert.IsTrue(lruList2.SequenceEqual(lruList1.Reverse())); 

            Assert.AreEqual(3, target.Misses);
            Assert.AreEqual(3, target.Hits);
            Assert.AreEqual(1.0, target.Usage);
        }
        [TestMethod()]
        public void KernelFunctionCacheConstructorTest3()
        {
            IKernel kernel = new Linear();

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

            Assert.AreEqual(9, target.Hits);
            Assert.AreEqual(81, target.Misses);

            var snapshot = target.GetDataCache();

            foreach (var entry in snapshot)
            {
                double a = target.GetOrCompute(entry.Key.Item1, entry.Key.Item2);
                double b = target.GetOrCompute(entry.Key.Item2, entry.Key.Item1);

                Assert.AreEqual(a, b);
            }


            Assert.AreEqual(81, target.Misses);
            Assert.AreEqual(29, target.Hits);
            Assert.AreEqual(1.0, target.Usage);
        }


        [TestMethod()]
        public void KernelFunctionCacheConstructorTest4()
        {
            IKernel kernel = new Linear();

            int cacheSize = 100;

            KernelFunctionCache target = new KernelFunctionCache(kernel, inputs, cacheSize);

            Assert.AreEqual(inputs.Length, target.Size);
        }


    }
}
