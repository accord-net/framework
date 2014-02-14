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
    using Accord.Statistics.Distributions.Multivariate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Statistics.Distributions.Fitting;

    [TestClass()]
    public class JointDistributionTest
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
        public void JointDistributionConstructorTest()
        {
            int[] symbols = { 3, 5 };
            JointDistribution target = new JointDistribution(symbols);

            double[] p = target.Frequencies;

            Assert.AreEqual(3 * 5, p.Length);

            for (int i = 0; i < p.Length; i++)
                p[i] = i;

            double actual;

            actual = target.ProbabilityMassFunction(new int[] { 0, 0 });
            Assert.AreEqual(0, actual);

            actual = target.ProbabilityMassFunction(new int[] { 0, 1 });
            Assert.AreEqual(1, actual);

            actual = target.ProbabilityMassFunction(new int[] { 1, 0 });
            Assert.AreEqual(5, actual);

            actual = target.ProbabilityMassFunction(new int[] { 2, 4 });
            Assert.AreEqual(14, actual);
        }

        [TestMethod()]
        public void JointDistributionConstructorTest2()
        {
            int[] symbols = { 3, 5, 2 };
            JointDistribution target = new JointDistribution(symbols);

            double[] p = target.Frequencies;

            Assert.AreEqual(3 * 5 * 2, p.Length);

            for (int i = 0; i < p.Length; i++)
                p[i] = i;

            double actual;

            actual = target.ProbabilityMassFunction(new int[] { 0, 0, 0 });
            Assert.AreEqual(0, actual);

            actual = target.ProbabilityMassFunction(new int[] { 0, 0, 1 });
            Assert.AreEqual(1, actual);

            actual = target.ProbabilityMassFunction(new int[] { 0, 1, 0 });
            Assert.AreEqual(2, actual);

            actual = target.ProbabilityMassFunction(new int[] { 0, 1, 1 });
            Assert.AreEqual(3, actual);

            actual = target.ProbabilityMassFunction(new int[] { 0, 2, 0 });
            Assert.AreEqual(4, actual);

            actual = target.ProbabilityMassFunction(new int[] { 0, 2, 1 });
            Assert.AreEqual(5, actual);

            actual = target.ProbabilityMassFunction(new int[] { 2, 4, 1 });
            Assert.AreEqual(29, actual);
        }

        [TestMethod()]
        public void FitTest()
        {
            int[] symbols = { 3, 5 };
            JointDistribution target = new JointDistribution(symbols);

            double[][] observations = 
            {
                new double[] { 0, 0 },
                new double[] { 1, 1 },
                new double[] { 2, 1 },
                new double[] { 0, 0 },
            };

            target.Fit(observations);

            double[] p = target.Frequencies;

            double actual;

            actual = target.ProbabilityMassFunction(new[] { 0, 0 });
            Assert.AreEqual(0.5, actual);

            actual = target.ProbabilityMassFunction(new[] { 1, 1 });
            Assert.AreEqual(0.25, actual);

            actual = target.ProbabilityMassFunction(new[] { 2, 1 });
            Assert.AreEqual(0.25, actual);
        }

        [TestMethod()]
        public void FitTest2()
        {
            int[] symbols = { 3, 5 };
            JointDistribution target = new JointDistribution(symbols);

            double[][] observations = 
            {
                new double[] { 0, 0 },
                new double[] { 1, 1 },
                new double[] { 2, 1 },
            };

            double[] weights = { 2, 1, 1 };

            target.Fit(observations, weights);

            double[] p = target.Frequencies;

            double actual;

            actual = target.ProbabilityMassFunction(new[] { 0, 0 });
            Assert.AreEqual(0.5, actual);

            actual = target.ProbabilityMassFunction(new[] { 1, 1 });
            Assert.AreEqual(0.25, actual);

            actual = target.ProbabilityMassFunction(new[] { 2, 1 });
            Assert.AreEqual(0.25, actual);

        }

    }
}
