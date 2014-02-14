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
    using Accord.Statistics.Distributions.Multivariate;
    using System.Globalization;

    [TestClass()]
    public class MultinomialDistributionTest
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

            int numberOfTrials = 5; 
            double[] probabilities = { 0.25, 0.75 };

            // Create a new Multinomial distribution with 5 trials for 2 symbols
            var dist = new MultinomialDistribution(numberOfTrials, probabilities);

            int dimensions = dist.Dimension; // 2

            double[] mean = dist.Mean;     // {  1.25, 3.75 }
            double[] median = dist.Median; // {  1.25, 3.75 }
            double[] var = dist.Variance;  // { -0.9375, -0.9375 }

            double pdf1 = dist.ProbabilityMassFunction(new[] { 2, 3 }); // 0.26367187499999994
            double pdf2 = dist.ProbabilityMassFunction(new[] { 1, 4 }); // 0.3955078125
            double pdf3 = dist.ProbabilityMassFunction(new[] { 5, 0 }); // 0.0009765625
            double lpdf = dist.LogProbabilityMassFunction(new[] { 1, 4 }); // -0.9275847384929139


            string str = dist.ToString(CultureInfo.InvariantCulture); 
            // output is "Multinomial(x; n = 5, p = { 0.25, 0.75 })"

            Assert.AreEqual(1.25, mean[0]);
            Assert.AreEqual(3.75, mean[1]);
            Assert.AreEqual(1.25, median[0]);
            Assert.AreEqual(3.75, median[1]);
            Assert.AreEqual(-0.9375, var[0]);
            Assert.AreEqual(-0.9375, var[1]);
            Assert.AreEqual(0.26367187499999994, pdf1);
            Assert.AreEqual(0.3955078125, pdf2);
            Assert.AreEqual(0.0009765625, pdf3);
            Assert.AreEqual(-0.9275847384929139, lpdf);
            Assert.AreEqual("Multinomial(x; n = 5, p = { 0.25, 0.75 })", str);
        }

        [TestMethod()]
        public void ProbabilityMassFunctionTest()
        {
            MultinomialDistribution dist = new MultinomialDistribution(5, 0.25, 0.25, 0.25, 0.25);

            int[] observation = { 1, 1, 1, 2 };

            double actual = dist.ProbabilityMassFunction(observation);
            double expected = 0.05859375;

            Assert.AreEqual(expected, actual, 1e-6);
        }

        [TestMethod()]
        public void ProbabilityMassFunctionTest2()
        {
            // Example from http://onlinestatbook.com/2/probability/multinomial.html

            MultinomialDistribution dist = new MultinomialDistribution(12, 0.40, 0.35, 0.25);

            int[] observation = { 7, 2, 3 };

            double actual = dist.ProbabilityMassFunction(observation);
            double expected = 0.02483712;

            Assert.AreEqual(expected, actual, 1e-6);
        }

        [TestMethod()]
        public void LogProbabilityMassFunctionTest()
        {
            MultinomialDistribution dist = new MultinomialDistribution(5, 0.25, 0.25, 0.25, 0.25);

            int[] observation = { 1, 1, 1, 2 };

            double actual = dist.LogProbabilityMassFunction(observation);
            double expected = System.Math.Log(0.058593750);

            Assert.AreEqual(expected, actual, 1e-6);
        }

        [TestMethod()]
        public void FitTest()
        {
            MultinomialDistribution dist = new MultinomialDistribution(7, new double[2]);

            double[][] observation =
            { 
                new double[] { 0, 2 },
                new double[] { 1, 2 },
                new double[] { 5, 1 },
            };

            dist.Fit(observation);

            Assert.AreEqual(dist.Probabilities[0], 0.857142857142857, 0.000000001);
            Assert.AreEqual(dist.Probabilities[1], 0.714285714285714, 0.000000001);
        }

    }
}
