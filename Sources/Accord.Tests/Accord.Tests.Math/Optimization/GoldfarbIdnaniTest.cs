// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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
    using System.Collections.Generic;
    using Accord.Math;
    using Accord.Math.Optimization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq.Expressions;
    using System;


    [TestClass()]
    public class GoldfarbIdnaniTest
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
        public void RunTest()
        {
            double[,] D = Matrix.Identity(3);
            double[] d = { 0, 5, 0 };

            double[,] A = 
            {
                { -4,  2,  0 },
                { -3,  1, -2 },
                {  0,  0,  1 },
            };


            double[] b = { -8, 2, 0 };

            GoldfarbIdnaniQuadraticSolver target = new GoldfarbIdnaniQuadraticSolver(3, A.Transpose(), b);

            double[] expectedSolution = { 0.4761905, 1.0476190, 2.0952381 };
            double expected = -2.380952;
            double actual = target.Minimize(D, d.Multiply(-1));
            Assert.AreEqual(expected, actual, 1e-6);
            Assert.IsFalse(double.IsNaN(actual));
            Assert.AreEqual(3, target.Iterations);

            Assert.AreEqual(0.4761905, target.Solution[0], 1e-6);
            Assert.AreEqual(1.0476190, target.Solution[1], 1e-6);
            Assert.AreEqual(2.0952381, target.Solution[2], 0.02);

            Assert.AreEqual(0.0000000, target.Lagrangian[0], 1e-6);
            Assert.AreEqual(0.2380952, target.Lagrangian[1], 1e-6);
            Assert.AreEqual(2.0952381, target.Lagrangian[2], 1e-6);


            foreach (double v in target.Solution)
                Assert.IsFalse(double.IsNaN(v));

            foreach (double v in target.Lagrangian)
                Assert.IsFalse(double.IsNaN(v));
        }

        [TestMethod()]
        public void RunTest1()
        {
            double[,] D = Matrix.Identity(3);
            double[] d = { 1, 5, 3 };

            double[,] A = 
            {
                { -4,  2,  0 },
                { -3,  1, -2 },
                {  0,  0,  1 },
            };


            double[] b = { -8, 2, 0 };

            GoldfarbIdnaniQuadraticSolver target = new GoldfarbIdnaniQuadraticSolver(3, A.Transpose(), b);

            double actual = target.Minimize(D, d.Multiply(-1));
            Assert.AreEqual(-12.41011, actual, 1e-5);
            Assert.IsFalse(double.IsNaN(actual));
            Assert.AreEqual(3, target.Iterations);
            Assert.AreEqual(0, target.Deletions);

            Assert.AreEqual(0.4157303, target.Solution[0], 1e-6);
            Assert.AreEqual(2.1123596, target.Solution[1], 1e-6);
            Assert.AreEqual(4.2247191, target.Solution[2], 1e-6);

            Assert.AreEqual(0.1460674, target.Lagrangian[0], 1e-6);
            Assert.AreEqual(0.0000000, target.Lagrangian[1], 1e-6);
            Assert.AreEqual(1.2247191, target.Lagrangian[2], 1e-6);

            foreach (double v in target.Lagrangian)
                Assert.IsFalse(double.IsNaN(v));

            foreach (double v in target.Solution)
                Assert.IsFalse(double.IsNaN(v));
        }

        [TestMethod()]
        public void RunTest2()
        {
            // Maximize f(x) = x² + 4y² -8x -16y
            //
            //      s.t. x + y <= 5
            //               x <= 3
            //             x,y >= 0
            //

            double[,] D = 
            {
                { 2, 0 }, // 1x²
                { 0, 8 }, // 4y²
            };

            double[] d = { -8, -16 };


            double[,] A = 
            {
                { 1, 1 }, // x + y
                { 1, 0 }, // x
            };

            double[] b = { 5, 3 };


            GoldfarbIdnaniQuadraticSolver target = new GoldfarbIdnaniQuadraticSolver(2, A.Transpose(), b);

            double actual = target.Minimize(D, d.Multiply(-1));
            Assert.AreEqual(64.8, actual);
            Assert.IsFalse(double.IsNaN(actual));
            Assert.AreEqual(2, target.Iterations);
            Assert.AreEqual(0, target.Deletions);

            Assert.AreEqual(4.8, target.Solution[0], 1e-6);
            Assert.AreEqual(0.2, target.Solution[1], 1e-6);

            Assert.AreEqual(17.6, target.Lagrangian[0]);
            Assert.AreEqual(0.00, target.Lagrangian[1]);

            foreach (double v in target.Lagrangian)
                Assert.IsFalse(double.IsNaN(v));

            foreach (double v in target.Solution)
                Assert.IsFalse(double.IsNaN(v));
        }

        [TestMethod()]
        public void RunTest3()
        {
            // Tested against R's QuadProg package
            /* solve.QP(matrix(c(10, -3,  1, -3, 11, -2, 1, -2, 12), 3, 3), c(1,5,3),
                     t(matrix( c(-4, 2, 1, -3, 1, -2, 0, -1, 2), 3,3)), c(-8,4,-1)) */

            double[,] D =
            {
                { 10, -3,  1 },
                { -3, 11, -2 },
                {  1, -2, 12 },
            };

            double[] d = { 1, 5, 3 };

            double[,] A = 
            {
                { -4,  2,  1 },
                { -3,  1, -2 },
                {  0, -1,  2 },
            };


            double[] b = { -8, 4, -1 };

            GoldfarbIdnaniQuadraticSolver target = new GoldfarbIdnaniQuadraticSolver(3, A.Transpose(), b);

            double actual = target.Minimize(D, d.Multiply(-1));

            Assert.AreEqual(6.8, actual, 1e-5);
            Assert.IsFalse(double.IsNaN(actual));

            Assert.AreEqual(4, target.Iterations);
            Assert.AreEqual(0, target.Deletions);

            Assert.AreEqual(+1.4, target.Solution[0], 1e-6);
            Assert.AreEqual(+0.8, target.Solution[1], 1e-6);
            Assert.AreEqual(-0.4, target.Solution[2], 1e-6);

            Assert.AreEqual(2.5333333, target.Lagrangian[0], 1e-6);
            Assert.AreEqual(9.7333333, target.Lagrangian[1], 1e-6);
            Assert.AreEqual(0.8666667, target.Lagrangian[2], 1e-6);

            Assert.AreEqual(1, target.ActiveConstraints[0]);
            Assert.AreEqual(0, target.ActiveConstraints[1]);
            Assert.AreEqual(2, target.ActiveConstraints[2]);

            foreach (double v in target.Lagrangian)
                Assert.IsFalse(double.IsNaN(v));

            foreach (double v in target.Solution)
                Assert.IsFalse(double.IsNaN(v));
        }

        [TestMethod()]
        public void RunTest4()
        {

            double[,] D = 
            {
                {  5, -2, -1 },
                { -2,  4,  3 },
                { -1,  3,  5 },
            };

            double[] d = { -2, +35, +47 };


            double[,] A = 
            {
                { 0, 0, 0 }, 
                { 0, 0, 0 }, 
                { 0, 0, 0 }, 
            };

            double[] b = { 0, 0, 0 };


            GoldfarbIdnaniQuadraticSolver target = new GoldfarbIdnaniQuadraticSolver(3, A.Transpose(), b);

            double actual = target.Minimize(D, d.Multiply(-1));
            Assert.AreEqual(-249.0, actual, 1e-6);
            Assert.IsFalse(double.IsNaN(actual));
            Assert.AreEqual(1, target.Iterations);
            Assert.AreEqual(0, target.Deletions);

            Assert.AreEqual(3, target.Solution[0], 1e-6);
            Assert.AreEqual(5, target.Solution[1], 1e-6);
            Assert.AreEqual(7, target.Solution[2], 1e-6);

            Assert.AreEqual(0, target.Lagrangian[0]);
            Assert.AreEqual(0, target.Lagrangian[1]);
            Assert.AreEqual(0, target.Lagrangian[2]);

            foreach (double v in target.Lagrangian)
                Assert.IsFalse(double.IsNaN(v));

            foreach (double v in target.Solution)
                Assert.IsFalse(double.IsNaN(v));
        }

        [TestMethod()]
        public void RunTest5()
        {
            // example from http://www.mail-archive.com/r-help@r-project.org/msg00831.html

            var cma = Matrix.Identity(10);

            var dva = new double[10];

            double[,] Ama =
            {
                {  1, 1, -1, 0,  0, 0,  0, 0,  0, 0,  0, 0 },
                { -1, 1,  0, 1,  0, 0,  0, 0,  0, 0,  0, 0 },
                {  1, 1,  0, 0, -1, 0,  0, 0,  0, 0,  0, 0 },
                { -1, 1,  0, 0,  0, 1,  0, 0,  0, 0,  0, 0 },
                {  1, 1,  0, 0,  0, 0, -1, 0,  0, 0,  0, 0 },
                { -1, 1,  0, 0,  0, 0,  0, 1,  0, 0,  0, 0 },
                {  1, 1,  0, 0,  0, 0,  0, 0, -1, 0,  0, 0 },
                { -1, 1,  0, 0,  0, 0,  0, 0,  0, 1,  0, 0 },
                {  1, 1,  0, 0,  0, 0,  0, 0,  0, 0, -1, 0 },
                { -1, 1,  0, 0,  0, 0,  0, 0,  0, 0,  0, 1 },
            };

            double[] bva = { 1, 1, -1, 0, -1, 0, -1, 0, -1, 0, -1, 0 };

            int meq = 2;

            GoldfarbIdnaniQuadraticSolver target = new GoldfarbIdnaniQuadraticSolver(10, Ama.Transpose(), bva, meq);

            double value = target.Minimize(cma, dva.Multiply(-1));

            for (int i = 0; i < target.Solution.Length; i += 2)
            {
                int N = target.Solution.Length / 2;
                Assert.AreEqual(1.0 / N, target.Solution[i], 1e-6);
                Assert.AreEqual(0.0, target.Solution[i + 1], 1e-6);
            }

            foreach (double v in target.Solution)
                Assert.IsFalse(double.IsNaN(v));

            foreach (double v in target.Lagrangian)
                Assert.IsFalse(double.IsNaN(v));

        }

        [TestMethod()]
        public void GoldfarbIdnaniConstructorTest1()
        {
            double[,] D = Matrix.Identity(3);
            double[] d = { 0, 5, 0 };

            double[,] A = 
            {
                { -4, -3, 0 },
                {  2,  1, 0 },
                {  0, -2, 1 },
            };

            double[] b = { -8, 2, 0 };

            List<LinearConstraint> constraints = new List<LinearConstraint>();
            constraints.Add(new LinearConstraint(-4, -3, +0) { Value = -8 });
            constraints.Add(new LinearConstraint(+2, +1, +0) { Value = +2 });
            constraints.Add(new LinearConstraint(+0, -2, +1) { Value = +0 });


            GoldfarbIdnaniQuadraticSolver target = new GoldfarbIdnaniQuadraticSolver(3, constraints);

            Assert.IsTrue(A.IsEqual(target.ConstraintMatrix));
            Assert.IsTrue(b.IsEqual(target.ConstraintValues));

        }

        [TestMethod()]
        public void GoldfarbIdnaniConstructorTest2()
        {
            // Solve the following optimization problem:
            //
            //  min f(x) = 2x² - xy + 4y² - 5x - 6y
            // 
            //  s.t.   x - y  ==   5  (x minus y should be equal to 5)
            //             x  >=  10  (x should be greater than or equal to 10)
            //

            // Lets first group the quadratic and linear terms. The
            // quadratic terms are +2x², +3y² and -4xy. The linear 
            // terms are -2x and +1y. So our matrix of quadratic
            // terms can be expressed as:

            double[,] Q = // 2x² -1xy +4y²
            {   
                /*           x              y      */
                /*x*/ { +2 /*xx*/ *2,  -1 /*xy*/    }, 
                /*y*/ { -1 /*xy*/   ,  +4 /*yy*/ *2 },
            };

            // Accordingly, our vector of linear terms is given by:

            double[] d = { -5 /*x*/, -6 /*y*/ }; // -5x -6y

            // We have now to express our constraints. We can do it
            // either by directly specifying a matrix A in which each
            // line refers to one of the constraints, expressing the
            // relationship between the different variables in the
            // constraint, like this:

            double[,] A = 
            {
                { 1, -1 }, // This line says that x + (-y) ... (a)
                { 1,  0 }, // This line says that x alone  ... (b)
            };

            double[] b = 
            {
                 5, // (a) ... should be equal to 5.
                10, // (b) ... should be greater than or equal to 10.
            };

            // Equalities must always come first, and in this case
            // we have to specify how many of the constraints are
            // actually equalities:

            int numberOfEqualities = 1;


            // Alternatively, we may use a more explicitly form:
            List<LinearConstraint> list = new List<LinearConstraint>();

            // Define the first constraint, which involves only x
            list.Add(new LinearConstraint(numberOfVariables: 1)
                {
                    // x is the first variable, thus located at
                    // index 0. We are specifying that x >= 10:

                    VariablesAtIndices = new[] { 0 }, // index 0 (x)
                    ShouldBe = ConstraintType.GreaterThanOrEqualTo,
                    Value = 10
                });

            // Define the second constraint, which involves x and y
            list.Add(new LinearConstraint(numberOfVariables: 2)
                {
                    // x is the first variable, located at index 0, and y is
                    // the second, thus located at 1. We are specifying that
                    // x - y = 5 by saying that the variable at position 0 
                    // times 1 plus the variable at position 1 times -1 
                    // should be equal to 5.

                    VariablesAtIndices = new int[] { 0, 1 }, // index 0 (x) and index 1 (y)
                    CombinedAs = new double[] { 1, -1 }, // when combined as x - y
                    ShouldBe = ConstraintType.EqualTo,
                    Value = 5
                });


            // Now we can finally create our optimization problem
            var target = new GoldfarbIdnaniQuadraticSolver(numberOfVariables: 2, constraints: list);


            Assert.IsTrue(A.IsEqual(target.ConstraintMatrix));
            Assert.IsTrue(b.IsEqual(target.ConstraintValues));
            Assert.AreEqual(numberOfEqualities, target.NumberOfEqualities);


            // And attempt to solve it.
            double minimumValue = target.Minimize(Q, d);


            Assert.AreEqual(170, minimumValue, 1e-10);
            Assert.AreEqual(10, target.Solution[0]);
            Assert.AreEqual(05, target.Solution[1]);

            foreach (double v in target.Solution)
                Assert.IsFalse(double.IsNaN(v));

            foreach (double v in target.Lagrangian)
                Assert.IsFalse(double.IsNaN(v));
        }

        [TestMethod()]
        public void GoldfarbIdnaniConstructorTest3()
        {
            // Solve the following optimization problem:
            //
            //  min f(x) = 2x² - xy + 4y² - 5x - 6y
            // 
            //  s.t.   x - y  ==   5  (x minus y should be equal to 5)
            //             x  >=  10  (x should be greater than or equal to 10)
            //

            // In this example we will be using some symbolic processing. 
            // The following variables could be initialized to any value.
            double x = 0, y = 0;

            // Create our objective function using a lambda expression
            var f = new QuadraticObjectiveFunction(() => 2 * (x * x) - (x * y) + 4 * (y * y) - 5 * x - 6 * y);

            // Now, create the constraints
            List<LinearConstraint> constraints = new List<LinearConstraint>();
            constraints.Add(new LinearConstraint(f, () => x - y == 5));
            constraints.Add(new LinearConstraint(f, () => x >= 10));

            // Now we create the quadratic programming solver for 2 variables, using the constraints.
            GoldfarbIdnaniQuadraticSolver solver = new GoldfarbIdnaniQuadraticSolver(2, constraints);


            double[,] A = 
            {
                { 1, -1 }, 
                { 1,  0 }, 
            };

            double[] b = 
            {
                 5, 
                10, 
            };

            Assert.IsTrue(A.IsEqual(solver.ConstraintMatrix));
            Assert.IsTrue(b.IsEqual(solver.ConstraintValues));


            double[,] Q = 
            {   
                { +2*2,  -1   }, 
                {   -1,  +4*2 },
            };

            double[] d = { -5, -6 };


            var actualQ = f.GetQuadraticTermsMatrix();
            var actuald = f.GetLinearTermsVector();

            Assert.IsTrue(Q.IsEqual(actualQ));
            Assert.IsTrue(d.IsEqual(actuald));


            // And attempt to solve it.
            double minimumValue = solver.Minimize(f);

        }

        [TestMethod()]
        public void GoldfarbIdnaniConstructorTest4()
        {
            // Solve the following optimization problem:
            //
            //  min f(x) = 2x² - xy + 4y² - 5x - 6y
            // 
            //  s.t.   x - y  ==   5  (x minus y should be equal to 5)
            //             x  >=  10  (x should be greater than or equal to 10)
            //

            var f = new QuadraticObjectiveFunction("2x² - xy + 4y² - 5x - 6y");

            List<LinearConstraint> constraints = new List<LinearConstraint>();
            constraints.Add(new LinearConstraint(f, "x-y = 5"));
            constraints.Add(new LinearConstraint(f, "x >= 10"));


            GoldfarbIdnaniQuadraticSolver target = new GoldfarbIdnaniQuadraticSolver(2, constraints);

            double[,] A = 
            {
                { 1, -1 }, 
                { 1,  0 }, 
            };

            double[] b = 
            {
                 5, 
                10, 
            };

            Assert.IsTrue(A.IsEqual(target.ConstraintMatrix));
            Assert.IsTrue(b.IsEqual(target.ConstraintValues));

            double[,] Q = 
            {   
                { +2*2,  -1   }, 
                {   -1,  +4*2 },
            };

            double[] d = { -5, -6 };


            var actualQ = f.GetQuadraticTermsMatrix();
            var actuald = f.GetLinearTermsVector();

            Assert.IsTrue(Q.IsEqual(actualQ));
            Assert.IsTrue(d.IsEqual(actuald));
        }

        [TestMethod()]
        public void GoldfarbIdnaniConstructorTest5()
        {
            // min 1x² - 2xy + 3y² +z² - 4x - 5y -z, 6x-7y <= 8, 9x + 1y <= 11, 9x-y <= 11, -z-y = 12
            // http://www.wolframalpha.com/input/?i=min+1x%C2%B2+-+2xy+%2B+3y%C2%B2+%2Bz%C2%B2+-+4x+-+5y+-z%2C+6x-7y+%3C%3D+8%2C+9x+%2B+1y+%3C%3D+11%2C+9x-y+%3C%3D+11%2C+-z-y+%3D+12

            double x = 0, y = 0, z = 0;

            var f = new QuadraticObjectiveFunction(() => x * x - 2 * x * y + 3 * y * y + z * z - 4 * x - 5 * y - z);

            List<LinearConstraint> constraints = new List<LinearConstraint>();
            constraints.Add(new LinearConstraint(f, () => 6 * x - 7 * y <= 8));
            constraints.Add(new LinearConstraint(f, () => 9 * x + 1 * y <= 11));
            constraints.Add(new LinearConstraint(f, () => 9 * x - y <= 11));
            constraints.Add(new LinearConstraint(f, () => -z - y == 12));


            GoldfarbIdnaniQuadraticSolver target = new GoldfarbIdnaniQuadraticSolver(f.NumberOfVariables, constraints);

            double value = target.Minimize(f);

            Assert.AreEqual(14376 / 109.0, value, 1e-10);

            Assert.AreEqual(-186 / 109.0, target.Solution[0], 1e-10);
            Assert.AreEqual(-284 / 109.0, target.Solution[1], 1e-10);
            Assert.AreEqual(-1024 / 109.0, target.Solution[2], 1e-10);

            foreach (double v in target.Solution)
                Assert.IsFalse(double.IsNaN(v));

        }

        [TestMethod()]
        public void GoldfarbIdnaniConstructorTest6()
        {
            // min 1x² - 2xy + 3y² +z² - 4x - 5y -z, 6x-7y <= 8, 9x + 1y <= 11, 9x-y <= 11, -z-y = 12
            // http://www.wolframalpha.com/input/?i=min+1x%C2%B2+-+2xy+%2B+3y%C2%B2+%2Bz%C2%B2+-+4x+-+5y+-z%2C+6x-7y+%3C%3D+8%2C+9x+%2B+1y+%3C%3D+11%2C+9x-y+%3C%3D+11%2C+-z-y+%3D+12

            var f = new QuadraticObjectiveFunction("1x² - 2xy + 3y² + z² - 4x - 5y -z");

            List<LinearConstraint> constraints = new List<LinearConstraint>();
            constraints.Add(new LinearConstraint(f, "6x-7y <= 8"));
            constraints.Add(new LinearConstraint(f, "9x + 1y <= 11"));
            constraints.Add(new LinearConstraint(f, "9x-y <= 11"));
            constraints.Add(new LinearConstraint(f, "-z-y = 12"));


            GoldfarbIdnaniQuadraticSolver target = new GoldfarbIdnaniQuadraticSolver(f.NumberOfVariables, constraints);

            double value = target.Minimize(f);

            Assert.AreEqual(14376 / 109.0, value, 1e-10);

            Assert.AreEqual(-186 / 109.0, target.Solution[0], 1e-10);
            Assert.AreEqual(-284 / 109.0, target.Solution[1], 1e-10);
            Assert.AreEqual(-1024 / 109.0, target.Solution[2], 1e-10);

            foreach (double v in target.Solution)
                Assert.IsFalse(double.IsNaN(v));
        }

        [TestMethod()]
        public void GoldfarbIdnaniConstructorTest7()
        {
            // Solve the following optimization problem:
            //
            //  min f(x) = 3x² + 2xy + 3y² - y
            // 
            //  s.t.   x >=  1
            //         y >=  1
            //

            double x = 0, y = 0;

            // http://www.wolframalpha.com/input/?i=min+x%C2%B2+%2B+2xy+%2B+y%C2%B2+-+y%2C+x+%3E%3D+1%2C+y+%3E%3D+1
            var f = new QuadraticObjectiveFunction(() => 3 * (x * x) + 2 * (x * y) + 3 * (y * y) - y);

            List<LinearConstraint> constraints = new List<LinearConstraint>();
            constraints.Add(new LinearConstraint(f, () => x >= 1));
            constraints.Add(new LinearConstraint(f, () => y >= 1));


            GoldfarbIdnaniQuadraticSolver target = new GoldfarbIdnaniQuadraticSolver(2, constraints);

            double[,] A = 
            {
                { 1, 0 }, 
                { 0, 1 }, 
            };

            double[] b = 
            {
                 1, 
                 1, 
            };

            Assert.IsTrue(A.IsEqual(target.ConstraintMatrix));
            Assert.IsTrue(b.IsEqual(target.ConstraintValues));

            double[,] Q = 
            {   
                { 6, 2 }, 
                { 2, 6 },
            };

            double[] d = { 0, -1 };


            var actualQ = f.GetQuadraticTermsMatrix();
            var actuald = f.GetLinearTermsVector();

            Assert.IsTrue(Q.IsEqual(actualQ));
            Assert.IsTrue(d.IsEqual(actuald));

            double minValue = target.Minimize(f);
            double[] solution = target.Solution;

            Assert.AreEqual(7, minValue);
            Assert.AreEqual(1, solution[0]);
            Assert.AreEqual(1, solution[1]);

            Assert.AreEqual(8, target.Lagrangian[0], 1e-5);
            Assert.AreEqual(7, target.Lagrangian[1], 1e-5);

            foreach (double v in target.Solution)
                Assert.IsFalse(double.IsNaN(v));

            foreach (double v in target.Lagrangian)
                Assert.IsFalse(double.IsNaN(v));
        }

        [TestMethod()]
        public void GoldfarbIdnaniConstructorTest8()
        {
            // Solve the following optimization problem:
            //
            //  min f(x) = x² + 2xy + y² - y
            // 
            //  s.t.   x >=  1
            //         y >=  1
            //

            double x = 0, y = 0;

            // http://www.wolframalpha.com/input/?i=min+x%C2%B2+%2B+2xy+%2B+y%C2%B2+-+y%2C+x+%3E%3D+1%2C+y+%3E%3D+1
            var f = new QuadraticObjectiveFunction(() => (x * x) + 2 * (x * y) + (y * y) - y);

            List<LinearConstraint> constraints = new List<LinearConstraint>();
            constraints.Add(new LinearConstraint(f, () => x >= 1));
            constraints.Add(new LinearConstraint(f, () => y >= 1));


            GoldfarbIdnaniQuadraticSolver target = new GoldfarbIdnaniQuadraticSolver(2, constraints);

            double[,] A = 
            {
                { 1, 0 }, 
                { 0, 1 }, 
            };

            double[] b = 
            {
                 1, 
                 1, 
            };

            Assert.IsTrue(A.IsEqual(target.ConstraintMatrix));
            Assert.IsTrue(b.IsEqual(target.ConstraintValues));

            double[,] Q = 
            {   
                { 2, 2 }, 
                { 2, 2 },
            };

            double[] d = { 0, -1 };


            var actualQ = f.GetQuadraticTermsMatrix();
            var actuald = f.GetLinearTermsVector();

            Assert.IsTrue(Q.IsEqual(actualQ));
            Assert.IsTrue(d.IsEqual(actuald));

            bool thrown = false;
            try
            {
                target.Minimize(f);
            }
            catch (NonPositiveDefiniteMatrixException)
            {
                thrown = true;
            }

            Assert.IsTrue(thrown);
        }


        [TestMethod()]
        public void GoldfarbIdnaniConstructorTest9()
        {
            // Solve the following optimization problem:
            //
            //  min f(x) = 2x² + xy + y² - 5y
            // 
            //  s.t.  -x - 3y >= -2
            //        -x -  y >= 0
            //              x >=  0
            //              y >=  0
            //



            double x = 0, y = 0;

            var f = new QuadraticObjectiveFunction(() => 2 * (x * x) + (x * y) + (y * y) - 5 * y);

            List<LinearConstraint> constraints = new List<LinearConstraint>();
            constraints.Add(new LinearConstraint(f, () => -x - 3 * y >= -2));
            constraints.Add(new LinearConstraint(f, () => -x - y >= 0));
            constraints.Add(new LinearConstraint(f, () => x >= 0));
            constraints.Add(new LinearConstraint(f, () => y >= 0));


            GoldfarbIdnaniQuadraticSolver target = new GoldfarbIdnaniQuadraticSolver(2, constraints);

            double[,] expectedA = 
            {
                { -1, -3 },
                { -1, -1 },
                {  1,  0 },
                {  0,  1 },
            };

            double[] expectedb = 
            {
                -2, 0, 0, 0
            };

            double[,] expectedQ = 
            {
                { 4, 1 },
                { 1, 2 },
            };

            double[] expectedd = 
            {
                0, -5
            };

            // Tested against R's QuadProg package
            /*
               Qmat = matrix(c(4,1,1,2),2,2)
               dvec = -c(0, -5)
               Amat =  matrix(c(-1, -3, -1, -1, 1, 0, 0, 1), 2,4)
               bvec = c(-2, 0, 0, 0)
               
               solve.QP(Qmat, dvec, Amat, bvec)
            */

            var actualA = target.ConstraintMatrix;
            var actualb = target.ConstraintValues;
            var actualQ = f.GetQuadraticTermsMatrix();
            var actuald = f.GetLinearTermsVector();

            Assert.IsTrue(expectedA.IsEqual(actualA));
            Assert.IsTrue(expectedb.IsEqual(actualb));
            Assert.IsTrue(expectedQ.IsEqual(actualQ));
            Assert.IsTrue(expectedd.IsEqual(actuald));

            double min = target.Minimize(f);

            double[] solution = target.Solution;

            Assert.AreEqual(0, solution[0], 1e-10);
            Assert.AreEqual(0, solution[1], 1e-10);

            Assert.AreEqual(0.0, min, 1e-10);

            Assert.AreEqual(0, target.Lagrangian[0], 1e-10);
            Assert.AreEqual(5, target.Lagrangian[1], 1e-10);
            Assert.AreEqual(5, target.Lagrangian[2], 1e-10);
            Assert.AreEqual(0, target.Lagrangian[3], 1e-10);


            Assert.IsFalse(Double.IsNaN(min));

            foreach (double v in target.Solution)
                Assert.IsFalse(double.IsNaN(v));

            foreach (double v in target.Lagrangian)
                Assert.IsFalse(double.IsNaN(v));
        }

        [TestMethod()]
        public void GoldfarbIdnaniMaximizeTest1()
        {
            // Solve the following optimization problem:
            //
            //  max f(x) = -2x² + xy - y² + 5y
            // 
            //  s.t.   x + y  <= 0
            //             y  >= 0
            //

            // Create our objective function using a text string
            var f = new QuadraticObjectiveFunction("-2x² + xy - y² + 5y");

            // Now, create the constraints
            List<LinearConstraint> constraints = new List<LinearConstraint>();
            constraints.Add(new LinearConstraint(f, "x + y <= 0"));
            constraints.Add(new LinearConstraint(f, "    y >= 0"));

            // Now we create the quadratic programming solver for 2 variables, using the constraints.
            GoldfarbIdnaniQuadraticSolver solver = new GoldfarbIdnaniQuadraticSolver(2, constraints);

            // And attempt to solve it.
            double maxValue = solver.Maximize(f);

            Assert.AreEqual(25 / 16.0, maxValue);

            Assert.AreEqual(-5 / 8.0, solver.Solution[0]);
            Assert.AreEqual(5 / 8.0, solver.Solution[1]);
        }

        [Ignore()] // TODO: Remove this tag
        [TestMethod()]
        public void GoldfarbIdnaniMinimizeTest1()
        {
            // This test reproduces Issue #33 at Google Code Tracker
            // https://code.google.com/p/accord/issues/detail?id=33

            // Create objective function using the
            // Hessian Q and linear terms vector d.

            double[,] Q =
            {
                { 0.12264004,  0.011579293, 0.103326825, 0.064073439 },
                { 0.011579293, 0.033856,    0.014311947, 0.014732381 },
                { 0.103326825, 0.014311947, 0.17715681,  0.067615114 },
                { 0.064073439, 0.014732381, 0.067615114, 0.11539609 }
            };

            Assert.IsTrue(Q.IsPositiveDefinite());

            double[] d = { 0, 0, 0, 0 };

            var f = new QuadraticObjectiveFunction(Q, d, "a", "b", "c", "d");

            // Now, create the constraints
            var constraints = new LinearConstraintCollection();

            constraints.Add(new LinearConstraint(f, "0.0732 * a + 0.0799 * b + 0.1926 * c + 0.0047 * d = 0.098"));
            constraints.Add(new LinearConstraint(f, "a + b + c + d = 1"));
            constraints.Add(new LinearConstraint(f, "a >= 0"));
            constraints.Add(new LinearConstraint(f, "b >= 0"));
            constraints.Add(new LinearConstraint(f, "c >= 0"));
            constraints.Add(new LinearConstraint(f, "d >= 0"));
            constraints.Add(new LinearConstraint(f, "a >= 0.5"));

            double[] b;
            int eq;
            double[,] A = constraints.CreateMatrix(4, out b, out eq);

            // Now we create the quadratic programming solver for 2 variables, using the constraints.
            GoldfarbIdnaniQuadraticSolver solver = new GoldfarbIdnaniQuadraticSolver(4, constraints);

            // And attempt to solve it.
            double minValue = solver.Minimize(f);

            double[] expected = { 0.5, 0.336259542, 0.163740458, 0 };
            double[] actual = solver.Solution;

            for (int i = 0; i < expected.Length; i++)
            {
                double e = expected[i];
                double a = actual[i];
                Assert.AreEqual(e, a);
            }
        }

    }
}
