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
    public class PolynomialTest
    {

        [Test]
        public void DistanceTest()
        {
            Polynomial target = new Polynomial(1);

            double[] x = new double[] { 1, 1 };
            double[] y = new double[] { 1, 1 };

            double expected = 0;
            double actual = target.Distance(x, y);
            Assert.AreEqual(expected, actual);


            x = new double[] { 0.5, 2.0 };
            y = new double[] { 1.3, -0.2 };

            expected = 5.48;
            actual = target.Distance(x, y);

            Assert.AreEqual(expected, actual);


            target = new Polynomial(3);

            x = new double[] { 9.4, 22.1 };
            y = new double[] { -6.21, 4 };

            expected = 192981940.60611719;
            actual = target.Distance(x, y);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FunctionTest()
        {
            Polynomial target = new Polynomial(1, 0);

            double[] x = new double[] { 1, 1 };
            double[] y = new double[] { 1, 1 };

            double expected = 2;
            double actual = target.Function(x, y);
            Assert.AreEqual(expected, actual);


            x = new double[] { 0.5, 2.0 };
            y = new double[] { 1.3, -0.2 };

            expected = 0.25;
            actual = target.Function(x, y);

            Assert.AreEqual(expected, actual);


            target = new Polynomial(3, 0);

            x = new double[] { 9.4, 22.1 };
            y = new double[] { -6.21, 4 };

            expected = System.Math.Pow(x.InnerProduct(y), 3);
            actual = target.Function(x, y);
            Assert.AreEqual(expected, actual, 0.0001);
        }

        [Test]
        public void FunctionTest2()
        {
            // Tested against R's kernlab

            double[][] data = 
            {
                new double[] { 5.1, 3.5, 1.4, 0.2 },
                new double[] { 5.0, 3.6, 1.4, 0.2 },
                new double[] { 4.9, 3.0, 1.4, 0.2 },
                new double[] { 5.8, 4.0, 1.2, 0.2 },
                new double[] { 4.7, 3.2, 1.3, 0.2 },
            };

            // rbf <- polydot(3)

            Polynomial kernel = new Polynomial(degree: 3, constant: 1);

            // Compute the kernel matrix
            double[,] actual = new double[5, 5];
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    actual[i, j] = kernel.Function(data[i], data[j]);

            double[,] expected =
            {
                { 70240.51, 69426.53, 57022.17,  99252.85, 55002.06 },
                { 69426.53, 68719.48, 56181.89,  98099.75, 54353.80 },
                { 57022.17, 56181.89, 46694.89,  80286.11, 44701.08 },
                { 99252.85, 98099.75, 80286.11, 141583.69, 77635.89 },
                { 55002.06, 54353.80, 44701.08,  77635.89, 43095.88 },

            };

            // Assert both are equal
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    Assert.AreEqual(expected[i, j], actual[i, j], 1e-2);
        }

        [Test]
        public void ExpandDistanceTest()
        {
            for (int i = 1; i <= 10; i++)
            {
                Polynomial kernel = new Polynomial(i, 0);

                var x = new double[] { 0.5, 2.0 };
                var y = new double[] { 1.3, -0.2 };

                var phi_x = kernel.Transform(x);
                var phi_y = kernel.Transform(y);

                double d1 = Distance.SquareEuclidean(phi_x, phi_y);
                double d2 = kernel.Distance(x, y);
                double d3 = Accord.Statistics.Tools.Distance(kernel, x, y);

                Assert.AreEqual(d1, d2, 1e-4);
                Assert.AreEqual(d1, d3, 1e-4);
                Assert.IsFalse(double.IsNaN(d1));
                Assert.IsFalse(double.IsNaN(d2));
                Assert.IsFalse(double.IsNaN(d3));
            }
        }

        [Test]
        public void ExpandReverseDistanceTest()
        {
            for (int i = 1; i <= 10; i++)
            {
                Polynomial kernel = new Polynomial(i, 0);

                var x = new double[] { 0.5, 2.0 };
                var y = new double[] { 1.3, -0.2 };

                var phi_x = kernel.Transform(x);
                var phi_y = kernel.Transform(y);

                //int expected_size = (int)System.Math.Pow(x.Length, i);
                //Assert.AreEqual(phi_x.Length, phi_y.Length);
                //Assert.AreEqual(phi_x.Length, expected_size);

                double d = Distance.SquareEuclidean(x, y);
                double phi_d = kernel.ReverseDistance(phi_x, phi_y);

                Assert.AreEqual(phi_d, d, 1e-6);
                Assert.IsFalse(double.IsNaN(phi_d));
                Assert.IsFalse(double.IsNaN(d));
            }
        }

        [Test]
        public void TransformTest_Linear()
        {
            double[][] data = 
            {
                new double[] { 5.1, 3.5, 1.4, 0.2 },
                new double[] { 5.0, 3.6, 1.4, 0.2 },
                new double[] { 4.9, 3.0, 1.4, 0.2 },
                new double[] { 5.8, 4.0, 1.2, 0.2 },
                new double[] { 4.7, 3.2, 1.3, 0.2 },
            };

            var target = new Polynomial(1);
            var linear = new Linear(constant: 1);
            Assert.AreEqual(target.Constant, linear.Constant);

            double[][] expected = data.Apply(x => linear.Transform(x));
            double[][] actual = data.Apply(target.Transform);

            Assert.IsTrue(expected.IsEqual(actual, 1e-10));
        }

        [Test]
        public void TransformTest_Quadratic()
        {
            double[][] data = 
            {
                new double[] { 5.1, 3.5, 1.4, 0.2 },
                new double[] { 5.0, 3.6, 1.4, 0.2 },
                new double[] { 4.9, 3.0, 1.4, 0.2 },
                new double[] { 5.8, 4.0, 1.2, 0.2 },
                new double[] { 4.7, 3.2, 1.3, 0.2 },
            };

            var target = new Polynomial(2);
            var quadratic = new Quadratic();

            double[][] expected = data.Apply(x => quadratic.Transform(x));
            double[][] actual = data.Apply(target.Transform);

            Assert.IsTrue(expected.IsEqual(actual, 1e-10));
        }

        [Test]
        public void DegreeChangeTest()
        {
            // https://github.com/accord-net/framework/issues/745
            var polynomial = new Polynomial();
            polynomial.Degree = 3;
            Assert.AreEqual(3, polynomial.Degree);
        }

    }
}
