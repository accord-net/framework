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
    using Accord.Statistics.Distributions.Multivariate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Math;
    using Accord.MachineLearning;
    using Accord.Statistics.Formats;
    using Accord.Tests.Statistics.Properties;
    using System.IO;
    using System.Data;

    [TestClass()]
    public class MultivariateMixtureDistributionTest
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
            MultivariateNormalDistribution[] components = new MultivariateNormalDistribution[2];
            components[0] = new MultivariateNormalDistribution(new double[] { 2 }, new double[,] { { 1 } });
            components[1] = new MultivariateNormalDistribution(new double[] { 5 }, new double[,] { { 1 } });

            var mixture = new MultivariateMixture<MultivariateNormalDistribution>(components);

            double[] expected = { 0.5, 0.5 };

            Assert.IsTrue(expected.IsEqual(mixture.Coefficients));
            Assert.AreEqual(components, mixture.Components);
        }

        [TestMethod()]
        public void ProbabilityDensityFunctionTest()
        {
            MultivariateNormalDistribution[] components = new MultivariateNormalDistribution[2];
            components[0] = new MultivariateNormalDistribution(new double[] { 2 }, new double[,] { { 1 } });
            components[1] = new MultivariateNormalDistribution(new double[] { 5 }, new double[,] { { 1 } });

            double[] coefficients = { 0.3, 0.7 };
            var mixture = new MultivariateMixture<MultivariateNormalDistribution>(coefficients, components);

            double[] x = { 1.2 };

            double expected =
                0.3 * components[0].ProbabilityDensityFunction(x) +
                0.7 * components[1].ProbabilityDensityFunction(x);

            double actual = mixture.ProbabilityDensityFunction(x);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void LogProbabilityDensityFunctionTest()
        {
            MultivariateNormalDistribution[] components = new MultivariateNormalDistribution[2];
            components[0] = new MultivariateNormalDistribution(new double[] { 2 }, new double[,] { { 1 } });
            components[1] = new MultivariateNormalDistribution(new double[] { 5 }, new double[,] { { 1 } });

            double[] coefficients = { 0.3, 0.7 };
            var mixture = new MultivariateMixture<MultivariateNormalDistribution>(coefficients, components);

            double[] x = { 1.2 };

            double expected = System.Math.Log(
                0.3 * components[0].ProbabilityDensityFunction(x) +
                0.7 * components[1].ProbabilityDensityFunction(x));

            double actual = mixture.LogProbabilityDensityFunction(x);

            Assert.AreEqual(expected, actual, 1e-10);
            Assert.IsFalse(double.IsNaN(actual));
        }

        [TestMethod()]
        public void FitTest()
        {
            double[] coefficients = { 0.50, 0.50 };

            MultivariateNormalDistribution[] components = new MultivariateNormalDistribution[2];
            components[0] = new MultivariateNormalDistribution(new double[] { 2 }, new double[,] { { 1 } });
            components[1] = new MultivariateNormalDistribution(new double[] { 5 }, new double[,] { { 1 } });

            var target = new MultivariateMixture<MultivariateNormalDistribution>(coefficients, components);

            double[][] values = { new double[] { 0 },
                                  new double[] { 1 }, 
                                  new double[] { 1 },
                                  new double[] { 0 },
                                  new double[] { 1 },
                                  new double[] { 6 },
                                  new double[] { 6 },
                                  new double[] { 5 },
                                  new double[] { 7 },
                                  new double[] { 5 } };

            double[][] part1 = values.Submatrix(0, 4);
            double[][] part2 = values.Submatrix(5, 9);



            target.Fit(values);


            var mean1 = Accord.Statistics.Tools.Mean(part1);
            var var1 = Accord.Statistics.Tools.Variance(part1);
            Assert.AreEqual(mean1[0], target.Components[0].Mean[0], 1e-5);
            Assert.AreEqual(var1[0], target.Components[0].Variance[0], 1e-5);

            var mean2 = Accord.Statistics.Tools.Mean(part2);
            var var2 = Accord.Statistics.Tools.Variance(part2);
            Assert.AreEqual(mean2[0], target.Components[1].Mean[0], 1e-5);
            Assert.AreEqual(var2[0], target.Components[1].Variance[0], 1e-5);


            var expectedMean = Accord.Statistics.Tools.Mean(values);
            var expectedVar = Accord.Statistics.Tools.Covariance(values);

            var actualMean = target.Mean;
            var actualVar = target.Covariance;

            Assert.AreEqual(expectedMean[0], actualMean[0], 0.0000001);
            // Assert.AreEqual(expectedVar[0, 0], actualVar[0, 0], 0.0000001);
        }

        [TestMethod()]
        public void FitTest2()
        {
            double[] coefficients = { 0.50, 0.50 };

            MultivariateNormalDistribution[] components = new MultivariateNormalDistribution[2];
            components[0] = new MultivariateNormalDistribution(new double[] { 2 }, new double[,] { { 1 } });
            components[1] = new MultivariateNormalDistribution(new double[] { 5 }, new double[,] { { 1 } });

            var target = new MultivariateMixture<MultivariateNormalDistribution>(coefficients, components);

            double[][] values = { new double[] { 2512512312 },
                                  new double[] { 1 }, 
                                  new double[] { 1 },
                                  new double[] { 0 },
                                  new double[] { 1 },
                                  new double[] { 6 },
                                  new double[] { 6 },
                                  new double[] { 5 },
                                  new double[] { 7 },
                                  new double[] { 5 } };

            double[] weights = { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            weights = weights.Divide(weights.Sum());

            double[][] part1 = values.Submatrix(1, 4);
            double[][] part2 = values.Submatrix(5, 9);


            target.Fit(values, weights);

            var mean1 = Accord.Statistics.Tools.Mean(part1);
            var var1 = Accord.Statistics.Tools.Variance(part1);
            Assert.AreEqual(mean1[0], target.Components[0].Mean[0], 1e-5);
            Assert.AreEqual(var1[0], target.Components[0].Variance[0], 1e-5);

            var mean2 = Accord.Statistics.Tools.Mean(part2);
            var var2 = Accord.Statistics.Tools.Variance(part2);
            Assert.AreEqual(mean2[0], target.Components[1].Mean[0], 1e-5);
            Assert.AreEqual(var2[0], target.Components[1].Variance[0], 1e-5);


            var expectedMean = Accord.Statistics.Tools.WeightedMean(values, weights);
            var expectedVar = Accord.Statistics.Tools.WeightedCovariance(values, weights);

            var actualMean = target.Mean;
            var actualVar = target.Covariance;

            Assert.AreEqual(expectedMean[0], actualMean[0], 0.0000001);
            Assert.AreEqual(expectedVar[0, 0], actualVar[0, 0], 0.68);
        }

        [TestMethod]
        public void MixtureWeightsFitTest()
        {
            // Randomly initialize some mixture components
            MultivariateNormalDistribution[] components = new MultivariateNormalDistribution[2];
            components[0] = new MultivariateNormalDistribution(new double[] { 2 }, new double[,] { { 1 } });
            components[1] = new MultivariateNormalDistribution(new double[] { 5 }, new double[,] { { 1 } });

            // Create an initial mixture
            var mixture = new MultivariateMixture<MultivariateNormalDistribution>(components);

            // Now, suppose we have a weighted data
            // set. Those will be the input points:

            double[][] points = new double[] { 0, 3, 1, 7, 3, 5, 1, 2, -1, 2, 7, 6, 8, 6 } // (14 points)
                .ToArray();

            // And those are their respective unormalized weights:
            double[] weights = { 1, 1, 1, 2, 2, 1, 1, 1, 2, 1, 2, 3, 1, 1 }; // (14 weights)

            // Let's normalize the weights so they sum up to one:
            weights = weights.Divide(weights.Sum());

            // Now we can fit our model to the data:
            mixture.Fit(points, weights);   // done!

            // Our model will be:
            double mean1 = mixture.Components[0].Mean[0]; // 1.41126
            double mean2 = mixture.Components[1].Mean[0]; // 6.53301

            // If we need the GaussianMixtureModel functionality, we can
            // use the estimated mixture to initialize a new model:
            GaussianMixtureModel gmm = new GaussianMixtureModel(mixture);

            Assert.AreEqual(mean1, gmm.Gaussians[0].Mean[0]);
            Assert.AreEqual(mean2, gmm.Gaussians[1].Mean[0]);

            Assert.AreEqual(1.4112610766836404, mean1, 1e-10);
            Assert.AreEqual(6.5330177004151082, mean2, 1e-10);

            Assert.AreEqual(mixture.Coefficients[0], gmm.Gaussians[0].Proportion);
            Assert.AreEqual(mixture.Coefficients[1], gmm.Gaussians[1].Proportion);
        }

        [TestMethod]
        public void MixtureWeightsFitTest2()
        {
            MemoryStream stream = new MemoryStream(Resources.CircleWithWeights);

            ExcelReader reader = new ExcelReader(stream, xlsx: false);

            DataTable table = reader.GetWorksheet("Sheet1");

            double[,] matrix = table.ToMatrix();

            double[][] points = matrix.Submatrix(null, 0, 1).ToArray();
            double[] weights = matrix.GetColumn(2);

            // Randomly initialize some mixture components
            MultivariateNormalDistribution[] components = new MultivariateNormalDistribution[2];
            components[0] = new MultivariateNormalDistribution(new double[] { 0, 1 }, Matrix.Identity(2));
            components[1] = new MultivariateNormalDistribution(new double[] { 1, 0 }, Matrix.Identity(2));

            // Create an initial mixture
            var mixture = new MultivariateMixture<MultivariateNormalDistribution>(components);

            mixture.Fit(points, weights);

            // Our model will be:
            double mean00 = mixture.Components[0].Mean[0];
            double mean01 = mixture.Components[0].Mean[1];
            double mean10 = mixture.Components[1].Mean[0];
            double mean11 = mixture.Components[1].Mean[1];

            Assert.AreEqual(-0.11704994950834195, mean00, 1e-10);
            Assert.AreEqual(0.11603470123007256, mean01, 1e-10);
            Assert.AreEqual(0.11814483652855159, mean10, 1e-10);
            Assert.AreEqual(-0.12029275652994373, mean11, 1e-10);
        }

    }
}
