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
    using Accord.Math.Optimization;
    using NUnit.Framework;
    using System;
    
    [TestFixture]
    public class ConjugateGradientTest
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
        public void MinimizeTest()
        {
            Func<double[], double> f = BroydenFletcherGoldfarbShannoTest.rosenbrockFunction;
            Func<double[], double[]> g = BroydenFletcherGoldfarbShannoTest.rosenbrockGradient;

            Assert.AreEqual(104, f(new[] { -1.0, 2.0 }));


            int n = 2; // number of variables
            double[] initial = { -1.2, 1 };

            ConjugateGradient cg = new ConjugateGradient(n, f, g);
            cg.Method = ConjugateGradientMethod.FletcherReeves;

            Assert.IsTrue(cg.Minimize(initial));
            double actual = cg.Value;
            double expected = 0;
            Assert.AreEqual(expected, actual, 1e-6);

            double[] result = cg.Solution;

            Assert.AreEqual(127, cg.Evaluations);
            Assert.AreEqual(34, cg.Iterations);
            Assert.AreEqual(1.0, result[0], 1e-3);
            Assert.AreEqual(1.0, result[1], 1e-3);
            Assert.IsFalse(double.IsNaN(result[0]));
            Assert.IsFalse(double.IsNaN(result[1]));

            double y = f(result);
            double[] d = g(result);

            Assert.AreEqual(0.0, y, 1e-6);
            Assert.AreEqual(0.0, d[0], 1e-3);
            Assert.AreEqual(0.0, d[1], 1e-3);

            Assert.IsFalse(double.IsNaN(y));
            Assert.IsFalse(double.IsNaN(d[0]));
            Assert.IsFalse(double.IsNaN(d[1]));
        }

        [Test]
        public void MinimizeTest2()
        {
            Func<double[], double> f = BroydenFletcherGoldfarbShannoTest.rosenbrockFunction;
            Func<double[], double[]> g = BroydenFletcherGoldfarbShannoTest.rosenbrockGradient;

            Assert.AreEqual(104, f(new[] { -1.0, 2.0 }));


            int n = 2; // number of variables
            double[] initial = { -1.2, 1 };

            ConjugateGradient cg = new ConjugateGradient(n, f, g);
            cg.Method = ConjugateGradientMethod.PolakRibiere;

            Assert.IsTrue(cg.Minimize(initial));
            double actual = cg.Value;
            double expected = 0;
            Assert.AreEqual(expected, actual, 1e-6);

            double[] result = cg.Solution;

            Assert.AreEqual(125, cg.Evaluations);
            Assert.AreEqual(32, cg.Iterations);
            Assert.AreEqual(1.0, result[0], 1e-3);
            Assert.AreEqual(1.0, result[1], 1e-3);
            Assert.IsFalse(double.IsNaN(result[0]));
            Assert.IsFalse(double.IsNaN(result[1]));

            double y = f(result);
            double[] d = g(result);

            Assert.AreEqual(0.0, y, 1e-6);
            Assert.AreEqual(0.0, d[0], 1e-3);
            Assert.AreEqual(0.0, d[1], 1e-3);

            Assert.IsFalse(double.IsNaN(y));
            Assert.IsFalse(double.IsNaN(d[0]));
            Assert.IsFalse(double.IsNaN(d[1]));
        }

        [Test]
        public void MinimizeTest3()
        {
            Func<double[], double> f = BroydenFletcherGoldfarbShannoTest.rosenbrockFunction;
            Func<double[], double[]> g = BroydenFletcherGoldfarbShannoTest.rosenbrockGradient;

            Assert.AreEqual(104, f(new[] { -1.0, 2.0 }));


            int n = 2; // number of variables
            double[] initial = { -1.2, 1 };

            ConjugateGradient cg = new ConjugateGradient(n, f, g);
            cg.Method = ConjugateGradientMethod.PositivePolakRibiere;

            Assert.IsTrue(cg.Minimize(initial));
            double actual = cg.Value;
            double expected = 0;
            Assert.AreEqual(expected, actual, 1e-6);

            double[] result = cg.Solution;

            Assert.AreEqual(143, cg.Evaluations);
            Assert.AreEqual(28, cg.Iterations);
            Assert.AreEqual(1.0, result[0], 1e-3);
            Assert.AreEqual(1.0, result[1], 1e-3);
            Assert.IsFalse(double.IsNaN(result[0]));
            Assert.IsFalse(double.IsNaN(result[1]));

            double y = f(result);
            double[] d = g(result);

            Assert.AreEqual(0.0, y, 1e-6);
            Assert.AreEqual(0.0, d[0], 1e-3);
            Assert.AreEqual(0.0, d[1], 1e-3);

            Assert.IsFalse(double.IsNaN(y));
            Assert.IsFalse(double.IsNaN(d[0]));
            Assert.IsFalse(double.IsNaN(d[1]));
        }
    }
}
