

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    
    [TestClass()]
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



        [TestMethod()]
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
    }
}
