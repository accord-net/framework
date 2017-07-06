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
    using System;
    using System.Globalization;


    [TestFixture]
    public class WrappedCauchyDistributionTest
    {


        [Test]
        public void ConstructorTest()
        {
            var dist = new WrappedCauchyDistribution(mu: 0.42, gamma: 3);

            double mean = dist.Mean;     // 0.42
            double var = dist.Variance;  // 0.950212931632136
            try { double median = dist.Median; Assert.Fail(); }
            catch { }
            try { double mode = dist.Mode; Assert.Fail(); }
            catch { }

            double pdf = dist.ProbabilityDensityFunction(x: 0.42); // 0.1758330112785475
            double lpdf = dist.LogProbabilityDensityFunction(x: 0.42); // -1.7382205338929015

            string str = dist.ToString(CultureInfo.InvariantCulture); // "WrappedCauchy(x; μ = 0.42, γ = 3)"

            Assert.AreEqual(0.42, mean);
            Assert.AreEqual(0.950212931632136, var);
            Assert.AreEqual(0.1758330112785475, pdf, 1e-10);
            Assert.AreEqual(-1.7382205338929015, lpdf, 1e-10);
            Assert.AreEqual("WrappedCauchy(x; μ = 0.42, γ = 3)", str);

            Assert.IsFalse(Double.IsNaN(pdf));
            Assert.IsFalse(Double.IsNaN(lpdf));

            bool thrown = false;
            try { dist.GetRange(0.95); }
            catch (NotSupportedException) { thrown = true; }

            Assert.IsTrue(thrown);

            Assert.AreEqual(-Math.PI, dist.Support.Min, 1e-10);
            Assert.AreEqual(+Math.PI, dist.Support.Max, 1e-10);

            Assert.AreEqual(dist.InverseDistributionFunction(0), dist.Support.Min);
            Assert.AreEqual(dist.InverseDistributionFunction(1), dist.Support.Max);
        }

    }
}
