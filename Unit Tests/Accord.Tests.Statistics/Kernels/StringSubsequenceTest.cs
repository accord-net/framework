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

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Kernels;
    using Accord.Math;
    using NUnit.Framework;
    using System.Diagnostics;

    [TestFixture]
    public class StringSubsequenceTest
    {


        [Test]
        public void BasicTest()
        {
            double lambda = 0.5;
            string[] words = { "cat", "car", "bat", "bar" };

            var ssk = new StringSubsequence(k: 2, lambda: 0.5, normalizationPower: 0);

            double actual, expected;
            actual = ssk.Function("car", "cat");
            expected = System.Math.Pow(lambda, 4);
            Assert.AreEqual(lambda * lambda * lambda * lambda, actual);

            actual = ssk.Function("car", "car");
            expected = 2 * System.Math.Pow(lambda, 4);
            Assert.AreEqual(lambda * lambda * lambda * lambda, actual);

            actual = ssk.Function("cat", "cat");
            expected = System.Math.Pow(lambda, 6);
            Assert.AreEqual(lambda * lambda * lambda * lambda, actual);
        }

    }
}
