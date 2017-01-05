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
    using Accord.Statistics.Kernels;
    using Accord.Math;
    using NUnit.Framework;
    using System.Diagnostics;

    [TestFixture]
    public class LinearTest
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


        [Test]
        public void FunctionTest()
        {
            IKernel linear = new Linear(1);

            double[] x = { 1, 1 };
            double[] y = { 1, 1 };

            double actual = linear.Function(x, y);

            double expected = 3;

            Assert.AreEqual(expected, actual);


            linear = new Linear(11.5);

            x = new double[] { 0.2, 5 };
            y = new double[] { 3, 0.7 };

            actual = linear.Function(x, y);
            expected = 15.6;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DistanceTest()
        {
            var linear = new Linear(1);

            double[] x = { 1, 1 };
            double[] y = { 1, 1 };

            double actual = linear.Distance(x, y);
            double expected = 0;

            Assert.AreEqual(expected, actual);


            linear = new Linear(11.5);

            x = new double[] { 0.2, 0.5 };
            y = new double[] { 0.3, -0.7 };

            actual = linear.Distance(x, y);
            expected = Accord.Statistics.Tools.Distance(linear, x, y);

            Assert.AreEqual(expected, actual, 1e-10);
        }

        [Test]
        public void ReverseDistanceTest()
        {
            var linear = new Linear(1);

            double[] x = { 1, 1 };
            double[] y = { 1, 1 };

            double actual = linear.ReverseDistance(x, y);
            double expected = 0;

            Assert.AreEqual(expected, actual);


            linear = new Linear(0);

            x = new double[] { 0.2, 0.5 };
            y = new double[] { 0.3, -0.7 };

            actual = linear.ReverseDistance(x, y);
            expected = Accord.Math.Distance.SquareEuclidean(x, y);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FunctionTest2()
        {
            double constant = 0.1;

            Linear target = new Linear(constant);

            double[] x = { 2.0, 3.1, 4.0 };
            double[] y = { 2.0, 3.1, 4.0 };

            double expected = Matrix.InnerProduct(x, y) + constant;
            double actual;

            actual = target.Function(x, y);
            Assert.AreEqual(expected, actual);

            actual = target.Function(x, x);
            Assert.AreEqual(expected, actual);

            actual = target.Function(y, y);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ExpandDistanceTest()
        {
            Linear kernel = new Linear(42);

            var x = new double[] { 0.5, 2.0 };
            var y = new double[] { 1.3, -0.2 };

            var phi_x = kernel.Transform(x);
            var phi_y = kernel.Transform(y);

            double phi_d = Distance.SquareEuclidean(phi_x, phi_y);
            double d = kernel.Distance(x, y);

            Assert.AreEqual(phi_d, d);
        }

        [Test]
        public void ExpandReverseDistanceTest()
        {
            Linear kernel = new Linear(42);

            var x = new double[] { 0.5, 2.0 };
            var y = new double[] { 1.3, -0.2 };

            var phi_x = kernel.Transform(x);
            var phi_y = kernel.Transform(y);

            double d = Distance.SquareEuclidean(x, y);
            double phi_d = kernel.ReverseDistance(phi_x, phi_y);

            Assert.AreEqual(phi_d, d, 1e-10);
            Assert.IsFalse(double.IsNaN(phi_d));
            Assert.IsFalse(double.IsNaN(d));
        }
    }
}
