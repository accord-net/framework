

namespace Accord.Tests.Statistics
{
    using Accord.Math;
    using Accord.Statistics.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass()]
    public class MannWhitneyWilcoxonTestTest
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
        public void MannWhitneyWilcoxonTestConstructorTest()
        {
            // The following example comes from Richard Lowry's page at
            // http://vassarstats.net/textbook/ch11a.html. As stated by
            // Richard, this example deals with persons seeking treatment
            // by claustrophobia. Those persons are randomly divided into
            // two groups, and each group receive a different treatment
            // for the disorder.
            
            // The hypothesis would be that treatment A would more effective
            // than B. To check this hypothesis, we can use Mann-Whitney's Test
            // to compare the medians of both groups.
            
            // Claustrophobia test scores for people treated with treatment A
            double[] sample1 = { 4.6, 4.7, 4.9, 5.1, 5.2, 5.5, 5.8, 6.1, 6.5, 6.5, 7.2 };

            // Claustrophobia test scores for people treated with treatment B
            double[] sample2 = { 5.2, 5.3, 5.4, 5.6, 6.2, 6.3, 6.8, 7.7, 8.0, 8.1 };

            // Create a new Mann-Whitney-Wilcoxon's test to compare the two samples
            MannWhitneyWilcoxonTest test = new MannWhitneyWilcoxonTest(sample1, sample2,
                TwoSampleHypothesis.FirstValueIsSmallerThanSecond);

            Assert.AreEqual(096.5, test.RankSum1);
            Assert.AreEqual(134.5, test.RankSum2);

            Assert.AreEqual(79.5, test.Statistic1);
            Assert.AreEqual(30.5, test.Statistic2);

            Assert.AreEqual(TwoSampleHypothesis.FirstValueIsSmallerThanSecond, test.Hypothesis);

            // Directional should be significant
            Assert.IsTrue(test.Significant);
            Assert.AreEqual(0.043834132843420748, test.PValue);


            test = new MannWhitneyWilcoxonTest(sample1, sample2);

            Assert.AreEqual(TwoSampleHypothesis.ValuesAreDifferent, test.Hypothesis);

            double[] expectedRank1 = { 1, 2, 3, 4, 5.5, 9, 11, 12, 15.5, 15.5, 18 };
            double[] expectedRank2 = { 5.5, 7, 8, 10, 13, 14, 17, 19, 20, 21 };

            Assert.IsTrue(expectedRank1.IsEqual(expectedRank1));
            Assert.IsTrue(expectedRank2.IsEqual(expectedRank2));

            Assert.AreEqual(096.5, test.RankSum1);
            Assert.AreEqual(134.5, test.RankSum2);

            Assert.AreEqual(79.5, test.Statistic1);
            Assert.AreEqual(30.5, test.Statistic2);

            // Non-directional is non-significant
            Assert.IsFalse(test.Significant);
        }

        [TestMethod()]
        public void MannWhitneyWilcoxonTestConstructorTest1()
        {
            // Example from Conover, "Practical nonparametric statistics", 1999  (pg. 218)

            double[] sample1 = { 14.8, 7.3, 5.6, 6.3, 9.0, 4.2, 10.6, 12.5, 12.9, 16.1, 11.4, 2.7 };

            double[] sample2 = 
            {
                12.7, 14.2, 12.6, 2.1, 17.7, 11.8, 16.9, 7.9, 16.0, 10.6,      5.6,
                5.6, 7.6, 11.3, 8.3,6.7, 3.6, 1.0, 2.4, 6.4, 9.1, 6.7, 18.6, 3.2, 
                6.2, 6.1, 15.3, 10.6, 1.8, 5.9, 9.9, 10.6, 14.8, 5.0, 2.6, 4.0
            };

            var target = new MannWhitneyWilcoxonTest(sample1, sample2);

            Assert.AreEqual(0.529, target.PValue, 1e-3);
        }
    }
}
