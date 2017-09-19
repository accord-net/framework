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
    using System.Linq;
    using Accord.Math;
    using NUnit.Framework;
    using Accord.Statistics;
    using System;
    using System.Collections.Generic;

    [TestFixture]
    public class ClassesTest
    {

        [Test]
        public void RandomTest()
        {
            #region doc_random
            // The Classes.Random method can be used to generate random class 
            // assignments. For example, let's say we would like to generate 
            // 100 binary assignments where the probability of having a positive
            // class assignment is of 80%:
            int[] y1 = Classes.Random(samples: 100, proportion: 0.8);

            // If we compute the histogram of y, the result will be:
            int[] hist1 = y1.Histogram(); // new int[] { 80, 20 } 

            // Let's say we would like to generate 100 assignments
            // where samples can belong to 1 of 3 different classes:
            int[] y2 = Classes.Random(samples: 100, classes: 3);

            // The histogram of y will be:
            int[] hist2 = y2.Histogram(); // new int[] { 34, 33, 33 }

            // Let's say we would like to generate the sample sample as above,
            // but we would like to control how many samples should fall into
            // each of the 3 categories:
            int[] y3 = Classes.Random(samples: 100, proportion: new[] { 0.2, 0.5, 0.3});

            // The histogram of y will be:
            int[] hist3 = y3.Histogram(); // new int[] { 20, 50, 30 }

            // The Random method can also generate balanced assignments according
            // to some pre-existing division. Let's say we have 100 samples that
            // have been already divided into 5 groups (i.e. the last result of y).
            // Now, we would like to separate those into two different groups such
            // that the proportion between the different classes in y is kept balanced
            // within those sub-groups:
            int[] y4 = Classes.Random(y3, categories: 2);

            // The histogram of y4 will be:
            int[] hist4 = y4.Histogram(); // new int[] { 50, 50 }

            // However, we can use those assignments to separate
            // the y3 labels into two 50-50 groups as we intended:
            int[][] r = Classes.Separate(y3, y4);

            // And as we can see, proportions are the same in both groups:
            int[] rhist1 = r[0].Histogram(); // new int[] { 10, 25, 15 }
            int[] rhist2 = r[1].Histogram(); // new int[] { 10, 25, 15 }
            #endregion

            Assert.AreEqual(new[] { 80, 20 }, hist1);
            Assert.AreEqual(new[] { 33, 34, 33 }, hist2);
            Assert.AreEqual(new[] { 20, 50, 30 }, hist3);
            Assert.AreEqual(new[] { 50, 50 }, hist4);
            Assert.AreEqual(new[] { 10, 25, 15 }, rhist1);
            Assert.AreEqual(new[] { 10, 25, 15 }, rhist2);
        }

    }
}
