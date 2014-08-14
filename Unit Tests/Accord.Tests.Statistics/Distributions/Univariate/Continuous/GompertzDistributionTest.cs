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
    using Accord.Controls;
    using Accord.Statistics.Testing;

    [TestClass()]
    public class GompertzDistributionTest
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
            var gompertz = new GompertzDistribution(eta: 4.2, b: 1.1);

            double median = gompertz.Median; // 0.13886469671401389

            double cdf = gompertz.DistributionFunction(x: 0.27); // 0.76599768199799145
            double pdf = gompertz.ProbabilityDensityFunction(x: 0.27); // 1.4549484164912097
            double lpdf = gompertz.LogProbabilityDensityFunction(x: 0.27); // 0.37497044741163688

            double ccdf = gompertz.ComplementaryDistributionFunction(x: 0.27); // 0.23400231800200855
            double icdf = gompertz.InverseDistributionFunction(p: cdf); // 0.26999999999766749

            double hf = gompertz.HazardFunction(x: 0.27); // 6.2176666834502088
            double chf = gompertz.CumulativeHazardFunction(x: 0.27); // 1.4524242576820101

            string str = gompertz.ToString(System.Globalization.CultureInfo.InvariantCulture);
              // "Gompertz(x; η = 4.2, b = 1.1)"

            Assert.AreEqual(0.13886469671401389, median);
            Assert.AreEqual(1.4524242576820101, chf);
            Assert.AreEqual(0.76599768199799145, cdf);
            Assert.AreEqual(1.4549484164912097, pdf);
            Assert.AreEqual(0.37497044741163688, lpdf);
            Assert.AreEqual(6.2176666834502088, hf);
            Assert.AreEqual(0.23400231800200855, ccdf);
            Assert.AreEqual(0.26999999999766749, icdf);
            Assert.AreEqual("Gompertz(x; η = 4.2, b = 1.1)", str);
        }

        [TestMethod()]
        public void MedianTest()
        {
            var target = new GompertzDistribution(eta: 42, b: 4.2);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5), 1e-8);
        }


    }
}
