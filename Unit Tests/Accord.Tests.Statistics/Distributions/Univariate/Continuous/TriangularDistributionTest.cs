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
    using Accord.Statistics;
    using NUnit.Framework;
    using System;
    using Accord.Math;
    using Accord.Statistics.Distributions.Multivariate;
    using System.Globalization;

    [TestFixture]
    public class TriangularDistributionTest
    {


        [Test]
        public void ConstructorTest1()
        {
            var tri = new TriangularDistribution(min: 1, max: 6, mode: 3);

            double mean = tri.Mean;     // 3.3333333333333335
            double median = tri.Median; // 3.2613872124741694
            double mode = tri.Mode;     // 3.0
            double var = tri.Variance;  // 1.0555555555555556

            double cdf = tri.DistributionFunction(x: 2); // 0.10000000000000001
            double pdf = tri.ProbabilityDensityFunction(x: 2); // 0.20000000000000001
            double lpdf = tri.LogProbabilityDensityFunction(x: 2); // -1.6094379124341003

            double ccdf = tri.ComplementaryDistributionFunction(x: 2); // 0.90000000000000002
            double icdf = tri.InverseDistributionFunction(p: cdf); // 2.0000000655718773

            double hf = tri.HazardFunction(x: 2); // 0.22222222222222224
            double chf = tri.CumulativeHazardFunction(x: 2); // 0.10536051565782628

            string str = tri.ToString(CultureInfo.InvariantCulture); // Triangular(x; a = 1, b = 6, c = 3)

            Assert.AreEqual(3.3333333333333335, mean);
            Assert.AreEqual(3.0, mode);
            Assert.AreEqual(3.2613872124741694, median);
            Assert.AreEqual(1.0555555555555556, var);
            Assert.AreEqual(0.10536051565782628, chf);
            Assert.AreEqual(0.10000000000000001, cdf);
            Assert.AreEqual(0.20000000000000001, pdf);
            Assert.AreEqual(-1.6094379124341003, lpdf);
            Assert.AreEqual(0.22222222222222224, hf);
            Assert.AreEqual(0.90000000000000002, ccdf);
            Assert.AreEqual(2.0000000655718773, icdf);
            Assert.AreEqual("Triangular(x; a = 1, b = 6, c = 3)", str);

            var range1 = tri.GetRange(0.95);
            var range2 = tri.GetRange(0.99);
            var range3 = tri.GetRange(0.01);

            Assert.AreEqual(1.7071067704914942, range1.Min);
            Assert.AreEqual(5.1339745973005186, range1.Max);
            Assert.AreEqual(1.3162277235820534, range2.Min);
            Assert.AreEqual(5.6127016687540774, range2.Max);
            Assert.AreEqual(1.3162277235820532, range3.Min);
            Assert.AreEqual(5.6127016687540774, range3.Max);

            Assert.AreEqual(1, tri.Support.Min);
            Assert.AreEqual(6, tri.Support.Max);

            Assert.AreEqual(tri.InverseDistributionFunction(0), tri.Support.Min);
            Assert.AreEqual(tri.InverseDistributionFunction(1), tri.Support.Max);
        }

        [Test]
        public void GenerateTest1()
        {
            Accord.Math.Tools.SetupGenerator(0);

            var target = new TriangularDistribution(-4.2, 7, 1);

            double[] samples = new double[10000000];

            for (int i = 0; i < samples.Length; i++)
            {
                double u = target.Generate();
                samples[i] = System.Math.Round(u, 2);
            }

            double min = samples.Min();
            double max = samples.Max();
            double mode = samples.Mode();

            Assert.AreEqual(min, target.Min, 1e-2);
            Assert.AreEqual(max, target.Max, 1e-2);
            Assert.AreEqual(mode, target.Mode, 0.035);
        }

        [Test]
        public void GenerateTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            var target = new TriangularDistribution(-4.2, 7, 1);

            double[] samples = target.Generate(10000000);

            for (int i = 0; i < samples.Length; i++)
                samples[i] = System.Math.Round(samples[i], 2);

            double min = samples.Min();
            double max = samples.Max();
            double mode = samples.Mode();

            Assert.AreEqual(min, target.Min, 1e-2);
            Assert.AreEqual(max, target.Max, 1e-2);
            Assert.AreEqual(mode, target.Mode, 0.035);
        }

        [Test]
        public void ctor_test()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TriangularDistribution(min: 0, max: 0, mode: 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TriangularDistribution(min: 1, max: 0, mode: 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TriangularDistribution(min: 1, max: 0.5, mode: 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TriangularDistribution(min: 1, max: 2.5, mode: 0));
        }
    }
}
