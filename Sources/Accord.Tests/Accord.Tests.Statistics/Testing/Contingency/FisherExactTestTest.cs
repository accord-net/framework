

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Statistics.Analysis;

    [TestClass()]
    public class FisherExactTestTest
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
