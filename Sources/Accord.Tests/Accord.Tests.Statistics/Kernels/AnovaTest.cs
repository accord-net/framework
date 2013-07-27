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
    using Accord.Statistics.Kernels;
    using Microsoft.VisualStudio.TestTools.UnitTesting;


    [TestClass()]
    public class AnovaTest
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
        public void FunctionTest()
        {
            int s = 2;
            int p = 2;
            Anova target = new Anova(s, p);

            // Values from examples by Manju M. Pai, 2003
            // The code, however, is computed differently.

            double[] x = new double[] { 5, 4 };
            double[] y = new double[] { 3, 2 };

            double expected = 120;
            double actual = target.Function(x, y);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void FunctionTest2()
        {
            int s = 2;
            int p = 1;
            Anova target = new Anova(s, p);

            double[] x = new double[] { 5, 4 };
            double[] y = new double[] { 3, 2 };

            double expected = 23;
            double actual = target.Function(x, y);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void FunctionTest3()
        {
            int s = 5;
            int p = 3;
            Anova target = new Anova(s, p);

            double[] x = new double[] { 5, 4, 3, 1, 1 };
            double[] y = new double[] { 3, 2, 5, 4, 1 };

            double expected = 4277;
            double actual = target.Function(x, y);
            Assert.AreEqual(expected, actual);
        }
    }
}
