﻿// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    using Accord.Statistics.Kernels.Sparse;
    using Accord.Statistics.Kernels;
    using NUnit.Framework;

    [TestFixture]
    public class SparseSigmoidTest
    {

        [Test]
        public void FunctionTest2()
        {
            SparseLinearTest.SparseTest(new Sigmoid(), new SparseSigmoid());
            SparseLinearTest.SparseTest(new Sigmoid(0, 0), new SparseSigmoid(0, 0));
            SparseLinearTest.SparseTest(new Sigmoid(0, 3.6), new SparseSigmoid(0, 3.6));
            SparseLinearTest.SparseTest(new Sigmoid(3.6, 0), new SparseSigmoid(3.6, 0));
        }

        [Test]
        public void FunctionTest()
        {
            Sigmoid dense = new Sigmoid(3.6, 1);
            SparseSigmoid target = new SparseSigmoid(3.6, 1);

            double[] sx = { 1, -0.555556, 2, +0.250000, 3, -0.864407, 4, -0.916667 };
            double[] sy = { 1, -0.666667, 2, -0.166667, 3, -0.864407, 4, -0.916667 };
            double[] sz = { 1, -0.944444, 3, -0.898305, 4, -0.916667 };

            double[] dx = { -0.555556, +0.250000, -0.864407, -0.916667 };
            double[] dy = { -0.666667, -0.166667, -0.864407, -0.916667 };
            double[] dz = { -0.944444, +0.000000, -0.898305, -0.916667 };

            double expected, actual;

            expected = dense.Function(dx, dy);
            actual = target.Function(sx, sy);
            Assert.AreEqual(expected, actual);

            expected = dense.Function(dx, dz);
            actual = target.Function(sx, sz);
            Assert.AreEqual(expected, actual);

            expected = dense.Function(dy, dz);
            actual = target.Function(sy, sz);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DistanceTest()
        {
            Sigmoid dense = new Sigmoid(3.6, 1);
            SparseSigmoid target = new SparseSigmoid(3.6, 1);

            double[] sx = { 1, -0.555556, 2, +0.250000, 3, -0.864407, 4, -0.916667 };
            double[] sy = { 1, -0.666667, 2, -0.166667, 3, -0.864407, 4, -0.916667 };
            double[] sz = { 1, -0.944444, 3, -0.898305, 4, -0.916667 };

            double[] dx = { -0.555556, +0.250000, -0.864407, -0.916667 };
            double[] dy = { -0.666667, -0.166667, -0.864407, -0.916667 };
            double[] dz = { -0.944444, +0.000000, -0.898305, -0.916667 };

            double expected, actual;

            expected = dense.Distance(dx, dy);
            actual = target.Distance(sx, sy);
            Assert.AreEqual(expected, actual);

            expected = dense.Distance(dx, dz);
            actual = target.Distance(sx, sz);
            Assert.AreEqual(expected, actual);

            expected = dense.Distance(dy, dz);
            actual = target.Distance(sy, sz);
            Assert.AreEqual(expected, actual);
        }
    }
}
