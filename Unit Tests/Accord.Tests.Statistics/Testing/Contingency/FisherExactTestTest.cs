

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Testing;
    using NUnit.Framework;
    using System;
    using Accord.Statistics.Analysis;

    [TestFixture]
    public class FisherExactTestTest
    {
        [Test]
        public void doc_test()
        {
            #region doc_test
            // Example from https://en.wikipedia.org/wiki/Fisher%27s_exact_test

            // This example comes directly from the Wikipedia page referenced above. In this example, 
            // a sample of teenagers might be divided into male and female on the one hand, and those
            // that are and are not currently studying for a statistics exam on the other. We hypothesize,
            // for example, that the proportion of studying individuals is higher among the women than
            // among the men, and we want to test whether any difference of proportions that we observe
            // is significant. The data might look like this:

            var matrix = new ConfusionMatrix(new int[,] 
            {
                /*                    Men     Women  */
                /* Studying    */  {   1,       9    },
                /* Not-studying*/  {  11,       3    },
            });

            // Let's check that the row totals and column
            // totals match the ones reported in Wikipedia:
            int[] rowTotals = matrix.RowTotals;    // should be 10, 14
            int[] colTotals = matrix.ColumnTotals; // should be 12, 12
            int total = matrix.NumberOfSamples;    // should be 24

            // The question we ask about these data is: knowing that 10 of these 24 teenagers are studiers,
            // and that 12 of the 24 are female, and assuming the null hypothesis that men and women are 
            // equally likely to study, what is the probability that these 10 studiers would be so unevenly
            // distributed between the women and the men? If we were to choose 10 of the teenagers at random,
            // what is the probability that 9 or more of them would be among the 12 women, and only 1 or fewer
            // from among the 12 men?

            var fet = new FisherExactTest(matrix, alternate: OneSampleHypothesis.ValueIsDifferentFromHypothesis);
            double p = fet.PValue; // should be approximately 0.0027594561852200832
            bool significant = fet.Significant; // should be true

            // The same result can be verified in R using:
            //  fisher.test(matrix(c(1, 9, 11, 3), 2,2))
            /*
                Fisher's Exact Test for Count Data

                data: matrix(c(1, 9, 11, 3), 2, 2)
                p - value = 0.002759
                alternative hypothesis: true odds ratio is not equal to 1
                95 percent confidence interval:
                            0.0006438284 0.4258840381
                sample estimates:
                odds ratio
                0.03723312
            */
            #endregion

            Assert.AreEqual(1, matrix.Matrix[0, 0]);
            Assert.AreEqual(9, matrix.Matrix[0, 1]);
            Assert.AreEqual(11, matrix.Matrix[1, 0]);
            Assert.AreEqual(3, matrix.Matrix[1, 1]);

            Assert.AreEqual(new[] { 10, 14 }, rowTotals);
            Assert.AreEqual(new[] { 12, 12 }, colTotals);
            Assert.AreEqual(24, total);
            Assert.AreEqual(0.0027594561852200832, p);
            Assert.IsTrue(significant);
        }

        [Test]
        public void FisherExactTestConstructorTest1()
        {
            // Example from http://rfd.uoregon.edu/files/rfd/StatisticalResources/lec_05a.txt

            ConfusionMatrix matrix = new ConfusionMatrix
            (
                 14, 10,
                 21, 3
            );

            {
                var target = new FisherExactTest(matrix, OneSampleHypothesis.ValueIsSmallerThanHypothesis);
                Assert.AreEqual(OneSampleHypothesis.ValueIsSmallerThanHypothesis, target.Hypothesis);
                Assert.AreEqual(DistributionTail.OneLower, target.Tail);
                Assert.AreEqual(0.02450, target.PValue, 1e-5);
            }

            {
                var target = new FisherExactTest(matrix, OneSampleHypothesis.ValueIsDifferentFromHypothesis);
                Assert.AreEqual(OneSampleHypothesis.ValueIsDifferentFromHypothesis, target.Hypothesis);
                Assert.AreEqual(DistributionTail.TwoTail, target.Tail);
                Assert.AreEqual(0.04899, target.PValue, 1e-4);
            }

            {
                var target = new FisherExactTest(matrix, OneSampleHypothesis.ValueIsGreaterThanHypothesis);
                Assert.AreEqual(OneSampleHypothesis.ValueIsGreaterThanHypothesis, target.Hypothesis);
                Assert.AreEqual(DistributionTail.OneUpper, target.Tail);
                Assert.AreEqual(0.99607, target.PValue, 1e-4);
            }
        }
    }
}
