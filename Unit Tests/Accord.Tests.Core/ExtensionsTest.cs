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

using Accord;
using NUnit.Framework;

namespace Accord.Tests
{
    [TestFixture]
    public class ExtensionsTest
    {
        [Test]
        public void CompareTest()
        {
            Assert.IsTrue(10.IsLessThan(11));
            Assert.IsTrue((-1).IsLessThanOrEqual(-1));

            Assert.IsTrue(10.IsGreaterThan(9));
            Assert.IsTrue((-1).IsGreaterThanOrEqual(-2));
        }

        [Test]
        public void MakeArrayType_test()
        {
            var t = typeof(double);
            var r = t.MakeArrayType(rank: 3, jagged: true);
            Assert.AreEqual(typeof(double[][][]), r);

            r = t.MakeArrayType(rank: 1, jagged: true);
            Assert.AreEqual(typeof(double[]), r);

            r = t.MakeArrayType(rank: 0, jagged: true);
            Assert.AreEqual(typeof(double), r);
        }
    }
}
