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
    using Accord.Math;
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;

    [TestFixture]
    public class GeneralizedParetoDistributionTest
    {

        [Test]
        public void ConstructorTest()
        {
            #region doc_example1
            // Create a new Generalized Pareto Distribution with mu = 0, sigma = 0.42, xi = 3
            var pareto = new GeneralizedParetoDistribution(location: 0, scale: 0.42, shape: 3);

            // Common measures
            double mean = pareto.Mean;     // -0.21
            double median = pareto.Median; //  0.98
            double var = pareto.Variance;  // -0.00882
            double mode = pareto.Mode;     //  0.19185185523755152

            // Cumulative distribution functions
            double cdf = pareto.DistributionFunction(x: 1.4); // 0.55035568697739079
            double ccdf = pareto.ComplementaryDistributionFunction(x: 1.4); // 0.44964431302260921
            double icdf = pareto.InverseDistributionFunction(p: cdf); // 1.3999999035548829

            // Probability density functions
            double pdf = pareto.ProbabilityDensityFunction(x: 1.4); // 0.097325608879352654
            double lpdf = pareto.LogProbabilityDensityFunction(x: 1.4); // -2.3296931293597707

            // Hazard (failure rate) functions
            double hf = pareto.HazardFunction(x: 1.4); // 0.21645021645021648
            double chf = pareto.CumulativeHazardFunction(x: 1.4); // 0.79929842426612341

            // String representation
            string str = pareto.ToString(CultureInfo.InvariantCulture); // Pareto(x; μ = 0, σ = 0.42, ξ = 3)
            #endregion

            Assert.AreEqual(-0.21, mean);
            Assert.AreEqual(0.98, median);
            Assert.AreEqual(-0.008819999999999998, var);
            Assert.AreEqual(0.19185185523755152, mode, 1e-10);
            Assert.AreEqual(0.79929842426612341, chf);
            Assert.AreEqual(0.55035568697739079, cdf);
            Assert.AreEqual(0.097325608879352654, pdf);
            Assert.AreEqual(-2.3296931293597707, lpdf);
            Assert.AreEqual(0.21645021645021648, hf);
            Assert.AreEqual(0.44964431302260921, ccdf);
            Assert.AreEqual(1.40, icdf, 1e-5);
            Assert.AreEqual("Pareto(x; μ = 0, σ = 0.42, ξ = 3)", str);

            var range1 = pareto.GetRange(0.95);
            var range2 = pareto.GetRange(0.99);
            var range3 = pareto.GetRange(0.01);

            Assert.AreEqual(0.023289267355975959, range1.Min, 1e-8);
            Assert.AreEqual(1119.8599999998519, range1.Max, 1e-8);
            Assert.AreEqual(0.0042854196206619614, range2.Min, 1e-8);
            Assert.AreEqual(139999.86000000086, range2.Max, 1e-8);
            Assert.AreEqual(0.0042854196206619493, range3.Min, 1e-8);
            Assert.AreEqual(139999.86000000086, range3.Max, 1e-8);
        }

        [Test]
        public void ParetoDistributionConstructorTest()
        {
            double expected, actual;

            {
                var target = new GeneralizedParetoDistribution(0, 3.1, 4.42);
                actual = target.ProbabilityDensityFunction(-1);
                expected = 0.0;
                Assert.AreEqual(expected, actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));

                actual = target.ProbabilityDensityFunction(0);
                expected = 0.32258064516129031;
                Assert.AreEqual(expected, actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));

                actual = target.ProbabilityDensityFunction(3.09);
                expected = 0.040736023124121959;
                Assert.AreEqual(expected, actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));

                actual = target.ProbabilityDensityFunction(3.1);
                expected = 0.040604655728907986;
                Assert.AreEqual(expected, actual, 1e-7);

                actual = target.ProbabilityDensityFunction(3.2);
                expected = 0.039332127082599325;
                Assert.AreEqual(expected, actual, 1e-7);

                actual = target.ProbabilityDensityFunction(5.8);
                expected = 0.021027167986227731;
                Assert.AreEqual(expected, actual, 1e-7);

                actual = target.ProbabilityDensityFunction(10);
                expected = 0.011412447781534748;
                Assert.AreEqual(expected, actual, 1e-7);
            }
        }

        [Test]
        public void MedianTest()
        {
            var target = new GeneralizedParetoDistribution(0, scale: 7.12, shape: 2);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5), 1e-6);
        }

    }
}
