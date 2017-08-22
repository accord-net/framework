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

namespace Accord.Tests.MachineLearning
{
    using NUnit.Framework;
    using System.Linq;
    using Accord.Math;
    using Accord.MachineLearning.Rules;

#if !NET35
    using System;
    using System.Collections.Generic;
#else
    using Accord.Compat;
#endif


    [TestFixture]
    public class AprioriTest
    {

        [Test]
        public void AprioriExampleTest1()
        {
            // Example from https://en.wikipedia.org/wiki/Apriori_algorithm

            SortedSet<int>[] dataset = 
            {
                new SortedSet<int> { 1, 2, 3, 4 },
                new SortedSet<int> { 1, 2, 4 },
                new SortedSet<int> { 1, 2 },
                new SortedSet<int> { 2, 3, 4 },
                new SortedSet<int> { 2, 3 },
                new SortedSet<int> { 3, 4 },
                new SortedSet<int> { 2, 4 },
            };

            var apriori = new Apriori(threshold: 3, confidence: 0);

            var classifier = apriori.Learn(dataset);

            var expected = new Tuple<int[], int>[]
            {
                Tuple.Create(new[] {1},   3),
                Tuple.Create(new[] {2},   6),
                Tuple.Create(new[] {3},   4),
                Tuple.Create(new[] {4},   5),
                Tuple.Create(new[] {1,2}, 3),
                //Tuple.Create(new[] {1,3}, 1),
                //Tuple.Create(new[] {1,4}, 2),
                Tuple.Create(new[] {2,3}, 3),
                Tuple.Create(new[] {2,4}, 4),
                Tuple.Create(new[] {3,4}, 3),
                //Tuple.Create(new[] {2,3,4}, 2),
            };

            var frequent = apriori.Frequent;

            foreach (var tuple in expected)
            {
                var a = frequent.Where(x => x.Key.SetEquals(tuple.Item1)).First();
                Assert.AreEqual(tuple.Item2, a.Value);
            }
        }

        [Test]
        public void ClassifierTest1()
        {
            // example from http://www3.cs.stonybrook.edu/~cse634/lecture_notes/07apriori.pdf

            string[][] dataset = 
            {
                new string[] { "1", "2", "5" },
                new string[] { "2", "4" },
                new string[] { "2", "3" },
                new string[] { "1", "2", "4" },
                new string[] { "1", "3" },
                new string[] { "2", "3" },
                new string[] { "1", "3" },
                new string[] { "1", "2", "3", "5" },
                new string[] { "1", "2", "3" },
            };

            var apriori = new Apriori<string>(threshold: 2, confidence: 0.7);

            var classifier = apriori.Learn(dataset);

            var rules = classifier.Rules;

            Assert.AreEqual(6, rules.Length);
            Assert.AreEqual(rules[0].ToString(), "[5] -> [1]; support: 2, confidence: 1");
            Assert.AreEqual(rules[1].ToString(), "[5] -> [2]; support: 2, confidence: 1");
            Assert.AreEqual(rules[2].ToString(), "[4] -> [2]; support: 2, confidence: 1");
            Assert.AreEqual(rules[3].ToString(), "[5] -> [1 2]; support: 2, confidence: 1");
            Assert.AreEqual(rules[4].ToString(), "[1 5] -> [2]; support: 2, confidence: 1");
            Assert.AreEqual(rules[5].ToString(), "[2 5] -> [1]; support: 2, confidence: 1");


            string[][] actual;

            actual = classifier.Decide(new string[] { "1", "5" });
            Assert.AreEqual("1", actual[0][0]);
            Assert.AreEqual("2", actual[1][0]);
            Assert.AreEqual("1", actual[2][0]);
            Assert.AreEqual("2", actual[2][1]);
            Assert.AreEqual("2", actual[3][0]);
            actual = classifier.Decide(new string[] { "2", "5" });
            Assert.AreEqual("1", actual[0][0]);
            Assert.AreEqual("2", actual[1][0]);
            Assert.AreEqual("1", actual[2][0]);
            Assert.AreEqual("2", actual[2][1]);
            Assert.AreEqual("1", actual[3][0]);
            actual = classifier.Decide(new string[] { "0", "5" });
            Assert.AreEqual("1", actual[0][0]);
            Assert.AreEqual("2", actual[1][0]);
            Assert.AreEqual("1", actual[2][0]);
            Assert.AreEqual("2", actual[2][1]);
        }
    }
}
