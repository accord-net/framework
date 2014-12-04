using System;
using Accord.Statistics.Distributions.Univariate.Continuous;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accord.Tests.Statistics.Distributions.Univariate.Continuous
{
    [TestClass]
    public class TrapezoidalDistributionTest
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
        public void BetaDistributionConstructorTest()
        {
            double x = 0.75d;

            double a = 0;
            double b = (1.0d/3.0d);
            double c = (2.0d/3.0d);
            double d = 1.0d;
            double n1 = 2.0d;
            double n3 = 2.0d;
            double alpha = 1.0d;

            var trapDist = new TrapezoidalDistribution(a, b, c, d, n1, n3, alpha);
            double mean = trapDist.Mean; //0.62499999999999989
            double variance = trapDist.Variance; //0.37103174603174593
            double pdf = trapDist.ProbabilityDensityFunction(x); //1.1249999999999998
            double cdf = trapDist.DistributionFunction(x); //1.28125
            string tostr = trapDist.ToString();//"Trapezoidal(x; a=0, b=0.333333333333333, c=0.666666666666667, d=1, n1=2, n3=2, α = 1)"

            Assert.AreEqual(mean, 0.625d, 0.00000001);
            Assert.AreEqual(variance, 0.37103175d, 0.00000001);
            Assert.AreEqual(pdf, 1.125, 0.000000001, "should match output from dtrapezoid in R");
            Assert.AreEqual(cdf, 1.28125,0.000000001);
            Assert.AreEqual(tostr, "Trapezoidal(x; a=0, b=0.333333333333333, c=0.666666666666667, d=1, n1=2, n3=2, α = 1)");
            //Verified using R package 'trapezoid'
        }
    }
}
