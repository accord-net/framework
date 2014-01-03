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
    using Accord.Math;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.MachineLearning;
    using System.Globalization;

    [TestClass()]
    public class MixtureDistributionTest
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
            // Create a new mixture containing two Normal distributions
            Mixture<NormalDistribution> mix = new Mixture<NormalDistribution>(
                new NormalDistribution(2, 1), new NormalDistribution(5, 1));

            // Common measures
            double mean   = mix.Mean;     // 3.5
            double median = mix.Median;   // 3.4999998506015895
            double var    = mix.Variance; // 3.25

            // Cumulative distribution functions
            double cdf = mix.DistributionFunction(x: 4.2);              // 0.59897597553494908
            double ccdf = mix.ComplementaryDistributionFunction(x: 4.2); // 0.40102402446505092

            // Probability mass functions
            double pmf1 = mix.ProbabilityDensityFunction(x: 1.2); // 0.14499174984363708
            double pmf2 = mix.ProbabilityDensityFunction(x: 2.3); // 0.19590437513747333
            double pmf3 = mix.ProbabilityDensityFunction(x: 3.7); // 0.13270883471234715
            double lpmf = mix.LogProbabilityDensityFunction(x: 4.2); // -1.8165661905848629

            // Quantile function
            double icdf1 = mix.InverseDistributionFunction(p: 0.17); // 1.5866611690305095
            double icdf2 = mix.InverseDistributionFunction(p: 0.46); // 3.1968506765456883
            double icdf3 = mix.InverseDistributionFunction(p: 0.87); // 5.6437596300843076

            // Hazard (failure rate) functions
            double hf = mix.HazardFunction(x: 4.2); // 0.40541978256972522
            double chf = mix.CumulativeHazardFunction(x: 4.2); // 0.91373394208601633

            // String representation:

            // Mixture(x; 0.5 * N(x; μ = 5, σ² = 1) + 0.5 * N(x; μ = 5, σ² = 1))
            string str = mix.ToString(CultureInfo.InvariantCulture);



            Assert.AreEqual(3.5, mean);
            Assert.AreEqual(3.4999998506015895, median);
            Assert.AreEqual(3.25, var);
            Assert.AreEqual(0.91373394208601633, chf, 1e-10);
            Assert.AreEqual(0.59897597553494908, cdf);
            Assert.AreEqual(0.14499174984363708, pmf1);
            Assert.AreEqual(0.19590437513747333, pmf2);
            Assert.AreEqual(0.13270883471234715, pmf3);
            Assert.AreEqual(-1.8165661905848629, lpmf);
            Assert.AreEqual(0.40541978256972522, hf);
            Assert.AreEqual(0.40102402446505092, ccdf);
            Assert.AreEqual(1.5866611690305095, icdf1);
            Assert.AreEqual(3.1968506765456883, icdf2);
            Assert.AreEqual(5.6437596300843076, icdf3);
            Assert.AreEqual("Mixture(x; 0.5*N(x; μ = 5, σ² = 1) + 0.5*N(x; μ = 5, σ² = 1))", str);
        }

        [TestMethod()]
        public void ConstructorTest1()
        {
            NormalDistribution[] components = new NormalDistribution[2];
            components[0] = new NormalDistribution(2, 1);
            components[1] = new NormalDistribution(5, 1);

            var mixture = new Mixture<NormalDistribution>(components);

            double[] expected = { 0.5, 0.5 };

            Assert.IsTrue(expected.IsEqual(mixture.Coefficients));
            Assert.AreEqual(components, mixture.Components);
        }

        [TestMethod()]
        public void ConstructorTest2()
        {
            // Create a new mixture containing two Normal distributions
            Mixture<NormalDistribution> mix = new Mixture<NormalDistribution>(
                new NormalDistribution(2, 1), new NormalDistribution(5, 1));

            // Compute in reverse order
            double var = mix.Variance; // 3.25
            double median = mix.Median;   // 3.4999998506015895
            double mean = mix.Mean;     // 3.5

            Assert.AreEqual(3.5, mean);
            Assert.AreEqual(3.4999998506015895, median);
            Assert.AreEqual(3.25, var);
        }

        [TestMethod()]
        public void FitTest()
        {
            double[] coefficients = { 0.50, 0.50 };

            NormalDistribution[] components = new NormalDistribution[2];
            components[0] = new NormalDistribution(2, 1);
            components[1] = new NormalDistribution(5, 1);

            var target = new Mixture<NormalDistribution>(coefficients, components);

            double[] values = { 0, 1, 1, 0, 1, 6, 6, 5, 7, 5 };
            double[] part1 = values.Submatrix(0, 4);
            double[] part2 = values.Submatrix(5, 9);

            MixtureOptions options = new MixtureOptions() { Threshold = 1e-10 };

            target.Fit(values, options);
            var actual = target;

            var mean1 = Accord.Statistics.Tools.Mean(part1);
            var var1 = Accord.Statistics.Tools.Variance(part1);
            Assert.AreEqual(mean1, actual.Components[0].Mean, 1e-6);
            Assert.AreEqual(var1, actual.Components[0].Variance, 1e-6);

            var mean2 = Accord.Statistics.Tools.Mean(part2);
            var var2 = Accord.Statistics.Tools.Variance(part2);
            Assert.AreEqual(mean2, actual.Components[1].Mean, 1e-6);
            Assert.AreEqual(var2, actual.Components[1].Variance, 1e-5);

            var expectedMean = Accord.Statistics.Tools.Mean(values);
            var actualMean = actual.Mean;
            Assert.AreEqual(expectedMean, actualMean, 1e-7);

            var expectedVar = Accord.Statistics.Tools.Variance(values, false);
            var actualVar = actual.Variance;
            Assert.AreEqual(expectedVar, actualVar, 0.15);
        }

        [TestMethod()]
        public void FitTest2()
        {
            double[] coefficients = { 0.50, 0.50 };

            NormalDistribution[] components = new NormalDistribution[2];
            components[0] = new NormalDistribution(2, 1);
            components[1] = new NormalDistribution(5, 1);

            var target = new Mixture<NormalDistribution>(coefficients, components);

            double[] values =  { 12512, 1, 1, 0, 1, 6, 6, 5, 7, 5 };
            double[] weights = {     0, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            weights = weights.Divide(weights.Sum());
            double[] part1 = values.Submatrix(1, 4);
            double[] part2 = values.Submatrix(5, 9);

            MixtureOptions opt = new MixtureOptions()
            {
                Threshold = 0.000001
            };

            target.Fit(values, weights, opt);

            var mean1 = Accord.Statistics.Tools.Mean(part1);
            var var1 = Accord.Statistics.Tools.Variance(part1);
            Assert.AreEqual(mean1, target.Components[0].Mean, 1e-5);
            Assert.AreEqual(var1, target.Components[0].Variance, 1e-5);

            var mean2 = Accord.Statistics.Tools.Mean(part2);
            var var2 = Accord.Statistics.Tools.Variance(part2);
            Assert.AreEqual(mean2, target.Components[1].Mean, 1e-5);
            Assert.AreEqual(var2, target.Components[1].Variance, 1e-5);

            var expectedMean = Accord.Statistics.Tools.WeightedMean(values, weights);
            var actualMean = target.Mean;
            Assert.AreEqual(expectedMean, actualMean, 1e-5);
        }

        [TestMethod()]
        public void ProbabilityDensityFunction()
        {
            NormalDistribution[] components = new NormalDistribution[2];
            components[0] = new NormalDistribution(2, 1);
            components[1] = new NormalDistribution(5, 1);

            double[] coefficients = { 0.4, 0.5 };

            var mixture = new Mixture<NormalDistribution>(coefficients, components);

            double expected = 0.4 * components[0].ProbabilityDensityFunction(0.42) +
                              0.5 * components[1].ProbabilityDensityFunction(0.42);

            double actual = mixture.ProbabilityDensityFunction(0.42);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void LogProbabilityDensityFunction()
        {
            NormalDistribution[] components = new NormalDistribution[2];
            components[0] = new NormalDistribution(2, 1);
            components[1] = new NormalDistribution(5, 1);

            double[] coefficients = { 0.4, 0.5 };

            var mixture = new Mixture<NormalDistribution>(coefficients, components);

            double expected = System.Math.Log(
                0.4 * components[0].ProbabilityDensityFunction(0.42) +
                0.5 * components[1].ProbabilityDensityFunction(0.42));

            double actual = mixture.LogProbabilityDensityFunction(0.42);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void MixtureWeightsFitTest()
        {
            // Randomly initialize some mixture components
            NormalDistribution[] components = new NormalDistribution[2];
            components[0] = new NormalDistribution(2, 1);
            components[1] = new NormalDistribution(5, 1);

            // Create an initial mixture
            Mixture<NormalDistribution> mixture = new Mixture<NormalDistribution>(components);

            // Now, suppose we have a weighted data
            // set. Those will be the input points:

            double[] points = { 0, 3, 1, 7, 3, 5, 1, 2, -1, 2, 7, 6, 8, 6 }; // (14 points)

            // And those are their respective unormalized weights:
            double[] weights = { 1, 1, 1, 2, 2, 1, 1, 1, 2, 1, 2, 3, 1, 1 }; // (14 weights)

            // Let's normalize the weights so they sum up to one:
            weights = weights.Divide(weights.Sum());

            // Now we can fit our model to the data:
            mixture.Fit(points, weights);   // done!

            // Our model will be:
            double mean1 = mixture.Components[0].Mean; // 1.41126
            double mean2 = mixture.Components[1].Mean; // 6.53301

            // If we need the GaussianMixtureModel functionality, we can
            // use the estimated mixture to initialize a new model:
            GaussianMixtureModel gmm = new GaussianMixtureModel(mixture);

            Assert.AreEqual(mean1, gmm.Gaussians[0].Mean[0]);
            Assert.AreEqual(mean2, gmm.Gaussians[1].Mean[0]);

            Assert.AreEqual(mean1, 1.4112610766836404, 1e-15);
            Assert.AreEqual(mean2, 6.5330177004151082, 1e-14);

            Assert.AreEqual(mixture.Coefficients[0], gmm.Gaussians[0].Proportion);
            Assert.AreEqual(mixture.Coefficients[1], gmm.Gaussians[1].Proportion);
        }
    }
}
