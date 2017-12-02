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
    using Accord.Math;
    using NUnit.Framework;
    using System.Linq;

    [TestFixture]
    public partial class VectorTest
    {

        [Test]
        public void ExpandTest()
        {
            int[] v = Vector.Zeros<int>(5);
            double[] u = Vector.Create(5, 1.0);
            int[] w = Vector.Create(1, 2, 3);

            Assert.IsTrue(v.IsEqual(new[] { 0, 0, 0, 0, 0 }));
            Assert.IsTrue(u.IsEqual(new[] { 1, 1, 1, 1, 1 }));
            Assert.IsTrue(w.IsEqual(new[] { 1, 2, 3 }));
        }

        [Test]
        public void GetIndicesTest()
        {
            double[] v = Vector.Ones(5);
            int[] idx = v.GetIndices();
            Assert.IsTrue(idx.IsEqual(new[] { 0, 1, 2, 3, 4 }));
        }

        [Test]
        public void sample_test()
        {
            // https://github.com/accord-net/framework/issues/862
            var r = Vector.Range(10000);
            int[] s = Vector.Sample(r, 10);
            Assert.IsTrue(s.Any(x => x > 10));
        }

    }
}
