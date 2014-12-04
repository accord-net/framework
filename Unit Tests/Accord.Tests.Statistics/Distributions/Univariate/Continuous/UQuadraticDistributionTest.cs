using System;
using Accord.Statistics.Distributions.Univariate.Continuous;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accord.Tests.Statistics.Distributions.Univariate.Continuous
{
    [TestClass]
    public class UQuadraticDistributionTest
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

        [TestMethod]
        public void Constructor_UQuadratic()
        {

            double a = -2.0d;
            double b = 2.0d;
            double x = 0.0d;

            var uQuadDist = new UQuadraticDistribution(a, b);
            double mean = uQuadDist.Mean; //0.0
            double variance = uQuadDist.Variance; //2.4
            double median = uQuadDist.Median; //0.0
            double pdf = uQuadDist.ProbabilityDensityFunction(x); //0.0
            double cdf = uQuadDist.DistributionFunction(x); //0.5
            string tostr = uQuadDist.ToString(); //UQuadratic(x; a = -2, b = 2)

            Assert.AreEqual(mean, 0);
            Assert.AreEqual(variance, 2.4d);
            Assert.AreEqual(median, 0);
            Assert.AreEqual(pdf, 0);
            Assert.AreEqual(cdf, 0.5);
            Assert.AreEqual(tostr, "UQuadratic(x; a = -2, b = 2)");
        }
    }
}
