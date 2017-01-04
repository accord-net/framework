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

namespace Accord.Tests.Interop.Math
{
    using Accord.Math;
    using Accord.Math.Integration;
    using AccordTestsMathCpp2;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class IntegralTest
    {
      
        [Test]
        public void InfiniteGaussKronrodTest()
        {
            for (int i = -10; i < 10; i++)
            {
                Func<double, double> pdf = (x) => Normal.Derivative(x - i);

                Func<double, double> E = (x) => x * pdf(x);
                UFunction UE = (x) => x * pdf(x);

                double expected = Quadpack.Integrate(UE,
                    Double.NegativeInfinity, Double.PositiveInfinity);

                double actual = InfiniteAdaptiveGaussKronrod.Integrate(E,
                    Double.NegativeInfinity, Double.PositiveInfinity);

                Assert.AreEqual(expected, actual, 1e-3);
                Assert.AreEqual(i, actual, 1e-3);
            }
        }

    }
}
