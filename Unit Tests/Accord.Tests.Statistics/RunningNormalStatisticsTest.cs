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
    using Accord.Statistics.Running;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Statistics;  
    
    [TestClass()]
    public class RunningNormalStatisticsTest
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
        public void ClearTest()
        {
            double[] values = { 0.5, -1.2, 0.7, 0.2, 1.1 };

            RunningNormalStatistics target = new RunningNormalStatistics();

            for (int i = 0; i < values.Length; i++)
                target.Push(values[i]);

            target.Clear();

            double[] values2 = { 1.5, -5.2, 0.7, 1.2, 9.1 };

            for (int i = 0; i < values.Length; i++)
                target.Push(values2[i]);

            Assert.AreEqual(Tools.Mean(values2), target.Mean, 1e-10);
            Assert.AreEqual(Tools.StandardDeviation(values2), target.StandardDeviation, 1e-10);
            Assert.AreEqual(Tools.Variance(values2), target.Variance, 1e-10);
        }

        [TestMethod()]
        public void MeanTest()
        {
            double[] values = { 0.5, -1.2, 0.7, 0.2, 1.1 };

            RunningNormalStatistics target = new RunningNormalStatistics();

            for (int i = 0; i < values.Length; i++)
                target.Push(values[i]);

            double expected = Tools.Mean(values);
            double actual = target.Mean;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void StandardDeviationTest()
        {
            double[] values = { 0.5, -1.2, 0.7, 0.2, 1.1 };

            RunningNormalStatistics target = new RunningNormalStatistics();

            for (int i = 0; i < values.Length; i++)
                target.Push(values[i]);

            double expected = Tools.StandardDeviation(values);
            double actual = target.StandardDeviation;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void VarianceTest()
        {
            double[] values = { 0.5, -1.2, 0.7, 0.2, 1.1 };

            RunningNormalStatistics target = new RunningNormalStatistics();

            for (int i = 0; i < values.Length; i++)
                target.Push(values[i]);

            double expected = Tools.Variance(values);
            double actual = target.Variance;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void RunningNormalStatisticsConstructorTest()
        {
            RunningNormalStatistics target = new RunningNormalStatistics();
            Assert.AreEqual(0, target.Mean);
            Assert.AreEqual(0, target.StandardDeviation);
            Assert.AreEqual(0, target.Variance);
        }

    }
}
