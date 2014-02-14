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



        [TestMethod()]
        public void SignTestConstructorTest()
        {
            // Example from http://www.unm.edu/~marcusj/1Samplesign.pdf

            double[] sample = 
            {
                1, 1, 2, 2, 3, 3, 4, 5, 5, 6, 7, 7, 8, 10,
                20, 22, 25, 27, 33, 40, 42, 50, 55, 75, 80 
            };

            SignTest target = new SignTest(sample, hypothesizedMedian: 30);

            // Wolfram Alpha gives 0.02896
            // GNU R gives 0.04329

            Assert.AreEqual(OneSampleHypothesis.ValueIsDifferentFromHypothesis, target.Hypothesis);
            Assert.AreEqual(0.043285, target.PValue, 1e-4);
            Assert.IsTrue(target.Significant);

        }


    }
}
