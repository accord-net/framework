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
    using NUnit.Framework;

    [TestFixture]
    public class FiniteDifferencesTest
    {

        [Test]
        public void ComputeTest()
        {
            int numberOfParameters = 2;
            FiniteDifferences target = new FiniteDifferences(numberOfParameters);

            double[] inputs = { -1, 0.4 };

            target.Function = BroydenFletcherGoldfarbShannoTest.rosenbrockFunction;

            double[] expected = BroydenFletcherGoldfarbShannoTest.rosenbrockGradient(inputs);
            double[] actual = target.Compute(inputs);

            Assert.IsTrue(expected.IsEqual(actual, 0.05));
        }

        [Test]
        public void test_order()
        {
            // https://www.wolframalpha.com/input/?i=third+derivative+of+(1+-+x)%5E2+%2B+100(y+-+x%5E2)%5E2+at+(-1,0.4)

            int numberOfParameters = 2;
            FiniteDifferences target = new FiniteDifferences(numberOfParameters)
            {
                NumberOfPoints = 7,
                Order = 3,
            };

            double[] inputs = { -1, 0.4 };

            target.Function = BroydenFletcherGoldfarbShannoTest.rosenbrockFunction;

            double[] expected = { -2400, 0 };
            double[] actual = target.Compute(inputs);

            Assert.IsTrue(expected.IsEqual(actual, 1e-5));
        }

        [Test]
        public void ComputeTest2()
        {
            #region doc_gradient
            // Create a simple function with two parameters: f(x,y) = x² + y
            Func<double[], double> function = x => Math.Pow(x[0], 2) + x[1];

            // The gradient w.r.t to x should be 2x,
            // the gradient w.r.t to y should be  1


            // Create a new finite differences calculator
            var calculator = new FiniteDifferences(2, function);

            // Evaluate the gradient function at the point (2, -1)
            double[] result = calculator.Compute(2, -1); // answer is (4, 1)
            #endregion

            Assert.AreEqual(4, result[0], 1e-10);
            Assert.AreEqual(1, result[1], 1e-10);
        }

        [Test]
        public void Hessian_test()
        {
            #region doc_hessian
            // Create a simple function with two parameters: f(x,y) = x² + y
            Func<double[], double> function = x => Math.Pow(x[0], 2) + x[1];

            // The gradient w.r.t to x should be 2x,
            // the gradient w.r.t to y should be  1

            // Create a new finite differences calculator
            var calculator = new FiniteDifferences(2, function);

            // Evaluate the gradient function at the point (2, -1)
            double[][] result = calculator.Hessian(new[] { 2.0, -1.0 }); // answer is [(2, 0), (0, 0)]
            #endregion

            double[][] expected =
            {
                new double[] { 2, 0 },
                new double[] { 0, 0 },
            };

            Assert.IsTrue(result.IsEqual(expected, 1e-8));
        }

        [Test]
        public void Hessian_test_2()
        {
            Func<double[], double> function = x => Math.Pow(x[0], 2) + x[1] + x[0] * x[1] + 47;

            var calculator = new FiniteDifferences(2, function);

            double[][] result = calculator.Hessian(new[] { 2.0, -1.0 });
            double[][] expected =
            {
                new double[] { 2, 1 },
                new double[] { 1, 0 },
            };

            Assert.IsTrue(result.IsEqual(expected, 1e-8));
        }

        [Test]
        public void Hessian_test_3()
        {
            // x² + log(y) + xy + exp(x+y) + 47
            Func<double[], double> function = x => Math.Pow(x[0], 2) + Math.Log(x[1]) + x[0] * x[1] + Math.Exp(x[0] + x[1]) + 47;

            var calculator = new FiniteDifferences(variables: 2)
            {
                Function = function,
                NumberOfPoints = 7
            };

            Func<double[], double[][]> expectedFormula = (double[] x) =>
                new double[][]
                {
                    new double[] { Math.Exp(x[0] + x[1]) + 2, Math.Exp(x[0] + x[1]) + 1 },
                    new double[] { Math.Exp(x[0] + x[1]) + 1, Math.Exp(x[0] + x[1]) - 1.0 / Math.Pow(x[1], 2) },
                };


            for (double i = 1; i < 10; i++)
            {
                for (double j = 1; j < 10; j++)
                {
                    double[] value = new double[] { i, j };
                    double[][] actual = calculator.Hessian(value);
                    double[][] expected = expectedFormula(value);

                    Assert.IsTrue(actual.IsEqual(expected, rtol: 1e-5));
                }
            }
        }
    }
}
