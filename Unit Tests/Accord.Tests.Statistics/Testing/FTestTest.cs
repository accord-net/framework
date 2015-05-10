

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Testing;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class FTestTest
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
        public void FTestConstructorTest()
        {
            double var1 = 1.05766555271071;
            double var2 = 1.16570834301777;
            int d1 = 49;
            int d2 = 49;

            FTest oneGreater = new FTest(var1, var2, d1, d2, TwoSampleHypothesis.FirstValueIsGreaterThanSecond);
            FTest oneSmaller = new FTest(var1, var2, d1, d2, TwoSampleHypothesis.FirstValueIsSmallerThanSecond);
            FTest twoTail = new FTest(var1, var2, d1, d2, TwoSampleHypothesis.ValuesAreDifferent);

            Assert.AreEqual(0.632, oneGreater.PValue, 1e-3);
            Assert.AreEqual(0.367, oneSmaller.PValue, 1e-3);
            Assert.AreEqual(0.734, twoTail.PValue, 1e-3);

            Assert.IsFalse(Double.IsNaN(oneGreater.PValue));
            Assert.IsFalse(Double.IsNaN(oneSmaller.PValue));
            Assert.IsFalse(Double.IsNaN(twoTail.PValue));
        }

        [Test]
        public void FTestConstructorTest2()
        {
            // The following example has been based on the page "F-Test for Equality 
            // of Two Variances", from NIST/SEMATECH e-Handbook of Statistical Methods:
            //
            //  http://www.itl.nist.gov/div898/handbook/eda/section3/eda359.htm
            //

            // Consider a data set containing 480 ceramic strength 
            // measurements for two batches of material. The summary
            // statistics for each batch are shown below:

            // BATCH 1:
            int numberOfObservations1 = 240;
            // double mean1 = 688.9987;
            double stdDev1 = 65.54909;
            double var1 = stdDev1 * stdDev1;

            // BATCH 2:
            int numberOfObservations2 = 240;
            // double mean2 = 611.1559;
            double stdDev2 = 61.85425;
            double var2 = stdDev2 * stdDev2;

            // Here, we will be testing the null hypothesis that
            // the variances for the two batches are equal.

            int degreesOfFreedom1 = numberOfObservations1 - 1;
            int degreesOfFreedom2 = numberOfObservations2 - 1;

            // Now we can create a F-Test to test the difference between variances
            var ftest = new FTest(var1, var2, degreesOfFreedom1, degreesOfFreedom2);

            double statistic = ftest.Statistic; // 1.123037
            double pvalue = ftest.PValue;       // 0.185191
            bool significant = ftest.Significant; // false

            // The F test indicates that there is not enough evidence 
            // to reject the null hypothesis that the two batch variances
            // are equal at the 0.05 significance level.

            Assert.AreEqual(1.1230374607443194, statistic);
            Assert.AreEqual(0.18519157993853722, pvalue);
            Assert.IsFalse(significant);
        }
    }
}
