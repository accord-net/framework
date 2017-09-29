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
    using System;
    using Accord.Math.Optimization;
    using NUnit.Framework;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Accord.Math;
    using System.Threading;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.ComponentModel;
    using Accord.Math.Random;

    [TestFixture]
    public class ZigguratNormalGeneratorTest
    {

        [Test]
        public void TestZigguratNormalGenerator_InitializesCorrectly()
        {
            var rng = new ZigguratNormalGenerator();
            double num = rng.Generate();

            Assert.NotNull(rng);
        }

        [Test]
        public void TestZigguratNormalGenerator_WithSeed_InitializesCorrectly()
        {
            var rng1 = new ZigguratNormalGenerator(seed: 5);
            var rng2 = new ZigguratNormalGenerator(seed: 5);

            double num1 = rng1.Generate();
            double num2 = rng2.Generate();

            Assert.AreEqual(num1, num2);
        }
    }
}
