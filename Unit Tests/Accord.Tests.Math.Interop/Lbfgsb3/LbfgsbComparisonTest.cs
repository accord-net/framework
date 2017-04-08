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
    using AccordTestsMathCpp2;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;

    [TestFixture]
    public class LbfgsbComparisonTest
    {
        private List<Specification> problems;

        public LbfgsbComparisonTest()
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
                LbfgsbComparer cmp = new LbfgsbComparer();

                Info[] expected = cmp.Expected(problem);
                OptimizationProgressEventArgs[] actual = cmp.Actual(problem);

                check(actual, expected);
            }
        }

        [Test]
        public void ParameterTest1()
        {
            LbfgsbComparer cmp = new LbfgsbComparer()
            {
                factr = 1.0e+7,
                l = null,
                u = null,
                pgtol = 1e-5,
                m = 10,
            };

            compute(problems, cmp);
        }


        [Test]
        public void ParameterRangeTest()
        {
            LbfgsbComparer[] tests =
            {
                new LbfgsbComparer() { m = -1 },
                new LbfgsbComparer() { factr = -1 },
                new LbfgsbComparer() { max_iterations = -1 },
                new LbfgsbComparer() { pgtol = -1 },
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


        private static void compute(List<Specification> problems, LbfgsbComparer cmp)
        {
            foreach (var problem in problems)
            {
                string actualStr = String.Empty;
                string expectedStr = String.Empty;

                cmp.l = null;
                cmp.u = null;

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


                check(actual, expected);
            }
        }

        private static void check(OptimizationProgressEventArgs[] actual, Info[] expected)
        {
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                var a = (BoundedBroydenFletcherGoldfarbShannoInnerStatus)actual[i].Tag;
                var e = expected[i];

                Assert.AreEqual(e.Iteration, actual[i].Iteration);
                Assert.AreEqual(e.Value, actual[i].Value);

                for (int j = 0; j < a.Work.Length; j++)
                    Assert.AreEqual(e.Work[j], a.Work[j]);

                for (int j = 0; j < actual[i].Gradient.Length; j++)
                    Assert.AreEqual(e.Gradient[j], actual[i].Gradient[j]);

                for (int j = 0; j < a.Integers.Length; j++)
                    Assert.AreEqual(e.isave[j], a.Integers[j]);

                for (int j = 0; j < a.Doubles.Length; j++)
                {
                    Assert.AreEqual(Double.IsNaN(e.dsave[j]), Double.IsNaN(a.Doubles[j]));
                    if (!Double.IsNaN(e.dsave[j]))
                        Assert.AreEqual(e.dsave[j], a.Doubles[j], 1e-200);
                }

                for (int j = 0; j < a.Booleans.Length; j++)
                    Assert.AreEqual(e.lsave[j], a.Booleans[j] ? 1 : 0);

                String trim = e.csave.Trim();
                Assert.AreEqual(trim, a.Strings);
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
            LbfgsbComparer cmp = new LbfgsbComparer();

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

            var lbfgs = new BoundedBroydenFletcherGoldfarbShanno(2, function, gradient);

            lbfgs.Minimize(start);

            Assert.AreEqual(BoundedBroydenFletcherGoldfarbShannoStatus.LineSearchFailed, lbfgs.Status);
        }

    }
}
