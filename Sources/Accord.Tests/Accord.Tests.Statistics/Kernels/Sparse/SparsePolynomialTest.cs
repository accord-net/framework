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
    using Accord.Statistics.Kernels.Sparse;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class SparsePolynomialTest
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
            Polynomial dense = new Polynomial(3);
            SparsePolynomial target = new SparsePolynomial(3);

            double[] sx = { 1, -0.555556, 2, +0.250000, 3, -0.864407, 4, -0.916667 };
            double[] sy = { 1, -0.666667, 2, -0.166667, 3, -0.864407, 4, -0.916667 };
            double[] sz = { 1, -0.944444, 3, -0.898305, 4, -0.916667 };

            double[] dx = { -0.555556, +0.250000, -0.864407, -0.916667 };
            double[] dy = { -0.666667, -0.166667, -0.864407, -0.916667 };
            double[] dz = { -0.944444, +0.000000, -0.898305, -0.916667 };

            double expected, actual;

            expected = dense.Function(dx, dy);
            actual = target.Function(sx, sy);
            Assert.AreEqual(expected, actual);

            expected = dense.Function(dx, dz);
            actual = target.Function(sx, sz);
            Assert.AreEqual(expected, actual);

            expected = dense.Function(dy, dz);
            actual = target.Function(sy, sz);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void DistanceTest()
        {
            Polynomial dense = new Polynomial(3);
            SparsePolynomial target = new SparsePolynomial(3);

            double[] sx = { 1, -0.555556, 2, +0.250000, 3, -0.864407, 4, -0.916667 };
            double[] sy = { 1, -0.666667, 2, -0.166667, 3, -0.864407, 4, -0.916667 };
            double[] sz = { 1, -0.944444, 3, -0.898305, 4, -0.916667 };

            double[] dx = { -0.555556, +0.250000, -0.864407, -0.916667 };
            double[] dy = { -0.666667, -0.166667, -0.864407, -0.916667 };
            double[] dz = { -0.944444, +0.000000, -0.898305, -0.916667 };

            double expected, actual;

            expected = dense.Distance(dx, dy);
            actual = target.Distance(sx, sy);
            Assert.AreEqual(expected, actual);

            expected = dense.Distance(dx, dz);
            actual = target.Distance(sx, sz);
            Assert.AreEqual(expected, actual);

            expected = dense.Distance(dy, dz);
            actual = target.Distance(sy, sz);
            Assert.AreEqual(expected, actual);
        }
    }
}
