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
    using Accord.Statistics.Testing.Power;
    using System;
    using Accord.Statistics.Distributions.Univariate;

    [TestFixture]
    public class GrubbTestTest
    {

        [Test]
        public void grubb_test_success()
        {
            // http://www.itl.nist.gov/div898/handbook/eda/section3/eda35h1.htm

            double[] sample =
            {
                199.31, 199.53, 200.19, 200.82, 201.92, 201.95, 202.18, 245.57
            };

            // Null Hypothesis: there are no outliers in the data
            // Alternative    : there is at least one outlier

            var target = new GrubbTest(sample, GrubbTestHypothesis.TheMaximumIsAnOutlier);

            Assert.AreEqual(2.4687646112124519, target.Statistic);
            Assert.AreEqual(GrubbTestHypothesis.TheMaximumIsAnOutlier, target.Hypothesis);
            Assert.AreEqual(DistributionTail.OneUpper, target.Tail);
            Assert.AreEqual(2.031652001549944, target.CriticalValue);
            Assert.AreEqual(1.5013193443991213E-07, target.PValue);
            Assert.IsTrue(target.Significant);
        }

        [Test]
        public void grubb_test_change_size()
        {
            // https://github.com/accord-net/framework/issues/759

            double[] sample =
            {
                199.31, 199.53, 200.19, 200.82, 201.92, 201.95, 202.18, 245.57
            };

            // Null Hypothesis: there are no outliers in the data
            // Alternative    : there is at least one outlier

            var target = new GrubbTest(sample, GrubbTestHypothesis.TheMaximumIsAnOutlier)
            {
                Size = 1e-8
            };

            Assert.AreEqual(2.4687646112124519, target.Statistic);
            Assert.AreEqual(GrubbTestHypothesis.TheMaximumIsAnOutlier, target.Hypothesis);
            Assert.AreEqual(DistributionTail.OneUpper, target.Tail);
            Assert.AreEqual(2.4723982413556524, target.CriticalValue);
            Assert.AreEqual(1.5013193443991213E-07, target.PValue);
            Assert.IsFalse(target.Significant);
        }


        [Test]
        public void TTestConstructorTest2()
        {
            double[] sample =
            {
                0, 1, 3, 0, 2, 4, 2, 0, 4
            };

            var target = new GrubbTest(sample, GrubbTestHypothesis.ThereAreOutliers);
            Assert.AreEqual(1.3537948868448253, target.Statistic);
            Assert.AreEqual(GrubbTestHypothesis.ThereAreOutliers, target.Hypothesis);
            Assert.AreEqual(DistributionTail.OneUpper, target.Tail);
            Assert.AreEqual(2.1095617886142675, target.CriticalValue);
            Assert.AreEqual(0.733289986283244, target.PValue);
            Assert.IsFalse(target.Significant);


            target = new GrubbTest(sample, GrubbTestHypothesis.TheMinimumIsAnOutlier);
            Assert.AreEqual(1.0830359094758602, target.Statistic);
            Assert.AreEqual(GrubbTestHypothesis.TheMinimumIsAnOutlier, target.Hypothesis);
            Assert.AreEqual(DistributionTail.OneUpper, target.Tail);
            Assert.AreEqual(2.1095617886142675, target.CriticalValue);
            Assert.AreEqual(1.0, target.PValue);
            Assert.IsFalse(target.Significant);
        }

    }
}
