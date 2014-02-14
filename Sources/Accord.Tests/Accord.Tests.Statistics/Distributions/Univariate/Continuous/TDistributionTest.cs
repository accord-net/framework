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
    using System;
    using Accord.Statistics.Distributions.Fitting;
    using System.Globalization;

    [TestClass()]
    public class TDistributionTest
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
            var t = new TDistribution(degreesOfFreedom: 4.2);

            double mean = t.Mean;     // 0.0
            double median = t.Median; // 0.0
            double var = t.Variance;  // 1.9090909090909089

            double cdf = t.DistributionFunction(x: 1.4); // 0.88456136730659074
            double pdf = t.ProbabilityDensityFunction(x: 1.4); // 0.13894002185341031
            double lpdf = t.LogProbabilityDensityFunction(x: 1.4); // -1.9737129364307417

            double ccdf = t.ComplementaryDistributionFunction(x: 1.4); // 0.11543863269340926
            double icdf = t.InverseDistributionFunction(p: cdf); // 1.4000000000000012

            double hf = t.HazardFunction(x: 1.4); // 1.2035833984833988
            double chf = t.CumulativeHazardFunction(x: 1.4); // 2.1590162088918525

            string str = t.ToString(CultureInfo.InvariantCulture); // T(x; df = 4.2)

            Assert.AreEqual(0.0, mean);
            Assert.AreEqual(0.0, median);
            Assert.AreEqual(1.9090909090909089, var);
            Assert.AreEqual(2.1590162088918525, chf);
            Assert.AreEqual(0.88456136730659074, cdf);
            Assert.AreEqual(0.13894002185341031, pdf);
            Assert.AreEqual(-1.9737129364307417, lpdf);
            Assert.AreEqual(1.2035833984833988, hf);
            Assert.AreEqual(0.11543863269340926, ccdf);
            Assert.AreEqual(1.4000000000000012, icdf);
            Assert.AreEqual("T(x; df = 4.2)", str);
        }

        [TestMethod()]
        public void VarianceTest()
        {
            TDistribution target = new TDistribution(3);
            double actual = target.Variance;
            double expected = 3;
            Assert.AreEqual(expected, actual);

            target = new TDistribution(2);
            actual = target.Variance;
            expected = Double.PositiveInfinity;
            Assert.AreEqual(expected, actual);

            target = new TDistribution(1);
            actual = target.Variance;
            Assert.IsTrue(Double.IsNaN(actual));
        }

        [TestMethod()]
        public void MeanTest()
        {
            TDistribution target;
            double actual;

            target = new TDistribution(1);
            actual = target.Mean;
            Assert.IsTrue(Double.IsNaN(actual));

            target = new TDistribution(2);
            actual = target.Mean;
            double expected = 0;
            Assert.AreEqual(expected, actual);

            target = new TDistribution(3);
            actual = target.Mean;
            expected = 0;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ProbabilityDensityFunctionTest()
        {
            TDistribution target = new TDistribution(1);
            double expected = 0.31830988618379075;
            double actual = target.ProbabilityDensityFunction(0);
            Assert.AreEqual(expected, actual);

            expected = 0.017076710632177614;
            actual = target.ProbabilityDensityFunction(4.2);
            Assert.AreEqual(expected, actual);

            target = new TDistribution(2);
            expected = 0.35355339059327379;
            actual = target.ProbabilityDensityFunction(0);
            Assert.AreEqual(expected, actual);

            expected = 0.011489146700777093;
            actual = target.ProbabilityDensityFunction(4.2);
            Assert.AreEqual(expected, actual);

            target = new TDistribution(3);
            expected = 0.36755259694786141;
            actual = target.ProbabilityDensityFunction(0);
            Assert.AreEqual(expected, actual);

            expected = 0.0077650207237835792;
            actual = target.ProbabilityDensityFunction(4.2);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void InverseDistributionFunctionTest()
        {
            TDistribution target;
            double[] expected;

            target = new TDistribution(1);
            expected = new double[] { 6.3138, 3.0777, 1.9626, 1.3764, 1, 0.7265, 0.5095, 0.3249, 0.1584, 0 };

            for (int i = 1; i <= 10; i++)
            {
                double percent = i / 10.0;
                double actual = target.InverseDistributionFunction(1.0 - percent / 2);
                Assert.AreEqual(expected[i - 1], actual, 1e-4);
                Assert.IsFalse(Double.IsNaN(actual));
            }

            target = new TDistribution(4.2);
            expected = new double[] { 2.103, 1.5192, 1.1814, 0.9358, 0.7373, 0.5664, 0.4127, 0.2699, 0.1334, 0 };

            for (int i = 1; i <= 10; i++)
            {
                double percent = i / 10.0;
                double actual = target.InverseDistributionFunction(1.0 - percent / 2);
                Assert.AreEqual(expected[i - 1], actual, 1e-4);
                Assert.IsFalse(Double.IsNaN(actual));
            }

        }

        [TestMethod()]
        public void InverseDistributionFunctionTest2()
        {
            TDistribution target = new TDistribution(24);

            double expected = 1.710882023;
            double actual = target.InverseDistributionFunction(0.95);

            Assert.AreEqual(expected, actual, 1e-06);
        }

        [TestMethod()]
        public void InverseDistributionFunctionLeftTailTest()
        {
            double[] a = { 0.1, 0.05, 0.025, 0.01, 0.005, 0.001, 0.0005 };

            double[,] expected =
            {
                { 1, 3.078, 6.314, 12.706, 31.821, 63.656, 318.289, 636.578 },
                { 2, 1.886, 2.920, 4.303, 6.965, 9.925, 22.328, 31.600 },
                { 3, 1.638, 2.353, 3.182, 4.541, 5.841, 10.214, 12.924 },
                { 4, 1.533, 2.132, 2.776, 3.747, 4.604, 7.173, 8.610 },
                { 5, 1.476, 2.015, 2.571, 3.365, 4.032, 5.894, 6.869 },
                { 6, 1.440, 1.943, 2.447, 3.143, 3.707, 5.208, 5.959 },
                { 7, 1.415, 1.895, 2.365, 2.998, 3.499, 4.785, 5.408 },
                { 8, 1.397, 1.860, 2.306, 2.896, 3.355, 4.501, 5.041 },
                { 9, 1.383, 1.833, 2.262, 2.821, 3.250, 4.297, 4.781 },
                { 10, 1.372, 1.812, 2.228, 2.764, 3.169, 4.144, 4.587 },
                { 11, 1.363, 1.796, 2.201, 2.718, 3.106, 4.025, 4.437 },
                { 12, 1.356, 1.782, 2.179, 2.681, 3.055, 3.930, 4.318 },
                { 13, 1.350, 1.771, 2.160, 2.650, 3.012, 3.852, 4.221 },
                { 14, 1.345, 1.761, 2.145, 2.624, 2.977, 3.787, 4.140 },
                { 15, 1.341, 1.753, 2.131, 2.602, 2.947, 3.733, 4.073 },
                { 16, 1.337, 1.746, 2.120, 2.583, 2.921, 3.686, 4.015 },
                { 17, 1.333, 1.740, 2.110, 2.567, 2.898, 3.646, 3.965 },
                { 18, 1.330, 1.734, 2.101, 2.552, 2.878, 3.610, 3.922 },
                { 19, 1.328, 1.729, 2.093, 2.539, 2.861, 3.579, 3.883 },
                { 20, 1.325, 1.725, 2.086, 2.528, 2.845, 3.552, 3.850 },
                { 21, 1.323, 1.721, 2.080, 2.518, 2.831, 3.527, 3.819 },
                { 22, 1.321, 1.717, 2.074, 2.508, 2.819, 3.505, 3.792 },
                { 23, 1.319, 1.714, 2.069, 2.500, 2.807, 3.485, 3.768 },
                { 24, 1.318, 1.711, 2.064, 2.492, 2.797, 3.467, 3.745 },
                { 25, 1.316, 1.708, 2.060, 2.485, 2.787, 3.450, 3.725 },
                { 26, 1.315, 1.706, 2.056, 2.479, 2.779, 3.435, 3.707 },
                { 27, 1.314, 1.703, 2.052, 2.473, 2.771, 3.421, 3.689 },
                { 28, 1.313, 1.701, 2.048, 2.467, 2.763, 3.408, 3.674 },
                { 29, 1.311, 1.699, 2.045, 2.462, 2.756, 3.396, 3.660 },
                { 30, 1.310, 1.697, 2.042, 2.457, 2.750, 3.385, 3.646 },
                { 60, 1.296, 1.671, 2.000, 2.390, 2.660, 3.232, 3.460 },
                { 120, 1.289, 1.658, 1.980, 2.358, 2.617, 3.160, 3.373 },
            };

            for (int i = 0; i < expected.GetLength(0); i++)
            {
                int df = (int)expected[i, 0];

                TDistribution target = new TDistribution(df);

                for (int j = 1; j < expected.GetLength(1); j++)
                {
                    double actual = target.InverseDistributionFunction(1.0 - a[j - 1]);
                    Assert.IsTrue(Math.Abs(expected[i, j] / actual - 1) < 1e-3);
                }
            }
        }


        [TestMethod()]
        public void LogProbabilityDensityFunctionTest()
        {
            TDistribution target = new TDistribution(1);
            double expected = System.Math.Log(0.31830988618379075);
            double actual = target.LogProbabilityDensityFunction(0);
            Assert.AreEqual(expected, actual);

            expected = System.Math.Log(0.017076710632177614);
            actual = target.LogProbabilityDensityFunction(4.2);
            Assert.AreEqual(expected, actual, 1e-6);

            target = new TDistribution(2);
            expected = System.Math.Log(0.35355339059327379);
            actual = target.LogProbabilityDensityFunction(0);
            Assert.AreEqual(expected, actual, 1e-6);

            expected = System.Math.Log(0.011489146700777093);
            actual = target.LogProbabilityDensityFunction(4.2);
            Assert.AreEqual(expected, actual, 1e-6);

            target = new TDistribution(3);
            expected = System.Math.Log(0.36755259694786141);
            actual = target.LogProbabilityDensityFunction(0);
            Assert.AreEqual(expected, actual, 1e-6);

            expected = System.Math.Log(0.0077650207237835792);
            actual = target.LogProbabilityDensityFunction(4.2);
            Assert.AreEqual(expected, actual, 1e-6);
        }

        [TestMethod()]
        public void FitTest()
        {
            bool thrown = false;
            TDistribution target = new TDistribution(1);
            try { target.Fit(null, null, null); }
            catch (NotSupportedException) { thrown = true; }
            Assert.IsTrue(thrown);
        }

        [TestMethod()]
        public void DistributionFunctionTest()
        {
            TDistribution target = new TDistribution(1);
            double expected = 0.5;
            double actual = target.DistributionFunction(0);
            Assert.IsFalse(Double.IsNaN(actual));
            Assert.AreEqual(expected, actual, 1e-15);

            expected = 0.92559723470138278;
            actual = target.DistributionFunction(4.2);
            Assert.AreEqual(expected, actual);

            target = new TDistribution(2);
            expected = 0.5;
            actual = target.DistributionFunction(0);
            Assert.AreEqual(expected, actual);

            expected = 0.97385836652685043;
            actual = target.DistributionFunction(4.2);
            Assert.AreEqual(expected, actual);

            target = new TDistribution(3);
            expected = 0.5;
            actual = target.DistributionFunction(0);
            Assert.IsFalse(Double.IsNaN(actual));
            Assert.AreEqual(expected, actual, 1e-15);

            expected = 0.98768396091153043;
            actual = target.DistributionFunction(4.2);
            Assert.AreEqual(expected, actual);

            expected = 0.16324737815131229;
            actual = target.DistributionFunction(-1.17);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CloneTest()
        {
            int degreesOfFreedom = 5;
            TDistribution target = new TDistribution(degreesOfFreedom);
            TDistribution clone = (TDistribution)target.Clone();

            Assert.AreNotSame(target, clone);
            Assert.AreEqual(target.DegreesOfFreedom, clone.DegreesOfFreedom);
            Assert.AreEqual(target.Mean, clone.Mean);
            Assert.AreEqual(target.Variance, clone.Variance);
        }

        [TestMethod()]
        public void TDistributionConstructorTest()
        {
            int degreesOfFreedom = 4;
            TDistribution target = new TDistribution(degreesOfFreedom);
            Assert.AreEqual(degreesOfFreedom, target.DegreesOfFreedom);

            bool thrown = false;
            try { target = new TDistribution(0); }
            catch (ArgumentOutOfRangeException) { thrown = true; }
            Assert.IsTrue(thrown);

            thrown = false;
            try { target = new TDistribution(-1); }
            catch (ArgumentOutOfRangeException) { thrown = true; }
            Assert.IsTrue(thrown);
        }

        [TestMethod()]
        public void MedianTest()
        {
            TDistribution target = new TDistribution(7.6);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }
    }
}
