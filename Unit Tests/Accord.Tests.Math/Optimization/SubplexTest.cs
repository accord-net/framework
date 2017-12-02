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
    public class SubplexTest
    {

        [Test]
        public void ConstructorTest1()
        {
            Func<double[], double> function = // min f(x) = 10 * (x+1)^2 + y^2
              x => 10.0 * Math.Pow(x[0] + 1.0, 2.0) + Math.Pow(x[1], 2.0);

            Subplex solver = new Subplex(2, function);

            Assert.IsTrue(solver.Minimize());
            double minimum = solver.Value;

            double[] solution = solver.Solution;

            Assert.AreEqual(0, minimum, 1e-10);
            Assert.AreEqual(-1, solution[0], 1e-5);
            Assert.AreEqual(0, solution[1], 1e-5);

            double expectedMinimum = function(solver.Solution);
            Assert.AreEqual(expectedMinimum, minimum);
        }

        [Test]
        public void ConstructorTest4()
        {
            // Weak version of Rosenbrock's problem.
            var function = new NonlinearObjectiveFunction(2, x =>
                Math.Pow(x[0] * x[0] - x[1], 2.0) + Math.Pow(1.0 + x[0], 2.0));

            Subplex solver = new Subplex(function);

            Assert.IsTrue(solver.Minimize());
            double minimum = solver.Value;
            double[] solution = solver.Solution;

            Assert.AreEqual(2, solution.Length);
            Assert.AreEqual(0, minimum, 1e-10);
            Assert.AreEqual(-1, solution[0], 1e-5);
            Assert.AreEqual(1, solution[1], 1e-4);

            double expectedMinimum = function.Function(solver.Solution);
            Assert.AreEqual(expectedMinimum, minimum);
        }

        [Test]
        public void ConstructorTest5()
        {
            var function = new NonlinearObjectiveFunction(2, x =>
                10.0 * Math.Pow(x[0] * x[0] - x[1], 2.0) + Math.Pow(1.0 + x[0], 2.0));

            Subplex solver = new Subplex(function);

            Assert.IsTrue(solver.Minimize());
            double minimum = solver.Value;
            double[] solution = solver.Solution;

            Assert.AreEqual(-0, minimum, 1e-6);
            Assert.AreEqual(-1, solution[0], 1e-3);
            Assert.AreEqual(+1, solution[1], 1e-3);

            double expectedMinimum = function.Function(solver.Solution);
            Assert.AreEqual(expectedMinimum, minimum);
        }

    }
}
