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

using Accord.Imaging;
using Accord.Math;
using NUnit.Framework;
using System.Drawing;

namespace Accord.Tests
{
    [TestFixture]
    public class HyperrectTest
    {
        [TestCase(1, 2, 3, 4)]
        public void ToRectangle_test(int x, int y, int w, int h)
        {
            Hyperrectangle target = new Hyperrectangle(x, y, w, h);
            Rectangle expected = new Rectangle(x, y, w, h);
            Rectangle actual = target.ToRectangle();

            Assert.AreEqual(expected, actual);
        }

        [TestCase(1.1, 2.2, 3.1, 4.2)]
        public void ToRectangleF_test(double x, double y, double w, double h)
        {
            Hyperrectangle target = new Hyperrectangle(x, y, w, h);
            RectangleF expected = new RectangleF((float)x, (float)y, (float)w, (float)h);
            RectangleF actual = target.ToRectangleF();

            Assert.AreEqual(expected, actual);
        }

        [TestCase(1, 2, 3, 4)]
        public void ToHyperrectangle_test(int x, int y, int w, int h)
        {
            Hyperrectangle expected = new Hyperrectangle(x, y, w, h);
            Rectangle target = new Rectangle(x, y, w, h);
            Hyperrectangle actual = target.ToHyperrectangle();

            Assert.AreEqual(expected.ToString("0.##"), actual.ToString("0.##"));
        }

        [TestCase(1.1, 2.2, 3.1, 4.2)]
        public void ToHyperrectangleF_test(double x, double y, double w, double h)
        {
            Hyperrectangle expected = new Hyperrectangle(x, y, w, h);
            RectangleF target = new RectangleF((float)x, (float)y, (float)w, (float)h);
            Hyperrectangle actual = target.ToHyperrectangle();

            Assert.AreEqual(expected.ToString("0.##"), actual.ToString("0.##"));
        }

        [TestCase(1.1, 2.2, 1.1, 2.2, 1.1, 2.2, 1.1, 2.2, true)]
        [TestCase(1.1, 2.2, 3.3, 2.2, 1.5, 2.5, 1.7, 2.5, true)]
        [TestCase(1.1, 2.2, 1.1, 4.4, 0.0, 0.0, 1.0, 1.0, false)]
        [TestCase(1.1, 2.2, 3.3, 4.4, 0.0, 0.1, 1.0, 4.4, false)]
        [TestCase(1.1, 2.2, 3.3, 4.4, 10.0, 20.1, 1.0, 4.4, false)]
        [TestCase(1.1, 2.2, 3.3, 4.4, 1.3, 0.1, 2.3, 4.4, true)]
        public void IntersectsWith_test(
            double x1, double y1, double w1, double h1,
            double x2, double y2, double w2, double h2, bool intersects)
        {
            Hyperrectangle r1 = new Hyperrectangle(x1, y1, w1, h1);
            Hyperrectangle r2 = new Hyperrectangle(x2, y2, w2, h2);

            RectangleF f1 = new RectangleF((float)x1, (float)y1, (float)w1, (float)h1);
            RectangleF f2 = new RectangleF((float)x2, (float)y2, (float)w2, (float)h2);

            bool expected = f1.IntersectsWith(f2);
            bool actual = r1.IntersectsWith(r2);

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected, intersects);
        }

        [TestCase(1, 2, 1, 2, 1, 2, 1, 2, true)]
        [TestCase(1, 2, 3, 2, 1, 2, 1, 2, true)]
        [TestCase(1, 2, 1, 4, 0, 0, 1, 1, false)]
        [TestCase(1, 2, 3, 4, 0, 0, 1, 4, false)]
        [TestCase(1, 2, 3, 4, 10, 20, 1, 4, false)]
        [TestCase(1, 2, 3, 4, 1, 0, 2, 4, true)]
        public void IntersectsWith_test(
            int x1, int y1, int w1, int h1,
            int x2, int y2, int w2, int h2, bool intersects)
        {
            Hyperrectangle r1 = new Hyperrectangle(x1, y1, w1, h1);
            Hyperrectangle r2 = new Hyperrectangle(x2, y2, w2, h2);

            RectangleF f1 = new RectangleF(x1, y1, w1, h1);
            RectangleF f2 = new RectangleF(x2, y2, w2, h2);

            bool expected = f1.IntersectsWith(f2);
            bool actual = r1.IntersectsWith(r2);

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected, intersects);
        }

        [TestCase(1.1, 2.2, 1.1, 2.2, 1.1, 2.2, true)]
        [TestCase(1.1, 2.2, 3.3, 2.2, 1.5, 2.5, true)]
        [TestCase(1.1, 2.2, 1.1, 4.4, 0.0, 0.0, false)]
        [TestCase(1.1, 2.2, 3.3, 4.4, 0.0, 0.1, false)]
        [TestCase(1.1, 2.2, 3.3, 4.4, 10.0, 20.1, false)]
        [TestCase(1.1, 2.2, 3.3, 4.4, 1.3, 0.1, false)]
        public void Contains_test(
            double rx, double ry, double rw, double rh,
            double x, double y, bool contains)
        {
            Hyperrectangle hr = new Hyperrectangle(rx, ry, rw, rh);
            RectangleF rf = new RectangleF((float)rx, (float)ry, (float)rw, (float)rh);

            bool expected = rf.Contains((float)x, (float)y);
            bool actual = hr.Contains(x, y);

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected, contains);
        }

        [TestCase(1, 2, 1, 2, 1, 2, true)]
        [TestCase(1, 2, 3, 2, 4, 4, false)]
        [TestCase(1, 2, 1, 4, 1, 4, true)]
        [TestCase(1, 2, 3, 4, 1, 1, false)]
        [TestCase(1, 2, 3, 4, 1, 3, true)]
        [TestCase(1, 2, 3, 4, 0, 2, false)]
        public void Contains_test(
            int rx, int ry, int rw, int rh,
            int x, int y, bool contains)
        {
            Hyperrectangle hr = new Hyperrectangle(rx, ry, rw, rh);
            Rectangle rf = new Rectangle(rx, ry, rw, rh);

            bool expected = rf.Contains(x, y);
            bool actual = hr.Contains(x, y);

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected, contains);
        }
    }
}
