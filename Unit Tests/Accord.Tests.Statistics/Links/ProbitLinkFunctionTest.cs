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
    public class ProbitLinkFunctionTest
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
        public void ProbitLinkFunctionConstructorTest()
        {
            ProbitLinkFunction target = new ProbitLinkFunction();

            double[] expected = 
            {
                Double.NegativeInfinity, -1.28155, -0.841621, -0.524401, -0.253347,
                0, 0.253347, 0.524401, 0.841621, 1.28155, Double.PositiveInfinity
            };

            for (int i = 0; i < 11; i++)
            {
                double x = i / 10.0;
                double y = expected[i];

                double fx = target.Function(x);
                double iy = target.Inverse(y);

                Assert.AreEqual(y, fx, 1e-5);
                Assert.AreEqual(x, iy, 1e-5);

                Assert.IsFalse(Double.IsNaN(fx));
                Assert.IsFalse(Double.IsNaN(iy));
            }
        }

      
        [TestMethod()]
        public void DerivativeTest()
        {
            ProbitLinkFunction target = new ProbitLinkFunction();

            double[] expected = 
            {
                0.398942, 0.396953, 0.391043, 0.381388, 0.36827, 0.352065,
                0.333225, 0.312254, 0.289692, 0.266085, 0.241971
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
