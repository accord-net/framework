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
    using Accord.Statistics;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Univariate;
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class VonMisesFisherDistributionTest
    {


        [Test]
        public void ConstructorTest1()
        {
            // If p=2 the distribution reduces to the von Mises distribution on the circle.

            double kappa = 4.2;
            var vm = new VonMisesDistribution(0, kappa);
            var target = new VonMisesFisherDistribution(new double[] { -1, 0 }, kappa);

            double s = Math.Sqrt(2) / 2;
            double[] mean = target.Mean;

            double a000 = target.ProbabilityDensityFunction(new double[] { +1, +0 });
            double a045 = target.ProbabilityDensityFunction(new double[] { +s, +s });
            double a090 = target.ProbabilityDensityFunction(new double[] { +0, +1 });
            double a135 = target.ProbabilityDensityFunction(new double[] { -s, +s });
            double a180 = target.ProbabilityDensityFunction(new double[] { -1, +0 });
            double a225 = target.ProbabilityDensityFunction(new double[] { -s, -s });
            double a270 = target.ProbabilityDensityFunction(new double[] { +0, -1 });
            double a315 = target.ProbabilityDensityFunction(new double[] { +s, -s });
            double a360 = target.ProbabilityDensityFunction(new double[] { +1, +0 });

            double offset = -Math.PI;
            double e000 = vm.ProbabilityDensityFunction(offset + 0);
            double e045 = vm.ProbabilityDensityFunction(offset + Math.PI / 4);
            double e090 = vm.ProbabilityDensityFunction(offset + Math.PI / 2);
            double e135 = vm.ProbabilityDensityFunction(offset + Math.PI * (3 / 4.0));
            double e180 = vm.ProbabilityDensityFunction(offset + Math.PI);
            double e225 = vm.ProbabilityDensityFunction(offset + Math.PI * (5 / 4.0));
            double e270 = vm.ProbabilityDensityFunction(offset + Math.PI * (3 / 2.0));
            double e315 = vm.ProbabilityDensityFunction(offset + Math.PI * (7 / 4.0));
            double e360 = vm.ProbabilityDensityFunction(offset + Math.PI * 2);


            Assert.AreEqual(e000, a000, 1e-8);
            Assert.AreEqual(e045, a045, 1e-8);
            Assert.AreEqual(e090, a090, 1e-8);
            Assert.AreEqual(e135, a135, 1e-8);
            Assert.AreEqual(e180, a180, 1e-8);
            Assert.AreEqual(e225, a225, 1e-8);
            Assert.AreEqual(e270, a270, 1e-8);
            Assert.AreEqual(e315, a315, 1e-8);
            Assert.AreEqual(e360, a360, 1e-8);
        }


        [Test]
        public void generate_test()
        {
            double kappa = 35;
            var vm = new VonMisesDistribution(0, kappa);
            var target = new VonMisesFisherDistribution(new double[] { -1, 0, 0 }, kappa);

            double[][] actual = target.Generate(100);

            var sphere = new UniformSphereDistribution(3);
            for (int i = 0; i < actual.Length; i++)
            {
                double p = sphere.ProbabilityDensityFunction(actual[i]);
                Assert.AreEqual(0.025330295910584444, p, 1e-8);
            }
        }
    }
}
