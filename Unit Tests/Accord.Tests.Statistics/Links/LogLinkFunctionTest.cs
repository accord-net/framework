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
    public class LogLinkFunctionTest
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
        public void LogLinkFunctionConstructorTest()
        {
            double beta = 0.52;
            double constant = 2.42;
            LogLinkFunction target = new LogLinkFunction(beta, constant);

            Assert.AreEqual(beta, target.B);
            Assert.AreEqual(constant, target.A);

            for (int i = 0; i < 11; i++)
            {
                double x = i / 10.0;
                double y = (Math.Log(x) - constant) / beta;

                Assert.AreEqual(y, target.Function(x), 1e-10);
                Assert.AreEqual(x, target.Inverse(y), 1e-10);
            }
        }

        [TestMethod()]
        public void LogLinkFunctionConstructorTest2()
        {
            LogLinkFunction target = new LogLinkFunction();

            Assert.AreEqual(1, target.B);
            Assert.AreEqual(0, target.A);

            for (int i = 0; i < 11; i++)
            {
                double x = i / 10.0;
                double y = Math.Log(x);

                Assert.AreEqual(y, target.Function(x), 1e-10);
                Assert.AreEqual(x, target.Inverse(y), 1e-10);
            }
        }

        [TestMethod()]
        public void DerivativeTest()
        {
            double beta = 0.52;
            double constant = 2.42;

            double[] expected =
            {
               5.84785, 6.15998, 6.48877, 6.83512, 7.19995, 7.58425, 
               7.98906, 8.41549, 8.86467, 9.33783, 9.83624
            };

            LogLinkFunction target = new LogLinkFunction(beta, constant);

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
