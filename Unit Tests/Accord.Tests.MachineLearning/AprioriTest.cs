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
            #region doc_learn_1
            // Example from https://en.wikipedia.org/wiki/Apriori_algorithm

            // Assume that a large supermarket tracks sales data by stock-keeping unit
            // (SKU) for each item: each item, such as "butter" or "bread", is identified 
            // by a numerical SKU. The supermarket has a database of transactions where each
            // transaction is a set of SKUs that were bought together.

            // Let the database of transactions consist of following itemsets:

            SortedSet<int>[] dataset =
            {
                // Each row represents a set of items that have been bought 
                // together. Each number is a SKU identifier for a product.
                new SortedSet<int> { 1, 2, 3, 4 }, // bought 4 items
                new SortedSet<int> { 1, 2, 4 },    // bought 3 items
                new SortedSet<int> { 1, 2 },       // bought 2 items
                new SortedSet<int> { 2, 3, 4 },    // ...
                new SortedSet<int> { 2, 3 },
                new SortedSet<int> { 3, 4 },
                new SortedSet<int> { 2, 4 },
            };

            // We will use Apriori to determine the frequent item sets of this database.
            // To do this, we will say that an item set is frequent if it appears in at 
            // least 3 transactions of the database: the value 3 is the support threshold.

            // Create a new a-priori learning algorithm with support 3
            Apriori apriori = new Apriori(threshold: 3, confidence: 0);

            // Use the algorithm to learn a set matcher
            AssociationRuleMatcher<int> classifier = apriori.Learn(dataset);

            // Use the classifier to find orders that are similar to 
            // orders where clients have bought items 1 and 2 together:
            int[][] matches = classifier.Decide(new[] { 1, 2 });

            // The result should be:
            //
            //   new int[][]
            //   {
            //       new int[] { 4 },
            //       new int[] { 3 }
            //   };

            // Meaning the most likely product to go alongside the products
            // being bought is item 4, and the second most likely is item 3.

            // We can also obtain the association rules from frequent itemsets:
            AssociationRule<int>[] rules = classifier.Rules;

            // The result will be:
            // {
            //     [1] -> [2]; support: 3, confidence: 1, 
            //     [2] -> [1]; support: 3, confidence: 0.5, 
            //     [2] -> [3]; support: 3, confidence: 0.5, 
            //     [3] -> [2]; support: 3, confidence: 0.75, 
            //     [2] -> [4]; support: 4, confidence: 0.66, 
            //     [4] -> [2]; support: 4, confidence: 0.8, 
            //     [3] -> [4]; support: 3, confidence: 0.75, 
            //     [4] -> [3]; support: 3, confidence: 0.6 
            // };
            #endregion

            Assert.AreEqual(8, rules.Length);
            Assert.AreEqual(rules[0].ToString(), "[1] -> [2]; support: 3, confidence: 1");
            Assert.AreEqual(rules[1].ToString(), "[2] -> [1]; support: 3, confidence: 0.5");
            Assert.AreEqual(rules[2].ToString(), "[2] -> [3]; support: 3, confidence: 0.5");
            Assert.AreEqual(rules[3].ToString(), "[3] -> [2]; support: 3, confidence: 0.75");
            Assert.AreEqual(rules[4].ToString(), "[2] -> [4]; support: 4, confidence: 0.666666666666667");
            Assert.AreEqual(rules[5].ToString(), "[4] -> [2]; support: 4, confidence: 0.8");
            Assert.AreEqual(rules[6].ToString(), "[3] -> [4]; support: 3, confidence: 0.75");
            Assert.AreEqual(rules[7].ToString(), "[4] -> [3]; support: 3, confidence: 0.6");

            var str = matches.ToCSharp();

            int[][] expectedMatches = new int[][]
            {
                new int[] { 4 },
                new int[] { 3 }
            };

            var expected = new Tuple<int[], int>[]
            {
                Tuple.Create(new[] {1},   3),
                Tuple.Create(new[] {2},   6),
                Tuple.Create(new[] {3},   4),
                Tuple.Create(new[] {4},   5),
                Tuple.Create(new[] {1,2}, 3),
                Tuple.Create(new[] {2,3}, 3),
                Tuple.Create(new[] {2,4}, 4),
                Tuple.Create(new[] {3,4}, 3),
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
            #region doc_learn_2
            // Example from http://www3.cs.stonybrook.edu/~cse634/lecture_notes/07apriori.pdf

            // For this example, we will consider a database consisting of 9 transactions:
            // - Minimum support count required will be 2 (i.e. min_sup = 2 / 9 = 22%);
            // - Minimum confidence required should be 70%;
            
            // The dataset of transactions can be as follows:
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

            // Create a new A-priori learning algorithm with the requirements
            var apriori = new Apriori<string>(threshold: 2, confidence: 0.7);

            // Use apriori to generate a n-itemset generation frequent pattern
            AssociationRuleMatcher<string> classifier = apriori.Learn(dataset);

            // Generate association rules from the itemsets:
            AssociationRule<string>[] rules = classifier.Rules;

            // The result should be:
            // {
            //   [5]   -> [1];   support: 2, confidence: 1,
            //   [5]   -> [2];   support: 2, confidence: 1,
            //   [4]   -> [2];   support: 2, confidence: 1,
            //   [5]   -> [1 2]; support: 2, confidence: 1,
            //   [1 5] -> [2];   support: 2, confidence: 1,
            //   [2 5] -> [1];   support: 2, confidence: 1,
            // }
            #endregion

            Assert.AreEqual(6, rules.Length);
            Assert.AreEqual(rules[0].ToString(), "[5] -> [1]; support: 2, confidence: 1");
            Assert.AreEqual(rules[1].ToString(), "[5] -> [2]; support: 2, confidence: 1");
            Assert.AreEqual(rules[2].ToString(), "[4] -> [2]; support: 2, confidence: 1");
            Assert.AreEqual(rules[3].ToString(), "[5] -> [1 2]; support: 2, confidence: 1");
            Assert.AreEqual(rules[4].ToString(), "[1 5] -> [2]; support: 2, confidence: 1");
            Assert.AreEqual(rules[5].ToString(), "[2 5] -> [1]; support: 2, confidence: 1");


            string[][] actual;

            actual = classifier.Decide(new string[] { "1", "5" });
            Assert.AreEqual("2", actual[0][0]);
            Assert.AreEqual("1", actual[1][0]);
            Assert.AreEqual("2", actual[1][1]);
            actual = classifier.Decide(new string[] { "2", "5" });
            Assert.AreEqual("1", actual[0][0]);
            Assert.AreEqual("1", actual[1][0]);
            Assert.AreEqual("2", actual[1][1]);
            actual = classifier.Decide(new string[] { "0", "5" });
            Assert.AreEqual("1", actual[0][0]);
            Assert.AreEqual("2", actual[0][1]);
            Assert.AreEqual("2", actual[1][0]);
            Assert.AreEqual("1", actual[2][0]);
        }
    }
}
