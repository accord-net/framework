﻿// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    using Accord.Statistics.Moving;
    using NUnit.Framework;
    using Accord.Statistics;    
    
    [TestFixture]
    public class MovingNormalStatisticsTest
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
        public void PushTest()
        {
            double[] values = { 0.24, 1.61, 2.22, 5.82 };

            int windowSize = values.Length;

            MovingNormalStatistics target = new MovingNormalStatistics(windowSize);

            for (int i = 0; i < values.Length; i++)
                target.Push(values[i]);

            double actualMean = target.Mean;
            double expectedMean = values.Mean();
            Assert.AreEqual(expectedMean, actualMean);

            double actualVariance = target.Variance;
            double expectedVariance = values.Variance();
            Assert.AreEqual(expectedVariance, actualVariance);
        }

        [Test]
        public void PushTest2()
        {
            double[] values = { 0.24, 1.61, 2.22, 5.82 };

            int windowSize = values.Length;

            MovingNormalStatistics target = new MovingNormalStatistics(windowSize);

            target.Push(0.29);
            target.Push(1.11);

            for (int i = 0; i < values.Length; i++)
                target.Push(values[i]);

            double actualMean = target.Mean;
            double expectedMean = values.Mean();
            Assert.AreEqual(expectedMean, actualMean);

            double actualVariance = target.Variance;
            double expectedVariance = values.Variance();
            Assert.AreEqual(expectedVariance, actualVariance);
        }

    }
}
