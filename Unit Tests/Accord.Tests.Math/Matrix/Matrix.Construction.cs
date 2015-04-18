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

    public partial class MatrixTest
    {


        [TestMethod()]
        public void StackTest()
        {
            var x1 = Vector.Ones(1000);
            var y1 = Vector.Zeros(1000);

            double[,] w1 = Matrix.Stack(x1, y1).Transpose();

            Assert.AreEqual(1000, w1.Rows());
            Assert.AreEqual(2, w1.Columns());
            Assert.AreEqual(w1.Length, x1.Length + y1.Length);

            for (int i = 0; i < x1.Length; i++)
            {
                Assert.AreEqual(1, x1[i]);
                Assert.AreEqual(1, w1[i, 0]);
            }

            for (int i = 0; i < y1.Length; i++)
            {
                Assert.AreEqual(0, y1[i]);
                Assert.AreEqual(0, w1[i, 1]);
            }

            var x = w1.GetColumn(0);
            var y = w1.GetColumn(1);

            Assert.IsTrue(x.IsEqual(x1));
            Assert.IsTrue(y.IsEqual(y1));
        }
    }
}
