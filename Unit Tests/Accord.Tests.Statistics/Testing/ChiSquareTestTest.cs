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
    using NUnit.Framework;
    using Accord.Statistics.Distributions.Univariate;


    [TestFixture]
    public class ChiSquareTestTest
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
        public void ConstructorTest()
        {
            double[] observed = { 639, 241 };
            double[] expected = { 660, 220 };

            int degreesOfFreedom = 1;
            var chi = new ChiSquareTest(expected, observed, degreesOfFreedom);

            Assert.AreEqual(2.668, chi.Statistic, 0.015);
            Assert.AreEqual(1, chi.DegreesOfFreedom);
            Assert.AreEqual(0.1020, chi.PValue, 1e-4);
        }

        [Test]
        public void ConstructorTest2()
        {
            double[] observed = { 6, 6, 16, 15, 4, 3 };
            double[] expected = { 6.24, 5.76, 16.12, 14.88, 3.64, 3.36 };

            int degreesOfFreedom = 2;
            var chi = new ChiSquareTest(expected, observed, degreesOfFreedom);

            Assert.AreEqual(0.0952, chi.Statistic, 0.001);
            Assert.AreEqual(2, chi.DegreesOfFreedom);
            Assert.AreEqual(false, chi.Significant);
        }

        [Test]
        public void ConstructorTest3()
        {
            // Example based on Wikipedia's article for χ²-Squared Test
            // http://en.wikipedia.org/wiki/Pearson's_chi-squared_test

            // Suppose we would like to test the hypothesis that a random sample of 
            // 100 people has been drawn from a population in which men and women are
            // equal in frequency. 

            // Under this hypothesis, the observed number of men and women would be 
            // compared to the theoretical frequencies of 50 men and 50 women. So,
            // after drawing our sample, we found out that there were 44 men and 56
            // women in the sample:

            //                     man  woman
            double[] observed = { 44, 56 };
            double[] expected = { 50, 50 };

            // If the null hypothesis is true (i.e., men and women are chosen with 
            // equal probability), the test statistic will be drawn from a chi-squared
            // distribution with one degree of freedom. If the male frequency is known, 
            // then the female frequency is determined.
            //
            int degreesOfFreedom = 1;

            // So now we have:
            //
            var chi = new ChiSquareTest(expected, observed, degreesOfFreedom);


            // The chi-squared distribution for 1 degree of freedom shows that the 
            // probability of observing this difference (or a more extreme difference 
            // than this) if men and women are equally numerous in the population is 
            // approximately 0.23. 

            double pvalue = chi.PValue; // 0.23

            // This probability is higher than conventional criteria for statistical
            // significance (0.001 or 0.05), so normally we would not reject the null
            // hypothesis that the number of men in the population is the same as the
            // number of women.

            bool significant = chi.Significant; // false

            Assert.AreEqual(0.23013934044341644, pvalue);
            Assert.IsFalse(significant);
        }

        [Test]
        public void ConstructorTest4()
        {
            double[] sample = 
            { 
                0.621, 0.503, 0.203, 0.477, 0.710, 0.581, 0.329, 0.480, 0.554, 0.382
            };

            var distribution = UniformContinuousDistribution.Standard;

            var chi = new ChiSquareTest(sample, distribution);

            Assert.AreEqual(0, chi.Statistic, 1e-6);
            Assert.AreEqual(3, chi.DegreesOfFreedom);
            Assert.AreEqual(1, chi.PValue, 1e-4);
            Assert.IsFalse(chi.Significant);
        }

        [Test]
        public void ConstructorTest5()
        {
            Accord.Math.Tools.SetupGenerator(0);
            double[] unif = UniformContinuousDistribution.Standard.Generate(10000);
            double[] norm = NormalDistribution.Standard.Generate(10000);

            var u = UniformContinuousDistribution.Standard;
            var n = NormalDistribution.Standard;

            {
                var chi = new ChiSquareTest(unif, u);
                Assert.AreEqual(2.7011909090910131, chi.Statistic, 1e-6);
                Assert.AreEqual(10, chi.DegreesOfFreedom);
                Assert.AreEqual(0.98760847271849528, chi.PValue, 1e-6);
                Assert.IsFalse(chi.Significant);
            }
            {
                var chi = new ChiSquareTest(unif, n);
                Assert.AreEqual(14865.499690909099, chi.Statistic, 1e-6);
                Assert.AreEqual(10, chi.DegreesOfFreedom);
                Assert.AreEqual(0, chi.PValue, 1e-6);
                Assert.IsTrue(chi.Significant);
            }
            {
                var chi = new ChiSquareTest(norm, u);
                Assert.AreEqual(3934.3426909090917, chi.Statistic, 1e-6);
                Assert.AreEqual(10, chi.DegreesOfFreedom);
                Assert.AreEqual(0, chi.PValue, 1e-6);
                Assert.IsTrue(chi.Significant);
            }
            {
                var chi = new ChiSquareTest(norm, n);
                Assert.AreEqual(6.2902909090909525, chi.Statistic, 1e-6);
                Assert.AreEqual(10, chi.DegreesOfFreedom);
                Assert.AreEqual(0.79031311920555392, chi.PValue, 1e-6);
                Assert.IsFalse(chi.Significant);
            }
        }

        [Test]
        public void ConstructorTest_WithZero_NoNaN()
        {
            double[] observed = { 639, 0 };
            double[] expected = { 660, 0 };

            int degreesOfFreedom = 1;
            var chi = new ChiSquareTest(expected, observed, degreesOfFreedom);

            Assert.AreEqual(0.66818181818181821, chi.Statistic, 0.015);
            Assert.AreEqual(1, chi.DegreesOfFreedom);
            Assert.AreEqual(0.41368622699316782, chi.PValue, 1e-4);
        }
    }
}
