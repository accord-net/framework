using System;
using System.Globalization;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Distributions.Univariate.Continuous;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accord.Tests.Statistics.Distributions.Univariate.Continuous
{
    [TestClass]
    public class BetaPrimeDistributionTest
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
        public void Confirm_BetPrimeDistribution_Relative_to_F_Distribution()
        {
            double alpha = 4.0d;
            double beta = 6.0d;

            FDistribution fdist = new FDistribution((int)alpha * 2, (int)beta *2);
            double fMean = fdist.Mean;
            double fPdf = (beta / alpha) * fdist.ProbabilityDensityFunction(4.0d);
            double fCdf = fdist.DistributionFunction(4.0d);

            var betaPrimeDist = new BetaPrimeDistribution(alpha, beta);
            double bpMean = (beta / alpha) * betaPrimeDist.Mean;
            double bpPdf = betaPrimeDist.ProbabilityDensityFunction((alpha /beta ) *4.0d);
            double bpCdf = betaPrimeDist.DistributionFunction((alpha /beta ) * 4.0d);
           
            Assert.AreEqual(fMean, bpMean, 0.00000001, "mean should be equal");
            Assert.AreEqual(fPdf, bpPdf, 0.00000001, "probability density should be equal");
            Assert.AreEqual(fCdf, bpCdf, 0.00000001, "cumulative distribution should be equal");

            //Beta Prime distribution is a scaled version of Pearson Type VI, which itself is scale of F distribution
        }


        [TestMethod]
        public void Example_BetaPrimeDistribution() {

            double alpha = 4.0d;
            double beta = 6.0d;

            var betaPrimeDist = new BetaPrimeDistribution(alpha, beta);
            double mean = betaPrimeDist.Mean; // 0.8
            double variance = betaPrimeDist.Variance; //0.36
            double mode = betaPrimeDist.Mode; //0.42857142857142855
            double pdf = betaPrimeDist.ProbabilityDensityFunction(4.0d); //0.0033030143999999975
            double cdf = betaPrimeDist.DistributionFunction(4.0d); //0.996933632
            string tostr = betaPrimeDist.ToString(); //BetaPrime(x; α = 4, β = 6)


            Assert.AreEqual(mean, 0.8);
            Assert.AreEqual(variance, 0.36);
            Assert.AreEqual(mode, 0.42857142857142855);
            Assert.AreEqual(pdf, 0.0033030143999999975);
            Assert.AreEqual(cdf, 0.996933632);
            Assert.AreEqual(tostr, "BetaPrime(x; α = 4, β = 6)");

        }

    }
}
