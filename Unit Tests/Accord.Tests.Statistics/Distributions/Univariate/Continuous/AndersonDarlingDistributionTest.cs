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
    using Accord.Math;
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;
    using System;
    using System.Globalization;

    [TestFixture]
    public class AndersonDarlingDistributionTest
    {

        [Test]
        public void ConstructorTest()
        {
            // Create a new Anderson Darling distribution (A²) for comparing against a Gaussian
            var a2 = new AndersonDarlingDistribution(AndersonDarlingDistributionType.Normal, 30);

            double median = a2.Median; // 0.33089957635450062

            double chf = a2.CumulativeHazardFunction(x: 0.27); // 0.42618068373640966
            double cdf = a2.DistributionFunction(x: 0.27); // 0.34700165471995292
            double ccdf = a2.ComplementaryDistributionFunction(x: 0.27); // 0.65299834528004708
            double icdf = a2.InverseDistributionFunction(p: cdf); // 0.27000000012207787

            string str = a2.ToString(CultureInfo.InvariantCulture); // "A²(x; n = 30)"

            Assert.AreEqual(0.33089957635450062, median, 1e-6);
            Assert.AreEqual(0.42618068373640966, chf, 1e-6);
            Assert.AreEqual(0.34700165471995292, cdf, 1e-6);
            Assert.AreEqual(0.65299834528004708, ccdf, 1e-6);
            Assert.AreEqual(0.27, icdf, 1e-6);
            Assert.AreEqual("A²(x; n = 30)", str);

            Assert.IsFalse(Double.IsNaN(median));

            var range1 = a2.GetRange(0.95);
            var range2 = a2.GetRange(0.99);
            var range3 = a2.GetRange(0.01);

            Assert.AreEqual(0.1552371857564955, range1.Min, 1e-6);
            Assert.AreEqual(0.73303544269106991, range1.Max, 1e-6);
            Assert.AreEqual(0.11507415861447158, range2.Min, 1e-6);
            Assert.AreEqual(1.0090123686318795, range2.Max, 1e-6);
            Assert.AreEqual(0.1150741586144715, range3.Min, 1e-6);
            Assert.AreEqual(1.0090123686318795, range3.Max, 1e-6);

            Assert.AreEqual(0, a2.Support.Min);
            Assert.AreEqual(Double.PositiveInfinity, a2.Support.Max);

            Assert.AreEqual(a2.InverseDistributionFunction(0), a2.Support.Min);
            Assert.AreEqual(a2.InverseDistributionFunction(1), a2.Support.Max);
        }

        [Test]
        public void uniform_test()
        {
            // Create a new Anderson Darling distribution (A²) for comparing against a Uniform distribution
            var a2 = new AndersonDarlingDistribution(AndersonDarlingDistributionType.Uniform, 10);

            double median = a2.Median; 

            double chf = a2.CumulativeHazardFunction(x: 0.27); 
            double cdf = a2.DistributionFunction(x: 0.27); 
            double ccdf = a2.ComplementaryDistributionFunction(x: 0.27);
            double icdf = a2.InverseDistributionFunction(p: cdf); 

            string str = a2.ToString(CultureInfo.InvariantCulture); // "A²(x; n = 30)"

            Assert.AreEqual(0.76936270639003257, median, 1e-6);
            Assert.AreEqual(0.0424239540571123, chf, 1e-6);
            Assert.AreEqual(0.041536650001198654, cdf, 1e-6);
            Assert.AreEqual(0.95846334999880134, ccdf, 1e-6);
            Assert.AreEqual(0.27, icdf, 1e-6);
            Assert.AreEqual("A²(x; n = 10)", str);

            Assert.IsFalse(Double.IsNaN(median));

            var range1 = a2.GetRange(0.95);
            var range2 = a2.GetRange(0.99);
            var range3 = a2.GetRange(0.01);

            Assert.AreEqual(0.28254198132705227, range1.Min, 1e-6);
            Assert.AreEqual(2.5127111321231137, range1.Max, 1e-6);
            Assert.AreEqual(0.20232764997988909, range2.Min, 1e-6);
            Assert.AreEqual(3.9195747018635374, range2.Max, 1e-6);
            Assert.AreEqual(0.20232764997988903, range3.Min, 1e-6);
            Assert.AreEqual(3.9195747018635374, range3.Max, 1e-6);

            Assert.AreEqual(0, a2.Support.Min);
            Assert.AreEqual(Double.PositiveInfinity, a2.Support.Max);

            Assert.AreEqual(a2.InverseDistributionFunction(0), a2.Support.Min);
            Assert.AreEqual(a2.InverseDistributionFunction(1), a2.Support.Max);
        }


        [Test]
        public void inverse_cdf()
        {
            var a2 = new AndersonDarlingDistribution(AndersonDarlingDistributionType.Normal, 30);

            Assert.AreEqual(0, a2.InverseDistributionFunction(0));
            Assert.AreEqual(Double.PositiveInfinity, a2.InverseDistributionFunction(1));
            Assert.AreEqual(1.4047406720947551d, a2.InverseDistributionFunction(0.999));

            Assert.AreEqual(1.4615690610009224E-06d, a2.DistributionFunction(0));
            Assert.AreEqual(0.98946087159962637d, a2.DistributionFunction(1));
            Assert.AreEqual(0.99996831752075233d, a2.DistributionFunction(2));

            double[] percentiles = Vector.Range(0.0, 1.0, stepSize: 0.1);
            {
                double p = 0.5;
                double icdf = a2.InverseDistributionFunction(p);
                double cdf = a2.DistributionFunction(icdf);
                Assert.AreEqual(cdf, 0.5, 0.05);
            }

            for (int i = 0; i < percentiles.Length; i++)
            {
                if (i == 5)
                    continue;

                double p = percentiles[i];
                double icdf = a2.InverseDistributionFunction(p);
                double cdf = a2.DistributionFunction(icdf);
                Assert.AreEqual(cdf, p, 1e-5);
            }
        }

        [Test]
        public void inverse_cdf_uniform()
        {
            var a2 = new AndersonDarlingDistribution(AndersonDarlingDistributionType.Uniform, 30);

            Assert.AreEqual(0, a2.InverseDistributionFunction(0));
            Assert.AreEqual(Double.PositiveInfinity, a2.InverseDistributionFunction(1));
            Assert.AreEqual(6.0001199778679908d, a2.InverseDistributionFunction(0.999));

            Assert.AreEqual(0, a2.DistributionFunction(0));
            Assert.AreEqual(0.64343026173645557d, a2.DistributionFunction(1));
            Assert.AreEqual(0.90775461488067177d, a2.DistributionFunction(2));

            double[] percentiles = Vector.Range(0.0, 1.0, stepSize: 0.1);
            {
                double p = 0.5;
                double icdf = a2.InverseDistributionFunction(p);
                double cdf = a2.DistributionFunction(icdf);
                Assert.AreEqual(cdf, 0.5, 0.05);
            }

            for (int i = 0; i < percentiles.Length; i++)
            {
                if (i == 5)
                    continue;

                double p = percentiles[i];
                double icdf = a2.InverseDistributionFunction(p);
                double cdf = a2.DistributionFunction(icdf);
                Assert.AreEqual(cdf, p, 1e-5);
            }
        }
    }
}
