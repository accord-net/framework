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
    using Accord.Math.Optimization;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class BoundedBroydenFletcherGoldfarbShannoTest
    {

        [Test]
        public void lbfgsTest()
        {
            Func<double[], double> f = rosenbrockFunction;
            Func<double[], double[]> g = rosenbrockGradient;

            Assert.AreEqual(104, f(new[] { -1.0, 2.0 }));


            int n = 2; // number of variables
            double[] initial = { -1.2, 1 };

            var lbfgs = new BoundedBroydenFletcherGoldfarbShanno(n, f, g);
            lbfgs.GradientTolerance = 1e-10;
            lbfgs.FunctionTolerance = 1e-10;

            double expected = 0;
            Assert.IsTrue(lbfgs.Minimize(initial));
            double actual = lbfgs.Value;


            Assert.AreEqual(expected, actual, 1e-10);

            double[] result = lbfgs.Solution;

            //Assert.AreEqual(49, lbfgs.Evaluations);
            //Assert.AreEqual(40, lbfgs.Iterations);
            Assert.AreEqual(1.0, result[0], 1e-6);
            Assert.AreEqual(1.0, result[1], 1e-6);

            double y = f(result);
            double[] d = g(result);

            Assert.AreEqual(0, y, 1e-10);
            Assert.AreEqual(0, d[0], 1e-6);
            Assert.AreEqual(0, d[1], 1e-6);
        }

        [Test]
        public void lbfgsTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            // Suppose we would like to find the minimum of the function
            // 
            //   f(x,y)  =  -exp{-(x-1)²} - exp{-(y-2)²/2}
            //

            // First we need write down the function either as a named
            // method, an anonymous method or as a lambda function:

            Func<double[], double> f = (x) =>
                -Math.Exp(-Math.Pow(x[0] - 1, 2)) - Math.Exp(-0.5 * Math.Pow(x[1] - 2, 2));

            // Now, we need to write its gradient, which is just the
            // vector of first partial derivatives del_f / del_x, as:
            //
            //   g(x,y)  =  { del f / del x, del f / del y }
            // 

            Func<double[], double[]> g = (x) => new double[] 
            {
                // df/dx = {-2 e^(-    (x-1)^2) (x-1)}
                2 * Math.Exp(-Math.Pow(x[0] - 1, 2)) * (x[0] - 1),

                // df/dy = {-  e^(-1/2 (y-2)^2) (y-2)}
                Math.Exp(-0.5 * Math.Pow(x[1] - 2, 2)) * (x[1] - 2)
            };

            // Finally, we can create the L-BFGS solver, passing the functions as arguments
            var lbfgs = new BoundedBroydenFletcherGoldfarbShanno(numberOfVariables: 2, function: f, gradient: g);

            // And then minimize the function:
            Assert.IsTrue(lbfgs.Minimize());
            double minValue = lbfgs.Value;
            double[] solution = lbfgs.Solution;

            // The resultant minimum value should be -2, and the solution
            // vector should be { 1.0, 2.0 }. The answer can be checked on
            // Wolfram Alpha by clicking the following the link:

            // http://www.wolframalpha.com/input/?i=maximize+%28exp%28-%28x-1%29%C2%B2%29+%2B+exp%28-%28y-2%29%C2%B2%2F2%29%29

            double expected = -2;
            Assert.AreEqual(expected, minValue, 1e-10);

            Assert.AreEqual(1, solution[0], 1e-3);
            Assert.AreEqual(2, solution[1], 1e-3);

        }

        // The famous Rosenbrock test function.
        public static double rosenbrockFunction(double[] x)
        {
            double a = x[1] - x[0] * x[0];
            double b = 1 - x[0];
            return b * b + 100 * a * a;
        }

        // Gradient of the Rosenbrock test function.
        public static double[] rosenbrockGradient(double[] x)
        {
            double a = x[1] - x[0] * x[0];
            double b = 1 - x[0];

            double f0 = -2 * b - 400 * x[0] * a;
            double f1 = 200 * a;

            return new[] { f0, f1 };
        }

        private static void createTestFunction(out Func<double[], double> f, out Func<double[], double[]> g)
        {
            // min f(x, y) = -exp(-(x-1)^2) - exp(-0.5*(y-2)^2)
            f = (x) => -Math.Exp(-Math.Pow(x[0] - 1, 2)) - Math.Exp(-0.5 * Math.Pow(x[1] - 2, 2));

            g = (x) => new[] 
            {
                2 * Math.Exp(-Math.Pow(x[0] - 1, 2)) * (x[0] - 1),
                Math.Exp(-0.5 * Math.Pow(x[1] - 2, 2)) * (x[1] - 2)
            };
        }

        [Test]
        public void NoFunctionTest()
        {
            var target = new BoundedBroydenFletcherGoldfarbShanno(2);

            Assert.Throws<InvalidOperationException>(() => target.Minimize(), "");
        }

        [Test]
        public void NoGradientTest()
        {
            var target = new BoundedBroydenFletcherGoldfarbShanno(2)
            {
                Function = (x) => 0.0
            };

            Assert.IsTrue(target.Minimize());

            // The optimizer should use finite differences as the gradient
        }

        [Test]
        public void WrongGradientSizeTest()
        {
            var target = new BoundedBroydenFletcherGoldfarbShanno(2)
            {
                Function = (x) => 0.0,
                Gradient = (x) => new double[1]
            };

            Assert.Throws<InvalidOperationException>(() => target.Minimize(), "");
        }

        [Test]
        public void MutableGradientSizeTest()
        {
            var target = new BoundedBroydenFletcherGoldfarbShanno(2)
            {
                Function = (x) => 0.0,
                Gradient = (x) => x
            };

            Assert.Throws<InvalidOperationException>(() => target.Minimize(), "");
        }

        [Test]
        public void ConstructorTest1()
        {
            Func<double[], double> function = // min f(x) = 10 * (x+1)^2 + y^2
                x => 10.0 * Math.Pow(x[0] + 1.0, 2.0) + Math.Pow(x[1], 2.0);

            Func<double[], double[]> gradient = x => new[] { 20 * (x[0] + 1), 2 * x[1] };

            var target = new BoundedBroydenFletcherGoldfarbShanno(2)
            {
                Function = function,
                Gradient = gradient
            };

            bool success = target.Minimize();
            double minimum = target.Value;
            double[] solution = target.Solution;

            Assert.IsTrue(success);

            Assert.AreEqual(0, minimum, 1e-10);
            Assert.AreEqual(-1, solution[0], 1e-5);
            Assert.AreEqual(0, solution[1], 1e-5);

            double expectedMinimum = function(target.Solution);
            Assert.AreEqual(expectedMinimum, minimum);
        }

     


        [Test]
        public void lbfgsTest3()
        {
            Accord.Math.Tools.SetupGenerator(0);

            Func<double[], double> f;
            Func<double[], double[]> g;
            createExpDiff(out f, out g);

            int errors = 0;

            for (int i = 0; i < 10000; i++)
            {
                double[] start = Accord.Math.Matrix.Random(2, -1.0, 1.0);

                var lbfgs = new BoundedBroydenFletcherGoldfarbShanno(numberOfVariables: 2,
                    function: f, gradient: g);

                lbfgs.FunctionTolerance = 1e3;

                Assert.IsTrue(lbfgs.Minimize(start));
                double minValue = lbfgs.Value;
                double[] solution = lbfgs.Solution;

                double expected = -2;

                if (Math.Abs(expected - minValue) > 1e-2)
                    errors++;
            }

            Assert.IsTrue(errors < 1000);
        }

        private static void createExpDiff(out Func<double[], double> f, out Func<double[], double[]> g)
        {
            f = (x) =>
                           -Math.Exp(-Math.Pow(x[0] - 1, 2)) - Math.Exp(-0.5 * Math.Pow(x[1] - 2, 2));

            g = (x) => new double[] 
            {
                // df/dx = {-2 e^(-    (x-1)^2) (x-1)}
                2 * Math.Exp(-Math.Pow(x[0] - 1, 2)) * (x[0] - 1),

                // df/dy = {-  e^(-1/2 (y-2)^2) (y-2)}
                Math.Exp(-0.5 * Math.Pow(x[1] - 2, 2)) * (x[1] - 2)
            };
        }

        

    }
}
