// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2015
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
    using Accord.Math;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public partial class VectorTest
    {

        [TestMethod()]
        public void ExpandTest()
        {
            var v = Vector.Create<int>(5);

            var u = Vector.Create(5, 1.0);

            var w = Vector.Create(5, new[] { 1, 2, 3 });

            var r = Vector.Create(2, new[] { 1, 2, 3 });

            Assert.IsTrue(v.IsEqual(0, 0, 0, 0, 0));
            Assert.IsTrue(u.IsEqual(1, 1, 1, 1, 1));
            Assert.IsTrue(w.IsEqual(1, 2, 3, 0, 0));
            Assert.IsTrue(r.IsEqual(1, 2));
        }

    }
}
