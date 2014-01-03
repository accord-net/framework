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

namespace Accord.Tests.Statistics
{
    using System;
    using Accord.Statistics.Links;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class LogitLinkFunctionTest
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
        public void LogitLinkFunctionConstructorTest()
        {
            LogitLinkFunction target = new LogitLinkFunction();
            Assert.AreEqual(0, target.A);
            Assert.AreEqual(1, target.B);

            for (int i = 0; i < 11; i++)
            {
                double x = i / 10.0;
                double y = Math.Log(x) - Math.Log(1 - x);

                Assert.AreEqual(y, target.Function(x), 1e-10);
                Assert.AreEqual(x, target.Inverse(y), 1e-10);
            }
        }

        [TestMethod()]
        public void LogitLinkFunctionConstructorTest1()
        {
            double beta = 3.14;
            double constant = 2.91;

            LogitLinkFunction target = new LogitLinkFunction(beta, constant);
            Assert.AreEqual(constant, target.A);
            Assert.AreEqual(beta, target.B);

            for (int i = 0; i < 11; i++)
            {
                double x = i / 10.0;
                double y = ((Math.Log(x) - Math.Log(1 - x)) - constant) / beta;

                Assert.AreEqual(y, target.Function(x), 1e-10);
                Assert.AreEqual(x, target.Inverse(y), 1e-10);
            }
        }

        [TestMethod()]
        public void DerivativeTest()
        {
            double[] expected =
            {
                0.25, 0.249376, 0.247517, 0.244458, 0.240261, 0.235004,
                0.228784, 0.221713, 0.21391, 0.2055, 0.196612
            };

            LogitLinkFunction target = new LogitLinkFunction();

            for (int i = 0; i < 11; i++)
            {
                double x = i / 10.0;
                double y = target.Inverse(x);

                double d1 = target.Derivative(x);
                double d2 = target.Derivative2(y);

                Assert.AreEqual(expected[i], d1, 1e-6);
                Assert.AreEqual(expected[i], d2, 1e-6);

                Assert.IsFalse(Double.IsNaN(d1));
                Assert.IsFalse(Double.IsNaN(d2));
            }
        }

        [TestMethod()]
        public void DerivativeTest2()
        {
            double beta = 4.2;
            double constant = 0.57;

            double[] expected =
            {
                0.230745, 0.197519, 0.157726, 0.119128, 0.0862579, 
                0.0605722, 0.0416275, 0.0281864, 0.0188941, 0.01258, 0.00833836
            };

            LogitLinkFunction target = new LogitLinkFunction(beta, constant);

            for (int i = 0; i < 11; i++)
            {
                double x = i / 10.0;
                double y = target.Inverse(x);

                double d1 = target.Derivative(x);
                double d2 = target.Derivative2(y);

                Assert.AreEqual(expected[i], d1, 1e-6);
                Assert.AreEqual(expected[i], d2, 1e-6);

                Assert.IsFalse(Double.IsNaN(d1));
                Assert.IsFalse(Double.IsNaN(d2));
            }
        }

    }
}
