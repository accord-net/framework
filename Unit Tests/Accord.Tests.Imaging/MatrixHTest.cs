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

namespace Accord.Tests.Imaging
{
    using Accord.Imaging;
    using NUnit.Framework;


    [TestFixture]
    public class MatrixHTest
    {

        [Test]
        public void MultiplyTest()
        {
            MatrixH A = new MatrixH();
            MatrixH B = new MatrixH();

            MatrixH expected = new MatrixH();
            MatrixH actual = A.Multiply(B);

            Assert.IsTrue(Accord.Math.Matrix.IsEqual(
                expected.Elements, actual.Elements));

            try
            {
                A = new MatrixH(new float[8] {1, 0, 0, 0, 1, 0, 0, 1});
                B = new MatrixH(new float[9] {1, 0, 0, 0, 1, 0, 0, 0, 1});
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            double[,] a = 
            {
                { 2, 3, 1 },
                { 2, 1, 7 },
                { 1, 2, 2 }
            };

            double[,] b = 
            {
                {  1,  3,  1 },
                { -2,  1, -4 },
                {  1, -1,  3 }
            };

            A = new MatrixH(a);
            B = new MatrixH(b);

            actual = A.Multiply(B);
            expected = new MatrixH(Accord.Math.Matrix.Multiply(a, b));

            Assert.IsTrue(Accord.Math.Matrix.IsEqual(
                (double[,])actual, (double[,])expected,
                0.001));
        }

    }
}
