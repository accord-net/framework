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

    using Accord.Statistics.Testing.Power;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Statistics.Testing;
    using Accord.Statistics;
    using Accord.Math;

    [TestClass()]
    public class ZTestPowerAnalysisTest
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
        public void ZTestPowerAnalysisConstructorTest1()
        {
            ZTestPowerAnalysis target;
            double actual, expected;

            target = new ZTestPowerAnalysis(OneSampleHypothesis.ValueIsDifferentFromHypothesis)
            {
                Effect = 0.2,
                Samples = 60,
                Size = 0.10,
            };

            target.ComputePower();

            expected = 0.4618951;
            actual = target.Power;
            Assert.AreEqual(expected, actual, 1e-5);


            target = new ZTestPowerAnalysis(OneSampleHypothesis.ValueIsSmallerThanHypothesis)
            {
                Effect = 0.2,
                Samples = 60,
                Size = 0.10,
            };

            target.ComputePower();

            expected = 0.00232198;
            actual = target.Power;
            Assert.AreEqual(expected, actual, 1e-5);


            target = new ZTestPowerAnalysis(OneSampleHypothesis.ValueIsGreaterThanHypothesis)
            {
                Effect = 0.2,
                Samples = 60,
                Size = 0.10,
            };

            target.ComputePower();

            expected = 0.6055124;
            actual = target.Power;
            Assert.AreEqual(expected, actual, 1e-5);
        }
    }
}
