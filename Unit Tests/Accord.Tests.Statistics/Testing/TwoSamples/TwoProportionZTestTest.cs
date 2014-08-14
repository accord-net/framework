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
    using AForge;
    using Accord.Statistics.Testing.Power;

    [TestClass()]
    public class TwoProportionZTestTest
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
        public void TwoProportionZTestConstructorTest()
        {
            // Example from http://stattrek.com/hypothesis-test/difference-in-proportions.aspx

            int sampleSize1 = 100;
            int sampleSize2 = 200;
            double sample1 = 0.38;
            double sample2 = 0.51;

            TwoProportionZTest target = new TwoProportionZTest(
                sample1, sampleSize1,
                sample2, sampleSize2,
                TwoSampleHypothesis.ValuesAreDifferent);

            Assert.AreEqual(-2.13, target.Statistic, 5e-3);
            Assert.AreEqual(0.034, target.PValue, 1e-3);
            Assert.IsTrue(target.Significant);
        }

    }
}
