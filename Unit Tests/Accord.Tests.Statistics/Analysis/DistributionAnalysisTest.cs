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
    using Accord.Statistics.Analysis;
    using NUnit.Framework;
    using Accord.Math;
    using Accord.Statistics.Testing;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Distributions.Fitting;
    using System;
    using System.Globalization;
    using System.Collections.Generic;
    using Accord.Statistics.Distributions;
#if NO_CULTURE
    using CultureInfo = Accord.Compat.CultureInfoEx;
#endif

    [TestFixture]
    public class DistributionAnalysisTest
    {

        [Test]
        public void learn1()
        {
            #region doc_learn
            // Let's say we would like to check from which possible 
            // distribution a given sample might have come from. 
            double[] x = { -1, 2, 5, 3, 2, 1, 4, 32, 0, 2, 4 };

            // Create a distribution analysis
            var da = new DistributionAnalysis();

            // Learn the analysis
            var fit = da.Learn(x);

            // Get the most likely distribution amongst the ones that 
            // have been tried (by default, only a few are tested)
            var mostLikely1 = fit[0].Distribution; // N(x; μ = 4.9, σ² = 83.9)

            // Sometimes it might be the case that we would like to
            // test against some other distributions than the default
            // ones. We can add them to the list of tested distributions:
            da.Distributions.Add(new VonMisesDistribution(1.0));

            // and re-learn the analysis
            fit = da.Learn(x);

            var mostLikely2 = fit[0].Distribution; // VonMises(x; μ = 1.92, κ = 0.18)

            // it is also possible to specify different sample 
            // weights (but not all distributions support it)
            double[] w = { 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0, 0.1, 0.1, 0.1 };

            // and re-learn the analysis with weights
            fit = da.Learn(x, w);

            var mostLikely3 = fit[0].Distribution; // VonMises(x; μ = 2.81, κ = 0.25

            #endregion

            string expected = (mostLikely1 as IFormattable).ToString("N2", CultureInfo.InvariantCulture);
            Assert.AreEqual(expected, "N(x; μ = 4.91, σ² = 83.89)");

            expected = (mostLikely2 as IFormattable).ToString("N2", CultureInfo.InvariantCulture);
            Assert.AreEqual(expected, "VonMises(x; μ = 1.92, κ = 0.18)");

            expected = (mostLikely3 as IFormattable).ToString("N2", CultureInfo.InvariantCulture);
            Assert.AreEqual(expected, "VonMises(x; μ = 2.82, κ = 0.25)");

            expected = fit[0].ToString("N2", CultureInfo.InvariantCulture);
            Assert.AreEqual(expected, "VonMises(x; μ = 2.82, κ = 0.25)");
            expected = fit[1].ToString("N2", CultureInfo.InvariantCulture);
            Assert.AreEqual(expected, "Poisson(x; λ = 2.20)");
            expected = fit[2].ToString("N2", CultureInfo.InvariantCulture);
            Assert.AreEqual(expected, "Gumbel(x; μ = 3.04, β = 1.46)");
            expected = fit[3].ToString("N2", CultureInfo.InvariantCulture);
            Assert.AreEqual(expected, "N(x; μ = 2.20, σ² = 3.51)");
        }

        [Test]
        public void ConstructorTest()
        {
            int n = 10000;
            double[] normal = NormalDistribution.Standard.Generate(n);
            double[] uniform = UniformContinuousDistribution.Standard.Generate(n);
            double[] poisson = PoissonDistribution.Standard.Generate(n).ToDouble();
            double[] gamma = GammaDistribution.Standard.Generate(n);

            {
                DistributionAnalysis analysis = new DistributionAnalysis(normal);
                analysis.Compute();
                Assert.AreEqual("Normal", analysis.GoodnessOfFit[0].Name);
            }

            {
                DistributionAnalysis analysis = new DistributionAnalysis(uniform);
                analysis.Compute();
                Assert.AreEqual("UniformContinuous", analysis.GoodnessOfFit[0].Name);
            }

            {
                DistributionAnalysis analysis = new DistributionAnalysis(poisson);
                analysis.Compute();
                Assert.AreEqual("Poisson", analysis.GoodnessOfFit[0].Name);
            }

            {
                DistributionAnalysis analysis = new DistributionAnalysis(gamma);
                analysis.Compute();
                Assert.AreEqual("Gamma", analysis.GoodnessOfFit[0].Name);
            }
        }


        [Test]
        public void options_test()
        {
            // Gamma Distribution Fit stalls for some arrays #301
            // https://github.com/accord-net/framework/issues/301

            double[] x = { 1.003, 1.012, 1.011, 1.057, 1.033, 1.051, 1.045, 1.045, 1.037, 1.059, 1.028, 1.032, 1.029, 1.031, 1.029, 1.023, 1.035 };

            DistributionAnalysis analysis = new DistributionAnalysis();

            analysis.Options[analysis.GetFirstIndex("GammaDistribution")] = new GammaOptions()
            {
                Iterations = 0
            };

            var fit1 = analysis.Learn(x);


            Assert.AreEqual("UniformContinuous", analysis.GoodnessOfFit[0].Name);
            Assert.AreEqual("Gamma", analysis.GoodnessOfFit[2].Name);

            var gamma = analysis.GoodnessOfFit[2].Distribution as GammaDistribution;
            Assert.AreEqual(1.0329411764705885, gamma.Mean, 1e-8);
            Assert.AreEqual(1.03286759780857, gamma.Median, 1e-8);
            Assert.AreEqual(1.0327204376888031, gamma.Mode, 1e-8);
            Assert.AreEqual(4530.2415457406223, gamma.Rate, 1e-8);
            Assert.AreEqual(4679.4730319532555, gamma.Shape, 1e-8);

            analysis.Distributions = new List<IFittableDistribution<double>>()
            {
                new GammaDistribution()
            };

            analysis.Options[analysis.GetFirstIndex("GammaDistribution")] = new GammaOptions()
            {
                Iterations = 1000
            };

            var fit2 = analysis.Learn(x);

            gamma = analysis.GoodnessOfFit[0].Distribution as GammaDistribution;
            Assert.AreEqual(1.0329411764705885, gamma.Mean, 1e-8);
            Assert.AreEqual(1.03286759780857, gamma.Median, 1e-8);
            Assert.AreEqual(1.0327204376888031, gamma.Mode, 1e-8);
            Assert.AreEqual(4530.2415515050052, gamma.Rate, 1e-8);
            Assert.AreEqual(4679.4730379075245, gamma.Shape, 1e-8);


            Assert.AreEqual("Gamma", analysis.GoodnessOfFit[0].Name);
        }
    }
}
