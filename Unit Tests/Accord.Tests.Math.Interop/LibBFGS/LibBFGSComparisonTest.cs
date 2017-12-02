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

namespace Accord.Tests.Interop.Math
{
    using Accord.Math.Optimization;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;

    [TestFixture]
    public class LibBFGSComparisonTest
    {
        private List<Specification> problems;

        public LibBFGSComparisonTest()
        {
            var list = new List<Specification>();

            list.Add(new Specification(2,
                (x) =>
                {
                    double a = x[1] - x[0] * x[0];
                    double b = 1 - x[0];
                    return b * b + 100 * a * a;
                },
                (x) =>
                {
                    double a = x[1] - x[0] * x[0];
                    double b = 1 - x[0];

                    double f0 = -2 * b - 400 * x[0] * a;
                    double f1 = 200 * a;

                    return new[] { f0, f1 };
                }, new[] { 0.0, 0 }));

            list.Add(new Specification(2,
                (x) => -Math.Exp(-Math.Pow(x[0] - 1, 2)) - Math.Exp(-0.5 * Math.Pow(x[1] - 2, 2)),
                (x) => new double[] 
                {
                    // df/dx = {-2 e^(-    (x-1)^2) (x-1)}
                    2 * Math.Exp(-Math.Pow(x[0] - 1, 2)) * (x[0] - 1),

                    // df/dy = {-  e^(-1/2 (y-2)^2) (y-2)}
                    Math.Exp(-0.5 * Math.Pow(x[1] - 2, 2)) * (x[1] - 2)
                }, null));

            list.Add(new Specification(2,
                x => 10.0 * Math.Pow(x[0] + 1.0, 2.0) + Math.Pow(x[1], 2.0),
                x => new[] { 20 * (x[0] + 1), 2 * x[1] },
                new double[2]));

            list.Add(new Specification(3, // uses a constant value
                x => 10.0 * Math.Pow(x[0] + 1.0, 2.0) + Math.Pow(x[1], 2.0),
                x => new[] { 20 * (x[0] + 1), 2 * x[1], 0 },
                new double[3]));

            problems = list;
        }

        [Test]
        public void DefaultBatchTest()
        {
            foreach (var problem in problems)
            {
                LBFGSComparer cmp = new LBFGSComparer();

                var expected = cmp.Expected(problem);
                var actual = cmp.Actual(problem);

                Assert.AreEqual(expected.Length, actual.Length);
                for (int i = 0; i < expected.Length; i++)
                {
                    var a = actual[i];
                    var e = expected[i];

                    Assert.AreEqual(e.fx, a.Value);

                    for (int j = 0; j < e.g.Length; j++)
                        Assert.AreEqual(e.g[j], a.Gradient[j]);
                    Assert.AreEqual(e.gnorm, a.GradientNorm);
                    Assert.AreEqual(e.step, a.Step);
                }
            }
        }

        [Test]
        public void ParameterTest1()
        {
            LBFGSComparer cmp = new LBFGSComparer()
            {
                m = 2,
                epsilon = 0.000000099999999999999995,
                past = 0,
                delta = 0.00000001,
                max_iterations = 0,
                linesearch = (LineSearch)0,
                ftol = 0.00000001,
                wolfe = 0,
                gtol = 0,
                xtol = 0.000000000000000099999999999999998,
                orthantwise_c = 0,
                orthantwise_start = 0,
                orthantwise_end = -1
            };

            compute(problems, cmp);
        }

        [Test]
        [Category("Slow")]
        public void ParameterBatchTest()
        {
            for (var past = 0; past < 10; past += 3)
                for (var delta = 1e-8; delta < 1; delta *= 100)
                    for (int max_iterations = 0, i = 1; max_iterations < 50; max_iterations += i * i, i++)
                        inner(problems, 4, 1e-5, past, delta, max_iterations, (int)LineSearch.Default);

        }

        [Test]
        [Category("Slow")]
        public void ParameterBatchTest2()
        {
            for (var m = 2; m < 10; m++)
                for (var epsilon = 1e-7; epsilon < 1; epsilon *= 1000)
                    for (var past = 0; past < 10; past += 5)
                        for (var linesearch = (int)LineSearch.Default; linesearch < 4; linesearch++)
                            inner(problems, m, epsilon, past, 1e-5, 50, linesearch);

        }

        private static void inner(List<Specification> problems, int m,
            double epsilon, int past, double delta, int max_iterations, int linesearch)
        {
            bool executed = false;

            for (var ftol = 1e-8; ftol < 1e-5; ftol *= 1000)
                for (var wolfe = 0.0; wolfe <= 1.0; wolfe += 0.4)
                    for (var gtol = 0.0; gtol <= 1.0; gtol += 0.5)
                        for (var xtol = 1.0e-16; xtol < 1e-6; xtol *= 10000)
                            for (var orthantwise_c = 0.0; orthantwise_c < 2; orthantwise_c += 0.4)
                                for (var orthantwise_start = 0; orthantwise_start < 2; orthantwise_start++)
                                    for (var orthantwise_end = -1; orthantwise_end < 2; orthantwise_end++)
                                    {
                                        executed = true;

                                        LBFGSComparer cmp = new LBFGSComparer()
                                        {
                                            m = m,
                                            epsilon = epsilon,
                                            past = past,
                                            delta = delta,
                                            max_iterations = max_iterations,
                                            linesearch = (LineSearch)linesearch,
                                            //max_linesearch = max_linesearch,
                                            // min_step = min_step,
                                            // max_step = max_step,
                                            ftol = ftol,
                                            wolfe = wolfe,
                                            gtol = gtol,
                                            xtol = xtol,
                                            orthantwise_c = orthantwise_c,
                                            orthantwise_start = orthantwise_start,
                                            orthantwise_end = orthantwise_end
                                        };

                                        compute(problems, cmp);
                                    }

            Assert.IsTrue(executed);
        }


