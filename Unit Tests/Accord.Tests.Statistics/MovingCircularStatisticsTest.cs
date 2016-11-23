// Accord Unit Tests
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
    using Math;

    [TestFixture]
    public class MovingCircularStatisticsTest
    {

        [Test]
        public void PushTest()
        {
            double[] values = { 0.24, 1.61, 2.22, 5.82 };

            int windowSize = values.Length;

            var target = new MovingCircularStatistics(windowSize);

            for (int i = 0; i < values.Length; i++)
                target.Push(values[i]);

            double actualMean = target.Mean;
            double expectedMean = Circular.Mean(values);
            Assert.AreEqual(expectedMean, actualMean);

            double actualVariance = target.Variance;
            double expectedVariance = Circular.Variance(values);
            Assert.AreEqual(expectedVariance, actualVariance);
        }

        [Test]
        public void PushTest2()
        {
            double[] values = { 0.24, 1.61, 2.22, 5.82 };

            int windowSize = values.Length;

            var target = new MovingCircularStatistics(windowSize);

            target.Push(0.29);
            target.Push(1.11);

            for (int i = 0; i < values.Length; i++)
                target.Push(values[i]);

            double actualMean = target.Mean;
            double expectedMean = Circular.Mean(values);
            Assert.AreEqual(expectedMean, actualMean, 1e-10);

            double actualVariance = target.Variance;
            double expectedVariance = Circular.Variance(values);
            Assert.AreEqual(expectedVariance, actualVariance, 1e-10);
        }

        [Test]
        public void constant_value_test()
        {
            double[] values = { 0.24, 0.24, 0.24, 0.24 };

            int windowSize = values.Length;

            var target = new MovingCircularStatistics(windowSize);

            for (int i = 0; i < values.Length; i++)
                target.Push(values[i]);

            double actualMean = target.Mean;
            double expectedMean = Circular.Mean(values);
            Assert.AreEqual(expectedMean, actualMean, 1e-10);

            double actualVariance = target.Variance;
            double expectedVariance = Circular.Variance(values);
            Assert.AreEqual(expectedVariance, actualVariance, 1e-10);
        }

        [Test]
        public void circular_test()
        {
            double[] values = Vector.Range(2.0, 5.0, 0.1);

            int windowSize = values.Length;

            var target = new MovingCircularStatistics(windowSize);

            for (int i = 0; i < values.Length; i++)
                target.Push(values[i]);

            double actualMean = target.Mean;
            double expectedMean = Circular.Mean(values);
            double wrongMean = values.Mean();
            Assert.AreEqual(actualMean, -2.8328292495480087, 1e-10);
            Assert.AreEqual(wrongMean, 3.4533333333333331, 1e-10);
            Assert.AreEqual(expectedMean, actualMean, 1e-10);

            double actualVariance = target.Variance;
            double expectedVariance = Circular.Variance(values);
            double wrongVar = values.Variance();
            Assert.AreEqual(actualVariance, 0.33804973557846185);
            Assert.AreEqual(wrongVar, 0.78533333333333344);
            Assert.AreEqual(expectedVariance, actualVariance);

            double actualStdDev = target.StandardDeviation;
            double expectedStdDev = Circular.StandardDeviation(values);
            double wrongStdDev = values.StandardDeviation();
            Assert.AreEqual(actualStdDev, 0.90836650658209);
            Assert.AreEqual(wrongStdDev, 0.88619034825105913);
            Assert.AreEqual(expectedStdDev, actualStdDev);
        }

    }
}
