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
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;
    using Accord.Math;
    using Accord.Statistics.Testing;

    [TestFixture]
    public class InverseChiSquareDistributionTest
    {


        [Test]
        public void ConstructorTest()
        {
            #region doc_ctor
            // Create a new Inverse Chi-Square distribution with 7 d. of freedom.
            var invchisq = new InverseChiSquareDistribution(degreesOfFreedom: 7);

            double mean = invchisq.Mean;     // 0.2
            double median = invchisq.Median; // 0.1575844721504685
            double var = invchisq.Variance;  // 75
            double mode = invchisq.Mode;     // 0.1111111111111111

            double cdf = invchisq.DistributionFunction(x: 6.27); // 0.99998842765450235
            double pdf = invchisq.ProbabilityDensityFunction(x: 6.27); // 0.0000063457380298844403
            double lpdf = invchisq.LogProbabilityDensityFunction(x: 6.27); // -11.967727146795536

            double ccdf = invchisq.ComplementaryDistributionFunction(x: 6.27); // 1.1572345497645742E-05
            double icdf = invchisq.InverseDistributionFunction(p: cdf); // 6.2700000113440124

            double hf = invchisq.HazardFunction(x: 6.27); // 0.54835366185492873
            double chf = invchisq.CumulativeHazardFunction(x: 6.27); // 11.366892314949228

            string str = invchisq.ToString(); // "Inv-χ²(x; df = 7)"
            #endregion

            Assert.AreEqual(0.2, mean, 1e-10);
            Assert.AreEqual(0.1575844721504685, median, 1e-6);
            Assert.AreEqual(75, var);
            Assert.AreEqual(0.1111111111111111, mode);
            Assert.AreEqual(11.366892314949228, chf);
            Assert.AreEqual(0.99998842765450235, cdf);
            Assert.AreEqual(0.0000063457380298844403, pdf);
            Assert.AreEqual(-11.967727146795536, lpdf);
            Assert.AreEqual(0.54835366185492873, hf);
            Assert.AreEqual(1.1572345497645742E-05, ccdf);
            Assert.AreEqual(6.2700000113440124, icdf, 1e-6);
            Assert.AreEqual("Inv-χ²(x; df = 7)", str);

            var range1 = invchisq.GetRange(0.95);
            var range2 = invchisq.GetRange(0.99);
            var range3 = invchisq.GetRange(0.01);

            Assert.AreEqual(0.071087647148133509, range1.Min, 1e-10);
            Assert.AreEqual(0.4613930157807814, range1.Max, 1e-10);
            Assert.AreEqual(0.054126298885838933, range2.Min, 1e-10);
            Assert.AreEqual(0.807074944498757, range2.Max, 1e-10);
            Assert.AreEqual(0.054126298885838926, range3.Min, 1e-10);
            Assert.AreEqual(0.807074944498757, range3.Max, 1e-10);

            Assert.AreEqual(0, invchisq.Support.Min);
            Assert.AreEqual(double.PositiveInfinity, invchisq.Support.Max);

            Assert.AreEqual(invchisq.InverseDistributionFunction(0), invchisq.Support.Min);
            Assert.AreEqual(invchisq.InverseDistributionFunction(1), invchisq.Support.Max);
        }

        [Test]
        public void GetRangeTest()
        {
            var invchisq = new InverseChiSquareDistribution(degreesOfFreedom: 1);

            var range1 = invchisq.GetRange(0.95);
            var range2 = invchisq.GetRange(0.99);
            var range3 = invchisq.GetRange(0.01);

            Assert.AreEqual(0.26031760700955658, range1.Min);
            Assert.AreEqual(254.31444455016248, range1.Max);

            Assert.AreEqual(0.1507182465101253, range2.Min);
            Assert.AreEqual(6365.864385106167, range2.Max);

            Assert.AreEqual(0.15071824651012525, range3.Min);
            Assert.AreEqual(6365.864385106167, range3.Max);
        }

        [Test]
        public void GetRangeTest2()
        {
            var invchisq = new InverseChiSquareDistribution(degreesOfFreedom: 4200);

            var range1 = invchisq.GetRange(0.95);
            var range2 = invchisq.GetRange(0.99);
            var range3 = invchisq.GetRange(0.01);

            Assert.AreEqual(0.00022955191926311784, range1.Min);
            Assert.AreEqual(0.0002470497945606555, range1.Max);
            Assert.AreEqual(0.00022625236260944044, range2.Min);
            Assert.AreEqual(0.00025044362899547131, range2.Max);
            Assert.AreEqual(0.00022625236260944012, range3.Min);
            Assert.AreEqual(0.00025044362899547131, range3.Max);
        }

        [Test]
        public void MedianTest()
        {
            var target = new ChiSquareDistribution(degreesOfFreedom: 4);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
        public void InverseTest()
        {
            double expected = 1.8307038053275149991e+01;
            double actual = ChiSquareDistribution.Inverse(0.95, 10);
            Assert.AreEqual(expected, actual, 1e-14);
        }

        [Test]
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

            Assert.IsTrue(expected.IsEqual(actual, atol: 1e-14));
        }

        [Test]
        public void InverseTest3()
        {
            var invchisq = new InverseChiSquareDistribution(degreesOfFreedom: 7);

            double cdf, icdf;


            // Round-trip with 6.27
            cdf = invchisq.DistributionFunction(x: 6.27);
            Assert.AreEqual(0.99998842765450235, cdf);

            icdf = invchisq.InverseDistributionFunction(p: cdf);
            Assert.AreEqual(6.2699998329362963, icdf, 1e-6);


            // Round-trip with 0
            cdf = invchisq.DistributionFunction(x: 0);
            Assert.AreEqual(0, cdf); 

            icdf = invchisq.InverseDistributionFunction(p: cdf);
            Assert.AreEqual(0, icdf, 1e-6);


            // Round-trip with 1
            cdf = invchisq.DistributionFunction(x: 1);
            Assert.AreEqual(0.99482853651651548, cdf);

            icdf = invchisq.InverseDistributionFunction(p: cdf);
            Assert.AreEqual(1, icdf, 1e-6);
        }

    }
}
