// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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
    using System.Collections.Generic;
    using Accord.Math;
    using Accord.Math.Optimization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;


    [TestClass()]
    public class AugmentedLagrangianSolverTest
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




        [TestMethod()]
        public void AugmentedLagrangianSolverConstructorTest1()
        {
            Accord.Math.Tools.SetupGenerator(0);

            // min 100(y-x*x)²+(1-x)²
            //
            // s.t.  x <= 0
            //       y <= 0
            //

            var f = new NonlinearObjectiveFunction(2,

                function: (x) => 100 * Math.Pow(x[1] - x[0] * x[0], 2) + Math.Pow(1 - x[0], 2),

                gradient: (x) => new[] 
                {
                    2.0 * (200.0 * x[0]*x[0]*x[0] - 200.0 * x[0] * x[1] + x[0] - 1), // df/dx
                    200 * (x[1] - x[0]*x[0])                                         // df/dy
                }

            );


            var constraints = new List<NonlinearConstraint>();

            constraints.Add(new NonlinearConstraint(f,

                function: (x) => x[0],
                gradient: (x) => new[] { 1.0, 0.0 },

                shouldBe: ConstraintType.LesserThanOrEqualTo, value: 0
            ));

            constraints.Add(new NonlinearConstraint(f,

                function: (x) => x[1],
                gradient: (x) => new[] { 0.0, 1.0 },

                shouldBe: ConstraintType.LesserThanOrEqualTo, value: 0
            ));

            var solver = new AugmentedLagrangian(f, constraints);

            Assert.IsTrue(solver.Minimize());
            double minValue = solver.Value;

            Assert.IsFalse(Double.IsNaN(minValue));
            Assert.AreEqual(1, minValue, 1e-5);
            Assert.AreEqual(0, solver.Solution[0], 1e-5);
            Assert.AreEqual(0, solver.Solution[1], 1e-5);
        }


        [TestMethod()]
        public void AugmentedLagrangianSolverConstructorTest2()
        {
            // min 100(y-x*x)²+(1-x)²
            //
            // s.t.  x >= 0
            //       y >= 0
            //

            var f = new NonlinearObjectiveFunction(2,

                function: (x) => 100 * Math.Pow(x[1] - x[0] * x[0], 2) + Math.Pow(1 - x[0], 2),

                gradient: (x) => new[] 
                {
                    2.0 * (200.0 * Math.Pow(x[0], 3) - 200.0 * x[0] * x[1] + x[0] - 1), // df/dx
                    200 * (x[1] - x[0]*x[0])                                            // df/dy
                }

            );


            var constraints = new List<NonlinearConstraint>();

            constraints.Add(new NonlinearConstraint(f,

                function: (x) => x[0],
                gradient: (x) => new[] { 1.0, 0.0 },

                shouldBe: ConstraintType.GreaterThanOrEqualTo, value: 0
            ));

            constraints.Add(new NonlinearConstraint(f,

                function: (x) => x[1],
                gradient: (x) => new[] { 0.0, 1.0 },

                shouldBe: ConstraintType.GreaterThanOrEqualTo, value: 0
            ));

            var solver = new AugmentedLagrangian(f, constraints);

            Assert.IsTrue(solver.Minimize());
            double minValue = solver.Value;

            Assert.AreEqual(0, minValue, 1e-10);
            Assert.AreEqual(1, solver.Solution[0], 1e-5);
            Assert.AreEqual(1, solver.Solution[1], 1e-5);

            Assert.IsFalse(Double.IsNaN(minValue));
            Assert.IsFalse(Double.IsNaN(solver.Solution[0]));
            Assert.IsFalse(Double.IsNaN(solver.Solution[1]));

        }

