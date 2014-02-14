// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
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
    using Microsoft.VisualStudio.TestTools.UnitTesting;


    [TestClass()]
    public class MatrixHTest
    {


        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }



        [TestMethod()]
        public void MultiplyTest()
        {
            MatrixH A = new MatrixH();
            MatrixH B = new MatrixH();

            MatrixH expected = new MatrixH();
            MatrixH actual = A.Multiply(B);

            Assert.IsTrue(Accord.Math.Matrix.IsEqual(
                expected.Elements, actual.Elements));


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
