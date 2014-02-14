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
    using System.Data;
    using Accord.Math;

    [TestClass()]
    public class WilcoxonSignedRankTestTest
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
        public void WilcoxonSignedRankTestConstructorTest()
        {
            double[] sample = { 17, 50, 45, 59.8, 21.74, 16, 9, 15.43, 5.12, 40, 35, 13.35, 13.4 };

            double hypothesizedMedian = 7.38;

            var target = new WilcoxonSignedRankTest(sample, hypothesizedMedian);

            Assert.AreEqual(OneSampleHypothesis.ValueIsDifferentFromHypothesis, target.Hypothesis);

            double[] delta = { 9.62, 42.62, 37.62, 52.42, 14.36, 8.62, 1.62, 8.05, 2.26, 32.62, 27.62, 5.97, 6.02 };
            double[] ranks = { 7, 12, 11, 13, 8, 6, 1, 5, 2, 10, 9, 3, 4 };

            Assert.IsTrue(delta.IsEqual(target.Delta, 1e-6));
            Assert.IsTrue(ranks.IsEqual(target.Ranks, 1e-6));

            Assert.AreEqual(89, target.Statistic);
            Assert.AreEqual(0.003, target.PValue, 1e-3);
            Assert.IsTrue(target.Significant);
        }

        [TestMethod()]
        public void WilcoxonSignedRankTestConstructorTest2()
        {
            // Example from https://onlinecourses.science.psu.edu/stat414/node/319
            double[] sample = { 5.0, 3.9, 5.2, 5.5, 2.8, 6.1, 6.4, 2.6, 1.7, 4.3 };

            double hypothesizedMedian = 3.7;

            var target = new WilcoxonSignedRankTest(sample, hypothesizedMedian);

            Assert.AreEqual(OneSampleHypothesis.ValueIsDifferentFromHypothesis, target.Hypothesis);

            // Absolute differences and original signs
            double[] delta = { +1.3, +0.2, +1.5, +1.8, +0.9, +2.4, +2.7, +1.1, +2.0, +0.6 };
            double[] signs = { +1.0, +1.0, +1.0, +1.0, -1.0, -1.0, +1.0, -1.0, -1.0, +1.0 };
            double[] ranks = { +5.0, +1.0, +6.0, +7.0, +3.0, +9.0, +10, +4.0, +8.0, +2.0 };

            Assert.AreEqual(40, target.Statistic);
            Assert.AreEqual(0.232, target.PValue, 1e-3);
            Assert.IsFalse(target.Significant);
        }

        [TestMethod()]
        public void WilcoxonSignedRankTestConstructorTest3()
        {
            // Example from https://onlinecourses.science.psu.edu/stat414/node/319

            double[] sample = 
            {
                35.5,   44.5,  39.8,  33.3,  51.4,  51.3,  30.5,  48.9,   42.1,   40.3,
                46.8,   38.0,  40.1,  36.8,  39.3,  65.4,  42.6,  42.8,   59.8,   52.4,
                26.2,   60.9,  45.6,  27.1,  47.3,  36.6,  55.6,  45.1,   52.2,   43.5,
            };

            double hypothesizedMedian = 45;

            var target = new WilcoxonSignedRankTest(sample, hypothesizedMedian);

            Assert.AreEqual(OneSampleHypothesis.ValueIsDifferentFromHypothesis, target.Hypothesis);


            Assert.AreEqual(200, target.Statistic);
            Assert.AreEqual(0.510, target.PValue, 1e-3);
            Assert.IsFalse(target.Significant);
        }
    }
}
