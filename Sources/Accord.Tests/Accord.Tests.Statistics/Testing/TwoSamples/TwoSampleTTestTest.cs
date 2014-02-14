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
    using Accord.Statistics.Testing.Power;

    [TestClass()]
    public class TwoSampleTTestTest
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
        public void TTestConstructorTest()
        {
            // Example from http://en.wikipedia.org/wiki/Student%27s_t-test#Two-sample_T.C2.A02_test

            double[] sample1 = { 30.02, 29.99, 30.11, 29.97, 30.01, 29.99 };
            double[] sample2 = { 29.89, 29.93, 29.72, 29.98, 30.02, 29.98 };

            TwoSampleTTest test;

            // Unequal variances
            test = new TwoSampleTTest(sample1, sample2, assumeEqualVariances: false);

            Assert.AreEqual(0.0485, test.StandardError, 1e-4);
            Assert.AreEqual(0.095, test.ObservedDifference, 1e-10);
            Assert.AreEqual(7.03, test.DegreesOfFreedom, 1e-3);

            Assert.AreEqual(0.091, test.PValue, 0.001);

            test = new TwoSampleTTest(sample1, sample2, assumeEqualVariances: false,
                alternate: TwoSampleHypothesis.FirstValueIsGreaterThanSecond);

            Assert.AreEqual(0.0485, test.StandardError, 1e-4);
            Assert.AreEqual(0.095, test.ObservedDifference, 1e-10);
            Assert.AreEqual(7.03, test.DegreesOfFreedom, 1e-3);

            Assert.AreEqual(0.045, test.PValue, 0.001);

            // Equal variances
            test = new TwoSampleTTest(sample1, sample2, assumeEqualVariances: true);

            Assert.AreEqual(0.0485, test.StandardError, 1e-4);
            Assert.AreEqual(0.095, test.ObservedDifference, 1e-10);
            Assert.AreEqual(10, test.DegreesOfFreedom);

            Assert.AreEqual(0.078, test.PValue, 0.001);

            test = new TwoSampleTTest(sample1, sample2, assumeEqualVariances: true,
                alternate: TwoSampleHypothesis.FirstValueIsGreaterThanSecond);

            Assert.AreEqual(0.0485, test.StandardError, 1e-4);
            Assert.AreEqual(0.095, test.ObservedDifference, 1e-10);
            Assert.AreEqual(10, test.DegreesOfFreedom);

            Assert.AreEqual(0.038, test.PValue, 0.0015);
        }

        [TestMethod()]
        public void TTestConstructorTest2()
        {

            // Example from Larson & Farber, Elementary Statistics: Picturing the world.

            /*
             * A random sample of 17 police officers in Brownsville has a mean annual
             * income of $35,800 and a standard deviation of $7,800.  In Greensville,
             * a random sample of 18 police officers has a mean annual income of $35,100
             * and a standard deviation of $7,375.  Test the claim at a = 0.01 that the
             * mean annual incomes in the two cities are not the same.  Assume the 
             * population variances are equal.
             */

            double mean1 = 35800;
            double stdDev1 = 7800;
            double var1 = stdDev1 * stdDev1;
            int n1 = 17;

            double mean2 = 35100;
            double stdDev2 = 7375;
            double var2 = stdDev2 * stdDev2;
            int n2 = 18;

            TwoSampleTTest test = new TwoSampleTTest(mean1, var1, n1, mean2, var2, n2,
                assumeEqualVariances: true,
                alternate: TwoSampleHypothesis.ValuesAreDifferent);

            Assert.AreEqual(33, test.DegreesOfFreedom);
            Assert.AreEqual(2564.92, test.StandardError, 1e-3);
            Assert.AreEqual(0.273, test.Statistic, 1e-3);

            test.Size = 0.01;

            Assert.IsFalse(test.Significant);
        }

        [TestMethod]
        public void SampleSizeTest1()
        {
            // Example from http://udel.edu/~mcdonald/statttest.html
            // Computed using R's function power.t.test 

            double mean1 = 3.2;
            double mean2 = 0;
            double var1 = System.Math.Pow(4.3, 2);
            double var2 = System.Math.Pow(4.3, 2);
            double alpha = 0.05;
            double power = 0.80;

            TwoSampleTTest test = new TwoSampleTTest(
                mean1: mean1, var1: var1, samples1: 10,
                mean2: mean2, var2: var2, samples2: 10,
                assumeEqualVariances: true, alternate: TwoSampleHypothesis.ValuesAreDifferent);

            var target = (TwoSampleTTestPowerAnalysis)test.Analysis.Clone();

            target.Power = power;
            target.Size = alpha;

            target.ComputeSamples(1);
            double actual = target.Samples1;

            double expected = 29.33682;

            Assert.AreEqual(expected, actual, 1e-5);
        }
    }
}
