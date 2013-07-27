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
    using Accord.Math;
    

    [TestClass()]
    public class StuartMaxwellTestTest
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
        public void StuartMaxwellTestConstructorTest()
        {
            // Example from http://www.john-uebersax.com/stat/mcnemar.htm#stuart

            int[,] matrix = 
            {
                { 20, 10,  5 },
                {  3, 30, 15 },
                {  0,  5, 40 },
            };

            GeneralConfusionMatrix a = new GeneralConfusionMatrix(matrix);
            
            StuartMaxwellTest target = new StuartMaxwellTest(a);

            Assert.AreEqual(13.76, target.Statistic, 0.01);
            Assert.AreEqual(2, target.DegreesOfFreedom);
            Assert.AreEqual(0.001, target.PValue, 1e-4);

            Assert.IsFalse(double.IsNaN(target.Statistic));
            Assert.IsFalse(double.IsNaN(target.PValue));
        }

       
    }
}
