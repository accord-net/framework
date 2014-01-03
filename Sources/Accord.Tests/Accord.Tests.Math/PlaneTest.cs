// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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

namespace Accord.Tests.Math
{
    using Accord.Math.Geometry;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using AForge.Math;
    using System.Globalization;
    using Accord.Math;
    
    [TestClass()]
    public class PlaneTest
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
        public void FromPointsTest()
        {
            Point3 point1 = new Point3(0, 1, -7);
            Point3 point2 = new Point3(3, 1, -9);
            Point3 point3 = new Point3(0, -5, -8);

            Plane actual = Plane.FromPoints(point1, point2, point3);
            Vector3 expected = new Vector3(-12, 3, -18);

            Assert.AreEqual(expected, actual.Normal);
        }

        [TestMethod()]
        public void FromPointsTest2()
        {
            Point3 point1 = new Point3(1, 2, -2);
            Point3 point2 = new Point3(3, -2, 1);
            Point3 point3 = new Point3(5, 1, -4);

            Plane expected = new Plane(11, 16, 14, -15);
            Plane actual = Plane.FromPoints(point1, point2, point3);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ToStringTest()
        {
            Plane target = new Plane(-12, 3, -18, 1);

            {
                string expected = "-12x +3y -18z +1 = 0";
                string actual = target.ToString();
                Assert.AreEqual(expected, actual);
            }

            {
                string expected = "x = +0.25y -1.5z +0.08333334";
                string actual = target.ToString('x', CultureInfo.InvariantCulture);
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
