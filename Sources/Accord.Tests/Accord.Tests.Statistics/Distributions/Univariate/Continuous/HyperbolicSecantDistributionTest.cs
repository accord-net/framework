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
    using System.Globalization;
    using Accord.Statistics.Distributions.Univariate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class HyperbolicSecantTest
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
        public void ConstructorTest1()
        {
            var sech = new HyperbolicSecantDistribution();

            double mean = sech.Mean;     // 0.0
            double median = sech.Median; // 0.0
            double mode = sech.Mode;     // 0.0
            double var = sech.Variance;  // 1.0

            double cdf = sech.DistributionFunction(x: 1.4); // 2.2939067361538474
            double pdf = sech.ProbabilityDensityFunction(x: 1.4); // 0.10955386512899701
            double lpdf = sech.LogProbabilityDensityFunction(x: 1.4); // -2.2113389316917877

            double ccdf = sech.ComplementaryDistributionFunction(x: 1.4); // -1.2939067361538474
            double icdf = sech.InverseDistributionFunction(p: cdf); // 1.4000000017042402

            double hf = sech.HazardFunction(x: 1.4); // -0.084669058493850285

            string str = sech.ToString(); // Sech(x)

            Assert.AreEqual(0, mean);
            Assert.AreEqual(0, median);
            Assert.AreEqual(1, var);
            Assert.AreEqual(0, mode);
            Assert.AreEqual(2.2939067361538474, cdf);
            Assert.AreEqual(0.10955386512899701, pdf);
            Assert.AreEqual(-2.2113389316917877, lpdf);
            Assert.AreEqual(-0.084669058493850285, hf);
            Assert.AreEqual(-1.2939067361538474, ccdf);
            Assert.AreEqual(1.4000000017042402, icdf);
            Assert.AreEqual("Sech(x)", str);
        }

    }
}
