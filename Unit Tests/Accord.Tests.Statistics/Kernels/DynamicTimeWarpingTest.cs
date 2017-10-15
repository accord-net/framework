// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
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
    using Accord.Math;
    using Accord.Math.Distances;
    using Accord.Statistics.Kernels;
    using NUnit.Framework;

    [TestFixture]
    public class DynamicTimeWarpingTest
    {


        [Test]
        public void FunctionTest()
        {
            var x = new double[] { 0, 4, 2, 1 };
            var y = new double[] { 3, 2, };

            DynamicTimeWarping target;
            double expected, actual;


            target = new DynamicTimeWarping(1);

            expected = 1;
            actual = target.Function(x, x);
            Assert.AreEqual(expected, actual, 1e-5);

            expected = 1;
            actual = target.Function(y, y);
            Assert.AreEqual(expected, actual, 1e-5);

            expected = -0.076696513742007991;
            actual = target.Function(x, y);
            Assert.AreEqual(expected, actual, 1e-5);
            actual = target.Function(y, x);
            Assert.AreEqual(expected, actual, 1e-5);


            target = new DynamicTimeWarping(2, 1.42);
            var z = new double[] { 3, 2, 1, 5, 7, 8 };

            expected = 1;
            actual = target.Function(x, x);
            Assert.AreEqual(expected, actual, 1e-5);

            expected = 1;
            actual = target.Function(y, y);
            Assert.AreEqual(expected, actual, 1e-5);

            expected = 1;
            actual = target.Function(z, z);
            Assert.AreEqual(expected, actual, 1e-5);

            expected = -0.10903562560104614;
            actual = target.Function(x, z);
            Assert.AreEqual(expected, actual, 1e-5);
            actual = target.Function(z, x);
            Assert.AreEqual(expected, actual, 1e-5);

            expected = 0.4208878392918925;
            actual = target.Function(x, y);
            Assert.AreEqual(expected, actual, 1e-5);
            actual = target.Function(y, x);
            Assert.AreEqual(expected, actual, 1e-5);


            target = new DynamicTimeWarping(1, 1e-5);

            expected = 1;
            actual = target.Function(x, x);
            Assert.AreEqual(expected, actual);

            expected = 1;
            actual = target.Function(y, y);
            Assert.AreEqual(expected, actual);

            expected = 0.000000000033333397321663391;
            actual = target.Function(x, y);
            Assert.AreEqual(expected, actual, 1e-5);
            actual = target.Function(y, x);
            Assert.AreEqual(expected, actual, 1e-5);



            target = new DynamicTimeWarping(1, 292.12);

            expected = 1;
            actual = target.Function(x, x);
            Assert.AreEqual(expected, actual, 1e-10);
            actual = target.Function(y, y);
            Assert.AreEqual(expected, actual, 1e-10);

            expected = 0.99985353675500488;
            actual = target.Function(x, y);
            Assert.AreEqual(expected, actual, 1e-10);
            actual = target.Function(y, x);
            Assert.AreEqual(expected, actual, 1e-10);
        }

        [Test]
        public void DistanceTest()
        {
            var x = new double[] { 0, 4, 2, 1 };
            var y = new double[] { 3, 2, };

            DynamicTimeWarping target;
            double expected, actual;


            target = new DynamicTimeWarping(1);

            expected = 0;
            actual = target.Distance(x, x);
            Assert.AreEqual(expected, actual, 1e-6);

            expected = 0;
            actual = target.Distance(y, y);
            Assert.AreEqual(expected, actual, 1e-10);

            expected = 2.1533930274840158;
            actual = target.Distance(x, y);
            Assert.AreEqual(expected, actual, 1e-10);
            actual = target.Distance(y, x);
            Assert.AreEqual(expected, actual, 1e-10);


            target = new DynamicTimeWarping(2, 1.42);
            var z = new double[] { 3, 2, 1, 5, 7, 8 };

            expected = 0;
            actual = target.Distance(x, x);
            Assert.AreEqual(expected, actual, 1e-10);

            expected = 0;
            actual = target.Distance(y, y);
            Assert.AreEqual(expected, actual, 1e-10);

            expected = 0;
            actual = target.Distance(z, z);
            Assert.AreEqual(expected, actual, 1e-10);

            expected = 2.2180712512020921;
            actual = target.Distance(x, z);
            Assert.AreEqual(expected, actual, 1e-10);
            actual = target.Distance(z, x);
            Assert.AreEqual(expected, actual, 1e-10);

            expected = 1.1582243214162151;
            actual = target.Distance(x, y);
            Assert.AreEqual(expected, actual, 1e-10);
            actual = target.Distance(y, x);
            Assert.AreEqual(expected, actual, 1e-10);


            target = new DynamicTimeWarping(1, 0.0000000001);

            expected = 0;
            actual = target.Distance(x, x);
            Assert.AreEqual(expected, actual);

            expected = 0;
            actual = target.Distance(y, y);
            Assert.AreEqual(expected, actual);

            expected = 1.9999999999333331;
            actual = target.Distance(x, y);
            Assert.AreEqual(expected, actual);
            actual = target.Distance(y, x);
            Assert.AreEqual(expected, actual);



            target = new DynamicTimeWarping(1, 292.12);

            expected = 0;
            actual = target.Distance(x, x);
            Assert.AreEqual(expected, actual, 1e-10);
            actual = target.Distance(y, y);
            Assert.AreEqual(expected, actual, 1e-10);

            expected = 0.00029292648999024173;
            actual = target.Distance(x, y);
            Assert.AreEqual(expected, actual, 1e-10);
            actual = target.Distance(y, x);
            Assert.AreEqual(expected, actual, 1e-10);
        }


        [Test]
        public void FunctionTest_EqualInputs()
        {
            var x = new double[] { 1, 2, 5, 1 };
            var y = new double[] { 1, 2, 5, 1 };

            var target = new DynamicTimeWarping(1, 4.2);

            double expected = target.Function(x, y);
            double actual = target.Function(x, x);

            Assert.AreEqual(expected, actual);
        }

#if !MONO
        [Test]
        public void generic_test()
        {
            var x = new int[] { 0, 4, 2, 1 };
            var y = new int[] { 3, 2, };

            double expected, actual;


            var target = new DynamicTimeWarping<Dirac<int>, int>();

            expected = 1;
            actual = target.Function(x, x);
            Assert.AreEqual(expected, actual, 1e-6);

            expected = 1;
            actual = target.Function(y, y);
            Assert.AreEqual(expected, actual, 1e-6);

            expected = 1;
            actual = target.Function(x, y);
            Assert.AreEqual(expected, actual, 1e-6);
            actual = target.Function(y, x);
            Assert.AreEqual(expected, actual, 1e-6);


            target = new DynamicTimeWarping<Dirac<int>, int>(sigma: 1.42);

            var z = new int[] { 3, 2, 1, 5, 7, 8 };

            expected = 1;
            actual = target.Function(x, x);
            Assert.AreEqual(expected, actual, 1e-6);

            expected = 1;
            actual = target.Function(y, y);
            Assert.AreEqual(expected, actual, 1e-6);

            expected = 1;
            actual = target.Function(z, z);
            Assert.AreEqual(expected, actual, 1e-6);

            expected = 0.28943244309220884;
            actual = target.Function(x, z);
            Assert.AreEqual(expected, actual, 1e-6);
            actual = target.Function(z, x);
            Assert.AreEqual(expected, actual, 1e-6);

            expected = 0.475256785561611;
            actual = target.Function(x, y);
            Assert.AreEqual(expected, actual, 1e-6);
            actual = target.Function(y, x);
            Assert.AreEqual(expected, actual, 1e-6);
        }
#endif
    }
}
