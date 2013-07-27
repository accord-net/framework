using Accord.Statistics.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Accord.Tests.Statistics
{

    [TestClass()]
    public class TwoSampleSignTestTest
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
        public void SignTestConstructorTest()
        {
            // Example from http://probabilityandstats.wordpress.com/2010/02/28/the-sign-test-more-examples/

            double[] sample1 = { 17, 26, 16, 28, 23, 35, 41, 18, 30, 29, 45, 8, 38, 31, 36 };
            double[] sample2 = { 21, 26, 19, 26, 30, 40, 43, 15, 29, 31, 46, 7, 43, 31, 37 };

            {
                TwoSampleSignTest target = new TwoSampleSignTest(sample1, sample2,
                    TwoSampleHypothesis.FirstValueIsSmallerThanSecond);

                Assert.AreEqual(TwoSampleHypothesis.FirstValueIsSmallerThanSecond, target.Hypothesis);
                Assert.AreEqual(0.1334, target.PValue, 1e-4);
                Assert.IsFalse(target.Significant);
            }
            {
                TwoSampleSignTest target = new TwoSampleSignTest(sample2, sample1,
                    TwoSampleHypothesis.FirstValueIsGreaterThanSecond);

                Assert.AreEqual(TwoSampleHypothesis.FirstValueIsGreaterThanSecond, target.Hypothesis);
                Assert.AreEqual(0.1334, target.PValue, 1e-4);
                Assert.IsFalse(target.Significant);
            }
        }

        [TestMethod()]
        public void SignTestConstructorTest2()
        {
            int positives = 9;
            int negatives = 1;

            {
                TwoSampleSignTest target = new TwoSampleSignTest(positives, positives + negatives,
                    TwoSampleHypothesis.FirstValueIsSmallerThanSecond);

                Assert.AreEqual(TwoSampleHypothesis.FirstValueIsSmallerThanSecond, target.Hypothesis);
                Assert.AreEqual(0.010742, target.PValue, 1e-5);
                Assert.IsTrue(target.Significant);
            }
            {
                TwoSampleSignTest target = new TwoSampleSignTest(negatives, positives + negatives,
                    TwoSampleHypothesis.FirstValueIsGreaterThanSecond);

                Assert.AreEqual(TwoSampleHypothesis.FirstValueIsGreaterThanSecond, target.Hypothesis);
                Assert.AreEqual(0.010742, target.PValue, 1e-5);
                Assert.IsTrue(target.Significant);
            }
        }

    }
}
