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
    using NUnit.Framework;
    using Accord.Math;

    [TestFixture]
    public class LaplacianTest
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



        [Test]
        public void FunctionTest()
        {
            double sigma = 2.1;
            Laplacian target = new Laplacian(sigma);

            double[] x = { 1, 3 };
            double[] y = { -1, 2 };

            double expected = 0.34479878120059682;
            double actual = target.Function(x, y);
            Assert.AreEqual(expected, actual);

            // For the same input, product should be 1
            Assert.AreEqual(1.0, target.Function(x, x));
        }

        [Test]
        public void LaplacianReverseDistanceTest()
        {
            var laplacian = new Laplacian(4.2);

            var x = new double[] { 0.2, 0.5 };
            var y = new double[] { 0.3, -0.7 };

            double expected = Distance.Euclidean(x, y);

            double df = laplacian.Distance(x, y);
            double actual = laplacian.ReverseDistance(df);

            Assert.AreEqual(expected, actual, 1e-10);
        }
    }
}
