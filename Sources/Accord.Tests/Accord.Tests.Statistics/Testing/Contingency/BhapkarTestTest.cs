// Accord Unit Tests
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;    
    
    [TestClass()]
    public class BhapkarTestTest
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
        public void BhapkarTestConstructorTest()
        {
            // Bhapkar, V.P. (1966). A note on the equivalence of two test criteria
            // for hypotheses in categorical data. Journal of the American Statistical
            // Association, 61, 228-235.

            int[,] vision =
            {
                { 1520,  266,  124,  66 },
                {  234, 1512,  432,  78 },
                {  117,  362, 1772, 205 },
                {   36,   82,  179, 492 },
            };

            GeneralConfusionMatrix a = new GeneralConfusionMatrix(vision);

            BhapkarTest target = new BhapkarTest(a);

            Assert.AreEqual(11.97572, target.Statistic, 1e-5);
            Assert.AreEqual(0.00746679746972, target.PValue, 1e-6);
            Assert.AreEqual(3, target.DegreesOfFreedom);

            Assert.IsFalse(double.IsNaN(target.Statistic));
            Assert.IsFalse(double.IsNaN(target.PValue));
        }

    }
}
