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
    using NUnit.Framework;
    using Accord.Math;

    [TestFixture]
    public class QuadraticTest
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
        public void DistanceTest()
        {
            Quadratic target = new Quadratic(1);

            double[] x = new double[] { 1, 1 };
            double[] y = new double[] { 1, 1 };

            double expected = 0;
            double actual = target.Distance(x, y);
            Assert.AreEqual(expected, actual);


            x = new double[] { 0.5, 2.0 };
            y = new double[] { 1.3, -0.2 };

            expected = 31.8904;
            actual = target.Distance(x, y);

            Assert.AreEqual(expected, actual);


            target = new Quadratic(3);

            x = new double[] { 9.4, 22.1 };
            y = new double[] { -6.21, 4 };

            expected = 337265.44515681011;
            actual = target.Distance(x, y);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FunctionTest()
        {
            Quadratic target = new Quadratic(0);

            double[] x = new double[] { 1, 1 };
            double[] y = new double[] { 1, 1 };

            double expected = System.Math.Pow(x.InnerProduct(y), 2);
            double actual = target.Function(x, y);
            Assert.AreEqual(expected, actual);


            x = new double[] { 0.5, 2.0 };
            y = new double[] { 1.3, -0.2 };

            expected = 0.0625;
            actual = target.Function(x, y);

            Assert.AreEqual(expected, actual);


            target = new Quadratic(4.2);

            x = new double[] { 9.4, 22.1 };
            y = new double[] { -6.21, 4 };

            expected = System.Math.Pow(x.InnerProduct(y) + 4.2, 2);
            actual = target.Function(x, y);
            Assert.AreEqual(expected, actual, 0.0001);
        }

        [Test]
        public void ExpandDistanceTest()
        {
            Quadratic kernel = new Quadratic(0);

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
            Quadratic kernel = new Quadratic(0);

            var x = new double[] { 0.5, 2.0 };
            var y = new double[] { 1.3, -0.2 };

            var phi_x = kernel.Transform(x);
            var phi_y = kernel.Transform(y);

            double d = Distance.SquareEuclidean(x, y);
            double phi_d = kernel.ReverseDistance(phi_x, phi_y);

            Assert.AreEqual(phi_d, d);
        }

        [Test]
        public void ExpandDistanceWithConstantTest()
        {
            Quadratic kernel = new Quadratic(4.2);

            var x = new double[] { 0.5, 2.0 };
            var y = new double[] { 1.3, -0.2 };

            var phi_x = kernel.Transform(x);
            var phi_y = kernel.Transform(y);

            double d1 = Distance.SquareEuclidean(phi_x, phi_y);
            double d2 = kernel.Distance(x, y);
            double d3 = Accord.Statistics.Tools.Distance(kernel, x, y);

            Assert.AreEqual(66.9624, d1);
            Assert.AreEqual(66.9624, d2, 1e-10);
            Assert.AreEqual(66.9624, d3, 1e-10);

            Assert.IsFalse(double.IsNaN(d1));
            Assert.IsFalse(double.IsNaN(d2));
            Assert.IsFalse(double.IsNaN(d3));
        }

        [Test]
        public void ExpandReverseDistanceWithConstantTest()
        {
            Quadratic kernel = new Quadratic(4.2);

            var x = new double[] { 0.5, 2.0 };
            var y = new double[] { 1.3, -0.2 };

            var phi_x = kernel.Transform(x);
            var phi_y = kernel.Transform(y);

            double d = Distance.SquareEuclidean(x, y);
            double phi_d = kernel.ReverseDistance(phi_x, phi_y);

            Assert.AreEqual(5.48, phi_d, 1e-10);
            Assert.AreEqual(5.48, d);

            Assert.IsFalse(double.IsNaN(phi_d));
            Assert.IsFalse(double.IsNaN(d));
        }
    }
}