#if !NET35
        [TestMethod()]
        public void AugmentedLagrangianSolverConstructorTest3()
        {
            // min x*y+ y*z
            //
            // s.t.  x^2 - y^2 + z^2 - 2  >= 0
            //       x^2 + y^2 + z^2 - 10 <= 0
            //

            double x = 0, y = 0, z = 0;

            var f = new NonlinearObjectiveFunction(

                function: () => x * y + y * z,

                gradient: () => new[] 
                {
                    y,     // df/dx
                    x + z, // df/dy
                    y,     // df/dz
                }

            );


            var constraints = new List<NonlinearConstraint>();

            constraints.Add(new NonlinearConstraint(f,

                function: () => x * x - y * y + z * z,
                gradient: () => new[] { 2 * x, -2 * y, 2 * z },

                shouldBe: ConstraintType.GreaterThanOrEqualTo, value: 2
            ));

            constraints.Add(new NonlinearConstraint(f,

                function: () => x * x + y * y + z * z,
                gradient: () => new[] { 2 * x, 2 * y, 2 * z },

                shouldBe: ConstraintType.LesserThanOrEqualTo, value: 10
            ));

            var solver = new AugmentedLagrangian(f, constraints);

            solver.Solution[0] = 1;
            solver.Solution[1] = 1;
            solver.Solution[2] = 1;

            bool success = solver.Minimize();
            double minValue = solver.Value;
            Assert.IsTrue(success);

            Assert.AreEqual(-6.9, minValue, 1e-1);
            Assert.AreEqual(+1.73, solver.Solution[0], 1e-2);
            Assert.AreEqual(-2.00, solver.Solution[1], 1e-2);
            Assert.AreEqual(+1.73, solver.Solution[2], 1e-2);

            Assert.IsFalse(Double.IsNaN(minValue));
            Assert.IsFalse(Double.IsNaN(solver.Solution[0]));
            Assert.IsFalse(Double.IsNaN(solver.Solution[1]));
            Assert.IsFalse(Double.IsNaN(solver.Solution[2]));

        }

        [TestMethod()]
        public void AugmentedLagrangianSolverConstructorTest4()
        {
            // min x*y+ y*z
            //
            // s.t.  x^2 - y^2 + z^2 - 2  >= 0
            //       x^2 + y^2 + z^2 - 10 <= 0
            //       x   + y               = 1
            //

            double x = 0, y = 0, z = 0;

            var f = new NonlinearObjectiveFunction(

                function: () => x * y + y * z,

                gradient: () => new[] 
                {
                    y,     // df/dx
                    x + z, // df/dy
                    y,     // df/dz
                }

            );


            var constraints = new List<NonlinearConstraint>();

            constraints.Add(new NonlinearConstraint(f,

                function: () => x * x - y * y + z * z,
                gradient: () => new[] { 2 * x, -2 * y, 2 * z },

                shouldBe: ConstraintType.GreaterThanOrEqualTo, value: 2
            ));

            constraints.Add(new NonlinearConstraint(f,

                function: () => x * x + y * y + z * z,
                gradient: () => new[] { 2 * x, 2 * y, 2 * z },

                shouldBe: ConstraintType.LesserThanOrEqualTo, value: 10
            ));

            constraints.Add(new NonlinearConstraint(f,

                function: () => x + y,
                gradient: () => new[] { 1.0, 1.0, 0.0 },

                shouldBe: ConstraintType.EqualTo, value: 1
            )
            {
                Tolerance = 1e-5
            });

            var solver = new AugmentedLagrangian(f, constraints);

            solver.Solution[0] = 1;
            solver.Solution[1] = 1;
            solver.Solution[2] = 1;

            Assert.IsTrue(solver.Minimize());
            double minValue = solver.Value;

            Assert.AreEqual(1, solver.Solution[0] + solver.Solution[1], 1e-4);

            Assert.IsFalse(Double.IsNaN(minValue));
            Assert.IsFalse(Double.IsNaN(solver.Solution[0]));
            Assert.IsFalse(Double.IsNaN(solver.Solution[1]));
            Assert.IsFalse(Double.IsNaN(solver.Solution[2]));
        }

        [TestMethod()]
        public void AugmentedLagrangianSolverConstructorTest5()
        {
            // Suppose we would like to minimize the following function:
            //
            //    f(x,y) = min 100(y-x²)²+(1-x)²
            //
            // Subject to the constraints
            //
            //    x >= 0  (x must be positive)
            //    y >= 0  (y must be positive)
            //

            double x = 0, y = 0;


            // First, we create our objective function
            var f = new NonlinearObjectiveFunction(

                // This is the objective function:  f(x,y) = min 100(y-x²)²+(1-x)²
                function: () => 100 * Math.Pow(y - x * x, 2) + Math.Pow(1 - x, 2),

                // The gradient vector:
                gradient: () => new[] 
                {
                    2 * (200 * Math.Pow(x, 3) - 200 * x * y + x - 1), // df/dx = 2(200x³-200xy+x-1)
                    200 * (y - x*x)                                   // df/dy = 200(y-x²)
                }

            );


            // Now we can start stating the constraints
            var constraints = new List<NonlinearConstraint>();

            // Add the non-negativity constraint for x
            constraints.Add(new NonlinearConstraint(f,

                // 1st constraint: x should be greater than or equal to 0
                function: () => x, shouldBe: ConstraintType.GreaterThanOrEqualTo, value: 0,

                gradient: () => new[] { 1.0, 0.0 }
            ));

            // Add the non-negativity constraint for y
            constraints.Add(new NonlinearConstraint(f,

                // 2nd constraint: y should be greater than or equal to 0
                function: () => y, shouldBe: ConstraintType.GreaterThanOrEqualTo, value: 0,

                gradient: () => new[] { 0.0, 1.0 }
            ));


            // Finally, we create the non-linear programming solver
            var solver = new AugmentedLagrangian(f, constraints);

            // And attempt to solve the problem
            Assert.IsTrue(solver.Minimize());
            double minValue = solver.Value;

            Assert.AreEqual(0, minValue, 1e-10);
            Assert.AreEqual(1, solver.Solution[0], 1e-6);
            Assert.AreEqual(1, solver.Solution[1], 1e-6);

            Assert.IsFalse(Double.IsNaN(minValue));
            Assert.IsFalse(Double.IsNaN(solver.Solution[0]));
            Assert.IsFalse(Double.IsNaN(solver.Solution[1]));
        }

        [TestMethod()]
        public void AugmentedLagrangianSolverConstructorTest6()
        {
            test1(new ConjugateGradient(2), 1e-3);
            test1(new BoundedBroydenFletcherGoldfarbShanno(2), 1e-4);
            test1(new BroydenFletcherGoldfarbShanno(2), 1e-4);
            test1(new ResilientBackpropagation(2), 1e-2);
        }

        [TestMethod()]
        public void AugmentedLagrangianSolverConstructorTest7()
        {
            test2(new ConjugateGradient(2));
            test2(new BroydenFletcherGoldfarbShanno(2));
        }

        private static void test1(IGradientOptimizationMethod inner, double tol)
        {

            // maximize 2x + 3y, s.t. 2x² + 2y² <= 50 and x+y = 1

            // Max x' * c
            //  x

            // s.t. x' * A * x <= k
            //      x' * i     = 1
            // lower_bound < x < upper_bound

            double[] c = { 2, 3 };
            double[,] A = { { 2, 0 }, { 0, 2 } };
            double k = 50;

            // Create the objective function
            var objective = new NonlinearObjectiveFunction(2,
                function: (x) => x.InnerProduct(c),
                gradient: (x) => c
            );

            // Test objective
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    double expected = i * 2 + j * 3;
                    double actual = objective.Function(new double[] { i, j });
                    Assert.AreEqual(expected, actual);
                }
            }


            // Create the optimization constraints
            var constraints = new List<NonlinearConstraint>();

            constraints.Add(new QuadraticConstraint(objective,
                quadraticTerms: A,
                shouldBe: ConstraintType.LesserThanOrEqualTo, value: k
            ));

            constraints.Add(new NonlinearConstraint(objective,
                function: (x) => x.Sum(),
                gradient: (x) => new[] { 1.0, 1.0 },
                shouldBe: ConstraintType.EqualTo, value: 1,
                withinTolerance: 1e-10
            ));


            // Test first constraint
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    double expected = i * (2 * i + 0 * j) + j * (0 * i + 2 * j);
                    double actual = constraints[0].Function(new double[] { i, j });
                    Assert.AreEqual(expected, actual);
                }
            }


            // Test second constraint
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    double expected = i + j;
                    double actual = constraints[1].Function(new double[] { i, j });
                    Assert.AreEqual(expected, actual);
                }
            }



            AugmentedLagrangian solver =
                new AugmentedLagrangian(inner, objective, constraints);

            Assert.AreEqual(inner, solver.Optimizer);

            Assert.IsTrue(solver.Maximize());
            double maxValue = solver.Value;

            Assert.AreEqual(6, maxValue, tol);
            Assert.AreEqual(-3, solver.Solution[0], tol);
            Assert.AreEqual(4, solver.Solution[1], tol);
        }

        private static void test2(IGradientOptimizationMethod inner)
        {
            // maximize 2x + 3y, s.t. 2x² + 2y² <= 50
            //
            // http://www.wolframalpha.com/input/?i=max+2x+%2B+3y%2C+s.t.+2x%C2%B2+%2B+2y%C2%B2+%3C%3D+50

            // Max x' * c
            //  x

            // s.t. x' * A * x <= k
            //      x' * i     = 1
            // lower_bound < x < upper_bound

            double[] c = { 2, 3 };
            double[,] A = { { 2, 0 }, { 0, 2 } };
            double k = 50;

            // Create the objective function
            var objective = new NonlinearObjectiveFunction(2,
                function: (x) => x.InnerProduct(c),
                gradient: (x) => c
            );

            // Test objective
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    double expected = i * 2 + j * 3;
                    double actual = objective.Function(new double[] { i, j });
                    Assert.AreEqual(expected, actual);
                }
            }


            // Create the optimization constraints
            var constraints = new List<NonlinearConstraint>();

            constraints.Add(new QuadraticConstraint(objective,
                quadraticTerms: A,
                shouldBe: ConstraintType.LesserThanOrEqualTo, value: k
            ));


            // Test first constraint
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    var input = new double[] { i, j };

                    double expected = i * (2 * i + 0 * j) + j * (0 * i + 2 * j);
                    double actual = constraints[0].Function(input);
                    Assert.AreEqual(expected, actual);
                }
            }


            // Create the solver algorithm
            AugmentedLagrangian solver =
                new AugmentedLagrangian(inner, objective, constraints);

            Assert.AreEqual(inner, solver.Optimizer);

            Assert.IsTrue(solver.Maximize());
            double maxValue = solver.Value;

            Assert.AreEqual(18.02, maxValue, 1e-2);
            Assert.AreEqual(2.77, solver.Solution[0], 1e-2);
            Assert.AreEqual(4.16, solver.Solution[1], 1e-2);
        }
     
