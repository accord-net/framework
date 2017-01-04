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
    using Accord.Math.Wavelets;
    using NUnit.Framework;
    using Accord.Math;

    [TestFixture]
    public class HaarTest
    {

        [Test]
        public void FWT2DTest()
        {
            double[,] original =
            {
                { 5, 6, 1, 2 },
                { 4, 2, 5, 5 },
                { 3, 1, 7, 1 },
                { 6, 3, 5, 1 }
            };

            double[,] data = (double[,])original.Clone();

            Haar.FWT(data, 1);

            double[,] expected = 
            {
                {  4.25,  3.25,  0.25, -0.25 },
                {  3.25,  3.5,   1.25,  2.5  },
                {  1.25, -1.75, -0.75, -0.25 },
                { -1.25,  0.5,  -0.25,  0.5  } 
            };

            // string dataStr = data.ToString(CSharpMatrixFormatProvider.InvariantCulture);

            Assert.IsTrue(Matrix.IsEqual(expected, data, 1e-5));

            Haar.IWT(data, 1);

            Assert.IsTrue(Matrix.IsEqual(data, original, 0.0001));
        }

        [Test]
        public void FWT2DTest2()
        {
            int levels = 2;

            double[,] original =
            {
                { 5, 6, 1, 2 },
                { 4, 2, 5, 5 },
                { 3, 1, 7, 1 },
                { 6, 3, 5, 1 }
            };

            double[,] data = (double[,])original.Clone();

            Haar.FWT(data, levels);

            double[,] expected = 
            {
                {  3.5625, 0.1875, 0.25, -0.25 },
                {  0.1875, 0.3125, 1.25,  2.5  },
                {  1.25,  -1.75,  -0.75, -0.25 },
                { -1.25,   0.5,   -0.25,  0.5  } 
            };

            string dataStr = data.ToString(CSharpMatrixFormatProvider.InvariantCulture);

            Assert.IsTrue(Matrix.IsEqual(expected, data, 1e-5));

            Haar.IWT(data, levels);

            Assert.IsTrue(Matrix.IsEqual(data, original, 0.0001));
        }

        [Test]
        public void IWTTest()
        {
            double[] original = { 1, 2, 3, 4 };
            double[] data = { 1, 2, 3, 4 };
            double[] expected = { 2.1213, 4.9497, -0.7071, -0.7071 };

            Haar.FWT(data);

            var d = data.Multiply(Constants.Sqrt2);

            Assert.IsTrue(Matrix.IsEqual(expected, d, 0.001));

            Haar.IWT(data);
            Assert.IsTrue(Matrix.IsEqual(original, data, 0.001));
        }

    }
}
