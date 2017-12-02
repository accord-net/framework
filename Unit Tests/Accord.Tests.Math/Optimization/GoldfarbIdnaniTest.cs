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
    using System.Collections.Generic;
    using Accord.Math;
    using Accord.Math.Optimization;
    using NUnit.Framework;
    using System.IO;
    using Accord.Tests.Math.Properties;
    using System.Globalization;
    using Accord.IO;
#if NO_CULTURE
    using CultureInfo = Accord.Compat.CultureInfoEx;
#endif

    [TestFixture]
    public class GoldfarbIdnaniTest
    {

        [Test]
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

            GoldfarbIdnani target = new GoldfarbIdnani(D, d.Multiply(-1), A.Transpose(), b);

            double[] expectedSolution = { 0.4761905, 1.0476190, 2.0952381 };
            double expected = -2.380952;
            Assert.IsTrue(target.Minimize());
            double actual = target.Value;
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

        [Test]
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

            GoldfarbIdnani target = new GoldfarbIdnani(D, d.Multiply(-1), A.Transpose(), b);

            Assert.IsTrue(target.Minimize());
            double actual = target.Value;
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

        [Test]
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


            GoldfarbIdnani target = new GoldfarbIdnani(D, d.Multiply(-1), A.Transpose(), b);

            Assert.IsTrue(target.Minimize());
            double actual = target.Value;
            Assert.AreEqual(64.8, actual, 1e-10);
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

        [Test]
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

            GoldfarbIdnani target = new GoldfarbIdnani(D, d.Multiply(-1), A.Transpose(), b);

            Assert.IsTrue(target.Minimize());
            double actual = target.Value;

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

        [Test]
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


            GoldfarbIdnani target = new GoldfarbIdnani(D, d.Multiply(-1), A.Transpose(), b);

            Assert.IsTrue(target.Minimize());
            double actual = target.Value;

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

        [Test]
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

            GoldfarbIdnani target = new GoldfarbIdnani(cma, dva.Multiply(-1), Ama.Transpose(), bva, meq);

            Assert.IsTrue(target.Minimize());
            double value = target.Value;

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

        [Test]
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

            QuadraticObjectiveFunction f = new QuadraticObjectiveFunction("2x² + y - z + 2");

            GoldfarbIdnani target = new GoldfarbIdnani(f, constraints);

            Assert.IsTrue(A.IsEqual(target.ConstraintMatrix));
            Assert.IsTrue(b.IsEqual(target.ConstraintValues));
        }

        [Test]
        public void GoldfarbIdnaniConstructorTest2()
        {
            #region doc_matrix
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


            // Alternatively, we may use an explicit form:
            var constraints = new List<LinearConstraint>()
            {
                // Define the first constraint, which involves only x
                new LinearConstraint(numberOfVariables: 1)
                {
                    // x is the first variable, thus located at
                    // index 0. We are specifying that x >= 10:

                    VariablesAtIndices = new[] { 0 }, // index 0 (x)
                    ShouldBe = ConstraintType.GreaterThanOrEqualTo,
                    Value = 10
                },

                // Define the second constraint, which involves x and y
                new LinearConstraint(numberOfVariables: 2)
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
                }
            };


            // Now we can finally create our optimization problem
            var solver = new GoldfarbIdnani(
                function: new QuadraticObjectiveFunction(Q, d),
                constraints: constraints);


            // And attempt solve for the min:
            bool success = solver.Minimize();

            // The solution was { 10, 5 }
            double[] solution = solver.Solution;

            // With the minimum value 170.0
            double minValue = solver.Value;
            #endregion


            Assert.IsTrue(A.IsEqual(solver.ConstraintMatrix));
            Assert.IsTrue(b.IsEqual(solver.ConstraintValues));
            Assert.AreEqual(numberOfEqualities, solver.NumberOfEqualities);

            Assert.AreEqual(170, minValue, 1e-10);
            Assert.AreEqual(10, solver.Solution[0]);
            Assert.AreEqual(05, solver.Solution[1]);

            foreach (double v in solver.Solution)
                Assert.IsFalse(double.IsNaN(v));

            foreach (double v in solver.Lagrangian)
                Assert.IsFalse(double.IsNaN(v));
        }

        [Test]
        public void GoldfarbIdnaniConstructorTest3()
        {
            // http://www.wolframalpha.com/input/?i=min+2x%C2%B2+-+xy+%2B+4y%C2%B2+-+5x+-+6y+s.t.+x+-+y++%3D%3D+++5%2C+x++%3E%3D++10
            #region doc_lambdas
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
            List<LinearConstraint> constraints = new List<LinearConstraint>()
            {
                new LinearConstraint(f, () => x - y == 5),
                new LinearConstraint(f, () => x >= 10)
            };

            // Now we create the quadratic programming solver 
            var solver = new GoldfarbIdnani(f, constraints);

            // And attempt solve for the min:
            bool success = solver.Minimize();

            // The solution was { 10, 5 }
            double[] solution = solver.Solution;

            // With the minimum value 170.0
            double minValue = solver.Value;
            #endregion

            Assert.AreEqual(170, solver.Value);
            Assert.IsTrue(success);

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


            var actualQ = f.QuadraticTerms;
            var actuald = f.LinearTerms;

            Assert.IsTrue(Q.IsEqual(actualQ));
            Assert.IsTrue(d.IsEqual(actuald));
        }

        [Test]
        public void GoldfarbIdnaniConstructorTest10()
        {
            // http://www.wolframalpha.com/input/?i=minimize+f%28x%2Cy%29+%3D+2x%5E2+-+xy+%2B+4y%5E2+-+5x+-+6y+-+100%2C+s.t.+x+-+y++%3D+++5%2C+x++%3E%3D++10

            double x = 0, y = 0;

            var f = new QuadraticObjectiveFunction(() => 2 * (x * x) - (x * y) + 4 * (y * y) - 5 * x - 6 * y - 100);

            List<LinearConstraint> constraints = new List<LinearConstraint>();
            constraints.Add(new LinearConstraint(f, () => x - y == 5));
            constraints.Add(new LinearConstraint(f, () => x >= 10));

            GoldfarbIdnani solver = new GoldfarbIdnani(f, constraints);

            double[,] Q =
            {
                { +2*2,  -1   },
                {   -1,  +4*2 },
            };

            double[] d = { -5, -6 };

            var actualQ = f.QuadraticTerms;
            var actuald = f.LinearTerms;

            Assert.IsTrue(Q.IsEqual(actualQ));
            Assert.IsTrue(d.IsEqual(actuald));
            Assert.AreEqual(-100, f.ConstantTerm);

            bool success = solver.Minimize();
            Assert.AreEqual(70, solver.Value);
            Assert.IsTrue(success);
        }

        [Test]
        public void GoldfarbIdnaniConstructorTest11()
        {
            // http://www.wolframalpha.com/input/?i=minimize+f%28x%2Cy%29+%3D+2x%5E2+-+xy+%2B+4y%5E2+-+5x+-+6y+-+100%2C+s.t.+x+-+y++%3D+++5%2C+x++%3E%3D++10

            double x = 0, y = 0;

            var f = new QuadraticObjectiveFunction(() => 2 * (x * x) - (x * y) + 4 * (y * y) - 5 * x - 6 * y + 100);

            List<LinearConstraint> constraints = new List<LinearConstraint>();
            constraints.Add(new LinearConstraint(f, () => x - y == 5));
            constraints.Add(new LinearConstraint(f, () => x >= 10));

            GoldfarbIdnani solver = new GoldfarbIdnani(f, constraints);

            double[,] Q =
            {
                { +2*2,  -1   },
                {   -1,  +4*2 },
            };

            double[] d = { -5, -6 };

            var actualQ = f.QuadraticTerms;
            var actuald = f.LinearTerms;

            Assert.IsTrue(Q.IsEqual(actualQ));
            Assert.IsTrue(d.IsEqual(actuald));
            Assert.AreEqual(100, f.ConstantTerm);

            bool success = solver.Minimize();
            Assert.AreEqual(270, solver.Value);
            Assert.IsTrue(success);
        }

        [Test]
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


            GoldfarbIdnani target = new GoldfarbIdnani(f, constraints);

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


            var actualQ = f.QuadraticTerms;
            var actuald = f.LinearTerms;

            Assert.IsTrue(Q.IsEqual(actualQ));
            Assert.IsTrue(d.IsEqual(actuald));
        }

        [Test]
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


            GoldfarbIdnani target = new GoldfarbIdnani(f, constraints);

            bool success = target.Minimize();
            double value = target.Value;

            Assert.IsTrue(success);

            Assert.AreEqual(14376 / 109.0, value, 1e-10);

            Assert.AreEqual(-186 / 109.0, target.Solution[0], 1e-10);
            Assert.AreEqual(-284 / 109.0, target.Solution[1], 1e-10);
            Assert.AreEqual(-1024 / 109.0, target.Solution[2], 1e-10);

            foreach (double v in target.Solution)
                Assert.IsFalse(double.IsNaN(v));

        }

        [Test]
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


            GoldfarbIdnani target = new GoldfarbIdnani(f, constraints);

            bool success = target.Minimize();
            double value = target.Value;

            Assert.IsTrue(success);
            Assert.AreEqual(14376 / 109.0, value, 1e-10);

            Assert.AreEqual(-186 / 109.0, target.Solution[0], 1e-10);
            Assert.AreEqual(-284 / 109.0, target.Solution[1], 1e-10);
            Assert.AreEqual(-1024 / 109.0, target.Solution[2], 1e-10);

            foreach (double v in target.Solution)
                Assert.IsFalse(double.IsNaN(v));
        }

        [Test]
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


            GoldfarbIdnani target = new GoldfarbIdnani(f, constraints);

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


            var actualQ = f.QuadraticTerms;
            var actuald = f.LinearTerms;

            Assert.IsTrue(Q.IsEqual(actualQ));
            Assert.IsTrue(d.IsEqual(actuald));

            bool success = target.Minimize();
            double minValue = target.Value;

            Assert.IsTrue(success);

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

        [Test]
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


            GoldfarbIdnani target = new GoldfarbIdnani(f, constraints);

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


            var actualQ = f.QuadraticTerms;
            var actuald = f.LinearTerms;

            Assert.IsTrue(Q.IsEqual(actualQ));
            Assert.IsTrue(d.IsEqual(actuald));

            bool success = target.Minimize();

            Assert.IsFalse(success);
        }

        [Test]
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


            GoldfarbIdnani target = new GoldfarbIdnani(f, constraints);

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
            var actualQ = f.QuadraticTerms;
            var actuald = f.LinearTerms;

            Assert.IsTrue(expectedA.IsEqual(actualA));
            Assert.IsTrue(expectedb.IsEqual(actualb));
            Assert.IsTrue(expectedQ.IsEqual(actualQ));
            Assert.IsTrue(expectedd.IsEqual(actuald));

            Assert.IsTrue(target.Minimize());
            double min = target.Value;

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

        [Test]
        public void GoldfarbIdnaniMaximizeTest1()
        {
            #region doc_string
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
            List<LinearConstraint> constraints = new List<LinearConstraint>()
            {
                new LinearConstraint(f, "x + y <= 0"),
                new LinearConstraint(f, "    y >= 0")
            };

            // Now we create the quadratic programming solver 
            var solver = new GoldfarbIdnani(f, constraints);

            // And attempt solve for the max:
            bool success = solver.Maximize();

            // The solution was { -0.625, 0.625 }
            double[] solution = solver.Solution;

            // With the minimum value 1.5625
            double maxValue = solver.Value;
            #endregion

            Assert.IsTrue(success);
            Assert.AreEqual(25 / 16.0, maxValue);

            Assert.AreEqual(-5 / 8.0, solver.Solution[0]);
            Assert.AreEqual(5 / 8.0, solver.Solution[1]);
        }

        [Test]
        public void GoldfarbIdnaniParseGlobalizationTestBase()
        {
            // minimize 0.5x² + 0.2y² + 0.3xy s.t. 0.01x + 0.02y - 0.03 = 0 AND x + y = 100
            // http://www.wolframalpha.com/input/?i=minimize+0.5x%C2%B2+%2B+0.2y%C2%B2+%2B+0.3xy+s.t.+0.01x+%2B+0.02y+-+0.03+%3D+0+AND+x+%2B+y+%3D+100

            String strObjective = "0.5x² + 0.2y² + 0.3xy";

            String[] strConstraints =
            {
                "0.01x + 0.02y - 0.03 = 0",
                "x + y = 100"
            };

            QuadraticObjectiveFunction function = new QuadraticObjectiveFunction(strObjective);
            LinearConstraintCollection cst = new LinearConstraintCollection();
            foreach (var tmpCst in strConstraints)
                cst.Add(new LinearConstraint(function, tmpCst));

            var classSolver = new Accord.Math.Optimization.GoldfarbIdnani(function, cst);
            bool status = classSolver.Minimize();
            double result = classSolver.Value;

            Assert.IsTrue(status);
            Assert.AreEqual(15553.60, result, 1e-10);
        }

        [Test]
        public void GoldfarbIdnaniParseGlobalizationTest()
        {
            var fr = CultureInfo.GetCultureInfo("fr-FR");

            Assert.AreEqual(",", fr.NumberFormat.NumberDecimalSeparator);

            String strObjective = 0.5.ToString(fr)
                + "x² +" + 0.2.ToString(fr) + "y² +"
                + 0.3.ToString(fr) + "xy";

            String[] strConstraints =
            {
                0.01.ToString(fr) + "x" + " + " +
                0.02.ToString(fr) + "y - " +
                0.03.ToString(fr) + " = 0",
                "x + y = 100"
            };

            QuadraticObjectiveFunction function = new QuadraticObjectiveFunction(strObjective, fr);
            LinearConstraintCollection cst = new LinearConstraintCollection();
            foreach (var tmpCst in strConstraints)
                cst.Add(new LinearConstraint(function, tmpCst, fr));

            var classSolver = new Accord.Math.Optimization.GoldfarbIdnani(function, cst);
            bool status = classSolver.Minimize();
            double result = classSolver.Value;

            Assert.IsTrue(status);
            Assert.AreEqual(15553.60, result, 1e-10);
        }

        [Test]
        public void GoldfarbIdnaniParseGlobalizationTest2()
        {
            String strObjective = 0.5.ToString(CultureInfo.InvariantCulture)
                + "x² +" + 0.2.ToString(CultureInfo.InvariantCulture) + "y² +"
                + 0.3.ToString(CultureInfo.InvariantCulture) + "xy";

            String[] strConstraints =
            {
                0.01.ToString(CultureInfo.InvariantCulture) + "x" + " + " +
                0.02.ToString(CultureInfo.InvariantCulture) + "y - " +
                0.03.ToString(CultureInfo.InvariantCulture) + " = 0",
                "x + y = 100"
            };

            QuadraticObjectiveFunction function = new QuadraticObjectiveFunction(strObjective);
            LinearConstraintCollection cst = new LinearConstraintCollection();
            foreach (var tmpCst in strConstraints)
                cst.Add(new LinearConstraint(function, tmpCst));

            var classSolver = new Accord.Math.Optimization.GoldfarbIdnani(function, cst);
            bool status = classSolver.Minimize();
            double result = classSolver.Value;

            Assert.IsTrue(status);
            Assert.AreEqual(15553.60, result, 1e-10);
        }

        [Test]
        public void GoldfarbIdnaniParseTest()
        {
            var s = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            String strObjective = "0" + s + "5x² + 0" + s + "2y² + 0" + s + "3xy";

            String[] strConstraints =
            {
                "0" + s + "01x + 0" + s + "02y - 0" + s + "03 = 0",
                "x + y = 100"
            };

            // Now we can start creating our function:
            QuadraticObjectiveFunction function = new QuadraticObjectiveFunction(strObjective, CultureInfo.CurrentCulture);
            LinearConstraintCollection cst = new LinearConstraintCollection();
            foreach (var tmpCst in strConstraints)
                cst.Add(new LinearConstraint(function, tmpCst, CultureInfo.CurrentCulture));

            var classSolver = new Accord.Math.Optimization.GoldfarbIdnani(function, cst);
            bool status = classSolver.Minimize();
            double result = classSolver.Value;

            Assert.IsTrue(status);
            Assert.AreEqual(15553.60, result, 1e-10);
        }

        [Test]
        public void GoldfarbIdnaniMinimizeWithEqualityTest()
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
                { 0.064073439, 0.014732381, 0.067615114, 0.11539609  }
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

            Assert.AreEqual(1, constraints[6].CombinedAs[0]);
            Assert.AreEqual(0.5, constraints[6].Value);
            Assert.AreEqual(0.1, constraints[6].GetViolation(new double[] { 0.6 }), 1e-10);
            Assert.AreEqual(-0.1, constraints[6].GetViolation(new double[] { 0.4 }), 1e-10);

            bool psd = Q.IsPositiveDefinite();

            double[] b;
            int eq;
            double[,] A = constraints.CreateMatrix(4, out b, out eq);

            // AccordTestsMathCpp2.Quadprog.Compute(A.GetLength(1), A.GetLength(0), 
            //    A.Reshape(), b, eq, Q.Reshape(), d);


            // Now we create the quadratic programming solver for 2 variables, using the constraints.
            GoldfarbIdnani solver = new GoldfarbIdnani(f, constraints);

            // And attempt to solve it.
            Assert.IsTrue(solver.Minimize());
            double minValue = solver.Value;

            double[] expected = { 0.50000000000000, 0.30967169476486, 0.19032830523514, 0 };
            double[] actual = solver.Solution;

            for (int i = 0; i < constraints.Count; i++)
            {
                double error = constraints[i].GetViolation(actual);
                Assert.IsTrue(error >= 0);
            }

            for (int i = 0; i < expected.Length; i++)
            {
                double e = expected[i];
                double a = actual[i];
                Assert.AreEqual(e, a, 1e-10);
            }
        }

        [Test]
        public void GoldfarbIdnaniMinimizeLessThanWithEqualityTest()
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
                { 0.064073439, 0.014732381, 0.067615114, 0.11539609  }
            };

            Assert.IsTrue(Q.IsPositiveDefinite());

            double[] d = { 0, 0, 0, 0 };

            var f = new QuadraticObjectiveFunction(Q, d, "a", "b", "c", "d");

            // Now, create the constraints
            var constraints = new LinearConstraintCollection();

            constraints.Add(new LinearConstraint(f, "0.0732 * a + 0.0799 * b + 0.1926 * c + 0.0047 * d = 0.098"));
            constraints.Add(new LinearConstraint(f, "a + b + c + d = 1"));
            constraints.Add(new LinearConstraint(f, "-a <= 0"));
            constraints.Add(new LinearConstraint(f, "-b <= 0"));
            constraints.Add(new LinearConstraint(f, "-c <= 0"));
            constraints.Add(new LinearConstraint(f, "-d <= 0"));
            constraints.Add(new LinearConstraint(f, "-a + 0.5 <= 0.0"));

            Assert.AreEqual(-1, constraints[6].CombinedAs[0]);
            Assert.AreEqual(-0.5, constraints[6].Value);
            Assert.AreEqual(0.1, constraints[6].GetViolation(new double[] { 0.6 }), 1e-10);
            Assert.AreEqual(-0.1, constraints[6].GetViolation(new double[] { 0.4 }), 1e-10);

            bool psd = Q.IsPositiveDefinite();

            double[] b;
            int eq;
            double[,] A = constraints.CreateMatrix(4, out b, out eq);

            // Now we create the quadratic programming solver for 2 variables, using the constraints.
            GoldfarbIdnani solver = new GoldfarbIdnani(f, constraints);

            // And attempt to solve it.
            Assert.IsTrue(solver.Minimize());
            double minValue = solver.Value;

            double[] expected = { 0.50000000000000, 0.30967169476486, 0.19032830523514, 0 };
            double[] actual = solver.Solution;

            for (int i = 0; i < constraints.Count; i++)
            {
                double error = constraints[i].GetViolation(actual);
                Assert.IsTrue(error >= 0);
            }

            for (int i = 0; i < expected.Length; i++)
            {
                double e = expected[i];
                double a = actual[i];
                Assert.AreEqual(e, a, 1e-10);
            }
        }


        [Test]
        public void GoldfarbIdnaniLargeSampleTest1()
        {
            var Q = readMatrixFile(new StringReader(Resources.dmatFull));
            var AMat = readMatrixFile(new StringReader(Resources.constraintMatrix11_15));
            var bvec = readVectorFile(new StringReader(Resources.constraints11_14));

            var dvec = new double[Q.GetLength(0)];
            double[] b = new double[bvec.Length];
            bvec.CopyTo(b, 0);

            bool psd = Q.IsPositiveDefinite();
            Assert.IsTrue(psd);

            GoldfarbIdnani gfI = new GoldfarbIdnani(Q, dvec, AMat, b, 2);

            bool success = gfI.Minimize();

            Assert.IsTrue(success);

            double[] soln = gfI.Solution;
            double value = Math.Sqrt(Matrix.Multiply(Matrix.Multiply(soln, Q), soln.Transpose())[0]);

            double expectedSol = 0.049316494677822;
            double actualSol = value;

            double[] expected =
            {
                0.74083116998144, // 2
                0.14799651298617, // 13
                0.11117231703249, // 14
            };

            double[] actual =
            {
                soln[1], soln[12], soln[13]
            };

            Assert.AreEqual(expectedSol, actualSol, 1e-8);
            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], actual[i], 1e-5);
        }

        [Test]
        public void GoldfarbIdnaniLargeSampleTest2()
        {
            var Q = readMatrixFile(new StringReader(Resources.dmatFull));
            var AMat = readMatrixFile(new StringReader(Resources.constraintMatrix11_15_2));
            var bvec = readVectorFile(new StringReader(Resources.constraints11_14_2));

            var dvec = new double[Q.GetLength(0)];
            double[] b = new double[bvec.Length];
            bvec.CopyTo(b, 0);

            bool psd = Q.IsPositiveDefinite();
            Assert.IsTrue(psd);

            GoldfarbIdnani gfI = new GoldfarbIdnani(Q, dvec, AMat, b, 2);

            for (int i = 0; i < gfI.ConstraintTolerances.Length; i++)
                Assert.AreEqual(LinearConstraint.DefaultTolerance, gfI.ConstraintTolerances[i]);

            bool success = gfI.Minimize();

            Assert.IsTrue(success);

            double[] soln = gfI.Solution;
            double value = Math.Sqrt(Matrix.Multiply(Matrix.Multiply(soln, Q), soln.Transpose())[0]);

            double expectedSol = 0.048224950997808;
            double actualSol = value;

            double[] expected =
            {
                0.41144782323407, // 2
                0.27310552838116, // 13
                0.31544664838498, // 14
            };

            double[] actual =
            {
                soln[1], soln[12], soln[13]
            };

            Assert.AreEqual(expectedSol, actualSol, 1e-8);
            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], actual[i], 1e-5);
        }

        [Test]
        public void GoldfarbIdnaniLargeSampleTest3_InfiniteLoop()
        {
            var Q = readMatrixFile(new StringReader(Resources.dmatFull));
            var AMat = readMatrixFile(new StringReader(Resources.constraintMatrix11_15_3));
            var bvec = readVectorFile(new StringReader(Resources.constraints11_14_3));

            var dvec = new double[Q.GetLength(0)];
            double[] b = new double[bvec.Length];
            bvec.CopyTo(b, 0);

            bool psd = Q.IsPositiveDefinite();
            Assert.IsTrue(psd);

            GoldfarbIdnani gfI = new GoldfarbIdnani(Q, dvec, AMat, b, 2);

            for (int i = 0; i < gfI.ConstraintTolerances.Length; i++)
                gfI.ConstraintTolerances[i] = 1e-10;

            bool success = gfI.Minimize();

            Assert.IsTrue(success);

            double[] soln = gfI.Solution;
            double value = Math.Sqrt(Matrix.Multiply(Matrix.Multiply(soln, Q), soln.Transpose())[0]);

            double expectedSol = 0.052870914138455;
            double actualSol = value;

            double[] expected =
            {
                0.4, // 2
                0.0016271524831373, // 4
                0, // 5
                0, // 13
                0, // 14
                0.59837284751680053 // 19     
            };

            double[] actual =
            {
                soln[1], soln[3], soln[4], soln[12], soln[13], soln[18]
            };

            Assert.AreEqual(expectedSol, actualSol, 1e-8);
            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], actual[i], 1e-5);
        }

        [Test]
        public void GoldfarbIdnani3()
        {
            double[] bvec = { 1, 0, -1, 0, -1, 0, -1, 0, -1 };
            double[] dvec = { -0.00090022881750228, -0.0011623872153178, -0.0012785347920969, -0.0014757189594252 };

            double[,] DMat = new double[4, 4];
            DMat[0, 0] = 0.00073558149743370;
            DMat[1, 1] = 0.00077910939546937;
            DMat[2, 2] = 0.00139571859557181;
            DMat[3, 3] = 0.00165142875705900;

            DMat[0, 1] = 0.00066386470011521;
            DMat[0, 2] = 0.00088725438967435;
            DMat[0, 3] = 0.00088643939798828;

            DMat[1, 2] = 0.00084474421503143;
            DMat[1, 3] = 0.00095081100765219;
            DMat[2, 3] = 0.00143043882089429;

            DMat[1, 0] = DMat[0, 1];
            DMat[2, 0] = DMat[0, 2];
            DMat[3, 0] = DMat[0, 3];
            DMat[2, 1] = DMat[1, 2];
            DMat[3, 1] = DMat[1, 3];
            DMat[3, 2] = DMat[2, 3];

            Assert.IsTrue(DMat.IsSymmetric());


            double[,] AMat = new double[9, 4];
            AMat[0, 0] = 1; AMat[1, 0] = 1; AMat[2, 0] = -1; AMat[3, 0] = 0; AMat[4, 0] = 0; AMat[5, 0] = 0; AMat[6, 0] = 0; AMat[7, 0] = 0; AMat[8, 0] = 0;
            AMat[0, 1] = 1; AMat[1, 1] = 0; AMat[2, 1] = 0; AMat[3, 1] = 1; AMat[4, 1] = -1; AMat[5, 1] = 0; AMat[6, 1] = 0; AMat[7, 1] = 0; AMat[8, 1] = 0;
            AMat[0, 2] = 1; AMat[1, 2] = 0; AMat[2, 2] = 0; AMat[3, 2] = 0; AMat[4, 2] = 0; AMat[5, 2] = 1; AMat[6, 2] = -1; AMat[7, 2] = 0; AMat[8, 2] = 0;
            AMat[0, 3] = 1; AMat[1, 3] = 0; AMat[2, 3] = 0; AMat[3, 3] = 0; AMat[4, 3] = 0; AMat[5, 3] = 0; AMat[6, 3] = 0; AMat[7, 3] = 1; AMat[8, 3] = -1;

            var oldA = (double[,])AMat.Clone();
            var oldD = (double[,])DMat.Clone();
            var oldb = (double[])bvec.Clone();
            var oldd = (double[])dvec.Clone();

            GoldfarbIdnani gfI = new GoldfarbIdnani(DMat, dvec, AMat, bvec, 1);

            Assert.AreEqual(4, gfI.NumberOfVariables);
            Assert.AreEqual(9, gfI.NumberOfConstraints);

            Assert.IsTrue(gfI.Minimize());

            Assert.IsTrue(oldA.IsEqual(AMat));
            Assert.IsTrue(oldD.IsEqual(DMat));
            Assert.IsTrue(oldb.IsEqual(bvec));
            Assert.IsTrue(oldd.IsEqual(dvec));

            double[] soln = gfI.Solution;
            double value = gfI.Value;

            Assert.AreEqual(0, soln[0], 1e-10);
            Assert.AreEqual(0.73222257311567, soln[1], 1e-5);
            Assert.AreEqual(0, soln[2], 1e-10);
            Assert.AreEqual(0.2677742688433, soln[3], 1e-8);
            Assert.AreEqual(-0.00079179497009427, value, 1e-6);

            double[] lagrangian = gfI.Lagrangian;
            double[] expected = { 0.0003730054618697, 0.00016053620578588, 0, 0, 0, 0.000060343913971918, 0, 0, 0 };
            for (int i = 0; i < lagrangian.Length; i++)
                Assert.AreEqual(expected[i], lagrangian[i], 1e-4);
        }

        private double[,] readMatrixFile(StringReader reader)
        {
            string line = null;
            List<string> str = new List<string>();
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Length != 0)
                    str.Add(line);
            }
            char[] sep = new char[] { ',' };
            string[] parts = str[0].Split(sep, StringSplitOptions.RemoveEmptyEntries);
            double[,] v = new double[str.Count, parts.Length];
            for (int i = 0; i < str.Count; i++)
            {
                parts = str[i].Split(sep, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < parts.Length; j++)
                {
                    string prt = parts[j];
                    double prtdbl = double.Parse(prt, CultureInfo.GetCultureInfo("en-US"));
                    v[i, j] = prtdbl;
                }
            }
