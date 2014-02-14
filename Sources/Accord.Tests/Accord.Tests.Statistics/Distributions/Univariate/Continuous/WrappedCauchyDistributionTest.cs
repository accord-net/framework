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
    using System.Globalization;


    [TestClass()]
    public class WrappedCauchyDistributionTest
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
            var dist = new WrappedCauchyDistribution(mu: 0.42, gamma: 3);

            double mean = dist.Mean;     // 0.42
            double var = dist.Variance;  // 0.950212931632136

            double pdf = dist.ProbabilityDensityFunction(x: 0.42); // 0.1758330112785475
            double lpdf = dist.LogProbabilityDensityFunction(x: 0.42); // -1.7382205338929015

            string str = dist.ToString(CultureInfo.InvariantCulture); // "WrappedCauchy(x; μ = 0.42, γ = 3)"

            Assert.AreEqual(0.42, mean);
            Assert.AreEqual(0.950212931632136, var);
            Assert.AreEqual(0.1758330112785475, pdf);
            Assert.AreEqual(-1.7382205338929015, lpdf);
            Assert.AreEqual("WrappedCauchy(x; μ = 0.42, γ = 3)", str);
        }

    }
}
