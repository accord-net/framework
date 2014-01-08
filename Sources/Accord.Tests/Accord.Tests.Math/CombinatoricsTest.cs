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
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class CombinatoricsTest
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
        public void TruthTableTest()
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
        public void TruthTableTest2()
        {
            int[] symbols = { 2, 3, 2 };

            int[][] expected =
            {
                new int[] { 0, 0, 0 },
                new int[] { 0, 0, 1 },
                new int[] { 0, 1, 0 },
                new int[] { 0, 1, 1 },
                new int[] { 0, 2, 0 },
                new int[] { 0, 2, 1 },
                new int[] { 1, 0, 0 },
                new int[] { 1, 0, 1 },
                new int[] { 1, 1, 0 },
                new int[] { 1, 1, 1 },
                new int[] { 1, 2, 0 },
                new int[] { 1, 2, 1 },
            };

            int[][] actual = Combinatorics.TruthTable(symbols);

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod()]
        public void SequencesTest()
        {
            int[] symbols = { 2, 3, 2 };

            int[][] expected =
            {
                new int[] { 0, 0, 0 },
                new int[] { 0, 0, 1 },
                new int[] { 0, 1, 0 },
                new int[] { 0, 1, 1 },
                new int[] { 0, 2, 0 },
                new int[] { 0, 2, 1 },
                new int[] { 1, 0, 0 },
                new int[] { 1, 0, 1 },
                new int[] { 1, 1, 0 },
                new int[] { 1, 1, 1 },
                new int[] { 1, 2, 0 },
                new int[] { 1, 2, 1 },
            };

            int[][] actual = Combinatorics.Sequences(symbols).Select(x => (int[])x.Clone()).ToArray();

            Assert.IsTrue(expected.IsEqual(actual));
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
