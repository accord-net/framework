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

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;


    [TestClass()]
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



        [TestMethod()]
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

    }
}
