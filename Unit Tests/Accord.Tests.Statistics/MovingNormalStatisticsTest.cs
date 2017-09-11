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
    using Accord.Statistics.Moving;
    using NUnit.Framework;
    using Accord.Statistics;
    using System;

    [TestFixture]
    public class MovingNormalStatisticsTest
    {

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

        [Test]
        public void doc_example()
        {
            #region doc_example
            // Moving Normal Statistics can be used to keep track of the mean, variance and standard 
            // deviation of the last N samples that have been fed to it. It is possible to feed one 
            // value at a time, and the class will keep track of the mean and variance of the samples 
            // without registering the samples themselves. 

            // An example where this class can be useful is when trying to filter the (x,y) trajectories 
            // of a movement tracker (i.e. to show the average point on the past 10 frames) and monitor 
            // the quality of the tracking (i.e. if the variance becomes too high, tracking might have been lost).

            // Fix seed for reproducible results
            Accord.Math.Random.Generator.Seed = 0;

            Random rnd = Accord.Math.Random.Generator.Random;

            // Create a estimator that will keep the mean and standard deviation of the past 5 samples:
            var normal = new MovingNormalStatistics(windowSize: 5);

            // Read 50 samples
            for (int i = 0; i < 50; i++)
                normal.Push(rnd.Next(1, 10));

            // The properties of the 5 last samples are:
            double sum = normal.Sum; // 21
            double mean = normal.Mean; // 4.2
            double stdDev = normal.StandardDeviation; // 2.4899799195977463
            double var = normal.Variance; // 6.2
            #endregion

            Assert.AreEqual(21, sum, 1e-10);
            Assert.AreEqual(4.2, mean, 1e-10);
            Assert.AreEqual(2.4899799195977463, stdDev, 1e-10);
            Assert.AreEqual(6.2, var, 1e-10);
        }
    }
}
