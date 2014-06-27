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
    using System;
    using Accord.Math.Integration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Statistics.Distributions.Univariate;
    using AccordTestsMathCpp2;

    [TestClass()]
    public class IntegralTest
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


        private static double function1(double x)
        {
            return 2 + Math.Cos(2 * Math.Sqrt(x));
        }

        [TestMethod()]
        public void TrapezoidalTest()
        {
            double actual;

            actual = Trapezoidal.Integrate(function1, 0, 2, 1);
            Assert.AreEqual(4.0486368718741526, actual, 1e-15);
            Assert.IsFalse(Double.IsNaN(actual));

            actual = Trapezoidal.Integrate(function1, 0, 2, 2);
            Assert.AreEqual(3.6081715993899337, actual, 1e-15);
            Assert.IsFalse(Double.IsNaN(actual));

            actual = Trapezoidal.Integrate(function1, 0, 2, 4);
            Assert.AreEqual(3.4971047822027077, actual, 1e-15);
            Assert.IsFalse(Double.IsNaN(actual));

            actual = Trapezoidal.Integrate(function1, 0, 2, 8);
            Assert.AreEqual(3.4692784302833672, actual, 1e-15);
            Assert.IsFalse(Double.IsNaN(actual));

            actual = Trapezoidal.Integrate(function1, 0, 2, 16);
            Assert.AreEqual(3.4623181105467689, actual, 1e-15);
            Assert.IsFalse(Double.IsNaN(actual));
        }

        private static double function2(double x)
        {
            return 1 / x;
        }

        [TestMethod()]
        public void RombergTest()
        {
            // Example from http://www.mathstat.dal.ca/~tkolokol/classes/1500/romberg.pdf

            double actual;

            actual = Romberg.Integrate(function2, 1, 2, 1);
            Assert.AreEqual(0.75, actual);

            actual = Romberg.Integrate(function2, 1, 2, 2);
            Assert.AreEqual(0.69444444444444431, actual);

            actual = Romberg.Integrate(function2, 1, 2, 4);
            Assert.AreEqual(0.6931474776448322, actual);

            actual = Romberg.Integrate(function2, 1, 2, 8);
            Assert.AreEqual(0.6931471805599444, actual);

            actual = Romberg.Integrate(function2, 1, 2, 16);
            Assert.AreEqual(0.69314718055994151, actual);
        }


        public static double function3(double x)
        {
            return x * Math.Exp(-(x * x));
        }

        [TestMethod()]
        public void GaussKronrodTest()
        {
            double expected = Math.Sin(2);
            double actual = NonAdaptiveGaussKronrod.Integrate(Math.Cos, 0, 2);

            Assert.AreEqual(expected, actual, 1e-6);
        }

        [TestMethod()]
        public void InfiniteGaussKronrodTest()
        {
            NormalDistribution norm;
            
            for (int i = -10; i < 10; i++)
            {
                norm = new NormalDistribution(i, 1);

                Func<double, double> E = (x) => x * norm.ProbabilityDensityFunction(x);
                UFunction UE = (x) => x * norm.ProbabilityDensityFunction(x);

                double expected = Quadpack.Integrate(UE, 
                    Double.NegativeInfinity, Double.PositiveInfinity);

                double actual = InfiniteAdaptiveGaussKronrod.Integrate(E,
                    Double.NegativeInfinity, Double.PositiveInfinity);

                Assert.AreEqual(expected, actual, 1e-3);
                Assert.AreEqual(norm.Mean, actual, 1e-3);
            }
        }

    }
}
