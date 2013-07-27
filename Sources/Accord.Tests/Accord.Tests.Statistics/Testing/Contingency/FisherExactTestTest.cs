

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

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


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
