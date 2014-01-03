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
    using Accord.Math.Optimization;
    using Accord.Math;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Math.Differentiation;
    using System;

    [TestClass()]
    public class FiniteDifferencesTest
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
        public void ComputeTest()
        {
            int numberOfParameters = 2;
            FiniteDifferences target = new FiniteDifferences(numberOfParameters);

            double[] inputs = { -1, 0.4 };

            target.Function = BroydenFletcherGoldfarbShannoTest.rosenbrockFunction;

            double[] expected = BroydenFletcherGoldfarbShannoTest.rosenbrockGradient(inputs);
            double[] actual = target.Compute(inputs);

            Assert.IsTrue(expected.IsEqual(actual, 0.05));
        }

        [TestMethod()]
        public void ComputeTest2()
        {
            // Create a simple function with two parameters: f(x,y) = x² + y
            Func<double[], double> function = x => Math.Pow(x[0], 2) + x[1];

            // The gradient w.r.t to x should be 2x,
            // the gradient w.r.t to y should be  1


            // Create a new finite differences calculator
            var calculator = new FiniteDifferences(2, function);

            // Evaluate the gradient function at the point (2, -1)
            double[] result = calculator.Compute(2, -1); // answer is (4, 1)

            Assert.AreEqual(4, result[0], 1e-10);
            Assert.AreEqual(1, result[1], 1e-10);
            Assert.IsFalse(Double.IsNaN(result[0]));
            Assert.IsFalse(Double.IsNaN(result[1]));
        }

    }
}
