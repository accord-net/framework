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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Math.Differentiation;
    using Accord.Math;


    [TestClass()]
    public class QuadraticObjectiveFunctionTest
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
        public void QuadraticConstructorTest()
        {
            double[,] quadraticTerms = 
            {
                {  1, 2, 3 },
                {  2, 5, 6 },
                {  3, 6, 9 },
            };

            double[] linearTerms = { 1, 2, 3 };

            var target = new QuadraticObjectiveFunction(quadraticTerms, linearTerms);

            var function = target.Function;
            var gradient = target.Gradient;

            FiniteDifferences fd = new FiniteDifferences(3, function);

            double[][] x =
            {
                new double[] { 1, 2, 3 },
                new double[] { 3, 1, 4 },
                new double[] { -6 , 5, 9 },
                new double[] { 31, 25, 246 },
                new double[] { -0.102, 0, 10 },
            };


            { // Function test
                for (int i = 0; i < x.Length; i++)
                {
                    double expected = 0.5 * 
                        (x[i].Multiply(quadraticTerms)).InnerProduct(x[i])
                        + linearTerms.InnerProduct(x[i]);

                    double actual = function(x[i]);

                    Assert.AreEqual(expected, actual, 1e-8);
                }
            }

            { // Gradient test
                for (int i = 0; i < x.Length; i++)
                {
                    double[] expected = fd.Compute(x[i]);
                    double[] actual = gradient(x[i]);

                    for (int j = 0; j < actual.Length; j++)
                        Assert.AreEqual(expected[j], actual[j], 1e-8);
                }
            }
        }

        [TestMethod()]
        public void LinearTest()
        {
            double[,] quadraticTerms = 
            {
                {  0, 0, 0 },
                {  0, 0, 0 },
                {  0, 0, 0 },
            };

            double[] linearTerms = { 1, 2, 3 };

            var target = new QuadraticObjectiveFunction(quadraticTerms, linearTerms);

            var function = target.Function;
            var gradient = target.Gradient;

            FiniteDifferences fd = new FiniteDifferences(3, function);

            double[][] x =
            {
                new double[] { 1, 2, 3 },
                new double[] { 3, 1, 4 },
                new double[] { -6 , 5, 9 },
                new double[] { 31, 25, 246 },
                new double[] { -0.102, 0, 10 },
            };


            { // Function test
                for (int i = 0; i < x.Length; i++)
                {
                    double expected = 0.5 * 
                        (x[i].Multiply(quadraticTerms)).InnerProduct(x[i])
                        + linearTerms.InnerProduct(x[i]);

                    double actual = function(x[i]);

                    Assert.AreEqual(expected, actual, 1e-8);
                }
            }

            { // Gradient test
                for (int i = 0; i < x.Length; i++)
                {
                    double[] expected = fd.Compute(x[i]);
                    double[] actual = gradient(x[i]);

                    for (int j = 0; j < actual.Length; j++)
                        Assert.AreEqual(expected[j], actual[j], 1e-8);
                }
            }
        }

        [TestMethod()]
        public void HomogeneousTest()
        {
            double[,] quadraticTerms = 
            {
                {  1, 3, 1 },
                {  3, 2, 2 },
                {  1, 2, 3 },
            };

            double[] linearTerms = { 0, 0, 0 };

            var target = new QuadraticObjectiveFunction(quadraticTerms, linearTerms);

            var function = target.Function;
            var gradient = target.Gradient;

            FiniteDifferences fd = new FiniteDifferences(3, function);

            double[][] x =
            {
                new double[] { 1, 2, 3 },
                new double[] { 3, 1, 4 },
                new double[] { -6 , 5, 9 },
                new double[] { 31, 25, 246 },
                new double[] { -0.102, 0, 10 },
            };

            { // Gradient test
                for (int i = 0; i < x.Length; i++)
                {
                    double[] expected = fd.Compute(x[i]);
                    double[] actual = gradient(x[i]);

                    for (int j = 0; j < actual.Length; j++)
                        Assert.AreEqual(expected[j], actual[j], 1e-6);
                }
            }
        }

        [TestMethod()]
        public void HomogeneousTest2()
        {
            double[,] quadraticTerms = 
            {
                {  1, 0, 1 },
                {  0, 2, 0 },
                {  1, 0, 1 },
            };

            double[] linearTerms = { 0, 0, 0 };

            var target = new QuadraticObjectiveFunction(quadraticTerms, linearTerms);

            var function = target.Function;
            var gradient = target.Gradient;

            FiniteDifferences fd = new FiniteDifferences(3, function);

            double[][] x =
            {
                new double[] { 1, 2, 3 },
                new double[] { 3, 1, 4 },
                new double[] { -6 , 5, 9 },
                new double[] { 31, 25, 246 },
                new double[] { -0.102, 0, 10 },
            };

            { // Gradient test
                for (int i = 0; i < x.Length; i++)
                {
                    double[] expected = fd.Compute(x[i]);
                    double[] actual = gradient(x[i]);

                    for (int j = 0; j < actual.Length; j++)
                        Assert.AreEqual(expected[j], actual[j], 1e-8);
                }
            }
        }


        [TestMethod()]
        public void FunctionTest()
        {
            double x = 0;
            double y = 0;

            Func<double> expected = () => -2 * x * x + x * y - y * y + 5 * y;
            var actual = new QuadraticObjectiveFunction("-2x² + xy - y² + 5y");

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    x = (i - 5) / 10.0;
                    y = (j - 5) / 10.0;

                    double a = actual.Function(new[] { x, y });
                    double e = expected();

                    Assert.AreEqual(e, a, 1e-10);
                    Assert.IsFalse(Double.IsNaN(a));
                    Assert.IsFalse(Double.IsNaN(e));
                }
            }
        }

    }
}
