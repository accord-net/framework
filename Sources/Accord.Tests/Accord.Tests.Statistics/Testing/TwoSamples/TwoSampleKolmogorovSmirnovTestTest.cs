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
    using System;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Math;

    [TestClass()]
    public class TwoSampleKolmogorovSmirnovTestTest
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
        public void TwoSampleKolmogorovSmirnovTestConstructorTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            // Create a K-S test to verify if two samples have been
            // drawn from different populations. In this example, we
            // will first generate a number of samples from different
            // distributions, and then check if the K-S test can indeed
            // see the difference:

            // Generate 15 points from a Normal distribution with mean 5 and sigma 2
            double[] sample1 = new NormalDistribution(mean: 5, stdDev: 1).Generate(25);

            // Generate 15 points from an uniform distribution from 0 to 10
            double[] sample2 = new UniformContinuousDistribution(a: 0, b: 10).Generate(25);

            // Now we can create a K-S test and test the unequal hypothesis:
            var test = new TwoSampleKolmogorovSmirnovTest(sample1, sample2,
                TwoSampleKolmogorovSmirnovTestHypothesis.SamplesDistributionsAreUnequal);

            bool significant = test.Significant; // outputs true

            Assert.IsTrue(test.Significant);
            Assert.AreEqual(0.44, test.Statistic, 1e-15);
            Assert.IsFalse(Double.IsNaN(test.Statistic));
            Assert.AreEqual(0.00826, test.PValue, 1e-5);
        }

        [TestMethod()]
        public void TwoSampleKolmogorovSmirnovTestConstructorTest2()
        {
            // The following example comes from the stats page of the College 
            // of Saint Benedict and Saint John's University (Kirkman, 1996). 
            // http://www.physics.csbsju.edu/stats/

            double[] redwell = 
            {
                23.4, 30.9, 18.8, 23.0, 21.4, 1, 24.6, 23.8, 24.1, 18.7, 16.3, 20.3,
                14.9, 35.4, 21.6, 21.2, 21.0, 15.0, 15.6, 24.0, 34.6, 40.9, 30.7, 
                24.5, 16.6, 1, 21.7, 1, 23.6, 1, 25.7, 19.3, 46.9, 23.3, 21.8, 33.3, 
                24.9, 24.4, 1, 19.8, 17.2, 21.5, 25.5, 23.3, 18.6, 22.0, 29.8, 33.3,
                1, 21.3, 18.6, 26.8, 19.4, 21.1, 21.2, 20.5, 19.8, 26.3, 39.3, 21.4, 
                22.6, 1, 35.3, 7.0, 19.3, 21.3, 10.1, 20.2, 1, 36.2, 16.7, 21.1, 39.1,
                19.9, 32.1, 23.1, 21.8, 30.4, 19.62, 15.5 
            };

            double[] whitney = 
            {
                16.5, 1, 22.6, 25.3, 23.7, 1, 23.3, 23.9, 16.2, 23.0, 21.6, 10.8, 12.2,
                23.6, 10.1, 24.4, 16.4, 11.7, 17.7, 34.3, 24.3, 18.7, 27.5, 25.8, 22.5,
                14.2, 21.7, 1, 31.2, 13.8, 29.7, 23.1, 26.1, 25.1, 23.4, 21.7, 24.4, 13.2,
                22.1, 26.7, 22.7, 1, 18.2, 28.7, 29.1, 27.4, 22.3, 13.2, 22.5, 25.0, 1,
                6.6, 23.7, 23.5, 17.3, 24.6, 27.8, 29.7, 25.3, 19.9, 18.2, 26.2, 20.4,
                23.3, 26.7, 26.0, 1, 25.1, 33.1, 35.0, 25.3, 23.6, 23.2, 20.2, 24.7, 22.6,
                39.1, 26.5, 22.7
            };


            // Create a non-parametric Kolmogorov-Smirnov's test
            var twoTail = new TwoSampleKolmogorovSmirnovTest(redwell, whitney,
                alternate: TwoSampleKolmogorovSmirnovTestHypothesis.SamplesDistributionsAreUnequal);

            var oneTailGreater = new TwoSampleKolmogorovSmirnovTest(redwell, whitney,
                alternate: TwoSampleKolmogorovSmirnovTestHypothesis.FirstSampleIsLargerThanSecond);

            var oneTailLesser = new TwoSampleKolmogorovSmirnovTest(redwell, whitney,
                alternate: TwoSampleKolmogorovSmirnovTestHypothesis.FirstSampleIsSmallerThanSecond);


            Assert.AreEqual(0.2204113924050633, twoTail.Statistic, 1e-10);
            Assert.AreEqual(0.2204113924050633, oneTailGreater.Statistic, 1e-10);
            Assert.AreEqual(0.1242088607594936, oneTailLesser.Statistic, 1e-10);

            Assert.AreEqual(0.03463090913864153, twoTail.PValue, 1e-10);
            Assert.AreEqual(0.0177488245823226, oneTailGreater.PValue, 1e-10);
            Assert.AreEqual(0.270697775095498, oneTailLesser.PValue, 1e-10);
        }

        [TestMethod()]
        public void TwoSampleKolmogorovSmirnovTestConstructorTest3()
        {
            double[] x = { 1, 2, 3, 4, 5 };
            double[] y = { 2.5, 4.5 };
            var target = new TwoSampleKolmogorovSmirnovTest(x, y);

            Assert.AreEqual(0.4, target.Statistic);

            double actual = target.PValue;

            Assert.AreEqual(0.952, actual, 1e-3);
        }
    }
}
