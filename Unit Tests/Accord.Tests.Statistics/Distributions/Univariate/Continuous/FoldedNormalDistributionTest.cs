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
    using System;
    using System.Globalization;
    using Accord.Statistics;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;

    [TestFixture]
    public class FoldedNormalDistributionTest
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



        [Test]
        public void ConstructorTest()
        {
            var fn = new FoldedNormalDistribution(mean: 4, stdDev: 4.2);

            double mean = fn.Mean;     // 4.765653108337438
            double median = fn.Median; // 4.259356588022813
            double mode = fn.Mode;     // 2.0806531871308014
            double var = fn.Variance;  // 10.928550450993715

            double cdf = fn.DistributionFunction(x: 1.4);           // 0.16867109769018807
            double pdf = fn.ProbabilityDensityFunction(x: 1.4);     // 0.11998602818182187
            double lpdf = fn.LogProbabilityDensityFunction(x: 1.4); // -2.1203799747969523

            double ccdf = fn.ComplementaryDistributionFunction(x: 1.4); // 0.83132890230981193
            double icdf = fn.InverseDistributionFunction(p: cdf);       // 1.4

            double hf = fn.HazardFunction(x: 1.4);            // 0.14433039420191671
            double chf = fn.CumulativeHazardFunction(x: 1.4); // 0.18472977144474392

            string str = fn.ToString(CultureInfo.InvariantCulture); // FN(x; μ = 4, σ² = 17.64)


            // Tested against GNU R's VGAM package
            //
            // dfnorm(1.4, mean= 4, sd= 4.2)
            // [1] 0.11998602818182191321
            //
            // pfnorm(1.4, mean= 4, sd= 4.2)
            // [1] 0.16867109769018800991
            //

            Assert.AreEqual(4.765653108337438, mean);
            Assert.AreEqual(4.2593565884089237, median);
            Assert.AreEqual(2.0806531871308014, mode);
            Assert.AreEqual(10.928550450993715, var);
            Assert.AreEqual(0.18472977144474392, chf);
            Assert.AreEqual(0.16867109769018800991, cdf, 1e-10); // from R
            Assert.AreEqual(0.11998602818182191321, pdf, 1e-10); // from R
            Assert.AreEqual(-2.1203799747969523, lpdf);
            Assert.AreEqual(0.14433039420191671, hf);
            Assert.AreEqual(0.83132890230981193, ccdf);
            Assert.AreEqual(1.4, icdf, 1e-8);
            Assert.AreEqual("FN(x; μ = 4, σ² = 17.64)", str);

            var range1 = fn.GetRange(0.95);
            Assert.AreEqual(0.41428977779338388, range1.Min);
            Assert.AreEqual(10.916197224646602, range1.Max);

            var range2 = fn.GetRange(0.99);
            Assert.AreEqual(0.082845881461525633, range2.Min);
            Assert.AreEqual(13.77249095493084, range2.Max);

            var range3 = fn.GetRange(0.01);
            Assert.AreEqual(0.082845881461525495, range3.Min);
            Assert.AreEqual(13.77249095493084, range3.Max);

        }

    }
}