#endif


        [TestMethod]
        public void ConstructorTest2()
        {
            Accord.Math.Tools.SetupGenerator(0);

            var function = new NonlinearObjectiveFunction(2,
                function: x => x[0] * x[1],
                gradient: x => new[] { x[1], x[0] });

            NonlinearConstraint[] constraints = 
            {
                new NonlinearConstraint(function,
                    function: x => 1.0 - x[0] * x[0] - x[1] * x[1],
                    gradient: x => new [] { -2 * x[0], -2 * x[1]}),
                new NonlinearConstraint(function,
                    function: x => x[0],
                    gradient: x => new [] { 1.0, 0.0}),
            };

            var target = new ConjugateGradient(2);
            AugmentedLagrangian solver = new AugmentedLagrangian(target, function, constraints);
            
            Assert.IsTrue(solver.Minimize());
            double minimum = solver.Value;

            double[] solution = solver.Solution;

            double sqrthalf = Math.Sqrt(0.5);

            Assert.AreEqual(-0.5, minimum, 1e-5);
            Assert.AreEqual(sqrthalf, solution[0], 1e-5);
            Assert.AreEqual(-sqrthalf, solution[1], 1e-5);

            double expectedMinimum = function.Function(solver.Solution);
            Assert.AreEqual(expectedMinimum, minimum);
        }

        [TestMethod]
        public void ConstructorTest3()
        {
            // minimize f(x) = x*y*z, 
            // s.t. 
            //   
            //    1 - x² - 2y² - 3z² > 0
            //    x > 0,
            //    y > 0
            //

            // Easy three dimensional minimization in ellipsoid.
            var function = new NonlinearObjectiveFunction(3,
                function: x => x[0] * x[1] * x[2], 
                gradient: x => new[] { x[1] * x[2], x[0] * x[2], x[0] * x[1] });

            NonlinearConstraint[] constraints = 
            {
                new NonlinearConstraint(3,
                    function: x =>  1.0 - x[0] * x[0] - 2.0 * x[1] * x[1] - 3.0 * x[2] * x[2],
                    gradient: x =>  new[] { -2.0 * x[0],  -4.0 * x[1], -6.0 * x[2] }),
                new NonlinearConstraint(3,
                    function: x =>  x[0],
                    gradient: x =>  new[] { 1.0, 0, 0 }),
                new NonlinearConstraint(3,
                    function: x =>  x[1],
                    gradient: x =>  new[] { 0, 1.0, 0 }),
                new NonlinearConstraint(3,
                    function: x =>  -x[2],
                    gradient: x =>  new[] { 0, 0, -1.0 }),
            };

            for (int i = 0; i < constraints.Length; i++)
			{
                Assert.AreEqual(ConstraintType.GreaterThanOrEqualTo, constraints[i].ShouldBe);
                Assert.AreEqual(0, constraints[i].Value);
			}

            var inner = new BroydenFletcherGoldfarbShanno(3);
            inner.LineSearch = LineSearch.BacktrackingArmijo;
            inner.Corrections = 10;

            var solver = new AugmentedLagrangian(inner, function, constraints);

            Assert.AreEqual(inner, solver.Optimizer);

            Assert.IsTrue(solver.Minimize());
            double minimum = solver.Value;
            double[] solution = solver.Solution;

            double[] expected = 
            {
                1.0 / Math.Sqrt(3.0), 1.0 / Math.Sqrt(6.0), -1.0 / 3.0
            };


            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], solver.Solution[i], 1e-3);
            Assert.AreEqual(-0.078567420132031968, minimum, 1e-4);

            double expectedMinimum = function.Function(solver.Solution);
            Assert.AreEqual(expectedMinimum, minimum);
        }
    }
}
