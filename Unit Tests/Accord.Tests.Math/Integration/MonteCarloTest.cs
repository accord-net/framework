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
    using Accord.Math.Integration;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class MonteCarloIntegralTest
    {

        [Test]
        public void MonteCarloTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            // A common Monte-Carlo integration example is to compute
            // the value of Pi. This is the same example given in the
            // Wikipedia's page for Monte-Carlo Integration at
            // https://en.wikipedia.org/wiki/Monte_Carlo_integration#Example

            Func<double, double, double> H = 
                (x, y) => (x * x + y * y <= 1) ? 1 : 0;

            double[] from = { -1, -1 };
            double[] to = { +1, +1 };

            int samples = 1000000;

            double area = MonteCarloIntegration.Integrate(x => H(x[0], x[1]), from, to, samples);

            Assert.AreEqual(Math.PI, area, 5e-3);
        }


    }
}
