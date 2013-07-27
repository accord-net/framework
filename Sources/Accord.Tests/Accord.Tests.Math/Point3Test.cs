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

using Accord.Math.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Accord.Math;

namespace Accord.Tests.Math
{


    [TestClass()]
    public class Point3Test
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
        public void CollinearTest()
        {
            {
                Point3 p1 = new Point3(0, 0, 0);
                Point3 p2 = new Point3(0, 0, 1);
                Point3 p3 = new Point3(0, 0, 2);

                bool expected = true;
                bool actual = Point3.Collinear(p1, p2, p3);

                Assert.AreEqual(expected, actual);
            }

            {
                Point3 p1 = new Point3(1, 0, 0);
                Point3 p2 = new Point3(0, 2, 1);
                Point3 p3 = new Point3(0, 0, 2);

                bool expected = false;
                bool actual = Point3.Collinear(p1, p2, p3);

                Assert.AreEqual(expected, actual);
            }

            {
                Point3 p1 = new Point3(134, 268, 402);
                Point3 p2 = new Point3(329, 658, 98);
                Point3 p3 = new Point3(125, 250, 375);

                bool expected = false;
                bool actual = Point3.Collinear(p1, p2, p3);

                Assert.AreEqual(expected, actual);
            }
        }
    }
}