        [Test]
        public void ParameterRangeTest()
        {
            LBFGSComparer[] tests =
            {
                new LBFGSComparer() { m = -1 },
                new LBFGSComparer() { epsilon = -1 },
                new LBFGSComparer() { past = -1 },
                new LBFGSComparer() { delta = -1},
                new LBFGSComparer() { max_iterations = -1 },
                new LBFGSComparer() { linesearch = (LineSearch)5 },
                new LBFGSComparer() { max_linesearch = -1 },
                new LBFGSComparer() { min_step = -1},
                new LBFGSComparer() { max_step = -1 },
                new LBFGSComparer() { ftol = -1 },
                new LBFGSComparer() { wolfe = -1 },
                new LBFGSComparer() { gtol = -1 },
                new LBFGSComparer() { xtol = -1 },
                new LBFGSComparer() { orthantwise_c = -1 },
                new LBFGSComparer() { orthantwise_start = -1 },
                new LBFGSComparer() { orthantwise_end = -1 },

                new LBFGSComparer()
                { 
                    linesearch = LineSearch.RegularWolfe,
                    wolfe = -1
                },

                new LBFGSComparer()
                { 
                    linesearch = LineSearch.StrongWolfe,
                    wolfe = -1
                },

                new LBFGSComparer()
                { 
                    orthantwise_end = 2,
                    orthantwise_start = 3,
                },
            };

            var problem = problems[0];

            for (int i = 0; i < tests.Length; i++)
            {
                var test = tests[i];

                string actual = String.Empty;
                string expected = String.Empty;
                try { test.Actual(problem); }
                catch (Exception ex)
                {
                    actual = ex.Data["Code"] as string;
                    if (actual == null)
                        throw;
                }

                test.Expected(problem);
                expected = test.NativeCode;

                if (actual == String.Empty)
                    actual = test.ActualMessage;

                Assert.AreEqual(expected, actual);
                Assert.AreNotEqual(String.Empty, actual);
            }
        }


        private static void compute(List<Specification> problems, LBFGSComparer cmp)
        {
            foreach (var problem in problems)
            {
                string actualStr = String.Empty;
                string expectedStr = String.Empty;

                OptimizationProgressEventArgs[] actual = null;

                try { actual = cmp.Actual(problem); }
                catch (Exception ex)
                {
                    actualStr = ex.Data["Code"] as string;
                    if (actualStr == null)
                        throw;
                }

                var expected = cmp.Expected(problem);
                expectedStr = cmp.NativeCode;

                if (actualStr == String.Empty)
                    actualStr = cmp.ActualMessage;

                Assert.AreEqual(expectedStr, actualStr);

                if (expectedStr != "LBFGS_SUCCESS")
                    continue;

                Assert.AreEqual(expected.Length, actual.Length);
                for (int i = 0; i < expected.Length; i++)
                {
                    var a = actual[i];
                    var e = expected[i];

                    Assert.AreEqual(e.fx, a.Value);

                    for (int j = 0; j < e.g.Length; j++)
                        Assert.AreEqual(e.g[j], a.Gradient[j]);
                    Assert.AreEqual(e.gnorm, a.GradientNorm);
                    Assert.AreEqual(e.step, a.Step);
                }
            }
        }

        [Test]
        public void InvalidLineSearchTest()
        {
            double t = 0;
            double s = 0;
            int n = 10;

            Func<double[], double> function = (parameters) =>
            {
                t = parameters[0];
                s = parameters[1];

                return -(n * Math.Log(s) - n * Math.Log(Math.PI));
            };

            Func<double[], double[]> gradient = (parameters) =>
            {
                t = parameters[0];
                s = parameters[1];

                double dt = -2.0;
                double ds = +2.0 - n / s;

                return new[] { dt, ds };
            };

            double[] start = { 0, 0 };


            Specification problem = new Specification(2, function, gradient, start);
            LBFGSComparer cmp = new LBFGSComparer();

            compute(new List<Specification>() { problem }, cmp);
        }

        [Test]
        public void InvalidLineSearchTest2()
        {
            int n = 10;

            Func<double[], double> function = (parameters) =>
            {
                return -(n * Math.Log(0) - n * Math.Log(Math.PI));
            };

            Func<double[], double[]> gradient = (parameters) =>
            {
                return new[] { 2.0, Double.NegativeInfinity };
            };

            double[] start = { 0, 0 };

            var lbfgs = new BroydenFletcherGoldfarbShanno(2, function, gradient);

            bool success = lbfgs.Minimize(start); 

            Assert.IsFalse(success);
        }

    }
}
