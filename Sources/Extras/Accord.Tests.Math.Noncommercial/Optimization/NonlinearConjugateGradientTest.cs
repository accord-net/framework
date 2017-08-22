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

namespace Accord.Tests.Extras.Math.Noncommercial
{
    using Accord.Math.Optimization;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class NonlinearConjugateGradientTest
    {

        [Test]
        public void MinimizeTest()
        {
            Func<double[], double> f = rosenbrockFunction;
            Func<double[], double[]> g = rosenbrockGradient;

            Assert.AreEqual(104, f(new[] { -1.0, 2.0 }));


            int n = 2; // number of variables
            double[] initial = { -1.2, 1 };

            var cg = new NonlinearConjugateGradient(n, f, g);

            Assert.IsTrue(cg.Minimize(initial));
            double actual = cg.Value;
            double expected = 0;
            Assert.AreEqual(expected, actual, 1e-6);

            double[] result = cg.Solution;

            Assert.AreEqual(180, cg.Evaluations);
            Assert.AreEqual(67, cg.Iterations);
            Assert.AreEqual(1.0, result[0], 1e-3);
            Assert.AreEqual(1.0, result[1], 1e-3);
            Assert.IsFalse(double.IsNaN(result[0]));
            Assert.IsFalse(double.IsNaN(result[1]));

            double y = f(result);
            double[] d = g(result);

            Assert.AreEqual(0.0, y, 1e-6);
            Assert.AreEqual(0.0, d[0], 1e-3);
            Assert.AreEqual(0.0, d[1], 1e-3);

            Assert.IsFalse(double.IsNaN(y));
            Assert.IsFalse(double.IsNaN(d[0]));
            Assert.IsFalse(double.IsNaN(d[1]));
        }

        [Test]
        public void expDiffTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            Func<double[], double> f;
            Func<double[], double[]> g;
            createExpDiff(out f, out g);

            int errors = 0;

            for (int i = 0; i < 1000; i++)
            {
                double[] start = Accord.Math.Matrix.Random(2, -1.0, 1.0);

                var lbfgs = new NonlinearConjugateGradient(numberOfVariables: 2, function: f, gradient: g);

                lbfgs.Minimize(start);
                double minValue = lbfgs.Value;
                double[] solution = lbfgs.Solution;

                double expected = -2;

                if (Math.Abs(expected - minValue) > 1e-3)
                    errors++;
            }

            Assert.IsTrue(errors <= 0);
        }

        private static void createExpDiff(out Func<double[], double> f, out Func<double[], double[]> g)
        {
            f = (x) => -Math.Exp(-Math.Pow(x[0] - 1, 2)) - Math.Exp(-0.5 * Math.Pow(x[1] - 2, 2));

            g = (x) => new double[] 
            {
                // df/dx = {-2 e^(-    (x-1)^2) (x-1)}
                2 * Math.Exp(-Math.Pow(x[0] - 1, 2)) * (x[0] - 1),

                // df/dy = {-  e^(-1/2 (y-2)^2) (y-2)}
                Math.Exp(-0.5 * Math.Pow(x[1] - 2, 2)) * (x[1] - 2)
            };
        }

        [Test]
        public void ConstructorTest2()
        {
            Func<double[], double> function = // min f(x) = 10 * (x+1)^2 + y^2
                x => 10.0 * Math.Pow(x[0] + 1.0, 2.0) + Math.Pow(x[1], 2.0);

            Func<double[], double[]> gradient = x => new[] { 20 * (x[0] + 1), 2 * x[1] };

            NonlinearConjugateGradient target = new NonlinearConjugateGradient(2)
            {
                Function = function,
                Gradient = gradient
            };

            Assert.IsTrue(target.Minimize());
            double minimum = target.Value;

            double[] solution = target.Solution;

            Assert.AreEqual(0, minimum, 1e-10);
            Assert.AreEqual(-1, solution[0], 1e-5);
            Assert.AreEqual(0, solution[1], 1e-5);

            double expectedMinimum = function(target.Solution);
            Assert.AreEqual(expectedMinimum, minimum);
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
    }
}
