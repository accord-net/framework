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
    using System;
    using Accord.Math.Optimization;
    using NUnit.Framework;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Accord.Math;
    using System.Threading;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.ComponentModel;

    [TestFixture]
#if !MONO
    [SingleThreaded]
#endif
    public class GeneratorTest
    {

        [Test]
        public void ParallelTest_zero()
        {
            Accord.Math.Random.Generator.Seed = 0;
            var l = create(100, 10, reset: false);
            int sameCount = count(l);
            Assert.IsTrue(sameCount > 50);
        }

        [Test]
        [Ignore("Re-enable this test when GH-870 is implemented")]
        public void ParallelTest_zero_2()
        {
            // https://github.com/accord-net/framework/issues/870
            Accord.Math.Random.Generator.Seed = 0;
            int n = 100000;
            var seeds = new int?[n];
            var values = new int[n];
            Parallel.For(0, seeds.Length, i =>
            {
                seeds[i] = Accord.Math.Random.Generator.ThreadSeed;
                values[i] = Accord.Math.Random.Generator.Random.Next();
            });

            Assert.IsTrue(seeds.All(x => x == 0));
            Assert.IsTrue(values.All(x => x == values[0]));
        }

        [Test]
        public void ParallelTest_less_than_zero()
        {
            Accord.Math.Random.Generator.Seed = -1;
            var l = create(1000, 10, reset: false);
            int sameCount = count(l);
            Assert.IsTrue(sameCount > 30);
        }

        [Test]
        public void ParallelTest_higher_than_zero()
        {
            Accord.Math.Random.Generator.Seed = 1;
            var l = create(100, 10, reset: false);
            int sameCount = count(l);
            Assert.IsTrue(sameCount == 0);
        }

        [Test]
        public void ParallelTest_zero_but_reinit()
        {
            Accord.Math.Random.Generator.Seed = 0;

            int rows = 100;
            int cols = 10;

            double[][] l = create(rows, cols, reset: true);

            int sameCount = count(l);
            Assert.IsTrue(sameCount == 0);
        }

        [Test]
        public void ParallelTest_null()
        {
            Accord.Math.Random.Generator.Seed = null;

            var l = create(100, 10, reset: false);
            int sameCount = count(l);
            Assert.IsTrue(sameCount == 0);
        }

        [Test]
        public void consistency()
        {
            Accord.Math.Random.Generator.Seed = 0;
            int[] actual = random(3);
            int[] expected = new int[] { 1559595546, 1755192844, 1649316166 };
            var str = actual.ToCSharp();
            Assert.IsTrue(expected.IsEqual(actual));

            Accord.Math.Random.Generator.Seed = -1;
            actual = random(3);
            expected = new int[] { 534011718, 237820880, 1002897798 };
            str = actual.ToCSharp();
            Assert.IsTrue(expected.IsEqual(actual));

            Accord.Math.Random.Generator.Seed = 1;
            actual = random(3);
            expected = new int[] { 607892308, 1910784178, 911229122 };
            str = actual.ToCSharp();
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [Test]
        public void thread_initialization()
        {
            Accord.Math.Random.Generator.Seed = 0;

            var values = new ConcurrentDictionary<int, List<int>>();
            var seeds = new ConcurrentDictionary<int, List<int?>>();

            Thread[] t = new Thread[100];

            for (int i = 0; i < 100; i++)
            {
                t[i] = new Thread(() =>
                {
                    int threadId = Thread.CurrentThread.ManagedThreadId;
                    var vd = values.GetOrAdd(threadId, new List<int>());
                    var sd = seeds.GetOrAdd(threadId, new List<int?>());

                    int? before = Accord.Math.Random.Generator.ThreadSeed;
                    var r = Accord.Math.Random.Generator.Random;
                    int? after = Accord.Math.Random.Generator.ThreadSeed;

                    int v = r.Next();
                    vd.Add(v);
                    sd.Add(before);
                    sd.Add(after);
                });
                t[i].Start();
            };

            for (int i = 0; i < 100; i++)
                t[i].Join();

            int[] keys = values.Keys.ToArray().Sorted();
            Assert.AreEqual(100, keys.Length);

            int? ex = null;
            for (int i = 0; i < keys.Length; i++)
            {
                var l = values[keys[i]];
                if (ex == null)
                    ex = l[0];
                else
                    Assert.AreEqual(ex.Value, l[0]);

                var s = seeds[keys[i]];
                // Assert.AreEqual(2, s.Count);
                Assert.IsNull(s[0]);
                Assert.AreEqual(0, s[1]);
            }
        }








        private static int[] random(int n)
        {
            var r = Accord.Math.Random.Generator.Random;
            int[] v = new int[n];
            for (int i = 0; i < v.Length; i++)
                v[i] = r.Next();
            return v;
        }

        private static int count(IList<double[]> l)
        {
            int sameCount = 0;
            for (int i = 0; i < l.Count; i++)
            {
                for (int j = 0; j < l.Count; j++)
                {
                    var li = l[i];
                    var lj = l[j];

                    if (i != j)
                        if (li.IsEqual(lj, atol: 1e-8))
                            sameCount++;
                }
            }

            return sameCount;
        }

        private static double[][] create(int rows, int cols, bool reset)
        {
            var l = new double[rows][];
            Thread[] t = new Thread[rows];
            for (int i = 0; i < rows; i++)
            {
                t[i] = new Thread(thread(cols, reset, l));
                t[i].Start(i);
            }

            for (int i = 0; i < rows; i++)
                t[i].Join();
            return l;
        }

        private static ParameterizedThreadStart thread(int cols, bool reset, double[][] l)
        {
            return (object obj) =>
            {
                int j = (int)obj;
                if (reset)
                    Accord.Math.Random.Generator.ThreadSeed = j;
                l[j] = Vector.Random(cols);
            };
        }
    }

}
