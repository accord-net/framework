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
    using Accord.Math.Optimization;
    using NUnit.Framework;

    [TestFixture]
    public class CobylaTest
    {

        [Test]
        public void ConstructorTest1()
        {
            Func<double[], double> function = // min f(x) = 10 * (x+1)^2 + y^2
            x => 10.0 * Math.Pow(x[0] + 1.0, 2.0) + Math.Pow(x[1], 2.0);

            Cobyla cobyla = new Cobyla(2, function);

            Assert.IsTrue(cobyla.Minimize());
            double minimum = cobyla.Value;

            double[] solution = cobyla.Solution;

            Assert.AreEqual(0, minimum, 1e-10);
            Assert.AreEqual(-1, solution[0], 1e-5);
            Assert.AreEqual(0, solution[1], 1e-5);

            double expectedMinimum = function(cobyla.Solution);
            Assert.AreEqual(expectedMinimum, minimum);
        }

        [Test]
        public void ConstructorTest2()
        {
            var function = new NonlinearObjectiveFunction(2, x => x[0] * x[1]);

            NonlinearConstraint[] constraints =
            {
                new NonlinearConstraint(function, x => 1.0 - x[0] * x[0] - x[1] * x[1])
            };

            Cobyla cobyla = new Cobyla(function, constraints);

            for (int i = 0; i < cobyla.Solution.Length; i++)
                cobyla.Solution[i] = 1;

            bool success = cobyla.Minimize();

            double violation = constraints[0].GetViolation(cobyla.Solution);

            var status = cobyla.Status;


            Assert.IsTrue(success);
            double minimum = cobyla.Value;

            double[] solution = cobyla.Solution;

            double sqrthalf = Math.Sqrt(0.5);

            Assert.AreEqual(-0.5, minimum, 1e-10);
            Assert.AreEqual(sqrthalf, solution[0], 1e-5);
            Assert.AreEqual(-sqrthalf, solution[1], 1e-5);

            double expectedMinimum = function.Function(cobyla.Solution);
            Assert.AreEqual(expectedMinimum, minimum);
        }

        [Test]
        public void ConstructorTest2_1()
        {
            var function = new NonlinearObjectiveFunction(2, x => x[0] * x[1]);

            NonlinearConstraint[] constraints =
            {
                new NonlinearConstraint(function, x =>  x[0] * x[0] + x[1] * x[1],
                    ConstraintType.LesserThanOrEqualTo, 1.0)
            };

            Cobyla cobyla = new Cobyla(function, constraints);

            for (int i = 0; i < cobyla.Solution.Length; i++)
                cobyla.Solution[i] = 1;

            Assert.IsTrue(cobyla.Minimize());
            double minimum = cobyla.Value;

            double[] solution = cobyla.Solution;

            double sqrthalf = Math.Sqrt(0.5);

            Assert.AreEqual(-0.5, minimum, 1e-10);
            Assert.AreEqual(sqrthalf, solution[0], 1e-5);
            Assert.AreEqual(-sqrthalf, solution[1], 1e-5);

            double expectedMinimum = function.Function(cobyla.Solution);
            Assert.AreEqual(expectedMinimum, minimum);
        }

        [Test]
        public void ConstructorTest2_2()
        {
            var function = new NonlinearObjectiveFunction(2, x => x[0] * x[1]);

            NonlinearConstraint[] constraints =
            {
                new NonlinearConstraint(2, x =>  x[0] * x[0] + x[1] * x[1] <= 1.0)
            };

            test_body(function, constraints);
        }

        private static void test_body(NonlinearObjectiveFunction function, NonlinearConstraint[] constraints)
        {
            Cobyla cobyla = new Cobyla(function, constraints);

            for (int i = 0; i < cobyla.Solution.Length; i++)
                cobyla.Solution[i] = 1;

            bool success = cobyla.Minimize();
            double minimum = cobyla.Value;
            Assert.IsTrue(success);

            double[] solution = cobyla.Solution;

            double sqrthalf = Math.Sqrt(0.5);

            Assert.AreEqual(-0.5, minimum, 1e-10);
            Assert.AreEqual(sqrthalf, solution[0], 1e-5);
            Assert.AreEqual(-sqrthalf, solution[1], 1e-5);
        }

        [Test]
        public void cobyla_should_accept_variable_values_gh688()
        {
            // https://github.com/accord-net/framework/issues/688
            var function = new NonlinearObjectiveFunction(2, x => x[0] * x[1]);
            double a = 1.0;

            NonlinearConstraint[] constraints =
            {
                new NonlinearConstraint(2, x =>  x[0] * x[0] + x[1] * x[1] <= a)
            };

            test_body(function, constraints);
        }

        private double propertyTest = 1;

        public double PropertyTest
        {
            get { return propertyTest; }
            set { propertyTest = value; }
        }

        [Test]
        public void cobyla_should_accept_property_values_gh688()
        {
            // https://github.com/accord-net/framework/issues/688
            var function = new NonlinearObjectiveFunction(2, x => x[0] * x[1]);

            NonlinearConstraint[] constraints =
            {
                new NonlinearConstraint(2, x =>  x[0] * x[0] + x[1] * x[1] <= PropertyTest)
            };

            test_body(function, constraints);
        }


        [Test]
        public void cobyla_should_accept_parameter_values_gh688()
        {
            // https://github.com/accord-net/framework/issues/688
            var function = new NonlinearObjectiveFunction(2, x => x[0] * x[1]);
            double a = 1.0;

            NonlinearConstraint[] constraints = create_constraints(a);

            test_body(function, constraints);
        }

        [Test]
        public void cobyla_should_not_accept_lambdas()
        {
            // https://github.com/accord-net/framework/issues/688
            var function = new NonlinearObjectiveFunction(2, x => x[0] * x[1]);

            Func<double> func = () => 1.0;

            Assert.Throws<ArgumentException>(() => new NonlinearConstraint(2, x => x[0] * x[0] + x[1] * x[1] <= func()));
        }

        private static NonlinearConstraint[] create_constraints(double a)
        {
            return new[]
            {
                new NonlinearConstraint(2, x => x[0] * x[0] + x[1] * x[1] <= a)
            };
        }

        [Test]
        public void ConstructorTest3()
        {
            // Easy three dimensional minimization in ellipsoid.
            var function = new NonlinearObjectiveFunction(3, x => x[0] * x[1] * x[2]);

            NonlinearConstraint[] constraints =
            {
                new NonlinearConstraint(3, x =>  1.0 - x[0] * x[0] - 2.0 * x[1] * x[1] - 3.0 * x[2] * x[2])
            };

            Cobyla cobyla = new Cobyla(function, constraints);

            for (int i = 0; i < cobyla.Solution.Length; i++)
                cobyla.Solution[i] = 1;

            Assert.IsTrue(cobyla.Minimize());
            double minimum = cobyla.Value;
            double[] solution = cobyla.Solution;

            double sqrthalf = Math.Sqrt(0.5);

            double[] expected =
            {
                1.0 / Math.Sqrt(3.0), 1.0 / Math.Sqrt(6.0), -1.0 / 3.0
            };


            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], cobyla.Solution[i], 1e-4);
            Assert.AreEqual(-0.078567420132031968, minimum, 1e-10);

            double expectedMinimum = function.Function(cobyla.Solution);
            Assert.AreEqual(expectedMinimum, minimum);
        }

        [Test]
        public void ConstructorTest4()
        {
            // Weak version of Rosenbrock's problem.
            var function = new NonlinearObjectiveFunction(2, x =>
                Math.Pow(x[0] * x[0] - x[1], 2.0) + Math.Pow(1.0 + x[0], 2.0));

            Cobyla cobyla = new Cobyla(function);

            Assert.IsTrue(cobyla.Minimize());
            double minimum = cobyla.Value;
            double[] solution = cobyla.Solution;

            Assert.AreEqual(0, minimum, 1e-8);
            Assert.AreEqual(-1, solution[0], 1e-5);
            Assert.AreEqual(1, solution[1], 1e-4);

            double expectedMinimum = function.Function(cobyla.Solution);
            Assert.AreEqual(expectedMinimum, minimum);
        }

        [Test]
        public void ConstructorTest5()
        {
            // Intermediate version of Rosenbrock's problem.
            var function = new NonlinearObjectiveFunction(2, x =>
                10.0 * Math.Pow(x[0] * x[0] - x[1], 2.0) + Math.Pow(1.0 + x[0], 2.0));

            Cobyla cobyla = new Cobyla(function);

            Assert.IsTrue(cobyla.Minimize());
            double minimum = cobyla.Value;
            double[] solution = cobyla.Solution;

            Assert.AreEqual(-0, minimum, 1e-6);
            Assert.AreEqual(-1, solution[0], 1e-3);
            Assert.AreEqual(+1, solution[1], 1e-3);

            double expectedMinimum = function.Function(cobyla.Solution);
            Assert.AreEqual(expectedMinimum, minimum);
        }

        [Test]
        public void ConstructorTest6()
        {
            // This problem is taken from Fletcher's book Practical Methods of
            // Optimization and has the equation number (9.1.15).
            var function = new NonlinearObjectiveFunction(2, x => -x[0] - x[1]);

            NonlinearConstraint[] constraints =
            {
                new NonlinearConstraint(2, x =>  x[1] - x[0] * x[0]),
                new NonlinearConstraint(2, x =>  1.0 - x[0] * x[0] - x[1] * x[1]),
            };

            Cobyla cobyla = new Cobyla(function, constraints);

            Assert.IsTrue(cobyla.Minimize());
            double minimum = cobyla.Value;
            double[] solution = cobyla.Solution;

            double sqrthalf = Math.Sqrt(0.5);
            Assert.AreEqual(-sqrthalf * 2, minimum, 1e-10);
            Assert.AreEqual(sqrthalf, solution[0], 1e-5);
            Assert.AreEqual(sqrthalf, solution[1], 1e-5);

            double expectedMinimum = function.Function(cobyla.Solution);
            Assert.AreEqual(expectedMinimum, minimum);
        }

        [Test]
        public void ConstructorTest6_0()
        {
            // This problem is taken from Fletcher's book Practical Methods of
            // Optimization and has the equation number (9.1.15).
            var function = new NonlinearObjectiveFunction(2, x => -x[0] - x[1]);

            NonlinearConstraint[] constraints =
            {
                new NonlinearConstraint(2, x =>  x[1] - x[0] * x[0] >= 0),
                new NonlinearConstraint(2, x =>  1.0 - x[0] * x[0] - x[1] * x[1] >= 0),
            };

            Cobyla cobyla = new Cobyla(function, constraints);

            bool success = cobyla.Minimize();
            double minimum = cobyla.Value;
            double[] solution = cobyla.Solution;

            Assert.IsTrue(success);

            double sqrthalf = Math.Sqrt(0.5);
            Assert.AreEqual(-sqrthalf * 2, minimum, 1e-10);
            Assert.AreEqual(sqrthalf, solution[0], 1e-5);
            Assert.AreEqual(sqrthalf, solution[1], 1e-5);

            double expectedMinimum = function.Function(cobyla.Solution);
            Assert.AreEqual(expectedMinimum, minimum);
        }

        [Test]
        public void ConstructorTest6_1()
        {
            /// This problem is taken from Fletcher's book Practical Methods of
            /// Optimization and has the equation number (9.1.15).
            var function = new NonlinearObjectiveFunction(2, x => -x[0] - x[1]);

            NonlinearConstraint[] constraints =
            {
                new NonlinearConstraint(2, x =>  x[1] - x[0] * x[0] >= 0),
                new NonlinearConstraint(2, x =>  -x[0] * x[0] - x[1] * x[1] >= -1.0),
            };

            Cobyla cobyla = new Cobyla(function, constraints);

            Assert.IsTrue(cobyla.Minimize());
            double minimum = cobyla.Value;
            double[] solution = cobyla.Solution;

            double sqrthalf = Math.Sqrt(0.5);
            Assert.AreEqual(-sqrthalf * 2, minimum, 1e-10);
            Assert.AreEqual(sqrthalf, solution[0], 1e-5);
            Assert.AreEqual(sqrthalf, solution[1], 1e-5);

            double expectedMinimum = function.Function(cobyla.Solution);
            Assert.AreEqual(expectedMinimum, minimum);
        }

        [Test]
        public void ConstructorTest6_2()
        {
            /// This problem is taken from Fletcher's book Practical Methods of
            /// Optimization and has the equation number (9.1.15).
            var function = new NonlinearObjectiveFunction(2, x => -x[0] - x[1]);

            NonlinearConstraint[] constraints =
            {
                new NonlinearConstraint(2, x =>  -(x[1] - x[0] * x[0]) <= 0),
                new NonlinearConstraint(2, x =>  -(-x[0] * x[0] - x[1] * x[1]) <= 1.0),
            };

            Cobyla cobyla = new Cobyla(function, constraints);

            Assert.IsTrue(cobyla.Minimize());
            double minimum = cobyla.Value;
            double[] solution = cobyla.Solution;

            double sqrthalf = Math.Sqrt(0.5);
            Assert.AreEqual(-sqrthalf * 2, minimum, 1e-10);
            Assert.AreEqual(sqrthalf, solution[0], 1e-5);
            Assert.AreEqual(sqrthalf, solution[1], 1e-5);

            double expectedMinimum = function.Function(cobyla.Solution);
            Assert.AreEqual(expectedMinimum, minimum);
        }

        [Test]
        public void ConstructorTest6_3()
        {
            bool thrown = false;

            try
            {
                var function = new NonlinearObjectiveFunction(2, x => -x[0] - x[1]);

                NonlinearConstraint[] constraints =
                {
                    new NonlinearConstraint(2, x =>  x[1] - x[0] * x[0]),
                    new NonlinearConstraint(4, x =>  1.0 - x[0] * x[0] - x[1] * x[1]),
                };

                Cobyla cobyla = new Cobyla(function, constraints);

                Assert.IsTrue(cobyla.Minimize());
                double minimum = cobyla.Value;
            }
            catch (Exception)
            {
                thrown = true;
            }

            Assert.IsTrue(thrown);
        }

        [Test]
        public void ConstructorTest7()
        {
            Accord.Math.Random.Generator.Seed = 0;

            /// This problem is taken from Fletcher's book Practical Methods of
            /// Optimization and has the equation number (14.4.2).
            var function = new NonlinearObjectiveFunction(3, x => x[2]);

            NonlinearConstraint[] constraints =
            {
                new NonlinearConstraint(3, x=> 5.0 * x[0] - x[1] + x[2]),
                new NonlinearConstraint(3, x =>  x[2] - x[0] * x[0] - x[1] * x[1] - 4.0 * x[1]),
                new NonlinearConstraint(3, x =>  x[2] - 5.0 * x[0] - x[1]),
            };

            Cobyla cobyla = new Cobyla(function, constraints);

            Assert.IsTrue(cobyla.Minimize());
            double minimum = cobyla.Value;
            double[] solution = cobyla.Solution;

            Assert.AreEqual(-3, minimum, 1e-5);
            Assert.AreEqual(0.0, solution[0], 1e-5);
            Assert.AreEqual(-3.0, solution[1], 1e-5);
            Assert.AreEqual(-3.0, solution[2], 1e-5);

            double expectedMinimum = function.Function(cobyla.Solution);
            Assert.AreEqual(expectedMinimum, minimum);

            Assert.IsTrue(Accord.Math.Random.Generator.HasBeenAccessed);
        }

        [Test]
        public void ConstructorTest8()
        {
            /// This problem is taken from page 66 of Hock and Schittkowski's book Test
            /// Examples for Nonlinear Programming Codes. It is their test problem Number
            /// 43, and has the name Rosen-Suzuki.
            var function = new NonlinearObjectiveFunction(4, x => x[0] * x[0]
                + x[1] * x[1] + 2.0 * x[2] * x[2]
                + x[3] * x[3] - 5.0 * x[0] - 5.0 * x[1]
                - 21.0 * x[2] + 7.0 * x[3]);

            NonlinearConstraint[] constraints =
            {
                new NonlinearConstraint(4, x=> 8.0 - x[0] * x[0]
                    - x[1] * x[1] - x[2] * x[2] - x[3] * x[3] - x[0] + x[1] - x[2] + x[3]),

                new NonlinearConstraint(4, x => 10.0 - x[0] * x[0]
                    - 2.0 * x[1] * x[1] - x[2] * x[2] - 2.0 * x[3] * x[3] + x[0] + x[3]),

                new NonlinearConstraint(4, x => 5.0 - 2.0 * x[0] * x[0]
                    - x[1] * x[1] - x[2] * x[2] - 2.0 * x[0] + x[1] + x[3])
            };

            Cobyla cobyla = new Cobyla(function, constraints);

            Assert.IsTrue(cobyla.Minimize());
            double minimum = cobyla.Value;
            double[] solution = cobyla.Solution;

            double[] expected =
            {
                0.0, 1.0, 2.0, -1.0
            };

            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], cobyla.Solution[i], 1e-4);
            Assert.AreEqual(-44, minimum, 1e-10);

            double expectedMinimum = function.Function(cobyla.Solution);
            Assert.AreEqual(expectedMinimum, minimum);
        }

        [Test]
        public void ConstructorTest9()
        {
            /// This problem is taken from page 111 of Hock and Schittkowski's
            /// book Test Examples for Nonlinear Programming Codes. It is their
            /// test problem Number 100.
            /// 
            var function = new NonlinearObjectiveFunction(7, x =>
                Math.Pow(x[0] - 10.0, 2.0) + 5.0 * Math.Pow(x[1] - 12.0, 2.0) + Math.Pow(x[2], 4.0) +
                3.0 * Math.Pow(x[3] - 11.0, 2.0) + 10.0 * Math.Pow(x[4], 6.0) + 7.0 * x[5] * x[5] + Math.Pow(x[6], 4.0) -
                4.0 * x[5] * x[6] - 10.0 * x[5] - 8.0 * x[6]);

            NonlinearConstraint[] constraints =
            {
                new NonlinearConstraint(7, x => 127.0 - 2.0 * x[0] * x[0] - 3.0 * Math.Pow(x[1], 4.0)
                    - x[2] - 4.0 * x[3] * x[3] - 5.0 * x[4]),
                new NonlinearConstraint(7, x => 282.0 - 7.0 * x[0] - 3.0 * x[1] - 10.0 * x[2] * x[2] - x[3] + x[4]),
                new NonlinearConstraint(7, x => 196.0 - 23.0 * x[0] - x[1] * x[1] - 6.0 * x[5] * x[5] + 8.0 * x[6]),
                new NonlinearConstraint(7, x => -4.0 * x[0] * x[0] - x[1] * x[1] + 3.0 * x[0] * x[1]
                    - 2.0 * x[2] * x[2] - 5.0 * x[5] + 11.0 * x[6])
            };

            Cobyla cobyla = new Cobyla(function, constraints);

            Assert.IsTrue(cobyla.Minimize());
            double minimum = cobyla.Value;
            double[] solution = cobyla.Solution;

            double[] expected =
            {
                2.330499, 1.951372, -0.4775414, 4.365726, -0.624487, 1.038131, 1.594227
            };

            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], cobyla.Solution[i], 1e-4);
            Assert.AreEqual(680.63005737443393, minimum, 1e-6);

            double expectedMinimum = function.Function(cobyla.Solution);
            Assert.AreEqual(expectedMinimum, minimum);
        }

        [Test]
        public void ConstructorTest10()
        {
            /// This problem is taken from page 415 of Luenberger's book Applied
            /// Nonlinear Programming. It is to maximize the area of a hexagon of
            /// unit diameter.
            /// 
            var function = new NonlinearObjectiveFunction(9, x =>
                -0.5 * (x[0] * x[3] - x[1] * x[2] + x[2] * x[8]
                - x[4] * x[8] + x[4] * x[7] - x[5] * x[6]));

            NonlinearConstraint[] constraints =
            {
                new NonlinearConstraint(9, x => 1.0 - x[2] * x[2] - x[3] * x[3]),
                new NonlinearConstraint(9, x =>  1.0 - x[8] * x[8]),
                new NonlinearConstraint(9, x =>  1.0 - x[4] * x[4] - x[5] * x[5]),
                new NonlinearConstraint(9, x =>  1.0 - x[0] * x[0] - Math.Pow(x[1] - x[8], 2.0)),
                new NonlinearConstraint(9, x =>  1.0 - Math.Pow(x[0] - x[4], 2.0) - Math.Pow(x[1] - x[5], 2.0)),
                new NonlinearConstraint(9, x =>  1.0 - Math.Pow(x[0] - x[6], 2.0) - Math.Pow(x[1] - x[7], 2.0)),
                new NonlinearConstraint(9, x =>  1.0 - Math.Pow(x[2] - x[4], 2.0) - Math.Pow(x[3] - x[5], 2.0)),
                new NonlinearConstraint(9, x =>  1.0 - Math.Pow(x[2] - x[6], 2.0) - Math.Pow(x[3] - x[7], 2.0)),
                new NonlinearConstraint(9, x =>  1.0 - x[6] * x[6] - Math.Pow(x[7] - x[8], 2.0)),
                new NonlinearConstraint(9, x =>  x[0] * x[3] - x[1] * x[2]),
                new NonlinearConstraint(9, x =>  x[2] * x[8]),
                new NonlinearConstraint(9, x =>  -x[4] * x[8]),
                new NonlinearConstraint(9, x =>  x[4] * x[7] - x[5] * x[6]),
                new NonlinearConstraint(9, x =>  x[8]),
            };

            Cobyla cobyla = new Cobyla(function, constraints);

            for (int i = 0; i < cobyla.Solution.Length; i++)
                cobyla.Solution[i] = 1;

            Assert.IsTrue(cobyla.Minimize());
            double minimum = cobyla.Value;
            double[] solution = cobyla.Solution;

            double[] expected =
            {
                0.688341, 0.725387, -0.284033, 0.958814, 0.688341, 0.725387, -0.284033, 0.958814, 0.0
            };

            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], cobyla.Solution[i], 1e-2);
            Assert.AreEqual(-0.86602540378486847, minimum, 1e-10);

            double expectedMinimum = function.Function(cobyla.Solution);
            Assert.AreEqual(expectedMinimum, minimum);
        }

    }
}
