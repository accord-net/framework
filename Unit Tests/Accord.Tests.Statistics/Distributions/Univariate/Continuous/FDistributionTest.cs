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
    using Accord.Statistics.Distributions.Univariate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Globalization;

    [TestClass()]
    public class FDistributionTest
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
            var F = new FDistribution(degrees1: 8, degrees2: 5);

            double mean = F.Mean;     // 1.6666666666666667
            double median = F.Median; // 1.0545096252132447
            double var = F.Variance;  // 7.6388888888888893

            double cdf = F.DistributionFunction(x: 0.27); // 0.049463408057268315
            double pdf = F.ProbabilityDensityFunction(x: 0.27); // 0.45120469723580559
            double lpdf = F.LogProbabilityDensityFunction(x: 0.27); // -0.79583416831212883

            double ccdf = F.ComplementaryDistributionFunction(x: 0.27); // 0.95053659194273166
            double icdf = F.InverseDistributionFunction(p: cdf); // 0.27

            double hf = F.HazardFunction(x: 0.27); // 0.47468419528555084
            double chf = F.CumulativeHazardFunction(x: 0.27); // 0.050728620222091653

            string str = F.ToString(CultureInfo.InvariantCulture); // F(x; df1 = 8, df2 = 5)

            Assert.AreEqual(1.6666666666666667, mean);
            Assert.AreEqual(1.0545096252132447, median);
            Assert.AreEqual(7.6388888888888893, var);
            Assert.AreEqual(0.050728620222091653, chf);
            Assert.AreEqual(0.049463408057268315, cdf);
            Assert.AreEqual(0.45120469723580559, pdf);
            Assert.AreEqual(-0.79583416831212883, lpdf);
            Assert.AreEqual(0.47468419528555084, hf);
            Assert.AreEqual(0.95053659194273166, ccdf);
            Assert.AreEqual(0.27, icdf);
            Assert.AreEqual("F(x; df1 = 8, df2 = 5)", str);
        }

        [TestMethod()]
        public void MeanVarianceTest()
        {
            int[] d = { 1, 2, 3, 4, 5 };
            double[] mean = { double.NaN, double.NaN, 3.0000, 2.0000, 1.6667 };
            double[] var = { double.NaN, double.NaN, double.NaN, double.NaN, 8.8889 };

            for (int i = 0; i < 5; i++)
            {
                FDistribution f = new FDistribution(d[i], d[i]);
                if (double.IsNaN(mean[i]))
                    Assert.IsTrue(double.IsNaN(f.Mean));
                else Assert.AreEqual(mean[i], f.Mean, 1e-4);


                if (double.IsNaN(var[i]))
                    Assert.IsTrue(double.IsNaN(f.Variance));
                else Assert.AreEqual(var[i], f.Variance, 1e-4);
            }
        }

        [TestMethod()]
        public void DistributionFunctionTest()
        {
            FDistribution f = new FDistribution(2, 3);

            Assert.AreEqual(f.DegreesOfFreedom1, 2);
            Assert.AreEqual(f.DegreesOfFreedom2, 3);

            double expected = 0.350480947161671;
            double actual = f.DistributionFunction(0.5);

            Assert.AreEqual(expected, actual, 1e-6);
        }

        [TestMethod()]
        public void MedianTest()
        {
            FDistribution target = new FDistribution(2, 3);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }

        [TestMethod()]
        public void DistributionFunctionTest2()
        {
            double actual;
            double expected;

            int[] nu1 = { 1, 2, 3, 4, 5 };
            int[] nu2 = { 6, 7, 8, 9, 10 };
            double[] x = { 2, 3, 4, 5, 6 };
            double[] cdf = { 0.7930, 0.8854, 0.9481, 0.9788, 0.9919 };
            FDistribution f;

            for (int i = 0; i < 5; i++)
            {
                f = new FDistribution(nu1[i], nu2[i]);
                expected = cdf[i];

                actual = f.DistributionFunction(x[i]);
                Assert.AreEqual(expected, actual, 1e-4);

                f = new FDistribution(nu2[i], nu1[i]);
                actual = 1 - f.DistributionFunction(1.0 / x[i]);
                Assert.AreEqual(expected, actual, 1e-4);
            }
        }

        [TestMethod()]
        public void ComplementaryDistributionFunctionTest()
        {
            double actual;
            double expected;

            int[] nu1 = { 1, 2, 3, 4, 5 };
            int[] nu2 = { 6, 7, 8, 9, 10 };
            double[] x = { 2, 3, 4, 5, 6 };
            double[] cdf = { 0.7930, 0.8854, 0.9481, 0.9788, 0.9919 };
            FDistribution f;

            for (int i = 0; i < 5; i++)
            {
                f = new FDistribution(nu1[i], nu2[i]);
                expected = cdf[i];

                actual = f.DistributionFunction(x[i]);
                Assert.AreEqual(expected, actual, 1e-4);

                f = new FDistribution(nu2[i], nu1[i]);
                actual = f.ComplementaryDistributionFunction(1.0 / x[i]);
                Assert.AreEqual(expected, actual, 1e-4);
            }
        }

        [TestMethod()]
        public void DistributionFunctionTest3()
        {

            double[] cdf = 
            {
                0, 0.0277778, 0.0816327, 0.140625, 0.197531, 0.25,
                0.297521, 0.340278, 0.378698, 0.413265, 0.444444
            };

            FDistribution target = new FDistribution(4, 2);

            for (int i = 0; i < 11; i++)
            {
                double x = i / 10.0;
                double actual = target.DistributionFunction(x);
                double expected = cdf[i];

                Assert.AreEqual(expected, actual, 1e-5);
                Assert.IsFalse(double.IsNaN(actual));
            }
        }

        [TestMethod()]
        public void ProbabilityDistributionFunctionTest()
        {
            FDistribution f = new FDistribution(2, 3);

            double expected = 0.487139289628747;
            double actual = f.ProbabilityDensityFunction(0.5);

            Assert.AreEqual(expected, actual, 1e-6);
        }

        [TestMethod()]
        public void LogProbabilityDistributionFunctionTest()
        {
            FDistribution f = new FDistribution(2, 3);

            double expected = System.Math.Log(0.487139289628747);
            double actual = f.LogProbabilityDensityFunction(0.5);

            Assert.AreEqual(expected, actual, 1e-6);
        }



        [TestMethod()]
        public void ProbabilityDistributionFunctionTest2()
        {
            FDistribution f = new FDistribution(2, 2);
            Assert.AreEqual(f.ProbabilityDensityFunction(1), 0.2500, 1e-4);
            Assert.AreEqual(f.ProbabilityDensityFunction(2), 0.1111, 1e-4);
            Assert.AreEqual(f.ProbabilityDensityFunction(3), 0.0625, 1e-4);
            Assert.AreEqual(f.ProbabilityDensityFunction(4), 0.0400, 1e-4);
            Assert.AreEqual(f.ProbabilityDensityFunction(5), 0.0278, 1e-4);
            Assert.AreEqual(f.ProbabilityDensityFunction(6), 0.0204, 1e-4);

            Assert.AreEqual(new FDistribution(5, 5).ProbabilityDensityFunction(3), 0.0689, 1e-4);
            Assert.AreEqual(new FDistribution(6, 6).ProbabilityDensityFunction(3), 0.0659, 1e-4);
            Assert.AreEqual(new FDistribution(7, 7).ProbabilityDensityFunction(3), 0.0620, 1e-4);
            Assert.AreEqual(new FDistribution(8, 8).ProbabilityDensityFunction(3), 0.0577, 1e-4);
            Assert.AreEqual(new FDistribution(9, 9).ProbabilityDensityFunction(3), 0.0532, 1e-4);
            Assert.AreEqual(new FDistribution(10, 10).ProbabilityDensityFunction(3), 0.0487, 1e-4);
        }

        [TestMethod()]
        public void LogProbabilityDistributionFunctionTest2()
        {
            FDistribution f = new FDistribution(2, 2);
            double actual;
            double expected;
            double x;

            for (int i = 1; i <= 6; i++)
            {
                x = i;
                actual = f.LogProbabilityDensityFunction(x);
                expected = System.Math.Log(f.ProbabilityDensityFunction(x));
                Assert.AreEqual(expected, actual, 1e-10);
            }

            for (int i = 5; i <= 10; i++)
            {
                f = new FDistribution(i, i);
                x = 3;
                actual = f.LogProbabilityDensityFunction(x);
                expected = System.Math.Log(f.ProbabilityDensityFunction(x));
                Assert.AreEqual(expected, actual, 1e-10);
            }
        }

        [TestMethod()]
        public void InverseDistributionFunctionTest()
        {

            double[] cdf = 
            {
                0.231238, 0.404508, 0.605516, 0.86038, 1.20711, 1.71825, 2.5611, 4.23607, 9.24342
            };

            FDistribution target = new FDistribution(4, 2);

            for (int i = 0; i < cdf.Length; i++)
            {
                double x = (i + 1) / 10.0;
                double actual = target.InverseDistributionFunction(x);
                double expected = cdf[i];

                Assert.AreEqual(expected, actual, 1e-5);
                Assert.IsFalse(double.IsNaN(actual));
            }
        }


    }
}
