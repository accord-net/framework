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

    using Accord.Statistics.Testing;
    using Accord.Statistics.Testing.Power;
    using NUnit.Framework;

    [TestFixture]
    public class ZTestPowerAnalysisTest
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
        public void ZTestPowerAnalysisConstructorTest1()
        {
            ZTestPowerAnalysis target;
            double actual, expected;

            target = new ZTestPowerAnalysis(OneSampleHypothesis.ValueIsDifferentFromHypothesis)
            {
                Effect = 0.2,
                Samples = 60,
                Size = 0.10,
            };

            target.ComputePower();

            expected = 0.4618951;
            actual = target.Power;
            Assert.AreEqual(expected, actual, 1e-5);


            target = new ZTestPowerAnalysis(OneSampleHypothesis.ValueIsSmallerThanHypothesis)
            {
                Effect = 0.2,
                Samples = 60,
                Size = 0.10,
            };

            target.ComputePower();

            expected = 0.00232198;
            actual = target.Power;
            Assert.AreEqual(expected, actual, 1e-5);


            target = new ZTestPowerAnalysis(OneSampleHypothesis.ValueIsGreaterThanHypothesis)
            {
                Effect = 0.2,
                Samples = 60,
                Size = 0.10,
            };

            target.ComputePower();

            expected = 0.6055124;
            actual = target.Power;
            Assert.AreEqual(expected, actual, 1e-5);
        }

        [Test]
        public void ZTestPowerAnalysisConstructorTest2()
        {
            // When creating a power analysis, we have three things we can
            // change. We can always freely configure two of those things
            // and then ask the analysis to give us the third.

            var analysis = new ZTestPowerAnalysis(OneSampleHypothesis.ValueIsDifferentFromHypothesis);

            // Those are:
            double e = analysis.Effect;   // the test's minimum detectable effect size
            double n = analysis.Samples;  // the number of samples in the test
            double p = analysis.Power;    // the probability of committing a type-2 error

            // Let's set the desired effect size and the 
            // number of samples so we can get the power

            analysis.Effect = 0.2; // we would like to detect at least 0.2 std. dev. apart
            analysis.Samples = 60; // we would like to use at most 60 samples
            analysis.ComputePower(); // what will be the power of this test?

            double power = analysis.Power; // The power is going to be 0.34 (or 34%)

            // Let's set the desired power and the number 
            // of samples so we can get the effect size

            analysis.Power = 0.8;  // we would like to create a test with 80% power
            analysis.Samples = 60; // we would like to use at most 60 samples
            analysis.ComputeEffect(); // what would be the minimum effect size we can detect?

            double effect = analysis.Effect; // The effect will be 0.36 standard deviations.

            // Let's set the desired power and the effect
            // size so we can get the number of samples

            analysis.Power = 0.8;  // we would like to create a test with 80% power
            analysis.Effect = 0.2; // we would like to detect at least 0.2 std. dev. apart
            analysis.ComputeSamples();

            double samples = analysis.Samples; // We would need around 197 samples.

            Assert.AreEqual(196.22199335872716, samples); // 196.22199335872716
            Assert.AreEqual(0.36168309642441332, effect);
            Assert.AreEqual(0.34062035960875625, power);
        }
    }
}
