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
    using Accord.Statistics.Distributions.Multivariate;
    using NUnit.Framework;
    using System;
    using Accord.Math;
    using Accord.Statistics.Distributions.Fitting;

    [TestFixture]
    public class JointDistributionTest
    {

        [Test]
        public void JointDistributionConstructorTest()
        {
            int[] symbols = { 3, 5 };
            JointDistribution target = new JointDistribution(symbols);

            double[] p = target.Frequencies;

            Assert.AreEqual(3 * 5, p.Length);

            for (int i = 0; i < p.Length; i++)
                p[i] = i;

            double actual;

            actual = target.ProbabilityMassFunction(new int[] { 0, 0 });
            Assert.AreEqual(0, actual);

            actual = target.ProbabilityMassFunction(new int[] { 0, 1 });
            Assert.AreEqual(1, actual);

            actual = target.ProbabilityMassFunction(new int[] { 1, 0 });
            Assert.AreEqual(5, actual);

            actual = target.ProbabilityMassFunction(new int[] { 2, 4 });
            Assert.AreEqual(14, actual);
        }

        [Test]
        public void JointDistributionConstructorTest2()
        {
            int[] symbols = { 3, 5, 2 };
            JointDistribution target = new JointDistribution(symbols);

            double[] p = target.Frequencies;

            Assert.AreEqual(3 * 5 * 2, p.Length);

            for (int i = 0; i < p.Length; i++)
                p[i] = i;

            double actual;

            actual = target.ProbabilityMassFunction(new int[] { 0, 0, 0 });
            Assert.AreEqual(0, actual);

            actual = target.ProbabilityMassFunction(new int[] { 0, 0, 1 });
            Assert.AreEqual(1, actual);

            actual = target.ProbabilityMassFunction(new int[] { 0, 1, 0 });
            Assert.AreEqual(2, actual);

            actual = target.ProbabilityMassFunction(new int[] { 0, 1, 1 });
            Assert.AreEqual(3, actual);

            actual = target.ProbabilityMassFunction(new int[] { 0, 2, 0 });
            Assert.AreEqual(4, actual);

            actual = target.ProbabilityMassFunction(new int[] { 0, 2, 1 });
            Assert.AreEqual(5, actual);

            actual = target.ProbabilityMassFunction(new int[] { 2, 4, 1 });
            Assert.AreEqual(29, actual);
        }

        [Test]
        public void FitTest()
        {
            int[] symbols = { 3, 5 };
            JointDistribution target = new JointDistribution(symbols);

            double[][] observations =
            {
                new double[] { 0, 0 },
                new double[] { 1, 1 },
                new double[] { 2, 1 },
                new double[] { 0, 0 },
            };

            target.Fit(observations);

            double[] p = target.Frequencies;

            double actual;

            actual = target.ProbabilityMassFunction(new[] { 0, 0 });
            Assert.AreEqual(0.5, actual);

            actual = target.ProbabilityMassFunction(new[] { 1, 1 });
            Assert.AreEqual(0.25, actual);

            actual = target.ProbabilityMassFunction(new[] { 2, 1 });
            Assert.AreEqual(0.25, actual);
        }

        [Test]
        public void FitTest2()
        {
            int[] symbols = { 3, 5 };
            JointDistribution target = new JointDistribution(symbols);

            double[][] observations =
            {
                new double[] { 0, 0 },
                new double[] { 1, 1 },
                new double[] { 2, 1 },
            };

            double[] weights = { 2, 1, 1 };

            target.Fit(observations, weights);

            double[] p = target.Frequencies;

            double actual;

            actual = target.ProbabilityMassFunction(new[] { 0, 0 });
            Assert.AreEqual(0.5, actual);

            actual = target.ProbabilityMassFunction(new[] { 1, 1 });
            Assert.AreEqual(0.25, actual);

            actual = target.ProbabilityMassFunction(new[] { 2, 1 });
            Assert.AreEqual(0.25, actual);

        }


        [Test]
        public void FitTest5()
        {
            JointDistribution target = new JointDistribution(new[] { 4 });
            double[] values = { 0.00, 1.00, 2.00, 3.00 };
            double[] weights = { 0.25, 0.25, 0.25, 0.25 };

            target.Fit(Jagged.ColumnVector(values), weights);

            double[] expected = { 0.25, 0.25, 0.25, 0.25 };
            double[] actual = target.Frequencies;

            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void FitTest6()
        {
            double[] expected = { 0.50, 0.00, 0.25, 0.25 };

            JointDistribution target;

            double[] values = { 0.00, 2.00, 3.00 };
            double[] weights = { 0.50, 0.25, 0.25 };
            target = new JointDistribution(new[] { 4 });
            target.Fit(Jagged.ColumnVector(values), weights);
            double[] actual = target.Frequencies;

            Assert.IsTrue(Matrix.IsEqual(expected, actual));

            // --

            double[] values2 = { 0.00, 0.00, 2.00, 3.00 };
            double[] weights2 = { 0.25, 0.25, 0.25, 0.25 };
            target = new JointDistribution(new[] { 4 });
            target.Fit(Jagged.ColumnVector(values2), weights2);
            double[] actual2 = target.Frequencies;
            Assert.IsTrue(Matrix.IsEqual(expected, actual2));
        }

        [Test]
        public void FitTest3()
        {
            JointDistribution target = new JointDistribution(new[] { -1 }, new[] { 4 });
            double[] values = { 0.00, 1.00, 2.00, 3.00 };
            double[] weights = { 0.25, 0.25, 0.25, 0.25 };

            target.Fit(Jagged.ColumnVector(values).Subtract(1), weights);

            double[] expected = { 0.25, 0.25, 0.25, 0.25 };
            double[] actual = target.Frequencies;

            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void FitTest4()
        {
            double[] expected = { 0.50, 0.00, 0.25, 0.25 };

            JointDistribution target;

            double[] values = { 0.00, 2.00, 3.00 };
            double[] weights = { 0.50, 0.25, 0.25 };
            target = new JointDistribution(new[] { -1 }, new[] { 4 });
            target.Fit(Jagged.ColumnVector(values).Subtract(1), weights);
            double[] actual = target.Frequencies;

            Assert.IsTrue(Matrix.IsEqual(expected, actual));

            // --

            double[] values2 = { 0.00, 0.00, 2.00, 3.00 };
            double[] weights2 = { 0.25, 0.25, 0.25, 0.25 };
            target = new JointDistribution(new[] { -1 }, new[] { 4 });
            target.Fit(Jagged.ColumnVector(values2).Subtract(1), weights2);
            double[] actual2 = target.Frequencies;
            Assert.IsTrue(Matrix.IsEqual(expected, actual2));
        }

        [Test]
        public void FitTest_vector_inputs()
        {
            double[] expected = { 0.50, 0.00, 0.25, 0.25 };

            JointDistribution target;

            double[] values = { 0.00, 2.00, 3.00 };
            double[] weights = { 0.50, 0.25, 0.25 };
            target = new JointDistribution(new[] { 4 });
            target.Fit(Jagged.ColumnVector(values), weights);
            double[] actual = target.Frequencies;

            Assert.IsTrue(Matrix.IsEqual(expected, actual));
        }

        [Test]
        public void DistributionFunctionTest()
        {
            var target = new JointDistribution(new[] { 0.1, 0.4, 0.5 });

            double actual;

            actual = target.DistributionFunction(new[] { 0 });
            Assert.AreEqual(0.1, actual, 1e-6);

            actual = target.DistributionFunction(new[] { 1 });
            Assert.AreEqual(0.5, actual, 1e-6);

            actual = target.DistributionFunction(new[] { 2 });
            Assert.AreEqual(1.0, actual, 1e-6);

            actual = target.DistributionFunction(new[] { 3 });
            Assert.AreEqual(1.0, actual, 1e-6);
        }

        [Test]
        public void MeanTest()
        {
            var target = new JointDistribution(new[] { 0.1, 0.4, 0.5 });
            double expected = 0 * 0.1 + 1 * 0.4 + 2 * 0.5;
            double actual = target.Mean[0];

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MeanTest2()
        {
            var target = new JointDistribution(new [] { 42 }, new[] { 0.1, 0.4, 0.5 });
            double expected = 42 * 0.1 + 43 * 0.4 + 44 * 0.5;
            double actual = target.Mean[0];

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MeanTest3()
        {
            var target = new JointDistribution(new[] { 2 }, new [] { 0.5, 0.5 });
            double expected = (2.0 + 3.0) / 2.0;
            double actual = target.Mean[0];

            Assert.AreEqual(expected, actual);
        }



        [Test]
        public void UniformTest()
        {
            int a = 2;
            int b = 5;
            int n = b - a + 1;

            // Wikipedia definitions
            double expectedMean = (a + b) / 2.0;
            double expectedVar = (System.Math.Pow(b - a + 1, 2) - 1) / 12.0;
            double p = 1.0 / n;


            JointDistribution dist = JointDistribution.Uniform(1, a, b);

            Assert.AreEqual(expectedMean, dist.Mean[0]);
            // Assert.AreEqual(expectedVar, dist.Variance);
            Assert.AreEqual(n, dist.Frequencies.Length);


            Assert.AreEqual(0, dist.ProbabilityMassFunction(new[] { 0 }));
            Assert.AreEqual(0, dist.ProbabilityMassFunction(new[] { 1 }));
            Assert.AreEqual(p, dist.ProbabilityMassFunction(new[] { 2 }));
            Assert.AreEqual(p, dist.ProbabilityMassFunction(new[] { 3 }));
            Assert.AreEqual(p, dist.ProbabilityMassFunction(new[] { 4 }));
            Assert.AreEqual(p, dist.ProbabilityMassFunction(new[] { 5 }));
            Assert.AreEqual(0, dist.ProbabilityMassFunction(new[] { 6 }));
            Assert.AreEqual(0, dist.ProbabilityMassFunction(new[] { 7 }));
        }

        [Test]
        public void ProbabilityMassFunctionTest()
        {
            JointDistribution dist = JointDistribution.Uniform(1, 2, 5);
            double p = 0.25;
            Assert.AreEqual(0, dist.ProbabilityMassFunction(new[] { 0 }));
            Assert.AreEqual(0, dist.ProbabilityMassFunction(new[] { 1 }));
            Assert.AreEqual(p, dist.ProbabilityMassFunction(new[] { 2 }));
            Assert.AreEqual(p, dist.ProbabilityMassFunction(new[] { 3 }));
            Assert.AreEqual(p, dist.ProbabilityMassFunction(new[] { 4 }));
            Assert.AreEqual(p, dist.ProbabilityMassFunction(new[] { 5 }));
            Assert.AreEqual(0, dist.ProbabilityMassFunction(new[] { 6 }));
            Assert.AreEqual(0, dist.ProbabilityMassFunction(new[] { 7 }));
        }

        [Test]
        public void LogProbabilityMassFunctionTest()
        {
            JointDistribution dist = JointDistribution.Uniform(1, 2, 5);

            double p = System.Math.Log(0.25);
            double l = System.Math.Log(0);

            Assert.AreEqual(l, dist.LogProbabilityMassFunction(new[] { 0 }));
            Assert.AreEqual(l, dist.LogProbabilityMassFunction(new[] { 1 }));
            Assert.AreEqual(p, dist.LogProbabilityMassFunction(new[] { 2 }));
            Assert.AreEqual(p, dist.LogProbabilityMassFunction(new[] { 3 }));
            Assert.AreEqual(p, dist.LogProbabilityMassFunction(new[] { 4 }));
            Assert.AreEqual(p, dist.LogProbabilityMassFunction(new[] { 5 }));
            Assert.AreEqual(l, dist.LogProbabilityMassFunction(new[] { 6 }));
            Assert.AreEqual(l, dist.LogProbabilityMassFunction(new[] { 7 }));
        }

        [Test]
        public void MarginalTest()
        {
            // Example from https://en.wikipedia.org/wiki/Marginal_distribution

            int[][] data =
            {
                new[] { 4, 2, 1, 1 },
                new[] { 2, 4, 1, 1 },
                new[] { 2, 2, 2, 2 },
                new[] { 8, 0, 0, 0 },
            };

            double[][] frequencies = data.Divide(32);
           
            var joint = new JointDistribution(frequencies.ToMatrix());
            double[] x = joint.MarginalDistributionFunction(0);
            double[] y = joint.MarginalDistributionFunction(1);

            double[][] expected =
            {
                new[] { 16 / 32.0, 8 / 32.0, 4 / 32.0, 4 / 32.0 },
                new[] { 8 / 32.0, 8 / 32.0, 8 / 32.0, 8 / 32.0 }
            };

            Assert.IsTrue(x.IsEqual(expected[0]));
            Assert.IsTrue(y.IsEqual(expected[1]));

            for (int i = 0; i < 4; i++)
            {
                double a = joint.MarginalDistributionFunction(0, i);
                Assert.AreEqual(a, expected[0][i]);
            }

            for (int i = 0; i < 4; i++)
            {
                double a = joint.MarginalDistributionFunction(1, i);
                Assert.AreEqual(a, expected[1][i]);
            }
        }
    }
}
