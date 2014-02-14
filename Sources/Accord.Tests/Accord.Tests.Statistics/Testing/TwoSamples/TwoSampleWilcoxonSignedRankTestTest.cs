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
    public class TwoSampleWilcoxonSignedRankTestTest
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
            // Example from http://en.wikipedia.org/wiki/Wilcoxon_signed-rank_test

            double[] sample1 = { 125, 115, 130, 140, 140, 115, 140, 125, 140, 135 };
            double[] sample2 = { 110, 122, 125, 120, 140, 124, 123, 137, 135, 145 };

            TwoSampleHypothesis alternate = TwoSampleHypothesis.ValuesAreDifferent;
            var target = new TwoSampleWilcoxonSignedRankTest(sample1, sample2, alternate);

            // Wikipedia uses an alternate definition for the W statistic. The framework
            // uses the positive W statistic. There is no difference, if the proper
            // respective statistical tables (or distributions) are followed.
            Assert.AreEqual(27, target.Statistic);
            Assert.IsFalse(target.Significant);
        }

        [TestMethod()]
        public void WilcoxonSignedRankTestConstructorTest2()
        {
            // Example from http://vassarstats.net/textbook/ch12a.html
            double[] sample1 = { 78, 24, 64, 45, 64, 52, 30, 50, 64, 50, 78, 22, 84, 40, 90, 72 };
            double[] sample2 = { 78, 24, 62, 48, 68, 56, 25, 44, 56, 40, 68, 36, 68, 20, 58, 32 };

            TwoSampleHypothesis alternate = TwoSampleHypothesis.FirstValueIsSmallerThanSecond;
            var target = new TwoSampleWilcoxonSignedRankTest(sample1, sample2, alternate);

            // The referred example uses the summed W statistic. The framework
            // uses the positive W statistic. There is no difference, if proper
            // respective statistical tables (or distributions) are followed.

            Assert.AreEqual(86, target.Statistic); // W = 67
            Assert.AreEqual(14, target.Samples);
            Assert.IsTrue(target.Significant);
            Assert.IsTrue(target.PValue > 0.01 && target.PValue < 0.05);
        }

        [TestMethod()]
        public void WilcoxonSignedRankTestConstructorTest3()
        {
            // Example from http://courses.wcupa.edu/rbove/Berenson/CD-ROM%20Topics/topice-10_5.pdf
            double[] sample1 = { 9.98, 9.88, 9.84, 9.99, 9.94, 9.84, 9.86, 10.12, 9.90, 9.91 };
            double[] sample2 = { 9.88, 9.86, 9.75, 9.80, 9.87, 9.84, 9.87, 9.86, 9.83, 9.86 };

            var target = new TwoSampleWilcoxonSignedRankTest(sample1, sample2,
                alternate: TwoSampleHypothesis.FirstValueIsSmallerThanSecond);

            Assert.AreEqual(TwoSampleHypothesis.FirstValueIsSmallerThanSecond, target.Hypothesis);

            double[] delta = { 0.1, 0.02, 0.09, 0.19, 0.07, 0, 0.01, 0.26, 0.07, 0.05 };
            double[] ranks = { 7, 2, 6, 8, 4.5, 0, 1, 9, 4.5, 3 };
            int[] signs = { +1, +1, +1, +1, +1, 0, -1, +1, +1, +1 };

            Assert.IsTrue(delta.IsEqual(target.Delta, 1e-6));
            Assert.IsTrue(ranks.IsEqual(target.Ranks, 1e-6));
            Assert.IsTrue(signs.IsEqual(target.Signs));

            Assert.AreEqual(44, target.Statistic);
            Assert.IsTrue(target.Significant);
        }

        [TestMethod()]
        public void WilcoxonSignedRankTestConstructorTest4()
        {
            // Example from http://mlsc.lboro.ac.uk/resources/statistics/wsrt.pdf

            double[] sample1 = { 2.0, 3.6, 2.6, 2.6, 7.3, 3.4, 14.9, 6.6, 2.3, 2.0, 6.8, 08.5 };
            double[] sample2 = { 3.5, 5.7, 2.9, 2.4, 9.9, 3.3, 16.7, 6.0, 3.8, 4.0, 9.1, 20.9 };

            var target = new TwoSampleWilcoxonSignedRankTest(sample2, sample1);

            Assert.AreEqual(TwoSampleHypothesis.ValuesAreDifferent, target.Hypothesis);


            double[] diffs = { +1.5, +2.1, +0.3, 0.2, +2.6, 0.1, +1.8, 0.6, +1.5, +2.0, +2.3, +12.4 };
            int[] signs =    { +1, +1, +1, -1, +1, -1, +1, -1, +1, +1, +1, +1 };

            Assert.IsTrue(diffs.IsEqual(target.Delta, 1e-6));
            Assert.IsTrue(signs.IsEqual(target.Signs));

            Assert.AreEqual(71, target.Statistic);
            Assert.AreEqual(0.01, target.PValue, 1e-2);
            Assert.IsTrue(target.Significant);
        }
    }
}
