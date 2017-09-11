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

namespace Accord.Tests.Math
{
    using System;
    using Accord.Math;
    using Accord.Math.Differentiation;
    using Accord.Math.Optimization;
    using NUnit.Framework;


    [TestFixture]
    public class QuadraticObjectiveFunctionTest
    {

        [Test]
        public void QuadraticConstructorTest()
        {
            double[,] quadraticTerms =
            {
                {  1, 2, 3 },
                {  2, 5, 6 },
                {  3, 6, 9 },
            };

            double[] linearTerms = { 1, 2, 3 };

            var target = new QuadraticObjectiveFunction(quadraticTerms, linearTerms);

            var function = target.Function;
            var gradient = target.Gradient;

            FiniteDifferences fd = new FiniteDifferences(3, function);

            double[][] x =
            {
                new double[] { 1, 2, 3 },
                new double[] { 3, 1, 4 },
                new double[] { -6 , 5, 9 },
                new double[] { 31, 25, 246 },
                new double[] { -0.102, 0, 10 },
            };


            { // Function test
                for (int i = 0; i < x.Length; i++)
                {
                    double expected = 0.5 *
                        (x[i].Multiply(quadraticTerms)).InnerProduct(x[i])
                        + linearTerms.InnerProduct(x[i]);

                    double actual = function(x[i]);

                    Assert.AreEqual(expected, actual, 1e-8);
                }
            }

            { // Gradient test
                for (int i = 0; i < x.Length; i++)
                {
                    double[] expected = fd.Compute(x[i]);
                    double[] actual = gradient(x[i]);

                    for (int j = 0; j < actual.Length; j++)
                        Assert.AreEqual(expected[j], actual[j], 1e-8);
                }
            }
        }

        [Test]
        public void LinearTest()
        {
            double[,] quadraticTerms =
            {
                {  0, 0, 0 },
                {  0, 0, 0 },
                {  0, 0, 0 },
            };

            double[] linearTerms = { 1, 2, 3 };

            var target = new QuadraticObjectiveFunction(quadraticTerms, linearTerms);

            var function = target.Function;
            var gradient = target.Gradient;

            FiniteDifferences fd = new FiniteDifferences(3, function);

            double[][] x =
            {
                new double[] { 1, 2, 3 },
                new double[] { 3, 1, 4 },
                new double[] { -6 , 5, 9 },
                new double[] { 31, 25, 246 },
                new double[] { -0.102, 0, 10 },
            };


            { // Function test
                for (int i = 0; i < x.Length; i++)
                {
                    double expected = 0.5 *
                        (x[i].Multiply(quadraticTerms)).InnerProduct(x[i])
                        + linearTerms.InnerProduct(x[i]);

                    double actual = function(x[i]);

                    Assert.AreEqual(expected, actual, 1e-8);
                }
            }

            { // Gradient test
                for (int i = 0; i < x.Length; i++)
                {
                    double[] expected = fd.Compute(x[i]);
                    double[] actual = gradient(x[i]);

                    for (int j = 0; j < actual.Length; j++)
                        Assert.AreEqual(expected[j], actual[j], 1e-8);
                }
            }
        }

        [Test]
        public void HomogeneousTest()
        {
            double[,] quadraticTerms =
            {
                {  8, 3, 1 },
                {  3, 4, 2 },
                {  1, 2, 6 },
            };

            double[] linearTerms = { 0, 0, 0 };

            var target = new QuadraticObjectiveFunction(quadraticTerms, linearTerms);

            var function = target.Function;
            var gradient = target.Gradient;

            FiniteDifferences fd = new FiniteDifferences(3, function);

            double[][] x =
            {
                new double[] { 1, 2, 3 },
                new double[] { 3, 1, 4 },
                new double[] { -6 , 5, 9 },
                new double[] { 31, 25, 246 },
                new double[] { -0.102, 0, 10 },
            };

            { // Gradient test
                for (int i = 0; i < x.Length; i++)
                {
                    double[] expected = fd.Compute(x[i]);
                    double[] actual = gradient(x[i]);

                    for (int j = 0; j < actual.Length; j++)
                        Assert.AreEqual(expected[j], actual[j], 1e-6);
                }
            }
        }

