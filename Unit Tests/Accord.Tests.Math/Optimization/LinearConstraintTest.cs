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
    using Accord.Math;

    [TestFixture]
    public class LinearConstraintTest
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

        [Test]
        public void DocumentationTest()
        {
            #region doc_example
            // Linear constraints are common in numerical optimization.
            // Constraints can be defined using strings, expressions or
            // vectors. Suppose we have a quadratic objective function:
            var f = new QuadraticObjectiveFunction("2x² + 4y² - 2xy + 6");

            // Then the following three are all equivalent:
            var lc1 = new LinearConstraint(f, "3*x + 5*y <= 7");

            double x = 0, y = 0; // Define some dummy variables
            var lc2 = new LinearConstraint(f, () => 3*x + 5*y <= 7);

            var lc3 = new LinearConstraint(numberOfVariables: 2)
            {
                CombinedAs = new double[] { 3, 5 },
                ShouldBe = ConstraintType.LesserThanOrEqualTo,
                Value = 7
            };

            // Then, we can check whether a constraint is violated and, if so,
            // by how much.
            double[] vector = { -2, 3 };

            if (lc1.IsViolated(vector))
            {
                // act on violation...
            }

            double violation = lc2.GetViolation(vector); // negative if violated

            #endregion

            double expected = -2;
            double v1 = lc1.GetViolation(vector);
            double v2 = lc2.GetViolation(vector);
            double v3 = lc3.GetViolation(vector);

            Assert.AreEqual(expected, v1);
            Assert.AreEqual(expected, v2);
            Assert.AreEqual(expected, v3);
        }

        [Test]
        public void ConstructorTest1()
        {
            var f = new QuadraticObjectiveFunction("a + b = 0");

            var constraints1 = new[]
            {
                new LinearConstraint(f, "0.0732 * a + 0.0799 * b = 0.098"),
                new LinearConstraint(f, "a + b = 1"),
                new LinearConstraint(f, "a >= 0"),
                new LinearConstraint(f, "b >= 0"),
                new LinearConstraint(f, "a >= 0.5")
            };

            var constraints2 = new[]
            {
                new LinearConstraint(f, "0.0732 * a + 0.0799 * b - 0.098 = 0"),
                new LinearConstraint(f, "a + b -2 = -1"),
                new LinearConstraint(f, "-a <= 0"),
                new LinearConstraint(f, "-b <= 0"),
                new LinearConstraint(f, "-a + 0.5 <= 0")
            };

            for (int i = 0; i < constraints1.Length; i++)
            {
                var c1 = constraints1[i];
                var c2 = constraints2[i];

                for (double a = -10; a < 10; a += 0.1)
                {
                    for (double b = -10; b < 10; b += 0.1)
                    {
                        double[] x = { a, b };
                        double actual = c1.GetViolation(x);
                        double expected = c2.GetViolation(x);
                        Assert.AreEqual(expected, actual);
                    }
                }
            }
        }

        [Test]
        public void ConstructorTest2()
        {
            double a = 0, b = 0;

            var f = new QuadraticObjectiveFunction(() => a + b);

            Assert.AreEqual(2, f.NumberOfVariables);
            Assert.AreEqual(0, f.Variables["a"]);
            Assert.AreEqual(1, f.Variables["b"]);
            Assert.AreEqual(1, f.LinearTerms[0]);
            Assert.AreEqual(1, f.LinearTerms[1]);

            var constraints1 = new[]
            {
                new LinearConstraint(f, () => 0.0732 * a + 0.0799 * b == 0.098),
                new LinearConstraint(f, () => a + b == 1),
                new LinearConstraint(f, () => a >= 0),
                new LinearConstraint(f, () => b >= 0),
                new LinearConstraint(f, () => a >= 0.5),
                new LinearConstraint(f, () => 1 + a >= -5),
                new LinearConstraint(f, () => -1 + a <= -5)
            };

            var constraints2 = new[]
            {
                new LinearConstraint(f, () => 0.0732 * a + 0.0799 * b - 0.098 == 0),
                new LinearConstraint(f, () => a + b -2 == -1),
                new LinearConstraint(f, () => -a + 1 <= +1),
                new LinearConstraint(f, () => -b <= 0),
                new LinearConstraint(f, () => -a + 0.5 <= 0),
                new LinearConstraint(f, () => a + 1 >= -5),
                new LinearConstraint(f, () => a - 1 <= -5)
            };

            Assert.AreEqual(0.098, constraints1[0].Value);
            Assert.AreEqual(0.098, constraints2[0].Value);

            Assert.AreEqual(0, constraints1[2].Value);
            Assert.AreEqual(0, constraints2[2].Value);

            Assert.AreEqual(1, constraints1[1].Value);
            Assert.AreEqual(1, constraints2[1].Value);

            for (int i = 0; i < constraints1.Length; i++)
            {
                var c1 = constraints1[i];
                var c2 = constraints2[i];

                for (a = -10; a < 10; a += 0.1)
                {
                    for (b = -10; b < 10; b += 0.1)
                    {
                        double[] x = { a, b };
                        double actual = c1.GetViolation(x);
                        double expected = c2.GetViolation(x);
                        Assert.AreEqual(expected, actual);
                    }
                }
            }
        }

        [Test]
        public void TestGradientWithIndices()
        {
            // Arrange1
            int[] indices1 = { 2, 4, 6 };
            int[] indices2 = { 0, 4, 6 };
            double[] combinedAs1 = { 2, 3, 4 };
            double[] combinedAs2 = { 9, 5, 1 };
            double[] x = Vector.Random(8);
            double[] expected1 = { 0, 0, 2, 0, 3, 0, 4, 0 };
            double[] expected2 = { 0, 0, 9, 0, 5, 0, 1, 0 };
            double[] expected3 = { 9, 0, 0, 0, 5, 0, 1, 0 };

            var linearConstraint = new LinearConstraint(indices1.Length)
            {
                CombinedAs = combinedAs1,
                VariablesAtIndices = indices1,
                ShouldBe = ConstraintType.EqualTo,
                Value = 42
            };

            // Act1
            double[] gradient = linearConstraint.Gradient(x);

            // Assert1
            Assert.True(gradient.IsEqual(expected1));

            // Arrange2
            linearConstraint.CombinedAs = combinedAs2;

            // Act2
            double[] gradient2 = linearConstraint.Gradient(x);

            // Assert2
            Assert.True(gradient2.IsEqual(expected2));

            // Arrange3
            linearConstraint.VariablesAtIndices = indices2;

            // Act3
            double[] gradient3 = linearConstraint.Gradient(x);

            // Assert3
            Assert.True(gradient3.IsEqual(expected3));
        }

        [Test]
        public void TestGradientWithoutIndices()
        {
            // Arrange1
            double[] combinedAs1 = { 2, 3, 4, 5, 6 };
            double[] combinedAs2 = { 9, 5, 1, 5, 8 };
            double[] x = Vector.Random(5);

            var linearConstraint = new LinearConstraint(combinedAs1)
            {
                ShouldBe = ConstraintType.EqualTo,
                Value = 42
            };

            // Act1
            double[] gradient = linearConstraint.Gradient(x);

            // Assert1
            Assert.True(gradient.IsEqual(combinedAs1));

            // Arrange2
            linearConstraint.CombinedAs = combinedAs2;

            // Act2
            double[] gradient2 = linearConstraint.Gradient(x);

            // Assert2
            Assert.True(gradient2.IsEqual(combinedAs2));
        }

    }
}
