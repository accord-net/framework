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
    using Accord.Statistics.Links;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class IdentityLinkFunctionTest
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
        public void IdentityLinkFunctionConstructorTest()
        {
            IdentityLinkFunction target = new IdentityLinkFunction();
            Assert.AreEqual(0, target.A);
            Assert.AreEqual(1, target.B);

            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(i, target.Function(i));
                Assert.AreEqual(i, target.Inverse(i));
                Assert.AreEqual(1, target.Derivative(i));
                Assert.AreEqual(1, target.Derivative(i));
            }
        }

        [TestMethod()]
        public void IdentityLinkFunctionConstructorTest1()
        {
            double mean = 3.14;
            double variance = 2.91;
            IdentityLinkFunction target = new IdentityLinkFunction(variance, mean);

            Assert.AreEqual(mean, target.A);
            Assert.AreEqual(variance, target.B);

            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual((i - mean) / variance, target.Function(i), 1e-10);
                Assert.AreEqual(i, target.Inverse((i - mean) / variance), 1e-10);
                Assert.AreEqual(variance, target.Derivative(i));
                Assert.AreEqual(variance, target.Derivative(i));
            }

        }


    }
}
