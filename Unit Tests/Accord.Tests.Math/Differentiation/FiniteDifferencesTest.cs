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
    using Accord.Math.Optimization;

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

        [Test]
        public void Hessian_test_4()
        {
            const int Size = 30;
            const double Tolerance = 1e-8;

            for (int i = 0; i < 10; i++)
            {
                double[,] mat = Matrix.Random(Size);
                double[,] Q = mat.DotWithTransposed(mat);
                double[] d = Vector.Random(Size);

                var qof = new QuadraticObjectiveFunction(Q, d);

                var calculator = new FiniteDifferences(Size, qof.Function);

                double[][] result = calculator.Hessian(Vector.Random(Size));

                Assert.IsTrue(result.IsEqual(Q, Tolerance));
            }
        }


        [Test]
        public void gh_853()
        {
            double[] aParam = { 1.790978, 9.408872E-05, 0.9888748, 1E-08 };

            Func<double[], double> funcionObjetivoLBFGS = x =>
            {
                int iSizeArray = 10;
                double[] arrayDecimalesTrabajo = { 2.2227, 2.2188, 2.2144, 2.204, 2.2006, 2.2053, 2.2053, 2.2035, 2.1969, 2.2033 };

                double mu = x[0];
                double omega = x[1];
                double alpha = x[2];
                double beta = x[3];

                double[] e2 = new double[iSizeArray];
                double mediae2 = 0.0;
                for (int i = 0; i <= iSizeArray - 1; i++)
                {
                    e2[i] = Math.Pow((arrayDecimalesTrabajo[i] - mu), 2);
                    mediae2 = mediae2 + e2[i];
                }

                mediae2 = mediae2 / iSizeArray;
                double[] sigmat2 = new double[iSizeArray];
                sigmat2[0] = omega + alpha * mediae2;
                for (int i = 1; i <= iSizeArray - 1; i++)
                {
                    sigmat2[i] = omega + alpha * e2[i - 1];
                }
                sigmat2[0] = sigmat2[0] + beta * mediae2;
                for (int i = 1; i <= iSizeArray - 1; i++)
                {
                    sigmat2[i] = sigmat2[i] + beta * sigmat2[i - 1];
                }

                double sumalikelihood = iSizeArray * Math.Log(2 * Math.PI);
                for (int i = 0; i <= iSizeArray - 1; i++)
                {
                    double dMathlog = Math.Log(sigmat2[i]);
                    if (double.IsInfinity(dMathlog))
                        break;
                    if (double.IsNaN(dMathlog))
                        break;
                    sumalikelihood = sumalikelihood + dMathlog + e2[i] / sigmat2[i];
                }
                return 0.5 * sumalikelihood;

            };

            var calculator = new FiniteDifferences(variables: 4, function: funcionObjetivoLBFGS)
            {
                NumberOfPoints = 7,
            };

            double[] result = calculator.Compute(aParam);
            double[][] actual = calculator.Hessian(aParam);

            Assert.AreEqual(actual[0][0], -57.6160442364672, 1e-2);
            Assert.AreEqual(actual[0][1], -1.27840745168738, 1e-2);
            Assert.AreEqual(actual[0][2], 0.0255249049748045, 1e-2);
            Assert.AreEqual(actual[0][3], 0.0231494572415752, 1e-2);
            Assert.AreEqual(actual[1][0], -1.27840745168738, 1e-2);
            Assert.AreEqual(actual[1][1], 173.045435758468, 1e-1);
            Assert.AreEqual(actual[1][2], 29.9809421733244, 1e-2);
            Assert.AreEqual(actual[1][3], 29.8568458845239, 1e-1);
            Assert.AreEqual(actual[2][0], 0.0255249049748045, 1e-2);
            Assert.AreEqual(actual[2][1], 29.9809421733244, 1e-2);
            Assert.AreEqual(actual[2][2], 5.2012616436059, 1e-2);
            Assert.AreEqual(actual[2][3], 5.17856868498256, 1e-2);
            Assert.AreEqual(actual[3][0], 0.0231494572415752, 1e-2);
            Assert.AreEqual(actual[3][1], 29.8568458845239, 1e-2);
            Assert.AreEqual(actual[3][2], 5.17856868498256, 1e-2);
            Assert.AreEqual(actual[3][3], 5.15878895157584, 1e-2);
        }

    }
}
