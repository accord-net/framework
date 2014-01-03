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

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Kernels;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class DynamicTimeWarpingTest
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
        public void FunctionTest2()
        {

        }

        [TestMethod()]
        public void FunctionTest()
        {
            var x = new double[] { 0, 4, 2, 1 };
            var y = new double[] { 3, 2, };

            DynamicTimeWarping target;
            double expected, actual;


            target = new DynamicTimeWarping(1);

            expected = 1;
            actual = target.Function(x, x);
            Assert.AreEqual(expected, actual, 0.000001);

            expected = 1;
            actual = target.Function(y, y);
            Assert.AreEqual(expected, actual, 0.000001);

            expected = -0.076696513742007991;
            actual = target.Function(x, y);
            Assert.AreEqual(expected, actual, 0.000001);
            actual = target.Function(y, x);
            Assert.AreEqual(expected, actual, 0.000001);


            target = new DynamicTimeWarping(2, 1.42);
            var z = new double[] { 3, 2, 1, 5, 7, 8 };

            expected = 1;
            actual = target.Function(x, x);
            Assert.AreEqual(expected, actual, 0.000001);

            expected = 1;
            actual = target.Function(y, y);
            Assert.AreEqual(expected, actual, 0.000001);

            expected = 1;
            actual = target.Function(z, z);
            Assert.AreEqual(expected, actual, 0.000001);

            expected = -0.10903562560104614;
            actual = target.Function(x, z);
            Assert.AreEqual(expected, actual, 0.000001);
            actual = target.Function(z, x);
            Assert.AreEqual(expected, actual, 0.000001);

            expected = 0.4208878392918925;
            actual = target.Function(x, y);
            Assert.AreEqual(expected, actual, 0.000001);
            actual = target.Function(y, x);
            Assert.AreEqual(expected, actual, 0.000001);


            target = new DynamicTimeWarping(1, 0.0000000001);

            expected = 1;
            actual = target.Function(x, x);
            Assert.AreEqual(expected, actual);

            expected = 1;
            actual = target.Function(y, y);
            Assert.AreEqual(expected, actual);

            expected = 0.000000000033333397321663391;
            actual = target.Function(x, y);
            Assert.AreEqual(expected, actual);
            actual = target.Function(y, x);
            Assert.AreEqual(expected, actual);



            target = new DynamicTimeWarping(1, 292.12);

            expected = 1;
            actual = target.Function(x, x);
            Assert.AreEqual(expected, actual, 0.000001);
            actual = target.Function(y, y);
            Assert.AreEqual(expected, actual, 0.000001);

            expected = 0.99985353675500488;
            actual = target.Function(x, y);
            Assert.AreEqual(expected, actual);
            actual = target.Function(y, x);
            Assert.AreEqual(expected, actual);
        }


    }
}
