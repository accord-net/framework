// Accord Unit Tests
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
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
    public class BinomialTestTest
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

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion



        [TestMethod()]
        public void BinomialTestConstructorTest1()
        {

            bool[] trials = { true, true, true, true, true, true, true, true, true, false };

            BinomialTest target = new BinomialTest(trials,
                hypothesizedProbability: 0.5, alternate: OneSampleHypothesis.ValueIsGreaterThanHypothesis);

            Assert.AreEqual(OneSampleHypothesis.ValueIsGreaterThanHypothesis, target.Hypothesis);
            Assert.AreEqual(DistributionTail.OneUpper, target.Tail);

            Assert.AreEqual(0.010742, target.PValue, 1e-5);
            Assert.IsTrue(target.Significant);
        }

        [TestMethod()]
        public void BinomialTestConstructorTest4()
        {

            bool[] trials = { false, false, false, false, false, false, false, false, false, true };

            BinomialTest target = new BinomialTest(trials,
                hypothesizedProbability: 0.5, alternate: OneSampleHypothesis.ValueIsSmallerThanHypothesis);

            Assert.AreEqual(OneSampleHypothesis.ValueIsSmallerThanHypothesis, target.Hypothesis);
            Assert.AreEqual(DistributionTail.OneLower, target.Tail);

            Assert.AreEqual(0.010742, target.PValue, 1e-5);
            Assert.IsTrue(target.Significant);
        }

        [TestMethod()]
        public void BinomialTestConstructorTest2()
        {
            int successes = 5;
            int trials = 6;
            double probability = 1 / 4.0;

            BinomialTest target = new BinomialTest(successes, trials,
                hypothesizedProbability: probability,
                alternate: OneSampleHypothesis.ValueIsGreaterThanHypothesis);

            Assert.AreEqual(OneSampleHypothesis.ValueIsGreaterThanHypothesis, target.Hypothesis);
            Assert.AreEqual(DistributionTail.OneUpper, target.Tail);

            Assert.AreEqual(0.004638, target.PValue, 1e-5);
            Assert.IsTrue(target.Significant);
        }

        [TestMethod()]
        public void BinomialTestConstructorTest3()
        {
            BinomialTest target = new BinomialTest(5, 18);

            Assert.AreEqual(OneSampleHypothesis.ValueIsDifferentFromHypothesis, target.Hypothesis);
            Assert.AreEqual(DistributionTail.TwoTail, target.Tail);

            Assert.AreEqual(0.09625, target.PValue, 1e-4);
            Assert.IsFalse(target.Significant);
        }

        [TestMethod()]
        public void BinomialTestConstructorTest6()
        {
            double[] expected =
            {
                0.000000000000, 0.00000000000, 0.0000001539975, 0.00592949743, 
                0.514242625443, 0.25905439494, 0.0053806543164, 0.00001078919,
                0.000000003115, 0.00000000000, 0.0000000000000
            };

            for (int i = 0; i <= 10; i++)
            {
                double p = i / 100.0 * 5;
                BinomialTest target = new BinomialTest(51, 235, p);

                Assert.AreEqual(DistributionTail.TwoTail, target.Tail);
                Assert.AreEqual(expected[i], target.PValue, 1e-5);
            }
        }

        [TestMethod()]
        public void BinomialTestConstructorTest7()
        {
            double[] expected =
            {
                0.00000000000, 0.02819385651, 0.382725376073,
                1.00000000000, 0.34347252004, 0.096252441406, 
                0.00707077678, 0.00026908252, 0.000002519659,
                0.00000000052, 0.00000000000 
            };

            for (int i = 0; i <= 10; i++)
            {
                double p = i / 10.0;
                BinomialTest target = new BinomialTest(5, 18, p);

                Assert.AreEqual(DistributionTail.TwoTail, target.Tail);
                Assert.AreEqual(expected[i], target.PValue, 5e-4);
            }
        }

    }
}
