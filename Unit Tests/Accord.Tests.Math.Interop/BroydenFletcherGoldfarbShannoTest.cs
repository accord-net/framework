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
    using Accord.Math.Optimization;
    using AccordTestsMathCpp2;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class BroydenFletcherGoldfarbShannoTest
    {

        [Test]
        public void ConstructorTest2()
        {
            Function function = // min f(x) = 10 * (x+1)^2 + y^2
                x => 10.0 * Math.Pow(x[0] + 1.0, 2.0) + Math.Pow(x[1], 2.0);

            Gradient gradient = x => new[] { 20 * (x[0] + 1), 2 * x[1] };


            double[] start = new double[2];

            BroydenFletcherGoldfarbShanno target = new BroydenFletcherGoldfarbShanno(2,
                function.Invoke, gradient.Invoke);

            Assert.IsTrue(target.Minimize());
            double minimum = target.Value;

            double[] solution = target.Solution;

            Assert.AreEqual(0, minimum, 1e-10);
            Assert.AreEqual(-1, solution[0], 1e-5);
            Assert.AreEqual(0, solution[1], 1e-5);

            double expectedMinimum = function(target.Solution);
            Assert.AreEqual(expectedMinimum, minimum);
        }

    }
}
