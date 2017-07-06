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
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;
    using System;
    using System.Globalization;

    [TestFixture]
    public class SymmetricGeometricDistributionTest
    {

        [Test]
        public void ConstructorTest()
        {
            // Create a Geometric distribution with 42% success probability
            var dist = new SymmetricGeometricDistribution(probabilityOfSuccess: 0.42);

            double mean = dist.Mean;     // 1.3809523809523812
            Assert.Throws<NotSupportedException>(() => { double d = dist.Median; });
            double var = dist.Variance;  // 3.2879818594104315
            double mode = dist.Mode;     // 0

            Assert.Throws<NotSupportedException>(() => dist.DistributionFunction(k: 2));
            Assert.Throws<NotSupportedException>(() => dist.ComplementaryDistributionFunction(k: 2));

            double pdf1 = dist.ProbabilityMassFunction(k: 0); // 0.42
            double pdf2 = dist.ProbabilityMassFunction(k: 1); // 0.2436
            double pdf3 = dist.ProbabilityMassFunction(k: 2); // 0.141288

            double lpdf = dist.LogProbabilityMassFunction(k: 2); // -1.956954918588067

            Assert.Throws<NotSupportedException>(() => dist.InverseDistributionFunction(p: 0.17));
            Assert.Throws<NotSupportedException>(() => dist.InverseDistributionFunction(p: 0.46));
            Assert.Throws<NotSupportedException>(() => dist.InverseDistributionFunction(p: 0.87));

            Assert.Throws<NotSupportedException>(() => dist.HazardFunction(x: 0));
            Assert.Throws<NotSupportedException>(() => dist.CumulativeHazardFunction(x: 0));

            string str = dist.ToString(CultureInfo.InvariantCulture); // "Geometric(x; p = 0.42)"

            Assert.AreEqual(0, mean);
            Assert.AreEqual(0, mode);
            Assert.AreEqual(5.1950113378684817, var);
            Assert.AreEqual(0.42, pdf1);
            Assert.AreEqual(0.36206896551724133, pdf2);
            Assert.AreEqual(0.21, pdf3);
            Assert.AreEqual(-1.5606477482646683, lpdf);
            Assert.AreEqual("SymmetricGeometric(x; p = 0.42)", str);
        }

        [Test]
        public void GeometricDistributionConstructorTest()
        {
            double successProbability = 0.9;
            var target = new SymmetricGeometricDistribution(successProbability);
            Assert.AreEqual(0.9, target.ProbabilityOfSuccess);
            Assert.AreEqual(0, target.Mean);
            Assert.AreEqual(0.13580246913580243, target.Variance);
        }


        [Test]
        public void FitTest()
        {
            double successProbability = 0;
            var target = new SymmetricGeometricDistribution(successProbability);

            double[] observations = { 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0 };
            double[] weights = null;
            IFittingOptions options = null;

            Assert.Throws<NotSupportedException>(() => target.Fit(observations, weights, options));
        }

        [Test]
        public void ProbabilityMassFunctionTest()
        {
            double successProbability = 0.42;
            var target = new SymmetricGeometricDistribution(successProbability);

            double[] expected =
            {
                0.040973520000000013,
                0.070644000000000012,
                0.12180000000000002,
                0.21,
                0.36206896551724133,
                0.42,
                0.36206896551724133,
                0.21,
                0.12180000000000002,
                0.070644000000000012,
                0.040973520000000013,
            };

            for (int i = -5; i <= 5; i++)
            {
                double e = expected[i + 5];
                double actual = target.ProbabilityMassFunction(i);
                Assert.AreEqual(e, actual, 1e-10);

                double log = target.LogProbabilityMassFunction(i);
                Assert.AreEqual(Math.Log(e), log);
            }
        }

    }
}
