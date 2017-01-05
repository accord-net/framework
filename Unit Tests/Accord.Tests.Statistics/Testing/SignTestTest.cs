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
    using Accord.Statistics.Testing;
    using NUnit.Framework;
    using System;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Math;
    using Accord.Statistics;

    [TestFixture]
    public class SignTestTest
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



        [Test]
        public void SignTestConstructorTest()
        {
            // Example from http://www.unm.edu/~marcusj/1Samplesign.pdf

            double[] sample = 
            {
                1, 1, 2, 2, 3, 3, 4, 5, 5, 6, 7, 7, 8, 10,
                20, 22, 25, 27, 33, 40, 42, 50, 55, 75, 80 
            };

            SignTest target = new SignTest(sample, hypothesizedMedian: 30);

            // Wolfram Alpha gives 0.02896
            // GNU R gives 0.04329

            Assert.AreEqual(OneSampleHypothesis.ValueIsDifferentFromHypothesis, target.Hypothesis);
            Assert.AreEqual(0.043285, target.PValue, 1e-4);
            Assert.IsTrue(target.Significant);

        }

        [Test]
        public void SignTestConstructorTest2()
        {
            // This example has been adapted from the Wikipedia's page about
            // the Z-Test, available from: http://en.wikipedia.org/wiki/Z-test

            // We would like to check whether a sample of 20
            // students with a median score of 96 points ...

            double[] sample = 
            { 
                106, 115, 96, 88, 91, 88, 81, 104, 99, 68,
                104, 100, 77, 98, 96, 104, 82, 94, 72, 96
            };

            // ... could have happened just by chance inside a 
            // population with an hypothesized median of 100 points.

            double hypothesizedMedian = 100;

            // So we start by creating the test:
            SignTest test = new SignTest(sample, hypothesizedMedian,
                OneSampleHypothesis.ValueIsSmallerThanHypothesis);

            // Now, we can check whether this result would be
            // unlikely under a standard significance level:

            bool significant = test.Significant; // false (so the event was likely)

            // We can also check the test statistic and its P-Value
            double statistic = test.Statistic; // 5
            double pvalue = test.PValue; // 0.99039

            Assert.AreEqual(96, sample.Median());
            Assert.AreEqual(statistic, 5);
            Assert.AreEqual(pvalue, 0.99039459228515625);
        }
    }
}