        [Test]
        public void HomogeneousTest2()
        {
            double[,] quadraticTerms =
            {
                {  1, 0, 1 },
                {  0, 2, 0 },
                {  1, 0, 1 },
            };

            double[] linearTerms = { 0, 0, 0 };

            var target = new QuadraticObjectiveFunction(quadraticTerms, linearTerms);

            var function = target.Function;
            var gradient = target.Gradient;

            FiniteDifferences fd = new FiniteDifferences(3, function);

            double[][] x =
            {
                new double[] { 1, 2, 3 },
                new double[] { 3, 1, 4 },
                new double[] { -6 , 5, 9 },
                new double[] { 31, 25, 246 },
                new double[] { -0.102, 0, 10 },
            };

            { // Gradient test
                for (int i = 0; i < x.Length; i++)
                {
                    double[] expected = fd.Compute(x[i]);
                    double[] actual = gradient(x[i]);

                    for (int j = 0; j < actual.Length; j++)
                        Assert.AreEqual(expected[j], actual[j], 1e-8);
                }
            }
        }

        [Test]
        public void FunctionTest()
        {
            double x = 0;
            double y = 0;

            Func<double> expected = () => -2 * x * x + x * y - y * y + 5 * y;
            var actual = new QuadraticObjectiveFunction("-2x² + xy - y² + 5y");

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    x = (i - 5) / 10.0;
                    y = (j - 5) / 10.0;

                    double a = actual.Function(new[] { x, y });
                    double e = expected();

                    Assert.AreEqual(e, a, 1e-10);
                    Assert.IsFalse(double.IsNaN(a));
                    Assert.IsFalse(double.IsNaN(e));
                }
            }
        }

        [Test]
        public void FunctionTest2()
        {
            double x = 0;
            double y = 0;
            double z = 0;

            Func<double> expected = () => -2 * x * x + x * y - y * y - 10 * x * z + z * z;
            var actual = new QuadraticObjectiveFunction("-2x² + xy - y² - 10xz + z²");

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    for (int k = 0; k < 10; k++)
                    {
                        x = (i - 5) / 10.0;
                        y = (j - 5) / 10.0;
                        z = (k - 5) / 10.0;

                        double a = actual.Function(new[] { x, y, z });
                        double e = expected();

                        Assert.AreEqual(e, a, 1e-10);
                        Assert.IsFalse(double.IsNaN(a));
                        Assert.IsFalse(double.IsNaN(e));
                    }
                }
            }
        }

        [Test]
        public void FunctionTest3()
        {
            double x = 0;

            Func<double> expected = () => x * x + 1;
            var actual = new QuadraticObjectiveFunction("x² + 1");

            for (int i = 0; i < 10; i++)
            {
                x = (i - 5) / 10.0;

                double a = actual.Function(new[] { x });
                double e = expected();

                Assert.AreEqual(e, a, 1e-10);
                Assert.IsFalse(double.IsNaN(a));
                Assert.IsFalse(double.IsNaN(e));
            }
        }

        [Test]
        public void FunctionTest4()
        {
            double x = 0;
            double y = 0;
            double z = 0;

            Func<double> expected = () => -x * y + y * z;
            var actual = new QuadraticObjectiveFunction("-x*y + y*z");

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    for (int k = 0; k < 10; k++)
                    {
                        x = (i - 5) / 10.0;
                        y = (j - 5) / 10.0;
                        z = (k - 5) / 10.0;

                        double a = actual.Function(new[] { x, y, z });
                        double e = expected();

                        Assert.AreEqual(e, a, 1e-10);
                        Assert.IsFalse(double.IsNaN(a));
                        Assert.IsFalse(double.IsNaN(e));
                    }
                }
            }
        }

        [Test]
        public void FunctionTest5()
        {
            var f1 = new QuadraticObjectiveFunction("x² + 1");
            var f2 = new QuadraticObjectiveFunction("-x*y + y*z");
            var f3 = new QuadraticObjectiveFunction("-2x² + xy - y² - 10xz + z²");
            var f4 = new QuadraticObjectiveFunction("-2x² + xy - y² + 5y");
            var f5 = new QuadraticObjectiveFunction("2x² -5");

            double x = 0, y = 0, z = 0;
            var g1 = new QuadraticObjectiveFunction(() => x * x + 1);
            var g2 = new QuadraticObjectiveFunction(() => -x * y + y * z);
            var g3 = new QuadraticObjectiveFunction(() => -2 * x * x + x * y - y * y - 10 * x * z + z * z);
            var g4 = new QuadraticObjectiveFunction(() => -2 * x * x + x * y - y * y + 5 * y);
            var g5 = new QuadraticObjectiveFunction(() => 2 * x * x - 5);


            QuadraticObjectiveFunction[] f = { f1, f2, f3, f4, f5 };
            QuadraticObjectiveFunction[] g = { g1, g2, g3, g4, g5 };

            for (int l = 0; l < f.Length; l++)
            {
                var fl = f[l];
                var gl = g[l];

                Assert.AreEqual(fl.NumberOfVariables, gl.NumberOfVariables);

                for (int i = 0; i < 10; i++)
                    for (int j = 0; j < 10; j++)
                        for (int k = 0; k < 10; k++)
                        {
                            x = (i - 5) / 10.0;
                            y = (j - 5) / 10.0;
                            z = (k - 5) / 10.0;

                            double a = fl.Function(new[] { x, y, z }.First(fl.NumberOfVariables));
                            double e = gl.Function(new[] { x, y, z }.First(fl.NumberOfVariables));

                            Assert.AreEqual(e, a, 1e-10);
                            Assert.IsFalse(double.IsNaN(a));
                            Assert.IsFalse(double.IsNaN(e));
                        }
            }
        }

        [Test]
        public void LambdaFunctionTest()
        {
            double x = 0;
            double y = 0;

            Func<double> expected = () => -2 * x * x + x * y - y * y + 5 * y;
            var actual = new QuadraticObjectiveFunction(() => -2 * x * x + x * y - y * y + 5 * y);

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    x = (i - 5) / 10.0;
                    y = (j - 5) / 10.0;

                    double a = actual.Function(new[] { x, y });
                    double e = expected();

                    Assert.AreEqual(e, a, 1e-10);
                    Assert.IsFalse(double.IsNaN(a));
                    Assert.IsFalse(double.IsNaN(e));
                }
            }
        }

        [Test]
        public void LambdaFunctionTest2()
        {
            double x = 0;
            double y = 0;
            double z = 0;

            Func<double> expected = () => -2 * x * x + x * y - y * y - 10 * x * z + z * z;
            var actual = new QuadraticObjectiveFunction(() => -2 * x * x + x * y - y * y - 10 * x * z + z * z);

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    for (int k = 0; k < 10; k++)
                    {
                        x = (i - 5) / 10.0;
                        y = (j - 5) / 10.0;
                        z = (k - 5) / 10.0;

                        double a = actual.Function(new[] { x, y, z });
                        double e = expected();

                        Assert.AreEqual(e, a, 1e-10);
                        Assert.IsFalse(double.IsNaN(a));
                        Assert.IsFalse(double.IsNaN(e));
                    }
        }

        [Test]
        public void LambdaFunctionTest3()
        {
            double x = 0;

            Func<double> expected = () => x * x + 1;
            var actual = new QuadraticObjectiveFunction(() => x * x + 1);

            for (int i = 0; i < 10; i++)
            {
                x = (i - 5) / 10.0;

                double a = actual.Function(new[] { x });
                double e = expected();

                Assert.AreEqual(e, a, 1e-10);
                Assert.IsFalse(Double.IsNaN(a));
                Assert.IsFalse(Double.IsNaN(e));
            }
        }

        [Test]
        public void LambdaFunctionTest4()
        {
            double x = 0;
            double y = 0;
            double z = 0;

            Func<double> expected = () => -x * y + y * z;
            var actual = new QuadraticObjectiveFunction(() => -x * y + y * z);

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    for (int k = 0; k < 10; k++)
                    {
                        x = (i - 5) / 10.0;
                        y = (j - 5) / 10.0;
                        z = (k - 5) / 10.0;

                        double a = actual.Function(new[] { x, y, z });
                        double e = expected();

                        Assert.AreEqual(e, a, 1e-10);
                        Assert.IsFalse(Double.IsNaN(a));
                        Assert.IsFalse(Double.IsNaN(e));
                    }
        }

        [Test]
        public void LambdaFunctionTest5()
        {
            double x = 0;
            double y = 0;
            double z = 0;

            Func<double> expected = () => 2 * y * x - x * y + y * z;
            var actual = new QuadraticObjectiveFunction(() => 2 * y * x - x * y + y * z);

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    for (int k = 0; k < 10; k++)
                    {
                        x = (i - 5) / 10.0;
                        y = (j - 5) / 10.0;
                        z = (k - 5) / 10.0;

                        double a = actual.Function(new[] { x, y, z });
                        double e = expected();

                        Assert.AreEqual(e, a, 1e-10);
                        Assert.IsFalse(Double.IsNaN(a));
                        Assert.IsFalse(Double.IsNaN(e));
                    }
        }

        [Test]
        public void OperatorAdditionTest()
        {
            double x = 0;
            double y = 0;
            double z = 0;

            Func<double> expected1 = () => 2 * y * x - x * y + y * z;
            Func<double> expected2 = () => z * x - x * y + y * z + 4 * z + 2;
            var actual1 = new QuadraticObjectiveFunction(() => 2 * y * x - x * y + y * z);
            var actual2 = new QuadraticObjectiveFunction(() => z * x - x * y + y * z + 4 * z + 2);
            var actual = actual1 + actual2;

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    for (int k = 0; k < 10; k++)
                    {
                        x = (i - 5) / 10.0;
                        y = (j - 5) / 10.0;
                        z = (k - 5) / 10.0;

                        double a = actual.Function(new[] { x, y, z });
                        double e = expected1() + expected2();

                        Assert.AreEqual(e, a, 1e-10);
                        Assert.IsFalse(Double.IsNaN(a));
                        Assert.IsFalse(Double.IsNaN(e));
                    }
        }

        [Test]
        [TestCase("x² + 1", "x² + 1")]
        [TestCase("x² + 1", "2x²")]
        [TestCase("-x*y + y*z", "-x*y + y*z")]
        [TestCase("-x*y + y*z", "-2x² + xy - y² - 10xz + z²")]
        [TestCase("-x*y + y*z", "-2x² + xy - y² + 5z")]
        [TestCase("-2x² + xy - y² - 10xz + z²", "-2x² + xy - y² - 10xz + z²")]
        [TestCase("-2x² + xy - y² - 10xz + z²", "-2x² + xy - y² + 5z")]
        [TestCase("-2x² + xy - y² + 5y", "-2x² + xy - y² + 5y")]
        [TestCase("-2x² + xy - y² + 5y", "2x² -5y -5")]
        [TestCase("2x² -5", "2x² -5")]
        public void OperatorAdditionTest2(string string1, string string2)
        {
            double x = 0;
            double y = 0;
            double z = 0;

            var actual1 = new QuadraticObjectiveFunction(string1);
            var actual2 = new QuadraticObjectiveFunction(string2);
            var actual = actual1 + actual2;

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    for (int k = 0; k < 10; k++)
                    {
                        x = (i - 5) / 10.0;
                        y = (j - 5) / 10.0;
                        z = (k - 5) / 10.0;

                        var arg = new[] { x, y, z }.First(actual1.NumberOfVariables);

                        double a = actual.Function(arg);
                        double e = actual1.Function(arg) + actual2.Function(arg);

                        Assert.AreEqual(e, a, 1e-10);
                        Assert.IsFalse(double.IsNaN(a));
                        Assert.IsFalse(double.IsNaN(e));
                    }
        }

        [Test]
        public void OperatorCompositionDocumentation()
        {
            #region doc_example
            // The quadratic objective function supports the notion
            // of vector addition and scalar multiplication. That is,
            // QP programs can be linearly combined to create new QP
            // problems. This idea can be useful when composing
            // objective functions.
            var f1 = new QuadraticObjectiveFunction("2x² + 4y² - 2xy + 6");
            var f2 = new QuadraticObjectiveFunction("3x² - 4y² + 6xy + 3x + 2y");

            // Suppose we have the functions:
            //      f₁(x,y) = 2x² - 2xy + 4y² + 6
            //      f₂(x,y) = 3x² + 6xy - 4y² + 3x + 2y
            //
            // Then we can create a new function - f(x,y) - defined as
            // some linear combination of f₁ and f₂. e.g.
            //      f(x,y) = {f₁ + 2f₂}(x,y)
            // 
            // In code, we can write this:
            QuadraticObjectiveFunction f = f1 + (2 * f2); // 8x² -4y² +10xy +6x +4y +6

            // And now we can test our new objective function:
            double[] x = { 1, 2 };

            double result1 = f1.Function(x);
            double result2 = f2.Function(x);

            double result = f.Function(x);          // should be 32
            double check = result1 + 2 * result2;   // should be the same
            #endregion

            Assert.AreEqual(result, check);
        }

        [Test]
        [TestCase("x² + 1", "x² + 1", 3, 5)]
        [TestCase("x² + 1", "2x²", 1, 0)]
        [TestCase("-x*y + y*z", "-x*y + y*z", 2, 2)]
        [TestCase("-x*y + y*z", "-2x² + xy - y² - 10xz + z²", 0, 0)]
        [TestCase("-x*y + y*z", "-2x² + xy - y² + 5z", -1, -1)]
        [TestCase("-2x² + xy - y² - 10xz + z²", "-2x² + xy - y² - 10xz + z²", 4, 5)]
        [TestCase("-2x² + xy - y² - 10xz + z²", "-2x² + xy - y² + 5z", 7, 6)]
        [TestCase("-2x² + xy - y² + 5y", "-2x² + xy - y² + 5y", 3, 9)]
        [TestCase("-2x² + xy - y² + 5y", "2x² -5y -5", 4, -7)]
        [TestCase("2x² -5", "2x² -5", 2, -2)]
        public void OperatorCompositionTest(string string1, string string2, double weight1, double weight2)
        {
            double x = 0;
            double y = 0;
            double z = 0;

            var actual1 = new QuadraticObjectiveFunction(string1);
            var actual2 = new QuadraticObjectiveFunction(string2);
            var actual = (weight1 * actual1) + (weight2 * actual2);

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    for (int k = 0; k < 10; k++)
                    {
                        x = (i - 5) / 10.0;
                        y = (j - 5) / 10.0;
                        z = (k - 5) / 10.0;

                        var arg = new[] { x, y, z }.First(actual1.NumberOfVariables);

                        double a = actual.Function(arg);
                        double e = (weight1 * actual1.Function(arg)) + (weight2 * actual2.Function(arg));

                        Assert.AreEqual(e, a, 1e-10);
                        Assert.IsFalse(double.IsNaN(a));
                        Assert.IsFalse(double.IsNaN(e));
                    }
        }

        [Test]
        [TestCase("x² + 1", "x² + 1")]
        [TestCase("x² + 1", "2x²")]
        [TestCase("-x*y + y*z", "-x*y + y*z")]
        [TestCase("-x*y + y*z", "-2x² + xy - y² - 10xz + z²")]
        [TestCase("-x*y + y*z", "-2x² + xy - y² + 5z")]
        [TestCase("-2x² + xy - y² - 10xz + z²", "-2x² + xy - y² - 10xz + z²")]
        [TestCase("-2x² + xy - y² - 10xz + z²", "-2x² + xy - y² + 5z")]
        [TestCase("-2x² + xy - y² + 5y", "-2x² + xy - y² + 5y")]
        [TestCase("-2x² + xy - y² + 5y", "2x² -5y -5")]
        [TestCase("2x² -5", "2x² -5")]
        public void OperatorSubtractionTest2(string string1, string string2)
        {
            double x = 0;
            double y = 0;
            double z = 0;

            var actual1 = new QuadraticObjectiveFunction(string1);
            var actual2 = new QuadraticObjectiveFunction(string2);
            var actual = actual1 - actual2;

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    for (int k = 0; k < 10; k++)
                    {
                        x = (i - 5) / 10.0;
                        y = (j - 5) / 10.0;
                        z = (k - 5) / 10.0;

                        var arg = new[] { x, y, z }.First(actual1.NumberOfVariables);

                        double a = actual.Function(arg);
                        double e = actual1.Function(arg) - actual2.Function(arg);

                        Assert.AreEqual(e, a, 1e-10);
                        Assert.IsFalse(double.IsNaN(a));
                        Assert.IsFalse(double.IsNaN(e));
                    }
        }

        [Test]
        public void OperatorScalingTest()
        {
            double x = 0;
            double y = 0;
            double z = 0;
            double scalar = 5;

            Func<double> expected1 = () => 2 * y * x - x * y + y * z;
            var actual1 = new QuadraticObjectiveFunction(() => 2 * y * x - x * y + y * z);
            var actual = actual1 * scalar;

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    for (int k = 0; k < 10; k++)
                    {
                        x = (i - 5) / 10.0;
                        y = (j - 5) / 10.0;
                        z = (k - 5) / 10.0;

                        double a = actual.Function(new[] { x, y, z });
                        double e = scalar * expected1();

                        Assert.AreEqual(e, a, 1e-10);
                        Assert.IsFalse(double.IsNaN(a));
                        Assert.IsFalse(double.IsNaN(e));
                    }
        }

        [Test]
        [TestCase("x² + 1", 6)]
        [TestCase("-x*y + y*z", 7)]
        [TestCase("-2x² + xy - y² - 10xz + z²", 0)]
        [TestCase("-2x² + xy - y² + 5y", -3)]
        [TestCase("2x² -5", 1)]
        public void OperatorScalingTest2(string string1, double scalar)
        {
            double x = 0;
            double y = 0;
            double z = 0;

            var actual1 = new QuadraticObjectiveFunction(string1);
            var actual = actual1 * scalar;

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    for (int k = 0; k < 10; k++)
                    {
                        x = (i - 5) / 10.0;
                        y = (j - 5) / 10.0;
                        z = (k - 5) / 10.0;

                        var arg = new[] { x, y, z }.First(actual1.NumberOfVariables);

                        double a = actual.Function(arg);
                        double e = actual1.Function(arg) * scalar;

                        Assert.AreEqual(e, a, 1e-10);
                        Assert.IsFalse(double.IsNaN(a));
                        Assert.IsFalse(double.IsNaN(e));
                    }
        }

        [Test]
        public void OperatorNegateTest()
        {
            double x = 0;
            double y = 0;
            double z = 0;

            Func<double> expected1 = () => 2 * y * x - x * y + y * z;
            var actual1 = new QuadraticObjectiveFunction(() => 2 * y * x - x * y + y * z);
            var actual = -actual1;

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    for (int k = 0; k < 10; k++)
                    {
                        x = (i - 5) / 10.0;
                        y = (j - 5) / 10.0;
                        z = (k - 5) / 10.0;

                        double a = actual.Function(new[] { x, y, z });
                        double e = -expected1();

                        Assert.AreEqual(e, a, 1e-10);
                        Assert.IsFalse(double.IsNaN(a));
                        Assert.IsFalse(double.IsNaN(e));
                    }
        }

        [Test]
        public void OperatorDivisionTest()
        {
            double x = 0;
            double y = 0;
            double z = 0;

            Func<double> expected1 = () => 2 * y * x - x * y + y * z;
            var actual1 = new QuadraticObjectiveFunction(() => 2 * y * x - x * y + y * z);
            var actual = actual1 / 5;

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    for (int k = 0; k < 10; k++)
                    {
                        x = (i - 5) / 10.0;
                        y = (j - 5) / 10.0;
                        z = (k - 5) / 10.0;

                        double a = actual.Function(new[] { x, y, z });
                        double e = expected1() / 5;

                        Assert.AreEqual(e, a, 1e-10);
                        Assert.IsFalse(double.IsNaN(a));
                        Assert.IsFalse(double.IsNaN(e));
                    }

            Assert.Throws<DivideByZeroException>(() =>
            {
                var resultant = actual / 0;
                Assert.Null(resultant);
            });
        }

        [Test]
        public void NonSymmetricThrowsExceptionTest()
        {
            double[,] quadraticTerms =
            {
                {  8, 3, 100000 },
                {  3, 4, 2 },
                {  1, 2, 6 },
            };

            double[] linearTerms = { 0, 2, 0 };

            Assert.Throws<NonSymmetricMatrixException>(() =>
            {
                var target = new QuadraticObjectiveFunction(quadraticTerms, linearTerms);
                Assert.NotNull(target);
            });
        }

        [Test]
        public void NonSymmetricSelfCorrectsTest()
        {
            double x = 0, y = 0;
            var f1 = new QuadraticObjectiveFunction(() => -2 * x * x + x * y - y * y - 3 * y * x);
            var f2 = new QuadraticObjectiveFunction("-2x² + xy - y² - 3yx");

            var correct = new QuadraticObjectiveFunction("-2x² + xy - y² - 3yx");

            double[] arg = { 3, 7 };

            double result1 = f1.Function(arg);
            double result2 = f2.Function(arg);
            double expected = correct.Function(arg);

            Assert.AreEqual(expected, result1);
            Assert.AreEqual(expected, result2);
        }
    }
}
