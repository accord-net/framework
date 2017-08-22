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
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;
    using System.Globalization;

    [TestFixture]
    public class MixtureDistributionTest
    {

        [Test]
        public void ConstructorTest()
        {
            // Create a new mixture containing two Normal distributions
            Mixture<NormalDistribution> mix = new Mixture<NormalDistribution>(
                new NormalDistribution(2, 1), new NormalDistribution(5, 1));

            // Common measures
            double mean = mix.Mean;     // 3.5
            double median = mix.Median;   // 3.4999998506015895
            double var = mix.Variance; // 3.25

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
            Assert.AreEqual(3.4999998506015895, median, 1e-10);
            Assert.AreEqual(3.25, var, 1e-10);
            Assert.AreEqual(0.91373394208601633, chf, 1e-10);
            Assert.AreEqual(0.59897597553494908, cdf, 1e-10);
            Assert.AreEqual(0.14499174984363708, pmf1, 1e-10);
            Assert.AreEqual(0.19590437513747333, pmf2, 1e-10);
            Assert.AreEqual(0.13270883471234715, pmf3, 1e-10);
            Assert.AreEqual(-1.8165661905848629, lpmf, 1e-10);
            Assert.AreEqual(0.40541978256972522, hf, 1e-10);
            Assert.AreEqual(0.40102402446505092, ccdf, 1e-10);
            Assert.AreEqual(1.5866611690305092, icdf1, 1e-10);
            Assert.AreEqual(3.1968506765456883, icdf2, 1e-10);
            Assert.AreEqual(5.6437596300843076, icdf3, 1e-10);
            Assert.AreEqual("Mixture(x; 0.5*N(x; μ = 2, σ² = 1) + 0.5*N(x; μ = 5, σ² = 1))", str);

            Assert.IsFalse(double.IsNaN(icdf1));

            var range1 = mix.GetRange(0.95);
            var range2 = mix.GetRange(0.99);
            var range3 = mix.GetRange(0.01);

            Assert.AreEqual(0.71839556342582434, range1.Min, 1e-10);
            Assert.AreEqual(6.2816044312576365, range1.Max, 1e-10);
            Assert.AreEqual(-0.053753308211290443, range2.Min, 1e-10);
            Assert.AreEqual(7.0537533150666105, range2.Max, 1e-10);
            Assert.AreEqual(-0.053753308211289402, range3.Min, 1e-10);
            Assert.AreEqual(7.0537533150666105, range3.Max, 1e-10);
        }

        [Test]
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

        [Test]
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
            Assert.AreEqual(3.4999998506015895, median, 1e-10);
            Assert.AreEqual(3.25, var);
        }

        [Test]
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

            var mean1 = Measures.Mean(part1);
            var var1 = Measures.Variance(part1);
            Assert.AreEqual(mean1, actual.Components[0].Mean, 1e-6);
            Assert.AreEqual(var1, actual.Components[0].Variance, 1e-6);

            var mean2 = Measures.Mean(part2);
            var var2 = Measures.Variance(part2);
            Assert.AreEqual(mean2, actual.Components[1].Mean, 1e-6);
            Assert.AreEqual(var2, actual.Components[1].Variance, 1e-5);

            var expectedMean = Measures.Mean(values);
            var actualMean = actual.Mean;
            Assert.AreEqual(expectedMean, actualMean, 1e-7);

            var expectedVar = Measures.Variance(values, false);
            var actualVar = actual.Variance;
            Assert.AreEqual(expectedVar, actualVar, 0.15);
        }

        [Test]
        public void FitTest2()
        {
            double[] coefficients = { 0.50, 0.50 };

            NormalDistribution[] components = new NormalDistribution[2];
            components[0] = new NormalDistribution(2, 1);
            components[1] = new NormalDistribution(5, 1);

            var target = new Mixture<NormalDistribution>(coefficients, components);

            double[] values = { 12512, 1, 1, 0, 1, 6, 6, 5, 7, 5 };
            double[] weights = { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            weights = weights.Divide(weights.Sum());
            double[] part1 = values.Submatrix(1, 4);
            double[] part2 = values.Submatrix(5, 9);

            MixtureOptions opt = new MixtureOptions()
            {
                Threshold = 0.000001
            };

            target.Fit(values, weights, opt);

            var mean1 = Measures.Mean(part1);
            var var1 = Measures.Variance(part1);
            Assert.AreEqual(mean1, target.Components[0].Mean, 1e-5);
            Assert.AreEqual(var1, target.Components[0].Variance, 1e-5);

            var mean2 = Measures.Mean(part2);
            var var2 = Measures.Variance(part2);
            Assert.AreEqual(mean2, target.Components[1].Mean, 1e-5);
            Assert.AreEqual(var2, target.Components[1].Variance, 1e-5);

            var expectedMean = Measures.WeightedMean(values, weights);
            var actualMean = target.Mean;
            Assert.AreEqual(expectedMean, actualMean, 1e-5);
        }

        [Test]
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

        [Test]
        public void ProbabilityDensityFunctionPerComponent()
        {
            NormalDistribution[] components = new NormalDistribution[2];
            components[0] = new NormalDistribution(2, 1);
            components[1] = new NormalDistribution(5, 1);

            double[] coefficients = { 0.4, 0.5 };

            var mixture = new Mixture<NormalDistribution>(coefficients, components);

            double expected = mixture.ProbabilityDensityFunction(0, 0.42) +
                              mixture.ProbabilityDensityFunction(1, 0.42);

            double actual = mixture.ProbabilityDensityFunction(0.42);

            Assert.AreEqual(expected, actual);
        }

        [Test]
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

        [Test]
        public void LogProbabilityDensityFunctionPerComponent()
        {
            NormalDistribution[] components = new NormalDistribution[2];
            components[0] = new NormalDistribution(2, 1);
            components[1] = new NormalDistribution(5, 1);

            double[] coefficients = { 0.4, 0.5 };

            var mixture = new Mixture<NormalDistribution>(coefficients, components);

            double expected = System.Math.Log(
                0.4 * components[0].ProbabilityDensityFunction(0.42));

            double actual = mixture.LogProbabilityDensityFunction(0, 0.42);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DistributionFunctionPerComponent()
        {
            NormalDistribution[] components = new NormalDistribution[2];
            components[0] = new NormalDistribution(2, 1);
            components[1] = new NormalDistribution(5, 1);

            double[] coefficients = { 0.4, 0.5 };

            var mixture = new Mixture<NormalDistribution>(coefficients, components);

            double expected = mixture.DistributionFunction(0, 0.42) +
                              mixture.DistributionFunction(1, 0.42);

            double actual = mixture.DistributionFunction(0.42);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MixtureWeightsFitTest()
        {
            // Randomly initialize some mixture components
            NormalDistribution[] components = new NormalDistribution[2];
            components[0] = new NormalDistribution(2, 1);
            components[1] = new NormalDistribution(5, 1);

            // Create an initial mixture
            Mixture<NormalDistribution> mixture = new Mixture<NormalDistribution>(components);

            // Now, suppose we have a weighted data set. Those will be the input points:
            double[] points = { 0, 3, 1, 7, 3, 5, 1, 2, -1, 2, 7, 6, 8, 6 }; // (14 points)

            // And those are their respective unnormalized weights:
            double[] weights = { 1, 1, 1, 2, 2, 1, 1, 1, 2, 1, 2, 3, 1, 1 }; // (14 weights)

            // Let's normalize the weights so they sum up to one:
            weights = weights.Divide(weights.Sum());

            // Now we can fit our model to the data:
            mixture.Fit(points, weights);   // done!

            // Our model will be:
            double mean1 = mixture.Components[0].Mean; // 1.41126
            double mean2 = mixture.Components[1].Mean; // 6.53301

            // With mixture coefficients
            double pi1 = mixture.Coefficients[0]; // 0.51408489193241225
            double pi2 = mixture.Coefficients[1]; // 0.48591510806758775

            Assert.AreEqual(1.4112610766836411, mean1);
            Assert.AreEqual(6.5330177004151064, mean2);

            Assert.AreEqual(0.51408489193241225, pi1);
            Assert.AreEqual(0.48591510806758775, pi2);

            /*
                        // If we need the GaussianMixtureModel functionality, we can
                        // use the estimated mixture to initialize a new model:
                        GaussianMixtureModel gmm = new GaussianMixtureModel(mixture);

                        Assert.AreEqual(mean1, gmm.Gaussians[0].Mean[0]);
                        Assert.AreEqual(mean2, gmm.Gaussians[1].Mean[0]);

                        Assert.AreEqual(mean1, 1.4112610766836404, 1e-15);
                        Assert.AreEqual(mean2, 6.5330177004151082, 1e-14);

                        Assert.AreEqual(mixture.Coefficients[0], gmm.Gaussians[0].Proportion);
                        Assert.AreEqual(mixture.Coefficients[1], gmm.Gaussians[1].Proportion);
            */
        }

        [Test]
        public void MixtureFitTest()
        {
            var samples1 = new NormalDistribution(mean: -2, stdDev: 0.5).Generate(100000);
            var samples2 = new NormalDistribution(mean: +4, stdDev: 0.5).Generate(100000);

            // Mix the samples from both distributions
            var samples = samples1.Concatenate(samples2);

            // Create a new mixture distribution with two Normal components
            var mixture = new Mixture<NormalDistribution>(new[] { 0.2, 0.8 },
                new NormalDistribution(-1),
                new NormalDistribution(+1));

            // Estimate the distribution
            mixture.Fit(samples, new MixtureOptions
            {
                Iterations = 50,
                Threshold = 0
            });

            var result = mixture.ToString("N2", System.Globalization.CultureInfo.InvariantCulture);

            Assert.AreEqual("Mixture(x; 0.50*N(x; μ = -2.00, σ² = 0.25) + 0.50*N(x; μ = 4.00, σ² = 0.25))", result);
        }
    }
}
