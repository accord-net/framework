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


    [TestFixture]
    public class MultinomialTestTest
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
        public void MultinomialTestConstructorTest()
        {
            // Example from http://www.stat.berkeley.edu/~stark/SticiGui/Text/chiSquare.htm

            int[] sample = { 45, 41, 9 };
            double[] hypothesizedProportion = { 18 / 38.0, 18 / 38.0, 2 / 38.0 };

            MultinomialTest target = new MultinomialTest(sample, hypothesizedProportion);

            Assert.AreEqual(18 / 38.0, target.HypothesizedProportions[0]);
            Assert.AreEqual(18 / 38.0, target.HypothesizedProportions[1]);
            Assert.AreEqual(2 / 38.0, target.HypothesizedProportions[2]);

            Assert.AreEqual(45 / 95.0, target.ObservedProportions[0]);
            Assert.AreEqual(41 / 95.0, target.ObservedProportions[1]);
            Assert.AreEqual(9 / 95.0, target.ObservedProportions[2]);


            Assert.AreEqual(3.55555556, target.Statistic, 1e-5);
        }

        [Test]
        public void MultinomialTestConstructorTest2()
        {
            // This example is based on the example available on About.com Statistics,
            // An Example of Chi-Square Test for a Multinomial Experiment By Courtney Taylor
            // http://statistics.about.com/od/Inferential-Statistics/a/An-Example-Of-Chi-Square-Test-For-A-Multinomial-Experiment.htm

            // In this example, we would like to test if a die is fair. For this, we
            // will be rolling the die 600 times, annotating the result every time 
            // the die falls. In the end, we got a one 106 times, a two 90 times, a 
            // three 98 times, a four 102 times, a five 100 times and a six 104 times:

            int[] sample = { 106, 90, 98, 102, 100, 104 };

            // If the die was fair, we should note that we would be expecting the
            // probabilities to be all equal to 1 / 6:

            double[] hypothesizedProportion = 
            { 
                //   1        2           3          4          5         6
                1 / 6.0,   1 / 6.0,   1 / 6.0,   1 / 6.0,   1 / 6.0,   1 / 6.0, 
            };

            // Now, we create our test using the samples and the expected proportion
            MultinomialTest test = new MultinomialTest(sample, hypothesizedProportion);

            double chiSquare = test.Statistic; // 1.6
            bool significant = test.Significant; // false

            // Since the test didn't come up significant, it means that we
            // don't have enough evidence to to reject the null hypothesis 
            // that the die is fair.

            Assert.AreEqual(1.6000000000000003, chiSquare);
            Assert.IsFalse(significant);
        }
    }
}
