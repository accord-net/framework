using System;
using System.Globalization;
using Accord.Statistics.Distributions.Univariate.Continuous;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accord.Tests.Statistics.Distributions.Univariate.Continuous
{
    [TestClass]
    public class KumaraswamyDistributionTest
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
        public void Constructor_KumaraswamyDistribution_PDF_given_0d2_AND_1d2_Parameters_Moments_matches_R_output()
        {
            var kumaraswamyDistribution = new KumaraswamyDistribution(0.2d, 1.2d);
            double mean = kumaraswamyDistribution.Mean; //0.1258821823337952
            double variance = kumaraswamyDistribution.Variance; //0.045500725605275683
            double median = kumaraswamyDistribution.Median; //0.016262209853672775
            double mode = kumaraswamyDistribution.Mode; //NaN  

            double pdf = kumaraswamyDistribution.ProbabilityDensityFunction(0.3); //0.46195081771596241
            double cdf = kumaraswamyDistribution.CumulativeHazardFunction(0.3); //1.8501524192880519
            string tostr = kumaraswamyDistribution.ToString(CultureInfo.InvariantCulture); // Kumaraswamy(x; a = 0.2, b = 1.2)

            Assert.AreEqual(tostr, "Kumaraswamy(x; a = 0.2, b = 1.2)");
            Assert.AreEqual(mean, 0.1258821823337952);
            Assert.AreEqual(variance, 0.045500725605275683);
            Assert.AreEqual(median, 0.016262209853672775);
            Assert.AreEqual(mode, double.NaN);
            Assert.AreEqual(pdf, 0.46195081771596241);
            Assert.AreEqual(cdf, 1.8501524192880519);
            
            /// values verified in R package = ActuDistns
        }
    }
}
