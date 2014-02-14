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
    public class LogLogLinkFunctionTest
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
        public void LogLogLinkFunctionConstructorTest2()
        {
            LogLogLinkFunction target = new LogLogLinkFunction();

            Assert.AreEqual(1, target.B);
            Assert.AreEqual(0, target.A);

            for (int i = 0; i < 11; i++)
            {
                double x = i / 10.0;
                double y = Math.Log(-Math.Log(x));

                Assert.AreEqual(y, target.Function(x), 1e-10);
                Assert.AreEqual(x, target.Inverse(y), 1e-10);
            }
        }

        [TestMethod()]
        public void LogLogLinkFunctionConstructorTest()
        {
            double beta = 3.14;
            double constant = 2.91;
            LogLogLinkFunction target = new LogLogLinkFunction(beta, constant);

            Assert.AreEqual(beta, target.B);
            Assert.AreEqual(constant, target.A);

            for (int i = 0; i < 11; i++)
            {
                double x = i / 10.0;
                double y = (Math.Log(-Math.Log(x)) - constant) / beta;

                Assert.AreEqual(y, target.Function(x), 1e-10);
                Assert.AreEqual(x, target.Inverse(y), 1e-10);
            }
        }

        [TestMethod()]
        public void DerivativeTest()
        {
            LogLogLinkFunction target = new LogLogLinkFunction();

            double[] expected =
            {
                -0.367879, -0.365982, -0.360089, -0.349987, -0.335604, -0.317042,
                -0.294605, -0.268809, -0.240378, -0.210219, -0.179374
            };

            for (int i = 0; i < 11; i++)
            {
                double x = i / 10.0;
                double y = target.Inverse(x);

                double d1 = target.Derivative(x);
                double d2 = target.Derivative2(y);

                Assert.AreEqual(expected[i], d1, 1e-5);
                Assert.AreEqual(expected[i], d2, 1e-5);

                Assert.IsFalse(Double.IsNaN(d1));
                Assert.IsFalse(Double.IsNaN(d2));
            }
        }


        [TestMethod()]
        public void DerivativeTest2()
        {
            double beta = 2;
            double constant = 0.5;
            LogLogLinkFunction target = new LogLogLinkFunction(beta, constant);

            double[] expected =
            {
               -0.634084, -0.537619, -0.420439, -0.297894, -0.187093, -0.101414,
               -0.0459225, -0.0166933, -0.00464008, -0.000929341, -0.000124732
            };

            for (int i = 0; i < 11; i++)
            {
                double x = i / 10.0;
                double y = target.Inverse(x);

                double d1 = target.Derivative(x);
                double d2 = target.Derivative2(y);

                Assert.AreEqual(expected[i], d1, 1e-5);
                Assert.AreEqual(expected[i], d2, 1e-5);

                Assert.IsFalse(Double.IsNaN(d1));
                Assert.IsFalse(Double.IsNaN(d2));
            }
        }
    }
}