#if NETCORE
            reader.Dispose();
#else
            reader.Close();
#endif
            return v;
        }

        private double[] readVectorFile(StringReader reader)
        {
            string line = null;
            List<string> str = new List<string>();
            while ((line = reader.ReadLine()) != null)
            {
                str.Add(line);
            }
            char[] sep = new char[] { ',' };
            double[] v = new double[str.Count];
            string[] parts = str[0].Split(sep);
            for (int i = 0; i < str.Count; i++)
            {
                parts = str[i].Split(sep);
                double prtdbl = double.Parse(parts[0], CultureInfo.GetCultureInfo("en-US"));
                v[i] = prtdbl;
            }
#if NETCORE
            reader.Dispose();
#else
            reader.Close();
#endif
            return v;
        }




        [Test]
        public void GoldfarbIdnani4()
        {
            // https://github.com/accord-net/framework/issues/171

            int n = 21;
            var Q = Matrix.Diagonal(n, 2.0);
            var d = Vector.Create(3132.0, 6264, 15660, 18792, 21924, 6264, 18792, 21924, 9396, 3132, 12528, 6264, 9396, 18792, 21924, 9396, 3132, 3132, 6264, 15660, 18792);

            int m = 44;

            double[] b =
            {
                703.999,
                -704.001,
                1267.999,
                -1268.001,
                1565.999,
                -1566.001,
                471.999,
                -472.001,
                1425.999,
                -1426.001,
                -107.001,
                -1164.001,
                -57.001,
                -311.001,
                -1433.001,
                -0.001,
                -0.001,
                -788.001,
                -472.001,
                -850.001,
                -273.001,
                -0.001, -0.001, -0.001, -0.001,
                -0.001, -0.001, -0.001, -0.001, -0.001,
                -0.001, -0.001, -0.001, -0.001, -0.001,
                -0.001, -0.001, -0.001, -0.001, -0.001,
                -0.001, -0.001, -0.001, -0.001
            };

            var A = Matrix.Create(m, n,
                 1.0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                -1, -1, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                 0, 0, 0, 0, -1, -1, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0,
                 0, 0, 0, 0, 0, 0, 0, 0, -1, -1, -1, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0,
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0,
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, -1, 0, 0, 0, 0, 0, 0,
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1,
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, -1, -1, -1, -1, -1,
                 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0,
                 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0,
                 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0,
                -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0,
                 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0,
                 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, 
                 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0,
                 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 
                 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 
                 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 
                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);


            var solver = new GoldfarbIdnani(Q, d, A, b, 0);

            //for (int i = 0; i < solver.ConstraintTolerances.Length; i++)
            //    solver.ConstraintTolerances[i] = 1e-1;

            Assert.IsTrue(solver.Minimize());

            QuadraticObjectiveFunction f = new QuadraticObjectiveFunction(Q, d);
            f.Function(solver.Solution);

            var constraints = LinearConstraintCollection.Create(A, b, 0);

            double[] violation = constraints.Apply(x => x.GetViolation(solver.Solution));
        }

        [Test]
        public void GoldfarbIdnaniMinimizeWithEqualityTest2()
        {
            // This test reproduces Issue #171 at GitHub
            // Solve the following optimization problem:
            //
            //  min f(x) = a² + b² + c² + d² + e² + f² + 10a + 10b + 30c + 20d + 30e + 20f
            // 
            //  s.t.                  a == 4
            //                b + c + d == 5
            //                    e + f == 1
            //                    a + b <= 7
            //                    c + e <= 1
            //                    d + f <= 2
            //                        a >= 0
            //                        b >= 0
            //                        c >= 0
            //                        d >= 0
            //                        e >= 0
            //                        f >= 0

            double[,] A =
            {
                    { 1,0,0,0,0,0,},
                    { 0,1,1,1,0,0,},
                    { 0,0,0,0,1,1,},
                    { -1,-1,0,0,0,0,},
                    { 0,0,-1,0,-1,0,},
                    { 0,0,0,-1,0,-1,},
                    { 1,0,0,0,0,0,},
                    { 0,1,0,0,0,0,},
                    { 0,0,1,0,0,0,},
                    { 0,0,0,1,0,0,},
                    { 0,0,0,0,1,0,},
                    { 0,0,0,0,0,1 },
            };

            double[] b =
            {
                4,5,1,-7,-1,-2,0,0,0,0,0,0
            };

            double[,] Q =
            {
                {  2,  0,  0,  0,  0,  0},
                {  0,  2,  0,  0,  0,  0},
                {  0,  0,  2,  0,  0,  0},
                {  0,  0,  0,  2,  0,  0},
                {  0,  0,  0,  0,  2,  0},
                {  0,  0,  0,  0,  0,  2 },
            };

            double[] d =
            {
                10,10,30,20,30,20
            };

            GoldfarbIdnani target = new GoldfarbIdnani(Q, d, A, b, 3);
            var tolerance = 0.001;
            target.ConstraintTolerances.ApplyInPlace(a => tolerance);

            Assert.IsTrue(target.Minimize());
            double[] solution = target.Solution;

            Assert.AreEqual(4, solution[0], tolerance);
            Assert.AreEqual(3, solution[1], tolerance);
            Assert.AreEqual(0.75, solution[2], tolerance);
            Assert.AreEqual(1.25, solution[3], tolerance);
            Assert.AreEqual(0.25, solution[4], tolerance);
            Assert.AreEqual(0.75, solution[5], tolerance);
        }

        [Test, Ignore("Same problem happens in R")]
        public void GoldfarbIdnaniMinimizeUnfeasible()
        {
            /*
                install.packages('quadprog')
                library('quadprog')
                 A = as.matrix(read.csv('C:\\Projects\\Accord.NET\\framework\\Unit Tests\\Accord.Tests.Math\\Resources\\unfeasible_qp_1\\unf_1_constraintMat.csv', header = FALSE))
                 b = as.matrix(read.csv('C:\\Projects\\Accord.NET\\framework\\Unit Tests\\Accord.Tests.Math\\Resources\\unfeasible_qp_1\\unf_1_constraintValues.csv', header = FALSE))
                 Q = as.matrix(read.csv('C:\\Projects\\Accord.NET\\framework\\Unit Tests\\Accord.Tests.Math\\Resources\\unfeasible_qp_1\\unf_1_quadTerms.csv', header = FALSE))
                 d = as.matrix(read.csv('C:\\Projects\\Accord.NET\\framework\\Unit Tests\\Accord.Tests.Math\\Resources\\unfeasible_qp_1\\unf_1_linTerms.csv', header = FALSE))
                 solve.QP(Q, d, t(A), b)
            */


            var A = CsvReader.FromText(Resources.unf_1_constraintMat, hasHeaders: false).ToMatrix();
            double[] b = CsvReader.FromText(Resources.unf_1_constraintValues, hasHeaders: false).ToMatrix().GetRow(0);

            var Q = CsvReader.FromText(Resources.unf_1_quadTerms, hasHeaders: false).ToMatrix();
            double[] d = CsvReader.FromText(Resources.unf_1_linTerms, hasHeaders: false).ToMatrix().GetRow(0);

            Assert.IsTrue(Q.IsSymmetric());

            GoldfarbIdnani target = new GoldfarbIdnani(Q, d, A, b);
            Assert.AreEqual(12, target.NumberOfConstraints);
            Assert.AreEqual(0, target.NumberOfEqualities);
            Assert.AreEqual(32, target.NumberOfVariables);

            Assert.IsTrue(target.Minimize());
            double[] solution = target.Solution;

            // Check:
            // minimize 1 / 2 * x ^ T D x +d ^ T x
            //       where   A1 x  = b1
            //               A2 x >= b2

            double[] error = solution.Multiply(A).Subtract(b);

            double tolerance = 1e-10;

            Assert.AreEqual(4, solution[0], tolerance);
            Assert.AreEqual(3, solution[1], tolerance);
            Assert.AreEqual(0.75, solution[2], tolerance);
            Assert.AreEqual(1.25, solution[3], tolerance);
            Assert.AreEqual(0.25, solution[4], tolerance);
            Assert.AreEqual(0.75, solution[5], tolerance);
        }
    }
}
