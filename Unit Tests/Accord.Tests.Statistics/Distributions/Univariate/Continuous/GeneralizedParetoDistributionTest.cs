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
    using Accord.Statistics.Distributions.Reflection;

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
            Assert.AreEqual(0.19185185523755152, mode, 1e-6);
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

            Assert.AreEqual(0.023289267355975959, range1.Min, 1e-6);
            Assert.AreEqual(1119.8599999998519, range1.Max, 1e-8);
            Assert.AreEqual(0.0042854196206619614, range2.Min, 1e-8);
            Assert.AreEqual(139999.86000000086, range2.Max, 1e-8);
            Assert.AreEqual(0.0042854196206619493, range3.Min, 1e-8);
            Assert.AreEqual(139999.86000000086, range3.Max, 1e-8);

            Assert.AreEqual(0, pareto.Support.Min);
            Assert.AreEqual(double.PositiveInfinity, pareto.Support.Max);

            Assert.AreEqual(pareto.InverseDistributionFunction(0), pareto.Support.Min);
            Assert.AreEqual(pareto.InverseDistributionFunction(1), pareto.Support.Max);
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

            double median = target.Median;
            Assert.AreEqual(10.68, median, 1e-10);

            Assert.AreEqual(median, target.InverseDistributionFunction(0.5), 1e-6);
        }

        [Test]
        public void MedianTest2()
        {
            var target = UnivariateDistributionInfo.CreateInstance<GeneralizedParetoDistribution>();

            Assert.AreEqual(1, target.Location);
            Assert.AreEqual(1, target.Scale);
            Assert.AreEqual(2, target.Shape);

            double median = target.Median;
            Assert.AreEqual(2.5, target.Median);

            Assert.AreEqual(median, target.InverseDistributionFunction(0.5), 1e-6);
        }

        [Test]
        public void ctor_test()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new GeneralizedParetoDistribution(location: 0, scale: 0, shape: 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => new GeneralizedParetoDistribution(location: 1, scale: 0, shape: 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => new GeneralizedParetoDistribution(location: 1, scale: 0, shape: 2));
            Assert.DoesNotThrow(() => new GeneralizedParetoDistribution(location: 1, scale: 1, shape: 1));
            Assert.DoesNotThrow(() => new GeneralizedParetoDistribution(location: 1, scale: 1, shape: 2));
        }

        [Test]
        public void MedianTest3()
        {
            var target = new GeneralizedParetoDistribution(location: 1, scale: 1, shape: 2);

            Assert.AreEqual(1, target.Location);
            Assert.AreEqual(1, target.Scale);
            Assert.AreEqual(2, target.Shape);


            double median = target.Median;
            Assert.AreEqual(2.5, median, 1e-10);

            Assert.AreEqual(median, target.InverseDistributionFunction(0.5), 1e-6);
        }

        [Test]
        public void MedianTest4()
        {
            var target = new GeneralizedParetoDistribution(location: 0, scale: 1, shape: 0);

            Assert.AreEqual(0, target.Location);
            Assert.AreEqual(1, target.Scale);
            Assert.AreEqual(0, target.Shape);


            double idf = target.InverseDistributionFunction(0.5);
            double median = target.Median;

            Assert.AreEqual(idf, median, 1e-10);
            Assert.AreEqual(idf, 0.69314718055994529, 1e-10);
        }

        [Test]
        public void zero_ksi()
        {
            var target = new GeneralizedParetoDistribution(-42, 5, 0);
            double[] expected, actual;
            string str;

            Assert.AreEqual(-42, target.Support.Min);
            Assert.AreEqual(Double.PositiveInfinity, target.Support.Max);

            expected = new double[] { 0, 0, 0.128807284216628, 0.0464472549459518, 0.0167486451184392, 0.0060394766844637, 0.00217780473371089, 0.000785305367661126, 0.000283177142069361, 0.000102112244602884, 3.68211587335158E-05 };
            actual = Vector.Interval(-50, 1.0, steps: 11).Apply(target.ProbabilityDensityFunction);
            str = actual.ToCSharp();
            Assert.IsTrue(expected.IsEqual(actual, 1e-10));

            expected = new double[] { 0, 0, 0.355963578916859, 0.767763725270241, 0.916256774407804, 0.969802616577682, 0.989110976331446, 0.996073473161694, 0.998584114289653, 0.999489438776986, 0.999815894206332 };
            actual = Vector.Interval(-50.0, 1.0, steps: 11).Apply((x) => target.DistributionFunction(x));
            str = actual.ToCSharp();
            Assert.IsTrue(expected.IsEqual(actual, 1e-10));

            expected = new double[] { -42, -41.4731974067983, -40.8842822434175, -40.2166252803068, -39.4458718811702, -38.534264076262, -37.4185463380999, -35.9801359783219, -33.9528104382541, -30.4870745323964, Double.PositiveInfinity };
            actual = Vector.Interval(0.0, 1.0, steps: 11).Apply((x) => target.InverseDistributionFunction(x));
            str = actual.ToCSharp();
            Assert.IsTrue(expected.IsEqual(actual, 1e-6));
        }

        [Test]
        public void negative_ksi()
        {
            var target = new GeneralizedParetoDistribution(-42, 5, -4.2);
            double[] expected, actual;
            string str;

            Assert.AreEqual(-42, target.Support.Min);
            Assert.AreEqual(-40.80952380952381, target.Support.Max);

            expected = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.2, 0.302883303831534, 0.808006344307731, 0, 0 };
            actual = Vector.Interval(-50.0, -40.0, steps: 21).Apply(target.ProbabilityDensityFunction);
            str = actual.ToCSharp();
            Assert.IsTrue(expected.IsEqual(actual, 1e-10));

            expected = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.121638418888552, 0.353594924553815, 1, 1 };
            actual = Vector.Interval(-50.0, -40.0, steps: 21).Apply((x) => target.DistributionFunction(x));
            str = actual.ToCSharp();
            Assert.IsTrue(expected.IsEqual(actual, 1e-10));

            expected = new double[] { -42, -41.5743086286744, -41.275859522191, -41.0756773630594, -40.9488253643646, -40.8742969157203, -40.8348968590133, -40.8171031247911, -40.8109043421369, -40.809599125212, -40.8095238095238 };
            actual = Vector.Interval(0.0, 1.0, steps: 11).Apply((x) => target.InverseDistributionFunction(x));
            str = actual.ToCSharp();
            Assert.IsTrue(expected.IsEqual(actual, 1e-6));
        }

        [Test]
        public void positive_ksi()
        {
            var target = new GeneralizedParetoDistribution(-42, 5, +4.2);
            double[] expected, actual;
            string str;

            Assert.AreEqual(-42, target.Support.Min);
            Assert.AreEqual(Double.PositiveInfinity, target.Support.Max);

            expected = new double[] { 0, 0, 0.0547351364089877, 0.0175660091778962, 0.00981130998772365, 0.0066129057162198, 0.00490525155264395, 0.00385679017970578, 0.0031536488437678, 0.00265243027038563, 0.00227879808261139 };
            actual = Vector.Interval(-50, 1.0, steps: 11).Apply(target.ProbabilityDensityFunction);
            str = actual.ToCSharp();
            Assert.IsTrue(expected.IsEqual(actual, 1e-10));

            expected = new double[] { 0, 0, 0.220571657536015, 0.373596112716222, 0.439970425900734, 0.480886901276745, 0.509867264859816, 0.532017079594501, 0.549785091063708, 0.564523998208087, 0.577055075867326 };
            actual = Vector.Interval(-50.0, 1.0, steps: 11).Apply((x) => target.DistributionFunction(x));
            str = actual.ToCSharp();
            Assert.IsTrue(expected.IsEqual(actual, 1e-10));

            expected = new double[] { -42, -41.3373620317749, -40.1513917163923, -37.8656069703902, -33.0166232588751, -21.3105075238593, 12.6653847611095, 143.796536677696, 983.394093115206, 18824.5856242939, Double.PositiveInfinity };
            actual = Vector.Interval(0.0, 1.0, steps: 11).Apply((x) => target.InverseDistributionFunction(x));
            str = actual.ToCSharp();
            Assert.IsTrue(expected.IsEqual(actual, 1e-6));
        }
    }
}
