// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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


    [TestFixture]
    public class DistributionAnalysisTest
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



        [Test]
#if !DEBUG
        [Timeout(2000)]
#endif
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
    }
}
