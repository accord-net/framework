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
    using System.Collections.Generic;
    using System;

    [TestFixture]
    public partial class VectorTest
    {
        [Test]
        public void linspace_comparison_tests()
        {
            Assert.AreEqual(new double[] { -12, -11, -10, -9, -8, -7, -6, -5, -4, -3, -2, -1,
                                            0,
                                            1,    2,   3,  4,  5,  6,  7,  8,  9, 10, 11, 12 }, Vector.Interval(-12, 12));

            // https://docs.scipy.org/doc/numpy/reference/generated/numpy.linspace.html
            Assert.AreEqual(new double[] { 2.0, 2.25, 2.5, 2.75, 3.0 }, Vector.Interval(2.0, 3.0, steps: 5));
            Assert.AreEqual(new double[] { 2.0, 2.2, 2.4, 2.6, 2.8 }, Vector.Interval(2.0, 3.0, steps: 5, includeLast: false));

          
            Assert.AreEqual(new int[] { 1 }, Vector.Interval(1, 3, 1));
            Assert.AreEqual(new int[] { 3 }, Vector.Interval(3, 1, 1));
            Assert.AreEqual(new int[] { 1, 1, 2, 2, 3 }, Vector.Interval(1, 3, 5));
            Assert.AreEqual(new double[] { 1, 1.5, 2, 2.5, 3 }, Vector.Interval(1.0, 3.0, 5));
            Assert.AreEqual(new int[] { 3 }, Vector.Interval(3, 1, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() => Vector.Interval(1, 3, -1));
        }

    }
}
