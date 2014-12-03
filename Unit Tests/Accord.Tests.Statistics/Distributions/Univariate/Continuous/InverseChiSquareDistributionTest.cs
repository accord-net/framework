// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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
    using Accord.Statistics.Distributions.Univariate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Controls;
    using Accord.Math;
    using Accord.Statistics.Testing;

    [TestClass()]
    public class InverseChiSquareDistributionTest
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
        public void ConstructorTest()
        {
            var invchisq = new InverseChiSquareDistribution(degreesOfFreedom: 7);

            double mean = invchisq.Mean;     // 0.2
            double median = invchisq.Median; // 6.345811068141737
            double var = invchisq.Variance;  // 75
            double mode = invchisq.Mode;     // 0.1111111111111111

            double cdf = invchisq.DistributionFunction(x: 6.27); // 0.50860033566176044
            double pdf = invchisq.ProbabilityDensityFunction(x: 6.27); // 0.0000063457380298844403
            double lpdf = invchisq.LogProbabilityDensityFunction(x: 6.27); // -11.967727146795536

            double ccdf = invchisq.ComplementaryDistributionFunction(x: 6.27); // 0.49139966433823956
            double icdf = invchisq.InverseDistributionFunction(p: cdf); // 6.2699998329362963

            double hf = invchisq.HazardFunction(x: 6.27); // 0.000012913598625327002
            double chf = invchisq.CumulativeHazardFunction(x: 6.27); // 0.71049750196765715

            string str = invchisq.ToString(); // "Inv-χ²(x; df = 7)"

            Assert.AreEqual(0.2, mean, 1e-10);
            Assert.AreEqual(6.345811068141737, median, 1e-6);
            Assert.AreEqual(75, var);
            Assert.AreEqual(0.1111111111111111, mode);
            Assert.AreEqual(0.71049750196765715, chf);
            Assert.AreEqual(0.50860033566176044, cdf);
            Assert.AreEqual(0.0000063457380298844403, pdf);
            Assert.AreEqual(-11.967727146795536, lpdf);
            Assert.AreEqual(0.000012913598625327002, hf);
            Assert.AreEqual(0.49139966433823956, ccdf);
            Assert.AreEqual(6.2699998329362963, icdf, 1e-6);
            Assert.AreEqual("Inv-χ²(x; df = 7)", str);

            var range1 = invchisq.GetRange(0.95);
            var range2 = invchisq.GetRange(0.99);
            var range3 = invchisq.GetRange(0.01);

            Assert.AreEqual(2.1673499092513264, range1.Min);
            Assert.AreEqual(14.067140449765922, range1.Max);
            Assert.AreEqual(1.2390421125300894, range2.Min);
            Assert.AreEqual(18.475307115523769, range2.Max);
            Assert.AreEqual(1.2390421125300894, range3.Min);
            Assert.AreEqual(18.475307115523773, range3.Max);
        }

        [TestMethod()]
        public void GetRangeTest()
        {
            var invchisq = new InverseChiSquareDistribution(degreesOfFreedom: 1);

            var range1 = invchisq.GetRange(0.95);
            var range2 = invchisq.GetRange(0.99);
            var range3 = invchisq.GetRange(0.01);

            Assert.AreEqual(0.0039321399872199164, range1.Min);
            Assert.AreEqual(3.8414588207219076, range1.Max);

            Assert.AreEqual(0.00015695842882115102, range2.Min);
            Assert.AreEqual(6.6348966014931827, range2.Max);

            Assert.AreEqual(0.00015695842882115102, range3.Min);
            Assert.AreEqual(6.6348966014931854, range3.Max);
        }

        [TestMethod()]
        public void GetRangeTest2()
        {
            var invchisq = new InverseChiSquareDistribution(degreesOfFreedom: 4200);

            var range1 = invchisq.GetRange(0.95);
            var range2 = invchisq.GetRange(0.99);
            var range3 = invchisq.GetRange(0.01);

            Assert.AreEqual(4050.3922378834536, range1.Min);
            Assert.AreEqual(4351.8817614873615, range1.Max);
            Assert.AreEqual(3989.7323067750908, range2.Min);
            Assert.AreEqual(4416.1499610177834, range2.Max);
            Assert.AreEqual(3989.7323067750908, range3.Min);
            Assert.AreEqual(4416.1499610177489, range3.Max);
        }

        [TestMethod()]
        public void MedianTest()
        {
            var target = new ChiSquareDistribution(degreesOfFreedom: 4);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }

        [TestMethod()]
        public void ProbabilityDensityFunctionTest()
        {
            int degreesOfFreedom;
            double actual, expected, x;
            ChiSquareDistribution target;

            degreesOfFreedom = 1;
            target = new ChiSquareDistribution(degreesOfFreedom);
            x = 1;
            actual = target.ProbabilityDensityFunction(x);
            expected = 0.2420;
            Assert.AreEqual(expected, actual, 1e-4);

            degreesOfFreedom = 2;
            target = new ChiSquareDistribution(degreesOfFreedom);
            x = 2;
            actual = target.ProbabilityDensityFunction(x);
            expected = 0.1839;
            Assert.AreEqual(expected, actual, 1e-4);

            degreesOfFreedom = 10;
            target = new ChiSquareDistribution(degreesOfFreedom);
            x = 2;
            actual = target.ProbabilityDensityFunction(x);
            expected = 0.0077;
            Assert.AreEqual(expected, actual, 1e-4);
        }

        [TestMethod()]
        public void LogProbabilityDensityFunctionTest()
        {
            int degreesOfFreedom;
            double actual, expected, x;
            ChiSquareDistribution target;

            degreesOfFreedom = 1;
            target = new ChiSquareDistribution(degreesOfFreedom);
            x = 1;
            actual = target.LogProbabilityDensityFunction(x);
            expected = System.Math.Log(target.ProbabilityDensityFunction(x));
            Assert.AreEqual(expected, actual, 1e-10);

            degreesOfFreedom = 2;
            target = new ChiSquareDistribution(degreesOfFreedom);
            x = 2;
            actual = target.LogProbabilityDensityFunction(x);
            expected = System.Math.Log(target.ProbabilityDensityFunction(x));
            Assert.AreEqual(expected, actual, 1e-10);

            degreesOfFreedom = 10;
            target = new ChiSquareDistribution(degreesOfFreedom);
            x = 2;
            actual = target.LogProbabilityDensityFunction(x);
            expected = System.Math.Log(target.ProbabilityDensityFunction(x));
            Assert.AreEqual(expected, actual, 1e-10);
        }

        [TestMethod()]
        public void DistributionFunctionTest()
        {
            int degreesOfFreedom;
            double actual, expected, x;
            ChiSquareDistribution target;

            degreesOfFreedom = 1;
            target = new ChiSquareDistribution(degreesOfFreedom);
            x = 5;
            actual = target.DistributionFunction(x);
            expected = 0.9747;
            Assert.AreEqual(expected, actual, 1e-4);


            degreesOfFreedom = 5;
            target = new ChiSquareDistribution(degreesOfFreedom);
            x = 5;
            actual = target.DistributionFunction(x);
            expected = 0.5841;
            Assert.AreEqual(expected, actual, 1e-4);
        }

        [TestMethod()]
        public void InverseTest()
        {
            double expected = 1.8307038053275149991e+01;
            double actual = ChiSquareDistribution.Inverse(0.95, 10);
            Assert.AreEqual(expected, actual, 1e-14);
        }

        [TestMethod()]
        public void InverseTest2()
        {
            double[] p = 
            {
                0.003898633, 0.956808760, 0.318487983,
                0.887227832, 0.641802182, 0.640345741,
                0.931996171, 0.426819547, 0.624824460,
                0.247553652, 0.282827901, 0.313780766,
                0.093206440, 0.392279489, 0.601228848
            };

            double[] expected = 
            {
                 2.3875256301085814295e-05, 4.0879013123718950240e+00, 1.6842875232305037914e-01,
                 2.5149366098649084122e+00, 8.4420178057142991612e-01, 8.3910013681477579883e-01,
                 3.3305572850409235208e+00, 3.1738990781989129264e-01, 7.8645062825981804089e-01,
                 9.9486814436765019787e-02, 1.3121839127554768556e-01, 1.6320884169279117892e-01,
                 1.3708641978779382772e-02, 2.6350767581793288485e-01, 7.1202870676363294589e-01,

            };


            double[] actual = new double[p.Length];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = ChiSquareDistribution.Inverse(p[i], 1);

            Assert.IsTrue(expected.IsEqual(actual, 1e-14));
        }


    }
}
