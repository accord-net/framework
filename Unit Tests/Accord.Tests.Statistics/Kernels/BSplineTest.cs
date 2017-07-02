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
    using System;
    using Accord.Math;
    using Accord.Statistics.Kernels;
    using NUnit.Framework;

    [TestFixture]
    public class BSplineTest
    {

        [Test]
        public void KernelTest()
        {
            double[] x, y;
            double actual;

            var target = new BSpline(10);
            x = new double[] { 1, 2, 3, 6, 2, 2, 1 };
            y = new double[] { 4, 5, 6, 9, 5, 3, 1 };
            actual = target.Function(x, y);
            Assert.AreEqual(0.00000000070098200355454978, actual, 1e-20);

            x = new double[] { 5, 11, 2, 9, 4, 5, 4 };
            y = new double[] { 4, 5, 1, 1, 3, 3, 4 };
            actual = target.Function(x, y);
            Assert.AreEqual(0.00000000000000017965181746434572, actual, 1e-10);

            target = new BSpline(20);
            x = new double[] { 1, 2, 3 };
            y = new double[] { 4, 5, 6 };
            actual = target.Function(x, y);
            Assert.AreEqual(0.00020921661189636344, actual, 1e-10);

            x = new double[] { 5, 11, 2 };
            y = new double[] { 4, 5, 1 };
            actual = target.Function(x, y);
            Assert.AreEqual(0.000042265155534640169, actual, 1e-10);

            target = new BSpline(20);
            x = new double[] { 0.1, 0.5, 0.01 };
            y = new double[] { 0.3, 0.02, 0.04 };
            actual = target.Function(x, y);
            Assert.AreEqual(0.0092334372187475831, actual, 1e-10);

            x = new double[] { 0.5, 0.11, 0.2 };
            y = new double[] { 0.4, 0.5, 0.1 };
            actual = target.Function(x, y);
            Assert.AreEqual(0.0093633595480067857, actual, 1e-10);
        }

        [Test]
        public void FunctionTest()
        {
            {
                double expected = cubic(0.5);
                double actual = Special.BSpline(3, 0.5);
                Assert.AreEqual(0.47916666666666669, actual, 1e-8);
                Assert.AreEqual(expected, actual, 1e-8);
            }

            for (double x = -10; x < 10; x = Math.Round(x + 0.01, 2))
            {
                double expected = cubic(x);
                double actual = Special.BSpline(3, x);
                Assert.IsTrue(expected.IsEqual(actual, rtol: 1e-5));
            }
        }

        static double cubic(double x)
        {
            // http://sepwww.stanford.edu/public/docs/sep105/sergey2/paper_html/node5.html

            double z = Math.Abs(x);
            if (z >= 0 && z < 1)
                return (4 - 6 * z * z + 3 * z * z * z) / 6;
            else if (z >= 1 && z < 2)
                return (2 - z) * (2 - z) * (2 - z) / 6;
            return 0;
        }
    }
}
