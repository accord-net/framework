using Accord.Statistics.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Accord.Tests.Statistics
{

    [TestClass()]
    public class SignTestTest
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

            double[] sample = 
            {
                1, 1, 2, 2, 3, 3, 4, 5, 5, 6, 7, 7, 8, 10,
                20, 22, 25, 27, 33, 40, 42, 50, 55, 75, 80 
            };

            SignTest target = new SignTest(sample, hypothesizedMedian: 30);

            Assert.AreEqual(OneSampleHypothesis.ValueIsDifferentFromHypothesis, target.Hypothesis);
            Assert.AreEqual(0.043285, target.PValue, 1e-4);
            Assert.IsTrue(target.Significant);

        }


    }
}
