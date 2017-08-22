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
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class IndependentTest
    {

        [Test]
        public void ConstructorTest()
        {
            var p1 = new NormalDistribution(4.2, 1);
            var p2 = new NormalDistribution(7.0, 2);

            Independent<NormalDistribution> target = new Independent<NormalDistribution>(p1, p2);

            Assert.AreEqual(target.Components[0], p1);
            Assert.AreEqual(target.Components[1], p2);

            Assert.AreEqual(2, target.Dimension);

            Assert.AreEqual(4.2, target.Mean[0]);
            Assert.AreEqual(7.0, target.Mean[1]);

            Assert.AreEqual(1, target.Variance[0]);
            Assert.AreEqual(4, target.Variance[1]);

            Assert.AreEqual(1, target.Covariance[0, 0]);
            Assert.AreEqual(4, target.Covariance[1, 1]);
            Assert.AreEqual(0, target.Covariance[0, 1]);
            Assert.AreEqual(0, target.Covariance[1, 0]);

            var text = target.ToString("N2", System.Globalization.CultureInfo.InvariantCulture);

            Assert.AreEqual("Independent(x0, x1; N(x0; μ = 4.20, σ² = 1.00) + N(x1; μ = 7.00, σ² = 4.00))", text);
        }

        [Test]
        public void ProbabilityFunctionTest()
        {
            var p1 = new NormalDistribution(4.2, 1);
            var p2 = new NormalDistribution(7.0, 2);

            Independent<NormalDistribution> target = new Independent<NormalDistribution>(p1, p2);

            double[] x;
            double actual, expected;

            x = new double[] { 4.2, 7.0 };
            actual = target.ProbabilityDensityFunction(x);
            expected = p1.ProbabilityDensityFunction(x[0]) * p2.ProbabilityDensityFunction(x[1]);
            Assert.AreEqual(expected, actual, 1e-10);
            Assert.IsFalse(double.IsNaN(actual));

            x = new double[] { 0.0, 0.0 };
            actual = target.ProbabilityDensityFunction(x);
            expected = p1.ProbabilityDensityFunction(x[0]) * p2.ProbabilityDensityFunction(x[1]);
            Assert.AreEqual(expected, actual, 1e-10);
            Assert.IsFalse(double.IsNaN(actual));

            x = new double[] { 7.0, 4.2 };
            actual = target.ProbabilityDensityFunction(x);
            expected = p1.ProbabilityDensityFunction(x[0]) * p2.ProbabilityDensityFunction(x[1]);
            Assert.AreEqual(expected, actual, 1e-10);
            Assert.IsFalse(double.IsNaN(actual));
        }

        [Test]
        public void CumulativeFunctionTest()
        {
            var p1 = new NormalDistribution(4.2, 1);
            var p2 = new NormalDistribution(7.0, 2);

            Independent<NormalDistribution> target = new Independent<NormalDistribution>(p1, p2);

            double[] x;
            double actual, expected;

            x = new double[] { 4.2, 7.0 };
            actual = target.DistributionFunction(x);
            expected = p1.DistributionFunction(x[0]) * p2.DistributionFunction(x[1]);
            Assert.AreEqual(expected, actual);

            x = new double[] { 0.0, 0.0 };
            actual = target.DistributionFunction(x);
            expected = p1.DistributionFunction(x[0]) * p2.DistributionFunction(x[1]);
            Assert.AreEqual(expected, actual);

            x = new double[] { 7.0, 4.2 };
            actual = target.DistributionFunction(x);
            expected = p1.DistributionFunction(x[0]) * p2.DistributionFunction(x[1]);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FitTest()
        {
            double[][] data =
            {
                new double[] { 1, 8 },
                new double[] { 2, 6 },
                new double[] { 5, 7 },
                new double[] { 3, 9 },
            };


            var p1 = new NormalDistribution(0, 1);
            var p2 = new NormalDistribution(0, 1);

            Independent<NormalDistribution> target = new Independent<NormalDistribution>(p1, p2);

            target.Fit(data);

            Assert.AreEqual(2.75, target.Mean[0]);
            Assert.AreEqual(7.50, target.Mean[1]);

            Assert.AreEqual(2.91666, target.Variance[0], 1e-5);
            Assert.AreEqual(1.66666, target.Variance[1], 1e-5);

            Assert.AreEqual(target.Variance[0], target.Covariance[0, 0]);
            Assert.AreEqual(target.Variance[1], target.Covariance[1, 1]);
            Assert.AreEqual(0, target.Covariance[0, 1]);
            Assert.AreEqual(0, target.Covariance[1, 0]);
        }

        [Test]
        public void FittingNoOptionsTest()
        {
            double[][] data1 =
            {
                new double[] { 0, 8 },
                new double[] { 0, 6 },
                new double[] { 0, 7 },
                new double[] { 0, 9 },
            };

            double[][] data2 =
            {
                new double[] { 8, 0 },
                new double[] { 6, 0 },
                new double[] { 7, 0 },
                new double[] { 9, 0 },
            };


            var p1 = new NormalDistribution(0, 1);
            var p2 = new NormalDistribution(0, 1);

            bool thrown1 = false;
            bool thrown2 = false;

            try
            {
                var target = new Independent<NormalDistribution>(p1, p2);
                target.Fit(data1);
            }
            catch (ArgumentException)
            {
                thrown1 = true;
            }

            try
            {
                var target = new Independent<NormalDistribution>(p1, p2);
                target.Fit(data2);
            }
            catch (ArgumentException)
            {
                thrown2 = true;
            }

            Assert.IsTrue(thrown1);
            Assert.IsTrue(thrown2);
        }

        [Test]
        public void FittingOptionsTest()
        {
            double[][] data1 =
            {
                new double[] { 0, 8 },
                new double[] { 0, 6 },
                new double[] { 0, 7 },
                new double[] { 0, 9 },
            };

            double[][] data2 =
            {
                new double[] { 8, 0 },
                new double[] { 6, 0 },
                new double[] { 7, 0 },
                new double[] { 9, 0 },
            };


            var p1 = new NormalDistribution(0, 1);
            var p2 = new NormalDistribution(0, 1);

            {
                Independent<NormalDistribution> target = new Independent<NormalDistribution>(p1, p2);

                target.Fit(data1, new IndependentOptions()
                {
                    InnerOption = new NormalOptions()
                    {
                        Regularization = 1e-5
                    }
                });

                Assert.AreEqual(0.00, target.Mean[0]);
                Assert.AreEqual(7.50, target.Mean[1]);

                Assert.AreEqual(1e-5, target.Variance[0]);
                Assert.AreEqual(1.66666, target.Variance[1], 1e-5);
            }

            {
                Independent<NormalDistribution> target = new Independent<NormalDistribution>(p1, p2);

                target.Fit(data2, new IndependentOptions()
                {
                    InnerOption = new NormalOptions()
                    {
                        Regularization = 1e-5
                    }
                });

                Assert.AreEqual(7.5, target.Mean[0]);
                Assert.AreEqual(0, target.Mean[1]);

                Assert.AreEqual(1.66666, target.Variance[0], 1e-5);
                Assert.AreEqual(1e-5, target.Variance[1]);
            }
        }

    }
}